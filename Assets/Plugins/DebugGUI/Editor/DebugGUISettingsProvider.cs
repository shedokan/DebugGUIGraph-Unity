using System.IO;
using UnityEditor;
using UnityEngine;

static class DebugGUISettingsProvider
{
    const string CreationPath = "Assets/Settings/Resources/DebugGUISettings.asset";

    [SettingsProvider]
    static SettingsProvider Create() => new("Project/DebugGUI", SettingsScope.Project)
    {
        label = "DebugGUI",
        keywords = new[] { "DebugGUI", "graph", "log", "debug" },
        guiHandler = _ =>
        {
            var settings = FindSettings();
            if (!settings)
            {
                EditorGUILayout.HelpBox(
                    $"No DebugGUISettings asset found in any Resources folder.\n" +
                    $"Create one below — it will be placed at:\n{CreationPath}",
                    MessageType.Info);

                if (GUILayout.Button("Create DebugGUISettings Asset"))
                    CreateAndSelect();
                return;
            }

            var editor = Editor.CreateEditor(settings);
            editor.OnInspectorGUI();
        }
    };

    static DebugGUISettings FindSettings()
    {
        var guids = AssetDatabase.FindAssets("t:DebugGUISettings");
        if (guids.Length == 0) return null;
        return AssetDatabase.LoadAssetAtPath<DebugGUISettings>(AssetDatabase.GUIDToAssetPath(guids[0]));
    }

    static void CreateAndSelect()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(CreationPath)!);
        AssetDatabase.Refresh();

        var asset = ScriptableObject.CreateInstance<DebugGUISettings>();
        AssetDatabase.CreateAsset(asset, CreationPath);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
