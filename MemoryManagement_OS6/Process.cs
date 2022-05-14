using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManagement_OS6
{
    internal class Process
    {
        private static int _count = 1;
        private int _id;
        private int _size;
        private Color _color;

        public Process(int size, Color color)
        {
            this._id = _count;
            this._color = color;
            this._size = size*1024;
            _count++;
        }

        public string Name
        {
            get
            {
                return String.Format("Process {0}", _id);
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public int Size
        {
            get
            {
                return _size;
            }
        }

        public Color Color
        {
            get
            {
                return _color;
            }
        }

        public void MakeAddressation()
        {
            //do some work
        }
    }
}
