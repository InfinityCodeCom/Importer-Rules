/*     INFINITY CODE 2013-2015      */
/*   http://www.infinity-code.com   */

using System;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.ImporterRules
{
    [Serializable]
    public class ImporterRuleModelSettings : ImporterRuleBaseSettings
    {
        private ModelImporterSettings settings;
        private bool secondaryUVAdvancedOptions;

        public ImporterRuleModelSettings()
        {
            settings = new ModelImporterSettings();
        }

        public override void DrawEditor()
        {
            GUILayout.Label("Meshes", EditorStyles.boldLabel);
            settings.globalScale = EditorGUILayout.FloatField("Scale Factor", settings.globalScale);
            settings.useFileUnits = EditorGUILayout.Toggle("Use File Units", settings.useFileUnits);
            settings.meshCompression =
                (ModelImporterMeshCompression) EditorGUILayout.EnumPopup("Mesh Compression", settings.meshCompression);
            settings.isReadable = EditorGUILayout.Toggle("Read/Write Enabled", settings.isReadable);
            settings.optimizeMesh = EditorGUILayout.Toggle("Optimize Mesh", settings.optimizeMesh);
            settings.importBlendShapes = EditorGUILayout.Toggle("Import BlendShapes", settings.importBlendShapes);
            settings.addCollider = EditorGUILayout.Toggle("Generate Colliders", settings.addCollider);
            settings.swapUVChannels = EditorGUILayout.Toggle("Swap UVs", settings.swapUVChannels);
            settings.generateSecondaryUV = EditorGUILayout.Toggle("Generate Lightmap UVs", settings.generateSecondaryUV);

            if (settings.generateSecondaryUV)
            {
                EditorGUI.indentLevel++;

                secondaryUVAdvancedOptions = EditorGUILayout.Foldout(secondaryUVAdvancedOptions, "Advanced");
                if (secondaryUVAdvancedOptions)
                {
                    settings.secondaryUVHardAngle =
                        Mathf.Round(EditorGUILayout.Slider("Hard Angle", settings.secondaryUVHardAngle, 0, 180));
                    settings.secondaryUVPackMargin =
                        Mathf.Round(EditorGUILayout.Slider("Pack Margin", settings.secondaryUVPackMargin, 1, 64));
                    settings.secondaryUVAngleDistortion =
                        Mathf.Round(EditorGUILayout.Slider("Angle Error", settings.secondaryUVAngleDistortion, 1, 75));
                    settings.secondaryUVAreaDistortion =
                        Mathf.Round(EditorGUILayout.Slider("Area Error", settings.secondaryUVAreaDistortion, 1, 75));
                }

                EditorGUI.indentLevel--;
            }

            GUILayout.Label("Normals & Tangents", EditorStyles.boldLabel);
            settings.normalImportMode =
                (ModelImporterTangentSpaceMode) EditorGUILayout.EnumPopup("Normals", settings.normalImportMode);

            EditorGUI.BeginDisabledGroup(settings.normalImportMode == ModelImporterTangentSpaceMode.None);

            settings.tangentImportMode =
                (ModelImporterTangentSpaceMode) EditorGUILayout.EnumPopup("Tangents", settings.tangentImportMode);

            bool allowSmootchingAngle = false;

            if (settings.normalImportMode == ModelImporterTangentSpaceMode.Calculate)
            {
                if (settings.tangentImportMode == ModelImporterTangentSpaceMode.Import)
                    settings.tangentImportMode = ModelImporterTangentSpaceMode.Calculate;
                if (settings.tangentImportMode == ModelImporterTangentSpaceMode.Calculate) allowSmootchingAngle = true;
            }
            else if (settings.normalImportMode == ModelImporterTangentSpaceMode.None)
                settings.tangentImportMode = ModelImporterTangentSpaceMode.None;

            EditorGUI.BeginDisabledGroup(!allowSmootchingAngle);

            settings.normalSmoothingAngle =
                Mathf.Round(EditorGUILayout.Slider("Smootching Angle", settings.normalSmoothingAngle, 0, 180));

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(settings.tangentImportMode == ModelImporterTangentSpaceMode.None);

            settings.splitTangentsAcrossSeams = EditorGUILayout.Toggle("Split Tangents",
                settings.splitTangentsAcrossSeams);

            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();

            GUILayout.Label("Materials", EditorStyles.boldLabel);

            settings.importMaterials = EditorGUILayout.Toggle("Import Materials", settings.importMaterials);
            if (settings.importMaterials)
            {
                settings.materialName =
                    (ModelImporterMaterialName)
                        EditorGUILayout.Popup("Material Naming", (int) settings.materialName,
                            new[] {"By Base Texture Name", "From Models Material", "Model Name + Models Material"});
                settings.materialSearch =
                    (ModelImporterMaterialSearch)
                        EditorGUILayout.Popup("Material Search", (int) settings.materialSearch,
                            new[] {"Local Materials Folder", "Recursive-Up", "Project-Wide"});
            }

            GUILayout.Label("Rig", EditorStyles.boldLabel);
            settings.animationType =
                (ModelImporterAnimationType) EditorGUILayout.EnumPopup("Animation Type", settings.animationType);

            GUILayout.Label("Animations", EditorStyles.boldLabel);
            settings.importAnimation = EditorGUILayout.Toggle("Import Animations", settings.importAnimation);
        }

        public override void GetSettingsFromImporter(AssetImporter modelImporter)
        {
            PropsToFields(modelImporter, settings);
        }

        public override void Load(XmlNode node)
        {
            FieldInfo[] props = typeof (ModelImporterSettings).GetFields();
            LoadSerialized(node, settings, props);
        }

        public override void Save(XmlElement element)
        {
            FieldInfo[] props = typeof (ModelImporterSettings).GetFields();
            SaveSerialized(element, settings, props);
        }

        public override void SetSettingsToImporter(AssetImporter modelImporter, string assetPath)
        {
            FieldsToProps(settings, modelImporter);
        }
    }
}