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
    public partial class Seed : Form
    {
        public Seed()
        {
            InitializeComponent();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int input = (int)numericUpDown1.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Apply
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Cancel
            this.Close();
        }
    }
}
