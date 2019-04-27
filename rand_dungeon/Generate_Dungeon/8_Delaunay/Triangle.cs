using System;

using rand_dungeon;

namespace Delaunay
{
    public class Triangle
    {
        public Point p1, p2, p3;

        public Circle outCircle;

        public Triangle(asd.Vector2DF p1, asd.Vector2DF p2, asd.Vector2DF p3)
        {
            this.p1 = new Point(p1);
            this.p2 = new Point(p2);
            this.p3 = new Point(p3);

            GetCircumscribedCirclesOfTriangle();
        }
        public Triangle(Point p1, Point p2, Point p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            GetCircumscribedCirclesOfTriangle();
        }

        public override bool Equals(Object obj) 
        {  
            try {
                Triangle t = (Triangle)obj;
                // ※ 同値判定に頂点を用いると、  
                // 三角形の頂点の順番を網羅的に考慮する分条件判定が多くなる。  
                return(p1.Equals(t.p1) && p2.Equals(t.p2) && p3.Equals(t.p3) ||  
                       p1.Equals(t.p2) && p2.Equals(t.p3) && p3.Equals(t.p1) ||  
                       p1.Equals(t.p3) && p2.Equals(t.p1) && p3.Equals(t.p2) ||
                       p1.Equals(t.p3) && p2.Equals(t.p2) && p3.Equals(t.p1) ||  
                       p1.Equals(t.p2) && p2.Equals(t.p1) && p3.Equals(t.p3) ||  
                       p1.Equals(t.p1) && p2.Equals(t.p3) && p3.Equals(t.p2) );  
            }
            catch
            {  
                return false;
            }  
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public void Draw(asd.Layer2D layer)
        {
            var poss = new Point[3] { p1, p2, p3 };

            for (int index = 0; index < 3; index++)
            {
                var line = new Line(poss[index], poss[(index + 1) % 3]);
                layer.AddObject(line);
            }
        }

        public bool HasCommonPoints(Triangle t)
        {  
            return (p1.Equals(t.p1) || p1.Equals(t.p2) || p1.Equals(t.p3) ||  
                    p2.Equals(t.p1) || p2.Equals(t.p2) || p2.Equals(t.p3) ||  
                    p3.Equals(t.p1) || p3.Equals(t.p2) || p3.Equals(t.p3) );  
        }

        // ======================================  
        // 三角形を与えてその外接円を求める  
        // ======================================  
        void GetCircumscribedCirclesOfTriangle()
        {
            // 三角形の各頂点座標を (x1, y1), (x2, y2), (x3, y3) とし、  
            // その外接円の中心座標を (x, y) とすると、  
            //     (x - x1) * (x - x1) + (y - y1) * (y - y1)  
            //   = (x - x2) * (x - x2) + (y - y2) * (y - y2)  
            //   = (x - x3) * (x - x3) + (y - y3) * (y - y3)  
            // より、以下の式が成り立つ  
            //  
            // x = { (y3 - y1) * (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1)  
            //     + (y1 - y2) * (x3 * x3 - x1 * x1 + y3 * y3 - y1 * y1)} / c  
            //  
            // y = { (x1 - x3) * (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1)  
            //     + (x2 - x1) * (x3 * x3 - x1 * x1 + y3 * y3 - y1 * y1)} / c  
            //  
            // ただし、  
            //   c = 2 * {(x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1)}
            var t = this;
            float x1 = t.p1.x;
            float y1 = t.p1.y;
            float x2 = t.p2.x;
            float y2 = t.p2.y;
            float x3 = t.p3.x;
            float y3 = t.p3.y;

            float c = (float)(2.0 * ((x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1)));
            float x = ((y3 - y1) * (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1)
                     + (y1 - y2) * (x3 * x3 - x1 * x1 + y3 * y3 - y1 * y1)) / c;
            float y = ((x1 - x3) * (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1)
                     + (x2 - x1) * (x3 * x3 - x1 * x1 + y3 * y3 - y1 * y1)) / c;
            Point center = new Point(x, y);

            // 外接円の半径 r は、半径から三角形の任意の頂点までの距離に等しい  
            float r = center.Dist(t.p1);

            this.outCircle =  new Circle(center, r);
        }
    }
}
