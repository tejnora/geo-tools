using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using CAD.Utils;

namespace CAD.Images
{

    public enum ImageType
    {
        itCit
    };

    public interface IImage
    {
        ImageType Type { get; }
        string FileLocation { get; set; }
        void saveToFile(string aLocation);
        BitmapSource ImageData{get;}
        bool getTestImage();
        System.Drawing.Rectangle getRect();
    }
}
