using System;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Sprite2Images
{
	class MainClass
	{
		Bitmap			_source = null;
		int				_imageCount = -1;
		DirectoryInfo	_outputFolder = null;
		Size			_spriteSize = Size.Empty;
		bool			_isValid = true;

		void Syntax(string arg, string errorMessage = null)
		{
			_isValid = false;
			var writer = Console.Error ?? Console.Out ;

			if (arg != null)
				writer.WriteLine("Invalid argument: {0}", arg);
			if (errorMessage != null)
				writer.WriteLine(errorMessage);
		}

		MainClass(string[] args)
		{
			foreach(string arg in args)
			{
				if (arg.StartsWith("-size=", StringComparison.OrdinalIgnoreCase))
				{
					if (! _spriteSize.IsEmpty)
					{
						Syntax(arg, "Sprite Size is already defined");
						return;
					}
					var size = arg.Substring(6).Split(new char[] { 'x', 'X' });
					if (size.Length != 2)
					{
						Syntax(arg);
						return ;
					}
					int width, height ;
					if (! int.TryParse(size[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out width) || width <= 0)
					{
						Syntax(arg, "Invalid width");
						return ;
					}
					if (! int.TryParse(size[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out height) || height <= 0)
					{
						Syntax(arg, "Invalid height");
						return ;
					}
					_spriteSize = new Size(width, height);
				}
				else if (arg.StartsWith("-count=", StringComparison.OrdinalIgnoreCase))
				{
					if (_imageCount > 0)
					{
						Syntax(arg, "Sprite count is already defined");
					}
					var count = arg.Substring(7).Trim();
					if (! int.TryParse(count, NumberStyles.Integer, CultureInfo.InvariantCulture, out _imageCount) || _imageCount <= 0)
					{
						Syntax(arg, "Invalid Sprite count");
						return;
					}
				}
				else if (arg.StartsWith("-output=", StringComparison.OrdinalIgnoreCase))
				{
					if (_outputFolder != null)
					{
						Syntax(arg, "Output is already defined");
						return;
					}
					var output = arg.Substring(8).Trim();
					if (string.IsNullOrEmpty(output))
					{
						Syntax(arg);
						return;
					}
					if (File.Exists(output))
					{
						Syntax(arg, "output should define a folder");
						return; 
					}
					try
					{
						_outputFolder = new DirectoryInfo(output);
					}
					catch(Exception e)
					{
						Syntax(arg, e.Message);
					}
				}
				else if (! arg.StartsWith("-", StringComparison.OrdinalIgnoreCase))
				{
					if (_source != null)
					{
						Syntax(arg, "Source image is already defined");
						return;
					}

					var file = arg.Trim();
					if (string.IsNullOrEmpty(file))
					{
						Syntax(arg);
						return;
					}
					FileInfo f = null ;
					try
					{
						f = new FileInfo(file);
					}
					catch(Exception e)
					{
						Syntax(arg, e.Message);
					}
					if (! f.Exists)
					{
						Syntax(arg, "Image file doesn't exists");
						return; 
					}
					try
					{
						using (Stream s = f.OpenRead())
						{
							_source = (Bitmap) System.Drawing.Bitmap.FromStream(s);
						}
					}
					catch(Exception e)
					{
						Syntax(arg, e.Message);
						return;
					}
				}
				else
				{
					Syntax(arg);
					return;
				} 
			}
		}
	
		bool Validate()
		{
			if (! _isValid)
				return false;
			if (_spriteSize.Width  > _source.Width || _spriteSize.Height > _source.Height)
			{
				Syntax(null, "Sprite size must be smaller than the image size");
				return false;
			}

			var xCount = (int) (_source.Width / _spriteSize.Width);
			var yCount = (int) (_source.Height / _spriteSize.Height);

			if (xCount * _spriteSize.Width != _source.Width)
			{
				Syntax(null, "Image width must be a multiple of the Sprite width");
				return false;
			}

			var totalImages = xCount * yCount ;

			if (totalImages < _imageCount)
			{
				Syntax(null, "Image not big enought to contain " + _imageCount + " sprites");
				return false;
			}

			return true;
		}

		void Export()
		{
			int index = 0;

			_outputFolder.Create();
			var path = _outputFolder.FullName;

			for (var y = 0; y < _source.Height && index < _imageCount ; y += _spriteSize.Height)
			{
				for (var x = 0 ; x < _source.Width && index < _imageCount ; x += _spriteSize.Width, index++)
				{
					var rect = new Rectangle(new Point(x, y), _spriteSize);

					using (var sprite = _source.Clone(rect, _source.PixelFormat))
					{
						var filename = Path.Combine(path, "f" + index + ".png");
						sprite.Save(filename, ImageFormat.Png);
					}
				}
			}
		}

		public static void Main (string[] args)
		{
			var engine = new MainClass(args);

			if (engine.Validate())
			{
				engine.Export();
			}
		}
	}
}
