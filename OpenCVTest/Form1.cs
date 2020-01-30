using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace OpenCVTest
{
    public partial class Form1 : Form
    {
        private Image<Bgr, byte> inputImage = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (FileDialog fileDialog = new OpenFileDialog())
                {
                    fileDialog.Filter = "Images (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
                    fileDialog.Title = "Выберите изображения";
                    
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        inputImage = new Image<Bgr, byte>(fileDialog.FileName);
                        pictureBox1.Image = inputImage.Bitmap;
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Image<Gray, byte> outputImage = inputImage.Convert<Gray, byte>()
                                                          .ThresholdBinary(new Gray(100), new Gray(255));
                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

                Mat hierarchy = new Mat();

                CvInvoke.FindContours(outputImage, contours, hierarchy, Emgu.CV.CvEnum.RetrType.Tree,
                                      Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                

                CvInvoke.DrawContours(inputImage, contours, -1, new MCvScalar(255, 0, 0));

                pictureBox1.Image = inputImage.Bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)  
        {
            Image<Gray, byte> blurImage = new Image<Gray, byte>(inputImage.Width, inputImage.Height, new Gray(0));
            //размытие входного изображения по Гауссу - на выходе blurImage. 
                 CvInvoke.GaussianBlur(inputImage, blurImage, new Size(9, 9), 1.3);
            
            //Canny для размытого изображения
                 blurImage = blurImage.Canny(70, 163); 
            

            //морфологическое закрытие контуров
            Mat kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(8, 8), new Point(-1, -1));
            blurImage = blurImage.MorphologyEx(Emgu.CV.CvEnum.MorphOp.Close, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(1.0));
            
            //поиск контуров
            try
            {
                 Image<Gray, byte> outputImage = blurImage.Convert<Gray, byte>()
                                                           .ThresholdBinary(new Gray(100), new Gray(255));
                 
                 VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                 VectorOfVectorOfPoint contours2 = new VectorOfVectorOfPoint();

                 Mat hierarchy = new Mat();

                 CvInvoke.FindContours(blurImage, contours, hierarchy, Emgu.CV.CvEnum.RetrType.Tree,
                                       Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);


                 //TODO убрать контуры с малой площадью (предварительно нужно замкнуть контуры, т.к. иначе они имеют большую площадь)
                 for (int i = 0; i < contours.Size; ++i)
                 {
                     if (CvInvoke.ContourArea(contours[i]) > 908)
                     {
                         contours2.Push(contours[i]);
                         //TODO fitEllipse  
                     }
                 }                      

                 CvInvoke.DrawContours(inputImage, contours2, -1, new MCvScalar(255, 0, 0));
                                      
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            pictureBox1.Image = inputImage.Bitmap;           

        }
    }
}
