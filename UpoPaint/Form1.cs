using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpoPaint
{
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private List<Point> points = new List<Point>();
        private List<Image> history;
        private int selectedShape = 0;
        private int undoCount = 0;
        public Form1()
        {
            InitializeComponent();
            nowyToolStripMenuItem_Click(null, null);
            buttonPenColor.BackColor = Color.Black;
        }

        private void addHistory()
        {
            Bitmap b = new Bitmap(pictureBoxPaint.Image);
            if(history.Count > 19)
            {
                history.Remove(history.First());
            }
            history.Add(b);
        }
        private void nowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBoxPaint.Image = new Bitmap(pictureBoxPaint.Width, pictureBoxPaint.Height);
            graphics = Graphics.FromImage(pictureBoxPaint.Image);
            graphics.Clear(Color.White);
            history = new List<Image>();
            undoCount = 0;
            addHistory();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button != MouseButtons.Left) return;

            points = new List<Point>();
            points.Add(e.Location);

            while (undoCount > 0)
            {
                history.Remove(history.Last());
                undoCount--;
            }
        }

        private void pictureBoxPaint_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            points.Add(e.Location);

            pictureBoxPaint.Image = new Bitmap(history.Last());
            graphics = Graphics.FromImage(pictureBoxPaint.Image);
            Pen p = new Pen(buttonPenColor.BackColor, (int)numericUpDown1.Value);
            Brush b = new SolidBrush(buttonFillColor.BackColor);
            draw(p, b);
            pictureBoxPaint.Refresh();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            points.Add(e.Location);

            Pen p = new Pen(buttonPenColor.BackColor, (int)numericUpDown1.Value);
            Brush b = new SolidBrush(buttonFillColor.BackColor);
            draw(p, b);
            addHistory();
            pictureBoxPaint.Refresh();
        }

        private void draw(Pen p, Brush b)
        {
            int x = Math.Min(points.First().X, points.Last().X);
            int y = Math.Min(points.First().Y, points.Last().Y);
            int width = Math.Abs(points.First().X - points.Last().X);
            int height = Math.Abs(points.First().Y - points.Last().Y);
            bool fillChecked = checkBoxFill.Checked;

            switch (selectedShape)
            {
                case 0:
                    graphics.DrawLine(p, points.First(), points.Last());
                    break;

                case 1:
                    if (fillChecked) graphics.FillEllipse(b, x, y, width, height);
                    graphics.DrawEllipse(p, x, y, width, height);
                    break;

                case 2:
                    if (fillChecked) graphics.FillRectangle(b, x, y, width, height);
                    graphics.DrawRectangle(p, x, y, width, height);
                    break;

                case 3:
                    graphics.DrawCurve(p, points.ToArray());
                    break;
            }
        }

        private void radioButtonLine_Click(object sender, EventArgs e)
        {
            selectedShape = 0;
        }

        private void radioButtonCircle_Click(object sender, EventArgs e)
        {
            selectedShape = 1;
        }

        private void radioButtonRectangle_Click(object sender, EventArgs e)
        {
            selectedShape = 2;
        }

        private void radioButtonCurve_Click(object sender, EventArgs e)
        {
            selectedShape = 3;
        }

        private void buttonFillColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = buttonFillColor.BackColor;
            if (cd.ShowDialog() != DialogResult.OK) return;
            
            buttonFillColor.BackColor = cd.Color;
            
        }

        private void buttonPenColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = buttonPenColor.BackColor;
            
            if (cd.ShowDialog() != DialogResult.OK) return;
            
            buttonPenColor.BackColor = cd.Color;
            
        }

        private void zapiszToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPEG|*.jpg|Bitmapa|*.bmp";

            if (sfd.ShowDialog() != DialogResult.OK) return;

            string extension = Path.GetExtension(sfd.FileName);

            switch (extension)
            {
                case ".jpg":
                    pictureBoxPaint.Image.Save(sfd.FileName, ImageFormat.Jpeg);  
                    break;

                case ".bmp":
                    pictureBoxPaint.Image.Save(sfd.FileName, ImageFormat.Bmp);
                    break;
            }
        }

        private void otworzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JPEG|*.jpg|Bitmapa|*.bmp";

            if (ofd.ShowDialog() != DialogResult.OK) return;

            pictureBoxPaint.Image = new Bitmap(ofd.FileName);
            graphics = Graphics.FromImage(pictureBoxPaint.Image);
            history = new List<Image>();
            addHistory();
            undoCount = 0;
        }

        private void cofnijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoCount >= history.Count - 1) return;
           
            undoCount++;
            pictureBoxPaint.Image = new Bitmap(history[history.Count - 1 - undoCount]);
            graphics = Graphics.FromImage(pictureBoxPaint.Image);
            
        }

        private void ponowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoCount <= 0) return;
            
            undoCount--;
            pictureBoxPaint.Image = new Bitmap(history[history.Count - 1 - undoCount]);
            graphics = Graphics.FromImage(pictureBoxPaint.Image);
            
        }
    }
}