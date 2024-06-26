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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.SummaryInforTreatmentRecordsUpdate
{
    public partial class frmSummaryInforTreatmentRecordsUpdate :  HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;

        internal MOS.SDO.HisTreatmentCommonInfoSDO currentTreatmentInfoSDO { get; set; }
        internal long TreatmentLogTypeId;
        internal long RecordId;
        internal MedirecordADO medirecordAdo;
        int ActionType = -1;
        int positionHandle = -1;
        public frmSummaryInforTreatmentRecordsUpdate(Inventec.Desktop.Common.Modules.Module _currentModule, MedirecordADO _medireCordAdo)
		:base(_currentModule)
        {
            InitializeComponent();
            this.currentModule = _currentModule;
            this.medirecordAdo = _medireCordAdo;
        }

        private void frmSummaryInforTreatmentRecordsUpdate_Load(object sender, EventArgs e)
        {
            WaitingManager.Show();
            SetIconFrm();
            if (this.medirecordAdo != null)
            {
                btnAdd.Enabled = false;
            }
            else 
            {
                btnSave.Enabled = false;
            }
            WaitingManager.Hide();
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
        private void InitComboStore()
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("DATA_STORE_CODE", "", 100, 1));
                //columnInfos.Add(new ColumnInfo("DATA_STORE_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("DATA_STORE_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboStore, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DATA_STORE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
        private void GetData()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMediRecordViewFilter recordFilter = new MOS.Filter.HisMediRecordViewFilter();
                var result = new BackendAdapter(param).Get<V_HIS_MEDI_RECORD>("api/HisMediRecord/GetView", ApiConsumers.MosConsumer, recordFilter, param);
                dtTime.EditValue = result.LOG_TIME;
                RecordId = result.ID;
                TreatmentLogTypeId = result.TREATMENT_LOG_TYPE_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnSave.Enabled && !btnAdd.Enabled)
                    return;
                positionHandle = -1;
                WaitingManager.Show();
                MOS.SDO.HisMediRecordSDO medirecordSdo = new HisMediRecordSDO();
                MOS.SDO.HisMediRecordSDO ResultmedirecordSdo = new HisMediRecordSDO();
                medirecordSdo.HisTreatmentLog = SetDataTreatmentLog();
                medirecordSdo.HisMediRecord = SetDataMedirecord();

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    ResultmedirecordSdo = new BackendAdapter(param)
                    .Post<MOS.SDO.HisMediRecordSDO>("api/HisMediRecord/Create", ApiConsumers.MosConsumer, medirecordSdo, param);
                }
                else
                {
                    medirecordSdo.HisTreatmentLog.TREATMENT_LOG_TYPE_ID = TreatmentLogTypeId;
                    medirecordSdo.HisMediRecord.ID = RecordId;
                    ResultmedirecordSdo = new BackendAdapter(param)
                        .Post<HisMediRecordSDO>("api/HisMediRecord/Update", ApiConsumers.MosConsumer, medirecordSdo, param);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
