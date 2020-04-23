using System;
using System.Drawing;
using System.Windows.Forms;

namespace GScalGOL
{ //adding as i go
    public partial class Form1 : Form
    {
        // The universe array
        private bool[,] universe = new bool[15, 15];
        bool[,] Pad = new bool[15, 15];
        bool[,] temp = new bool[15, 15];

        // Drawing colors
        private Color gridColor = Color.Black;
       
        private Color cellColor = Color.Gray;
        private Color deadcellC = Color.White;
        // The Timer class
        public Timer timer = new Timer();
    
        // Generation count
        private int generations = 0;
        private int aliveCells = 0;



        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running


         

        }

        //coount neighbors
        private int CountNeighbors(int x, int y)
        {

            int sum = 0;

            for (int i = -1; i < 2; i++)
            {
              
                for (int j = -1; j < 2; j++)
                {
                  

                    int col = (y + i + universe.GetLength(1)) % universe.GetLength(1);

                    int row = (x + j + universe.GetLength(0)) % universe.GetLength(0);

                    if (universe[row, col] == true)
                    {
                        sum++;
                    }
                }

            }
            if (universe[x, y])
            {
                sum--;
            }
            return sum;
        }
            // Calculate the next generation of cells
            private void NextGeneration()
        {
            temp = universe;

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    var cellState = universe[x, y];


                    int nCount = CountNeighbors(x, y);
                    // Draw count within current position

                    if (cellState == false && nCount == 3)
                    {
                        Pad[x, y] = true;
                    }
                    else if (cellState == true && (nCount < 2 | nCount > 3))
                    {
                        Pad[x, y] = false;
                    }
                    else
                    {
                        Pad[x, y] = cellState;
                    }


                }


            }
                universe = Pad;
                Pad = temp;
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
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            Brush deadCellBrush = new SolidBrush(deadcellC);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;
                    Font font = new Font("Arial", 20f);

                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    Rectangle rect = new Rectangle(0, 0, 100, 100);
                   // int CountNeighbors = 8;
                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    else if (universe[x, y] == false)
                    {
                        e.Graphics.FillRectangle(deadCellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                  //  e.Graphics.DrawString(CountNeighbors(x,y).ToString(), font, Brushes.Black, rect, stringFormat);
                    stringFormat.Dispose();
                    font.Dispose();
                    TextRenderer.DrawText(e.Graphics, CountNeighbors(x, y).ToString(), this.Font, cellRect, Color.Black, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                 //   e.Graphics.DrawString(CountNeighbors(x, y).ToString(), graphicsPanel1.Font, Brushes.Black, cellRect.Location);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
            deadCellBrush.Dispose();
          
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PlayStartButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void PauseStopButtom_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void NextStepButton_Click(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }

        private void ResetGOL_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }

            // set Generations To 0 and Living Cells
            generations = 0;
            
            graphicsPanel1.Invalidate();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

            timer.Enabled = false;

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }

            // set Generations To 0 and Living Cells
            generations = 0;

            graphicsPanel1.Invalidate();
        }

        private void RandomButton_Click(object sender, EventArgs e)
        {
            int currentSeed;
            Random seedRand = new Random();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {

                    currentSeed = seedRand.Next(0, 2);

                    if (currentSeed == 0)
                    {
                        universe[x, y] = false;
                    }
                    else
                    {
                        universe[x, y] = true;
                    }
                }
            }
            countAliveCells();
            graphicsPanel1.Invalidate();
        }

        void countAliveCells() {
            aliveCells = 0;

            for (int a = 0; a < universe.GetLength(1); a++)
            {
                for (int b = 0; b < universe.GetLength(0); b++)
                {

                    if (universe[b,a] == true)
                    {
                        aliveCells++;
                    }

                }

            }
            //return aliveCells;
        
        }



    }
    }
    
