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

namespace HIS.Desktop.Plugins.PrepareAndExport.Run
{
	public partial class frmPrepareAndExport
	{
		private List<HIS_EXP_MEST> listResultTextTab2 { get; set; }

		private async Task LoadTab2()
		{
			try
			{
				Action myaction = () =>
				{
					lstTab2 = new List<HIS_EXP_MEST>();
					lstTab2.AddRange(lstAll.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && o.IS_CONFIRM == 1 && o.PRIORITY > 0).OrderByDescending(o => o.PRIORITY).ThenBy(o => o.NUM_ORDER).ToList());
					lstTab2.AddRange(lstAll.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && o.IS_CONFIRM == 1 && (o.PRIORITY == 0 || o.PRIORITY == null)).OrderBy(o => o.NUM_ORDER).ToList());
				};
				Task task = new Task(myaction);
				task.Start();
				await task;
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
		}

		private void repPrintPrinted_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				dataPrintMps480= (HIS_EXP_MEST)gvPrinted.GetFocusedRow();
				CommonParam param = new CommonParam();
				HisExpMestMedicineViewFilter mrCheckFilter = new HisExpMestMedicineViewFilter();
				mrCheckFilter.AGGR_EXP_MEST_ID = dataPrintMps480.ID;
				lstExpMestMedicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, mrCheckFilter, param);

				HisExpMestMaterialViewFilter mtCheckFilter = new HisExpMestMaterialViewFilter();
				mtCheckFilter.AGGR_EXP_MEST_ID = dataPrintMps480.ID;
				lstExpMestMaterial = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, mtCheckFilter, param);
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
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
				Approve(data,ref param,ref success);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void Approve(HIS_EXP_MEST data,ref CommonParam param, ref bool success)
		{
			try
			{
				HisExpMestSDO sdo = new HisExpMestSDO();
				sdo.ExpMestId = data.ID;
				sdo.ReqRoomId = currentModule.RoomId;
				WaitingManager.Show();
				string api = String.Empty;
				switch (data.EXP_MEST_TYPE_ID)
				{
					case IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK:
						api = "api/HisExpMest/AggrExamApprove";
						break;
					case IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL:
						api = "api/HisExpMest/AggrApprove";
						break;
				}
				List<HIS_EXP_MEST> rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_EXP_MEST>>(api, ApiConsumers.MosConsumer, sdo, param);
				WaitingManager.Hide();
				if (rs != null && rs.Count > 0)
				{
					success = true;
					foreach (var item in lstAll)
					{
						if (item.ID == data.ID)
						{
							item.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
							if (lstTab3 == null || lstTab3.Count == 0)
								lstTab3 = new List<HIS_EXP_MEST>();
							lstTab3.Add(item);
							gcPrepareMedicine.DataSource = null;
							gcPrepareMedicine.DataSource = lstTab3;
							if (!string.IsNullOrEmpty(txtGateCodeString) && dteStt.DateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
							{
								CreateThreadCallPatientRefresh();
							}
							break;
						}
					}
					LoadTab2();
				}
				MessageManager.Show(this.ParentForm, param, success);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repDeletePrinted_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			CommonParam param = new CommonParam();
			bool success;
			try
			{

				HIS_EXP_MEST data = (HIS_EXP_MEST)gvPrinted.GetFocusedRow();
				if (MessageBox.Show("Bạn có chắc muốn hủy đơn tổng hợp không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					WaitingManager.Show();
					string api = String.Empty;
					switch (data.EXP_MEST_TYPE_ID)
					{
						case IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK:
							api = "api/HisExpMest/AggrExamDelete";
							break;
						case IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL:
							api = "api/HisExpMest/AggrDelete";
							break;
					}
					success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(api, ApiConsumers.MosConsumer, data.ID, param);
					WaitingManager.Hide();
					if (success)
					{
						lstAll.Remove(data);
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
					if(listResultTextTab2!=null && listResultTextTab2.Count == 1)
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
					if (priority != null & priority == 1)
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
