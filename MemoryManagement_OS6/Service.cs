using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManagement_OS6
{
    internal class Service
    {
        public static int ConvertBytesIntoMB(int value)
        {
            return value / 1024;
        }

        public static int ConvertMBIntoBytes(int value)
        {
            return value * 1024;
        }
    }
}
