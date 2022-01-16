using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Tests.PS.V1.Extensions;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace Twisted.Tests.PS.V1.PC;

//[TestClass]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Tests_DPC
{
    [PublicAPI]
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void Test_UA2PLAY_2PARENA_DPC()
    {
        Test_DPC(@"GameData\UA2PLAY\2PARENA.DPC");
    }

    [TestMethod]
    public void Test_UA2PLAY_2PCANALS_DPC()
    {
        Test_DPC(@"GameData\UA2PLAY\2PCANALS.DPC");
    }

    [TestMethod]
    public void Test_UA2PLAY_2PFWY_DPC()
    {
        Test_DPC(@"GameData\UA2PLAY\2PFWY.DPC");
    }

    [TestMethod]
    public void Test_UA2PLAY_2PPARK_DPC()
    {
        Test_DPC(@"GameData\UA2PLAY\2PPARK.DPC");
    }

    [TestMethod]
    public void Test_UA2PLAY_2PROOF_DPC()
    {
        Test_DPC(@"GameData\UA2PLAY\2PROOF.DPC");
    }

    [TestMethod]
    public void Test_UA2PLAY_2PWH_DPC()
    {
        Test_DPC(@"GameData\UA2PLAY\2PWH.DPC");
    }

    [TestMethod]
    public void Test_UADPC_PARK1_DPC()
    {
        Test_DPC(@"GameData\UADPC\PARK1.DPC");
    }

    [TestMethod]
    public void Test_UADPC_PARK2_DPC()
    {
        Test_DPC(@"GameData\UADPC\PARK2.DPC");
    }

    [TestMethod]
    public void Test_UADPC_ROOF1_DPC()
    {
        Test_DPC(@"GameData\UADPC\ROOF1.DPC");
    }

    [TestMethod]
    public void Test_UADPC_ROOF2_DPC()
    {
        Test_DPC(@"GameData\UADPC\ROOF2.DPC");
    }

    [TestMethod]
    public void Test_UADPC_SUBURB1_DPC()
    {
        Test_DPC(@"GameData\UADPC\SUBURB1.DPC");
    }

    [TestMethod]
    public void Test_UADPC_SUBURB2_DPC()
    {
        Test_DPC(@"GameData\UADPC\SUBURB2.DPC");
    }

    [TestMethod]
    public void Test_UADPC_WH1_DPC()
    {
        Test_DPC(@"GameData\UADPC\WH1.DPC");
    }

    [TestMethod]
    public void Test_UADPC_WH2_DPC()
    {
        Test_DPC(@"GameData\UADPC\WH2.DPC");
    }

    [TestMethod]
    public void Test_UADPC_XARENA1_DPC()
    {
        Test_DPC(@"GameData\UADPC\XARENA1.DPC");
    }

    [TestMethod]
    public void Test_UADPC_XARENA2_DPC()
    {
        Test_DPC(@"GameData\UADPC\XARENA2.DPC");
    }

    [TestMethod]
    public void Test_UADPC_XFWY1_DPC()
    {
        Test_DPC(@"GameData\UADPC\XFWY1.DPC");
    }

    [TestMethod]
    public void Test_UADPC_XFWY2_DPC()
    {
        Test_DPC(@"GameData\UADPC\XFWY2.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_AUDSRN_DPC()
    {
        Test_DPC(@"GameData\UASHELL\AUDSRN.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_CALYSRN_DPC()
    {
        Test_DPC(@"GameData\UASHELL\CALYSRN.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_CARCARDS_DPC()
    {
        Test_DPC(@"GameData\UASHELL\CARCARDS.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_CAREND_DPC()
    {
        Test_DPC(@"GameData\UASHELL\CAREND.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_CARS_DPC()
    {
        Test_DPC(@"GameData\UASHELL\CARS.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_COPEND_DPC()
    {
        Test_DPC(@"GameData\UASHELL\COPEND.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_DEVSCRN_DPC()
    {
        Test_DPC(@"GameData\UASHELL\DEVSCRN.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_INFOSTAT_DPC()
    {
        Test_DPC(@"GameData\UASHELL\INFOSTAT.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_INSCRN_DPC()
    {
        Test_DPC(@"GameData\UASHELL\INSCRN.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_KEYSCRN_DPC()
    {
        Test_DPC(@"GameData\UASHELL\KEYSCRN.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_KIRKSCRN_DPC()
    {
        Test_DPC(@"GameData\UASHELL\KIRKSCRN.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_LEGAL_DPC()
    {
        Test_DPC(@"GameData\UASHELL\LEGAL.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_MMAXEND_DPC()
    {
        Test_DPC(@"GameData\UASHELL\MMAXEND.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_NETSCRN_DPC()
    {
        Test_DPC(@"GameData\UASHELL\NETSCRN.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_OPTSRN_DPC()
    {
        Test_DPC(@"GameData\UASHELL\OPTSRN.DPC");
    }

    [TestMethod]
    public void Test_UASHELL_TTLSRN_DPC()
    {
        Test_DPC(@"GameData\UASHELL\TTLSRN.DPC");
    }

    private void Test_DPC(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

        path = Storage.GetPath(path);

        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream, Encoding.Default, true);

        var magic = reader.ReadStringAscii(4);
        if (magic != "DCPM")
            throw new InvalidDataException("Not a DPC file.");

        var version = reader.ReadInt32();
        if (version != 0x00000043)
            throw new InvalidDataException($"Invalid version: 0x{version:X8}");

        var time = DateTime.UnixEpoch.AddSeconds(reader.ReadInt32());

        TestContext.Write($"{nameof(time)}: {time}");

        var startingAddress = reader.ReadInt32();
        Assert.AreEqual(unchecked((int)0x800188B8), startingAddress);

        int ReadAddress()
        {
            return reader.ReadInt32() - startingAddress;
        }

        int ReadAddress1(BinaryReader binaryReader)
        {
            return binaryReader.ReadInt32() - startingAddress;
        }

        var primitiveHeaderSection = ReadAddress();
        Assert.AreEqual(20, primitiveHeaderSection);
        stream.Position = primitiveHeaderSection;
        var numberOfPrimitiveBlocks = reader.ReadInt32();
        // Assert.AreEqual(1, int32);

        var read = reader.Read(ReadAddress1, numberOfPrimitiveBlocks);

        foreach (var address in read)
        {
            Assert.IsTrue(address >= 0 && address < stream.Length);
            stream.Position = address;
            var int16 = reader.ReadInt16();
            int16 = (short)((int16 >> 8) | (int16 << 8));
            Assert.AreEqual(0x0107, int16);
            var unknown1 = reader.ReadInt16();
            var ints     = reader.Read(s => s.ReadInt32(), 4);
            var count    = reader.ReadInt32();
            // Assert.AreEqual(1, count, stream.Position.ToString());
        }
    }
}