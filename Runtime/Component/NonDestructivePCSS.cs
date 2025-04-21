namespace Numeira
{
    [AddComponentMenu("NDPCSS/Non-Destructive PCSS")]
    internal sealed class NonDestructivePCSS : NdpcssComponentBase
    {
        [SerializeReference]
        public ShaderSetting ShaderSetting = new PCSSv6Settings() { IsDefault = true };
    }
}