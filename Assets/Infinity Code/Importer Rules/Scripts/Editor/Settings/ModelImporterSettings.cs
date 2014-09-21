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
        public ModelImporterTangentSpaceMode normalImportMode = ModelImporterTangentSpaceMode.Import;
        public float normalSmoothingAngle = 60;
        public bool optimizeMesh = true;
        public float secondaryUVAngleDistortion = 8;
        public float secondaryUVAreaDistortion = 15;
        public float secondaryUVHardAngle = 88;
        public float secondaryUVPackMargin = 4;
        public bool splitTangentsAcrossSeams = true;
        public bool swapUVChannels;
        public ModelImporterTangentSpaceMode tangentImportMode = ModelImporterTangentSpaceMode.Calculate;
        public bool useFileUnits;
    }
}