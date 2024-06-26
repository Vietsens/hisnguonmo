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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.SwapService.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SwapService
{
    public partial class frmSwapService :  HIS.Desktop.Utility.FormBase
    {
        public void InitLanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLangManager.InitResourceLanguageManager();

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSwapService.layoutControl1.Text", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
                this.gridViewSwapService.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmSwapService.gridViewSwapService.OptionsFind.FindNullPrompt", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
                this.gridColServiceCode.Caption = Inventec.Common.Resource.Get.Value("frmSwapService.gridColServiceCode.Caption", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
                this.gridColServiceName.Caption = Inventec.Common.Resource.Get.Value("frmSwapService.gridColServiceName.Caption", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
                this.gridColAmount.Caption = Inventec.Common.Resource.Get.Value("frmSwapService.gridColAmount.Caption", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
                this.gridColPrice.Caption = Inventec.Common.Resource.Get.Value("frmSwapService.gridColPrice.Caption", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
                this.gridCol.Caption = Inventec.Common.Resource.Get.Value("frmSwapService.gridCol.Caption", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
                this.repositoryItemcboPatientType.NullText = Inventec.Common.Resource.Get.Value("frmSwapService.repositoryItemcboPatientType.NullText", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
                this.gridColExpend.Caption = Inventec.Common.Resource.Get.Value("frmSwapService.gridColExpend.Caption", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
                this.gridColIsOutParentFee.Caption = Inventec.Common.Resource.Get.Value("frmSwapService.gridColIsOutParentFee.Caption", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmSwapService.gridColumn1.Caption", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
                this.btnSwapService.Text = Inventec.Common.Resource.Get.Value("frmSwapService.btnSwapService.Text", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmSwapService.txtKeyword.Properties.NullValuePrompt", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSwapService.Text", ResourceLangManager.LanguageUCSwapService, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
