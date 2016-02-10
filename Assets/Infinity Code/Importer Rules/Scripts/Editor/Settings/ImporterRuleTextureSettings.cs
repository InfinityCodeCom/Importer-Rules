/*     INFINITY CODE 2013-2016      */
/*   http://www.infinity-code.com   */

#if UNITY_4_5 || UNITY_4_6
#define UNITY_4_6L
#endif

#if !UNITY_4_6L && !UNITY_4_7
#define UNITY_5P
#endif

#if !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
#define UNITY_5_3P
#endif

using System;
using System.Collections.Generic;
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
            TextContent("Texture"), 
            TextContent("Normal map"),
            TextContent("GUI (Editor \\ Legacy)"), 
            TextContent("Sprite (2d \\ uGUI)"),
            TextContent("Cursor"), 
#if UNITY_4_6L
            TextContent("Reflection"),
#else
            TextContent("Cubemap"),
#endif
            TextContent("Cookie"), 
            TextContent("Lightmap"),
            TextContent("Advanced")
        };

        private static readonly int[] textureTypeValues = {0, 1, 2, 8, 7, 3, 4, 6, 5};
        private static readonly GUIContent textureTypeLabel = new GUIContent("Texture Type");
        private static readonly string[] bumpFilteringOptions = {"Sharp", "Smooth"};

#if UNITY_4_6L
        private static readonly string[] refMapOptions =
        {
            "Sphere mapped", "Cylindrical", "Simple Sphere", "Nice Sphere", "6 Frames Layout"
        };
#endif

        private static readonly string[] spriteAlignmentOptions =
        {
            "Center", "TopLeft", "Top", "TopRight", "Left",
            "Right", "BottomLeft", "Bottom", "BottomRight", "Custom"
        };

        private static readonly string[] kMaxTextureSizeStrings =
        {
            "32", "64", "128", "256", "512", "1024", "2048", "4096"
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
            (int) TextureImporterFormat.RGBA32,

#if UNITY_5_3P
            (int) TextureImporterFormat.AutomaticCrunched,
            (int) TextureImporterFormat.DXT1Crunched,
            (int) TextureImporterFormat.DXT5Crunched,
#endif
        };

        private static readonly TextureImporterFormat[] kFormatsWithCompressionSettings = 
        {
            TextureImporterFormat.PVRTC_RGB2,
            TextureImporterFormat.PVRTC_RGB4,
            TextureImporterFormat.PVRTC_RGBA2,
            TextureImporterFormat.PVRTC_RGBA4,
            TextureImporterFormat.ATC_RGB4,
            TextureImporterFormat.ATC_RGBA8,
            TextureImporterFormat.ETC_RGB4,
            TextureImporterFormat.ETC2_RGB4,
            TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA,
            TextureImporterFormat.ETC2_RGBA8,
            TextureImporterFormat.ASTC_RGB_4x4,
            TextureImporterFormat.ASTC_RGB_5x5,
            TextureImporterFormat.ASTC_RGB_6x6,
            TextureImporterFormat.ASTC_RGB_8x8,
            TextureImporterFormat.ASTC_RGB_10x10,
            TextureImporterFormat.ASTC_RGB_12x12,
            TextureImporterFormat.ASTC_RGBA_4x4,
            TextureImporterFormat.ASTC_RGBA_5x5,
            TextureImporterFormat.ASTC_RGBA_6x6,
            TextureImporterFormat.ASTC_RGBA_8x8,
            TextureImporterFormat.ASTC_RGBA_10x10,
            TextureImporterFormat.ASTC_RGBA_12x12,
#if UNITY_5_3P
            TextureImporterFormat.AutomaticCrunched,
            TextureImporterFormat.DXT1Crunched,
            TextureImporterFormat.DXT5Crunched,
#endif
        };

        private bool useOriginalSize = false;

#if UNITY_4_6L
        public const TextureImporterType reflectionType = TextureImporterType.Reflection;
#else
        public const TextureImporterType reflectionType = TextureImporterType.Cubemap;
