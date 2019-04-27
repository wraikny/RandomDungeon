using System;

namespace rand_dungeon
{
    class MainClass
    {

        [STAThread]
        static void Main(string[] args)
        {
            asd.Engine.Initialize("Dungeon", 800, 800, new asd.EngineOption());
#if false
            asd.Engine.File.AddRootDirectory("Resources/");
#endif
            var scene = new Dungeon();

            asd.Engine.ChangeSceneWithTransition(scene, new asd.TransitionFade(0.0f, 0.5f));

            while (asd.Engine.DoEvents())
            {
                if (asd.Engine.Keyboard.GetKeyState(asd.Keys.Escape) == asd.KeyState.Push)
                {
                    break;
                }

                asd.Engine.Update();
            }

            asd.Engine.Terminate();
        }
    }
}
