namespace Numeira;

internal enum LilToonShaderVariant : uint
{
    lts   =       LilToonShaderType.Base,

    ltsc  = lts  | LilToonShaderRenderingMode.Cutout,
    ltst  = lts  | LilToonShaderRenderingMode.Transparent,
    ltsot = ltst | LilToonShaderPass.One,
    ltstt = ltst | LilToonShaderPass.Two,
    
    ltso   = lts   | LilToonShaderOption.Outline,
    ltsco  = ltsc  | LilToonShaderOption.Outline,
    ltsto  = ltst  | LilToonShaderOption.Outline,
    ltsoto = ltsot | LilToonShaderOption.Outline,
    ltstto = ltstt | LilToonShaderOption.Outline,

    ltsoo  = lts  | LilToonShaderOption.OutlineOnly,
    ltscoo = ltsc | LilToonShaderOption.OutlineOnly,
    ltstoo = ltst | LilToonShaderOption.OutlineOnly,

    ltstess   = lts   | LilToonShaderOption.Tesselation,
    ltstessc  = ltsc  | LilToonShaderOption.Tesselation,
    ltstesst  = ltst  | LilToonShaderOption.Tesselation,
    ltstessot = ltsot | LilToonShaderOption.Tesselation,
    ltstesstt = ltstt | LilToonShaderOption.Tesselation,
    
    ltstesso   = ltstess   | LilToonShaderOption.Outline,
    ltstessco  = ltstessc  | LilToonShaderOption.Outline,
    ltstessto  = ltstesst  | LilToonShaderOption.Outline,
    ltstessoto = ltstessot | LilToonShaderOption.Outline,
    ltstesstto = ltstesstt | LilToonShaderOption.Outline,
    
    ltsl    = lts    | LilToonShaderType.Lite,
    ltslc   = ltsc   | LilToonShaderType.Lite,
    ltslt   = ltst   | LilToonShaderType.Lite,
    ltslot  = ltsot  | LilToonShaderType.Lite,
    ltsltt  = ltstt  | LilToonShaderType.Lite,
    
    ltslo   = ltso   | LilToonShaderType.Lite,
    ltslco  = ltsco  | LilToonShaderType.Lite,
    ltslto  = ltsto  | LilToonShaderType.Lite,
    ltsloto = ltsoto | LilToonShaderType.Lite,
    ltsltto = ltstto | LilToonShaderType.Lite,

    ltsm    =        LilToonShaderType.Multi,
    ltsmo   = ltsm | LilToonShaderOption.Outline,
    ltsmref = ltsm | LilToonShaderRenderingMode.Refraction,
    ltsmfur = ltsm | LilToonShaderOption.Fur,
    ltsmgem = ltsm | LilToonShaderRenderingMode.Gem,

    ltsfs = lts | LilToonShaderRenderingMode.FakeShadow,
}

internal enum LilToonShaderType : uint
{
    Invalid    = 0,
    Base       = 0b_0001,
    Lite       = 0b_0010,
    Multi      = 0b_0100,
}

internal enum LilToonShaderRenderingMode : uint
{
    Opaque             = 0,
    Cutout             = 0b_00000001_0000,
    Transparent        = 0b_00000010_0000,

    Refraction         = 0b_00000100_0000,
    RefractionBlur     = 0b_00001000_0000,
    Gem                = 0b_00010000_0000,
    FakeShadow         = 0b_10000000_0000,
}

internal enum LilToonShaderOption : uint
{
    Outline     = 0b_00000001_00000000_0000,
    Tesselation = 0b_00000010_00000000_0000,
    Fur         = 0b_00000100_00000000_0000,
    OutlineOnly = 0b_00001000_00000000_0000,
}

internal enum LilToonShaderPass : uint
{
    Default = 0b_00_00000000_00000000_0000,
    One     = 0b_01_00000000_00000000_0000,
    Two     = 0b_10_00000000_00000000_0000,
}

