﻿/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using InfinityCode.ImporterRules;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ImporterRulesWindow : EditorWindow
{
    private const string settingsFilename = "ImporterRules.xml";

    public static bool logUserRules = true;
    private static List<ImporterRule> rules;
    private static ImporterRulesWindow wnd;

    private Vector2 scrollPosition;

    public static int countRules
    {
        get { return rules.Count; }
    }
    

    public static void AddRule(ImporterRule rule)
    {
        if (rules == null) Load();
        rules.Add(rule);
        Save();
    }

    public static bool ApplyFirstRule(ImporterRulesTypes assetType, string assetPath, AssetImporter assetImporter)
    {
        if (rules == null) Load();
        List<ImporterRule> currentRules = rules.Where(r => r.enabled && r.type == assetType).ToList();
        if (currentRules.Count == 0) return false;

        foreach (ImporterRule rule in currentRules)
        {
            if (rule.CheckPath(assetPath))
            {
                rule.Apply(assetImporter, assetPath);
                return true;
            }
        }

        return false;
    }

    [MenuItem("Assets/Create Importer Rule from Asset")]
    public static void CreateRuleFromAsset()
    {
        Object[] selections = Selection.GetFiltered(typeof (Object), SelectionMode.Assets);

        if (selections.Length != 1)
        {
            EditorUtility.DisplayDialog("Failed", "Select one asset", "OK");
            return;
        }

        Object selection = selections[0];

        bool allowCreate = false;

        if (selection is Texture) allowCreate = true;
        else if (selection is Font) allowCreate = true;
        else if (selection is AudioClip) allowCreate = true;
        else if (selection is GameObject)
        {
            string assetPath = AssetDatabase.GetAssetPath(selection);
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
            if (assetImporter is ModelImporter) allowCreate = true;
        }

        if (!allowCreate)
        {
            EditorUtility.DisplayDialog("Failed", "Unknown importer", "OK");
            return;
        }

        if (wnd == null) OpenWindow();
        ImporterRulesEditorWindow.OpenWindow(selection);
    }

    private void DeleteAllRules()
    {
        if (!EditorUtility.DisplayDialog("Delete all rules", "You really want to delete all the rules?", "Delete", "Cancel")) return;
        rules.Clear();
        Save();
    }

    private void DisableAllRules()
    {
        foreach (ImporterRule rule in rules) rule.enabled = false;
        Save();
    }

    private void DrawRules()
    {
        if (rules == null) return;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < rules.Count; i++)
        {
            rules[i].DrawPreview(i);
        }
        if (rules.RemoveAll(r => r.deleted) > 0) Save();

        EditorGUILayout.EndScrollView();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Add new rule", EditorStyles.toolbarButton))
        {
            ImporterRulesEditorWindow.OpenWindow();
        }

        if (GUILayout.Button("Options", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Disable all rules"), false, DisableAllRules);
            menu.AddItem(new GUIContent("Delete all rules"), false, DeleteAllRules);
            menu.ShowAsContext();
        }

        if (GUILayout.Button("Help", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("View online documentation"), false, OnViewDocs);
            menu.AddItem(new GUIContent("Open product page"), false, OnProductPage);
            menu.AddItem(new GUIContent("Support"), false, OnSendMail);
            menu.ShowAsContext();
        } 

        EditorGUILayout.EndHorizontal(); 
    }

    public static void DuplicateRule(ImporterRule rule)
    {
        ImporterRule newRule = ImporterRulesEditorWindow.OpenWindow();
        newRule.CopyFrom(rule);
        string name = newRule.name;
        Regex regex = new Regex(@"\d+$", RegexOptions.IgnoreCase);
        if (regex.IsMatch(name))
            newRule.name = regex.Replace(name, match => (int.Parse(match.Value) + 1).ToString());
        else newRule.name += " 2";
    }

    public static string GetNewRuleName()
    {
        const string defName = "New rule";
        string name = defName;
        int index = 2;

        while (rules.Any(r => r.name == name))
        {
            name = defName + " " + index++;
        }
        return name;
    }

    private static void Load()
    {
        rules = new List<ImporterRule>();

        if (!File.Exists(settingsFilename)) return;
        XmlDocument document = new XmlDocument();
        document.Load(settingsFilename);

        XmlNode firstNode = document.FirstChild;

        XmlAttribute logAttr = firstNode.Attributes["logUsedRules"];
        if (logAttr != null) logUserRules = logAttr.Value == "True";

        foreach (XmlNode node in firstNode.ChildNodes) rules.Add(new ImporterRule(node));
    }

    private void OnEnable()
    {
        wnd = this;
        Load();
    }

    private void OnDestroy()
    {
        Save();
        ImporterRulesEditorWindow.CloseWindow();
    }

    private void OnGUI()
    {
        DrawToolbar();
        DrawRules();

        EditorGUI.BeginChangeCheck();
        logUserRules = EditorGUILayout.Toggle("Log used rules", logUserRules);
        if (EditorGUI.EndChangeCheck()) Save();
    }

    private void OnProductPage()
    {
        Process.Start("http://infinity-code.com/products/importer-rules");
    }

    private void OnSendMail()
    {
        Process.Start("mailto:support@infinity-code.com?subject=Importer Rules");
    }

    private void OnViewDocs()
    {
        Process.Start("http://infinity-code.com/docs/importer-rules");
    }

    [MenuItem("Window/Infinity Code/Importer Rules")]
    public static void OpenWindow()
    {
        wnd = GetWindow<ImporterRulesWindow>(false, "Importer Rules", true);
    }

    public static void RedrawWindow()
    {
        wnd.Repaint();
    }

    public static void Save()
    {
        XmlDocument document = new XmlDocument();
        XmlElement firstNode = document.CreateChild("Rules");

        firstNode.SetAttribute("logUsedRules", logUserRules ? "True" : "False");

        foreach (ImporterRule rule in rules)
        {
            XmlElement ruleElement = firstNode.CreateChild("Rule");
            rule.Save(ruleElement);
        }

        File.WriteAllText(settingsFilename, document.InnerXml);
    }

    public static void SetRuleIndex(ImporterRule rule, int index)
    {
        if (index < 0 || index > countRules - 1) return;

        int oldIndex = rules.IndexOf(rule);
        rules.Remove(rule);
        if (index > oldIndex) index--;
        rules.Insert(index, rule);
    }
}