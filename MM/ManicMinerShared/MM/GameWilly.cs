using System;
using CocosSharp;

namespace ManicMiner
{
	partial class Game
	{		
		///////////////////////////////////////
		//	Do Willy
		///////////////////////////////////////
		public static void DoWilly(this CCLayerColor layer)
		{
			var inputKey = GetWillyInput();

			layer.CheckRoboHit();

			if (CheckWillyKillBlock() != 0)
				_willy.Mode = WillyMode.Death;

			if (_willy.Mode == WillyMode.Normal)
		    {
				CheckWillyFall();
				inputKey = CheckWillyConveyorBelt(inputKey);
			}

			switch (_willy.Mode)
		    {
				case WillyMode.Normal:
		        {
					layer.CheckCrumb();
					if (inputKey == 1)
		            {
						DoWillyLeft();
						_willy.FallHeight=0;
		            }
					else if (inputKey == 2)
		            {
		                DoWillyRight();
						_willy.FallHeight=0;
		            }
					else if (inputKey == 4)
		            {
						_willy.Mode 		= WillyMode.JumpUp;
						_willy.Jump			= 0;
						_willy.FallHeight	= 0;
		                DoWillyJump();
		            }
					else if (inputKey == 5)
		            {
						if (_willy.Direction == Direction.Right)
		                {
							_willy.Direction = Direction.Left;
							_willy.Mode = WillyMode.JumpUp;
							_willy.Jump = 0;
							_willy.FallHeight = 0;
		                    DoWillyJump();
		                }
		                else
		                {
							_willy.Mode = WillyMode.JumpLeft;
							_willy.Jump = 0;
							_willy.FallHeight = 0;
		                    DoWillyLeft();
		                    DoWillyJump();
		                }
		            }
					else if (inputKey == 6)
		            {
						if (_willy.Direction == Direction.Left)
		                {
							_willy.Direction = Direction.Right;
							_willy.Mode = WillyMode.JumpUp;
							_willy.Jump = 0;
							_willy.FallHeight = 0;
		                    DoWillyJump();
		                }
		                else
		                {
							_willy.Mode = WillyMode.JumpRight;
							_willy.Jump = 0;
							_willy.FallHeight=0;
		                    DoWillyRight();
		                    DoWillyJump();
		                }
		            }
		            else
		            {
						_willy.FallHeight=0;
		            }
		            break;
		        }
				case WillyMode.JumpUp:
		        {
		        	DoWillyJump();
		            break;
		        }
				case WillyMode.JumpLeft:
		        {
					DoWillyLeft();
					DoWillyJump();
		            break;
				}
				case WillyMode.JumpRight:
		        {
					DoWillyRight();
					DoWillyJump();
		            break;
		        }
				case WillyMode.Fall:
		        {
					DoWillyFall();
		            break;
				}
				case WillyMode.Death:
		        {
					DoDeath();
		            break;
		        }
		    }
			layer.CheckKeys();
			if (CheckExit())
				GAMEmode = 3;
			layer.CheckSwitches();
			layer.DrawWilly();
		}

		public static bool	LeftOn 	= false;
		public static bool	RightOn	= false;
		public static bool	JumpOn  = false;

		///////////////////////////////////////
		//	Get Willys Input
		///////////////////////////////////////
		static int GetWillyInput()
		{
		    int lft = LeftOn ? 1 : 0;
			int rgt = RightOn? 2 : 0;
			int jmp = JumpOn ? 4 : 0;
			
			return (lft | rgt | jmp);
		}

		///////////////////////////////////////
		//	Get a Block
		///////////////////////////////////////
		public static int GetBlock(int x, int y)
		{
			return cROOM[(y/8)+1, (x/8)+1];
		}

