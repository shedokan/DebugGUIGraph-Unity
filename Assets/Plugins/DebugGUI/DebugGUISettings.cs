using UnityEngine;

public enum ScreenCorner { TopLeft, TopRight, BottomLeft, BottomRight }

[CreateAssetMenu(fileName = "DebugGUISettings", menuName = "DebugGUI Settings")]
public class DebugGUISettings : ScriptableObject
{
    [SerializeField] public bool enableGraphs = true;
    [SerializeField] public bool enableLogs = true;

    [SerializeField] public Color backgroundColor = new(0f, 0f, 0f, 0.7f);
    [SerializeField] public Color scrubberColor = new(1f, 1f, 0f, 0.7f);
    [SerializeField] public int graphWidth = 300;
    [SerializeField] public int graphHeight = 100;
    [SerializeField] public float temporaryLogLifetime = 5;

    [SerializeField] public float scale = 2.5f;
    [SerializeField] public float lineThickness = 1f;

    // Replicates Unity's Canvas Scaler "Scale With Screen Size" formula.
    // graph sizes are authored at referenceResolution; scale is applied on top.
    [SerializeField] public bool scaleWithScreenSize = true;
    [SerializeField] public Vector2 referenceResolution = new(1080, 2340);
    // 0 = match width, 1 = match height, 0.5 = blend (same as Canvas Scaler slider)
    [SerializeField] [Range(0f, 1f)] public float matchWidthOrHeight = 0.5f;

    [SerializeField] public ScreenCorner graphInitialCorner = ScreenCorner.BottomLeft;
    [SerializeField] public Vector2 graphInitialOffset = new(20, 20);
    [SerializeField] public ScreenCorner logInitialCorner = ScreenCorner.TopLeft;
    [SerializeField] public Vector2 logInitialOffset = Vector2.zero;

    public float EffectiveScale
    {
        get
        {
            if (scaleWithScreenSize && referenceResolution.x > 0 && referenceResolution.y > 0)
            {
                // Same log-space blend Unity's CanvasScaler uses internally
                float logW = Mathf.Log(Screen.width  / referenceResolution.x, 2f);
                float logH = Mathf.Log(Screen.height / referenceResolution.y, 2f);
                return scale * Mathf.Pow(2f, Mathf.Lerp(logW, logH, matchWidthOrHeight));
            }
            return scale;
        }
    }

    public int ScaledGraphWidth => Mathf.RoundToInt(graphWidth * EffectiveScale);
    public int ScaledGraphHeight => Mathf.RoundToInt(graphHeight * EffectiveScale);
    public float ScaledLineThickness => lineThickness * EffectiveScale;
}
