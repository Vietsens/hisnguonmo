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
                    HisPatientFilter filter = new HisPatientFilter();
                    filter.ID = data.PATIENT_ID;
                    var patient = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, null);
                    if(patient != null && patient.Count > 0)
                    {
                        txtBuyerEmail.Text = patient.FirstOrDefault().EMAIL;
                    }
                    //
                    //                    Nếu TDL_PATIENT_WORK_PLACE_ID trong V_HIS_TREATMENT_FEE có giá trị thì hiển thị bản ghi HIS_WORK_PLACE có ID tương ứng, và bỏ check checkbox “Khác”
                    //Nếu TDL_PATIENT_WORK_PLACE_ID không có thông tin và TDL_PATIENT_WORK_PLACE_NAME có thông tin thì hiển thị dữ liệu tại TDL_PATIENT_WORK_PLACE_NAME và tự động check vào checkbox “Khác”
                    //Nếu cả 2 trường đếu ko có thông tin thì mặc định bỏ check checkbox “Khác”

                    if (!IsPin)
                    {
                        txtBuyerAccountNumber.Text = data.TDL_PATIENT_ACCOUNT_NUMBER ?? "";
                        txtBuyerAddress.Text = data.TDL_PATIENT_ADDRESS ?? "";
                        if (chkAddressBhyt.Checked && resultPatientType != null && !string.IsNullOrEmpty(resultPatientType.ADDRESS))
                        {
                            txtBuyerAddress.Text = resultPatientType.ADDRESS;
                        }
                        txtBuyerName.Text = data.TDL_PATIENT_NAME ?? "";
                        txtBuyerTaxCode.Text = data.TDL_PATIENT_TAX_CODE ?? "";
                        if (data.TDL_PATIENT_WORK_PLACE_ID.HasValue)
                        {
                            cboBuyerOrganization.EditValue = dtWorkPlace.Where(o => o.ID == data.TDL_PATIENT_WORK_PLACE_ID).FirstOrDefault().ID;
                            txtBuyerOrganization.Text = "";
                            chkOther.Checked = false;
                        }
                        else if (!string.IsNullOrEmpty(data.TDL_PATIENT_WORK_PLACE))
                        {
                            cboBuyerOrganization.EditValue = null;
                            txtBuyerOrganization.Text = data.TDL_PATIENT_WORK_PLACE;
                            chkOther.Checked = true;
                        }
                        else
                        {
                            cboBuyerOrganization.EditValue = null;
                            txtBuyerOrganization.Text = "";
                            chkOther.Checked = false;
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
                            decimal? heinRatio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(resultPatientType.HEIN_TREATMENT_TYPE_CODE, resultPatientType.HEIN_CARD_NUMBER, resultPatientType.LEVEL_CODE, resultPatientType.RIGHT_ROUTE_CODE, (data.TOTAL_HEIN_PRICE ?? 0 + data.TOTAL_PATIENT_PRICE_BHYT ?? 0));
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
