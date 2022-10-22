namespace WSGP.OutputObject
{
    public abstract class DataObjectBaseAbstract
    {
        public Vysledek Vysledek { get; set; }
        public bool IsValid()
        {
            return Vysledek.Kod == "0";
        }
    }
}
