using System;

namespace ManicMiner
{
	partial class Game
	{
		public static int MakeBlockTag(int x, int y)
		{
			return y * 32 + x; // Up to 511
		}

		public static int MakeKeyTag(int key)
		{
			return 512 + key; // Up to 517
		}

		public static int MakeConveyorTag(int conv)
		{
			return 520 + conv; // Up to 546
		}

		public static int MakeHRobotTag(int robo)
		{
			return 550 + robo; // Up to 554
		}

		public static int MakeVRobotTag(int robo)
		{
			return 555 + robo; // Up to 559
		}

		public static int MakeLevelFourteenTag(int i)
		{
			return 560 + i; // Up to 563
		}

		public static int MakeLiveTag(int live)
		{
			return 570 + live; // Up to 578
		}

		public static int MakeSwitchTag(int s)
		{
			return 590 + s; // Up to 592
		}

		public static int	AirTag			{ get { return 580;  } }
		public static int	ExitTag 		{ get { return 1000; } }
		public static int 	EugeneTag   	{ get { return 1001; } }
		public static int	KongTag			{ get { return 1002; } }
		public static int	ScoreTag		{ get { return 1003; } }
		public static int	HighScoreTag	{ get { return 1004; } }
		public static int 	WillyTag 		{ get { return 2000; } }

		public static int MakeSPGTag(int spg)
		{
			return 3000 + spg;
		}
	}
}

