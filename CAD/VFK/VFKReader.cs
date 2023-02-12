using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CAD.VFK;
using VFK.Tables;
using VFK.GUI;

namespace VFK
{
    class VFKReader
    {
        private VFKMain _vfkMain;
        VFK.GUI.ImportInfoDialogContext _ImportDialogContext;
        public VFKReader(VFKMain aVFKMain, VFK.GUI.ImportInfoDialogContext aImportDialogContext)
        {
            _vfkMain = aVFKMain;
            _ImportDialogContext = aImportDialogContext;
        }

        public void parseFile(string aName)
        {
            Encoding enc = Encoding.UTF8;
            using (Stream fileStream = new FileStream(aName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(fileStream, enc))
                {
                    int lineNumber = 1;
                    string singleLine = string.Empty;
                    while (!reader.EndOfStream)
                    {
                        singleLine += reader.ReadLine();
                        if (singleLine.Length < 1) continue;
                        if (singleLine[singleLine.Length - 1] == 0xA4)
                        {
                            singleLine = singleLine.Remove(singleLine.Length - 1);
                            singleLine += "\r\n";
                            continue;
                        }
                        processLine(singleLine, lineNumber++);
                        singleLine = string.Empty;
                    }
                }
            }
        }
        bool _HeaderEnded;
        public bool processLine(string aText, int lineNumber)
        {
            if (aText.Length < 2)
            {
                addErrorMessage("Unexpected line.", lineNumber, 1);
                return false;
            }
            if (aText[0] != '&')
            {
                addErrorMessage("Unexpected character", lineNumber, 5);
                return false;
            }
            switch (aText[1])
            {
                case 'H':
                    return parseHeader(aText, lineNumber);
                case 'B':
                    {
                        _HeaderEnded = true;
                        return parseFileBlockHeader(aText, lineNumber);
                    }
                case 'D':
                    {
                        if (_HeaderEnded)
                            return parseFileBlockData(aText, lineNumber);
                        return true;
                    }
            }
            return false;
        }

        public bool parseHeader(string aText, int aLineNumber)
        {
            int semicolonPos = aText.IndexOf(';');
            if (semicolonPos == -1)
            {
                addErrorMessage("In heder line cound not be found semicolon.", aLineNumber, 2);
                return false;
            }
            string name = aText.Substring(2, semicolonPos - 2);
            semicolonPos++;
            string subString = Utils.getCssStringValue(aText, ref semicolonPos);
            switch (name)
            {
                case "VERZE":
                    {
                        var res = false;
                        _vfkMain.Header.VERZE = (float)Utils.parseDouble(subString, ref res);
                        if (_vfkMain.Header.VERZE < 5.0)
                            throw new VfkImportException("Vfk file version is not supported.");
                        if (!res)
                            goto end;
                        _ImportDialogContext.Verze = subString;
                        return true;
                    }
                case "VYTVORENO":
                    {
                        bool res = false;
                        _vfkMain.Header.VYTVORENO = Utils.parseDateTime(subString, ref res);
                        if (!res)
                            goto end;
                        _ImportDialogContext.Vytvoreno = subString;
                        return true;
                    }
                case "PUVOD":
                    {
                        _vfkMain.Header.PUVOD = subString;
                        return true;
                    }
                case "CODEPAGE":
                    {
                        _ImportDialogContext.KodovaStranka = subString;
                        return true;
                    }
                case "SKUPINA":
                    {
                        _vfkMain.Header.SKUPINA = subString;
                        return true;
                    }
                case "JMENO":
                    {
                        _vfkMain.Header.JMENO = subString;
                        _ImportDialogContext.Vytvoril = subString;
                        return true;
                    }
                case "PLATNOST":
                    {
                        bool res = false;
                        _vfkMain.Header.PLATNOST_OD = Utils.parseDateTime(subString, ref res);
                        if (!res)
                            goto end;
                        subString = Utils.getCssStringValue(aText, ref semicolonPos);
                        _vfkMain.Header.PLATNOST_DO = Utils.parseDateTime(subString, ref res);
                        if (!res)
                            goto end;
                        _ImportDialogContext.ExportZaobdobi = _vfkMain.Header.PLATNOST_OD.ToString() + " - " + _vfkMain.Header.PLATNOST_DO.ToString();
                        return true;
                    }
                case "ZMENY":
                    {
                        bool res = false;
                        _vfkMain.Header.ZMENY = Utils.parseUInt16(subString, ref res);
                        if (!res)
                            goto end;
                        return true;
                    }
                case "NAVRHY":
                    {
                        bool res = false;
                        _vfkMain.Header.NAVRHY = Utils.parseUInt16(subString, ref res);
                        if (!res)
                            goto end;
                        return true;
                    }
                case "KATUZE":
                case "OPSUB":
                case "PAR":
                case "POLYG":
                    {
                        //TODO omezujici podminky
                        addErrorMessage("TODO omezujici podminky", aLineNumber, 0);
                        return true;
                    };
                case "OSOBDATA":
                    return true;
            }
        end:
            addErrorMessage(String.Format("Could not parse header {0}.", name), aLineNumber, 2);

            return false;
        }

