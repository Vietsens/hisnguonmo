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
using HIS.Desktop.ADO;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TreatmentList.Properties;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentList
{
    public partial class UCTreatmentList : UserControlBase
    {
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCTreatmentList
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentList.Resources.Lang", typeof(UCTreatmentList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDelete.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnDelete.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnGuiHS.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnGuiHS.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnGuiHS.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnGuiHS.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn26.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn27.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn28.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn29.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn29.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn30.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn31.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn31.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn32.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn33.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn33.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn34.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn35.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn35.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn36.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn36.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn37.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn37.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn38.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn38.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn39.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn39.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn40.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn40.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn41.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn41.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn42.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn42.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn43.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn43.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn44.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn44.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn45.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn45.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn46.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn46.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn47.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn47.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn48.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn48.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn49.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn49.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSocialInsuranceNumber.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentList.txtSocialInsuranceNumber.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSocialInsuranceNumber.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.txtSocialInsuranceNumber.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboKhoaVaoVien.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentList.cboKhoaVaoVien.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnXuatXML.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnXuatXML.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnXuatXML.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnXuatXML.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrintfKSK.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnPrintfKSK.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrintfKSK.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnPrintfKSK.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnPrintHuongDan.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.BtnPrintHuongDan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentList.txtPatientName.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtOutCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentList.txtOutCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtInCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentList.txtInCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtStoreCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentList.txtStoreCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboContract.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentList.cboContract.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrintServiceReq.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnPrintServiceReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImportKsk.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnImportKsk.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImportKsk.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnImportKsk.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDienDieuTri.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentList.cboDienDieuTri.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboKieuBenhNhan.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentList.cboKieuBenhNhan.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTrangThai.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentList.cboTrangThai.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentList.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatient.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentList.txtPatient.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.n.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.n.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.AREA.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.AREA.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.radioButtonAtDepartment.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.radioButtonAtDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.InDepartment.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.InDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.radioButton1.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.radioButton1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCodeFind.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnCodeFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutCreateTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutInDateCome.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutInDateCome.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboOutOfHospital.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.cboOutOfHospital.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutOutTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutOutTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutOutDateCome.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutOutDateCome.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsRejectStore.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.chkIsRejectStore.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsRejectStore.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.chkIsRejectStore.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentList.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.navBarGroupCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupOutTime.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.navBarGroupOutTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupRejectStore.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.navBarGroupRejectStore.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarPaytientType.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.navBarPaytientType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatment.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentList.txtTreatment.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grOrder.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grOrder.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grOrder.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grStatus.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grStatus.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grStatus.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItembtnStatus.Caption.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.repositoryItembtnStatus.Caption.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grTimeLine.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grTimeLine.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grBedRoomIn.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grBedRoomIn.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grServicePackgeView.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grServicePackgeView.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grFixTreatment.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grFixTreatment.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grServiceReq.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grServiceReq.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grServiceReq.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grServiceReq.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grServiceReqList.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grServiceReqList.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grEditTreatment.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grEditTreatment.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grMergePatient.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grMergePatient.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grMergePatient.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grMergePatient.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grInfantInformation.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grInfantInformation.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grAccidentHurt.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grAccidentHurt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grAccidentHurt.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grAccidentHurt.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grFinish.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grFinish.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grTreatmentSumary.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grTreatmentSumary.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grTreatmentRecord.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grTreatmentRecord.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grTreatmentRecord.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grTreatmentRecord.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grFeeInfo.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grFeeInfo.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grUnfinish.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grUnfinish.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grSarPrintList.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grSarPrintList.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grPaySereServ.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grPaySereServ.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grPaySereServ.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grPaySereServ.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grTreatmentCode.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grTreatmentCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grPatientCode.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grPatientCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grVirPatientName.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grVirPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grVirPatientName.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grVirPatientName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grSocialInsuranceNumber.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grSocialInsuranceNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grSocialInsuranceNumber.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grSocialInsuranceNumber.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grTdlHeinCardNumber.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grTdlHeinCardNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grTdlHeinCardNumber.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grTdlHeinCardNumber.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grDobSt.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grDobSt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grDobSt.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grDobSt.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grGenderName.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grGenderName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grGenderName.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grGenderName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grVirAddress.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grVirAddress.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grVirAddress.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grVirAddress.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grInDepartmentName.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grInDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grIsChronicStr.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grIsChronicStr.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grInTimeStr.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grInTimeStr.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grInTimeStr.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grInTimeStr.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grInOrder.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grInOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grClinicalInTimeSt.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grClinicalInTimeSt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grClinicalInTimeSt.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grClinicalInTimeSt.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grOutTimeSt.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grOutTimeSt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grOutTimeSt.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grOutTimeSt.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grIcdMainText.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grIcdMainText.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grIcdMainText.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grIcdMainText.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grIcdText.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grIcdText.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grIcdText.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grIcdText.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grCreateTimeSt.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grCreateTimeSt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grCreateTimeSt.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grCreateTimeSt.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grCreator.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grCreator.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grModifyTimeSt.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grModifyTimeSt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grModifyTimeSt.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.grModifyTimeSt.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.frModifier.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.frModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.frModifier.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.frModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grColPrint.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grColPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grTreatmentEndTypeName.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.grTreatmentEndTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Treatment_SentIntegrateHis.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn_Treatment_SentIntegrateHis.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn1.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn2.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn6.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn7.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn8.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn9.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn10.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn11.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn12.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn13.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn14.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn15.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn16.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn17.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn18.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn19.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn20.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEndDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTreatmentList.cboEndDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.a.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.a.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem101.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem39.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem39.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
