namespace Numeira
{
    [AddComponentMenu("NDPCSS/PCSS Material Settings")]
    internal sealed class PCSSMaterialSetting : NdpcssComponentBase
    {
        public Material[] Materials = { };

        public bool Ignore = false;

        [SerializeReference]
        public ShaderSetting Setting = new PCSSv6Settings();
    }
}