internal static class LilToonShaderVariantExt
{
    public static bool TryGetLilToonShaderTypes(this Shader shader, out LilToonShaderVariant result)
    {
        var fileName = Path.GetFileNameWithoutExtension((AssetDatabase.GetAssetPath(shader) ?? "").AsSpan());
        if (fileName.IsEmpty)
        {
            result = default;
            return false;
        }
        int i = 0;
        result = (LilToonShaderVariant)LilToonShaderType.Invalid;
        foreach(var range in fileName.Split('_')) // ('_')ﾉｼ
        {
            var seq = fileName[range];
            if (i == 0)
            {
                if (seq.Equals("lts", StringComparison.Ordinal))
                    result = result.ToBase();

                if (seq.Equals("ltsl", StringComparison.Ordinal))
                    result = result.ToLite();

                if (seq.Equals("ltsmulti", StringComparison.Ordinal))
                    result = result.ToMulti();
            }
            else
            {
                if (seq.Equals("o", StringComparison.Ordinal))
                    result = result.Outline();

                else if (seq.Equals("cutout", StringComparison.Ordinal))
                    result = result.SetRenderingMode(LilToonShaderRenderingMode.Cutout);

                else if (seq.Equals("trans", StringComparison.Ordinal))
                    result = result.SetRenderingMode(LilToonShaderRenderingMode.Transparent);

                else if (seq.Equals("onetrans", StringComparison.Ordinal))
                    result = result.SetRenderingMode(LilToonShaderRenderingMode.Transparent).SetPass(LilToonShaderPass.One);

                else if (seq.Equals("twotrans", StringComparison.Ordinal))
                    result = result.SetRenderingMode(LilToonShaderRenderingMode.Transparent).SetPass(LilToonShaderPass.Two);

                else if (seq.Equals("tess", StringComparison.Ordinal))
                    result = result.Tesselation();

                else if (seq.Equals("fur", StringComparison.Ordinal))
                    result = result.Fur();

                else if (seq.Equals("gem", StringComparison.Ordinal))
                    result = result.SetRenderingMode(LilToonShaderRenderingMode.Gem).SetPass(LilToonShaderPass.Default);

                else if (seq.Equals("one", StringComparison.Ordinal))
                    result = result.SetPass(LilToonShaderPass.One);

                else if (seq.Equals("two", StringComparison.Ordinal))
                    result = result.SetPass(LilToonShaderPass.Two);

                else if (seq.Equals("ref", StringComparison.Ordinal))
                    result = result.SetRenderingMode(LilToonShaderRenderingMode.Refraction);

                else if (seq.Equals("blur", StringComparison.Ordinal))
                    result = result.SetRenderingMode(LilToonShaderRenderingMode.RefractionBlur);

                else if (seq.Equals("fakeshadow", StringComparison.Ordinal))
                    result = result.SetRenderingMode(LilToonShaderRenderingMode.FakeShadow);

                else if (seq.Equals("oo", StringComparison.Ordinal))
                    result = result.OutlineOnly();
            }

            i++;
        }

        return result.GetShaderType() != LilToonShaderType.Invalid;
    }

    private const uint ShaderTypeMask = 0b1111;
    private const uint RenderingModeMask = 0b0000_11111111_0000;
    private const uint OptionMask = 0b00_11111111_000000000_0000;
    private const uint PassMask = 0b_11_00000000_00000000_0000;

    public static LilToonShaderVariant ToBase(this LilToonShaderVariant shader)
        => (LilToonShaderVariant)(((uint)shader & ~ShaderTypeMask) | (uint)LilToonShaderType.Base);

    public static LilToonShaderVariant ToLite(this LilToonShaderVariant shader)
        => (LilToonShaderVariant)(((uint)shader & ~ShaderTypeMask) | (uint)LilToonShaderType.Lite);

    public static LilToonShaderVariant ToMulti(this LilToonShaderVariant shader)
        => (LilToonShaderVariant)((((uint)shader & ~ShaderTypeMask) | (uint)LilToonShaderType.Lite) & ~((uint)LilToonShaderOption.Tesselation | RenderingModeMask));

    public static LilToonShaderType GetShaderType(this LilToonShaderVariant shader)
        => (LilToonShaderType)((uint)shader & ShaderTypeMask);

    public static LilToonShaderRenderingMode GetRenderingMode(this LilToonShaderVariant shader)
        => (LilToonShaderRenderingMode)(((uint)shader & RenderingModeMask));

    public static LilToonShaderVariant SetRenderingMode(this LilToonShaderVariant shader, LilToonShaderRenderingMode mode) 
        => (LilToonShaderVariant)(((uint)shader & ~RenderingModeMask) | (uint)mode);

    public static LilToonShaderPass GetPass(this LilToonShaderVariant shader)
        => (LilToonShaderPass)((uint)shader & PassMask);

    public static LilToonShaderVariant SetPass(this LilToonShaderVariant shader, LilToonShaderPass pass) 
        => (LilToonShaderVariant)(((uint)shader & ~PassMask) | (uint)pass);

    public static LilToonShaderVariant Outline(this LilToonShaderVariant shader, bool enable = true)
        => shader.Option(LilToonShaderOption.Outline, enable);

    public static LilToonShaderVariant Tesselation(this LilToonShaderVariant shader, bool enable = true)
        => shader.Option(LilToonShaderOption.Tesselation, enable);

    public static LilToonShaderVariant Fur(this LilToonShaderVariant shader, bool enable = true)
        => shader.Option(LilToonShaderOption.Fur, enable);

    public static LilToonShaderVariant OutlineOnly(this LilToonShaderVariant shader, bool enable = true)
        => shader.Option(LilToonShaderOption.OutlineOnly, enable);

    private static LilToonShaderVariant Option(this LilToonShaderVariant shader, LilToonShaderOption option, bool enable = true)
        => enable ?
            (LilToonShaderVariant)((uint)shader |  (uint)option):
            (LilToonShaderVariant)((uint)shader & ~(uint)option);
}
