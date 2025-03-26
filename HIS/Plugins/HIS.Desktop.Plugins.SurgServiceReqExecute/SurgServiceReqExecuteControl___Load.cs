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
using ACS.EFMODEL.DataModels;
using AutoMapper;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Config;
using ACS.Filter;
using System.IO;
using Newtonsoft.Json;
using System.Drawing;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    public partial class SurgServiceReqExecuteControl : UserControlBase
    {
        private void InitLanguage()
        {
            try
            {
                ResourceLangManager.InitResourceLanguageManager();

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.tileViewColumn2.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.tileViewColumn2.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.tileViewColumnName.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.tileViewColumnName.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.tileViewColumnSTTImage.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.tileViewColumnSTTImage.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlLeft.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlLeft.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnCoppyInfo.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnCoppyInfo.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnCoppyInfo.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnCoppyInfo.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.chkServiceCode.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkServiceCode.Properties.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnSavePtttTemp.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnSavePtttTemp.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboPtttTemp.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboPtttTemp.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                //this.dropDownButtonGPBL.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.dropDownButtonGPBL.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.dropDownButtonGPBL.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.dropDownButtonGPBL.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn2.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn8.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn8.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn3.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn4.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn5.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnTuTruc.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnTuTruc.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnAssignBlood.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnAssignBlood.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColServiceCode_InEkip.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColServiceCode_InEkip.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColServiceName_InEKip.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColServiceName_InEKip.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColAmount.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColAmount.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColUnit_InEkip.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColUnit_InEkip.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColSereServAttachServiceCode.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColSereServAttachServiceCode.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColSereServAttachServiceName.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColSereServAttachServiceName.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColSereServAttachAmount.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColSereServAttachAmount.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColSereServAttachUnit.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColSereServAttachUnit.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.grdColSTT.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.grdColSTT.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.grdColServiceCode.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.grdColServiceCode.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.grdColServiceName.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.grdColServiceName.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.grdColUnit.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.grdColUnit.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.grdColNumber.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.grdColNumber.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.grdColObjectPay.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.grdColObjectPay.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumnMaty.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumnMaty.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                //this.btnAssignPre.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnAssignPre.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                //this.ddbPhatSinh.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.ddbPhatSinh.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.ddbPhatSinh.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.ddbPhatSinh.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlGroup1.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlGroup1.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlGroup2.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlGroup2.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciPtttTemp.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciPtttTemp.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciPtttTemp.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciPtttTemp.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem55.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem55.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnOther.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnOther.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlRight.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlRight.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.chkKetThuc.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkKetThuc.Properties.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.xtraTabPage1.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnChoosePtttMethods.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnChoosePtttMethods.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnImagePublic.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnImagePublic.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnCreateImageLuuDo.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnCreateImageLuuDo.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabPageMoTa.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.xtraTabPageMoTa.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabPageGhiChu.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.xtraTabPageGhiChu.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabPageLuocDo.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.xtraTabPageLuocDo.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.tileViewItemElement1.Text = Inventec.Common.Resource.Get.Value("tileViewItemElement1.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.tileViewItemElement2.Text = Inventec.Common.Resource.Get.Value("tileViewItemElement2.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.tileViewIsChecked.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.tileViewIsChecked.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.tileViewColumn4.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.tileViewColumn4.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.Checked.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.Checked.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumnImage.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumnImage.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn7.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboIcdCmName.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboIcdCmName.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.chkIcdCm.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkIcdCm.Properties.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.chkSaveGroup.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkSaveGroup.Properties.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnChonMauPTCHuyenKhoaMat.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnChonMauPTCHuyenKhoaMat.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboDepartment.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboIcd3.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboIcd3.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.chkIcd3.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkIcd3.Properties.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboIcd2.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboIcd2.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.chkIcd2.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkIcd2.Properties.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboIcd1.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboIcd1.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.chkIcd1.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkIcd1.Properties.Caption", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboPhuongPhapThucTe.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboPhuongPhapThucTe.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboBanMo.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboBanMo.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboMachine.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboMachine.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboMoKTCao.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboMoKTCao.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboKQVoCam.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboKQVoCam.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboPhuongPhap2.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboPhuongPhap2.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboLoaiPT.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboLoaiPT.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnKTDT.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnKTDT.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.toolTipItem2.Text = Inventec.Common.Resource.Get.Value("toolTipItem2.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnSaveEkipTemp.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnSaveEkipTemp.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboEkipTemp.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboEkipTemp.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboMethod.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboMethod.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnSwapService.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnSwapService.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.txtIcdExtraCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.txtIcdExtraCode.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnDepartmentTran.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnDepartmentTran.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnPrint.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnSave.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.btnFinish.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnFinish.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboDeathSurg.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboDeathSurg.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboCatastrophe.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboCatastrophe.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cboCondition.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboCondition.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cbbBloodRh.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cbbBloodRh.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cbbBlood.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cbbBlood.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cbbEmotionlessMethod.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cbbEmotionlessMethod.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.cbbPtttGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cbbPtttGroup.Properties.NullText", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.txtIcdText.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciKetLuan.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciKetLuan.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem3.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem14.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciTinhTrang.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciTinhTrang.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciCachThuc.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciCachThuc.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciStartTime.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciStartTime.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciChuanDoanPhu.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciChuanDoanPhu.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciChuanDoanPhu.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciChuanDoanPhu.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciEkipTemp.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciEkipTemp.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciPhanLoai.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciPhanLoai.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem21.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem21.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem21.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciPhuongPhap.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciPhuongPhap.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciVoCam.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciVoCam.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem28.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem28.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem28.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciBanMo.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciBanMo.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem17.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem17.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem17.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciMachine.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciMachine.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciMachine.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciMachine.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciTaiBien.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciTaiBien.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciTuVongTrong.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciTuVongTrong.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem9.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem9.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem9.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcd1.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcd1.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcd1.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcd1.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcd2.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcd2.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcd2.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcd2.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcd3.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcd3.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcd3.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcd3.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciThoiGianKetThuc.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciThoiGianKetThuc.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem2.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem2.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem2.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciChkGroup.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciChkGroup.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcdCmCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcdCmCode.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcdCmCode.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcdCmCode.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcdCmSubCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcdCmSubCode.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcdCmSubCode.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcdCmSubCode.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem42.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem42.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem42.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem42.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciDepartment.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciDepartment.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciNhomMau.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciNhomMau.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciRh.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciRh.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciKTC.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciKTC.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.lciKTC.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciKTC.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem1.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem39.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem39.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem56.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem56.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem58.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem58.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem58.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem58.Text", Base.ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async void LoadGridSereServInAttach()
        {
            try
            {
                if (this.sereServ != null)
                {
                    sereServInPackages = new List<V_HIS_SERE_SERV_5>();
                    sereServInPackages = sereServs.Where(o => o.PARENT_ID == this.sereServ.ID).ToList();
                    if (sereServInPackages.Count > 0 && sereServInPackages != null)
                    {
                        gridControlSereServAttach.DataSource = null;
                        gridControlSereServAttach.DataSource = sereServInPackages;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async void LoadGridSereServInEkip()
        {
            try
            {
                if (this.sereServ != null && this.sereServ.EKIP_ID.HasValue)
                {
                    sereServInEkips = new List<V_HIS_SERE_SERV_5>();
                    sereServInEkips = sereServs.Where(o => o.EKIP_ID == this.sereServ.EKIP_ID.Value && o.ID != this.sereServ.ID && o.IS_NO_EXECUTE != 1).ToList();
                    if (sereServInEkips.Count > 0 && sereServInEkips != null)
                    {
                        gridControlServServInEkip.DataSource = null;
                        gridControlServServInEkip.DataSource = sereServInEkips;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadDropDownButtonOther()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_CHUYEN_PHONG", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(btnChangeExecuteRoom_Click)));
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_DAU_HIEU_SINH_TON", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(btnDhst_Click)));
                //Close MediRecord
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_KET_THUC_DIEU_TRI", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(btnTreatmentClose_Click)));
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMMAIN_CAPTION_MODULE_ASSAIN_BLOOD", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(AssignServiceBloodClick)));
                //ddbtnOther.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void RefeshSerservPTTTOnSaveSuccess()
        {
            try
            {
                //Khi lưu thành công
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServPtttViewFilter hisSereServPtttFilter = new MOS.Filter.HisSereServPtttViewFilter();
                hisSereServPtttFilter.SERE_SERV_ID = sereServ.ID;

                List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT> hisSereServPttt = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, hisSereServPtttFilter, param);
                if (hisSereServPttt != null && hisSereServPttt.Count > 0)
                {
                    this.sereServPTTT = hisSereServPttt[0];
                    LoadDropDownButtonOther();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnChangeExecuteRoom_Click(object sender, EventArgs e)
        {
            try
            {
                //frmChangeExamRoom frm = new frmChangeExamRoom(this.serviceReq, RefeshData);
                //frm.ShowDialog();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void AssignServiceBloodClick(object sender, EventArgs e)
        {
            try
            {
                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>();
                V_HIS_SERE_SERV sereServInput = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>(sereServ);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnTreatmentClose_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (this.serviceReq == null)
            //        throw new ArgumentNullException("HisServiceReqWithOrderSDO is null");

            //    EXE.APP.Modules.Main.frmMain formMain = SessionManager.GetFormMain();
            //    if (formMain == null)
            //        throw new ArgumentNullException("formMain is null");

            //    EXE.APP.Modules.ResolvedFinish.frmCloseTreatment closeTreatmentControl = new EXE.APP.Modules.ResolvedFinish.frmCloseTreatment(this.serviceReq.TREATMENT_ID);
            //    closeTreatmentControl.ShowDialog();
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void btnDhst_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (this.serviceReq == null)
            //    {
            //        Inventec.Common.Logging.LogSystem.Warn("HisServiceReqWithOrderSDO is null");
            //        return;
            //    }

            //    EXE.APP.Modules.ServiceExtra.Dhst.frmVitalSignsAdd frmDhstAdd = new Modules.ServiceExtra.Dhst.frmVitalSignsAdd(this.serviceReq.TREATMENT_ID);
            //    frmDhstAdd.action = EXE.LOGIC.LocalStore.GlobalStore.ActionAdd;
            //    frmDhstAdd.ShowDialog();
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void RefeshData()
        {
            btnSave.Enabled = false;
            btnFinish.Enabled = false;
        }

        public async Task GetSereServByTreatment()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("serviceReq.TREATMENT_ID: " + serviceReq.TREATMENT_ID);
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                sereServs = new List<V_HIS_SERE_SERV_5>();
                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.TREATMENT_ID = this.serviceReq.TREATMENT_ID;
                List<HIS_SERE_SERV> sereServ8s = await new BackendAdapter(param)
                    .GetAsync<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);
                if (sereServ8s != null && sereServ8s.Count > 0)
                {
                    List<HIS_DEPARTMENT> departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();
                    List<V_HIS_ROOM> rooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>();
                    foreach (var item in sereServ8s)
                    {
                        V_HIS_SERE_SERV_5 sereServ5 = new V_HIS_SERE_SERV_5();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_5>(sereServ5, item);
                        V_HIS_SERVICE service = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        if (service != null)
                        {
                            sereServ5.TDL_SERVICE_NAME = service.SERVICE_NAME;
                            sereServ5.TDL_SERVICE_CODE = service.SERVICE_CODE;
                            sereServ5.TDL_SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                            sereServ5.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                            sereServ5.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                            sereServ5.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                        }

                        sereServs.Add(sereServ5);
                    }

                    sereServs = sereServs != null ? sereServs.OrderByDescending(o => o.ID).ToList() : null;

                    this.LoadGridSereServByServiceReq();
                    this.LoadGridSereServInAttach();
                    this.LoadGridSereServInEkip();
                    this.SetEnableControl();
                    this.LoadSereServExt();
                    this.FillDataDefaultToControl();
                    this.SetButtonDeleteGridLookup();
                    this.LoadDataSesePtttMetod();
                    this.FillDataBySereServPttt();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataSesePtttMetod()
        {
            try
            {
                var listMethod = new List<HIS_SESE_PTTT_METHOD>();
                Action myaction = () =>
                {
                    CommonParam param = new CommonParam();
                    this.listSesePtttMetod = new List<PtttMethodADO>();
                    HisSesePtttMethodFilter filter = new HisSesePtttMethodFilter();
                    filter.TDL_SERVICE_REQ_ID = this.serviceReq.ID;
                    listMethod = new BackendAdapter(param).Get<List<HIS_SESE_PTTT_METHOD>>("api/HisSesePtttMethod/Get", ApiConsumers.MosConsumer, filter, param);
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                LoadDataSesePtttMethodDetail(listMethod);
                ProcessLoadRealPtttMethod(this.sereServPTTT);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private async void LoadDataSesePtttMethodDetail(List<HIS_SESE_PTTT_METHOD> listMethod)
        {
            try
            {
                if (listMethod != null && listMethod.Count > 0)
                {
                    var oldData = this.listSesePtttMetod.Where(o => listMethod.Select(s => s.TDL_SERE_SERV_ID).Contains(o.SERE_SERV_ID)).ToList();
                    if (oldData != null && oldData.Count > 0)
                    {
                        this.listSesePtttMetod = this.listSesePtttMetod.Where(o => !oldData.Select(s => s.ID).Contains(o.ID)).ToList();
                    }

                    List<long> listEkipId = listMethod.Select(s => s.EKIP_ID ?? 0).Distinct().ToList();

                    CommonParam param = new CommonParam();
                    List<HIS_EKIP_USER> ekipUser = new List<HIS_EKIP_USER>();
                    if (listEkipId.Count > 0)
                    {
                        HisEkipUserFilter userFilter = new HisEkipUserFilter();
                        userFilter.EKIP_IDs = listEkipId;
                        ekipUser = new BackendAdapter(param).Get<List<HIS_EKIP_USER>>("api/HisEkipUser/Get", ApiConsumers.MosConsumer, userFilter, param);
                    }

                    foreach (var item in listMethod)
                    {
                        PtttMethodADO ado = new PtttMethodADO();
                        ado.ID = item.PTTT_METHOD_ID;
                        ado.AMOUNT = item.AMOUNT;
                        ado.PTTT_GROUP_ID = item.PTTT_GROUP_ID;
                        ado.SERE_SERV_ID = item.TDL_SERE_SERV_ID;
                        ado.EKIP_ID = item.EKIP_ID;
                        ado.SERVICE_REQ_ID = item.TDL_SERVICE_REQ_ID;

                        if (item.EKIP_ID.HasValue)
                        {
                            ado.EkipUsersADO = new EkipUsersADO();
                            // EkipUsersADO ado = new EkipUsersADO();
                            List<HIS_EKIP_USER> kips = ekipUser.Where(o => o.EKIP_ID == item.EKIP_ID).ToList();
                            List<HisEkipUserADO> lst = new List<HisEkipUserADO>();
                            for (int i = 0; i < kips.Count; i++)
                            {
                                HisEkipUserADO Ekipado = new HisEkipUserADO();
                                Ekipado.USERNAME = kips[i].USERNAME;
                                Ekipado.LOGINNAME = kips[i].LOGINNAME;
                                Ekipado.EXECUTE_ROLE_ID = kips[i].EXECUTE_ROLE_ID;
                                lst.Add(Ekipado);
                            }
                            ado.EkipUsersADO.idPtttMethod = item.PTTT_METHOD_ID;
                            ado.EkipUsersADO.listEkipUser = lst;
                        }

                        var method = BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == item.PTTT_METHOD_ID);
                        if (method != null)
                        {
                            ado.PTTT_METHOD_CODE = method.PTTT_METHOD_CODE;
                            ado.PTTT_METHOD_NAME = method.PTTT_METHOD_NAME;
                        }

                        this.listSesePtttMetod.Add(ado);

                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public async Task LoadAfterAssign()
        {
            try
            {
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                sereServs = new List<V_HIS_SERE_SERV_5>();
                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.TREATMENT_ID = this.serviceReq.TREATMENT_ID;
                List<HIS_SERE_SERV> sereServ8s = await new BackendAdapter(param)
                    .GetAsync<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);
                if (sereServ8s != null && sereServ8s.Count > 0)
                {
                    List<HIS_DEPARTMENT> departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();
                    List<V_HIS_ROOM> rooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>();
                    foreach (var item in sereServ8s)
                    {
                        V_HIS_SERE_SERV_5 sereServ5 = new V_HIS_SERE_SERV_5();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_5>(sereServ5, item);
                        V_HIS_SERVICE service = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        if (service != null)
                        {
                            sereServ5.TDL_SERVICE_NAME = service.SERVICE_NAME;
                            sereServ5.TDL_SERVICE_CODE = service.SERVICE_CODE;
                            sereServ5.TDL_SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                            sereServ5.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                            sereServ5.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                            sereServ5.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                        }

                        sereServs.Add(sereServ5);
                    }
                    sereServs = sereServs != null ? sereServs.OrderByDescending(o => o.ID).ToList() : null;

                    this.LoadGridSereServByServiceReq();
                    this.LoadGridSereServInAttach();
                    this.LoadGridSereServInEkip();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadGridSereServByServiceReq()
        {
            try
            {
                if (sereServs != null && sereServs.Count > 0)
                {
                    string serviceCodeOverExpend = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE.SERVICE_CODE.OVER_EXPEND");
                    sereServbyServiceReqs = sereServs.Where(o => o.SERVICE_REQ_ID == this.serviceReq.ID && o.TDL_SERVICE_CODE != serviceCodeOverExpend).ToList();
                    grdControlService.DataSource = sereServbyServiceReqs;
                    if (this.sereServ != null)
                    {
                        this.sereServ = sereServbyServiceReqs.First(o => o.ID == this.sereServ.ID);
                    }
                    this.sereServ = sereServbyServiceReqs[0];


                    this.LoadSereServLast();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async Task LoadSereServLast()
        {
            try
            {
                if (sereServ != null && serviceReq != null && chkServiceCode.Checked)
                {
                    CommonParam param = new CommonParam();
                    HisSereServRecentPtttFilter filter = new HisSereServRecentPtttFilter();
                    filter.COUNT = 5;
                    filter.INTRUCTION_TIME = serviceReq.INTRUCTION_TIME;
                    filter.PATIENT_ID = serviceReq.TDL_PATIENT_ID;
                    filter.SERVICE_ID = sereServ.SERVICE_ID;
                    sereServLasts = await new BackendAdapter(param)
                        .GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/GetRecentPttt", ApiConsumers.MosConsumer, filter, param);
                    gridControlSereServLast.DataSource = this.sereServLasts;
                }
                else
                {
                    CommonParam param = new CommonParam();
                    HisSereServRecentPtttFilter filter = new HisSereServRecentPtttFilter();
                    filter.COUNT = 5;
                    filter.INTRUCTION_TIME = serviceReq.INTRUCTION_TIME;
                    filter.PATIENT_ID = serviceReq.TDL_PATIENT_ID;
                    sereServLasts = await new BackendAdapter(param)
                        .GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/GetRecentPttt", ApiConsumers.MosConsumer, filter, param);
                    gridControlSereServLast.DataSource = this.sereServLasts;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async Task LoadSereServExt()
        {
            try
            {
                //nếu có thời gian bắt đầu/kết thúc đã lưu thì load theo thời gian đã lưu
                CommonParam param = new CommonParam();
                HisSereServExtFilter ssExtFilter = new HisSereServExtFilter();
                ssExtFilter.SERE_SERV_IDs = new List<long>() { this.sereServ.ID };
                if (dicSereServCopy != null && dicSereServCopy.Count > 0 && dicSereServCopy.ContainsKey(sereServ.ID))
                    ssExtFilter.SERE_SERV_IDs.Add(dicSereServCopy[sereServ.ID]);
                var SereServExts = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, ssExtFilter, param);
                this.SereServExt = new HIS_SERE_SERV_EXT();//xuandv
                if (SereServExts != null && SereServExts.Count > 0)
                {
                    //add vào danh sách
                    foreach (var item in ListSereServExt)
                    {
                        HIS_SERE_SERV_EXT ext = ListSereServExt.FirstOrDefault(o => o.ID == item.ID);
                        if (ext != null)
                            ext = item;//gán lại để cập nhật dữ liệu mới nhất
                        else
                            ListSereServExt.Add(item);
                    }
                    var dfSereServExt = SereServExts.FirstOrDefault(o => o.SERE_SERV_ID == this.sereServ.ID);
                    if (dicSereServCopy != null && dicSereServCopy.Count > 0 && dicSereServCopy.ContainsKey(sereServ.ID))
                        this.SereServExt = SereServExts.FirstOrDefault(o => o.SERE_SERV_ID == dicSereServCopy[sereServ.ID]);
                    else
                        this.SereServExt = dfSereServExt;
                    txtConclude.Text = this.SereServExt.CONCLUDE;
                    txtDescription.Text = this.SereServExt.DESCRIPTION;
                    txtResultNote.Text = this.SereServExt.NOTE;
                    txtIntructionNote.Text = !string.IsNullOrEmpty(SereServExt.INSTRUCTION_NOTE) || (!txtIntructionNote.Text.Equals(SereServExt.INSTRUCTION_NOTE)) ? this.SereServExt.INSTRUCTION_NOTE : dfSereServExt.INSTRUCTION_NOTE;
                    txtMachineCode.Text = this.SereServExt.MACHINE_CODE;
                    cboMachine.EditValue = this.SereServExt.MACHINE_ID;
                    LogSystem.Info("SereServExts: " + SereServExts.Count);
                    if (this.SereServExt.BEGIN_TIME.HasValue)
                    {
                        dtStart.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.SereServExt.BEGIN_TIME.Value) ?? DateTime.Now;
                    }
                    else
                    {
                        dtStart.EditValue = null;
                    }

                    if (this.SereServExt.END_TIME.HasValue)
                    {
                        dtFinish.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.SereServExt.END_TIME.Value) ?? DateTime.Now;
                        dtFinish.Properties.Buttons[1].Visible = true;
                        if (this.SereServExt.BEGIN_TIME.HasValue)
                        {
                            DateTime? dateBefore = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.SereServExt.BEGIN_TIME.Value);
                            DateTime? dateAfter = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.SereServExt.END_TIME.Value);
                            if (dateBefore != null && dateAfter != null)
                            {
                                TimeSpan difference = dateAfter.Value - dateBefore.Value;
                                LogSystem.Warn(difference + "_______________!!!!");
                                spinExcuteTimeAdd.EditValue = difference.TotalMinutes;
                            }
                        }

                    }
                    else
                    {
                        dtFinish.EditValue = null;
                        dtFinish.Properties.Buttons[1].Visible = false;
                    }
                }
                else
                {
                    txtConclude.Text = "";
                    txtDescription.Text = "";
                    txtResultNote.Text = "";
                    txtIntructionNote.Text = "";
                    txtMachineCode.Text = "";
                    cboMachine.EditValue = null;
                }

                //if (serviceReq != null && this.sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.TakeIntrucionTimeByServiceReq") == "1" && ((SereServExt != null && SereServExt.BEGIN_TIME == null) || SereServExt == null))
                //{
                //    dtStart.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME) ?? DateTime.Now;
                //}

                //để sau api GetAsync để dữ liệu được giữ nguyên.
                //nếu có cấu hình thì gán thời gian theo cấu hình
                if (!String.IsNullOrWhiteSpace(HisConfigCFG.TAKE_INTRUCTION_TIME_BY_SERVICE_REQ) && (SereServExt == null || !SereServExt.BEGIN_TIME.HasValue))
                {
                    if (serviceReq != null
                        && this.sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                        && HisConfigCFG.TAKE_INTRUCTION_TIME_BY_SERVICE_REQ == "1")
                    {
                        dtStart.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME) ?? DateTime.Now;
                    }
                    else if (serviceReq != null
                        && (this.sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || this.sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                        && HisConfigCFG.TAKE_INTRUCTION_TIME_BY_SERVICE_REQ == "2")
                    {
                        dtStart.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME) ?? DateTime.Now;
                    }
                    else if (serviceReq != null
                      && (this.sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || this.sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                      && HisConfigCFG.TAKE_INTRUCTION_TIME_BY_SERVICE_REQ == "3")
                    {
                        dtStart.DateTime = DateTime.Now;
                    }
                }

                if (spinExcuteTimeAdd.EditValue != null && spinExcuteTimeAdd.Value > 0 && dtStart.DateTime != DateTime.MinValue && (SereServExt == null || !SereServExt.END_TIME.HasValue))
                {
                    dtFinish.DateTime = dtStart.DateTime.AddMinutes((double)spinExcuteTimeAdd.Value);
                }

                //xuandv
                this.InitButtonOther();

                List<long> ssIds = new List<long>();
                if (dicSereServCopy != null && dicSereServCopy.Count > 0 && dicSereServCopy.ContainsKey(sereServ.ID))
                    ssIds.Add(dicSereServCopy[sereServ.ID]);
                else
                    ssIds.Add(this.sereServ.ID);
                ProcessLoadSereServFile(ssIds);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private bool CheckLessTime(ref string serviceCode)
        {
            bool rs = false;
            try
            {
                if (this.sereServbyServiceReqs != null && this.sereServbyServiceReqs.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisSereServExtFilter filter = new HisSereServExtFilter();
                    filter.SERE_SERV_IDs = this.sereServbyServiceReqs.Select(o => o.ID).ToList();

                    var rsApi = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, param);
                    if (rsApi != null && rsApi.Count > 0)
                    {
                        if (rsApi.Count == this.sereServbyServiceReqs.Count)
                        {
                            var check = rsApi.Where(o => o.BEGIN_TIME == null || o.END_TIME == null).ToList();
                            if (check != null && check.Count > 0)
                            {
                                rs = true;
                                serviceCode = string.Join(",", this.sereServbyServiceReqs.Where(o => check.Select(p => p.SERE_SERV_ID).Contains(o.ID)).Select(o => o.TDL_SERVICE_CODE).ToList());
                            }
                        }
                        else if (rsApi.Count < this.sereServbyServiceReqs.Count)
                        {
                            rs = true;
                            serviceCode = string.Join(",", this.sereServbyServiceReqs.Where(o => !rsApi.Select(p => p.SERE_SERV_ID).Contains(o.ID)).Select(o => o.TDL_SERVICE_CODE).ToList());
                        }
                    }
                    else
                    {
                        rs = true;
                        serviceCode = string.Join(",", this.sereServbyServiceReqs.Select(o => o.TDL_SERVICE_CODE).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return false;
            }
            return rs;
        }


        private void GetSereServPtttBySereServId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServPtttViewFilter hisSereServPtttFilter = new HisSereServPtttViewFilter();
                if (dicSereServCopy != null && dicSereServCopy.Count > 0 && dicSereServCopy.ContainsKey(sereServ.ID))
                    hisSereServPtttFilter.SERE_SERV_ID = dicSereServCopy[sereServ.ID];
                else
                    hisSereServPtttFilter.SERE_SERV_ID = this.sereServ.ID;
                var SereServPttt = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, hisSereServPtttFilter, param);
                if (SereServPttt != null && SereServPttt.Count > 0)
                {
                    //add vào danh sách
                    foreach (var item in SereServPttt)
                    {
                        V_HIS_SERE_SERV_PTTT pttt = hisSereServPttt.FirstOrDefault(o => o.ID == item.ID);
                        if (pttt != null)
                            hisSereServPttt.Remove(pttt);//gán lại để cập nhật dữ liệu mới nhất
                        hisSereServPttt.Add(item);
                    }
                }
                if (dicSereServCopy != null && dicSereServCopy.Count > 0 && dicSereServCopy.ContainsKey(sereServ.ID))
                    this.sereServPTTT = (hisSereServPttt != null && hisSereServPttt.Count > 0) ? hisSereServPttt.FirstOrDefault(o => o.SERE_SERV_ID == dicSereServCopy[sereServ.ID]) : null;
                else
                    this.sereServPTTT = (hisSereServPttt != null && hisSereServPttt.Count > 0) ? hisSereServPttt.FirstOrDefault(o => o.SERE_SERV_ID == sereServ.ID) : null;
                if (this.sereServPTTT != null && this.sereServPTTT.EYE_SURGRY_DESC_ID > 0)
                {
                    param = new CommonParam();
                    HisEyeSurgryDescFilter eyeSurgryDescFilter = new HisEyeSurgryDescFilter();
                    eyeSurgryDescFilter.ID = this.sereServPTTT.EYE_SURGRY_DESC_ID;
                    var eyeSurgDescs = new BackendAdapter(param)
                    .Get<List<HIS_EYE_SURGRY_DESC>>("api/HisEyeSurgryDesc/Get", ApiConsumers.MosConsumer, eyeSurgryDescFilter, param);
                    this.currentEyeSurgDesc = (eyeSurgDescs != null && eyeSurgDescs.Count > 0) ? eyeSurgDescs.FirstOrDefault() : null;
                }
                else
                {
                    this.currentEyeSurgDesc = new HIS_EYE_SURGRY_DESC();
                }
                if (this.sereServPTTT != null)
                {
                    HisStentConcludeFilter filter = new HisStentConcludeFilter();
                    filter.SERE_SERV_ID = sereServPTTT.SERE_SERV_ID;
                    stentConcludeSave = new BackendAdapter(param).Get<List<HIS_STENT_CONCLUDE>>("api/HisStentConclude/Get", ApiConsumers.MosConsumer, filter, param);
                }

                if (this.sereServPTTT != null && this.sereServPTTT.SKIN_SURGERY_DESC_ID > 0)
                {
                    param = new CommonParam();
                    HisSkinSurgeryDescFilter skinSurgeryDescFilter = new HisSkinSurgeryDescFilter();
                    skinSurgeryDescFilter.ID = this.sereServPTTT.SKIN_SURGERY_DESC_ID;
                    var skinSurgeryDescs = new BackendAdapter(param)
                    .Get<List<HIS_SKIN_SURGERY_DESC>>("api/HisSkinSurgeryDesc/Get", ApiConsumers.MosConsumer, skinSurgeryDescFilter, param);
                    if (skinSurgeryDescs != null && skinSurgeryDescs.Count > 0)
                    {
                        this.SkinSurgeryDes = new SkinSurgeryDesADO();
                        this.SkinSurgeryDes.SURGERY_POSITION_ID = this.sereServPTTT.SURGERY_POSITION_ID;
                        Inventec.Common.Mapper.DataObjectMapper.Map<SkinSurgeryDesADO>(SkinSurgeryDes, skinSurgeryDescs.FirstOrDefault());
                    }
                    else
                    {
                        skinSurgeryDescs = null;
                    }
                }
                else
                {
                    this.SkinSurgeryDes = new SkinSurgeryDesADO();
                }

                this.InitPrintSurgService();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FillDataBySereServPttt()
        {
            try
            {
                GetSereServPtttBySereServId();
                this.refreshControl();
                this.SetDefaultCboPTTTGroup(sereServ);
                this.SetDefaultCboICD9CmChinh(sereServ);
                this.ComboMethodPTTT();
                this.ComboPTTTGroup();
                this.ComboEmotionlessMothod();
                this.ComboBlood();//Nhóm máu
                this.ComboBloodRh();//Nhóm máu RH
                this.ComboPtttCondition();//Tình hình Pttt
                this.ComboCatastrophe();//Tai biến trong PTTT
                this.ComboDeathWithin();//Tử vong trong PTTT

                this.ComboLoaiPT();
                this.ComboPhuongPhap2();
                this.ComboKQVoCam();
                this.ComboMoKTCao();
                this.ComboHisMachine();
                this.LoadComboPtttTable(cboBanMo);
                this.ComboPhuongPhapThucTe();
                LoadDefaultControl();
                if (this.sereServPTTT != null && this.sereServPTTT.ID > 0)
                {
                    LogSystem.Warn("LOAD 1");
                    FillDataToCboIcd(txtIcdCode1, txtIcd1, cboIcd1, chkIcd1, this.sereServPTTT.ICD_CODE, this.sereServPTTT.ICD_NAME);
                    FillDataToCboIcd(txtIcdCode2, txtIcd2, cboIcd2, chkIcd2, this.sereServPTTT.BEFORE_PTTT_ICD_CODE, this.sereServPTTT.BEFORE_PTTT_ICD_NAME);
                    FillDataToCboIcd(txtIcdCode3, txtIcd3, cboIcd3, chkIcd3, this.sereServPTTT.AFTER_PTTT_ICD_CODE, this.sereServPTTT.AFTER_PTTT_ICD_NAME);
                    FillDataToCboIcdCm(txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm, this.sereServPTTT.ICD_CM_CODE, this.sereServPTTT.ICD_CM_NAME);
                    LogSystem.Warn("LOAD 2");
                    if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_SUB_CODE))
                    {
                        this.txtIcdExtraCode.Text = this.sereServPTTT.ICD_SUB_CODE;
                    }
                    if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_TEXT))
                    {
                        this.txtIcdText.Text = this.sereServPTTT.ICD_TEXT;
                    }

                    if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_CM_SUB_CODE))
                    {
                        this.txtIcdCmSubCode.Text = this.sereServPTTT.ICD_CM_SUB_CODE;
                    }
                    if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_CM_TEXT))
                    {
                        this.txtIcdCmSubName.Text = this.sereServPTTT.ICD_CM_TEXT;
                    }

                    this.txtMANNER.Text = this.sereServPTTT.MANNER;

                    if (!string.IsNullOrEmpty(sereServPTTT.SUBS_DIRECTOR_LOGINNAME) && lstReAcsUserADO.Exists(o => o.LOGINNAME == sereServPTTT.SUBS_DIRECTOR_LOGINNAME))
                    {
                        cboHospSubs.EditValue = sereServPTTT.SUBS_DIRECTOR_LOGINNAME;
                    }

                    if (!string.IsNullOrEmpty(sereServPTTT.SUBS_HEAD_LOGINNAME) && lstReAcsUserADO.Exists(o => o.LOGINNAME == sereServPTTT.SUBS_HEAD_LOGINNAME))
                    {
                        cboEndDeptSubs.EditValue = sereServPTTT.SUBS_HEAD_LOGINNAME;
                    }
                }
                else
                {
                    if (this.sereServ != null && !this.sereServ.EKIP_ID.HasValue)
                    {
                        this.txtMANNER.Text = this.sereServ.TDL_SERVICE_NAME;
                    }
                }
                if (this.sereServ.TDL_HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CDHA)//neu loai la cdha thi bat buoc nhap ket luan - IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CDHA
                {
                    this.lciKetLuan.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadDetailSereServPttt()
        {
            try
            {
                if (this.sereServPTTT != null && this.sereServPTTT.ID > 0)
                {
                    LogSystem.Warn("LOAD 3");
                    FillDataToCboIcd(txtIcdCode1, txtIcd1, cboIcd1, chkIcd1, this.sereServPTTT.ICD_CODE, this.sereServPTTT.ICD_NAME);
                    FillDataToCboIcd(txtIcdCode2, txtIcd2, cboIcd2, chkIcd2, this.sereServPTTT.BEFORE_PTTT_ICD_CODE, this.sereServPTTT.BEFORE_PTTT_ICD_NAME);
                    FillDataToCboIcd(txtIcdCode3, txtIcd3, cboIcd3, chkIcd3, this.sereServPTTT.AFTER_PTTT_ICD_CODE, this.sereServPTTT.AFTER_PTTT_ICD_NAME);
                    FillDataToCboIcdCm(txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm, this.sereServPTTT.ICD_CM_CODE, this.sereServPTTT.ICD_CM_NAME);
                    LogSystem.Warn("LOAD 4");
                    if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_SUB_CODE))
                    {
                        txtIcdExtraCode.Text = this.sereServPTTT.ICD_SUB_CODE;
                    }
                    if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_TEXT))
                    {
                        txtIcdText.Text = this.sereServPTTT.ICD_TEXT;
                    }

                    if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_CM_SUB_CODE))
                    {
                        this.txtIcdCmSubCode.Text = this.sereServPTTT.ICD_CM_SUB_CODE;
                    }
                    if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_CM_TEXT))
                    {
                        this.txtIcdCmSubName.Text = this.sereServPTTT.ICD_CM_TEXT;
                    }

                    txtMANNER.Text = this.sereServPTTT.MANNER;

                    ProcessLoadRealPtttMethod(this.sereServPTTT);

                    if (this.sereServPTTT.PTTT_PRIORITY_ID > 0)
                    {
                        var data = BackendDataWorker.Get<HIS_PTTT_PRIORITY>().FirstOrDefault(p => p.ID == this.sereServPTTT.PTTT_PRIORITY_ID);
                        if (data != null)
                        {
                            txtLoaiPT.Text = data.PTTT_PRIORITY_CODE;
                            cboLoaiPT.EditValue = data.ID;
                        }
                        //else
                        //{
                        //    txtLoaiPT.Text = "";
                        //    cboLoaiPT.EditValue = null;
                        //}
                    }
                    if (this.sereServPTTT.EMOTIONLESS_METHOD_SECOND_ID > 0)
                    {
                        var data = BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(p => p.ID == this.sereServPTTT.EMOTIONLESS_METHOD_SECOND_ID);
                        if (data != null)
                        {
                            txtPhuongPhap2.Text = data.EMOTIONLESS_METHOD_CODE;
                            cboPhuongPhap2.EditValue = data.ID;
                        }
                        else
                        {
                            txtPhuongPhap2.Text = "";
                            cboPhuongPhap2.EditValue = null;
                        }
                    }
                    if (this.sereServPTTT.EMOTIONLESS_RESULT_ID > 0)
                    {
                        var data = BackendDataWorker.Get<HIS_EMOTIONLESS_RESULT>().FirstOrDefault(p => p.ID == this.sereServPTTT.EMOTIONLESS_RESULT_ID);
                        if (data != null)
                        {
                            txtKQVoCam.Text = data.EMOTIONLESS_RESULT_CODE;
                            cboKQVoCam.EditValue = data.ID;
                        }
                        else
                        {
                            txtKQVoCam.Text = "";
                            cboKQVoCam.EditValue = null;
                        }
                    }
                    if (this.sereServPTTT.PTTT_HIGH_TECH_ID > 0)
                    {
                        var data = BackendDataWorker.Get<HIS_PTTT_HIGH_TECH>().FirstOrDefault(p => p.ID == this.sereServPTTT.PTTT_HIGH_TECH_ID);
                        if (data != null)
                        {
                            txtMoKTCao.Text = data.PTTT_HIGH_TECH_CODE;
                            cboMoKTCao.EditValue = data.ID;
                        }
                        else
                        {
                            txtMoKTCao.Text = "";
                            cboMoKTCao.EditValue = null;
                        }
                    }
                    if (this.sereServPTTT.PTTT_TABLE_ID > 0)
                    {
                        var data = BackendDataWorker.Get<HIS_PTTT_TABLE>().FirstOrDefault(p => p.ID == this.sereServPTTT.PTTT_TABLE_ID);
                        if (data != null)
                        {
                            txtBanMoCode.Text = data.PTTT_TABLE_CODE;
                            cboBanMo.EditValue = data.ID;
                        }
                        else
                        {
                            txtBanMoCode.Text = "";
                            cboBanMo.EditValue = null;
                        }
                    }

                    if (!string.IsNullOrEmpty(this.sereServPTTT.SUBS_DIRECTOR_LOGINNAME) && lstReAcsUserADO.Exists(o => o.LOGINNAME == this.sereServPTTT.SUBS_DIRECTOR_LOGINNAME))
                    {
                        cboHospSubs.EditValue = this.sereServPTTT.SUBS_DIRECTOR_LOGINNAME;
                    }

                    if (!string.IsNullOrEmpty(this.sereServPTTT.SUBS_HEAD_LOGINNAME) && lstReAcsUserADO.Exists(o => o.LOGINNAME == this.sereServPTTT.SUBS_HEAD_LOGINNAME))
                    {
                        cboEndDeptSubs.EditValue = this.sereServPTTT.SUBS_HEAD_LOGINNAME;
                    }
                }
                else if (this.sereServ != null && !this.sereServ.EKIP_ID.HasValue)
                {
                    this.txtMANNER.Text = this.sereServ.TDL_SERVICE_NAME;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void clearTabpage()
        {
            try
            {
                lstAllEkip = new List<HisEkipUserADO>();


                //for (int x = xtraTabControl2.TabPages.Count; x > 0; x--)
                //{
                //    if (x != 0)
                //    {
                //        xtraTabControl2.TabPages.Remove(xtraTabControl2.TabPages[x]);
                //    }
                //}

                for (int x = 1; x < xtraTabControl2.TabPages.Count; x++)
                {
                    xtraTabControl2.TabPages.Remove(xtraTabControl2.TabPages[x]);
                    x--;
                }

                ucEkip.FillDataToGrid(lstAllEkip);
                ucEkip.FillDataToInformationSurg(false);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void createTabPage(List<PtttMethodADO> lstData)
        {
            try
            {
                foreach (var item in lstData)
                {
                    if (item.EkipUsersADO != null && item.EkipUsersADO.listEkipUser != null && item.EkipUsersADO.listEkipUser.Count > 0)
                    {
                        EkipUsersADO ekipADO = new EkipUsersADO();
                        DevExpress.XtraEditors.PanelControl panel = new DevExpress.XtraEditors.PanelControl();
                        panel.Dock = System.Windows.Forms.DockStyle.Fill;
                        panel.Name = "panel_" + item.ID;
                        DevExpress.XtraTab.XtraTabPage tab = new DevExpress.XtraTab.XtraTabPage();
                        tab.Controls.Add(panel);
                        tab.Text = item.PTTT_METHOD_NAME;
                        tab.Name = item.ID.ToString();
                        xtraTabControl2.TabPages.Add(tab);
                        UCEkipUser uc = new UCEkipUser();
                        panel.Controls.Add(uc);
                        uc.Dock = DockStyle.Fill;

                        foreach (var ekip in item.EkipUsersADO.listEkipUser)
                        {
                            HisEkipUserADO ado = new HisEkipUserADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HisEkipUserADO>(ado, ekip);
                            lstAllEkip.Add(ado);
                        }
                        uc.FillDataToGrid(item.EkipUsersADO.listEkipUser);
                        //uc.SetDepartmentID(Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue != null ? cboDepartment.EditValue.ToString() : "0"));
                        uc.FillDataToInformationSurg(false);
                    }

                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }


        private async void ProcessLoadRealPtttMethod(V_HIS_SERE_SERV_PTTT dataPttt)
        {
            try
            {
                layoutControlItem25.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                txtPhuongPhapTT.Text = "";
                cboPhuongPhapThucTe.EditValue = null;
                if (dataPttt == null)
                {
                    return;
                }
                if (dataPttt.REAL_PTTT_METHOD_ID > 0)
                {
                    var data = BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(p => p.ID == dataPttt.REAL_PTTT_METHOD_ID);
                    if (data != null)
                    {
                        txtPhuongPhapTT.Text = data.PTTT_METHOD_CODE;
                        cboPhuongPhapThucTe.EditValue = data.ID;
                    }
                }
                else if (this.listSesePtttMetod.Exists(o => o.SERE_SERV_ID == dataPttt.SERE_SERV_ID))
                {
                    layoutControlItem25.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    List<string> pp = new List<string>();
                    var lstPtttMethod = this.listSesePtttMetod.Where(o => o.SERE_SERV_ID == dataPttt.SERE_SERV_ID).ToList();
                    if (lstPtttMethod != null && lstPtttMethod.Count > 0)
                    {
                        foreach (var item in lstPtttMethod)
                        {
                            pp.Add(string.Format("{0} - {1}", item.PTTT_METHOD_NAME, item.AMOUNT));
                        }
                    }
                    txtPhuongPhapTT.Text = string.Join("; ", pp);
                    if (IsChoosePTTT)
                    {
                        clearTabpage();
                    }
                    createTabPage(lstPtttMethod);
                    if (IsChoosePTTT)
                    {
                        ucEkip.FillDataToGrid(lstAllEkip);
                    }
                    ucEkip.FillDataToInformationSurg(false);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void AddDataToGridEkip(long? id)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal async void FillDataDefaultToControl()
        {
            try
            {
                if (this.sereServ != null)
                {
                    lblIntructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(sereServ.TDL_INTRUCTION_TIME);
                    CommonParam param = new CommonParam();
                    List<HisEkipUserADO> ekipUserAdos = new List<HisEkipUserADO>();
                    if (this.sereServ.EKIP_ID.HasValue)
                    {
                        HisEkipUserViewFilter hisEkipUserFilter = new HisEkipUserViewFilter();
                        hisEkipUserFilter.EKIP_ID = this.sereServ.EKIP_ID;
                        hisEkipUserFilter.ORDER_DIRECTION = "ASC";
                        hisEkipUserFilter.ORDER_FIELD = "ID";
                        var lst = new BackendAdapter(param)
                .Get<List<V_HIS_EKIP_USER>>(HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumers.MosConsumer, hisEkipUserFilter, param);
                        lst = lst.Where(o => o.IS_ACTIVE == 1).ToList();
                        if (lst.Count > 0)
                        {
                            foreach (var item in lst)
                            {
                                var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == item.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                                if (dataCheck == null || dataCheck.ID == 0)
                                    continue;
                                Mapper.CreateMap<V_HIS_EKIP_USER, HisEkipUserADO>();
                                var HisEkipUserProcessing = Mapper.Map<V_HIS_EKIP_USER, HisEkipUserADO>(item);
                                SetDepartment(HisEkipUserProcessing);
                                ekipUserAdos.Add(HisEkipUserProcessing);
                            }
                            ekipUserAdos.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                            ekipUserAdos.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            ucEkip.FillDataToGrid(ekipUserAdos);
                        }
                    }
                    else if (this.serviceReq.EKIP_PLAN_ID.HasValue) //tiennv
                    {
                        HisEkipPlanUserViewFilter hisEkipPlanUserFilter = new HisEkipPlanUserViewFilter();
                        hisEkipPlanUserFilter.EKIP_PLAN_ID = this.serviceReq.EKIP_PLAN_ID;
                        var lst = new BackendAdapter(param)
                .Get<List<V_HIS_EKIP_PLAN_USER>>("api/HisEkipPlanUser/GetView", ApiConsumers.MosConsumer, hisEkipPlanUserFilter, param);

                        if (lst.Count > 0)
                        {
                            foreach (var item in lst)
                            {
                                var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == item.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                                if (dataCheck == null || dataCheck.ID == 0)
                                    continue;
                                Mapper.CreateMap<V_HIS_EKIP_PLAN_USER, HisEkipUserADO>();
                                var HisEkipUserProcessing = Mapper.Map<V_HIS_EKIP_PLAN_USER, HisEkipUserADO>(item);
                                SetDepartment(HisEkipUserProcessing);
                                ekipUserAdos.Add(HisEkipUserProcessing);
                            }
                            ucEkip.FillDataToGrid(ekipUserAdos);

                        }
                    }

                    if (this.sereServ.PACKAGE_ID.HasValue)
                    {
                        HIS_PACKAGE package = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PACKAGE>().FirstOrDefault(o => o.ID == this.sereServ.PACKAGE_ID.Value);
                        lblPackageType.Text = package != null ? package.PACKAGE_NAME : "";
                    }
                    else
                    {
                        lblPackageType.Text = "";
                    }
                }
                else
                {
                    txtConclude.Text = "";
                    txtDescription.Text = "";
                    txtResultNote.Text = "";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async void LoadTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = serviceReq.TREATMENT_ID;
                this.vhisTreatment = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async void LoadPatient()
        {
            try
            {
                HisPatientFilter patientFilter = new HisPatientFilter();
                patientFilter.ID = vhisTreatment.PATIENT_ID;
                this.Patient = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, new CommonParam()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async void SetEnableControl()
        {
            try
            {
                if (this.sereServ != null)
                {
                    LogSystem.Debug("SetEnableControl.1");
                    if (this.serviceReq != null && this.serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        btnSurgAssignAndCopy.Enabled = true;
                    }
                    else
                    {
                        btnSurgAssignAndCopy.Enabled = false;
                    }
                    #region Old
                    //if (HisConfigKeys.CheckPermissonOption != "1")
                    //{
                    //    if ((this.sereServ.IS_NO_EXECUTE != null || this.serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT) || this.vhisTreatment.IS_PAUSE == 1)
                    //    {
                    //        LogSystem.Debug("SetEnableControl.2");
                    //        if (this.isAllowEditInfo)
                    //        {
                    //            LogSystem.Debug("SetEnableControl.3");
                    //            btnSwapService.Enabled = false;
                    //            dropDownButtonGPBL.Enabled = false;
                    //            btnDepartmentTran.Enabled = false;
                    //        }
                    //        else
                    //        {
                    //            LogSystem.Debug("SetEnableControl.4");

                    //            ReadOnlyICD(true, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                    //            ReadOnlyICD(true, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                    //            ReadOnlyICD(true, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                    //            ReadOnlyICD(true, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);

                    //            txtIcdCmSubCode.ReadOnly = true;
                    //            txtIcdCmSubName.ReadOnly = true;
                    //            txtLoaiPT.ReadOnly = true;
                    //            cboLoaiPT.ReadOnly = true;
                    //            txtBanMoCode.ReadOnly = true;
                    //            cboBanMo.ReadOnly = true;
                    //            txtPhuongPhap2.ReadOnly = true;
                    //            cboPhuongPhap2.ReadOnly = true;
                    //            txtPhuongPhapTT.ReadOnly = true;
                    //            cboPhuongPhapThucTe.ReadOnly = true;
                    //            txtKQVoCam.ReadOnly = true;
                    //            cboKQVoCam.ReadOnly = true;
                    //            txtMachineCode.ReadOnly = true;
                    //            cboMachine.ReadOnly = true;
                    //            txtMoKTCao.ReadOnly = true;
                    //            cboMoKTCao.ReadOnly = true;

                    //            cboEkipTemp.ReadOnly = true;
                    //            txtIcdExtraCode.ReadOnly = true;
                    //            txtIcdText.ReadOnly = true;
                    //            txtPtttGroupCode.ReadOnly = true;
                    //            cbbPtttGroup.ReadOnly = true;
                    //            txtMethodCode.ReadOnly = true;
                    //            cboMethod.ReadOnly = true;
                    //            txtBlood.ReadOnly = true;
                    //            cbbBlood.ReadOnly = true;
                    //            txtEmotionlessMethod.ReadOnly = true;
                    //            cbbEmotionlessMethod.ReadOnly = true;
                    //            txtCondition.ReadOnly = true;
                    //            cboCondition.ReadOnly = true;
                    //            txtBloodRh.ReadOnly = true;
                    //            cbbBloodRh.ReadOnly = true;
                    //            txtCatastrophe.ReadOnly = true;
                    //            cboCatastrophe.ReadOnly = true;
                    //            txtDeathSurg.ReadOnly = true;
                    //            cboDeathSurg.ReadOnly = true;
                    //            dtStart.ReadOnly = true;
                    //            dtFinish.ReadOnly = true;
                    //            txtMANNER.ReadOnly = true;
                    //            txtDescription.ReadOnly = true;
                    //            txtConclude.ReadOnly = true;
                    //            txtResultNote.ReadOnly = true;
                    //            cbbPtttGroup.Properties.Buttons[0].Enabled = false;
                    //            dtStart.Properties.Buttons[0].Enabled = false;
                    //            dtFinish.Properties.Buttons[0].Enabled = false;
                    //            cbbPtttGroup.Properties.Buttons[1].Enabled = false;
                    //            dtStart.Properties.Buttons[1].Enabled = false;
                    //            dtFinish.Properties.Buttons[1].Enabled = false;
                    //            ucEkip.SetEnableControl(true);
                    //            //lciInformationSurg.Enabled = false;                          
                    //            //btnSwapService.Enabled = false;
                    //            //btnTrackingCreate.Enabled = false;
                    //            //layoutControlItem18.Enabled = false;
                    //            //layoutControlItem22.Enabled = false;
                    //            //layoutControlItem24.Enabled = false;
                    //            //layoutControlItem16.Enabled = false;
                    //            //layoutControlItem20.Enabled = false;
                    //            //layoutControlItem26.Enabled = false;
                    //            //layoutControlItem12.Enabled = false;

                    //            btnSave.Enabled = false;
                    //            btnFinish.Enabled = false;
                    //            btnSaveEkipTemp.Enabled = false;
                    //        }

                    //        ddbPhatSinh.Enabled = false;
                    //        btnKTDT.Enabled = false;
                    //        btnAssignBlood.Enabled = false;
                    //        btnTuTruc.Enabled = false;
                    //        btnAssignPre.Enabled = false;
                    //    }
                    //    else if (!String.IsNullOrWhiteSpace(Config.HisConfigKeys.CheckPermisson) && Config.HisConfigKeys.CheckPermisson.Contains("1"))
                    //    {
                    //        btnKTDT.Enabled = false;
                    //        btnAssignBlood.Enabled = true;
                    //        btnTuTruc.Enabled = true;
                    //        //btnSwapService.Enabled = false;
                    //        ddbPhatSinh.Enabled = true;
                    //        btnAssignPre.Enabled = true;
                    //        btnSave.Enabled = true;
                    //        btnFinish.Enabled = !(HisConfigKeys.allowFinishWhenAccountIsDoctor == "1" && BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).FirstOrDefault().IS_DOCTOR != 1);
                    //        //btnTrackingCreate.Enabled = true;

                    //        txtPtttGroupCode.ReadOnly = false;
                    //        cbbPtttGroup.ReadOnly = false;
                    //        cbbPtttGroup.Properties.Buttons[0].Enabled = true;
                    //        cbbPtttGroup.Properties.Buttons[1].Enabled = true;

                    //        //#17292
                    //        bool isDoctor = false;
                    //        var _employee = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>().FirstOrDefault(p => p.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                    //        if (_employee != null && _employee.IS_DOCTOR == (short)1)
                    //        {
                    //            isDoctor = true;
                    //        }

                    //        if (isDoctor)
                    //        {
                    //            txtMANNER.ReadOnly = false;
                    //            txtDescription.ReadOnly = false;
                    //            txtConclude.ReadOnly = false;
                    //            txtResultNote.ReadOnly = false;

                    //            txtMachineCode.ReadOnly = true;
                    //            cboMachine.ReadOnly = true;
                    //            txtLoaiPT.ReadOnly = true;
                    //            cboLoaiPT.ReadOnly = true;
                    //            txtBanMoCode.ReadOnly = true;
                    //            cboBanMo.ReadOnly = true;
                    //            txtPhuongPhap2.ReadOnly = true;
                    //            cboPhuongPhap2.ReadOnly = true;
                    //            cboPhuongPhapThucTe.ReadOnly = true;
                    //            txtPhuongPhapTT.ReadOnly = true;
                    //            txtKQVoCam.ReadOnly = true;
                    //            cboKQVoCam.ReadOnly = true;
                    //            txtMoKTCao.ReadOnly = true;
                    //            cboMoKTCao.ReadOnly = true;

                    //            txtIcdExtraCode.ReadOnly = true;
                    //            txtIcdText.ReadOnly = true;
                    //            txtMethodCode.ReadOnly = true;
                    //            cboMethod.ReadOnly = true;
                    //            txtBlood.ReadOnly = true;
                    //            cbbBlood.ReadOnly = true;
                    //            txtEmotionlessMethod.ReadOnly = true;
                    //            cbbEmotionlessMethod.ReadOnly = true;
                    //            txtCondition.ReadOnly = true;
                    //            cboCondition.ReadOnly = true;
                    //            txtBloodRh.ReadOnly = true;
                    //            cbbBloodRh.ReadOnly = true;
                    //            txtCatastrophe.ReadOnly = true;
                    //            cboCatastrophe.ReadOnly = true;
                    //            txtDeathSurg.ReadOnly = true;
                    //            cboDeathSurg.ReadOnly = true;
                    //            dtStart.ReadOnly = true;
                    //            dtFinish.ReadOnly = true;
                    //            cboEkipTemp.ReadOnly = true;
                    //            btnSaveEkipTemp.Enabled = false;
                    //            dtStart.Properties.Buttons[0].Enabled = false;
                    //            dtFinish.Properties.Buttons[0].Enabled = false;
                    //            dtStart.Properties.Buttons[1].Enabled = false;
                    //            dtFinish.Properties.Buttons[1].Enabled = false;
                    //            ucEkip.SetEnableControl(true);

                    //            ReadOnlyICD(true, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                    //            ReadOnlyICD(true, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                    //            ReadOnlyICD(true, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                    //            ReadOnlyICD(true, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);

                    //            txtIcdCmSubCode.ReadOnly = true;
                    //            txtIcdCmSubName.ReadOnly = true;
                    //            txtLoaiPT.ReadOnly = true;
                    //            cboLoaiPT.ReadOnly = true;
                    //            txtBanMoCode.ReadOnly = true;
                    //            cboBanMo.ReadOnly = true;
                    //            txtPhuongPhap2.ReadOnly = true;
                    //            cboPhuongPhap2.ReadOnly = true;
                    //            txtPhuongPhapTT.ReadOnly = true;
                    //            cboPhuongPhapThucTe.ReadOnly = true;
                    //            txtKQVoCam.ReadOnly = true;
                    //            cboKQVoCam.ReadOnly = true;
                    //            txtMachineCode.ReadOnly = true;
                    //            cboMachine.ReadOnly = true;
                    //            txtMoKTCao.ReadOnly = true;
                    //            cboMoKTCao.ReadOnly = true;
                    //        }
                    //        else
                    //        {
                    //            txtMANNER.ReadOnly = true;
                    //            txtDescription.ReadOnly = true;
                    //            txtConclude.ReadOnly = true;
                    //            txtResultNote.ReadOnly = true;

                    //            txtMachineCode.ReadOnly = false;
                    //            cboMachine.ReadOnly = false;
                    //            txtLoaiPT.ReadOnly = false;
                    //            cboLoaiPT.ReadOnly = false;
                    //            txtBanMoCode.ReadOnly = false;
                    //            cboBanMo.ReadOnly = false;
                    //            txtPhuongPhap2.ReadOnly = false;
                    //            cboPhuongPhap2.ReadOnly = false;
                    //            txtPhuongPhapTT.ReadOnly = false;
                    //            cboPhuongPhapThucTe.ReadOnly = false;
                    //            txtKQVoCam.ReadOnly = false;
                    //            cboKQVoCam.ReadOnly = false;
                    //            txtMoKTCao.ReadOnly = false;
                    //            cboMoKTCao.ReadOnly = false;

                    //            txtIcdExtraCode.ReadOnly = false;
                    //            txtIcdText.ReadOnly = false;
                    //            txtMethodCode.ReadOnly = false;
                    //            cboMethod.ReadOnly = false;
                    //            txtBlood.ReadOnly = false;
                    //            cbbBlood.ReadOnly = false;
                    //            txtEmotionlessMethod.ReadOnly = false;
                    //            cbbEmotionlessMethod.ReadOnly = false;
                    //            txtCondition.ReadOnly = false;
                    //            cboCondition.ReadOnly = false;
                    //            txtBloodRh.ReadOnly = false;
                    //            cbbBloodRh.ReadOnly = false;
                    //            txtCatastrophe.ReadOnly = false;
                    //            cboCatastrophe.ReadOnly = false;
                    //            txtDeathSurg.ReadOnly = false;
                    //            cboDeathSurg.ReadOnly = false;
                    //            dtStart.ReadOnly = false;
                    //            dtFinish.ReadOnly = false;
                    //            cboEkipTemp.ReadOnly = false;
                    //            btnSaveEkipTemp.Enabled = true;
                    //            dtStart.Properties.Buttons[0].Enabled = true;
                    //            dtFinish.Properties.Buttons[0].Enabled = true;
                    //            dtStart.Properties.Buttons[1].Enabled = true;
                    //            dtFinish.Properties.Buttons[1].Enabled = true;
                    //            ucEkip.SetEnableControl(false);

                    //            ReadOnlyICD(false, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                    //            ReadOnlyICD(false, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                    //            ReadOnlyICD(false, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                    //            ReadOnlyICD(false, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);

                    //            txtIcdCmSubCode.ReadOnly = false;
                    //            txtIcdCmSubName.ReadOnly = false;
                    //            txtLoaiPT.ReadOnly = false;
                    //            cboLoaiPT.ReadOnly = false;
                    //            txtBanMoCode.ReadOnly = false;
                    //            cboBanMo.ReadOnly = false;
                    //            txtPhuongPhap2.ReadOnly = false;
                    //            cboPhuongPhap2.ReadOnly = false;
                    //            txtPhuongPhapTT.ReadOnly = false;
                    //            cboPhuongPhapThucTe.ReadOnly = false;
                    //            txtKQVoCam.ReadOnly = false;
                    //            cboKQVoCam.ReadOnly = false;
                    //            txtMachineCode.ReadOnly = false;
                    //            cboMachine.ReadOnly = false;
                    //            txtMoKTCao.ReadOnly = false;
                    //            cboMoKTCao.ReadOnly = false;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        txtIcdExtraCode.ReadOnly = false;
                    //        txtIcdText.ReadOnly = false;
                    //        txtPtttGroupCode.ReadOnly = false;
                    //        cbbPtttGroup.ReadOnly = false;
                    //        txtMethodCode.ReadOnly = false;
                    //        cboMethod.ReadOnly = false;
                    //        txtBlood.ReadOnly = false;
                    //        cbbBlood.ReadOnly = false;
                    //        txtEmotionlessMethod.ReadOnly = false;
                    //        cbbEmotionlessMethod.ReadOnly = false;
                    //        txtCondition.ReadOnly = false;
                    //        cboCondition.ReadOnly = false;
                    //        txtBloodRh.ReadOnly = false;
                    //        cbbBloodRh.ReadOnly = false;
                    //        txtCatastrophe.ReadOnly = false;
                    //        cboCatastrophe.ReadOnly = false;
                    //        txtDeathSurg.ReadOnly = false;
                    //        cboDeathSurg.ReadOnly = false;
                    //        dtStart.ReadOnly = false;
                    //        dtFinish.ReadOnly = false;
                    //        txtMANNER.ReadOnly = false;
                    //        txtDescription.ReadOnly = false;
                    //        txtConclude.ReadOnly = false;
                    //        txtResultNote.ReadOnly = false;
                    //        //lciInformationSurg.Enabled = true;
                    //        btnKTDT.Enabled = false;
                    //        btnAssignBlood.Enabled = true;
                    //        btnTuTruc.Enabled = true;
                    //        //btnSwapService.Enabled = false;
                    //        ddbPhatSinh.Enabled = true;
                    //        btnAssignPre.Enabled = true;
                    //        btnSave.Enabled = true;
                    //        btnFinish.Enabled = !(HisConfigKeys.allowFinishWhenAccountIsDoctor == "1" && BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).FirstOrDefault().IS_DOCTOR != 1);
                    //        cboEkipTemp.ReadOnly = false;
                    //        btnSaveEkipTemp.Enabled = true;
                    //        //btnTrackingCreate.Enabled = true;

                    //        cbbPtttGroup.Properties.Buttons[0].Enabled = true;
                    //        dtStart.Properties.Buttons[0].Enabled = true;
                    //        dtFinish.Properties.Buttons[0].Enabled = true;
                    //        cbbPtttGroup.Properties.Buttons[1].Enabled = true;
                    //        dtStart.Properties.Buttons[1].Enabled = true;
                    //        dtFinish.Properties.Buttons[1].Enabled = true;
                    //        ucEkip.SetEnableControl(false);
                    //        //layoutControlItem18.Enabled = true;
                    //        //layoutControlItem22.Enabled = true;
                    //        //layoutControlItem16.Enabled = true;
                    //        //layoutControlItem24.Enabled = true;
                    //        //layoutControlItem20.Enabled = true;
                    //        //layoutControlItem26.Enabled = true;
                    //        //layoutControlItem12.Enabled = true;
                    //        ReadOnlyICD(false, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                    //        ReadOnlyICD(false, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                    //        ReadOnlyICD(false, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                    //        ReadOnlyICD(false, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);

                    //        txtIcdCmSubCode.ReadOnly = false;
                    //        txtIcdCmSubName.ReadOnly = false;
                    //        txtLoaiPT.ReadOnly = false;
                    //        cboLoaiPT.ReadOnly = false;
                    //        txtBanMoCode.ReadOnly = false;
                    //        cboBanMo.ReadOnly = false;
                    //        txtPhuongPhap2.ReadOnly = false;
                    //        cboPhuongPhap2.ReadOnly = false;
                    //        txtPhuongPhapTT.ReadOnly = false;
                    //        cboPhuongPhapThucTe.ReadOnly = false;
                    //        txtKQVoCam.ReadOnly = false;
                    //        cboKQVoCam.ReadOnly = false;
                    //        txtMachineCode.ReadOnly = false;
                    //        cboMachine.ReadOnly = false;
                    //        txtMoKTCao.ReadOnly = false;
                    //        cboMoKTCao.ReadOnly = false;
                    //    }
                    //}
                    //else
                    #endregion
                    if (serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        if (this.isAllowEditInfo)
                        {
                            LogSystem.Debug("SetEnableControl.1.1");
                            btnSwapService.Enabled = false;
                            dropDownButtonGPBL.Enabled = false;
                            btnDepartmentTran.Enabled = false;
                        }
                        else
                        {
                            LogSystem.Debug("SetEnableControl.1.2");

                            ReadOnlyICD(true, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                            ReadOnlyICD(true, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                            ReadOnlyICD(true, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                            ReadOnlyICD(true, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);

                            txtIcdCmSubCode.ReadOnly = true;
                            txtIcdCmSubName.ReadOnly = true;
                            txtLoaiPT.ReadOnly = true;
                            cboLoaiPT.ReadOnly = true;
                            txtBanMoCode.ReadOnly = true;
                            cboBanMo.ReadOnly = true;
                            txtPhuongPhap2.ReadOnly = true;
                            cboPhuongPhap2.ReadOnly = true;
                            txtPhuongPhapTT.ReadOnly = true;
                            cboPhuongPhapThucTe.ReadOnly = true;
                            txtKQVoCam.ReadOnly = true;
                            cboKQVoCam.ReadOnly = true;
                            txtMachineCode.ReadOnly = true;
                            cboMachine.ReadOnly = true;
                            txtMoKTCao.ReadOnly = true;
                            cboMoKTCao.ReadOnly = true;

                            cboEkipTemp.ReadOnly = true;
                            txtIcdExtraCode.ReadOnly = true;
                            txtIcdText.ReadOnly = true;
                            txtPtttGroupCode.ReadOnly = true;
                            cbbPtttGroup.ReadOnly = true;
                            txtMethodCode.ReadOnly = true;
                            cboMethod.ReadOnly = true;
                            txtBlood.ReadOnly = true;
                            cbbBlood.ReadOnly = true;
                            txtEmotionlessMethod.ReadOnly = true;
                            cbbEmotionlessMethod.ReadOnly = true;
                            txtCondition.ReadOnly = true;
                            cboCondition.ReadOnly = true;
                            txtBloodRh.ReadOnly = true;
                            cbbBloodRh.ReadOnly = true;
                            txtCatastrophe.ReadOnly = true;
                            cboCatastrophe.ReadOnly = true;
                            txtDeathSurg.ReadOnly = true;
                            cboDeathSurg.ReadOnly = true;
                            dtStart.ReadOnly = true;
                            dtFinish.ReadOnly = true;
                            txtMANNER.ReadOnly = true;
                            txtDescription.ReadOnly = true;
                            txtConclude.ReadOnly = true;
                            txtResultNote.ReadOnly = true;
                            cbbPtttGroup.Properties.Buttons[0].Enabled = false;
                            dtStart.Properties.Buttons[0].Enabled = false;
                            dtFinish.Properties.Buttons[0].Enabled = false;
                            cbbPtttGroup.Properties.Buttons[1].Enabled = false;
                            dtStart.Properties.Buttons[1].Enabled = false;
                            dtFinish.Properties.Buttons[1].Enabled = false;
                            ucEkip.SetEnableControl(true);
                            //lciInformationSurg.Enabled = false;                          
                            //btnSwapService.Enabled = false;
                            //btnTrackingCreate.Enabled = false;
                            //layoutControlItem18.Enabled = false;
                            //layoutControlItem22.Enabled = false;
                            //layoutControlItem24.Enabled = false;
                            //layoutControlItem16.Enabled = false;
                            //layoutControlItem20.Enabled = false;
                            //layoutControlItem26.Enabled = false;
                            //layoutControlItem12.Enabled = false;

                            btnSave.Enabled = false;
                            btnFinish.Enabled = false;
                            btnSaveEkipTemp.Enabled = false;
                        }

                        ddbPhatSinh.Enabled = false;
                        btnKTDT.Enabled = false;
                        btnAssignBlood.Enabled = false;
                        btnTuTruc.Enabled = false;
                        btnAssignPre.Enabled = false;
                        btnPresYhct.Enabled = false;
                    }
                    else if (HisConfigKeys.CheckPermissonOption == "1" && this.serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT)
                    {
                        var _employee = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>().FirstOrDefault(p => p.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                        if (_employee.DEPARTMENT_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.Module.RoomId).DepartmentId)
                        {
                            txtDescription.ReadOnly = true;
                            txtResultNote.ReadOnly = true;
                            cardControl.Enabled = false;
                            btnImagePublic.Enabled = false;
                            btnCreateImageLuuDo.Enabled = false;
                            btnChonMauPTCHuyenKhoaMat.Enabled = true;
                            btnChoosePtttMethods.Enabled = true;
                            txtIntructionNote.ReadOnly = false;
                            txtIntructionNote.ReadOnly = false;
                            txtPtttGroupCode.ReadOnly = false;
                            cbbPtttGroup.ReadOnly = false;

                            txtMANNER.ReadOnly = false;
                            txtMachineCode.ReadOnly = false;
                            cboMachine.ReadOnly = false;
                            txtLoaiPT.ReadOnly = false;
                            cboLoaiPT.ReadOnly = false;
                            txtBanMoCode.ReadOnly = false;
                            cboBanMo.ReadOnly = false;
                            txtPhuongPhap2.ReadOnly = false;
                            cboPhuongPhap2.ReadOnly = false;
                            txtPhuongPhapTT.ReadOnly = false;
                            cboPhuongPhapThucTe.ReadOnly = false;
                            txtKQVoCam.ReadOnly = false;
                            cboKQVoCam.ReadOnly = false;
                            txtMoKTCao.ReadOnly = false;
                            cboMoKTCao.ReadOnly = false;

                            txtIcdExtraCode.ReadOnly = false;
                            txtIcdText.ReadOnly = false;
                            txtMethodCode.ReadOnly = false;
                            cboMethod.ReadOnly = false;
                            txtBlood.ReadOnly = false;
                            cbbBlood.ReadOnly = false;
                            txtEmotionlessMethod.ReadOnly = false;
                            cbbEmotionlessMethod.ReadOnly = false;
                            txtCondition.ReadOnly = false;
                            cboCondition.ReadOnly = false;
                            txtBloodRh.ReadOnly = false;
                            cbbBloodRh.ReadOnly = false;
                            txtCatastrophe.ReadOnly = false;
                            cboCatastrophe.ReadOnly = false;
                            txtDeathSurg.ReadOnly = false;
                            cboDeathSurg.ReadOnly = false;
                            dtStart.ReadOnly = false;
                            dtFinish.ReadOnly = false;
                            cboEkipTemp.ReadOnly = false;
                            btnSaveEkipTemp.Enabled = true;
                            dtStart.Properties.Buttons[0].Enabled = true;
                            dtFinish.Properties.Buttons[0].Enabled = true;
                            dtStart.Properties.Buttons[1].Enabled = true;
                            dtFinish.Properties.Buttons[1].Enabled = true;
                            cboBanMo.Properties.Buttons[1].Enabled = true;
                            cboPhuongPhap2.Properties.Buttons[1].Enabled = true;
                            cbbBlood.Properties.Buttons[1].Enabled = true;
                            cbbBloodRh.Properties.Buttons[1].Enabled = true;
                            cboKQVoCam.Properties.Buttons[1].Enabled = true;
                            cboMoKTCao.Properties.Buttons[1].Enabled = true;
                            cboDeathSurg.Properties.Buttons[1].Enabled = true;
                            cboMachine.Properties.Buttons[1].Enabled = true;
                            cboDepartment.Properties.Buttons[1].Enabled = true;
                            cboMethod.Properties.Buttons[1].Enabled = true;
                            cbbPtttGroup.Properties.Buttons[1].Enabled = true;
                            cboCondition.Properties.Buttons[1].Enabled = true;
                            cboCatastrophe.Properties.Buttons[1].Enabled = true;
                            ucEkip.SetEnableControl(false);
                            IsReadOnlyGridViewEkipUser = false;
                            ReadOnlyICD(false, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                            ReadOnlyICD(false, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                            ReadOnlyICD(false, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                            ReadOnlyICD(false, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);

                            txtIcdCmSubCode.ReadOnly = false;
                            txtIcdCmSubName.ReadOnly = false;
                            txtLoaiPT.ReadOnly = false;
                            cboLoaiPT.ReadOnly = false;
                            txtBanMoCode.ReadOnly = false;
                            cboBanMo.ReadOnly = false;
                            txtPhuongPhap2.ReadOnly = false;
                            cboPhuongPhap2.ReadOnly = false;
                            txtPhuongPhapTT.ReadOnly = false;
                            cboPhuongPhapThucTe.ReadOnly = false;
                            txtKQVoCam.ReadOnly = false;
                            cboKQVoCam.ReadOnly = false;
                            txtMachineCode.ReadOnly = false;
                            cboMachine.ReadOnly = false;
                            txtMoKTCao.ReadOnly = false;
                            cboMoKTCao.ReadOnly = false;


                            btnSave.Enabled = true;
                            btnAssignBlood.Enabled = true;
                            btnTuTruc.Enabled = true;
                            btnAssignPre.Enabled = true;
                            btnPresYhct.Enabled = true;
                            dropDownButtonGPBL.Enabled = true;
                            ddbPhatSinh.Enabled = true;
                            btnSwapService.Enabled = true;
                            btnDepartmentTran.Enabled = true;
                            btnOther.Enabled = true;
                            btnPrint.Enabled = true;
                            btnKTDT.Enabled = true;
                            btnFinish.Enabled = !(HisConfigKeys.allowFinishWhenAccountIsDoctor == "1" && BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).FirstOrDefault().IS_DOCTOR != 1);
                        }
                        else
                        {
                            txtDescription.ReadOnly = false;
                            cardControl.Enabled = true;
                            cardControl.Enabled = true;
                            btnImagePublic.Enabled = true;
                            btnCreateImageLuuDo.Enabled = true;
                            btnChonMauPTCHuyenKhoaMat.Enabled = false;
                            txtConclude.ReadOnly = true;
                            txtResultNote.ReadOnly = false;
                            btnSave.Enabled = true;
                            ReadOnlyICD(false, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                            ReadOnlyICD(false, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                            ReadOnlyICD(false, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                            ReadOnlyICD(false, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);
                            txtIcdCmSubCode.ReadOnly = false;
                            txtIcdCmSubName.ReadOnly = false;
                            txtIcdExtraCode.ReadOnly = false;
                            txtIcdText.ReadOnly = false;
                            txtIntructionNote.ReadOnly = true;
                            txtPtttGroupCode.ReadOnly = true;
                            cbbPtttGroup.ReadOnly = true;
                            btnChoosePtttMethods.Enabled = false;
                            txtMANNER.ReadOnly = true;
                            txtMachineCode.ReadOnly = true;
                            cboMachine.ReadOnly = true;
                            txtLoaiPT.ReadOnly = true;
                            cboLoaiPT.ReadOnly = true;
                            txtBanMoCode.ReadOnly = true;
                            cboBanMo.ReadOnly = true;
                            txtPhuongPhap2.ReadOnly = true;
                            cboPhuongPhap2.ReadOnly = true;
                            txtPhuongPhapTT.ReadOnly = true;
                            cboPhuongPhapThucTe.ReadOnly = true;
                            txtKQVoCam.ReadOnly = true;
                            cboKQVoCam.ReadOnly = true;
                            txtMoKTCao.ReadOnly = true;
                            cboMoKTCao.ReadOnly = true;
                            txtMethodCode.ReadOnly = true;
                            cboMethod.ReadOnly = true;
                            txtBlood.ReadOnly = true;
                            cbbBlood.ReadOnly = true;
                            txtEmotionlessMethod.ReadOnly = true;
                            cbbEmotionlessMethod.ReadOnly = true;
                            txtCondition.ReadOnly = true;
                            cboCondition.ReadOnly = true;
                            txtBloodRh.ReadOnly = true;
                            cbbBloodRh.ReadOnly = true;
                            txtCatastrophe.ReadOnly = true;
                            cboCatastrophe.ReadOnly = true;
                            txtDeathSurg.ReadOnly = true;
                            cboDeathSurg.ReadOnly = true;
                            dtStart.ReadOnly = true;
                            dtFinish.ReadOnly = true;
                            cboEkipTemp.ReadOnly = true;
                            btnSaveEkipTemp.Enabled = false;
                            dtStart.Properties.Buttons[0].Enabled = false;
                            dtFinish.Properties.Buttons[0].Enabled = false;
                            dtStart.Properties.Buttons[1].Enabled = false;
                            dtFinish.Properties.Buttons[1].Enabled = false;
                            cboBanMo.Properties.Buttons[1].Enabled = false;
                            cboPhuongPhap2.Properties.Buttons[1].Enabled = false;
                            cbbBlood.Properties.Buttons[1].Enabled = false;
                            cbbBloodRh.Properties.Buttons[1].Enabled = false;
                            cboKQVoCam.Properties.Buttons[1].Enabled = false;
                            cboMoKTCao.Properties.Buttons[1].Enabled = false;
                            cboDeathSurg.Properties.Buttons[1].Enabled = false;
                            cboMachine.Properties.Buttons[1].Enabled = false;
                            cboDepartment.Properties.Buttons[1].Enabled = false;
                            cboMethod.Properties.Buttons[1].Enabled = false;
                            cbbPtttGroup.Properties.Buttons[1].Enabled = false;
                            cboCondition.Properties.Buttons[1].Enabled = false;
                            cboCatastrophe.Properties.Buttons[1].Enabled = false;
                            ucEkip.SetEnableControl(true);

                            IsReadOnlyGridViewEkipUser = true;
                            txtLoaiPT.ReadOnly = true;
                            cboLoaiPT.ReadOnly = true;
                            txtBanMoCode.ReadOnly = true;
                            cboBanMo.ReadOnly = true;
                            txtPhuongPhap2.ReadOnly = true;
                            cboPhuongPhap2.ReadOnly = true;
                            txtPhuongPhapTT.ReadOnly = true;
                            cboPhuongPhapThucTe.ReadOnly = true;
                            txtKQVoCam.ReadOnly = true;
                            cboKQVoCam.ReadOnly = true;
                            txtMachineCode.ReadOnly = true;
                            cboMachine.ReadOnly = true;
                            txtMoKTCao.ReadOnly = true;
                            cboMoKTCao.ReadOnly = true;

                            chkSaveGroup.ReadOnly = true;


                            btnAssignBlood.Enabled = false;
                            btnTuTruc.Enabled = false;
                            btnAssignPre.Enabled = false;
                            btnPresYhct.Enabled = false;
                            dropDownButtonGPBL.Enabled = false;
                            ddbPhatSinh.Enabled = false;
                            btnSwapService.Enabled = false;
                            btnDepartmentTran.Enabled = false;
                            btnOther.Enabled = false;
                            btnPrint.Enabled = false;
                            btnKTDT.Enabled = false;

                            spinExcuteTimeAdd.ReadOnly = true;
                            cboDepartment.ReadOnly = true;
                            btnFinish.Enabled = false;
                        }
                    }
                    else if (HisConfigKeys.CheckPermissonOption == "2" && this.serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT)
                    {
                        btnSave.Enabled = true;
                        var enable = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).FirstOrDefault().DEPARTMENT_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.Module.RoomId).DepartmentId;
                        if (enable)
                        {
                            txtDescription.ReadOnly = !enable;
                            txtResultNote.ReadOnly = !enable;
                            cardControl.Enabled = true;
                            btnImagePublic.Enabled = true;
                            btnCreateImageLuuDo.Enabled = true;
                            btnChonMauPTCHuyenKhoaMat.Enabled = true;
                            txtIntructionNote.ReadOnly = !enable;
                            txtMANNER.ReadOnly = !enable;
                            txtConclude.ReadOnly = !enable;
                            ReadOnlyICD(!enable, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                            ReadOnlyICD(!enable, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                            ReadOnlyICD(!enable, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                            ReadOnlyICD(!enable, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);
                            txtIcdCmSubCode.ReadOnly = !enable;
                            txtIcdCmSubName.ReadOnly = !enable;
                            txtIcdExtraCode.ReadOnly = !enable;
                            txtIcdText.ReadOnly = !enable;

                            txtBanMoCode.ReadOnly = !enable;
                            cboBanMo.ReadOnly = !enable;
                            txtPhuongPhap2.ReadOnly = !enable;
                            cboPhuongPhap2.ReadOnly = !enable;
                            txtPhuongPhapTT.ReadOnly = !enable;
                            cboPhuongPhapThucTe.ReadOnly = !enable;
                            cboPhuongPhapThucTe.Enabled = true;
                            txtPhuongPhapTT.Enabled = true;
                            btnChoosePtttMethods.Enabled = enable;
                            txtBlood.ReadOnly = !enable;
                            cbbBlood.ReadOnly = !enable;
                            txtBloodRh.ReadOnly = !enable;
                            cbbBloodRh.ReadOnly = !enable;

                            txtKQVoCam.ReadOnly = !enable;
                            cboKQVoCam.ReadOnly = !enable;
                            txtCondition.ReadOnly = !enable;
                            cboCondition.ReadOnly = !enable;
                            txtMoKTCao.ReadOnly = !enable;
                            cboMoKTCao.ReadOnly = !enable;
                            txtCatastrophe.ReadOnly = !enable;
                            cboCatastrophe.ReadOnly = !enable;
                            txtDeathSurg.ReadOnly = !enable;
                            cboDeathSurg.ReadOnly = !enable;
                            txtMachineCode.ReadOnly = !enable;
                            cboMachine.ReadOnly = !enable;

                            txtPtttGroupCode.ReadOnly = !enable;
                            cbbPtttGroup.ReadOnly = !enable;
                            txtLoaiPT.ReadOnly = !enable;
                            cboLoaiPT.ReadOnly = !enable;
                            txtMethodCode.ReadOnly = !enable;
                            cboMethod.ReadOnly = !enable;
                            txtEmotionlessMethod.ReadOnly = !enable;
                            cbbEmotionlessMethod.ReadOnly = !enable;
                            dtStart.ReadOnly = !enable;
                            dtFinish.ReadOnly = !enable;
                            dtStart.Properties.Buttons[0].Enabled = enable;
                            dtFinish.Properties.Buttons[0].Enabled = enable;
                            dtStart.Properties.Buttons[1].Enabled = enable;
                            dtFinish.Properties.Buttons[1].Enabled = enable;
                            cboBanMo.Properties.Buttons[1].Enabled = !enable;
                            cboPhuongPhap2.Properties.Buttons[1].Enabled = !enable;
                            cbbBlood.Properties.Buttons[1].Enabled = !enable;
                            cbbBloodRh.Properties.Buttons[1].Enabled = !enable;
                            cboKQVoCam.Properties.Buttons[1].Enabled = !enable;
                            cboMoKTCao.Properties.Buttons[1].Enabled = !enable;
                            cboDeathSurg.Properties.Buttons[1].Enabled = !enable;
                            cboMachine.Properties.Buttons[1].Enabled = !enable;
                            spinExcuteTimeAdd.ReadOnly = !enable;
                            cboDepartment.Properties.Buttons[1].Enabled = enable;
                            cboMethod.Properties.Buttons[1].Enabled = enable;
                            cbbPtttGroup.Properties.Buttons[1].Enabled = enable;
                            cboCondition.Properties.Buttons[1].Enabled = enable;
                            cboCatastrophe.Properties.Buttons[1].Enabled = enable;
                            cboEkipTemp.ReadOnly = !enable;
                            cboDepartment.ReadOnly = !enable;
                            ucEkip.SetEnableControl(!enable);
                            IsReadOnlyGridViewEkipUser = !enable;
                            chkSaveGroup.ReadOnly = !enable;

                            btnAssignBlood.Enabled = enable;
                            btnTuTruc.Enabled = enable;
                            btnAssignPre.Enabled = enable;
                            btnPresYhct.Enabled = enable;
                            dropDownButtonGPBL.Enabled = enable;
                            ddbPhatSinh.Enabled = enable;
                            btnSwapService.Enabled = enable;
                            btnDepartmentTran.Enabled = enable;
                            btnOther.Enabled = enable;
                            btnPrint.Enabled = enable;
                            btnKTDT.Enabled = enable;
                        }
                        else
                        {
                            btnSave.Enabled = true;
                            txtDescription.ReadOnly = false;
                            txtResultNote.ReadOnly = false;
                            cardControl.Enabled = true;
                            btnImagePublic.Enabled = true;
                            btnCreateImageLuuDo.Enabled = true;
                            btnChonMauPTCHuyenKhoaMat.Enabled = true;
                            txtIntructionNote.ReadOnly = false;
                            txtMANNER.ReadOnly = false;
                            txtConclude.ReadOnly = false;
                            btnChoosePtttMethods.Enabled = true;
                            ReadOnlyICD(false, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                            ReadOnlyICD(false, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                            ReadOnlyICD(false, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                            ReadOnlyICD(false, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);
                            txtIcdCmSubCode.ReadOnly = false;
                            txtIcdCmSubName.ReadOnly = false;
                            txtIcdExtraCode.ReadOnly = false;
                            txtIcdText.ReadOnly = false;

                            txtBanMoCode.ReadOnly = false;
                            cboBanMo.ReadOnly = false;
                            txtPhuongPhap2.ReadOnly = false;
                            cboPhuongPhap2.ReadOnly = false;
                            txtPhuongPhapTT.ReadOnly = false;
                            cboPhuongPhapThucTe.ReadOnly = false;
                            cboPhuongPhapThucTe.Enabled = true;
                            txtPhuongPhapTT.Enabled = true;
                            txtBlood.ReadOnly = false;
                            cbbBlood.ReadOnly = false;
                            txtBloodRh.ReadOnly = false;
                            cbbBloodRh.ReadOnly = false;

                            txtKQVoCam.ReadOnly = false;
                            cboKQVoCam.ReadOnly = false;
                            txtCondition.ReadOnly = false;
                            cboCondition.ReadOnly = false;
                            txtMoKTCao.ReadOnly = false;
                            cboMoKTCao.ReadOnly = false;
                            txtCatastrophe.ReadOnly = false;
                            cboCatastrophe.ReadOnly = false;
                            txtDeathSurg.ReadOnly = false;
                            cboDeathSurg.ReadOnly = false;
                            txtMachineCode.ReadOnly = false;
                            cboMachine.ReadOnly = false;

                            txtPtttGroupCode.ReadOnly = false;
                            cbbPtttGroup.ReadOnly = false;
                            txtPtttGroupCode.Enabled = true;
                            cbbPtttGroup.Enabled = true;
                            txtLoaiPT.ReadOnly = false;
                            cboLoaiPT.ReadOnly = false;
                            txtMethodCode.ReadOnly = false;
                            cboMethod.ReadOnly = false;
                            txtEmotionlessMethod.ReadOnly = false;
                            cbbEmotionlessMethod.ReadOnly = false;
                            dtStart.ReadOnly = false;
                            dtFinish.ReadOnly = false;
                            dtStart.Properties.Buttons[0].Enabled = true;
                            dtFinish.Properties.Buttons[0].Enabled = true;
                            dtStart.Properties.Buttons[1].Enabled = true;
                            dtFinish.Properties.Buttons[1].Enabled = true;
                            cboBanMo.Properties.Buttons[1].Enabled = true;
                            cboPhuongPhap2.Properties.Buttons[1].Enabled = true;
                            cbbBlood.Properties.Buttons[1].Enabled = true;
                            cbbBloodRh.Properties.Buttons[1].Enabled = true;
                            cboKQVoCam.Properties.Buttons[1].Enabled = true;
                            cboMoKTCao.Properties.Buttons[1].Enabled = true;
                            cboDeathSurg.Properties.Buttons[1].Enabled = true;
                            cboMachine.Properties.Buttons[1].Enabled = true;
                            cboDepartment.Properties.Buttons[1].Enabled = false;
                            cboMethod.Properties.Buttons[1].Enabled = true;
                            cbbPtttGroup.Properties.Buttons[1].Enabled = true;
                            cboCondition.Properties.Buttons[1].Enabled = true;
                            cboCatastrophe.Properties.Buttons[1].Enabled = true;
                            cboPhuongPhapThucTe.Properties.Buttons[1].Enabled = true;
                            spinExcuteTimeAdd.ReadOnly = false;
                            cboEkipTemp.ReadOnly = true;
                            cboDepartment.ReadOnly = true;
                            ucEkip.SetEnableControl(true);
                            layoutControlItem56.Enabled = false;
                            IsReadOnlyGridViewEkipUser = false;
                            chkSaveGroup.ReadOnly = false;

                            btnAssignBlood.Enabled = true;
                            btnTuTruc.Enabled = true;
                            btnAssignPre.Enabled = true;
                            btnPresYhct.Enabled = true;
                            dropDownButtonGPBL.Enabled = true;
                            ddbPhatSinh.Enabled = true;
                            btnSwapService.Enabled = true;
                            btnDepartmentTran.Enabled = true;
                            btnOther.Enabled = true;
                            btnPrint.Enabled = true;
                            btnKTDT.Enabled = true;
                        }
                    }
                    else if (HisConfigKeys.CheckPermissonOption != "1" && HisConfigKeys.CheckPermissonOption != "2")
                    {
                        btnSave.Enabled = true;
                        txtDescription.ReadOnly = false;
                        txtResultNote.ReadOnly = false;
                        cardControl.Enabled = true;
                        btnImagePublic.Enabled = true;
                        btnCreateImageLuuDo.Enabled = true;
                        btnChonMauPTCHuyenKhoaMat.Enabled = true;
                        txtIntructionNote.ReadOnly = false;
                        txtMANNER.ReadOnly = false;
                        txtConclude.ReadOnly = false;
                        btnChoosePtttMethods.Enabled = true;
                        ReadOnlyICD(false, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                        ReadOnlyICD(false, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                        ReadOnlyICD(false, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                        ReadOnlyICD(false, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);
                        txtIcdCmSubCode.ReadOnly = false;
                        txtIcdCmSubName.ReadOnly = false;
                        txtIcdExtraCode.ReadOnly = false;
                        txtIcdText.ReadOnly = false;

                        txtBanMoCode.ReadOnly = false;
                        cboBanMo.ReadOnly = false;
                        txtPhuongPhap2.ReadOnly = false;
                        cboPhuongPhap2.ReadOnly = false;
                        txtPhuongPhapTT.ReadOnly = false;
                        cboPhuongPhapThucTe.ReadOnly = false;
                        txtBlood.ReadOnly = false;
                        cbbBlood.ReadOnly = false;
                        txtBloodRh.ReadOnly = false;
                        cbbBloodRh.ReadOnly = false;

                        txtKQVoCam.ReadOnly = false;
                        cboKQVoCam.ReadOnly = false;
                        txtCondition.ReadOnly = false;
                        cboCondition.ReadOnly = false;
                        txtMoKTCao.ReadOnly = false;
                        cboMoKTCao.ReadOnly = false;
                        txtCatastrophe.ReadOnly = false;
                        cboCatastrophe.ReadOnly = false;
                        txtDeathSurg.ReadOnly = false;
                        cboDeathSurg.ReadOnly = false;
                        txtMachineCode.ReadOnly = false;
                        cboMachine.ReadOnly = false;

                        txtPtttGroupCode.ReadOnly = false;
                        cbbPtttGroup.ReadOnly = false;
                        txtLoaiPT.ReadOnly = false;
                        cboLoaiPT.ReadOnly = false;
                        txtMethodCode.ReadOnly = false;
                        cboMethod.ReadOnly = false;
                        txtEmotionlessMethod.ReadOnly = false;
                        cbbEmotionlessMethod.ReadOnly = false;
                        dtStart.ReadOnly = false;
                        dtFinish.ReadOnly = false;
                        dtStart.Properties.Buttons[0].Enabled = true;
                        dtFinish.Properties.Buttons[0].Enabled = true;
                        dtStart.Properties.Buttons[1].Enabled = true;
                        dtFinish.Properties.Buttons[1].Enabled = true;
                        cboBanMo.Properties.Buttons[1].Enabled = true;
                        cboPhuongPhap2.Properties.Buttons[1].Enabled = true;
                        cbbBlood.Properties.Buttons[1].Enabled = true;
                        cbbBloodRh.Properties.Buttons[1].Enabled = true;
                        cboKQVoCam.Properties.Buttons[1].Enabled = true;
                        cboMoKTCao.Properties.Buttons[1].Enabled = true;
                        cboDeathSurg.Properties.Buttons[1].Enabled = true;
                        cboMachine.Properties.Buttons[1].Enabled = true;
                        cboDepartment.Properties.Buttons[1].Enabled = true;
                        cboMethod.Properties.Buttons[1].Enabled = true;
                        cbbPtttGroup.Properties.Buttons[1].Enabled = true;
                        cboCondition.Properties.Buttons[1].Enabled = true;
                        cboCatastrophe.Properties.Buttons[1].Enabled = true;
                        spinExcuteTimeAdd.ReadOnly = false;
                        cboEkipTemp.ReadOnly = false;
                        cboDepartment.ReadOnly = false;
                        ucEkip.SetEnableControl(false);
                        IsReadOnlyGridViewEkipUser = false;
                        chkSaveGroup.ReadOnly = false;

                        btnAssignBlood.Enabled = true;
                        btnTuTruc.Enabled = true;
                        btnAssignPre.Enabled = true;
                        btnPresYhct.Enabled = true;
                        dropDownButtonGPBL.Enabled = true;
                        ddbPhatSinh.Enabled = true;
                        btnSwapService.Enabled = true;
                        btnDepartmentTran.Enabled = true;
                        btnOther.Enabled = true;
                        btnPrint.Enabled = true;
                        btnKTDT.Enabled = true;
                    }

                    //

                    var rs = TypeRequiredEmotionlessMethodOption(this.sereServ);
                    lciVoCam.AppearanceItemCaption.ForeColor = rs.RequiredEmotionlessOption != null && this.sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT ? System.Drawing.Color.Maroon : System.Drawing.Color.Black;
                    InValid();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        dynamic TypeRequiredEmotionlessMethodOption(V_HIS_SERE_SERV_5 ss) // viec 76788
        {
            dynamic rs = new System.Dynamic.ExpandoObject();
            rs.RequiredEmotionlessOption = null;
            rs.IsServiceTypePT = false;
            rs.IsServiceTypeTT = false;
            try
            {
                if (HisConfigCFG.RequiredEmotionlessMethodOption == "1")
                {
                    rs.RequiredEmotionlessOption = 1;
                    rs.IsServiceTypePT = ss.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT;
                }
                else if (HisConfigCFG.RequiredEmotionlessMethodOption == "2")
                {
                    rs.RequiredEmotionlessOption = 2;
                    if (ss.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                    {
                        rs.IsServiceTypePT = true;
                    }
                    else if (ss.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                    {
                        rs.IsServiceTypeTT = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return rs;
        }

        private void InValid()
        {
            try
            {
                var ColorStart = lciStartTime.AppearanceItemCaption.ForeColor;
                if (dtStart.ReadOnly)
                {
                    this.lciStartTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.dxValidationProvider1.SetValidationRule(dtStart, null);
                }
                else
                {
                    this.lciStartTime.AppearanceItemCaption.ForeColor = ColorStart;
                    ValidationStartTime();
                }
                var Color = layoutControlItem17.AppearanceItemCaption.ForeColor;
                if (txtLoaiPT.ReadOnly)
                {
                    layoutControlItem17.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.dxValidationProvider1.SetValidationRule(txtLoaiPT, null);

                }
                else
                {
                    layoutControlItem17.AppearanceItemCaption.ForeColor = Color;
                    if (PriorityIsRequired == 1 || (PriorityIsRequired == 2 && this.serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT))
                        ValidationGridLookUpWithTextEdit(cboLoaiPT, txtLoaiPT);
                }
                var KLColor = this.lciKetLuan.AppearanceItemCaption.ForeColor;
                var ColorPL = lciPhanLoai.AppearanceItemCaption.ForeColor;
                if (txtPtttGroupCode.ReadOnly)
                {
                    lciPhanLoai.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.dxValidationProvider1.SetValidationRule(txtPtttGroupCode, null);
                }
                else
                {
                    lciPhanLoai.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.dxValidationProvider1.SetValidationRule(txtPtttGroupCode, null);

                    if (HisConfigCFG.REQUIRED_GROUPPTTT_OPTION != "1")
                    {
                        this.sereServ = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5)grdViewService.GetFocusedRow();
                        Inventec.Common.Logging.LogSystem.Debug("sereServValidate " + sereServ);
                        if (this.sereServ != null)
                        {
                            var surgMisuService = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                            if (surgMisuService != null)
                            {
                                if (surgMisuService != null)
                                {
                                    if (surgMisuService.PTTT_GROUP_ID.HasValue)
                                    {
                                        lciPhanLoai.AppearanceItemCaption.ForeColor = ColorPL;
                                        ValidationLookUpWithTextEdit(cbbPtttGroup, txtPtttGroupCode);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        lciPhanLoai.AppearanceItemCaption.ForeColor = ColorPL;
                        ValidationLookUpWithTextEdit(cbbPtttGroup, txtPtttGroupCode);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void refreshControl()
        {
            try
            {
                //txtIcdText.Text = "";
                txtMethodCode.Text = "";
                cboMethod.EditValue = null;
                txtPtttGroupCode.Text = "";
                cbbPtttGroup.EditValue = null;
                txtEmotionlessMethod.Text = "";
                cbbEmotionlessMethod.EditValue = null;
                txtCatastrophe.Text = "";
                cboCatastrophe.EditValue = null;
                txtDeathSurg.Text = "";
                cboDeathSurg.EditValue = null;
                txtBlood.Text = "";
                cbbBlood.EditValue = null;
                txtBloodRh.Text = "";
                cbbBloodRh.EditValue = null;
                txtCondition.Text = "";
                cboCondition.EditValue = null;
                txtMANNER.Text = "";
                dtFinish.EditValue = null;
                dtStart.EditValue = null;
                //InitIcd();

                txtMoKTCao.Text = "";
                cboMoKTCao.EditValue = null;
                txtKQVoCam.Text = "";
                cboKQVoCam.EditValue = null;
                txtPhuongPhap2.Text = "";
                cboPhuongPhap2.EditValue = null;
                cboPhuongPhapThucTe.EditValue = null;
                txtPhuongPhapTT.Text = "";
                txtLoaiPT.Text = "";
                cboLoaiPT.EditValue = null;
                layoutControlItem25.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadPtttGroupCode(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbbPtttGroup.Focus();
                    cbbPtttGroup.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().Where(o => o.PTTT_GROUP_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbbPtttGroup.EditValue = data[0].ID;
                            cbbPtttGroup.Properties.Buttons[1].Visible = cbbPtttGroup.Properties.Buttons[1].Enabled ? true : false;
                            txtMethodCode.Focus();
                            txtMethodCode.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_GROUP_CODE == searchCode);
                            if (search != null)
                            {
                                cbbPtttGroup.EditValue = search.ID;
                                cbbPtttGroup.Properties.Buttons[1].Visible = cbbPtttGroup.Properties.Buttons[1].Enabled ? true : false;
                                txtMethodCode.Focus();
                                txtMethodCode.SelectAll();
                            }
                            else
                            {
                                cbbPtttGroup.EditValue = null;
                                cbbPtttGroup.Focus();
                                cbbPtttGroup.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cbbPtttGroup.EditValue = null;
                        cbbPtttGroup.Focus();
                        cbbPtttGroup.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadLoaiPT(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboLoaiPT.Focus();
                    cboLoaiPT.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>().Where(o => o.PTTT_PRIORITY_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboLoaiPT.EditValue = data[0].ID;
                            cboLoaiPT.Properties.Buttons[1].Visible = true;
                            txtBanMoCode.Focus();
                            txtBanMoCode.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_PRIORITY_CODE == searchCode);
                            if (search != null)
                            {
                                cboLoaiPT.EditValue = search.ID;
                                cboLoaiPT.Properties.Buttons[1].Visible = true;
                                txtBanMoCode.Focus();
                                txtBanMoCode.SelectAll();
                            }
                            else
                            {
                                cboLoaiPT.EditValue = null;
                                cboLoaiPT.Focus();
                                cboLoaiPT.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboLoaiPT.EditValue = null;
                        cboLoaiPT.Focus();
                        cboLoaiPT.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadBanMo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboBanMo.Focus();
                    cboBanMo.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_TABLE>().Where(o =>
                        o.IS_ACTIVE == 1
                        && o.PTTT_TABLE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            txtBanMoCode.Text = data[0].PTTT_TABLE_CODE;
                            cboBanMo.EditValue = data[0].ID;
                            cboBanMo.Properties.Buttons[1].Visible = true;
                            txtMethodCode.Focus();
                            txtMethodCode.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_TABLE_CODE == searchCode);
                            if (search != null)
                            {
                                cboBanMo.EditValue = search.ID;
                                cboBanMo.Properties.Buttons[1].Visible = true;
                                txtMethodCode.Focus();
                                txtMethodCode.SelectAll();
                            }
                            else
                            {
                                cboBanMo.EditValue = null;
                                cboBanMo.Focus();
                                cboBanMo.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboBanMo.EditValue = null;
                        cboBanMo.Focus();
                        cboBanMo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadMethod(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMethod.Focus();
                    cboMethod.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(o => o.PTTT_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboMethod.EditValue = data[0].ID;
                            cboMethod.Properties.Buttons[1].Visible = cboMethod.Properties.Buttons[1].Enabled ? true : false;
                            txtPhuongPhap2.Focus();
                            txtPhuongPhap2.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cboMethod.EditValue = search.ID;
                                cboMethod.Properties.Buttons[1].Visible = cboMethod.Properties.Buttons[1].Enabled ? true : false;
                                txtPhuongPhap2.Focus();
                                txtPhuongPhap2.SelectAll();
                            }
                            else
                            {
                                cboMethod.EditValue = null;
                                cboMethod.Focus();
                                cboMethod.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboMethod.EditValue = null;
                        cboMethod.Focus();
                        cboMethod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadPhuongPhap2(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPhuongPhap2.Focus();
                    cboPhuongPhap2.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(o =>
                        o.IS_ACTIVE == 1
                        && (o.IS_SECOND == 1 || (o.IS_FIRST != 1 && o.IS_SECOND != 1))
                        && o.EMOTIONLESS_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPhuongPhap2.EditValue = data[0].ID;
                            cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                            txtPhuongPhapTT.Focus();
                            txtPhuongPhapTT.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EMOTIONLESS_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cboPhuongPhap2.EditValue = search.ID;
                                cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                                txtPhuongPhapTT.Focus();
                                txtPhuongPhapTT.SelectAll();
                            }
                            else
                            {
                                cboPhuongPhap2.EditValue = null;
                                cboPhuongPhap2.Focus();
                                cboPhuongPhap2.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboPhuongPhap2.EditValue = null;
                        cboPhuongPhap2.Focus();
                        cboPhuongPhap2.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        internal void LoadPhuongPhapThucTe(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPhuongPhapThucTe.Focus();
                    cboPhuongPhapThucTe.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(o => o.PTTT_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPhuongPhapThucTe.EditValue = data[0].ID;
                            cboPhuongPhapThucTe.Properties.Buttons[1].Visible = true;
                            txtEmotionlessMethod.Focus();
                            txtEmotionlessMethod.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cboPhuongPhapThucTe.EditValue = search.ID;
                                cboPhuongPhapThucTe.Properties.Buttons[1].Visible = true;
                                txtEmotionlessMethod.Focus();
                                txtEmotionlessMethod.SelectAll();
                            }
                            else
                            {
                                cboPhuongPhapThucTe.EditValue = null;
                                cboPhuongPhapThucTe.Focus();
                                cboPhuongPhapThucTe.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboPhuongPhapThucTe.EditValue = null;
                        cboPhuongPhapThucTe.Focus();
                        cboPhuongPhapThucTe.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadBlood(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbbBlood.Focus();
                    cbbBlood.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_ABO>().Where(o => o.BLOOD_ABO_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbbBlood.EditValue = data[0].ID;
                            cbbBlood.Properties.Buttons[1].Visible = true;
                            txtBloodRh.Focus();
                            txtBloodRh.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.BLOOD_ABO_CODE == searchCode);
                            if (search != null)
                            {
                                cbbBlood.EditValue = search.ID;
                                cbbBlood.Properties.Buttons[1].Visible = true;
                                txtBloodRh.Focus();
                                txtBloodRh.SelectAll();
                            }
                            else
                            {
                                cbbBlood.EditValue = null;
                                cbbBlood.Focus();
                                cbbBlood.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cbbBlood.EditValue = null;
                        cbbBlood.Focus();
                        cbbBlood.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadEmotionlessMethod(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbbEmotionlessMethod.Focus();
                    cbbEmotionlessMethod.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(o =>
                        o.IS_ACTIVE == 1
                        && (o.IS_FIRST == 1 || (o.IS_FIRST != 1 && o.IS_SECOND != 1))
                        && o.EMOTIONLESS_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbbEmotionlessMethod.EditValue = data[0].ID;
                            cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                            txtCondition.Focus();
                            txtCondition.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EMOTIONLESS_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cbbEmotionlessMethod.EditValue = search.ID;
                                cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                                txtCondition.Focus();
                                txtCondition.SelectAll();
                            }
                            else
                            {
                                cbbEmotionlessMethod.EditValue = null;
                                cbbEmotionlessMethod.Focus();
                                cbbEmotionlessMethod.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cbbEmotionlessMethod.EditValue = null;
                        cbbEmotionlessMethod.Focus();
                        cbbEmotionlessMethod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadKQVoCam(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboKQVoCam.Focus();
                    cboKQVoCam.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_RESULT>().Where(o =>
                        o.IS_ACTIVE == 1
                        && o.EMOTIONLESS_RESULT_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboKQVoCam.EditValue = data[0].ID;
                            cboKQVoCam.Properties.Buttons[1].Visible = true;
                            txtCondition.Focus();
                            txtCondition.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EMOTIONLESS_RESULT_CODE == searchCode);
                            if (search != null)
                            {
                                cboKQVoCam.EditValue = search.ID;
                                cboKQVoCam.Properties.Buttons[1].Visible = true;
                                txtCondition.Focus();
                                txtCondition.SelectAll();
                            }
                            else
                            {
                                cboKQVoCam.EditValue = null;
                                cboKQVoCam.Focus();
                                cboKQVoCam.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboKQVoCam.EditValue = null;
                        cboKQVoCam.Focus();
                        cboKQVoCam.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadCondition(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboCondition.Focus();
                    cboCondition.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CONDITION>().Where(o => o.PTTT_CONDITION_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboCondition.EditValue = data[0].ID;
                            cboCondition.Properties.Buttons[1].Visible = cboCondition.Properties.Buttons[1].Enabled ? true : false;
                            txtBlood.Focus();
                            txtBlood.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_CONDITION_CODE == searchCode);
                            if (search != null)
                            {
                                cboCondition.EditValue = search.ID;
                                cboCondition.Properties.Buttons[1].Visible = cboCondition.Properties.Buttons[1].Enabled ? true : false;
                                txtBlood.Focus();
                                txtBlood.SelectAll();
                            }
                            else
                            {
                                cboCondition.EditValue = null;
                                cboCondition.Focus();
                                cboCondition.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboCondition.EditValue = null;
                        cboCondition.Focus();
                        cboCondition.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadMoKTCao(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMoKTCao.Focus();
                    cboMoKTCao.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_HIGH_TECH>().Where(o =>
                        o.IS_ACTIVE == 1
                        && o.PTTT_HIGH_TECH_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboMoKTCao.EditValue = data[0].ID;
                            cboMoKTCao.Properties.Buttons[1].Visible = true;
                            dtStart.Focus();
                            dtStart.SelectAll();
                            dtStart.ShowPopup();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_HIGH_TECH_CODE == searchCode);
                            if (search != null)
                            {
                                cboMoKTCao.EditValue = search.ID;
                                cboMoKTCao.Properties.Buttons[1].Visible = true;
                                dtStart.Focus();
                                dtStart.SelectAll();
                                dtStart.ShowPopup();
                            }
                            else
                            {
                                cboMoKTCao.EditValue = null;
                                cboMoKTCao.Focus();
                                cboMoKTCao.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboMoKTCao.EditValue = null;
                        cboMoKTCao.Focus();
                        cboMoKTCao.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadBloodRh(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbbBloodRh.Focus();
                    cbbBloodRh.ShowPopup();
                }
                else
                {
                    var data = dataBloodRh.Where(o => o.BLOOD_RH_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbbBloodRh.EditValue = data[0].ID;
                            cbbBloodRh.Properties.Buttons[1].Visible = true;
                            txtCatastrophe.Focus();
                            txtCatastrophe.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.BLOOD_RH_CODE == searchCode);
                            if (search != null)
                            {
                                cbbBloodRh.EditValue = search.ID;
                                cbbBloodRh.Properties.Buttons[1].Visible = true;
                                txtCatastrophe.Focus();
                                txtCatastrophe.SelectAll();
                            }
                            else
                            {
                                cbbBloodRh.EditValue = null;
                                cbbBloodRh.Focus();
                                cbbBloodRh.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cbbBloodRh.EditValue = null;
                        cbbBloodRh.Focus();
                        cbbBloodRh.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadCatastrophe(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboCatastrophe.Focus();
                    cboCatastrophe.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CATASTROPHE>().Where(o => o.PTTT_CATASTROPHE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboCatastrophe.EditValue = data[0].ID;
                            cboCatastrophe.Properties.Buttons[1].Visible = cboCatastrophe.Properties.Buttons[1].Enabled ? true : false;
                            txtDeathSurg.Focus();
                            txtDeathSurg.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_CATASTROPHE_CODE == searchCode);
                            if (search != null)
                            {
                                cboCatastrophe.EditValue = search.ID;
                                cboCatastrophe.Properties.Buttons[1].Visible = cboCatastrophe.Properties.Buttons[1].Enabled ? true : false;
                                txtDeathSurg.Focus();
                                txtDeathSurg.SelectAll();
                            }
                            else
                            {
                                cboCatastrophe.EditValue = null;
                                cboCatastrophe.Focus();
                                cboCatastrophe.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboCatastrophe.EditValue = null;
                        cboCatastrophe.Focus();
                        cboCatastrophe.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadMachine(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMachine.Focus();
                    cboMachine.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>().Where(o =>
                        o.IS_ACTIVE == 1
                        && o.MACHINE_CODE.Contains(searchCode)
                        && ("," + o.ROOM_IDS + ",").Contains("," + this.Module.RoomId + ",")).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboMachine.EditValue = data[0].ID;
                            cboMachine.Properties.Buttons[1].Visible = true;
                            txtMoKTCao.Focus();
                            txtMoKTCao.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.MACHINE_CODE == searchCode);
                            if (search != null)
                            {
                                cboMachine.EditValue = search.ID;
                                cboMachine.Properties.Buttons[1].Visible = true;
                                txtMoKTCao.Focus();
                                txtMoKTCao.SelectAll();
                            }
                            else
                            {
                                cboMachine.EditValue = null;
                                cboMachine.Focus();
                                cboMachine.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboMachine.EditValue = null;
                        cboMachine.Focus();
                        cboMachine.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        //internal void LoadTypePackage(string searchCode, bool isExpand)
        //{
        //    try
        //    {
        //        if (String.IsNullOrEmpty(searchCode))
        //        {
        //            cboTypePackage.Focus();
        //            cboTypePackage.ShowPopup();
        //        }
        //        else
        //        {
        //            var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PRICE_POLICY>().Where(o => o.PRICE_POLICY_CODE.Contains(searchCode)).ToList();
        //            if (data != null)
        //            {
        //                if (data.Count == 1)
        //                {
        //                    cboTypePackage.EditValue = data[0].ID;
        //                    txtDeathSurg.Focus();
        //                    txtDeathSurg.SelectAll();
        //                }
        //                else
        //                {
        //                    var search = data.FirstOrDefault(m => m.PRICE_POLICY_CODE == searchCode);
        //                    if (search != null)
        //                    {
        //                        cboTypePackage.EditValue = search.ID;
        //                        txtDeathSurg.Focus();
        //                        txtDeathSurg.SelectAll();
        //                    }
        //                    else
        //                    {
        //                        cboTypePackage.EditValue = null;
        //                        cboTypePackage.Focus();
        //                        cboTypePackage.ShowPopup();
        //                    }

        //                }
        //            }
        //            else
        //            {
        //                cboTypePackage.EditValue = null;
        //                cboTypePackage.Focus();
        //                cboTypePackage.ShowPopup();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void LoadDeathSurg(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDeathSurg.Focus();
                    cboDeathSurg.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().Where(o => o.DEATH_CAUSE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboDeathSurg.EditValue = data[0].ID;
                            txtMachineCode.Focus();
                            txtMachineCode.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.DEATH_CAUSE_CODE == searchCode);
                            if (search != null)
                            {
                                cboDeathSurg.EditValue = search.ID;
                                txtMachineCode.Focus();
                                txtMachineCode.SelectAll();
                            }
                            else
                            {
                                cboDeathSurg.EditValue = null;
                                cboDeathSurg.Focus();
                                cboDeathSurg.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboDeathSurg.EditValue = null;
                        cboDeathSurg.Focus();
                        cboDeathSurg.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        //private void FillDataIntoUserCombo(HisEkipUserADO data, LookUpEdit editor)
        //{
        //    try
        //    {
        //        List<ACS.EFMODEL.DataModels.ACS_USER> listUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>();
        //        var sereServPTTTADOs = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
        //        if (sereServPTTTADOs != null && sereServPTTTADOs.Count > 0)
        //        {
        //            var sereServPTTTLoginNames = sereServPTTTADOs.Select(o => o.LOGINNAME).ToList();
        //            listUser = listUser.Where(o => (!sereServPTTTLoginNames.Contains(o.LOGINNAME)
        //                || o.LOGINNAME == data.LOGINNAME)).ToList();
        //        }
        //        editor.Properties.DataSource = listUser;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void LoadServiceReq(long serviceReqId)
        {
            try
            {
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ID = serviceReqId;
                serviceReq = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, new CommonParam()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async void LoadExecuteRoleUser()
        {
            try
            {
                LogSystem.Error("---S");
                if (BackendDataWorker.IsExistsKey<HIS_EXECUTE_ROLE_USER>())
                {
                    executeRoleUsers = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_ROLE_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    executeRoleUsers = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_EXECUTE_ROLE>>("api/HisExecuteRoleUser/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (executeRoleUsers != null) BackendDataWorker.UpdateToRam(typeof(HIS_EXECUTE_ROLE), executeRoleUsers, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                LogSystem.Error("---F");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void EnableButtonByServiceReqSTT(long serviceReqSttId)
        {
            try
            {
                if (serviceReqSttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                {
                    btnSurgAssignAndCopy.Enabled = true;
                    lciChuanDoanPhu.Enabled = false;
                    txtIcdText.Enabled = false;
                    lciPhanLoai.Enabled = false;
                    cbbPtttGroup.Enabled = false;
                    lciPhuongPhap.Enabled = false;
                    cboMethod.Enabled = false;
                    lciNhomMau.Enabled = false;
                    cbbBlood.Enabled = false;
                    lciVoCam.Enabled = false;
                    cbbEmotionlessMethod.Enabled = false;
                    lciTinhTrang.Enabled = false;
                    cboCondition.Enabled = false;
                    lciRh.Enabled = false;
                    cbbBloodRh.Enabled = false;
                    lciTaiBien.Enabled = false;
                    cboCatastrophe.Enabled = false;
                    lciTuVongTrong.Enabled = false;
                    cboDeathSurg.Enabled = false;
                    lciStartTime.Enabled = false;
                    lciThoiGianKetThuc.Enabled = false;
                    lciCachThuc.Enabled = false;
                    txtDescription.Enabled = false;
                    lciKetLuan.Enabled = false;
                    txtResultNote.Enabled = false;
                    // lciInformationSurg.Enabled = false;
                    layoutControlItem56.Enabled = false;
                    //ucEkip.EnableControl(false);
                    btnKTDT.Enabled = false;
                    btnAssignBlood.Enabled = false;
                    btnTuTruc.Enabled = false;
                    //btnSwapService.Enabled = false;
                    ddbPhatSinh.Enabled = false;
                    btnAssignPre.Enabled = false;
                    btnPresYhct.Enabled = false;
                    btnSave.Enabled = false;
                    btnFinish.Enabled = false;
                    EnableICD(false, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                    EnableICD(false, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                    EnableICD(false, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                    EnableICD(false, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);

                    txtIcdCmSubCode.Enabled = false;
                    txtIcdCmSubName.Enabled = false;
                    txtLoaiPT.Enabled = false;
                    cboLoaiPT.Enabled = false;
                    txtBanMoCode.Enabled = false;
                    cboBanMo.Enabled = false;
                    txtPhuongPhap2.Enabled = false;
                    cboPhuongPhap2.Enabled = false;
                    txtPhuongPhapTT.Enabled = false;
                    cboPhuongPhapThucTe.Enabled = false;
                    txtKQVoCam.Enabled = false;
                    cboKQVoCam.Enabled = false;
                    txtMachineCode.Enabled = false;
                    cboMachine.Enabled = false;
                    txtMoKTCao.Enabled = false;
                    cboMoKTCao.Enabled = false;
                }
                else
                {
                    btnSurgAssignAndCopy.Enabled = false;
                    EnableICD(true, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                    EnableICD(true, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                    EnableICD(true, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                    EnableICD(true, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);
                    txtIcdText.Enabled = true;
                    lciPhanLoai.Enabled = true;
                    cbbPtttGroup.Enabled = true;
                    lciPhuongPhap.Enabled = true;
                    cboMethod.Enabled = true;
                    lciNhomMau.Enabled = true;
                    cbbBlood.Enabled = true;
                    lciVoCam.Enabled = true;
                    cbbEmotionlessMethod.Enabled = true;
                    lciTinhTrang.Enabled = true;
                    cboCondition.Enabled = true;
                    lciRh.Enabled = true;
                    cbbBloodRh.Enabled = true;
                    lciTaiBien.Enabled = true;
                    cboCatastrophe.Enabled = true;
                    lciTuVongTrong.Enabled = true;
                    cboDeathSurg.Enabled = true;
                    lciStartTime.Enabled = true;
                    lciThoiGianKetThuc.Enabled = true;
                    lciCachThuc.Enabled = true;

                    txtDescription.Enabled = true;
                    lciKetLuan.Enabled = true;
                    txtResultNote.Enabled = true;

                    ucEkip.EnableControl(true);
                    btnKTDT.Enabled = true;
                    btnAssignBlood.Enabled = true;
                    btnTuTruc.Enabled = true;
                    //btnSwapService.Enabled = true;
                    ddbPhatSinh.Enabled = true;
                    btnAssignPre.Enabled = true;
                    btnPresYhct.Enabled = true;
                    btnSave.Enabled = true;
                    btnFinish.Enabled = !(HisConfigKeys.allowFinishWhenAccountIsDoctor == "1" && BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).FirstOrDefault().IS_DOCTOR != 1);

                    txtIcdCmSubCode.Enabled = true;
                    txtIcdCmSubName.Enabled = true;
                    txtLoaiPT.Enabled = true;
                    cboLoaiPT.Enabled = true;
                    txtBanMoCode.Enabled = true;
                    cboBanMo.Enabled = true;
                    txtPhuongPhap2.Enabled = true;
                    cboPhuongPhap2.Enabled = true;
                    txtPhuongPhapTT.Enabled = true;
                    cboPhuongPhapThucTe.Enabled = true;
                    txtKQVoCam.Enabled = true;
                    cboKQVoCam.Enabled = true;
                    txtMachineCode.Enabled = true;
                    cboMachine.Enabled = true;
                    txtMoKTCao.Enabled = true;
                    cboMoKTCao.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async void SetButtonDeleteGridLookup()
        {
            try
            {
                ButtonDeleteGridLookup(cboMethod);
                ButtonDeleteLookup(cbbPtttGroup);
                ButtonDeleteLookup(cbbEmotionlessMethod);
                ButtonDeleteLookup(cbbBlood);
                ButtonDeleteLookup(cbbBloodRh);
                ButtonDeleteLookup(cboCondition);
                ButtonDeleteLookup(cboCatastrophe);
                ButtonDeleteLookup(cboDeathSurg);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ButtonDeleteGridLookup(GridLookUpEdit control)
        {
            try
            {
                if (control.EditValue != null)
                {
                    control.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    control.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ButtonDeleteLookup(LookUpEdit control)
        {
            try
            {
                if (control.EditValue != null)
                {
                    control.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    control.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public async Task ComboEkipTemp(GridLookUpEdit cbo)
        {
            try
            {
                var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.Module.RoomId).DepartmentId;

                string logginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                CommonParam param = new CommonParam();
                HisEkipTempFilter filter = new HisEkipTempFilter();
                ekipTemps = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP>>("/api/HisEkipTemp/Get", ApiConsumers.MosConsumer, filter, param);
                if (ekipTemps != null && ekipTemps.Count > 0)
                {
                    ekipTemps = ekipTemps.Where(o => (o.IS_PUBLIC == 1 || o.CREATOR == logginName || (o.IS_PUBLIC_IN_DEPARTMENT == 1 && o.DEPARTMENT_ID == DepartmentID)) && o.IS_ACTIVE == 1).OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EKIP_TEMP_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EKIP_TEMP_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, ekipTemps, controlEditorADO);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public async Task LoadComboDepartment(GridLookUpEdit cbo)
        {
            try
            {
                var departmentClinic = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_CLINICAL == 1 && o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, departmentClinic, controlEditorADO);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }



        public enum ButtonOther
        {
            TO_DIEU_TRI,
            CHUAN_BI_TRUOC_MO
        }

        private void InitButtonOther()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();

                DXMenuItem menuItem1 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.ButtonOther.ToDieuTri", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickButtonOther));
                menuItem1.Tag = ButtonOther.TO_DIEU_TRI;
                menu.Items.Add(menuItem1);
                //DXMenuItem menuItem2 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecute.ButtonOther.ChuanBiTruocMo", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickButtonOther));
                //menuItem2.Tag = ButtonOther.CHUAN_BI_TRUOC_MO;
                //menu.Items.Add(menuItem2);

                if (this.SereServExt != null && this.SereServExt.ID > 0)//xuandv them dieu kien , do update json_form_id vao SS_ext
                {
                    HIS_SERVICE_REQ _serviceReq = new HIS_SERVICE_REQ();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(_serviceReq, this.serviceReq);

                    V_HIS_SERE_SERV _sereServ = new V_HIS_SERE_SERV();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(_sereServ, this.sereServ);

                    HIS.Desktop.Plugins.Library.FormOtherSereServPttt.FormOtherProcessor form = new Library.FormOtherSereServPttt.FormOtherProcessor(_serviceReq, _sereServ, this.sereServPTTT, this.SereServExt, (HIS.Desktop.Common.RefeshReference)BtnRefreshPhimTat);
                    var lstBar = form.GetDXMenuItem();
                    if (lstBar != null && lstBar.Count > 0)
                    {
                        foreach (var item in lstBar)
                        {
                            menu.Items.Add(item);
                        }
                    }
                }

                btnOther.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async void InitButtonPhatSinh()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();

                DXMenuItem menuItem1 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnAssignOutKip.Text", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(btnAssignOutKip_Click));
                menu.Items.Add(menuItem1);

                DXMenuItem menuItem2 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnAss.Text", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(btnAssInKip_Click));
                menu.Items.Add(menuItem2);

                DXMenuItem menuItem3 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.dropDownButtonGPBL.DVKhongPhatSinh", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(btnDVKhongPhatSinhDVKT_Click));
                menu.Items.Add(menuItem3);

                ddbPhatSinh.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async void InitButtonGPBL()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();

                DXMenuItem menuItem1 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.dropDownButtonGPBL.DVPhatSinh", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(btnDVPhatSinh_Click));
                menu.Items.Add(menuItem1);

                DXMenuItem menuItem2 = new DXMenuItem(Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.dropDownButtonGPBL.DVKhongPhatSinh", ResourceLangManager.LanguageUCSurgServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(btnDVKhongPhatSinhGPBL_Click));
                menu.Items.Add(menuItem2);

                dropDownButtonGPBL.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnDVPhatSinh_Click(object sender, EventArgs e)
        {
            try
            {
                IsActionOtherButton = true;
                if (!btnSaveClick(true))
                    return;

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                if (moduleData == null) LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();

                    listArgs.Add(this.serviceReq);
                    listArgs.Add(this.sereServ);
                    listArgs.Add(this.serviceReq.TREATMENT_ID);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnDVKhongPhatSinhDVKT_Click(object sender, EventArgs e)
        {

            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();

                    AssignServiceADO AssignServiceADO = new AssignServiceADO(serviceReq.TREATMENT_ID, 0, 0);
                    AssignServiceADO.TreatmentId = serviceReq.TREATMENT_ID;
                    AssignServiceADO.PatientDob = serviceReq.TDL_PATIENT_DOB;
                    AssignServiceADO.PatientName = serviceReq.TDL_PATIENT_NAME;
                    AssignServiceADO.GenderName = serviceReq.TDL_PATIENT_GENDER_NAME;
                    AssignServiceADO.IsAutoEnableEmergency = true;
                    listArgs.Add(AssignServiceADO);

                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDVKhongPhatSinhGPBL_Click(object sender, EventArgs e)
        {
            try
            {
                IsActionOtherButton = true;
                if (!btnSaveClick(true))
                    return;

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                if (moduleData == null) LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.serviceReq.TREATMENT_ID);

                    listArgs.Add(this.serviceReq.ID);
                    listArgs.Add(this.serviceReq);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void onClickButtonOther(object sender, EventArgs e)
        {
            try
            {
                DXMenuItem item = sender as DXMenuItem;
                ButtonOther type = (ButtonOther)(item.Tag);
                ButtonOtherProcess(type);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ButtonOtherProcess(ButtonOther type)
        {
            try
            {
                switch (type)
                {
                    case ButtonOther.TO_DIEU_TRI:
                        this.btnTrackingCreate_Click(null, null);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void BtnRefreshPhimTat()
        {
            try
            {
                if (this.sereServ != null && this.sereServ.ID > 0)
                {
                    CommonParam param = new CommonParam();
                    HisSereServExtFilter ssExtFilter = new HisSereServExtFilter();
                    ssExtFilter.SERE_SERV_ID = this.sereServ.ID;
                    var SereServExts = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, ssExtFilter, param);

                    if (SereServExts != null && SereServExts.Count > 0)
                    {
                        this.SereServExt = SereServExts.FirstOrDefault();
                        if (this.SereServExt != null)
                        {
                            InitButtonOther();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private bool ProcessLoadSereServFile(List<long> sereServId)
        {
            bool result = false;
            try
            {
                var currentSereServFiles = GetSereServFilesBySereServId(sereServId);
                if (currentSereServFiles != null && currentSereServFiles.Count > 0)
                {
                    result = true;
                    this.imageADOs = new List<ImageADO>();
                    foreach (MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE item in currentSereServFiles)
                    {
                        LogSystem.Debug(LogUtil.TraceData("item.URL", item.URL) + "____" + LogUtil.TraceData(LogUtil.GetMemberName(() => sereServId), sereServId));
                        System.IO.MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(item.URL);
                        if (stream != null && stream.Length > 0)
                        {
                            ImageADO tileNew = new ImageADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV_FILE>(tileNew, item);
                            tileNew.FileName = item.SERE_SERV_FILE_NAME + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            tileNew.IsChecked = false;

                            tileNew.streamImage = new MemoryStream();
                            stream.Position = 0;
                            stream.CopyTo(tileNew.streamImage);
                            stream.Position = 0;
                            tileNew.IMAGE_DISPLAY = System.Drawing.Image.FromStream(stream);
                            this.imageADOs.Add(tileNew);
                        }
                    }
                    ProcessLoadGridImage(this.imageADOs);
                }
                else
                {
                    cardControl.DataSource = null;
                    this.imageADOs = null;
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogSystem.Error(ex);
            }
            return result;
        }

        private List<HIS_SERE_SERV_FILE> GetSereServFilesBySereServId(List<long> sereServId)
        {
            List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE> result = null;
            try
            {
                if (sereServId != null && sereServId.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisSereServFileFilter filter = new MOS.Filter.HisSereServFileFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.SERE_SERV_IDs = sereServId;
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "ID";
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE>>(RequestUriStore.HIS_SERE_SERV_FILE_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }
    }
}
