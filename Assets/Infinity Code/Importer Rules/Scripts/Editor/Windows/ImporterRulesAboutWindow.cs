using UnityEditor;
using UnityEngine;

public class ImporterRulesAboutWindow:EditorWindow
{
    [MenuItem("Window/Infinity Code/Importer Rules/About", false, 1)]
    public static void OpenWindow()
    {
        ImporterRulesAboutWindow window = GetWindow<ImporterRulesAboutWindow>(true, "About", true);
        window.minSize = new Vector2(200, 100);
        window.maxSize = new Vector2(200, 100);
    }

    public void OnGUI()
    {
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle textStyle = new GUIStyle(EditorStyles.label);
        textStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Importer Rules", titleStyle);
        GUILayout.Label("version " + ImporterRulesWindow.version, textStyle);
        GUILayout.Label("created Infinity Code", textStyle);
        GUILayout.Label("2013-2016", textStyle);
    }
}