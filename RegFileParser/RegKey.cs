namespace Nefarius.Utilities.Registry;

public unsafe struct RegKey
{
    internal RegKey(int start, int length, char* content)
    {
        Start = start;
        Length = length;
        _content = content;
    }

    private readonly char* _content;

    internal int Start { get; }

    internal int Length { get; }
}
