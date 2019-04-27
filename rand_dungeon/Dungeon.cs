using System;
using System.Collections.Generic;
using System.Linq;

using Delaunay;
using SpanningTree;

namespace rand_dungeon
{
    public class Dungeon : asd.Scene
    {
        public float cameraN = 1f;
        float timesN = 1f;

        asd.Layer2D roomLayer = new asd.Layer2D();
        asd.Layer2D graphLayer = new asd.Layer2D();
        asd.Layer2D corridorLayer = new asd.Layer2D();

        public const int tileSize = 4;
        public asd.Vector2DF center = new asd.Vector2DF(asd.Engine.WindowSize.X / 2, asd.Engine.WindowSize.Y / 2);

        bool doneDelaunay;
        bool doneTreed;
        bool doneCorridorLine;
        bool doneCorridor;

        public bool doneMakeDungeon;

        float wMean, hMean;
        public asd.Vector2DF cameraPos;

        public bool stopRoom;
        bool selectedMain;

        public List<Line> LineList { get; set; }

        public List<Point> pointList;
        public List<Room> mainRoomList;

        asd.Vector2DF range;

        public Dungeon()
        {
            Serialize();
        }

        void Serialize()
        {
            wMean = 0;
            hMean = 0;

            stopRoom = false;
            selectedMain = false;
            doneDelaunay = false;
            doneTreed = false;
            doneCorridorLine = false;
            doneCorridor = false;
            doneMakeDungeon = false;

            AddLayer(corridorLayer);
            AddLayer(roomLayer);
            AddLayer(graphLayer);

            corridorLayer.AddObject(new Camera());
            roomLayer.AddObject(new Camera());
            graphLayer.AddObject(new Camera());

            int n = 500;
            for (int i = 0; i < n; i++)
            {
                int x = tileSize * 16, y = tileSize * 16;
                int lx = tileSize * 3, ly = tileSize * 3;

                x = (int)(x * timesN);
                y = (int)(y * timesN);

                var size = new asd.Vector2DF(Randm(lx + (x - lx) * Global.random.NextDouble(), tileSize),
                                             Randm(ly + (y - ly) * Global.random.NextDouble(), tileSize));

                wMean += size.X;
                hMean += size.Y;

                float w = (float)(50 + 50 * Global.random.NextDouble());
                float h = (float)(50 + 50 * Global.random.NextDouble());
                Room room = new Room(RandomPointsInCircle(w * timesN, h * timesN), size, timesN);
                roomLayer.AddObject(room);
            }

            wMean /= n;
            hMean /= n;

            pointList = new List<Point>();
            LineList = new List<Line>();
            mainRoomList = new List<Room>();

            isSceneChanging = false;
        }

        void Deserialize()
        {
            foreach (asd.Layer2D layer in Layers)
            {
                layer.Clear();
                RemoveLayer(layer);
            }
        }

        bool isSceneChanging;

        protected override void OnUpdated()
        {
            base.OnUpdated();
            if (!stopRoom)
            {
                var rooms = roomLayer.Objects.OfType<Room>();
                CameraPos(rooms);
                cameraN = Math.Max(cameraN, 1.2f * timesN);
                stopRoom = rooms.All(x => !x.moving);
            }
            else if (!selectedMain)
            {
                foreach(Room room in roomLayer.Objects.OfType<Room>())
                {
                    room.Round();
                }
                SelectMain();
                selectedMain = true;
            }
            else if (mainRoomList.Count() < 10)
            {
                Deserialize();
                Serialize();
            }
            else if (!doneDelaunay)
            {
                DelaunayDiagram();
                doneDelaunay = true;
            }
            else if(!doneTreed)
            {
                Tree();
                doneTreed = true;
            }
            else if(!doneCorridorLine)
            {
                CorridorLine();
                doneCorridorLine = true;
            }
            else if(!doneCorridor)
            {
                Corridor();
                doneCorridor = true;
            }
            else if(!doneMakeDungeon)
            {
                foreach(Room room in roomLayer.Objects.OfType<Room>().Where(x => !x.isMainRoom && !x.isCorridorRoom))
                {
                    roomLayer.RemoveObject(room);
                    room.Dispose();
                }
                var rooms = roomLayer.Objects.OfType<Room>().Where(x => x.isMainRoom || x.isCorridorRoom);
                CameraPos(rooms);

                doneMakeDungeon = true;
            }
            else if(!isSceneChanging && asd.Engine.Keyboard.GetKeyState(asd.Keys.Enter) == asd.KeyState.Push)
            {
                isSceneChanging = true;
                Deserialize();
                Serialize();
                /*
                var scene = new Dungeon();
                asd.Engine.ChangeSceneWithTransition(scene, new asd.TransitionFade(0.0f, 0.5f));
                */
            }

            //cameraN = 1f;
        }

