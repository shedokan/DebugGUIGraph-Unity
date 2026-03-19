using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using static DebugGUI;

namespace WeavUtils
{
    public class GraphWindow : DebugGUIWindow
    {
        const int graphLabelFontSize = 12;
        const int graphLabelPadding = 5;
        const int graphBlockPadding = 3;
        const int scrubberBackgroundWidth = 55;

        List<GraphContainer> graphs = new();
        HashSet<MonoBehaviour> attributeContainers = new();
        Dictionary<Type, int> typeInstanceCounts = new();
        Dictionary<object, GraphContainer> graphDictionary = new();
        Dictionary<MonoBehaviour, List<GraphAttributeKey>> attributeKeys = new();
        Dictionary<Type, HashSet<FieldInfo>> debugGUIGraphFields = new();
        Dictionary<Type, HashSet<PropertyInfo>> debugGUIGraphProperties = new();
        Dictionary<Type, HashSet<MethodInfo>> debugGUIGraphMethods = new();
        SortedDictionary<int, List<GraphContainer>> graphGroups = new();

        bool freezeGraphs;
        float graphLabelBoxWidth;

        GUIStyle graphLabelStyle;

        protected void InitializeGUIStyles()
        {
            graphLabelStyle = new GUIStyle();
            graphLabelStyle.fontSize = graphLabelFontSize;
        }

        public override void Init()
        {
            base.Init();

            InitializeGUIStyles();
            RegisterAttributes();

            // Default to top right
            rect.position = new Vector2(Screen.width - GetDraggableRect().width, 0);
        }

        void LateUpdate()
        {
            if (!DebugGUIInput.LeftMouseButtonPressed)
            {
                freezeGraphs = false;
            }

            if (!freezeGraphs)
            {
                CleanUpDeletedAttributes();
                PollGraphAttributes();
            }
        }

        protected override void OnGUI()
        {
            if(!IsInitialized) return;
            
            base.OnGUI();

            if (Event.current.type != EventType.Repaint)
                return;

            if (cachedLineHeight == 0)
                cachedLineHeight = GetMultilineStringSize(graphLabelStyle, string.Empty).y;

            int groupNum = 0;
            foreach (var group in graphGroups.Values)
            {
                DrawGraphGroup(group, groupNum);
                groupNum++;
            }

            foreach (var label in deferredLabels)
            {
                graphLabelStyle.normal.textColor = label.color;
                DrawLabel(label.position, label.label, style: graphLabelStyle);
            }
            deferredLabels.Clear();
        }

        public void Graph(object key, float val)
        {
            if (!graphDictionary.ContainsKey(key))
            {
                CreateGraph(key);
            }

            if (freezeGraphs) return;

            graphDictionary[key].Push(val);
            // Todo: optimize away?
            RecalculateGraphLabelWidth();
        }

        public void CreateGraph(object key)
        {
            AddGraph(key, new GraphContainer(Settings.graphWidth));
            RecalculateGraphLabelWidth();
        }

        public void ClearGraph(object key)
        {
            if (graphDictionary.ContainsKey(key))
                graphDictionary[key].Clear();
        }

        public void RemoveGraph(object key)
        {
            if (graphDictionary.ContainsKey(key))
            {
                var graph = graphDictionary[key];
                graphs.Remove(graph);
                graphDictionary.Remove(key);
                graphGroups[graph.group].Remove(graph);
                if (graphGroups[graph.group].Count == 0)
                {
                    graphGroups.Remove(graph.group);
                }
                RecalculateGraphLabelWidth();
            }
        }

        public void SetGraphProperties(object key, string label, float min, float max, int group, Color color, bool autoScale)
        {
            if (graphDictionary.ContainsKey(key))
            {
                RemoveGraph(key);
            }

            var graph = new GraphContainer(Settings.graphWidth, group);
            AddGraph(key, graph);

            graph.name = label;
            graph.SetMinMax(min, max);
            graph.color = color;
            graph.autoScale = autoScale;
        }

        public void ReinitializeAttributes()
        {
            // Clean up graphs
            List<object> toRemove = new List<object>();
            foreach (var key in graphDictionary.Keys)
            {
                if (key is GraphAttributeKey)
                    toRemove.Add(key);
            }
            foreach (var key in toRemove)
            {
                RemoveGraph(key);
            }

            attributeContainers = new();
            debugGUIGraphFields = new();
            debugGUIGraphProperties = new();
            typeInstanceCounts = new();
            attributeKeys = new();

            RegisterAttributes();
        }

