using Nefarius.Utilities.Registry;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestParsingBigRegFile()
    {
        RegFile file = new(@"D:\Development\GitHub\Nefarius.Utilities.Registry\software_export.reg");

        KeyValuePair<string, Dictionary<string, RegValue>> cachedEntries = file.RegValues.First();

        Dictionary<string, RegValueBinary> binValues = cachedEntries.Value
            .Where(v => v.Value.Type == RegValueType.Binary)
            .ToDictionary(pair => pair.Key, pair => (RegValueBinary)pair.Value);

        Assert.That(binValues, Is.Not.Empty);

        KeyValuePair<string, RegValueBinary>? recordEntry =
            binValues.FirstOrDefault(pair => pair.Value.Value.First() == 0x36);

        Assert.That(recordEntry, Is.Not.Null);

        byte[] recordBlob = recordEntry.Value.Value.Value.ToArray();

        Assert.That(recordBlob, Is.Not.Empty);
    }
}