        Dictionary<string, Definition> _mDefintions = new Dictionary<string, Definition>();
        public bool parseFileBlockHeader(string aText, int aLineNumber)
        {
            int semicolonPos = aText.IndexOf(';');
            if (semicolonPos == -1)
            {
                addErrorMessage("In block heder line cound not be found semicolon.", aLineNumber, 2);
                return false;
            }
            string name = aText.Substring(2, semicolonPos - 2);
            string definitionText = aText.Substring(semicolonPos + 1);
            switch (name)
            {
                case "PAR":
                case "BUD":
                case "BDP":
                case "RZO":
                case "SOBR":
                case "SBP":
                case "SBM":
                case "HP":
                case "ZVB":
                case "OB":
                case "OP":
                case "DPM":
                case "OBBP":
                case "ZPMZ":
                case "SPOL":
                case "KATUZE":
                case "MAPLIS":
                case "TEL":
                case "HBPEJ":
                case "OBPEJ":
                    _mDefintions[name] = new Definition(name, definitionText);
                    return true;
                /*case "ZPOCHN":
                case "DRUPOZ":
                case "ZPVYPO":
                case "ZDPAZE":
                case "ZPURVY":
                case "TYPBUD":
                case "OBCE":
                case "CABU":
                case "CASOBC":
                case "OKRESY":
                case "KRAJE":
                case "NKRAJE":
                case "ZPVYBU":
                case "OPSUB":
                case "VLA":
                case "CHAROS":
                case "JPV":
                case "TYPRAV":
                case "KODCHB":
                case "TYPSOS":
                case "TYPPPD":
                case "POM":
                case "SPOM":
                case "NZ":
                case "NZZP":
                case "RECI":
                case "DOCI":
                case "REZBP":*/
                default:
                    {
                        addErrorMessage(string.Format("Item {0} is not implemented", name), aLineNumber, 3);
                    } break;
            }
            return false;
        }
        public bool parseFileBlockData(string aText, int aLineNumber)
        {
            var semicolonPos = aText.IndexOf(';');
            if (semicolonPos == -1)
            {
                addErrorMessage("In data block line cound not be found semicolon.", aLineNumber, 2);
                return false;
            }
            var name = aText.Substring(2, semicolonPos - 2);
            var definitionText = aText.Substring(semicolonPos + 1);
            try
            {
                IVFKDataTableItem table = null;
                if (_mDefintions.ContainsKey(name))
                {
                    table = _mDefintions[name].GetVFKTable(definitionText);
                    table.MarkItemFromImport();
                }
                switch (name)
                {
                    case "PAR":
                        _vfkMain.VFKPARTable.addTableData((VFKPARTableItem)table);
                        _ImportDialogContext.PAR++;
                        break;
                    case "BUD":
                        _vfkMain.VFKBUDTable.addTableData((VFKBUDTableItem)table);
                        _ImportDialogContext.BUD++;
                        break;
                    case "BDP":
                        _vfkMain.VFKBDPTable.addTableData((VFKBDPTableItem)table);
                        _ImportDialogContext.BDP++;
                        break;
                    case "RZO":
                        _vfkMain.VFKRZOTable.addTableData((VFKRZOTableItem)table);
                        _ImportDialogContext.RZO++;
                        break;
                    case "SOBR":
                        _vfkMain.VFKSOBRTable.addTableData((VFKSOBRTableItem)table);
                        _ImportDialogContext.SOBR++;
                        break;
                    case "SBP":
                        _vfkMain.VFKSBPTable.addTableData((VFKSBPTableItem)table);
                        _ImportDialogContext.SBM++;
                        break;
                    case "SBM":
                        _vfkMain.VFKSBMTable.addTableData((VFKSBMTableItem)table);
                        _ImportDialogContext.SBM++;
                        break;
                    case "HP":
                        _vfkMain.VFKHPTable.addTableData((VFKHPTableItem)table);
                        _ImportDialogContext.HP++;
                        break;
                    case "ZVB":
                        _vfkMain.VFKZVBTable.addTableData((VFKZVBTableItem)table);
                        _ImportDialogContext.ZVB++;
                        break;
                    case "OB":
                        _vfkMain.VFKOBTable.addTableData((VFKOBTableItem)table);
                        _ImportDialogContext.OB++;
                        break;
                    case "OP":
                        _vfkMain.VFKOPTable.addTableData((VFKOPTableItem)table);
                        _ImportDialogContext.OP++;
                        break;
                    case "DPM":
                        _vfkMain.VFKDPMTable.addTableData((VFKDPMTableItem)table);
                        _ImportDialogContext.DPM++;
                        break;
                    case "OBBP":
                        _vfkMain.VFKOBBPTable.addTableData((VFKOBBPTableItem)table);
                        _ImportDialogContext.OBBP++;
                        break;
                    case "ZPMZ":
                        _vfkMain.VFKZPMZTable.addTableData((VFKZPMZTableItem)table);
                        _ImportDialogContext.ZPMZ++;
                        break;
                    case "SPOL":
                        _vfkMain.VFKSPOLTable.addTableData((VFKSPOLTableItem)table);
                        _ImportDialogContext.SPOL++;
                        break;
                    case "KATUZE":
                        _vfkMain.VFKKATUZETable.addTableData((VFKKATUZETableItem)table);
                        _ImportDialogContext.KATUZE++;
                        break;
                    case "ZPOCHN":
                        _ImportDialogContext.ZPOCHN++;
                        break;
                    case "DRUPOZ":
                        _ImportDialogContext.DRUPOZ++;
                        break;
                    case "ZPVYPO":
                        _ImportDialogContext.ZPVYPO++;
                        break;
                    case "ZDPAZE":
                        _ImportDialogContext.ZDPAZE++;
                        break;
                    case "ZPURVY":
                        _ImportDialogContext.ZPURVY++;
                        break;
                    case "TYPBUD":
                        _ImportDialogContext.TYPBUD++;
                        break;
                    case "MAPLIS":
                        _vfkMain.VFKMAPLISTable.addTableData((VFKMAPLISTableItem)table);
                        _ImportDialogContext.MAPLIS++;
                        break;
                    case "OBCE":
                        _ImportDialogContext.OBCE++;
                        break;
                    case "CABU":
                        _ImportDialogContext.CABU++;
                        break;
                    case "CASOBC":
                        _ImportDialogContext.CASOBC++;
                        break;
                    case "OKRESY":
                        _ImportDialogContext.OKRESY++;
                        break;
                    case "KRAJE":
                        _ImportDialogContext.KATUZE++;
                        break;
                    case "NKRAJE":
                        _ImportDialogContext.NKRAJE++;
                        break;
                    case "ZPVYBU":
                        _ImportDialogContext.ZPVYBU++;
                        break;
                    case "OPSUB":
                        _ImportDialogContext.OPSUB++;
                        break;
                    case "VLA":
                        _ImportDialogContext.VLA++;
                        break;
                    case "CHAROS":
                        _ImportDialogContext.CHAROS++;
                        break;
                    case "TEL":
                        _vfkMain.VfkTelTable.addTableData((VFKTELTableItem)table);
                        _ImportDialogContext.TEL++;
                        break;
                    case "JPV":
                        _ImportDialogContext.JPV++;
                        break;
                    case "TYPRAV":
                        _ImportDialogContext.TYPRAV++;
                        break;
                    case "KODCHB":
                        _ImportDialogContext.KODCHB++;
                        break;
                    case "TYPSOS":
                        _ImportDialogContext.TYPSOS++;
                        break;
                    case "TYPPPD":
                        _ImportDialogContext.TYPPPD++;
                        break;
                    case "POM":
                        _ImportDialogContext.POM++;
                        break;
                    case "SPOM":
                        _ImportDialogContext.SPOM++;
                        break;
                    case "HBPEJ":
                        _vfkMain.VFKHBPEJTable.addTableData((VFKHBPEJTableItem)table);
                        _ImportDialogContext.HBPEJ++;
                        break;
                    case "OBPEJ":
                        _vfkMain.VFKOBPEJTable.addTableData((VFKOBPEJTableItem)table);
                        _ImportDialogContext.OBPEJ++;
                        break;
                    case "NZ":
                        _ImportDialogContext.NZ++;
                        break;
                    case "NZZP":
                        _ImportDialogContext.NZZP++;
                        break;
                    case "RECI":
                        _ImportDialogContext.RECI++;
                        break;
                    case "DOCI":
                        _ImportDialogContext.DOCI++;
                        break;
                    case "REZBP":
                        _ImportDialogContext.REZBP++;
                        break;
                    case "OBDEBO":
                        _ImportDialogContext.OBDEBO++;
                        break;
                    case "BUDOBJ":
                        _ImportDialogContext.BUDOBJ++;
                        break;
                    case "ADROBJ":
                        _ImportDialogContext.ADROBJ++; 
                        break;
                    case "HPOLYGDATA":
                        _ImportDialogContext.HPOLYGDATA++;
                        break;
                    default:
                        throw new Exception(name);
                }
            }
            catch (Exception)
            {
                addErrorMessage(string.Format("Can not parse {0} data block.", name), aLineNumber, 5);
                return false;
            }
            return true;
        }
        public void addErrorMessage(string aValue, int aNumberOfLine, int aErrorValue)
        {
            _ImportDialogContext.Errors.Add(new ImportInfoDialogContext.ErrorInfo(aValue, aNumberOfLine, aErrorValue));
        }

    }
}
