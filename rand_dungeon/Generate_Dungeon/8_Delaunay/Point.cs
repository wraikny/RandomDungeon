using System;
namespace Delaunay
{
    public class Point
    {
        public float x, y;
        public asd.Vector2DF pos;

        public int label{ get; set; }

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
            pos = new asd.Vector2DF(x, y);
        }
        public Point(asd.Vector2DF v)
        {
            this.x = v.X;
            this.y = v.Y;
            pos = new asd.Vector2DF(x, y);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(Object obj)
        {
            try {
                Point p = (Point)obj;
                return (Equals(x, p.x) && Equals(y, p.y));
            }
            catch
            {
                return false;
            }
        }

        public void Draw(asd.Layer2D layer, float cameraN=1)
        {
            var geometryObj = new asd.GeometryObject2D();

            var arc = new asd.CircleShape();

            arc.OuterDiameter = 8 * cameraN;
            arc.InnerDiameter = 0;
            arc.Position = pos;

            geometryObj.Shape = arc;
            geometryObj.Color = new asd.Color(255, 255, 0, 200);
            geometryObj.Position = new asd.Vector2DF(0, 0);

            layer.AddObject(geometryObj);
        }

        public float SqDist(Point p)
        {
            return (p.pos - pos).SquaredLength;
        }
        public float Dist(Point p)
        {
            return (p.pos - pos).Length;
        }

    }
}
