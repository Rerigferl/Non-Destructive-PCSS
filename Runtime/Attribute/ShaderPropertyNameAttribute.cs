
namespace Numeira;

internal class ShaderPropertyNameAttribute : Attribute
{
    public ShaderPropertyNameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
