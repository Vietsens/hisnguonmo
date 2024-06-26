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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using MOS.SDO;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.Plugins.ServiceReqList.ADO;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.ServiceReqList.Reason
{
    public partial class frmReason : HIS.Desktop.Utility.FormBase
    {
        ServiceReqADO serviceReqAdo;
        ListMedicineADO sereServAdo;
        string moduleLink = "HIS.Desktop.Plugins.ServiceReqList";
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        Inventec.Desktop.Common.Modules.Module module;
        bool isSereServ;
        bool isInit;

        public frmReason(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        public frmReason(Inventec.Desktop.Common.Modules.Module module, ServiceReqADO _serviceReq)
            : this(module)
        {
            this.module = module;
            this.serviceReqAdo = _serviceReq;
            InitializeComponent();
        }

        public frmReason(Inventec.Desktop.Common.Modules.Module module, ServiceReqADO _serviceReq, ListMedicineADO _sereServ)
            : this(module)
        {
            this.module = module;
            this.serviceReqAdo = _serviceReq;
            this.sereServAdo = _sereServ;
            InitializeComponent();
        }

        private void frmReason_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetDefaultValue();
                InitControlState();
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
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmReason.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmReason.btnPrint.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmReason.btnSave.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.chkReasonClose.Properties.Caption = Inventec.Common.Resource.Get.Value("frmReason.chkReasonClose.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.chkResonPrint.Properties.Caption = Inventec.Common.Resource.Get.Value("frmReason.chkResonPrint.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmReason.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmReason.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                txtReason.Text = "BS YC hoàn dịch vụ";
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
                isInit = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkResonPrint.Name)
                        {
                            chkResonPrint.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkReasonClose.Name)
                        {
                            chkReasonClose.Checked = item.VALUE == "1";
                        }
                    }
                }
                isInit = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void chkResonPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                    return;
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkResonPrint.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkResonPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkResonPrint.Name;
                    csAddOrUpdate.VALUE = (chkResonPrint.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkReasonClose_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                    return;
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkReasonClose.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkReasonClose.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkReasonClose.Name;
                    csAddOrUpdate.VALUE = (chkReasonClose.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtReason.Text, 500))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng nhập lý do nhỏ hơn 500 ký tự.");
                    return;
                }
                bool success = false;
                CommonParam param = new CommonParam();
                if (this.sereServAdo != null)
                {
                    HisSereServAcceptNoExecuteSDO AcceptNoExecuteSDO = new HisSereServAcceptNoExecuteSDO();
                    AcceptNoExecuteSDO.SereServId = sereServAdo.ID;
                    AcceptNoExecuteSDO.WorkingRoomId = this.module.RoomId;
                    AcceptNoExecuteSDO.NoExecuteReason = txtReason.Text;
                    var result = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisSereServ/AcceptNoExecute", ApiConsumers.MosConsumer, AcceptNoExecuteSDO, param);
                    if (result != null)
                    {
                        success = true;
                        if (chkResonPrint.Checked)
                            ReasonPrint();
                        if (chkReasonClose.Checked)
                            this.Close();
                    }
                }
                else
                {

                    HisServiceReqAcceptNoExecuteSDO AcceptNoExecuteSDO = new HisServiceReqAcceptNoExecuteSDO();
                    AcceptNoExecuteSDO.ServiceReqId = this.serviceReqAdo.ID;
                    AcceptNoExecuteSDO.WorkingRoomId = this.module.RoomId;
                    AcceptNoExecuteSDO.NoExecuteReason = txtReason.Text;
                    var result = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisSereServ/AcceptNoExecuteByServiceReq", ApiConsumers.MosConsumer, AcceptNoExecuteSDO, param);
                    if (result != null)
                    {
                        success = true;
                        if (chkResonPrint.Checked)
                            ReasonPrint();
                        if (chkReasonClose.Checked)
                            this.Close();
                    }
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReasonPrint()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate("Mps000433", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000433":
                        InGiayDeNghiDoiTraDichVu(printTypeCode, fileName, ref result);
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

        private void InGiayDeNghiDoiTraDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.serviceReqAdo.ID > 0)
                {
                    V_HIS_SERVICE_REQ serviceReqChangePrint = GetServiceReqForPrint(this.serviceReqAdo.ID);
                    WaitingManager.Show();
                    List<HIS_SERE_SERV> dataSereServ = new List<HIS_SERE_SERV>();
                    V_HIS_TREATMENT treatment = new V_HIS_TREATMENT();

                    HisSereServFilter ssfilter = new HisSereServFilter();
                    ssfilter.SERVICE_REQ_ID = serviceReqChangePrint.ID;
                    ssfilter.ORDER_DIRECTION = "DESC";
                    var listSereServ = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (listSereServ != null && listSereServ.Count > 0)
                        dataSereServ = listSereServ.Where(o => o.IS_ACCEPTING_NO_EXECUTE == 1).ToList();

                    HisTreatmentViewFilter tmfilter = new HisTreatmentViewFilter();
                    tmfilter.ID = serviceReqChangePrint.TREATMENT_ID;
                    treatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, tmfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null).FirstOrDefault();

                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;
                    LoadCurrentPatientTypeAlter(serviceReqChangePrint.TREATMENT_ID, ref patientTypeAlter);

                    MPS.Processor.Mps000433.PDO.Mps000433PDO mps000433PDO = new MPS.Processor.Mps000433.PDO.Mps000433PDO(serviceReqChangePrint, listSereServ, treatment, patientTypeAlter);
                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.serviceReqAdo.TDL_TREATMENT_CODE, printTypeCode, this.module != null ? module.RoomId : 0);

                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000433PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000433PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrentPatientTypeAlter(long treatmentId, ref V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter)
        {
            try
            {
                CommonParam param = new CommonParam();
                hisPatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, treatmentId, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private V_HIS_SERVICE_REQ GetServiceReqForPrint(long serviceReqId)
        {
            V_HIS_SERVICE_REQ result = new V_HIS_SERVICE_REQ();
            try
            {
                if (serviceReqId > 0)
                {
                    HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                    serviceReqFilter.ID = serviceReqId;
                    serviceReqFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var serviceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (serviceReq != null && serviceReq.Count > 0)
                    {
                        result = serviceReq.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new V_HIS_SERVICE_REQ();
            }
            return result;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ReasonPrint();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                isSereServ = false;
                module = null;
                currentControlStateRDO = null;
                controlStateWorker = null;
                moduleLink = null;
                sereServAdo = null;
                serviceReqAdo = null;
                this.btnPrint.Click -= new System.EventHandler(this.btnPrint_Click);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.FormClosed -= new System.Windows.Forms.FormClosedEventHandler(this.frmReason_FormClosed);
                this.Load -= new System.EventHandler(this.frmReason_Load);
                emptySpaceItem1 = null;
                layoutControlItem5 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                layoutControlItem2 = null;
                chkResonPrint = null;
                chkReasonClose = null;
                btnSave = null;
                btnPrint = null;
                layoutControlItem1 = null;
                txtReason = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmReason_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // ReasonClose
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateReasonClose = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkReasonClose.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateReasonClose != null)
                {
                    csAddOrUpdateReasonClose.VALUE = (chkReasonClose.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdateReasonClose = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateReasonClose.KEY = chkReasonClose.Name;
                    csAddOrUpdateReasonClose.VALUE = (chkReasonClose.Checked ? "1" : "");
                    csAddOrUpdateReasonClose.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateReasonClose);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                //
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateResonPrint = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkResonPrint.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateResonPrint != null)
                {
                    csAddOrUpdateResonPrint.VALUE = (chkResonPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdateResonPrint = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateResonPrint.KEY = chkResonPrint.Name;
                    csAddOrUpdateResonPrint.VALUE = (chkResonPrint.Checked ? "1" : "");
                    csAddOrUpdateResonPrint.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateResonPrint);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
