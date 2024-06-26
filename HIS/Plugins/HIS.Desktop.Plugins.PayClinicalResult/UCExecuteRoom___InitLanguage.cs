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
using System.Collections;
using Inventec.Desktop.Plugins.ExecuteRoom.Delegate;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.UC.TreeSereServ7;
using HIS.Desktop.Plugins.PayClinicalResult.Base;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using AutoMapper;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraBars;
using Inventec.Desktop.Plugins.ExecuteRoom;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.PayClinicalResult
{
    public partial class UCExecuteRoom : HIS.Desktop.Utility.UserControlBase
    {
        public void InitLanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLangManager.InitResourceLanguageManager();
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl1.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.btnServiceReqList.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnServiceReqList.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.btnUnStart.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnUnStart.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.btnTreatmentHistory.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnTreatmentHistory.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.btnRoomTran.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnRoomTran.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.btnDepositReq.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnDepositReq.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.btnBordereau.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnBordereau.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.btnExecute.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnExecute.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl3.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl5.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn3.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn4.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn5.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn6.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn7.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn8.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl4.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem4.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem13.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem14.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem15.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem16.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                //this.lciPatientTypeName.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciPatientTypeName.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.layoutControlGroup6.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlGroup6.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.layoutControlGroupTreeSereServ.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlGroupTreeSereServ.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.layoutControlGroup4.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlGroup4.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl2.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn2.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.grdColTRANGTHAI_IMG.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColTRANGTHAI_IMG.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit1.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.repositoryItemPictureEdit1.NullText", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn1.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.grdColPRIORIRY_DISPLAY.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColPRIORIRY_DISPLAY.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.grdColPRIORIRY_DISPLAY.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColPRIORIRY_DISPLAY.ToolTip", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit2.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.repositoryItemPictureEdit2.NullText", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.grdColNUM_ORDER.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColNUM_ORDER.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                //this.grdColServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColServiceReqCode.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.grdColTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColTreatmentCode.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.grdColVirPatientName.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColVirPatientName.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.grdColRequestDate.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColRequestDate.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.grdColServiceReqTypeName.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColServiceReqTypeName.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.grdColPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColPatientCode.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.gcolSERVICE_REQ_STT_ID.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gcolSERVICE_REQ_STT_ID.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.gcolSERVICE_REQ_STT_NAME.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gcolSERVICE_REQ_STT_NAME.Caption", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnFind.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.txtSearchKey.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExecuteRoom.txtSearchKey.Properties.NullValuePrompt", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.lciCreatefrom.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciCreatefrom.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.lciCreateTo.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciCreateTo.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                this.repositoryItemButton_CallPatient.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.CallPatientTooltip.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());

               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
