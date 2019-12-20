using System;

namespace ManicMiner
{
	partial class Game
	{		
		public const int 	LevelCount = 20;

		public static int	HiScore	
		{ 
			get; 
			set; 
		}

		private static int _score = 0;

		public static int 		Score
		{ 
			get { return _score; }
			set
			{
				_score = value ;
				if (value > HiScore)
					HiScore = value; 
			}
		}

		public static int		LIVES;
		public static int		LIVESidx;

		public static int		ROOM;		
		public static int		GAMEmode;

		public static int		HOLEl;
		public static int		HOLEy;
		
		public static int[]		SPGx= new int[65];
		public static int[]		SPGy= new int[65];

		public static int		EXTRA;
		public static int		EXTRAdelta;

		public static bool		PAUSE;
		public static int		PAUSEh;

		public static bool		cCRUMBModified;
		public static bool 		cSWITCHsModified;

		///////////////////////////////////////
		//	Level Variables
		///////////////////////////////////////

		public static int[,,]	ROOMS = new int[22,17,33];
		public static string[]	TITLES = new string[22];
		public static int[]		BORDERS = new int[22];
		public static int[]		AIR = new int[22];

		public static int[]		WILLYsx = new int[22];
		public static int[]		WILLYsy = new int[22];
		public static bool[]	WILLYsd = new bool[22];

		public static ConveyorBelt[]	CONVEYORS = new ConveyorBelt[20];
		public static Key[,]			KEYS = new Key[20,5];
		public static Switch[,]			SWITCHES = new Switch[20,2];
//		public static int[,]	SWITCHx = new int[22,3];
//		public static int[,]	SWITCHy = new int[22,3];
//		public static int[,]	SWITCHs = new int[22,3];

		public static int[]		EXITx = new int[22];
		public static int[]		EXITy = new int[22];

		public static HRobot[,]	HROBO = new HRobot[20,4];
		public static VRobot[,]	VROBO = new VRobot[20,4];

		public static int[,]	SKYpx = new int[4,5];
		public static int[,]	SKYpy = new int[4,5];

		///////////////////////////////////////
		//	Current Room
		///////////////////////////////////////

		public static int		BLOCKoff;
		public static int[,]	cROOM = new int[17,33];
		public static int[,]	cCRUMB = new int[17,33];
		public static string	cTITLE;
		public static int		cBORDER;
		public static int		cAIR;
		public static int		cAIRp;

		public static int		cSWITCH1m;
		public static int		cSWITCH2m;

		public static Exit			_exit;
		public static ConveyorBelt	_conveyorBelt;
		public static Willy			_willy;		
		public static Eugene		_eugene;
		public static Kong			_kong;

		public static Switch[]		_switches= new Switch[3];
		public static Key[]			_keys	 = new Key[5];
		public static HRobot[]		_hRobots = new HRobot[4];
		public static VRobot[]		_vRobots = new VRobot[4];
		public static Skylab[]		_skylabs = new Skylab[3];
	}
}

