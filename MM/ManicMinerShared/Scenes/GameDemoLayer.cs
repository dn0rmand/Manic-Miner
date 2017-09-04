using System;
using System.Collections.Generic;
using CocosSharp;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;
using CocosDenshion;

namespace ManicMiner
{
	public class GameDemoLayer : BaseGameLayer
    {
    	int _room ;
    
		public GameDemoLayer(int room) : base(room == 20 ? "final" : "ingame", true)
        {
        	_room = room;
        }

        protected override void StopMusic ()
		{
			if (_room == Game.LevelCount)
				base.StopMusic ();
		}

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

			Game.ROOM = _room;
			Game.LIVES= 5;
			DrawRoom();

			this.AddTouchListener((touches, ccevent) => {
#if DEBUG
				if (_room < Game.LevelCount)
					GotoDemoScene(_room+1);
				else
#endif
					GotoPianoScene();
			});
		}

		void DrawRoom()
		{
			int lives = -1;

			this.UnscheduleAll();
			this.RemoveAllChildren(true);

			Game.CopyRoom(true);
			Game.BLOCKoff = (Game.ROOM-1)*16;
			 
			this.DrawAirBackground();
			this.Print(0, 16*8, Game.cTITLE, 0);

			this.DrawAir(false);
			this.DrawLives(ref lives, 0, true);

			this.BuildRoom();

			this.DrawCrumb();
			this.DrawKeys();
			this.DrawSwitches();
			this.DrawConv();
			this.DoRobots();
			this.DoSpecialRobot();
			this.DrawExit();

			int index = 0;

			Schedule(t => this.DrawKeys(), Game.KeyAnimationSpeed);
			Schedule(t => this.DrawConv(), Game.ConvAnimationSpeed);
			Schedule(t => this.AnimateLives(ref index, true), Game.WillyAnimationSpeed);
			Schedule(t => 
			{
				this.DoRobots();
				this.DoSpecialRobot();
			}, Game.RobotAnimationSpeed);
			Schedule(t => this.DrawExit(), 		Game.ExitAnimationSpeed);
			Schedule(t => this.DrawAir(false),	Game.AirSpeed);
			ScheduleOnce(t => this.GotoDemoScene(_room + 1), 10); // 10 seconds
        }

		void GotoDemoScene(int room)
		{
			if (room > Game.LevelCount)
				GotoPianoScene();
			else
				Window.DefaultDirector.SwitchScene(GameDemoLayer.CreateScene(Window, room));
		}

		public static CCScene CreateScene(CCWindow mainWindow)
		{
			return CreateScene(mainWindow, 1);
		}

		static CCScene CreateScene(CCWindow mainWindow, int room)
        {
        	return InnerCreateScene(mainWindow, new GameDemoLayer(room));
//			CCSize winSize = new CCSize(256, 192); // 256,192
        }
    }
}