		///////////////////////////////////////
		//	Check Crumbling Block
		///////////////////////////////////////
		static void CheckCrumb(this CCLayerColor layer)
		{
			int blk1 = cCRUMB[(_willy.Y/8)+3, (_willy.X/8)+1];
			int blk2 = cCRUMB[(_willy.Y/8)+3, (_willy.X/8)+2];

			if (blk1 != 0)
			{
				blk1--;
				cCRUMB[(_willy.Y/8)+3, (_willy.X/8)+1]=blk1;
				if (blk1 == 0)
					layer.RemoveRoomBlock((_willy.X/8)+1, (_willy.Y/8)+3);
				else
					cCRUMBModified = true ;
			}

			if (blk2 != 0)
			{
				blk2--;
				cCRUMB[(_willy.Y/8)+3, (_willy.X/8)+2]=blk2;
				if (blk2 == 0)
					layer.RemoveRoomBlock((_willy.X/8)+2, (_willy.Y/8)+3);
				else
					cCRUMBModified = true ;
			}
		}
			
		///////////////////////////////////////
		//	Draw Willy
		///////////////////////////////////////
		static void DrawWilly(this CCLayerColor layer)
		{
			if (_willy.Direction == Direction.Left)
			{
				layer.MoveSprite(2000, _willy.X & 248, _willy.Y, 8+((_willy.X & 15) >> 1));
			}
			else
			{
				layer.MoveSprite(2000, _willy.X & 248, _willy.Y, (_willy.X & 15) >> 1);
			}
		}

		///////////////////////////////////////
		//	Do Willy Left
		///////////////////////////////////////
		static void DoWillyLeft()
		{
			if (_willy.Direction == Direction.Right)
			{
				_willy.Direction = Direction.Left;
			}
			else
			{
				_willy.X -= 2;
				int blk1=GetBlock(_willy.X, _willy.Y);
				int blk2=GetBlock(_willy.X, _willy.Y+8);
				int blk3=GetBlock(_willy.X, _willy.Y+12);
				if (blk1 == 3 || blk2 == 3 || blk3 == 3)
				{
					_willy.X += 2;
				}
			}
		}

		///////////////////////////////////////
		//	Do Willy Right
		///////////////////////////////////////
		static void DoWillyRight()
		{
			if (_willy.Direction == Direction.Left)
			{
				_willy.Direction = Direction.Right;
			}
			else
			{
				_willy.X += 2;
				int blk1=GetBlock(_willy.X+8, _willy.Y);
				int blk2=GetBlock(_willy.X+8, _willy.Y+8);
				int blk3=GetBlock(_willy.X+8, _willy.Y+12);
				if (blk1 == 3 || blk2 == 3 || blk3 == 3)
				{
					_willy.X -= 2;
				}
			}
		}

		///////////////////////////////////////
		//	Do Willy Jump
		///////////////////////////////////////
		static void DoWillyJump()
		{
			int jp = ((_willy.Jump & 254)-8)/2;
			_willy.Y += jp;

			if (_willy.Jump < 8)
			{
				int blk1=GetBlock(_willy.X, _willy.Y);
				int blk2=GetBlock(_willy.X+8, _willy.Y);
				if (blk1 == 3 || blk2 == 3)
				{
					_willy.Mode = WillyMode.Fall;
					_willy.Y = (_willy.Y+8) & 248;
				}
			}

			if (_willy.Jump > 11)
			{
				if ((_willy.Y & 7) == 0)
				{
					int blk1=GetBlock(_willy.X, _willy.Y+16);
					int blk2=GetBlock(_willy.X+8, _willy.Y+16);
					if (blk1 != 0 || blk2 != 0)
					{
						_willy.Mode = WillyMode.Normal;
						_willy.Jump=0;
						_willy.Y = (_willy.Y & 248);
					}
				}
			}
			
			_willy.Jump++;
			if (_willy.Jump == 18)
			{
				_willy.Mode = WillyMode.Normal;
				_willy.Jump = 0;
				CheckWillyFall();
			}

			if (_willy.Jump > 12)
				_willy.FallHeight += jp;
		}

