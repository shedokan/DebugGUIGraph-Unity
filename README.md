
<h1 align="center"> Debug GUI Graph - Unity </h1> <br>
<p align="center">
    <img alt="DebugGUI Unity Icon" title="DebugGUIUnity" src="https://github.com/WeaverDev/DebugGUIGraph-Unity/assets/22682921/59a21da6-ff7a-476b-be45-41f4c3b32b79" width="350">
</p>

<h4 align="center">
  Simple and easy to use graphing debug utility.
</h4>

<br>

## Description
DebugGUI Graph provides a way to debug continuous systems by providing an inspectable graphing GUI and logging overlay. It also provides an optional attribute-based abstraction for a one-line injection into your existing code.

## Requirements

- **Unity 6000+** (URP, HDRP, and Built-in render pipelines supported)
- The shader `Hidden/Internal-Colored` must be in **Project Settings → Graphics → Always Included Shaders** for mobile builds, or placed in a `Resources` folder.

## Installation

1. Download the latest commit or stable Release.
2. Place the `Plugins` folder into your project `Assets` folder.

---

## Quick Start

```csharp
// One attribute — instant graph
[DebugGUIGraph(min: -1, max: 1, r: 0, g: 1, b: 0, autoScale: true)]
float speed;

// Vector types automatically split into per-component graphs (.X / .Y / .Z)
[DebugGUIGraph]
Vector3 velocity;

// Print a value in the log overlay
[DebugGUIPrint]
float health;
```

---

## Attribute: `[DebugGUIGraph]`

Attach to any **field**, **property**, or **parameterless method** to graph its value automatically.

```csharp
[DebugGUIGraph(
    r: 1, g: 0.3f, b: 0,   // line color (RGB 0–1)
    min: 0, max: 100,       // graph scale range (used for clamping when autoScale is off)
    group: 0,               // which row the graph appears in
    autoScale: true         // expand scale range automatically if values exceed it
)]
float myValue;
```

All parameters are optional. Defaults: white line, range 0–1, group 0, autoScale on.

The min/max labels displayed on the graph always reflect the **actual observed data range**, regardless of the `min`/`max` parameters or `autoScale`. They show `-` until the first value arrives.

### Supported on fields, properties, and methods

```csharp
[DebugGUIGraph] public float speed;              // field
[DebugGUIGraph] public float Speed => rb.speed;  // property
[DebugGUIGraph] public float GetSpeed() => ...;  // parameterless method
```

---

## Supported Value Types

### Scalar (single graph line)

| Type | Notes |
|------|-------|
| `float`, `double`, `decimal` | |
| `int`, `uint`, `short`, `ushort`, `byte`, `sbyte`, `long` | Cast to float |
| `bool` | `true` = 1, `false` = 0 |

### Multi-component (overlaid component graphs, same group row)

Component graphs are color-coded using Unity's axis convention and overlaid in the same graph box so you can see relationships at a glance.

| Type | Graphs created | Colors |
|------|---------------|--------|
| `Vector2` / `Vector2Int` | `.X` `.Y` | Red, Green |
| `Vector3` / `Vector3Int` | `.X` `.Y` `.Z` | Red, Green, Blue |
| `Vector4` | `.X` `.Y` `.Z` `.W` | Red, Green, Blue, Magenta |
| `Quaternion` | `.X` `.Y` `.Z` `.W` | Red, Green, Blue, Magenta |
| `Color` | `.R` `.G` `.B` `.A` | Red, Green, Blue, White |
| `Color32` | `.R` `.G` `.B` `.A` | Red, Green, Blue, White (normalized 0–1) |

---

## Manual Graph API

For more control, manage graphs directly with an arbitrary key.

```csharp
void Awake()
{
    DebugGUI.SetGraphProperties(this, "Speed", min: 0, max: 100, group: 0, Color.cyan, autoScale: false);
}

void Update()
{
    DebugGUI.Graph(this, currentSpeed);
}
```

Other graph methods: `RemoveGraph(key)`, `ClearGraph(key)`, `ExportGraphs()` (saves JSON to `persistentDataPath`).

---

## Log Overlay

```csharp
// Attribute — polls every frame, shows "MyScript myField: value"
[DebugGUIPrint]
float health;

// Manual persistent log (stays until removed/updated)
DebugGUI.LogPersistent(this, $"HP: {health}");
DebugGUI.RemovePersistent(this);
DebugGUI.ClearPersistent();

// Temporary log (fades after `temporaryLogLifetime` seconds)
DebugGUI.Log("Hit!");
```

---

## Runtime Enable / Disable

Windows can be shown or hidden independently at any point during play.

```csharp
DebugGUI.SetEnabled(false);          // hide everything
DebugGUI.SetGraphsEnabled(false);    // hide graphs only
DebugGUI.SetLogsEnabled(false);      // hide logs only
DebugGUI.SetEnabled(true);           // restore both
```

Data continues to accumulate while hidden, so graphs resume seamlessly when re-shown.

---

## UI Interaction

| Action | Mouse | Touch |
|--------|-------|-------|
| **Drag** window | Middle mouse button | Two-finger press and drag |
| **Scrub** graph values | Hover over graph area | Single finger hover |
| **Freeze** updates | Hold left button on graph | Hold finger on graph |
| **Toggle** a graph line | Click its name label | Tap its name label |

---

## Settings (`DebugGUISettings` ScriptableObject)

Settings are stored in your project (not inside the package) so they stay under your version control.

**First-time setup:**
1. Open **Edit → Project Settings → DebugGUI**
2. Click **Create DebugGUISettings Asset**
3. The asset is created at `Assets/Settings/Resources/DebugGUISettings.asset`

After creation, all settings are editable directly in the Project Settings window. You can also select the asset in the Project panel and edit it in the Inspector, or create one manually via **Assets → Create → DebugGUI Settings** and place it in any `Resources` folder.

If no asset exists at runtime, built-in defaults are used automatically.

| Setting | Description |
|---------|-------------|
| `enableGraphs` / `enableLogs` | Initial visibility on Play |
| `graphWidth` / `graphHeight` | Graph box size in pixels |
| `lineThickness` | Graph line thickness (scaled with `scale`) |
| `scale` | Global UI scale multiplier |
| `scaleWithScreenSize` | Scale UI to match a reference resolution (like Canvas Scaler) |
| `referenceResolution` | Resolution graphs are authored at (used when `scaleWithScreenSize` is on) |
| `matchWidthOrHeight` | 0 = match width, 1 = match height, 0.5 = blend (same as Canvas Scaler slider) |
| `backgroundColor` | Window background color |
| `scrubberColor` | Scrubber line color |
| `temporaryLogLifetime` | Seconds before transient logs fade |
| `graphInitialCorner` / `logInitialCorner` | Starting screen corner |
| `graphInitialOffset` / `logInitialOffset` | Pixel offset from the starting corner |

---

## Tests

Runtime tests are in `Plugins/DebugGUI/Tests/Runtime/` and can be run from **Window → General → Test Runner** (PlayMode).

- **Type acceptance** — every supported scalar and multi-component type tested across fields, properties, and methods
- **Enable/disable** — independent and combined toggle coverage
- **Graph API** — push, clear, remove, autoScale, multi-group
- **Log API** — persistent, transient, update, remove
- **Export** — file exists, valid JSON, filename pattern

---

## Contribution
If you spot a bug, please create an [Issue](https://github.com/WeaverDev/DebugGUIGraph-Unity/issues).

## Acknowledgements
Special thanks to [TheSniperFan](https://github.com/TheSniperFan) for the 2.0 conversion from texture to GL lines.

## License
[MIT License](LICENSE)
