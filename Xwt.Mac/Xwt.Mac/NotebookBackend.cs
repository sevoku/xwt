// 
// NotebookBackend.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using MonoMac.AppKit;
using Xwt.Backends;
using System.Collections.Generic;
using System.Drawing;


namespace Xwt.Mac
{
	public class NotebookBackend: ViewBackend<NSTabView,IWidgetEventSink>, INotebookBackend
	{
		public NotebookBackend ()
		{
			ViewObject = new TabView ();
			Widget.AutoresizesSubviews = true;
		}
		
		public override void EnableEvent (object eventId)
		{
			if (eventId is NotebookEvent) {
				NotebookEvent ev = (NotebookEvent) eventId;
				if (ev == NotebookEvent.CurrentTabChanged) {
					Widget.WillSelect += HandleWidgetWillSelect;
				}
			}
			base.EnableEvent (eventId);
		}
		
		public override void DisableEvent (object eventId)
		{
			if (eventId is NotebookEvent) {
				NotebookEvent ev = (NotebookEvent) eventId;
				if (ev == NotebookEvent.CurrentTabChanged) {
					Widget.WillSelect -= HandleWidgetWillSelect;
				}
			}
			base.DisableEvent (eventId);
		}

		void HandleWidgetWillSelect (object sender, NSTabViewItemEventArgs e)
		{
			((INotebookEventSink)EventSink).OnCurrentTabChanged ();
		}

		#region INotebookBackend implementation
		public void Add (IWidgetBackend widget, NotebookTab tab)
		{
			//NSTabViewItem item = new NSTabViewItem ();
			TabViewItem item = new TabViewItem ();
			item.Label = tab.Label;
			if (!tab.Image.IsNull)
				item.Image = tab.Image.ToNSImage ();
			item.View = GetWidgetWithPlacement (widget);
			Widget.Add (item);
		}

		public void Remove (IWidgetBackend widget)
		{
			var v = GetWidgetWithPlacement (widget);
			var t = FindTab (v);
			if (t != null) {
				Widget.Remove (t);
				RemoveChildPlacement (t.View);
			}
		}
		
		public void UpdateLabel (NotebookTab tab, string hint)
		{
			IWidgetBackend widget = (IWidgetBackend) Toolkit.GetBackend (tab.Child);
			var v = GetWidget (widget);
			var t = FindTab (v);
			if (t != null) {
				if (!tab.Image.IsNull)
					t.Image = tab.Image.ToNSImage ();
				else
					t.Image = null;
				t.Label = tab.Label;
			}
		}
		
		public int CurrentTab {
			get {
				return Widget.IndexOf (Widget.Selected);
			}
			set {
				Widget.SelectAt (value);
			}
		}

		public Xwt.NotebookTabOrientation TabOrientation {
			get {
				NotebookTabOrientation tabPos = NotebookTabOrientation.Top;
				switch (Widget.TabViewType) {
				case NSTabViewType.NSBottomTabsBezelBorder:
					tabPos = NotebookTabOrientation.Bottom;
					break;
				case NSTabViewType.NSLeftTabsBezelBorder:
					tabPos = NotebookTabOrientation.Left;
					break;
				case NSTabViewType.NSRightTabsBezelBorder:
					tabPos = NotebookTabOrientation.Right;
					break;
				}
				return tabPos;
			}
			set {
				NSTabViewType type = NSTabViewType.NSTopTabsBezelBorder;
				switch (value) {
				case NotebookTabOrientation.Bottom:
					type = NSTabViewType.NSBottomTabsBezelBorder;
					break;
				case NotebookTabOrientation.Left:
					type = NSTabViewType.NSLeftTabsBezelBorder;
					break;
				case NotebookTabOrientation.Right:
					type = NSTabViewType.NSRightTabsBezelBorder;
					break;
				}
				Widget.TabViewType = type;
			}
		}

		public virtual bool ExpandTabLabels { get; set; }

		#endregion
		
		TabViewItem FindTab (NSView v)
		{
			foreach (var t in Widget.Items.Cast<TabViewItem>().DefaultIfEmpty()) {
				if (t != null && t.View == v)
					return t;
			}
			return null;
		}
	}
	
	class TabView: NSTabView, IViewObject
	{
		public ViewBackend Backend { get; set; }
		public NSView View {
			get { return this; }
		}
	}

	class TabViewItem : NSTabViewItem
	{
		public NSImage Image { get; set; } 

		public override void DrawLabel (bool shouldTruncateLabel, System.Drawing.RectangleF labelRect)
		{
			if (Image != null) {
				Image.Draw (new RectangleF(labelRect.X, labelRect.Y, labelRect.Height, labelRect.Height), 
					new RectangleF(0, 0, Image.Size.Width, Image.Size.Height), NSCompositingOperation.SourceOver, 1, true, null);
				labelRect.X += labelRect.Height + 3;
				labelRect.Width -= labelRect.Height + 3;
			}
			base.DrawLabel (shouldTruncateLabel, labelRect);
		}

		public override SizeF SizeOfLabel (bool computeMin)
		{
			var labelSize = base.SizeOfLabel (computeMin);
			if (Image != null)
				labelSize.Width += labelSize.Height + 3;
			return labelSize;
		}
	}
}

