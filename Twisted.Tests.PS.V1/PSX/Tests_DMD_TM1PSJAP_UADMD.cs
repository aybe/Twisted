using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests.PS.V1.PSX;

[TestClass]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class Tests_DMD_TM1PSJAP_UADMD : Tests_DMD
{
    [TestMethod]
    public void TM1PSJAP_UADMD_CARS_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\CARS.DMD");
    }

    [TestMethod]
    public void TM1PSJAP_UADMD_CARSEND_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\CARSEND.DMD");
    }

    [TestMethod]
    public void TM1PSJAP_UADMD_PARK1_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\PARK1.DMD");
    }

    [TestMethod]
    public void TM1PSJAP_UADMD_PARK2_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\PARK2.DMD");
    }

    [TestMethod]
    public void TM1PSJAP_UADMD_ROOF1_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\ROOF1.DMD");
    }

    [TestMethod]
    public void TM1PSJAP_UADMD_ROOF2_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\ROOF2.DMD");
    }

    [TestMethod]
    public void TM1PSJAP_UADMD_SUBURB1_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\SUBURB1.DMD");
    }

    [TestMethod]
    public void TM1PSJAP_UADMD_SUBURB2_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\SUBURB2.DMD");
    }

    [TestMethod]
    public void TM1PSJAP_UADMD_WH1_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\WH1.DMD");
    }

    [TestMethod]
    public void TM1PSJAP_UADMD_WH2_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\WH2.DMD");
    }

    [TestMethod]
    public void TM1PSJAP_UADMD_XARENA1_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\XARENA1.DMD");
    }

    [TestMethod]
    public void TM1PSJAP_UADMD_XARENA2_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\XARENA2.DMD");
    }

    [TestMethod]
    public void TM1PSJAP_UADMD_XFWY1_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\XFWY1.DMD");
    }

    [TestMethod]
    public void TM1PSJAP_UADMD_XFWY2_DMD()
    {
        TestDMD(@".twisted\TM1PSJAP\UADMD\XFWY2.DMD");
    }
}