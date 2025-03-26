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
using System.Windows.Forms;

namespace Inventec.DrawTools
{
	internal partial class FrmAbout : Form
	{
		public FrmAbout()
		{
			InitializeComponent();
		}

		private void FrmAbout_Load(object sender, EventArgs e)
		{
			Text = "About " + Application.ProductName;

			lblText.Text = "Program: " + Application.ProductName + "\n" +
			               "Version: " + Application.ProductVersion;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
