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
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExecuteRoom.Resources.Lang", typeof(HIS.Desktop.Plugins.ExecuteRoom.UCExecuteRoom__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnServiceReqList.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnServiceReqList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnUnStart.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnUnStart.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTreatmentHistory2.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnTreatmentHistory2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSummaryInforTreatmentRecords.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnSummaryInforTreatmentRecords.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAggrHospitalFees.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnAggrHospitalFees.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTreatmentHistory.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnTreatmentHistory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRoomTran.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnRoomTran.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDepositReq.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnDepositReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnBordereau.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnBordereau.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExecute.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnExecute.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientTypeName.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciPatientTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup6.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlGroup6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroupTreeSereServ.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlGroupTreeSereServ.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup4.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlGroup4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTRANGTHAI_IMG.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColTRANGTHAI_IMG.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit1.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.repositoryItemPictureEdit1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPRIORIRY_DISPLAY.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColPRIORIRY_DISPLAY.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPRIORIRY_DISPLAY.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColPRIORIRY_DISPLAY.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit2.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.repositoryItemPictureEdit2.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNUM_ORDER.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColNUM_ORDER.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColServiceReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColVirPatientName.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColVirPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRequestDate.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColRequestDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceReqTypeName.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColServiceReqTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcolSERVICE_REQ_STT_ID.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gcolSERVICE_REQ_STT_ID.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcolSERVICE_REQ_STT_NAME.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gcolSERVICE_REQ_STT_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchKey.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExecuteRoom.txtSearchKey.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreatefrom.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciCreatefrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreateTo.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciCreateTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
