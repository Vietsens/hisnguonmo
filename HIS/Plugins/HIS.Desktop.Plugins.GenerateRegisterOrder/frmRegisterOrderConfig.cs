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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.GenerateRegisterOrder.ADO;
using HIS.Desktop.Plugins.GenerateRegisterOrder.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.GenerateRegisterOrder
{
    public partial class frmRegisterOrderConfig : FormBase
    {
        bool statecheckColumn = false;
        bool loadingForm = false;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.GenerateRegisterOrder";
        private Inventec.Desktop.Common.Modules.Module module;
        const int ConfigTimeForAutoOpen = 500;

        private List<HisRegisterGateADO> listRegisterAdos = new List<HisRegisterGateADO>();
        int indexFocused = 0;
        List<long> lstId = new List<long>();
        public frmRegisterOrderConfig(Inventec.Desktop.Common.Modules.Module moduleData)
            : base()
        {
            InitializeComponent();
            this.module = moduleData;
            this.SetIcon();
        }

        private void SetIcon()
        {

            try
            {
                string iconPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmRegisterOrderConfig_Load(object sender, EventArgs e)
        {
            try
            {
                this.loadingForm = true;
                WaitingManager.Show();
                SetCheckAllColumn(false);
                SetDefaultSetting();
                InitControlState();
                this.LoadRegisterGate();
                if (chkAutoOpen.Checked)
                {
                    CreateThreadAutoOpen();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally 
            {
                this.loadingForm = false;
            }
        }

        private void CreateThreadAutoOpen()
        {
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadAutoOpen));
                //thread.Priority = System.Threading.ThreadPriority.Highest;
                thread.IsBackground = true;
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
                try
                {
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    thread.Abort();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadAutoOpen()
        {
            try
            {
                System.Threading.Thread.Sleep(ConfigTimeForAutoOpen);
                ProcessThietLap();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultSetting()
        {
            try
            {
                txtColumn.Text = "4";
                txtSttSize.Text = "10";
                txtTitleSize.Text = "10";
                txtSizeItem.Text = "120";
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == "HisRegisterGateADO")
                        {
                            var arrStr = item.VALUE.Split(';');
                            for (int i = 0; i < arrStr.Length; i++)
                            {
                                lstId.Add(Int64.Parse(arrStr[i]));
                            }
                        }
                        else if (item.KEY == txtColumn.Name)
                        {
                            txtColumn.Text = item.VALUE;
                        }
                        else if (item.KEY == txtSttSize.Name)
                        {
                            txtSttSize.Text = item.VALUE;
                        }
                        else if (item.KEY == txtTitleSize.Name)
                        {
                            txtTitleSize.Text = item.VALUE;
                        }
                        else if (item.KEY == txtSizeItem.Name)
                        {
                            txtSizeItem.Text = item.VALUE;
                        }
                        else if (item.KEY == chkAutoOpen.Name)
                        {
                            chkAutoOpen.Checked = item.VALUE == true.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadRegisterGate()
        {
            try
            {
                listRegisterAdos = new List<HisRegisterGateADO>();
                HisRegisterGateCurrentNumOrderFilter filter = new HisRegisterGateCurrentNumOrderFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                List<HisRegisterGateSDO> registerSdos = new BackendAdapter(new CommonParam()).Get<List<HisRegisterGateSDO>>("api/HisRegisterGate/GetCurrentNumOrder", ApiConsumers.MosConsumer, filter, null);
                Mapper.CreateMap<HIS_REGISTER_GATE, HisRegisterGateADO>();
                if (registerSdos != null && registerSdos.Count > 0)
                {
                    foreach (HisRegisterGateSDO sdo in registerSdos)
                    {
                        HisRegisterGateADO ado = new HisRegisterGateADO();
                        ado.ID = sdo.Id;
                        ado.REGISTER_GATE_CODE = sdo.RegisterGateCode;
                        ado.REGISTER_GATE_NAME = sdo.RegisterGateName;
                        ado.CURRENT_NUM_ORDER = (sdo.CurrentNumOrder ?? 0);
                        ado.BEGIN_NUM_ORDER = (sdo.CurrentNumOrder ?? 0) + 1;
                        ado.isChecked = false;
                        if (lstId != null && lstId.Count > 0)
                            ado.isChecked = (lstId.Where(o => o == sdo.Id).ToList() != null && lstId.Where(o => o == sdo.Id).ToList().Count > 0) ? true : false;
                        listRegisterAdos.Add(ado);
                    }
                }
                listRegisterAdos = listRegisterAdos.OrderBy(o => o.REGISTER_GATE_CODE).ToList();
                gridControlRegisterGate.BeginUpdate();
                gridControlRegisterGate.DataSource = listRegisterAdos;
                gridControlRegisterGate.EndUpdate();
                var check = listRegisterAdos.Where(o => o.isChecked).ToList();
                if (check != null && check.Count > 0 && check.Count == listRegisterAdos.Count)
                {
                    SetCheckAllColumn(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRegisterGate_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BEGIN_NUM_ORDER")
                {
                    HisRegisterGateADO row = (HisRegisterGateADO)gridViewRegisterGate.GetRow(e.RowHandle);
                    if (row != null)
                    {
                        row.IsUpdate = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnConfig.Enabled) return;
                ProcessThietLap();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessThietLap()
        {
            try
            {
                WaitingManager.Show();
                var checkList = this.listRegisterAdos.Where(o => o.isChecked).ToList();
                if (checkList == null || checkList.Count == 0)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Bạn chưa chọn dãy cấp số", MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                List<HisRegisterGateADO> updates = this.listRegisterAdos != null ? this.listRegisterAdos.Where(o => o.IsUpdate && o.isChecked).ToList() : null;

                bool success = false;
                CommonParam param = new CommonParam();

                if (updates != null && updates.Count > 0)
                {

                    if (updates.Any(a => a.BEGIN_NUM_ORDER < 0))
                    {
                        WaitingManager.Hide();
                        XtraMessageBox.Show(ResourceMessage.SoThuTuBatDauKhongDuocPhepNhoHonKhong, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DevExpress.Utils.DefaultBoolean.True);
                        return;
                    }
                    List<HisRegisterGateSDO> upSdos = new List<HisRegisterGateSDO>();
                    foreach (var ado in updates)
                    {
                        HisRegisterGateSDO s = new HisRegisterGateSDO();
                        s.Id = ado.ID;
                        s.UpdateNumOrder = (ado.BEGIN_NUM_ORDER == 0) ? ado.BEGIN_NUM_ORDER : (ado.BEGIN_NUM_ORDER - 1);
                        upSdos.Add(s);
                    }

                    success = new BackendAdapter(param).Post<bool>("api/HisRegisterGate/UpdateNumOrder", ApiConsumers.MosConsumer, upSdos, param);
                }
                else
                {
                    success = true;
                }

                WaitingManager.Hide();
                if (!success)
                {
                    MessageManager.Show(param, success);
                    return;
                }
                //this.Hide();
                List<HisRegisterGateADO> lstSend = this.listRegisterAdos != null ? this.listRegisterAdos.Where(o => o.isChecked).ToList() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstSend), lstSend));
                SettingADO stAdo = new SettingADO();
                stAdo.Columns = !string.IsNullOrEmpty(txtColumn.Text) ? Int64.Parse(txtColumn.Text.Trim()) : 4;
                stAdo.SizeItem = !string.IsNullOrEmpty(txtSizeItem.Text) ? Int64.Parse(txtSizeItem.Text.Trim()) : 120;
                stAdo.SizeTitle = !string.IsNullOrEmpty(txtTitleSize.Text) ? Int64.Parse(txtTitleSize.Text.Trim()) : 10;
                stAdo.SizeStt = !string.IsNullOrEmpty(txtSttSize.Text) ? Int64.Parse(txtSttSize.Text.Trim()) : 10;

                frmGenerateRegisterNumOrder frm = new frmGenerateRegisterNumOrder(this.module, lstSend, stAdo, ReloadRegisterGate);

                SetControlState(txtColumn);
                SetControlState(txtSizeItem);
                SetControlState(txtTitleSize);
                SetControlState(txtSttSize);
                SetControlState(chkAutoOpen);
                SetControlState_HisRegisterGateADO(lstSend);

                frm.ShowDialog();
                
                //this.Close();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadRegisterGate()
        {
            try
            {
                this.lstId = this.listRegisterAdos != null ? this.listRegisterAdos.Where(o => o.isChecked).Select(o=>o.ID).ToList() : new List<long>();
                LoadRegisterGate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlState_HisRegisterGateADO(List<HisRegisterGateADO> lstSend)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "HisRegisterGateADO" && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = string.Join(";", lstSend.Select(o => o.ID).ToList());
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = "HisRegisterGateADO";
                    csAddOrUpdate.VALUE = string.Join(";", lstSend.Select(o => o.ID).ToList());
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlState(TextEdit txt)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == txt.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = txt.Text;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = txt.Name;
                    csAddOrUpdate.VALUE = txt.Text;
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlState(CheckEdit chk)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chk.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = chk.Checked.ToString();
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chk.Name;
                    csAddOrUpdate.VALUE = chk.Checked.ToString();
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRegisterGate_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisRegisterGateADO data = (HisRegisterGateADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRegisterGate_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    HisRegisterGateADO data = (HisRegisterGateADO)gridViewRegisterGate.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "isChecked")
                    {
                        if (data.isChecked)
                            e.RepositoryItem = repositoryItemButton_Checked;
                        else
                            e.RepositoryItem = repositoryItemButton_UnCheck;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_UnCheck_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (HisRegisterGateADO)gridViewRegisterGate.GetRow(gridViewRegisterGate.FocusedRowHandle);
                if (data != null)
                {
                    indexFocused = gridViewRegisterGate.FocusedRowHandle;
                    foreach (var item in listRegisterAdos)
                    {
                        if (item.REGISTER_GATE_CODE == data.REGISTER_GATE_CODE)
                            item.isChecked = !data.isChecked;
                    }
                }
                gridControlRegisterGate.DataSource = null;
                gridControlRegisterGate.DataSource = listRegisterAdos;
                gridViewRegisterGate.FocusedRowHandle = indexFocused;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_Checked_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

                var data = (HisRegisterGateADO)gridViewRegisterGate.GetRow(gridViewRegisterGate.FocusedRowHandle);
                if (data != null)
                {
                    indexFocused = gridViewRegisterGate.FocusedRowHandle;
                    foreach (var item in listRegisterAdos)
                    {
                        if (item.REGISTER_GATE_CODE == data.REGISTER_GATE_CODE)
                            item.isChecked = !data.isChecked;
                    }
                }
                gridControlRegisterGate.DataSource = null;
                gridControlRegisterGate.DataSource = listRegisterAdos;
                gridViewRegisterGate.FocusedRowHandle = indexFocused;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRegisterGate_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InColumnPanel)
                    {
                        if (hi.Column.FieldName == "isChecked")
                        {
                            statecheckColumn = !statecheckColumn;
                            this.SetCheckAllColumn(statecheckColumn);
                            this.GridCheckChange(statecheckColumn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridCheckChange(bool checkedAll)
        {
            try
            {
                foreach (var item in this.listRegisterAdos)
                {
                    item.isChecked = checkedAll;
                }
                listRegisterAdos = listRegisterAdos.OrderBy(o => o.REGISTER_GATE_CODE).ToList();
                this.gridViewRegisterGate.BeginUpdate();
                this.gridViewRegisterGate.GridControl.DataSource = this.listRegisterAdos;
                this.gridViewRegisterGate.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCheckAllColumn(bool state)
        {
            try
            {
                this.grd_Check.Image = state ? global::HIS.Desktop.Plugins.GenerateRegisterOrder.Properties.Resources.checkbox : global::HIS.Desktop.Plugins.GenerateRegisterOrder.Properties.Resources.blank_check_box;
                this.grd_Check.ImageAlignment = System.Drawing.StringAlignment.Center;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtColumn_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSizeItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTitleSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSttSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
