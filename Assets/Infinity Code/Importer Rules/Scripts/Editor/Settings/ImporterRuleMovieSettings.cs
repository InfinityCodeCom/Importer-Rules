/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System.Xml;
using UnityEditor;

namespace InfinityCode.ImporterRules
{
    [System.Serializable]
    public class ImporterRuleMovieSettings : ImporterRuleBaseSettings
    {
        private bool linearTexture;
        private float quality = 0.5f;

        public override void DrawEditor()
        {
            linearTexture = EditorGUILayout.Toggle("Bypass sRGB Sampling", linearTexture);
            quality = EditorGUILayout.Slider("Quality", quality, 0f, 1f);
        }

        public override void GetSettingsFromImporter(AssetImporter importer)
        {
            MovieImporter movieImporter = importer as MovieImporter;
            linearTexture = movieImporter.linearTexture;
            quality = movieImporter.quality;
        }

        public override void Load(XmlNode node)
        {
            node.TryGetValue("Quality", ref quality);
            node.TryGetValue("LinearTexture", ref linearTexture);
        }

        public override void Save(XmlElement element)
        {
            element.CreateChild("Quality", quality);
            element.CreateChild("LinearTexture", linearTexture);
        }

        public override void SetSettingsToImporter(AssetImporter importer, string assetPath)
        {
            MovieImporter movieImporter = importer as MovieImporter;
            movieImporter.linearTexture = linearTexture;
            movieImporter.quality = quality;
        }
    }
}