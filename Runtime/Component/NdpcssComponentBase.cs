using VRC.SDKBase;

namespace Numeira;

internal abstract class NdpcssComponentBase : MonoBehaviour, IEditorOnly, IDisposable
{
    public void Dispose()
    {
        Object.DestroyImmediate(this);
    }
}
