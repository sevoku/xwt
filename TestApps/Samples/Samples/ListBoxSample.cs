// 
// ListBoxSample.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc
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
using Xwt;
using Xwt.Drawing;

namespace Samples
{
	public class ListBoxSample: VBox
	{
		DataField<string> name = new DataField<string> ();
		DataField<Image> icon = new DataField<Image> ();
		
		public ListBoxSample ()
		{
			var filterBox = new HBox ();
			var txtFilter = new TextEntry ();
			filterBox.PackStart (new Label ("Filter:"));
			filterBox.PackStart (txtFilter, true);
			PackStart (filterBox);

			// Default list box
			
			ListBox list = new ListBox ();
			
			for (int n=0; n<100; n++)
				list.Items.Add ("Value " + n);
			
			PackStart (list, true);
			
			// Custom list box
			
			ListBox customList = new ListBox ();
			ListStore store = new ListStore (name, icon);
			customList.DataSource = store;
			customList.Views.Add (new ImageCellView (icon));
			customList.Views.Add (new TextCellView (name));
			
			var png = Image.FromResource (typeof(App), "class.png");
			
			for (int n=0; n<100; n++) {
				var r = store.AddRow ();
				store.SetValue (r, icon, png);
				store.SetValue (r, name, "Value " + n);
			}
			PackStart (customList, true);

			txtFilter.Changed += (sender, e) => {
				if (String.IsNullOrEmpty (txtFilter.Text))
					list.Filter = null;
				else {
					if (list.Filter == null)
						list.Filter = row => {
						if (String.IsNullOrEmpty (txtFilter.Text))
							return true;
						if (store.GetValue (row, name).Contains (txtFilter.Text))
							return true;
						return false;
					};
					list.Refilter();
				}
			};
		}	
	}
}

