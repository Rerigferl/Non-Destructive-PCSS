using nadena.dev.ndmf;
using nadena.dev.ndmf.runtime;

namespace Numeira;

internal static partial class Ndpcss
{
    const string Hidden = "Hidden/";
    const string ShaderName = "PCSS4VRC/PCSS4lilToon/";
    const string MenuPath = "GameObject/PCSS for VRChat/Non-destructive Setup";

    public static readonly Dictionary<LilToonShaderVariant, Deffered<Shader>> PCSSShaderMap = new()
    {
        {LilToonShaderVariant.lts,          () => Shader.Find($"{ShaderName}lilToon")},
        {LilToonShaderVariant.ltsc,         () => Shader.Find($"{Hidden}{ShaderName}Cutout")},
        {LilToonShaderVariant.ltst,         () => Shader.Find($"{Hidden}{ShaderName}Transparent")},
        {LilToonShaderVariant.ltsot,        () => Shader.Find($"{Hidden}{ShaderName}OnePassTransparent")},
        {LilToonShaderVariant.ltstt,        () => Shader.Find($"{Hidden}{ShaderName}TwoPassTransparent")},
        {LilToonShaderVariant.ltso,         () => Shader.Find($"{Hidden}{ShaderName}OpaqueOutline")},
        {LilToonShaderVariant.ltsco,        () => Shader.Find($"{Hidden}{ShaderName}CutoutOutline")},
        {LilToonShaderVariant.ltsto,        () => Shader.Find($"{Hidden}{ShaderName}TransparentOutline")},
        {LilToonShaderVariant.ltsoto,       () => Shader.Find($"{Hidden}{ShaderName}OnePassTransparentOutline")},
        {LilToonShaderVariant.ltstto,       () => Shader.Find($"{Hidden}{ShaderName}TwoPassTransparentOutline")},
        {LilToonShaderVariant.ltstess,      () => Shader.Find($"{Hidden}{ShaderName}Tessellation/Opaque")},
        {LilToonShaderVariant.ltstessc,     () => Shader.Find($"{Hidden}{ShaderName}Tessellation/Cutout")},
        {LilToonShaderVariant.ltstesst,     () => Shader.Find($"{Hidden}{ShaderName}Tessellation/Transparent")},
        {LilToonShaderVariant.ltstessot,    () => Shader.Find($"{Hidden}{ShaderName}Tessellation/OnePassTransparent")},
        {LilToonShaderVariant.ltstesstt,    () => Shader.Find($"{Hidden}{ShaderName}Tessellation/TwoPassTransparent")},
        {LilToonShaderVariant.ltstesso,     () => Shader.Find($"{Hidden}{ShaderName}Tessellation/OpaqueOutline")},
        {LilToonShaderVariant.ltstessco,    () => Shader.Find($"{Hidden}{ShaderName}Tessellation/CutoutOutline")},
        {LilToonShaderVariant.ltstessto,    () => Shader.Find($"{Hidden}{ShaderName}Tessellation/TransparentOutline")},
        {LilToonShaderVariant.ltstessoto,   () => Shader.Find($"{Hidden}{ShaderName}Tessellation/OnePassTransparentOutline")},
        {LilToonShaderVariant.ltstesstto,   () => Shader.Find($"{Hidden}{ShaderName}Tessellation/TwoPassTransparentOutline")},
        {LilToonShaderVariant.ltsl,         () => Shader.Find($"{ShaderName}lilToonLite")},
        {LilToonShaderVariant.ltslc,        () => Shader.Find($"{Hidden}{ShaderName}Lite/Cutout")},
        {LilToonShaderVariant.ltslt,        () => Shader.Find($"{Hidden}{ShaderName}Lite/Transparent")},
        {LilToonShaderVariant.ltslot,       () => Shader.Find($"{Hidden}{ShaderName}Lite/OnePassTransparent")},
        {LilToonShaderVariant.ltsltt,       () => Shader.Find($"{Hidden}{ShaderName}Lite/TwoPassTransparent")},
        {LilToonShaderVariant.ltslo,        () => Shader.Find($"{Hidden}{ShaderName}Lite/OpaqueOutline")},
        {LilToonShaderVariant.ltslco,       () => Shader.Find($"{Hidden}{ShaderName}Lite/CutoutOutline")},
        {LilToonShaderVariant.ltslto,       () => Shader.Find($"{Hidden}{ShaderName}Lite/TransparentOutline")},
        {LilToonShaderVariant.ltsloto,      () => Shader.Find($"{Hidden}{ShaderName}Lite/OnePassTransparentOutline")},
        {LilToonShaderVariant.ltsltto,      () => Shader.Find($"{Hidden}{ShaderName}Lite/TwoPassTransparentOutline")},
        {LilToonShaderVariant.ltsm,         () => Shader.Find($"{ShaderName}lilToonMulti")},
        {LilToonShaderVariant.ltsmo,        () => Shader.Find($"{Hidden}{ShaderName}MultiOutline") },
    };

