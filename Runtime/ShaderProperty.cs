namespace Numeira;

internal interface IShaderProperty
{
    public bool Override { get; set; }
    public void Apply(Material material, string propertyName);
}

[Serializable]
public struct ShaderProperty<T> : IShaderProperty, IEquatable<ShaderProperty<T>>, IEquatable<T>
{
    public T? Value;
    public bool Override;

    public ShaderProperty(T? value, bool @override = false)
    {
        Value = value;
        Override = @override;
    }

    bool IShaderProperty.Override { get => Override; set => Override = value; }

    void IShaderProperty.Apply(Material material, string propertyName)
    {
        if (typeof(T) == typeof(float))
            material.SetFloat(propertyName, As<float>(Value!));

        else if (typeof(T) == typeof(int))
            material.SetInt(propertyName, As<int>(Value!));

        else if (typeof(T) == typeof(bool))
            material.SetFloat(propertyName, As<bool>(Value!) ? 1 : 0);

        else if (typeof(T) == typeof(Color))
            material.SetColor(propertyName, As<Color>(Value!));

        else if (typeof(T) == typeof(Texture2D))
            material.SetTexture(propertyName, As<Texture2D>(Value!));
    }

    static TTo As<TTo>(T value) => Unsafe.As<T, TTo>(ref value);

    public readonly bool Equals(ShaderProperty<T> other)
    {
        return Override == other.Override &&
            EqualityComparer<T?>.Default.Equals(Value, other.Value);
    }

    public bool Equals(T other) => EqualityComparer<T?>.Default.Equals(Value, other);

    public static implicit operator ShaderProperty<T>(T value) => new(value);
}