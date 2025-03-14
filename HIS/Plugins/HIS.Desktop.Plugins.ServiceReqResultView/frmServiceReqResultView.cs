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
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using EMR.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ProcessorBase.EmrBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceReqResultView
{
    public partial class frmServiceReqResultView : HIS.Desktop.Utility.FormBase
    {
        long? sereServId;
        Inventec.Desktop.Common.Modules.Module currentModule;
        internal V_HIS_SERE_SERV_4 sereServ;
        MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt;
        SAR.EFMODEL.DataModels.SAR_PRINT currentSarPrint;
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ currentServiceReq;
        Dictionary<string, object> dicParam;
        Dictionary<string, Image> dicImage;
        List<string> keyPrint = new List<string>() { "<#CONCLUDE_PRINT;>", "<#NOTE_PRINT;>", "<#DESCRIPTION_PRINT;>", "<#CURRENT_USERNAME_PRINT;>" };
        private string UserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();

        const string license_code = "OLLUE/Go5Omzy/We6ff6Gu12mbXI2a9bl7PP5+Cd26QFJO+etKbW+q183/YAGORbl/r2HfKi5vLOzbBxpbSzy653s+X1D5+t8PT26KF+xrLUE/Go5Omzy/We6ff6Gu12mbXK2a9bl7PP5+Cd26QFJO+etKbW+q183/YAGORbl/r2HfKi5vLOzbFppbSzy653s+X1D5+t8PT26KF+xrLUE/Go5Omzy/We6ff6Gu12mbbC2a9bl7PP5+Cd26QFJO+etKbW+q183/YAGORbl/r2HfKi5vLOzbFrpbSzy653s+X1D5+t8PT26KF+xrLUE/Go5Omzy/We6ff6Gu12mbbE2a9bl7PP5+Cd26QFJO+etKbW+q183/YAGORbl/r2HfKi5vLOzbFtpbSzy653s+X1D5+t8PT26KF+xrLUE/Go5Omzy/We6ff6Gu12mbbG2a9bl7PP5+Cd26QFJO+etKbW+q183/YAGORbl/r2HfKi5vLOzbFvpbSzy653s+X1D5+t8PT26KF+xrLUE/Go5Omzy/We6ff6Gu12mbbI2a9bl7PP5+Cd26QFJO+etKbW+q183/YAGORbl/r2HfKi5vLOzbFxpbSzy653s+X1D5+t8PT26KF+xrLUE/Go5Omzy/We6ff6Gu12mbbK2a9bl7PP5+Cd26QFJO+etKbW+q183/YAGORbl/r2HfKi5vLOzbJppbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbBwpbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbBxpbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbBypbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbFppbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbFqpbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbFrpbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbFspbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbFtpbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbFupbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbFvpbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbFwpbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbFxpbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbFypbSzy653s+X1D5+t8PT26KF+xrLoEOFbl/r2HfKi5vLOzbJppbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbBwpbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbBxpbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbBypbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbFppbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbFqpbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbFrpbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbFspbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbFtpbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbFupbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbFvpbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbFwpbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbFxpbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbFypbSzy653s+X1D5+t8PT26KF+xrLhD+Vbl/r2HfKi5vLOzbJppbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbBwpbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbBxpbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbBypbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbFppbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbFqpbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbFrpbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbFspbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbFtpbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbFupbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbFvpbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbFwpbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbFxpbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbFypbSzy653s+X1D5+t8PT26KF+xrLoG+Vbl/r2HfKi5vLOzbJppbSzy653s7PyF+uo7sLNGvGd3PbaGeWol+jyH+R2mbXA3K5pp7TCzZ+s7ObWI++i6ekE7PN2mbXA3K5ysL3KzZ+v3PYEFO6ntKbDzZ9otcAEFOan2PgGHeR38fbJ4diazf3eE9F6xbb/+MeAvf33Irx2s7MEFOan2PgGHeR3s7P9FOKe5ff26XXj7fQQ7azcws0X6Jzc8gQQyJ21tcTetnWm8PoO5Kfq6doPvXXY8P0a9nez5fUPn63w9PbooX7G";

        const string MPS000354 = "Mps000354";
        private HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeAlter;
        private ADO.PatientADO patient;
        string currentBussinessCode;
        internal const string EmrDocumentTypeCode = "HIS.Desktop.Plugins.ServiceExecute.EmrDocumentTypeCode";

        bool isSense = false;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.ServiceReqResultView";
        bool isContineuCheckbox = false;
        Task taskForm_Load = null;
        bool isLoadingForm = false;

        public frmServiceReqResultView()
        {
            InitializeComponent();
        }

        public frmServiceReqResultView(Inventec.Desktop.Common.Modules.Module currentModule, long sereServId)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.sereServId = sereServId;
                EO.Base.Runtime.EnableEOWP = true;
                EO.WebBrowser.Runtime.AddLicense(license_code);
                webView1.NewWindow += NewWindowBrowser;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmServiceReqResultView_Load(object sender, EventArgs e)
        {
            try
            {
                this.isLoadingForm = true;
                this.taskForm_Load = Task.Factory.StartNew(() =>
                {
                    while (this.isLoadingForm)
                    {
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                SetIconFrm();
                //luôn ẩn header và chỉ hiển thị tab his
                xtraTab.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
                xtraTab.SelectedTabPage = xtraTabHis;
                xtraTabControl_TabHIS.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
                xtraTabControl_TabHIS.SelectedTabPage = xtraTabPage_TabDocument;
                chkAutoOpenWeb.Checked = false;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                zoomFactor();
                LoadDataBySereServId();

                Task[] taskall = new Task[3];

                taskall[0] = Task.Factory.StartNew(() => { LoadTreatmentWithPaty(); });
                taskall[1] = Task.Factory.StartNew(() => { GetPatientById(); });
                taskall[2] = Task.Factory.StartNew(() => { KiemTraThongTinPhieuKetQuaDienTu(); });
                Task.WaitAll(taskall);

                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this.listEmrDocument", this.listEmrDocument));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this.isShowEmrDocument", this.isShowEmrDocument));
                if (this.isShowEmrDocument)
                {
                    ProcessJoinDocument(this.documentData);
                    xtraTabControl_TabHIS.SelectedTabPage = xtraTabPage_TabPdf;
                }
                ProcessDicParamForPrint();
                btnPrint.Focus();
                InitControlState();
                if (!isSense && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.PrintOption") == "1"
                    && this.isShowEmrDocument == false)
                {
                    PrintOption1(false);
                    this.Close();
                }
                if (sereServ != null && (sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT))
                {
                    GeneratePopupMenu();
                    this.lciDropDownPrint.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciPrint.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    this.lciDropDownPrint.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lciPrint.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                this.isLoadingForm = false;
            }
        }

        void SetIconFrm()
        {
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

        private void GeneratePopupMenu()
        {
            try
            {
                DevExpress.Utils.Menu.DXPopupMenu menu = new DevExpress.Utils.Menu.DXPopupMenu();
                menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Phiếu thủ thuật phẫu thuật", new EventHandler((u, v) => PrintProcess(PrintType.IN_PHIEU_PHAU_THUAT_THU_THUAT))));
                menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Phiếu in kết quả", new EventHandler(btnPrint_Click)));
                btnDropDownPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void zoomFactor()
        {
            try
            {
                float zoom = 0;
                if (txtDescription.Document.Sections[0].Page.Landscape)
                    zoom = (float)(txtDescription.Width - 400) / (txtDescription.Document.Sections[0].Page.Height / 3);
                else
                    zoom = (float)(txtDescription.Width - 400) / (txtDescription.Document.Sections[0].Page.Width / 3);
                txtDescription.ActiveView.ZoomFactor = zoom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBySereServId()
        {
            try
            {
                if (this.sereServId == null) throw new ArgumentNullException("sereServId is null");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisSereServView4Filter filter = new MOS.Filter.HisSereServView4Filter();
                filter.ID = this.sereServId;
                var rs = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).Get<List<V_HIS_SERE_SERV_4>>
                    (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GETVIEW_4, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (rs != null && rs.Count > 0)
                {
                    sereServ = rs[0];
                    GetServiceReq();
                    SereServClickRow(rs[0]);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this.sereServ", this.sereServ));
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool FormatValid(string format)
        {
            string allowableLetters = "\r";
            bool check = false;

            if (format.Contains(allowableLetters))
                check = true;
            else
            {
                check = false;
            }

            return check;
        }

        bool FormatValid_(string format)
        {
            string allowableLetters = "\n";

            bool check = false;
            if (format.Contains(allowableLetters))
                check = true;
            else
            {
                check = false;
            }
            return check;
        }

        private void SereServClickRow(V_HIS_SERE_SERV_4 sereServ)
        {
            try
            {
                if (sereServ != null)
                {
                    WaitingManager.Show();
                    this.sereServ = sereServ;
                    sereServExt = GetSereServExtBySereServId(sereServ.ID);
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServExt), sereServExt));
                    if (sereServExt != null && sereServExt.ID > 0)
                    {
                        ProcessLoadSereServExtDescriptionPrint(sereServExt);

                        if (sereServExt.CONCLUDE != "" && sereServExt.CONCLUDE != null)
                        {
                            string tempConclude = "";
                            if (sereServExt.CONCLUDE.Contains("<br/>") || FormatValid(sereServExt.CONCLUDE) || FormatValid_(sereServExt.CONCLUDE))
                            {
                                if (FormatValid(sereServExt.CONCLUDE))
                                {
                                    tempConclude = sereServExt.CONCLUDE.Replace("\r", "\r\n");
                                }
                                else if (FormatValid_(sereServExt.CONCLUDE))
                                {
                                    tempConclude = sereServExt.CONCLUDE.Replace("\n", "\r\n");
                                }
                                else
                                {
                                    tempConclude = sereServExt.CONCLUDE.Replace("<br/>", "\r\n");
                                }

                                txtConclude.Text = tempConclude;
                            }
                            else
                            {
                                txtConclude.Text = sereServExt.CONCLUDE;
                            }
                        }
                        if (sereServExt.NOTE != "" && sereServExt.NOTE != null)
                        {
                            string tempNote = "";
                            if (sereServExt.NOTE.Contains("<br/>") || FormatValid(sereServExt.NOTE) || FormatValid_(sereServExt.NOTE))
                            {

                                if (FormatValid(sereServExt.NOTE))
                                {
                                    tempNote = sereServExt.NOTE.Replace("\r", "\r\n");
                                }
                                else if (FormatValid_(sereServExt.NOTE))
                                {
                                    tempNote = sereServExt.NOTE.Replace("\n", "\r\n");
                                }
                                else
                                {
                                    tempNote = sereServExt.NOTE.Replace("<br/>", "\r\n");
                                }

                                txtNote.Text = tempNote;
                            }
                            else
                            {
                                txtNote.Text = sereServExt.NOTE;
                            }
                        }
                        if (sereServExt.BEGIN_TIME != null)
                        {
                            lblStartTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(sereServExt.BEGIN_TIME ?? 0);
                        }
                        if (sereServExt.END_TIME != null)
                        {
                            lblEndTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(sereServExt.END_TIME ?? 0);
                        }

                        lblNumFilm.Text = "" + (sereServExt.NUMBER_OF_FILM ?? 0);
                        var size = BackendDataWorker.Get<HIS_FILM_SIZE>().Where(s => s.ID == sereServExt.FILM_SIZE_ID).FirstOrDefault();
                        lblSizeFilm.Text = size != null ? size.FILM_SIZE_NAME : "";
                        txtDesciptions.Text = sereServExt.DESCRIPTION;
                        var login_names = sereServExt.SUBCLINICAL_RESULT_LOGINNAME;
                        var user_names = sereServExt.SUBCLINICAL_RESULT_USERNAME;
                        if (!string.IsNullOrEmpty(login_names) && !string.IsNullOrEmpty(user_names))
                        {
                            var loginName = login_names.Split(';').ToList();
                            var username = user_names.Split(';').ToList();
                            string ktv = "";
                            for (int i = 0; i < loginName.Count; i++)
                            {
                                ktv += "" + loginName[i] + "-" + username[i];
                                ktv += ";";
                            }
                            if (ktv.EndsWith(";"))
                            {
                                ktv = ktv.TrimEnd(';');
                            }
                            txtktv.Text = ktv;
                        }

                        if (currentServiceReq != null)
                        {
                            var login_extnames = currentServiceReq.EXECUTE_LOGINNAME;
                            var user_extnames = currentServiceReq.EXECUTE_USERNAME;
                            if (!string.IsNullOrEmpty(login_extnames) && !string.IsNullOrEmpty(user_extnames))
                            {
                                var loginNameExt = login_extnames.Split(';').ToList();
                                var usernameExt = user_extnames.Split(';').ToList();
                                string ptv = "";
                                for (int i = 0; i < loginNameExt.Count; i++)
                                {
                                    ptv += "" + loginNameExt[i] + "-" + usernameExt[i];
                                    ptv += ";";
                                }
                                if (ptv.EndsWith(";"))
                                {
                                    ptv = ptv.TrimEnd(';');
                                }
                                txtPtv.Text = ptv;
                            }
                        }
                    }
                    else
                    {
                        txtConclude.Text = "";
                        txtNote.Text = "";
                        txtDescription.Text = "";
                    }

                    string url = "";
                    try
                    {
                        if (PacsCFG.PACS_ADDRESS != null && PacsCFG.PACS_ADDRESS.Count > 0)
                        {
                            PacsAddress address = null;
                            List<PacsAddress> listValidAddress = new List<PacsAddress>();
                            foreach (var item in PacsCFG.PACS_ADDRESS)
                            {
                                if (!String.IsNullOrWhiteSpace(item.RoomCode))
                                {
                                    string[] listRoomCode = item.RoomCode.Split('|');
                                    if (listRoomCode != null && listRoomCode.Contains(sereServ.EXECUTE_ROOM_CODE))
                                    {
                                        listValidAddress.Add(item);
                                    }
                                }
                            }
                            address = listValidAddress.OrderByDescending(o => !String.IsNullOrWhiteSpace(o.CloudInfo))
                                                    .ThenByDescending(o => !String.IsNullOrWhiteSpace(o.Api))
                                                    .FirstOrDefault();
                            if (address != null && !String.IsNullOrWhiteSpace(address.CloudInfo))
                            {
                                xtraTab.ShowTabHeader = DevExpress.Utils.DefaultBoolean.True;
                                if (sereServ != null && sereServ.ID > 0)
                                {
                                    CreateThreadGetLinkResult(sereServ.ID, false);
                                }
                            }
                            else if (address != null && !String.IsNullOrWhiteSpace(address.Api))
                            {
                                if (!String.IsNullOrWhiteSpace(sereServExt.JSON_FORM_ID) && sereServExt.JSON_FORM_ID.StartsWith("http"))
                                {
                                    url = sereServExt.JSON_FORM_ID;
                                }
                                else
                                {
                                    HIS_PATIENT patient = GetPatientById(sereServ.TDL_PATIENT_ID);

                                    url = string.Format("http://{0}{1}", address.Address, address.Api);

                                    if (address.Api.Trim().StartsWith("http"))
                                    {
                                        url = address.Api;
                                    }

                                    string idChiDinh = sereServ.ID.ToString();
                                    string idBenhNhan = patient.PATIENT_CODE;
                                    string idDotVaoVien = sereServ.TDL_TREATMENT_CODE;
                                    string PACS_BASE_URI = ConfigSystems.URI_API_PACS;

                                    url = url.Replace("<#PACS_BASE_URI;>", PACS_BASE_URI);
                                    var urlSplit = address.Api.Split('=', '&');
                                    var keyUrl = urlSplit.Where(o => o.Contains(":")).ToList();
                                    foreach (var item in keyUrl)
                                    {
                                        if (item.Contains("idChiDinh"))
                                        {
                                            url = url.Replace(":idChiDinh", idChiDinh);
                                        }
                                        else if (item.Contains("idBenhNhan"))
                                        {
                                            url = url.Replace(":idBenhNhan", idBenhNhan);
                                        }
                                        else if (item.Contains("idDotVaoVien"))
                                        {
                                            url = url.Replace(":idDotVaoVien", idDotVaoVien);
                                        }
                                    }
                                }
                                isSense = true;
                            }
                            else if (address != null && String.IsNullOrWhiteSpace(address.Api))
                            {
                                XtraMessageBox.Show("Chưa cấu hình địa chỉ xem kết quả");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        isSense = false;
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }

                    if (isSense)
                    {
                        xtraTab.ShowTabHeader = DevExpress.Utils.DefaultBoolean.True;
                        txtUrl.Text = url;
                        webView1.Url = url;
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadGetLinkResult(long sereServId, bool isOpenWeb)
        {
            Thread threadGetLinkResult = new Thread(() => GetLinkResult(sereServId, isOpenWeb));
            try
            {
                threadGetLinkResult.Start();
            }
            catch (Exception ex)
            {
                threadGetLinkResult.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetLinkResult(long sereServId, bool isOpenWeb)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                string apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<string>("api/HisSereServExt/GetLinkResult", ApiConsumer.ApiConsumers.MosConsumer, sereServId, param);

                if (this.taskForm_Load != null)
                {
                    this.taskForm_Load.Wait();
                }
                if (!String.IsNullOrWhiteSpace(apiResult))
                {
                    success = true;
                    string url = string.Format("http://{0}", apiResult);
                    if (apiResult.Trim().StartsWith("http"))
                    {
                        url = apiResult;
                    }
                    Invoke(new Action(() =>
                    {
                        xtraTab.ShowTabHeader = DevExpress.Utils.DefaultBoolean.True;
                        txtUrl.Text = url;
                        if (chkAutoOpenWeb.Checked || isOpenWeb)
                        {
                            System.Diagnostics.Process.Start(url);
                        }
                        webView1.Url = url;
                    }));
                }
                else
                {
                    success = false;
                    Invoke(new Action(() =>
                    {
                        string msg = param != null ? param.GetMessage() : "";
                        if (String.IsNullOrWhiteSpace(msg))
                        {
                            MessageManager.Show("Lấy thông tin link xem kết quả thất bại");
                        }
                        else
                        {
                            MessageManager.Show(msg);
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NewWindowBrowser(object sender, EO.WebBrowser.NewWindowEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.TargetUrl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_PATIENT GetPatientById(long? patientId)
        {
            HIS_PATIENT result = new HIS_PATIENT();
            try
            {
                if (patientId.HasValue)
                {
                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisPatientFilter filter = new MOS.Filter.HisPatientFilter();
                    filter.ID = patientId;
                    var rs = new Inventec.Common.Adapter.BackendAdapter
                           (paramCommon).Get<List<HIS_PATIENT>>
                           (ApiConsumer.HisRequestUriStore.HIS_PATIENT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                    if (rs != null && rs.Count > 0)
                    {
                        result = rs.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                result = new HIS_PATIENT();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT GetSereServExtBySereServId(long sereServId)
        {
            MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServExtFilter filter = new MOS.Filter.HisSereServExtFilter();
                filter.SERE_SERV_ID = sereServId;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE> GetSereServFilesBySereServId(long sereServId)
        {
            List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServFileFilter filter = new MOS.Filter.HisSereServFileFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.SERE_SERV_ID = sereServId;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE>>("/api/HisSereServFile/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessLoadSereServExtDescriptionPrint(MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt)
        {
            try
            {
                if (sereServExt != null && sereServExt.ID > 0)
                {
                    currentSarPrint = GetListPrintByDescriptionPrint(sereServExt);
                    if (currentSarPrint != null && currentSarPrint.ID > 0)
                    {
                        txtDescription.RtfText = Utility.TextLibHelper.BytesToStringConverted(currentSarPrint.CONTENT);

                        btnPrint.Enabled = true;
                    }
                    else
                    {
                        if (sereServExt.DESCRIPTION != "" && sereServExt.DESCRIPTION != null)
                        {
                            string tempDescription = "";
                            if (sereServExt.DESCRIPTION.Trim().Contains("<br/>"))
                            {
                                tempDescription = sereServExt.DESCRIPTION.Replace("<br/>", "\r\n");
                                txtDescription.Text = tempDescription;
                            }
                            else
                            {
                                txtDescription.Text = sereServExt.DESCRIPTION;
                            }
                        }
                        else
                            txtDescription.Text = "";
                    }

                    zoomFactor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private SAR.EFMODEL.DataModels.SAR_PRINT GetListPrintByDescriptionPrint(MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt)
        {
            SAR.EFMODEL.DataModels.SAR_PRINT result = null;
            try
            {
                List<long> printIds = GetListPrintIdBySereServ(sereServExt);
                if (printIds != null && printIds.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    SAR.Filter.SarPrintFilter filter = new SAR.Filter.SarPrintFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.IDs = printIds;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>(ApiConsumer.SarRequestUriStore.SAR_PRINT_GET, ApiConsumer.ApiConsumers.SarConsumer, filter, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        result = apiResult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private List<long> GetListPrintIdBySereServ(MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT item)
        {
            List<long> result = new List<long>();
            try
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this.currentSarPrint", this.currentSarPrint));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this.sereServExt", this.sereServExt));
                if (this.isShowEmrDocument)
                {
                    this.pdfViewer1.Print();
                }
                else if (this.currentSarPrint == null && this.sereServExt != null && !String.IsNullOrEmpty(this.sereServExt.DESCRIPTION))
                {
                    PrintProcess(PrintType.IN_PHIEU_KET_QUA_CAN_LAM_SANG_TONG_HOP);
                }
                else if (txtDescription.Text == "")
                {
                    PrintProcess(PrintType.IN_PHIEU_KET_QUA);
                }
                else
                {
                    if (!btnPrint.Enabled) return;

                    //1: In sử dụng biểu mẫu. 2: In trực tiếp dữ liệu do người dùng nhập ở màn hình xử lý"
                    if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.PrintOption") == "1")
                    {
                        PrintOption1(false);
                    }
                    else
                    {
                        PrintOption2(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal enum PrintType
        {
            IN_PHIEU_KET_QUA,
            IN_PHIEU_KET_QUA_CAN_LAM_SANG_TONG_HOP,
            IN_PHIEU_PHAU_THUAT_THU_THUAT
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_PHIEU_KET_QUA:
                        richEditorMain.RunPrintTemplate(PrintTypeCode.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA__MPS000015, DelegateRunPrinterTest);
                        break;
                    case PrintType.IN_PHIEU_KET_QUA_CAN_LAM_SANG_TONG_HOP:
                        richEditorMain.RunPrintTemplate(PrintTypeCode.PRINT_TYPE_CODE__BIEUMAU__PHIEU_KET_QUA_CAN_LAM_SANG_TONG_HOP__MPS000471, DelegateRunPrinterTest);
                        break;
                    case PrintType.IN_PHIEU_PHAU_THUAT_THU_THUAT:
                        richEditorMain.RunPrintTemplate(PrintTypeCode.PRINT_TYPE_CODE__BIEUMAU__PHIEU_PHAU_THUAT_THU_THUAT__MPS000033, DelegateRunPrinterTest);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinterTest(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCode.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA__MPS000015:
                        Print_PhieuYCInKetQua_Mps000015(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCode.PRINT_TYPE_CODE__BIEUMAU__PHIEU_KET_QUA_CAN_LAM_SANG_TONG_HOP__MPS000471:
                        Print_PhieuKetQuaCLSTongHop_Mps000471(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCode.PRINT_TYPE_CODE__BIEUMAU__PHIEU_PHAU_THUAT_THU_THUAT__MPS000033:
                        Print_PhieuPhauThuatThuThuat_Mps000033(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void Print_PhieuYCInKetQua_Mps000015(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter();
                sereServFilter.ID = this.sereServId;
                List<HIS_SERE_SERV> _SereServs = new List<HIS_SERE_SERV>();
                _SereServs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV>>
                   (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, param);
                long _treatmentId = 0;
                long _serviceReqId = 0;
                List<MPS.Processor.Mps000015.PDO.Mps000015ADO> _Mps000015ADOs = new List<MPS.Processor.Mps000015.PDO.Mps000015ADO>();
                if (_SereServs != null && _SereServs.Count > 0)
                {
                    _treatmentId = _SereServs[0].TDL_TREATMENT_ID ?? 0;
                    _serviceReqId = _SereServs[0].SERVICE_REQ_ID ?? 0;

                    _Mps000015ADOs.AddRange((from r in _SereServs select new MPS.Processor.Mps000015.PDO.Mps000015ADO(r, txtNote.Text, txtConclude.Text)).ToList());

                }

                //Lấy thông tin thẻ BHYT
                MOS.Filter.HisPatientTypeAlterViewAppliedFilter hisPTAlterFilter = new HisPatientTypeAlterViewAppliedFilter();
                hisPTAlterFilter.TreatmentId = _treatmentId;
                hisPTAlterFilter.InstructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMddHHmm") + "00");
                var PatyAlterBhyt = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, hisPTAlterFilter, param);

                //Loai Patient_type_name
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.ID = _serviceReqId;
                var ServiceReqPrint = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();

                MPS.Processor.Mps000015.PDO.SingleKeys _SingleKeys = new MPS.Processor.Mps000015.PDO.SingleKeys();

                //Mức hưởng BHYT
                decimal ratio_text = 0;
                if (PatyAlterBhyt != null)
                {
                    string levelCode = PatyAlterBhyt.LEVEL_CODE;
                    ratio_text = GetDefaultHeinRatioForView(PatyAlterBhyt.HEIN_CARD_NUMBER, PatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, levelCode, PatyAlterBhyt.RIGHT_ROUTE_CODE);
                }

                _SingleKeys.Ratio = ratio_text;

                MOS.Filter.HisTreatmentBedRoomFilter bedRoomFilter = new HisTreatmentBedRoomFilter();
                bedRoomFilter.TREATMENT_ID = _treatmentId;
                var _TreatmentBedRoom = new BackendAdapter(param).Get<List<HIS_TREATMENT_BED_ROOM>>("/api/HisTreatmentBedRoom/Get", ApiConsumers.MosConsumer, bedRoomFilter, param).FirstOrDefault();

                if (_TreatmentBedRoom != null && _TreatmentBedRoom.BED_ID > 0)
                {
                    var bedName = BackendDataWorker.Get<HIS_BED>().FirstOrDefault(p => p.ID == _TreatmentBedRoom.BED_ID);
                    _SingleKeys.BED_NAME = bedName != null ? bedName.BED_NAME : null;
                }
                if (ServiceReqPrint != null)
                {
                    var depart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.ID == ServiceReqPrint.REQUEST_DEPARTMENT_ID);
                    _SingleKeys.REQUEST_DEPARTMENT_NAME = depart != null ? depart.DEPARTMENT_NAME : null;

                    var roomName = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == ServiceReqPrint.REQUEST_ROOM_ID);
                    _SingleKeys.REQUEST_ROOM_NAME = roomName != null ? roomName.ROOM_NAME : null;
                }
                if (_SereServs != null && _SereServs[0].TDL_SERVICE_TYPE_ID > 0)
                {
                    var typeName = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == _SereServs[0].TDL_SERVICE_TYPE_ID);
                    _SingleKeys.SERVICE_TYPE_NAME = typeName != null ? typeName.SERVICE_TYPE_NAME : null;
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((ServiceReqPrint != null ? ServiceReqPrint.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                MPS.Processor.Mps000015.PDO.Mps000015PDO mps000015RDO = new MPS.Processor.Mps000015.PDO.Mps000015PDO(
                    PatyAlterBhyt,
                    ServiceReqPrint,
                    _Mps000015ADOs,
                    _SingleKeys
                    );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000015RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000015RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void Print_PhieuPhauThuatThuThuat_Mps000033(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                // Lấy thông tin bệnh nhân

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = sereServ.TDL_PATIENT_ID;
                V_HIS_PATIENT patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                MPS.Processor.Mps000033.PDO.PatientADO currentPatient = new MPS.Processor.Mps000033.PDO.PatientADO(patient);
                //Lấy thông tin chuyển khoa
                var departmentTran = PrintGlobalStore.getDepartmentTran(sereServ.TDL_TREATMENT_ID ?? 0);
                MOS.Filter.HisTreatmentViewFilter treatmentViewFilter = new HisTreatmentViewFilter();
                treatmentViewFilter.ID = sereServ.TDL_TREATMENT_ID ?? 0;
                V_HIS_TREATMENT vhisTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("/api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatmentViewFilter, param).FirstOrDefault();

                MOS.Filter.HisServiceReqViewFilter filterServiceReq = new MOS.Filter.HisServiceReqViewFilter();
                filterServiceReq.ID = sereServ.SERVICE_REQ_ID ?? 0;
                var serviceReq = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, filterServiceReq, param).FirstOrDefault();
                //Khoa hien tai
                if (serviceReq != null)
                {
                    HIS_DEPARTMENT department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_DEPARTMENT_ID);
                    if (department != null)
                    {
                        serviceReq.REQUEST_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                    }

                    V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                    if (room != null)
                    {
                        serviceReq.REQUEST_ROOM_NAME = room.ROOM_NAME;
                    }
                }
                HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
                sereServFilter.ID = sereServId;
                V_HIS_SERE_SERV_5 sereServ5 = new BackendAdapter(param)
                    .Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, param).FirstOrDefault();
                List<V_HIS_EKIP_USER> vEkipUsers = new List<V_HIS_EKIP_USER>();
                if (sereServ.EKIP_ID != null)
                {
                    HisEkipUserViewFilter ekipUserFilter = new HisEkipUserViewFilter();
                    ekipUserFilter.EKIP_ID = sereServ.EKIP_ID;
                    vEkipUsers = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER>>("api/HisEkipUser/GetView", ApiConsumers.MosConsumer, ekipUserFilter, param);
                }
                List<HIS_SERE_SERV_EXT> listSereServExt = new List<HIS_SERE_SERV_EXT>();
                MOS.Filter.HisSereServExtFilter filterSereServExt = new MOS.Filter.HisSereServExtFilter();
                filterSereServExt.SERE_SERV_ID = this.sereServId;
                var SereServExt = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filterSereServExt, param).FirstOrDefault();
                object dfdf = Activator.CreateInstance(vEkipUsers.GetType());

                MOS.Filter.HisSereServPtttViewFilter filter = new MOS.Filter.HisSereServPtttViewFilter();
                filter.SERE_SERV_ID = sereServ.ID;
                var sereServPttts = new BackendAdapter(param)
                   .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                V_HIS_BED_LOG currentBedLog = new V_HIS_BED_LOG();
                V_HIS_BED_LOG lastBedLog = new V_HIS_BED_LOG();
                MOS.Filter.HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                bedLogFilter.TREATMENT_ID = sereServ.TDL_TREATMENT_ID;
                var bedLogList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogFilter, null);
                if (bedLogList != null && bedLogList.Count > 0 && SereServExt != null)
                {
                    currentBedLog = bedLogList.Where(o => o.START_TIME <= SereServExt.BEGIN_TIME
                        && (!o.FINISH_TIME.HasValue || o.FINISH_TIME.Value > SereServExt.BEGIN_TIME))
                        .OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    lastBedLog = bedLogList.Where(o => o.START_TIME <= SereServExt.BEGIN_TIME)
                        .OrderByDescending(o => o.START_TIME).FirstOrDefault();
                }

                HisSereServFileFilter ssffilter = new MOS.Filter.HisSereServFileFilter();
                ssffilter.SERE_SERV_ID = sereServ.ID;
                var sereServFiles = new BackendAdapter(param).Get<List<HIS_SERE_SERV_FILE>>("api/HisSereServFile/Get", ApiConsumers.MosConsumer, ssffilter, param);

                HisSesePtttMethodViewFilter filterSese = new HisSesePtttMethodViewFilter();
                filterSese.TDL_SERE_SERV_ID = sereServ.ID;
                var sesePtttMethod = new BackendAdapter(param)
                  .Get<List<MOS.EFMODEL.DataModels.V_HIS_SESE_PTTT_METHOD>>("api/HisSesePtttMethod/GetView", ApiConsumers.MosConsumer, filterSese, param);

                List<string> listManner = new List<string>();
                HIS_SKIN_SURGERY_DESC skinDesc = null;
                if (sereServPttts != null)
                {
                    listManner.Add(sereServPttts.MANNER);
                    HisSkinSurgeryDescFilter filterSkinDesc = new HisSkinSurgeryDescFilter();
                    filterSkinDesc.ID = sereServPttts.SKIN_SURGERY_DESC_ID;
                    skinDesc = new BackendAdapter(param)
                  .Get<List<MOS.EFMODEL.DataModels.HIS_SKIN_SURGERY_DESC>>("api/HisSkinSurgeryDesc/Get", ApiConsumers.MosConsumer, filterSkinDesc, param).FirstOrDefault();
                }

                WaitingManager.Hide();
                MPS.Processor.Mps000033.PDO.Mps000033PDO rdo = new MPS.Processor.Mps000033.PDO.Mps000033PDO(currentPatient, departmentTran, serviceReq, sereServ5, SereServExt, sereServPttts, vhisTreatment, vEkipUsers, null, currentBedLog, lastBedLog, listManner, skinDesc, sereServFiles, sesePtttMethod);
                MPS.ProcessorBase.Core.PrintData PrintData;
                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
        private void Print_PhieuKetQuaCLSTongHop_Mps000471(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.sereServId == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Print_PhieuKetQuaCLSTongHop_Mps000471(): this.sereServId == null");
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                List<V_HIS_SERE_SERV> listSereServ = new List<V_HIS_SERE_SERV>();
                MOS.Filter.HisSereServViewFilter filterSereServ = new MOS.Filter.HisSereServViewFilter();
                filterSereServ.ID = this.sereServId;
                listSereServ = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, filterSereServ, param);

                List<V_HIS_SERVICE_REQ> listServiceSeq = new List<V_HIS_SERVICE_REQ>();
                V_HIS_SERVICE_REQ ServiceReqPrint = new V_HIS_SERVICE_REQ();
                if (listSereServ != null && listSereServ.Count == 1)
                {
                    MOS.Filter.HisServiceReqViewFilter filterServiceReq = new MOS.Filter.HisServiceReqViewFilter();
                    filterServiceReq.ID = listSereServ[0].SERVICE_REQ_ID ?? 0;
                    listServiceSeq = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, filterServiceReq, param);
                    if (listServiceSeq != null && listServiceSeq.Count == 1)
                        ServiceReqPrint = listServiceSeq[0];
                }

                List<HIS_SERE_SERV_EXT> listSereServExt = new List<HIS_SERE_SERV_EXT>();
                MOS.Filter.HisSereServExtFilter filterSereServExt = new MOS.Filter.HisSereServExtFilter();
                filterSereServExt.SERE_SERV_ID = this.sereServId;
                listSereServExt = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filterSereServExt, param);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((ServiceReqPrint != null ? ServiceReqPrint.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listServiceSeq", listServiceSeq));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listSereServ", listSereServ));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listSereServExt", listSereServExt));
                MPS.Processor.Mps000471.PDO.Mps000471PDO mps000471RDO = new MPS.Processor.Mps000471.PDO.Mps000471PDO(
                    listServiceSeq,
                    listSereServ,
                    listSereServExt
                    );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000471RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000471RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public decimal GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            decimal result = 0;
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void barButtonItem_Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnEmr_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnEmr.Enabled)
                    return;

                SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                Library.EmrGenerate.EmrGenerateProcessor generateProcessor = new Library.EmrGenerate.EmrGenerateProcessor();
                InputADO inputADO = generateProcessor.GenerateInputADOWithPrintTypeCode(currentServiceReq.TDL_TREATMENT_CODE, "", true, currentModule.RoomId);
                HIS_SERE_SERV_TEMP temp = null;
                if (currentSarPrint != null)
                {
                    inputADO.DocumentTypeCode = currentSarPrint.EMR_DOCUMENT_TYPE_CODE;
                    inputADO.DocumentGroupCode = currentSarPrint.EMR_DOCUMENT_GROUP_CODE;
                    if (!String.IsNullOrWhiteSpace(currentSarPrint.EMR_BUSINESS_CODES))
                    {
                        var codes = currentSarPrint.EMR_BUSINESS_CODES.Split(';').ToList();
                        if (codes.Count() == 1)
                        {
                            inputADO.BusinessCode = codes[0];
                        }
                        else
                        {
                            var listBussiness = BackendDataWorker.Get<EMR.EFMODEL.DataModels.EMR_BUSINESS>().Where(o => codes.Contains(o.BUSINESS_CODE)).ToList();
                            MPS.ProcessorBase.EmrBusiness.frmChooseBusiness frmChooseBusiness = new MPS.ProcessorBase.EmrBusiness.frmChooseBusiness(ChooseBusinessClick, listBussiness);
                            frmChooseBusiness.ShowDialog();

                            inputADO.BusinessCode = currentBussinessCode;
                            currentSarPrint.EMR_BUSINESS_CODES = currentBussinessCode;
                            CreateThreadUpdateBusinessCode(currentSarPrint);
                        }
                    }

                    inputADO.HisCode = string.Format("SERVICE_REQ_CODE:{0} SER_SERV_ID:{1}", this.sereServ.TDL_SERVICE_REQ_CODE, sereServExt.SERE_SERV_ID);
                    inputADO.DocumentName = (String.Format("{0} (Mã điều trị: {1})", currentSarPrint.TITLE, currentServiceReq.TDL_TREATMENT_CODE));

                    if (!string.IsNullOrWhiteSpace(currentSarPrint.ADDITIONAL_INFO) && currentSarPrint.ADDITIONAL_INFO.Contains("SERE_SERV_TEMP_CODE"))
                    {
                        string TEMP_CODE = currentSarPrint.ADDITIONAL_INFO.Replace("SERE_SERV_TEMP_CODE:", "").Trim();
                        temp = BackendDataWorker.Get<HIS_SERE_SERV_TEMP>().FirstOrDefault(o => o.SERE_SERV_TEMP_CODE == TEMP_CODE);
                    }
                }

                if (temp != null)
                {
                    if (String.IsNullOrWhiteSpace(inputADO.DocumentTypeCode))
                    {
                        inputADO.DocumentTypeCode = temp.EMR_DOCUMENT_TYPE_CODE;
                    }

                    if (String.IsNullOrWhiteSpace(inputADO.DocumentGroupCode))
                    {
                        inputADO.DocumentGroupCode = temp.EMR_DOCUMENT_GROUP_CODE;
                    }
                }

                if (String.IsNullOrWhiteSpace(inputADO.DocumentTypeCode))
                {
                    inputADO.DocumentTypeCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(EmrDocumentTypeCode);
                }

                if (!String.IsNullOrWhiteSpace(inputADO.DocumentName) && sereServ != null)
                {
                    inputADO.DocumentName = (String.Format("{0} (Mã điều trị: {1})", sereServ.TDL_SERVICE_NAME, currentServiceReq.TDL_TREATMENT_CODE));//Tên văn bản cần tạo
                }

                if (String.IsNullOrWhiteSpace(inputADO.HisCode))
                {
                    inputADO.HisCode = string.Format("SERVICE_REQ_CODE:{0} SER_SERV_ID:{1}", this.sereServ.TDL_SERVICE_REQ_CODE, this.sereServ.ID);
                }
                inputADO.IsSign = true;

                Inventec.Common.Logging.LogSystem.Debug("BtnEmr_Click.isSense");
                CommonParam paramCommon = new CommonParam();
                var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
                emrFilter.TREATMENT_CODE__EXACT = sereServ.TDL_TREATMENT_CODE;
                emrFilter.HIS_CODE__EXACT = sereServ.ID.ToString();
                emrFilter.IS_DELETE = false;
                var documents = new BackendAdapter(paramCommon).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, emrFilter, paramCommon);
                if (documents == null || documents.Count <= 0)
                {

                    emrFilter.HIS_CODE__EXACT = inputADO.HisCode;
                    documents = new BackendAdapter(paramCommon).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, emrFilter, paramCommon);
                }

                if (documents != null && documents.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("BtnEmr_Click.isSense Has document: " + documents.Count);
                    paramCommon = new CommonParam();

                    var listData = GetEmrDocumentFile(documents.Select(o => o.ID).ToList(), false, null, null, inputADO.HisCode, ref paramCommon);
                    if (listData != null && listData.Count > 0)
                    {
                        inputADO.IsExport = inputADO.IsPrint = inputADO.IsReject = inputADO.IsSave = inputADO.IsSign = false;

                        string temFile = System.IO.Path.GetTempFileName();
                        temFile = temFile.Replace(".tmp", ".pdf");

                        List<string> joinStreams = new List<string>();

                        List<MemoryStream> documentData = new List<MemoryStream>();
                        foreach (var item in listData)
                        {
                            if (item.Extension.ToLower().Equals("pdf"))
                            {
                                string pdfAddFile = Utils.GenerateTempFileWithin();
                                Utils.ByteToFile(Utils.StreamToByte(new MemoryStream(Convert.FromBase64String(item.Base64Data))), pdfAddFile);
                                joinStreams.Add(pdfAddFile);
                            }
                        }

                        Stream currentStream = File.Open(temFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                        var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                        var pages = new List<int>();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu joinStreams: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => joinStreams), joinStreams));

                        foreach (var file in joinStreams)
                        {
                            iTextSharp.text.pdf.PdfReader pdfReader = null;
                            pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                            pages = new List<int>();
                            for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                            {
                                pages.Add(i);
                            }
                            pdfReader.SelectPages(pages);
                            pdfConcat.AddPages(pdfReader);
                            pdfReader.Close();
                        }
                        try
                        {
                            pdfConcat.Close();
                        }
                        catch { }

                        foreach (var file in joinStreams)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch { }
                        }

                        libraryProcessor.ShowPopup(temFile, inputADO);//truyền vào đường dẫn file cần ký, các định dạng hỗ trợ là: pdf,doc,docx,xls,xlsx,rdlc,...
                        System.IO.File.Delete(temFile);
                    }
                    else
                    {
                        MessageManager.Show(this.ParentForm, paramCommon, false);
                    }
                }
                else if (txtDescription.Text != "")
                {
                    var currentSsPdf = GetSereServFilesBySereServId(sereServ.ID).Where(o => o.FILE_TYPE == 2).ToList();
                    string OutputFile = Utils.GenerateTempFileWithin();

                    List<string> joinStreams = new List<string>();
                    DevExpress.XtraRichEdit.RichEditControl printDocument = ProcessDocumentBeforePrint(txtDescription);
                    if (printDocument == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("printDocument is null");
                        return;
                    }
                    ProcessColumnMaping(temp, ref inputADO);
                    String temFile = System.IO.Path.GetTempFileName();
                    temFile = temFile.Replace(".tmp", ".pdf");
                    printDocument.ExportToPdf(temFile);
                    joinStreams.Add(temFile);


                    if (currentSsPdf != null && currentSsPdf.Count > 0)
                    {
                        var dataPdf = currentSsPdf.OrderBy(o => o.NUM_ORDER).ToList();
                        foreach (var item in dataPdf)
                        {
                            var stream = Inventec.Fss.Client.FileDownload.GetFile(item.URL);

                            if (stream != null && stream.Length > 0)
                            {
                                stream.Position = 0;
                                string pdfFile = Utils.GenerateTempFileWithin();
                                Utils.ByteToFile(Utils.StreamToByte(stream), pdfFile);
                                joinStreams.Add(pdfFile);
                            }
                        }
                    }
                    Stream currentStream = File.Open(OutputFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                    var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                    var pages = new List<int>();

                    foreach (var file in joinStreams)
                    {
                        iTextSharp.text.pdf.PdfReader pdfReader = null;
                        pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                        pages = new List<int>();
                        for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                        {
                            pages.Add(i);
                        }
                        pdfReader.SelectPages(pages);
                        pdfConcat.AddPages(pdfReader);
                        pdfReader.Close();
                    }
                    try
                    {
                        pdfConcat.Close();
                    }
                    catch { }

                    foreach (var file in joinStreams)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch { }
                    }

                    libraryProcessor.ShowPopup(OutputFile, inputADO);//truyền vào đường dẫn file cần ký, các định dạng hỗ trợ là: pdf,doc,docx,xls,xlsx,rdlc,...

                    System.IO.File.Delete(OutputFile);
                }
                else
                {
                    XtraMessageBox.Show("Không tìm thấy văn bản ký điện tử");
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => paramCommon), paramCommon));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessColumnMaping(HIS_SERE_SERV_TEMP sereServTemp, ref Inventec.Common.SignLibrary.ADO.InputADO emrInputADO)
        {
            try
            {
                if (sereServTemp != null && sereServTemp.ID > 0)
                {
                    if (!String.IsNullOrEmpty(sereServTemp.EMR_COLUMN_MAPPING))
                    {
                        try
                        {
                            List<HIS.Desktop.Plugins.ServiceReqResultView.ADO.EmrColumnMappingADO> emrColumnMappingADOs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HIS.Desktop.Plugins.ServiceReqResultView.ADO.EmrColumnMappingADO>>(sereServTemp.EMR_COLUMN_MAPPING);
                            if (emrColumnMappingADOs != null && emrColumnMappingADOs.Count > 0)
                            {
                                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrColumnMappingADOs), emrColumnMappingADOs));
                                System.Reflection.PropertyInfo[] piInputADOs = typeof(Inventec.Common.SignLibrary.ADO.InputADO).GetProperties();
                                foreach (var emrColumn in emrColumnMappingADOs)
                                {
                                    if (dicParam.ContainsKey(emrColumn.Key))
                                    {
                                        object value = dicParam[emrColumn.Key];
                                        string valueDataType = "";
                                        var pi = piInputADOs.FirstOrDefault(t => t.Name == emrColumn.EmrColumn);
                                        if (pi != null && value != null)
                                        {
                                            try
                                            {
                                                if (pi.PropertyType.Equals(typeof(short)) || pi.PropertyType.Equals(typeof(short?)))
                                                {
                                                    pi.SetValue(emrInputADO, (short)(value));
                                                    valueDataType = "short";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(decimal)) || pi.PropertyType.Equals(typeof(decimal?)))
                                                {
                                                    pi.SetValue(emrInputADO, (decimal)(value));
                                                    valueDataType = "decimal";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(long)) || pi.PropertyType.Equals(typeof(long?)))
                                                {
                                                    DateTime? dt = null;
                                                    if (value.ToString().Contains("/") || value.ToString().Contains("-"))
                                                    {
                                                        string strDate = "", hours = "";
                                                        if (value.ToString().Contains(":"))
                                                        {
                                                            var arrDate = value.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                                            if (arrDate != null && arrDate.Count() > 1)
                                                            {
                                                                foreach (var itemD in arrDate)
                                                                {
                                                                    if (itemD.Contains(":"))
                                                                    {
                                                                        hours = itemD.Trim().Replace("h", ":");
                                                                        if (hours.Length == 5)
                                                                        {
                                                                            hours += ":00";
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var arrDate1 = itemD.Trim().Split(new string[] { "/", "-" }, StringSplitOptions.RemoveEmptyEntries);
                                                                        if (arrDate1 != null && arrDate1.Count() > 0)
                                                                        {
                                                                            strDate = String.Format("{0}-{1}-{2}", arrDate1[2], arrDate1[1], arrDate1[0]);
                                                                        }
                                                                    }
                                                                }
                                                                dt = DateTime.Parse(String.Format("{0} {1}", strDate, hours));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var arrDate1 = value.ToString().Split(new string[] { "/", "-" }, StringSplitOptions.RemoveEmptyEntries);
                                                            if (arrDate1 != null && arrDate1.Count() > 0)
                                                            {
                                                                strDate = String.Format("{0}-{1}-{2}", arrDate1[2], arrDate1[1], arrDate1[0]);
                                                                hours = "00:00:00";
                                                                dt = DateTime.Parse(String.Format("{0} {1}", strDate, hours));
                                                            }
                                                        }
                                                        pi.SetValue(emrInputADO, Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dt));
                                                    }
                                                    else
                                                        pi.SetValue(emrInputADO, (long)(value));

                                                    valueDataType = "long";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(int)) || pi.PropertyType.Equals(typeof(int?)))
                                                {
                                                    pi.SetValue(emrInputADO, (int)(value));
                                                    valueDataType = "int";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(double)) || pi.PropertyType.Equals(typeof(double?)))
                                                {
                                                    pi.SetValue(emrInputADO, (double)(value));
                                                    valueDataType = "double";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(float)) || pi.PropertyType.Equals(typeof(float?)))
                                                {
                                                    pi.SetValue(emrInputADO, (float)(value));
                                                    valueDataType = "float";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(bool)) || pi.PropertyType.Equals(typeof(bool?)))
                                                {
                                                    pi.SetValue(emrInputADO, (bool)(value));
                                                    valueDataType = "bool";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(DateTime)) || pi.PropertyType.Equals(typeof(DateTime?)))
                                                {
                                                    DateTime? dt = null;
                                                    if (value.ToString().Contains("/") || value.ToString().Contains("-"))
                                                    {
                                                        string strDate = "", hours = "";
                                                        if (value.ToString().Contains(":"))
                                                        {
                                                            var arrDate = value.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                                            if (arrDate != null && arrDate.Count() > 1)
                                                            {
                                                                foreach (var itemD in arrDate)
                                                                {
                                                                    if (itemD.Contains(":"))
                                                                    {
                                                                        hours = itemD.Trim().Replace("h", ":");
                                                                        if (hours.Length == 5)
                                                                        {
                                                                            hours += ":00";
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var arrDate1 = itemD.Trim().Split(new string[] { "/", "-" }, StringSplitOptions.RemoveEmptyEntries);
                                                                        if (arrDate1 != null && arrDate1.Count() > 0)
                                                                        {
                                                                            strDate = String.Format("{0}-{1}-{2}", arrDate1[2], arrDate1[1], arrDate1[0]);
                                                                        }
                                                                    }
                                                                }
                                                                dt = DateTime.Parse(String.Format("{0} {1}", strDate, hours));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var arrDate1 = value.ToString().Split(new string[] { "/", "-" }, StringSplitOptions.RemoveEmptyEntries);
                                                            if (arrDate1 != null && arrDate1.Count() > 0)
                                                            {
                                                                strDate = String.Format("{0}-{1}-{2}", arrDate1[2], arrDate1[1], arrDate1[0]);
                                                                hours = "00:00:00";
                                                                dt = DateTime.Parse(String.Format("{0} {1}", strDate, hours));
                                                            }
                                                        }
                                                    }
                                                    else if (value is long || value is long?)
                                                        dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)value);
                                                    else if (value is DateTime || value is DateTime?)
                                                        dt = (DateTime?)value;
                                                    pi.SetValue(emrInputADO, dt);
                                                    valueDataType = "DateTime";
                                                }
                                                else
                                                {
                                                    pi.SetValue(emrInputADO, value);
                                                    valueDataType = "string";
                                                }
                                                object newValue = pi.GetValue(emrInputADO);
                                            }
                                            catch (Exception ex1)
                                            {
                                                Inventec.Common.Logging.LogSystem.Warn(ex1);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex2)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private List<EmrDocumentFileSDO> GetEmrDocumentFile(List<long> docIds, bool? IsMerge, bool? IsShowPatientSign, bool? IsShowWatermark, string hisCode, ref CommonParam paramCommon)
        {
            EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
            var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
            emrFilter.IDs = docIds;
            sdo.EmrDocumentViewFilter = emrFilter;
            sdo.HisCode = hisCode;
            sdo.IsMerge = IsMerge;
            sdo.IsShowPatientSign = IsShowPatientSign;
            sdo.IsShowWatermark = IsShowWatermark;
            sdo.IsView = true;
            var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
            sdo.RoomCode = room != null ? room.ROOM_CODE : null;
            sdo.DepartmentCode = room != null ? room.DEPARTMENT_CODE : null;
            return new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, paramCommon);
        }

        private void CreateThreadUpdateBusinessCode(SAR.EFMODEL.DataModels.SAR_PRINT data)
        {
            Thread update = new Thread(UpdateBusinessCode);
            try
            {
                update.Start(data);
            }
            catch (Exception ex)
            {
                update.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateBusinessCode(object obj)
        {
            try
            {
                if (obj != null && obj is SAR.EFMODEL.DataModels.SAR_PRINT)
                {
                    var data = (SAR.EFMODEL.DataModels.SAR_PRINT)obj;
                    CommonParam param = new CommonParam();
                    this.currentSarPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT>(ApiConsumer.SarRequestUriStore.SAR_PRINT_UPDATE, ApiConsumer.ApiConsumers.SarConsumer, data, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChooseBusinessClick(EMR.EFMODEL.DataModels.EMR_BUSINESS dataBusiness)
        {
            try
            {
                if (dataBusiness != null)
                {
                    this.currentBussinessCode = dataBusiness != null ? dataBusiness.BUSINESS_CODE : "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenSignConfig(EMR.TDO.DocumentTDO obj)
        {
            try
            {
                if (obj != null)
                {
                    EMR.Filter.EmrDocumentFilter filter = new EMR.Filter.EmrDocumentFilter();
                    filter.DOCUMENT_CODE__EXACT = obj.DocumentCode;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT>>(EMR.URI.EmrDocument.GET, ApiConsumer.ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, null);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        List<object> _listObj = new List<object>();
                        _listObj.Add(apiResult.Max(o => o.ID));//truyền vào id lớn nhất;

                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("EMR.Desktop.Plugins.EmrSign", currentModule.RoomId, currentModule.RoomTypeId, _listObj);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private RichEditControl ProcessDocumentBeforePrint(RichEditControl document)
        {
            RichEditControl result = null;
            try
            {
                if (document != null)
                {
                    result = new RichEditControl();
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.ID = sereServ.SERVICE_REQ_ID;
                    var lstServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    long? finishTime = null;
                    if (lstServiceReq != null && lstServiceReq.Count > 0)
                    {
                        finishTime = lstServiceReq.FirstOrDefault().FINISH_TIME;
                    }

                    result.RtfText = document.RtfText;
                    var tgkt = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.ThoiGianKetThuc");
                    string PrintTimeOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.PrintTimeOption");
                    string printTimeStr = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    if (PrintTimeOption == "2")
                    {
                        var timeResult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<ACS.SDO.TimerSDO>("api/Timer/Sync", ApiConsumer.ApiConsumers.AcsConsumer, null, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        if (timeResult != null && timeResult.DateNow > DateTime.MinValue)
                        {
                            printTimeStr = timeResult.DateNow.ToString("dd/MM/yyyy HH:mm:ss");
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(tgkt))
                    {
                        foreach (var section in result.Document.Sections)
                        {
                            if (PrintTimeOption == "1" || PrintTimeOption == "2")
                            {
                                section.Margins.HeaderOffset = 50;
                                section.Margins.FooterOffset = 50;
                                var myHeader = section.BeginUpdateHeader(DevExpress.XtraRichEdit.API.Native.HeaderFooterType.Odd);
                                //xóa header nếu có dữ liệu
                                myHeader.Delete(myHeader.Range);

                                myHeader.InsertText(myHeader.CreatePosition(0),
                                    String.Format(Inventec.Common.Resource.Get.Value("NgayIn",
                                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), printTimeStr));
                                myHeader.Fields.Update();
                                section.EndUpdateHeader(myHeader);
                            }

                            string finishTimeStr = "";
                            if (finishTime.HasValue)
                            {
                                finishTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(finishTime.Value);
                            }

                            var rangeSeperators = result.Document.FindAll(tgkt, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                            if (rangeSeperators != null && rangeSeperators.Length > 0)
                            {
                                for (int i = 0; i < rangeSeperators.Length; i++)
                                    result.Document.Replace(rangeSeperators[i], finishTimeStr);
                            }
                        }
                    }

                    //key hiển thị màu trắng khi in sẽ thay key
                    if (sereServExt != null)
                    {
                        //đổi về màu đen để hiển thị.
                        foreach (var key in keyPrint)
                        {
                            var rangeSeperators = result.Document.FindAll(key, SearchOptions.CaseSensitive);
                            foreach (var rang in rangeSeperators)
                            {
                                CharacterProperties cp = result.Document.BeginUpdateCharacters(rang);
                                cp.ForeColor = Color.Black;
                                result.Document.EndUpdateCharacters(cp);
                            }
                        }

                        result.Document.ReplaceAll("<#CONCLUDE_PRINT;>", sereServExt.CONCLUDE ?? "", SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#NOTE_PRINT;>", sereServExt.NOTE ?? "", SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#DESCRIPTION_PRINT;>", sereServExt.DESCRIPTION ?? "", SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#CURRENT_USERNAME_PRINT;>", lstServiceReq.FirstOrDefault().EXECUTE_USERNAME, SearchOptions.CaseSensitive);

                        foreach (var item in dicParam)
                        {
                            if (item.Value != null && CheckType(item.Value))
                            {
                                string key = string.Format("<#{0}_PRINT;>", item.Key);
                                var rangeSeperators = result.Document.FindAll(key, SearchOptions.CaseSensitive);
                                foreach (var rang in rangeSeperators)
                                {
                                    CharacterProperties cp = result.Document.BeginUpdateCharacters(rang);
                                    cp.ForeColor = Color.Black;
                                    result.Document.EndUpdateCharacters(cp);
                                }

                                result.Document.ReplaceAll(key, item.Value.ToString(), SearchOptions.CaseSensitive);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckType(object value)
        {
            bool result = false;
            try
            {
                result = value.GetType() == typeof(long) || value.GetType() == typeof(int) || value.GetType() == typeof(string) || value.GetType() == typeof(short) || value.GetType() == typeof(decimal) || value.GetType() == typeof(double) || value.GetType() == typeof(float);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void PrintOption2(bool printNow)
        {
            try
            {
                var printDocument = ProcessDocumentBeforePrint(txtDescription);
                if (printDocument == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("printDocument is null");
                    return;
                }

                if (printNow)
                {
                    printDocument.Print();
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printDocument.Print();
                }
                else
                {
                    printDocument.ShowPrintPreview();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintOption1(bool printNow)
        {
            try
            {
                Dictionary<string, string> dicRtfText = new Dictionary<string, string>();

                dicRtfText["DESCRIPTION_WORD"] = txtDescription.RtfText;

                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == MPS000354);

                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.TreatmentWithPatientTypeAlter != null ? TreatmentWithPatientTypeAlter.TREATMENT_CODE : ""), MPS000354, currentModule != null ? currentModule.RoomId : 0);

                richEditorMain.RunPrintTemplate(printTemplate.PRINT_TYPE_CODE, printTemplate.FILE_PATTERN, sereServ.TDL_SERVICE_NAME, null, null, dicParam, dicImage, inputADO, dicRtfText, printNow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDicParamForPrint()
        {
            try
            {
                ProcessDicParam();

                //bổ sung các key nhóm cha của dv
                var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                if (service.PARENT_ID.HasValue)
                {
                    var serviceParent = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == service.PARENT_ID);
                    if (serviceParent != null)
                    {
                        this.dicParam.Add("SERVICE_CODE_PARENT", serviceParent.SERVICE_CODE);
                        this.dicParam.Add("SERVICE_NAME_PARENT", serviceParent.SERVICE_NAME);
                        this.dicParam.Add("HEIN_SERVICE_BHYT_CODE_PARENT", serviceParent.HEIN_SERVICE_BHYT_CODE);
                        this.dicParam.Add("HEIN_SERVICE_BHYT_NAME_PARENT", serviceParent.HEIN_SERVICE_BHYT_NAME);
                    }
                }

                dicParam["IS_COPY"] = "BẢN SAO";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDicParam()
        {
            try
            {
                // chế biến dữ liệu thành các key đơn thêm vào biểu mẫu tương tự như mps excel
                this.dicParam = new Dictionary<string, object>();
                this.dicImage = new Dictionary<string, Image>();

                HIS.Desktop.Print.SetCommonKey.SetCommonSingleKey(this.dicParam);//commonkey
                if (currentServiceReq != null)
                {
                    dicParam.Add("INTRUCTION_TIME_FULL_STR",
                            GetCurrentTimeSeparateBeginTime(
                            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                            currentServiceReq.INTRUCTION_TIME) ?? DateTime.Now));

                    dicParam.Add("INTRUCTION_DATE_FULL_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        currentServiceReq.INTRUCTION_TIME));

                    dicParam.Add("INTRUCTION_TIME_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(
                            currentServiceReq.INTRUCTION_TIME));

                    dicParam.Add("START_TIME_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(
                            currentServiceReq.START_TIME ?? 0));

                    dicParam.Add("START_TIME_FULL_STR",
                            GetCurrentTimeSeparateBeginTime(
                            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                            currentServiceReq.START_TIME ?? 0) ?? DateTime.Now));

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
                    dicParam.Add("AGE", CalculatorAge(currentServiceReq.TDL_PATIENT_DOB, false));
                    dicParam.Add("STR_YEAR", currentServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    dicParam.Add("VIR_PATIENT_NAME", currentServiceReq.TDL_PATIENT_NAME);

                    var executeRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentServiceReq.EXECUTE_ROOM_ID);
                    if (executeRoom != null)
                    {
                        dicParam.Add("EXECUTE_DEPARTMENT_CODE", executeRoom.DEPARTMENT_CODE);
                        dicParam.Add("EXECUTE_DEPARTMENT_NAME", executeRoom.DEPARTMENT_NAME);
                        dicParam.Add("EXECUTE_ROOM_CODE", executeRoom.ROOM_CODE);
                        dicParam.Add("EXECUTE_ROOM_NAME", executeRoom.ROOM_NAME);
                    }

                    var reqRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentServiceReq.REQUEST_ROOM_ID);
                    if (reqRoom != null)
                    {
                        dicParam.Add("REQUEST_DEPARTMENT_CODE", reqRoom.DEPARTMENT_CODE);
                        dicParam.Add("REQUEST_DEPARTMENT_NAME", reqRoom.DEPARTMENT_NAME);
                        dicParam.Add("REQUEST_ROOM_CODE", reqRoom.ROOM_CODE);
                        dicParam.Add("REQUEST_ROOM_NAME", reqRoom.ROOM_NAME);
                    }
                }

                if (TreatmentWithPatientTypeAlter != null)
                {
                    if (!String.IsNullOrEmpty(TreatmentWithPatientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        dicParam.Add("HEIN_CARD_NUMBER_SEPARATE",
                            HeinCardHelper.SetHeinCardNumberDisplayByNumber(TreatmentWithPatientTypeAlter.HEIN_CARD_NUMBER));
                        dicParam.Add("STR_HEIN_CARD_FROM_TIME",
                            Inventec.Common.DateTime.Convert.TimeNumberToDateString(
                            TreatmentWithPatientTypeAlter.HEIN_CARD_FROM_TIME));
                        dicParam.Add("STR_HEIN_CARD_TO_TIME",
                            Inventec.Common.DateTime.Convert.TimeNumberToDateString(
                            TreatmentWithPatientTypeAlter.HEIN_CARD_TO_TIME));
                        dicParam.Add("HEIN_CARD_ADDRESS", TreatmentWithPatientTypeAlter.HEIN_CARD_ADDRESS);
                    }
                    else
                    {
                        dicParam.Add("HEIN_CARD_NUMBER_SEPARATE", "");
                        dicParam.Add("STR_HEIN_CARD_FROM_TIME", "");
                        dicParam.Add("STR_HEIN_CARD_TO_TIME", "");
                        dicParam.Add("HEIN_CARD_ADDRESS", "");
                    }

                    var patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == TreatmentWithPatientTypeAlter.PATIENT_TYPE_CODE);
                    if (patientType != null)
                        dicParam.Add("PATIENT_TYPE_NAME", patientType.PATIENT_TYPE_NAME);
                    else
                        dicParam.Add("PATIENT_TYPE_NAME", "");

                    var treatmentType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == TreatmentWithPatientTypeAlter.TREATMENT_TYPE_CODE);
                    if (treatmentType != null)
                        dicParam.Add("TREATMENT_TYPE_NAME", treatmentType.TREATMENT_TYPE_NAME);
                    else
                        dicParam.Add("TREATMENT_TYPE_NAME", "");

                    dicParam.Add("TREATMENT_ICD_CODE", TreatmentWithPatientTypeAlter.ICD_CODE);
                    dicParam.Add("TREATMENT_ICD_NAME", TreatmentWithPatientTypeAlter.ICD_NAME);
                    dicParam.Add("TREATMENT_ICD_SUB_CODE", TreatmentWithPatientTypeAlter.ICD_SUB_CODE);
                    dicParam.Add("TREATMENT_ICD_TEXT", TreatmentWithPatientTypeAlter.ICD_TEXT);

                    AddKeyIntoDictionaryPrint<HisTreatmentWithPatientTypeInfoSDO>(TreatmentWithPatientTypeAlter, this.dicParam, false);

                    int AGE_NUM = Inventec.Common.DateTime.Calculation.Age(TreatmentWithPatientTypeAlter.TDL_PATIENT_DOB, TreatmentWithPatientTypeAlter.IN_TIME);
                    dicParam.Add("AGE_NUM", AGE_NUM);
                }
                else
                {
                    dicParam.Add("HEIN_CARD_NUMBER_SEPARATE", "");
                    dicParam.Add("STR_HEIN_CARD_FROM_TIME", "");
                    dicParam.Add("STR_HEIN_CARD_TO_TIME", "");
                    dicParam.Add("HEIN_CARD_ADDRESS", "");
                    var typeAlter = new HisTreatmentWithPatientTypeInfoSDO();
                    AddKeyIntoDictionaryPrint<HisTreatmentWithPatientTypeInfoSDO>(typeAlter, this.dicParam, false);
                }

                if (patient != null)
                    AddKeyIntoDictionaryPrint<ADO.PatientADO>(patient, this.dicParam, false);
                HIS_SERE_SERV sereS = new HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereS, this.sereServ);
                AddKeyIntoDictionaryPrint<HIS_SERVICE_REQ>(this.currentServiceReq, this.dicParam, true);
                AddKeyIntoDictionaryPrint<HIS_SERE_SERV>(sereS, this.dicParam, false);
                AddKeyIntoDictionaryPrint<HIS_SERE_SERV_EXT>(this.sereServExt, this.dicParam, true);

                if (this.sereServExt != null)
                {
                    if (!dicParam.ContainsKey("END_TIME_FULL_STR"))
                        dicParam.Add("END_TIME_FULL_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                                this.sereServExt.END_TIME ?? 0));
                    else
                        dicParam["END_TIME_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                                this.sereServExt.END_TIME ?? 0);
                    if (!dicParam.ContainsKey("BEGIN_TIME_FULL_STR"))
                        dicParam.Add("BEGIN_TIME_FULL_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                                this.sereServExt.BEGIN_TIME ?? 0));
                    else
                        dicParam["BEGIN_TIME_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                                this.sereServExt.BEGIN_TIME ?? 0);

                    if (this.sereServExt.MACHINE_ID.HasValue)
                    {
                        var machine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>().FirstOrDefault(o => o.ID == this.sereServExt.MACHINE_ID.Value);
                        if (machine != null)
                        {
                            dicParam["MACHINE_NAME"] = machine.MACHINE_NAME;
                        }
                    }

                    if (sereServExt.END_TIME.HasValue)
                    {
                        dicParam["EXECUTE_DATE_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(sereServExt.END_TIME.Value);
                        dicParam["EXECUTE_TIME_FULL_STR"] = GetCurrentTimeSeparateBeginTime(sereServExt.END_TIME.Value);
                    }
                    else if (sereServExt.MODIFY_TIME.HasValue)
                    {
                        dicParam["EXECUTE_DATE_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(sereServExt.MODIFY_TIME.Value);
                        dicParam["EXECUTE_TIME_FULL_STR"] = GetCurrentTimeSeparateBeginTime(sereServExt.MODIFY_TIME.Value);
                    }
                    else
                    {
                        dicParam["EXECUTE_DATE_FULL_STR"] = "";
                        dicParam["EXECUTE_TIME_FULL_STR"] = "";
                    }
                }
                else
                {
                    dicParam["EXECUTE_DATE_FULL_STR"] = "";
                    dicParam["EXECUTE_TIME_FULL_STR"] = "";
                    dicParam["MACHINE_NAME"] = "";
                }

                dicParam.Add("USER_NAME", UserName);

                //bỏ key để phục vụ đổ dữ liệu khi in
                foreach (var item in keyPrint)
                {
                    dicParam.Remove(item);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        /// <summary>
        /// Hiển thị định dạng 23 giờ 59 phút Ngày 12 tháng 10 năm 2015
        /// </summary>
        /// <returns></returns>
        internal static string GetCurrentTimeSeparateBeginTime(long time)
        {
            string result = "";
            try
            {
                if (time > 0)
                {
                    string temp = time.ToString();
                    string year = string.Format("{0:00}", temp.Substring(0, 4));
                    string month = string.Format("{0:00}", temp.Substring(4, 2));
                    string day = string.Format("{0:00}", temp.Substring(6, 2));
                    string hours = string.Format("{0:00}", temp.Substring(8, 2));
                    string minute = string.Format("{0:00}", temp.Substring(10, 2));
                    result = string.Format("{0} giờ {1} phút ngày {2} tháng {3} năm {4}", hours, minute, day, month, year);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private string CalculatorAge(long ageYearNumber, bool isHl7)
        {
            string tuoi = "";
            try
            {
                string caption__Tuoi = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__AGE");
                string caption__ThangTuoi = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__MONTH_OLDS");
                string caption__NgayTuoi = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__DAY_OLDS");
                string caption__GioTuoi = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__HOURS_OLDS");

                if (isHl7)
                {
                    caption__Tuoi = "T";
                    caption__ThangTuoi = "TH";
                    caption__NgayTuoi = "NT";
                    caption__GioTuoi = "GT";
                }

                if (ageYearNumber > 0)
                {
                    System.DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ageYearNumber).Value;
                    if (dtNgSinh == System.DateTime.MinValue) throw new ArgumentNullException("dtNgSinh");

                    TimeSpan diff__hour = (System.DateTime.Now - dtNgSinh);
                    TimeSpan diff__month = (System.DateTime.Now.Date - dtNgSinh.Date);

                    //- Dưới 24h: tính chính xác đến giờ.
                    double hour = diff__hour.TotalHours;
                    if (hour < 24)
                    {
                        tuoi = ((int)hour + " " + caption__GioTuoi);
                    }
                    else
                    {
                        long tongsogiay__hour = diff__hour.Ticks;
                        System.DateTime newDate__hour = new System.DateTime(tongsogiay__hour);
                        int month__hour = ((newDate__hour.Year - 1) * 12 + newDate__hour.Month - 1);
                        if (month__hour == 0)
                        {
                            //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                            tuoi = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                        }
                        else
                        {
                            long tongsogiay = diff__month.Ticks;
                            System.DateTime newDate = new System.DateTime(tongsogiay);
                            int month = ((newDate.Year - 1) * 12 + newDate.Month - 1);
                            if (month == 0)
                            {
                                //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                                tuoi = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                            }
                            else
                            {
                                //- Dưới 72 tháng tuổi: tính chính xác đến tháng như hiện tại
                                if (month < 72)
                                {
                                    tuoi = (month + " " + caption__ThangTuoi);
                                }
                                //- Trên 72 tháng tuổi: tính chính xác đến năm: tuổi= năm hiện tại - năm sinh
                                else
                                {
                                    int year = System.DateTime.Now.Year - dtNgSinh.Year;
                                    tuoi = (year + " " + caption__Tuoi);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                tuoi = "";
            }
            return tuoi;
        }

        /// <summary>
        /// gán dữ liệu vào diction để fill data vào word
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="dicParamPlus"></param>
        /// <param name="autoOveride"> ghi đè dữ liệu</param>
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

        private void LoadTreatmentWithPaty()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("1. Begin LoadTreatmentWithPaty");
                CommonParam param = new CommonParam();

                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = this.sereServ.TDL_TREATMENT_ID ?? 0;
                filter.INTRUCTION_TIME = this.sereServ.TDL_INTRUCTION_TIME;
                var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    TreatmentWithPatientTypeAlter = apiResult.FirstOrDefault();
                }
                Inventec.Common.Logging.LogSystem.Info("1. End LoadTreatmentWithPaty");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetPatientById()
        {
            try
            {
                MOS.Filter.HisPatientViewFilter patientFilter = new MOS.Filter.HisPatientViewFilter();
                patientFilter.ID = sereServ.TDL_PATIENT_ID;
                CommonParam param = new CommonParam();
                var patients = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_PATIENT>>(ApiConsumer.HisRequestUriStore.HIS_PATIENT_GET, ApiConsumer.ApiConsumers.MosConsumer, patientFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (patients != null && patients.Count > 0)
                {
                    patient = new ADO.PatientADO(patients.First());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetServiceReq()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter reqFilter = new MOS.Filter.HisServiceReqFilter();
                reqFilter.ID = this.sereServ.SERVICE_REQ_ID;
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, reqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (result != null && result.Count > 0)
                {
                    currentServiceReq = result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnOpenWeb_Click(object sender, EventArgs e)
        {
            try
            {
                OpenWebFromConfig(true, false, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenWebFromConfig(bool isShowMess, bool isCheck, bool isOpenWeb = false)
        {
            try
            {
                string url = "";
                try
                {
                    if (PacsCFG.PACS_ADDRESS != null && PacsCFG.PACS_ADDRESS.Count > 0)
                    {
                        var dtExecuteRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(p => p.ROOM_ID == currentServiceReq.EXECUTE_ROOM_ID);
                        if (dtExecuteRoom != null)
                        {
                            PacsAddress address = null;
                            List<PacsAddress> listValidAddress = new List<PacsAddress>();
                            foreach (var item in PacsCFG.PACS_ADDRESS)
                            {
                                if (!String.IsNullOrWhiteSpace(item.RoomCode))
                                {
                                    string[] listRoomCode = item.RoomCode.Split('|');
                                    if (listRoomCode != null && listRoomCode.Contains(dtExecuteRoom.EXECUTE_ROOM_CODE))
                                    {
                                        listValidAddress.Add(item);
                                    }
                                }
                            }
                            address = listValidAddress.OrderByDescending(o => !String.IsNullOrWhiteSpace(o.CloudInfo))
                                                    .ThenByDescending(o => !String.IsNullOrWhiteSpace(o.Api))
                                                    .FirstOrDefault();
                            if (address != null && !String.IsNullOrWhiteSpace(address.CloudInfo))
                            {
                                url = txtUrl.Text;
                                if (String.IsNullOrWhiteSpace(url) || !url.StartsWith("http"))
                                {
                                    if (sereServ != null && sereServ.ID > 0)
                                    {
                                        CreateThreadGetLinkResult(sereServ.ID, isOpenWeb);
                                        return;
                                    }
                                }
                                else
                                {
                                    isSense = true;
                                }
                            }
                            else if (address != null && !String.IsNullOrWhiteSpace(address.Api))
                            {
                                if (isShowMess)
                                {
                                    if (!chkAutoOpenWeb.Checked)
                                    {
                                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn tự động mở trình duyệt cho lần xem kết quả tiếp theo không?", "Thông báo", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                                        {
                                            isContineuCheckbox = true;
                                            chkAutoOpenWeb.Checked = true;
                                        }
                                    }
                                    isContineuCheckbox = false;
                                }
                                if (isCheck)
                                {
                                    if (!(currentServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL || currentServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT))
                                    {
                                        return;
                                    }
                                }

                                if (!String.IsNullOrWhiteSpace(sereServExt.JSON_FORM_ID) && sereServExt.JSON_FORM_ID.StartsWith("http"))
                                {
                                    url = sereServExt.JSON_FORM_ID;
                                }
                                else
                                {
                                    HIS_PATIENT patient = GetPatientById(sereServ.TDL_PATIENT_ID);

                                    url = string.Format("http://{0}{1}", address.Address, address.Api);

                                    if (address.Api.Trim().StartsWith("http"))
                                    {
                                        url = address.Api;
                                    }

                                    string idChiDinh = sereServ.ID.ToString();
                                    string idBenhNhan = patient.PATIENT_CODE;
                                    string idDotVaoVien = sereServ.TDL_TREATMENT_CODE;
                                    string PACS_BASE_URI = ConfigSystems.URI_API_PACS;

                                    url = url.Replace("<#PACS_BASE_URI;>", PACS_BASE_URI);
                                    var urlSplit = address.Api.Split('=', '&');
                                    var keyUrl = urlSplit.Where(o => o.Contains(":")).ToList();
                                    foreach (var item in keyUrl)
                                    {
                                        if (item.Contains("idChiDinh"))
                                        {
                                            url = url.Replace(":idChiDinh", idChiDinh);
                                        }
                                        else if (item.Contains("idBenhNhan"))
                                        {
                                            url = url.Replace(":idBenhNhan", idBenhNhan);
                                        }
                                        else if (item.Contains("idDotVaoVien"))
                                        {
                                            url = url.Replace(":idDotVaoVien", idDotVaoVien);
                                        }
                                    }
                                }

                                isSense = true;
                            }
                            else if (address == null || (address != null && String.IsNullOrWhiteSpace(address.Api)))
                            {
                                isSense = false;
                                if (isShowMess)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Phòng xử lý chưa được thiết lập đường dẫn xem kết quả vui lòng thử lại sau.", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK);
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                        else
                        {
                            isSense = false;
                            if (isShowMess)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Phòng xử lý chưa được thiết lập đường dẫn xem kết quả vui lòng thử lại sau.", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK);
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    isSense = false;
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                if (isSense)
                {
                    xtraTab.ShowTabHeader = DevExpress.Utils.DefaultBoolean.True;
                    txtUrl.Text = url;
                    System.Diagnostics.Process.Start(url);
                    webView1.Url = url;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAutoOpenWeb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAutoOpenWeb.Checked && !isContineuCheckbox)
                {
                    OpenWebFromConfig(false, true);
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoOpenWeb.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoOpenWeb.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoOpenWeb.Name;
                    csAddOrUpdate.VALUE = (chkAutoOpenWeb.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkAutoOpenWeb.Name)
                        {
                            chkAutoOpenWeb.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDropDownPrint_Click(object sender, EventArgs e)
        {
            btnDropDownPrint.ShowDropDown();
        }
    }
}
