//#define USE_WEAK_HANDLERS

using System;
using CocosSharp;
using System.Collections.Generic;
using CocosDenshion;
using System.Reflection;

namespace ManicMiner
{
	static partial class Helper
	{
		public static Action<CCEvent> Weak(this Action<CCEvent> handler)
		{
#if USE_WEAK_HANDLERS
			if (handler == null)
				return null;
			var t = handler.Target ;

			if (t is WeakReference)
				return handler;

			var method = handler.Method;
			var target = new WeakReference(t);

			t = null; // Just to be sure we don't hold a reference

			return (evt) =>
			{
				var o = target.Target;
				if (o != null && method != null)
					method.Invoke(o, new object[] { evt }); 
				else
					return;
			};
#else
			return handler;
#endif
		}

		public static Action<List<CCTouch>, CCEvent> Weak(this Action<List<CCTouch>, CCEvent> handler)
		{
#if USE_WEAK_HANDLERS
			if (handler == null)
				return null;
			var t = handler.Target ;

			if (t is WeakReference)
				return handler;

			var method = handler.Method;
			var target = new WeakReference(t);

			//t = null; // Just to be sure we don't hold a reference

			return (touches, evt) =>
			{
				var o = target.Target;
				if (o != null && method != null)
					method.Invoke(o, new object[] { touches, evt }); 
				else
					return;
			};
#else
			return handler;
#endif
		}

		public static Action<CCTouch, CCEvent> Weak(this Action<CCTouch, CCEvent> handler)
		{
#if USE_WEAK_HANDLERS
			if (handler == null)
				return null;
			var t = handler.Target ;

			if (t is WeakReference)
				return handler;

			var method = handler.Method;
			var target = new WeakReference(t);

			t = null; // Just to be sure we don't hold a reference

			return (touch, evt) =>
			{
				var o = target.Target;
				if (o != null && method != null)
					method.Invoke(o, new object[] { touch, evt }); 
				else
					return;
			};
#else
			return handler;
#endif
		}

		public static Func<CCTouch, CCEvent, bool> Weak(this Func<CCTouch, CCEvent, bool> handler)
		{
#if USE_WEAK_HANDLERS
			if (handler == null)
				return null;
			var t = handler.Target ;

			if (t is WeakReference)
				return handler;

			var method = handler.Method;
			var target = new WeakReference(t);

			t = null; // Just to be sure we don't hold a reference

			return (touch, evt) =>
			{
				var o = target.Target;
				if (o != null && method != null)
					return (bool) method.Invoke(o, new object[] { touch, evt }); 
				else
					return false;
			};
#else
			return handler;
#endif
		}

		public static void AddTouchListener(this CCNode layer, Action<List<CCTouch>, CCEvent> handler)
		{
			var touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesEnded = handler.Weak();
			layer.AddEventListener(touchListener, layer);
		}

		public static void AddTouchListener(this CCNode layer, CCNode node, Action<CCEvent> endHandler)
		{
			layer.AddTouchListener(node, null, endHandler);
		}

		public static void AddTouchListener(this CCNode layer, CCNode node, Action<CCEvent> startHandler, Action<CCEvent> endHandler)
		{
			var listener = new CCEventListenerTouchOneByOne();

			startHandler = startHandler.Weak();
			endHandler   = endHandler.Weak();

			listener.OnTouchBegan = (touch, evt) =>
			{
				bool ok = node.BoundingBox.ContainsPoint(touch.Location);

				if (ok && startHandler != null)
					startHandler(evt);

				return ok;
			};

			listener.OnTouchEnded = (touch, evt) => 
			{
				endHandler(evt);
			};

			layer.AddEventListener(listener, node);
		}
	}
}

