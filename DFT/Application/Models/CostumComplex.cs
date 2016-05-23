using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Web;

namespace DFT.Application.Models
{
    public class CostumComplex
    {
        public static CostumComplex[] Parse(Complex[] array)
        {
            int length = array.Length;
            CostumComplex[] result = new CostumComplex[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Parse(array[i]);
            }
            return result;
        }
        public static CostumComplex Parse(Complex complex)
        {
            return new CostumComplex(complex.Real, complex.Imaginary);
        }


        public double Real
        {
            get
            {
                return real;
            }
        }

        private double real;

        public double Imaginary
        {
            get
            {
                return imaginary;
            }
        }

        private double imaginary;

        public double Magnitude
        {
            get
            {
                return Math.Sqrt(Math.Pow(real, 2) + Math.Pow(imaginary, 2));
            }
        }

        public double Phase
        {
            get
            {
                return Math.Atan2(imaginary, real);
            }
        }

        public override String ToString()
        {
            return "{ " + real.ToString().Replace(',','.') + ", " + imaginary.ToString().Replace(',', '.') + "i }";
        }


        public CostumComplex(double real, double imaginary)
        {
            this.real = real;
            this.imaginary = imaginary;
        }

    }
}