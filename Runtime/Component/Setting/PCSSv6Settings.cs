
using System.Runtime.InteropServices;

namespace Numeira
{
    [Serializable]
    internal sealed record class PCSSv6Settings : ShaderSetting
    {
        //_IsOn ("IsOn", Float) = 1 // [Toggle]
        [ShaderPropertyName("_IsOn")]
        [DisplayName("Enable")]
        public ShaderProperty<bool> IsOn = true;

        //Blocker_Samples("Blocker Samples", Range(4, 32)) = 12 // [IntRange]
        [ShaderPropertyName("Blocker_Samples")]
        [Range(4, 32)]
        public ShaderProperty<int> BlockerSamples = 12;

        //PCF_Samples("Filter Samples", Range(8, 64)) = 24 // [IntRange]
        [ShaderPropertyName("PCF_Samples")]
        [Range(8, 64)]
        public ShaderProperty<int> FilterSamples = 24;

        //Softness("Softness", Range(0, 0.005)) = 0.001
        [ShaderPropertyName("Softness")]
        [Range(0, 0.005f)]
        public ShaderProperty<float> Softness = 0.001f;

        //SoftnessFalloff("SoftnessFalloff", Range(0, 2)) = 1
        [ShaderPropertyName("SoftnessFalloff")]
        [Range(0, 2)]
        public ShaderProperty<float> SoftnessFalloff = 1;

        //SoftnessRange("SoftnessRange", Range(0, 0.001)) = 0.0001
        [ShaderPropertyName("SoftnessRange")]
        [Range(0, 0.001f)]
        public ShaderProperty<float> SoftnessRange = 0.0001f;

        //MaxDistance("MaxDistance", Range(0, 1)) = 0.2
        [ShaderPropertyName("MaxDistance")]
        [Range(0, 1)]
        public ShaderProperty<float> MaxDistance = 0.2f;

        //PenumbraWithMaxSamples("PenumbraWithMaxSamples", Range(1, 50)) = 5
        [ShaderPropertyName("PenumbraWithMaxSamples")]
        [Range(1, 50)]
        public ShaderProperty<float> PenumbraWithMaxSamples = 5;

        //Blocker_GradientBias("Blocker_GradientBias", Range(0, 0.001)) =  0.0002
        [ShaderPropertyName("Blocker_GradientBias")]
        [Range(0, 0.001f)]
        public ShaderProperty<float> BlockerGradientBias = 0.0002f;

        //PCF_GradientBias("PCF_GradientBias", Range(0, 0.001)) = 0.0002
        [ShaderPropertyName("PCF_GradientBias")]
        [Range(0, 0.001f)]
        public ShaderProperty<float> PCFGradientBias = 0.0002f;

        //_ReceiveMaskTex("ReceiveMask", 2D) = "white" {}
        [ShaderPropertyName("_ReceiveMaskTex")]
        public ShaderProperty<Texture2D?> ReceiveMask = new(null);

        //_ReceiveMaskStrength("ReceiveMaskStrength", Range(0,1)) = 1
        [ShaderPropertyName("_ReceiveMaskStrength")]
        [Range(0, 1)]
        public ShaderProperty<float> ReceiveMaskStrength = 1;

        //_CastMaskTex("CastMask", 2D) = "white" {}
        [ShaderPropertyName("_CastMaskTex")]
        public ShaderProperty<Texture2D?> CastMask = new(null);

        //_CastMaskStrength("CastMaskStrength", Range(0,1)) = 1
        [ShaderPropertyName("_CastMaskStrength")]
        [Range(0, 1)]
        public ShaderProperty<float> CastMaskStrength = 1;

        //_ShadowNormalBias("ShadowNormalBias", Range(0,0.01)) = 0.0025
        [ShaderPropertyName("_ShadowNormalBias")]
        [Range(0, 0.01f)]
        public ShaderProperty<float> ShadowNormalBias = 0.0025f;

        //_ShadowCasterBias("_ShadowCasterBias", Range(0,0.1)) = 0.001
        [ShaderPropertyName("_ShadowCasterBias")]
        [Range(0, 0.1f)]
        public ShaderProperty<float> ShadowCasterBias = 0.001f;

        //_ShadowCasterBiasOffset("_ShadowCasterBiasOffset", Range(0,1)) = 0
        [ShaderPropertyName("_ShadowCasterBiasOffset")]
        [Range(0, 1)]
        public ShaderProperty<float> ShadowCasterBiasOffset = 0;

        //_EnvLightStrength("EnvironmentLightStrength", Range(0,1)) = 0.2
        [ShaderPropertyName("_EnvLightStrength")]
        [Range(0, 1)]
        public ShaderProperty<float> EnvironmentLightStrength = 0.2f;

        //_ShadowDistance("_ShadowDistance", Range(0.01, 100)) = 10.0
        [ShaderPropertyName("_ShadowDistance")]
        [Range(0.01f, 100)]
        public ShaderProperty<float> ShadowDistance = 10;