        void CameraPos(IEnumerable<Room> rooms)
        {
            cameraPos.X = rooms.Average(x => x.Position.X);
            cameraPos.Y = rooms.Average(x => x.Position.Y);

            rooms = rooms.OrderBy(x => x.Position.X);
            range.X = rooms.Last().rd.X - rooms.First().lu.X;
            cameraPos.X = (rooms.First().lu.X + rooms.Last().rd.X) / 2;

            rooms = rooms.OrderBy(x => x.Position.Y);
            range.Y = rooms.Last().rd.Y - rooms.First().lu.Y;
            cameraPos.Y = (rooms.First().lu.Y + rooms.Last().rd.Y) / 2;

            cameraN = (float)Math.Max(range.X / asd.Engine.WindowSize.X, range.Y / asd.Engine.WindowSize.Y) * 1.05f;
        }

        void SelectMain()
        {   
            var label = 0;
            foreach (var room in roomLayer.Objects.OfType<Room>())
            {
                if(room.size.X > wMean * 1.25 && room.size.Y > hMean * 1.25)
                {
                    room.isMainRoom = true;
                    roomLayer.RemoveObject(room);
                    roomLayer.AddObject(room);

                    var point = new Point(room.Position);
                    point.Draw(graphLayer, cameraN);

                    point.label = label;
                    room.label = label;

                    pointList.Add(point);
                    mainRoomList.Add(room);

                    label++;
                }
                else if ((room.size.X < wMean * 0.1 || room.size.Y < hMean * 0.1))
                {
                    room.Dispose();
                }
                else if(roomLayer.Objects.OfType<Room>()
                        .Any(x => !(x.rd.X < room.lu.X + tileSize * 2 || room.rd.X - tileSize * 2 < x.lu.X) && 
                                  !(x.rd.Y < room.lu.Y + tileSize * 2 || room.rd.Y - tileSize * 2 < x.lu.Y) && x != room))
                {
                    room.Dispose();
                }
            }
        }

        void DelaunayDiagram()
        {
            var delaunay = new DelaunayTriangles(pointList, graphLayer, range, this);
            delaunay.Draw();
            doneDelaunay = true;
        }

        void Tree()
        {
            int vNum = roomLayer.Objects.OfType<Room>().Count(x => x.isMainRoom);

            var kruskal = new Kruskal(vNum, graphLayer.Objects.OfType<Line>());
            LineList = kruskal.Compute();

            var unSelectedLines = new List<Line>();

            foreach (Line line in graphLayer.Objects.OfType<Line>().Where(x => !(LineList.Any(x.Equals))))
            {
                graphLayer.RemoveObject(line);
                unSelectedLines.Add(line);
            }

            foreach (Line line in new List<Line>(unSelectedLines)
                     .OrderBy(i => Guid.NewGuid())
                     .Take((int)(unSelectedLines.Count() * 0.1))
                    )
            {
                graphLayer.AddObject(line);
                unSelectedLines.Remove(line);
            }
            foreach(Line line in unSelectedLines)
            {
                line.Dispose();
            }
        }

