/*     INFINITY CODE 2013-2016      */
/*   http://www.infinity-code.com   */

#if UNITY_5

using System;
using System.Xml;
using UnityEditor;
using UnityEngine;


namespace InfinityCode.ImporterRules
{
    [Serializable]
    public class ImporterRuleAudioSettingsUFive : ImporterRuleBaseSettings
    {
        public bool forceToMono;
        public bool loadInBackground;
        public bool preloadAudioData = true;
        public AudioClipLoadType loadType = AudioClipLoadType.DecompressOnLoad;
        public float quality = 1;
        public AudioSampleRateSetting sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;

        public override void DrawEditor()
        {
            forceToMono = EditorGUILayout.Toggle("Force to mono", forceToMono);
            loadInBackground = EditorGUILayout.Toggle("Load In Background", loadInBackground);
            preloadAudioData = EditorGUILayout.Toggle("Preload Audio Data", preloadAudioData);

            loadType = (AudioClipLoadType) EditorGUILayout.EnumPopup("Load Type", loadType);
            quality = EditorGUILayout.Slider("Quality", quality, 0, 1);
            sampleRateSetting = (AudioSampleRateSetting)EditorGUILayout.EnumPopup("Sample Rate Settings", sampleRateSetting);
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
            AudioImporter audioImporter = importer as AudioImporter;
            audioImporter.forceToMono = forceToMono;
            audioImporter.loadInBackground = loadInBackground;
            audioImporter.preloadAudioData = preloadAudioData;
            AudioImporterSampleSettings sampleSettings = audioImporter.defaultSampleSettings;
            sampleSettings.loadType = loadType;
            sampleSettings.quality = quality;
            sampleSettings.sampleRateSetting = sampleRateSetting;
            audioImporter.defaultSampleSettings = sampleSettings;
            //audioImporter.SaveAndReimport();
        }
    }
}


#endif