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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.Prepare.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
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

namespace HIS.Desktop.Plugins.Prepare
{
    public partial class frmPrepare : FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        HIS_TREATMENT treatment { get; set; }
        HIS_PREPARE prepare { get; set; }
        PEnum.ACTION_TYPE actionType { get; set; }
        int positionHandleControlLeft = -1;
        List<V_HIS_PREPARE_METY> prepareMetybyTreatments { get; set; }
        List<V_HIS_PREPARE_MATY> prepareMatybyTreatments { get; set; }
        DelegateRefreshData refesshData;

        public frmPrepare(Inventec.Desktop.Common.Modules.Module _module, HIS_TREATMENT _treatment)
        {
            InitializeComponent();
            try
            {
                this.treatment = _treatment;
                this.currentModule = _module;
                this.actionType = PEnum.ACTION_TYPE.CREATE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public frmPrepare(Inventec.Desktop.Common.Modules.Module _module, HIS_PREPARE _prepare, DelegateRefreshData _refeshData)
        {
            InitializeComponent();
            try
            {
                this.prepare = _prepare;
                this.currentModule = _module;
                this.actionType = PEnum.ACTION_TYPE.UPDATE;
                this.refesshData = _refeshData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmPrepare()
        {
            InitializeComponent();
        }

        private void frmPrepare_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                ValidateControl();
                InitDataInput();
                LoadMediMatyToCbo();
                InitPrepareMetyMatyGrid();
                LoadPrepareMetyByTreatment();
                LoadPrepareMatyByTreatment();
                LoadPrepareEdit();
                LoadPrepareMetyMatyLog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPrepareMety_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                PrepareMetyMatyADO data = gridViewPrepareMety.GetFocusedRow() as PrepareMetyMatyADO;
                if (data == null) return;
                if (e.Column.FieldName == "METY_MATY_TYPE_ID")
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewPrepareMety_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                PrepareMetyMatyADO data = view.GetFocusedRow() as PrepareMetyMatyADO;
                if (view.FocusedColumn.FieldName == "METY_MATY_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        editor.EditValue = data.METY_MATY_TYPE_ID;
                        editor.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(reposityClosedClick);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void reposityClosedClick(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                GridLookUpEdit editor = sender as GridLookUpEdit;
                if (editor != null && editor.EditValue != null)
                {
                    long id = Inventec.Common.TypeConvert.Parse.ToInt64(editor.EditValue.ToString());
                    List<PrepareMetyMatyADO> list = editor.Properties.DataSource as List<PrepareMetyMatyADO>;
                    if (list != null && list.Count > 0)
                    {
                        PrepareMetyMatyADO prepareMetyMatyADO = list.FirstOrDefault(o => o.METY_MATY_TYPE_ID == id);
                        PrepareMetyMatyADO temp = gridViewPrepareMety.GetFocusedRow() as PrepareMetyMatyADO;
                        if (temp != null && prepareMetyMatyADO != null)
                        {
                            temp.MANUFACTURER_NAME = prepareMetyMatyADO.MANUFACTURER_NAME;
                            temp.ACTIVE_INGR_BHYT_NAME = prepareMetyMatyADO.ACTIVE_INGR_BHYT_NAME;
                            temp.CONCENTRA = prepareMetyMatyADO.CONCENTRA;
                            temp.METY_MATY_TYPE_ID = prepareMetyMatyADO.METY_MATY_TYPE_ID;
                            temp.METY_MATY_TYPE_CODE = prepareMetyMatyADO.METY_MATY_TYPE_CODE;
                            temp.METY_MATY_TYPE_NAME = prepareMetyMatyADO.METY_MATY_TYPE_NAME;
                            temp.NATIONAL_NAME = prepareMetyMatyADO.NATIONAL_NAME;
                            temp.SERVICE_UNIT_NAME = prepareMetyMatyADO.SERVICE_UNIT_NAME;
                            temp.TYPE = prepareMetyMatyADO.TYPE;
                            gridViewPrepareMety.LayoutChanged();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditActionPlus_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var prepareMetyMatys = gridControlPrepareMety.DataSource as List<PrepareMetyMatyADO>;
                PrepareMetyMatyADO prepareMetyMatyADO = new PrepareMetyMatyADO();
                prepareMetyMatyADO.ACTION = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                prepareMetyMatys.Add(prepareMetyMatyADO);
                gridControlPrepareMety.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditActionMinus_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var prepareMetyMatys = gridControlPrepareMety.DataSource as List<PrepareMetyMatyADO>;
                var prepareMetyMatyADO = gridViewPrepareMety.GetFocusedRow() as PrepareMetyMatyADO;
                if (prepareMetyMatyADO != null)
                {
                    prepareMetyMatys.Remove(prepareMetyMatyADO);
                    gridControlPrepareMety.RefreshDataSource();
                    gridViewPrepareMety.FocusedColumn = gridViewPrepareMety.Columns[1];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPrepareMety_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "ACTION_DELETE")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    List<PrepareMetyMatyADO> temps = gridControlPrepareMety.DataSource as List<PrepareMetyMatyADO>;
                    if (rowSelected > 0 || (temps != null && temps.Count > 1))
                    {
                        e.RepositoryItem = repositoryItemButtonEditActionMinus;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPrepareMety_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                this.gridViewPrepareMety_CustomRowError(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPrepareMety_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewPrepareMety.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlPrepareMety.DataSource as List<PrepareMetyMatyADO>;
                var row = listDatas[index];
                if (e.ColumnName == "REQ_AMOUNT")
                {
                    if (row.METY_MATY_TYPE_ID > 0 && (row.REQ_AMOUNT ?? 0) <= 0)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = (string)("Số lượng yêu cầu phải lớn hơn 0 ");
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }

                //if (e.ColumnName == "APPROVAL_AMOUNT")
                //{
                //    if (row.METY_MATY_TYPE_ID > 0 && (row.APPROVAL_AMOUNT ?? 0) <= 0)
                //    {
                //        e.Info.ErrorType = ErrorType.Warning;
                //        e.Info.ErrorText = (string)("Số lượng duyệt phải lớn hơn 0");
                //    }
                //    else
                //    {
                //        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                //        e.Info.ErrorText = "";
                //    }
                //}
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPrepareMety_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {

            try
            {
                if (e.Column.FieldName == "STT")
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPrepareMetyLog_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "STT")
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
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

                if (positionHandleControlLeft == -1)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlLeft > edit.TabIndex)
                {
                    positionHandleControlLeft = edit.TabIndex;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                this.positionHandleControlLeft = -1;
                if (!dxValidationProvider1.Validate() || !CheckValiGridPrepareMetyMaty())
                    return;
                WaitingManager.Show();
                HisPrepareSDO hisPrepareSDO = new HisPrepareSDO();
                this.CreateData(ref hisPrepareSDO);
                string uri = actionType == PEnum.ACTION_TYPE.CREATE ? "api/HisPrepare/Create" : actionType == PEnum.ACTION_TYPE.UPDATE ? "api/HisPrepare/Update" : "";
                var result = new BackendAdapter(param)
                    .Post<HisPrepareResultSDO>(uri, ApiConsumers.MosConsumer, hisPrepareSDO, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    success = true;
                    prepare = result.HisPrepare;
                    actionType = PEnum.ACTION_TYPE.UPDATE;
                    if (this.refesshData != null)
                    {
                        this.refesshData();
                    }
                    List<PrepareMetyMatyADO> prepareMetyMatyADOs = gridControlPrepareMety.DataSource as List<PrepareMetyMatyADO>;
                    if (prepareMetyMatyADOs != null && prepareMetyMatyADOs.Count > 0)
                    {
                        foreach (var item in prepareMetyMatyADOs)
                        {
                            if (item.TYPE == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
                            {
                                HIS_PREPARE_METY prepareMety = result.HisPrepareMetys != null ? result.HisPrepareMetys.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.METY_MATY_TYPE_ID) : null;
                                if (prepareMety != null)
                                {
                                    item.ID = prepareMety.ID;
                                }
                            }
                            else if (item.TYPE == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                            {
                                HIS_PREPARE_MATY prepareMaty = result.HisPrepareMatys != null ? result.HisPrepareMatys.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.METY_MATY_TYPE_ID) : null;
                                if (prepareMaty != null)
                                {
                                    item.ID = prepareMaty.ID;
                                }
                            }
                        }
                    }
                }
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateData(ref HisPrepareSDO data)
        {
            try
            {
                data.Description = txtDescription.Text;
                data.FromTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFromTime.DateTime).Value;
                data.ToTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtToTime.DateTime).Value;
                data.ReqLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                data.ReqUsername = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                long treatmentId = treatment != null ? treatment.ID : prepare != null ? prepare.TREATMENT_ID : 0;
                data.TreatmentId = treatmentId;
                if (actionType == PEnum.ACTION_TYPE.UPDATE && prepare != null)
                {
                    data.Id = prepare.ID;
                }
                data.MaterialTypes = GetPrepareMaty();
                data.MedicineTypes = GetPrepareMety();
                data.ReqRoomId = currentModule.RoomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_PREPARE_METY> GetPrepareMety()
        {
            List<HIS_PREPARE_METY> result = new List<HIS_PREPARE_METY>();
            try
            {
                List<PrepareMetyMatyADO> prepareMetyMatyADOs = gridControlPrepareMety.DataSource as List<PrepareMetyMatyADO>;
                if (prepareMetyMatyADOs != null && prepareMetyMatyADOs.Count > 0)
                {
                    var temps = prepareMetyMatyADOs.Where(o => o.TYPE == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM).ToList();
                    if (temps != null && temps.Count > 0)
                    {
                        foreach (var item in temps)
                        {
                            HIS_PREPARE_METY pmt = new HIS_PREPARE_METY();
                            pmt.APPROVAL_AMOUNT = item.APPROVAL_AMOUNT;
                            pmt.MEDICINE_TYPE_ID = item.METY_MATY_TYPE_ID;
                            if (item.ID.HasValue)
                                pmt.ID = item.ID.Value;
                            if (prepare != null)
                                pmt.PREPARE_ID = prepare.ID;
                            pmt.REQ_AMOUNT = item.REQ_AMOUNT ?? 0;
                            result.Add(pmt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<HIS_PREPARE_MATY> GetPrepareMaty()
        {
            List<HIS_PREPARE_MATY> result = new List<HIS_PREPARE_MATY>();
            try
            {
                List<PrepareMetyMatyADO> prepareMetyMatyADOs = gridControlPrepareMety.DataSource as List<PrepareMetyMatyADO>;
                if (prepareMetyMatyADOs != null && prepareMetyMatyADOs.Count > 0)
                {
                    var temps = prepareMetyMatyADOs.Where(o => o.TYPE == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM).ToList();
                    if (temps != null && temps.Count > 0)
                    {
                        foreach (var item in temps)
                        {
                            HIS_PREPARE_MATY pmt = new HIS_PREPARE_MATY();
                            pmt.APPROVAL_AMOUNT = item.APPROVAL_AMOUNT;
                            pmt.MATERIAL_TYPE_ID = item.METY_MATY_TYPE_ID;
                            if (item.ID.HasValue)
                                pmt.ID = item.ID.Value;
                            if (prepare != null)
                                pmt.PREPARE_ID = prepare.ID;
                            pmt.REQ_AMOUNT = item.REQ_AMOUNT ?? 0;
                            result.Add(pmt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void barButtonItemCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
