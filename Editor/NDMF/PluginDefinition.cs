using nadena.dev.ndmf;
using nadena.dev.ndmf.animator;

[assembly: ExportsPlugin(typeof(Numeira.PluginDefinition))]

namespace Numeira;

internal sealed class PluginDefinition : Plugin<PluginDefinition>
{
    public override string DisplayName => "Non-Destructive Real-Shadow System";

    public override string QualifiedName => "numeira.non-destructive-real-shadow";

    protected override void Configure()
    {
        InPhase(BuildPhase.Transforming)
            .WithRequiredExtension(typeof(VirtualControllerContext), seq =>
        {
            seq.Run("ad", context =>
            {
                var sessionContext = context.GetState(SessionContext.Init);
                var component = sessionContext.ComponentRoot;
                if (component == null)
                    return;

                foreach(var renderer in context.AvatarRootObject.GetComponentsInChildren<Renderer>(true))
                {
                    var materials = renderer.sharedMaterials;
                    foreach (ref var material in materials.AsSpan())
                    {
                        material = Ndpcss.ToPCSSMaterial( material, sessionContext);
                    }
                    renderer.sharedMaterials = materials;
                }

                foreach(var clip in 
                    context.Extension<VirtualControllerContext>().Controllers.Values
                    .SelectMany(x => x.Layers)
                    .Select(x => x.StateMachine!)
                    .Where(x => x != null)
                    .SelectMany(x => x.AllStates())
                    .SelectMany(x => x.AllClips()))
                {
                    foreach(var bind in clip.GetObjectCurveBindings())
                    {
                        var curves = clip.GetObjectCurve(bind) ?? Array.Empty<ObjectReferenceKeyframe>();
                        foreach (ref var curve in curves.AsSpan())
                        {
                            if (curve.value is Material material)
                                curve.value = Ndpcss.ToPCSSMaterial(material, sessionContext);
                        }
                        clip.SetObjectCurve(bind, curves);
                    }
                }

                sessionContext.ApplyPCSSSettings();

                var prefab = Ndpcss.CreateControlPrefab();
                prefab.transform.parent = context.AvatarRootTransform;

                if (Application.isPlaying)
                {
                    prefab.GetComponentInChildren<Light>().cullingMask |= 1;
                }

            })
            .PreviewingWith(new Ndpcss.Preview());
        });
    }
}
