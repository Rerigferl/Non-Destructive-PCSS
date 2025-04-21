using System.Diagnostics.CodeAnalysis;

namespace Numeira;

internal static class NullableUtil
{
    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfNull<T>(this T? obj) where T : Object
        => obj.ThrowIfNull(() => new NullReferenceException());

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfNull<T, TException>(this T? obj, TException? _ = default) where T : Object where TException : Exception, new()
    {
        if (obj == null)
            Throw();
        return obj;

        [DoesNotReturn]
        static void Throw() => throw new TException();
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfNull<T, TException>(this T? obj, Func<TException> exception) where T : Object where TException : Exception
    {
        if (obj == null)
            Throw(exception());
        return obj;

        [DoesNotReturn]
        static void Throw(TException exception) => throw exception;
    }
}
