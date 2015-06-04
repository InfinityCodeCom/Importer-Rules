﻿/*     INFINITY CODE 2013-2015      */
/*   http://www.infinity-code.com   */

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.ImporterRules
{
    [Serializable]
    public class ImporterRule
    {
        public bool deleted;
        public bool enabled = true;
        public bool expanded;
        public string name;
        public string path;
        public string pattern = "";
        public ImporterRulesPathComparer pathComparer = ImporterRulesPathComparer.allAssets;
        public ImporterRulesTypes type = ImporterRulesTypes.texture;

#if UNITY_4_5 || UNITY_4_6
        [SerializeField] private ImporterRuleAudioSettings audioSettings;
#else
        [SerializeField] private ImporterRuleAudioSettingsUFive audioSettings;
#endif
        [SerializeField] private ImporterRuleFontSettings fontSettings;
        [SerializeField] private ImporterRuleModelSettings modelSettings;
        [SerializeField] private ImporterRuleMovieSettings movieSettings;
        [SerializeField] private ImporterRuleTextureSettings textureSettings;

        private ImporterRuleBaseSettings activeSettings;

        private Vector2 scrollPosition;

        public ImporterRule()
        {
            name = ImporterRulesWindow.GetNewRuleName();

            InitSettings();

            activeSettings = textureSettings;
        }

        public ImporterRule(XmlNode node)
        {
            node.TryGetValue("Name", ref name);
            node.TryGetValue("Enabled", ref enabled);

            string typeString = "";
            if (node.TryGetValue("Type", ref typeString)) type = (ImporterRulesTypes)Enum.Parse(typeof(ImporterRulesTypes), typeString);

            string pathComparerString = "";
            if (node.TryGetValue("PathComparer", ref pathComparerString)) pathComparer = (ImporterRulesPathComparer)Enum.Parse(typeof(ImporterRulesPathComparer), pathComparerString);

            node.TryGetValue("Path", ref path);
            node.TryGetValue("Pattern", ref pattern);

            XmlNode propsNode = node["Props"];

            InitSettings();

            UpdateActiveSettings();

            activeSettings.Load(propsNode);
        }

        public void Apply(AssetImporter assetImporter, string assetPath)
        {
            if (ImporterRulesWindow.logUserRules) Debug.Log("Apply rule [" + name + "]: " + assetPath.Substring(7));
            activeSettings.SetSettingsToImporter(assetImporter, assetPath);
        }

        private void ApplyToExists()
        {
            ImporterRulesWindow.ignoreApplyFirstRule = true;

            string typeStr = default(string);

            if (type == ImporterRulesTypes.texture) typeStr = "texture";
            else if (type == ImporterRulesTypes.trueTypeFont) typeStr = "font";
            else if (type == ImporterRulesTypes.movie) typeStr = "movietexture";
            else if (type == ImporterRulesTypes.model) typeStr = "model";
            else if (type == ImporterRulesTypes.audio) typeStr = "audioclip";

            if (string.IsNullOrEmpty(typeStr)) return;

            string[] files = AssetDatabase.FindAssets("t:" + typeStr).Select(guid => AssetDatabase.GUIDToAssetPath(guid)).Where(fn => !string.IsNullOrEmpty(fn) && CheckPath(fn)).ToArray();
            if (files.Length == 0) return;
            Object[] objects = files.Select(fn => AssetImporter.GetAtPath(fn)).ToArray();

            if (objects == null || objects.Length == 0) return;

            for (int i = 0; i < objects.Length; i++)
            {
                Object obj = objects[i];
                string assetPath = files[i];

                EditorUtility.DisplayProgressBar("Apply rule", assetPath, i / (float)objects.Length);

                AssetImporter assetImporter = obj as AssetImporter;
                if (type == ImporterRulesTypes.audio && assetImporter is MovieImporter) continue;
                if (type == ImporterRulesTypes.movie || type == ImporterRulesTypes.trueTypeFont)
                    ImporterRulesPreprocess.AddWaitPath(assetPath);

                Apply(assetImporter, assetPath);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }

            EditorUtility.ClearProgressBar();

            ImporterRulesWindow.ignoreApplyFirstRule = false;
        }

        public bool CheckPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath)) return false;
            if (pathComparer == ImporterRulesPathComparer.allAssets) return true;
            if (pathComparer != ImporterRulesPathComparer.regex &&  string.IsNullOrEmpty(path)) return true;
            if (pathComparer == ImporterRulesPathComparer.regex && string.IsNullOrEmpty(pattern)) return true;

            assetPath = assetPath.FixPath().Substring(7).ToLower();

            if (path.Length > assetPath.Length) return false;

            string curPath = path.FixPath().ToLower();
            if (pathComparer == ImporterRulesPathComparer.startWith)
            {
                string str = assetPath.Substring(0, path.Length);
                if (str == curPath) return true;
            }
            else if (pathComparer == ImporterRulesPathComparer.contains)
            {
                if (assetPath.Contains(curPath)) return true;
            }
            else if (pathComparer == ImporterRulesPathComparer.regex)
            {
                Regex regex = new Regex(pattern);
                if (regex.IsMatch(assetPath)) return true;
            }
            return false;
        }

        public void CopyFrom(ImporterRule rule)
        {
            name = rule.name;
            path = rule.path;
            pattern = rule.pattern;
            pathComparer = rule.pathComparer;
            type = rule.type;
            audioSettings = rule.audioSettings;
            fontSettings = rule.fontSettings;
            modelSettings = rule.modelSettings;
            movieSettings = rule.movieSettings;
            textureSettings = rule.textureSettings;
            UpdateActiveSettings();
        }

        public void DrawEditor(bool useScroll = true)
        {
            name = EditorGUILayout.TextField("Name: ", name);

            EditorGUI.BeginChangeCheck();
            type = (ImporterRulesTypes)EditorGUILayout.EnumPopup("Type: ", type);
            if (EditorGUI.EndChangeCheck()) UpdateActiveSettings();

            pathComparer = (ImporterRulesPathComparer)EditorGUILayout.EnumPopup("Path comparer: ", pathComparer);

            if (pathComparer == ImporterRulesPathComparer.contains ||
                pathComparer == ImporterRulesPathComparer.startWith)
            {
                GUILayout.BeginHorizontal();
                path = EditorGUILayout.TextField("Path: ", path);
                GUI.SetNextControlName("PathBrowseButton");
                if (GUILayout.Button(new GUIContent("...", "Browse"), GUILayout.ExpandWidth(false)))
                {
                    string folderPath = EditorUtility.OpenFolderPanel("Path", Application.dataPath, "");
                    if (!string.IsNullOrEmpty(folderPath))
                    {
                        if (!folderPath.Contains(Application.dataPath))
                        {
                            EditorUtility.DisplayDialog("Error", "The folder must be inside the folder Assets", "OK");
                        }
                        else if (folderPath != Application.dataPath)
                        {
                            path = folderPath.Substring(Application.dataPath.Length + 1) + "/";
                        }
                        else path = "";
                        GUI.FocusControl("PathBrowseButton");
                    }
                }
                GUILayout.EndHorizontal();
            }
            else if (pathComparer == ImporterRulesPathComparer.regex) pattern = EditorGUILayout.TextField("Pattern: ", pattern);

            EditorGUILayout.Space();

            if (useScroll) scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            activeSettings.DrawEditor();

            if (useScroll) EditorGUILayout.EndScrollView();
        }

        public void DrawPreview(int index)
        {
            string[] types = { "TextureImporter", "ModelImporter", "AudioImporter", "MovieImporter", "TrueTypeFontImporter" };

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            expanded = GUILayout.Toggle(expanded, "", EditorStyles.foldout, GUILayout.ExpandWidth(false));

            EditorGUI.BeginChangeCheck();
            enabled = GUILayout.Toggle(enabled, "", GUILayout.ExpandWidth(false));
            if (EditorGUI.EndChangeCheck()) ImporterRulesWindow.Save();

            GUILayout.Label(name + " [" + types[(int)type] + "]");

            if (ImporterRulesWindow.countRules > 1)
            {
                if (GUILayout.Button(new GUIContent("▲", "Move up"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
                {
                    ImporterRulesWindow.SetRuleIndex(this, index - 1);
                }
                if (GUILayout.Button(new GUIContent("▼", "Move down"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
                {
                    ImporterRulesWindow.SetRuleIndex(this, index + 1);
                }
            }
            if (GUILayout.Button(new GUIContent("►", "Apply the rule to the existing assets"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                if (EditorUtility.DisplayDialog("Apply the rule", "Apply the rule to the existing assets?", "Apply", "Cancel"))
                {
                    ApplyToExists();
                }
            }
            if (GUILayout.Button(new GUIContent("E", "Edit"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                ImporterRulesEditorWindow.OpenWindow(this);
            }
            if (GUILayout.Button(new GUIContent("D", "Duplicate"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                ImporterRulesWindow.DuplicateRule(this);
            }
            if (GUILayout.Button(new GUIContent("X", "Delete"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                if (EditorUtility.DisplayDialog("Delete rule", "You sure want to delete a rule?", "Delete", "Cancel"))
                {
                    deleted = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (expanded)
            {
                EditorGUILayout.Space();
                EditorGUI.BeginDisabledGroup(true);

                DrawEditor(false);

                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndVertical();
        }

        private void InitSettings()
        {
#if UNITY_4_5 || UNITY_4_6
            audioSettings = new ImporterRuleAudioSettings();
#else
            audioSettings = new ImporterRuleAudioSettingsUFive();
#endif
            fontSettings = new ImporterRuleFontSettings();
            modelSettings = new ImporterRuleModelSettings();
            movieSettings = new ImporterRuleMovieSettings();
            textureSettings = new ImporterRuleTextureSettings();
        }

        public void LoadFromAsset(Object obj)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);

            FileAttributes attr = File.GetAttributes(assetPath);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                path = assetPath.Substring(7) + "/";
                pathComparer = ImporterRulesPathComparer.startWith;
                return;
            }

            AssetImporter importer = AssetImporter.GetAtPath(assetPath);

            if (importer == null) return;

            if (importer is TextureImporter) type = ImporterRulesTypes.texture;
            else if (importer is ModelImporter) type = ImporterRulesTypes.model;
            else if (importer is MovieImporter) type = ImporterRulesTypes.movie;
            else if (importer is AudioImporter) type = ImporterRulesTypes.audio;
            else if (importer is TrueTypeFontImporter) type = ImporterRulesTypes.trueTypeFont;

            UpdateActiveSettings();
            activeSettings.GetSettingsFromImporter(importer);
        }

        public void Save(XmlElement element)
        {
            element.CreateChild("Name", name);
            element.CreateChild("Type", type.ToString());
            element.CreateChild("Enabled", enabled);
            if (pathComparer != ImporterRulesPathComparer.allAssets)
            {
                element.CreateChild("PathComparer", pathComparer.ToString());
                if (pathComparer != ImporterRulesPathComparer.regex && !string.IsNullOrEmpty(path)) element.CreateChild("Path", path);
                if (pathComparer == ImporterRulesPathComparer.regex && !string.IsNullOrEmpty(pattern)) element.CreateChild("Pattern", pattern);
            }
            XmlElement propsElement = element.CreateChild("Props");

            activeSettings.Save(propsElement);
        }

        private void UpdateActiveSettings()
        {
            if (type == ImporterRulesTypes.texture) activeSettings = textureSettings;
            else if (type == ImporterRulesTypes.model) activeSettings = modelSettings;
            else if (type == ImporterRulesTypes.movie) activeSettings = movieSettings;
            else if (type == ImporterRulesTypes.audio) activeSettings = audioSettings;
            else if (type == ImporterRulesTypes.trueTypeFont) activeSettings = fontSettings;
        }
    }
}