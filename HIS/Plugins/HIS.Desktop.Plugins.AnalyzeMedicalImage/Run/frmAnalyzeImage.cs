using DevExpress.Utils.OAuth.Provider;
using DevExpress.XtraEditors;
using DevExpress.XtraExport;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraPrinting.Native;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AnalyzeMedicalImage.ADO;
using HIS.Desktop.Plugins.AnalyzeMedicalImage.Config;
using HIS.Desktop.Plugins.AnalyzeMedicalImage.PopopImg;
using HIS.Desktop.Utility;
using IMSys.DbConfig.HIS_RS;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using Newtonsoft.Json;
using SAR.EFMODEL.DataModels;   
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Utils.Drawing.Helpers.NativeMethods;

namespace HIS.Desktop.Plugins.AnalyzeMedicalImage.Run
{
    public partial class frmAnalyzeImage : FormBase
    {
        DelegateSelectData DelSelectData { get; set; }                   
        public long TreatmentId = 0;
        public TimeSpan? TimeOut = null;
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> SereServs = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
        List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT> lstvEmrDocument = new List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>();
        List<HIS_SERE_SERV_FILE> lstSereServFile = new List<HIS_SERE_SERV_FILE>();
        List<MOS.EFMODEL.DataModels.HIS_TREATMENT> lsthisTreatment = new List<MOS.EFMODEL.DataModels.HIS_TREATMENT>();
        List<HIS_SERE_SERV> lstHisSereSerrv = new List<HIS_SERE_SERV>();
        List<HIS_SERE_SERV_EXT> lstSereServExt = new List<HIS_SERE_SERV_EXT>();
        List<SAR.EFMODEL.DataModels.SAR_PRINT> lstsarPrint = new List<SAR.EFMODEL.DataModels.SAR_PRINT>();
        List<SereServFileADO> lstSereServFileADO = new List<SereServFileADO>();
        List<EMR.EFMODEL.DataModels.EMR_TREATMENT> lstEmrTreatment = new List<EMR.EFMODEL.DataModels.EMR_TREATMENT>();   
        private SereServFileADO currentDataClick;
        AIResponse aiResponse = new AIResponse();
        public frmAnalyzeImage(long TreatmentId, TimeSpan? TimeOut, DelegateSelectData DelSelectData, List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> SereServs)
        {
            InitializeComponent();
            try
            {
                this.TreatmentId = TreatmentId;
                this.TimeOut = TimeOut;   
                this.DelSelectData = DelSelectData;
                this.SereServs = SereServs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmAnalyzeImage_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataTreatment();
                ProcessSereServImp();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataTreatment()
        {    
            try
            {
                if (this.TreatmentId != null && this.TreatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = this.TreatmentId;
                    LogSystem.Info("HisTreatmentFilter " + LogUtil.TraceData("HisTreatmentFilter ", filter));
                    lsthisTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                    LogSystem.Info("lsthisTreatment " + LogUtil.TraceData("lsthisTreatment ", lsthisTreatment));
                    MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment = lsthisTreatment.FirstOrDefault();
                    if (hisTreatment != null)
                    {
                        lblTreatmentCode.Text = hisTreatment.TREATMENT_CODE;
                        lblPatientName.Text = hisTreatment.TDL_PATIENT_NAME;
                        lblGender.Text = hisTreatment.TDL_PATIENT_GENDER_NAME;
                        lblIcdName.Text = hisTreatment.ICD_CODE + " - " + hisTreatment.ICD_NAME;
                        lblIcdText.Text = hisTreatment.ICD_SUB_CODE + " - " + hisTreatment.ICD_TEXT;
                        if (hisTreatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        {
                            lblPatientDoc.Text = String.Format("{0} ({1})", hisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4), MPS.AgeUtil.CalculateFullAge(hisTreatment.TDL_PATIENT_DOB)); ;
                        }
                        else
                        {
                            lblPatientDoc.Text = String.Format("{0} ({1})", Inventec.Common.DateTime.Convert.TimeNumberToDateString(hisTreatment.TDL_PATIENT_DOB), MPS.AgeUtil.CalculateFullAge(hisTreatment.TDL_PATIENT_DOB)); ;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<SereServFileADO> ProcessSereServImp()
        {
            List<SereServFileADO> lstSereServFileADOs = new List<SereServFileADO>();
            try
            {
                Inventec.Desktop.Common.Message.WaitingManager.Show();
                if (this.TreatmentId != null && this.TreatmentId > 0)
                {
                    FillDataSereServ(this.TreatmentId, this.SereServs);
                    
                    foreach (var sereServ in lstHisSereSerrv)
                    {
                        var sereServAdo = new SereServFileADO();
                        CopyProperties(sereServ, sereServAdo);
                        lstSereServFileADOs.Add(sereServAdo);
                    }

                    if (lstHisSereSerrv != null && lstHisSereSerrv.Count > 0)
                    {
                        FillDataSarPrint(lstHisSereSerrv, lstSereServFileADOs, this.TreatmentId);
                    }

                    if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HAS_CONNECTION_EMR") == "1")
                    {
                        FillDataEmrDocuments(lstSereServFileADOs, lstHisSereSerrv);
                    }
                }   

                FillDataAttachedFilesFss(lstHisSereSerrv, lstSereServFileADOs);
                LogSystem.Info("lstHisSereSerrv___1 " + LogUtil.TraceData("lstHisSereSerrv___1 ", lstHisSereSerrv));
                lstSereServFileADO = lstSereServFileADOs.Where(o => (o.URL != null && o.URL_NAME != null) || o.FileContent != null).GroupBy(o => o.SERVICE_ID).Select(g => g.First()).ToList();//.OrderBy(o => o.Key).ToList();

                LogSystem.Info("lstHisSereSerrv___2 " + LogUtil.TraceData("lstHisSereSerrv___2 ", lstHisSereSerrv));
                CardControl.DataSource = lstSereServFileADO;

                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                return lstSereServFileADO;
                
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        private void FillDataAttachedFilesFss(List<HIS_SERE_SERV> lstsereServ, List<SereServFileADO> lstSereServFileADOs)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServFileFilter filter = new HisSereServFileFilter();
                filter.SERE_SERV_IDs = lstsereServ.Select(o => o.ID).ToList();
                LogSystem.Info("filter.SERE_SERV_IDs " + LogUtil.TraceData("filter.SERE_SERV_IDs ", lstsereServ.Select(o => o.ID).ToList()));
                lstSereServFile = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE>>("api/HisSereServFile/Get", ApiConsumers.MosConsumer, filter, param).ToList();

                if (lstSereServFile != null && lstSereServFile.Count > 0)
                {
                    string[] validExtensions = new[] { ".png", ".jpg", ".jpeg" };
                    foreach (var file in lstSereServFile)
                    {
                        var sereServAdo = lstSereServFileADOs.FirstOrDefault(x => x.ID == file.SERE_SERV_ID);
                        if (sereServAdo != null)
                        {
                            string ext = Path.GetExtension(file.URL)?.ToLower();
                            string finalUrl = file.URL;

                            var newSereServAdo = new SereServFileADO
                            {
                                ID = sereServAdo.ID,
                                SERVICE_ID = sereServAdo.SERVICE_ID,
                                ServiceName = sereServAdo.TDL_SERVICE_NAME,
                                TDL_EXECUTE_BRANCH_ID = sereServAdo.TDL_EXECUTE_BRANCH_ID,
                                TDL_SERVICE_NAME = sereServAdo.TDL_SERVICE_NAME,
                                TDL_INTRUCTION_TIME = sereServAdo.TDL_INTRUCTION_TIME,
                                CREATE_TIME = file.CREATE_TIME,
                                URL_NAME = file.SERE_SERV_FILE_NAME,
                                URL = finalUrl,
                                IsFss = false,
                                IsChecked = true,
                                Key = string.Format("{0}_{1}_{2}", sereServAdo.TDL_INTRUCTION_TIME, sereServAdo.ID, sereServAdo.TDL_SERVICE_NAME)
                            };

                            if (!validExtensions.Contains(ext))
                            {
                                newSereServAdo.Extension = finalUrl.Split('.').LastOrDefault();
                                finalUrl = GetPathDefault();
                                newSereServAdo.image = System.Drawing.Image.FromFile(finalUrl);
                            }
                            else
                            {
                                MemoryStream fs = null;
                                fs = Inventec.Fss.Client.FileDownload.GetFile(finalUrl);
                                if (fs != null && fs.Length > 0)
                                {
                                    newSereServAdo.image = System.Drawing.Image.FromStream(fs);
                                }
                                newSereServAdo.Extension = finalUrl.Split('.').LastOrDefault();
                            }

                            lstSereServFileADOs.Add(newSereServAdo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string GetPathDefault()
        {
            string imageDefaultPath = string.Empty;
            try
            {
                string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                imageDefaultPath = localPath + "\\Img\\ImageStorage\\notImage.jpg";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return imageDefaultPath;
        }
        private void FillDataEmrDocuments( List<SereServFileADO> lstSereServFileADOs, List<HIS_SERE_SERV> lstHisSereSerrv)  
        {
            try
            {
                CommonParam common = new CommonParam();
                EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
                filter.TREATMENT_CODE__EXACT = lblTreatmentCode.Text;
                filter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
                filter.IS_DELETE = false;
                LogSystem.Info("EmrDocumentViewFilter " + LogUtil.TraceData("EmrDocumentViewFilter ", filter));

                lstvEmrDocument = new Inventec.Common.Adapter.BackendAdapter(common)
                    .Get<List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, filter, common).ToList();
                
                LogSystem.Info("lstvEmrDocument " + LogUtil.TraceData("lstvEmrDocument ", lstvEmrDocument));
                
                if (lstvEmrDocument != null && lstvEmrDocument.Count > 0)
                {
                    string pathDefault = GetPathDefault();
                    foreach (var item in lstHisSereSerrv)
                    {
                        var emrDocs = lstvEmrDocument.Where(o => !string.IsNullOrEmpty(o.HIS_CODE) &&
                            o.HIS_CODE.Contains("SERVICE_REQ_CODE:" + item.TDL_SERVICE_REQ_CODE)).ToList();

                        if (emrDocs.Any())
                        {
                            var sereServAdo = lstSereServFileADOs.FirstOrDefault(x => x.ID == item.ID);   
                            if (sereServAdo != null)
                            {
                                foreach (var emrDoc in emrDocs)
                                {
                                    var newSereServAdo = new SereServFileADO
                                    {
                                        ID = sereServAdo.ID,
                                        SERVICE_ID = sereServAdo.SERVICE_ID,
                                        ServiceName = sereServAdo.TDL_SERVICE_NAME,
                                        TDL_EXECUTE_BRANCH_ID = sereServAdo.TDL_EXECUTE_BRANCH_ID,
                                        TDL_SERVICE_NAME = sereServAdo.TDL_SERVICE_NAME,
                                        TDL_INTRUCTION_TIME = sereServAdo.TDL_INTRUCTION_TIME,
                                        URL_NAME = emrDoc.DOCUMENT_NAME,
                                        Extension = "pdf",
                                        IsChecked = true,
                                        IsFss = false,
                                        URL = emrDoc.LAST_VERSION_URL,
                                        Key = string.Format("{0}_{1}_{2}", sereServAdo.TDL_INTRUCTION_TIME, sereServAdo.ID, sereServAdo.TDL_SERVICE_NAME),
                                        image = System.Drawing.Image.FromFile(pathDefault)
                                    };

                                    lstSereServFileADOs.Add(newSereServAdo);
                                }
                            }
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataSarPrint(List<HIS_SERE_SERV> lstsereServ, List<SereServFileADO> lstSereServFileADOs, long TreatmentId)
        {
            try
            {
                Inventec.Desktop.Common.Message.WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisSereServExtFilter filter = new HisSereServExtFilter();
                filter.TDL_TREATMENT_ID = TreatmentId;
                filter.SERE_SERV_IDs  = lstsereServ.Select(o => o.ID).ToList();
                LogSystem.Info("HisSereServExtFilter " + LogUtil.TraceData("HisSereServExtFilter ", filter));
                lstSereServExt = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, param).ToList();
                LogSystem.Info("lstSereServExt " + LogUtil.TraceData("lstSereServExt ", lstSereServExt));
                if (lstSereServExt != null && lstSereServExt.Count > 0)
                {
                    foreach (var ext in lstSereServExt)
                    {
                        var sereServAdo = lstSereServFileADOs.FirstOrDefault(x => x.ID == ext.SERE_SERV_ID);
                        if (sereServAdo != null)
                        {
                            if (ext.DESCRIPTION_SAR_PRINT_ID != null)
                            {
                                lstSereServFileADOs.Add(ProcessSarPrint(ext.DESCRIPTION_SAR_PRINT_ID, sereServAdo));
                            }
                            else if (!string.IsNullOrEmpty(ext.DESCRIPTION))
                            {
                                SereServFileADO newSereServAdo = new SereServFileADO();
                                newSereServAdo.ID = sereServAdo.ID;
                                newSereServAdo.SERVICE_ID = sereServAdo.SERVICE_ID;
                                newSereServAdo.ServiceName = sereServAdo.TDL_SERVICE_NAME;
                                newSereServAdo.TDL_EXECUTE_BRANCH_ID = sereServAdo.TDL_EXECUTE_BRANCH_ID;
                                newSereServAdo.TDL_SERVICE_NAME = sereServAdo.TDL_SERVICE_NAME;
                                newSereServAdo.TDL_INTRUCTION_TIME = sereServAdo.TDL_INTRUCTION_TIME;
                                newSereServAdo.URL_NAME = "Mô tả";
                                newSereServAdo.Content = ext.DESCRIPTION;
                                newSereServAdo.Key = string.Format("{0}_{1}_{2}", sereServAdo.TDL_INTRUCTION_TIME, sereServAdo.ID, sereServAdo.TDL_SERVICE_NAME);
                                string pathLocal = GetPathDefault();
                                newSereServAdo.image = System.Drawing.Image.FromFile(pathLocal);
                                newSereServAdo.IsChecked = true;
                                newSereServAdo.IsFss = false;
                                lstSereServFileADOs.Add(newSereServAdo);
                            }
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }        
        private SereServFileADO ProcessSarPrint(string sarPrintId, SereServFileADO lstSereServFileADOs)
        {
            try
            {
                CommonParam param = new CommonParam();
                SereServFileADO newSereServAdo = new SereServFileADO();
                SAR.Filter.SarPrintFilter printFilter = new SAR.Filter.SarPrintFilter();
                printFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                printFilter.ID = long.Parse(sarPrintId);
                
                lstsarPrint = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>("api/SarPrint/Get", ApiConsumers.SarConsumer, printFilter, param).ToList();
                
                if (lstsarPrint != null && lstsarPrint.Count > 0)
                {
                    var firstPrint = lstsarPrint.First();
                    newSereServAdo.ID = lstSereServFileADOs.ID;
                    newSereServAdo.SERVICE_ID = lstSereServFileADOs.SERVICE_ID;
                    newSereServAdo.ServiceName = lstSereServFileADOs.TDL_SERVICE_NAME;
                    newSereServAdo.TDL_EXECUTE_BRANCH_ID = lstSereServFileADOs.TDL_EXECUTE_BRANCH_ID;
                    newSereServAdo.TDL_SERVICE_NAME = lstSereServFileADOs.TDL_SERVICE_NAME;
                    newSereServAdo.TDL_INTRUCTION_TIME = lstSereServFileADOs.TDL_INTRUCTION_TIME;
                    newSereServAdo.URL_NAME = firstPrint.TITLE;
                    newSereServAdo.Key = string.Format("{0}_{1}_{2}", lstSereServFileADOs.TDL_INTRUCTION_TIME, lstSereServFileADOs.ID, lstSereServFileADOs.TDL_SERVICE_NAME);
                    newSereServAdo.FileContent = firstPrint.CONTENT;
                    if (Utility.TextLibHelper.BytesToString(firstPrint.CONTENT) != null)
                    {
                        newSereServAdo.Content = Utility.TextLibHelper.BytesToString(firstPrint.CONTENT);
                    }
                    newSereServAdo.IsChecked = true;
                    newSereServAdo.IsFss = false;
                    string pathLocal = GetPathDefault();
                    newSereServAdo.image = System.Drawing.Image.FromFile(pathLocal);
                }

                string text = Utility.TextLibHelper.BytesToString(newSereServAdo.FileContent);
                if (!string.IsNullOrEmpty(text))
                    return newSereServAdo;
                return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        private void FillDataSereServ(long TreatmentId, List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> ServiceReqs)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<long> ServiceTypeIds = new List<long>();
                ServiceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS);
                ServiceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA);
                ServiceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);
                ServiceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA);
                ServiceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL);
                ServiceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT);
                ServiceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN);
                HisSereServFilter hisSereServFilter = new HisSereServFilter();
                hisSereServFilter.TREATMENT_ID = this.TreatmentId;
                hisSereServFilter.TDL_SERVICE_TYPE_IDs = ServiceTypeIds;
                hisSereServFilter.HAS_EXECUTE = true;

                if (this.SereServs != null && this.SereServs.Count > 0)
                {
                    hisSereServFilter.IDs = ServiceReqs.Select(x => x.ID).ToList();
                }

                lstHisSereSerrv = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, hisSereServFilter, param).Where(o => o.IS_DELETE != 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CopyProperties(HIS_SERE_SERV source, SereServFileADO target)
        {
            try
            {
                var sourceProperties = typeof(HIS_SERE_SERV).GetProperties();
                foreach (var prop in sourceProperties)
                {
                    if (prop.CanRead && prop.CanWrite)
                    {
                        var value = prop.GetValue(source);
                        prop.SetValue(target, value);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnImageAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                List<long> branhid= GetBranchCode(lstSereServFileADO);
                var branchCode = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().FirstOrDefault(p => branhid.Contains(p.ID));
                AIResponse data = new AIResponse();
                if (!string.IsNullOrEmpty(AppConfigKeys.AIConnectionInfo))
                {
                    var requestData = new
                    {
                        AppCode = "HIS",
                        FileContents = GetFileContents(lstSereServFileADO),
                        Base64Imagess = GetBase64Images(lstSereServFileADO),
                        LoginName = branchCode.BRANCH_CODE + "." + loginName,
                        Prompt = "Phân tích nội dung hình ảnh y tế được cung cấp. Mô tả chi tiết các đặc điểm bất thường (nếu có) như màu sắc, hình dạng, kích thước, mật độ, hoặc các dấu hiệu lạ. Đưa ra các giả thuyết hoặc khả năng y tế có thể liên quan đến những đặc điểm đó. Gợi ý bệnh nhân nên khám tại chuyên khoa nào để được chẩn đoán chính xác hơn. Đưa ra các lời khuyên ban đầu về cách giảm đau hoặc tự theo dõi trước khi đi khám. Lưu ý: Bạn chỉ mô tả và gợi ý một cách tham khảo, không đưa ra chẩn đoán y khoa chính thức. Hãy trả lời hoàn toàn bằng tiếng Việt, chính xác, chi tiết và dễ hiểu"
                    };

                    using (var client = new HttpClient())
                    {
                        var authenKey = "j2fyWtvUDZwYoPqT1pyqHYAFGY/3PZVR87/CoghZr8ttkZn1/RHZuQg89cPz9sqCBIG27uisGRNEfe2BP2M/m3qMdf8moG+ypGl7nVHVc7VVLSSaNGDA42iQwW4vnC01ngqrN0CidHiI12ZBawXFlVfFh+2UpLE3lSd8hR2o97nq++6DQ9MBzuEfzKDnV3Qsyq+VwPm4yoKz/2kum7TUWWcqT6pnvZb5qdezXiMItqLY8SI2JRPcc+TDxQ4mD9z3wC9JsEDk/uBXJy259PEqBbTxA+rL+cs+6fevZnnXqhjQgG9MIfe0lcQ0n9xVdDYvZZaE8Q4/CrUb52CjmDvwCw==";

                        string fullrequestUri = AppConfigKeys.AIConnectionInfo;
                        client.BaseAddress = new Uri(AppConfigKeys.AIConnectionInfo);
                        client.Timeout = TimeOut ?? TimeSpan.FromMinutes(5);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Add("AuthenKey", authenKey);

                        var stringPayload = JsonConvert.SerializeObject(requestData);
                        var content = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                        Inventec.Desktop.Common.Message.WaitingManager.Show();
                        HttpResponseMessage resp = null;
                        try
                        {
                            Inventec.Common.Logging.LogSystem.Debug("_____sendJsonData : " + stringPayload);
                            resp = client.PostAsync(fullrequestUri, content).Result;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resp), resp));
                        }
                        catch (HttpRequestException ex)
                        {

                            throw new Exception("Lỗi kết nối đến");
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        Inventec.Desktop.Common.Message.WaitingManager.Hide();
                        if (resp == null || !resp.IsSuccessStatusCode)
                        {
                            int statusCode = resp.StatusCode.GetHashCode();
                            if (resp.Content != null)
                            {
                                string errorData = resp.Content.ReadAsStringAsync().Result;
                                Inventec.Common.Logging.LogSystem.Error("errorData: " + errorData);
                            }

                            throw new Exception(string.Format(" trả về thông tin lỗi. Mã lỗi: {0}", statusCode));
                        }

                        Inventec.Desktop.Common.Message.WaitingManager.Hide();
                        string responseData = resp.Content.ReadAsStringAsync().Result;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => responseData), responseData));
                        data = JsonConvert.DeserializeObject<AIResponse>(responseData);
                        aiResponse = data;

                        if (this.DelSelectData != null && data != null && !string.IsNullOrEmpty(data.Results))
                        {
                            btnResultsAI.Enabled = true;
                            lblYKhoa.Text = data.Results;
                        }
                        else if (data != null && !string.IsNullOrEmpty(data.Results))
                        {
                            lblYKhoa.Text = data.Results;
                        }

                        if (data == null)
                        {
                            throw new Exception("Dữ liệu trả về không đúng");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<long> GetBranchCode(List<SereServFileADO> selectedFiles)
        {
            try
            {
                var br = new List<long>();
                foreach (var item in selectedFiles)
                {
                    if (item.TDL_EXECUTE_BRANCH_ID != null && item.TDL_EXECUTE_BRANCH_ID > 0)
                    {
                        br.Add(item.TDL_EXECUTE_BRANCH_ID);  
                    }
                }
                return br.Distinct().ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);      
                return null;
            }
        }
        private List<string> GetFileContents(List<SereServFileADO> selectedFiles)
        {
            try
            {
                var fileContents = new List<string>();
                foreach (var item in selectedFiles)
                {
                    if (!string.IsNullOrEmpty(item.Content))
                    {
                        fileContents.Add(item.Content);
                    }
                }
                return fileContents;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        private List<string> GetBase64Images(List<SereServFileADO> selectedFiles)
        {
            try
            {
                var base64Images = new List<string>();
                MemoryStream stream = new MemoryStream();
                foreach (var item in selectedFiles )
                {
                    byte[] bytes = stream.ToArray();
                    string dataUrl = string.Format("data:{0};base64,{1}", item.Extension == "pdf" ? "pdf" : ("image/" + item.Extension), Convert.ToBase64String(bytes));

                    base64Images.Add(dataUrl);
                }
                return base64Images;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        private void toggleSwitch2_Toggled(object sender, EventArgs e)
        {
            try
            {
                bool check = false;
                if (toggleSwitch2.IsOn)
                {
                    check = false;
                    layoutControlItem7.Text = "Bỏ chọn tất cả";
                }
                else
                {
                    check = true;
                    layoutControlItem7.Text = "Chọn tất cả";
                }

                var lstData = CardControl.DataSource as List<SereServFileADO>;
                if (lstData != null && lstData.Count > 0)
                {
                    foreach (var item in lstData)
                    {
                        item.IsChecked = check;
                    }

                    CardControl.DataSource = null;
                    CardControl.DataSource = lstData;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void tileView1_ItemClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
            try
            {
                e.Item.Checked = !e.Item.Checked;
                TileView tileView = sender as TileView;
                this.currentDataClick = (SereServFileADO)tileView.GetRow(e.Item.RowHandle);
                this.currentDataClick.IsChecked = e.Item.Checked;
                foreach (var item in lstSereServFileADO)
                {
                    if (item == currentDataClick)
                    {
                        item.IsChecked = currentDataClick.IsChecked;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void tileView1_ItemDoubleClick(object sender, TileViewItemClickEventArgs e)
        {
            try
            {
                TileView tileView = sender as TileView;
                this.currentDataClick = (SereServFileADO)tileView.GetRow(e.Item.RowHandle);
                this.currentDataClick.IsFss = true;
                foreach (var item in lstSereServFileADO)
                {
                    if (item == currentDataClick)
                    {
                        item.IsFss = true;
                        break;
                    }
                }

                if (lstSereServFileADO != null && lstSereServFileADO.Count > 0)
                {
                    var frmImg = new frmImage(lstSereServFileADO);
                    frmImg.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnResultsAI_Click(object sender, EventArgs e)
        {
            try
            {
                if (aiResponse != null)
                {
                    this.DelSelectData(aiResponse.Results);
                }
                
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void lblYKhoa_Click(object sender, EventArgs e)
        {

        }
    }
}
