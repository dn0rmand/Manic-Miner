using System;
using CocosSharp;
using System.Collections.Generic;
using CocosDenshion;
using System.Reflection;

namespace ManicMiner
{
	static partial class Helper
	{
		public class Label : CCNode
		{
			string	_message;
			int		_color;
			int		_originalColor ;

			protected virtual int CharWidth  { get { return (int) Helper.FontSize.Width; } }
			protected virtual int CharHeight { get { return (int) Helper.FontSize.Height; } }
			protected virtual int CharCount  { get { return 96;} }

			protected virtual CCSpriteFrame GetLetter(int chr, int color)
			{
				return GetFont(chr+(color*CharCount));
			}

			public Label(string ss, int color)
			{
				this.ContentSize = new CCSize(CharWidth * ss.Length , CharHeight);
				_color   = color;
				_originalColor = color ;

				_message = ss ;
				Rebuild();
			}
		
			public void SetText(string text, int? color = null)
			{
				_message = text;
				_color 	 = color.GetValueOrDefault(_color);
					
				Rebuild();
			}

			public void SetColor(int color)
			{
				_color = color;
				Rebuild();
			}

			public void UpdateLetter(CCSprite letter, char c, int color = -1)
			{
				int  cr = ((byte)c) - 32;
				if (c == '©')
					cr = CharCount - 1;
				
				if (color < 0)
					color = _color;

				CCSpriteFrame img = GetLetter(cr, color);
				letter.SpriteFrame = img;
			}

			void Rebuild()
			{
				int xpos      = 0;
				int nodeIndex = 0;
				int color 	  = _color;

				// this.RemoveAllChildren();

				for (int i = 0 ; i < _message.Length ; i++)
				{
					char c  = _message[i];

					int  cr = ((byte)c) - 32;
					if (c == '©')
						cr = CharCount - 1;
				
					if (c == '`')
					{
						c = _message[i+1];
						color = ((byte)c) - CharCount;
						if (color == _originalColor)
							color = _color;
						i++;
					}
					else
					{		
						CCSpriteFrame img = GetLetter(cr, color);

						if (this.ChildrenCount > nodeIndex)
						{
							CCSprite letter = this.Children[nodeIndex] as CCSprite;

							letter.SpriteFrame = img;
						}
						else
						{
							CCSprite letter = new CCSprite(img);

							letter.PositionX = xpos + (CharWidth / 2) ;
							letter.PositionY = CharHeight / 2;
					
							this.AddChild(letter);
						}

						nodeIndex++;
						xpos += CharWidth;
					}
				}

				// Cleanup Extra letters.

				for(int i = this.ChildrenCount ; i > nodeIndex ; i--)
				{
					var child = this.Children[i-1];
					this.RemoveChild(child);
				} 

				var size = this.ContentSize;
				size.Width = xpos;
				this.ContentSize = size;
				this.AnchorPoint = CCPoint.AnchorMiddle;
			} 
		}

		class SmallLabel : Label
		{
			protected override int CharWidth  { get { return (int) Helper.SmallFontSize.Width; } }
			protected override int CharHeight { get { return (int) Helper.SmallFontSize.Height; } }
			protected override int CharCount  { get { return 64; } }

			protected override CCSpriteFrame GetLetter(int chr, int color)
			{
				if (chr > 3)
					chr--;
				return GetSmallFont( chr > 0 ? (chr)+(color*CharCount) : 0 );
			}

			public SmallLabel(string ss, int color) : base(ss.ToUpperInvariant(), color)
			{
			}
		}

		static readonly CCSize _fontSize 		= new CCSize(8, 8);
		static readonly CCSize _smallFontSize 	= new CCSize(4, 6);

		public static CCSize FontSize
		{
			get { return _fontSize; }
		}

		public static CCSize SmallFontSize
		{
			get { return _smallFontSize; }
		}

		public static CCSpriteFrame GetFont(int index)
		{
			var font = CCSpriteSheetCache.Instance.AddSpriteSheet("font.plist");
			return font["f"+index];
		}

		public static CCSpriteFrame GetSmallFont(int index)
		{
			var font = CCSpriteSheetCache.Instance.AddSpriteSheet("fonts.plist");
			return font["f"+index];
		}

		static Label InnerPrint(this CCNode layer, int x, int y, string ss, int col, int tag)
		{
			var label = new Label(ss, col);

			if (tag > 0)
				label.Tag = tag;

			if (x < 0)
				x = (int) ((layer.ContentSize.Width - label.ContentSize.Width) / 2);
			layer.AddSprite(label, x, y);
			return label;
		}

		public static Label Print(this CCNode layer, int x, int y, string ss, int color, int tag = -1)
		{
			if (tag > 0)
			{
				var text = layer.GetChildByTag(tag) as Helper.Label;
				if (text == null)
				{
					layer.RemoveChildByTag(tag);
					text = layer.InnerPrint(x, x, ss, color, tag);
				}
				else
					text.SetText(ss, color);

				return text;
			}
			else
			{
				return layer.InnerPrint(x, y, ss, color, tag);
			}
		}

		public static CCNode PrintButton(this CCNode layer, int x, int y, string ss, int col)
		{
			var background = new CCDrawNode();

			background.AnchorPoint = CCPoint.AnchorMiddle;
			background.ContentSize = new CCSize(ss.Length * 8 + 4, 12);
			var label = background.Print(2, 2, ss, col);
			var size  = label.ContentSize+4;
			background.ContentSize = size ;
			if (x < 0)
				x = (int) ((layer.ContentSize.Width - background.ContentSize.Width) / 2);

			layer.AddSprite(background, x, y-2);

			background.DrawRect(new CCRect(0, 0, size.Width, size.Height), CCColor4B.White, 0.5f, CCColor4B.Gray);
			return background;
		}
		 
		public static Label SmallPrint(this CCNode layer, int x, int y, string ss, int col)
		{
			var label = new SmallLabel(ss, col);

			if (x < 0)
				x = (int) ((layer.ContentSize.Width - label.ContentSize.Width) / 2);

			layer.AddSprite(label, x, (y+1) * (int) SmallFontSize.Height);
			return label;
		}
	}
}