        [Serializable]
        class DataExport
        {
            public GraphContainer.DataExport[] data;
        }
        public string ToJson()
        {
            var dataExport = new GraphContainer.DataExport[graphs.Count];

            for (int i = 0; i < graphs.Count; i++)
            {
                dataExport[i] = graphs[i].AsDataExport();
            }

            var json = JsonUtility.ToJson(new DataExport() { data = dataExport });
            return json;
        }

        public override Rect GetDraggableRect()
        {
            RefreshRect();
            return base.GetDraggableRect();
        }

        private void AddGraph(object key, GraphContainer graph)
        {
            graph.OnLabelSizeChange += RefreshRect;

            graphDictionary.Add(key, graph);
            graphs.Add(graph);

            if (!graphGroups.ContainsKey(graph.group))
            {
                graphGroups.Add(graph.group, new List<GraphContainer>());
            }

            graphGroups[graph.group].Add(graph);

            RecalculateGraphLabelWidth();
        }

        private void PollGraphAttributes()
        {
            foreach (var node in attributeContainers)
            {
                if (!node || !attributeKeys.TryGetValue(node, out var attributeKey))
                    continue;

                foreach (var key in attributeKey)
                {
                    var val = TryGetMemberValue(key.memberInfo, node);
                    if(val == null) continue;

                    if (val is float fVal)
                        graphDictionary[key].Push(fVal);
                    else if (val is int i)
                        graphDictionary[key].Push(i);
                    else if (val is Vector2 vec2){
                        graphDictionary[key].Push(vec2.x);
                        // TODO: Support x and y together
                        // graphDictionary[key + "_y"].Push(vec2.y);
                    }
                    else if (val is Vector3 vec3){
                        graphDictionary[key].Push(vec3.x);
                        // TODO: Support x and y and z together
                        // graphDictionary[key + "_y"].Push(vec3.y);
                        // graphDictionary[key + "_z"].Push(vec3.z);
                    }
                    else
                        Debug.LogWarning($"Unsupported DebugGUIGraph attribute type: {val.GetType()}");
                }
            }
        }

