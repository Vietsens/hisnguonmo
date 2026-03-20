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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraPrinting.Native;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary.ADO;
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

namespace HIS.Desktop.Plugins.PrepareAndExportByArea.Run
{
    public partial class frmPrepareAndExportByArea
    {
        //Danh sách chờ
        private async Task LoadTab1()
        {
            try
            {
                Action myaction = () =>
                {
                    lstTab1 = new List<HIS_EXP_MEST>();
                    // L?y danh sách treatment code có ít nh?t 1 phi?u không ph?i EXECUTE
                    var treatmentCodesWithNonExecute = lstAll
                        .Where(o => o.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        .Select(o => o.TDL_TREATMENT_CODE)
                        .Distinct()
                        .ToList();

                    var filteredData = lstAll.Where(o =>
                        o.IS_CONFIRM != 1 &&
                        (o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST ||
                         (treatmentCodesWithNonExecute != null && treatmentCodesWithNonExecute.Contains(o.TDL_TREATMENT_CODE)))
                    ).ToList();
                    // Lọc dữ liệu theo điều kiện ban đầu
                    //var filteredData = lstAll.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && o.IS_CONFIRM != 1).ToList();

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

                    lstTab1 = groupedData
                    .Select(group =>
                    {
                        var orderedItems = group
                            .OrderByDescending(o => o.PRIORITY != null && o.PRIORITY == 1 ? 1 : 0)
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
                    .OrderByDescending(o => o.PRIORITY != null && o.PRIORITY == 1 ? 1 : 0)
                    .ThenBy(o => o.NUM_ORDER)
                    .ToList();
                };
                Task task = new Task(myaction);
                task.Start();
                await task;
                gcWaiting.DataSource = null;
                if (lstTab1 != null && lstTab1.Count > 0)
                {
                    gcWaiting.DataSource = lstTab1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gvWaiting_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
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

        private void repDeleteWaiting_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = true;
            try
            {
                HIS_EXP_MEST data = (HIS_EXP_MEST)gvWaiting.GetFocusedRow();
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
                            bool apiSuccess = CallDeleteApi(expMest, ref param);
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
                        LoadTab1();
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

        private bool CallDeleteApi(HIS_EXP_MEST expMest, ref CommonParam param)
        {
            try
            {
                string api = String.Empty;
                switch (expMest.EXP_MEST_TYPE_ID)
                {
                    case IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK:
                        api = "api/HisExpMest/AggrExamDelete";
                        break;
                    default:
                        return false;
                }

                return new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(api, ApiConsumers.MosConsumer, expMest.ID, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            ExpMestDetailResultSDO sdo;
            bool success = false;
            try
            {
                if (lstTab1 != null && lstTab1.Count > 0)
                {
                    // Lấy dòng đầu tiên
                    HIS_EXP_MEST firstExpMest = lstTab1.First();

                    // Lấy tất cả phiếu xuất cùng nhóm (cùng TDL_TREATMENT_CODE, IS_CONFIRM, EXP_MEST_STT_ID)
                    List<long> groupedExpMestIds = expCodeToId(firstExpMest.EXP_MEST_CODE);

                    if (groupedExpMestIds.Count == 0)
                    {
                        LogSystem.Debug("Không tìm thấy phiếu xuất nào");
                        return;
                    }

                    dataPrintMps480 = firstExpMest;

                    WaitingManager.Show();

                    sdo = new Inventec.Common.Adapter.BackendAdapter(param)
                        .Post<ExpMestDetailResultSDO>("api/HisExpMest/ConfirmAndGetDetails",
                                                       ApiConsumers.MosConsumer,
                                                       groupedExpMestIds,
                                                       param);

                    WaitingManager.Hide();

                    if (sdo != null)
                    {
                        dataPrintMps480 = sdo.ExpMest;
                        lstExpMestMedicine = sdo.ExpMestMedicines;
                        lstExpMestMaterial = sdo.ExpMestMaterials;
                        lstVExpMest = sdo.ViewExpMests;

                        // Cập nhật tất cả các phiếu trong nhóm
                        foreach (var expMestId in groupedExpMestIds)
                        {
                            var item = lstAll.FirstOrDefault(x => x.ID == expMestId);
                            if (item != null)
                            {
                                item.IS_CONFIRM = 1;

                                // Thêm vào Tab 2 (Đã in) nếu chưa có
                                if (lstTab2 == null)
                                    lstTab2 = new List<HIS_EXP_MEST>();

                                if (!lstTab2.Any(x => x.ID == item.ID))
                                {
                                    lstTab2.Add(item);
                                }
                            }
                        }

                        // Reload tab 2
                        gcPrinted.DataSource = null;
                        gcPrinted.DataSource = lstTab2;

                        // Lấy thông tin treatment
                        HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                        if (dataPrintMps480 != null && dataPrintMps480.TDL_TREATMENT_ID != null)
                        {
                            treatmentFilter.ID = dataPrintMps480.TDL_TREATMENT_ID;
                        }
                        else if (lstExpMestMedicine != null && lstExpMestMedicine.Count > 0)
                        {
                            treatmentFilter.ID = lstExpMestMedicine.FirstOrDefault(o => o.TDL_TREATMENT_ID != null)?.TDL_TREATMENT_ID;
                        }
                        else if (lstExpMestMaterial != null && lstExpMestMaterial.Count > 0)
                        {
                            treatmentFilter.ID = lstExpMestMaterial.FirstOrDefault(o => o.TDL_TREATMENT_ID != null)?.TDL_TREATMENT_ID;
                        }

                        if (treatmentFilter.ID != null)
                        {
                            List<HIS_TREATMENT> lstTreatment = new Inventec.Common.Adapter.BackendAdapter(param)
                                .Get<List<HIS_TREATMENT>>("api/HisTreatment/Get",
                                                           ApiConsumer.ApiConsumers.MosConsumer,
                                                           treatmentFilter,
                                                           param);
                            if (lstTreatment != null && lstTreatment.Count > 0)
                            {
                                treatment = lstTreatment.FirstOrDefault();
                            }
                        }

                        success = true;

                        // Reload Tab 1 để remove các item đã xử lý
                        LoadTab1();
                        LoadTab2();

                        IsPrintNow = true;
                        PrintMps480();
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
        private void PrintMps480()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate("Mps000480", this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000480":
                        LoadBieuMauMps480(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadBieuMauMps480(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataPrintMps480), dataPrintMps480));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstExpMestMedicine), lstExpMestMedicine));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstExpMestMaterial), lstExpMestMaterial));
                MPS.Processor.Mps000480.PDO.Mps000480PDO pdo = new MPS.Processor.Mps000480.PDO.Mps000480PDO(
                    dataPrintMps480,
                    lstExpMestMedicine,
                    lstExpMestMaterial,
                    treatment,
                    lstVExpMest
                    );
                WaitingManager.Hide();
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(dataPrintMps480 != null ? dataPrintMps480.TDL_TREATMENT_CODE : treatment.TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                if (IsPrintNow)
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                else
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void repPrintWaiting_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            ExpMestDetailResultSDO sdo;
            bool success = false;
            try
            {
                dataPrintMps480 = (HIS_EXP_MEST)gvWaiting.GetFocusedRow();
                WaitingManager.Show();
                // Lấy dòng đầu tiên

                // Lấy tất cả phiếu xuất cùng nhóm (cùng TDL_TREATMENT_CODE, IS_CONFIRM, EXP_MEST_STT_ID)
                List<long> groupedExpMestIds = expCodeToId(dataPrintMps480.EXP_MEST_CODE);

                if (groupedExpMestIds.Count == 0)
                {
                    LogSystem.Debug("Không tìm thấy phiếu xuất nào");
                    return;
                }

                WaitingManager.Show();

                sdo = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Post<ExpMestDetailResultSDO>("api/HisExpMest/ConfirmAndGetDetails",
                                                   ApiConsumers.MosConsumer,
                                                   groupedExpMestIds,
                                                   param);

                WaitingManager.Hide();

                if (sdo != null)
                {
                    dataPrintMps480 = sdo.ExpMest;
                    lstExpMestMedicine = sdo.ExpMestMedicines;
                    lstExpMestMaterial = sdo.ExpMestMaterials;
                    lstVExpMest = sdo.ViewExpMests;

                    // Cập nhật tất cả các phiếu trong nhóm
                    foreach (var expMestId in groupedExpMestIds)
                    {
                        var item = lstAll.FirstOrDefault(x => x.ID == expMestId);
                        if (item != null)
                        {
                            item.IS_CONFIRM = 1;

                            // Thêm vào Tab 2 (Đã in) nếu chưa có
                            if (lstTab2 == null)
                                lstTab2 = new List<HIS_EXP_MEST>();

                            if (!lstTab2.Any(x => x.ID == item.ID))
                            {
                                lstTab2.Add(item);
                            }
                        }
                    }

                    // Reload tab 2
                    gcPrinted.DataSource = null;
                    gcPrinted.DataSource = lstTab2;

                    // Lấy thông tin treatment
                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    if (dataPrintMps480 != null && dataPrintMps480.TDL_TREATMENT_ID != null)
                    {
                        treatmentFilter.ID = dataPrintMps480.TDL_TREATMENT_ID;
                    }
                    else if (lstExpMestMedicine != null && lstExpMestMedicine.Count > 0)
                    {
                        treatmentFilter.ID = lstExpMestMedicine.FirstOrDefault(o => o.TDL_TREATMENT_ID != null)?.TDL_TREATMENT_ID;
                    }
                    else if (lstExpMestMaterial != null && lstExpMestMaterial.Count > 0)
                    {
                        treatmentFilter.ID = lstExpMestMaterial.FirstOrDefault(o => o.TDL_TREATMENT_ID != null)?.TDL_TREATMENT_ID;
                    }

                    if (treatmentFilter.ID != null)
                    {
                        List<HIS_TREATMENT> lstTreatment = new Inventec.Common.Adapter.BackendAdapter(param)
                            .Get<List<HIS_TREATMENT>>("api/HisTreatment/Get",
                                                       ApiConsumer.ApiConsumers.MosConsumer,
                                                       treatmentFilter,
                                                       param);
                        if (lstTreatment != null && lstTreatment.Count > 0)
                        {
                            treatment = lstTreatment.FirstOrDefault();
                        }
                    }

                    success = true;

                    // Reload Tab 1 để remove các item đã xử lý
                    LoadTab1();
                    LoadTab2();
                    IsPrintNow = true;
                    PrintMps480();
                }
                MessageManager.Show(this.ParentForm, param, success);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gvWaiting_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
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


        private void gcWaiting_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (gvWaiting.FocusedColumn == gridColumn6)
                    //{
                    //	var dataCellTreatmentCode = gvWaiting.GetRowCellValue(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle, gridColumn6);
                    //	if (dataCellTreatmentCode != null && !string.IsNullOrEmpty(dataCellTreatmentCode.ToString()))
                    //	{
                    //		string code = dataCellTreatmentCode.ToString().Trim();
                    //		if (code.Length < 12 && checkDigit(code))
                    //		{
                    //			code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    //			gvWaiting.SetRowCellValue(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle, gridColumn6, code);
                    //			gcWaiting_ProcessGridKey(sender,e);
                    //		}
                    //	}
                    //}					
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
