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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.ApproveAggrImpMest.ADO;
using HIS.Desktop.Plugins.ApproveAggrImpMest.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApproveAggrImpMest
{
    public partial class frmApproveAggrImpMest : FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData = null;
        long impMestId;
        V_HIS_IMP_MEST_2 impMest { get; set; }
        List<ImpMestSDO> impMestDetails { get; set; }
        List<ImpMestMediMateADO> ImpMestMediMateADOs { get; set; }
        List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines { get; set; }
        List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials { get; set; }
        DelegateRefreshData delegateRefreshData { get; set; }
        HIS_MEDI_STOCK mediStock { get; set; }


        public frmApproveAggrImpMest(long impMestId, Inventec.Desktop.Common.Modules.Module moduleData, DelegateRefreshData _delegateRefreshData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.impMestId = impMestId;
                this.moduleData = moduleData;
                this.delegateRefreshData = _delegateRefreshData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmApproveAggrImpMest_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                LoadImpMestAndImpMestDetail();
                LoadImpMestMedicineAndMaterial();
                LoadDataToControl();
                FillDataToButtonPrint();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestChild_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                List<ImpMestSDO> impMestCheckeds = new List<ImpMestSDO>();
                int[] selectRows = gridViewImpMestChild.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        impMestCheckeds.Add((ImpMestSDO)gridViewImpMestChild.GetRow(selectRows[i]));
                    }
                }
                else
                {
                    var dataSource = (List<ImpMestSDO>)gridControlImpMestChild.DataSource;
                    foreach (var item in dataSource)
                    {
                        item.IsHighLight = false;
                    }
                    gridViewImpMestChild.BeginDataUpdate();
                    gridControlImpMestChild.DataSource = dataSource;
                    gridViewImpMestChild.EndDataUpdate();
                }


                LoadDataToGridMediMate(impMestCheckeds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAppro_Click(object sender, EventArgs e)
        {
            try
            {

                if (this.impMest != null)
                {
                    if (impMest.IMP_MEST_TYPE_ID != null && impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT && impMest.IS_MOBA_CHANGE_AMOUNT == 1)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.V_HIS_IMP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(data, this.impMest);
                        List<object> listArgs = new List<object>();
                        listArgs.Add(data);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ApproveMobaImpMest", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    }
                    else
                    {
                        MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, this.impMest);
                        data.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL;
                        UpdateStatusProcess(data);
                    }

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateStatusProcess(HIS_IMP_MEST data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                var result = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (HisRequestUriStore.HIS_IMP_MEST_UPDATE_STATUS, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                if (result != null)
                {
                    success = true;
                    if (this.delegateRefreshData != null)
                    {
                        this.delegateRefreshData();
                    }
                    LoadImpMest();
                    LoadDataToControl();
                    gridControlImpMestChild.RefreshDataSource();
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnImp_Click(object sender, EventArgs e)
        {
            try
            {

                if (this.impMest != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, this.impMest);
                    data.ID = this.impMest.ID;
                    var result = new Inventec.Common.Adapter.BackendAdapter
                   (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                   (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_IMPORT, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                    if (result != null)
                    {
                        success = true;
                        if (this.delegateRefreshData != null)
                        {
                            this.delegateRefreshData();
                        }
                        LoadImpMest();
                        LoadDataToControl();
                        gridControlImpMestChild.RefreshDataSource();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestChild_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (this.impMest != null && this.impMest.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST && this.CheckMediStockCurrent())
                        {
                            e.RepositoryItem = repositoryItemButtonEditDelete;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEditDelete_Disabled;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toggleSMediMateType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewImpMestChild_SelectionChanged(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestChild_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ImpMestSDO impMest = (ImpMestSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (impMest != null)
                    {
                        if (e.Column.FieldName == "DOB_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(impMest.TDL_PATIENT_DOB ?? 0);
                        }
                        else if (e.Column.FieldName == "TIME_REQUEST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(impMest.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TDL_INTRUCTION_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(impMest.TDL_INTRUCTION_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestChild_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "DELETE")
                {
                    if (this.CheckMediStockCurrent())
                    {
                        if (this.impMest.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST)
                        {
                            DialogResult myResult;
                            myResult = MessageBox.Show("Bạn có muốn xóa phiếu nhập này không?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            bool success = false;
                            CommonParam param = new CommonParam();
                            if (myResult == DialogResult.OK)
                            {
                                WaitingManager.Show();
                                ImpMestSDO impMestDel = gridViewImpMestChild.GetFocusedRow() as ImpMestSDO;
                                if (impMestDel != null)
                                {
                                    bool result = new BackendAdapter(param)
                                    .Post<bool>(UriRequestStore.IMP_MEST__REMOVE_AGGR, ApiConsumers.MosConsumer, impMestDel.ID, param);
                                    if (result)
                                    {
                                        success = true;
                                        impMestDetails.Remove(impMestDel);
                                        gridControlImpMestChild.RefreshDataSource();
                                    }
                                }
                                WaitingManager.Hide();
                                MessageManager.Show(this, param, success);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMediMate_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {

                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    ImpMestMediMateADO pData = (ImpMestMediMateADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "CONVERT_AMOUNT"
                        && pData.CONVERT_RATIO != null
                        && pData.CONVERT_RATIO > 0)
                    {
                        e.Value = pData.AMOUNT * pData.CONVERT_RATIO;
                    }

                    if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.EXPIRED_DATE ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMediMate_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                var data = (ImpMestMediMateADO)gridViewMediMate.GetRow(e.RowHandle);
                if (data != null)
                {
                    // nếu thuốc là gây nghiện hướng thần
                    if (data.IsMedicine && (data.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT || data.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN))
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                    else if (data.IsMedicine)// thuốc thường
                    {
                        e.Appearance.ForeColor = Color.Black;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEdit_BNSuDung_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var focus = (ImpMestMediMateADO)gridViewMediMate.GetFocusedRow();
                if (focus != null)
                {
                    var dataSource = (List<ImpMestSDO>)gridControlImpMestChild.DataSource;

                    if (toggleSMediMateType.IsOn)// theo loại thuốc
                    {
                        if (focus.IsMedicine)
                        {
                            var medicineCheck = this.impMestMedicines != null && this.impMestMedicines.Count() > 0 ? this.impMestMedicines.Where(o => o.MEDICINE_TYPE_ID == focus.MEDI_MATE_TYPE_ID).ToList() : null;
                            foreach (var item in dataSource)
                            {
                                if (medicineCheck != null && medicineCheck.Count() > 0 && medicineCheck.Select(o => o.IMP_MEST_ID).Contains(item.ID))
                                {
                                    item.IsHighLight = true;
                                }
                                else
                                {
                                    item.IsHighLight = false;
                                }
                            }
                        }
                        else
                        {
                            var materialCheck = this.impMestMaterials != null && this.impMestMaterials.Count() > 0 ? this.impMestMaterials.Where(o => o.MATERIAL_TYPE_ID == focus.MEDI_MATE_TYPE_ID).ToList() : null;
                            foreach (var item in dataSource)
                            {
                                if (materialCheck != null && materialCheck.Count() > 0 && materialCheck.Select(o => o.IMP_MEST_ID).Contains(item.ID))
                                {
                                    item.IsHighLight = true;
                                }
                                else
                                {
                                    item.IsHighLight = false;
                                }
                            }
                        }

                    }
                    else// theo lô
                    {
                        if (focus.IsMedicine)
                        {
                            var medicineCheck = this.impMestMedicines != null && this.impMestMedicines.Count() > 0 ? this.impMestMedicines.Where(o => o.MEDICINE_ID == focus.MEDI_MATE_ID && o.MEDICINE_TYPE_ID == focus.MEDI_MATE_TYPE_ID).ToList() : null;

                            foreach (var item in dataSource)
                            {
                                if (medicineCheck != null && medicineCheck.Count() > 0 && medicineCheck.Select(o => o.IMP_MEST_ID).Contains(item.ID))
                                {
                                    item.IsHighLight = true;
                                }
                                else
                                {
                                    item.IsHighLight = false;
                                }
                            }
                        }
                        else if (this.impMestMaterials != null && this.impMestMaterials.Count() > 0)
                        {
                            var materialCheck = this.impMestMaterials != null && this.impMestMaterials.Count() > 0 ? this.impMestMaterials.Where(o => o.MATERIAL_ID == focus.MEDI_MATE_ID && o.MATERIAL_TYPE_ID == focus.MEDI_MATE_TYPE_ID).ToList() : null;

                            foreach (var item in dataSource)
                            {
                                if (materialCheck != null && materialCheck.Count() > 0 && materialCheck.Select(o => o.IMP_MEST_ID).Contains(item.ID))
                                {
                                    item.IsHighLight = true;
                                }
                                else
                                {
                                    item.IsHighLight = false;
                                }
                            }
                        }
                    }

                    var select = gridViewImpMestChild.GetSelectedRows();

                    gridControlImpMestChild.BeginUpdate();
                    gridControlImpMestChild.DataSource = dataSource;
                    foreach (var item in select)
                    {
                        gridViewImpMestChild.SelectRow(item);
                    }
                    gridControlImpMestChild.EndUpdate();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestChild_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                var data = (ImpMestSDO)gridViewImpMestChild.GetRow(e.RowHandle);
                if (data != null && data.IsHighLight)
                {
                    e.Appearance.BackColor = Color.Yellow;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmApproveAggrImpMest_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (this.delegateRefreshData != null)
                {

                    this.delegateRefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
