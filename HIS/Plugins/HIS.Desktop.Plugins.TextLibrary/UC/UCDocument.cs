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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TextLibrary.UC
{
    public partial class UCDocument : UserControl
    {
        public UCDocument()
        {
            InitializeComponent();
        }
        public void SetText(string text)
        {
            try
            {
                   this.txtContent.Text = text;   
            }
            catch (Exception ex)
            {
                         Inventec.Common.Logging.LogSystem.Error(ex);  
            }
        }

        public void SetRtfText( string text) 
        { 
            try
            {
                this.txtContent.RtfText = text;              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        public string getRtfText()
        {
            try
            {
                return txtContent.RtfText;
            }
            catch (Exception ex)
            {
                return "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetFont(Font font)
        {
            try
            {
                txtContent.Appearance.Text.Font = font;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool getLandscape(int i) 
        {
            try
            {
                return txtContent.Document.Sections[i].Page.Landscape;
            }
            catch (Exception ex)
            {
                return false;
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        public int getWidth()
        {
            try
            {
                return txtContent.Width;
            }
            catch (Exception ex)
            {
                return 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public float getPageHeight(int i) 
        {
            try
            {
                return txtContent.Document.Sections[i].Page.Height;
            }
            catch (Exception ex)
            {
                return 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public float getPageWidth(int i)
        {
            try
            {
                return txtContent.Document.Sections[i].Page.Width;
            }
            catch (Exception ex)
            {
                return 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetZoomFactor(float zoom)
        {
            try
            {
                txtContent.ActiveView.ZoomFactor = zoom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        public void Focus() 
        {
            try
            {
                txtContent.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }
    }
}
