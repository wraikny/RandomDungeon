using System;
namespace Delaunay
{
    public class Circle
    {
        public Point center;
        public float radius;

        public Circle(Point c, float r)
        {
            this.center = c;
            this.radius = r;
        }

        public void Draw(asd.Layer2D layer)
        {
            var arc = new asd.CircleShape
            {
                OuterDiameter = radius * 2,
                Position = new asd.Vector2DF(center.x, center.y)
            };
            var geometryObj = new asd.GeometryObject2D
            {
                Shape = arc,
                Color = new asd.Color(0, 0, 255),
                Position = new asd.Vector2DF(0, 0)
            };
            layer.AddObject(geometryObj);
        }
    }
}
