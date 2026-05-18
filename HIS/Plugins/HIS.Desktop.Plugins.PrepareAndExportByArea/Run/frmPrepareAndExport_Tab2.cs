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
using MOS.SDO;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using System.Collections;
using HIS.Desktop.ApplicationFont;
using MOS.Filter;

namespace HIS.Desktop.Plugins.PrepareAndExportByArea.Run
{
    public partial class frmPrepareAndExportByArea
    {
        private List<HIS_EXP_MEST> listResultTextTab2 { get; set; }
        private bool _isLoadingTab2;
        //Danh sách đã in
        private async Task LoadTab2()
        {
            if (_isLoadingTab2) return;
            _isLoadingTab2 = true;
            try
            {
                Action action;
                if (chkNotPrint.Checked)
                {
                    action = () =>
                    {
                        lstTab2 = new List<HIS_EXP_MEST>();

                        //// L?y danh sách treatment code có ít nh?t 1 phi?u không ph?i EXECUTE
                        //var treatmentCodesWithNonExecute = lstAll
                        //    .Where(o => o.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        //    .Select(o => o.TDL_TREATMENT_CODE)
                        //    .Distinct()
                        //    .ToList();

                        //// L?c d? li?u theo di?u ki?n: IS_CONFIRM == 1 VÀ (ID__REQUEST HO?C thu?c treatment code có phi?u không ph?i EXECUTE)
                        //var filteredData = lstAll.Where(o =>
                        //    (o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST ||
                        //     (treatmentCodesWithNonExecute != null && treatmentCodesWithNonExecute.Contains(o.TDL_TREATMENT_CODE)))
                        //).ToList();

                        List<HIS_EXP_MEST> filteredData = new List<HIS_EXP_MEST>();
                        filteredData.AddRange(lstAll.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && o.PRIORITY > 0).OrderByDescending(o => o.PRIORITY).ThenBy(o => o.NUM_ORDER).ToList());
                        filteredData.AddRange(lstAll.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && (o.PRIORITY == 0 || o.PRIORITY == null)).OrderBy(o => o.NUM_ORDER).ToList());

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

                        lstTab2 = groupedData
                        .Select(group =>
                        {
                            var orderedItems = group
                                .OrderByDescending(o => o.PRIORITY != null && o.PRIORITY == 1 ? 1 : 0)
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
                                TDL_TREATMENT_ID = first.TDL_TREATMENT_ID
                            };
                        }).OrderByDescending(o => o.PRIORITY != null && o.PRIORITY == 1 ? 1 : 0)
                                .ThenBy(o => o.NUM_ORDER)
                        .ToList();
                    };
                }
                else
                {
                    action = () =>
                    {
                        lstTab2 = new List<HIS_EXP_MEST>();

                        //// L?y danh sách treatment code có ít nh?t 1 phi?u không ph?i EXECUTE
                        //var treatmentCodesWithNonExecute = lstAll
                        //    .Where(o => o.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        //    .Select(o => o.TDL_TREATMENT_CODE)
                        //    .Distinct()
                        //    .ToList();

                        //// L?c d? li?u theo di?u ki?n: IS_CONFIRM == 1 VÀ (ID__REQUEST HO?C thu?c treatment code có phi?u không ph?i EXECUTE)
                        //var filteredData = lstAll.Where(o =>
                        //    (o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST ||
                        //     (treatmentCodesWithNonExecute != null && treatmentCodesWithNonExecute.Contains(o.TDL_TREATMENT_CODE)))
                        //).ToList();

                        List<HIS_EXP_MEST> filteredData = new List<HIS_EXP_MEST>();
                        filteredData.AddRange(lstAll.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && o.IS_CONFIRM == 1 && o.PRIORITY > 0).OrderByDescending(o => o.PRIORITY).ThenBy(o => o.NUM_ORDER).ToList());
                        filteredData.AddRange(lstAll.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && o.IS_CONFIRM == 1 && (o.PRIORITY == 0 || o.PRIORITY == null)).OrderBy(o => o.NUM_ORDER).ToList());

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

