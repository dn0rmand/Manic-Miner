using System;
using CocosSharp;
using System.Collections.Generic;
using CocosDenshion;
using System.Reflection;

namespace ManicMiner
{
	public static partial class Helper
	{
		public static void PreDetach(this CCNode node)
		{
			if (node == null)
				return ;
			
			if (node.ChildrenCount > 0)
				foreach(CCNode n in node.Children)
					n.PreDetach();

			node.UnscheduleAll();
			node.RemoveEventListeners(true);
			node.StopAllActions();
		}

		public static void PostDetach(this CCNode node)
		{
			if (node == null)
				return ;
			
			if (node.ChildrenCount > 0)
				foreach(CCNode n in node.Children)
					n.PostDetach();

			node.RemoveAllChildren(true);
			node.Cleanup();
			node.RemoveFromParent(false);

			node.Dispose();
		}

		public static void SwitchScene(this CCDirector director, CCScene scene)
		{
			var currentScene = director.RunningScene;

			if (currentScene != null)
			{
				currentScene.PreDetach();

				var transition = scene; // new CCTransitionScene(1f, scene);
				director.ReplaceScene(transition);

				currentScene.PostDetach();
			}
			else
				director.ReplaceScene(scene);
		}

		[System.Diagnostics.Conditional("DEBUG")]
		public static void DumpGCSize()
		{
			long size = GC.GetTotalMemory(true);
			Console.WriteLine("{0:##.##0}",
					size >= 1073741824 		? decimal.Divide(size, 1073741824) + " GB" :
					size >= 1048576 		? decimal.Divide(size, 1048576) + " MB" :
					size >= 1024 			? decimal.Divide(size, 1024) + " KB" :
					size > 0 & size < 1024 	? size + "Bytes" : "0 Bytes");
		}
	}
}

