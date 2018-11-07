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
            String text = "";   //maybe I need a "wait for whole string" function, that makes a string out of all chars are pressed until the user clicks somewhere again!
            if (c >= 32)
            {

                s.figures.Add(new Figure("TekstTool", startpunt, startpunt, s.PenKleur, c.ToString()));     ////////

                Graphics gr = s.MaakBitmapGraphics();
                
                
                s.figures[s.figures.Count-1].DrawFigure(gr);
                
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
            this.Bezig(s.CreateGraphics(), this.startpunt, p);      
        }
        public override void MuisLos(SchetsControl s, Point p)
        {   base.MuisLos(s, p);
            this.Compleet(s,s.MaakBitmapGraphics(), this.startpunt, p);                         //added SchetsControl as a parameter to complete, since they need the figures-list of the schetscontrol object
            s.Invalidate();
        }
        public override void Letter(SchetsControl s, char c)
        {
        }
        public abstract void Bezig(Graphics g, Point p1, Point p2);     


        public abstract void Compleet(SchetsControl s, Graphics g, Point p1, Point p2);        //added SchetsControl as a parameter to complete, since they need the figures-list of the schetscontrol object

    }

    public class RechthoekTool : TweepuntTool       
    {
        public override string ToString() { return "kader"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {   g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));      
           
        }
        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            s.figures.Add(new Figure("RechthoekTool", p1, p2, s.PenKleur, ""));
            s.figures[s.figures.Count - 1].DrawFigure(g);
        }
    }
    
    public class VolRechthoekTool : RechthoekTool
    {
        public override string ToString() { return "vlak"; }

        public override void Compleet(SchetsControl s,Graphics g, Point p1, Point p2)
        {   
            s.figures.Add(new Figure("VolRechthoekTool", p1, p2, s.PenKleur, ""));
            for (int i = 0; i < s.figures.Count; i++)
            {
                s.figures[i].DrawFigure(g);
            }
        }
    }

    public class CircleTool : TweepuntTool     
    {                                           
        public override string ToString() { return "ovaal"; }   

        public override void Bezig (Graphics g, Point p1, Point p2)
        {
            g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2)); 
        }
        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            s.figures.Add(new Figure("CircleTool", p1, p2, s.PenKleur, ""));
            for (int i = 0; i < s.figures.Count; i++)
            {
                s.figures[i].DrawFigure(g);
            }
        }
    }

    public class VolCircleTool : TweepuntTool
    {
        public override string ToString() { return "ovaal2"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }
        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            s.figures.Add(new Figure("VolCircleTool", p1, p2, s.PenKleur, ""));
            for (int i = 0; i < s.figures.Count; i++)
            {
                s.figures[i].DrawFigure(g);
            }
        }
    }

        public class LijnTool : TweepuntTool
        {
            public override string ToString() { return "lijn"; }

            public override void Bezig(Graphics g, Point p1, Point p2)
            {
                g.DrawLine(MaakPen(this.kwast, 3), p1, p2);
            }
            public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
            {
                s.figures.Add(new Figure("LijnTool", p1, p2, s.PenKleur, ""));
                for (int i = 0; i < s.figures.Count; i++)
                {
                    s.figures[i].DrawFigure(g);
                }
                    
            }
        }

    public class PenTool : LijnTool
    {
        public override string ToString() { return "pen"; }

        public override void MuisDrag(SchetsControl s, Point p)
        {   this.MuisLos(s, p);
            this.MuisVast(s, p);
        }
        
        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            s.figures.Add(new Figure("PenTool", p1, p2, s.PenKleur, ""));
            for (int i = 0; i < s.figures.Count; i++)
            {
                s.figures[i].DrawFigure(g);
            }
        }
        
    }
    
    //new Gum Tool
    
    public class GumTool : StartpuntTool                            // no need to be a pen or even a tweepunt tool anymore
    {
        public override string ToString() { return "gum"; }

        public override void Letter(SchetsControl s, char c) { }   //they are only defined to make the ISchetsTool Interface happy
        
        public override void MuisDrag(SchetsControl s, Point p) { } //they are only defined to make the ISchetsTool Interface happy
        
        public override void MuisVast(SchetsControl s, Point p)
        {
            startpunt = p;
            Delete(s, p, s.MaakBitmapGraphics());
        }
        
        public void Delete (SchetsControl s, Point p, Graphics g)
        { 

            for (int i = s.figures.Count - 1; i >= 0; i--)       //delete the clicked on figure //-1 because Count will give you the third object as 3 but its index is 2
            {
                if (raakt(s.figures[i],p))                      //if startpunt is inside the s.figure[i] 'area' we need the 'raakt' function for this
                {
                    s.figures.RemoveAt(i);
                    break;
                }
             }
            for (int i = 0; i < s.figures.Count; i++)       //draw all figures after the to be deleted has be removed
            {
                s.figures[i].DrawFigure(g);                  
            }

        }
        public bool raakt(Figure f,Point p)
        {
            bool touche = false;
            if (f.soort == "RechthoekTool")
                ;
            else if (f.soort == "VolRechthoekTool")
                if (f.startpunt.X < p.X && p.X < f.endpunt.X && f.startpunt.Y < p.Y && p.Y < f.endpunt.Y)
                    touche = true;
            else if (f.soort == "CircleTool")
                ;//formula for circle perimeter 
            else if (f.soort == "VolCircleTool")
                ;
            else if (f.soort == "LijnTool")
                ;
            else if (f.soort == "PenTool")
                ;
            else if (f.soort == "TekstTool")
                ;


            return touche;
        }
        

    }
    
}
