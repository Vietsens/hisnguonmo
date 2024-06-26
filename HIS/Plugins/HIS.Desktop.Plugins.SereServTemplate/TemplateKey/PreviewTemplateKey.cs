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
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SereServTemplate.TemplateKey
{
    public partial class PreviewTemplateKey : HIS.Desktop.Utility.FormBase
    {
        List<TemplateKeyAdo> listTemplateKey = new List<TemplateKeyAdo>();
        HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeAlter = new HisTreatmentWithPatientTypeInfoSDO();
        HIS_SERE_SERV_EXT sereServExt = new HIS_SERE_SERV_EXT();
        HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
        HIS_SERVICE_REQ currentServiceReq = new HIS_SERVICE_REQ();
        PatientADO patient = new PatientADO();

        List<string> removeKey = new List<string>() { "APP_CREATOR", "APP_MODIFIER", "IS_DELETE", "_ID", ".ID" };

        public PreviewTemplateKey(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PreviewTemplateKey_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();

                ProcessListKey();
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
                this.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_TEMPLATE_KEY__FORM");
                this.txtSearch.Properties.NullValuePrompt = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_TEMPLATE_KEY__TXT_SEARCH");
                this.Gc_Key.Caption = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_TEMPLATE_KEY__GC_KEY");
                this.Gc_Stt.Caption = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_TEMPLATE_KEY__GC_STT");
                this.Gc_Value.Caption = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_TEMPLATE_KEY__GC_VALUE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetResourceMessage(string key)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageFormSereServTemplate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void ProcessListKey()
        {
            try
            {
                Dictionary<string, object> dicParam = ProcessDicParam();
                if (dicParam != null && dicParam.Count > 0)
                {
                    listTemplateKey = new List<TemplateKeyAdo>();
                    foreach (var item in dicParam)
                    {
                        if (item.Key == "ID") continue;
                        if (CheckKey(item.Key)) continue;

                        listTemplateKey.Add(new TemplateKeyAdo(String.Format("<#{0};>", item.Key), (item.Value != null ? item.Value.ToString() : "")));
                    }
                }

                listTemplateKey.Add(new TemplateKeyAdo("<#CONCLUDE_PRINT;>", "Kết luận. Key chỉ có dữ liệu khi in"));
                listTemplateKey.Add(new TemplateKeyAdo("<#NOTE_PRINT;>", "Ghi chú. Key chỉ có dữ liệu khi in"));
                listTemplateKey.Add(new TemplateKeyAdo("<#DESCRIPTION_PRINT;>", "Mô tả. Key chỉ có dữ liệu khi in"));
                listTemplateKey.Add(new TemplateKeyAdo("<#CURRENT_USERNAME_PRINT;>", "Người in hiện tại. Key chỉ có dữ liệu khi in"));
                listTemplateKey.Add(new TemplateKeyAdo("<#BED_CODE;>", "Mã giường. Key chỉ có dữ liệu khi in"));
                listTemplateKey.Add(new TemplateKeyAdo("<#BED_NAME;>", "Tên giường. Key chỉ có dữ liệu khi in"));

                if (listTemplateKey.Count > 0)
                {
                    listTemplateKey = listTemplateKey.OrderBy(o => o.KEY).ToList();
                }

                GridControlKey.BeginUpdate();
                GridControlKey.DataSource = listTemplateKey;
                GridControlKey.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckKey(string p)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(p))
                {
                    foreach (var item in removeKey)
                    {
                        if (p.EndsWith(item))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private Dictionary<string, object> ProcessDicParam()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            try
            {
                // chế biến dữ liệu thành các key đơn thêm vào biểu mẫu tương tự như mps excel
                var dicParam = new Dictionary<string, object>();
                long time = Inventec.Common.DateTime.Get.Now() ?? 0;
                HIS.Desktop.Print.SetCommonKey.SetCommonSingleKey(dicParam);//commonkey
                dicParam.Add("SERE_SERV_QRCODE", "QRcode cho phép quét để lấy địa chỉ nhằm hiển thị hình ảnh PACS của dịch vụ tương ứng");
                dicParam.Add("INTRUCTION_TIME_FULL_STR", GetCurrentTimeSeparateBeginTime(DateTime.Now));
                dicParam.Add("INTRUCTION_DATE_FULL_STR",
                Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(time));
                dicParam.Add("INTRUCTION_TIME_STR",
                Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(time));
                dicParam.Add("START_TIME_STR",
                Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(time));
                dicParam.Add("START_TIME_FULL_STR", GetCurrentTimeSeparateBeginTime(DateTime.Now));
                dicParam.Add("ICD_MAIN_TEXT", currentServiceReq.ICD_NAME);
                dicParam.Add("NATIONAL_NAME", currentServiceReq.TDL_PATIENT_NATIONAL_NAME);
                dicParam.Add("WORK_PLACE", currentServiceReq.TDL_PATIENT_WORK_PLACE_NAME);
                dicParam.Add("ADDRESS", currentServiceReq.TDL_PATIENT_ADDRESS);
                dicParam.Add("CAREER_NAME", currentServiceReq.TDL_PATIENT_CAREER_NAME);
                dicParam.Add("PATIENT_CODE", currentServiceReq.TDL_PATIENT_CODE);
                dicParam.Add("DISTRICT_CODE", currentServiceReq.TDL_PATIENT_DISTRICT_CODE);
                dicParam.Add("GENDER_NAME", currentServiceReq.TDL_PATIENT_GENDER_NAME);
                dicParam.Add("MILITARY_RANK_NAME", currentServiceReq.TDL_PATIENT_MILITARY_RANK_NAME);
                dicParam.Add("VIR_ADDRESS", currentServiceReq.TDL_PATIENT_ADDRESS);
                dicParam.Add("AGE", "8");
                dicParam.Add("AGE_STRING", "8 giờ tuổi / 8 ngày tuổi / 8 tháng tuổi / 8 tuổi");
                dicParam.Add("STR_YEAR", "2010");
                dicParam.Add("VIR_PATIENT_NAME", currentServiceReq.TDL_PATIENT_NAME);
                dicParam.Add("EXECUTE_DEPARTMENT_CODE", "");
                dicParam.Add("EXECUTE_DEPARTMENT_NAME", "");
                dicParam.Add("EXECUTE_ROOM_CODE", "");
                dicParam.Add("EXECUTE_ROOM_NAME", "");
                dicParam.Add("REQUEST_DEPARTMENT_CODE", "");
                dicParam.Add("REQUEST_DEPARTMENT_NAME", "");
                dicParam.Add("REQUEST_ROOM_CODE", "");
                dicParam.Add("REQUEST_ROOM_NAME", "");
                dicParam.Add("HEIN_CARD_NUMBER_SEPARATE", "");
                dicParam.Add("STR_HEIN_CARD_FROM_TIME", "");
                dicParam.Add("STR_HEIN_CARD_TO_TIME", "");
                dicParam.Add("HEIN_CARD_ADDRESS", "");
                dicParam.Add("PATIENT_TYPE_NAME", "");
                dicParam.Add("TREATMENT_TYPE_NAME", "");
                dicParam.Add("SAMPLE_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(time));
                dicParam.Add("RECEIVE_SAMPLE_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(time));
                dicParam.Add("PAAN_POSITION_NAME", currentServiceReq.PAAN_POSITION_ID);
                dicParam.Add("TREATMENT_ICD_CODE", TreatmentWithPatientTypeAlter.ICD_CODE);
                dicParam.Add("TREATMENT_ICD_NAME", TreatmentWithPatientTypeAlter.ICD_NAME);
                dicParam.Add("TREATMENT_ICD_SUB_CODE", TreatmentWithPatientTypeAlter.ICD_SUB_CODE);
                dicParam.Add("TREATMENT_ICD_TEXT", TreatmentWithPatientTypeAlter.ICD_TEXT);
                AddKeyIntoDictionaryPrint<HisTreatmentWithPatientTypeInfoSDO>(TreatmentWithPatientTypeAlter, dicParam, false);
                AddKeyIntoDictionaryPrint<PatientADO>(patient, dicParam, false);
                AddKeyIntoDictionaryPrint<HIS_SERVICE_REQ>(currentServiceReq, dicParam, true);
                AddKeyIntoDictionaryPrint<HIS_SERE_SERV>(sereServ, dicParam, false);
                AddKeyIntoDictionaryPrint<HIS_SERE_SERV_EXT>(sereServExt, dicParam, true);
                dicParam.Add("USER_NAME", "");
                dicParam.Add("CONCLUDE_NEW", "<#CONCLUDE;>");
                dicParam.Add("NOTE_NEW", "<#NOTE;>");

                for (int i = 0; i < 10; i++)
                {
                    dicParam.Add("IMAGE_DATA_" + (i + 1), null);
                }

                result = dicParam;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> dicParamPlus, bool autoOveride)
        {
            try
            {
                if (data != null)
                {
                    PropertyInfo[] pis = typeof(T).GetProperties();
                    if (pis != null && pis.Length > 0)
                    {
                        foreach (var pi in pis)
                        {
                            if (pi.GetGetMethod().IsVirtual) continue;

                            var searchKey = dicParamPlus.SingleOrDefault(o => o.Key == pi.Name);
                            if (String.IsNullOrEmpty(searchKey.Key))
                            {
                                dicParamPlus.Add(pi.Name, pi.GetValue(data));
                            }
                            else
                            {
                                if (autoOveride)
                                    dicParamPlus[pi.Name] = pi.GetValue(data);
                                else if (dicParamPlus[pi.Name] == null)
                                    dicParamPlus[pi.Name] = pi.GetValue(data);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hiển thị định dạng 23:59 Ngày 12 tháng 10 năm 2015
        /// </summary>
        /// <returns></returns>
        internal static string GetCurrentTimeSeparateBeginTime(System.DateTime now)
        {
            string result = "";
            try
            {
                if (now != DateTime.MinValue)
                {
                    string month = string.Format("{0:00}", now.Month);
                    string day = string.Format("{0:00}", now.Day);
                    string hour = string.Format("{0:00}", now.Hour);
                    string hours = string.Format("{0:00}", now.Hour);
                    string minute = string.Format("{0:00}", now.Minute);
                    string strNgay = "ngày";
                    string strThang = "tháng";
                    string strNam = "năm";
                    result = string.Format("{0}" + ":" + "{1} " + strNgay + " {2} " + strThang + " {3} " + strNam + " {4}", hours, minute, now.Day, now.Month, now.Year);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var data = new List<TemplateKeyAdo>();

                var search = txtSearch.Text.Trim().ToLower();
                if (!String.IsNullOrEmpty(search))
                {
                    data = listTemplateKey.Where(o => o.KEY.ToLower().Contains(search)).ToList();
                }
                else
                {
                    data = listTemplateKey;
                }

                GridControlKey.BeginUpdate();
                GridControlKey.DataSource = data;
                GridControlKey.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridViewKey_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (TemplateKeyAdo)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
