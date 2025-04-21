using nadena.dev.ndmf;

namespace Numeira;

internal sealed class SessionContext
{
    public static SessionContext Init(BuildContext context) => new(context);

    public SessionContext(BuildContext context) : this(context.AvatarRootObject.GetComponentInChildren<NonDestructivePCSS>())
    {
        BuildContext = context;
    }

    public SessionContext(NonDestructivePCSS? component)
    {
        ComponentRoot = component;
        MaterialCache = new(); 
        var settings = (component?.GetComponentsInChildren<PCSSMaterialSetting>() ?? Array.Empty<PCSSMaterialSetting>()).ToArray();
        Ignores = settings.Where(x => x.Ignore).SelectMany(x => x.Materials).Select(x => ObjectRegistry.GetReference(x)).ToHashSet() ?? new();
        MaterialSettings = settings.Where(x => !x.Ignore).SelectMany(x => x.Materials.Select(y => (Mat: y, Setting: x))).GroupBy(x => x.Mat).ToDictionary(x => ObjectRegistry.GetReference(x.Key), x => x.Select(x => x.Setting.Setting).ToArray());
    }

    public BuildContext? BuildContext { get; }
    public NonDestructivePCSS? ComponentRoot { get; }
    public HashSet<ObjectReference> Ignores { get; }
    public Dictionary<Material, Material> MaterialCache { get; }
    public Dictionary<ObjectReference, ShaderSetting[]> MaterialSettings { get; }

}