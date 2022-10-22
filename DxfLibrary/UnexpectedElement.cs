using System;

namespace DxfLibrary
{
    internal class UnexpectedElement : Exception
    {
        #region Constructor
        internal UnexpectedElement()
            : base("Unexpected bug.\n Conntact manufacture.")
        {
        }
        #endregion
    }
}
