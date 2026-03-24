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
using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using System.Threading;
using MOS.SDO;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.Data;
using DevExpress.XtraBars.Utils;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.PrepareAndExportByArea.Run
{
    public partial class frmPrepareAndExportByArea
    {
        //Danh sách đã soạn thuốc
        private void LoadTab3()
        {
            try
            {
                lstTab3 = new List<HIS_EXP_MEST>();

                // Lọc dữ liệu theo điều kiện
                var filteredData = lstAll.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE && o.IS_ABSENT != 1).ToList();
                
                // Lọc loại bỏ các treatment code có phiếu xuất không phải EXECUTE
                var treatmentCodesWithNonExecute = lstAll
                    .Where(o => o.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                    .Select(o => o.TDL_TREATMENT_CODE)
                    .Distinct()
                    .ToList();
                
                if (treatmentCodesWithNonExecute != null && treatmentCodesWithNonExecute.Count > 0)
                {
                    filteredData = filteredData.Where(o => !treatmentCodesWithNonExecute.Contains(o.TDL_TREATMENT_CODE)).ToList();
                }

                // Gom nhóm theo TDL_TREATMENT_CODE, IS_CONFIRM, EXP_MEST_STT_ID
                var groupedData = filteredData
                    .GroupBy(o => new
                    {
                        o.TDL_TREATMENT_CODE,
                        o.IS_CONFIRM,
                        o.NUM_ORDER
                    })
                    .OrderBy(g => g.Key.NUM_ORDER)
                    .ThenBy(g => g.Key.IS_CONFIRM)
                    .ThenBy(g => g.Key.TDL_TREATMENT_CODE);

                if (chkCallAll.Checked)
                {
                    lstTab3 = groupedData
                        .Select(group =>
                        {
                            // Trong mỗi nhóm, sắp xếp theo PRIORITY và NUM_ORDER
                            var orderedItems = group.OrderByDescending(o => o.PRIORITY != null && o.PRIORITY == 1 ? 1 : 0)
                                //.ThenByDescending(o => o.PRIORITY)
                                .ThenBy(o => o.NUM_ORDER)
                                .ToList();

                            var first = orderedItems.First();

                            return new HIS_EXP_MEST
                            {
                                ID = first.ID,
                                TDL_TREATMENT_CODE = first.TDL_TREATMENT_CODE,
                                IS_CONFIRM = first.IS_CONFIRM,
                                EXP_MEST_STT_ID = first.EXP_MEST_STT_ID,
                                EXP_MEST_CODE = string.Join(",", orderedItems.Select(x => x.EXP_MEST_CODE)),
                                PRIORITY = first.PRIORITY,
                                NUM_ORDER = first.NUM_ORDER,
                                TDL_PATIENT_NAME = first.TDL_PATIENT_NAME,
                                TDL_PATIENT_GENDER_NAME = first.TDL_PATIENT_GENDER_NAME,
                                TDL_PATIENT_DOB = first.TDL_PATIENT_DOB,
                                TDL_PATIENT_ADDRESS = first.TDL_PATIENT_ADDRESS,
                                GATE_CODE = first.GATE_CODE,
                                EXP_MEST_TYPE_ID = first.EXP_MEST_TYPE_ID
                            };
                        })
                        .OrderByDescending(o => o.PRIORITY != null && o.PRIORITY == 1 ? 1 : 0)
                        .ThenBy(o => o.NUM_ORDER)
                        .ToList();
                }
                else
                {
                    lstTab3 = groupedData
                    .Select(group =>
                    {
                        // Trong mỗi nhóm, sắp xếp theo PRIORITY và NUM_ORDER
                        var orderedItems = group.OrderByDescending(o => o.PRIORITY != null && o.PRIORITY == 1 ? 1 : 0)
                            //.ThenByDescending(o => o.PRIORITY)
                            .ThenBy(o => o.NUM_ORDER)
                            .ToList();

                        var first = orderedItems.First();

                        return new HIS_EXP_MEST
                        {
                            ID = first.ID,
                            TDL_TREATMENT_CODE = first.TDL_TREATMENT_CODE,
                            IS_CONFIRM = first.IS_CONFIRM,
                            EXP_MEST_STT_ID = first.EXP_MEST_STT_ID,
                            EXP_MEST_CODE = string.Join(",", orderedItems.Select(x => x.EXP_MEST_CODE)),
                            PRIORITY = first.PRIORITY,
                            NUM_ORDER = first.NUM_ORDER,
                            TDL_PATIENT_NAME = first.TDL_PATIENT_NAME,
                            TDL_PATIENT_GENDER_NAME = first.TDL_PATIENT_GENDER_NAME,
                            TDL_PATIENT_DOB = first.TDL_PATIENT_DOB,
                            TDL_PATIENT_ADDRESS = first.TDL_PATIENT_ADDRESS,
                            GATE_CODE = first.GATE_CODE,
                            EXP_MEST_TYPE_ID = first.EXP_MEST_TYPE_ID
                        };
                    })
                    .OrderBy(o => o.NUM_ORDER)
                    .ToList();
                }
                gcPrepareMedicine.DataSource = null;
                if (lstTab3 != null && lstTab3.Count > 0)
                {
                    gcPrepareMedicine.DataSource = lstTab3;
                }
                gcPrepareMedicine.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        gvPrepareMedicine.Focus();
                        gvPrepareMedicine.FocusedRowHandle = DevExpress.XtraGrid.GridControl.AutoFilterRowHandle;

                        gvPrepareMedicine.FocusedColumn = gridColumn26;

                        gvPrepareMedicine.ShowEditor(); 
                        var ed = gvPrepareMedicine.ActiveEditor as DevExpress.XtraEditors.BaseEdit;
                        ed?.SelectAll();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }));
                Inventec.Common.Logging.LogSystem.Debug("QUẦY __" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => txtGateCodeString), txtGateCodeString));
                Inventec.Common.Logging.LogSystem.Debug("IP __" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => txtIpCPA), txtIpCPA));
                if (!string.IsNullOrEmpty(txtGateCodeString) && dteStt.DateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                {
                    CreateThreadCallPatientRefresh();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                frmConfig frm = new frmConfig(IsOpen, GateConfig, IpConfig);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void IpConfig(string obj)
        {
            try
            {
                txtIpCPA = obj;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GateConfig(string obj)
        {
            try
            {
                txtGateCodeString = obj;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void IsOpen(bool obj)
        {
            if (obj)
            {
                //CreateThreadCallPatientRefresh();				
            }
        }

        private void btnAbsent_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HIS_EXP_MEST data = currentCall;
                if (data == null) return;

                // Lấy danh sách ID từ lstAll
                List<long> groupedExpMestIds = expCodeToId(data.EXP_MEST_CODE);
                if (groupedExpMestIds.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy phiếu xuất trong danh sách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                HisExpMestSDO sdo = new HisExpMestSDO();
                sdo.ExpMestIds = groupedExpMestIds;
                sdo.ReqRoomId = this.currentModule.RoomId;

                WaitingManager.Show();
                List<V_HIS_EXP_MEST> rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<V_HIS_EXP_MEST>>("api/HisExpMest/Absent", ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();

                if (rs != null && rs.Count > 0)
                {
                    success = true;

                    // Cập nhật trạng thái cho tất cả phiếu trong kết quả
                    foreach (var rsItem in rs)
                    {
                        var item = lstAll.FirstOrDefault(x => x.ID == rsItem.ID);
                        if (item != null)
                        {
                            item.EXP_MEST_STT_ID = rsItem.EXP_MEST_STT_ID;
                            item.IS_ABSENT = 1;

                            // Thêm vào Tab 4 (Vắng mặt) nếu chưa có
                            if (lstTab4 == null)
                                lstTab4 = new List<HIS_EXP_MEST>();

                            if (!lstTab4.Any(x => x.ID == item.ID))
                            {
                                lstTab4.Add(item);
                            }
                        }
                    }

                    // Reload tab 4
                    gcAbssentN.DataSource = null;
                    gcAbssentN.DataSource = lstTab4;

                    try
                    {
                        gvPrepareMedicine.ActiveFilterString = "";
                        gvPrepareMedicine.ClearColumnsFilter();
                        gvPrepareMedicine.SetRowCellValue(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle, gridColumn26, null);
                    }
                    catch { }

                    LoadTab3();
                    LoadTab4();

                    currentCall = null;
                    txtCurrentCall.Text = "";
                }

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGaveMedicine_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HIS_EXP_MEST data = currentCall;
                if (data == null) return;
                // Lấy danh sách ID từ lstAll
                List<long> groupedExpMestIds = expCodeToId(data.EXP_MEST_CODE);

                if (groupedExpMestIds.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy phiếu xuất trong danh sách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                HisExpMestSDO sdo = new HisExpMestSDO();
                sdo.ExpMestIds = groupedExpMestIds;
                sdo.ReqRoomId = this.currentModule.RoomId;

                WaitingManager.Show();
                string api = "api/HisExpMest/AggrExamExportList";
                List<V_HIS_EXP_MEST> rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<V_HIS_EXP_MEST>>(api, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();

                if (rs != null && rs.Count > 0)
                {
                    success = true;

                    // Cập nhật trạng thái cho tất cả phiếu trong nhóm
                    foreach (var expMestId in groupedExpMestIds)
                    {
                        var item = lstAll.FirstOrDefault(x => x.ID == expMestId);
                        if (item != null)
                        {
                            // Tìm thông tin từ kết quả API
                            var rsItem = rs.FirstOrDefault(x => x.ID == item.ID);
                            if (rsItem != null)
                            {
                                item.EXP_MEST_STT_ID = rsItem.EXP_MEST_STT_ID;
                            }
                            item.IS_ABSENT = null;

                            // Thêm vào Tab 5 (Đã phát thuốc) nếu chưa có
                            if (lstTab5 == null)
                                lstTab5 = new List<HIS_EXP_MEST>();

                            if (!lstTab5.Any(x => x.ID == item.ID))
                            {
                                lstTab5.Add(item);
                            }
                        }
                    }

                    // Reload tab 5
                    gcPassMedicine.DataSource = null;
                    gcPassMedicine.DataSource = lstTab5;

                    try
                    {
                        gvPrepareMedicine.ActiveFilterString = "";
                        gvPrepareMedicine.ClearColumnsFilter();
                        gvPrepareMedicine.SetRowCellValue(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle, gridColumn26, null);
                    }
                    catch { }

                    LoadTab3();
                    LoadTab5();
                    currentCall = null;
                    txtCurrentCall.Text = "";
                }

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCall_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (string.IsNullOrEmpty(txtGateCodeString))
            //    {
            //        frmConfig frm = new frmConfig(IsOpen, GateConfig, IpConfig);
            //        frm.ShowDialog();
            //        return;
            //    }
            //    if (currentCall != null)
            //    {
            //        if (this.clienttManager == null)
            //            this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager(txtIpCPA);
            //        bool rs = this.clienttManager.RecallOrderDataClientBool(currentCall.NUM_ORDER.ToString());
            //        Inventec.Common.Logging.LogSystem.Error("GỌI LẠI ___ " + rs);
            //    }
            //    else
            //    {
            //        Inventec.Common.Logging.LogSystem.Error("KẾT NỐI ___ ");
            //        CreateThreadCallPatientCPA();
            //        CallPatientCPA();
            //    }
            //}
            try
            {
                if (string.IsNullOrEmpty(txtGateCodeString))
                {
                    new frmConfig(IsOpen, GateConfig, IpConfig).ShowDialog();
                    return;
                }

                if (this.clienttManager == null)
                    this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager(txtIpCPA);
                LoadListDataSource();
                LoadTab3();
                var target = GetTargetFromPrepareGrid();
                if (target == null) return;
                if (currentCall != null && currentCall.GATE_CODE == txtGateCodeString)
                {
                   //if (currentCall.GATE_CODE != txtGateCodeString)
                   //     currentCall = target; 
                    //if (target != null && target.ID != currentCall.ID)
                    //{
                    //    return;
                    //}
                    bool recallRs = this.clienttManager.RecallOrderDataClientBool(
                        currentCall.NUM_ORDER.ToString(),
                        txtGateCodeString
                    );

                    Inventec.Common.Logging.LogSystem.Error("GỌI LẠI ___ " + recallRs);
                    return;
                }

                if (target != null)
                {
                    CallSpecific(target);
                    return;
                }
                //var one = GetMinOrderForMyGate();
                //if (one != null)
                //{
                //    CallSpecific(one);
                //    return;
                //}

                CallPatientCPA();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadCallPatientCPA()
        {
            Thread thread = new System.Threading.Thread(CallPatientCPA);
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

        private void CallPatientCPA()
        {
            try
            {

                if (this.clienttManager == null)
                    this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager(txtIpCPA);
                string rs = this.clienttManager.CallOrderDataString(txtGateCodeString, chkCallAll.Checked);
                Inventec.Common.Logging.LogSystem.Error("GỌI ___" + rs);
                if (rs == null)
                    return;
                if (lstAll.FirstOrDefault(o => o.NUM_ORDER == Int64.Parse(rs)) == null || !chkAutoLoadTab.Checked)
                {
                    LoadListDataSource();
                    LoadAllTab();
                }
                currentCall = lstAll.FirstOrDefault(o => o.NUM_ORDER == Int64.Parse(rs));
                txtCurrentCall.Text = currentCall.NUM_ORDER + " - " + currentCall.TDL_PATIENT_NAME + " - " + currentCall.TDL_TREATMENT_CODE;
                if (rs != currentCall.GATE_CODE)
                {
                    CommonParam param = new CommonParam();
                    bool success;
                    if (currentCall != null)
                    {
                        ExpMestCallSDO sdo = new ExpMestCallSDO();
                        sdo.ExpMestId = currentCall.ID;
                        sdo.GateCode = txtGateCodeString;
                        WaitingManager.Show();
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisExpMest/Call", ApiConsumers.MosConsumer, sdo, param);
                        WaitingManager.Hide();
                        if (success)
                        {
                            foreach (var item in lstAll)
                            {
                                if (item.ID == currentCall.ID)
                                {
                                    item.GATE_CODE = txtGateCodeString;
                                    break;
                                }
                            }
                            LoadTab3();
                        }
                        MessageManager.Show(this.ParentForm, param, success);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCallAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == chkCallAll.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.PrepareAndExportByArea").FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = chkCallAll.Checked ? "1" : "0";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkCallAll.Name;
                    csAddOrUpdate.VALUE = chkCallAll.Checked ? "1" : "0";
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.PrepareAndExportByArea";
                    if (currentControlStateRDO == null)
                        currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDO.Add(csAddOrUpdate);
                }
                controlStateWorker.SetData(currentControlStateRDO);
                LoadTab3();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void gvPrepareMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    long? priority = (long?)view.GetRowCellValue(e.RowHandle, "PRIORITY");
                    if (priority != null & priority == 1)
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void txtCurrentCall_TextChanged(object sender, EventArgs e)
        {
            try
            {
                btnAbsent.Enabled = !string.IsNullOrEmpty(txtCurrentCall.Text);
                btnGaveMedicine.Enabled = !string.IsNullOrEmpty(txtCurrentCall.Text);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void gvPrepareMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {

                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_EXP_MEST pData = (HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "DOB_str")
                    {
                        if (pData.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        {
                            e.Value = pData.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        }
                        else
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.TDL_PATIENT_DOB ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gvPrepareMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                string gateCode = (gvPrepareMedicine.GetRowCellValue(e.RowHandle, "GATE_CODE") ?? "").ToString();
                if (e.Column.FieldName == "ReCall")
                {
                    if (!string.IsNullOrEmpty(gateCode))
                    {
                        e.RepositoryItem = repReCall;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repReCall_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

                HIS_EXP_MEST data = (HIS_EXP_MEST)gvPrepareMedicine.GetFocusedRow();
                if (this.clienttManager == null)
                    this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager(txtIpCPA);
                bool rs = this.clienttManager.RecallOrderDataClientBool(data.NUM_ORDER.ToString(), txtGateCodeString);
                Inventec.Common.Logging.LogSystem.Error("GỌI LẠI TRÊN LƯỚI ___ " + rs);
                currentCall = data;
                txtCurrentCall.Text = currentCall.NUM_ORDER + " - " + currentCall.TDL_PATIENT_NAME + " - " + currentCall.TDL_TREATMENT_CODE;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repUnapprove_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_EXP_MEST data = (HIS_EXP_MEST)gvPrepareMedicine.GetFocusedRow();
                if (data == null) return;

                if (data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();

                    // Lấy danh sách ID từ lstAll
                    List<long> groupedExpMestIds = expCodeToId(data.EXP_MEST_CODE);

                    if (groupedExpMestIds.Count == 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy phiếu xuất trong danh sách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (gcPrepareMedicine.DataSource != null)
                    {
                        // Tạo SDO với danh sách IDs
                        HisExpMestSDO sdo = new HisExpMestSDO();
                        sdo.ExpMestIds = groupedExpMestIds;
                        sdo.ReqRoomId = this.currentModule.RoomId;

                        // Gọi API hủy duyệt với danh sách IDs (GỌI 1 LẦN)
                        List<V_HIS_EXP_MEST> apiResults = new Inventec.Common.Adapter.BackendAdapter(param)
                            .Post<List<V_HIS_EXP_MEST>>("api/HisExpMest/AggrExamUnapproveList", 
                                                         ApiConsumer.ApiConsumers.MosConsumer, 
                                                         sdo, 
                                                         param);

                        if (apiResults != null && apiResults.Count > 0)
                        {
                            success = true;

                            // Cập nhật trạng thái cho tất cả phiếu trong kết quả
                            foreach (var apiResult in apiResults)
                            {
                                var item = lstAll.FirstOrDefault(x => x.ID == apiResult.ID);
                                if (item != null)
                                {
                                    var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiResult.EXP_MEST_STT_ID);
                                    if (expMestSTT != null)
                                    {
                                        item.EXP_MEST_STT_ID = expMestSTT.ID;
                                    }
                                    item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                }
                            }

                            gvPrepareMedicine.BeginUpdate();
                            LoadTab3();
                            LoadTab2();

                            gvPrepareMedicine.EndUpdate();
                        }
                    }

                    WaitingManager.Hide();

                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_EXP_MEST GetTargetFromPrepareGrid()
        {
            try
            {

                //lấy thông tin quày hiện tại
                //nếu có danh sách, thì lấy ưu tiên phiếu đã gọi nhưng chưa cấp phát sau đó thì đén chưa có quầy
                if (gvPrepareMedicine.FocusedRowHandle >= 0)
                    return gvPrepareMedicine.GetFocusedRow() as HIS_EXP_MEST;
                else
                {
                    if (lstTab3 == null)
                        return null;

                    var a = lstTab3.FirstOrDefault(o => o.GATE_CODE == txtGateCodeString && (!chkCallAll.Checked ? (o.PRIORITY == null || o.PRIORITY == 0) : true) && (o.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || (o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE && o.IS_ABSENT == 1)));
                    if (a != null)
                    {
                        return a;
                    }

                    var b = lstTab3.FirstOrDefault(o => string.IsNullOrWhiteSpace(o.GATE_CODE) && (!chkCallAll.Checked ? (o.PRIORITY == null || o.PRIORITY == 0) : true));
                    return b;
                }

                //if (gvPrepareMedicine.FocusedRowHandle == DevExpress.XtraGrid.GridControl.AutoFilterRowHandle && gvPrepareMedicine.RowCount == 1)
                //{
                //    int rowHandle = gvPrepareMedicine.GetVisibleRowHandle(0);
                //    return gvPrepareMedicine.GetRow(rowHandle) as HIS_EXP_MEST;
                //}    
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