        GraphContainer lastPressedGraphLabel;
        private void DrawGraphGroup(List<GraphContainer> group, int groupNum)
        {
            Vector2 relativeMousePos = DebugGUIInput.MousePosition;
            relativeMousePos.y = Screen.height - relativeMousePos.y;
            relativeMousePos -= rect.position;

            Vector2 graphBlockSize = new Vector2(Settings.graphWidth + graphBlockPadding, Settings.graphHeight + graphBlockPadding);

            var groupOrigin = new Vector2(0, graphBlockSize.y * groupNum);
            var groupGraphRect = new Rect(
                groupOrigin.x + graphLabelBoxWidth + graphBlockPadding,
                groupOrigin.y,
                Settings.graphWidth,
                Settings.graphHeight
            );

            // Label background
            DrawRect(new Rect(
                groupOrigin.x,
                groupOrigin.y,
                graphLabelBoxWidth,
                Settings.graphHeight),
            Settings.backgroundColor);

            // Graph background
            DrawRect(new Rect(
                groupOrigin.x + graphBlockPadding + graphLabelBoxWidth,
                groupOrigin.y,
                graphBlockSize.x,
                Settings.graphHeight),
            Settings.backgroundColor);

            // Magic padding offsets
            Vector2 textOrigin = groupOrigin + new Vector2(0, 14);
            Vector2 minMaxOrigin = groupOrigin + new Vector2(graphLabelBoxWidth - 10, 0);
            foreach (var graph in group)
            {
                var textSize = GetMultilineStringSize(graphLabelStyle, in graph.name);
                textOrigin.y += textSize.y;
                var maxWidthOfMinMaxStrings = Mathf.Max(
                    GetMultilineStringSize(graphLabelStyle, graph.minString).x,
                    GetMultilineStringSize(graphLabelStyle, graph.maxString).x
                );
                minMaxOrigin += Vector2.left * (maxWidthOfMinMaxStrings + graphLabelPadding);

                // Label button logic
                var labelRect = new Rect(textOrigin - textSize + new Vector2(graphLabelBoxWidth - (graphLabelPadding * 2), graphLabelPadding), textSize);
                // Enable disable
                var isHovered = labelRect.Contains(relativeMousePos);
                var isPressed = isHovered && DebugGUIInput.LeftMouseButtonPressed;

                // Button click
                if (lastPressedGraphLabel == graph && !isPressed && isHovered)
                {
                    graph.visible = !graph.visible;
                }

                if (isPressed)
                {
                    lastPressedGraphLabel = graph;
                }
                else if (lastPressedGraphLabel == graph)
                {
                    lastPressedGraphLabel = null;
                }

                var graphColor = graph.GetModifiedColor(isHovered);

                // Name
                DrawLabelDeferred(
                    labelRect.position,
                    graph.name,
                    graphColor
                );

                // Max
                DrawLabelDeferred(
                    minMaxOrigin,
                    graph.maxString,
                    graphColor
                );

                // Min
                DrawLabelDeferred(
                    minMaxOrigin + new Vector2(0, Settings.graphHeight - 20),
                    graph.minString,
                    graphColor
                );

                // Graph
                if (graph.visible)
                {
                    graph.Draw(new Rect(
                        groupGraphRect.position + rect.position,
                        groupGraphRect.size
                    ));
                }
            }

            // Scrubber
            if (groupGraphRect.Contains(relativeMousePos))
            {
                if (DebugGUIInput.LeftMouseButtonPressed)
                {
                    freezeGraphs = true;
                }

                // Background
                Vector2 scrubberOrigin = new Vector2(relativeMousePos.x, groupOrigin.y);
                if (relativeMousePos.x > groupGraphRect.max.x - scrubberBackgroundWidth)
                {
                    scrubberOrigin.x -= scrubberBackgroundWidth;
                }

                var rect = new Rect(
                    scrubberOrigin.x,
                    scrubberOrigin.y,
                    scrubberBackgroundWidth,
                    Settings.graphHeight
                );
                DrawRect(rect, Settings.backgroundColor);

                DrawLine(
                    new Vector2(
                        relativeMousePos.x,
                        groupOrigin.y
                    ), new Vector2(
                        relativeMousePos.x,
                        groupOrigin.y + Settings.graphHeight
                    ),
                    Settings.scrubberColor
                );

                // Scrubber labels
                Vector2 textPos = scrubberOrigin + new Vector2(graphLabelPadding, graphLabelPadding * 3);
                var groupMousePosX = (relativeMousePos.x - groupOrigin.x);
                int sampleIndex = (int)(groupGraphRect.width - groupMousePosX + graphLabelBoxWidth + graphBlockPadding);
                foreach (GraphContainer graph in group)
                {
                    var text = graph.GetValue(sampleIndex).ToString("F3");
                    DrawLabelDeferred(
                        textPos,
                        text,
                        color: graph.color
                    );

                    textPos.y += cachedLineHeight;
                }
            }
        }

        private Rect GetGraphWindowRect()
        {
            return new Rect(
                new Vector2(-graphLabelBoxWidth, 0) + rect.position,
                new Vector2(
                    Settings.graphWidth + graphLabelBoxWidth + graphBlockPadding,
                    (Settings.graphHeight + graphBlockPadding) * graphGroups.Count
                )
            );
        }

        private void RegisterAttributes()
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                Type mbType = mb.GetType();
                HashSet<MonoBehaviour> uniqueAttributeContainers = new();

                // Ensure collections exist for this type
                if (!debugGUIGraphFields.ContainsKey(mbType)) debugGUIGraphFields[mbType] = new HashSet<FieldInfo>();
                if (!debugGUIGraphProperties.ContainsKey(mbType)) debugGUIGraphProperties[mbType] = new HashSet<PropertyInfo>();
                // Assuming you have added a dictionary for methods:
                if (!debugGUIGraphMethods.ContainsKey(mbType)) debugGUIGraphMethods[mbType] = new HashSet<MethodInfo>();

                // Retrieve Fields, Properties, and Methods in one unified collection
                var members = mbType.GetMembers(bindingFlags)
                    .Where(m => m.MemberType == MemberTypes.Field || 
                                m.MemberType == MemberTypes.Property || 
                                m.MemberType == MemberTypes.Method);

