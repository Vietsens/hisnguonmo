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
using DevExpress.Utils.Menu;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.TransactionBill.ADO;
using HIS.Desktop.Plugins.TransactionBill.Config;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
//using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Client;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBill
{
    public partial class frmTransactionBill : HIS.Desktop.Utility.FormBase
    {
        private async Task FillInfoPatient(V_HIS_TREATMENT_FEE data)
        {
            try
            {
                if (data != null)
                {
                    txtPatient.Text = data.TDL_PATIENT_CODE;
                    txtPatientName.Text = data.TDL_PATIENT_NAME;
                    txtDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    txtGender.Text = data.TDL_PATIENT_GENDER_NAME;
                    txtAddress.Text = data.TDL_PATIENT_ADDRESS;                    
                    //
                    //                    Nếu TDL_PATIENT_WORK_PLACE_ID trong V_HIS_TREATMENT_FEE có giá trị thì hiển thị bản ghi HIS_WORK_PLACE có ID tương ứng, và bỏ check checkbox “Khác”
                    //Nếu TDL_PATIENT_WORK_PLACE_ID không có thông tin và TDL_PATIENT_WORK_PLACE_NAME có thông tin thì hiển thị dữ liệu tại TDL_PATIENT_WORK_PLACE_NAME và tự động check vào checkbox “Khác”
                    //Nếu cả 2 trường đếu ko có thông tin thì mặc định bỏ check checkbox “Khác”

                    Inventec.Common.Logging.LogSystem.Debug("TransactionBill.FillInfoPatient >>> IsPin=" + IsPin + " | TDL_PATIENT_WORK_PLACE_ID=" + data.TDL_PATIENT_WORK_PLACE_ID + " | TDL_PATIENT_WORK_PLACE_NAME=" + data.TDL_PATIENT_WORK_PLACE_NAME + " | TDL_PATIENT_WORK_PLACE=" + data.TDL_PATIENT_WORK_PLACE);
                    if (!IsPin)
                    {
                        radioBuyerUser.Checked = true;
                        HisPatientFilter filter = new HisPatientFilter();
                        filter.ID = data.PATIENT_ID;
                        var patient = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, null);
                        var patientData = (patient != null && patient.Count > 0) ? patient.FirstOrDefault() : null;
                        if (patientData != null)
                        {
                            txtBuyerEmail.Text = patientData.EMAIL;
                        }
                        txtBuyerAccountNumber.Text = patientData?.ACCOUNT_NUMBER ?? data.TDL_PATIENT_ACCOUNT_NUMBER ?? "";
                        txtSDT.Text = data.TDL_PATIENT_PHONE ?? "";
                        txtBuyerName.Text = data.TDL_PATIENT_NAME ?? "";
                        if (patientData != null && !string.IsNullOrEmpty(patientData.BUD_REL_UNIT_CODE))
                        {
                            txtMaQH.Text = patientData.BUD_REL_UNIT_CODE;
                        }
                        else if (!string.IsNullOrEmpty(data.TDL_PATIENT_BUD_REL_UNIT_CODE))
                        {
                            txtMaQH.Text = data.TDL_PATIENT_BUD_REL_UNIT_CODE;
                        }
                        else
                        {
                            txtMaQH.Text = "";
                        }
                        string autoLoadData = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TransactionBill.AutoLoadOrgAndTaxCodeByPatient");
                        Inventec.Common.Logging.LogSystem.Debug("TransactionBill.FillInfoPatient >>> autoLoadData=[" + autoLoadData + "] | patientData null=" + (patientData == null) + " | patientData.WORK_PLACE_ID=" + patientData?.WORK_PLACE_ID + " | patientData.WORK_PLACE=" + patientData?.WORK_PLACE);

                        string patientTaxCode = patientData?.TAX_CODE ?? data.TDL_PATIENT_TAX_CODE;
                        txtBuyerTaxCode.Text = patientTaxCode ?? "";
                        txtBuyerTaxCode2.Text = (autoLoadData == "1") ? (patientTaxCode ?? "") : "";

                        long? workPlaceId = patientData?.WORK_PLACE_ID ?? data.TDL_PATIENT_WORK_PLACE_ID;
                        string workPlaceName = patientData?.WORK_PLACE ?? data.TDL_PATIENT_WORK_PLACE;
                        Inventec.Common.Logging.LogSystem.Debug("TransactionBill.FillInfoPatient >>> workPlaceId=" + workPlaceId + " | workPlaceName=[" + workPlaceName + "]");
                        if (workPlaceId.HasValue)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("TransactionBill.FillInfoPatient >>> BRANCH: co workPlaceId");
                            cboBuyerOrganization.EditValue = (autoLoadData == "1") ? workPlaceId : null;
                            txtBuyerOrganization.Text = "";
                            chkOther.Checked = false;

                            checkEdit1.Checked = false;
                            txtBuyerOrganzation2.Text = "";
                            cboBuyerOrganzation2.EditValue = (autoLoadData == "1") ? workPlaceId : null;
                            txtBuyerTaxCode2.Text = (autoLoadData == "1") ? (patientTaxCode ?? dtWorkPlace?.FirstOrDefault(o => o.ID == workPlaceId)?.TAX_CODE) : null;
                        }
                        else if (!string.IsNullOrEmpty(workPlaceName))
                        {
                            Inventec.Common.Logging.LogSystem.Debug("TransactionBill.FillInfoPatient >>> BRANCH: khong co ID, co text=[" + workPlaceName + "] -> chkOther=true, checkEdit1=" + (autoLoadData == "1"));
                            cboBuyerOrganization.EditValue = null;
                            txtBuyerOrganization.Text = workPlaceName;
                            chkOther.Checked = true;

                            cboBuyerOrganzation2.EditValue = null;
                            txtBuyerOrganzation2.Text = (autoLoadData == "1") ? workPlaceName : "";
                            checkEdit1.Checked = (autoLoadData == "1");
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("TransactionBill.FillInfoPatient >>> BRANCH: khong co ca ID lan text -> clear");
                            cboBuyerOrganization.EditValue = null;
                            txtBuyerOrganization.Text = "";
                            chkOther.Checked = false;

                            cboBuyerOrganzation2.EditValue = null;
                            txtBuyerOrganzation2.Text = "";
                            checkEdit1.Checked = false;
                        }

                        // Gán sau combo đơn vị vì EditValueChanged của combo ghi đè địa chỉ bằng địa chỉ đơn vị
                        txtBuyerAddress.Text = !string.IsNullOrWhiteSpace(patientData?.VIR_ADDRESS) ? patientData.VIR_ADDRESS : (data.TDL_PATIENT_ADDRESS ?? "");
                        if (chkAddressBhyt.Checked && resultPatientType != null && !string.IsNullOrEmpty(resultPatientType.ADDRESS))
                        {
                            txtBuyerAddress.Text = resultPatientType.ADDRESS;
                        }

                        if (!string.IsNullOrEmpty(data.TDL_PATIENT_CCCD_NUMBER))
                        {
                            txtBuyerIdentityNumber.Text = data.TDL_PATIENT_CCCD_NUMBER;
                            // Giá trị ValueMember của CCCD là 2
                            cboBuyerIdentity.EditValue = "CCCD";
                        }
                        else if (!string.IsNullOrEmpty(data.TDL_PATIENT_CMND_NUMBER))
                        {
                            txtBuyerIdentityNumber.Text = data.TDL_PATIENT_CMND_NUMBER;
                            // Giá trị ValueMember của CMND là 1
                            cboBuyerIdentity.EditValue = "CMND";
                        }
                        else if (!string.IsNullOrEmpty(data.TDL_PATIENT_PASSPORT_NUMBER))
                        {
                            txtBuyerIdentityNumber.Text = data.TDL_PATIENT_PASSPORT_NUMBER;
                            // Giá trị ValueMember của PASSPORT là 3
                            cboBuyerIdentity.EditValue = "PASSPORT";
                        }
                        else
                        {
                            txtBuyerIdentityNumber.Text = "";
                            cboBuyerIdentity.EditValue = null;
                        }
                    }
                    //
                    if (this.resultPatientType == null || this.resultPatientType.ID == 0)
                    {
                        this.resultPatientType = new BackendAdapter(new CommonParam())
.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, currentTreatment.ID, null);
                    }
                    if (resultPatientType != null)
                    {
                        txtHeinCard.Text = TrimHeinCardNumber(resultPatientType.HEIN_CARD_NUMBER);
                        txtHeinFrom.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(resultPatientType.HEIN_CARD_FROM_TIME ?? 0);
                        txtHeinTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(resultPatientType.HEIN_CARD_TO_TIME ?? 0);
                        txtMediOrg.Text = resultPatientType.HEIN_MEDI_ORG_NAME;
                        txtPatienType.Text = resultPatientType.PATIENT_TYPE_NAME ?? "";
                        string rightRoute = "";
                        if (resultPatientType.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                        {
                            rightRoute = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__RIGHT_ROUTE_TRUE", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                        }
                        else
                        {
                            rightRoute = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__RIGHT_ROUTE_FALSE", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                        }
                        lblRightRoute.Text = rightRoute ?? "";
                        string ratio = "";
                        if (resultPatientType.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            // TT BHYT moi: truyen CLINICAL_IN_TIME
                            decimal? heinRatio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(resultPatientType.HEIN_TREATMENT_TYPE_CODE, resultPatientType.HEIN_CARD_NUMBER, resultPatientType.LEVEL_CODE, resultPatientType.RIGHT_ROUTE_CODE, resultPatientType.FACILITY_CLASS, resultPatientType.FORMER_LEVEL_CODE, (long)(resultPatientType.CLASSIFY_POINT ?? 0), data.CLINICAL_IN_TIME ?? 0);
                            if (heinRatio.HasValue)
                            {
                                ratio = ((long)(heinRatio.Value * 100)).ToString() + "%";
                            }
                        }
                        lblHeinRatio.Text = ratio ?? "";
                    }
                    else
                    {
                        txtHeinCard.Text = "";
                        txtHeinFrom.Text = "";
                        txtHeinTo.Text = "";
                        txtMediOrg.Text = "";
                        lblRightRoute.Text = "";
                        lblHeinRatio.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static string TrimHeinCardNumber(string chucodau)
        {
            string result = "";
            try
            {
                result = System.Text.RegularExpressions.Regex.Replace(chucodau, @"[-,_ ]|[_]{2}|[_]{3}|[_]{4}|[_]{5}", "").ToUpper();
            }
            catch (Exception ex)
            {
                LogSystem.Error("Không thể tách thẻ BHYT");
            }
            return result;
        }
    }
}
