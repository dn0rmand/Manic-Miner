using System;
using System.Collections.Generic;
using CocosSharp;

namespace ManicMiner
{
	public class GameCreditsLayer : BaseGameLayer
    {
		public GameCreditsLayer() : base("options", true)
        {
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

			this.Print(-1,  8, "`bM`f `dN`e `cC `e `cI`b `fE`d ", 2);
			this.Print(-1, 11, "`b `fA`d `eI`c  `eM`c `bN`f `dR", 2);

			this.SmallPrint(-1,  6, "Based on an original game by Matthew Smith", 7);

			this.SmallPrint(-1,  7, "@ 1983 BUG-BYTE Ltd and Software Projects Ltd", 7);
			this.SmallPrint(-1,  8, "@ 1997 Alchemist Research", 7);

			this.SmallPrint(-1, 11, "Converted to iPhone/iPad by Dominique Normand", 3);
			this.SmallPrint(-1, 12, "from Andy Noble's version", 3);

			this.SmallPrint(-1, 15, "Graphics`e.............................`fAndy Noble", 4);
			this.SmallPrint(-1, 16, "Music Arranged`e....................`fMatt Simmonds", 4);
			this.SmallPrint(-1, 17, "BlitzBasic Programming`e...............`fAndy Noble", 4);
			this.SmallPrint(-1, 18, "iPhone/iPad Programming`e.......`fDominique Normand", 4);

			this.SmallPrint(-1, 22, "Tap `eAnywhere`g to return to the Title",7);
			//this.SmallPrint(-1, 25, "@ 2014 Dominique Normand", 3);

			this.AddTouchListener((touches, ccevent) => GotoPianoScene());
        }

		public static CCScene CreateScene(CCWindow mainWindow)
        {
        	return InnerCreateScene(mainWindow, new GameCreditsLayer());
        }
    }
}