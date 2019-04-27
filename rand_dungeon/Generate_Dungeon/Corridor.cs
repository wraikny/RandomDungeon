using System;
namespace rand_dungeon
{
    public class Corridor : asd.GeometryObject2D
    {
        public asd.Vector2DF size;
        asd.RectangleShape rect;

        public asd.Vector2DF lu, rd;

        asd.Vector2DF center;

        Dungeon dungeon;

        float N;

        public Corridor(asd.Vector2DF pos, asd.Vector2DF size, float N)
        {
            Position = pos;
            this.size = size;

            rect = new asd.RectangleShape
            {
                DrawingArea = new asd.RectF(-size / 2, size)
            };
            Shape = rect;

            Color = new asd.Color(50, 50, 50, 255);

            lu = Position - size / 2;
            rd = Position + size / 2;

            this.N = N;
        }

        protected override void OnAdded()
        {
            base.OnAdded();

            this.dungeon = (Dungeon)Layer.Scene;
            this.center = dungeon.center;
            /*
            Position = VRandm(Position, tileSize);
            lu = VRandm(Position - size / 2, tileSize);
            rd = VRandm(Position + size / 2, tileSize);
            */
        }

        asd.Vector2DF VRandm(asd.Vector2DF v, int m)
        {
            return new asd.Vector2DF((float)(Math.Floor(((v.X + m - 1) / m)) * m),
                                     (float)(Math.Floor(((v.Y + m - 1) / m)) * m));
        }
    }
}
