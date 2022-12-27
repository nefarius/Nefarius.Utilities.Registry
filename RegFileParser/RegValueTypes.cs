using Ardalis.SmartEnum;

namespace Nefarius.Utilities.Registry;

/// <summary>
///     Represents possible data types of a <see cref="RegValue"/>.
/// </summary>
public sealed class RegValueType : SmartEnum<RegValueType>
{
    /// <summary>
    ///     A device driver's list of possible hardware resources it or one of the physical devices it controls can use, from
    ///     which the system writes a subset into the \ResourceMap tree.
    /// </summary>
    public static readonly RegValueType ResourceRequirementsList = new("REG_RESOURCE_REQUIREMENTS_LIST", 10);

    /// <summary>
    ///     A 64-bit number.
    /// </summary>
    public static readonly RegValueType Qword = new("REG_QWORD", 11);

    /// <summary>
    ///     A 32-bit number.
    /// </summary>
    public static readonly RegValueType Dword = new("REG_DWORD", 4);

    /// <summary>
    ///     A sequence of null-terminated strings, terminated by an empty string (\0). The following is an example:
    ///     String1\0String2\0String3\0LastString\0\0 The first \0 terminates the first string, the second to the last \0
    ///     terminates the last string, and the final \0 terminates the sequence. Note that the final terminator must be
    ///     factored into the length of the string.
    /// </summary>
    public static readonly RegValueType MultiSz = new("REG_MULTI_SZ", 7);

    /// <summary>
    ///     A null-terminated Unicode string that contains the target path of a symbolic link that was created by calling the
    ///     RegCreateKeyEx function with REG_OPTION_CREATE_LINK.
    /// </summary>
    public static readonly RegValueType Link = new("REG_LINK", 6);

    /// <summary>
    ///     A null-terminated string that contains unexpanded references to environment variables (for example, "%PATH%"). It
    ///     will be a Unicode or ANSI string depending on whether you use the Unicode or ANSI functions. To expand the
    ///     environment variable references, use the ExpandEnvironmentStrings function.
    /// </summary>
    public static readonly RegValueType ExpandSz = new("REG_EXPAND_SZ", 2);

    /// <summary>
    ///     No defined value type.
    /// </summary>
    public static readonly RegValueType None = new("REG_NONE", 0);

    /// <summary>
    ///     Binary data in any form.
    /// </summary>
    public static readonly RegValueType Binary = new("REG_BINARY", 3);

    /// <summary>
    ///     A null-terminated string. This will be either a Unicode or an ANSI string, depending on whether you use the Unicode
    ///     or ANSI functions.
    /// </summary>
    public static readonly RegValueType Sz = new("REG_SZ", 1);

    internal RegValueType(string name, int value) : base(name, value)
    {
    }
}