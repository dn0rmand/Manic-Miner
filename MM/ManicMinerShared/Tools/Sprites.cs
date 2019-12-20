using System;
using CocosSharp;
using System.Collections.Generic;
using CocosDenshion;
using System.Reflection;

namespace ManicMiner
{
	static partial class Helper
	{
		static List<byte[]>		_16x16Mask=null;

		public static CCSpriteFrame GetImage(string name)
		{
			var images = CCSpriteSheetCache.Instance.AddSpriteSheet("other.plist");
			return images[name];
		}

		public static CCSpriteFrame GetSwitch(int index)
		{
			return GetImage("switches-" + index);
		}

		public static CCSpriteFrame GetBlock(int index, int subIndex = -1)
		{
			var blocks = CCSpriteSheetCache.Instance.AddSpriteSheet("blocks.plist");
			if (subIndex > 0)
				return blocks["f"+index+"-"+subIndex];
			else
				return blocks["f"+index];
		}

		public static CCSpriteFrame GetTitleAir(int index)
		{
			return GetImage("titleair-" + index);
		}

		public static byte[] GetCollisionMask(int index)
		{
			if (_16x16Mask == null)
			{
				_16x16Mask = new List<byte[]>();

				var masks = CCFileUtils.GetFileBytes("16x16.mask");	

				for(int i = 0 ; i < 484 ; i++)
				{
					int 	offset = i * (16*16);
					byte[]	mask   = new byte[16*16];

					Array.Copy(masks, offset, mask, 0, 16*16);
					_16x16Mask.Add(mask);
				}
			}

			return _16x16Mask[index];
		}

		public static CCSpriteFrame GetImage16(int index)
		{
			var blocks = CCSpriteSheetCache.Instance.AddSpriteSheet("16x16.plist");
			return blocks["f"+index];
		}

//		public static CCSpriteFrame GetFont(int index)
//		{
//			var font = CCSpriteSheetCache.Instance.AddSpriteSheet("font.plist");
//			return font["f"+index];
//		}
//
//		public static CCSpriteFrame GetSmallFont(int index)
//		{
//			var font = CCSpriteSheetCache.Instance.AddSpriteSheet("fonts.plist");
//			return font["f"+index];
//		}

		public static CCNode AddSprite(this CCNode layer, string image, int x, int y)
		{
			var sprite = GetImage(image);
			return layer.AddSprite(sprite, x, y);
		}

		public static CCNode AddSprite(this CCNode layer, CCSpriteFrame spriteframe, int x, int y)
		{
			var sprite = new CCSprite(spriteframe);
			sprite.IsAntialiased = false;
			return layer.AddSprite(sprite, x, y);
		}

		public static CCNode AddSprite(this CCNode layer, CCNode sprite, int x, int y)
		{
			layer.MoveSprite(sprite, x, y);
			layer.AddChild(sprite);
			return sprite;
		}

		public static void MoveSprite(this CCNode layer, CCNode sprite, int x, int y)
		{
			var size	= sprite.ContentSize;
			var center 	= sprite.AnchorPointInPoints; // size.Center;

			var xx = center.X + x;
			var yy = layer.ContentSize.Height - y - center.Y;

			sprite.Position = new CCPoint(xx, yy);
		}

		public static void MoveSprite(this CCLayerColor layer, int tag, int x, int y, int index)
		{
			CCSprite s = (CCSprite) layer.GetChildByTag(tag);

			if (s == null)
				layer.AddSprite(Helper.GetImage16(index), x, y).Tag = tag;
			else
			{
				s.SpriteFrame = Helper.GetImage16(index);
				layer.MoveSprite(s, x, y);
			}
		}
	}
}

