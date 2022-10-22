using System;

namespace WSGP.InputObject
{
   [Flags]
   public enum VfExportType
   {
      Nemo = 0x1,
      Bdpa = 0x2,
      Vlst = 0x4,
      Jpvz = 0x8,
      Pkmp = 0x10,
      Bpej = 0x20,
      Gmpl = 0x40,
      Reze = 0x80,
      Debo = 0x100
   }
}
