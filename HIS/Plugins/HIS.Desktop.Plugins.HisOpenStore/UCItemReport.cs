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

namespace HIS.Desktop.Plugins.HisOpenStore
{
    public partial class UCItemReport : UserControl
    {
        List<object> listArgs = new List<object>();
        public event EventHandler _Click;
        long roomID = 0;
        long roomTypeID = 0;
        public int widthParent;
        public int heightParent;
        //Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        public UCItemReport(UCHisOpenStore UCHis)
        {
            InitializeComponent();
            this.roomID = UCHis.roomID;
            this.roomTypeID = UCHis.roomTypeID;
        }
        public UCItemReport(Inventec.Desktop.Common.Modules.Module currentModule, int widthForm, int heightForm)
        {
            InitializeComponent();
            this.roomID = currentModule.RoomId;
            this.roomTypeID = currentModule.RoomTypeId;

            widthParent = widthForm;
            heightParent = heightForm;
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.OpensourceHisStore", roomID, roomTypeID, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
           
        }

        private void picReport_Click(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.OpensourceHisStore", roomID, roomTypeID, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCItemReport_Load(object sender, EventArgs e)
        {
            try
            {
                if (widthParent < 1914 && heightParent < 831)
                {
                    label1.Size = new Size(550 * widthParent / 1914, 40);
                    picReport.Size = new Size(550 * widthParent / 1914, 300 * heightParent / 831);
                    label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
                }
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
           
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisOpenStore.Resources.Lang", typeof(UCItemReport).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                //this.lblTitle.Text = Inventec.Common.Resource.Get.Value("UCItemReport.lblTitle.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void picReport_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    picReport.ContextMenuStrip = new ContextMenuStrip();
                    ContextMenuStrip menu = (sender as Control).ContextMenuStrip;
                    if (menu != null)
                    {
                        menu.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
