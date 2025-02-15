﻿/*
Copyright (c) 2018, John Lewin
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
*/

using System;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrintLibrary;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public class SearchableSectionWidget : SectionWidget
	{
		public event EventHandler<StringEventArgs> SearchInvoked;

		private TextEditWithInlineCancel searchPanel;

		public SearchableSectionWidget(string sectionTitle, GuiWidget sectionContent, ThemeConfig theme, int headingPointSize = -1, bool expandingContent = true, bool expanded = true, string serializationKey = null, bool defaultExpansion = false, bool setContentVAnchor = true, string emptyText = null)
			: base(sectionTitle, sectionContent, theme, theme.CreateSearchButton(), headingPointSize, expandingContent, expanded, serializationKey, defaultExpansion, setContentVAnchor)
		{
			var headerRow = this.Children.First();

			searchPanel = new TextEditWithInlineCancel(theme, emptyText)
			{
				Visible = false,
				BackgroundColor = theme.TabBarBackground,
				MinimumSize = new Vector2(0, headerRow.Height)
			};

			searchPanel.TextEditWidget.Margin = new BorderDouble(3, 0);

			searchPanel.TextEditWidget.ActualTextEditWidget.EnterPressed += (s, e) =>
			{
				var filter = searchPanel.TextEditWidget.Text.Trim();

				this.SearchInvoked?.Invoke(this, new StringEventArgs(filter));

				searchPanel.Visible = false;
				headerRow.Visible = true;
				searchPanel.TextEditWidget.Text = "";
			};

			searchPanel.ResetButton.Click += (s, e) =>
			{
				searchPanel.Visible = false;
				headerRow.Visible = true;
				searchPanel.TextEditWidget.Text = "";
			};

			var searchButton = this.rightAlignedContent as GuiWidget;
			searchButton.Click += (s, e) =>
			{
				searchPanel.Visible = true;
				headerRow.Visible = false;
			};

			this.AddChild(searchPanel, 1);
		}
	}
}