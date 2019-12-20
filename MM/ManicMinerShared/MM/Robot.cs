using System;

namespace ManicMiner
{
	struct HRobot
	{
		public int			X;
		public int			Y;
		public Direction	Direction;
		public int			Min;
		public int			Max;
		public int 			Frame;
		public int			Flip;
		public int			Gfx;
		public int			Speed;
		public int			Tag;

		public bool Valid { get { return X != -1; } }

		public int ImageIndex
		{
			get
			{
				var idx = Gfx + (X & Frame)/2;

				if (Direction != Direction.Right)
					idx += Flip;

				return idx;
			}
		}

		public void Move(CocosSharp.CCLayerColor layer)
		{
			if (! Valid)
				return;

			if (Direction == Direction.Left)
            {
				X -= Speed;

				if (X <= Min)
					Direction = Direction.Right;
            }
            else
            {
				X += Speed;

				if (X > Max)
                    Direction = Direction.Left;

			}
			layer.DrawRobot(Tag, X & 248, Y, ImageIndex);
		}

		public bool Collide(CocosSharp.CCLayerColor layer, int willyX, int willyY, int frameOffset)
		{
			if (! Valid)
				return false;

			return layer.ImagesCollide(willyX & 248, willyY, frameOffset + (willyX & 15) >> 1, X & 248, Y, ImageIndex);
		}
	}

	struct VRobot
	{
		public int			X;
		public int			Y;
		public bool			GoingUp;
		public int			Min;
		public int			Max;
		public int 			Frame;
		public int			Gfx;
		public int			Speed;
		public int			Tag;

		public bool Valid { get { return X != -1; } }

		public int ImageIndex
		{
			get
			{
				return Gfx + Frame;
			}
		}

		public void Move(CocosSharp.CCLayerColor layer)
		{
			if (! Valid)
				return;

			if (GoingUp)
            {
				Y -= Speed;

				if (Y < Min)
				{
					GoingUp = false;
					Y += Speed;
				}
            }
            else
            {
				Y += Speed;

				if (Y > Max)
				{
					GoingUp = true;
					Y -= Speed;
				}
			}
			layer.DrawRobot(Tag, X & 248, Y, ImageIndex);
			Frame = (Frame + 1) & 3;
		}

		public bool Collide(CocosSharp.CCLayerColor layer, int willyX, int willyY, int frameOffset)
		{
			if (! Valid)
				return false;

			return layer.ImagesCollide(willyX & 248, willyY, frameOffset + (willyX & 15) >> 1, X, Y, ImageIndex);
		}
	}

	struct Eugene
	{
		int			X;
		int			Y;
		bool		GoingUp;
		bool		CanExit;
		int			Min;
		int			Max;
		int			Tag;
		int			_frame;

		public void Reset(int tag)
		{
			X 		= 120;
			Y 		= 1;
			GoingUp = false;
			CanExit = false;
			Min		= 1;
			Max		= 87;
			_frame  = 7;
			Tag		= tag;
		}

		public void Move(CocosSharp.CCLayerColor layer, bool canExit)
		{
			if (CanExit)
			{
				if (Y < Max)
					Y++;
				layer.MoveSprite(Tag, X, Y, 412 + _frame);
				_frame = (_frame + 1) & 7;
			}
			else
			{
				if (GoingUp)
	            {
					Y--;

					if (Y < Min)
					{
						GoingUp = false;
						Y++;
					}
	            }
	            else
	            {
					Y++;

					if (Y > Max)
					{
						GoingUp = true;
						Y--;
					}
				}
				layer.MoveSprite(Tag, X, Y, 418);

				if (canExit)
				{
					CanExit=true;
					_frame = 0;
				}
			}
		}

		public bool Collide(CocosSharp.CCLayerColor layer, int willyX, int willyY, int frameOffset)
		{
			return layer.ImagesCollide(willyX & 248, willyY, frameOffset + (willyX & 15) >> 1, X, Y, 418);
		}
	}

