using System;
using CocosSharp;
using System.Diagnostics;

namespace ManicMiner
{
	partial class Game
	{		
		static Game()
		{
			SetupData();
		}

		public static float WillyAnimationSpeed { get { return 0.2f; } }
		public static float KeyAnimationSpeed 	{ get { return 0.02f;} }
		public static float ConvAnimationSpeed	{ get { return 0.1f; } }
		public static float RobotAnimationSpeed	{ get { return 0.1f; } }
		public static float ExitAnimationSpeed	{ get { return 0.4f; } }
		public static float AirSpeed			{ get { return 0.1f; } }
		public static float LevelDoneSpeed 		{ get { return 0.01f;} }
		public static float EndLevelSpeed		{ get { return 0.05f;} }

		///////////////////////////////////////
		//	Set Up Data
		///////////////////////////////////////
		public static void SetupData()
		{
			try
			{
				int roomIndex 	= 0;
				int keyIndex  	= 0;
				int switchIndex = 0;
				int robotIndex  = 0;
				int skyIndex 	= 0;

				for (int lev = 1 ; lev <= LevelCount ; lev++)
				{
					//	Setup Level Data
			
					for (int y = 1 ; y <= 16 ; y++)
					{
						for	(int x = 1 ; x <= 32 ; x++)
						{
							ROOMS[lev, y, x] = LEVELS[roomIndex++];
						}
					}

					//	Setup Level Name
			
					var idx = lev - 1;

					TITLES[lev]  = LEVnames[idx];
					BORDERS[lev] = LEVbord[idx];
					WILLYsx[lev] = WILLYstartx[idx];
					WILLYsy[lev] = WILLYstarty[idx];
					WILLYsd[lev] = WILLYstartd[idx]!=0;

					CONVEYORS[idx].Init(CONVxpos[idx], CONVypos[idx], CONVdir[idx]!=0, CONVlen[idx]);

					EXITx[lev] 	 = EXITxpos[idx];
					EXITy[lev] 	 = EXITypos[idx];
					AIR[lev]	 = AIRcount[idx];

					//	Setup Key Positions

					for (int x = 0; x < 5; x++, keyIndex++)
						KEYS[lev-1,x].Init(x, KEYxpos[keyIndex], KEYypos[keyIndex], KEYstat[keyIndex] == 0);

					//	Setup Switch Positions

					for (int x = 0; x < 2; x++, switchIndex++)
						SWITCHES[lev-1, x].Init(x, SWITCHxpos[switchIndex], SWITCHypos[switchIndex], SWITCHstat[switchIndex]);

					//	Setup Robots 

					HRobot hRobot = new HRobot();
					VRobot vRobot = new VRobot();

					for (int x = 0; x < 4; x++, robotIndex++)
					{
						hRobot.X 	 	= HROBOxpos[robotIndex];
						hRobot.Y	 	= HROBOypos[robotIndex];
						hRobot.Min 		= HROBOminpos[robotIndex];  
						hRobot.Max 		= HROBOmaxpos[robotIndex];  
						hRobot.Direction= HROBOdir[robotIndex]!=0 ? Direction.Left : Direction.Right;
						hRobot.Speed	= HROBOspeed[robotIndex];
						hRobot.Gfx		= HROBOgra[robotIndex];
						hRobot.Flip		= HROBOfli[robotIndex];
						hRobot.Frame	= HROBOani[robotIndex];

						vRobot.X 	 	= VROBOxpos[robotIndex];
						vRobot.Y	 	= VROBOypos[robotIndex];
						vRobot.Min 		= VROBOminpos[robotIndex];  
						vRobot.Max 		= VROBOmaxpos[robotIndex];  
						vRobot.GoingUp	= VROBOdir[robotIndex]!=0;
						vRobot.Speed	= VROBOspeed[robotIndex];
						vRobot.Gfx		= VROBOgra[robotIndex];
						vRobot.Frame	= VROBOani[robotIndex];

						HROBO[lev-1, x] = hRobot;
						VROBO[lev-1, x] = vRobot;
					}
				
					if (lev <= 3)
					{
						//	Setup SKY

						for (int x = 1; x <= 4; x++, skyIndex++)
						{
							SKYpx[lev,x]=SKYxpos[skyIndex];
							SKYpy[lev,x]=SKYypos[skyIndex];
						}
					}
				}	
			}
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
				throw e;
			}

