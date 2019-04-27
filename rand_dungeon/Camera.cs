using System;
using System.Linq;

namespace rand_dungeon
{
    public class Camera : asd.CameraObject2D
    {
        public asd.Vector2DF pos;
        float N;
        public Camera()
        {
            pos = new asd.Vector2DF(0, 0);

            Src = new asd.RectI((int)(pos.X - asd.Engine.WindowSize.X / 2), (int)(pos.Y - asd.Engine.WindowSize.Y / 2), 
                                asd.Engine.WindowSize.X, asd.Engine.WindowSize.Y);
            
            Dst = new asd.RectI(0, 0, asd.Engine.WindowSize.X, asd.Engine.WindowSize.Y);
        }
        protected override void OnAdded()
        {
            base.OnAdded();

            N = ((Dungeon)Layer.Scene).cameraN;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            N = ((Dungeon)Layer.Scene).cameraN;
            pos = ((Dungeon)Layer.Scene).cameraPos;

            Src = new asd.RectI((int)(pos.X - asd.Engine.WindowSize.X / 2 * N), (int)(pos.Y - asd.Engine.WindowSize.Y / 2 * N), 
                                (int)(asd.Engine.WindowSize.X * N), (int)(asd.Engine.WindowSize.Y * N));
        }
    }
}
