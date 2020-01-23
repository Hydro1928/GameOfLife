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

        private void button3_Click(object sender, EventArgs e)
        {
            //Randomize Button
            Random rng = new Random();
            int box = rng.Next(10000000);
            numericUpDown1.Value = box;
        }
        
    }
}
