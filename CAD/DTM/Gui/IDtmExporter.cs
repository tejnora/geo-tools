using System;

namespace CAD.DTM.Gui
{
    public interface IDtmExporter
    {
        void AddElement(string ns, string name, string text);
        void AddElement(string ns, string name, int value);
        void AddElement(string ns, string name, DateTime value);
        void BeginElement(string ns, string name);
        void EndElement();
        void AddAttribute(string name, string value);
        void AddPCData(string value);

        void AddSpolecneAtributyVsechObjektu();
    }
}
