using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GeoCalculations.Contours
{
    public static class ContoursFileParser
    {
        public static IList<ContourNode> Parse(string path)
        {
            var fileContent = File.ReadAllLines(path);
            var points = new List<ContourNode>(fileContent.Count());
            foreach (var line in fileContent)
            {
                try
                {
                    var items = Regex.Split(line.Trim(' '), @"[,]{1,1}").ToArray();
                    var pointNumber = items[0];
                    var cordinateY = double.Parse(items[1], CultureInfo.InvariantCulture);
                    var cordinageX = double.Parse(items[2], CultureInfo.InvariantCulture);
                    var height = double.Parse(items[3], CultureInfo.InvariantCulture);
                    points.Add(new ContourNode(pointNumber, cordinageX, cordinateY, height));
                }
                catch { }
            }
            return points;
        }
    }
}
