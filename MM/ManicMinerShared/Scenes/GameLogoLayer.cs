using System;
using System.Collections.Generic;
using CocosSharp;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;
using CocosDenshion;

namespace ManicMiner
{
    public class GameLogoLayer : BaseGameLayer
    {
		float		_LOGOy;
		float		_LOGOacc;
		float		_LOGOthrust;
		CCSprite	_logo ;

		public GameLogoLayer() : base(null, false)
        {
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

			_LOGOy   	= 120+240;
			_LOGOthrust = 0;
			_LOGOacc 	= 0.5f;

            _logo = new CCSprite("logo.png");
			var frame = _logo.SpriteFrame;

			_logo.Position = new CCPoint(160, _LOGOy);

			AddChild(_logo);

			Schedule( t => { MoveLogo(t); }, 0.01f);
			this.AddTouchListener((touches, ccevent) => GotoPianoScene());
        }

		void MoveLogo(float dt)
		{
			_logo.PositionY = _LOGOy;

			if (_LOGOthrust < 16)
				_LOGOthrust = _LOGOthrust + _LOGOacc;

			_LOGOy -= _LOGOthrust;

			if (_LOGOy <= 120)
			{
				_LOGOy 		= 120;
				_LOGOthrust = - _LOGOthrust;
				_LOGOacc 	= _LOGOacc + 0.1f ;

				if (_LOGOacc > 3)
				{
					UnscheduleAll();
					ScheduleOnce( t => { GotoPianoScene(); }, 2f);
				}
			}
		}

		public static CCScene CreateScene(CCWindow mainWindow)
        {
			CCSize winSize = new CCSize(320, 240);
            mainWindow.SetDesignResolutionSize(winSize.Width, winSize.Height, CCSceneResolutionPolicy.ExactFit);

            var scene = new CCScene (mainWindow);
			var layer = new GameLogoLayer();

            scene.AddChild (layer);

            return scene;
        }
    }
}