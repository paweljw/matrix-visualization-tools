using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace Matrix_Visualization_Tool
{
    public partial class Form1 : Form
    {

        public delegate void runNextFrameDelegate();

        public void runNextFrame()
        {
            nextFrame();
            _paint();
        }

        int drawingOffset()
        {
            uint squareSize = X / mxSize;
            uint remainder = X - (squareSize * mxSize);

            return (int)(remainder / 2);
        }

        StreamReader sr;
        Stream s;

        bool fileOpen;

        uint mxSize = 0;

        uint X, Y;

        Bitmap bmp;
        Graphics g;

        uint currentFrame = 0;

        double[][] matrix;

        Thread playThread;

        public Form1()
        {
            InitializeComponent();

            g = panel1.CreateGraphics();

            X = Convert.ToUInt32(panel1.Width);
            Y = Convert.ToUInt32(panel1.Height);

            bmp = new Bitmap((int)X, (int)Y);

            panel1.Paint += panel1_Paint;

            this.AllowDrop = true;
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;

        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            openFile(files[0]);
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void panel1_Paint(object sender, PaintEventArgs e)
        {
            clearPlot();
            plotGrid();
            plotAllMatrix();
            _paint();
        }

        private void openFile(string file)
        {
            try
            {
                sr = new StreamReader(file);
                mxSize = Convert.ToUInt32(sr.ReadLine());
                fileOpen = true;

                currentFrame = 0;

                // Generate matrix
                matrix = new double[mxSize][];

                for (uint i = 0; i < mxSize; i++)
                {
                    matrix[i] = new double[mxSize];
                    for (int j = 0; j < mxSize; j++)
                    {
                        matrix[i][j] = 0;
                    }
                }

                clearPlot();
                plotGrid();
                _paint();
            }
            catch (Exception eyargh)
            {
                Console.WriteLine(eyargh.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult res = ofd.ShowDialog();

            if (res == DialogResult.OK)
            {
                String file = ofd.FileName;
                openFile(file);       
            }
        }

        private void clearPlot()
        {   
            try
            {
                LockBitmap lb = new LockBitmap(bmp);
                lb.LockBits();

                for (int i = 0; i < X; i++)
                    for (int j = 0; j < X; j++)
                        lb.SetPixel(i, j, Color.Black);

                lb.UnlockBits();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void plotGrid()
        {
            // If there is no matrix to be plotted, exit
            if (mxSize == 0) return;

            try
            {
                LockBitmap lb = new LockBitmap(bmp);
                lb.LockBits();

                uint squareSize = X / mxSize;

                // vertical and horizontal lines
                for (uint i = 0; i <= mxSize; i++)
                {
                    uint loc_x = i * squareSize;
                    for (uint loc_y = 0; loc_y < squareSize * mxSize; loc_y++)
                    {
                        lb.SetPixel(drawingOffset() + (int)loc_x, drawingOffset() + (int)loc_y, Color.FromArgb(0x22, 0x22, 0x22));
                        lb.SetPixel(drawingOffset() + (int)loc_y, drawingOffset() + (int)loc_x, Color.FromArgb(0x22, 0x22, 0x22));
                    }
                }

                lb.UnlockBits();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void plotAt(int x, int y, double v)
        {
            uint squareSize = X / mxSize; // correction for grid

            LockBitmap lb = new LockBitmap(bmp);

            try
            {
                lb.LockBits();

                    

                for (int _x = (x==0) ? 0 : 1; _x < squareSize; _x++)
                {
                    for (int _y = (y==0) ? 0 : 1; _y < squareSize; _y++)
                    {
                        lb.SetPixel(drawingOffset() + (int)(x * squareSize + _x), drawingOffset() + (int)(y * squareSize + _y), (Math.Abs(v) > 0.00001) ? Color.Green : Color.Black);
                    }
                }

                lb.UnlockBits();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void plotAt_noBlack(int x, int y, double v)
        {
            uint squareSize = X / mxSize; // correction for grid

            LockBitmap lb = new LockBitmap(bmp);

            if (Math.Abs(v) > 0.00001)
            {
                try
                {
                    lb.LockBits();

                    for (int _x = (x == 0) ? 0 : 1; _x < squareSize; _x++)
                    {
                        for (int _y = (y == 0) ? 0 : 1; _y < squareSize; _y++)
                        {
                            lb.SetPixel(drawingOffset() + (int)(x * squareSize + _x), drawingOffset() + (int)(y * squareSize + _y), Color.Green);
                        }
                    }

                    lb.UnlockBits();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void plotAllMatrix()
        {
            for (int i = 0; i < mxSize; i++)
            {
                for (int j = 0; j < mxSize; j++)
                {
                    plotAt_noBlack(i, j, matrix[i][j]);
                }
            }
        }

        private void _paint()
        {
            g.DrawImage(bmp, new Point(0, 0));
        }

        // next frame button

        private void nextFrame()
        {
            currentFrame += 1;

            // Seek stream to beginning
            sr.BaseStream.Position = 0;
            sr.DiscardBufferedData();

            // Continue until we find the frame we want
            string line;
            bool beganReading = false;

            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains("---FRAME::"))
                {
                    if (beganReading == true) break;
                    else
                        if (currentFrame == Convert.ToUInt32(line.Replace("---", "").Replace("FRAME::", "")))
                            beganReading = true;
                }
                else
                {
                    if (beganReading)
                    {
                        string[] coords;
                        string val;

                        string[] splt = line.Split(new char[] { '|' });

                        coords = splt[0].Split(new char[] { ',' });
                        val = splt[1].Replace(".", ",");

                        double dval = Convert.ToDouble(val);

                        uint x = Convert.ToUInt32(coords[0]), y = Convert.ToUInt32(coords[1]);

                        matrix[x][y] = dval;
                        if(currentFrame == 1)
                            plotAt_noBlack((int)x, (int)y, dval);
                        else
                            plotAt((int)x, (int)y, dval);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            runNextFrame();
        }

        private void threadFunction()
        {
            while (true)
            {
                try
                {
                    this.Invoke(new runNextFrameDelegate(runNextFrame));
                    System.Threading.Thread.Sleep(1000 / 50);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            playThread = new Thread(new ThreadStart(threadFunction));
            playThread.IsBackground = true;
            playThread.Start();
        }
    }
}
