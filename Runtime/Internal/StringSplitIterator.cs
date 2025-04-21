namespace Numeira;

static partial class SpanExt
{
    public static SpanSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> span, T separator) where T : IEquatable<T>
    {
        return new SpanSplitEnumerator<T>(span, separator);
    }
}


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
internal ref struct SpanSplitEnumerator<T> where T : IEquatable<T>
{
    /// <summary>The input span being split.</summary>
    private readonly ReadOnlySpan<T> _source;

    /// <summary>A single separator to use when <see cref="_splitMode"/> is <see cref="SpanSplitEnumeratorMode.SingleElement"/>.</summary>
    private readonly T _separator = default!;
    /// <summary>
    /// A separator span to use when <see cref="_splitMode"/> is <see cref="SpanSplitEnumeratorMode.Sequence"/> (in which case
    /// it's treated as a single separator) or <see cref="SpanSplitEnumeratorMode.Any"/> (in which case it's treated as a set of separators).
    /// </summary>
    private readonly ReadOnlySpan<T> _separatorBuffer;

    /// <summary>Mode that dictates how the instance was configured and how its fields should be used in <see cref="MoveNext"/>.</summary>
    private SpanSplitEnumeratorMode _splitMode;
    /// <summary>The inclusive starting index in <see cref="_source"/> of the current range.</summary>
    private int _startCurrent = 0;
    /// <summary>The exclusive ending index in <see cref="_source"/> of the current range.</summary>
    private int _endCurrent = 0;
    /// <summary>The index in <see cref="_source"/> from which the next separator search should start.</summary>
    private int _startNext = 0;

    /// <summary>Gets an enumerator that allows for iteration over the split span.</summary>
    /// <returns>Returns a <see cref="SpanSplitEnumerator{T}"/> that can be used to iterate over the split span.</returns>
    public SpanSplitEnumerator<T> GetEnumerator() => this;

    /// <summary>Gets the source span being enumerated.</summary>
    /// <returns>Returns the <see cref="ReadOnlySpan{T}"/> that was provided when creating this enumerator.</returns>
    public readonly ReadOnlySpan<T> Source => _source;

    /// <summary>Gets the current element of the enumeration.</summary>
    /// <returns>Returns a <see cref="Range"/> instance that indicates the bounds of the current element withing the source span.</returns>
    public Range Current => new Range(_startCurrent, _endCurrent);

    internal SpanSplitEnumerator(ReadOnlySpan<T> source, ReadOnlySpan<T> separator)
    {
        _source = source;
        _separatorBuffer = separator;
        _splitMode = separator.Length == 0 ?
            SpanSplitEnumeratorMode.EmptySequence :
            SpanSplitEnumeratorMode.Sequence;
    }

    /// <summary>Initializes the enumerator for <see cref="SpanSplitEnumeratorMode.SingleElement"/>.</summary>
    internal SpanSplitEnumerator(ReadOnlySpan<T> source, T separator)
    {
        _source = source;
        _separator = separator;
        _separatorBuffer = default;
        _splitMode = SpanSplitEnumeratorMode.SingleElement;
    }

    /// <summary>
    /// Advances the enumerator to the next element of the enumeration.
    /// </summary>
    /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the enumeration.</returns>
    public bool MoveNext()
    {
        // Search for the next separator index.
        int separatorIndex, separatorLength;
        switch (_splitMode)
        {
            case SpanSplitEnumeratorMode.None:
                return false;

            case SpanSplitEnumeratorMode.SingleElement:
                separatorIndex = _source.Slice(_startNext).IndexOf(_separator);
                separatorLength = 1;
                break;

            case SpanSplitEnumeratorMode.Any:
                separatorIndex = _source.Slice(_startNext).IndexOfAny(_separatorBuffer);
                separatorLength = 1;
                break;

            case SpanSplitEnumeratorMode.Sequence:
                separatorIndex = _source.Slice(_startNext).IndexOf(_separatorBuffer);
                separatorLength = _separatorBuffer.Length;
                break;

            default:
                separatorIndex = -1;
                separatorLength = 1;
                break;
        }

        _startCurrent = _startNext;
        if (separatorIndex >= 0)
        {
            _endCurrent = _startCurrent + separatorIndex;
            _startNext = _endCurrent + separatorLength;
        }
        else
        {
            _startNext = _endCurrent = _source.Length;

            // Set _splitMode to None so that subsequent MoveNext calls will return false.
            _splitMode = SpanSplitEnumeratorMode.None;
        }

        return true;
    }
    private enum SpanSplitEnumeratorMode
    {
        /// <summary>Either a default <see cref="SpanSplitEnumerator{T}"/> was used, or the enumerator has finished enumerating and there's no more work to do.</summary>
        None = 0,

        /// <summary>A single T separator was provided.</summary>
        SingleElement,

        /// <summary>A span of separators was provided, each of which should be treated independently.</summary>
        Any,

        /// <summary>The separator is a span of elements to be treated as a single sequence.</summary>
        Sequence,

        /// <summary>The separator is an empty sequence, such that no splits should be performed.</summary>
        EmptySequence,
    }
}

