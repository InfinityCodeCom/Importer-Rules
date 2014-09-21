using System;
using System.Text.RegularExpressions;
using InfinityCode.ImporterRules;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ImporterRulesEditorWindow:EditorWindow
{
    private static ImporterRule tempRule;
    private static ImporterRule activeRule;
    private static ImporterRulesEditorWindow wnd;

    public static void CloseWindow()
    {
        if (wnd == null) return;
        wnd.Close();
        wnd = null;
    }

    private void OnEnable()
    {
        wnd = this;
    }

    private void OnGUI()
    {
        if (tempRule == null) return;

        tempRule.DrawEditor();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save"))
        {
            if (tempRule.pathComparer == ImporterRulesPathComparer.regEx && !string.IsNullOrEmpty(tempRule.pattern))
            {
                try
                {
                    new Regex(tempRule.pattern);
                }
                catch (Exception)
                {
                    Debug.LogWarning("Regex pattern contains an error.");
                    return;
                }
            }

            if (activeRule != null) activeRule.CopyFrom(tempRule);
            else ImporterRulesWindow.AddRule(tempRule);
            ImporterRulesWindow.Save();
            ImporterRulesWindow.RedrawWindow();
            Close();
        }

        if (GUILayout.Button("Cancel"))
        {
            activeRule = null;
            Close();
        }

        GUILayout.EndHorizontal();
    }

    public static ImporterRule OpenWindow(ImporterRule rule = null)
    {
        activeRule = rule;
        wnd = GetWindow<ImporterRulesEditorWindow>(false, "Rules Editor", true);
        tempRule = new ImporterRule();
        if (rule != null) tempRule.CopyFrom(rule);
        return tempRule;
    }

    public static void OpenWindow(Object obj)
    {
        OpenWindow();
        tempRule.LoadFromAsset(obj);
    }
}