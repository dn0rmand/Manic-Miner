using System;
using CocosDenshion;
using CocosSharp;

namespace ManicMiner
{
    public class ManicMinerApplicationDelegate : CCApplicationDelegate
    {
		bool 	_started = false;
    	 
    	// Global Settings

    	public static bool 				SoundEnabled 	{ get; set; }
    	public static bool 				MusicEnabled 	{ get; set; }
		public static bool 				UnlockDoors		{ get; set; }
    	public static bool 				Invincible		{ get; set; }
		public static bool 				InfiniteLives	{ get; set; }
    	public static bool 				DisplayStats	{ get; set; }
		public static bool 				NoAds			{ get; set; }

    	// Callbacks to Device Specific methods

    	public static Action<CCWindow>	ShowAdCallback;
		public static Action<CCWindow>	HideAdCallback;
		public static Action<int>		SaveHighscoreCallback;
		public static Func<string>		ReadStateCallback;
		public static Action<string>	SaveStateCallback;

		// Device Specific method accessor

		public static void SaveHighscore(int highScore)
		{
			if (SaveHighscoreCallback != null)
				SaveHighscoreCallback(highScore);
		}

    	public static void ShowAd(CCWindow window)
    	{
    		if (ShowAdCallback != null)
    			ShowAdCallback(window);
    	}

    	public static void HideAd(CCWindow window)
    	{
			if (HideAdCallback != null)
    			HideAdCallback(window);
    	}

    	string GameState
    	{
    		get
    		{
    			if (ReadStateCallback != null)
    				return ReadStateCallback();
    			else
					return string.Empty;
    		}
    		set
    		{
    			if (SaveStateCallback != null)
    				SaveStateCallback(value);
    		}
    	}

		void SaveState(CCWindow window)
        {
			if (! _started)
				return;	

        	if (window == null)
        		return;

        	var director = window.DefaultDirector;

        	if (director == null)
        		return;

			var 	currentScene = window.DefaultDirector.RunningScene;
			string	state		 = null;

			if (currentScene != null && currentScene.ChildrenCount > 0)
			{
				foreach(var child in currentScene.Children)
				{
					if (child is GameLayer)
					{
						state = ((GameLayer)child).SaveState();
						break;
					}
				}
			}
		
			GameState = state;
        }

        public override void ApplicationDidFinishLaunching (CCApplication application, CCWindow mainWindow)
        {        	
            application.PreferMultiSampling = false;
            application.ContentRootDirectory = "Content";
            application.ContentSearchPaths.Add("gfx");

            mainWindow.DisplayStats = DisplayStats;

            CCSize winSize = new CCSize(320, 240);// mainWindow.WindowSizeInPixels;
            mainWindow.SetDesignResolutionSize(winSize.Width, winSize.Height, CCSceneResolutionPolicy.ExactFit);

            CCScene scene = GameLayer.CreateScene(mainWindow, GameState);

            if (scene == null)
	            scene = GamePianoLayer.CreateScene(mainWindow);
	        else
		        GameState = null;

   	        mainWindow.RunWithScene(scene);
            _started = true;
        }

        public override void ApplicationDidEnterBackground (CCApplication application)
        {
            // stop all of the animation actions that are running.
            application.Paused = true;
            Helper.PauseMusic();
			SaveHighscore(Game.HiScore); // Ensure Highscore is saved.
			SaveState(application.MainWindow); // In case the app gets killed while in the background
        }

        public void WillTerminate(CCApplication application)
        {
			SaveHighscore(Game.HiScore); // Ensure Highscore is saved.
        	SaveState(application.MainWindow);
        }        	

        public override void ApplicationWillEnterForeground (CCApplication application)
        {
            application.Paused = false;
			Helper.ResumeMusic();

			if (_started && application.MainWindow != null)
				application.MainWindow.DisplayStats = DisplayStats;
        }
    }
}