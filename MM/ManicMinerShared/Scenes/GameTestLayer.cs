using System;
using System.Collections.Generic;
using CocosSharp;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;
using CocosDenshion;
using System.IO;

namespace ManicMiner
{
    public class GameTestLayer : CCLayerColor
    {
    	int robot = 0;
    	int index = 16;

		public GameTestLayer() : base ()
        {
			this.AddTouchListener((touches, ccevent) => 
			{
				robot += 1;
				index += 8;
				if (robot == 17)
				{
					index -= 4;
				}
				//GotoPianoScene();
			});

			Color = CCColor3B.Black;
			Opacity = 255;
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

			var map = new CCTMXTiledMap("room-1.tmx");
			this.AddChild(map);
			map.PositionY = 192;

			// this.CreateBackground(5);
           // var index 	= 16;
            var anim  		= 0;
            var x 			= 50;
            var direction	= 0;

			Schedule(t => 
			{
				this.DrawRobot(100, x, 50, index + (direction * 4) + anim);
				if (direction == 0)
				{
					anim = anim+1 ;
					if (anim == 4)
					{
						anim = 0;
						x += 8;	
					}
									
					if (x > 100)
					{
						x = 100;
						direction = 1;
					}
				}
				else
				{
					anim = anim-1;
					if (anim < 0)
					{
						x -= 8;					
						anim = 3;
					}
					if (x <= 50)
					{
						x = 50;
						direction = 0;
					}
				}
			},
			Game.RobotAnimationSpeed);
        }

		void MoveSprite(float dt)
		{
		}

		void GotoPianoScene()
		{
			this.UnscheduleAll();
			Helper.StopMusic();
			Window.DefaultDirector.SwitchScene(GamePianoLayer.CreateScene(Window));
		}

		public static CCScene CreateScene(CCWindow mainWindow)
        {
			CCSize winSize = new CCSize(256, 192); // 256,192
            mainWindow.SetDesignResolutionSize(winSize.Width, winSize.Height, CCSceneResolutionPolicy.ExactFit);

            var scene = new CCScene (mainWindow);
			var layer = new GameTestLayer();

            scene.AddChild (layer);

            return scene;
        }
    }
}