	struct Kong
	{
		int		_x;
		int		_y;
		int		_mode;
		int		_tag;
		int		_iteration;
		int		_frame;

		public void Animate()
		{
			_iteration++;
			if (_iteration==8)
			{
				_iteration 	= 0;
				_frame 		= (_frame + 1) & 1;
			}
		}

		public void Reset(int tag)
		{
			_x    		= 120;
			_y    		= 0;
			_mode 		= 0;
			_frame		= 0;
			_iteration 	= 0;
			_tag  		= tag;
		}

		int ImageIndex
		{
			get
			{
				switch (_mode)
				{
					case 0:
						return 408 + _frame;
					case 1:
						return 410 + _frame;
					default:
						return -1;
				}
			}
		}

		public void Move(CocosSharp.CCLayerColor layer)
		{
			int idx = ImageIndex ;

			if (idx >= 0)
				layer.MoveSprite(_tag, _x, _y, idx);
		}

		public void Fall(CocosSharp.CCLayerColor layer)
		{
			switch (_mode)
			{
				case 0:
					layer.RemoveRoomBlock(16, 3);
					layer.RemoveRoomBlock(17, 3);
					Move(layer);
					_mode = 1;
					break;

				case 1:
					_y += 4;
					// TODO: Kong Falling sound
					//SoundPitch(SFXjump2,22050-(KONGy*(((KONGp & 1)+1)*50)));
					//Helper.PlayEffect("jump");
					Move(layer);
					Game.Score += 100;
					if (_y >= 104)
						_mode = 2;
					break;
			}
		}

		public bool Collide(CocosSharp.CCLayerColor layer, int willyX, int willyY, int frameOffset)
		{
			int imgIdx = ImageIndex ;
			if (imgIdx < 0)
				return false;
			else
				return layer.ImagesCollide(willyX & 248, willyY, frameOffset + (willyX & 15) >> 1, _x, _y, imgIdx);
		}
	}

	struct Skylab
	{
		int 	_x ;
		int 	_y ;
		int 	_mode ;
		int 	_max ;
		int 	_frame ;
		int 	_speed ;
		int 	_pos ;
		int		_tag ;
		int 	_index ;
		 
		public static int GetPos(int index)
		{
			switch (index)
			{
				case 1:
					return 1;
				case 2:
					return 3;
				case 3:
					return 2;
			}

			return 0;
		}

		public void Init(int index, int x, int y, int tag)
		{
			_pos = GetPos(index);
			switch (index)
			{
				case 1:
					_speed 	= 4;
					_max 	= 72;
					break ;
				case 2:
					_speed 	= 3;
					_max 	= 56 ;
					break;
				case 3:
					_speed 	= 1;
					_max 	= 32;
					break;
			}

			_x 		= x;
			_y 		= y;
			_mode 	= 0;
			_frame 	= 0;
			_index 	= index;
			_tag 	= tag;
		}

		int ImageIndex
		{
			get
			{
				return 252 + _index*8 + _frame;
			}
		}
		public void Move(CocosSharp.CCLayerColor layer)
		{
			switch (_mode)
			{
				case 0:
					_y += _speed;
					if (_y > _max)
					{
						_y = _max;
						_mode = 1;
						_frame++;
					}
					layer.MoveSprite(_tag, _x, _y, ImageIndex);
					break;
				case 1:
					_frame++;
					if (_frame == 7)
						_mode = 2;
					layer.MoveSprite(_tag, _x, _y, ImageIndex);
					break;
				case 2:
					_pos = (_pos + 1) % 4;
					if (_pos == 0)
						_pos = 1;
					_x = Game.SKYpx[_index, _pos];
					_y = Game.SKYpy[_index, _pos];
					_frame = 0;
					_mode = 0;
					break;
			}
		}

		public bool Collide(CocosSharp.CCLayerColor layer, int willyX, int willyY, int frameOffset)
		{
			return layer.ImagesCollide(willyX & 248, willyY, frameOffset + ((willyX & 15) >> 1), _x, _y, ImageIndex);
		}
	}
}
