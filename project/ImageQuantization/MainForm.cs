using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix,res;

        private void btnOpen_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
               // ImageOperations.distinct_color(ImageMatrix);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            /*
            int Number = int.Parse(textBox1.Text);
            res = ImageOperations.Quazation_of_image(Number,ImageMatrix);
            ImageOperations.DisplayImage(res, pictureBox2);
             */
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int Number = int.Parse(textBox1.Text);
            res = ImageOperations.Quazation_of_image(Number, ImageMatrix, textBox2, textBox3);
            ImageOperations.DisplayImage(res, pictureBox2);
            pictureBox2.Image.Save("D:\\itestcase.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }
    }
}