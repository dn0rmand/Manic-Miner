using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CocosSharp;
using System.Reflection;
using System.Diagnostics;

namespace ManicMiner
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
    	bool GetValue(string name, bool defaultValue)
    	{
			if (NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString(name)) == null)
				return defaultValue ;
			else
				return NSUserDefaults.StandardUserDefaults.BoolForKey(name);
    	}

    	void UpdateSettings()
    	{    		
			ManicMinerApplicationDelegate.SoundEnabled 	= GetValue("sound", true);
			ManicMinerApplicationDelegate.MusicEnabled 	= GetValue("music", true);
			ManicMinerApplicationDelegate.Invincible   	= GetValue("unlockDoors", false);
			ManicMinerApplicationDelegate.Invincible   	= GetValue("invincible", false);
			ManicMinerApplicationDelegate.InfiniteLives	= GetValue("infiniteLives", false);
			ManicMinerApplicationDelegate.DisplayStats	= GetValue("stats", false);
			ManicMinerApplicationDelegate.NoAds 		= ! GetValue("showAds", true);
    	}

		CCApplication					_gameApplication;
		ManicMinerApplicationDelegate	_gameDelegate;
    	string 							_state;
		int 							_highScore ;

    	string ReadState()
    	{
			var state = NSUserDefaults.StandardUserDefaults.StringForKey("savedState");
			_state = state ?? string.Empty;
			return _state;
    	}

    	void SaveState(string state)
    	{
    		state = state ?? string.Empty; // Don't like null values.

			if (state != _state)
    		{
				_state = state;
				NSUserDefaults.StandardUserDefaults.SetString(state, "savedState");
				NSUserDefaults.StandardUserDefaults.Synchronize();
    		}
    	}

    	void SaveHighscore(int value)
		{
			if (value != _highScore)
			{
				NSUserDefaults.StandardUserDefaults.SetInt(value, "highscore");
				NSUserDefaults.StandardUserDefaults.Synchronize();
				_highScore = value;
			}
    	}

    	public AppDelegate()
    	{
			ManicMinerApplicationDelegate.HideAdCallback 		= HideAd ;
			ManicMinerApplicationDelegate.ShowAdCallback 		= ShowAd;
			ManicMinerApplicationDelegate.SaveHighscoreCallback = SaveHighscore;
			ManicMinerApplicationDelegate.ReadStateCallback		= ReadState;
			ManicMinerApplicationDelegate.SaveStateCallback		= SaveState;
    	}

        public override void FinishedLaunching (UIApplication app)
        {
            _gameApplication = new CCApplication();
                    			
			_highScore = NSUserDefaults.StandardUserDefaults.IntForKey("highscore");

			Game.HiScore = _highScore;

			UpdateSettings();

			_gameDelegate = new ManicMinerApplicationDelegate();
			_gameApplication.ApplicationDelegate = _gameDelegate;

			_gameApplication.StartGame ();
			StartMemoryDump();
        }

        [Conditional("DEBUG")]
        void StartMemoryDump()
        {
			NSTimer.CreateRepeatingScheduledTimer(5, () => { Helper.DumpGCSize(); });
        }

        public override void DidEnterBackground (UIApplication application)
		{
			MoveOffScreen();
		}

        public override void WillEnterForeground (UIApplication application)
		{
			UpdateSettings();
		}

		public override void WillTerminate (UIApplication application)
		{
			_gameDelegate.WillTerminate(_gameApplication);
		}

		public override void ReceiveMemoryWarning (UIApplication application)
		{
			_gameApplication.PurgeAllCachedData();
			#if DEBUG
			WillTerminate(application);
			#endif
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations (UIApplication application, UIWindow forWindow)
		{
			return UIInterfaceOrientationMask.Landscape;
		}
    }
}