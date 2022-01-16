using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests.PS.V1.PSX;

[TestClass]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class Tests_DMD_TM1PSUSA_UA2PLAY : Tests_DMD
{
    [TestMethod]
    public void TM1PSUSA_UA2PLAY_2PARENA_DMD()
    {
        TestDMD(@".twisted\TM1PSUSA\UA2PLAY\2PARENA.DMD");
    }

    [TestMethod]
    public void TM1PSUSA_UA2PLAY_2PCANALS_DMD()
    {
        TestDMD(@".twisted\TM1PSUSA\UA2PLAY\2PCANALS.DMD");
    }

    [TestMethod]
    public void TM1PSUSA_UA2PLAY_2PFWY_DMD()
    {
        TestDMD(@".twisted\TM1PSUSA\UA2PLAY\2PFWY.DMD");
    }

    [TestMethod]
    public void TM1PSUSA_UA2PLAY_2PPARK_DMD()
    {
        TestDMD(@".twisted\TM1PSUSA\UA2PLAY\2PPARK.DMD");
    }

    [TestMethod]
    public void TM1PSUSA_UA2PLAY_2PROOF_DMD()
    {
        TestDMD(@".twisted\TM1PSUSA\UA2PLAY\2PROOF.DMD");
    }

    [TestMethod]
    public void TM1PSUSA_UA2PLAY_2PWH_DMD()
    {
        TestDMD(@".twisted\TM1PSUSA\UA2PLAY\2PWH.DMD");
    }

    [TestMethod]
    public void TM1PSUSA_UA2PLAY_XARENA1_DMD()
    {
        TestDMD(@".twisted\TM1PSUSA\UA2PLAY\XARENA1.DMD");
    }
}