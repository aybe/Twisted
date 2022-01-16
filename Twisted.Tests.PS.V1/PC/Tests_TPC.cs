using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace Twisted.Tests.PS.V1.PC;

//[TestClass]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Tests_TPC
{
    [TestMethod]
    public void Test_UA2PLAY_2PARENA_TPC()
    {
        Test_TPC(@"GameData\UA2PLAY\2PARENA.TPC");
    }

    [TestMethod]
    public void Test_UA2PLAY_2PCANALS_TPC()
    {
        Test_TPC(@"GameData\UA2PLAY\2PCANALS.TPC");
    }

    [TestMethod]
    public void Test_UA2PLAY_2PFWY_TPC()
    {
        Test_TPC(@"GameData\UA2PLAY\2PFWY.TPC");
    }

    [TestMethod]
    public void Test_UA2PLAY_2PPARK_TPC()
    {
        Test_TPC(@"GameData\UA2PLAY\2PPARK.TPC");
    }

    [TestMethod]
    public void Test_UA2PLAY_2PROOF_TPC()
    {
        Test_TPC(@"GameData\UA2PLAY\2PROOF.TPC");
    }

    [TestMethod]
    public void Test_UA2PLAY_2PWH_TPC()
    {
        Test_TPC(@"GameData\UA2PLAY\2PWH.TPC");
    }

    [TestMethod]
    public void Test_UADPC_PARK1_TPC()
    {
        Test_TPC(@"GameData\UADPC\PARK1.TPC");
    }

    [TestMethod]
    public void Test_UADPC_PARK2_TPC()
    {
        Test_TPC(@"GameData\UADPC\PARK2.TPC");
    }

    [TestMethod]
    public void Test_UADPC_ROOF1_TPC()
    {
        Test_TPC(@"GameData\UADPC\ROOF1.TPC");
    }

    [TestMethod]
    public void Test_UADPC_ROOF2_TPC()
    {
        Test_TPC(@"GameData\UADPC\ROOF2.TPC");
    }

    [TestMethod]
    public void Test_UADPC_SUBURB1_TPC()
    {
        Test_TPC(@"GameData\UADPC\SUBURB1.TPC");
    }

    [TestMethod]
    public void Test_UADPC_SUBURB2_TPC()
    {
        Test_TPC(@"GameData\UADPC\SUBURB2.TPC");
    }

    [TestMethod]
    public void Test_UADPC_WH1_TPC()
    {
        Test_TPC(@"GameData\UADPC\WH1.TPC");
    }

    [TestMethod]
    public void Test_UADPC_WH2_TPC()
    {
        Test_TPC(@"GameData\UADPC\WH2.TPC");
    }

    [TestMethod]
    public void Test_UADPC_XARENA1_TPC()
    {
        Test_TPC(@"GameData\UADPC\XARENA1.TPC");
    }

    [TestMethod]
    public void Test_UADPC_XARENA2_TPC()
    {
        Test_TPC(@"GameData\UADPC\XARENA2.TPC");
    }

    [TestMethod]
    public void Test_UADPC_XFWY1_TPC()
    {
        Test_TPC(@"GameData\UADPC\XFWY1.TPC");
    }

    [TestMethod]
    public void Test_UADPC_XFWY2_TPC()
    {
        Test_TPC(@"GameData\UADPC\XFWY2.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_AUDSRN_TPC()
    {
        Test_TPC(@"GameData\UASHELL\AUDSRN.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_CALYSRN_TPC()
    {
        Test_TPC(@"GameData\UASHELL\CALYSRN.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_CARCARDS_TPC()
    {
        Test_TPC(@"GameData\UASHELL\CARCARDS.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_CAREND_TPC()
    {
        Test_TPC(@"GameData\UASHELL\CAREND.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_CARS_TPC()
    {
        Test_TPC(@"GameData\UASHELL\CARS.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_COPEND_TPC()
    {
        Test_TPC(@"GameData\UASHELL\COPEND.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_DEVSCRN_TPC()
    {
        Test_TPC(@"GameData\UASHELL\DEVSCRN.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_INFOSTAT_TPC()
    {
        Test_TPC(@"GameData\UASHELL\INFOSTAT.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_INSCRN_TPC()
    {
        Test_TPC(@"GameData\UASHELL\INSCRN.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_KEYSCRN_TPC()
    {
        Test_TPC(@"GameData\UASHELL\KEYSCRN.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_KIRKSCRN_TPC()
    {
        Test_TPC(@"GameData\UASHELL\KIRKSCRN.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_LEGAL_TPC()
    {
        Test_TPC(@"GameData\UASHELL\LEGAL.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_MMAXEND_TPC()
    {
        Test_TPC(@"GameData\UASHELL\MMAXEND.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_NETSCRN_TPC()
    {
        Test_TPC(@"GameData\UASHELL\NETSCRN.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_OPTSRN_TPC()
    {
        Test_TPC(@"GameData\UASHELL\OPTSRN.TPC");
    }

    [TestMethod]
    public void Test_UASHELL_TTLSRN_TPC()
    {
        Test_TPC(@"GameData\UASHELL\TTLSRN.TPC");
    }

    private void Test_TPC(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

        path = Storage.GetPath(path);

        using var stream = File.OpenRead(path);

        new TPC(stream);
        Assert.AreEqual(stream.Length, stream.Position);
    }
}