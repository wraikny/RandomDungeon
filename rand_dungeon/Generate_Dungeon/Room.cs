using System;
using System.Linq;

namespace rand_dungeon
{
    public class Room : asd.GeometryObject2D
    {
        public asd.Vector2DF size;
        asd.RectangleShape rect;

        public asd.Vector2DF lu, rd;

        public bool moving;

        asd.Vector2DF beforePos;

        asd.Vector2DF center;

        Dungeon dungeon;
        int tileSize;

        public bool isMainRoom { get; set; }
        public bool isCorridorRoom { get; set; }

        float latency = (float)3f; // over 2, 10 is very good

        public int label { get; set; }
        public float Latency { get => latency; set => latency = value; }

        float N;

        int cnt;

        public Room(asd.Vector2DF pos, asd.Vector2DF size, float N)
        {
            Position = pos;
            this.size = size;

            rect = new asd.RectangleShape();
            rect.DrawingArea = new asd.RectF(-size / 2, size);
            Shape = rect;

            Color = new asd.Color(255, 255, 255, 200);

            lu = Position - size / 2;
            rd = Position + size / 2;

            moving = false;
            tileSize = Dungeon.tileSize;

            isMainRoom = false;
            isCorridorRoom = false;

            this.N = N;

            cnt = 0;
        }

        protected override void OnAdded()
        {
            base.OnAdded();

            this.dungeon = (Dungeon)Layer.Scene;
            this.center = dungeon.center;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!dungeon.stopRoom)
            {
                beforePos = Position;
                Collide();
                moving = !((beforePos - Position).Length < cnt / 60 / Latency / latency / N / N / 5000);
                cnt++;
            }

            if (moving)
            {
                Color = new asd.Color(255, 0, 0, 200);
            }
            else if (!isMainRoom && !isCorridorRoom)
            {
                Color = new asd.Color(255, 255, 255, 200);
            }
            else if (isMainRoom)
            {
                Color = new asd.Color(0, 0, 200, 255);
            }
            else if (isCorridorRoom)
            {
                Color = new asd.Color(155, 150, 100, 255);
            }
        }

        asd.Vector2DF VRandm(asd.Vector2DF v, int m)
        {
            return new asd.Vector2DF((float)(Math.Floor(((v.X + m - 1) / m)) * m),
                                     (float)(Math.Floor(((v.Y + m - 1) / m)) * m));
        }

        public void UpdatePos(asd.Vector2DF dpos)
        {
            Position += dpos;
            Position = VRandm(Position, tileSize);
            lu = Position - size / 2;
            rd = Position + size / 2;
        }

        public void Round()
        {
            Position = VRandm(Position, tileSize);
            lu = Position - size / 2;
            rd = Position + size / 2;
        }

        public bool IsCollidedWith(Room room)
        {
            return room != this && !(rd.X < room.lu.X || room.rd.X < lu.X) && !(rd.Y < room.lu.Y || room.rd.Y < lu.Y);
        }

        void Collide()
        {
            foreach (Room obj in Layer.Objects.OfType<Room>().Where(x => IsCollidedWith(x)))
            {

                if ((Position - ((Dungeon)Layer.Scene).cameraPos).SquaredLength > (obj.Position - ((Dungeon)Layer.Scene).cameraPos).SquaredLength)
                {
                    Move(obj);
                    obj.Move(this);
                }
                else
                {
                    obj.Move(this);
                    Move(obj);
                }
            }
        }

        void Move(Room obj)
        {
            float xd = 0, yd = 0;
            asd.Vector2DF dpos;

            if (obj.Position.X < Position.X)
            {
                xd = Math.Abs(obj.rd.X - lu.X);
            }
            else
            {
                xd = Math.Abs(rd.X - obj.lu.X);
            }

            if (obj.Position.Y < Position.Y)
            {
                yd = Math.Abs(obj.rd.Y - lu.Y);
            }
            else
            {
                yd = Math.Abs(rd.Y - obj.lu.Y);
            }

            yd = (float)Math.Max(0, yd + N * latency * 0.9);
			xd = (float)Math.Max(0, xd + N * latency * 0.9);

            if (xd < yd)
            {
                dpos = new asd.Vector2DF(xd * Math.Sign(Position.X - obj.Position.X), 0);
            }
            else
            {
                dpos = new asd.Vector2DF(0, yd * Math.Sign(Position.Y - obj.Position.Y));
            }

            if (!(Math.Abs(dpos.X) < 0.0001 && (Math.Abs(dpos.Y) < 0.0001)))
            {
                UpdatePos(dpos / Latency);
            }
        }
    }
}
