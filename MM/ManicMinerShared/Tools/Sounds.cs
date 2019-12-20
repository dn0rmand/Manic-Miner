using System;
using CocosSharp;
using System.Collections.Generic;
using CocosDenshion;
using System.Reflection;

namespace ManicMiner
{
	static partial class Helper
	{
		static CCEffectPlayer[]	_effectPlayer = new CCEffectPlayer[] 
		{
			new CCEffectPlayer(),
			new CCEffectPlayer()
		};

		static CCEffectPlayer GetPlayer(bool main) { return _effectPlayer[main ? 0 : 1]; }

		public static void StopEffect(bool main = true)
		{
			var player = GetPlayer(main);
			if (player.Playing)
				player.Stop();
		}

		public static void PlayEffect(string name, bool main = true)
		{
			if (ManicMinerApplicationDelegate.SoundEnabled)
			{
				var player = GetPlayer(main);
				player.Stop();
				player.Open("Sounds/" + name, main ? 1 : 2);
				player.Play();
			}
		}

		public static void PlayMusic(string name, bool loop = true)
		{
			if (ManicMinerApplicationDelegate.MusicEnabled)
			{
				if (_lastMusic == name)
					CCSimpleAudioEngine.SharedEngine.ResumeBackgroundMusic();
				else
					CCSimpleAudioEngine.SharedEngine.PlayBackgroundMusic("Sounds/" + name, loop);
				CCSimpleAudioEngine.SharedEngine.BackgroundMusicVolume = 0.5f;
				_lastMusic = name;
			}
			_musicPaused = false;
		}

		public static void StopMusic()
		{
			//CCSimpleAudioEngine.SharedEngine.StopBackgroundMusic();
			CCSimpleAudioEngine.SharedEngine.PauseBackgroundMusic();

			StopEffect();
			StopEffect(false);

			_musicPaused = false;
			_soundPaused = false;
		}

		public static void ResumeMusic()
		{
			if (ManicMinerApplicationDelegate.MusicEnabled && _musicPaused)
				CCSimpleAudioEngine.SharedEngine.ResumeBackgroundMusic();

			if (ManicMinerApplicationDelegate.SoundEnabled && _soundPaused)
			{
				_effectPlayer[0].Resume();
				_effectPlayer[1].Resume();
			}

			_musicPaused = false;
			_soundPaused = false;
		}

		static string	_lastMusic   = null;
		static bool 	_musicPaused = false;
		static bool 	_soundPaused = false;

		public static void PauseMusic()
		{
			if (CCSimpleAudioEngine.SharedEngine.BackgroundMusicPlaying)
			{
				CCSimpleAudioEngine.SharedEngine.PauseBackgroundMusic();
				_musicPaused = true ;
			}
			else
				_musicPaused = false;

			if (_effectPlayer[0].Playing || _effectPlayer[1].Playing)
			{
				_soundPaused = true;
				_effectPlayer[0].Pause();
				_effectPlayer[1].Pause();
			}
			else
				_soundPaused = false;
		}
	}
}

