using WSGP.OutputObject;

namespace WSGP.InputObject
{
    interface IRezervacePrvku
    {
        string Xml { get; }
        RezervPrvkuAbstract CreateResponseObject();
    }
}
