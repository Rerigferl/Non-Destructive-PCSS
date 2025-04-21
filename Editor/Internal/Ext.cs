
using nadena.dev.ndmf.animator;

namespace Numeira;

internal static class Ext
{
    public static void Add<T, TKey, TDict>(this TDict dictionary, TKey key, Func<T> factory) where T : Object where TDict : IDictionary<TKey, Deffered<T>>
    {
        dictionary.Add(key, new(factory));
    }

    public static HashCode GetArrayHashCode<T>(this T[] array)
    {
        var hash = new HashCode();
        foreach(var x in array)
        {
            hash.Add(x);
        }
        return hash;
    }

    public static IEnumerable<VirtualClip> AllClips(this VirtualState state)
    {
        return Walk(state.Motion, new());

        static IEnumerable<VirtualClip> Walk(VirtualMotion? motion, HashSet<VirtualMotion> visited)
        {
            if (motion == null || !visited.Add(motion)) yield break;

            if (motion is VirtualClip clip)
            {
                yield return clip;
            }
            else if (motion is VirtualBlendTree blendTree)
            {
                foreach(var x in blendTree.Children)
                {
                    foreach(var y in Walk(x.Motion, visited))
                    {
                        yield return y;
                    }
                }
            }
        }

    }
}