        void CorridorLine()
        {
            foreach (Line line in graphLayer.Objects.OfType<Line>())
            {
                Room staR = mainRoomList[line.start.label];
                Room endR = mainRoomList[line.end.label];
                asd.Vector2DF mid = line.mid;

                asd.Vector2DF start, end;
                Line corridorLine;

                if ((staR.lu.Y < mid.Y && mid.Y < staR.rd.Y) &&
                   (endR.lu.Y < mid.Y && mid.Y < endR.rd.Y) &&
                   ((staR.rd.X < endR.lu.X) || (endR.rd.X < staR.lu.X))
                  )
                {
                    start = new asd.Vector2DF(staR.Position.X, mid.Y);
                    end = new asd.Vector2DF(endR.Position.X, mid.Y);

                    corridorLine = new Line(start, end);
                    corridorLayer.AddObject(corridorLine);
                }
                else if ((staR.lu.X < mid.X && mid.X < staR.rd.X) &&
                         (endR.lu.X < mid.X && mid.X < endR.rd.X) &&
                         ((staR.rd.Y < endR.lu.Y) || (endR.rd.Y < staR.lu.Y))
                        )
                {
                    start = new asd.Vector2DF(mid.X, staR.Position.Y);
                    end = new asd.Vector2DF(mid.X, endR.Position.Y);

                    corridorLine = new Line(start, end);
                    corridorLayer.AddObject(corridorLine);
                }
                else
                {
                    start = new asd.Vector2DF(staR.Position.X, endR.Position.Y);
                    end = new asd.Vector2DF(endR.Position.X, staR.Position.Y);

                    corridorLine = new Line(staR.Position, start);
                    corridorLayer.AddObject(corridorLine);

                    corridorLine = new Line(staR.Position, end);
                    corridorLayer.AddObject(corridorLine);

                    corridorLine = new Line(endR.Position, start);
                    corridorLayer.AddObject(corridorLine);

                    corridorLine = new Line(endR.Position, end);
                    corridorLayer.AddObject(corridorLine);
                }

            }
        }

        void Corridor()
        {
            var corridorList = new List<Corridor>();

            foreach(Line line in corridorLayer.Objects.OfType<Line>())
            {
                asd.Vector2DF size;
                float thickness = (float)(3 * tileSize);

				size.X = (float)(Convert.ToInt32(Equals(line.start.pos.Y, line.end.pos.Y)) * Math.Sqrt(line.sqLength) + thickness);
                size.Y = (float)(Convert.ToInt32(Equals(line.start.pos.X, line.end.pos.X)) * Math.Sqrt(line.sqLength) + thickness);

                var corridor = new Corridor(line.mid, size, timesN);
                corridorList.Add(corridor);
                corridorLayer.AddObject(corridor);

                corridorLayer.RemoveObject(line);
                line.Dispose();
            }
            foreach(Room room in roomLayer.Objects.OfType<Room>()
                    .Where(x => !x.isMainRoom && 
                           (x.size.X < wMean * 1.25 || x.size.Y < hMean * 1.25)))
            {
                room.isCorridorRoom = corridorList
                    .Any(obj => (!(obj.rd.X - tileSize/2 < room.lu.X || room.rd.X < obj.lu.X + tileSize/2) && 
                                 !(obj.rd.Y - tileSize/2 < room.lu.Y || room.rd.Y < obj.lu.Y + tileSize/2)));
            }
        }

        public float Randm(double n, int m)
        {
            return (float)(Math.Floor(((n + m - 1) / m)) * m);
        }

        asd.Vector2DF RandomPointsInCircle(float rX, float rY)
        {
            float t = (float)(2 * Math.PI * Global.random.NextDouble());
            float r = (float)(Global.random.NextDouble() + Global.random.NextDouble());
            if (r > 1) r = 2 - r;
            float x = Randm(rX * r * Math.Cos(t), tileSize);
            float y = Randm(rY * r * Math.Sin(t), tileSize);
            return new asd.Vector2DF(x, y);
        }
    }
}
