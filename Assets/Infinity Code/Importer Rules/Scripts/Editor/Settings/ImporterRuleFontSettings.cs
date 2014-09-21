using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.ImporterRules
{
    public class ImporterRuleFontSettings:ImporterRuleBaseSettings
    {
        public string customCharacters = "";
        public FontRenderingMode fontRenderingMode = FontRenderingMode.Smooth;
        public int fontSize = 16;
        public FontTextureCase fontTextureCase = FontTextureCase.Dynamic;
        public bool includeFontData = true;

        public override void DrawEditor()
        {
            fontSize = Mathf.Clamp(EditorGUILayout.IntField("Font Size", fontSize), 1, 500);
            fontRenderingMode = (FontRenderingMode) EditorGUILayout.EnumPopup("Rendering Mode", fontRenderingMode);
            fontTextureCase = (FontTextureCase) EditorGUILayout.EnumPopup("Character", fontTextureCase);
            if (fontTextureCase == FontTextureCase.Dynamic)
            {
                includeFontData = EditorGUILayout.Toggle("Incl. Font Data", includeFontData);
            }
            else if (fontTextureCase == FontTextureCase.CustomSet)
            {
                EditorGUILayout.PrefixLabel("Custom Chars");
                EditorGUI.BeginChangeCheck();
                customCharacters = EditorGUILayout.TextArea(customCharacters, GUILayout.MinHeight(32f));
                if (EditorGUI.EndChangeCheck())
                {
                    customCharacters = new string(customCharacters.Distinct().ToArray()).Replace("\n", string.Empty).Replace("\r", string.Empty);
                }
            }
            
        }

        public override void GetSettingsFromImporter(AssetImporter importer)
        {
            PropsToFields(importer, this);
        }

        public override void Load(XmlNode node)
        {
            LoadSerialized(node, this, GetType().GetFields());
        }

        public override void Save(XmlElement element)
        {
            SaveSerialized(element, this, GetType().GetFields());
        }

        public override void SetSettingsToImporter(AssetImporter importer)
        {
            FieldsToProps(this, importer);
        }
    }
}