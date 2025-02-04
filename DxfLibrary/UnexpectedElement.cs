using System;

namespace DxfLibrary
{
    internal class UnexpectedElement : Exception
    {
                internal UnexpectedElement()
            : base("Unexpected bug.\n Conntact manufacture.")
        {
        }
            }
}
