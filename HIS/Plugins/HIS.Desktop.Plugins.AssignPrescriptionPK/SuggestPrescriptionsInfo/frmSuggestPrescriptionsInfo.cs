using DevExpress.Data;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Edit;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using Inventec.Desktop.Common.Message;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.SuggestPrescriptionsInfo
{
    public partial class frmSuggestPrescriptionsInfo : FormBase
    {
        AISuggestionData aISuggestionData = new AISuggestionData();
        Action<List<MediMatyTypeADO>> Results { get; set; }
        object requestData = null;
        /// <summary>vCong 62522: lỗi gần nhất khi gọi dịch vụ AI gợi ý kê đơn.</summary>
        string lastErrorMessage;

        static Inventec.Common.IntegrationError.IntegrationPartner AiPartner
        {
            get { return Inventec.Common.IntegrationError.IntegrationPartner.Ai("dịch vụ AI gợi ý kê đơn"); }
        }
        public frmSuggestPrescriptionsInfo(
            Action<List<MediMatyTypeADO>> Results, object requestData)
        {
            InitializeComponent();
            this.Results = Results;
            this.requestData = requestData;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmSuggestPrescriptionsInfo_LoadAsync(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SuggestPrescriptionsInfoADO data = CreateRequest<SuggestPrescriptionsInfoADO>(HisConfigCFG.SuggestPrescriptionsInfo, requestData);
                if (data == null)
                {
                    // vCong 62522: truoc day data null -> NullReference, form trong va khong bao gi
                    WaitingManager.Hide();
                    string msg = !string.IsNullOrEmpty(this.lastErrorMessage) ? this.lastErrorMessage
                        : Inventec.Common.IntegrationError.IntegrationErrorMessage.NoResponse(AiPartner, "lấy gợi ý kê đơn");
                    lblNote.Text = msg;
                    DevExpress.XtraEditors.XtraMessageBox.Show(msg, "Thông báo");
                    return;
                }
                aISuggestionData = data.ai_suggestion;

                WaitingManager.Hide();
                if (aISuggestionData == null || aISuggestionData.MediMatySuggestions == null)
                {
                    lblNote.Text = "Không có gợi ý thuốc/vật tư nào cho yêu cầu này.";
                    return;
                }
                LoadAISuggestion();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                DevExpress.XtraEditors.XtraMessageBox.Show(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, AiPartner, "lấy gợi ý kê đơn"), "Thông báo");
            }
        }
        public T CreateRequest<T>(string requestUri, object sendData)
        {
            T data = default(T);
            this.lastErrorMessage = null;
            try
            {
                if (string.IsNullOrWhiteSpace(requestUri))
                    throw new Inventec.Common.IntegrationError.IntegrationException(Inventec.Common.IntegrationError.ErrorSource.Config, AiPartner, "Chưa thiết lập địa chỉ dịch vụ AI gợi ý kê đơn (cấu hình hệ thống HIS.Desktop.AI.SuggestPrescriptionsInfo)");

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (var client = new HttpClient())
                {
                    string fullrequestUri = requestUri;

                    client.BaseAddress = new Uri(requestUri);
                    client.Timeout = new TimeSpan(0, 5, 0);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("AuthenKey", "j2fyWtvUDZwYoPqT1pyqHYAFGY/3PZVR87/CoghZr8ttkZn1/RHZuQg89cPz9sqCBIG27uisGRNEfe2BP2M/m3qMdf8moG+ypGl7nVHVc7VVLSSaNGDA42iQwW4vnC01ngqrN0CidHiI12ZBawXFlVfFh+2UpLE3lSd8hR2o97nq++6DQ9MBzuEfzKDnV3Qsyq+VwPm4yoKz/2kum7TUWWcqT6pnvZb5qdezXiMItqLY8SI2JRPcc+TDxQ4mD9z3wC9JsEDk/uBXJy259PEqBbTxA+rL+cs+6fevZnnXqhjQgG9MIfe0lcQ0n9xVdDYvZZaE8Q4/CrUb52CjmDvwCw==");
                    var stringPayload = JsonConvert.SerializeObject(sendData);
                    var content = new StringContent(stringPayload, Encoding.UTF8, "application/json");


                    HttpResponseMessage resp = null;
                    try
                    {
                        Inventec.Common.Logging.LogSystem.Debug("_____sendJsonData : " + stringPayload);
                        resp = client.PostAsync(fullrequestUri, content).Result;
                    }
                    catch (Exception ex)
                    {
                        // vCong 62522: timeout/DNS/SSL/khong ket noi -> exception co nguon
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.WrapCall(ex, AiPartner, fullrequestUri);
                    }
                    if (resp == null || !resp.IsSuccessStatusCode)
                    {
                        Inventec.Common.IntegrationError.IntegrationException httpEx = Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(AiPartner, fullrequestUri, resp);
                        Inventec.Common.Logging.LogSystem.Error("errorData: " + httpEx.Message);
                        throw httpEx;
                    }
                    string responseData = Inventec.Common.IntegrationError.IntegrationHttpError.ReadBody(resp);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => responseData), responseData));
                    try
                    {
                        data = JsonConvert.DeserializeObject<T>(responseData);
                    }
                    catch (Exception ex)
                    {
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.ParseError(AiPartner, fullrequestUri, responseData, ex);
                    }
                    if (data == null)
                    {
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.ParseError(AiPartner, fullrequestUri, responseData, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                this.lastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, AiPartner, "lấy gợi ý kê đơn");
            }

            return data;
        }
        public void LoadAISuggestion()
        {
            try
            {
                aISuggestionData.MediMatySuggestions.ForEach(o =>
                {
                    if (!string.IsNullOrEmpty(o.HTU_CODE_NAME))
                    {
                        var htu = BackendDataWorker.Get<HIS_HTU>().FirstOrDefault(p => p.HTU_CODE == o.HTU_CODE_NAME.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim());
                        o.HTU_CODE_NAME = htu != null ? htu.HTU_NAME : null;
                    }
                    else
                    {
                        o.HTU_CODE_NAME = " - ";
                    }

                    if (!string.IsNullOrEmpty(o.SERVICE_UNIT_CODE_NAME))
                    {
                        var uni = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(p => p.SERVICE_UNIT_CODE == o.SERVICE_UNIT_CODE_NAME.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim());
                        o.SERVICE_UNIT_CODE_NAME = uni != null ? uni.SERVICE_UNIT_NAME : null;
                    }
                    else o.SERVICE_UNIT_CODE_NAME = " - ";
                });
                grcService.DataSource = aISuggestionData.MediMatySuggestions;
                lblNote.Text = aISuggestionData.Explanation;
                lblEnd.Text = "       " + aISuggestionData.End;
                grvService.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void repMmEdit_Popup(object sender, EventArgs e)
        {
            try
            {
                MemoExPopupForm form = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as MemoExPopupForm;
                form.OkButton.Text = "Đồng ý";
                form.CloseButton.Text = "Huỷ bỏ";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void btnClosefrm_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                var data = grvService.GetSelectedRows()
                    .Select(r => grvService.GetRow(r) as MediMatySuggestionsADO)
                    .Where(m => m != null)
                    .ToList();
                Results((from m in data
                         select new MediMatyTypeADO(m, BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.SERVICE_UNIT_NAME == (m.SERVICE_UNIT_CODE_NAME ?? " - ").Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim()), BackendDataWorker.Get<HIS_HTU>().FirstOrDefault(o => o.HTU_NAME == (m.HTU_CODE_NAME ?? " - ").Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim()))).ToList());

                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvService_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
        }

        private void repMmEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
