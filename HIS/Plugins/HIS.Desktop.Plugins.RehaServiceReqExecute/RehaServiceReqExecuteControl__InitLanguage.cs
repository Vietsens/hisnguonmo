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
using AutoMapper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.RehaServiceReqExecute.Base;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
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

namespace HIS.Desktop.Plugins.RehaServiceReqExecute
{
    public partial class RehaServiceReqExecuteControl : HIS.Desktop.Utility.UserControlBase
    {
        private void InitLanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLangManager.InitResourceLanguageManager();

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControl1.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.txtIcdExtraName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.txtIcdExtraName.Properties.NullValuePrompt", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.btnAssignPre.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.btnAssignPre.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.btnAssignService.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.btnAssignService.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.btnThemTTTap.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.btnThemTTTap.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.cboPrint.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.cboPrint.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.btnSave.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.btnFinish.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.btnFinish.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.labelControl2.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.labelControl1.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.grdColllSTT.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.grdColllSTT.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.grdColllServiceCode.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.grdColllServiceCode.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.grdColllServiceName.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.grdColllServiceName.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.grdColllDonvitinh.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.grdColllDonvitinh.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.grdColllTechnicalName.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.grdColllTechnicalName.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.grdColllTime.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.grdColllTime.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.grdColllSoLan.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.grdColllSoLan.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.gridColumn9.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.grdColllObjectOfPayment.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.grdColllObjectOfPayment.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumnCreator.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.gridColumnCreator.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.grdCollSTT.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.grdCollSTT.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.grdCollTechnicalCode.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.grdCollTechnicalCode.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.grdCollTechnicalName.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.grdCollTechnicalName.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.grdCollDonViTapLuyen.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.grdCollDonViTapLuyen.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.grdCollCount.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.grdCollCount.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumnChonTap.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.gridColumnChonTap.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.chkIcds.Properties.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.chkIcds.Properties.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.cboIcds.Properties.NullText = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.cboIcds.Properties.NullText", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.gridColumn1.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.gridColumn2.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.gridColumn3.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.gridColumn4.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.gridColumn5.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.gridColumn6.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.gridColumn7.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.gridColumn8.Caption", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.lciEditIcd.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControlItem5.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                //this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControlItem13.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                //this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControlItem14.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                //this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControlItem15.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                //this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControlItem16.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.lciTrieuChungTruoc.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControlItem3.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.lciHoHapTruoc.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControlItem8.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.lciDienTimTruoc.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControlItem9.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.lciTrieuChungSau.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControlItem10.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.lciHoHapSau.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControlItem11.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.lciDienTimSau.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControlItem12.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.lciBenhChinh.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControlItem17.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                this.lciBenhPhu.Text = Inventec.Common.Resource.Get.Value("RehaServiceReqExecuteControl.layoutControlItem20.Text", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
