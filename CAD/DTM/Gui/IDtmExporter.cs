using System;

namespace CAD.DTM.Gui
{
    public interface IDtmExporter
    {
        void AddElement(string ns, string name, string text);
        void AddElement(string ns, string name, int value);
        void AddElement(string ns, string name, DateTime value);
        void AddElement(string ns, string name, double value);
        void AddElement(string ns, string name, bool value);
        void BeginElement(string ns, string name, bool addNsToAttribute = false);
        void EndElement();
        void AddAttribute(string name, string value);
        void AddAttribute(string name, int value);
        void AddStringData(string value);
    }
}
