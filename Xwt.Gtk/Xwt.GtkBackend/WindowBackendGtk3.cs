//
// WindowBackendGtk3.cs
//
// Author:
//       Vsevolod Kukol <sevo@sevo.org>
//
// Copyright (c) 2015 Vsevolod Kukol
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
using Xwt.Backends;
using Gtk;

namespace Xwt.GtkBackend
{
	public class WindowBackendGtk3: WindowBackend, IWindowFrameBackend
	{
		Gtk.HeaderBar titlebar;

		public override void Initialize ()
		{
			base.Initialize ();

			if (GtkWorkarounds.GtkMajorVersion >= 3 && GtkWorkarounds.GtkMinorVersion >= 10) {
				titlebar = new Gtk.HeaderBar ();
				titlebar.ShowCloseButton = true;
				Window.SetTitlebar (titlebar);
				titlebar.Show ();
			}
		}

		public override void GetMetrics (out Size minSize, out Size decorationSize)
		{
			if (titlebar != null) {
				var hs = titlebar.SizeRequest ();
				minSize = new Size (hs.Width, 0);
				decorationSize = new Size (0, hs.Height);
			}
			else {
				minSize = decorationSize = Size.Zero;
			}
		}
	}
}

