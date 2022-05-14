using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManagement_OS6
{
    internal class MemoryDispatcher
    {
        private int _size;
        private int _segmentSize;
        private List<int> _cells;
        private List<Process> _processesList;
        private Dictionary<int, (int, int)> _processesDictionary;
        private List<int> _bitMatrix;
        private string _errorMessage;

        public MemoryDispatcher(int memorySize, int segmentSize)
        {
            this._size = memorySize;
            this._segmentSize = segmentSize;

            _bitMatrix = new List<int>();
            for(int i=0; i< memorySize / segmentSize; i++)
            {
                _bitMatrix.Add(0);
            }

            _processesList = new List<Process>();
            _processesDictionary = new Dictionary<int, (int,int)>();

            _cells = new List<int>();
            for(int i=0; i < memorySize; i++)
            {
                _cells.Add(i + 1);
            }
        }

        public List<Process> Processes
        {
            get
            {
                return _processesList; 
            }
        }

        public string Error
        {
            get
            {
                return _errorMessage;
            }
        }

        public List<int> BitMatrix
        {
            get
            {
                return _bitMatrix;
            }
        }

        public void AddProcess(Process process)
        {
            int processSizeMB = Service.ConvertBytesIntoMB(process.Size);
            int segmentCount = processSizeMB % _segmentSize != 0 ? processSizeMB / _segmentSize + 1 : processSizeMB / _segmentSize;
            if (_processesDictionary.Count == 0)
            {
                _processesList.Add(process);
                _processesDictionary.Add(process.Id, (1, process.Size));
                for(int i=0; i < segmentCount; i++)
                {
                    _bitMatrix[i] = 1;
                }
            }
            else
            {
                int startSegment = FindSegment(segmentCount);
                if (startSegment != -1)
                {
                    int baseRegister = Service.ConvertMBIntoBytes(startSegment)*_segmentSize +1;
                    _processesList.Add(process);
                    _processesDictionary.Add(process.Id, (baseRegister, baseRegister + process.Size - 1));
                    for(int i=startSegment; i < startSegment + segmentCount; i++)
                    {
                        _bitMatrix[i] = 1;
                    }
                }
            }
        }

        public void RemoveProcess(string processName)
        {
            Process removeProcess = GetProcessByName(processName);
            if(removeProcess != null)
            {
                int processSizeMB = Service.ConvertBytesIntoMB(removeProcess.Size);
                int segmentCount = processSizeMB % _segmentSize != 0 ? processSizeMB / _segmentSize + 1 : processSizeMB / _segmentSize;

                int startSegment = Service.ConvertBytesIntoMB(_processesDictionary[removeProcess.Id].Item1) / _segmentSize;

                _processesList.Remove(removeProcess);
                _processesDictionary.Remove(removeProcess.Id);

                for(int i=startSegment; i < startSegment + segmentCount; i++)
                {
                    _bitMatrix[i] = 0;
                }
            }
        }

        public int MakeAddressation(string processName, int cellAdress)
        {
            Process process = GetProcessByName(processName);
            if (process != null)
            {
                int offsetAdress = cellAdress + _processesDictionary[process.Id].Item1-1;
                if (offsetAdress <= _processesDictionary[process.Id].Item2)
                {
                    process.MakeAddressation();
                    return offsetAdress;
                }
                else
                {
                    _errorMessage = "Restriction register was exceeded.";
                }
            }
            else
            {
                _errorMessage = "Process wasn't found.";
            }
            return 0;
        }

        public int GetProcessBaseRegister(int id)
        {
            return _processesDictionary[id].Item1;
        }

        public int GetProcessRestrictiveRegister(int id)
        {
            return _processesDictionary[id].Item2;
        }

        private int FindSegment(int requiredSize)
        {
            int count = 0;
            int segment = -1;
            for(int i=0; i < _bitMatrix.Count; i++)
            {
                if (_bitMatrix[i] == 0)
                {
                    if (count == 0) segment = i;
                    count++;
                }
                else
                {
                    count = 0;
                    segment = -1;
                }
                if (count == requiredSize) break;
            }
            return segment;
        }

        private Process GetProcessByName(string name)
        {
            return _processesList.Find((process) => process.Name.Equals(name));
        }
    }
}
