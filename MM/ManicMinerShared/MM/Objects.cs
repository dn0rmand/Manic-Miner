using System;
using CocosSharp;

namespace ManicMiner
{
	struct ConveyorBelt
	{
		int			_x;
		int			_y;
		Direction	_direction;
		int			_length;
		int			_frame;

		public void Init(int x, int y, bool reversed, int length)
		{
			Init(x, y, reversed ? Direction.Left : Direction.Right, length);
		}

		public void Init(int x, int y, Direction direction, int length)
		{
			_x 			= x;
			_y 			= y;
			_direction 	= direction;
			_length		= length;
			_frame		= 0;
		}

		public void Draw(CCLayerColor layer, int blockOffset)
		{
			if (_length > 0)
			{
				for (int x = 0 ; x < _length ; x++)
				{
					var tag 	= Game.MakeConveyorTag(x);
					var block 	= Helper.GetBlock(blockOffset + 7 + _frame);

					var sprite = (CCSprite) layer.GetChildByTag(tag);
					if (sprite != null)
					{
						sprite.SpriteFrame = block;
					}
					else
					{
						layer.AddSprite(block, _x + (x*8), _y).Tag = tag;
					}
				}

				if (_direction == Direction.Right)
					_frame = (_frame + 1) & 3;
				else
					_frame = (_frame - 1) & 3;
			}
		}

		///////////////////////////////////////
		//	Check if is Willy On ConveyorBelt
		///////////////////////////////////////
		bool WillyOnConveyorBelt(Willy willy)
		{
			int blk1 = Game.GetBlock(willy.X  , willy.Y+16);
			int blk2 = Game.GetBlock(willy.X+8, willy.Y+16);
			return (blk1 == 7 || blk2 == 7);
		}

		public int Check(Willy willy, int inputKey)
		{
			if (WillyOnConveyorBelt(willy))
		    {
				if (willy.Direction != _direction || (inputKey & 3) == 0)
		        {
					if (_direction == Direction.Right)
		            {
						inputKey = ((inputKey & 253) | 1);
					}
		            else
		            {
						inputKey = ((inputKey & 254) | 2);
					}
				}
			}

			return inputKey;
		}
	}

	struct Exit
	{
		int		_x;
		int		_y;
		bool 	_animation;
		int		_tag ;

		public bool Activated;

		public void Init(int x, int y, int tag, bool demo)
		{
			_x   = x;
			_y   = y;
			_tag = tag;
			_animation = false;
			Activated = demo;
		}			

		public void Draw(CCLayerColor layer, int room)
		{
			int index = room;

			if (_animation || ! Activated)
				index += 419;
			else
				index += 439;

			CCSprite sprite = (CCSprite) layer.GetChildByTag(_tag);

			if (sprite == null)
				layer.AddSprite(Helper.GetImage16(index), _x, _y).Tag = _tag;
			else
				sprite.SpriteFrame = Helper.GetImage16(index);

			_animation = ! _animation;
		}

		public bool Overlaps(Willy willy)
		{
			return Activated && Game.RectsOverlap(_x, _y, 16, 16, willy.X+4, willy.Y+8, 2, 2);
		}
	}

	struct Key
	{
		int 	_x;
		int 	_y;
		int 	_block;
		int 	_skip;
		int		_tag;
		bool 	_picked;

		public void Init(int idx, int x, int y, bool picked)
		{
			_x		= x;
			_y		= y;
			_picked	= picked;
			_block	= idx;
			_skip	= 0;
			_tag	= Game.MakeKeyTag(idx);
		}

		public bool Draw(CCLayerColor layer)
		{
			if (! _picked)
			{
				var block	= Helper.GetBlock(Game.BLOCKoff + 11 + _block);

				var keyNode = layer.GetChildByTag(_tag) as CCSprite;
				if (keyNode == null)
				{
					keyNode = (CCSprite) layer.AddSprite(block, _x, _y);
					keyNode.Tag = _tag;
				}
				else
					keyNode.SpriteFrame = block;

				_skip++;
				if (_skip == 2)
				{
					_block = (_block + 1) & 3;
					_skip = 0;
				}
				return true;
			}
			else
				return false;
		}

		public void CheckAndPick(CCLayerColor layer, Willy willy)
		{
			if (! _picked)
	        {
				if (Game.RectsOverlap(_x, _y, 8, 8, willy.X, willy.Y, 10, 18))
	            {
					_picked = true;
					Helper.PlayEffect("pick", false);
					Game.Score += 100;
					layer.RemoveChildByTag(_tag);
				}
			}
		}
	}

	struct Switch
	{
		int	_x ;
		int _y ;
		int _image;
		int _tag;

		public void Init(int idx, int x, int y, int image)
		{
			_tag = Game.MakeSwitchTag(idx);
			_x = x;
			_y = y;
			_image = image;
		}

		public int Image { get { return _image; } }

		public void Draw(CCLayerColor layer)
		{
			if (_image != 0)
			{
				var s = Helper.GetSwitch(_image - 1);

				var node = (CCSprite) layer.GetChildByTag(_tag);
				if (node == null)
					layer.AddSprite(s, _x, _y).Tag = _tag;
				else
					node.SpriteFrame = s;
			}
		}

		public bool Check(CCLayerColor layer, Willy willy)
		{
			if (_image == 1)
			{
				if (Game.RectsOverlap(_x, _y, 8, 8, willy.X, willy.Y, 10, 18))
	            {
	            	_image = 2;
	            	return true ;
				}
			}

			return false;
		}
	}
}

