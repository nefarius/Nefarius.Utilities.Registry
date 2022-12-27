using Ardalis.SmartEnum;

namespace Nefarius.Utilities.Registry;

public sealed class RegValueType : SmartEnum<RegValueType>
{
    public static readonly RegValueType ResourceRequirementsList = new("REG_RESOURCE_REQUIREMENTS_LIST", 1);

    public static readonly RegValueType Qword = new("REG_QWORD", 2);

    public static readonly RegValueType Dword = new("REG_DWORD", 3);

    public static readonly RegValueType MultiSz = new("REG_MULTI_SZ", 4);

    public static readonly RegValueType Link = new("REG_LINK", 5);

    public static readonly RegValueType ExpandSz = new("REG_EXPAND_SZ", 6);

    public static readonly RegValueType None = new("REG_NONE", 7);

    public static readonly RegValueType Binary = new("REG_BINARY", 8);

    public static readonly RegValueType Sz = new("REG_SZ", 9);

    internal RegValueType(string name, int value) : base(name, value)
    {
    }
}