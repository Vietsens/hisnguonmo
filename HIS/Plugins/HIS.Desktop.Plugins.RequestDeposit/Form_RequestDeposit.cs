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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.ListDepositRequest;
using MOS.EFMODEL.DataModels;
using HIS.UC.ListDepositRequest.ADO;
using HIS.UC.ListDepositRequest.Reload;
using HIS.UC.ListDepositRequest.GetSelectRow;
using HIS.UC.ListDepositRequest.Run;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using Inventec.Common.RichEditor.Base;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Number;
using HIS.Desktop.Print;
using Inventec.Common.RichEditor.DAL;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.RequestDeposit
{
    public partial class Form_RequestDeposit : HIS.Desktop.Utility.FormBase
    {
        ListDepositRequestProcessor listDepositReqProcessor = null;
        UserControl ucRequestDeposit = null;
        V_HIS_DEPOSIT_REQ VHisDepositReq = new V_HIS_DEPOSIT_REQ();
        List<V_HIS_DEPOSIT_REQ> listDepositReq = new List<V_HIS_DEPOSIT_REQ>();
        List<V_HIS_DEPOSIT_REQ> currentlistDepositReq = new List<V_HIS_DEPOSIT_REQ>();
        V_HIS_DEPOSIT_REQ depositReq = new V_HIS_DEPOSIT_REQ();
        private V_HIS_DEPOSIT_REQ depositReqPrint { get; set; }
        V_HIS_DEPOSIT_REQ currentdepositReq = new V_HIS_DEPOSIT_REQ();
        ListDepositRequestInitADO listDepositRequestADO = new ListDepositRequestInitADO();
        List<HIS_DEPOSIT_REASON> depositReasons;
        bool isObligatory;
        public const string Difference = "Difference";

        internal int action = -1;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentID;
        bool isUpdate = false;

        bool isSupplement = false;

        long roomId;
        long roomTypeId;
        Inventec.Common.RichEditor.RichEditorStore richEditorMain;

        int positionHandleControl = -1;
        public Form_RequestDeposit(Inventec.Desktop.Common.Modules.Module module, long treatmentID)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.treatmentID = treatmentID;
                InitListDepositReqGrid();
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Form_RequestDeposit_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                CheckLockTreatmentFee();
                SetCaptionByLanguageKey();
                //InitLinkText();
                LoadDefaultDepositReason();
                InitLinkText();

                getDataDepositReq(this.treatmentID);
                FillDataToGridDepositReq();
                this.action = GlobalVariables.ActionAdd;
                EnableControlChanged(action);
                SetPrice();//xuandv
                spinEditPrice.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckLockTreatmentFee()
        {
            try
            {
                if (this.treatmentID > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                    filter.ID = this.treatmentID;

                    var treatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TREATMENT>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).FirstOrDefault();
                    if (treatment != null && treatment.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Hồ sơ đã khóa viện phí", "Thông báo");
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDefaultDepositReason()
        {
            try
            {
                this.isObligatory = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.RequestDeposit.ReasonMustBeEnteredByCategory") == "1";
                if (isObligatory)
                {
                    layoutControlItem4.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    txtGhiChu.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitLinkText()
        {

            try
            {
                CommonParam param = new CommonParam();
                HisDepositReasonFilter filter = new HisDepositReasonFilter();
                filter.IS_ACTIVE = 1;
                filter.IS_COMMON = 1;
                depositReasons = new BackendAdapter(param).Get<List<HIS_DEPOSIT_REASON>>("api/HisDepositReason/Get", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("depositReasons___:", depositReasons));
                depositReasons = depositReasons.Where(o => !string.IsNullOrWhiteSpace(o.ABBREVIATION)).OrderBy(p => p.ABBREVIATION).ToList();

                this.linkLabel1.Links.Clear();
                var numberOfText = depositReasons.Count;
                if (numberOfText > 0)
                {
                    int total = 0;
                    int oldLocation = 0;
                    //var linkLabel = new LinkLabel();
                    //linkLabel.LinkClicked += linkLabel_LinkClicked;

                    for (int i = 0; i < numberOfText; i++)
                    {
                        //depositReasons[i].ABBREVIATION
                        linkLabel1.Text += depositReasons[i].ABBREVIATION + "; ";
                        linkLabel1.Links.Add(oldLocation, depositReasons[i].ABBREVIATION.Length + 1, depositReasons[i].DEPOSIT_REASON_NAME);
                        oldLocation += depositReasons[i].ABBREVIATION.Length + 2;
                        total += depositReasons[i].ABBREVIATION.Length + 3;
                    }


                    linkLabel1.Text += "Khác...";
                    linkLabel1.Links.Add(oldLocation, 7, Difference);

                    //int after = total / linkLabel1.Width;
                    //linkLabel1.Height = after * 24;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (e.Link.LinkData == Difference)
                {
                    CommonParam param = new CommonParam();
                    HisDepositReasonFilter filter = new HisDepositReasonFilter();
                    filter.IS_ACTIVE = 1;
                    filter.IS_COMMON = 1;
                    depositReasons = new BackendAdapter(param).Get<List<HIS_DEPOSIT_REASON>>("api/HisDepositReason/Get", ApiConsumers.MosConsumer, filter, param);

                    Form_RequestDepositReason frm = new Form_RequestDepositReason(depositReasons, GetReasonSelected);
                    frm.ShowDialog();
                }
                else
                    txtGhiChu.Text = e.Link.LinkData + "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        //private void InitLinkText()
        //{
        //    try
        //    {
        //        CommonParam param = new CommonParam();
        //        HisDepositReasonFilter filter = new HisDepositReasonFilter();
        //        filter.IS_ACTIVE = 1;
        //        filter.IS_COMMON = 1;
        //        depositReasons = new BackendAdapter(param).Get<List<HIS_DEPOSIT_REASON>>("api/HisDepositReason/Get", ApiConsumers.MosConsumer, filter, param);
        //        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("depositReasons___:", depositReasons));
        //        depositReasons = depositReasons.Where(o => !string.IsNullOrWhiteSpace(o.ABBREVIATION)).OrderBy(p => p.ABBREVIATION).ToList();
        //        //this.lciLinkText.Controls.Clear();
        //        //this.xtraScrollableContentLibrary.Controls.Clear();
        //        var numberOfText = depositReasons.Count;
        //        if (numberOfText == 0)
        //        {
        //            layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        //        }
        //        if (numberOfText > 0)
        //        {
        //            int oldLocation = 0;
        //            for (int i = 0; i < numberOfText; i++)
        //            {
        //                // Khoi tao labelcontrol
        //                DevExpress.XtraEditors.HyperlinkLabelControl hyperlinkLabelControl1 = new DevExpress.XtraEditors.HyperlinkLabelControl();
        //                //if (i % 2 == 0)
        //                //    hyperlinkLabelControl1.ForeColor = Color.Blue;
        //                hyperlinkLabelControl1.Cursor = System.Windows.Forms.Cursors.Hand;

        //                hyperlinkLabelControl1.Name = depositReasons[i].ABBREVIATION;
        //                hyperlinkLabelControl1.TabIndex = 4;
        //                hyperlinkLabelControl1.Text = depositReasons[i].ABBREVIATION + ";";
        //                hyperlinkLabelControl1.Tag = depositReasons[i];
        //                var keySize = hyperlinkLabelControl1.CalcBestSize();
        //                hyperlinkLabelControl1.Location = new System.Drawing.Point(oldLocation, 0);
        //                hyperlinkLabelControl1.Size = new System.Drawing.Size(keySize.Width, 13);
        //                hyperlinkLabelControl1.StyleController = this.lciLinkText;

        //                hyperlinkLabelControl1.Click += new System.EventHandler(this.hyperlinkLabelControl1_Click);


        //                //hyperlinkLabelControl1.Font = new Font(lblControl.Font, FontStyle.Bold);
        //                //lblControl.StyleController = this.lciContentLibrary;

        //                oldLocation += keySize.Width + 3;
        //                this.lciLinkText.Controls.Add(hyperlinkLabelControl1);
        //            }

        //            // Link Text "Khác..."
        //            DevExpress.XtraEditors.HyperlinkLabelControl hyperlinkLabelAnother = new DevExpress.XtraEditors.HyperlinkLabelControl();
        //            //if (i % 2 == 0)
        //            //    hyperlinkLabelControl1.ForeColor = Color.Blue;
        //            hyperlinkLabelAnother.Cursor = System.Windows.Forms.Cursors.Hand;

        //            hyperlinkLabelAnother.Name = "lblLinkAnother";
        //            hyperlinkLabelAnother.TabIndex = 4;
        //            hyperlinkLabelAnother.Text = "Khác...";
        //            //hyperlinkLabelAnother.Tag = depositReasons[i];
        //            var labelSize = hyperlinkLabelAnother.CalcBestSize();
        //            hyperlinkLabelAnother.Location = new System.Drawing.Point(oldLocation, 0);
        //            hyperlinkLabelAnother.Size = new System.Drawing.Size(labelSize.Width, 13);
        //            hyperlinkLabelAnother.StyleController = this.lciLinkText;

        //            hyperlinkLabelAnother.Click += new System.EventHandler(this.hyperlinkLabelAnother_Click);
        //            this.lciLinkText.Controls.Add(hyperlinkLabelAnother);
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void hyperlinkLabelAnother_Click(object sender, EventArgs e)
        {
            try
            {
                Form_RequestDepositReason frm = new Form_RequestDepositReason(this.depositReasons, GetReasonSelected);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetReasonSelected(HIS_DEPOSIT_REASON depositReason)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("selecteddata__:", depositReason));
                if (depositReason != null)
                {
                    txtGhiChu.Text = depositReason.DEPOSIT_REASON_NAME;
                }
                //this.dicSereServPttt[this.clickServiceADO.ID] = sereservPttt;
                ////this.dicEkipUser[this.clickServiceADO.ID] = ekipUserADOs;

                ////if (ekipUserADOs != null && ekipUserADOs.Count > 0)
                ////{
                ////    gridControlEkip.DataSource = null;
                ////    gridControlEkip.DataSource = ekipUserADOs;
                ////    gridControlEkip.RefreshDataSource();
                ////}

                //this.sereServExt = sereServExt;
                //Inventec.Common.Logging.LogSystem.Debug("SaveSSPtttInfoClick sereservPttt MANNER " + Inventec.Common.Logging.LogUtil.TraceData("", sereservPttt.MANNER));
                //Inventec.Common.Logging.LogSystem.Debug("SaveSSPtttInfoClick sereServExt INSTRUCTION_NOTE " + Inventec.Common.Logging.LogUtil.TraceData("", sereServExt.INSTRUCTION_NOTE));
                //if (this.sereServExt != null)
                //{
                //    if (this.dicSereServExt != null && this.dicSereServExt.ContainsKey(this.sereServExt.SERE_SERV_ID))
                //    {
                //        this.dicSereServExt[this.sereServExt.SERE_SERV_ID] = this.sereServExt;
                //    }
                //    else if (this.dicSereServExt != null)
                //    {
                //        this.dicSereServExt.Add(this.sereServExt.SERE_SERV_ID, this.sereServExt);
                //    }

                //    if (this.sereServExt.MACHINE_ID != this.sereServ.MACHINE_ID)
                //    {
                //        var serviceADOEdit = listServiceADO != null ? listServiceADO.Where(o => o.ID == this.sereServExt.SERE_SERV_ID).FirstOrDefault() : null;
                //        if (serviceADOEdit != null)
                //        {
                //            serviceADOEdit.MACHINE_ID = this.sereServExt.MACHINE_ID;
                //        }
                //        gridControlSereServ.RefreshDataSource();
                //    }
                //}
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.sereServExt), this.sereServExt));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void hyperlinkLabelControl1_Click(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.HyperlinkLabelControl hyperlinkLabelControl = sender as DevExpress.XtraEditors.HyperlinkLabelControl;
                HIS_DEPOSIT_REASON data = hyperlinkLabelControl.Tag as HIS_DEPOSIT_REASON;

                if (data != null)
                {
                    //string bitString = HIS.Desktop.Utility.TextLibHelper.BytesToString(data.DEPOSIT_REASON_NAME);
                    txtGhiChu.Text = data.DEPOSIT_REASON_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RequestDeposit.Resources.Lang", typeof(HIS.Desktop.Plugins.RequestDeposit.Form_RequestDeposit).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("Form_RequestDeposit.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("Form_RequestDeposit.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("Form_RequestDeposit.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("Form_RequestDeposit.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("Form_RequestDeposit.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("Form_RequestDeposit.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("Form_RequestDeposit.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("Form_RequestDeposit.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("Form_RequestDeposit.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("Form_RequestDeposit.barButtonItem3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("Form_RequestDeposit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        void Grid_DeleteClick(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                //currentdepositReq = null;
                if (data != null)
                {
                    var process = new V_HIS_DEPOSIT_REQ();
                    process = data;
                    var result = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_DEPOSIT_REQ_DELETE, ApiConsumer.ApiConsumers.MosConsumer, process.ID, param);
                    if (result)
                    {
                        success = true;
                        getDataDepositReq(this.treatmentID);
                        FillDataToGridDepositReq();
                        txtGhiChu.Text = null;
                        spinEditPrice.EditValue = null;
                        this.action = GlobalVariables.ActionAdd;
                        EnableControlChanged(action);
                        RemoveValidate();
                    }
                    WaitingManager.Hide();

                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void Grid_PrintClick(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                if (barManagerPrint == null)
                {
                    barManagerPrint = new DevExpress.XtraBars.BarManager();
                }
                barManagerPrint.Form = this;
                LoadMenuPrint(barManagerPrint, data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private HIS_CONFIG selectedConfig = new HIS_CONFIG();

        List<HIS_CONFIG> listConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(s => s.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(s.VALUE)).ToList();
        class ConfigInfo
        {
            public string BANK { get; set; }
            public string VALUE { get; set; }
        }
        void Grid_QRClick(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                var currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(s => s.ID == this.currentModule.RoomId && !string.IsNullOrEmpty(s.QR_CONFIG_JSON)) ;
                if (currentRoom != null && currentRoom.Count() > 0)
                {
                    ConfigInfo _config = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigInfo>(currentRoom.FirstOrDefault().QR_CONFIG_JSON);
                    HIS_CONFIG _cf = new HIS_CONFIG();
                    if(string.IsNullOrWhiteSpace(_config.BANK)) MessageBox.Show(this, "Cấu hình thiếu thông tin ngân hàng.","Thông báo",MessageBoxButtons.OK);
                    _cf.KEY = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", _config.BANK.Trim());
                    _cf.VALUE = _config.VALUE;
                    //co cau hinh QR o buong benh
                    List<object> listArgs = new List<object>();
                    TransReqQRADO adoqr = new TransReqQRADO();
                    adoqr.TreatmentId = treatmentID;
                    adoqr.ConfigValue = _cf;
                    adoqr.TransReqId = CreateReqType.Deposit;
                    adoqr.DepositReq = data;
                    listArgs.Add(adoqr);
                    LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR " + LogUtil.TraceData("listArgs", listArgs));
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                }
                else
                {


                    if (this.listConfig.Count > 1)
                    {

                        popupMenu1.ClearLinks();

                        foreach (var item in listConfig)
                        {
                            string key = "";
                            string value = item.KEY;
                            int index = value.IndexOf("Info");
                            if (index > 0)
                            {
                                var shotkey = value.Substring(0, index);
                                string[] parts = shotkey.Split('.');
                                if (parts.Length > 0)
                                {
                                    key = parts[parts.Length - 1]; // Lay phan cuoi cung
                                }
                            }
                            else
                            {
                                key = item.KEY;
                            }


                            BarButtonItem btnOption = new BarButtonItem(null, key);
                            btnOption.ItemClick += (s, args) =>
                            {

                                selectedConfig = item;
                                List<object> listArgs = new List<object>();
                                TransReqQRADO adoqr = new TransReqQRADO();
                                adoqr.TreatmentId = treatmentID;
                                adoqr.ConfigValue = selectedConfig;
                                adoqr.TransReqId = CreateReqType.Deposit;
                                adoqr.DepositReq = data;


                                listArgs.Add(adoqr);
                                LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR ; KEY: " + selectedConfig.KEY);

                                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                            };
                            popupMenu1.AddItem(btnOption);
                        }
                        popupMenu1.Manager = barManager1;
                        popupMenu1.ShowPopup(Control.MousePosition);
                    }
                    else
                    {
                        selectedConfig = listConfig[0];
                        List<object> listArgs = new List<object>();
                        TransReqQRADO adoqr = new TransReqQRADO();
                        adoqr.TreatmentId = treatmentID;
                        adoqr.ConfigValue = selectedConfig;
                        adoqr.TransReqId = CreateReqType.Deposit;
                        adoqr.DepositReq = data;
                        listArgs.Add(adoqr);
                        LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR " + selectedConfig.KEY);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);


                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadMenuPrint(DevExpress.XtraBars.BarManager barManager, V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumerStore.SarConsumer, UriBaseStore.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                currentdepositReq = data;
                PopupMenu menu = new PopupMenu(barManager);
                menu.ItemLinks.Clear();

                BarButtonItem itemYeuCauTamUng = new BarButtonItem(barManager, "Phiếu yêu cầu tạm ứng");
                itemYeuCauTamUng.ItemClick += new ItemClickEventHandler(GiayYeuCauTamUngClick);

                BarButtonItem itemXacNhanTamUng = new BarButtonItem(barManager, "Phiếu xác nhận tạm ứng");
                itemXacNhanTamUng.ItemClick += new ItemClickEventHandler(GiayXacNhanTamUngClick);
                if (currentdepositReq != null)
                {
                    menu.AddItems(new BarItem[] { itemYeuCauTamUng });
                    if (currentdepositReq.DEPOSIT_ID != null && currentdepositReq.TRANSACTION_IS_CANCEL != 1)
                        menu.AddItems(new BarItem[] { itemXacNhanTamUng });
                }
                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GiayXacNhanTamUngClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (currentdepositReq != null && currentdepositReq.TRANSACTION_IS_CANCEL != 1)
                {
                    depositReqPrint = currentdepositReq;
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__XAC_NHAN_TAM_UNG__MPS000489, delegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GiayYeuCauTamUngClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (currentdepositReq != null)
                {
                    depositReqPrint = currentdepositReq;
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__YEU_CAU_TAM_UNG__MPS000091, delegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void Grid_RowCellClick(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                currentdepositReq = null;
                if (data != null)
                {
                    currentdepositReq = data;
                    spinEditPrice.Value = data.AMOUNT;
                    txtGhiChu.Text = data.DESCRIPTION;
                    this.action = GlobalVariables.ActionEdit;
                    EnableControlChanged(action);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitListDepositReqGrid()
        {
            try
            {

                this.listDepositReqProcessor = new ListDepositRequestProcessor();
                ListDepositRequestInitADO ado = new ListDepositRequestInitADO();
                ado.ListDepositReqGrid_CustomUnboundColumnData = depositReqGrid__CustomUnboundColumnData;
                ado.ListDepositReqGrid_RowCellClick = Grid_RowCellClick;
                ado.ListDepositReqGrid_CustomRowCellEdit = gridView_CustomRowCellEdit;
                ado._btnDelete_Click = Grid_DeleteClick;
                ado._btnPrint_Click = Grid_PrintClick;
                ado._btnQR_Click = Grid_QRClick;
                ado.ListDepositReqGrid_RowCellStyle = gridView_RowCellStyle;
                ado.barManager = barManagerPrint;
                ado.visibleColumn = true;
                ado.IsShowSearchPanel = false;
                ado.IsShowPagingPanel = false;
                ado.ListDepositReqColumn = new List<ListDepositRequestColumn>();

                ListDepositRequestColumn colAmount = new ListDepositRequestColumn("Số tiền", "AMOUNT_DISPLAY", 120, false, true);
                colAmount.VisibleIndex = 4;
                colAmount.Format = new DevExpress.Utils.FormatInfo();
                colAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAmount.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colAmount.Format.FormatString = "#,##0.00";
                ado.ListDepositReqColumn.Add(colAmount);

                ListDepositRequestColumn colAmountTransaction = new ListDepositRequestColumn("Số tiền đã đóng", "AMOUNT_DISPLAY_TRANSACTION", 120, false, true);
                colAmountTransaction.VisibleIndex = 4;
                colAmountTransaction.Format = new DevExpress.Utils.FormatInfo();
                colAmountTransaction.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAmountTransaction.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colAmountTransaction.Format.FormatString = "#,##0.00";
                ado.ListDepositReqColumn.Add(colAmountTransaction);

                ListDepositRequestColumn colDescription = new ListDepositRequestColumn("Ghi chú", "DESCRIPTION", 200, false, true);
                colDescription.VisibleIndex = 5;
                ado.ListDepositReqColumn.Add(colDescription);

                ListDepositRequestColumn colRoomName = new ListDepositRequestColumn("Phòng yêu cầu", "ROOM_NAME", 120, false, true);
                colRoomName.VisibleIndex = 6;
                ado.ListDepositReqColumn.Add(colRoomName);

                ListDepositRequestColumn colDepartName = new ListDepositRequestColumn("Khoa yêu cầu", "DEPARTMENT_NAME", 120, false, true);
                colDepartName.VisibleIndex = 7;
                ado.ListDepositReqColumn.Add(colDepartName);

                ListDepositRequestColumn colCreateTime = new ListDepositRequestColumn("Thời gian tạo", "CREATE_TIME_DISPLAY", 120, false, true);
                colCreateTime.VisibleIndex = 8;
                colCreateTime.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListDepositReqColumn.Add(colCreateTime);

                ListDepositRequestColumn colCreator = new ListDepositRequestColumn("Người tạo", "CREATOR", 80, false, true);
                colCreator.VisibleIndex = 9;
                ado.ListDepositReqColumn.Add(colCreator);

                ListDepositRequestColumn colModifyTime = new ListDepositRequestColumn("Thời gian sửa", "MODIFY_TIME_DISPLAY", 120, false, true);
                colModifyTime.VisibleIndex = 10;
                colModifyTime.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListDepositReqColumn.Add(colModifyTime);

                ListDepositRequestColumn colModifier = new ListDepositRequestColumn("Người sửa", "MODIFIER", 80, false, true);
                colModifier.VisibleIndex = 11;
                ado.ListDepositReqColumn.Add(colModifier);

                this.ucRequestDeposit = (UserControl)this.listDepositReqProcessor.Run(ado);
                if (this.ucRequestDeposit != null)
                {
                    this.panelControl1.Controls.Add(this.ucRequestDeposit);
                    this.ucRequestDeposit.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void SetPrice()
        {
            try
            {
                HisTreatmentFeeViewFilter treatFilter = new HisTreatmentFeeViewFilter();
                treatFilter.ID = this.treatmentID;
                var treat = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, treatFilter, null).FirstOrDefault();
                if (treat != null)
                {
                    decimal totalReceive = ((treat.TOTAL_DEPOSIT_AMOUNT ?? 0) + (treat.TOTAL_BILL_AMOUNT ?? 0) - (treat.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (treat.TOTAL_BILL_FUND ?? 0) - (treat.TOTAL_REPAY_AMOUNT ?? 0)) - (treat.TOTAL_BILL_EXEMPTION ?? 0);

                    decimal totalReceiveMore = (treat.TOTAL_PATIENT_PRICE ?? 0) - totalReceive - (treat.TOTAL_BILL_FUND ?? 0) - (treat.TOTAL_BILL_EXEMPTION ?? 0);
                    // string TotalReceiveMorePrice = Inventec.Common.Number.Convert.NumberToString(totalReceiveMore, ConfigApplications.NumberSeperator);
                    if (totalReceiveMore > 0)
                    {
                        this.spinEditPrice.Value = totalReceiveMore;
                    }
                    else
                    {
                        this.spinEditPrice.EditValue = null;
                    }
                }
                else
                {
                    this.spinEditPrice.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                getDataDepositReq(this.treatmentID);
                FillDataToGridDepositReq();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDepositReqCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    getDataDepositReq(this.treatmentID);
                    FillDataToGridDepositReq();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
