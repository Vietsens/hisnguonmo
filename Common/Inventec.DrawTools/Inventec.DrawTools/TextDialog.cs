/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventec.DrawTools
{
	public partial class TextDialog : Form
	{
		public TextDialog()
		{
			InitializeComponent();
		}

		private string _text;

		public string TheText
		{
			get { return _text; }
			set { _text = value; }
		}

		private Font _font;

		public Font TheFont
		{
			get { return _font; }
			set { _font = value; }
		}

		private Color _color;

		public Color TheColor
		{
			get { return _color; }
			set { _color = value; }
		}

		private void TextDialog_Load(object sender, EventArgs e)
		{
			_color = Color.Black;
			_font = txtTheText.Font;
			_text = "";
		}

		private void btnFont_Click(object sender, EventArgs e)
		{
			dlgFont.AllowSimulations = true;
			dlgFont.AllowVectorFonts = true;
			dlgFont.AllowVerticalFonts = true;
			dlgFont.MaxSize = 200;
			dlgFont.MinSize = 4;
			dlgFont.ShowApply = false;
			dlgFont.ShowColor = true;
			dlgFont.ShowEffects = true;
			if (dlgFont.ShowDialog() ==
			    DialogResult.OK)
			{
				_font = dlgFont.Font;
				_color = dlgFont.Color;
				txtTheText.Font = _font;
				txtTheText.ForeColor = _color;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			_text = txtTheText.Text;
		}
	}
}
