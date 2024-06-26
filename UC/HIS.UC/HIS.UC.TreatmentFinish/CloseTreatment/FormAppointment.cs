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
using HIS.Desktop.Library.CacheClient;
using HIS.UC.TreatmentFinish.Resources;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
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

namespace HIS.UC.TreatmentFinish.CloseTreatment
{
    public partial class FormAppointment : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private TreatmentEndInputADO treatmentEndInputADO;
        private int positionHandle = -1;
        private MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO { get; set; }
        internal delegate void GetString(MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO);
        internal GetString MyGetData;
        long TreatmentEndAppointmentTimeDefault;
        bool TreatmentEndHasAppointmentTimeDefault;
        private bool isNotLoadWhileChangeControlStateInFirst;
        private ControlStateWorker controlStateWorker;
        private List<ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.UC.TreatmentFinish.FormAppointment";
        #endregion

        #region Construct
        public FormAppointment()
        {
            InitializeComponent();
        }

        public FormAppointment(TreatmentEndInputADO _treatmentEndInputADO)
            : this()
        {
            try
            {
                this.treatmentEndInputADO = _treatmentEndInputADO;
                this.TreatmentEndAppointmentTimeDefault = _treatmentEndInputADO.TreatmentEndAppointmentTimeDefault;
                this.TreatmentEndHasAppointmentTimeDefault = _treatmentEndInputADO.TreatmentEndHasAppointmentTimeDefault;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormAppointment_Load(object sender, EventArgs e)
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                InitControlState();
                SetIcon();

                LoadKeysFromlanguage();

                SetDefaultValueControl();

                ValidationTimeAppointments();
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                //layout
                Resources.ResourceLanguageManager.LanguageFormAppointment = new ResourceManager("HIS.UC.TreatmentFinish.Resources.Lang", typeof(FormAppointment).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageFormAppointment, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormAppointment.btnSave.Text", Resources.ResourceLanguageManager.LanguageFormAppointment, LanguageManager.GetCulture());
                this.lciTimeAppointments.Text = Inventec.Common.Resource.Get.Value("FormAppointment.lciTimeAppointments.Text", Resources.ResourceLanguageManager.LanguageFormAppointment, LanguageManager.GetCulture());
                this.lciTimeAppointments.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormAppointment.lciTimeAppointments.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageFormAppointment, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormAppointment.bar1.Text", Resources.ResourceLanguageManager.LanguageFormAppointment, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("FormAppointment.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageFormAppointment, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormAppointment.Text", Resources.ResourceLanguageManager.LanguageFormAppointment, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                currentTreatmentFinishSDO = treatmentEndInputADO.HisTreatmentFinishSDO;
                if (currentTreatmentFinishSDO.AppointmentTime.HasValue)
                {
                    DateTime? timeAppointment = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatmentFinishSDO.AppointmentTime.Value);
                    dtTimeAppointments.EditValue = timeAppointment;
                }
                else
                {
                    DateTime time = new DateTime();
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.TREATMENT_ID = currentTreatmentFinishSDO.TreatmentId;
                    filter.SERVICE_REQ_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "CREATE_TIME";
                    var prescriptionList = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (prescriptionList != null && prescriptionList.Count > 0)
                    {
                        var hisPrescription = prescriptionList.Where(o => o.USE_TIME_TO.HasValue).OrderByDescending(o => o.USE_TIME_TO).FirstOrDefault();
                        if (hisPrescription != null)
                        {
                            time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisPrescription.USE_TIME_TO.Value) ?? DateTime.MinValue;
                        }
                    }

                    long appointmentTimeDefaultCFG = TreatmentEndAppointmentTimeDefault;

                    if (TreatmentEndHasAppointmentTimeDefault)
                    {
                        if (time != null && time != DateTime.MinValue)
                        {
                            //if (time.DayOfWeek == DayOfWeek.Saturday)
                            //{
                            //    dtTimeAppointments.EditValue = time.AddDays(-1);
                            //}
                            //else if (time.DayOfWeek == DayOfWeek.Sunday)
                            //{
                            //    dtTimeAppointments.EditValue = time.AddDays(-2);
                            //}
                            //else
                            dtTimeAppointments.EditValue = time;
                        }
                        else if (appointmentTimeDefaultCFG > 0)
                        {
                            long appointmentTimeDefault = 0;
                            if (treatmentEndInputADO.AppointmentTime > 0)
                            {
                                appointmentTimeDefault = Inventec.Common.DateTime.Calculation.Add(treatmentEndInputADO.AppointmentTime, appointmentTimeDefaultCFG, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY) ?? 0;
                            }
                            else
                                appointmentTimeDefault = Inventec.Common.DateTime.Calculation.Add(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0, appointmentTimeDefaultCFG, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY) ?? 0;
                            dtTimeAppointments.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(appointmentTimeDefault);
                        }
                        else
                            dtTimeAppointments.EditValue = DateTime.Now.AddDays(1);
                    }
                    else
                    {
                        if (appointmentTimeDefaultCFG > 0)
                        {
                            long appointmentTimeDefault = 0;
                            if (treatmentEndInputADO.AppointmentTime > 0)
                            {
                                appointmentTimeDefault = Inventec.Common.DateTime.Calculation.Add(treatmentEndInputADO.AppointmentTime, appointmentTimeDefaultCFG, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY) ?? 0;
                            }
                            else
                                appointmentTimeDefault = Inventec.Common.DateTime.Calculation.Add(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0, appointmentTimeDefaultCFG, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY) ?? 0;
                            dtTimeAppointments.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(appointmentTimeDefault);
                        }
                        else if (time != null && time != DateTime.MinValue)
                        {
                            dtTimeAppointments.EditValue = time;
                        }
                        else
                            dtTimeAppointments.EditValue = DateTime.Now.AddDays(1);
                    }
                }
                dtTimeAppointments.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProviderControl.Validate())
                {
                    return;
                }

                if (!chkNotCheckT7CN.Checked)
                {
                    if (dtTimeAppointments.DateTime.DayOfWeek == DayOfWeek.Sunday)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoNgayHenLaChuNhat,
                        Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                    }
                    else if (dtTimeAppointments.DateTime.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoNgayHenLaThuBay,
                        Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                    }
                }

                long dtAppointmentTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeAppointments.DateTime) ?? 0;

                if (dtAppointmentTime >= treatmentEndInputADO.AppointmentTime)
                {
                    currentTreatmentFinishSDO.AppointmentTime = dtAppointmentTime;
                    MyGetData(currentTreatmentFinishSDO);
                    this.Close();
                }
                else
                {
                    MessageBox.Show(ResourceMessage.CanhBaoThoiGianHenKhamSoVoiThoiGianKetThucDieuTri);
                    dtTimeAppointments.Focus();
                    dtTimeAppointments.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Validation
        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationTimeAppointments()
        {
            try
            {
                CommonBaseEditor.ValidationSingleControl(this.dtTimeAppointments, this.dxValidationProviderControl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        #endregion

        #region Shotcut
        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
        #endregion

        private void chkNotCheckT7CN_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkNotCheckT7CN.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkNotCheckT7CN.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkNotCheckT7CN.Name;
                    csAddOrUpdate.VALUE = (chkNotCheckT7CN.Checked ? "1" : "");
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
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkNotCheckT7CN.Name)
                        {
                            chkNotCheckT7CN.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

    }
}
