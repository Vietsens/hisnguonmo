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
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.WorkPlace
{
    public partial class UCWorkPlaceTextbox1 : DevExpress.XtraEditors.XtraUserControl
    {
        DelegateFocusMoveout focusMoveout;
        public UCWorkPlaceTextbox1(DelegateFocusMoveout focusMoveout)
        {
            InitializeComponent();
            this.focusMoveout = focusMoveout;
            this.SetCaptionByLanguageKey();
        }

        public string GetValue()
        {
            string result = "";
            try
            {
                result = txtWorkPlace1.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        public void FocusControl()
        {
            try
            {
                txtWorkPlace1.Focus();
                txtWorkPlace1.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValue(object data)
        {
            try
            {
                txtWorkPlace1.Text = (string)data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtWorkPlace1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.focusMoveout != null)
                    {
                        this.focusMoveout();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCWorkPlaceTextbox1
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.WorkPlace.Resources.Lang", typeof(UCWorkPlaceTextbox1).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCWorkPlaceTextbox1.layoutControl1.Text", His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UCWorkPlaceTextbox1.layoutControlItem1.Text", His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
