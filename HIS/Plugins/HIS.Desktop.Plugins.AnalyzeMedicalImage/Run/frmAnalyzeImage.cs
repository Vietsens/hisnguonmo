using DevExpress.Office.Utils;
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
using System.Threading;
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
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
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

                    lsthisTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);

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
                if (this.TreatmentId > 0)
                {
                    FillDataSereServ(this.TreatmentId, this.SereServs, lstSereServFileADOs);
                    
                    if (lstHisSereSerrv != null && lstHisSereSerrv.Count > 0)
                    {
                        FillDataSarPrint(lstHisSereSerrv, lstSereServFileADOs, this.TreatmentId);
                                     
                        FillDataAttachedFilesFss(lstHisSereSerrv, lstSereServFileADOs);
                    }

                    if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HAS_CONNECTION_EMR") == "1")
                    {
                        FillDataEmrDocuments(lstSereServFileADOs, lstHisSereSerrv);
                    }

                }
               
                lstSereServFileADO = lstSereServFileADOs;

                CardControl.DataSource = lstSereServFileADO.OrderBy(o => o.Key).ToList();

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

                lstSereServFile = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE>>("api/HisSereServFile/Get", ApiConsumers.MosConsumer, filter, param).ToList();

                if (lstSereServFile != null && lstSereServFile.Count > 0)
                {
                    string[] validExtensions = new[] { ".png", ".jpg", ".jpeg" };
                    foreach (var file in lstSereServFile)
                    {
                        var sereServAdo = lstsereServ.FirstOrDefault(x => x.ID == file.SERE_SERV_ID);
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
                                IsFss = true,
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

                lstvEmrDocument = new Inventec.Common.Adapter.BackendAdapter(common)
                    .Get<List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, filter, common).ToList();
                
                if (lstvEmrDocument != null && lstvEmrDocument.Count > 0)
                {
                    string pathDefault = GetPathDefault();
                    foreach (var item in lstHisSereSerrv)
                    {
                        var emrDocs = lstvEmrDocument.Where(o => !string.IsNullOrEmpty(o.HIS_CODE) &&
                            o.HIS_CODE.Contains("SERVICE_REQ_CODE:" + item.TDL_SERVICE_REQ_CODE)).ToList();

                        if (emrDocs.Any())
                        {
                            foreach (var emrDoc in emrDocs)
                            {
                                var newSereServAdo = new SereServFileADO
                                {
                                    ID = item.ID,
                                    SERVICE_ID = item.SERVICE_ID,
                                    ServiceName = item.TDL_SERVICE_NAME,
                                    TDL_EXECUTE_BRANCH_ID = item.TDL_EXECUTE_BRANCH_ID,
                                    TDL_SERVICE_NAME = item.TDL_SERVICE_NAME,
                                    TDL_INTRUCTION_TIME = item.TDL_INTRUCTION_TIME,
                                    URL_NAME = emrDoc.DOCUMENT_NAME + "_" + Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.TDL_INTRUCTION_TIME),
                                    Extension = "pdf",
                                    IsChecked = true,
                                    IsFss = true,
                                    URL = emrDoc.LAST_VERSION_URL,
                                    Key = string.Format("{0}_{1}_{2}", item.TDL_INTRUCTION_TIME, item.ID, item.TDL_SERVICE_NAME),
                                    image = System.Drawing.Image.FromFile(pathDefault)
                                };

                                lstSereServFileADOs.Add(newSereServAdo);
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

                if (lstsereServ != null && lstsereServ.Count > 0)
                {
                    lstSereServExt = GetSereServExt(lstsereServ.Select(o => o.ID).ToList());
                    if (lstSereServExt != null && lstSereServExt.Count > 0)
                    {
                        lstsarPrint = GetSarPrint(lstSereServExt);
                    }

                    foreach (var item in lstsereServ)
                    {
                        var ext = lstSereServExt.FirstOrDefault(x => x.SERE_SERV_ID == item.ID);
                        if (ext != null && !string.IsNullOrEmpty(ext.DESCRIPTION_SAR_PRINT_ID))
                        {
                            var prints = GetListPrintIdBySereServ(new List<HIS_SERE_SERV_EXT>() { ext });
                            var SarPrints = lstsarPrint.Where(o => prints.Contains(o.ID));
                            foreach (var sar in SarPrints)
                            {
                                SereServFileADO newSereServAdo = new SereServFileADO();
                                newSereServAdo.ID = item.ID;
                                newSereServAdo.SERVICE_ID = item.SERVICE_ID;
                                newSereServAdo.ServiceName = item.TDL_SERVICE_NAME;
                                newSereServAdo.TDL_EXECUTE_BRANCH_ID = item.TDL_EXECUTE_BRANCH_ID;
                                newSereServAdo.TDL_SERVICE_NAME = item.TDL_SERVICE_NAME;
                                newSereServAdo.TDL_INTRUCTION_TIME = item.TDL_INTRUCTION_TIME;
                                newSereServAdo.URL_NAME = sar.TITLE + "_" + Inventec.Common.DateTime.Convert.TimeNumberToDateString(newSereServAdo.TDL_INTRUCTION_TIME);
                                newSereServAdo.Key = string.Format("{0}_{1}_{2}", item.TDL_INTRUCTION_TIME, item.ID, item.TDL_SERVICE_NAME);
                                newSereServAdo.FileContent = sar.CONTENT;
                                newSereServAdo.IsChecked = true;
                                newSereServAdo.IsFss = false;
                                string pathLocal = GetPathDefault();
                                newSereServAdo.image = System.Drawing.Image.FromFile(pathLocal);
                                newSereServAdo.Content = Utility.TextLibHelper.BytesToString(sar.CONTENT);
                                if (!string.IsNullOrEmpty(newSereServAdo.Content))
                                    lstSereServFileADOs.Add(newSereServAdo);
                            }
                        }
                        else if(ext != null && !string.IsNullOrEmpty(ext.DESCRIPTION))
                        {
                            SereServFileADO newSereServAdo = new SereServFileADO();
                            newSereServAdo.ID = item.ID;
                            newSereServAdo.SERVICE_ID = item.SERVICE_ID;
                            newSereServAdo.ServiceName = item.TDL_SERVICE_NAME;
                            newSereServAdo.TDL_EXECUTE_BRANCH_ID = item.TDL_EXECUTE_BRANCH_ID;
                            newSereServAdo.TDL_SERVICE_NAME = item.TDL_SERVICE_NAME;
                            newSereServAdo.TDL_INTRUCTION_TIME = item.TDL_INTRUCTION_TIME;
                            newSereServAdo.URL_NAME = "Mô tả" + "_" + Inventec.Common.DateTime.Convert.TimeNumberToDateString(newSereServAdo.TDL_INTRUCTION_TIME);
                            newSereServAdo.Content = ext.DESCRIPTION;
                            newSereServAdo.Key = string.Format("{0}_{1}_{2}", item.TDL_INTRUCTION_TIME, item.ID, item.TDL_SERVICE_NAME);
                            string pathLocal = GetPathDefault();
                            newSereServAdo.image = System.Drawing.Image.FromFile(pathLocal);
                            newSereServAdo.IsChecked = true;
                            newSereServAdo.IsFss = false;
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
        private List<long> GetListPrintIdBySereServ(List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT> ext)
        {
            List<long> result = new List<long>();
            try
            {
                foreach (var item in ext)
                {
                    if (!String.IsNullOrEmpty(item.DESCRIPTION_SAR_PRINT_ID))
                    {
                        var arrIds = item.DESCRIPTION_SAR_PRINT_ID.Split(',', ';');
                        if (arrIds != null && arrIds.Length > 0)
                        {
                            foreach (var id in arrIds)
                            {
                                long printId = Inventec.Common.TypeConvert.Parse.ToInt64(id);
                                if (printId > 0)
                                {
                                    result.Add(printId);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private List<SAR.EFMODEL.DataModels.SAR_PRINT> GetSarPrint(List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT> sereServExts)
        {
            List<SAR.EFMODEL.DataModels.SAR_PRINT> result = null;
            try
            {
                List<long> printIds = GetListPrintIdBySereServ(sereServExts);
                if (printIds != null && printIds.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    SAR.Filter.SarPrintFilter filter = new SAR.Filter.SarPrintFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.IDs = printIds;
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>(ApiConsumer.SarRequestUriStore.SAR_PRINT_GET, ApiConsumer.ApiConsumers.SarConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT> GetSereServExt(List<long> sereServId)
        {
            List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServExtFilter filter = new MOS.Filter.HisSereServExtFilter();
                filter.SERE_SERV_IDs = sereServId;
                filter.TDL_TREATMENT_ID = this.TreatmentId;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private void FillDataSereServ(long TreatmentId, List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> ServiceReqs, List<SereServFileADO> lstSereServFileADOs)
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
        private void btnImageAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> FileContent = new List<string>();
                List<string> Base64Images = new List<string>();

                var lstData = CardControl.DataSource as List<SereServFileADO>;
                lstData = lstData.Where(o => o.IsChecked).ToList();
                if (lstData == null || lstData.Count == 0)
                    return;
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                List<long> branhid= GetBranchCode(lstData);
                var branchCode = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().FirstOrDefault(p => branhid.Contains(p.ID));

                var lstdataFss = lstData.Where(o => o.IsFss).ToList();
                if (lstdataFss != null && lstdataFss.Count > 0)
                {
                    foreach (var item in lstdataFss)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            var tmp = Inventec.Fss.Client.FileDownload.GetFile(item.URL);
                            tmp.Seek(0, SeekOrigin.Begin);
                            tmp.CopyTo(memoryStream);
                            Base64Images.Add(GetBase64Images(memoryStream, item));
                        }
                    }
                }

                var lstdataNotFss = lstData.Where(o => !o.IsFss).ToList();
                foreach (var item in lstdataNotFss)
                {
                    if (item.FileContent != null && item.FileContent.Count() > 0)
                    {
                        string text = Utility.TextLibHelper.BytesToString(item.FileContent);
                        if (!string.IsNullOrEmpty(text))
                            FileContent.Add(text);
                    }
                    else
                    {
                        string tempDescription = "";
                        if (item.Content.Trim().Contains("<br/>"))
                        {
                            tempDescription = item.Content.Replace("<br/>", "\r\n");
                            FileContent.Add(tempDescription);
                        }
                        else
                        {
                            FileContent.Add(item.Content);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(AppConfigKeys.AIConnectionInfo))
                {
                    var requestData = new
                    {
                        AppCode = "HIS",
                        FileContents = FileContent,
                        Base64Imagess = Base64Images,
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
                        Inventec.Desktop.Common.Message.WaitingManager.Hide();
                        if (resp == null || !resp.IsSuccessStatusCode)
                        {
                            int statusCode = resp.StatusCode.GetHashCode();
                            if (resp.Content != null)
                            {
                                string errorData = resp.Content.ReadAsStringAsync().Result;
                            }

                            throw new Exception(string.Format(" trả về thông tin lỗi. Mã lỗi: {0}", statusCode));
                        }

                        Inventec.Desktop.Common.Message.WaitingManager.Hide();
                        string responseData = resp.Content.ReadAsStringAsync().Result;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => responseData), responseData));
                        this.aiResponse = JsonConvert.DeserializeObject<AIResponse>(responseData);

                        if (this.DelSelectData != null && this.aiResponse != null && !string.IsNullOrEmpty(this.aiResponse.Results))
                        {
                            btnResultsAI.Enabled = true;
                            lblYKhoa.Text = this.aiResponse.Results;
                            lblYKhoa.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
                        }
                        else if (this.aiResponse != null && !string.IsNullOrEmpty(this.aiResponse.Results))
                        {
                            lblYKhoa.Text = this.aiResponse.Results;
                            lblYKhoa.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
                        }

                        if (this.aiResponse == null)
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
        private string GetBase64Images(MemoryStream stream, SereServFileADO ado)
        {
            try
            {
                byte[] bytes = stream.ToArray();
                return string.Format("data:{0};base64,{1}", ado.Extension == "pdf" ? "pdf" : ("image/" + ado.Extension), Convert.ToBase64String(bytes));
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

                var frmImg = new frmImage(this.currentDataClick);
                frmImg.ShowDialog();
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
        private void tileView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.IsForGroupRow)
                {
                    var service = e.DisplayText.Split('_').Last();
                    e.DisplayText = string.Format("{0}", service);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