			return ;
		}

		///////////////////////////////////////
		//	Copy Room into Current Room
		///////////////////////////////////////
		public static void CopyRoom(bool demo)
		{
			cCRUMBModified = true ;

			for (int y = 1 ; y <= 16 ; y++)
			{
				for (int x=1 ; x <= 32 ; x++)
				{
					cROOM[y,x]=ROOMS[ROOM,y,x];
					if (cROOM[y,x]==4)
						cCRUMB[y,x]=8;
					else
						cCRUMB[y,x]=0;
				}
			}

			for (int x = 0 ; x < 5 ; x++)
			{
				_keys[x] = KEYS[ROOM-1, x];
			}
			
			for (int x=0 ; x < 2 ; x++)
			{
				_switches[x] = SWITCHES[ROOM-1, x];
			}

			_conveyorBelt = CONVEYORS[ROOM-1];

			cTITLE	= TITLES[ROOM];
			cBORDER	= BORDERS[ROOM];
			cAIR	= AIR[ROOM]+31;
			cAIRp	= 8;
			cSWITCHsModified = true;

			_exit.Init(EXITx[ROOM], EXITy[ROOM], ExitTag, demo) ;

			for (int x = 0 ; x < 4 ; x++)
			{
				_hRobots[x] = HROBO[ROOM-1, x];
				_hRobots[x].Tag	= MakeHRobotTag(x);

				_vRobots[x] = VROBO[ROOM-1, x];
				_vRobots[x].Tag = MakeVRobotTag(x);
			}

			_eugene.Reset(EugeneTag);
			_kong.Reset(KongTag);

			HOLEl=2;
			HOLEy=95;
			
			cSWITCH1m=0;
			cSWITCH2m=0;

			for (int x = 1 ; x <= 3 ; x++)
			{
				var pos = Skylab.GetPos(x);
				_skylabs[x-1].Init(x, SKYpx[x, pos], SKYpy[x, pos], MakeLevelFourteenTag(x));
			}
				
			_willy.X = WILLYsx[ROOM];
			_willy.Y = WILLYsy[ROOM];
			_willy.Direction = WILLYsd[ROOM] ? Direction.Left : Direction.Right;
			_willy.Mode = 0;
			_willy.FallHeight = 0;	
			_willy.Jump = 0;
		}

		///////////////////////////////////////
		//	Build Map
		///////////////////////////////////////
		public static void CreateBackground(this CCLayerColor layer ,int room)
		{
			CCColor4B color ;

			switch (room)
			{
				case 2:
					color = new CCColor4B(7, 7, 91);
					break;
				case 5:
					color = new CCColor4B(139, 0, 0);
					break;
				case 14:
					color = new CCColor4B(7, 7, 91);
					break;
				case 19:
					color = new CCColor4B(0, 79, 0);
					break;

				default:
					return;
			}

			var size = new CCSize(32*8, 16*8);
			var back = new CCDrawNode();
			back.ContentSize = size ;
			back.AnchorPoint = CCPoint.AnchorMiddle;
			back.DrawRect(new CCRect(0, 0, size.Width, size.Height), color, 0, color);
			layer.AddSprite(back, 0, 0);
		}

		public static void BuildRoom(this CCLayerColor layer)
		{
			layer.CreateBackground(ROOM);

			for (int y=0 ; y <= 15 ; y++)
			{
				for (int x=0 ; x <= 31; x++)
				{
					var dat=cROOM[y+1,x+1];
					CCSpriteFrame block;

					if (dat==4)
						dat = 0;
					
					if (dat != 0)
					{
						block = Helper.GetBlock(BLOCKoff+dat);
					
						var node = layer.AddSprite(block, x*8, y*8);
						node.Tag = MakeBlockTag(x, y);
					}
				}
			}
			if (ROOM == 20)
			{
				layer.AddSprite("background", 0,  0);
				// layer.AddSprite("sun", 		 60, 32);
			}
		}

