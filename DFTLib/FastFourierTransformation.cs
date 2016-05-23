using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DFTLib
{
    public class FastFourierTransformation
    {
        private float[,] FFTLog;                 // Log of Fourier Magnitude
        private float[,] FFTPhaseLog;            // Log of Fourier Phase
        private int xPoints, yPoints;                      //Number of Points in Width & height
        private int Width, Height;

        public Bitmap Image;               // Input Object Image
        public Bitmap FourierPlot;       // Generated Fouruer Magnitude Plot
        public Bitmap PhasePlot;         // Generated Fourier Phase Plot

        public int[,] GreyImage;         //GreyScale Image Array Generated from input Image
        public float[,] FourierMagnitude;
        public float[,] FourierPhase;
        public int[,] FFTNormalized;     // Normalized FFT Magnitude : Scale 0-1
        public int[,] FFTPhaseNormalized;// Normalized FFT Phase : Scale 0-1
        public Complex[,] FFTShifted;    // Shifted FFT 
        public Complex[,] Output;        // FFT Normal
        public Complex[,] Fourier;       // 
        public Complex[,] FFTNormal;     // FFT Normal

        /// <summary>
        /// Parameterized Constructor for FFT Reads Input Bitmap to a Greyscale Array
        /// </summary>
        /// <param name="Input">Input Image</param>
        public FastFourierTransformation(Bitmap Input)
        {
            Image = Input;
            Width = xPoints = Input.Width;
            Height = yPoints = Input.Height;
            ReadImage();
        }

        /// <summary>
        /// Constructor for Inverse FFT
        /// </summary>
        /// <param name="Input"></param>
        public FastFourierTransformation(Complex[,] Input)
        {
            xPoints = Width = Input.GetLength(0);
            yPoints = Height = Input.GetLength(1);
            Fourier = Input;

        }

        /// <summary>
        /// Parameterized Constructor for FFT
        /// </summary>
        /// <param name="Input">Greyscale Array</param>
        public FastFourierTransformation(int[,] Input)
        {
            GreyImage = Input;
            Width = xPoints = Input.GetLength(0);
            Height = yPoints = Input.GetLength(1);
        }

        /// <summary>
        /// Parameterized Constructor for FFT
        /// </summary>
        public FastFourierTransformation()
        {
        }

        /// <summary>
        /// Function to Read Bitmap to greyscale Array
        /// </summary>
        private void ReadImage()
        {
            int i, j;
            int bytesPerPixel = 4;
            GreyImage = new int[Width, Height];  //[Row,Column]
            BitmapData bitmapData = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* imagePointer = (byte*)bitmapData.Scan0;
                for (i = 0; i < bitmapData.Height; i++)
                {
                    for (j = 0; j < bitmapData.Width; j++)
                    {
                        GreyImage[j, i] = (int)((imagePointer[0] + imagePointer[1] + imagePointer[2]) / 3.0);
                        imagePointer += bytesPerPixel;
                    }
                    imagePointer += bitmapData.Stride - (bitmapData.Width * bytesPerPixel);
                }
            }
            Image.UnlockBits(bitmapData);
        }

        public Bitmap ConvertToImage()
        {
            int i, j;
            Bitmap image = new Bitmap(Width, Height);
            BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, Width, Height),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {

                byte* imagePointer1 = (byte*)bitmapData1.Scan0;

                for (i = 0; i < bitmapData1.Height; i++)
                {
                    for (j = 0; j < bitmapData1.Width; j++)
                    {
                        // write the logic implementation here
                        imagePointer1[0] = (byte)GreyImage[j, i];
                        imagePointer1[1] = (byte)GreyImage[j, i];
                        imagePointer1[2] = (byte)GreyImage[j, i];
                        imagePointer1[3] = (byte)255;
                        //4 bytes per pixel
                        imagePointer1 += 4;
                    }//end for j

                    //4 bytes per pixel
                    imagePointer1 += (bitmapData1.Stride - (bitmapData1.Width * 4));
                }//end for i
            }//end unsafe
            image.UnlockBits(bitmapData1);
            return image;// col;
        }

        public Bitmap ConvertToImage(int[,] image)
        {
            int i, j;
            Bitmap output = new Bitmap(image.GetLength(0), image.GetLength(1));
            BitmapData bitmapData1 = output.LockBits(new Rectangle(0, 0, image.GetLength(0), image.GetLength(1)),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* imagePointer1 = (byte*)bitmapData1.Scan0;
                for (i = 0; i < bitmapData1.Height; i++)
                {
                    for (j = 0; j < bitmapData1.Width; j++)
                    {
                        imagePointer1[0] = (byte)image[j, i];
                        imagePointer1[1] = (byte)image[j, i];
                        imagePointer1[2] = (byte)image[j, i];
                        imagePointer1[3] = 255;
                        //4 bytes per pixel
                        imagePointer1 += 4;
                    }//end for j
                    //4 bytes per pixel
                    imagePointer1 += (bitmapData1.Stride - (bitmapData1.Width * 4));
                }//end for i
            }//end unsafe
            output.UnlockBits(bitmapData1);
            return output;// col;

        }

        /// <summary>
        /// Calculate Forward Fast Fourier Transform of Input Image
        /// </summary>
        public void Forward()
        {
            //Initializing Fourier Transform Array
            int i, j;
            Fourier = new Complex[Width, Height];
            Output = new Complex[Width, Height];

            //Copy Image Data to the Complex Array
            for (i = 0; i < Width; i++)
                for (j = 0; j < Height; j++)
                {
                    Fourier[i, j] = (double)GreyImage[i, j];
                }

            //Calling Forward Fourier Transform
            Output = FFT2D(Fourier, xPoints, yPoints, 1);
        }

        /// <summary>
        /// Shift The FFT of the Image
        /// </summary>
        public Complex[] AddShift(Complex[] normal, int count)
        {
            int i;
            Complex[] shifted = new Complex[count];

            int countlimit = (count / 2);

            for (i = 0; i < countlimit; i++)
            {
                shifted[i + countlimit] = normal[i];
                shifted[i] = normal[i + countlimit];
            }
            return shifted;
        }

        /// <summary>
        /// Shift The FFT of the Image
        /// </summary>
        public Complex[,] AddShift(Complex[,] normal, int xPoints, int yPoints)
        {
            int i, j;
            Complex[,] shifted = new Complex[xPoints, yPoints];

            int xlimit = (xPoints / 2);
            int ylimit = (yPoints / 2);

            for (i = 0; i < xlimit; i++)
                for (j = 0; j < ylimit; j++)
                {
                    shifted[i + xlimit, j + ylimit] = normal[i, j];
                    shifted[i, j] = normal[i + xlimit, j + ylimit];
                    shifted[i + xlimit, j] = normal[i, j + ylimit];
                    shifted[i, j + ylimit] = normal[i + xlimit, j];
                }
            return shifted;
        }

        /// <summary>
        /// Shift The FFT of the Image
        /// </summary>
        public void AddShift()
        {
            int i, j;
            FFTShifted = new Complex[xPoints, yPoints];

            int xlimit = (xPoints / 2);
            int ylimit = (yPoints / 2);

            for (i = 0; i < xlimit; i++)
                for (j = 0; j < ylimit; j++)
                {
                    FFTShifted[i + xlimit, j + ylimit] = Output[i, j];
                    FFTShifted[i, j] = Output[i + xlimit, j + ylimit];
                    FFTShifted[i + xlimit, j] = Output[i, j + ylimit];
                    FFTShifted[i, j + ylimit] = Output[i + xlimit, j];
                }
        }

        /// <summary>
        /// Removes FFT Shift for FFTshift Array
        /// </summary>
        public void RemoveShift()
        {
            int i, j;
            FFTNormal = new Complex[xPoints, yPoints];

            int xlimit = (xPoints / 2);
            int ylimit = (yPoints / 2);

            for (i = 0; i < xlimit; i++)
                for (j = 0; j < ylimit; j++)
                {
                    FFTNormal[i + xlimit, j + ylimit] = FFTShifted[i, j];
                    FFTNormal[i, j] = FFTShifted[i + xlimit, j + ylimit];
                    FFTNormal[i + xlimit, j] = FFTShifted[i, j + ylimit];
                    FFTNormal[i, j + ylimit] = FFTShifted[i + xlimit, j];
                }
        }

        /// <summary>
        /// FFT Plot Method for Shifted FFT
        /// </summary>
        /// <param name="Output"></param>
        public void Plot(Complex[,] Output)
        {
            int i, j;
            float max;

            FFTLog = new float[xPoints, yPoints];
            FFTPhaseLog = new float[xPoints, yPoints];

            FourierMagnitude = new float[xPoints, yPoints];
            FourierPhase = new float[xPoints, yPoints];

            FFTNormalized = new int[xPoints, yPoints];
            FFTPhaseNormalized = new int[xPoints, yPoints];

            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FourierMagnitude[i, j] = (float)Output[i, j].Magnitude;
                    FourierPhase[i, j] = (float)Output[i, j].Phase;
                    FFTLog[i, j] = (float)Math.Log(1 + FourierMagnitude[i, j]);
                    FFTPhaseLog[i, j] = (float)Math.Log(1 + Math.Abs(FourierPhase[i, j]));
                }

            //Generating Magnitude Bitmap
            max = FFTLog[0, 0];
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    if (FFTLog[i, j] > max)
                        max = FFTLog[i, j];
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTLog[i, j] = FFTLog[i, j] / max;
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTNormalized[i, j] = (int)(2000 * FFTLog[i, j]);
                }

            //Transferring Image to Fourier Plot
            FourierPlot = ConvertToImage(FFTNormalized);

            //generating phase Bitmap
            FFTPhaseLog[0, 0] = 0;
            max = FFTPhaseLog[1, 1];
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    if (FFTPhaseLog[i, j] > max)
                        max = FFTPhaseLog[i, j];
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTPhaseLog[i, j] = FFTPhaseLog[i, j] / max;
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTPhaseNormalized[i, j] = (int)(255 * FFTPhaseLog[i, j]);
                }

            //Transferring Image to Fourier Plot
            PhasePlot = ConvertToImage(FFTPhaseNormalized);
        }

        /// <summary>
        /// generate FFT Image for Display Purpose
        /// </summary>
        public void Plot()
        {
            int i, j;
            float max;
            FFTLog = new float[xPoints, yPoints];
            FFTPhaseLog = new float[xPoints, yPoints];

            FourierMagnitude = new float[xPoints, yPoints];
            FourierPhase = new float[xPoints, yPoints];

            FFTNormalized = new int[xPoints, yPoints];
            FFTPhaseNormalized = new int[xPoints, yPoints];

            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FourierMagnitude[i, j] = (float)Output[i, j].Magnitude;
                    FourierPhase[i, j] = (float)Output[i, j].Phase;
                    FFTLog[i, j] = (float)Math.Log(1 + FourierMagnitude[i, j]);
                    FFTPhaseLog[i, j] = (float)Math.Log(1 + Math.Abs(FourierPhase[i, j]));
                }

            //Generating Magnitude Bitmap
            max = FFTLog[0, 0];
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    if (FFTLog[i, j] > max)
                        max = FFTLog[i, j];
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTLog[i, j] = FFTLog[i, j] / max;
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTNormalized[i, j] = (int)(1000 * FFTLog[i, j]);
                }

            //Transferring Image to Fourier Plot
            FourierPlot = ConvertToImage(FFTNormalized);

            //generating phase Bitmap
            max = FFTPhaseLog[0, 0];
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    if (FFTPhaseLog[i, j] > max)
                        max = FFTPhaseLog[i, j];
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTPhaseLog[i, j] = FFTPhaseLog[i, j] / max;
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTPhaseNormalized[i, j] = (int)(2000 * FFTLog[i, j]);
                }

            //Transferring Image to Fourier Plot
            PhasePlot = ConvertToImage(FFTPhaseNormalized);
        }

        /// <summary>
        /// Calculate Inverse from Complex [,]  Fourier Array
        /// </summary>
        public void Inverse()
        {
            //Initializing Fourier Transform Array
            int i, j;

            //Calling Forward Fourier Transform
            Output = new Complex[xPoints, yPoints];
            Output = FFT2D(Fourier, xPoints, yPoints, -1);

            Image = null;  // Setting Object Image to Null

            //Copying Real Image Back to Greyscale
            //Copy Image Data to the Complex Array
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    GreyImage[i, j] = (int)Output[i, j].Magnitude;

                }
            Image = ConvertToImage(GreyImage);
        }

        /// <summary>
        /// Generates Inverse FFT of Given Input Fourier
        /// </summary>
        /// <param name="Fourier"></param>
        public void Inverse(Complex[,] Fourier)
        {
            //Initializing Fourier Transform Array
            int i, j;

            //Calling Forward Fourier Transform
            Output = new Complex[xPoints, yPoints];
            Output = FFT2D(Fourier, xPoints, yPoints, -1);

            //Copying Real Image Back to Greyscale
            //Copy Image Data to the Complex Array
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    GreyImage[i, j] = (int)Output[i, j].Magnitude;

                }
            Image = ConvertToImage(GreyImage);
        }

        /*-------------------------------------------------------------------------
            Perform a 2D FFT inplace given a complex 2D array
            The direction dir, 1 for forward, -1 for reverse
            The size of the array (xPoints,yPoints)
            Return false if there are memory problems or
            the dimensions are not powers of 2
        */
        public Complex[,] FFT2D(Complex[,] input, int xPoints, int yPoints, int dir)
        {
            int i, j;
            int power; //Power of 2 for current number of points
            double[] real;
            double[] imag;
            //COMPLEX[,] output = new COMPLEX[xPoints, yPoints] ; //Copying Array COMPLEX [xPoints,yPoints]
            Complex[,] output = input; //Copying Array COMPLEX [xPoints,yPoints]


            // Transform the Rows 
            real = new double[xPoints];
            imag = new double[xPoints];
            power = (int)Math.Log((double)xPoints, 2); //Finding power of 2 for current number of points e.g. for nx=512 m=9

            for (j = 0; j < yPoints; j++)
            {
                for (i = 0; i < xPoints; i++)
                {
                    real[i] = input[i, j].Real;
                    imag[i] = input[i, j].Imaginary;
                }

                // Calling 1D FFT Function for Rows
                FFT1D(dir, power, ref real, ref imag);

                for (i = 0; i < xPoints; i++)
                {
                    output[i, j] = new Complex(real[i], imag[i]);
                }
            }

            // Transform the columns  
            real = new double[yPoints];
            imag = new double[yPoints];
            power = (int)Math.Log((double)yPoints, 2); //Finding power of 2 for current number of points e.g. for nx=512 m=9

            for (i = 0; i < xPoints; i++)
            {
                for (j = 0; j < yPoints; j++)
                {
                    real[j] = input[i, j].Real;
                    imag[j] = input[i, j].Imaginary;
                }

                // Calling 1D FFT Function for Columns
                FFT1D(dir, power, ref real, ref imag);

                for (j = 0; j < yPoints; j++)
                {
                    output[i, j] = new Complex(real[j], imag[j]);
                }
            }

            return output;
        }

        /*-------------------------------------------------------------------------
            This computes an in-place complex-to-complex FFT
            x and y are the real and imaginary arrays of 2^m points.
            dir = 1 gives forward transform
            dir = -1 gives reverse transform
            Formula: forward
                     N-1
                      ---
                    1 \         - j k 2 pi n / N
            X(K) = --- > x(n) e                  = Forward transform
                    N /                            n=0..N-1
                      ---
                     n=0
            Formula: reverse
                     N-1
                     ---
                     \          j k 2 pi n / N
            X(n) =    > x(k) e                  = Inverse transform
                     /                             k=0..N-1
                     ---
                     k=0
            */
        private void FFT1D(int dir, int power, ref double[] real, ref double[] imag)
        {
            long nn, i, i1, j, k, l, l1, l2;
            double c1, c2, tx, ty, t1, t2, u1, u2, z;

            /* Calculate the number of points */
            nn = 1;
            for (i = 0; i < power; i++)
                nn *= 2;

            /* Do the bit reversal */
            i1 = nn >> 1;
            j = 0;
            for (i = 0; i < nn - 1; i++)
            {
                if (i < j)
                {
                    tx = real[i];
                    ty = imag[i];
                    real[i] = real[j];
                    imag[i] = imag[j];
                    real[j] = tx;
                    imag[j] = ty;
                }
                k = i1;
                while (k <= j)
                {
                    j -= k;
                    k >>= 1;
                }
                j += k;
            }

            /* Compute the FFT */
            c1 = -1.0;
            c2 = 0.0;
            l2 = 1;
            for (l = 0; l < power; l++)
            {
                l1 = l2;
                l2 <<= 1;
                u1 = 1.0;
                u2 = 0.0;
                for (j = 0; j < l1; j++)
                {
                    for (i = j; i < nn; i += l2)
                    {
                        i1 = i + l1;
                        t1 = u1 * real[i1] - u2 * imag[i1];
                        t2 = u1 * imag[i1] + u2 * real[i1];
                        real[i1] = real[i] - t1;
                        imag[i1] = imag[i] - t2;
                        real[i] += t1;
                        imag[i] += t2;
                    }
                    z = u1 * c1 - u2 * c2;
                    u2 = u1 * c2 + u2 * c1;
                    u1 = z;
                }
                c2 = Math.Sqrt((1.0 - c1) / 2.0);
                if (dir == 1)
                    c2 = -c2;
                c1 = Math.Sqrt((1.0 + c1) / 2.0);
            }

            /* Scaling for forward transform */
            if (dir == 1)
            {
                for (i = 0; i < nn; i++)
                {
                    real[i] /= (double)nn;
                    imag[i] /= (double)nn;
                }
            }

        }

        public Complex[] FFT1D(double[] input, int? roundDigits = null)
        {
            double real, imag;
            int length = input.Length;
            Complex[] output = new Complex[length];

            for (int k = 0; k < length; k++)
            {
                real = 0;
                imag = 0;
                for (int n = 0; n < length; n++)
                {
                    real += input[n] * Math.Cos((-2 * Math.PI * k * n) / length);
                    imag += input[n] * Math.Sin((-2 * Math.PI * k * n) / length);
                }

                if (roundDigits.HasValue)
                {
                    real = Math.Round(real, roundDigits.Value);
                    imag = Math.Round(imag, roundDigits.Value);
                }
                output[k] = new Complex(real, imag);
            }

            return output;
        }

    }
}
