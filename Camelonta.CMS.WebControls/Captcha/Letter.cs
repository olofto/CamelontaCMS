using System;
using System.Drawing;
using System.Linq;

/// <summary>
/// Summary description for Letter
/// </summary>
public class Letter
{
    readonly string[] ValidFonts = { "Segoe Script", "Century", "Eccentric Std", "Freestyle Script", "Viner Hand ITC" };
    public Letter(char c)
    {
        Random rnd = new Random();
        font = new Font(ValidFonts[rnd.Next(ValidFonts.Count() - 1)], rnd.Next(20) + 20, GraphicsUnit.Pixel);
        letter = c;
    }
    public Font font
    {
        get;
        private set;
    }
    public Size LetterSize
    {
        get
        {
            var Bmp = new Bitmap(1, 1);
            var Grph = Graphics.FromImage(Bmp);
            return Grph.MeasureString(letter.ToString(), font).ToSize();
        }
    }
    public char letter
    {
        get;
        private set;
    }
    public int space
    {
        get;
        set;
    }

}
