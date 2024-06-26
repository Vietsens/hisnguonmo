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
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.VaccinationExam.ADO;
using Inventec.Common.Controls.EditorLoader;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using DevExpress.XtraEditors;

namespace HIS.Desktop.Plugins.VaccinationExam
{
    public partial class UCVaccinationExam : UserControlBase
    {
        private void LoadDataToCombo()
        {
            try
            {
                LoadPatientTypeToCombo();
                LoadAcsUserToCombo();
                LoadExecuteRoomToCombo();
                LoadVaccineTypesCombo();
                LoadComboSpecialistDepartment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboSpecialistDepartment()
        {
            try
            {
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().Where(o=>o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboSpecialistDepartment, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMedicineByMediStock(long? mediStockId)
        {
            try
            {
                List<D_HIS_MEDI_STOCK_1> mediStocks = null;
                if (mediStockId.HasValue)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    MOS.Filter.DHisMediStock1Filter filter = new MOS.Filter.DHisMediStock1Filter();
                    filter.MEDI_STOCK_IDs = new List<long> { mediStockId.Value };
                    filter.IS_VACCINE = true;
                    mediStocks = new BackendAdapter(param).Get<List<D_HIS_MEDI_STOCK_1>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, param);
                    WaitingManager.Hide();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "Mã thuốc", 75, 1, true));
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "Tên thuốc", 200, 2, true));
                columnInfos.Add(new ColumnInfo("AMOUNT", "Số lượng", 75, 3, true));
                columnInfos.Add(new ColumnInfo("CONCENTRA", "Hàm lượng", 75, 4, true));
                columnInfos.Add(new ColumnInfo("ACTIVE_INGR_BHYT_NAME", "Hoạt chất", 75, 5, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, true, 500);
                ControlEditorLoader.Load(repositoryItemCustomGridLookUpMedicineType, mediStocks, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientTypeToCombo()
        {
            try
            {
                List<HIS_PATIENT_TYPE> patientTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>()
                    .Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboPatientType, patientTypes, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadAcsUserToCombo()
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cboAcsUser, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadExecuteRoomToCombo()
        {
            try
            {
                var mestRoomTemps = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == this.roomId).ToList();
                var mediStockId__Actives = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == null || o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => o.ID).ToList();
                mestRoomTemps = mestRoomTemps.Where(o => mediStockId__Actives != null && mediStockId__Actives.Contains(o.MEDI_STOCK_ID))
                    .ToList();
                mestRooms = (from r in mestRoomTemps
                             select
                               new V_HIS_MEST_ROOM
                               {
                                   MEDI_STOCK_CODE = r.MEDI_STOCK_CODE,
                                   MEDI_STOCK_ID = r.MEDI_STOCK_ID,
                                   MEDI_STOCK_NAME = r.MEDI_STOCK_NAME
                               }).Distinct().ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "Mã kho", 150, 1, true));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "Tên kho", 250, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "MEDI_STOCK_ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboMediStockName, mestRoomTemps, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMediStock(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMediStockName.Focus();
                    cboMediStockName.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.MEDI_STOCK_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboMediStockName.EditValue = data[0].MEDI_STOCK_ID;
                            cboMediStockName.Properties.Buttons[1].Visible = true;
                            cboPatientType.Focus();
                            cboPatientType.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.MEDI_STOCK_CODE.ToLower() == searchCode.ToLower());
                            if (search != null)
                            {
                                cboMediStockName.EditValue = search.MEDI_STOCK_ID;
                                cboMediStockName.Properties.Buttons[1].Visible = true;
                                cboPatientType.Focus();
                                cboPatientType.SelectAll();
                            }
                            else
                            {
                                cboMediStockName.EditValue = null;
                                cboMediStockName.Focus();
                                cboMediStockName.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboMediStockName.EditValue = null;
                        cboMediStockName.Focus();
                        cboMediStockName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadAcsUser(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboAcsUser.Focus();
                    cboAcsUser.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME.ToLower().Contains(searchCode.ToLower())).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboAcsUser.EditValue = data[0].LOGINNAME;
                            cboAcsUser.Properties.Buttons[1].Visible = true;
                            btnSave.Focus();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.LOGINNAME == searchCode);
                            if (search != null)
                            {
                                cboAcsUser.EditValue = search.LOGINNAME;
                                cboAcsUser.Properties.Buttons[1].Visible = true;
                                btnSave.Focus();
                            }
                            else
                            {
                                cboAcsUser.EditValue = null;
                                cboAcsUser.Focus();
                                cboAcsUser.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboAcsUser.EditValue = null;
                        cboAcsUser.Focus();
                        cboAcsUser.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadVaccineTypesCombo()
        {
            try
            {
                this.VaccineTypes = BackendDataWorker.Get<HIS_VACCINE_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("VACCINE_TYPE_CODE", "Mã", 100, 1, true));
                columnInfos.Add(new ColumnInfo("VACCINE_TYPE_NAME", "Tên", 400, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("VACCINE_TYPE_NAME", "ID", columnInfos, true, 500);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(repositoryItemCboVaccineType, this.VaccineTypes, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSpecialistDepartment(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboSpecialistDepartment.Focus();
                    cboSpecialistDepartment.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o =>o.DEPARTMENT_CODE != null && o.DEPARTMENT_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            txtSpecialistDepartment.Text = data[0].DEPARTMENT_CODE;
                            cboSpecialistDepartment.EditValue = data[0].ID;
                        }
                        else
                        {
                            var rs = data.FirstOrDefault(m => m.DEPARTMENT_CODE == searchCode);
                            if (rs != null)
                            {
                                txtSpecialistDepartment.Text = rs.DEPARTMENT_CODE;
                                cboSpecialistDepartment.EditValue = rs.ID;
                            }
                            else
                            {
                                cboSpecialistDepartment.EditValue = null;
                                cboSpecialistDepartment.Focus();
                                cboSpecialistDepartment.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboSpecialistDepartment.EditValue = null;
                        cboSpecialistDepartment.Focus();
                        cboSpecialistDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
