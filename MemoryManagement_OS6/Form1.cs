using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryManagement_OS6
{
    public partial class Form1 : Form
    {
        private int _segmentSize;
        private MemoryDispatcher _memoryDispatcher;
        public Form1()
        {
            InitializeComponent();

            comboBox3.SelectedIndex = 0;
        }

        private void InitializeDataGrid()
        {
            dataGridView1.Rows.Clear();

            int start_adress = 0;
            List<int> bitMatrix = _memoryDispatcher.BitMatrix;
            for (int i = 0; i < bitMatrix.Count; i++)
            {
                dataGridView1.Rows.Add(bitMatrix[i],i + 1, String.Format("{0}...{1}", start_adress + 1, start_adress + 1024 * _segmentSize));
                start_adress += 1024 * _segmentSize;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int memorySize;
            if(int.TryParse(memorySizeTextBox.Text, out memorySize))
            {
                if (memorySize > 0)
                {
                   
                    _segmentSize = int.Parse(comboBox3.SelectedItem.ToString());

                    _memoryDispatcher = new MemoryDispatcher(memorySize, _segmentSize);

                    InitializeDataGrid();
                }
            }
        }

        private void Rerender()
        {
            //clear
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.DefaultCellStyle.BackColor = Color.White;
            }
            listView1.Items.Clear();
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            foreach (Process process in _memoryDispatcher.Processes)
            {
                int baseRegister = _memoryDispatcher.GetProcessBaseRegister(process.Id);
                int restrictionRegister = _memoryDispatcher.GetProcessRestrictiveRegister(process.Id);

                //label
                adressingLabel.Text = String.Empty;

                //dataGridView
                int restrictionRegisterMB = Service.ConvertBytesIntoMB(restrictionRegister);
                int end = restrictionRegisterMB % _segmentSize != 0 ? restrictionRegisterMB / _segmentSize + 1 : restrictionRegisterMB / _segmentSize;
                for (int i = Service.ConvertBytesIntoMB(baseRegister)/_segmentSize; i < end; i++)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = process.Color;
                }

                //listView
                string[] arr =
                 {
                        process.Name,
                         baseRegister.ToString(),
                         restrictionRegister.ToString()
                    };

                ListViewItem processItem = new ListViewItem(arr);
                processItem.BackColor = process.Color;

                listView1.Items.Add(processItem);

                //comboBoxes
                comboBox1.Items.Add(process.Name);
                comboBox2.Items.Add(process.Name);
            }
        }

        private Color GenerateColor()
        {
            Random random = new Random();
            Color color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
            return color;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int processSize;
            if(int.TryParse(pushProcessTextBox.Text, out processSize))
            {
                if(processSize >0 && _memoryDispatcher!=null)
                {
                    Color processColor = GenerateColor();
                    Process process = new Process(processSize, processColor);
                    _memoryDispatcher.AddProcess(process);
                    InitializeDataGrid();
                    Rerender();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string selectedProcessName = (string)comboBox1.SelectedItem;
            if (selectedProcessName != null)
            {
                _memoryDispatcher.RemoveProcess(selectedProcessName);
                InitializeDataGrid();
                Rerender();
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string selectedProcessName = (string)comboBox2.SelectedItem;
            if (selectedProcessName != null)
            {
                int cellAddress;
                if (int.TryParse(addressTextBox.Text, out cellAddress) && cellAddress > 0)
                {
                    int result = _memoryDispatcher.MakeAddressation(selectedProcessName, cellAddress);
                    if (result != 0)
                    {
                        adressingLabel.Text = String.Format("{0} has addressed to cell {1}.", selectedProcessName, result);
                    }
                    else
                    {
                        adressingLabel.Text = _memoryDispatcher.Error;
                    }
                }
            }
        }
    }
}
