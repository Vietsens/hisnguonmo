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
using Inventec.UC.Paging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LIS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using LIS.Filter;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraEditors.ViewInfo;
using LIS.Desktop.Plugins.LisAntibioticRange.ADO;
using HIS.Desktop.LocalStorage.BackendData;

namespace LIS.Desktop.Plugins.LisAntibioticRange.Run
{
	public partial class frmLisAntibioticRange : HIS.Desktop.Utility.FormBase
	{
		#region Declare
		int rowCount = 0;
		int dataTotal = 0;
		int startPage = 0;
		PagingGrid pagingGrid;
		int ActionType = -1;
		int positionHandle = -1;
		V_LIS_ANTIBIOTIC_RANGE currentData;
		List<string> arrControlEnableNotChange = new List<string>();
		Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
		Inventec.Desktop.Common.Modules.Module moduleData;

		List<LIS_ANTIBIOTIC> datasAntibiotic = null;
		List<LIS_BACTERIUM> datasBacterium = null;
		List<LIS_TECHNIQUE> datasTechnique = null;
		List<BacAntibioticADO> lstADOAntibiotic = new List<BacAntibioticADO>();
		List<BacAntibioticADO> lstADOBacterium = new List<BacAntibioticADO>();
		#endregion

		public frmLisAntibioticRange(Inventec.Desktop.Common.Modules.Module moduleData) : base(moduleData)
		{
			InitializeComponent();
			pagingGrid = new PagingGrid();
			this.moduleData = moduleData;
			try
			{
				string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
				this.Icon = Icon.ExtractAssociatedIcon(iconPath);
			}
			catch (Exception ex)
			{
				LogSystem.Warn(ex);
			}
		}

		private void frmLisAntibioticRange_Load(object sender, EventArgs e)
		{
			try
			{
				LoadListAntibiotic();
				LoadListBacterium();
				Show();
			}
			catch (Exception ex)
			{

				LogSystem.Warn(ex);
			}
		}
		private void Show()
		{
			SetDefaultValue();
			//Focus default
			SetDefaultFocus();
			EnableControlChanged(this.ActionType);

			FillDataToControl();
			FillDataToControlsForm();

			// kiem tra du lieu nhap vao
			ValidateForm();


			//Set tabindex control
			InitTabIndex();
		}

		private void SetDefaultValue()
		{
			try
			{
				InitCombo();
				this.ActionType = GlobalVariables.ActionAdd;
				txtSearch.Text = "";
				txtActibiotic.Text = "";
				txtBacterium.Text = "";
				txtMaxValue.Text = "";
				txtMinValue.Text = "";
				txtSriValue.Text = "";
				cboAntibiotic.EditValue = null;
				cboBacterium.EditValue = null;
				cboTechnique.EditValue = null;
				chkEqualMin.Checked = false;
				chkEqualMax.Checked = false;
				Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
			}
			catch (Exception ex)
			{

				LogSystem.Warn(ex);
			}
		}

		private void SetDefaultFocus()
		{
			try
			{
				txtSearch.Focus();
				txtSearch.SelectAll();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Debug(ex);
			}
		}

		private void EnableControlChanged(int action)
		{
			btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
			btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
		}

		private void FillDataToControl()
		{
			try
			{
				WaitingManager.Show();


				int pageSize = 0;
				if (ucPaging1.pagingGrid != null)
				{
					pageSize = ucPaging1.pagingGrid.PageSize;
				}
				else
				{
					pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
				}

				LoadPaging(new CommonParam(0, pageSize));

				CommonParam param = new CommonParam();
				param.Limit = rowCount;
				param.Count = dataTotal;
				ucPaging1.Init(LoadPaging, param, pageSize, this.gridControl1);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
				WaitingManager.Hide();
			}
		}

