using System;

namespace ManicMiner
{
	struct Willy
	{
		public int			X;
		public int			Y;
		public Direction	Direction;
		public int			FallHeight;
		public int			Jump;

		WillyMode			_mode;

		public WillyMode 	Mode
		{
			get 
			{ 
				return _mode; 
			}
			set 
			{ 
				if (value == _mode)
					return; // No Change

				switch (value)
				{
					case WillyMode.Normal:
						Helper.StopEffect();
						break ;
					
					case WillyMode.JumpUp:
					case WillyMode.JumpLeft:
					case WillyMode.JumpRight:
						switch (_mode)
						{
							case WillyMode.JumpLeft:
							case WillyMode.JumpRight:
							case WillyMode.JumpUp:
								break;

							default:
								// Starting to Jump
								Helper.PlayEffect("jump");
								break;
						}
						break;

					case WillyMode.Fall:
						Helper.PlayEffect("falling");
						break;

					case WillyMode.Death:
						if (! ManicMinerApplicationDelegate.Invincible)
						{
							Helper.StopMusic();
							Helper.PlayEffect("die");
						}
						else // Ignore value by setting it to previous one
							value = _mode ;
						break;
				}

				_mode = value;
			}
		} 
	}
}

