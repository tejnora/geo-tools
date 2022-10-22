namespace GeoCalculations.Points
{
    public enum ImportExportElementType
    {
        NUM,
        X,
        Y,
        Z,
        MEAS,
        HZ,
        HZ1,
        HZ2,
        VERT,
        VERT1,
        VERT2,
        DIST,
        DIST1,
        DIST2,
        DISABLED,
        IH,
        DH,
        DH1,
        DH2,
        SIGNAL,
        SIGNAL1,
        SIGNAL2
    }

    public interface IPointImport
    {
        void SetElement(ImportExportElementType type, object value);
    }
}
