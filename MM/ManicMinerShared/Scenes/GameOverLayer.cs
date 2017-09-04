using System;
using System.Collections.Generic;
using CocosSharp;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;
using CocosDenshion;

namespace ManicMiner
{
    public class GameOverLayer : BaseGameLayer
    {
    	const string	_gameOverText = "Game   Over";

    	CCSprite 		_boot ;
		CCDrawNode	 	_leg ;
		Helper.Label	_gameOver ;
		int				_overInk = 0 ;
		int				_timeout = 100;

		public GameOverLayer() : base(null, true)
        {
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

			int BOOTy = -66;

			this.AddSprite(Helper.GetImage16(460), 120, 112);
			this.AddSprite(Helper.GetImage16(  3), 118,  96);

			_leg = new CCDrawNode();
			_leg.AnchorPoint = CCPoint.AnchorMiddle;
			_leg.ContentSize = new CCSize(8, 96);
			this.AddSprite(_leg, 122, BOOTy - 96);

			_leg.Color = new CCColor3B(0x67, 0x07, 0x07);
			_leg.Clear();

			_leg.DrawSegment(new CCPoint(7, 0), new CCPoint(7, 95), 1, new CCColor4F(new CCColor3B(0xBB, 0x23, 0x23)));
			_leg.DrawSegment(new CCPoint(6, 0), new CCPoint(6, 95), 1, new CCColor4F(new CCColor3B(0xC7, 0x37, 0x37)));
			_leg.DrawSegment(new CCPoint(5, 0), new CCPoint(5, 95), 1, new CCColor4F(new CCColor3B(0xD3, 0x4F, 0x4F)));
			_leg.DrawSegment(new CCPoint(4, 0), new CCPoint(4, 95), 1, new CCColor4F(new CCColor3B(0xC7, 0x37, 0x37)));
			_leg.DrawSegment(new CCPoint(3, 0), new CCPoint(3, 95), 1, new CCColor4F(new CCColor3B(0xBB, 0x23, 0x23)));
			_leg.DrawSegment(new CCPoint(2, 0), new CCPoint(2, 95), 1, new CCColor4F(new CCColor3B(0x8F, 0x13, 0x13)));
			_leg.DrawSegment(new CCPoint(1, 0), new CCPoint(1, 95), 1, new CCColor4F(new CCColor3B(0x7B, 0x0B, 0x0B)));
			_leg.DrawSegment(new CCPoint(0, 0), new CCPoint(0, 95), 1, new CCColor4F(new CCColor3B(0x67, 0x07, 0x07)));
			_leg.DrawSegment(new CCPoint(-1,0), new CCPoint(-1,95), 1, new CCColor4F(CCColor3B.Black)); 

			_boot = (CCSprite) this.AddSprite(Helper.GetImage16(461), 120, BOOTy);

			this.DrawAirBackground();
			this.Print(0, 16*8, Game.cTITLE, 0);

			_gameOver = this.Print(82, 48, _gameOverText, 0);

			this.Schedule(t => DoBoot(ref BOOTy), Game.LevelDoneSpeed);
			this.Schedule(t => DoText(BOOTy), Game.EndLevelSpeed);
        }

		void DoBoot(ref int BOOTy)
		{
			if (BOOTy < 96)
			{
				BOOTy++;

				if (BOOTy == -20)
					Helper.PlayEffect("game-over");

				this.MoveSprite(_leg , 122, BOOTy - 96);
				this.MoveSprite(_boot, 120, BOOTy);
			}
		}

		void DoText(int BOOTy)
		{
			if (BOOTy >= 96)
			{
				int ink = _overInk;
				int idx = 0;

				foreach(CCSprite letter in _gameOver.Children)
				{
					var c = _gameOverText[idx++] ;
					if (c == ' ')
						continue ;
					
					_gameOver.UpdateLetter(letter, c, ink+1);
					ink = (ink + 1) % 6;
				}

				_overInk = (_overInk + 1) % 6;

				_timeout--;
				if (_timeout < 0)
					GotoPianoScene();
			}
		}

		public static CCScene CreateScene(CCWindow mainWindow)
        {
//			CCSize winSize = new CCSize(256, 192); // 256,192
	        return InnerCreateScene(mainWindow, new GameOverLayer());
        }
    }
}