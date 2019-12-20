using System;
using CocosSharp;
using System.Diagnostics;
using System.Collections.Generic;

namespace ManicMiner
{
	public class Menu : CCLayerColor
	{
		string 		_title;
		string[]	_items;
		CCSize		_size;

		public static Menu Create(string title, params string[] items)
		{
			return new Menu(title, items);
		}

		Menu(string title, string[] items)
		{
			_title = title ;
			_items = items;

			// Calculate Size 

			CCSize	fontSize = Helper.FontSize;
			CCSize	size = new CCSize(title.Length * fontSize.Width + 8, fontSize.Height + 12);

			foreach(string item in items)
			{
				var width = item.Length * fontSize.Width + 8;
				if (width > size.Width)
					size.Width = width;

				size.Height += fontSize.Height + 4;
			}

			_size = size;
			this.ZOrder = 1000;
		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			var background = new CCDrawNode();

			background.ContentSize = _size + 8;

			var center = this.BoundingBox.Center;

			// Calculation doesn't make sense but it works
			background.PositionX = (center.X - background.ContentSize.Width/2) / 2;  
			background.PositionY = (center.Y - background.ContentSize.Height/2) / 2;

			this.AddChild(background);

			background.DrawRect(background.BoundingBox, new CCColor4B(0, 0, 0, 128), 1, CCColor4B.White);

			int y = (int)((ContentSize.Height - _size.Height)/2 + 4);
			this.Print(-1, y, _title, 6);

			CCSize	fontSize = Helper.FontSize;
			int		index    = 0;

			y += 4; // Extra space for Title

			foreach(string item in _items)
			{
				y += (int) fontSize.Height + 4;
				var sprite = this.Print(-1, y, item, 7);

				var idx = ++index;
				background.AddTouchListener(sprite, t => OnClick(idx));
			}
		}

		void OnClick(int index)
		{
			if (Clicked != null)
				Clicked(this, index);
		}

		public event EventHandler<int>	Clicked;
	}
}

