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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.HisAssignBlood
{
    public partial class frmHisAssignBlood : HIS.Desktop.Utility.FormBase
    {

        //Load người chỉ định
        private void InitComboUser()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboUser, BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMediStock(long? patientTypeId)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "MEDI_STOCK_ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboMediStockExport_TabBlood, this.currentMestRoomByRooms, controlEditorADO);

                if (this.currentMestRoomByRooms != null && this.currentMestRoomByRooms.Count > 0)
                {
                    this.cboMediStockExport_TabBlood.EditValue = this.currentMestRoomByRooms[0].MEDI_STOCK_ID;             
                }
                else
                {
                    this.cboMediStockExport_TabBlood.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPatientType(GridLookUpEdit cboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRepositoryPatientType(List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(repositoryItemcboPatientType_TabBlood_GridLookUp, data, controlEditorADO);
                }
                else
                    ControlEditorLoader.Load(repositoryItemcboPatientType_TabBlood_GridLookUp, currentPatientTypeWithPatientTypeAlter, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBloodRH()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBloodRH, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBloodABO()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBloodABO, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRespositoryBloodRH()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemcboBloodRH, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRespositoryBloodABO()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemcboBloodABO, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Load nhóm xử lý
        private void InitComboExecuteGroup()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("EXECUTE_GROUP_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_GROUP_NAME", "ID", columnInfos, false, 250);
                var executeGroups = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP>().FindAll(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                ControlEditorLoader.Load(cboExecuteGroup_TabBlood, executeGroups, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
