
namespace Numeira;

internal sealed class Deffered<T> : IDisposable where T : Object
{
    private readonly Func<T> factory;
    private T? cache;

    public Deffered(Func<T> factory)
    {
        this.factory = factory;
    }

    public T Value
    {
        get
        {
            if (cache == null)
                cache = factory();
            return cache;
        }
    }

    public void Refresh() => cache = null;

    public void Dispose() => Object.DestroyImmediate(cache);

    public static implicit operator T(Deffered<T> deffered) => deffered.Value;
}
