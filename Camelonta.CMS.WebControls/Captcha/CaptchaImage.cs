/*

Scrambled text generator.
Developed by: Aref Karimi
Email: Arefkr@gmail.com
Last update: 20 October 2009

This code is free to use. 
 
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.IO;
using System.Reflection;

namespace Camelonta.CMS.WebControls
{
    public class CaptchaImage : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/jpeg";
            var CaptchaText = SecurityHelper.DecryptString(
                Convert.FromBase64String(context.Request.QueryString["CaptchaText"]));
            if (CaptchaText != null)
            {
                List<Letter> letter = new List<Letter>();
                int TotalWidth = 0;
                int MaxHeight = 0;
                foreach (char c in CaptchaText)
                {
                    var ltr = new Letter(c);
                    letter.Add(ltr);
                    int space = (new Random()).Next(5) + 1;
                    ltr.space = space;
                    System.Threading.Thread.Sleep(1);
                    TotalWidth += ltr.LetterSize.Width + space;
                    if (MaxHeight < ltr.LetterSize.Height)
                        MaxHeight = ltr.LetterSize.Height;
                    System.Threading.Thread.Sleep(1);
                }
                const int HMargin = 5;
                const int VMargin = 3;

                Bitmap bmp = new Bitmap(TotalWidth + HMargin, MaxHeight + VMargin);
                var Grph = Graphics.FromImage(bmp);
                Grph.FillRectangle(new SolidBrush(Color.Lavender), 0, 0, bmp.Width, bmp.Height);
                Pixelate(ref bmp, context);
                int xPos = HMargin;
                foreach (var ltr in letter)
                {
                    Grph.DrawString(ltr.letter.ToString(), ltr.font, new SolidBrush(Color.Navy), xPos, VMargin);
                    xPos += ltr.LetterSize.Width + ltr.space;
                }

                bmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        private void Pixelate(ref Bitmap bmp, HttpContext context)
        {
            var grp = Graphics.FromImage(bmp);
            System.Drawing.Image background = GetBackground();
            grp.DrawImage(background, new Rectangle(0, 0, bmp.Width, bmp.Height));
        }

        public System.Drawing.Image GetBackground()
        {
            System.Drawing.Imaging.ImageFormat imgFormat = System.Drawing.Imaging.ImageFormat.Png;

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("Camelonta.CMS.WebControls.Captcha.background.png");

            // Create an Image object from the stream
            System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
            
            return img;
        }
    }
}