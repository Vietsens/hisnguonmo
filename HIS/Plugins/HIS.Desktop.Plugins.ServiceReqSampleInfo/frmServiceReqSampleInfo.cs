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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ServiceReqSampleInfo.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceReqSampleInfo
{
    public partial class frmServiceReqSampleInfo : FormBase
    {
        long serviceReqId;
        HIS_SERVICE_REQ serviceReq;
        Inventec.Desktop.Common.Modules.Module currentModule;
        int positionHandle = -1;
        RefeshReference RefreshData = null;

        public frmServiceReqSampleInfo()
        {
            InitializeComponent();
        }
        public frmServiceReqSampleInfo(Inventec.Desktop.Common.Modules.Module module, long _ServiceReqId, RefeshReference _RefreshData)
            : base(module)
        {
            InitializeComponent();
            this.serviceReqId = _ServiceReqId;
            this.currentModule = module;
            this.RefreshData = _RefreshData;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

        }

        private void GetServiceRedById(long serviceReqId)
        {
            try
            {
                serviceReq = new HIS_SERVICE_REQ();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.ID = serviceReqId;
                serviceReq = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmServiceReqSampleInfo_Load(object sender, EventArgs e)
        {
            try
            {
                GetServiceRedById(this.serviceReqId);
                InitComboSampler();
                InitComboSampleType();
                FillDataDefaultToEditor(serviceReq);
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboSampleType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TEST_SAMPLE_TYPE_NAME", "ID", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboSampleType, BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboSampler()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "ID", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboSampler, BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataDefaultToEditor(HIS_SERVICE_REQ data)
        {
            try
            {
                txtBarcode.Text = data.BARCODE;
                lblServiceReqCode.Text = data.SERVICE_REQ_CODE;
                lblTreatmentCode.Text = data.TDL_TREATMENT_CODE;
                lblPatientName.Text = data.TDL_PATIENT_NAME;
                lblGender.Text = BackendDataWorker.Get<HIS_GENDER>().Where(o => o.ID == data.TDL_PATIENT_GENDER_ID).FirstOrDefault().GENDER_NAME;
                lblDob.Text = data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1 ? data.TDL_PATIENT_DOB.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                dtSampleTime.DateTime = DateTime.Now;
                cboSampler.EditValue = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).FirstOrDefault().ID;

                //xử lý tự động chọn loại mẫu bệnh phẩm
                if (!String.IsNullOrWhiteSpace(data.TDL_SERVICE_IDS))
                {
                    List<long> serviceIds = GetListIds(data.TDL_SERVICE_IDS) ?? new List<long>();
                    List<V_HIS_SERVICE> listService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => serviceIds.Contains(o.ID)).ToList() ?? new List<V_HIS_SERVICE>();
                    List<string> listSampleTypeCode = listService.Select(s => s.SAMPLE_TYPE_CODE ?? "").ToList() ?? new List<string>();
                    List<HIS_TEST_SAMPLE_TYPE> listTestSampleType = BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().Where(o => !String.IsNullOrWhiteSpace(o.TEST_SAMPLE_TYPE_CODE)
                                                                                                                        && listSampleTypeCode.Contains(o.TEST_SAMPLE_TYPE_CODE)).ToList();
                    if (listTestSampleType != null
                        && listTestSampleType.Count == 1
                        && cboSampleType.EditValue == null)
                    {
                        cboSampleType.EditValue = listTestSampleType[0].ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<long> GetListIds(string str)
        {
            List<long> result = new List<long>();
            try
            {
                if (String.IsNullOrWhiteSpace(str))
                    return result;
                var list = str.Split(';');
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        long id = 0;
                        if (Int64.TryParse(item, out id))
                            result.Add(id);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void txtSampler_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var user = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME == txtSampler.Text);
                    if (user == null || user.Count() == 1)
                    {
                        txtSampler.Text = user.FirstOrDefault().LOGINNAME;
                        cboSampler.EditValue = user.FirstOrDefault().ID;
                        txtSampleType.Focus();
                    }
                    else
                    {
                        cboSampler.Focus();
                        cboSampler.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtSampleType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var type = BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().Where(o => o.TEST_SAMPLE_TYPE_CODE == txtSampleType.Text);
                    if (type == null || type.Count() == 1)
                    {
                        cboSampleType.Text = type.FirstOrDefault().TEST_SAMPLE_TYPE_CODE;
                        cboSampleType.EditValue = type.FirstOrDefault().ID;
                        btnSave.Select();
                        btnSave.Focus();
                    }
                    else
                    {
                        cboSampleType.Focus();
                        cboSampleType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();

            try
            {
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (CheckIntructionTime())
                {
                    XtraMessageBox.Show(string.Format("Xử lý thất bại. Thời gian lấy mẫu không được nhỏ hơn thời gian y lệnh {0}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(serviceReq.INTRUCTION_TIME)), "Thông báo");
                    return;
                }
                bool success = false;
                WaitingManager.Show();
                ServiceReqSampleInfoSDO updateDTO = new ServiceReqSampleInfoSDO();
                UpdateDTOFromDataForm(ref updateDTO);

                var resultData = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisServiceReq/UpdateSampleInfo", ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {

                    success = true;
                    btnSave.Enabled = !success;
                }
                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool CheckIntructionTime()
        {
            bool rs = false;
            try
            {
                var sampleTime = Convert.ToInt64(dtSampleTime.DateTime.ToString("yyyyMMddHHmm00"));
                rs = this.serviceReq.INTRUCTION_TIME > sampleTime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        private void UpdateDTOFromDataForm(ref ServiceReqSampleInfoSDO currentDTO)
        {
            try
            {
                currentDTO.ServiceReqId = this.serviceReqId;
                currentDTO.ReqRoomId = this.currentModule.RoomId;
                currentDTO.SampleTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtSampleTime.DateTime);
                if (cboSampler.EditValue != null)
                {
                    var user = BackendDataWorker.Get<ACS_USER>().Where(o => o.ID == Convert.ToInt64(cboSampler.EditValue)).FirstOrDefault();
                    currentDTO.SamplerLoginname = user.LOGINNAME;
                    currentDTO.SamplerUsername = user.USERNAME;

                }
                else
                {
                    currentDTO.SamplerLoginname = null;
                    currentDTO.SamplerUsername = null;
                }
                currentDTO.TestSampleTypeId = cboSampleType.EditValue != null ? (long?)Convert.ToInt64(cboSampleType.EditValue) : null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                //ValidationSingleControl(dtSampleTime);
                ValidSampleTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidSampleTime()
        {
            try
            {
                SampleTimeValidationRule rule = new SampleTimeValidationRule();
                rule.dtSampleTime = dtSampleTime;
                dxValidationProvider1.SetValidationRule(dtSampleTime, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSampler_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit grd = sender as GridLookUpEdit;
                if (grd.EditValue != null)
                {
                    var sampler = BackendDataWorker.Get<ACS_USER>().Where(o => o.ID == Convert.ToInt64(grd.EditValue));
                    if (sampler != null && sampler.Count() > 0)
                    {
                        txtSampler.Text = sampler.FirstOrDefault().LOGINNAME;

                    }
                    else
                    {
                        txtSampler.Text = "";
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSampleType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit grd = sender as GridLookUpEdit;
                if (grd.EditValue != null)
                {
                    var sampleType = BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().Where(o => o.ID == Convert.ToInt64(grd.EditValue));
                    if (sampleType != null && sampleType.Count() > 0)
                    {
                        txtSampleType.Text = sampleType.FirstOrDefault().TEST_SAMPLE_TYPE_CODE;

                    }
                    else
                    {
                        txtSampleType.Text = "";
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmServiceReqSampleInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (RefreshData != null)
                    RefreshData();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtSampleTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtSampleTime.DateTime == DateTime.MinValue)
                {
                    dtSampleTime.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
