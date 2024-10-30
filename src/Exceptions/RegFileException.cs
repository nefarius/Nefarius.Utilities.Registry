using System;
using System.Diagnostics.CodeAnalysis;

namespace Nefarius.Utilities.Registry.Exceptions;

/// <summary>
///     A <see cref="RegFile" /> exception.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class RegFileException : Exception
{
    internal RegFileException(string message) : base(message)
    {
    }

    internal RegFileException(string message, Exception inner) : base(message, inner)
    {
    }
}