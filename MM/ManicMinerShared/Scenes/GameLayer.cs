using System;
using System.Collections.Generic;
using CocosSharp;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;
using CocosDenshion;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Text;

namespace ManicMiner
{
	public class GameLayer : BaseGameLayer
    {
    	int 	_room;
    	int		_openScore, _openLives;
		int 	_oldScore, _oldHiScore, _oldLives;
		bool	_restored = false ;
		Menu	_menu = null;

		public GameLayer(int room) : base(room == Game.LevelCount ? "final" : "ingame", false)
        {
			_room = room;
			_openScore = Game.Score ;
			_openLives = Game.LIVES;
		}

		GameLayer(int room, int score, int lives) : base(room == Game.LevelCount ? "final" : "ingame", false)
        {
			_room 	   = room;
			_openScore = Game.Score = score ;
			_openLives = Game.LIVES = lives ;
			_restored  = true ; 
		}

		public static CCScene CreateScene(CCWindow mainWindow, string state)
		{
			if (string.IsNullOrWhiteSpace(state))
				return null;

			string[] values = state.Split(new char[] { ',' }, 3);
			if (values.Length != 3)
				return null;
			
			int room, lives, score;

			if (! int.TryParse(values[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out room))
				return null;

			if (! int.TryParse(values[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out score))
				return null;

			if (! int.TryParse(values[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out lives))
				return null;
			
			if (room < 1 || room > Game.LevelCount)
				return null;
			if (score < 0)
				return null;
			if (lives < 0)
				return null;

			return InnerCreateScene(mainWindow, new GameLayer(room, score, lives));
		}

		public string SaveState()
		{
			Game.GAMEmode = 5; // Pause Game
			return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", _room, _openScore, _openLives);
		}

		public override void OnExit ()
		{
			_menu = null;
			base.OnExit ();
		}

		void SetMainTouchListener()
		{
			Func<CCTouch, CCEvent, bool> 	startHandler = (touch, ccevent) => (touch.Location.Y >= 50 && Game.GAMEmode == 1);
			Action<CCTouch, CCEvent>		endHandler   = (touch, ccevent) => Game.GAMEmode = 5; // Show Pause Menu

			var touchListener = new CCEventListenerTouchOneByOne ();

			touchListener.OnTouchBegan = startHandler.Weak();
			touchListener.OnTouchEnded = endHandler.Weak();

			AddEventListener(touchListener, this);
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();
			Game.InitGame(_room);
			if (_restored)
				Game.GAMEmode = 5;
			DrawRoom();
		}

		void DrawRoom()
		{
			ManicMinerApplicationDelegate.SaveHighscore(Game.HiScore);
			_oldLives = _oldHiScore = _oldScore = -1;

			Game.CopyRoom(false);
			Game.BLOCKoff = (Game.ROOM-1)*16;
			 
			this.DrawAirBackground();
			this.Print(0, 16*8, Game.cTITLE, 0);
			this.Print(0, 19*8, "High Score          Score", 6);
			this.PrintScore(ref _oldScore);
			this.PrintHiScore(ref _oldHiScore);

			this.DrawAir(false);
			this.DrawLives(ref _oldLives, Game.LIVESidx);

			this.BuildRoom();

			this.DrawCrumb();
			this.DrawKeys();
			this.DrawSwitches();
			this.DrawConv();
			this.DoRobots();
			this.DoWilly();
			this.DoSpecialRobot();
			this.DrawExit();

			_oldLives = Game.LIVES;

//	256 - 40 = 216
//  216 - 40 = 176
//
//  192 - 40 = 152

			var jump  = this.AddSprite("jump",   0, 152);
			var left  = this.AddSprite("left", 176, 152);
			var right = this.AddSprite("right",216, 152);

			this.AddTouchListener(jump,
				t => { Game.JumpOn = true; },
				t => { Game.JumpOn = false; });
			this.AddTouchListener(left,
				t => { Game.LeftOn = true; },
				t => { DelayAction(() => { Game.LeftOn = false; }); });
			this.AddTouchListener(right,
				t => { Game.RightOn = true; },
				t => { DelayAction(() => { Game.RightOn = false; }); });
			
			SetMainTouchListener();

			Schedule(t => this.DrawKeys(), 		Game.KeyAnimationSpeed);
			Schedule(t => this.DrawConv(), 		Game.ConvAnimationSpeed);
			Schedule(t => this.DrawExit(), 		Game.ExitAnimationSpeed);
			Schedule(t => this.DrawAir(false), 	Game.AirSpeed);
			Schedule(t => this.MainLoop(),		Game.RobotAnimationSpeed);					
			Schedule(t => this.AnimateLives(ref Game.LIVESidx, true), Game.WillyAnimationSpeed);
        }

        void DelayAction(Action action)
        {
        #if DEBUG
        	if (MonoTouch.ObjCRuntime.Runtime.Arch == MonoTouch.ObjCRuntime.Arch.SIMULATOR)
        	{
				MonoTouch.Foundation.NSTimer.CreateScheduledTimer(2, () => { action(); });
			}
			else
				action();
		#else
			action();
		#endif
        }
         
        void Blink(Action afterBlink)
		{
			Game.GAMEmode = 0;

			var blink = new CCBlink(0.5f, 4);
			var callback = new CCCallFunc(afterBlink);
			var sequence = new CCSequence(blink, callback);

			this.RunAction(sequence);
        }

        void MainLoop()
        {
			switch (Game.GAMEmode)
			{
				case 0:
					// Does nothing
					break;

				case 1:
					this.DrawCrumb();
					this.DrawSwitches();
					this.DoRobots();
					this.DoWilly();
					this.DoSpecialRobot();
					this.PrintScore(ref _oldScore);
					this.PrintHiScore(ref _oldHiScore);
					this.DrawLives(ref _oldLives, Game.LIVESidx);
					break;

				case 2:
					this.CheckExtraLive();
					this.Blink(() =>
					{
						if (! ManicMinerApplicationDelegate.InfiniteLives)
							Game.LIVES--;
						if (Game.LIVES==0)
							GotoGameOverScene();
						else
							GotoRoom(Game.ROOM);
					});
					break;

				case 3:
					this.LevelDone(() => 
					{
						if (Game.ROOM == 1)
							GotoGameEndScene(); 
						else
							GotoRoom(Game.ROOM);
					});
					break;
				
				
				case 5: 
					// Pause and Ask to Quit or resume
					if (_menu == null)
					{
						if (ManicMinerApplicationDelegate.UnlockDoors)
							_menu = Menu.Create("PAUSED", "Resume", "End Game", "Next Level");
						else
							_menu = Menu.Create("PAUSED", "Resume", "End Game");
						
						_menu.Clicked += (sender, e) =>
						{
							switch(e)
							{
								case 1:
									Game.GAMEmode = 1;
									_menu.Visible = false;
									this.Resume();
									break;

								case 2:
									GotoPianoScene();
									break;

								case 3:
									Game.GAMEmode = 3; // Next Level
									_menu.Visible = false;
									this.Resume();
									break;							
							}
						};

						this.AddChild(_menu);
					}
					else
						_menu.Visible = true;

					this.Pause();
					break;
				
				// For Debug Purpose
				case 6:
					GotoGameEndScene();
					break;
			}
	    }

		void GotoGameEndScene()
	    {
			Window.DefaultDirector.SwitchScene(GameEndLayer.CreateScene(Window));
	    }

	    void GotoGameOverScene()
	    {
			Window.DefaultDirector.SwitchScene(GameOverLayer.CreateScene(Window));
	    }

		void GotoRoom(int room)
		{
			Window.DefaultDirector.SwitchScene(GameLayer.CreateScene(Window, room));
		}

		public static CCScene CreateScene(CCWindow mainWindow)
		{
			return CreateScene(mainWindow, 0);
		}

		static CCScene CreateScene(CCWindow mainWindow, int room)
        {
			return InnerCreateScene(mainWindow, new GameLayer(room));
        }
    }
}
