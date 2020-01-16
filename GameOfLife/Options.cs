using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        public int timerInterval
        {
            get
            {
                return (int)numericUpDown1.Value;
            }

            set
            {
                numericUpDown1.Value = value;
            }
        }

        public int WidthUniverse
        {
            get
            {
                return (int)numericUpDown2.Value;
            }

            set
            {
                numericUpDown2.Value = value;
            }
        }

        public int HightUniverse
        {
            get
            {
                return (int)numericUpDown3.Value;
            }

            set
            {
                numericUpDown3.Value = value;
            }
        }
    }
}
