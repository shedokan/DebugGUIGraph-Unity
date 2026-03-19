using UnityEngine;

public enum ScreenCorner { TopLeft, TopRight, BottomLeft, BottomRight }

// [CreateAssetMenu(fileName = "DebugGUISettings", menuName = "DebugGUI/Settings", order = 1)]
public class DebugGUISettings : ScriptableObject
{
    [SerializeField] public bool enableGraphs = true;
    [SerializeField] public bool enableLogs = true;

    [SerializeField] public Color backgroundColor = new Color(0f, 0f, 0f, 0.7f);
    [SerializeField] public Color scrubberColor = new Color(1f, 1f, 0f, 0.7f);
    [SerializeField] public int graphWidth = 300;
    [SerializeField] public int graphHeight = 100;
    [SerializeField] public float temporaryLogLifetime = 5;

    [SerializeField] public float scale = 1f;
    [SerializeField] public bool autoDPIScale = false;

    [SerializeField] public ScreenCorner graphInitialCorner = ScreenCorner.TopRight;
    [SerializeField] public Vector2 graphInitialOffset = new Vector2(0, 20);
    [SerializeField] public ScreenCorner logInitialCorner = ScreenCorner.TopLeft;
    [SerializeField] public Vector2 logInitialOffset = Vector2.zero;

    public float EffectiveScale => autoDPIScale && Screen.dpi > 0
        ? scale * (Screen.dpi / 96f)
        : scale;

    public int ScaledGraphWidth => Mathf.RoundToInt(graphWidth * EffectiveScale);
    public int ScaledGraphHeight => Mathf.RoundToInt(graphHeight * EffectiveScale);
}