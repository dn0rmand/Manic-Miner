using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CocosSharp;
using System.Reflection;
using System.Diagnostics;
using MonoTouch.iAd;

namespace ManicMiner
{
    partial class AppDelegate
    {
    	ADBannerView	_adBanner = null;
    	UIView			_gameView = null;
    	bool			_visible  = false;
    	bool			_ready	  = false;

		bool NoAds { get { return ManicMinerApplicationDelegate.NoAds; } }

    	void InitGameView(CCWindow window)
    	{
   			if (_gameView == null)
   			{   				
				Debug.Assert(window != null, "window is null");
				var xnaWindowInfo = window.GetType().GetProperty("XnaWindow", BindingFlags.Instance | BindingFlags.NonPublic);

    			Debug.Assert(xnaWindowInfo != null, "window doesn't have a XnaWindow property");
    			var xnaWindow = xnaWindowInfo.GetValue(window);
				Debug.Assert(xnaWindow != null, "window.XnaWindow is null");
				Debug.Assert(xnaWindow.GetType().Name == "iOSGameWindow", "Window.XnaWindow isn't an iOSGameWindow");
				var controllerInfo = xnaWindow.GetType().GetField("_viewController", BindingFlags.Instance | BindingFlags.NonPublic);
				Debug.Assert(controllerInfo != null, "window.XnaWindow doesn't have a _viewController field");
    			var viewController = controllerInfo.GetValue(xnaWindow) as UIViewController;
    			Debug.Assert(viewController != null, "viewController shouldn't be null"); 
    			_gameView = viewController.View;
    		}
    	}

		void OnAddLoaded(object sender, EventArgs e)
		{
			_adBanner.AdLoaded -= OnAddLoaded;
			_ready = true;
			if (_visible)
				MoveIntoView();
		}

		void MoveIntoView()
		{
			if (_ready && _gameView != null && _adBanner != null && ! NoAds)
			{
				var frame = _gameView.Frame;
				var bannerFrame = _adBanner.Frame;

				bannerFrame.Width = frame.Width;
				bannerFrame.Y     = frame.Bottom - _adBanner.Frame.Height ;

				_adBanner.Frame  = bannerFrame;
				_adBanner.Hidden = false;
			}
		}

		void MoveOffScreen()
		{
			if (_gameView != null && _adBanner != null)
			{
				var frame 		= _gameView.Frame;
				var bannerFrame = _adBanner.Frame;

				bannerFrame.Width = frame.Width;
				bannerFrame.Y     = - (bannerFrame.Height + 10) ;

				_adBanner.Frame  = bannerFrame;
				_adBanner.Hidden = true;
			}
		}

    	void ShowAd(CCWindow window)
    	{
			if (NoAds)
    			return;

    		InitGameView(window);

    		if (_adBanner == null)
    		{
    			_ready = false;
				_adBanner = new ADBannerView(ADAdType.Banner);
				_adBanner.AdLoaded += OnAddLoaded;
				if (_gameView != null)
					_gameView.Add(_adBanner);

				MoveOffScreen();
			}

			_visible = true;
			MoveIntoView();
    	}

		void HideAd(CCWindow window)
    	{
    		_visible = false ;
    		MoveOffScreen();
    	}
    }
}