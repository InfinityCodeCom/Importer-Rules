/*     INFINITY CODE 2013-2016      */
/*   http://www.infinity-code.com   */

using UnityEditor;

namespace InfinityCode.ImporterRules
{
    public class ModelImporterSettings
    {
        public bool addCollider;
        public ModelImporterAnimationType animationType = ModelImporterAnimationType.Generic;
        public bool generateSecondaryUV;
        public float globalScale = 0.01f;
        public bool importAnimation = true;
        public bool importBlendShapes = true;
        public bool importMaterials = true;
        public bool isReadable = true;
        public ModelImporterMaterialName materialName = ModelImporterMaterialName.BasedOnTextureName;
        public ModelImporterMaterialSearch materialSearch = ModelImporterMaterialSearch.RecursiveUp;
        public ModelImporterMeshCompression meshCompression = ModelImporterMeshCompression.Off;

#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
        public ModelImporterTangentSpaceMode normalImportMode = ModelImporterTangentSpaceMode.Import;
        public ModelImporterTangentSpaceMode tangentImportMode = ModelImporterTangentSpaceMode.Calculate;
#else
        public ModelImporterNormals normalImportMode = ModelImporterNormals.Import;
        public ModelImporterNormals tangentImportMode = ModelImporterNormals.Calculate;
#endif
        public float normalSmoothingAngle = 60;
        public bool optimizeMesh = true;
        public float secondaryUVAngleDistortion = 8;
        public float secondaryUVAreaDistortion = 15;
        public float secondaryUVHardAngle = 88;
        public float secondaryUVPackMargin = 4;
        public bool splitTangentsAcrossSeams = true;
        public bool swapUVChannels;
        public bool useFileUnits;
    }
}