using System;
using System.Collections.Generic;
using CocosSharp;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;
using CocosDenshion;

namespace ManicMiner
{
	public abstract class BaseGameLayer : CCLayerColor
    {
    	bool 	_showAd;
    	string 	_musicName;

		public BaseGameLayer(string musicName, bool showAd) : base ()
        {
        	_musicName	= musicName;
        	_showAd 	= showAd;
			Color 		= CCColor3B.Black; // (255, 255, 255);
			Opacity 	= 255;
			IsSerializable = false;
        }

		public override void OnEnter ()
		{
			base.OnEnter ();
			if (! string.IsNullOrEmpty(_musicName))
				Helper.PlayMusic(_musicName);
			if (_showAd)
				ManicMinerApplicationDelegate.ShowAd(this.Window);
			else
				ManicMinerApplicationDelegate.HideAd(this.Window);	
		}

		protected virtual void StopMusic()
		{
			Helper.StopMusic();
		}

		public override void OnExit ()
		{
			StopMusic();
			base.OnExit();
		}

		public void GotoPianoScene()
		{
			Window.DefaultDirector.SwitchScene(GamePianoLayer.CreateScene(Window));
		}

		protected static CCScene InnerCreateScene(CCWindow mainWindow, BaseGameLayer layer)
        {
			CCSize winSize = new CCSize(256,192); //320 - 64, 240 - 44);
            mainWindow.SetDesignResolutionSize(winSize.Width, winSize.Height, CCSceneResolutionPolicy.ExactFit);

            var scene = new CCScene (mainWindow);
            scene.AddChild (layer);
            return scene;
        }
    }
}