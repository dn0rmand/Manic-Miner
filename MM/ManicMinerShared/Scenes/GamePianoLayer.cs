using System;
using System.Collections.Generic;
using CocosSharp;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;
using CocosDenshion;

namespace ManicMiner
{
    public class GamePianoLayer : BaseGameLayer
    {
		const int _windowWidth 	= 320 - 64;
		const int _charWidth 	= 8;
		const int _maxChar 		= (_windowWidth / _charWidth) + 1 ;

		const string _scrollText = 
				"MANIC MINER . . . BUG-BYTE ltd. 1983 . .  By Matthew Smith . . . " +
				"Miner Willy, while prospecting down Surbiton way, stumbles upon an " +
				"ancient, long forgotten mine-shaft. On further exploration, he finds " +
				"evidence of a lost civilisation far superior to our own, which used " +
				"automatons to dig deep into the Earth's core to supply the essential raw " +
				"materials for their advanced industry. After centuries of peace and " +
				"prosperity, the civilisation was torn apart by war, and lapsed into a long " +
				"dark age, abandoning their industry and machines. Nobody, however, " +
				"thought to tell the mine robots to stop working, and through countless " +
				"aeons they had steadly accumulated a huge stockpile of valuable metals " +
				"and minerals, and Miner Willy realises that he now has the opportunity to " +
				"make his fortune by finding the underground store. Can YOU take the " +
				"challenge and guide Willy through the underground caverns to the " +
				"surface and riches. In order to move to the next chamber, you must " +
				"collect all the flashing keys in the room while avoiding nasties like " +
				"POISONOUS PANSIES and SPIDERS and SLIME and worst of all, MANIC MINING " +
				"ROBOTS. When you have all the keys, you can enter the portal which will " +
				"now be flashing. The game ends when you have been 'got' or fallen " +
				"heavily three times.";
		
		int _nextChar = _maxChar;

		public GamePianoLayer() : base("title", true)
        {
        }

		float GetXPos(CCNode node)
		{
			return node.BoundingBox.LowerLeft.X;
		}

		void Scroll(Helper.Label scroller)
		{
			if (GetXPos(scroller) > 0)
			{
				scroller.PositionX--;
				return ;
			}
			else
			{
				CCSprite	letter = null;
				float		maxX = -1, minX = 320;

				foreach(var node in scroller.Children)
				{
					node.PositionX--;
					var x = GetXPos(node);
					if (x > maxX)
						maxX = node.PositionX;
					if (x < minX)
					{
						minX = x ;
						letter = (CCSprite)node;
					}
				}

				if (minX < -_charWidth)
				{
					if (_nextChar < _scrollText.Length)
					{
						letter.PositionX = maxX + _charWidth;
						scroller.UpdateLetter(letter, _scrollText[_nextChar++]);
					}
					else
					{
						scroller.RemoveChild(letter);
						if (scroller.ChildrenCount == 0)
						{
							GotoDemoScene(null);
						}
					}
				}
			}
		}
			
        protected override void AddedToScene ()
        {
            base.AddedToScene ();

            this.AddSprite("background-title",  0,  0);
//			this.AddSprite("bgfill1", 	152, 40);
//			this.AddSprite("bgfill2", 	176, 40);
//			this.AddSprite("sun", 		 60, 32);
			this.AddSprite("piano", 	  0, 64);

			this.DrawAirBackground();

			//this.Print(-1, 16*8, "©2014 Dominique Normand" ,0);

			int y = ManicMinerApplicationDelegate.NoAds ? 168 : 92;

			var credits = this.PrintButton(/*48*/ 20, y, "Credits" , 0);
			var start 	= this.PrintButton(/*48*/ -1, y, "  Play  ", 2);
			var demo 	= this.PrintButton(/*48*/175, y, "  Demo  ", 0);

			this.AddTouchListener(start, 	GotoGameScene);
			this.AddTouchListener(credits, 	GotoCreditsScene);
			this.AddTouchListener(demo, 	GotoDemoScene);

			start = start.Children.First();

			var scroller = this.Print(256, 152, _scrollText.Substring(0, _maxChar), 6);

			var miner = new CCSprite(Helper.GetImage16(0));
			miner.IsAntialiased = false;
			this.AddSprite(miner, 232, 72);

			int index = 0;
			//int offset= 1;

			ScheduleOnce( tt => Schedule( t => Scroll(scroller), 0.01f), 10f); // 10 seconds

			Schedule( t => miner.AnimateWilly(238, ref index), Game.WillyAnimationSpeed);

			/*
			int opacityOffset = -5;
			int opacity = 255;

			Schedule( t => 
			{
				opacity += opacityOffset;

				if (opacity < 0)
				{
					opacity = 0;
					opacityOffset = -opacityOffset;
				}
				else if (opacity > 255)
				{
					opacity = 255;
					opacityOffset = -opacityOffset;
				}

				foreach(CCNode node in start.Children)
					node.Opacity = (byte)opacity;
				start.Opacity = (byte)opacity;
			}, Game.LevelDoneSpeed);
			*/
        }

		void GotoCreditsScene(CCEvent evt)
		{
			Window.DefaultDirector.SwitchScene(GameCreditsLayer.CreateScene(Window));
		}

		void GotoDemoScene(CCEvent evt)
		{
			Window.DefaultDirector.SwitchScene(GameDemoLayer.CreateScene(Window));
		}

		void GotoGameScene(CCEvent evt)
		{
			Window.DefaultDirector.SwitchScene(GameLayer.CreateScene(Window));
		}

		public static CCScene CreateScene(CCWindow mainWindow)
        {
			//CCSize winSize = new CCSize(_windowWidth, 240 - 56);

			return InnerCreateScene(mainWindow, new GamePianoLayer());
        }
    }
}