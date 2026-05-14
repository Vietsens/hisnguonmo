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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using MOS.SDO;

namespace HIS.Desktop.Plugins.PrepareAndExportByArea.Run
{
	public partial class frmPrepareAndExportByArea
	{
        private async Task LoadTab5()
        {
            try
            {
                Action myaction = () =>
                {
                    lstTab5 = new List<HIS_EXP_MEST>();
                    
                    // Lọc dữ liệu theo điều kiện
                    var filteredData = lstAll.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE).ToList();
                    
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

                    lstTab5 = groupedData
                    .Select(group =>
                    {
                        // Trong mỗi nhóm, sắp xếp theo PRIORITY và NUM_ORDER
                        var orderedItems = group.OrderByDescending(o => o.PRIORITY != null && o.PRIORITY == 1 ? 1 : 0)
                            .ThenByDescending(o => o.PRIORITY)
                            .ThenBy(o => o.NUM_ORDER)
                            .ToList(); ;

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
                            EXP_MEST_TYPE_ID = first.EXP_MEST_TYPE_ID,
                            IS_ABSENT = first.IS_ABSENT,
                            IS_HTX = first.IS_HTX,
                        };
                    })
                    .OrderByDescending(o => o.PRIORITY != null && o.PRIORITY == 1 ? 1 : 0)
                    .ThenBy(o => o.NUM_ORDER)
                    .ToList();
                };
				Task task = new Task(myaction);
				task.Start();
				await task;
				gcPassMedicine.DataSource = null;
				if (lstTab5 != null && lstTab5.Count > 0)
				{
					gcPassMedicine.DataSource = lstTab5;
				}
                gcPassMedicine.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        gvPassMedicine.Focus();
                        gvPassMedicine.FocusedRowHandle = DevExpress.XtraGrid.GridControl.AutoFilterRowHandle;

                        gvPassMedicine.FocusedColumn = gridColumn6;

                        gvPassMedicine.ShowEditor();
                        var ed = gvPassMedicine.ActiveEditor as DevExpress.XtraEditors.BaseEdit;
                        ed?.SelectAll();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }));
            }
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gvPassMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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

        private void repMinusN_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HIS_EXP_MEST data = (HIS_EXP_MEST)gvPassMedicine.GetFocusedRow();
                if (data != null)
                {
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
                    string api = "api/HisExpMest/AggrExamUnExportList";
                    List<V_HIS_EXP_MEST> rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<V_HIS_EXP_MEST>>(api, ApiConsumers.MosConsumer, sdo, param);
                    WaitingManager.Hide();
                    if (rs != null)
                    {
                        success = true;

                        // Cập nhật trạng thái cho tất cả phiếu trong nhóm
                        foreach (var expMestId in groupedExpMestIds)
                        {
                            var item = lstAll.FirstOrDefault(x => x.ID == expMestId);
                            if (item != null)
                            {
                                var rsItem = rs.FirstOrDefault(x => x.ID == item.ID);
                                if(rsItem != null)
                                {
                                    item.EXP_MEST_STT_ID = rsItem.EXP_MEST_STT_ID;
                                }
                                item.IS_ABSENT = rsItem.IS_ABSENT;
                                if (item.IS_ABSENT != 1)
                                {
                                    if (lstTab3 == null || lstTab3.Count == 0)
                                        lstTab3 = new List<HIS_EXP_MEST>();
                                    lstTab3.Add(item);
                                    gcPrepareMedicine.DataSource = null;
                                    gcPrepareMedicine.DataSource = lstTab3;
                                }
                                else if (item.IS_ABSENT == 1)
                                {
                                    if (lstTab4 == null || lstTab4.Count == 0)
                                        lstTab4 = new List<HIS_EXP_MEST>();
                                    lstTab4.Add(item);
                                    gcAbssentN.DataSource = null;
                                    gcAbssentN.DataSource = lstTab4;
                                }
                            }
                        }

                        // Reload tab 5
                        gcPassMedicine.DataSource = null;
                        gcPassMedicine.DataSource = lstTab5;
                        if (!string.IsNullOrEmpty(txtGateCodeString) && dteStt.DateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                        {
                            CreateThreadCallPatientRefresh();
                        }
                        LoadTab3();
                        LoadTab4();
                        LoadTab5();
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

		private void gvPassMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
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

	}
}
