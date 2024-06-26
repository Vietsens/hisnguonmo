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
using Inventec.UC.Paging;
using HIS.Desktop.Plugins.HisBloodTypeVolume.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Logging;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using System.Collections;

namespace HIS.Desktop.Plugins.HisBloodTypeVolume.Run
{
	public partial class frmBloodTypeVolume : FormBase
	{
		#region Declare
		int rowCount = 0;
		int dataTotal = 0;
		int startPage = 0;
		PagingGrid pagingGrid;
		int ActionType = -1;
		int positionHandle = -1;
		Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
		Inventec.Desktop.Common.Modules.Module moduleData;
		List<HIS_BLOOD_TYPE> datasBloodType { get; set; }
		List<HIS_BLOOD_VOLUME> datasBloodVolumn { get; set; }

		HIS_BLTY_VOLUME currentBltyVolume { get; set; }
		#endregion
		public frmBloodTypeVolume(Inventec.Desktop.Common.Modules.Module moduleData) : base(moduleData)
		{
			InitializeComponent();
			try
			{				
				string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
				this.Icon = Icon.ExtractAssociatedIcon(iconPath);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void frmBloodTypeVolume_Load(object sender, EventArgs e)
		{
			try
			{
				SetLanguage();
				LoadListBloodType();
				LoadListBloodVolume();
				SetDefaultValue();
				EnableControlChanged(this.ActionType);

				FillDataToControl();

				// kiem tra du lieu nhap vao
				ValidateForm();


				//Set tabindex control
				InitTabIndex();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetLanguage()
		{
			try
			{
				Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisBloodTypeVolume.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBloodTypeVolume.Run.frmBloodTypeVolume).Assembly);

				this.btnEdit.Text = Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.HisBloodTypeVolume.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnAdd.Text = Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.HisBloodTypeVolume.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.HisBloodTypeVolume.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnFind.Text = Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.HisBloodTypeVolume.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());				
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
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
				Inventec.Core.ApiResultObject<List<HIS_BLTY_VOLUME>> apiResult = null;
				HisBltyVolumeFilter filter = new HisBltyVolumeFilter();
				SetFilterNavBar(ref filter);
				filter.ORDER_DIRECTION = "DESC";
				filter.ORDER_FIELD = "MODIFY_TIME";
				gridView1.BeginUpdate();
				apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_BLTY_VOLUME>>("api/HisBltyVolume/Get", ApiConsumers.MosConsumer, filter, paramCommon);
				if (apiResult != null)
				{
					var data = (List<HIS_BLTY_VOLUME>)apiResult.Data;
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

		private void SetFilterNavBar(ref HisBltyVolumeFilter filter)
		{
			try
			{
				if (cboSearch.EditValue != null)
					filter.BLOOD_TYPE_ID = Int64.Parse(cboSearch.EditValue.ToString());
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
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

		#region Validate
		private void ValidateForm()
		{
			try
			{
				ValidateBloodType();
				ValidateBloodVolumn();				
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void ValidateBloodVolumn()
		{
			try
			{
				ValidationGridControls valid = new ValidationGridControls();
				valid.cbo = this.cboBloodVolumn;			
				valid.ErrorText = "Trường dữ liệu bắt buộc";
				valid.ErrorType = ErrorType.Warning;
				this.dxValidationProvider1.SetValidationRule(cboBloodVolumn, valid);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void ValidateBloodType()
		{
			try
			{
				ValidationGridControls valid = new ValidationGridControls();
				valid.cbo = this.cboBloodType;
				valid.ErrorText = "Trường dữ liệu bắt buộc";
				valid.ErrorType = ErrorType.Warning;
				this.dxValidationProvider1.SetValidationRule(cboBloodType, valid);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		#endregion
		private void SetDefaultValue()
		{
			try
			{
				InitCombo();
				this.ActionType = GlobalVariables.ActionAdd;
				cboSearch.EditValue = null;
				cboBloodType.EditValue = null;
				cboBloodVolumn.EditValue = null;
				Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
			}
			catch (Exception ex)
			{
				LogSystem.Warn(ex);
			}
		}

		private void EnableControlChanged(int action)
		{
			btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
			btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
		}

		#region DataCombo
		private void InitCombo()
		{
			try
			{
				InitCboBloodType();
				InitCboBloodVolumn();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitCboBloodVolumn()
		{
			try
			{

				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("BLOOD_TYPE_CODE", "Mã", 150, 1));
				columnInfos.Add(new ColumnInfo("BLOOD_TYPE_NAME", "Tên", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_TYPE_NAME", "ID", columnInfos, true, 400);
				ControlEditorLoader.Load(cboBloodType, datasBloodType, controlEditorADO);
				cboBloodType.Properties.ImmediatePopup = true;
				ControlEditorLoader.Load(cboSearch, datasBloodType, controlEditorADO);
				cboSearch.Properties.ImmediatePopup = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitCboBloodType()
		{
			try
			{

				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("VOLUME", "Dung tích", 250, 1));
				ControlEditorADO controlEditorADO = new ControlEditorADO("VOLUME", "ID", columnInfos, true, 150);
				ControlEditorLoader.Load(cboBloodVolumn, datasBloodVolumn, controlEditorADO);
				cboBloodVolumn.Properties.ImmediatePopup = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadListBloodType()
		{
			try
			{

				CommonParam paramCommon = new CommonParam();
				dynamic filter = new System.Dynamic.ExpandoObject();
				datasBloodType = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_BLOOD_TYPE>>("api/HisBloodType/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);


				datasBloodType = datasBloodType != null ? datasBloodType.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadListBloodVolume()
		{
			try
			{
				CommonParam paramCommon = new CommonParam();
				dynamic filter = new System.Dynamic.ExpandoObject();
				datasBloodVolumn = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_BLOOD_VOLUME>>("api/HisBloodVolume/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
				datasBloodVolumn = datasBloodVolumn != null ? datasBloodVolumn.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_DONATION == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		#endregion

		private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

		#region GridView

		private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
		{
			try
			{
				DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
				if (e.RowHandle >= 0)
				{

					HIS_BLTY_VOLUME data = (HIS_BLTY_VOLUME)((IList)((BaseView)sender).DataSource)[e.RowHandle];
					if (e.Column.FieldName == "DELETE")
					{
						e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repbtnDeleteEnable : repbtnDeleteDisable);

					}

				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSession.Warn(ex);
			}
		}

		private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
		{
			try
			{
				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					HIS_BLTY_VOLUME pData = (HIS_BLTY_VOLUME)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

					if (e.Column.FieldName == "STT")
					{
						e.Value = e.ListSourceRowIndex + 1 + startPage;
					}else if(e.Column.FieldName == "BLOOD_TYPE_CODE")
					{						
						e.Value = datasBloodType.FirstOrDefault(o => o.ID == pData.BLOOD_TYPE_ID).BLOOD_TYPE_CODE;
					}
					else if (e.Column.FieldName == "BLOOD_TYPE_NAME")
					{
						e.Value = datasBloodType.FirstOrDefault(o => o.ID == pData.BLOOD_TYPE_ID).BLOOD_TYPE_NAME;
					}
					else if (e.Column.FieldName == "BLOOD_VOLUME_TYPE")
					{
						e.Value = datasBloodVolumn.FirstOrDefault(o=>o.ID ==  datasBloodType.FirstOrDefault(p => p.ID == pData.BLOOD_TYPE_ID).BLOOD_VOLUME_ID).VOLUME;
					}
					else if (e.Column.FieldName == "BLOOD_VOLUME")
					{
						e.Value = datasBloodVolumn.FirstOrDefault(o => o.ID == pData.BLOOD_VOLUME_ID).VOLUME;
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

		private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
		{
			
		}

		private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
		{
			try
			{
				if (e.Column.FieldName != "DELETE")
				{
					var rowData = (HIS_BLTY_VOLUME)gridView1.GetFocusedRow();
					if (rowData != null)
					{
						currentBltyVolume = rowData;
						ChangedDataRow(rowData);

					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		#endregion

		private void ChangedDataRow(HIS_BLTY_VOLUME data)
		{
			try
			{
				if (data != null)
				{
					cboBloodType.EditValue = data.BLOOD_TYPE_ID;
					cboBloodVolumn.EditValue = data.BLOOD_VOLUME_ID;					
					
					this.ActionType = GlobalVariables.ActionEdit;
					EnableControlChanged(this.ActionType);
					btnEdit.Enabled = (this.currentBltyVolume.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
					positionHandle = -1;
					Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboBloodVolumn_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				if(cboBloodVolumn.EditValue!=null)
				{
					var check = datasBloodVolumn.FirstOrDefault(o => o.ID == Int64.Parse(cboBloodVolumn.EditValue.ToString()));
					if(check!=null)
					{
						cboBloodVolumn.EditValue = check.ID;				
						if (btnRefresh.Enabled)
							btnRefresh.Focus();
						if (btnAdd.Enabled)
							btnAdd.Focus();
						if (btnEdit.Enabled)
							btnEdit.Focus();
					}
					else
					{
						cboBloodVolumn.EditValue = null;
						cboBloodVolumn.ShowPopup();
						cboBloodVolumn.Focus();
					}
					cboBloodVolumn.Properties.Buttons[1].Visible = true;
				}
				else
				{
					cboBloodVolumn.EditValue = null;
					cboBloodVolumn.Properties.Buttons[1].Visible = false;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		

		private void cboBloodType_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (cboBloodType.EditValue != null)
				{					
					cboBloodType.Properties.Buttons[1].Visible = true;
					cboBloodVolumn.Focus();
					cboBloodVolumn.ShowPopup();
				}
				else
				{
					cboBloodType.Properties.Buttons[1].Visible = false;
				}
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
				WaitingManager.Show();
				HIS_BLTY_VOLUME obj = new HIS_BLTY_VOLUME();
				if (currentBltyVolume != null)
					obj.ID = currentBltyVolume.ID;
				obj.BLOOD_TYPE_ID = Int64.Parse(cboBloodType.EditValue.ToString());
				obj.BLOOD_VOLUME_ID = Int64.Parse(cboBloodVolumn.EditValue.ToString());

				if (ActionType == GlobalVariables.ActionAdd)
				{
					obj.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
					var resultData = new BackendAdapter(param).Post<HIS_BLTY_VOLUME>("api/HisBltyVolume/Create", ApiConsumers.MosConsumer, obj, param);
					if (resultData != null)
					{
						success = true;
					}
				}
				else
				{
					var resultData = new BackendAdapter(param).Post<HIS_BLTY_VOLUME>("api/HisBltyVolume/Update", ApiConsumers.MosConsumer, obj, param);
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

		#region ActionButton
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

		private void repbtnDeleteEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				CommonParam param = new CommonParam();
				var rowData = (HIS_BLTY_VOLUME)gridView1.GetFocusedRow();
				if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{


					if (rowData != null)
					{
						bool success = false;
						success = new BackendAdapter(param).Post<bool>("api/HisBltyVolume/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
						if (success)
						{
							FillDataToControl();						
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

		private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			btnFind_Click(null,null) ;
		}

		private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			if (btnEdit.Enabled)
				btnEdit_Click(null,null);
		}

		private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			if (btnAdd.Enabled)
				btnAdd_Click(null, null);
		}

		private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
				btnRefresh_Click(null, null);
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
		#endregion

		private void cboBloodVolumn_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
				{
					cboBloodVolumn.EditValue = null;
				}	
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboBloodType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
				{
					cboBloodType.EditValue = null;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboSearch_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (cboSearch.EditValue != null)
				{
					cboSearch.Properties.Buttons[1].Visible = true;
					btnFind.Focus();
				}
				else
				{
					cboSearch.Properties.Buttons[1].Visible = false;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
