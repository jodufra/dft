using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DFTLib
{
    public static class FileManager
    {
        private static List<String> ImagesExtensions = new List<string>() { ".bmp" };

        public static String UploadPhoto(HttpPostedFileBase postedFileBase, String filename)
        {
            if (postedFileBase != null)
            {
                var folder = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["Folders:Photos"].Replace('/', '\\') + "\\";
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);
                
                if (postedFileBase.ContentLength == 0 || !ImagesExtensions.Contains(System.IO.Path.GetExtension(postedFileBase.FileName)))
                    return null;
                filename = System.IO.Path.GetFileNameWithoutExtension(filename) + ".bmp";
                postedFileBase.SaveAs(folder + filename);
                return filename;
            }
            return null;
        }

        public static String UploadPhoto(Image image, String filename)
        {
            try
            {
                var folder = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["Folders:Photos"].Replace('/', '\\') + "\\";
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);
                filename = System.IO.Path.GetFileNameWithoutExtension(filename) + ".bmp";
                image.Save(folder + filename, ImageFormat.Bmp);
                return filename;
            }
            catch
            {
                return null;
            }
        }

    }
}
