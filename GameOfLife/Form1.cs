using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        // The universe array
        public static int iWidth = Properties.Settings.Default.UniverseWidth;
        public static int iHeight = Properties.Settings.Default.UniverseHight;
        bool[,] universe = new bool[iWidth, iHeight];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        //Alive Cell count
        public int CellCount = 0;

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();
            //Sets the colors based on the default
            graphicsPanel1.BackColor = Properties.Settings.Default.BackroundColor;
            cellColor = Properties.Settings.Default.CellColor;
            gridColor = Properties.Settings.Default.GridColor;
            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }
        
        private int CountNeighbors(int x, int y)
        {
            if (toolStripMenu_Finite.Checked)
            {
                //Comments are wrong for posistion...
                int count = 0;
                //Bottom Left
                if (x - 1 >= 0 && y - 1 >= 0 && universe[x - 1, y - 1])
                {
                    count++;
                }
                //Bottem Middle
                if (y - 1 >= 0 && universe[x, y - 1])
                {
                    count++;
                }
                //Bottem Right
                if (x + 1 < universe.GetLength(0) && y - 1 >= 0 && universe[x + 1, y - 1])
                {
                    count++;
                }
                //Middle Left
                if (x - 1 >= 0 && universe[x - 1, y])
                {
                    count++;
                }
                //Middle Right
                if (x + 1 < universe.GetLength(0) && universe[x + 1, y])
                {
                    count++;
                }
                //Top Left
                if (x - 1 >= 0 && y + 1 < universe.GetLength(1) && universe[x - 1, y + 1])
                {
                    count++;
                }
                //Top Middle
                if (y + 1 < universe.GetLength(1) && universe[x, y + 1])
                {
                    count++;
                }
                //Top Right
                if (x + 1 < universe.GetLength(0) && y + 1 < universe.GetLength(1) && universe[x + 1, y + 1])
                {
                    count++;
                }

                return count;
            }
            else
            {
                int xpos = universe.GetLength(0);
                int ypos = universe.GetLength(1);
                int leftx = ((x - 1) < 0) ? xpos - 1 : (x - 1);
                int rightx = ((x + 1) >= xpos) ? 0 : (x + 1);
                int topy = ((y - 1) < 0) ? ypos - 1 : (y - 1);
                int bottomy = ((y + 1) >= ypos) ? 0 : (y + 1);

                int count = 0;

                if (rightx < xpos && universe[rightx, y])
                {
                    count++;
                }
                if (rightx < xpos && bottomy < ypos && universe[rightx, bottomy])
                {
                    count++;
                }
                if (bottomy < ypos && universe[x, bottomy])
                {
                    count++;
                }
                if (leftx >= 0 && universe[leftx, y])
                {
                    count++;
                }
                if (leftx >= 0 && topy >= 0 && universe[leftx, topy])
                {
                    count++;
                }
                if (topy >= 0 && universe[x, topy])
                {
                    count++;
                }
                if (leftx >= 0 && bottomy < ypos && universe[leftx, bottomy])
                {
                    count++;
                }
                if (rightx < xpos && topy >= 0 && universe[rightx, topy])
                {
                    count++;
                }
                return count;
            }
        }
        //Count the amount of alive cells
        public int CountCell()
        {
            int count = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x,y])
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        // Calculate the next generation of cells
        private void NextGeneration()
        {        //Game of Life Rules  
            bool[,] scratchPad = new bool[universe.GetLength(0), universe.GetLength(1)];
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y])
                    {

                        if (CountNeighbors(x, y) < 2 || CountNeighbors(x, y) > 3)
                        {
                            scratchPad[x, y] = false;

                        }
                        else
                        {
                            scratchPad[x, y] = true;
                        }
                    }
                    else
                    {
                        if (CountNeighbors(x, y) == 3)
                        {
                            scratchPad[x, y] = true;
                        }
                    }
                }
            }
            //creates a temp universe and copies main universe.
            bool[,] temp = universe;
            //Sets main universe to scratchpad
            universe = scratchPad;
            //and scratch pad to temp universe
            scratchPad = temp;
            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);


            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    //Convert Rectangle to RectangleF for floats
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;
                    Font font = new Font("Arial", 12f);
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    //int neighbors = CountNeighbors(x, y);
                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    if (toolStripMenu_NeighborCount.Checked)
                    {
                        if (CountNeighbors(x, y) >= 1)
                        {
                            e.Graphics.DrawString(CountNeighbors(x, y).ToString(), font, Brushes.Black, cellRect, stringFormat);
                        }
                    }
                    //e.Graphics.DrawString(CountNeighbors(x, y).ToString(), font, Brushes.Black, cellRect, stringFormat) ;
                    
                    // Outline the cell with a pen
                    if (toolStripMenu_Grid.Checked)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }
                    
                }
            }
            //Gets the amount of alive cells and prints them
            CellCount = CountCell();
            toolStripStatusLabel1.Text = "Number of alive cells = " + CellCount.ToString();
            string TORorFIN = string.Empty;
            if (toolStripMenu_Toroidal.Checked)
            {
                TORorFIN = "Toroidal";
            }
            else
            {
                TORorFIN = "Finite";
            }

            if (toolStripMenu_HUD.Checked)
            {
                //Setting up for the HUD
                Font fonty = new Font("Arial", 16f);
                Color temp = Color.FromArgb(190, 255, 0, 0);
                Brush HudBrush = new SolidBrush(temp);
                StringFormat stringFormat1 = new StringFormat();
                stringFormat1.Alignment = StringAlignment.Near;
                stringFormat1.LineAlignment = StringAlignment.Near;
                string HUDstring = "Generations = " + generations + " \nCell Count = " + CellCount + " \nBoundry Type = " + TORorFIN + " \nUniverse Size = " + universe.GetLength(0) + ", " + universe.GetLength(1);
                e.Graphics.DrawString(HUDstring, fonty, HudBrush, 0, 0, stringFormat1);

            }
            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x, (int)y];
                //CountNeighbors(x, y);
                //if (universe[x, y] == false)
                //{
                //    Neighbors[x, y] = 0;
                //}

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Closes the program apon clicking close from menu
            this.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Clears the universe
            generations = 0;
            CellCount = 0;
            timer.Stop();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            graphicsPanel1.Invalidate();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            //Creates a new universe and clears the varibles
            generations = 0;
            CellCount = 0;
            timer.Stop();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            graphicsPanel1.Invalidate();
        }
        public void New()
        {
            //this method is for clearing the universe and starting fresh
            generations = 0;
            CellCount = 0;
            timer.Stop();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            graphicsPanel1.Invalidate();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //resets the color to default
            Properties.Settings.Default.Reset();


            graphicsPanel1.BackColor = Properties.Settings.Default.BackroundColor;
            cellColor = Properties.Settings.Default.CellColor;
            gridColor = Properties.Settings.Default.GridColor;

            //gridColor = Color.Black;
            //cellColor = Color.Gray;
            //graphicsPanel1.BackColor = Color.White;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // calls timer start
            timer.Start();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //calls timer stop
            timer.Stop();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //calls next generation
            NextGeneration();
        }

        private void backcolorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = graphicsPanel1.BackColor;
            //sets backround color based on user input
            if (DialogResult.OK == cd.ShowDialog())
            {
                graphicsPanel1.BackColor = cd.Color;
                //graphicsPanel1.Invalidate();
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options opt = new Options();
            //Creating a temp bool
            bool[,] temp;
            if(DialogResult.OK == opt.ShowDialog())
            {
                //making the temp bool the size that the user inputs
                temp = new bool[(int)opt.numericUpDown2.Value, (int)opt.numericUpDown3.Value];
                //Setting the universe the the user input []
                universe = temp;
                //Setting the timer interval to the user input
                timer.Interval = (int)opt.numericUpDown1.Value;
            }
            graphicsPanel1.Invalidate();
            
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Seed seed = new Seed();
            if(DialogResult.OK == seed.ShowDialog())
            {
                New();

                //string seedinputstring = seed.numericUpDown1.ToString();
                int seedinputint = (int)seed.numericUpDown1.Value;// seedinputstring.GetHashCode();


                toolStripStatusLabel2.Text = "Seed = " + seedinputint;
                int mSeed1 = seedinputint;

                //Creating a random based off the seed.
                Random rand = new Random(mSeed1);
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Iterate through the universe in the x, left to right
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        //Calling for a random number.
                        int randy = rand.Next(mSeed1);
                        if (randy % 2 == 0)
                        {
                            universe[x, y] = false;
                        }
                        else
                        {
                            universe[x, y] = true;
                        }
                        //Increasing the seed by 1 to make the seed the same every time
                        mSeed1++;
                    }
                }
                graphicsPanel1.Invalidate();
            }

            

        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = cellColor;
            //Shows the color dialog box
            if (DialogResult.OK == cd.ShowDialog())
            {
                //sets the cell colors to the selected color
                cellColor = cd.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = gridColor;
            //shows the color dialog box
            if (DialogResult.OK == cd.ShowDialog())
            {
                //Sets the grid color to the selected color
                gridColor = cd.Color;
                graphicsPanel1.Invalidate();
            }
        }
        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            //Random number based from time
            Random rand = new Random();
            int MainSeed = rand.Next();
            //displays the seed in the form
            toolStripStatusLabel2.Text = "Seed = " + MainSeed;
            //creates a temp seed to be adjusted
            int mSeed1 = MainSeed;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    //Randoms next based on the seed
                    int randy = rand.Next(mSeed1);
                    if (randy % 2 == 0)
                    {
                        universe[x, y] = false;
                    }
                    else
                    {
                        universe[x, y] = true;
                    }
                    //Increases to make the seed repeatable
                    mSeed1++;
                }
            }
            graphicsPanel1.Invalidate();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //sets the default color to the color that was last used
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.BackroundColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.CellColor = cellColor;
            //saves the color
            Properties.Settings.Default.Save();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //sets the color to the last saved color in default
            Properties.Settings.Default.Reload();

            graphicsPanel1.BackColor = Properties.Settings.Default.BackroundColor;
            cellColor = Properties.Settings.Default.CellColor;
            gridColor = Properties.Settings.Default.GridColor;
        }

        private void toolStripMenu_NeighborCount_Click(object sender, EventArgs e)
        {
            //To enable or disable neighbor count displaying
            if (toolStripMenu_NeighborCount.Checked)
            {
                toolStripMenu_NeighborCount.Checked = false;
            }
            else
            {
                toolStripMenu_NeighborCount.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        private void toolStripMenu_Grid_Click(object sender, EventArgs e)
        {
            //to enable or disable the grid from showing
            if (toolStripMenu_Grid.Checked)
            {
                toolStripMenu_Grid.Checked = false;
            }
            else
            {
                toolStripMenu_Grid.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!This is my comment.");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                { 
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x,y])
                        {
                            currentRow += 'O';
                        }
                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                        else
                        {
                            currentRow += '.';
                        }
                    }
                    writer.WriteLine(currentRow);
                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if (row[0] == '!')
                    {
                        continue;
                    }

                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.

                    else
                    {
                        maxHeight++;
                        if (row.Length > maxWidth)
                        {
                            maxWidth = row.Length;
                        }
                    }
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.

                universe = new bool[maxWidth, maxHeight];

                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                int temp = 0;
                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row[0] == '!')
                    {
                        continue;
                    }
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    else
                    {
                            // If row[xPos] is a 'O' (capital O) then
                            // set the corresponding cell in the universe to alive.
                            for (int x = 0; x < row.Length; x++)
                            {
                                if (row[x] == 'O')
                                {
                                    universe[x, temp] = true;
                                }
                                else
                                {
                                    universe[x, temp] = false;
                                }
                            }
                            temp++;
                            // If row[xPos] is a '.' (period) then
                            // set the corresponding cell in the universe to dead.
                        

                    }
                }

                // Close the file.
                reader.Close();
                graphicsPanel1.Invalidate();
            }
        }

        private void toolStripMenu_Toroidal_Click(object sender, EventArgs e)
        {
            if (toolStripMenu_Toroidal.Checked)
            {
                toolStripMenu_Toroidal.Checked = false;
                toolStripMenu_Finite.Checked = true;
            }
            else
            {
                toolStripMenu_Toroidal.Checked = true;
                toolStripMenu_Finite.Checked = false;
            }
            graphicsPanel1.Invalidate();
        }

        private void toolStripMenu_Finite_Click(object sender, EventArgs e)
        {
            if (toolStripMenu_Finite.Checked)
            {
                toolStripMenu_Toroidal.Checked = true;
                toolStripMenu_Finite.Checked = false;
            }
            else
            {
                toolStripMenu_Toroidal.Checked = false;
                toolStripMenu_Finite.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        private void toolStripMenu_HUD_Click(object sender, EventArgs e)
        {
            if (toolStripMenu_HUD.Checked)
            {
                toolStripMenu_HUD.Checked = false;
            }
            else
            {
                toolStripMenu_HUD.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if (row[0] == '!')
                    {
                        continue;
                    }

                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.

                    else
                    {
                        maxHeight++;
                        if (row.Length > maxWidth)
                        {
                            maxWidth = row.Length;
                        }
                    }
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.

                if (maxWidth > universe.GetLength(0) || maxHeight > universe.GetLength(1))
                {
                    universe = new bool[maxWidth, maxHeight];
                }

                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                int temp = 0;
                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row[0] == '!')
                    {
                        continue;
                    }
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    else
                    {
                        // If row[xPos] is a 'O' (capital O) then
                        // set the corresponding cell in the universe to alive.
                        for (int x = 0; x < row.Length; x++)
                        {
                            if (row[x] == 'O')
                            {
                                universe[x, temp] = true;
                            }
                            else
                            {
                                universe[x, temp] = false;
                            }
                        }
                        temp++;
                        // If row[xPos] is a '.' (period) then
                        // set the corresponding cell in the universe to dead.


                    }
                }

                // Close the file.
                reader.Close();
                graphicsPanel1.Invalidate();
            }
        }
    }
}
