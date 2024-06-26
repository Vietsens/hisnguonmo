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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.HisCarerCardBorrow.Validation;
using HIS.Desktop.Plugins.HisCarerCardBorrow.ADO;
using Inventec.Common.Controls.EditorLoader;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.HisCarerCardBorrow.Popup;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;

namespace HIS.Desktop.Plugins.HisCarerCardBorrow.Run
{
	public partial class UCCarerCardBorrow : UserControlBase
	{
		#region Derlare
		int rowCount = 0;
		int dataTotal = 0;
		int startPage = 0;
		int pageSize;
		Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
		int lastRowHandle = -1;
		DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
		DevExpress.Utils.ToolTipControlInfo lastInfo = null;
		#endregion
		public UCCarerCardBorrow(Inventec.Desktop.Common.Modules.Module moduleData)
		{
			InitializeComponent();
			try
			{
				this.currentModule = moduleData;
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void SetDefaultValue()
		{
			try
			{
				dteGivingFrom.EditValue = null;
				dteGivingTo.EditValue = null;
				dteReceivingFrom.EditValue = null;
				dteReceivingTo.EditValue = null;
				rdSelected.SelectedIndex = 0;
				txtCarerCardNumber.Text = String.Empty;
				txtInCode.Text = String.Empty;
				txtKeyWord.Text = String.Empty;
				txtPatientCode.Text = String.Empty;
				txtTreatmentCode.Text = String.Empty;
				txtCarerCardNumber.Focus();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void FillDataToGrid()
		{
			try
			{
				WaitingManager.Show();

				if (ucPaging.pagingGrid != null)
				{
					pageSize = ucPaging.pagingGrid.PageSize;
				}
				else
				{
					pageSize = (int)ConfigApplications.NumPageSize;
				}
				LoadGridData(new CommonParam(0, pageSize));
				CommonParam param = new CommonParam();
				param.Limit = rowCount;
				param.Count = dataTotal;
				ucPaging.Init(LoadGridData, param, pageSize, gridControl1);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
				WaitingManager.Hide();
			}
		}
		private void LoadGridData(object param)
		{
			try
			{
				startPage = ((CommonParam)param).Start ?? 0;
				int limit = ((CommonParam)param).Limit ?? 0;
				CommonParam paramCommon = new CommonParam(startPage, limit);
				ApiResultObject<List<V_HIS_CARER_CARD_BORROW>> apiResult = null;
				HisCarerCardBorrowViewFilter filter = new HisCarerCardBorrowViewFilter();
				SetFilter(ref filter);
				gridView1.BeginUpdate();
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
				apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_CARER_CARD_BORROW>>("api/HisCarerCardBorrow/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
				if (apiResult != null)
				{
					var data = apiResult.Data;
					if (data != null && data.Count > 0)
					{
						gridControl1.DataSource = data;
					}
					else
					{
						gridControl1.DataSource = null;

					}
					rowCount = (data == null ? 0 : data.Count);
					dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
				}
				else
				{
					rowCount = 0;
					dataTotal = 0;
					gridControl1.DataSource = null;
				}
				gridView1.EndUpdate();

				#region Process has exception
				HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
				#endregion
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
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
		private void SetFilter(ref HisCarerCardBorrowViewFilter filter)
		{
			try
			{
				filter.ORDER_FIELD = "MODIFY_TIME";
				filter.ORDER_DIRECTION = "DESC";
				if (!string.IsNullOrEmpty(txtCarerCardNumber.Text))
				{
					filter.CARER_CARD_NUMBER__EXACT = txtCarerCardNumber.Text.Trim();
				}
				else if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
				{
					string codeTreatment = txtTreatmentCode.Text.Trim();
					if (codeTreatment.Length < 12 && checkDigit(codeTreatment))
					{
						codeTreatment = string.Format("{0:000000000000}", Convert.ToInt64(codeTreatment));
						txtTreatmentCode.Text = codeTreatment;
					}
					filter.TREATMENT_CODE__EXACT = codeTreatment;
				}
				else if (!String.IsNullOrEmpty(txtInCode.Text))
				{
					filter.IN_CODE__EXACT = txtInCode.Text.Trim();
				}
				else if (!String.IsNullOrEmpty(txtPatientCode.Text))
				{
					string codePatient = txtPatientCode.Text.Trim();
					if (codePatient.Length < 10 && checkDigit(codePatient))
					{
						codePatient = string.Format("{0:0000000000}", Convert.ToInt64(codePatient));
						txtPatientCode.Text = codePatient;
					}
					filter.PATIENT_CODE__EXACT = codePatient;
				}
				else if (!String.IsNullOrEmpty(txtPatientName.Text))
				{
					filter.TDL_PATIENT_NAME = txtPatientName.Text.Trim();
				}
				else
					filter.KEY_WORD = txtKeyWord.Text.Trim();
				if (dteReceivingFrom.EditValue != null && dteReceivingFrom.DateTime != DateTime.MinValue)
					filter.GIVE_BACK_TIME__FROM = Int64.Parse(
						Convert.ToDateTime(dteReceivingFrom.EditValue).ToString("yyyyMMddHHmm") + "00");
				if (dteReceivingTo.EditValue != null && dteReceivingTo.DateTime != DateTime.MinValue)
					filter.GIVE_BACK_TIME__TO = Int64.Parse(
						Convert.ToDateTime(dteReceivingTo.EditValue).ToString("yyyyMMddHHmm") + "59");
				if (dteGivingFrom.EditValue != null && dteGivingFrom.DateTime != DateTime.MinValue)
					filter.BORROW_TIME__FROM = Int64.Parse(
						Convert.ToDateTime(dteGivingFrom.EditValue).ToString("yyyyMMddHHmm") + "00");
				if (dteGivingTo.EditValue != null && dteGivingTo.DateTime != DateTime.MinValue)
					filter.BORROW_TIME__TO = Int64.Parse(
						Convert.ToDateTime(dteGivingTo.EditValue).ToString("yyyyMMddHHmm") + "59");
				switch (rdSelected.SelectedIndex)
				{
					case 0:
						filter.HAS_GIVE_BACK_TIME = null;
						filter.IS_LOST = null;
						break;
					case 1:
						filter.HAS_GIVE_BACK_TIME = true;
						break;
					case 2:
						filter.HAS_GIVE_BACK_TIME = false;
						break;
					case 3:
						filter.IS_LOST = true;
						break;
					default:
						break;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
		{
			try
			{
				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
					var data = (V_HIS_CARER_CARD_BORROW)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
					if (e.Column.FieldName == "STT")
					{
						e.Value = e.ListSourceRowIndex + 1 + startPage;
					}
					else if (e.Column.FieldName == "STATUS")
					{
						if (data.IS_LOST == 1)
						{
							e.Value = imageList1.Images[2];
						}
						else if (data.GIVE_BACK_TIME == null)
						{
							e.Value = imageList1.Images[0];
						}
						else if (data.GIVE_BACK_TIME != null)
						{
							e.Value = imageList1.Images[1];
						}
						else
						{
							e.Value = "";
						}
					}
					else if (e.Column.FieldName == "BORROW_TIME_str")
					{
						e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.BORROW_TIME);
					}
					else if (e.Column.FieldName == "GIVE_BACK_TIME_str")
					{
						e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.GIVE_BACK_TIME ?? 0);
					}					
					else if (e.Column.FieldName == "GIVING_USER")
					{
						e.Value = !string.IsNullOrEmpty(data.GIVING_LOGINNAME) ? data.GIVING_LOGINNAME + " - " + data.GIVING_USERNAME : "";
					}
					else if (e.Column.FieldName == "RECEIVING_USER")
					{
						e.Value = !string.IsNullOrEmpty(data.RECEIVING_LOGINNAME) ? data.RECEIVING_LOGINNAME + " - " + data.RECEIVING_USERNAME : "";
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtTreatmentCode_Leave(object sender, EventArgs e)
		{
			try
			{
				if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
				{
					string codeTreatment = txtTreatmentCode.Text.Trim();
					if (codeTreatment.Length < 12 && checkDigit(codeTreatment))
					{
						codeTreatment = string.Format("{0:000000000000}", Convert.ToInt64(codeTreatment));
						txtTreatmentCode.Text = codeTreatment;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtPatientCode_Leave(object sender, EventArgs e)
		{
			try
			{
				if (!String.IsNullOrEmpty(txtPatientCode.Text))
				{
					string codePatient = txtPatientCode.Text.Trim();
					if (codePatient.Length < 10 && checkDigit(codePatient))
					{
						codePatient = string.Format("{0:0000000000}", Convert.ToInt64(codePatient));
						txtPatientCode.Text = codePatient;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					FillDataToGrid();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					FillDataToGrid();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					FillDataToGrid();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repbtnUser_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				var row = (V_HIS_CARER_CARD_BORROW)gridView1.GetFocusedRow();
				if (row != null)
				{
					frmCarerCardBorrow frm = new frmCarerCardBorrow(row);
					frm.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnBorrow_Click(object sender, EventArgs e)
		{
			try
			{
				frmBorrow frm = new frmBorrow((RefeshReference)FillDataToGrid,currentModule);
				frm.ShowDialog();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repbtnDeleteEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				var row = (V_HIS_CARER_CARD_BORROW)gridView1.GetFocusedRow();
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));
				if (row != null)
				{
					if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có chắc muốn xóa dữ liệu?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						CommonParam param = new CommonParam();
						WaitingManager.Show();
						if (row != null)
						{
							HisCarerCardBorrowDeleteSDO sdo = new HisCarerCardBorrowDeleteSDO();
							sdo.CarerCardBorrowId = row.ID;
							sdo.RequestRoomId = currentModule.RoomId;
							Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
							var rs = new BackendAdapter(param).Post<bool>("api/HisCarerCardBorrow/Delete", ApiConsumers.MosConsumer, sdo, param);
							Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
							if (rs)
							{
								FillDataToGrid();
								WaitingManager.Hide();
							}
							WaitingManager.Hide();
							MessageManager.Show(this.ParentForm, param, rs);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void repbtnGiveback_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				var row = (V_HIS_CARER_CARD_BORROW)gridView1.GetFocusedRow();
				if (row != null)
				{
					frmGiveBack frm = new frmGiveBack((RefeshReference)FillDataToGrid,row,currentModule);
					frm.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repbtnNotGiveback_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				var row = (V_HIS_CARER_CARD_BORROW)gridView1.GetFocusedRow();
				if (row != null)
				{
					CommonParam param = new CommonParam();
					WaitingManager.Show();
					HisCarerCardBorrowUnGiveBackSDO sdo = new HisCarerCardBorrowUnGiveBackSDO();
					sdo.CarerCardBorrowId = row.ID;
					sdo.RequestRoomId = currentModule.RoomId;
					Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
					var rs = new BackendAdapter(param).Post<HIS_CARER_CARD_BORROW>("api/HisCarerCardBorrow/UnGiveBack", ApiConsumers.MosConsumer, sdo, param);
					Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
					if (rs!=null)
					{
						FillDataToGrid();
						WaitingManager.Hide();
					}
					WaitingManager.Hide();
					MessageManager.Show(this.ParentForm, param, rs!=null);

				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repbtnLost_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				var row = (V_HIS_CARER_CARD_BORROW)gridView1.GetFocusedRow();
				if (row != null)
				{
					frmLost frm = new frmLost((RefeshReference)FillDataToGrid,row,currentModule);
					frm.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repbtnNotLost_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				var row = (V_HIS_CARER_CARD_BORROW)gridView1.GetFocusedRow();
				if (row != null)
				{
					CommonParam param = new CommonParam();
					HisCarerCardBorrowUnLostSDO sdo = new HisCarerCardBorrowUnLostSDO();
					sdo.CarerCardBorrowId = row.ID;
					sdo.RequestRoomId = currentModule.RoomId;

					Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
					var rs = new BackendAdapter(param).Post<HIS_CARER_CARD_BORROW>("api/HisCarerCardBorrow/UnLost", ApiConsumers.MosConsumer, sdo, param);
					Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
					if (rs!=null)
					{
						FillDataToGrid();
						WaitingManager.Hide();
					}
					WaitingManager.Hide();
					MessageManager.Show(this.ParentForm, param, rs!=null);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
		{
			try
			{
				DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
				if (e.RowHandle >= 0)
				{

					V_HIS_CARER_CARD_BORROW data = (V_HIS_CARER_CARD_BORROW)((IList)((BaseView)sender).DataSource)[e.RowHandle];
					if (e.Column.FieldName == "LOST")
					{
						if (data.GIVE_BACK_TIME == null && data.IS_LOST != 1 && data.TREATMENT_IS_ACTIVE == 1)
							e.RepositoryItem = repbtnLost;
						else if (data.IS_LOST == 1 && data.TREATMENT_IS_ACTIVE == 1)
							e.RepositoryItem = repbtnNotLost;
					}
					if (e.Column.FieldName == "GIVEBACK")
					{
						if (data.GIVE_BACK_TIME == null && data.IS_LOST != 1 && data.TREATMENT_IS_ACTIVE == 1)
							e.RepositoryItem = repbtnGiveback;
						else if (data.GIVE_BACK_TIME != null && data.IS_LOST != 1 && data.TREATMENT_IS_ACTIVE == 1)
							e.RepositoryItem = repbtnNotGiveback;
					}					
					if (e.Column.FieldName == "DELETE")
					{
						if (data.GIVE_BACK_TIME == null)
							e.RepositoryItem = repbtnDeleteEnable;
						else
							e.RepositoryItem = repbtnDeleteDisable;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
		{
			try
			{
				if (e.Info == null && e.SelectedControl == gridControl1)
				{
					DevExpress.XtraGrid.Views.Grid.GridView view = gridControl1.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
					GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
					if (info.InRowCell)
					{
						if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
						{
							lastColumn = info.Column;
							lastRowHandle = info.RowHandle;

							string text = "";
							if (info.Column.FieldName == "STATUS")
							{
								if (!String.IsNullOrEmpty((view.GetRowCellValue(lastRowHandle, "IS_LOST") ?? "").ToString()) && (view.GetRowCellValue(lastRowHandle, "IS_LOST") ?? "").ToString() == "1")
								{
									text = "Báo mất";
								}
								else if (view.GetRowCellValue(lastRowHandle, "GIVE_BACK_TIME") == null)
								{
									text = "Đang mượn";
								}
								else if (view.GetRowCellValue(lastRowHandle, "GIVE_BACK_TIME") != null)
								{
									text = "Đã trả";
								}
							}
							lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
						}
						e.Info = lastInfo;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			try
			{
				SetDefaultValue();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void UCCarerCardBorrow_Load(object sender, EventArgs e)
		{
			try
			{
				SetDefaultValue();
				FillDataToGrid();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnFind_Click(object sender, EventArgs e)
		{
			try
			{
				FillDataToGrid();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtCarerCardNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyData == Keys.Enter)
				{
					FillDataToGrid();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
 		}

		private void txtInCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyData == Keys.Enter)
				{
					FillDataToGrid();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtPatientName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyData == Keys.Enter)
				{
					FillDataToGrid();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
