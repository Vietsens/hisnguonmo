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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.HisMediPartList.Resources;

namespace HIS.Desktop.Plugins.HisMediPartList
{
	public partial class frmHisMediPartList : HIS.Desktop.Utility.FormBase
	{
		#region global
		Inventec.Desktop.Common.Modules.Module moduleData;
		MOS.EFMODEL.DataModels.V_HIS_MEDI_PART currentData;
		MOS.EFMODEL.DataModels.HIS_MEDI_PART resultData;
		int positionHandle = -1;
		int ActionType = -1;
		private const short IS_ACTIVE_TRUE = 1;
		private const short IS_ACTIVE_FALSE = 0;
		int rowCount;
		int dataTotal;
		DelegateSelectData delegateSelect = null;
		internal long id;
		int startPage;
		int limit;
		public static ResourceManager LanguageResource { get; set; }
		#endregion

		#region loadData
		public frmHisMediPartList(Inventec.Desktop.Common.Modules.Module moduleData)
		:base(moduleData)
		{
			// TODO: Complete member initialization
			try
			{
				InitializeComponent();
				this.moduleData = moduleData;
				SetIcon();
				SetCaptionByLanguageKey();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void frmHisMediPartList_Load(object sender, EventArgs e)
		{
			try
			{
				InitComboCashierRoom(lueSelect);
				FillDataToGridControl();
				SetDefaultValue();
				enableControlChanged(this.ActionType);
				setDefaultFocus();
				ValidateForm();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		#endregion

		#region action
		private void btnSearch_Click(object sender, EventArgs e)
		{
			FillDataToGridControl();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			try
			{
				saveProcess();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
			try
			{
				saveProcess();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnNull_Click(object sender, EventArgs e)
		{
			this.ActionType = GlobalVariables.ActionAdd;
			enableControlChanged(this.ActionType);
			positionHandle = -1;
			Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
				(dxValidationProviderEditorInfo, dxErrorProvider);
			resetFormData();
			txtId.Focus();
		}
		#endregion

		#region event
		private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
		{
			try
			{
				if (e.RowHandle >= 0)
				{
					V_HIS_MEDI_PART data = (V_HIS_MEDI_PART)gridviewFormList.GetRow(e.RowHandle);
					if (e.Column.FieldName == "Lock")
					{
						e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_TRUE ? btnUnlock : btnLock);
					}
					if (e.Column.FieldName == "Delete")
					{
						if (data.IS_ACTIVE == IS_ACTIVE_TRUE)
						{
							e.RepositoryItem = btnDelete;
						}
						else
						{
							e.RepositoryItem = btnUndelete;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridControlFormList_Click(object sender, EventArgs e)
		{
			try
			{
				currentData = new V_HIS_MEDI_PART();
				currentData = (MOS.EFMODEL.DataModels.V_HIS_MEDI_PART)gridviewFormList.GetFocusedRow();
				if (currentData != null)
				{
					ChangedDataRow(currentData);
					setDefaultFocus();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
		{
			try
			{
				DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
				V_HIS_MEDI_PART data = (V_HIS_MEDI_PART)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

				if (e.Column.FieldName == "STT")
				{
					e.Value = e.ListSourceRowIndex + 1 + startPage;
				}

				if (e.Column.FieldName == "status")
				{
					if (data.IS_ACTIVE == IS_ACTIVE_TRUE)
					{
						e.Value = "Đang hoạt động";
					}
					else
					{
						e.Value = "Đã khóa";
					}
				}
				if (e.Column.FieldName == "CREATE_TIME_STR")
				{
					try
					{
						string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
						e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

					}
					catch (Exception ex)
					{
						Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
					}
				}
				else if (e.Column.FieldName == "MODIFY_TIME_STR")
				{
					try
					{
						string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
						e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

					}
					catch (Exception ex)
					{
						Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			CommonParam param = new CommonParam();
			V_HIS_MEDI_PART hisDepart = new V_HIS_MEDI_PART();
			bool notHandler = false;

			try
			{
				V_HIS_MEDI_PART dataDepart = (V_HIS_MEDI_PART)gridviewFormList.GetFocusedRow();
				if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					V_HIS_MEDI_PART dataNew = new V_HIS_MEDI_PART();
					dataNew.ID = dataDepart.ID;
					WaitingManager.Show();
					hisDepart = new BackendAdapter(param).Post<V_HIS_MEDI_PART>(RequestUriStore.HIS_MEDI_PART_CHANGELOCK, ApiConsumers.MosConsumer, dataNew, param);

					notHandler = true;
					MessageManager.Show(this.ParentForm, param, notHandler);
					WaitingManager.Hide();
				}

				FillDataToGridControl();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			CommonParam param = new CommonParam();
			V_HIS_MEDI_PART hisDepart = new V_HIS_MEDI_PART();
			bool notHandler = false;

			try
			{
				V_HIS_MEDI_PART dataDepart = (V_HIS_MEDI_PART)gridviewFormList.GetFocusedRow();
				if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					V_HIS_MEDI_PART dataNew = new V_HIS_MEDI_PART();
					dataNew.ID = dataDepart.ID;
					WaitingManager.Show();
					hisDepart = new BackendAdapter(param).Post<V_HIS_MEDI_PART>(RequestUriStore.HIS_MEDI_PART_CHANGELOCK, ApiConsumers.MosConsumer, dataNew, param);

					notHandler = true;
					MessageManager.Show(this.ParentForm, param, notHandler);
					WaitingManager.Hide();
				}

				FillDataToGridControl();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDI_PART)gridviewFormList.GetFocusedRow();
				if (rowData != null)
				{
					bool success = false;
					CommonParam param = new CommonParam();
					if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						success = new BackendAdapter(param).Post<bool>(RequestUriStore.HIS_MEDI_PART_DELETE,
						ApiConsumers.MosConsumer, rowData, param);
						if (success)
						{
							FillDataToGridControl();
						}
						MessageManager.Show(this, param, success);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtSearch_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					btnSearch_Click(null, null);
				}
				else if (e.KeyCode == Keys.Down)
				{
					gridviewFormList.Focus();
					gridviewFormList.FocusedRowHandle = 0;
					var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDI_PART)gridviewFormList.GetFocusedRow();
					if (rowData != null)
					{
						ChangedDataRow(rowData);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
		{
			DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
			if (e.RowHandle >= 0)
			{
				V_HIS_MEDI_PART data = (V_HIS_MEDI_PART)((IList)((BaseView)sender).DataSource)[e.RowHandle];
				if (e.Column.FieldName == "status")
				{
					if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
						e.Appearance.ForeColor = Color.Red;
					else
						e.Appearance.ForeColor = Color.Green;
				}
			}
		}

		private void txtId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					txtName.Focus();
					txtName.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtId_KeyPress(object sender, KeyPressEventArgs e)
		{
			try
			{
				if ((int)e.KeyChar > 127)
				{
					e.Handled = true;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					lueSelect.Focus();
					lueSelect.ShowPopup();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		#endregion

		#region private
		private void FillDataToGridControl()
		{
			try
			{
				WaitingManager.Show();
				loadPaging(new CommonParam(0, (ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize)));

				CommonParam param = new CommonParam();
				param.Limit = rowCount;
				param.Count = dataTotal;
				ucPaging.Init(loadPaging, param, ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize);

				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void loadPaging(object param)
		{
			try
			{
				startPage = ((CommonParam)param).Start ?? 0;
				limit = ((CommonParam)param).Limit ?? 0;
				CommonParam paramCommon = new CommonParam(startPage, limit);



				//var dataGrid = new BackendAdapter(paramCommon).Get<List<V_HIS_MEDI_PART>>
				//  (RequestUriStore.HIS_MEDI_PART_GET_VIEW, ApiConsumers.MosConsumer, filterMP, paramCommon);
				//gridviewFormList.GridControl.DataSource = dataGrid;

				


				Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_MEDI_PART>> apiResult = null;
				MOS.Filter.HisMediPartViewFilter filterMP = new HisMediPartViewFilter();
				SetFilterNavBar(ref filterMP);
				apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MEDI_PART>>
					(RequestUriStore.HIS_MEDI_PART_GET_VIEW, ApiConsumers.MosConsumer, filterMP, paramCommon);
				if (apiResult != null)
				{
					var data = (List<MOS.EFMODEL.DataModels.V_HIS_MEDI_PART>)apiResult.Data;
					if (data != null)
					{
						gridviewFormList.GridControl.DataSource = data;
						rowCount = (data == null ? 0 : data.Count);
						dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitComboCashierRoom(Control cboCashierRoom)
		{
			try
			{
				CommonParam paramCommon = new CommonParam();

				MOS.Filter.HisMediStockViewFilter filterMS = new HisMediStockViewFilter();
				filterMS.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

				var dataLue = new BackendAdapter(paramCommon).Get<List<V_HIS_MEDI_STOCK>>
				(RequestUriStore.HIS_MEDI_STOCK_GET_VIEW, ApiConsumers.MosConsumer, filterMS, paramCommon);
				
				//data = data.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
				//foreach (var item in data)
				//{
				//	if (item.IS_ACTIVE == IS_ACTIVE_TRUE)
				//	{
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();

				columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
				columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));

				ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
				//ControlEditorLoader.Load(cboCashierRoom, data.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
				ControlEditorLoader.Load(cboCashierRoom, dataLue, controlEditorADO);
				//	}
				//	else
				//	{
				//		return;
				//	}
				//}				
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void saveProcess()
		{
			CommonParam param = new CommonParam();
			try
			{
				bool success = false;
				if (!btnEdit.Enabled && !btnAdd.Enabled)
					return;

				positionHandle = -1;
				if (!dxValidationProviderEditorInfo.Validate())
					return;

				WaitingManager.Show();

				//bool str = txtId.Text.IsNormalized(NormalizationForm.FormD);
				//if (!str)
				//{
				//	MessageBox.Show("Mã sai định dạng" + "\n" + "Không được nhập có dấu", "Thông báo");
				//	txtId.Focus();
				//	txtId.SelectAll();
				//	return;
				//}

				MOS.EFMODEL.DataModels.HIS_MEDI_PART updateDTO = new MOS.EFMODEL.DataModels.HIS_MEDI_PART();

				if (this.currentData != null && this.currentData.ID > 0)
				{
					loadCurrent(currentData.ID, ref updateDTO);
				}
				updateDTOFromDataForm(ref updateDTO);
				if (ActionType == GlobalVariables.ActionAdd)
				{
					updateDTO.IS_ACTIVE = IS_ACTIVE_TRUE;
					resultData = new BackendAdapter(param).
						Post<MOS.EFMODEL.DataModels.HIS_MEDI_PART>(RequestUriStore.HIS_MEDI_PART_CREATE, ApiConsumers.MosConsumer, updateDTO, param);

					if (resultData != null)
					{
						success = true;
						FillDataToGridControl();
						resetFormData();
					}
				}
				else
				{
					if (updateDTO != null)
					{
						resultData = new BackendAdapter(param).
							Post<MOS.EFMODEL.DataModels.HIS_MEDI_PART>
							(RequestUriStore.HIS_MEDI_PART_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);

						if (resultData != null)
						{
							success = true;
							FillDataToGridControl();
						}
					}
				}

				if (success)
					setDefaultFocus();
				WaitingManager.Hide();

				MessageManager.Show(this, param, success);

				txtId.Focus();
				txtId.SelectAll();

				SessionManager.ProcessTokenLost(param);
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void loadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_MEDI_PART currentDTO)
		{
			try
			{
				CommonParam param = new CommonParam();
				HisMediPartFilter filter = new HisMediPartFilter();
				filter.ID = currentId;
				currentDTO = new BackendAdapter(param).
					Get<List<MOS.EFMODEL.DataModels.HIS_MEDI_PART>>
					(RequestUriStore.HIS_MEDI_PART_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void updateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_MEDI_PART currentDTO)
		{
			try
			{
				currentDTO.MEDI_PART_CODE = txtId.Text.Trim();
				currentDTO.MEDI_PART_NAME = txtName.Text.Trim();
				currentDTO.MEDI_STOCK_ID = (long)lueSelect.EditValue;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void resetFormData()
		{
			try
			{
				if (!lcEditInfo.IsInitialized) return;
				lcEditInfo.BeginUpdate();

				foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditInfo.Items)
				{
					DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
					if (lci != null && lci.Control != null && lci.Control is BaseEdit)
					{
						DevExpress.XtraEditors.BaseEdit formatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
						formatFrm.ResetText();
						formatFrm.EditValue = null;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			finally
			{
				lcEditInfo.EndUpdate();
			}
		}
		private void updateDataAfterEdit(MOS.EFMODEL.DataModels.HIS_MEDI_PART data)
		{
			try
			{
				if (data == null)
					throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_MEDI_PART) is null");
				var rowData = (MOS.EFMODEL.DataModels.HIS_MEDI_PART)gridviewFormList.GetFocusedRow();
				if (rowData != null)
				{
					Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_MEDI_PART>(rowData, data);
					gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void RefeshDataAfterSave()
		{
			try
			{
				if (this.delegateSelect != null)
				{
					this.delegateSelect(resultData);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void SetFilterNavBar(ref HisMediPartViewFilter filter)
		{
			try
			{
				filter.KEY_WORD = txtSearch.Text.Trim();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_MEDI_PART data)
		{
			try
			{
				if (data != null)
				{
					fillDataToEditControl(data);
					this.ActionType = GlobalVariables.ActionEdit;
					enableControlChanged(this.ActionType);

					btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
					positionHandle = -1;
					Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
						(dxValidationProviderEditorInfo, dxErrorProvider);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void fillDataToEditControl(MOS.EFMODEL.DataModels.V_HIS_MEDI_PART data)
		{
			try
			{
				if (data != null)
				{
					this.id = data.ID;
					txtId.Text = Convert.ToString(data.MEDI_PART_CODE);
					lueSelect.EditValue = data.MEDI_STOCK_ID;
					txtName.Text = data.MEDI_PART_NAME;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void enableControlChanged(int action)
		{
			try
			{
				btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
				btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void setDefaultFocus()
		{
			try
			{
				txtId.Focus();
				txtId.SelectAll();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetDefaultValue()
		{
			try
			{
				this.ActionType = GlobalVariables.ActionAdd;
				txtSearch.Text = "";
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		#endregion

		#region validate
		private void ValidateForm()
		{
			try
			{
				ValidationSingleControl(txtId);
				ValidationSingleControl(txtName);
				ValidationSingleControl(lueSelect);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
		{
			try
			{
				LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
				validRule.txtTextEdit = textEdit;
				validRule.cbo = cbo;
				validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
				validRule.ErrorType = ErrorType.Warning;
				dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
		{
			try
			{
				GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
				validRule.txtTextEdit = textEdit;
				validRule.cbo = cbo;
				validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
				validRule.ErrorType = ErrorType.Warning;
				dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidationSingleControl(BaseEdit control)
		{
			try
			{
				ControlEditValidationRule validRule = new ControlEditValidationRule();
				validRule.editor = control;
				validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
				validRule.ErrorType = ErrorType.Warning;
				dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		#endregion

		private void itemSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				btnSearch_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void itemEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				btnEdit_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void itemAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			btnAdd_Click(null, null);
		}

		private void itemNull_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			btnNull_Click(null, null);
		}

		private void SetCaptionByLanguageKey()
		{
			try
			{
				

				//Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
				//TODO				
				////Khoi tao doi tuong resource
				ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisMediPartList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisMediPartList.frmHisMediPartList).Assembly);

				////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
				this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisMediPartList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisMediPartList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lcEditInfo.Text = Inventec.Common.Resource.Get.Value("frmHisMediPartList.lcEditInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lueSelect.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMediPartList.lueSelect.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisMediPartList.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnNull.Text = Inventec.Common.Resource.Get.Value("frmHisMediPartList.btnNull.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisMediPartList.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisMediPartList.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisMediPartList.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisMediPartList.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHisMediPartList.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.bar2.Text = Inventec.Common.Resource.Get.Value("frmHisMediPartList.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.itemSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.itemSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.itemEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.itemEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.itemAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.itemAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.itemNull.Caption = Inventec.Common.Resource.Get.Value("frmHisMediPartList.itemNull.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.Text = Inventec.Common.Resource.Get.Value("frmHisMediPartList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
				{
					this.Text = this.moduleData.text;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetIcon()
		{
			try
			{
				this.Icon = Icon.ExtractAssociatedIcon
					(System.IO.Path.Combine
					(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
					System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
			}
			catch (Exception ex)
			{
				LogSystem.Warn(ex);
			}
		}

		private void lueSelect_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
		{
			try
			{
				if (e.CloseMode == PopupCloseMode.Normal)
				{
					if (this.ActionType == GlobalVariables.ActionAdd)
						btnAdd.Focus();
					if (this.ActionType == GlobalVariables.ActionEdit)
						btnEdit.Focus();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