		///////////////////////////////////////
		//	Check Willy Fall
		///////////////////////////////////////
		static void CheckWillyFall()
		{
			int blk1=GetBlock(_willy.X, _willy.Y+16);
			int blk2=GetBlock(_willy.X+8, _willy.Y+16);
			if (blk1 == 0 && blk2 == 0)
			{
				_willy.Mode = WillyMode.Fall;
			}
		}

		///////////////////////////////////////
		//	Do Willy Fall
		///////////////////////////////////////
		static void DoWillyFall()
		{
			_willy.Y += 4;
			int blk1 = GetBlock(_willy.X, _willy.Y+16);
			int blk2 = GetBlock(_willy.X+8, _willy.Y+16);
			if (blk1 != 0 || blk2 != 0)
		    {
				_willy.Y &= 248;
				_willy.Mode = WillyMode.Normal;
				if (_willy.FallHeight >= 32 && ! CheckExit())
					_willy.Mode = WillyMode.Death; // Die unless level is finished.
				else
					_willy.FallHeight = 0;
		    }
		    else
		    {
				_willy.FallHeight += 4;
		    }
		}

		///////////////////////////////////////
		//	Check Willy hit Kill block
		///////////////////////////////////////
		static int CheckWillyKillBlock()
		{
			int blk1 = GetBlock(_willy.X, _willy.Y);
			int blk2 = GetBlock(_willy.X+8, _willy.Y);
			int blk3 = GetBlock(_willy.X, _willy.Y+8);
			int blk4 = GetBlock(_willy.X+8, _willy.Y+8);
			int blk5 = GetBlock(_willy.X, _willy.Y+16);
			int blk6 = GetBlock(_willy.X+8, _willy.Y+16);

			int hit=0;

			if (blk1 == 5 || blk2 == 5 || blk3 == 5 || blk4 == 5 || blk5 == 5 || blk6 == 5)
		    {
				hit = 1;
			}
			
			if (blk1 == 6 || blk2 == 6 || blk3 == 6 || blk4 == 6 || blk5 == 6 || blk6 == 6)
		    {
		        hit = 1;
			}
			return hit;
		}

		///////////////////////////////////////
		//	Do Willy Death
		///////////////////////////////////////
		static void DoDeath()
		{
			GAMEmode=2;
		}

		///////////////////////////////////////
		//	Check Keys
		///////////////////////////////////////
		static void CheckKeys(this CCLayerColor layer)
		{
			for (int i = 0 ; i < 5 ; i++)
				_keys[i].CheckAndPick(layer, _willy);
		}

		///////////////////////////////////////
		//	Check Exit
		///////////////////////////////////////
		static bool CheckExit() // bool justChecking)
		{
			return _exit.Overlaps(_willy);
		}

		///////////////////////////////////////
		//	Check is Willy On ConveyorBelt
		///////////////////////////////////////
		static int CheckWillyConveyorBelt(int inputKey)
		{
			return _conveyorBelt.Check(_willy, inputKey); 
		}

		///////////////////////////////////////
		//	Check if hit Robo
		///////////////////////////////////////
		static void CheckRoboHit(this CCLayerColor layer)
		{
			for (int i=0 ; i < 4 ; i++)
			{
				if (_hRobots[i].Collide(layer, _willy.X, _willy.Y, _willy.Direction == Direction.Left ? 8 : 0))
					_willy.Mode = WillyMode.Death;

				if (_vRobots[i].Collide(layer, _willy.X, _willy.Y, _willy.Direction == Direction.Left ? 8 : 0))
					_willy.Mode = WillyMode.Death;
			}

			switch (ROOM)
			{
				case 5:
					if (_eugene.Collide(layer, _willy.X, _willy.Y, _willy.Direction == Direction.Left ? 8 : 0))
						_willy.Mode = WillyMode.Death;
					break;
				case 8:
					if (_kong.Collide(layer, _willy.X, _willy.Y, _willy.Direction == Direction.Left ? 8 : 0))
						_willy.Mode = WillyMode.Death;
					break;
				case 12:
					if (_kong.Collide(layer, _willy.X, _willy.Y, _willy.Direction == Direction.Left ? 8 : 0))
						_willy.Mode = WillyMode.Death;
					break;
				case 14:
					for (int i = 0; i < 3; i++)
					{
						if (_skylabs[i].Collide(layer, _willy.X, _willy.Y, _willy.Direction == Direction.Left ? 8 : 0))
						{
							_willy.Mode = WillyMode.Death;
							break;
						}
					}
					break;
			}
		}

