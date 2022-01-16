using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace Twisted.Tests.PS.V1.PSX;

//[TestClass]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Tests_TMS
{
    [TestMethod]
    public void TM1PSEUR_UA2PLAY_2PARENA_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UA2PLAY\2PARENA.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UA2PLAY_2PCANALS_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UA2PLAY\2PCANALS.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UA2PLAY_2PFWY_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UA2PLAY\2PFWY.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UA2PLAY_2PPARK_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UA2PLAY\2PPARK.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UA2PLAY_2PROOF_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UA2PLAY\2PROOF.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UA2PLAY_2PWH_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UA2PLAY\2PWH.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UA2PLAY_XARENA1_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UA2PLAY\XARENA1.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_CARS_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\CARS.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_CARSEND_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\CARSEND.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_PARK1_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\PARK1.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_PARK2_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\PARK2.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_ROOF1_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\ROOF1.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_ROOF2_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\ROOF2.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_SUBURB1_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\SUBURB1.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_SUBURB2_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\SUBURB2.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_WH1_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\WH1.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_WH2_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\WH2.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_XARENA1_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\XARENA1.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_XARENA2_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\XARENA2.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_XFWY1_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\XFWY1.TMS");
    }

    [TestMethod]
    public void TM1PSEUR_UATMS_XFWY2_TMS()
    {
        TestTMS(@".twisted\TM1PSEUR\UATMS\XFWY2.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UA2PLAY_2PARENA_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UA2PLAY\2PARENA.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UA2PLAY_2PCANALS_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UA2PLAY\2PCANALS.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UA2PLAY_2PFWY_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UA2PLAY\2PFWY.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UA2PLAY_2PPARK_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UA2PLAY\2PPARK.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UA2PLAY_2PROOF_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UA2PLAY\2PROOF.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UA2PLAY_2PWH_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UA2PLAY\2PWH.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UA2PLAY_XARENA1_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UA2PLAY\XARENA1.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_CARS_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\CARS.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_CARSEND_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\CARSEND.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_PARK1_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\PARK1.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_PARK2_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\PARK2.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_ROOF1_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\ROOF1.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_ROOF2_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\ROOF2.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_SUBURB1_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\SUBURB1.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_SUBURB2_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\SUBURB2.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_WH1_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\WH1.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_WH2_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\WH2.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_XARENA1_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\XARENA1.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_XARENA2_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\XARENA2.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_XFWY1_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\XFWY1.TMS");
    }

    [TestMethod]
    public void TM1PSJAP_UATMS_XFWY2_TMS()
    {
        TestTMS(@".twisted\TM1PSJAP\UATMS\XFWY2.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UA2PLAY_2PARENA_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UA2PLAY\2PARENA.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UA2PLAY_2PCANALS_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UA2PLAY\2PCANALS.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UA2PLAY_2PFWY_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UA2PLAY\2PFWY.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UA2PLAY_2PPARK_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UA2PLAY\2PPARK.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UA2PLAY_2PROOF_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UA2PLAY\2PROOF.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UA2PLAY_2PWH_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UA2PLAY\2PWH.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UA2PLAY_XARENA1_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UA2PLAY\XARENA1.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_CARS_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\CARS.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_CARSEND_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\CARSEND.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_PARK1_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\PARK1.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_PARK2_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\PARK2.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_ROOF1_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\ROOF1.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_ROOF2_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\ROOF2.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_SUBURB1_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\SUBURB1.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_SUBURB2_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\SUBURB2.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_WH1_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\WH1.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_WH2_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\WH2.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_XARENA1_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\XARENA1.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_XARENA2_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\XARENA2.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_XFWY1_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\XFWY1.TMS");
    }

    [TestMethod]
    public void TM1PSUSA_UATMS_XFWY2_TMS()
    {
        TestTMS(@".twisted\TM1PSUSA\UATMS\XFWY2.TMS");
    }

    private void TestTMS(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
    }
}