		public static void RemoveRoomBlock(this CCLayerColor layer, int x, int y)
		{
			cROOM[y,x] = 0;
			layer.RemoveChildByTag(MakeBlockTag(x-1, y-1));
		}

		static void ReplaceNode(this CCLayerColor layer, CCNode oldNode, CCNode newNode)
		{
			newNode.Position 	 = oldNode.Position ;
			newNode.Tag		 	 = oldNode.Tag ;
			newNode.GlobalZOrder = oldNode.GlobalZOrder;
			newNode.AnchorPoint  = oldNode.AnchorPoint;
			newNode.IgnoreAnchorPointForPosition = oldNode.IgnoreAnchorPointForPosition;

			layer.RemoveChild(oldNode);
			layer.AddChild(newNode);
		}
					 
		///////////////////////////////////////
		//	Draw Crumbling Blocks
		///////////////////////////////////////
		public static void DrawCrumb(this CCLayerColor layer)
		{
			if (! cCRUMBModified)
				return;

			cCRUMBModified = false;

			for (int y=0 ; y<=15; y++)
			{
				for (int x=0 ; x<=31; x++)
				{
					var dat=cCRUMB[y+1,x+1];
					if (dat!=0)
					{
						var tag	 = MakeBlockTag(x, y);
						var node = layer.GetChildByTag(tag) as CCSprite;

						if (node == null)
						{
							CCSpriteFrame block;

							if (dat < 8)
								block =  Helper.GetBlock(BLOCKoff+4, 8-dat);
							else
								block = Helper.GetBlock(BLOCKoff+4);

							node  = (CCSprite) layer.AddSprite(block, x*8, y*8);
							node.Tag = tag;
						}
						else if (dat < 8)
							node.SpriteFrame = Helper.GetBlock(BLOCKoff+4, 8-dat);
					}
				}
			}
		}

		///////////////////////////////////////
		//	Draw keys
		///////////////////////////////////////
		public static void DrawKeys(this CCLayerColor layer)
		{
			int count=0;
			for (int i = 0 ; i < 5 ; i++)
			{
				if (_keys[i].Draw(layer))
					count++;
			}

			if (count == 0 || ManicMinerApplicationDelegate.UnlockDoors)
				_exit.Activated = true;
		}

		///////////////////////////////////////
		//	Draw Switches
		///////////////////////////////////////
		public static void DrawSwitches(this CCLayerColor layer)
		{
			if (Game.cSWITCHsModified)
			{
				Game.cSWITCHsModified = false;

				for (int i = 0 ; i < 2 ; i++)
					_switches[i].Draw(layer);
			}
		}

		///////////////////////////////////////
		//	Draw Rolling Carpets
		///////////////////////////////////////
		public static void DrawConv(this CCLayerColor layer)
		{
			_conveyorBelt.Draw(layer, BLOCKoff);
		}

		///////////////////////////////////////
		//	Add or Move Robot to position x, y
		///////////////////////////////////////
		public static void DrawRobot(this CCLayer layer, int tag, int x, int y, int index)
		{
			var robot = Helper.GetImage16(index);

			CCSprite sprite = (CCSprite) layer.GetChildByTag(tag);
			if (sprite == null)
			{
				layer.AddSprite(robot, x, y).Tag = tag;
			}
			else
			{
				sprite.SpriteFrame = robot;
				layer.MoveSprite(sprite, x, y);
			}
		}

		///////////////////////////////////////
		//	Do Horizontal & Vertical Robots
		///////////////////////////////////////
		public static void DoRobots(this CCLayerColor layer)
        {
			for	(int i = 0 ; i < 4 ; i++)
			{
				_hRobots[i].Move(layer);
				_vRobots[i].Move(layer);
			}
		}

		///////////////////////////////////////
		//	Draw Exit Door
		///////////////////////////////////////
		public static void DrawExit(this CCLayerColor layer)
		{
			_exit.Draw(layer, ROOM);
		}

