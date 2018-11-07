using System;
using System.Collections.Generic;
using System.Drawing;

namespace SchetsEditor
{
    public class Schets
    {
        private Bitmap bitmap;

        List<Figure> figures { get; set; }     //declaring a member variable for the list of figures next to the bitmap

        public Schets()
        {
            bitmap = new Bitmap(1, 1);
            figures = new List<Figure>();   //initializing the figures with an empty figure list 
        }
        public Graphics BitmapGraphics
        {
            get { return Graphics.FromImage(bitmap); }
        }
        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
            {
                Bitmap nieuw = new Bitmap(Math.Max(sz.Width, bitmap.Size.Width)
                                         , Math.Max(sz.Height, bitmap.Size.Height)
                                         );
                Graphics gr = Graphics.FromImage(nieuw);
                gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
                gr.DrawImage(bitmap, 0, 0);
                bitmap = nieuw;

            }
        }
        public void Teken(Graphics gr)
        {
            gr.DrawImage(bitmap, 0, 0);

        }
        public void Schoon()
        {
            Graphics gr = Graphics.FromImage(bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
        }
        public void Roteer()
        {
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }
    }
    public class Figure     //defining a type "figure", of which the objects will represent the user-drawn figures and stored in the figures list and painted on the bitmap from there.
    {
        ISchetsTool soort;
        Point startpunt;
        Point endpunt;
        Brush kleur;
        String text;

        public Figure(ISchetsTool tempSoort, Point tempStartpunt, Point tempEndpunt, Brush tempKleur) // this is a second constructor for when there is no text. Is there a better way for optional parameters?
        {
            soort = tempSoort;
            startpunt = tempStartpunt;
            endpunt = tempEndpunt;
            kleur = tempKleur;
        }

        public Figure(ISchetsTool tempSoort, Point tempStartpunt, Point tempEndpunt, Brush tempKleur, String tempText)
        {
            soort = tempSoort;
            startpunt = tempStartpunt;
            endpunt = tempEndpunt;
            kleur = tempKleur;
            text = tempText;
        }
        public void DrawFigure(Graphics gr)
        {
            if (this.soort is RechthoekTool)    //this line might activate VolRechthoekTools as well
                gr.DrawRectangle(TweepuntTool.MaakPen(this.kleur, 3), TweepuntTool.Punten2Rechthoek(this.startpunt, this.endpunt));
            else if (this.soort is VolRechthoekTool)
                ;
        }        
    }
}