                foreach (var member in members)
                {
                    if (Attribute.GetCustomAttribute(member, typeof(DebugGUIGraphAttribute)) is DebugGUIGraphAttribute graphAttribute)
                    {
                        // Attempt to extract the value as a float
                        var value = TryGetMemberValue(member, mb);
                        if (!IsSupportedType(value))
                        {
                            Debug.LogError($"Type {value.GetType()} returned by {mbType.Name}.{member.Name} is not supported supported (or method requires parameters). This member will be ignored.");
                            continue;
                        }

                        uniqueAttributeContainers.Add(mb);
                        GraphAttributeKey key = null;

                        // Route the member to its specific collection and generate the key
                        if (member is FieldInfo field)
                        {
                            debugGUIGraphFields[mbType].Add(field);
                            key = new GraphAttributeKey(field);
                        }
                        else if (member is PropertyInfo property)
                        {
                            debugGUIGraphProperties[mbType].Add(property);
                            key = new GraphAttributeKey(property);
                        }
                        else if (member is MethodInfo method)
                        {
                            debugGUIGraphMethods[mbType].Add(method);
                            key = new GraphAttributeKey(method); // Note: See prerequisites below
                        }

                        // Build and configure the graph
                        GraphContainer graph = new GraphContainer(Settings.graphWidth, graphAttribute.group)
                        {
                            name = member.Name,
                            max = graphAttribute.max,
                            min = graphAttribute.min,
                            autoScale = graphAttribute.autoScale
                        };
                        graph.OnLabelSizeChange += RefreshRect;
                        
                        if (!graphAttribute.color.Equals(default(Color)))
                            graph.color = graphAttribute.color;

                        // Register the key
                        if (!attributeKeys.ContainsKey(mb))
                            attributeKeys[mb] = new List<GraphAttributeKey>();
                        
                        attributeKeys[mb].Add(key);
                        AddGraph(key, graph);
                    }
                }

