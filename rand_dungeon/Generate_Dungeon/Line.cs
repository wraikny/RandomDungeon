using System;
using Delaunay;

namespace rand_dungeon
{
    public class Line : asd.GeometryObject2D
    {
        asd.LineShape line = new asd.LineShape();

        public Point start, end;
        public float sqLength;

        public asd.Vector2DF mid;

        public Line(Point start, Point end)
        {
            line.StartingPosition = start.pos;
            line.EndingPosition = end.pos;

            this.start = start;
            this.end = end;
        }
        public Line (asd.Vector2DF start, asd.Vector2DF end)
        {
            line.StartingPosition = start;
            line.EndingPosition = end;

            this.start = new Point(start);
            this.end = new Point(end);
        }

        protected override void OnAdded()
        {
            base.OnAdded();

            Serialize();
        }

        void Serialize()
        {
            line.Thickness = (float)2.5f * ((Dungeon)Layer.Scene).cameraN;

            Shape = line;
            Color = new asd.Color(255, 255, 0, 200);
            Position = new asd.Vector2DF(0, 0);

            sqLength = start.SqDist(end);
            mid = (start.pos + end.pos) / 2;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(Object obj)
        {
            try
            {
                Line p = (Line)obj;
                return ((Equals(start, p.start) && Equals(end, p.end))) || ((Equals(start, p.end) && Equals(end, p.start)));
            }
            catch
            {
                return false;
            }
        }
    }
}