		private void LoadPaging(object param)
		{
			try
			{
				startPage = ((CommonParam)param).Start ?? 0;
				int limit = ((CommonParam)param).Limit ?? 0;
				CommonParam paramCommon = new CommonParam(startPage, limit);
				Inventec.Core.ApiResultObject<List<V_LIS_ANTIBIOTIC_RANGE>> apiResult = null;
				LisAntibioticRangeViewFilter filter = new LisAntibioticRangeViewFilter();
				SetFilterNavBar(ref filter);
				filter.ORDER_DIRECTION = "DESC";
				filter.ORDER_FIELD = "MODIFY_TIME";
				gridView1.BeginUpdate();
				apiResult = new BackendAdapter(paramCommon).GetRO<List<V_LIS_ANTIBIOTIC_RANGE>>("api/LisAntibioticRange/GetView", ApiConsumers.LisConsumer, filter, paramCommon);
				if (apiResult != null)
				{
					var data = (List<V_LIS_ANTIBIOTIC_RANGE>)apiResult.Data;
					if (data != null)
					{
						gridView1.GridControl.DataSource = data;
						rowCount = (data == null ? 0 : data.Count);
						dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
					}
				}
				gridView1.EndUpdate();

				#region Process has exception
				SessionManager.ProcessTokenLost(paramCommon);
				#endregion
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
		}

		private void SetFilterNavBar(ref LisAntibioticRangeViewFilter filter)
		{
			try
			{
				filter.KEY_WORD = txtSearch.Text.Trim();
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
		}

		private void FillDataToControlsForm()
		{
			try
			{
				InitComboAntibiotic();
				InitComboBacterium();
				InitComboTechnique();

				//TODO
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

        private void InitComboTechnique()
        {
			//try
			//{
			//	List<ColumnInfo> columnInfos = new List<ColumnInfo>();
			//	columnInfos.Add(new ColumnInfo("NATIONAL_CODE", "", 3, 1));
			//	columnInfos.Add(new ColumnInfo("NATIONAL_NAME", "", 100, 2));
			//	ControlEditorADO controlEditorADO = new ControlEditorADO("NATIONAL_NAME", "ID", columnInfos, false, 103);
			//	//ControlEditorLoader.Load(cboTechnique, BackendDataWorker.Get<LIS.EFMODEL.DataModels.LIS_TECHNIQUE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), controlEditorADO);

			//}
			//catch (Exception ex)
			//{
			//	Inventec.Common.Logging.LogSystem.Warn(ex);
			//}
		}

        private void InitComboAntibiotic()
		{
			try
			{
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("NATIONAL_CODE", "", 3, 1));
				columnInfos.Add(new ColumnInfo("NATIONAL_NAME", "", 100, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("NATIONAL_NAME", "ID", columnInfos, false, 103);

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitComboBacterium()
		{
			try
			{
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("NATIONAL_CODE", "", 3, 1));
				columnInfos.Add(new ColumnInfo("NATIONAL_NAME", "", 100, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("NATIONAL_NAME", "ID", columnInfos, false, 103);

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitTabIndex()
		{
			try
			{
				if (dicOrderTabIndexControl != null)
				{
					foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
					{
						SetTabIndexToControl(itemOrderTab, layoutControl1);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
		{
			bool success = false;
			try
			{
				if (!layoutControlEditor.IsInitialized) return success;
				layoutControlEditor.BeginUpdate();
				try
				{
					foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
					{
						DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
						if (lci != null && lci.Control != null)
						{
							BaseEdit be = lci.Control as BaseEdit;
							if (be != null)
							{
								//Cac control dac biet can fix khong co thay doi thuoc tinh enable
								if (itemOrderTab.Key.Contains(be.Name))
								{
									be.TabIndex = itemOrderTab.Value;
								}
							}
						}
					}
				}
				finally
				{
					layoutControlEditor.EndUpdate();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}

			return success;
		}

		private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
		{
			try
			{
				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					V_LIS_ANTIBIOTIC_RANGE pData = (V_LIS_ANTIBIOTIC_RANGE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

					short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
					if (e.Column.FieldName == "STT")
					{
						e.Value = e.ListSourceRowIndex + 1 + startPage;
					}
					else if (e.Column.FieldName == "IS_ACTIVE_STR")
					{
						try
						{
							if (status == 1)
								e.Value = "Hoạt động";
							else
								e.Value = "Tạm khóa";

						}

						catch (Exception ex)
						{
							Inventec.Common.Logging.LogSystem.Error(ex);
						}
					}
					else if (e.Column.FieldName == "IS_ACCEPT_EQUAL_MAX_BOL")
					{
						e.Value = pData.IS_ACCEPT_EQUAL_MAX == 1 ? true : false;
					}
					else if (e.Column.FieldName == "IS_ACCEPT_EQUAL_MIN_BOL")
					{
						e.Value = pData.IS_ACCEPT_EQUAL_MIN == 1 ? true : false;
					}
					else if (e.Column.FieldName == "CREATE_TIME_STR")
					{
						try
						{

							e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);

						}
						catch (Exception ex)
						{
							Inventec.Common.Logging.LogSystem.Error(ex);
						}
					}
					else if (e.Column.FieldName == "MODIFY_TIME_STR")
					{
						try
						{
							e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
						}
						catch (Exception ex)
						{
							Inventec.Common.Logging.LogSystem.Error(ex);
						}
					}
				}

				gridControl1.RefreshDataSource();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
		{
			try
			{
				if (e.Column.FieldName != "LOCK" || e.Column.FieldName != "DELETE")
				{
					var rowData = (V_LIS_ANTIBIOTIC_RANGE)gridView1.GetFocusedRow();
					if (rowData != null)
					{
						currentData = rowData;
						ChangedDataRow(rowData);

					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ChangedDataRow(V_LIS_ANTIBIOTIC_RANGE data)
		{
			try
			{
				if (data != null)
				{
					InitCboFromCboBacterium(data.BACTERIUM_ID);
					InitCboFromCboAntibiotic(data.ANTIBIOTIC_ID);
					if(lstADOAntibiotic.Where(o=>o.ANTIBIOTIC_ID == data.ANTIBIOTIC_ID).ToList() !=null && lstADOAntibiotic.Where(o => o.ANTIBIOTIC_ID == data.ANTIBIOTIC_ID).ToList().Count > 0)
					{
						cboAntibiotic.EditValue = data.ANTIBIOTIC_ID;
						txtActibiotic.Text = data.ANTIBIOTIC_CODE;
					}
					else 
					{
						cboAntibiotic.EditValue = null ;
						txtActibiotic.Text = "";
					}
					if (lstADOBacterium.Where(o => o.BACTERIUM_ID == data.BACTERIUM_ID).ToList() != null && lstADOBacterium.Where(o => o.BACTERIUM_ID == data.BACTERIUM_ID).ToList().Count > 0)
					{
						cboBacterium.EditValue = data.BACTERIUM_ID;
						txtBacterium.Text = data.BACTERIUM_CODE;
					}
					else
					{
						InitCboBacterium();
						cboBacterium.EditValue = data.BACTERIUM_ID;
						txtBacterium.Text = data.BACTERIUM_CODE;
					}
					cboTechnique.EditValue = data.TECHNIQUE_ID;
					txtMaxValue.Text = data.MAX_VALUE;
					chkEqualMax.Checked = data.IS_ACCEPT_EQUAL_MAX == (short?)1;
					txtMinValue.Text = data.MIN_VALUE;
					chkEqualMin.Checked = data.IS_ACCEPT_EQUAL_MIN == (short?)1;
					txtSriValue.Text = data.SRI_VALUE;
					this.ActionType = GlobalVariables.ActionEdit;
					EnableControlChanged(this.ActionType);
					btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE);
					positionHandle = -1;
					Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
					BackendDataWorker.Reset<V_LIS_ANTIBIOTIC_RANGE>();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
		{
			try
			{
				DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
				if (e.RowHandle >= 0)
				{

					V_LIS_ANTIBIOTIC_RANGE data = (V_LIS_ANTIBIOTIC_RANGE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
					if (e.Column.FieldName == "LOCK")
					{
						e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__FALSE ? btnLock__Enable : btnLock__Disable);

					}

					if (e.Column.FieldName == "DELETE")
					{
						e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDelete__Enable : btnDelete__Disable);

					}

				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSession.Warn(ex);
			}
		}

		private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
		{
			DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
			if (e.RowHandle >= 0)
			{
				V_LIS_ANTIBIOTIC_RANGE data = (V_LIS_ANTIBIOTIC_RANGE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
				if (e.Column.FieldName == "IS_ACTIVE_STR")
				{
					if (data.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__FALSE)
						e.Appearance.ForeColor = Color.Red;
					else
						e.Appearance.ForeColor = Color.Green;
				}
			}
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{

			try
			{
				SaveProcess();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}

		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			try
			{
				SaveProcess();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SaveProcess()
		{
			CommonParam param = new CommonParam();

			try
			{
				bool success = false;
				if (!btnEdit.Enabled && !btnAdd.Enabled)
					return;

				positionHandle = -1;
				if (!dxValidationProvider1.Validate())
					return;
				if (string.IsNullOrEmpty(txtMinValue.Text) && string.IsNullOrEmpty(txtMaxValue.Text))
				{
					XtraMessageBox.Show("Vui lòng nhập giá trị nhỏ nhất hoặc lớn nhất", "Thông báo", MessageBoxButtons.OK);
					return;
				}
				WaitingManager.Show();
				LIS_ANTIBIOTIC_RANGE obj = new LIS_ANTIBIOTIC_RANGE();
				if (currentData != null)
					obj.ID = currentData.ID;
				obj.ANTIBIOTIC_ID = Int64.Parse(cboAntibiotic.EditValue.ToString());
				obj.BACTERIUM_ID = Int64.Parse(cboBacterium.EditValue.ToString());
				//obj.TECHNIQUE_ID = Int64.Parse(cboTechnique.EditValue.ToString());
				if (cboTechnique.EditValue != null)
					obj.TECHNIQUE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboTechnique.EditValue.ToString());
				obj.MAX_VALUE = txtMaxValue.Text;
				obj.MIN_VALUE = txtMinValue.Text;
				obj.SRI_VALUE = txtSriValue.Text;
				obj.IS_ACCEPT_EQUAL_MAX = chkEqualMax.Checked ? (short?)1 : 0;
				obj.IS_ACCEPT_EQUAL_MIN = chkEqualMin.Checked ? (short?)1 : 0;

				if (ActionType == GlobalVariables.ActionAdd)
				{
					obj.IS_ACTIVE = IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE;
					var resultData = new BackendAdapter(param).Post<LIS_ANTIBIOTIC_RANGE>("api/LisAntibioticRange/Create", ApiConsumers.LisConsumer, obj, param);
					if (resultData != null)
					{
						success = true;
					}
				}
				else
				{
					var resultData = new BackendAdapter(param).Post<LIS_ANTIBIOTIC_RANGE>("api/LisAntibioticRange/Update", ApiConsumers.LisConsumer, obj, param);
					if (resultData != null)
					{
						success = true;

					}
				}
				if (success)
				{
					FillDataToControl();
					SetDefaultValue();
					EnableControlChanged(this.ActionType);
				}


				WaitingManager.Hide();

				#region Hien thi message thong bao
				MessageManager.Show(this, param, success);
				#endregion

				#region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
				SessionManager.ProcessTokenLost(param);
				#endregion
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			try
			{
				SetDefaultValue();
				EnableControlChanged(this.ActionType);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnLock__Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			CommonParam param = new CommonParam();
			LIS_ANTIBIOTIC_RANGE success = new LIS_ANTIBIOTIC_RANGE();
			bool notHandler = false;
			try
			{

				V_LIS_ANTIBIOTIC_RANGE data = (V_LIS_ANTIBIOTIC_RANGE)gridView1.GetFocusedRow();
				if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					LIS_ANTIBIOTIC_RANGE data1 = new LIS_ANTIBIOTIC_RANGE();
					data1.ID = data.ID;
					WaitingManager.Show();
					success = new Inventec.Common.Adapter.BackendAdapter(param).Post<LIS_ANTIBIOTIC_RANGE>("api/LisAntibioticRange/ChangeLock", ApiConsumers.LisConsumer, data1, param);
					WaitingManager.Hide();
					if (success != null)
					{
						notHandler = true;
						FillDataToControl();
					}
					MessageManager.Show(this, param, notHandler);
				}

			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnLock__Disable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			CommonParam param = new CommonParam();
			LIS_ANTIBIOTIC_RANGE success = new LIS_ANTIBIOTIC_RANGE();
			bool notHandler = false;
			try
			{

				V_LIS_ANTIBIOTIC_RANGE data = (V_LIS_ANTIBIOTIC_RANGE)gridView1.GetFocusedRow();
				if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					LIS_ANTIBIOTIC_RANGE data1 = new LIS_ANTIBIOTIC_RANGE();
					data1.ID = data.ID;
					WaitingManager.Show();
					success = new Inventec.Common.Adapter.BackendAdapter(param).Post<LIS_ANTIBIOTIC_RANGE>("api/LisAntibioticRange/ChangeLock", ApiConsumers.LisConsumer, data1, param);
					WaitingManager.Hide();
					if (success != null)
					{
						notHandler = true;
						FillDataToControl();
					}
					MessageManager.Show(this, param, notHandler);
				}

			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnDelete__Disable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			CommonParam param = new CommonParam();
			V_LIS_ANTIBIOTIC_RANGE success = new V_LIS_ANTIBIOTIC_RANGE();
			bool notHandler = false;
			try
			{

				V_LIS_ANTIBIOTIC_RANGE data = (V_LIS_ANTIBIOTIC_RANGE)gridView1.GetFocusedRow();
				if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					V_LIS_ANTIBIOTIC_RANGE data1 = new V_LIS_ANTIBIOTIC_RANGE();
					data1.ID = data.ID;
					WaitingManager.Show();
					success = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_LIS_ANTIBIOTIC_RANGE>("api/LisAntibioticRange/ChangeLock", ApiConsumers.LisConsumer, data1, param);
					WaitingManager.Hide();
					if (success != null)
					{
						notHandler = true;
						FillDataToControl();
					}
					MessageManager.Show(this, param, notHandler);
				}

			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnDelete__Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			btnEdit.Enabled = false;
			try
			{
				CommonParam param = new CommonParam();
				var rowData = (V_LIS_ANTIBIOTIC_RANGE)gridView1.GetFocusedRow();
				if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{


					if (rowData != null)
					{
						bool success = false;
						success = new BackendAdapter(param).Post<bool>("api/LisAntibioticRange/Delete", ApiConsumers.LisConsumer, rowData.ID, param);
						if (success)
						{
							FillDataToControl();
							currentData = ((List<V_LIS_ANTIBIOTIC_RANGE>)gridControl1.DataSource).FirstOrDefault();


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

		private void cboBacterium_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
				{
					cboBacterium.EditValue = null;
					txtBacterium.Text = "";
					if (cboAntibiotic.EditValue != null)
					{
						InitCboFromCboAntibiotic(Int64.Parse(cboAntibiotic.EditValue.ToString()));
					}
					else
					{
						InitCboBacterium();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtBacterium_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (cboAntibiotic.EditValue != null && !string.IsNullOrEmpty(txtBacterium.Text))
					{
						var dt = lstADOBacterium.Where(o => o.BACTERIUM_CODE == txtBacterium.Text).ToList();
						if (dt != null && dt.Count > 0)
						{
							cboBacterium.EditValue = dt.First().BACTERIUM_ID;
							txtActibiotic.Focus();
							txtActibiotic.SelectAll();
						}
						else
						{
							cboBacterium.Focus();
							cboBacterium.ShowPopup();
						}
					}
					else if (cboAntibiotic.EditValue == null && !string.IsNullOrEmpty(txtBacterium.Text))
					{
						var dt = datasBacterium.Where(o => o.BACTERIUM_CODE == txtBacterium.Text).ToList();
						if (dt != null && dt.Count > 0)
						{
							cboBacterium.EditValue = dt.First().ID;
							txtActibiotic.Focus();
							txtActibiotic.SelectAll();
						}
						else
						{
							cboBacterium.Focus();
							cboBacterium.ShowPopup();
						}
					}
					else
					{
						cboBacterium.Focus();
						cboBacterium.ShowPopup();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void InitCboFromCboBacterium(long id)
		{
			try
			{
				List<BacAntibioticADO> lstADO = new List<BacAntibioticADO>();
				CommonParam commonParam = new CommonParam();
				LisBacAntibioticFilter filter = new LisBacAntibioticFilter();
				filter.BACTERIUM_ID = id;
				var dtAnntibiotic = new BackendAdapter(commonParam).Get<List<LIS_BAC_ANTIBIOTIC>>("api/LisBacAntibiotic/Get", ApiConsumers.LisConsumer, filter, commonParam);
				if (dtAnntibiotic != null && dtAnntibiotic.Count > 0)
				{
					foreach (var item in dtAnntibiotic)
					{
						BacAntibioticADO ado = new BacAntibioticADO();
						Inventec.Common.Mapper.DataObjectMapper.Map<BacAntibioticADO>(ado, item);
						if (datasAntibiotic.Where(o => o.ID == ado.ANTIBIOTIC_ID).ToList() != null && datasAntibiotic.Where(o => o.ID == ado.ANTIBIOTIC_ID).ToList().Count > 0)
						{
							ado.ANTIBIOTIC_NAME = datasAntibiotic.First(o => o.ID == ado.ANTIBIOTIC_ID).ANTIBIOTIC_NAME;
							ado.ANTIBIOTIC_CODE = datasAntibiotic.First(o => o.ID == ado.ANTIBIOTIC_ID).ANTIBIOTIC_CODE;
							lstADO.Add(ado);
						}
					}
				}
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("ANTIBIOTIC_CODE", "Mã", 150, 1));
				columnInfos.Add(new ColumnInfo("ANTIBIOTIC_NAME", "Tên", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("ANTIBIOTIC_NAME", "ANTIBIOTIC_ID", columnInfos, true, 400);
				ControlEditorLoader.Load(cboAntibiotic, lstADO, controlEditorADO);
				lstADOAntibiotic = lstADO;
				cboAntibiotic.Properties.ImmediatePopup = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void InitCboFromCboAntibiotic(long id)
		{
			try
			{
				List<BacAntibioticADO> lstADO = new List<BacAntibioticADO>();
				CommonParam commonParam = new CommonParam();
				LisBacAntibioticFilter filter = new LisBacAntibioticFilter();
				filter.ANTIBIOTIC_ID = id;
				var dtAnntibiotic = new BackendAdapter(commonParam).Get<List<LIS_BAC_ANTIBIOTIC>>("api/LisBacAntibiotic/Get", ApiConsumers.LisConsumer, filter, commonParam);
				if (dtAnntibiotic != null && dtAnntibiotic.Count > 0)
				{
					foreach (var item in dtAnntibiotic)
					{
						BacAntibioticADO ado = new BacAntibioticADO();
						Inventec.Common.Mapper.DataObjectMapper.Map<BacAntibioticADO>(ado, item);
						if (datasBacterium.Where(o => o.ID == ado.BACTERIUM_ID).ToList() != null && datasBacterium.Where(o => o.ID == ado.BACTERIUM_ID).ToList().Count > 0)
						{
							ado.BACTERIUM_CODE = datasBacterium.First(o => o.ID == ado.BACTERIUM_ID).BACTERIUM_CODE;
							ado.BACTERIUM_NAME = datasBacterium.First(o => o.ID == ado.BACTERIUM_ID).BACTERIUM_NAME;
							lstADO.Add(ado);
						}
					}

				}
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("BACTERIUM_CODE", "Mã", 150, 1));
				columnInfos.Add(new ColumnInfo("BACTERIUM_NAME", "Tên", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("BACTERIUM_NAME", "BACTERIUM_ID", columnInfos, true, 400);
				ControlEditorLoader.Load(cboBacterium, lstADO, controlEditorADO);
				lstADOBacterium = lstADO;
				cboBacterium.Properties.ImmediatePopup = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboBacterium_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (cboBacterium.EditValue != null)
				{
					InitCboFromCboBacterium(Int64.Parse(cboBacterium.EditValue.ToString()));
					var dt = datasBacterium.First(o => o.ID == Int64.Parse(cboBacterium.EditValue.ToString()));
					if (dt != null)
					{
						txtBacterium.Text = dt.BACTERIUM_CODE;
						cboBacterium.EditValue = dt.ID;
						txtActibiotic.Focus();
						txtActibiotic.SelectAll();

					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtActibiotic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (cboBacterium.EditValue != null && !string.IsNullOrEmpty(txtActibiotic.Text))
					{
						var dt = lstADOAntibiotic.Where(o => o.ANTIBIOTIC_CODE == txtActibiotic.Text).ToList();
						if (dt != null && dt.Count > 0)
						{
							cboAntibiotic.EditValue = dt.First().ANTIBIOTIC_ID;
							txtMinValue.Focus();
							txtMinValue.SelectAll();
						}
						else
						{
							cboAntibiotic.Focus();
							cboAntibiotic.ShowPopup();
						}
					}
					else if (cboAntibiotic.EditValue == null && !string.IsNullOrEmpty(txtActibiotic.Text))
					{
						var dt = datasAntibiotic.Where(o => o.ANTIBIOTIC_CODE == txtActibiotic.Text).ToList();
						if (dt != null && dt.Count > 0)
						{
							cboAntibiotic.EditValue = dt.First().ID;
							txtMinValue.Focus();
							txtMinValue.SelectAll();
						}
						else
						{
							cboAntibiotic.Focus();
							cboAntibiotic.ShowPopup();
						}
					}
					else
					{
						cboAntibiotic.Focus();
						cboAntibiotic.ShowPopup();
					}

				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboAntibiotic_EditValueChanged(object sender, EventArgs e)
		{

		}

		private void cboAntibiotic_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
				{
					cboAntibiotic.EditValue = null;
					txtActibiotic.Text = "";

					if (cboBacterium.EditValue != null)
					{
						InitCboFromCboBacterium(Int64.Parse(cboBacterium.EditValue.ToString()));
					}
					else
					{
						InitCboAntibiotic();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				btnFind_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnFind_Click(object sender, EventArgs e)
		{
			try
			{
				FillDataToControl();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				if (btnEdit.Enabled)
					btnEdit_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				if (btnAdd.Enabled)
					btnAdd_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				btnRefresh_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
		{
			try
			{
				BaseEdit edit = e.InvalidControl as BaseEdit;
				if (edit == null)
					return;

				BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
				if (viewInfo == null)
					return;

				if (positionHandle == -1)
				{
					positionHandle = edit.TabIndex;
					if (edit.Visible)
					{
						edit.SelectAll();
						edit.Focus();
					}
				}
				if (positionHandle > edit.TabIndex)
				{
					positionHandle = edit.TabIndex;
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

		private void txtMinValue_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					txtMaxValue.Focus();
					txtMaxValue.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtMaxValue_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					txtSriValue.Focus();
					txtSriValue.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					btnFind_Click(null, null);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtSriValue_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (btnAdd.Enabled)
						btnAdd.Focus();
					else if (btnEdit.Enabled)
						btnEdit.Focus();
					else
						btnRefresh.Focus();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboBacterium_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					txtActibiotic.Focus();
					txtActibiotic.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboAntibiotic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					cboTechnique.Focus();
					cboTechnique.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}


		private void cboBacterium_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
		{
			try
			{
				if (cboBacterium.EditValue != null)
				{
					InitCboFromCboBacterium(Int64.Parse(cboBacterium.EditValue.ToString()));
					var dt = datasBacterium.First(o => o.ID == Int64.Parse(cboBacterium.EditValue.ToString()));
					if (dt != null)
					{
						txtBacterium.Text = dt.BACTERIUM_CODE;
						cboBacterium.EditValue = dt.ID;
						txtActibiotic.Focus();
						txtActibiotic.SelectAll();

					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboAntibiotic_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
		{
			try
			{
				if (cboAntibiotic.EditValue != null)
				{
					InitCboFromCboAntibiotic(Int64.Parse(cboAntibiotic.EditValue.ToString()));
					var dt = datasAntibiotic.First(o => o.ID == Int64.Parse(cboAntibiotic.EditValue.ToString()));
					if (dt != null)
					{
						txtActibiotic.Text = dt.ANTIBIOTIC_CODE;
						cboAntibiotic.EditValue = dt.ID;
						cboTechnique.Focus();
						cboTechnique.SelectAll();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

        private void cboTechnique_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
			try
			{
				if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
				{
					cboTechnique.EditValue = null;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

        private void cboTechnique_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
			try
			{
				if (e.CloseMode == PopupCloseMode.Normal)
				{
					if (cboTechnique.EditValue != null)
					{
						txtMinValue.Focus();
					}					
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

        private void cboTechnique_EditValueChanged(object sender, EventArgs e)
        {
			
		}

        private void cboTechnique_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					txtMinValue.Focus();
					txtMinValue.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
    }
}