                // Tally up the containers
                foreach (var attributeContainer in uniqueAttributeContainers)
                {
                    attributeContainers.Add(attributeContainer);
                    Type type = attributeContainer.GetType();
                    
                    if (!typeInstanceCounts.ContainsKey(type))
                        typeInstanceCounts[type] = 0;
                        
                    typeInstanceCounts[type]++;
                }
            }
        }
        // Local helper function to safely extract values from any member type
        [CanBeNull]
        private object TryGetMemberValue(MemberInfo memberInfo, MonoBehaviour instance)
        {
            try
            {
                if(memberInfo is MethodInfo met)
                {
                    Debug.Log($"{memberInfo}.{memberInfo.Name}: {met.GetParameters()}");
                }
                return memberInfo switch
                {
                    FieldInfo f => f.GetValue(instance),
                    PropertyInfo p => p.GetValue(instance, null),
                    // For methods, ensure there are no required parameters before invoking
                    MethodInfo m => m.GetParameters().Count((p) => !p.HasDefaultValue) == 0 ? m.Invoke(instance, null) : null,
                    _ => null
                };
            }
            catch(Exception e)
            {
                Debug.LogWarning($"[TryGetMemberValue] Exception: {memberInfo}.{memberInfo.Name}: {e}");
                // Catches invocation exceptions (e.g., property getter throws)
                return null;
            }
        }

                        // TODO: Test tjat the tpes supported here and polling are the same
        private bool IsSupportedType(object value) => value is float or int or Vector2 or Vector3;

        private void CleanUpDeletedAttributes()
        {
            // Clear out associated keys
            foreach (var mb in attributeContainers)
            {
                if (mb == null)
                {
                    if (!attributeKeys.ContainsKey(mb)) continue;
                    var keys = attributeKeys[mb];
                    foreach (var key in keys)
                    {
                        RemoveGraph(key);
                    }

                    attributeKeys.Remove(mb);

                    Type type = mb.GetType();
                    typeInstanceCounts[type]--;
                    if (typeInstanceCounts[type] == 0)
                    {
                        if (debugGUIGraphFields.ContainsKey(type))
                            debugGUIGraphFields.Remove(type);
                        if (debugGUIGraphProperties.ContainsKey(type))
                            debugGUIGraphProperties.Remove(type);
                    }
                }
            }

            // Finally clear out removed nodes
            attributeContainers.RemoveWhere(node => node == null);
        }

        void RefreshRect()
        {
            var lastWidth = rect.width;
            RecalculateGraphLabelWidth();
            rect.size = new Vector2(
                Settings.graphWidth + graphLabelBoxWidth + graphBlockPadding,
                (Settings.graphHeight + graphBlockPadding) * graphGroups.Count);
            // Grow to the left instead of right
            rect.position += new Vector2(lastWidth - rect.width, 0);
        }

        void RecalculateGraphLabelWidth()
        {
            float width = 0;
            foreach (var group in graphGroups.Values)
            {
                float minMaxWidth = graphLabelPadding;
                foreach (var graph in group)
                {
                    // Names
                    width = Mathf.Max(GetMultilineStringSize(graphLabelStyle, in graph.name).x, width);

                    // Minmax labels per group
                    var maxWidthOfMinMaxStrings = Mathf.Max(
                        GetMultilineStringSize(graphLabelStyle, graph.minString).x,
                        GetMultilineStringSize(graphLabelStyle, graph.maxString).x
                    );
                    minMaxWidth += maxWidthOfMinMaxStrings + graphLabelPadding;
                }

                width = Mathf.Max(minMaxWidth, width);
            }

            graphLabelBoxWidth = width + graphLabelPadding * 2;
        }

        // GUI calls will break later GL calls, so we defer the label drawing

        List<DeferredLabel> deferredLabels = new();
        struct DeferredLabel
        {
            public Vector2 position;
            public string label;
            public Color color;
        }

        void DrawLabelDeferred(Vector2 pos, string label, Color color)
        {
            deferredLabels.Add(new DeferredLabel() { position = pos, label = label, color = color });
        }

        private class GraphContainer
        {
            public Action OnLabelSizeChange;

            public string name = "<uninitialized>";

            // Value at the top of the graph
            public float max = 1;
            // Value at the bottom of the graph
            public float min = 0;
            public bool autoScale;
            public Color color;
            // Graph order on screen
            public readonly int group;

            private int currentIndex;
            private readonly float[] values;
            private readonly Vector2[] graphPoints;

            public string minString = null;
            public string maxString = null;
            public bool visible = true;

            public GraphContainer(int width, int group = 0)
            {
                this.group = group;
                values = new float[width];
                graphPoints = new Vector2[width];
                SetMinMax(min, max);
            }

            public Color GetModifiedColor(bool highlighted)
            {
                if (!highlighted && visible) return color;

                Color.RGBToHSV(color, out float h, out float s, out float v);

                if (!visible) v *= 0.3f;
                if (highlighted) v *= (v > 0.9f ? 0.7f : 1.2f);
                return Color.HSVToRGB(h, s, v);
            }

            public void SetMinMax(float min, float max)
            {
                OnLabelSizeChange?.Invoke();
                this.min = min;
                this.max = max;

                minString = min.ToString("F2");
                maxString = max.ToString("F2");
            }

            // Add a data point to the beginning of the graph
            public void Push(float val)
            {
                if (autoScale && (val > max || val < min))
                {
                    SetMinMax(Mathf.Min(val, min), Mathf.Max(val, max));
                }
                else
                {
                    // Prevent drawing outside frame
                    val = Mathf.Clamp(val, min, max);
                }

                values[currentIndex] = val;
                currentIndex = (currentIndex + 1) % values.Length;
            }

            public void Clear()
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = 0;
                }
            }

            public void Draw(Rect rect)
            {
                GL.Begin(GL.LINE_STRIP);
                {
                    GL.Color(color);

                    int num = values.Length;
                    for (int i = 0; i < num; i++)
                    {
                        float value = values[Mod(currentIndex - i - 1, values.Length)];
                        // Note flipped inverse lerp min max to account for y = down in GL
                        GL.Vertex3(
                            rect.x + (rect.width * ((float)i / num)),
                            rect.y + (Mathf.InverseLerp(max, min, value) * Settings.graphHeight),
                            0.0f);
                    }
                }
                GL.End();
            }

            public float GetValue(int index)
            {
                return values[Mod(currentIndex + index, values.Length)];
            }

            [Serializable]
            public class DataExport
            {
                public string name;
                public float[] values;

                public DataExport(string name, float[] values)
                {
                    this.name = name;
                    this.values = values;
                }
            }

            public DataExport AsDataExport()
            {
                return new DataExport(name, values);
            }

            private static int Mod(int n, int m)
            {
                return ((n % m) + m) % m;
            }
        }

        public class GraphAttributeKey
        {
            public MemberInfo memberInfo;
            public GraphAttributeKey(MemberInfo memberInfo)
            {
                this.memberInfo = memberInfo;
            }
        }
    }
}