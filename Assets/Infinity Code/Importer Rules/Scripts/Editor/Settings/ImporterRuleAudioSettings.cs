/*     INFINITY CODE 2013-2016      */
/*   http://www.infinity-code.com   */

#if UNITY_4_5 || UNITY_4_6

using System.Xml;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.ImporterRules
{
    public class ImporterRuleAudioSettings : ImporterRuleBaseSettings
    {
        public bool threeD = true;
        public int compressionBitrate = 156;
        public bool forceToMono;
        public AudioImporterFormat format = AudioImporterFormat.Compressed;
        public bool hardware;
        public AudioImporterLoadType loadType = AudioImporterLoadType.CompressedInMemory;
        public bool loopable;

        public override void DrawEditor()
        {
            format = (AudioImporterFormat)(EditorGUILayout.Popup("Audio Format", (int)format + 1, new[] { "Native", "Compressed" }) - 1);
            threeD = EditorGUILayout.Toggle("3D Sound", threeD);
            forceToMono = EditorGUILayout.Toggle("Force to mono", forceToMono);

            if (format == AudioImporterFormat.Compressed)
            {
                loadType = (AudioImporterLoadType)EditorGUILayout.Popup("Load type", (int)loadType, new[] { "Decompress on load", "Compressed in memory", "Stream from disc" });
            }
            else
            {
                int curType = Mathf.Clamp((int)loadType - 1, 0, 1);
                loadType = (AudioImporterLoadType)(EditorGUILayout.Popup("Load type", curType, new[] { "Load into memory", "Stream from disc" }) + 1);
            }

            hardware = EditorGUILayout.Toggle("Hardware decoding", hardware);
            loopable = EditorGUILayout.Toggle("Gapless looping", loopable);

            compressionBitrate = EditorGUILayout.IntSlider("Compression (kbps)", compressionBitrate, 45, 500);
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

        public override void SetSettingsToImporter(AssetImporter importer, string assetPath)
        {
            FieldsToProps(this, importer);
        }
    }
}

#endif