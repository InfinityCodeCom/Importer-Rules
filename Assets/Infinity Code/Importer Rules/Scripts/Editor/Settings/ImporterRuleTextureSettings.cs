using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.ImporterRules
{
    [Serializable]
    public class ImporterRuleTextureSettings : ImporterRuleBaseSettings
    {
        private enum AdvancedTextureType
        {
            Default,
            NormalMap,
            LightMap
        };

        private enum CookieMode
        {
            Spotlight,
            Directional,
            Point
        };

        public TextureImporterSettings settings;
        public TextureImporterType textureType = TextureImporterType.Image;

        private static readonly GUIContent[] textureTypeOptions =
        {
            TextContent("Texture"), TextContent("Normal map"),
            TextContent("GUI (Editor \\ Legacy)"), TextContent("Sprite (2d \\ uGUI)"),
            TextContent("Cursor"), TextContent("Reflection"),
            TextContent("Cookie"), TextContent("Lightmap"),
            TextContent("Advanced")
        };

        private static readonly int[] textureTypeValues = {0, 1, 2, 8, 7, 3, 4, 6, 5};
        private static readonly GUIContent textureTypeLabel = new GUIContent("Texture Type");
        private static readonly string[] bumpFilteringOptions = {"Sharp", "Smooth"};

        private static readonly string[] refMapOptions =
        {
            "Sphere mapped", "Cylindrical", "Simple Sphere", "Nice Sphere",
            "6 Frames Layout"
        };

        private static readonly string[] spriteAlignmentOptions =
        {
            "Center", "TopLeft", "Top", "TopRight", "Left",
            "Right", "BottomLeft", "Bottom", "BottomRight", "Custom"
        };

        private static readonly string[] spriteModeOptions = {"Single", "Multiple"};

        private static readonly string[] kMaxTextureSizeStrings =
        {
            "32", "64", "128", "256", "512", "1024", "2048",
            "4096"
        };

        private static readonly int[] kMaxTextureSizeValues = {32, 64, 128, 256, 512, 1024, 2048, 4096};

        private static readonly int[] texFormatValues =
        {
            (int) TextureImporterFormat.AutomaticCompressed,
            (int) TextureImporterFormat.DXT1,
            (int) TextureImporterFormat.DXT5,
            (int) TextureImporterFormat.ETC_RGB4,
            (int) TextureImporterFormat.PVRTC_RGB2,
            (int) TextureImporterFormat.PVRTC_RGBA2,
            (int) TextureImporterFormat.PVRTC_RGB4,
            (int) TextureImporterFormat.PVRTC_RGBA4,
            (int) TextureImporterFormat.ATC_RGB4,
            (int) TextureImporterFormat.ATC_RGBA8,
            (int) TextureImporterFormat.ETC2_RGB4,
            (int) TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA,
            (int) TextureImporterFormat.ETC2_RGBA8,
            (int) TextureImporterFormat.ASTC_RGB_4x4,
            (int) TextureImporterFormat.ASTC_RGB_5x5,
            (int) TextureImporterFormat.ASTC_RGB_6x6,
            (int) TextureImporterFormat.ASTC_RGB_8x8,
            (int) TextureImporterFormat.ASTC_RGB_10x10,
            (int) TextureImporterFormat.ASTC_RGB_12x12,
            (int) TextureImporterFormat.ASTC_RGBA_4x4,
            (int) TextureImporterFormat.ASTC_RGBA_5x5,
            (int) TextureImporterFormat.ASTC_RGBA_6x6,
            (int) TextureImporterFormat.ASTC_RGBA_8x8,
            (int) TextureImporterFormat.ASTC_RGBA_10x10,
            (int) TextureImporterFormat.ASTC_RGBA_12x12,
            (int) TextureImporterFormat.Automatic16bit,
            (int) TextureImporterFormat.RGB16,
            (int) TextureImporterFormat.ARGB16,
            (int) TextureImporterFormat.RGBA16,
            (int) TextureImporterFormat.AutomaticTruecolor,
            (int) TextureImporterFormat.RGB24,
            (int) TextureImporterFormat.Alpha8,
            (int) TextureImporterFormat.ARGB32,
            (int) TextureImporterFormat.RGBA32
        };

        public ImporterRuleTextureSettings()
        {
            settings = new TextureImporterSettings
            {
                maxTextureSize = 1024,
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Bilinear,
                textureFormat = TextureImporterFormat.AutomaticCompressed,
                heightmapScale = 0.25f,
                aniso = 1,
                spriteAlignment = 0,
                spriteMode = 1,
                spritePixelsToUnits = 100,
                generateCubemap = TextureImporterGenerateCubemap.FullCubemap
            };
        }

        private static string[] BuildTextureStrings(int[] textureFormatValues)
        {
            string[] strArray = new string[textureFormatValues.Length];
            for (int i = 0; i < textureFormatValues.Length; i++)
                strArray[i] = GetTextureFormatString((TextureImporterFormat) textureFormatValues[i]);
            return strArray;
        }

        public override void DrawEditor()
        {
            int newTextureImporterType = EditorGUILayout.IntPopup(textureTypeLabel, (int) textureType,
                textureTypeOptions, textureTypeValues);

            if (newTextureImporterType != (int) textureType)
            {
                textureType = (TextureImporterType) newTextureImporterType;
                settings.ApplyTextureType(textureType, true);
            }

            switch (textureType)
            {
                case TextureImporterType.Image:
                    OnImageGUI();
                    break;
                case TextureImporterType.Bump:
                    OnBumpGUI();
                    break;
                case TextureImporterType.Sprite:
                    OnSpriteGUI();
                    break;
                case TextureImporterType.Reflection:
                    OnReflectionGUI();
                    break;
                case TextureImporterType.Cookie:
                    OnCookieGUI();
                    break;
                case TextureImporterType.Advanced:
                    OnAdvancedGUI();
                    break;
            }

            EditorGUILayout.Space();

            if (textureType != TextureImporterType.GUI && textureType != TextureImporterType.Sprite &&
                textureType != TextureImporterType.Reflection && textureType != TextureImporterType.Cookie &&
                textureType != TextureImporterType.Lightmap)
            {
                settings.wrapMode = (TextureWrapMode) EditorGUILayout.EnumPopup("Wrap Mode", settings.wrapMode);
                settings.filterMode = (FilterMode) EditorGUILayout.EnumPopup("Filter Mode", settings.filterMode);
                if (settings.filterMode != FilterMode.Point && textureType != TextureImporterType.Cursor)
                {
                    settings.aniso = EditorGUILayout.IntSlider("Aniso Level", settings.aniso, 0, 9);
                }
            }
            else if (textureType == TextureImporterType.GUI || textureType == TextureImporterType.Sprite)
            {
                settings.filterMode = (FilterMode) EditorGUILayout.EnumPopup("Filter Mode", settings.filterMode);
            }
            else if (textureType == TextureImporterType.Reflection || textureType == TextureImporterType.Cookie ||
                     textureType == TextureImporterType.Lightmap)
            {
                settings.filterMode = (FilterMode) EditorGUILayout.EnumPopup("Filter Mode", settings.filterMode);
                if (settings.filterMode != FilterMode.Point)
                {
                    settings.aniso = EditorGUILayout.IntSlider("Aniso Level", settings.aniso, 0, 9);
                }
            }

            EditorGUILayout.Space();

            settings.maxTextureSize = EditorGUILayout.IntPopup("Max Size", settings.maxTextureSize,
                kMaxTextureSizeStrings, kMaxTextureSizeValues);

            if (textureType == TextureImporterType.Cookie)
            {
                EditorGUI.BeginDisabledGroup(true);
                settings.textureFormat = TextureImporterFormat.Alpha8;
                EditorGUILayout.IntPopup("Format", 0, new[] {"8 bit Alpha"}, new[] {0});
                EditorGUI.EndDisabledGroup();
            }
            else if (textureType == TextureImporterType.Advanced)
            {
                if (!texFormatValues.Contains((int) settings.textureFormat))
                    settings.textureFormat = TextureImporterFormat.AutomaticCompressed;
                settings.textureFormat =
                    (TextureImporterFormat)
                        EditorGUILayout.IntPopup("Format", (int) settings.textureFormat,
                            BuildTextureStrings(texFormatValues), texFormatValues);
            }
            else
            {
                int[] textureFormats =
                {
                    (int) TextureImporterFormat.AutomaticCompressed,
                    (int) TextureImporterFormat.Automatic16bit,
                    (int) TextureImporterFormat.AutomaticTruecolor
                };

                string[] testureFormatStrings =
                {
                    "Compressed", "16 bits", "Truecolor"
                };

                if (!textureFormats.Contains((int) settings.textureFormat))
                    settings.textureFormat = TextureImporterFormat.AutomaticCompressed;
                settings.textureFormat =
                    (TextureImporterFormat)
                        EditorGUILayout.IntPopup("Format", (int) settings.textureFormat, testureFormatStrings,
                            textureFormats);
            }
        }

        public override void GetSettingsFromImporter(AssetImporter importer)
        {
            TextureImporter textureImporter = importer as TextureImporter;
            textureImporter.ReadTextureSettings(settings);
            textureType = textureImporter.textureType;
        }

        private static string GetTextureFormatString(TextureImporterFormat tf)
        {
            if (tf == TextureImporterFormat.ARGB16) return "ARGB 16 bit";
            if (tf == TextureImporterFormat.ARGB32) return "ARGB 32 bit";
            if (tf == TextureImporterFormat.ASTC_RGBA_10x10) return "RGBA Compressed ASTC 10x10 block";
            if (tf == TextureImporterFormat.ASTC_RGBA_12x12) return "RGBA Compressed ASTC 12x12 block";
            if (tf == TextureImporterFormat.ASTC_RGBA_4x4) return "RGBA Compressed ASTC 4x4 block";
            if (tf == TextureImporterFormat.ASTC_RGBA_5x5) return "RGBA Compressed ASTC 5x5 block";
            if (tf == TextureImporterFormat.ASTC_RGBA_6x6) return "RGBA Compressed ASTC 6x6 block";
            if (tf == TextureImporterFormat.ASTC_RGBA_8x8) return "RGBA Compressed ASTC 8x8 block";
            if (tf == TextureImporterFormat.ASTC_RGB_10x10) return "RGB Compressed ASTC 10x10 block";
            if (tf == TextureImporterFormat.ASTC_RGB_12x12) return "RGB Compressed ASTC 12x12 block";
            if (tf == TextureImporterFormat.ASTC_RGB_4x4) return "RGB Compressed ASTC 4x4 block";
            if (tf == TextureImporterFormat.ASTC_RGB_5x5) return "RGB Compressed ASTC 5x5 block";
            if (tf == TextureImporterFormat.ASTC_RGB_6x6) return "RGB Compressed ASTC 6x6 block";
            if (tf == TextureImporterFormat.ASTC_RGB_8x8) return "RGB Compressed ASTC 8x8 block";
            if (tf == TextureImporterFormat.ATC_RGB4) return "RGB Compressed ATC 4 bits";
            if (tf == TextureImporterFormat.ATC_RGBA8) return "RGBA Compressed ATC 8 bits";
            if (tf == TextureImporterFormat.Alpha8) return "Alpha 8 bits";
            if (tf == TextureImporterFormat.DXT1) return "RGB Compressed DXT1";
            if (tf == TextureImporterFormat.DXT5) return "RGBA Compressed DXT5";
            if (tf == TextureImporterFormat.ETC2_RGB4) return "RGB Compressed ETC2 4 bits";
            if (tf == TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA)
                return "RGB + 1 bit alpha Compressed ETC2 4 bits";
            if (tf == TextureImporterFormat.ETC2_RGBA8) return "RGBA Compressed ETC2 8 bits";
            if (tf == TextureImporterFormat.ETC_RGB4) return "RGB Compressed ETC 4 bits";
            if (tf == TextureImporterFormat.PVRTC_RGB2) return "RGB Compressed PVRTC 2 bits";
            if (tf == TextureImporterFormat.PVRTC_RGB4) return "RGB Compressed PVRTC 4 bits";
            if (tf == TextureImporterFormat.PVRTC_RGBA2) return "RGBA Compressed PVRTC 2 bits";
            if (tf == TextureImporterFormat.PVRTC_RGBA4) return "RGBA Compressed PVRTC 4 bits";
            if (tf == TextureImporterFormat.RGB16) return "RGB 16 bit";
            if (tf == TextureImporterFormat.RGB24) return "RGB 24 bit";
            if (tf == TextureImporterFormat.RGBA16) return "RGBA 16 bit";
            if (tf == TextureImporterFormat.RGBA32) return "RGBA 32 bit";

            if (tf == TextureImporterFormat.Automatic16bit) return "Automatic 16 bits";
            if (tf == TextureImporterFormat.AutomaticCompressed) return "Automatic Compressed";
            if (tf == TextureImporterFormat.AutomaticTruecolor) return "Automatic Truecolor";

            return "Unknown";
        }

        public override void Load(XmlNode node)
        {
            string typeStr = "";
            if (node.TryGetValue("TextureType", ref typeStr))
            {
                textureType = (TextureImporterType) Enum.Parse(typeof (TextureImporterType), typeStr);
                settings.ApplyTextureType(textureType, true);
            }

            PropertyInfo[] props = typeof (TextureImporterSettings).GetProperties();
            LoadSerialized(node, settings, props);
        }

        private void OnAdvancedGUI()
        {
            settings.npotScale =
                (TextureImporterNPOTScale) EditorGUILayout.EnumPopup("Non Power of 2", settings.npotScale);
            settings.generateCubemap =
                (TextureImporterGenerateCubemap) EditorGUILayout.EnumPopup("Generate Cubemap", settings.generateCubemap);
            settings.readable = EditorGUILayout.Toggle("Read/Write Enabled", settings.readable);

            AdvancedTextureType normalMap = AdvancedTextureType.Default;
            if (settings.normalMap) normalMap = AdvancedTextureType.NormalMap;
            else if (settings.lightmap) normalMap = AdvancedTextureType.LightMap;

            string[] displayedOptions = {"Default", "Normal Map", "Lightmap"};
            normalMap = (AdvancedTextureType) EditorGUILayout.Popup("Import Type", (int) normalMap, displayedOptions);
            switch (normalMap)
            {
                case AdvancedTextureType.Default:
                    settings.normalMap = false;
                    settings.lightmap = false;
                    settings.convertToNormalMap = false;
                    break;

                case AdvancedTextureType.NormalMap:
                    settings.normalMap = true;
                    settings.lightmap = false;
                    settings.linearTexture = true;
                    break;

                case AdvancedTextureType.LightMap:
                    settings.normalMap = false;
                    settings.lightmap = true;
                    settings.convertToNormalMap = false;
                    settings.linearTexture = true;
                    break;
            }
            EditorGUI.indentLevel++;
            switch (normalMap)
            {
                case AdvancedTextureType.NormalMap:
                    OnBumpGUI();
                    break;
                case AdvancedTextureType.Default:
                    OnImageGUI();
                    settings.linearTexture = EditorGUILayout.Toggle("Bypass sRGB Sampling", settings.linearTexture);
                    OnSpriteGUI();
                    break;
            }
            EditorGUI.indentLevel--;

            settings.mipmapEnabled = EditorGUILayout.Toggle("Generate Mip Maps", settings.mipmapEnabled);
            if (settings.mipmapEnabled)
            {
                EditorGUI.indentLevel++;
                settings.generateMipsInLinearSpace = EditorGUILayout.Toggle("In Linear Space",
                    settings.generateMipsInLinearSpace);
                settings.borderMipmap = EditorGUILayout.Toggle("Border Mip Maps", settings.borderMipmap);
                settings.mipmapFilter =
                    (TextureImporterMipFilter)
                        EditorGUILayout.IntPopup("Mip Map Filtering", (int) settings.mipmapFilter,
                            new[] {"Box", "Kaiser"},
                            new[]
                            {(int) TextureImporterMipFilter.BoxFilter, (int) TextureImporterMipFilter.KaiserFilter});
                settings.fadeOut = EditorGUILayout.Toggle("Fadeout Mip Maps", settings.fadeOut);

                if (settings.fadeOut)
                {
                    EditorGUI.indentLevel++;

                    float minValue = settings.mipmapFadeDistanceStart;
                    float maxValue = settings.mipmapFadeDistanceEnd;
                    EditorGUILayout.MinMaxSlider(new GUIContent("Fade Range"), ref minValue, ref maxValue, 0f, 10f);
                    settings.mipmapFadeDistanceStart = Mathf.RoundToInt(minValue);
                    settings.mipmapFadeDistanceEnd = Mathf.RoundToInt(maxValue);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
        }

        private void OnBumpGUI()
        {
            settings.convertToNormalMap = EditorGUILayout.Toggle("Create from Grayscale", settings.convertToNormalMap);
            if (settings.convertToNormalMap)
            {
                settings.heightmapScale = EditorGUILayout.Slider("Bumpiness", settings.heightmapScale, 0, 0.3f);
                settings.normalMapFilter =
                    (TextureImporterNormalFilter)
                        EditorGUILayout.Popup("Filtering", (int) settings.normalMapFilter, bumpFilteringOptions);
            }
        }

        private void OnCookieGUI()
        {
            CookieMode spot;
            if (settings.borderMipmap) spot = CookieMode.Spotlight;
            else if ((int) settings.generateCubemap != 0) spot = CookieMode.Point;
            else spot = CookieMode.Directional;

            spot = (CookieMode) EditorGUILayout.EnumPopup("Light Type", spot);
            SetCookieMode(spot);

            if (spot == CookieMode.Point) OnReflectionGUI();

            settings.grayscaleToAlpha = EditorGUILayout.Toggle("Alpha from Grayscale", settings.grayscaleToAlpha);
        }

        private void OnImageGUI()
        {
            settings.grayscaleToAlpha = EditorGUILayout.Toggle("Alpha from Grayscale", settings.grayscaleToAlpha);
            if (settings.grayscaleToAlpha)
                settings.alphaIsTransparency = EditorGUILayout.Toggle("Alpha is Transparency",
                    settings.alphaIsTransparency);
        }

        private void OnReflectionGUI()
        {
            settings.generateCubemap =
                (TextureImporterGenerateCubemap)
                    (EditorGUILayout.Popup("Mapping", (int) settings.generateCubemap - 1, refMapOptions) + 1);
            settings.seamlessCubemap = EditorGUILayout.Toggle("Fixup Edge Seams", settings.seamlessCubemap);
        }

        private void OnSpriteGUI()
        {
            int[] optionValues = {1, 2};
            settings.spriteMode = EditorGUILayout.IntPopup("Sprite Mode", settings.spriteMode, spriteModeOptions,
                optionValues);
            settings.spritePixelsToUnits = EditorGUILayout.FloatField("Pixels to Units", settings.spritePixelsToUnits);

            if (textureType == TextureImporterType.Advanced)
            {
                settings.spriteMeshType =
                    (SpriteMeshType) EditorGUILayout.EnumPopup("Mesh Type", settings.spriteMeshType);
                settings.spriteExtrude =
                    (uint) EditorGUILayout.IntSlider("Extrude Edges", (int) settings.spriteExtrude, 0, 32);
            }

            if (settings.spriteMode == 1)
                settings.spriteAlignment = EditorGUILayout.Popup("Pivot", settings.spriteAlignment,
                    spriteAlignmentOptions);
            if (settings.spriteAlignment == 9)
                settings.spritePivot = EditorGUILayout.Vector2Field("", settings.spritePivot);
        }

        public override void Save(XmlElement element)
        {
            element.CreateChild("TextureType", textureType.ToString());
            PropertyInfo[] props = typeof (TextureImporterSettings).GetProperties();
            SaveSerialized(element, settings, props);
        }

        public override void SetSettingsToImporter(AssetImporter importer)
        {
            TextureImporter textureImporter = importer as TextureImporter;
            textureImporter.SetTextureSettings(settings);
        }

        private void SetCookieMode(CookieMode cm)
        {
            switch (cm)
            {
                case CookieMode.Spotlight:
                    settings.borderMipmap = true;
                    settings.wrapMode = (TextureWrapMode) 1;
                    settings.generateCubemap = 0;
                    break;

                case CookieMode.Directional:
                    settings.borderMipmap = false;
                    settings.wrapMode = 0;
                    settings.generateCubemap = 0;
                    break;

                case CookieMode.Point:
                    settings.borderMipmap = false;
                    settings.wrapMode = (TextureWrapMode) 1;
                    settings.generateCubemap = (TextureImporterGenerateCubemap) 3;
                    break;
            }
        }
    }
}