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
            }
        }
        T CreateRequest<T>(string requestUri, Dictionary<string, object> sendData)
        {
            T data = default(T);
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (var client = new HttpClient())
                {
                    string fullrequestUri = requestUri;

                    client.BaseAddress = new Uri(requestUri);
                    client.Timeout = new TimeSpan(0, 5, 0);
                    client.DefaultRequestHeaders.Accept.Clear();
                    var stringPayload = JsonConvert.SerializeObject(sendData);
                    var content = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                    HttpResponseMessage resp = null;
                    try
                    {
                        Inventec.Common.Logging.LogSystem.Debug("_____sendJsonData : " + stringPayload);
                        resp = client.PostAsync(fullrequestUri, content).Result;
                    }
                    catch (HttpRequestException ex)
                    {
                        throw new Exception("Lỗi kết nối đến");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    if (resp == null || !resp.IsSuccessStatusCode)
                    {
                        int statusCode = resp.StatusCode.GetHashCode();
                        if (resp.Content != null)
                        {
                            try
                            {
                                string errorData = resp.Content.ReadAsStringAsync().Result;
                                Inventec.Common.Logging.LogSystem.Error("errorData: " + errorData);
                            }
                            catch { }
                        }

                        throw new Exception(string.Format(" trả về thông tin lỗi. Mã lỗi: {0}", statusCode));
                    }
                    string responseData = resp.Content.ReadAsStringAsync().Result;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => responseData), responseData));
                    data = JsonConvert.DeserializeObject<T>(responseData);
                    if (data == null)
                    {
                        throw new Exception("Dữ liệu trả về không đúng");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