    [MenuItem(MenuPath)]
    public static void AppendToAvatar()
    {
        // 26f14b3bad5df1b4fad13c0e7f6ba8d8
        var prefab = AssetUtil.LoadAssetFromGUID<GameObject>("26f14b3bad5df1b4fad13c0e7f6ba8d8");

        var obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, Selection.activeGameObject.transform);
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj.GetComponent<NonDestructivePCSS>());
    }

    [MenuItem("GameObject/PCSS for VRChat/Non-destructive Setup")]
    public static bool ValidAvatar()
    {
        var selection = Selection.activeGameObject;
        return selection != null && RuntimeUtil.IsAvatarRoot(selection.transform);
    }

    public static Material? ToPCSSMaterial(Material? material, SessionContext context)
    {
        if (material == null || material.shader == null || context.Ignores.Contains(ObjectRegistry.GetReference(material)))
            return material;

        if (context.MaterialCache.TryGetValue(material, out var clone))
        {
            return clone;
        }

        if (!material.shader.TryGetLilToonShaderTypes(out var shaderVariant))
        {
            context.MaterialCache[material] = material;
            return material;
        }

        clone = new(material)
        {
            name = $"{material.name}(PCSS)",
            parent = null
        };
        context.MaterialCache[material] = clone;
        ObjectRegistry.RegisterReplacedObject(material, clone);

        if (!PCSSShaderMap.TryGetValue(shaderVariant, out var shader))
        {
            shader = PCSSShaderMap[LilToonShaderVariant.lts];
        }

        clone.shader = shader;

        SetupPCSSMaterial(clone);

        return clone;
    }

    private static void SetupPCSSMaterial(Material material)
    {
        material.SetTexture("_EnvLightLevelTexture", AssetUtil.LoadAssetFromGUID<CustomRenderTexture>("7c7ffb8c28fc09e4b82b34eed9ff1f25"));
        material.SetTexture("_IgnoreCookieTexture", AssetUtil.LoadAssetFromGUID<Texture2D>("48cc868204b0e5b4b9b4d997b633a668"));

        material.SetFloat("_BlendOpFA", 0);
        if (material.shader.name.Contains("Transparent"))
        {
            material.SetColor("_DropShadowColor", new Color(0.5f, 0.5f, 0.5f));
        }
        material.SetFloat("_AlphaBoostFA", 1);

        if (material.GetFloat("_StencilRef") == 0 || material.GetFloat("_StencilPass") == 0)
        {
            material.SetFloat("_StencilComp", 8);
            material.SetFloat("_StencilPass", 2);
            material.SetFloat("_StencilRef", 125);
        }
    }

    public static void ApplyPCSSSettings(this SessionContext context)
    {
        var component = context.ComponentRoot;
        if (component == null)
            return;

        foreach (var (original, material) in context.MaterialCache)
        {
            component.ShaderSetting.Apply(material);

            if (context.MaterialSettings.TryGetValue(ObjectRegistry.GetReference(original), out var settings))
            {
                foreach (var setting in settings)
                {
                    setting.Apply(material);
                }
            }
        }
    }

    public static GameObject CreateControlPrefab()
    {
        var prefab = AssetUtil.LoadAssetFromGUID<GameObject>("2cb7e7a1bfa698a4ab7437bc4dc1e3c2").ThrowIfNull(default(ArgumentNullException));
        prefab = Object.Instantiate(prefab);
        prefab.name = "SelfLight";
        return prefab;
    }
}
