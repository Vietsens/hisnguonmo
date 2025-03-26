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
using System.Globalization;
using System.Windows.Forms;

namespace Inventec.DrawTools
{
	internal partial class PropertiesDialog : Form
	{
		public PropertiesDialog()
		{
			InitializeComponent();
		}

		private GraphicsProperties properties;
		private const string undefined = "??";
		private const int maxWidth = 5;

		public GraphicsProperties Properties
		{
			get { return properties; }
			set { properties = value; }
		}

		private void PropertiesDialog_Load(object sender, EventArgs e)
		{
			InitControls();
			SetColor();
			SetPenWidth();
		}

		private void InitControls()
		{
			for (int i = 1; i <= maxWidth; i++)
			{
				cmbPenWidth.Items.Add(i.ToString(CultureInfo.InvariantCulture));
			}
		}

		private void SetColor()
		{
			if (properties.Color.HasValue)
				lblColor.BackColor = properties.Color.Value;
			else
				lblColor.Text = undefined;
		}

		private void SetPenWidth()
		{
			if (properties.PenWidth.HasValue)
			{
				int penWidth = properties.PenWidth.Value;

				if (penWidth < 1)
					penWidth = 1;

				if (penWidth > maxWidth)
					penWidth = maxWidth;

				label2.Text = penWidth.ToString(CultureInfo.InvariantCulture);
				cmbPenWidth.SelectedIndex = penWidth - 1;
			}
			else
			{
				label2.Text = undefined;
			}
		}

		private void ReadValues()
		{
			if (cmbPenWidth.Text != undefined)
			{
				properties.PenWidth = cmbPenWidth.SelectedIndex + 1;
			}

			if (lblColor.Text.Length == 0)
			{
				properties.Color = lblColor.BackColor;
			}
		}

		private void cmbPenWidth_SelectedIndexChanged(object sender, EventArgs e)
		{
			int width = cmbPenWidth.SelectedIndex + 1;
			lblPenWidth.Text = width.ToString(CultureInfo.InvariantCulture);
		}

		private void btnSelectColor_Click(object sender, EventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			dlg.Color = lblColor.BackColor;

			if (dlg.ShowDialog(this) ==
			    DialogResult.OK)
			{
				lblColor.BackColor = dlg.Color;
				lblColor.Text = "";
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			ReadValues();
			DialogResult = DialogResult.OK;
		}
	}
}
