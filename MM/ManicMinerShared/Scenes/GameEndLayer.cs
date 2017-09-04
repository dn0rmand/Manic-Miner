using System;
using System.Collections.Generic;
using CocosSharp;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;
using CocosDenshion;

namespace ManicMiner
{
    public class GameEndLayer : BaseGameLayer
    {
		string[] _endText = new string[] 
		{
			"                                ",
			"        Congratulations!        ",
			"                                ",

			" Prospecting down Surbiton way  ",
			"    was not such a bad idea     ",
			"          after all.            ",

			"   All that hard work really    ",
			"           paid off.            ",
			"                                ",

			"Willy now has so much money that",
			" he has no idea what to do with ",
			"            it all.             ",

			"                                ",
			"      Have you any ideas ?      ",
			"                                ",

			"                                ",
			"           Manic Miner          ",
			"                                ",

			"  Based on an original game by  ",
			"         Matthew Smith          ",
			"                                ",

			" Converted by Dominique Normand ",
			"                                ",
			" Based on Andy Noble's version  ",

//			"                                ",
//			"     2014 Dominique Normand     ",
//			"                                ",
		};

		public GameEndLayer() : base("done", true)
        {
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();
       
			CCSprite miner = new CCSprite(Helper.GetImage16(462));
			miner.IsAntialiased = false;
			this.AddSprite("end", 176, 8);
			this.AddSprite(miner, 184,16);

			Helper.Label[] labels = new Helper.Label[]
			{
				this.Print(0, 88, _endText[0], 7),
				this.Print(0, 96, _endText[1], 7),
				this.Print(0,104, _endText[2], 7)
			};

			foreach(var label in labels)
			foreach(var letter in label.Children)
				letter.Opacity = 0;

			int frame = 0;

			Schedule( t => 
			{
				miner.SpriteFrame = Helper.GetImage16(462+frame);

				frame = (frame + 1) & 15;
			}, Game.WillyAnimationSpeed);	
				
			int	index = 0;
			int ink	  = 0;
			bool up	  = true;
			long delay= 10;

			Schedule( t =>
			{
				if (delay > 0)
				{
					delay--;
				}
				else if (up)
				{
					ink += 16;

					var ik = (byte) (ink > 255 ? 255 : ink);

					foreach(var label in labels)
					foreach(var letter in label.Children)
						letter.Opacity = ik;

					if (ink > 255)
					{
						ink   = 256;
						delay = 100; // Longer Delay to give time to read ~ 5 seconds
						up	  = false;
					}
				}
				else
				{
					ink -= 16;
					if (ink < 0)
						ink = 0;

					if (ink == 0)
					{
						delay = 10; // Tiny delay ~ 0.5 seconds
						up 	  = true;
						// Update text here
						index = (index + 3) % _endText.Length;

						for(int i = 0 ; i < 3 ; i++)
						{
							var label = labels[i];
							var text  = _endText[index+i];
							int idx   = 0;

							foreach(CCSprite letter in label.Children)
							{
								letter.Opacity = 0;
								label.UpdateLetter(letter, text[idx++]);
							}
						}
					}
					else
					{
						var ik = (byte) ink;

						foreach(var label in labels)
							foreach(var letter in label.Children)
								letter.Opacity = ik;
					}
				}
			}, Game.EndLevelSpeed);

			this.AddTouchListener((touches, ccevent) => GotoPianoScene());
        }
        
		public static CCScene CreateScene(CCWindow mainWindow)
        {
        	return InnerCreateScene(mainWindow, new GameEndLayer());
			// CCSize winSize = new CCSize(256, 192); // 256,192
        }
    }
}