		///////////////////////////////////////
		//	Do Special Robot
		///////////////////////////////////////
		public static void DoSpecialRobot(this CCLayerColor layer)
		{
			switch (ROOM)
			{
				case 5:
					_eugene.Move(layer, _exit.Activated);
					break;
				case 8:
					layer.DoLevelEight();
					break;

				case 12:
					layer.DoLevelEight();
					break;

				case 14:
					layer.DoLevelFourteen();
					break;

				case 19:
					layer.DoSPG();
					layer.CheckSPG();
					break;
			}
		}

		///////////////////////////////////////
		//	Do Level 8
		///////////////////////////////////////
		static void DoLevelEight(this CCLayerColor layer)
		{
			_kong.Animate();

			switch (_switches[1].Image)
			{
				case 1:
					_kong.Move(layer);
					break;
				case 2:
					_kong.Fall(layer);
					break;
			}

			if (_switches[0].Image == 2)
				layer.DoHole();
		}

		///////////////////////////////////////
		//	Do Hole
		///////////////////////////////////////
		static void DoHole(this CCLayerColor layer)
		{
			switch (cSWITCH1m)
			{
				case 0:
					HOLEy=HOLEy-1;
					HOLEl=HOLEl+2;
					if (HOLEy==87)
					{
						cSWITCH1m=1;
						layer.RemoveRoomBlock(18, 12);
						layer.RemoveRoomBlock(18, 13);
						_hRobots[1].Max = _hRobots[1].Max + 24;
					}
					break;
			}
		}

		///////////////////////////////////////
		//	Do Level 14
		///////////////////////////////////////
		static void DoLevelFourteen(this CCLayerColor layer)
		{			
			for (int i = 0 ; i < 3 ; i++)
				_skylabs[i].Move(layer);
		}

		/// <summary>
		/// Draws the air background.
		/// </summary>
		public static void DrawAirBackground(this CCLayerColor layer)
		{
			layer.AddSprite(Helper.GetTitleAir(0), 0, 128);
			layer.AddSprite(Helper.GetTitleAir(1), 0, 136);
		}

		///////////////////////////////////////
		//	Draw Air
		///////////////////////////////////////
		public static void DrawAir(this CCLayerColor layer, bool levelDone)
		{
			var tag = AirTag;

			if (Game.GAMEmode == 2) // Doing the Kill so pause the air.
			{
				layer.RemoveChildByTag(tag); // Remove because transparency doesn't apply
				return;
			}

			int width = cAIR - 32;
			var node = (CCDrawNode) layer.GetChildByTag(tag);

			if (node == null)
			{
				if (width > 0)
				{
					node = new CCDrawNode();
					node.Tag = tag;
					node.AnchorPoint = CCPoint.Zero;
					node.ContentSize = new CCSize(width, 5);
					layer.AddSprite(node, 32, 142);
				}
			}
			else if (width <= 0)
			{
				layer.RemoveChild(node);
				node = null;
			}

			if (node != null)
			{
				node.Clear();
				node.DrawSegment(new CCPoint(0, 4), new CCPoint(width, 4), 1, new CCColor4F(new CCColor3B(110, 110, 110)));
				node.DrawSegment(new CCPoint(0, 3), new CCPoint(width, 3), 1, new CCColor4F(new CCColor3B(160, 160, 160)));
				node.DrawSegment(new CCPoint(0, 2), new CCPoint(width, 2), 1, new CCColor4F(new CCColor3B(210, 210, 210)));
				node.DrawSegment(new CCPoint(0, 1), new CCPoint(width, 1), 1, new CCColor4F(new CCColor3B(160, 160, 160)));
				node.DrawSegment(new CCPoint(0, 0), new CCPoint(width, 0), 1, new CCColor4F(new CCColor3B(110, 110, 110)));
			}

			if (! ManicMinerApplicationDelegate.Invincible)
			{
				cAIRp--;
				if (cAIRp <= 0)
				{
					cAIRp = 8;
					cAIR--;

					if (cAIR<=32 && ! levelDone)
						_willy.Mode = WillyMode.Death;
				}
			}
		}

