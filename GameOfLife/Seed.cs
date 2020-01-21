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
            //Apply Button
            //Apply
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Cancel Button
            //Cancel
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Randomize Button
            Random rng = new Random();
            int box = rng.Next(10000);
        }
    }
}
