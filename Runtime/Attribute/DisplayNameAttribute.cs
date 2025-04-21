namespace Numeira;

[AttributeUsage(AttributeTargets.Field)]
internal sealed class DisplayNameAttribute : Attribute
{
    public DisplayNameAttribute(string name) => Name = name;

    public string Name { get; }
}