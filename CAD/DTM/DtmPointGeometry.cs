namespace CAD.DTM
{
    class DtmPointGeometry
        : IDtmGeometry
    {
        public string Id { get; set; }
        public string SrsName { get; set; }
        public int SrsDimension { get; set; }
        public DtmPoint Point { get; set; }
    }
}