		///////////////////////////////////////
		//	Check Switches
		///////////////////////////////////////
		static void CheckSwitches(this CCLayerColor layer)
		{
		    for (int i = 0 ; i < 2 ; i++)
		    {
				if (_switches[i].Check(layer, _willy))
					Game.cSWITCHsModified = true;
			}
		}

		///////////////////////////////////////
		//	Check SPG
		///////////////////////////////////////
		static void CheckSPG(this CCLayerColor layer)
		{
			for (int i = 0 ; i <= 64 ; i++)
		    {
				if (SPGx[i] != -1)
		        {
					if (RectsOverlap(SPGx[i]*8, SPGy[i]*8, 8, 8, _willy.X, _willy.Y, 8, 16))
		            {
						cAIRp = cAIRp / 2;
		            }
				}
			}
		}

		static CCRect Index2Rect(int index)
		{
			var x = index % 20;
			var y = index / 20;

			var rect = new CCRect(x * 16, y * 16, 16, 16);

			return rect;
		}

		static int MakeMaskOffset(int x, int y, int refX, int refY)
		{
			var offX = x - refX;
			if (offX < 0 || offX >= 16)
				return -1;

			var offY = y - refY;
			if (offY < 0 || offY >= 16)
				return -1;

			return offX + offY*16;
		}

		public static bool ImagesCollide(this CCLayerColor layer, int x1, int y1, int idx1, int x2, int y2, int idx2)
		{
			CCRect rect1 = new CCRect(x1, y1, 16, 16);
			CCRect rect2 = new CCRect(x2, y2, 16, 16);
			bool   collide;

			CCRect intersect = Intersection(rect1, rect2, out collide);

			if (! collide)
				return false;

			var mask1  = Helper.GetCollisionMask(idx1);
			var mask2  = Helper.GetCollisionMask(idx2);

			for (float x = intersect.MinX ; x <= intersect.MaxX ; x++)
			{
				for (float y = intersect.MinY ; y <= intersect.MaxY ; y++)
				{
					int off1 = MakeMaskOffset((int)x, (int)y, x1, y1);
					if (off1 < 0)
						continue;
					int off2 = MakeMaskOffset((int)x, (int)y, x2, y2);
					if (off2 < 0)
						continue;

					if (mask1[off1] != 0 && mask2[off2] != 0)
						return true; 
				}
			}

			return false;
		}

		public static bool RectsOverlap(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2)
		{
			var rect1 = new CCRect(x1, y1, w1, h1);
			var rect2 = new CCRect(x2, y2, w2, h2);

			bool res;

			var rect = Intersection(rect1, rect2, out res);
			if (res == true && (rect.MaxX == rect.MinX || rect.MaxY == rect.MinY))
				res = false ;
			return res ;
		} 

		public static CCRect Intersection(CCRect r1, CCRect r2, out bool collide)
		{
			collide = true;

			float minX = Math.Max(r1.MinX, r2.MinX);
			float minY = Math.Max(r1.MinY, r2.MinY);
			float maxX = Math.Min(r1.MaxX, r2.MaxX);
			float maxY = Math.Min(r1.MaxY, r2.MaxY);

			float width = maxX - minX;
			float height= maxY - minY;

			if (width < 0 || height < 0)
			{
				collide = false;
				return CCRect.Zero;
			}
			else
			{
				collide = true;
				return new CCRect(minX, minY, maxX - minX, maxY - minY);
			}
		}
	}
}