        //_ShadowClamp("ShadowClamp", Range(0, 1)) = 0
        [ShaderPropertyName("_ShadowClamp")]
        [Range(0, 1)]
        public ShaderProperty<float> ShadowClamp = 0;

        //_ShadowDensity("ShadowDensity", Range(0, 1)) = 0.1
        [ShaderPropertyName("_ShadowDensity")]
        [Range(0, 1)]
        public ShaderProperty<float> ShadowDensity = 0.1f;

        //_DropShadowColor("ShadowColor", Color) = (0,0,0)
        [ShaderPropertyName("_DropShadowColor")]
        public ShaderProperty<Color> ShadowColor = Color.black;

        //_EnvLightLevelTexture("EnvLightLevelTexture", 2D) = "white" {}
        [ShaderPropertyName("_EnvLightLevelTexture")]
        public ShaderProperty<Texture2D?> EnvLightLevelTexture = new(null);

        //_EnvLightAdjustLevel("EnvLightAdjustLevel", Range(0, 1)) = 1
        [ShaderPropertyName("_EnvLightAdjustLevel")]
        [Range(0, 1)]
        public ShaderProperty<float> EnvLightAdjustLevel = 1;

        //_ShadowcoordzOffset("ShadowcoordzOffset", Range(0, 3)) = 0
        [ShaderPropertyName("_ShadowcoordzOffset")]
        [Range(0, 3)]
        public ShaderProperty<float> ShadowcoordZOffset = 0;

        [Space(8)]

        //_InterpolationStrength("InterpolationStrength", Range(0, 1)) = 1
        [ShaderPropertyName("_InterpolationStrength")]
        [Range(0, 1)]
        public ShaderProperty<float> InterpolationStrength = 1;

        //_MinusNormalOffset("MinusNormalOffset", Range(0, 1)) = 0
        [ShaderPropertyName("_MinusNormalOffset")]
        [Range(0, 1)]
        public ShaderProperty<float> MinusNormalOffset = 0;

        //_PlusNormalOffset("PlusNormalOffset", Range(0, 1)) = 0.01
        [ShaderPropertyName("_PlusNormalOffset")]
        [Range(0, 1)]
        public ShaderProperty<float> PlusNormalOffset = 0.01f;

        //_ShadingThreshold("ShadingThreshold", Range(0, 1)) = 0
        [ShaderPropertyName("_ShadingThreshold")]
        [Range(0, 1)]
        public ShaderProperty<float> ShadingThreshold = 0;

        //_ShadingBlurRadius("ShadingBlurRadius", Range(0, 3)) = 3
        [ShaderPropertyName("_ShadingBlurRadius")]
        [Range(0, 3)]
        public ShaderProperty<float> ShadingBlurRadius = 3;

        //_ShadingCutOffThreshold("ShadingCutOffThreshold", Range(0, 1)) = 1
        [ShaderPropertyName("_ShadingCutOffThreshold")]
        [Range(0, 1)]
        public ShaderProperty<float> ShadingCutOffThreshold = 1;

        //_ShadingCutOffBlurRadius("ShadingCutOffThreshold", Range(0, 3)) = 0.8
        [ShaderPropertyName("_ShadingCutOffBlurRadius")]
        [Range(0, 3)]
        public ShaderProperty<float> ShadingCutOffBlurRadius = 0.8f;

        [Space(8)]

        ////_EnableSurfaceSmoothing("EnableSurfaceSmoothing", Float) = 1 // [Toggle]
        [ShaderPropertyName("//_EnableSurfaceSmoothing")]
        public ShaderProperty<bool> EnableSurfaceSmoothing = true;

        [Space(8)]

        //_ShadowColorOverrideTexture("ShadowColorOverrideTexture", 2D) = "white" {}
        [ShaderPropertyName("_ShadowColorOverrideTexture")]
        public ShaderProperty<Texture2D?> ShadowColorOverrideTexture = new(null);

        //_ShadowColorOverrideStrength("ShadowColorOverrideStrength", Range(0,1)) = 0
        [ShaderPropertyName("_ShadowColorOverrideStrength")]
        [Range(0, 1)]
        public ShaderProperty<float> ShadowColorOverrideStrength = 0;
 
        [Space(8)]

        //_ShadowBiasMaskTexture("_ShadowBiasMaskTexture", 2D) = "white" {}
        [ShaderPropertyName("_ShadowBiasMaskTexture")]
        public ShaderProperty<Texture2D?> ShadowBiasMaskTexture = new(null);

        //_ShadowBiasMaskStrength("_ShadowBiasMaskStrength", Range(0,1)) = 0
        [ShaderPropertyName("_ShadowBiasMaskStrength")]
        [Range(0, 1)]
        public ShaderProperty<float> ShadowBiasMaskStrength = 0;
    }
}