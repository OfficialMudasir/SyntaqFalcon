using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace Syntaq.Falcon.Web.Areas.Falcon.Utility
{
    public static class IconUtility
    {
        public static HtmlString Generate(string FullName)
        {
            var name1 = "?";
            var name2 = "?";
            if (!string.IsNullOrEmpty(FullName))
            {
                string[] names = FullName.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                name1 = names.Length > 0 ? names[0] : "?";
                 name2 = names.Length > 1 ? names[1] : "?";
            }



            string genimage = GenerateCircle(name1, name2);
            return new HtmlString($"<img src='data:image/png;base64, {genimage}'/>");
        }

        private static List<string> _BackgroundColours = new List<string> {
            "1abc9c", "2ecc71", "3498db", "9b59b6", "34495e", "16a085", "27ae60", "2980b9", "8e44ad", "2c3e50", "f1c40f", "e67e22", "e74c3c", "95a5a6", "f39c12", "d35400", "c0392b", "bdc3c7", "7f8c8d"
        };
        private static string GenerateCircle(string firstName, string lastName)
        {
            var avatarString = string.Format("{0}{1}", firstName[0], lastName[0]).ToUpper();

            var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
            var bgColour = _BackgroundColours[randomIndex];

            var bmp = new Bitmap(32, 32);
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            var font = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(bmp);

            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            using (Brush b = new SolidBrush(ColorTranslator.FromHtml("#" + bgColour)))
            {
                graphics.FillEllipse(b, new Rectangle(0, 0, 30, 30));
            }
            graphics.DrawString(avatarString, font, new SolidBrush(Color.WhiteSmoke), 15, 15, sf);
            graphics.Flush();

            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            byte[] imageBytes = ms.ToArray();
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
    }
}
