using System.Collections.Generic;
using System.Threading;
using InfinityCode.ImporterRules;
using UnityEditor;
using UnityEngine;

public class ImporterRulesPreprocess: AssetPostprocessor
{
    private static bool allowPostprocess = true;
    private static List<string> waitPaths;

    private static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (!allowPostprocess) return;

        if (waitPaths == null) waitPaths = new List<string>();

        allowPostprocess = false; 
        foreach (string assetPath in importedAssets)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
            if (assetImporter is MovieImporter)
            {
                TryApplyRule(assetPath, assetImporter, ImporterRulesTypes.movie);
            }
            else if (assetImporter is TrueTypeFontImporter)
            {
                TryApplyRule(assetPath, assetImporter, ImporterRulesTypes.trueTypeFont);
            }
        }

        allowPostprocess = true;
    }

    private static void TryApplyRule(string assetPath, AssetImporter assetImporter, ImporterRulesTypes type)
    {
        if (waitPaths.Contains(assetPath)) waitPaths.Remove(assetPath);
        else if (ImporterRulesWindow.ApplyFirstRule(type, assetPath, assetImporter))
        {
            waitPaths.Add(assetPath);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }
            
    }

    private void OnPreprocessAudio()
    {
        ImporterRulesWindow.ApplyFirstRule(ImporterRulesTypes.audio, assetPath, assetImporter);
    }

    private void OnPreprocessModel()
    {
        ImporterRulesWindow.ApplyFirstRule(ImporterRulesTypes.model, assetPath, assetImporter);
    }

    private void OnPreprocessTexture()
    {
        ImporterRulesWindow.ApplyFirstRule(ImporterRulesTypes.texture, assetPath, assetImporter);
    }
}