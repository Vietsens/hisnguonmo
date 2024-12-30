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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AggrExpMestPrintFilter.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrExpMestPrintFilter
{
    internal partial class frmAggregateExpMestPrintFilter : HIS.Desktop.Utility.FormBase
    {
        #region varialbe declare
        internal List<V_HIS_ROOM> RoomDTO2s = new List<V_HIS_ROOM>();
        internal List<V_HIS_ROOM> RoomDTO3s = new List<V_HIS_ROOM>();
        internal List<HIS_SERVICE_UNIT> ServiceUnits = new List<HIS_SERVICE_UNIT>();
        internal List<HIS_MEDICINE_USE_FORM> MedicineUseForms = new List<HIS_MEDICINE_USE_FORM>();

        internal List<V_HIS_EXP_MEST> _AggrExpMests = new List<V_HIS_EXP_MEST>();

        internal V_HIS_EXP_MEST aggrExpMest;
        internal HIS_DEPARTMENT department;
        internal Inventec.Desktop.Common.Modules.Module currrentModule;
        internal long departmentId = 0;
        internal List<long> serviceUnitIds = new List<long>();
        internal List<long> useFormIds = new List<long>();
        internal List<long> reqRoomIds = new List<long>();
        internal long printType;
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool isPrint = false;
        internal long timeType = 1;
        #endregion

        #region contructor
        internal frmAggregateExpMestPrintFilter(Inventec.Desktop.Common.Modules.Module currrentModule, V_HIS_EXP_MEST aggrExpMest, long printType, Desktop.ADO.AggrExpMestPrintSDO printSdo)
            : base(currrentModule)
        {
            try
            {
                InitializeComponent();
                this.aggrExpMest = aggrExpMest;
                this.printType = printType;
                this.currrentModule = currrentModule;
                departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == currrentModule.RoomId).DepartmentId;
                InitControl();
                if (printSdo != null)
                {
                    this.chkPrintNow.Checked = printSdo.PrintNow ?? false;
                    if (printSdo.PrintNow == true)
                    {
                        isPrint = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal frmAggregateExpMestPrintFilter(Inventec.Desktop.Common.Modules.Module currrentModule, List<V_HIS_EXP_MEST> _aggrExpMests, long printType, Desktop.ADO.AggrExpMestPrintSDO printSdo)
            : base(currrentModule)
        {
            try
            {
                InitializeComponent();
                this._AggrExpMests = _aggrExpMests;

                if (_aggrExpMests != null && _aggrExpMests.Count > 0)
                {
                    this.aggrExpMest = _aggrExpMests.FirstOrDefault();
                }

                this.printType = printType;
                this.currrentModule = currrentModule;
                departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == currrentModule.RoomId).DepartmentId;
                InitControl();
                if (printSdo != null)
                {
                    this.chkPrintNow.Checked = printSdo.PrintNow ?? false;
                    if (printSdo.PrintNow == true)
                    {
                        isPrint = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Load
        private void frmRequestPatientReport_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                FillDataToForm();
                InitControlState();
                InitCombobox();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombobox()
        {
            try
            {
                List<FilterTimeADO> lstTime = new List<FilterTimeADO>
                    {
                        new FilterTimeADO { Id = (long)1, TimeName = "Thời gian chỉ định" },
                        new FilterTimeADO { Id = (long)2, TimeName = "Thời gian dự trù" },
                    };
                cboChooseTime.Properties.DataSource = lstTime;
                cboChooseTime.Properties.DisplayMember = "TimeName";
                cboChooseTime.Properties.ValueMember = "Id";
                cboChooseTime.Properties.ImmediatePopup = true;
                cboChooseTime.Properties.View.OptionsView.ShowColumnHeaders = false;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboChooseTime.Properties.View.Columns.AddField("TimeName");
                col2.VisibleIndex = 1;
                col2.Width = 200;
                //col2.Caption = "Tất cả";
                cboChooseTime.Properties.PopupFormWidth = 200;
                cboChooseTime.Properties.PopupFormSize = new Size(200, 100);
                cboChooseTime.EditValue = (long)1;
                //cboChooseTime.Text = "Thời gian chỉ định";
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
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
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

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToForm()
        {
            try
            {
                WaitingManager.Show();
                RoomDTO2s.Clear();
                grdRoom.DataSource = null;
                if (this.printType == 6)
                {
                    LoadDataReqMetyMaty(this.aggrExpMest);
                }
                else
                {
                    if (this.printType == 5 || this.printType == 7)
                    {
                        LoadDataMedicineAndMaterial_(this._AggrExpMests);
                    }
                    else 
                    {
                        LoadDataMedicineAndMaterial(this.aggrExpMest);
                    }
                }
                CommonParam param = new CommonParam();
                reqRoomIds = reqRoomIds.Distinct().ToList();
                RoomDTO2s = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.DEPARTMENT_ID == departmentId && reqRoomIds.Contains(o.ID)).ToList();
                grdRoom.DataSource = RoomDTO2s;
                gridViewRoom.SelectAll();

                serviceUnitIds = serviceUnitIds.Distinct().ToList();
                useFormIds = useFormIds.Distinct().ToList();
                LoadDataToGridControlServiceUnit(serviceUnitIds);
                LoadDataToGridControlUseForm(useFormIds);

                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Info("this.printType: " + this.printType + " ___"+this.isPrint);
                if (this.printType == 3 || isPrint)
                {
                    ProcessPrint();
                    this.Close();
                }
               
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Events control

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSendRequest_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSendRequest_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessPrint();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView3_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                RoomDTO3s.Clear();
                int[] rows = gridViewRoom.GetSelectedRows();
                for (int i = 0; i < rows.Length; i++)
                {
                    RoomDTO3s.Add((MOS.EFMODEL.DataModels.V_HIS_ROOM)gridViewRoom.GetRow(rows[i]));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void gridViewServiceUnit_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                ServiceUnits.Clear();
                int[] rows = gridViewServiceUnit.GetSelectedRows();
                for (int i = 0; i < rows.Length; i++)
                {
                    ServiceUnits.Add((MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT)gridViewServiceUnit.GetRow(rows[i]));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewUseForm_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

            try
            {
                MedicineUseForms.Clear();
                int[] rows = gridViewUseForm.GetSelectedRows();
                for (int i = 0; i < rows.Length; i++)
                {
                    MedicineUseForms.Add((MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM)gridViewUseForm.GetRow(rows[i]));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSendRequest_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrintNow_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintNow.Name && o.MODULE_LINK == currrentModule.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintNow.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintNow.Name;
                    csAddOrUpdate.VALUE = (chkPrintNow.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = currrentModule.ModuleLink;
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
                this.controlStateWorker = new Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(currrentModule.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPrintNow.Name)
                        {
                            chkPrintNow.Checked = item.VALUE == "1";
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

		private void frmAggregateExpMestPrintFilter_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
                this.Dispose(true);
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}
	}
}
