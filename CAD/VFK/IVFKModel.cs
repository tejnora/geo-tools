using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VFK
{
    interface IVFKModel
    {
        void OnImportVFK(string aLocation);
        void OnRemoveVFKData();
        bool isImportedVFKFile();
        void OnVFKEdidOfParcel();
    }
}
