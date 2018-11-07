using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public abstract class StartpuntTool : ISchetsTool
    {
        protected Point startpunt;
        protected Brush kwast;

        public virtual void MuisVast(SchetsControl s, Point p)
        {   startpunt = p;
        }
        public virtual void MuisLos(SchetsControl s, Point p)
        {   kwast = new SolidBrush(s.PenKleur);
        }
        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);
    }

    public class TekstTool : StartpuntTool
    {
        public override string ToString() { return "tekst"; }

        public override void MuisDrag(SchetsControl s, Point p) { }

        public override void Letter(SchetsControl s, char c)
        {
            if (c >= 32)
            {
                Graphics gr = s.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz = 
                gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
                gr.DrawString   (tekst, font, kwast, 
                                              this.startpunt, StringFormat.GenericTypographic);
                // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
                startpunt.X += (int)sz.Width;
                s.Invalidate();
            }
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {   return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                                , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                                );
        }
        public static Pen MaakPen(Brush b, int dikte)
        {   Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public override void MuisVast(SchetsControl s, Point p)
        {   base.MuisVast(s, p);
            kwast = Brushes.Gray;
        }
        public override void MuisDrag(SchetsControl s, Point p)
        {   s.Refresh();
            this.Bezig(s,s.CreateGraphics(), this.startpunt, p);        //added SchetsControl as a parameter to Bezig, since they need the figures-list of the schetscontrol object
        }
        public override void MuisLos(SchetsControl s, Point p)
        {   base.MuisLos(s, p);
            this.Compleet(s,s.MaakBitmapGraphics(), this.startpunt, p);     //added SchetsControl as a parameter to Bezig, since they need the figures-list of the schetscontrol object
            s.Invalidate();
        }
        public override void Letter(SchetsControl s, char c)
        {
        }
        public abstract void Bezig(SchetsControl s, Graphics g, Point p1, Point p2);     //added SchetsControl as a parameter to Bezig, since they need the figures-list of the schetscontrol object
        
        
        public virtual void Compleet(SchetsControl s,Graphics g, Point p1, Point p2)        //added SchetsControl as a parameter to complete, since they need the figures-list of the schetscontrol object
        {   this.Bezig(s, g, p1, p2);
        }
    }

    public class RechthoekTool : TweepuntTool       
    {
        public override string ToString() { return "kader"; }

        public override void Bezig(SchetsControl s, Graphics g, Point p1, Point p2)
        {   //g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));       //this will be replaced
            //the problem is  figures is (or should be)a property of a schets object! but in this class is no schets object! this thing kinda needs to be in SchetsControl ! (has UserControl de bitmap bewerking as well?)
            // The tool methods get the schetsControl as a parameter ! that's how they acces the bitmap!
            s.figures.Add(new Figure("RechthoekTool",p1,p2,kwast,""));                           //this is the replacement           
            s.figures.ForEach(Console.WriteLine);
            Console.ReadLine();
            for (int i = 0; i < s.figures.Count; i++)                                        //this is the replacement
            {
                s.figures[i].DrawFigure(g);
            }                                                                               //this is the replacement

        }
    }
    
    public class VolRechthoekTool : RechthoekTool
    {
        public override string ToString() { return "vlak"; }

        public override void Compleet(SchetsControl s,Graphics g, Point p1, Point p2)
        {   g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
            
        }
    }

    public class CircleTool : TweepuntTool     
    {                                           
        public override string ToString() { return "ovaal"; }   

        public override void Bezig (SchetsControl s, Graphics g, Point p1, Point p2)
        {
            g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2)); 
        }
    }

    public class VolCircleTool : TweepuntTool
    {
        public override string ToString() { return "ovaal2"; }   

        public override void Bezig(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));    // is Punten2Rechthoek okay or should we write Punten2Circle?
        }
    }

    public class LijnTool : TweepuntTool
    {
        public override string ToString() { return "lijn"; }

        public override void Bezig(SchetsControl s, Graphics g, Point p1, Point p2)
        {   g.DrawLine(MaakPen(this.kwast,3), p1, p2);
        }
    }

    public class PenTool : LijnTool
    {
        public override string ToString() { return "pen"; }

        public override void MuisDrag(SchetsControl s, Point p)
        {   this.MuisLos(s, p);
            this.MuisVast(s, p);
        }
    }

    //old gum tool
    /*
    public class GumTool : PenTool
    {
        public override string ToString() { return "gum"; }
        public override void Bezig(Graphics g, Point p1, Point p2)      
        {   g.DrawLine(MaakPen(Brushes.White, 7), p1, p2);
        }
    }
    */
    
    //new Gum Tool
    // instead of drawing on the bitmap, only a click position is recorded
    //then the element / figure in the "list of all drawn figures" that corresponds to that position is removed from that list
    //to be sophisticated you could go up the list from last to first and stop after you found a drawing this ensures only the last figure drawn there gets deleted.
    //also when the bitmap is drawn from this list going down the list from first to last, later figures paint over earlier figures. nice.
    //to do this, maybe the bitmap should be drawn from that list only
    //the list that is necessary next to the bitmap also needs a mehtod that will reconstruct (or just construct) the bitmap based on that list.
    //the method that draws a figure probably needs to be changed dramatically 
    public class GumTool : StartpuntTool                            // no need to be a pen or even a tweepunt tool anymore
    {
        public override string ToString() { return "gum"; }

        public override void Letter(SchetsControl s, char c) { }   //they are only defined to make the ISchetsTool Interface happy
        
        public override void MuisDrag(SchetsControl s, Point p) { } //they are only defined to make the ISchetsTool Interface happy

        // Figure selected;  
        // for (i=figures.Length;i>0;i--)
        //     if (figure[i].position == this.startpoint)
        //          figures.delete[i]
        //          break
        // 
        // invalidate(); //if necessary  
        
        
    }
    
}