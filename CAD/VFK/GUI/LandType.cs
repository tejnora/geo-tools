using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VFK.GUI
{
    [Serializable]
    public class LandTypes
    {
        public List<LandTypeNode> LandTypeNodes
        {
            get; set;
        }
        private LandTypes()
        {
            LandUseNode sklenik = new LandUseNode() {KOD = 1, NAZEV = "skleník, pařeniště"};
            LandUseNode skola = new LandUseNode(){KOD = 2, NAZEV = "škola"};
            LandUseNode plantazDreviny = new LandUseNode() {KOD = 3, NAZEV = "plantáž dřeviny"};
            LandUseNode lesJinyNezHospodarsky = new LandUseNode() {KOD = 4, NAZEV = "les jiný neľ hospodářský"};
            LandUseNode lesniPozemek = new LandUseNode() { KOD = 5, NAZEV = "lesní pozemek, na kterém je budova" };
            LandUseNode rybnik = new LandUseNode() { KOD = 6, NAZEV = "rybník" };
            LandUseNode korytoPrirozene = new LandUseNode() { KOD = 7, NAZEV = "koryto vodního toku přirozené nebo upravené" };
            LandUseNode korytoUmele = new LandUseNode() {KOD = 8, NAZEV = "koryto vodního toku umělé"};
            LandUseNode vodniNadrzPrirodni = new LandUseNode() { KOD = 9, NAZEV = "vodní nádrž přírodní" };
            LandUseNode vodniNadrzUmela = new LandUseNode() { KOD = 10, NAZEV = "vodní nádrž umělá" };
            LandUseNode zamokrenaPlocha = new LandUseNode() { KOD = 11, NAZEV = "zamokřená plocha" };
            LandUseNode spolecnyDvur = new LandUseNode() { KOD = 12, NAZEV = "společný dvůr" };
            LandUseNode zboreniste = new LandUseNode() { KOD = 13, NAZEV = "zbořeniště" };
            LandUseNode draha = new LandUseNode() { KOD = 14, NAZEV = "dráha" };
            LandUseNode dalnice = new LandUseNode() { KOD = 15, NAZEV = "dálnice" };
            LandUseNode silnice = new LandUseNode() { KOD = 16, NAZEV = "silnice" };
            LandUseNode ostatniKomunikace = new LandUseNode() { KOD = 17, NAZEV = "ostatní komunikace" };
            LandUseNode dopravniPlocha = new LandUseNode() { KOD = 18, NAZEV = "ostatní dopravní plocha" };
            LandUseNode zelen = new LandUseNode() { KOD = 19, NAZEV = "zeleň" };
            LandUseNode sportoviste = new LandUseNode() { KOD = 20, NAZEV = "sportoviątě a rekreační plocha" };
            LandUseNode hrbitov = new LandUseNode() { KOD = 21, NAZEV = "hřbitov, urnový háj" };
            LandUseNode kulturniPlocha = new LandUseNode() { KOD = 22, NAZEV = "kulturní a osvětová plocha" };
            LandUseNode manipPlocha = new LandUseNode() { KOD = 23, NAZEV = "manipulační plocha" };
            LandUseNode dobyvaciProstro = new LandUseNode() { KOD = 24, NAZEV = "dobývací prostor" };
            LandUseNode skadka = new LandUseNode() { KOD = 25, NAZEV = "skládka" };
            LandUseNode jinaPlocha = new LandUseNode() { KOD = 26, NAZEV = "jiná plocha" };
            LandUseNode neplodnaPlocha = new LandUseNode() { KOD = 27, NAZEV = "neplodná půda" };
            LandUseNode vodniPlocha = new LandUseNode() { KOD = 28, NAZEV = "vodní plocha, na které je budova" };
            LandUseNode undefined = new LandUseNode(){KOD = UInt32.MaxValue,NAZEV = "----------------"};
            LandTypeNodes = new List<LandTypeNode>();
            LandTypeNode node = new LandTypeNode() { KOD = UInt32.MaxValue, NAZEV = "-----------", ZKRATKA = "----------" };
            node.LandUseNode=new List<LandUseNode>(){undefined};
            LandTypeNodes.Add(node);
            node = new LandTypeNode(){KOD = 2, NAZEV = "orná půda", ZKRATKA = "orná půda", STAVEBNI_PARCELA = false};
            node.LandUseNode = new List<LandUseNode>() { sklenik, skola, plantazDreviny, dobyvaciProstro, undefined };
            LandTypeNodes.Add(node);
            node = new LandTypeNode() {KOD = 3, NAZEV = "chmelnice", ZKRATKA = "chmelnice", STAVEBNI_PARCELA = false};
            node.LandUseNode = new List<LandUseNode>() { sklenik, skola, plantazDreviny, dobyvaciProstro, undefined };LandTypeNodes.Add(node);
            node = new LandTypeNode() {KOD = 4, NAZEV = "vinice", ZKRATKA = "vinice", STAVEBNI_PARCELA = false};
            node.LandUseNode = new List<LandUseNode>() { sklenik, skola, plantazDreviny, dobyvaciProstro, undefined }; LandTypeNodes.Add(node);
            node = new LandTypeNode() {KOD = 5, NAZEV = "zahrada", ZKRATKA = "zahrada", STAVEBNI_PARCELA = false};
            node.LandUseNode = new List<LandUseNode>() { sklenik, skola, plantazDreviny, dobyvaciProstro, undefined }; LandTypeNodes.Add(node);
            node = new LandTypeNode() {KOD = 6, NAZEV = "ovocný sad", ZKRATKA = "ovocný sad", STAVEBNI_PARCELA = false};
            node.LandUseNode = new List<LandUseNode>() { sklenik, skola, plantazDreviny, dobyvaciProstro, undefined }; LandTypeNodes.Add(node);
            node=new LandTypeNode() { KOD = 7, NAZEV = "trvalý travní porost", ZKRATKA = "trvalý travní porost", STAVEBNI_PARCELA = false };
            node.LandUseNode = new List<LandUseNode>() { sklenik, skola, plantazDreviny, dobyvaciProstro, undefined }; LandTypeNodes.Add(node);
            node=new LandTypeNode() { KOD = 10, NAZEV = "lesní pozemek", ZKRATKA = "lesní poz", STAVEBNI_PARCELA = false };
            node.LandUseNode = new List<LandUseNode>() { sklenik, skola, plantazDreviny, lesJinyNezHospodarsky, lesniPozemek, ostatniKomunikace,
            sportoviste,dobyvaciProstro, undefined};
            LandTypeNodes.Add(node);
            node=new LandTypeNode() { KOD = 11, NAZEV = "vodní plocha", ZKRATKA = "vodní pl.", STAVEBNI_PARCELA = false };
            node.LandUseNode = new List<LandUseNode>(){rybnik, korytoPrirozene, korytoUmele, vodniNadrzPrirodni, vodniNadrzUmela,
                zamokrenaPlocha, vodniPlocha, undefined}; 
            LandTypeNodes.Add(node);
            node=new LandTypeNode() { KOD = 13, NAZEV = "zastavěná plocha a nádvoří", ZKRATKA = "zast. pl.", STAVEBNI_PARCELA = true };
            node.LandUseNode = new List<LandUseNode>() { spolecnyDvur, zboreniste, undefined };
            LandTypeNodes.Add(node);
            node=new LandTypeNode() { KOD = 14, NAZEV = "ostatní plocha", ZKRATKA = "ostat.pl.", STAVEBNI_PARCELA = false };
            node.LandUseNode = new List<LandUseNode>() { plantazDreviny, draha, dalnice, silnice, ostatniKomunikace,
            dopravniPlocha,zelen,hrbitov,kulturniPlocha,dobyvaciProstro,skadka,jinaPlocha,manipPlocha,neplodnaPlocha, undefined};
            LandTypeNodes.Add(node);
        }
    }

    [Serializable]
    public class LandTypeNode
    {
        public List<LandUseNode> LandUseNode
        {
            get; set;
        }
        public UInt32 KOD
        {
            get; set;
        }
        public string NAZEV
        {
            get; set;
        }
        public string ZKRATKA
        {
            get; set;
        }
        public bool STAVEBNI_PARCELA
        {
            get; set;
        }
    }

    [Serializable]
    public class LandUseNode
    {
        public UInt32 KOD
        {
            get; set;
        }
        public string NAZEV
        {
            get; set;
        }
    }
}
