using DevExpress.XtraEditors.Popup;
using DevExpress.XtraPrinting.Native;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.AssignService.ADO;
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService
{
    public partial class frmSuggestServiceAi : FormBase
    {
        private Action<List<SereServADO>> Results { get; set; }
        private RequestAIADO Request { get; set; }
        /// <summary>vCong 62522: lỗi gần nhất khi gọi dịch vụ AI gợi ý chỉ định.</summary>
        private string lastErrorMessage;

        private static Inventec.Common.IntegrationError.IntegrationPartner AiPartner
        {
            get { return Inventec.Common.IntegrationError.IntegrationPartner.Ai("dịch vụ AI gợi ý chỉ định"); }
        }
        public frmSuggestServiceAi( RequestAIADO request, Action<List<SereServADO>> Results)
        {
            InitializeComponent();

            try
            {
                this.Results = Results;
                this.Request = request;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void frmHintByAi_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var result = CreateRequest<ResponseAIADO>(HisConfigCFG.SuggestAssignServicesInfo, new Dictionary<string, object>()
                        {
                            { "icd_code", Request.icd_code },
                            { "gender", Request.gender_name},
                            { "age", Request.age},
                            { "top_n", 5}, // Giá trị mặc định là 5

                        });
                WaitingManager.Hide();
                if (result == null && !string.IsNullOrEmpty(this.lastErrorMessage))
                {
                    // vCong 62522: loi goi dich vu AI -> bao ro nguon, khong hien "khong co goi y"
                    lblNote.Text = this.lastErrorMessage;
                    lblWarning.Text = string.Empty;
                    grcService.DataSource = null;
                    DevExpress.XtraEditors.XtraMessageBox.Show(this.lastErrorMessage, "Thông báo");
                    return;
                }
                if (result == null || result.AISuggestion == null || result.AISuggestion.ChiDinhGoiY == null)
                {
                    lblNote.Text = "Không có gợi ý dịch vụ nào cho yêu cầu này.";
                    lblWarning.Text = string.Empty;
                    grcService.DataSource = null;
                    return;
                }
                lblNote.Text = result.AISuggestion.Note;

                if (string.IsNullOrEmpty(result.AISuggestion.Warning))// ẩn label kết thúc nếu không có giá trị
                {
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lblWarning.Text = string.Empty;
                }
                else
                {
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lblWarning.Text = "    " + result.AISuggestion.Warning;// thụt đầu dòng cho dễ nhìn
                }
                    
                grcService.DataSource = result.AISuggestion.ChiDinhGoiY;
                grvService.SelectAll();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                DevExpress.XtraEditors.XtraMessageBox.Show(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, AiPartner, "lấy gợi ý chỉ định"), "Thông báo");
            }
        }
        T CreateRequest<T>(string requestUri, Dictionary<string, object> sendData)
        {
            T data = default(T);
            this.lastErrorMessage = null;
            try
            {
                if (string.IsNullOrWhiteSpace(requestUri))
                    throw new Inventec.Common.IntegrationError.IntegrationException(Inventec.Common.IntegrationError.ErrorSource.Config, AiPartner, "Chưa thiết lập địa chỉ dịch vụ AI gợi ý chỉ định (cấu hình hệ thống " + HisConfigCFG.CONFIG_KEY__SuggestAssignServicesInfo + ")");
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
                this.lastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, AiPartner, "lấy gợi ý chỉ định");
            }

            return data;
        }
        private void btnOk_Click(object sender, EventArgs e)
        {

            try
            {
                var selected = grvService.GetSelectedRows().Select(r => new SereServADO
                {
                    TDL_SERVICE_CODE = grvService.GetRowCellValue(r, "TDL_SERVICE_CODE").ToString(),
                    TDL_SERVICE_NAME = grvService.GetRowCellValue(r, "TDL_SERVICE_NAME").ToString(),
                    AMOUNT = Convert.ToDecimal(grvService.GetRowCellValue(r, "AMOUNT")),
                    InstructionNote = grvService.GetRowCellValue(r, "InstructionNote")?.ToString(),
                    IsExpend = grvService.GetRowCellValue(r, "IsExpend") is bool b && b,
                }).ToList();
                this.Results(selected); 
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void repMmEdit_Popup(object sender, EventArgs e)
        {
            MemoExPopupForm form = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as MemoExPopupForm;
            form.OkButton.Text = "Đồng ý";
            form.CloseButton.Text = "Bỏ qua";
        }

        private void grvService_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            //if (e.Column.FieldName == "IsExpend" && e.IsGetData)
            //{
            //    var item = (AISuggestionItem)e.Row;
            //    e.Value = (item.IsExpend ?? "").Trim() == "1";
            //}
        }
    }
}
