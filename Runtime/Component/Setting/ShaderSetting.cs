
using System.Reflection;

namespace Numeira;

internal abstract record class ShaderSetting
{
    [HideInInspector]
    public bool IsDefault = false;

    public virtual void Apply(Material destination)
    {
        foreach(var field in GetType().GetFields())
        {
            if (field.GetCustomAttribute<ShaderPropertyNameAttribute>() is not { } attr ||
                !typeof(IShaderProperty).IsAssignableFrom(field.FieldType))
                continue;

            var property = (IShaderProperty)field.GetValue(this);
            if (!IsDefault && !property.Override)
                continue;
            property.Apply(destination, attr.Name);
        }
    }
}