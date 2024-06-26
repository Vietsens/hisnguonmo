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
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
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

namespace HIS.Desktop.Plugins.UpdateVaccinationExam.Run
{
    public partial class frmUpdateVaccinationExam : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module moduleData;
        long _vaccinationExamId = 0;
        MOS.SDO.WorkPlaceSDO currentWorkPlace;
        DelegateSelectData _delegateSelectData = null;

        HIS_VACCINATION_EXAM _currentVaccinationExam = null;
        #endregion

        #region Construct
        public frmUpdateVaccinationExam()
        {
            InitializeComponent();
        }

        public frmUpdateVaccinationExam(Inventec.Desktop.Common.Modules.Module moduleData, long vaccinationExamId)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                this.moduleData = moduleData;
                this._vaccinationExamId = vaccinationExamId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmUpdateVaccinationExam(Inventec.Desktop.Common.Modules.Module moduleData, long vaccinationExamId, DelegateSelectData delegateSelectData)
            : this(moduleData, vaccinationExamId)
        {
            try
            {
                this._delegateSelectData = delegateSelectData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void frmUpdateVaccinationExam_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Public method
        public void MeShow()
        {
            try
            {
                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                SetDefaultValue();

                this._currentVaccinationExam = GetVaccinationExamByID(this._vaccinationExamId);

                InitFormData();

                if (this._currentVaccinationExam != null)
                {
                    FillDataToEditorControl(this._currentVaccinationExam);
                    lciVaccinationExamCode.Enabled = false;
                    txtVaccinationExamCode.ReadOnly = true;
                }

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        private HIS_VACCINATION_EXAM GetVaccinationExamByID(long id)
        {
            HIS_VACCINATION_EXAM result = null;
            try
            {
                CommonParam param = new CommonParam();
                if (id > 0)
                {
                    MOS.Filter.HisVaccinationExamFilter filter = new HisVaccinationExamFilter();
                    filter.ID = id;
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_VACCINATION_EXAM>>(HisRequestUriStore.MOS_HIS_VACCINATION_EXAM_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                return null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InitFormData()
        {
            try
            {
                InitComboRequestRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRequestRoom()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ID", columnInfos, false, 200);
                var dataSource = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_VACCINE == 1).ToList();
                ControlEditorLoader.Load(cboRequestRoom, dataSource, controlEditorADO);

                if (_currentVaccinationExam != null)
                {
                    var executeRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(o => o.ROOM_ID == _currentVaccinationExam.EXECUTE_ROOM_ID).ToList();
                    if (executeRoom != null)
                        cboRequestRoom.EditValue = executeRoom.FirstOrDefault().ID;
                }
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
                ResetFormData();
                dxValidationProviderEditorInfo.RemoveControlError(txtRequestRoom);
                dxValidationProviderEditorInfo.RemoveControlError(dtRequestTime);

                currentWorkPlace = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == currentModuleBase.RoomId && o.RoomTypeId == currentModuleBase.RoomTypeId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtVaccinationExamCode.Focus();
                txtVaccinationExamCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!layoutControlRoot.IsInitialized) return;
                layoutControlRoot.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlRoot.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is DevExpress.XtraEditors.BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtVaccinationExamCode.Focus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControlRoot.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_VACCINATION_EXAM data)
        {
            try
            {
                if (data != null)
                {
                    txtVaccinationExamCode.Text = data.VACCINATION_EXAM_CODE;
                    if (data.CONCLUDE == null)
                    {
                        lblConclude.Text = "Chưa kết luận";
                    }
                    else
                    {
                        if (data.CONCLUDE == 1)
                        {
                            lblConclude.Text = "Đủ điều kiện tiêm";
                        }
                        else if (data.CONCLUDE == 1)
                        {
                            lblConclude.Text = "Không đủ điều kiện tiêm";
                        }
                        else
                        {
                            lblConclude.Text = "";
                        }
                    }
                    lblTDLPatientName.Text = data.TDL_PATIENT_NAME;
                    lblTDLPatientDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    //cboRequestRoom.EditValue = data.REQUEST_ROOM_ID != 0 ? (long?)data.REQUEST_ROOM_ID : null;
                    var requestTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.REQUEST_TIME);
                    if (requestTime != null && requestTime != DateTime.MinValue)
                    {
                        dtRequestTime.DateTime = requestTime ?? new DateTime();
                    }
                    else
                    {
                        dtRequestTime.EditValue = null;
                    }

                }
                else
                {
                    txtVaccinationExamCode.Text = "";
                    lblConclude.Text = "";
                    lblTDLPatientName.Text = "";
                    lblTDLPatientDOB.Text = "";
                    //cboRequestRoom.EditValue = null;
                    dtRequestTime.EditValue = null;
                }
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
                ProcessSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSave()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                HisVaccinationExamSDO updateDTO = new HisVaccinationExamSDO();
                //MOS.EFMODEL.DataModels.HIS_VACCINATION_EXAM updateDTO = new MOS.EFMODEL.DataModels.HIS_VACCINATION_EXAM();

                if (this._currentVaccinationExam != null && this._vaccinationExamId > 0)
                {
                    LoadCurrent(this._vaccinationExamId, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);

                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("updateDTO_____", updateDTO));

                var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_VACCINATION_EXAM>("api/HisVaccinationExam/UpdateSdo", ApiConsumers.MosConsumer, updateDTO, param);
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("resultData_____", resultData));
                if (resultData != null)
                {
                    success = true;
                    FillDataToEditorControl(resultData);
                }

                if (success)
                {
                    SetDefaultFocus();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref HisVaccinationExamSDO updateDTO)
        {
            try
            {
                if (updateDTO.VaccinationExam == null)
                    updateDTO.VaccinationExam = new HIS_VACCINATION_EXAM();

                if (cboRequestRoom.EditValue != null)
                {
                    updateDTO.RequestRoomId = currentWorkPlace.RoomId;
                }
                if (dtRequestTime.EditValue != null && dtRequestTime.DateTime != DateTime.MinValue)
                {
                    updateDTO.VaccinationExam.REQUEST_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((DateTime?)dtRequestTime.DateTime) ?? 0;
                }
                updateDTO.VaccinationExam.REQUEST_ROOM_NAME = Convert.ToInt64(cboRequestRoom.EditValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrent(long id, ref HisVaccinationExamSDO updateDTO)
        {
            try
            {
                updateDTO.VaccinationExam = GetVaccinationExamByID(id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void txtRequestRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtRequestRoom.Text))
                    {
                        var requestRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(o => o.EXECUTE_ROOM_CODE != null && o.EXECUTE_ROOM_CODE.Trim().ToUpper() == txtRequestRoom.Text.Trim().ToUpper());
                        if (requestRoom != null)
                        {
                            cboRequestRoom.EditValue = requestRoom.FirstOrDefault().ID;
                            txtRequestRoom.Text = requestRoom.FirstOrDefault().EXECUTE_ROOM_CODE;
                        }
                        else
                        {
                            cboRequestRoom.ShowPopup();
                        }
                    }
                    else
                    {
                        cboRequestRoom.Focus();
                        cboRequestRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRequestRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboRequestRoom.EditValue != null && !string.IsNullOrEmpty(cboRequestRoom.EditValue.ToString()))
                {
                    var requestRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(o => o.ID == Convert.ToInt64(cboRequestRoom.EditValue)).FirstOrDefault();
                    txtRequestRoom.Text = requestRoom.EXECUTE_ROOM_CODE;
                }
                else
                {
                    txtRequestRoom.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
