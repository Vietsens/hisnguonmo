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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PublicMedicineByDate.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PublicMedicineByDate
{
    public partial class frmPublicMedicinesByDate : HIS.Desktop.Utility.FormBase
    {
        internal long _treatmentId;
        internal L_HIS_TREATMENT_BED_ROOM _TreatmentBedRoom { get; set; }
        Inventec.Desktop.Common.Modules.Module currentModule;
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        internal string _MedicalInstruction = "";
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.PublicMedicineByDate";

        public frmPublicMedicinesByDate()
        {
            InitializeComponent();
        }

        public frmPublicMedicinesByDate(Inventec.Desktop.Common.Modules.Module currentModule,
            L_HIS_TREATMENT_BED_ROOM curentTreatment)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            this._TreatmentBedRoom = curentTreatment;
            this._treatmentId = curentTreatment.TREATMENT_ID;
        }

        public frmPublicMedicinesByDate(Inventec.Desktop.Common.Modules.Module currentModule,
            long treatmentId)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            this._treatmentId = treatmentId;
        }

        private void frmPublicMedicinesByDate_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                dtDatePublic.EditValue = DateTime.Now;
                tmIntructionTimeFrom.EditValue = null;
                tmIntructionTimeTo.EditValue = null;

                InitControlState();
                if (chkSign.Checked == false)
                {
                    chkPrintDocumentSigned.Enabled = false;
                    chkPrintDocumentSigned.Checked = false;
                }
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                var currentDepartmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                var department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == currentDepartmentId);
                if (department != null)
                {
                    rdoRequestDepatment__Current.Text = department.DEPARTMENT_NAME;
                    rdoRequestDepatment__Current.CheckState = CheckState.Checked;
                }
                //chkSortByName.CheckState = CheckState.Checked;
                WaitingManager.Hide();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.PublicMedicineByDate.Resources.Lang", typeof(HIS.Desktop.Plugins.PublicMedicineByDate.frmPublicMedicinesByDate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicinesByDate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPublicByDate.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicinesByDate.btnPublicByDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDatePublic.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicinesByDate.lciDatePublic.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicinesByDate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemTao.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicinesByDate.barButtonItemTao.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMedicine.Properties.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicinesByDate.chkMedicine.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicinesByDate.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKy.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicinesByDate.lciKy.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPrintCheck.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicinesByDate.lciPrintCheck.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMaterial.Properties.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicinesByDate.chkMaterial.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicinesByDate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicinesByDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private bool GetAllSereServV2()
        {
            bool result = true;
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                this._MedicalInstruction = "";
                //1.Get ServiceReq là đơn nt và tt, theo khoa yc
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = this._treatmentId;
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long>();
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT);

                if (rdoRequestDepatment__Current.Checked)
                {
                    serviceReqFilter.REQUEST_DEPARTMENT_ID = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                }

                if (dtDatePublic.EditValue != null && dtDatePublic.DateTime != DateTime.MinValue)
                {
                    if (tmIntructionTimeFrom.EditValue != null)
                    {
                        serviceReqFilter.USE_TIME_OR_INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtDatePublic.EditValue ?? "0").ToString()).ToString("yyyyMMdd") + String.Format("{0:00}{1:00}00", tmIntructionTimeFrom.TimeSpan.Hours, tmIntructionTimeFrom.TimeSpan.Minutes));
                    }
                    else
                    {
                        serviceReqFilter.USE_TIME_OR_INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtDatePublic.EditValue ?? "0").ToString()).ToString("yyyyMMdd") + "000000");
                    }

                    if (tmIntructionTimeTo.EditValue != null)
                    {
                        serviceReqFilter.USE_TIME_OR_INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtDatePublic.EditValue ?? "0").ToString()).ToString("yyyyMMdd") + String.Format("{0:00}{1:00}00", tmIntructionTimeTo.TimeSpan.Hours, tmIntructionTimeTo.TimeSpan.Minutes));
                    }
                    else
                    {
                        serviceReqFilter.USE_TIME_OR_INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtDatePublic.EditValue ?? "0").ToString()).ToString("yyyyMMdd") + "235959");
                    }
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqFilter), serviceReqFilter));

                var _currentServiceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param);

                List<long> _expMestIds = new List<long>();
                List<long> _serviceReqIds = new List<long>();
                if (_currentServiceReqs != null && _currentServiceReqs.Count > 0)
                {
                    //2.Get ExpMest có SERVICE_REQ_ID là đơn thuốc
                    _serviceReqIds = _currentServiceReqs.Select(p => p.ID).ToList();

                    //Lay thong tin to dieu tri
                    MOS.Filter.HisTrackingFilter trackingFilter = new HisTrackingFilter();
                    trackingFilter.IDs = _currentServiceReqs.Select(p => p.TRACKING_ID ?? 0).ToList();
                    var _Trackings = new BackendAdapter(param).Get<List<HIS_TRACKING>>(HisRequestUriStore.HIS_TRACKING_GET, ApiConsumers.MosConsumer, trackingFilter, param);
                    if (_Trackings != null && _Trackings.Count > 0)
                    {
                        foreach (var itemTr in _Trackings)
                        {
                            this._MedicalInstruction += itemTr.MEDICAL_INSTRUCTION + "; ";
                        }
                    }

                    MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                    expMestFilter.SERVICE_REQ_IDs = _serviceReqIds;
                    var _currentExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                    if (_currentExpMests != null && _currentExpMests.Count > 0)
                    {
                        if (rdoRequestDepatment__Current.Checked)
                        {
                            _expMestIds = _currentExpMests.Where(p => p.REQ_DEPARTMENT_ID == WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId).DepartmentId).Select(p => p.ID).ToList();
                        }
                        else
                        {
                            _expMestIds = _currentExpMests.Select(p => p.ID).ToList();
                        }
                    }

                    WaitingManager.Hide();
                }
                else
                {
                    WaitingManager.Hide();

                    result = false;
                    return result;
                }

                CreateThreadLoadData(_expMestIds);

                CreateThreadLoadDataNgoaiKho(_serviceReqIds);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
                result = false;
            }
            return result;
        }

        void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {

                        if (item.KEY == ControlStateConstan.chkSign)
                        {
                            chkSign.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.chkPrintDocumentSigned)
                        {
                            chkPrintDocumentSigned.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == Config.ControlStateConstan.chkAccordingToSetting)
                        {
                            chkAccordingToSetting.Checked = item.VALUE == "1";
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
                //if (!chkAccordingToSetting.Checked)
                //{
                //    chkSortByName.Checked = true;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPublicByDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtDatePublic.EditValue != null)
                {
                    PrintProcess(PrintType.PHIEU_CONG_KHAI_THUOC_THEO_NGAY);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long TimeNumberToDateNumber(long? timeNumber)
        {
            long result = 0;
            try
            {
                string dateNumberString = timeNumber.ToString().Substring(0, 8);
                result = Inventec.Common.TypeConvert.Parse.ToInt64(dateNumberString);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void barButtonItemTao_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPublicByDate.Focus();
                btnPublicByDate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkKy_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkPrintDocumentSigned.Enabled = chkSign.Checked;
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                if (chkSign.Checked == false)
                {
                    chkPrintDocumentSigned.Checked = false;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkSign && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkSign;
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintDocumentSigned_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkPrintDocumentSigned && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkPrintDocumentSigned;
                    csAddOrUpdate.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoRequestDepatment__Current_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoRequestDepatment__Current.Checked)
                {
                    rdoRequestDepatment__All.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoRequestDepatment__All_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoRequestDepatment__All.Checked)
                {
                    rdoRequestDepatment__Current.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tmIntructionTimeFrom_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    tmIntructionTimeFrom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tmIntructionTimeTo_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    tmIntructionTimeTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void chkAccordingToSetting_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAccordingToSetting.Checked)
                {
                    chkSortByName.Checked = false;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == Config.ControlStateConstan.chkAccordingToSetting && o.MODULE_LINK == Config.ControlStateConstan.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAccordingToSetting.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = Config.ControlStateConstan.chkAccordingToSetting;
                    csAddOrUpdate.VALUE = (chkAccordingToSetting.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = Config.ControlStateConstan.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSortByName_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkSortByName.Checked)
                {
                    chkAccordingToSetting.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
