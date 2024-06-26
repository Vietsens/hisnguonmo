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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentLockFee
{
    public partial class frmTreatmentLockFee : HIS.Desktop.Utility.FormBase
    {
        HIS_TREATMENT treatment = null;
        long treatmentId;
        const decimal MinRound = (decimal)0.0001;
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool isNotLoadWhileChangeControlStateInFirst;

        public frmTreatmentLockFee(Inventec.Desktop.Common.Modules.Module module, long data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                treatmentId = data;
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmFeeLock_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeyFrmLanguage();
                InitControlState();
                dtFeeLockTime.DateTime = DateTime.Now;
                bool enable = false;
                if (treatmentId > 0)
                {
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = treatmentId;
                    var lstTreat = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, null);
                    if (lstTreat != null && lstTreat.Count == 1)
                    {
                        this.treatment = lstTreat.First();
                        if (this.treatment.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            enable = true;
                        }
                    }
                }
                dtFeeLockTime.Enabled = HisTreatmentFeeLockTimeCFG.IsEnableControlFeeLockTime;
                btnSave.Enabled = enable;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(currentModule.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPrintPrescription.Name)
                        {
                            chkPrintPrescription.Checked = item.VALUE == "1";
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
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
                if (!btnSave.Enabled || this.treatment == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                HisTreatmentFeeViewFilter _feeFilter = new HisTreatmentFeeViewFilter();
                _feeFilter.ID = treatmentId;
                var lstTreatFee = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, _feeFilter, param);
                if (lstTreatFee != null && lstTreatFee.Count > 0)
                {
                    V_HIS_TREATMENT_FEE dataFee = lstTreatFee.FirstOrDefault();
                    decimal totalReceive = ((dataFee.TOTAL_DEPOSIT_AMOUNT ?? 0) + (dataFee.TOTAL_BILL_AMOUNT ?? 0) - (dataFee.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (dataFee.TOTAL_BILL_FUND ?? 0) - (dataFee.TOTAL_REPAY_AMOUNT ?? 0)) - (dataFee.TOTAL_BILL_EXEMPTION ?? 0);

                    decimal totalReceiveMore = (dataFee.TOTAL_PATIENT_PRICE ?? 0) - totalReceive - (dataFee.TOTAL_BILL_FUND ?? 0) - (dataFee.TOTAL_BILL_EXEMPTION ?? 0);

                    if (totalReceiveMore > MinRound)
                    {
                        WaitingManager.Hide();
                        string price = Inventec.Common.Number.Convert.NumberToString(totalReceiveMore, ConfigApplications.NumberSeperator);
                        string cauHoi = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LOCK_FEE__BAN_CO_MUON_CHAC_KHOA_VIEN_PHI", Base.ResourceLangManager.LanguageFrmTreatmentLockFee, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        string thongBao = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LOCK_FEE__THONG_BAO", Base.ResourceLangManager.LanguageFrmTreatmentLockFee, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());//"Thông báo"

                        string mess = "Bệnh nhân đang thiếu " + "<color=red>" + price + "</color>  đ viện phí.\n" + cauHoi;
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(mess, thongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                        {
                            return;
                        }
                        WaitingManager.Show();
                    }
                    else if (totalReceiveMore < -MinRound)
                    {
                        WaitingManager.Hide();
                        string price = Inventec.Common.Number.Convert.NumberToString(Math.Abs(totalReceiveMore), ConfigApplications.NumberSeperator);
                        string cauHoi = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LOCK_FEE__BAN_CO_MUON_CHAC_KHOA_VIEN_PHI", Base.ResourceLangManager.LanguageFrmTreatmentLockFee, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        string thongBao = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LOCK_FEE__THONG_BAO", Base.ResourceLangManager.LanguageFrmTreatmentLockFee, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());//"Thông báo"

                        string mess = "Bệnh nhân đang thừa " + "<color=blue>" + price + "</color>  đ viện phí.\n" + cauHoi;
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(mess, thongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                        {
                            return;
                        }
                        WaitingManager.Show();
                    }
                }

                HisTreatmentLockSDO sdo = new HisTreatmentLockSDO();
                sdo.TreatmentId = this.treatment.ID;
                if (dtFeeLockTime.EditValue != null && dtFeeLockTime.DateTime != DateTime.MinValue)
                {
                    sdo.FeeLockTime = Convert.ToInt64(dtFeeLockTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                if (this.currentModule != null)
                {
                    sdo.RequestRoomId = this.currentModule.RoomId;
                }
                if (this.treatment.IS_LOCK_FEE != 1)
                {
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/Lock", ApiConsumers.MosConsumer, sdo, param);
                    if (rs != null)
                    {
                        success = true;
                        if (chkPrintPrescription.Checked)
                        {
                            PrintPrescription();
                        }
                    }
                }
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this, param, success);
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                this.layoutFeeLockTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LAYOUT_FEE_LOCK_TIME", Base.ResourceLangManager.LanguageFrmTreatmentLockFee, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LOCK_FEE__BTN_SAVE", Base.ResourceLangManager.LanguageFrmTreatmentLockFee, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutFeeLockTime.Text = Inventec.Common.Resource.Get.Value("frmTreatmentLockFee.layoutFeeLockTime.Text", Base.ResourceLangManager.LanguageFrmTreatmentLockFee, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmTreatmentLockFee.bar1.Text", Base.ResourceLangManager.LanguageFrmTreatmentLockFee, LanguageManager.GetCulture());
                this.bbtnRCSave.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLockFee.bbtnRCSave.Caption", Base.ResourceLangManager.LanguageFrmTreatmentLockFee, LanguageManager.GetCulture());
                this.chkPrintPrescription.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLockFee.chkPrintPrescription.Properties.Caption", Base.ResourceLangManager.LanguageFrmTreatmentLockFee, LanguageManager.GetCulture());
                this.chkPrintPrescription.ToolTip = Inventec.Common.Resource.Get.Value("frmTreatmentLockFee.chkPrintPrescription.ToolTip", Base.ResourceLangManager.LanguageFrmTreatmentLockFee, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmTreatmentLockFee.Text", Base.ResourceLangManager.LanguageFrmTreatmentLockFee, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPrescription()
        {
            try
            {
                var param = new CommonParam();
                HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                reqFilter.TREATMENT_ID = this.treatmentId;
                reqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK;
                var listServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, reqFilter, param);
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    MOS.SDO.OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();

                    outPatientPresResultSDO.ServiceReqs = listServiceReq;

                    //Get ServiceReqMety
                    HisServiceReqMetyFilter hisServiceReqMetyFilter = new HisServiceReqMetyFilter();
                    hisServiceReqMetyFilter.SERVICE_REQ_IDs = listServiceReq.Select(o => o.ID).ToList();
                    var listHisServiceReqMety = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, hisServiceReqMetyFilter, param);
                    outPatientPresResultSDO.ServiceReqMeties = listHisServiceReqMety;

                    //Get ServiceReqMaty
                    HisServiceReqMatyFilter hisServiceReqMatyFilter = new HisServiceReqMatyFilter();
                    hisServiceReqMatyFilter.SERVICE_REQ_IDs = listServiceReq.Select(o => o.ID).ToList();
                    var listHisServiceReqMaty = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, hisServiceReqMatyFilter, param);
                    outPatientPresResultSDO.ServiceReqMaties = listHisServiceReqMaty;

                    //Get ExpMest
                    HisExpMestFilter hisExpMestFilter = new HisExpMestFilter();
                    hisExpMestFilter.SERVICE_REQ_IDs = listServiceReq.Select(o => o.ID).ToList();
                    var listHisExpMest = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, hisExpMestFilter, param);
                    outPatientPresResultSDO.ExpMests = listHisExpMest;

                    //Get ExpMestMedicine
                    HisExpMestMedicineFilter hisExpMestMedicineFilter = new HisExpMestMedicineFilter();
                    hisExpMestMedicineFilter.TDL_SERVICE_REQ_IDs = listServiceReq.Select(o => o.ID).ToList();
                    var listHisExpMestMedicine = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, hisExpMestMedicineFilter, param);
                    outPatientPresResultSDO.Medicines = listHisExpMestMedicine;

                    //Get ExpMestMaterial
                    HisExpMestMaterialFilter hisExpMestMaterialFilter = new HisExpMestMaterialFilter();
                    hisExpMestMaterialFilter.TDL_SERVICE_REQ_IDs = listServiceReq.Select(o => o.ID).ToList();
                    var listHisExpMestMaterial = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, hisExpMestMaterialFilter, param);
                    outPatientPresResultSDO.Materials = listHisExpMestMaterial;

                    List<MOS.SDO.OutPatientPresResultSDO> listOutPatientPresResultSDO = new List<OutPatientPresResultSDO> { outPatientPresResultSDO };

                    var PrintPresProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(listOutPatientPresResultSDO, this.currentModule);
                    PrintPresProcessor.Print("Mps000234", true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrintPrescription_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintPrescription.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintPrescription.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintPrescription.Name;
                    csAddOrUpdate.VALUE = (chkPrintPrescription.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
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
    }
}