		///////////////////////////////////////
		//	Draw Lives
		///////////////////////////////////////
		public static void DrawLives(this CCLayerColor layer, ref int oldValue, int index, bool demo = false)
        {
        	if (oldValue == LIVES)
        		return ;

            int count = oldValue = LIVES;

			if (count>8)
				count=8;
			
			int y = demo ? 148 : 168;
			for (int x=1 ; x <= 8 ; x++)
			{
				var tag = MakeLiveTag(x-1);
				if (x >= count)
				{
					layer.RemoveChildByTag(tag);
					continue;
				}
				var node = layer.GetChildByTag(tag);
				if (node == null)
					layer.CreateWillyLive(tag, (x-1)*16 + 10, y, index);
			}
		}

		static int[] _blocks = new int[] 
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,

//			15,
//			14,
//			13,
//			12,
//			11,
//			10,
//			9,
//			8
		};
		static int[] _xoffsets = new int[] 
		{
			 0,
			-2,
			-4,
			-6,
			 0,
			-2,
			-4,
			-6,
		};

		public static void CreateWillyLive(this CCLayerColor layer, int tag, int x, int y, int index)
		{
			var xoff = _xoffsets[index];
			var block= _blocks[index];

			index = (index + 1) % _blocks.Length;

			var img = Helper.GetImage16(block);
			layer.AddSprite(img, x + xoff - 8, y).Tag = tag;
		}

		public static void AnimateWilly(this CCSprite willy, int x, ref int index)
		{
			var xoff = _xoffsets[index];
			var block= _blocks[index];

			index = (index + 1) % _blocks.Length;
			willy.SpriteFrame = Helper.GetImage16(block);
			willy.PositionX   = x + xoff;
		}

		public static void AnimateLives(this CCLayerColor layer, ref int index, bool demo)
		{
			int count = LIVES-1; // One of lives if the one playing ( not shown here )

			if (count > 8)
				count = 8;

			// Update Sprites

			var idx = index;

			for (int x=1 ; x <= count; x++)
			{
				var meIdx = idx ;
				var tag   = MakeLiveTag(x-1);
				var miner = layer.GetChildByTag(tag) as CCSprite ;
				if (miner != null)
					miner.AnimateWilly((x-1)*16 + 10, ref meIdx);

				index = meIdx;
			}
		}

		public static void InitGame(int room)
		{
			if (room == 0)
			{
				Score		= 0;			
				LIVES		= 3;
				LIVESidx 	= 0;
				EXTRA		= 10000;
				EXTRAdelta	= 10000;
			}
			ROOM 		= room == 0 ? 1 : room;
			BLOCKoff	= (ROOM-1)*16;
			GAMEmode	= 1;
			PAUSE		= false;
			PAUSEh		= 0;
			LeftOn 		= false;
			RightOn		= false;
			JumpOn		= false;
		}

		///////////////////////////////////////
		//	Print Score
		///////////////////////////////////////
		public static void PrintScore(this CCLayerColor layer, ref int oldValue)
		{
			var score = Score;
			if (score > 999999)
				score = 999999;

			if (oldValue == score)
				return ;
			oldValue = score;

			var poo = score.ToString("D6");

			var text = layer.GetChildByTag(ScoreTag) as Helper.Label;
			if (text == null)
			{
				layer.RemoveChildByTag(ScoreTag);
				layer.Print(208,152, poo, 6).Tag = ScoreTag;
			}
			else
				text.SetText(poo);

//			if (Score > Highscore)
//				Highscore = Score;
		
			layer.CheckExtraLive();
		}

		public static void CheckExtraLive(this CCLayerColor layer)
		{
			if (Score > EXTRA)
			{
				EXTRA =EXTRA+EXTRAdelta;
				LIVES += 1;
			}
		}

		///////////////////////////////////////
		//	Print Score
		///////////////////////////////////////
		public static void PrintHiScore(this CCLayerColor layer, ref int oldValue)
		{
			var score = HiScore;
			if (score > 999999)
				score = 999999;
			
			if (score == oldValue)
				return;
			oldValue = score;
			var poo = score.ToString("D6");

			layer.RemoveChildByTag(HighScoreTag);
			layer.Print(88, 152,	poo, 6).Tag = HighScoreTag;
		}

		///////////////////////////////////////
		//	Level Done Animation
		///////////////////////////////////////
		public static void LevelDone(this CCLayerColor layer, Action callback)
		{
			layer.UnscheduleAll();

			Helper.StopMusic();
			Helper.PlayEffect("end-level");

			int oldScore = -1;
			int oldHiscore=-1;
			int oldLives  =-1;

			Action<float> action = null;

			action = t =>
			{
				cAIR--;
				if (cAIR < 0)
					cAIR = 0;

				Score += 9;
				layer.PrintScore(ref oldScore);
				layer.PrintHiScore(ref oldHiscore);
				layer.DrawLives(ref oldLives, LIVESidx);
				layer.DrawAir(true);

				if (cAIR < 32)
				{
					Helper.StopEffect();
					layer.Unschedule(action);
					ROOM++;
					if(ROOM > LevelCount)
						ROOM = 1;

					GAMEmode = 1;

					if (callback != null)
						callback();
				}
			};

			layer.Schedule(action, Game.LevelDoneSpeed);
		}

		///////////////////////////////////////
		//	Draw SPG Block
		///////////////////////////////////////
		static void SPGblock(this CCLayerColor layer, int tag, int x, int y)
		{
			var node = layer.GetChildByTag(tag);

			if (node == null)
			{
				var block = Helper.GetBlock(64);

				node = layer.AddSprite(block, x, y);
				node.Tag = tag;
				node.Opacity = 64;
			}
			else
			{
				layer.MoveSprite(node, x, y);
			}
		}

		///////////////////////////////////////
		//	Do SPG
		///////////////////////////////////////
		static void DoSPG(this CCLayerColor layer)
		{
			layer.FindSPG();
			for (int i=0 ; i<=64 ; i++)
			{
				var tag  = MakeSPGTag(i);
				if (SPGx[i]!=-1)
					layer.SPGblock(tag, SPGx[i]*8,SPGy[i]*8);
				else
					layer.RemoveChildByTag(tag);					
			}
		}

		///////////////////////////////////////
		//	Find SPG
		///////////////////////////////////////
		static void FindSPG(this CCLayerColor layer)
		{
			for (int ii=0 ; ii <= 64 ; ii++)
			{
				SPGx[ii]=-1;
				SPGy[ii]=-1;
			}

			int i=0;
			int x=23;
			int y=0;
			bool dir=false;
			bool done=false;
			
			do
			{
				int blockhit = cROOM[y+1,x+1];
				bool robohit = layer.SPGCheckRobo(x,y);
			
				if (blockhit==0 && !robohit)
				{
					SPGx[i]=x;
					SPGy[i]=y;
				}
				else if (blockhit!=0 && robohit)
				{
					SPGx[i]=-1;
					SPGy[i]=-1;
					done=true;
				}
				else if (blockhit==0 && robohit)
				{
					SPGx[i]=x;
					SPGy[i]=y;
					dir = !dir;
				}
				else if (blockhit!=0 && !robohit)
				{
					SPGx[i]=-1;
					SPGy[i]=-1;
					done=true;
				}

				i++;
				if (i==64)
					done=true;
			
				if (! dir)
				{
					y++;
					if (y==16)
					{
						done=true;
						SPGx[i]=-1;
						SPGy[i]=-1;
					}
				}
				else
				{
					x--;
					if (x==0)
					{
						done=true;
						SPGx[i]=-1;
						SPGy[i]=-1;
					}
				}
			}
			while (! done);			
		}

		///////////////////////////////////////
		//	Check if SPG hit Robot
		///////////////////////////////////////
		static bool SPGCheckRobo(this CCLayerColor layer, int x, int y)
		{
			x *= 8;
			y *= 8;

			for (int i = 0 ; i < 4 ; i++)
			{
				if (_hRobots[i].Valid && RectsOverlap(x, y, 8, 8, _hRobots[i].X, _hRobots[i].Y, 16, 16))
					return true;
				if (_vRobots[i].Valid && RectsOverlap(x, y, 8, 8, _vRobots[i].X, _vRobots[i].Y, 16, 16))
						return true;
			}

			return false;
		}
	}
}

