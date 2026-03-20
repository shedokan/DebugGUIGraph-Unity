using UnityEngine;
using UnityEngine.Rendering;

namespace WeavUtils
{
    // Draggable window clamped to the corners
    public class DebugGUIWindow : MonoBehaviour
    {
        private static DebugGUISettings Settings => DebugGUI.Settings;
        
        protected const int outOfScreenClampPaddingBase = 30;
        protected readonly Vector2 PaddingBase = new(5, 5);

        protected int outOfScreenClampPadding => Mathf.RoundToInt(outOfScreenClampPaddingBase * Settings.EffectiveScale);
        protected Vector2 Padding => PaddingBase * Settings.EffectiveScale;


        static bool dragInProgress;
        bool dragged;

        protected bool IsInitialized;
        protected Rect rect;
        protected float cachedLineHeight;

        Vector2 lastMousePos;

        // Only needed for GL line drawing (graph curves, scrubber)
        static Material drawMat;

        // 1x1 white texture used for IMGUI rect drawing — works on all platforms
        static Texture2D colorTexture;
        static Texture2D ColorTexture
        {
            get
            {
                if (colorTexture != null) return colorTexture;
                colorTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                colorTexture.SetPixel(0, 0, Color.white);
                colorTexture.Apply();
                return colorTexture;
            }
        }

        Material CreateMaterial()
        {
            // TODO: Use a better shader, maybe unlit? or UI?

            // Hidden/Internal-Colored must be in Project Settings > Graphics > Always Included Shaders
            // for mobile builds. Alternatively, place it in a Resources folder.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            Material mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            mat.SetInt("_ZWrite", 0);
            return mat;
        }

        public virtual Rect GetDraggableRect()
        {
            return new Rect(rect.position, rect.size + Padding * 2);
        }

        public virtual void Init()
        {
            if (drawMat == null)
                drawMat = CreateMaterial();
            IsInitialized = true;
        }

        protected virtual void OnEnable()
        {
            // SRP (URP / HDRP) — endCameraRendering fires after each camera finishes
            RenderPipelineManager.endCameraRendering += OnSRPEndCameraRendering;
        }

        protected virtual void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= OnSRPEndCameraRendering;
        }

        void OnSRPEndCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            if (cam != Camera.main) return;
            RenderGLOverlay();
        }

        // Called by the built-in render pipeline after the camera renders the scene.
        // Skipped when an SRP is active (handled by endCameraRendering above).
        void OnRenderObject()
        {
            if (GraphicsSettings.currentRenderPipeline != null) return;
            if (Camera.current != Camera.main) return;
            RenderGLOverlay();
        }

        void RenderGLOverlay()
        {
            if (drawMat == null) return;
            drawMat.SetPass(0);
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0); // maps GL coords to screen pixels, top-left origin
            DrawGL();
            GL.PopMatrix();
        }

        // Override in subclasses to perform GL drawing (lines, graph curves).
        // The material pass and pixel matrix are already set up when this is called.
        // Do NOT call IMGUI (GUI.*) methods here.
        protected virtual void DrawGL() { }

        void Update()
        {
            // Flip mouse Y
            var mousePos = DebugGUIInput.MousePosition;
            mousePos.y = Screen.height - mousePos.y;

            if (DebugGUIInput.MiddleMouseButtonDown)
            {
                if (!dragInProgress)
                {
                    var rect = GetDraggableRect();
                    if (rect.Contains(mousePos))
                    {
                        dragged = true;
                        dragInProgress = true;
                        lastMousePos = mousePos; // seed so first delta is zero in touch
                    }
                }
            }
            else if (DebugGUIInput.MiddleMouseButtonUp)
            {
                if (dragged) dragInProgress = false;
                dragged = false;
            }

            if (dragged)
            {
                var mouseDelta = mousePos - lastMousePos;
                Move(mouseDelta);
            }
            lastMousePos = mousePos;
        }

        protected void Move(Vector2 delta = default)
        {
            rect.position += delta;

            var viewportRect = new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height));

            var min = -GetDraggableRect().size + Vector2.one * outOfScreenClampPadding;
            var max = viewportRect.size - Vector2.one * outOfScreenClampPadding;

            // Limit graph window offset so we can't get lost off screen
            rect.position = new Vector2(
                Mathf.Clamp(rect.position.x, min.x, max.x),
                Mathf.Clamp(rect.position.y, min.y, max.y)
            );
        }

        protected virtual void OnGUI()
        {
            // Only draw once per frame
            if (Event.current.type != EventType.Repaint)
                return;
        }

        GUIContent tmpGuiContent = new();
        protected Vector2 GetMultilineStringSize(GUIStyle style, in string str)
        {
            tmpGuiContent.text = str;
            style.CalcMinMaxWidth(tmpGuiContent, out _, out float width);
            var height = style.CalcHeight(tmpGuiContent, width);
            return new Vector2(width, height);
        }

        // Draws a solid color rectangle using IMGUI — works on all platforms including Android and iOS.
        // Call this from OnGUI only.
        protected void DrawRect(Rect rect, Color color, Vector2 padding = default)
        {
            rect.position += this.rect.position;
            rect.size += padding * 2;
            var prevColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, ColorTexture);
            GUI.color = prevColor;
        }

        // Draws a thick line using GL quads. Call this from DrawGL() only.
        protected void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            start += rect.position;
            end += rect.position;

            float halfThickness = DebugGUI.Settings.ScaledLineThickness * 0.5f;
            Vector2 dir = (end - start).normalized;
            // Perpendicular offset for thickness
            Vector2 perp = new Vector2(-dir.y, dir.x) * halfThickness;

            GL.Begin(GL.QUADS);
            {
                GL.Color(color);
                GL.Vertex3(start.x - perp.x, start.y - perp.y, 0f);
                GL.Vertex3(start.x + perp.x, start.y + perp.y, 0f);
                GL.Vertex3(end.x + perp.x, end.y + perp.y, 0f);
                GL.Vertex3(end.x - perp.x, end.y - perp.y, 0f);
            }
            GL.End();
        }

        protected void DrawLabel(Vector2 pos, string label, Vector2 padding = default, GUIStyle style = null)
        {
            DrawLabel(new Rect(pos, GetMultilineStringSize(GUIStyle.none, in label)), label, padding, style);
        }

        protected void DrawLabel(Rect rect, string label, Vector2 padding = default, GUIStyle style = null)
        {
            rect.position += this.rect.position;
            tmpGuiContent.text = label;
            GUI.Label(new Rect(rect.position + padding, rect.size + padding), tmpGuiContent, style ?? GUIStyle.none);
        }
        

        protected static Vector2 CalculateInitialPosition(ScreenCorner corner, Vector2 offset, Vector2 windowSize)
        {
            return corner switch
            {
                ScreenCorner.TopLeft     => offset,
                ScreenCorner.TopRight    => new Vector2(Screen.width - windowSize.x - offset.x, offset.y),
                ScreenCorner.BottomLeft  => new Vector2(offset.x, Screen.height - windowSize.y - offset.y),
                ScreenCorner.BottomRight => new Vector2(Screen.width - windowSize.x - offset.x, Screen.height - windowSize.y - offset.y),
                _ => offset
            };
        }
    }
}
