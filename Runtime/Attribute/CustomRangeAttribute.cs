global using RangeAttribute = Numeira.CustomRangeAttribute;

namespace Numeira;

[AttributeUsage(AttributeTargets.Field)]
internal sealed class CustomRangeAttribute : Attribute
{
    public CustomRangeAttribute(float min, float max) => (Min, Max) = (min, max);

    public float Min { get; }
    public float Max { get; }
}
