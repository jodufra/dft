using DFT.Application.Models;
using DFTLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Web;
using System.Web.Mvc;

namespace DFT.Controllers
{
    public class DFTController : Controller
    {
        public class DFTImageModel
        {
            public String Input { get; set; }
            public String Mag { get; set; }
            public String Phase { get; set; }
            public String Inverse { get; set; }
            public bool HasImages
            {
                get
                {
                    return !String.IsNullOrEmpty(Input) &&
                        !String.IsNullOrEmpty(Mag);
                }
            }

            public String Src(String filename)
            {
                if (String.IsNullOrEmpty(filename))
                {
                    return filename;
                }
                return "/" + ConfigurationManager.AppSettings["Folders:Photos"] + "/" + filename;
            }
        }
        // GET: DFT
        public ActionResult Image(HttpPostedFileBase image)
        {
            var model = new DFTImageModel();
            if (image != null)
            {
                Image input = Bitmap.FromStream(image.InputStream, true, true);
                model.Input = FileManager.UploadPhoto(input, "input");

                FastFourierTransformation fft = new FastFourierTransformation((Bitmap)input);
                fft.Forward();
                fft.AddShift();
                fft.Plot(fft.FFTShifted);
                //fft.Plot();
                fft.Inverse();

                Image mag = (Image)fft.FourierPlot;
                model.Mag = FileManager.UploadPhoto(mag, "mag");

                Image phase = (Image)fft.PhasePlot;
                model.Phase = FileManager.UploadPhoto(phase, "phase");

                Image inverse = (Image)fft.Image;
                model.Inverse = FileManager.UploadPhoto(inverse, "inverse");
            }
            return View(model);
        }



        public class DFTCalculationModel
        {
            public string Input { get; set; }
            public List<double> Values { get; set; }
            public List<string> Errors { get; set; }
            public CostumComplex[] Output { get; set; }

        }
        // GET: DFT
        public ActionResult Calculation()
        {
            var model = new DFTCalculationModel();
            model.Input = Request["values"];
            model.Values = new List<double>();
            model.Errors = new List<string>();

            if ( !String.IsNullOrEmpty(model.Input))
            {
                foreach (var item in model.Input.Split(' ').Where(v => !String.IsNullOrEmpty(v)).ToList())
                {
                    try
                    {
                        model.Values.Add(Double.Parse(item.Replace(',', '.'), CultureInfo.InvariantCulture));
                    }
                    catch
                    {
                        model.Errors.Add(item + " is and invalid input value");
                    }
                }

                if (!model.Errors.Any())
                {
                    FastFourierTransformation fft = new FastFourierTransformation();
                    model.Output = CostumComplex.Parse(fft.FFT1D(model.Values.ToArray(), 4));

                }

            }

            return View(model);
        }

    }
}