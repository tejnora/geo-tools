namespace GeoCalculations.Points
{
    public interface IPointExporter
    {
        void AddEmptyLine();
        void AddValue(ImportExportElementType type, object value);
        void FlushValues();
        void AddSettingValue(string name, string value);
    }
}