#endif

        public ImporterRuleTextureSettings()
        {
            InitSettings();
        }

        private static string[] BuildTextureStrings(int[] textureFormatValues)
        {
            string[] strArray = new string[textureFormatValues.Length];
            for (int i = 0; i < textureFormatValues.Length; i++) strArray[i] = GetTextureFormatString((TextureImporterFormat) textureFormatValues[i]);
            return strArray;
        }

        public override void DrawEditor()
        {
            int newTextureImporterType = EditorGUILayout.IntPopup(textureTypeLabel, (int) textureType,
                textureTypeOptions, textureTypeValues);

            if (newTextureImporterType != (int) textureType)
            {
                textureType = (TextureImporterType) newTextureImporterType;
                InitSettings(settings.maxTextureSize, settings.textureFormat);
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
                case reflectionType:
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
                textureType != reflectionType && textureType != TextureImporterType.Cookie &&
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
            else if (textureType == reflectionType || textureType == TextureImporterType.Cookie ||
                     textureType == TextureImporterType.Lightmap)
            {
                settings.filterMode = (FilterMode) EditorGUILayout.EnumPopup("Filter Mode", settings.filterMode);
                if (settings.filterMode != FilterMode.Point)
                {
                    settings.aniso = EditorGUILayout.IntSlider("Aniso Level", settings.aniso, 0, 9);
                }
            }

            EditorGUILayout.Space();

            settings.maxTextureSize = EditorGUILayout.IntPopup("Max Size", settings.maxTextureSize, kMaxTextureSizeStrings, kMaxTextureSizeValues);

            useOriginalSize = EditorGUILayout.Toggle("Use Original image size", useOriginalSize);

            if (textureType == TextureImporterType.Cookie)
            {
                EditorGUI.BeginDisabledGroup(true);
                settings.textureFormat = TextureImporterFormat.Alpha8;
                EditorGUILayout.IntPopup("Format", 0, new[] {"8 bit Alpha"}, new[] {0});
                EditorGUI.EndDisabledGroup();
            }
            else if (textureType == TextureImporterType.Advanced)
            {
                if (!texFormatValues.Contains((int) settings.textureFormat)) settings.textureFormat = TextureImporterFormat.AutomaticCompressed;
                settings.textureFormat = (TextureImporterFormat) EditorGUILayout.IntPopup("Format", (int) settings.textureFormat, BuildTextureStrings(texFormatValues), texFormatValues);
            }
            else
            {
                int[] textureFormats =
                {
                    (int) TextureImporterFormat.AutomaticCompressed,
                    (int) TextureImporterFormat.Automatic16bit,
                    (int) TextureImporterFormat.AutomaticTruecolor,
#if UNITY_5_3P
                    (int) TextureImporterFormat.AutomaticCrunched,
#endif
                };

                string[] testureFormatStrings =
                {
                    "Compressed",
                    "16 bits",
                    "Truecolor",
#if UNITY_5_3P
                    "Crunched",
#endif
                };

                if (!textureFormats.Contains((int) settings.textureFormat)) settings.textureFormat = TextureImporterFormat.AutomaticCompressed;
                settings.textureFormat = (TextureImporterFormat) EditorGUILayout.IntPopup("Format", (int) settings.textureFormat, testureFormatStrings, textureFormats);
            }

            if (ArrayUtility.Contains(kFormatsWithCompressionSettings, settings.textureFormat)) settings.compressionQuality = EditorGUILayout.IntSlider("Compression Quality", settings.compressionQuality, 0, 100);
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
            if (tf == TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA) return "RGB + 1 bit alpha Compressed ETC2 4 bits";
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

#if UNITY_5_3P
            if (tf == TextureImporterFormat.AutomaticCrunched) return "Automatic Crunched";
            if (tf == TextureImporterFormat.DXT1Crunched) return "RGB Crunched DXT1";
            if (tf == TextureImporterFormat.DXT5Crunched) return "RGBA Crunched DXT5";
#endif

            return "Unknown";
        }

        private void InitSettings(int maxTextureSize = 1024, TextureImporterFormat textureFormat = TextureImporterFormat.AutomaticCompressed)
        {
            settings = new TextureImporterSettings
            {
                maxTextureSize = maxTextureSize,
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Bilinear,
                textureFormat = textureFormat,
                heightmapScale = 0.25f,
                aniso = 1,
                spriteAlignment = 0,
#if UNITY_4_5
                spritePixelsToUnits = 100,
#else
                spritePixelsPerUnit = 100
#endif
            };

            settings.mipmapEnabled = textureType == TextureImporterType.Image || textureType == TextureImporterType.Advanced || textureType == reflectionType;

            settings.linearTexture = textureType == TextureImporterType.Bump || textureType == TextureImporterType.GUI;
            settings.npotScale = (textureType == TextureImporterType.GUI || textureType == TextureImporterType.Sprite) ? TextureImporterNPOTScale.None: TextureImporterNPOTScale.ToNearest;
            settings.spriteMode = (textureType == TextureImporterType.Sprite) ? 1 : 0;
            settings.readable = textureType == TextureImporterType.Cursor;
            settings.generateCubemap = (textureType == reflectionType)? TextureImporterGenerateCubemap.FullCubemap: TextureImporterGenerateCubemap.None;

            settings.ApplyTextureType(textureType, true);
        }

        public override void Load(XmlNode node)
        {
            string typeStr = "";
            if (node.TryGetValue("TextureType", ref typeStr)) textureType = (TextureImporterType) Enum.Parse(typeof (TextureImporterType), typeStr);

            node.TryGetValue("UseOriginalSize", ref useOriginalSize);

            LoadSerialized(node, settings, typeof (TextureImporterSettings).GetProperties());

            settings.ApplyTextureType(textureType, true);
        }

        private void OnAdvancedGUI()
        {
            settings.npotScale = (TextureImporterNPOTScale) EditorGUILayout.EnumPopup("Non Power of 2", settings.npotScale);

#if UNITY_4_6L
            settings.generateCubemap = (TextureImporterGenerateCubemap) EditorGUILayout.EnumPopup("Generate Cubemap", settings.generateCubemap);
#else
            string[] generateCubemapDisplayedOptions =
            {
                "None",
                "Auto", 
                "6 Frames Layout (Cubic Environment)", 
                "Latitude-Longitude Layout (Cylindrical)", 
                "Mirrored Ball (Spheremap)"
            };
            List<int> generateCubemapValues = new List<int>
            {
                (int)TextureImporterGenerateCubemap.None,
                (int)TextureImporterGenerateCubemap.AutoCubemap, 
                (int)TextureImporterGenerateCubemap.FullCubemap,
                (int)TextureImporterGenerateCubemap.Cylindrical, 
                (int)TextureImporterGenerateCubemap.Spheremap
            };
            int generateCubemap = generateCubemapValues.IndexOf((int)settings.generateCubemap);
            generateCubemap = EditorGUILayout.Popup("Mapping", generateCubemap, generateCubemapDisplayedOptions);
            settings.generateCubemap = (TextureImporterGenerateCubemap)generateCubemapValues[generateCubemap];

            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(settings.generateCubemap == TextureImporterGenerateCubemap.None);

#if UNITY_5P
            string[] cubemapConvolutionOptions =
            {
                "None",
                "Specular (Glossy Reflection)",
                "Diffuse (Irradians)"
            };

            List<int> cubemapConvolutionValues = new List<int>
            {
                (int) TextureImporterCubemapConvolution.None,
                (int) TextureImporterCubemapConvolution.Specular,
                (int) TextureImporterCubemapConvolution.Diffuse,
            };

            int cubemapConvolution = cubemapConvolutionValues.IndexOf((int)settings.cubemapConvolution);

            cubemapConvolution = EditorGUILayout.Popup("Convolution Type", cubemapConvolution, cubemapConvolutionOptions);
            settings.cubemapConvolution = (TextureImporterCubemapConvolution) cubemapConvolutionValues[cubemapConvolution];

            if (settings.cubemapConvolution == TextureImporterCubemapConvolution.Diffuse)
            {
                EditorGUI.indentLevel++;
                settings.cubemapConvolutionSteps = EditorGUILayout.IntField("Steps", settings.cubemapConvolutionSteps);
                settings.cubemapConvolutionExponent = EditorGUILayout.FloatField("Exponent", settings.cubemapConvolutionExponent);
                EditorGUI.indentLevel--;
            }
#endif

            settings.seamlessCubemap = EditorGUILayout.Toggle("Fixup Edge Seams", settings.seamlessCubemap);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;
#endif

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
#if UNITY_5P
                    settings.rgbm = (TextureImporterRGBMMode) EditorGUILayout.EnumPopup("Encode as RGBM", settings.rgbm);
#endif
                    OnSpriteGUI();
                    break;
            }
            EditorGUI.indentLevel--;

            settings.mipmapEnabled = EditorGUILayout.Toggle("Generate Mip Maps", settings.mipmapEnabled);
            if (settings.mipmapEnabled)
            {
                EditorGUI.indentLevel++;
                settings.generateMipsInLinearSpace = EditorGUILayout.Toggle("In Linear Space", settings.generateMipsInLinearSpace);
                settings.borderMipmap = EditorGUILayout.Toggle("Border Mip Maps", settings.borderMipmap);
                settings.mipmapFilter = (TextureImporterMipFilter) EditorGUILayout.IntPopup("Mip Map Filtering", (int) settings.mipmapFilter, new[] {"Box", "Kaiser"}, new[] {(int) TextureImporterMipFilter.BoxFilter, (int) TextureImporterMipFilter.KaiserFilter});
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
                settings.normalMapFilter = (TextureImporterNormalFilter)EditorGUILayout.Popup("Filtering", (int) settings.normalMapFilter, bumpFilteringOptions);
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
#if !UNITY_5_3P
            if (settings.grayscaleToAlpha) 
            {
#endif
                settings.alphaIsTransparency = EditorGUILayout.Toggle("Alpha is Transparency", settings.alphaIsTransparency);
#if !UNITY_5_3P
        }
#endif
        }

        private void OnReflectionGUI()
        {
#if UNITY_4_6L
            settings.generateCubemap = (TextureImporterGenerateCubemap) (EditorGUILayout.Popup("Mapping", (int) settings.generateCubemap - 1, refMapOptions) + 1);
#else
            string[] displayedOptions =
            {
                "Auto", 
                "6 Frames Layout (Cubic Environment)", 
                "Latitude-Longitude Layout (Cylindrical)", 
                "Mirrored Ball (Spheremap)"
            };
            List<int> values = new List<int>
            {
                (int)TextureImporterGenerateCubemap.AutoCubemap, 
                (int)TextureImporterGenerateCubemap.FullCubemap,
                (int)TextureImporterGenerateCubemap.Cylindrical, 
                (int)TextureImporterGenerateCubemap.Spheremap
            };
            int generateCubemap = values.IndexOf((int)settings.generateCubemap);
            generateCubemap = EditorGUILayout.Popup("Mapping", generateCubemap, displayedOptions);
            settings.generateCubemap = (TextureImporterGenerateCubemap)values[generateCubemap];
#endif
            settings.seamlessCubemap = EditorGUILayout.Toggle("Fixup Edge Seams", settings.seamlessCubemap);
        }

        private void OnSpriteGUI()
        {
            if (textureType == TextureImporterType.Advanced)
            {
                int[] optionValues =
                {
                    (int)SpriteImportMode.None,
                    (int)SpriteImportMode.Single,
                    (int)SpriteImportMode.Multiple,
#if UNITY_5_3P
                    (int)SpriteImportMode.Polygon,
#endif
                };

                string[] spriteModeOptions =
                {
                    "None",
                    "Single",
                    "Multiple",
#if UNITY_5_3P
                    "Polygon",
#endif
                };
                settings.spriteMode = EditorGUILayout.IntPopup("Sprite Mode", settings.spriteMode, spriteModeOptions,
                    optionValues);
            }
            else
            {
                int[] optionValues =
                {
                    (int)SpriteImportMode.Single,
                    (int)SpriteImportMode.Multiple,
#if UNITY_5_3P
                    (int)SpriteImportMode.Polygon,
#endif
                };

                string[] spriteModeOptions =
                {
                    "Single",
                    "Multiple",
#if UNITY_5_3P
                    "Polygon",
#endif
                };
                settings.spriteMode = EditorGUILayout.IntPopup("Sprite Mode", settings.spriteMode, spriteModeOptions, optionValues);
            }

            if (settings.spriteMode != 0)
            {
                EditorGUI.indentLevel++;

#if UNITY_4_5
                settings.spritePixelsToUnits = EditorGUILayout.FloatField("Pixels to Units", settings.spritePixelsToUnits);
#else
                settings.spritePixelsPerUnit = EditorGUILayout.FloatField("Pixels to Units", settings.spritePixelsPerUnit);
#endif

                if (textureType == TextureImporterType.Advanced)
                {
                    settings.spriteMeshType = (SpriteMeshType)EditorGUILayout.EnumPopup("Mesh Type", settings.spriteMeshType);
                    settings.spriteExtrude = (uint)EditorGUILayout.IntSlider("Extrude Edges", (int)settings.spriteExtrude, 0, 32);
                }

                if (settings.spriteMode == (int)SpriteImportMode.Single) settings.spriteAlignment = EditorGUILayout.Popup("Pivot", settings.spriteAlignment, spriteAlignmentOptions);
                if (settings.spriteAlignment == 9) settings.spritePivot = EditorGUILayout.Vector2Field("", settings.spritePivot);
                EditorGUI.indentLevel--;
            }
        }

        public override void Save(XmlElement element)
        {
            element.CreateChild("TextureType", textureType.ToString());
            element.CreateChild("UseOriginalSize", useOriginalSize);
            PropertyInfo[] props = typeof (TextureImporterSettings).GetProperties();
            SaveSerialized(element, settings, props);
        }

        public override void SetSettingsToImporter(AssetImporter importer, string assetPath)
        {
            TextureImporter textureImporter = importer as TextureImporter;
            textureImporter.SetTextureSettings(settings);
            textureImporter.textureType = textureType;

            if (useOriginalSize)
            {
                int width = 0, height = 0;
                if (ImporterRuleImageUtils.GetImageSize(assetPath, out width, out height))
                {
                    int max = Mathf.Clamp(Mathf.Max(width, height), 32, 4096);
                    textureImporter.maxTextureSize = Mathf.ClosestPowerOfTwo(max);
                }
            }
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