                        lstTab2 = groupedData
                            .Select(group =>
                            {
                                var orderedItems = group
                                    .OrderByDescending(o => o.PRIORITY > 0 ? 1 : 0)
                                    .ThenByDescending(o => o.PRIORITY)
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
                                    TDL_TREATMENT_ID = first.TDL_TREATMENT_ID
                                };
                            })
                            .ToList();
                    };
                }
                await Task.Run(action);
                gcPrinted.DataSource = null;
                if (lstTab2 != null && lstTab2.Count > 0)
                {
                    gcPrinted.DataSource = lstTab2;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                _isLoadingTab2 = false;
            }
        }

        private void repPrintPrinted_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                dataPrintMps480 = (HIS_EXP_MEST)gvPrinted.GetFocusedRow();
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter mrCheckFilter = new HisExpMestMedicineViewFilter();
                mrCheckFilter.AGGR_EXP_MEST_IDs = expCodeToId(dataPrintMps480.EXP_MEST_CODE);
                lstExpMestMedicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, mrCheckFilter, param);

                HisExpMestMaterialViewFilter mtCheckFilter = new HisExpMestMaterialViewFilter();
                mtCheckFilter.AGGR_EXP_MEST_IDs = expCodeToId(dataPrintMps480.EXP_MEST_CODE);
                lstExpMestMaterial = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, mtCheckFilter, param);
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();

                HisExpMestViewFilter expFilter = new HisExpMestViewFilter();
                expFilter.IDs = expCodeToId(dataPrintMps480.EXP_MEST_CODE);
                lstVExpMest = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expFilter, param);

                if (dataPrintMps480.TDL_TREATMENT_ID != null)
                {
                    treatmentFilter.ID = dataPrintMps480.TDL_TREATMENT_ID;
                }
                else if (lstExpMestMedicine != null && lstExpMestMedicine.Count > 0)
                {
                    treatmentFilter.ID = lstExpMestMedicine.FirstOrDefault(o => o.TDL_TREATMENT_ID != null).TDL_TREATMENT_ID;
                }
                else if (treatmentFilter.ID != null && lstExpMestMaterial != null && lstExpMestMaterial.Count > 0)
                {
                    treatmentFilter.ID = lstExpMestMaterial.FirstOrDefault(o => o.TDL_TREATMENT_ID != null).TDL_TREATMENT_ID;
                }

                if (treatmentFilter.ID != null)
                {
                    List<HIS_TREATMENT> lstTreatment = new List<HIS_TREATMENT>();
                    lstTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param);
                    if (lstTreatment != null && lstTreatment.Count > 0)
                    {
                        treatment = lstTreatment.FirstOrDefault();
                    }
                }
                IsPrintNow = false;
                PrintMps480();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repApprovePrinted_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HIS_EXP_MEST data = (HIS_EXP_MEST)gvPrinted.GetFocusedRow();
                Approve(data, ref param, ref success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Approve(HIS_EXP_MEST data, ref CommonParam param, ref bool success)
        {
            try
            {
                if (data == null) return;

                // Lấy danh sách ID từ lstAll
                List<long> groupedExpMestIds = expCodeToId(data.EXP_MEST_CODE);

                HisExpMestSDO sdo = new HisExpMestSDO();
                sdo.ExpMestIds = groupedExpMestIds;
                sdo.ReqRoomId = currentModule.RoomId;

                WaitingManager.Show();
                string api = "api/HisExpMest/AggrExamApproveList";
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
                            item.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;

                            // Thêm vào Tab 3 (Chuẩn bị thuốc) nếu chưa có
                            if (lstTab3 == null)
                                lstTab3 = new List<HIS_EXP_MEST>();

                            if (!lstTab3.Any(x => x.ID == item.ID))
                            {
                                lstTab3.Add(item);
                            }
                        }
                    }

                    // Reload tab 3
                    gcPrepareMedicine.DataSource = null;
                    gcPrepareMedicine.DataSource = lstTab3;

                    // Refresh CPA nếu cần
                    if (!string.IsNullOrEmpty(txtGateCodeString) && dteStt.DateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                    {
                        CreateThreadCallPatientRefresh();
                    }

                    LoadTab2();
                    LoadTab3();
                }

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repDeletePrinted_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = true;
            try
            {
                HIS_EXP_MEST data = (HIS_EXP_MEST)gvPrinted.GetFocusedRow();
                if (data == null) return;

                if (MessageBox.Show("Bạn có chắc muốn hủy đơn tổng hợp không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Lấy danh sách ID từ lstAll
                    List<long> expMestIds = expCodeToId(data.EXP_MEST_CODE);

                    if (expMestIds.Count == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy phiếu xuất trong danh sách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    WaitingManager.Show();

                    // Gọi API xóa từng phiếu xuất
                    foreach (var id in expMestIds)
                    {
                        var expMest = lstAll.FirstOrDefault(o => o.ID == id);
                        if (expMest != null)
                        {
                            string api = String.Empty;
                            switch (expMest.EXP_MEST_TYPE_ID)
                            {
                                case IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK:
                                    api = "api/HisExpMest/AggrExamDelete";
                                    break;
                                default:
                                    continue;
                            }

                            bool apiSuccess = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(api, ApiConsumers.MosConsumer, expMest.ID, param);
                            if (!apiSuccess)
                            {
                                success = false;
                                break; // Dừng lại nếu có lỗi
                            }
                        }
                    }

                    WaitingManager.Hide();

                    if (success)
                    {
                        // Xóa tất cả các phiếu đã xóa thành công khỏi lstAll
                        foreach (var id in expMestIds)
                        {
                            var itemToRemove = lstAll.FirstOrDefault(x => x.ID == id);
                            if (itemToRemove != null)
                            {
                                lstAll.Remove(itemToRemove);
                            }
                        }
                        LoadTab2();
                    }

                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gvPrinted_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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

        private void gvPrinted_CalcRowHeight(object sender, DevExpress.XtraGrid.Views.Grid.RowHeightEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (listResultTextTab2 != null && listResultTextTab2.Count == 1)
                    {
                        CommonParam param = new CommonParam();
                        bool success = false;
                        Approve(listResultTextTab2[0], ref param, ref success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gvPrinted_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    long? priority = (long?)view.GetRowCellValue(e.RowHandle, "PRIORITY");
                    if (priority != null && priority == 1)
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gcPrinted_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (gvPrinted.FocusedColumn == gridColumn17)
                    //{
                    //	var dataCellTreatmentCode = gvPrinted.GetRowCellValue(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle,gridColumn17);
                    //	if (dataCellTreatmentCode!=null &&!string.IsNullOrEmpty(dataCellTreatmentCode.ToString()))
                    //	{
                    //		string code = dataCellTreatmentCode.ToString().Trim();
                    //		if (code.Length < 12 && checkDigit(code))
                    //		{
                    //			code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    //			gvPrinted.SetRowCellValue(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle, gridColumn17, code);
                    //			gcPrinted_ProcessGridKey(sender, e);

                    //		}						
                    //	}
                    //}	
                    bool IsApprove = false;
                    if (gvPrinted.FocusedColumn == gridColumn17 && gvPrinted.FocusedRowHandle == DevExpress.XtraGrid.GridControl.AutoFilterRowHandle && gvPrinted.RowCount == 1)
                    {
                        IsApprove = true;
                        CommonParam param = new CommonParam();
                        bool success = false;
                        Approve((HIS_EXP_MEST)gvPrinted.GetRow(0), ref param, ref success);
                        gvPrinted.SetRowCellValue(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle, gridColumn17, "");
                    }
                    else if (gvPrinted.FocusedColumn == gridColumn16 && gvPrinted.FocusedRowHandle == DevExpress.XtraGrid.GridControl.AutoFilterRowHandle && gvPrinted.RowCount == 1)
                    {
                        IsApprove = true;
                        CommonParam param = new CommonParam();
                        bool success = false;
                        Approve((HIS_EXP_MEST)gvPrinted.GetRow(0), ref param, ref success);
                        gvPrinted.SetRowCellValue(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle, gridColumn16, "");
                    }
                    else if (gvPrinted.FocusedRowHandle != DevExpress.XtraGrid.GridControl.AutoFilterRowHandle)
                    {
                        IsApprove = true;
                        CommonParam param = new CommonParam();
                        bool success = false;
                        Approve((HIS_EXP_MEST)gvPrinted.GetFocusedRow(), ref param, ref success);
                    }
                    if (IsApprove)
                    {
                        gcPrinted.DataSource = null;
                        gcPrinted.DataSource = lstTab2;
                        gvPrinted.Focus();
                        gvPrinted.FocusedColumn = gridColumn17;
                        gvPrinted.FocusedRowHandle = DevExpress.XtraGrid.GridControl.AutoFilterRowHandle;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

    }
}
