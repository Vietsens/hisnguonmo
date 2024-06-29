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
using HIS.Desktop.Utility;
using DevExpress.XtraEditors;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.HisOpenStore
{
    public partial class UCHisOpenStore : UserControlBase
    {
        public long roomID = 0;
        public long roomTypeID = 0;
        bool avtive = true;
        //private UCItemFeature UCVaItemFeature;
        //private UCItemOrder UCVaItemOrder;
        //private UCItemReport UCVaItemReport;
        //private UCItemService UCVaItemService; 
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        public UCHisOpenStore(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            this.currentModule = module;
            this.roomID = module.RoomId;
            this.roomTypeID = module.RoomTypeId;
        }

        private void UCHisOpenStore_Load(object sender, EventArgs e)
        {
            SetCaptionByLanguageKey();
           
        }
      
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisOpenStore.Resources.Lang", typeof(UCHisOpenStore).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.tileControlMain.Text = Inventec.Common.Resource.Get.Value("UCHisOpenStore.tileControlMain.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tileControlMain_Resize(object sender, EventArgs e)
        {
            try
            {
                tileControlMain.Controls.Clear();
                if (avtive == true)
                {
                    int widthForm = tileControlMain.Width;
                    int heightForm = tileControlMain.Height;

                    UCItemFeature UCVaItemFeature = new UCItemFeature(widthForm, heightForm);
                    UCItemOrder UCVaItemOrder = new UCItemOrder(widthForm, heightForm);
                    UCItemReport UCVaItemReport = new UCItemReport(currentModule, widthForm, heightForm);
                    UCItemService UCVaItemService = new UCItemService(widthForm, heightForm);
                    UCHisOpenStore UCMain = new UCHisOpenStore(currentModule);
                    
                    if (widthForm < 1914 && heightForm < 831)
                    {
                        UCVaItemReport.Size = new Size(550 * widthForm / 1914, 350 * heightForm / 831);
                        UCVaItemOrder.Size = new Size(650 * widthForm / 1914, 380 * heightForm / 831);
                        UCVaItemService.Size = new Size(550 * widthForm / 1914, 350 * heightForm / 831);
                        UCVaItemFeature.Size = new Size(550 * widthForm / 1914, 350 * heightForm / 831);

                        //UCVaItemReport.Location = new Point(100, 15);
                        //UCVaItemFeature.Location = new Point(UCVaItemReport.Location.X + 500, UCVaItemReport.Location.Y);
                        //UCVaItemService.Location = new Point(UCVaItemReport.Location.X - 17, UCVaItemReport.Location.Y + 240);
                        //UCVaItemOrder.Location = new Point(UCVaItemService.Location.X + 600, UCVaItemService.Location.Y - 5);

                        UCVaItemReport.Left = 100;
                        UCVaItemReport.Top = 15;
                        UCVaItemFeature.Left = 900 * widthForm / 1914;
                        UCVaItemFeature.Top = 15;
                        UCVaItemService.Left = 100;
                        UCVaItemService.Top =  400* heightForm / 814;
                        UCVaItemOrder.Left = 1050 * widthForm / 1914;
                        UCVaItemOrder.Top = 430 * heightForm / 814;

                        tileControlMain.Controls.Add(UCVaItemOrder);
                        tileControlMain.Controls.Add(UCVaItemService);
                        tileControlMain.Controls.Add(UCVaItemFeature);
                        tileControlMain.Controls.Add(UCVaItemReport);
                    }
                    else
                    {
                        UCVaItemOrder.Size = new Size(700, 375);
                        tileControlMain.Controls.Add(UCVaItemOrder);

                        UCVaItemOrder.Size = new Size(700, 375);
                        // UCVaItemReport.Margin = new Padding(500, 15, 100, 15);
                        UCVaItemReport.Left = 100;
                        UCVaItemReport.Top = 15;
                        UCVaItemFeature.Left = 900;
                        UCVaItemFeature.Top = 15;
                        UCVaItemService.Left = 100;
                        UCVaItemService.Top = 400;
                        UCVaItemOrder.Left = 1050;
                        UCVaItemOrder.Top = 450;

                    }

                    tileControlMain.Controls.Add(UCVaItemService);
                    tileControlMain.Controls.Add(UCVaItemOrder);
                    tileControlMain.Controls.Add(UCVaItemFeature);
                    tileControlMain.Controls.Add(UCVaItemReport);
                }
                avtive = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
