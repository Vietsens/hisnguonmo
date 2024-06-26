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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MealRationDetail.ADO;
using HIS.Desktop.Plugins.ExpMestViewDetail;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Print;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;


using DevExpress.XtraGrid.Columns;
using System.Globalization;

namespace HIS.Desktop.Plugins.MealRationDetail.MealRationDetail
{
	public partial class frmMealRationDetail : HIS.Desktop.Utility.FormBase
	{
		#region Declaration
		MOS.EFMODEL.DataModels.V_HIS_RATION_SUM RationSum;// phiếu tổng hợp truyền sang
		List<ServiceReqSDO> ServiceReqFromRationSums; // các phiếu chỉ định con của tổng hợp hiện tại
													  //List<ServiceReqSDO> ExpMestAll;
		Inventec.Desktop.Common.Modules.Module moduleData;
		List<V_HIS_SERVICE_REQ> serviceReqs = new List<V_HIS_SERVICE_REQ>();
		List<V_HIS_SERE_SERV_RATION> sereServsRationCurrent = new List<V_HIS_SERE_SERV_RATION>();
		DelegateSelectData delegateSelectData = null;
		List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
		List<HIS_RATION_TIME> currentRationTime = null;
		List<HIS_RATION_GROUP> currentRationGroup = null;
		long roomId = 0;
		long roomTypeId = 0;

		HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
		List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
		#endregion

		#region Construct
		public frmMealRationDetail(Inventec.Desktop.Common.Modules.Module moduleData, MOS.EFMODEL.DataModels.V_HIS_RATION_SUM _rationSum, DelegateSelectData _delegateSelectData)
			: base(moduleData)
		{
			try
			{
				InitializeComponent();
				delegateSelectData = _delegateSelectData;
				this.moduleData = moduleData;
				this.RationSum = _rationSum;
				this.roomId = moduleData.RoomId;
				this.roomTypeId = moduleData.RoomTypeId;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		#endregion

		#region Load
		private void frmMealRationDetail_Load(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				InitControlState();
				SetCaptionByLanguageKey();
				SetDataToCommonInfo(this.RationSum);
				GetServiceReqFromRationSum(this.RationSum.ID);
				LoadDataSereServRation();
				FillDataToGridSereServ();
				LoadCombo();
				GetControlAcs();
				EnableControl();
				SetIcon();

				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitControlState()
		{
			try
			{
				toggleSwitchTheoNhom.IsOn = false;
				bool? IsState = null;
				this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
				this.currentControlStateRDO = controlStateWorker.GetData(moduleData.ModuleLink);
				if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
				{
					foreach (var item in this.currentControlStateRDO)
					{
						if (item.KEY == toggleSwitchTheoNhom.Name)
						{
							IsState = item.VALUE == "1";
						}
					}
				}
				if (IsState != null)
					toggleSwitchTheoNhom.IsOn = (bool)IsState;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadDataSereServRation()
		{
			try
			{
				currentRationTime = BackendDataWorker.Get<HIS_RATION_TIME>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
				currentRationGroup = BackendDataWorker.Get<HIS_RATION_GROUP>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
				MOS.Filter.HisSereServRationViewFilter filter = new MOS.Filter.HisSereServRationViewFilter();
				CommonParam paramCommon = new CommonParam();
				filter.SERVICE_REQ_IDs = ServiceReqFromRationSums.Select(o => o.ID).ToList();
				this.sereServsRationCurrent = new Inventec.Common.Adapter.BackendAdapter
					(paramCommon).Get<List<V_HIS_SERE_SERV_RATION>>
					("api/HisSereServRation/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		#endregion

		#region private function
		void GetControlAcs()
		{
			try
			{
				//CommonParam param = new CommonParam();
				//ACS.SDO.AcsTokenLoginSDO tokenLoginSDOForAuthorize = new ACS.SDO.AcsTokenLoginSDO();
				//tokenLoginSDOForAuthorize.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
				//tokenLoginSDOForAuthorize.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;

				//var acsAuthorize = new BackendAdapter(param).Get<ACS.SDO.AcsAuthorizeSDO>(HIS.Desktop.ApiConsumer.AcsRequestUriStore.ACS_TOKEN__AUTHORIZE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, tokenLoginSDOForAuthorize, param);

				//var acsAuthorize = BackendDataWorker.Get<HIS.Desktop.LocalStorage.LocalData.GlobalVariables.AcsAuthorizeSDO>().Where(o => o.LOGIN_NAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() && o.APPLICATION_CODE == GlobalVariables.APPLICATION_CODE).ToList();
				var acsAuthorize = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.AcsAuthorizeSDO;

				if (acsAuthorize != null)
				{
					controlAcs = acsAuthorize.ControlInRoles.ToList();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadCombo()
		{
			LoadComboBuaAn();
			LoadComboSuatAn();
		}

		private void LoadComboBuaAn()
		{
			CommonParam param = new CommonParam();
			//HisRationTimeFilter filer = new HisRationTimeFilter();
			//filer.IDs = ServiceReqFromRationSums.Select(o => o.RATION_TIME_ID ?? 0).ToList();
			var RationTimes = BackendDataWorker.Get<HIS_RATION_TIME>().Where(o => ServiceReqFromRationSums.Exists(p => p.RATION_TIME_ID == o.ID)).ToList();
			//new BackendAdapter(param).Get<List<HIS_RATION_TIME>>("api/HisRationTime/Get", ApiConsumer.ApiConsumers.MosConsumer, filer, param);
			List<ColumnInfo> columnInfos = new List<ColumnInfo>();
			columnInfos.Add(new ColumnInfo("RATION_TIME_CODE", "", 100, 1));
			columnInfos.Add(new ColumnInfo("RATION_TIME_NAME", "", 500, 2));
			ControlEditorADO controlEditorADO = new ControlEditorADO("RATION_TIME_NAME", "ID", columnInfos, false, 600);
			ControlEditorLoader.Load(cboBuaAn, RationTimes, controlEditorADO);
		}

		private void LoadComboSuatAn()
		{
			CommonParam param = new CommonParam();
			//HisSereServRationViewFilter filer = new HisSereServRationViewFilter();
			//filer.SERVICE_REQ_IDs = ServiceReqFromRationSums.Select(o => o.ID).ToList();
			//var data = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_RATION>>("api/HisSereServRation/GetView", ApiConsumer.ApiConsumers.MosConsumer, filer, param).ToList();
			//var dataGroup = data.GroupBy(o => new { o.SERVICE_ID }).Select(grp => grp.FirstOrDefault()).ToList();
			//Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("dataGroup", dataGroup));
			List<ColumnInfo> columnInfos = new List<ColumnInfo>();
			columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
			columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
			ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "SERVICE_ID", columnInfos, false, 350);
			ControlEditorLoader.Load(cboSuatAn, sereServsRationCurrent.GroupBy(o => new { o.SERVICE_ID }).Select(grp => grp.FirstOrDefault()).ToList(), controlEditorADO);
		}

		private void SetIcon()
		{
			try
			{
				this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		void EnableControl()
		{
			try
			{
				if (RationSum.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__APPROVAL && moduleData.RoomId == RationSum.ROOM_ID)
				{
					btnApprovalCancel.Enabled = true;
				}
				if (RationSum.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ && moduleData.RoomId == RationSum.ROOM_ID)
				{
					btnApproval.Enabled = true;
				}
				else
					btnApproval.Enabled = false;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		void SetDataToGridControlMedicineMaterial(List<SereServADO> expMestMedicineMaterials)
		{
			try
			{
				gridControlSereSevRationDetail.BeginUpdate();
				gridControlSereSevRationDetail.DataSource = expMestMedicineMaterials;
				gridControlSereSevRationDetail.EndUpdate();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		// gán dữ liệu vào thông tin chung
		void SetDataToCommonInfo(MOS.EFMODEL.DataModels.V_HIS_RATION_SUM RationSum)
		{
			try
			{
				if (RationSum != null)
				{
					lblCreator.Text = RationSum.APPROVAL_LOGINNAME + " - " + RationSum.APPROVAL_USERNAME;
					lblCreateTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(RationSum.APPROVAL_TIME ?? 0);
					lblExpMestCode.Text = RationSum.RATION_SUM_CODE;
					lblMedistock.Text = RationSum.ROOM_NAME;
					lblReqDepartment.Text = RationSum.REQ_DEPARTMENT_CODE + " - " + RationSum.REQ_DEPARTMENT_NAME;
					lblReqName.Text = RationSum.REQ_LOGINNAME + " - " + RationSum.REQ_USERNAME;
					lblReqTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(RationSum.REQ_TIME ?? 0);
					Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("RationSum.REQ_TIME", RationSum.REQ_TIME));
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		// lấy các phiếu con từ phiếu tổng hợp suất ăn được chọn
		void GetServiceReqFromRationSum(long rationSumID)
		{
			try
			{
				if (rationSumID > 0)
				{
					CommonParam param = new CommonParam();
					MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
					serviceReqFilter.RATION_SUM_ID = rationSumID;
					var ServiceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, param);
					ServiceReqFromRationSums = new List<ServiceReqSDO>();
					foreach (var item in ServiceReqs)
					{
						ServiceReqSDO ExpMestSDO = new ServiceReqSDO(item);
						ServiceReqFromRationSums.Add(ExpMestSDO);
					}
					//ExpMestAll = new List<ServiceReqSDO>();
					//ExpMestAll.AddRange(ServiceReqFromRationSums);
					//Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("data Front", ServiceReqFromRationSums.Select(o => o.SERVICE_REQ_CODE)));

					//FilterWithSearch(ServiceReqFromRationSums);

					//Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("data End", ServiceReqFromRationSums.Select(o => o.SERVICE_REQ_CODE)));

					gridControlServiceReq.BeginUpdate();
					gridControlServiceReq.DataSource = ServiceReqFromRationSums;
					gridControlServiceReq.EndUpdate();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		List<ServiceReqSDO> FilterWithSearch(List<ServiceReqSDO> expMests)
		{
			List<ServiceReqSDO> result = new List<ServiceReqSDO>();
			try
			{
				if (expMests == null || expMests.Count == 0)
				{
					return result;
				}
				result = expMests;

				if (cboBuaAn.EditValue != null)
				{
					result = result.Where(o => o.RATION_TIME_ID == Convert.ToInt32(cboBuaAn.EditValue.ToString())).ToList();
				}
				if (cboSuatAn.EditValue != null)
				{
					result = result.Where(o => o.SERVICE_REQ_TYPE_ID == Convert.ToInt32(cboSuatAn.EditValue.ToString())).ToList();
				}

				if (!String.IsNullOrWhiteSpace(txtPatientName.Text))
				{
					result = result.Where(o => !String.IsNullOrWhiteSpace(o.TDL_PATIENT_NAME)
						&& o.TDL_PATIENT_NAME.ToLower().Contains(txtPatientName.Text.Trim().ToLower())).ToList();
				}
			}
			catch (Exception ex)
			{
				result = new List<ServiceReqSDO>();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return result;
		}
		#endregion

		#region public function
		#endregion

		#region Event handler
		private void cboPrint_Click(object sender, EventArgs e)
		{
			try
			{
				//List<V_HIS_RATION_SUM> rationSumSelectList = new List<V_HIS_RATION_SUM>();
				//var selectrationSum = gridViewServiceReqChild.GetSelectedRows();
				//if (selectrationSum != null && selectrationSum.Count() > 0)
				//{
				//    foreach (var item in selectrationSum)
				//    {
				//        var department = (V_HIS_RATION_SUM)gridViewServiceReqChild.GetRow(item);
				//        rationSumSelectList.Add(department);
				//    }
				//}

				//if (RationSum == null || rationSumSelectList.Count == 0)
				//{
				//    MessageBox.Show("Chưa chọn phiếu tổng hợp");
				//    return;
				//}
				List<V_HIS_RATION_SUM> selectrationSum = new List<V_HIS_RATION_SUM>();
				selectrationSum.Add(RationSum);
				Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RationSumPrint").FirstOrDefault();
				if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.RationSumPrint'");
				moduleData.RoomId = this.roomId;
				moduleData.RoomTypeId = this.roomTypeId;
				if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
				{
					List<object> listArgs = new List<object>();
					listArgs.Add(selectrationSum);
					listArgs.Add(moduleData);
					var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
					if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

					((Form)extenceInstance).ShowDialog();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnApproval_Click(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				bool success = false;
				CommonParam param = new CommonParam();
				var dataRow = RationSum;
				if (dataRow != null)
				{
					HisRationSumUpdateSDO dataUpdate = new HisRationSumUpdateSDO();
					dataUpdate.RationSumId = dataRow.ID;
					dataUpdate.WorkingRoomId = this.moduleData.RoomId;
					var Result = new BackendAdapter(param).Post<HIS_RATION_SUM>(RequestUriStore.Ration_Sum_Approve, ApiConsumers.MosConsumer, dataUpdate, param);
					if (Result != null)
					{
						success = true;
						GetServiceReqFromRationSum(this.RationSum.ID);
					}
				}
				MessageManager.Show(this.ParentForm, param, success);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				LogSystem.Error(ex);
			}
		}

		private void refreshData(object data)
		{
			try
			{
				if (data is HisAggrExpMestSDO)
				{
					var expMestApprove = (MOS.SDO.HisAggrExpMestSDO)data;
					//EnableBottomButton(expMestApprove.serviceReqIds, expMestApprove.RequestRoomId);
				}
				delegateSelectData(new HIS_EXP_MEST());
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void SetFilterSereServRation(ref MOS.Filter.HisSereServRationViewFilter filter)
		{
			try
			{
				filter.ORDER_FIELD = "MODIFY_TIME";
				filter.ORDER_DIRECTION = "DESC";

				List<ServiceReqSDO> serviceReqs = new List<ServiceReqSDO>();
				int[] selectRows = gridViewServiceReqChild.GetSelectedRows();

				if (selectRows != null && selectRows.Count() > 0)
				{
					foreach (var item in selectRows)
					{
						var serviceReq = (ServiceReqSDO)gridViewServiceReqChild.GetRow(item);
						serviceReqs.Add(serviceReq);
					}
				}

				// filter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN;

				filter.SERVICE_REQ_IDs = serviceReqs.Select(o => o.ID).Distinct().ToList();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void FillDataToGridSereServ()
		{
			try
			{
				WaitingManager.Show();
				CommonParam paramCommon = new CommonParam();
				//MOS.Filter.HisSereServRationViewFilter filter = new MOS.Filter.HisSereServRationViewFilter();
				//SetFilterSereServRation(ref filter);
				List<ServiceReqSDO> serviceReqs = new List<ServiceReqSDO>();
				int[] selectRows = gridViewServiceReqChild.GetSelectedRows();

				if (selectRows != null && selectRows.Count() > 0)
				{
					foreach (var item in selectRows)
					{
						var serviceReq = (ServiceReqSDO)gridViewServiceReqChild.GetRow(item);
						serviceReqs.Add(serviceReq);
					}
				}
				if (serviceReqs == null || serviceReqs.Count() == 0)
				{
					gridViewSereSevRationDetail.BeginDataUpdate();
					gridControlSereSevRationDetail.DataSource = null;
					gridViewSereSevRationDetail.EndDataUpdate();
					WaitingManager.Hide();
					return;
				}
				gridViewSereSevRationDetail.BeginUpdate();
				//var datas = new Inventec.Common.Adapter.BackendAdapter
				//	(paramCommon).Get<List<V_HIS_SERE_SERV_RATION>>
				//	("api/HisSereServRation/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

				int skip = 0;
				List<long> srIds = serviceReqs.Select(s => s.ID).Distinct().ToList();
				List<V_HIS_SERE_SERV_RATION> datas = new List<V_HIS_SERE_SERV_RATION>();
				while (srIds.Count - skip > 0)
				{
					var listIds = srIds.Skip(skip).Take(100).ToList();
					skip += 100;
					datas.AddRange(sereServsRationCurrent.Where(o => listIds.Exists(p => p == o.SERVICE_REQ_ID)).ToList());
				}
				datas = datas.OrderByDescending(o => o.MODIFY_TIME).ToList();
				if (datas != null && datas.Count > 0)
				{
					List<SereServADO> dataSource = GroupSereServ(datas);
					dataSource = (dataSource != null && dataSource.Count() > 0)
						? dataSource.OrderBy(q => q.SERVICE_NAME).ToList() : dataSource;

					gridControlSereSevRationDetail.DataSource = dataSource;
				}
				else
				{
					gridControlSereSevRationDetail.DataSource = null;
				}
				gridViewSereSevRationDetail.EndUpdate();

				#region Process has exception
				HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
				#endregion
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewExpMestChild_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			FillDataToGridSereServ();
		}

		private void ButtonEditRemoveEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				WaitingManager.Show();
				bool success = false;
				CommonParam param = new CommonParam();
				var serviceReqSelection = (ServiceReqSDO)gridViewServiceReqChild.GetFocusedRow();
				var dataRow = RationSum;
				if (dataRow != null)
				{
					HisRationSumUpdateSDO dataUpdate = new HisRationSumUpdateSDO();
					dataUpdate.RationSumId = dataRow.ID;
					dataUpdate.WorkingRoomId = this.moduleData.RoomId;
					dataUpdate.ServiceReqId = serviceReqSelection.ID;
					success = new BackendAdapter(param).Post<bool>(RequestUriStore.Ration_Sum_Remove, ApiConsumers.MosConsumer, dataUpdate, param);
					if (success)
					{
						GetServiceReqFromRationSum(this.RationSum.ID);
					}
				}
				MessageManager.Show(this.ParentForm, param, success);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				LogSystem.Error(ex);
			}
		}

		private void gridViewExpMestChild_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
		{
			try
			{
				DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
				if (e.RowHandle >= 0)
				{

					ServiceReqSDO data = (ServiceReqSDO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
					if (e.Column.FieldName == "DELETE_ITEM")
					{
						if (this.RationSum.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ)
						{
							e.RepositoryItem = ButtonEditRemoveEnable;
						}
						else
						{
							e.RepositoryItem = repositoryItemButtonEditNone;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridViewExpMestChild_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
		{
			try
			{
				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
					ServiceReqSDO pData = (ServiceReqSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
					if (e.Column.FieldName == "STT")
					{
						e.Value = e.ListSourceRowIndex + 1;
					}
					else if (e.Column.FieldName == "DOB_STR")
					{
						try
						{
							e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.TDL_PATIENT_DOB);
						}
						catch (Exception ex)
						{
							Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao DOB_STR", ex);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridViewMedicineMaterialDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
		{
			try
			{
				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
					SereServADO pData = (SereServADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
					if (e.Column.FieldName == "STT")
					{
						e.Value = e.ListSourceRowIndex + 1;
					}
					if (e.Column.FieldName == "INTRUCTION_TIME_STR")
					{
						e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.INTRUCTION_TIME);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		ServiceReqSDO expMestFocus;
		private void gridViewExpMestChild_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
				{
					GridView view = sender as GridView;
					GridHitInfo hi = view.CalcHitInfo(e.Location);
					if (hi.InRowCell)
					{
						var expMestFocus = (ServiceReqSDO)gridViewServiceReqChild.GetRow(hi.RowHandle);
						if (hi.Column.FieldName == "DELETE_ITEM")
						{
							if (this.RationSum.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ)
							{
								ButtonEditRemoveEnable_ButtonClick(null, null);
							}
						}
						if (hi.Column.FieldName == "PRINT_TEM_DINH_DUONG")
						{
							ButtonEdit__Print_Click(null, null);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void bbtnApproval_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				if (btnApproval.Enabled)
				{
					btnApproval_Click(null, null);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void bbtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				if (btnApprovalCancel.Enabled)
				{
					btnApproval_Click(null, null);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		#endregion

		private void toggleSwitchTheoLo_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == toggleSwitchTheoNhom.Name && o.MODULE_LINK == moduleData.ModuleLink).FirstOrDefault() : null;
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
				if (csAddOrUpdate != null)
				{
					csAddOrUpdate.VALUE = (toggleSwitchTheoNhom.IsOn ? "1" : "");
				}
				else
				{
					csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
					csAddOrUpdate.KEY = toggleSwitchTheoNhom.Name;
					csAddOrUpdate.VALUE = (toggleSwitchTheoNhom.IsOn ? "1" : "");
					csAddOrUpdate.MODULE_LINK = moduleData.ModuleLink;
					if (this.currentControlStateRDO == null)
						this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
					this.currentControlStateRDO.Add(csAddOrUpdate);
				}
				this.controlStateWorker.SetData(this.currentControlStateRDO);

				WaitingManager.Hide();
				FillDataToGridSereServ();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void bbtnYLenhThuocBN_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				//btnYLenhThuocBenhNhan_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		public void CloseServiceReqPatientForm(object obj)
		{
			try
			{
				this.Enabled = true;
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
			}
		}

		private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			btnSearch_Click(null, null);
		}

		private void btnSearch_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.ServiceReqFromRationSums != null && this.ServiceReqFromRationSums.Count > 0)
				{
					List<ServiceReqSDO> dataGridContrl = new List<ServiceReqSDO>();
					if (cboSuatAn.EditValue != null)
					{
						foreach (var item in ServiceReqFromRationSums)
						{
							var SereCheck = sereServsRationCurrent.Any(o => o.SERVICE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboSuatAn.EditValue.ToString() ?? ""));
							if (SereCheck)
							{
								dataGridContrl.Add(item);
							}
						}
					}
					else
						dataGridContrl = this.ServiceReqFromRationSums;
					if (cboBuaAn.EditValue != null)
					{
						dataGridContrl = dataGridContrl.Where(o => o.RATION_TIME_ID == Convert.ToInt32(cboBuaAn.EditValue.ToString())).ToList();
					}
					if (!string.IsNullOrEmpty(txtPatientName.Text.Trim()))
					{
						dataGridContrl = dataGridContrl.Where(o => o.TDL_PATIENT_NAME.ToUpper().Trim().Contains(txtPatientName.Text.Trim().ToUpper())).ToList();
					}
					gridViewServiceReqChild.BeginUpdate();
					gridControlServiceReq.DataSource = dataGridContrl;
					gridViewServiceReqChild.EndUpdate();
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
				if (e.KeyCode == Keys.Enter)
				{
					btnSearch_Click(null, null);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewMedicineMaterialDetail_Click(object sender, EventArgs e)
		{
			try
			{

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewExpMestChild_RowCellStyle(object sender, RowCellStyleEventArgs e)
		{
			try
			{
				//if (e.RowHandle < 0)
				//    return;
				//var data = (ServiceReqSDO)gridViewServiceReqChild.GetRow(e.RowHandle);
				//if (data != null && data.IsHighLight)
				//{
				//    e.Appearance.BackColor = Color.Yellow;
				//}

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void InTem(string printTypeCode, string fileName, ref bool result)
		{
			try
			{
				WaitingManager.Show();

				List<V_HIS_SERE_SERV_15> SereServPrintList = new List<V_HIS_SERE_SERV_15>();
				var serviceReqSelection = (ServiceReqSDO)gridViewServiceReqChild.GetFocusedRow();
				string RATION_TIME_NAME = BackendDataWorker.Get<HIS_RATION_TIME>().Where(o => o.ID == serviceReqSelection.RATION_TIME_ID.Value).FirstOrDefault().RATION_TIME_NAME;
				string RATION_TIME_CODE = BackendDataWorker.Get<HIS_RATION_TIME>().Where(o => o.ID == serviceReqSelection.RATION_TIME_ID.Value).FirstOrDefault().RATION_TIME_CODE;
				CommonParam param = new CommonParam();
				HisSereServViewFilter filer = new HisSereServViewFilter();
				filer.SERVICE_REQ_ID = serviceReqSelection.ID;
				var sereSev = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_15>>("api/HisSereServ/GetvIEW15", ApiConsumer.ApiConsumers.MosConsumer, filer, param);

				//var sereSev = BackendDataWorker.Get<V_HIS_SERE_SERV>().Where(o => o.SERVICE_REQ_ID == serviceReqSelection.ID);
				List<V_HIS_SERVICE_REQ> ServiceReqPrintList = new List<V_HIS_SERVICE_REQ>();
				V_HIS_SERVICE_REQ serviceReqInput = new V_HIS_SERVICE_REQ();
				Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ>(serviceReqInput, serviceReqSelection);
				serviceReqInput.RATION_TIME_CODE = RATION_TIME_CODE;
				serviceReqInput.RATION_TIME_NAME = RATION_TIME_NAME;
				ServiceReqPrintList.Add(serviceReqInput);
				SereServPrintList.AddRange(sereSev);

				var treatBedRoomFilter = new HisTreatmentBedRoomFilter();
				treatBedRoomFilter.TREATMENT_ID = serviceReqSelection.TREATMENT_ID;
				var treatmentBedRoom = new BackendAdapter(param).Get<List<HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/Get", ApiConsumer.ApiConsumers.MosConsumer, treatBedRoomFilter, param);

				var BedLogFilter = new HisBedLogViewFilter();
				BedLogFilter.TREATMENT_ID = serviceReqSelection.TREATMENT_ID;
				var bedLog = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, BedLogFilter, param);

				//var skip = 0;
				//while (SereServPrintList.Count - skip > 0)
				//{
				//    var sereServSubList = SereServPrintList.Skip(skip).Take(8).ToList();
				//    skip = skip + 8;
				//}

				MPS.Processor.Mps000371.PDO.Mps000371PDO pdo = new MPS.Processor.Mps000371.PDO.Mps000371PDO(
				   ServiceReqPrintList,
				   SereServPrintList,
				   BackendDataWorker.Get<HIS_SERVICE_UNIT>(),
				   treatmentBedRoom,
				   bedLog);

				MPS.ProcessorBase.Core.PrintData printData = null;

				string printerName = "";
				if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
				{
					printerName = GlobalVariables.dicPrinter[printTypeCode];
				}

				Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((""), printTypeCode, this.moduleData != null ? this.moduleData.RoomId : 0);
				WaitingManager.Hide();

				if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
				{
					printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
				}
				else
				{
					printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
				}
				result = MPS.MpsPrinter.Run(printData);
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
					case PrintTypeCode.PRINT_TYPE_CODE__MPS000371:
						InTem(printTypeCode, fileName, ref result);
						break;
					default:
						break;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
				result = false;
			}
			return result;
		}

		private void ButtonEdit__Print_Click(object sender, EventArgs e)
		{
			try
			{
				Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
				store.RunPrintTemplate(PrintTypeCode.PRINT_TYPE_CODE__MPS000371, DelegateRunPrinter);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnApprovalCancel_Click(object sender, EventArgs e)
		{
			try
			{
				//WaitingManager.Show();
				bool success = false;
				CommonParam param = new CommonParam();
				var dataRow = RationSum;
				if (dataRow != null)
				{
					HisRationSumUpdateSDO dataUpdate = new HisRationSumUpdateSDO();
					dataUpdate.RationSumId = dataRow.ID;
					dataUpdate.WorkingRoomId = this.moduleData.RoomId;
					var Result = new BackendAdapter(param).Post<HIS_RATION_SUM>(RequestUriStore.Ration_Sum_Unapprove, ApiConsumers.MosConsumer, dataUpdate, param);
					if (Result != null)
					{
						success = true;
						GetServiceReqFromRationSum(this.RationSum.ID);
					}
				}
				MessageManager.Show(this.ParentForm, param, success);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				LogSystem.Error(ex);
			}
		}

		private void ButtonEditRemoveEnable_Click(object sender, EventArgs e)
		{

		}

		private List<SereServADO> GroupSereServ(List<V_HIS_SERE_SERV_RATION> input)
		{
			List<SereServADO> result = new List<SereServADO>();
			try
			{
				WaitingManager.Hide();
				if (input != null && input.Count() > 0)
				{
					if (toggleSwitchTheoNhom.IsOn)
					{
						GridColumn colPatientType = gridViewSereSevRationDetail.Columns["PATIENT_TYPE_NAME"];
						if (colPatientType != null)
							colPatientType.Visible = false;

						GridColumn colName = gridViewSereSevRationDetail.Columns["RATION_TIME_NAME"];
						if (colName != null)
							colName.Visible = false;

						GridColumn colTime = gridViewSereSevRationDetail.Columns["INTRUCTION_TIME_STR"];
						if (colTime != null)
							colTime.Visible = false;

						GridColumn colServiceUnitName = gridViewSereSevRationDetail.Columns["SERVICE_UNIT_NAME"];
						if (colServiceUnitName != null)
							colServiceUnitName.Visible = false;

						var groupSS = input.GroupBy(o => new { o.RATION_GROUP_ID }).ToArray();

						foreach (var group in groupSS)
						{
							var firstItem = group.FirstOrDefault();
							SereServADO sereServADO = new SereServADO(firstItem);
							var checkRationGroup = currentRationGroup.FirstOrDefault(o => o.ID == firstItem.RATION_GROUP_ID);
							if (checkRationGroup != null)
							{
								sereServADO.SERVICE_CODE = checkRationGroup.RATION_GROUP_CODE;
								sereServADO.SERVICE_NAME = checkRationGroup.RATION_GROUP_NAME;
							}
							else
							{
								sereServADO.SERVICE_CODE = "";
								sereServADO.SERVICE_NAME = "(Không xác định)";
							}
							sereServADO.SERVICE_UNIT_NAME = "";
							sereServADO.AMOUNT_SUM = group.Sum(o => o.AMOUNT);
							result.Add(sereServADO);
						}						
					}
					else
					{
						//var groupSS = input.OrderBy(o => o.SERVICE_NAME).ToList();
						GridColumn colPatientType = gridViewSereSevRationDetail.Columns["PATIENT_TYPE_NAME"];
						if (colPatientType != null)
						{
							colPatientType.VisibleIndex = 3;
							colPatientType.Visible = true;
						}

						GridColumn colServiceUnitName = gridViewSereSevRationDetail.Columns["SERVICE_UNIT_NAME"];
						if (colServiceUnitName != null)
						{
							colServiceUnitName.VisibleIndex = 4;
							colServiceUnitName.Visible = true;
						}

						GridColumn colName = gridViewSereSevRationDetail.Columns["RATION_TIME_NAME"];
						if (colName != null)
							colName.Visible = true;

						GridColumn colTime = gridViewSereSevRationDetail.Columns["INTRUCTION_TIME_STR"];
						if (colTime != null)
							colTime.Visible = true;

						foreach (var group in input)
						{
							SereServADO sereServADO = new SereServADO();
							sereServADO.AMOUNT_SUM = group.AMOUNT;
							sereServADO.SERVICE_CODE = group.SERVICE_CODE;
							sereServADO.SERVICE_NAME = group.SERVICE_NAME;
							sereServADO.PATIENT_TYPE_NAME = group.PATIENT_TYPE_NAME;
							sereServADO.SERVICE_UNIT_NAME = group.SERVICE_UNIT_NAME;
							sereServADO.RATION_TIME_NAME = group.RATION_TIME_NAME;
							sereServADO.INTRUCTION_TIME = group.INTRUCTION_TIME;
							sereServADO.IS_RATION = group.IS_RATION;
							if (Convert.ToDecimal(group.AMOUNT, new CultureInfo("en-US")) == Convert.ToDecimal(0.5, new CultureInfo("en-US")) && currentRationTime != null && currentRationTime.Count == 1)
							{
								sereServADO.SERVICE_NAME = "Bữa chiều";
							}
							result.Add(sereServADO);
						}
						if (colPatientType != null && result != null)
						{
							if (result.Where(o => o.IS_RATION == 1).ToList() != null && result.Count == result.Where(o => o.IS_RATION == 1).ToList().Count)
							{
								colPatientType.Caption = "Mức ăn";
								colPatientType.ToolTip = "Mức ăn";
							}
							else if (result.Where(o => o.IS_RATION == null).ToList() != null && result.Count == result.Where(o => o.IS_RATION == null).ToList().Count)
							{
								colPatientType.Caption = "ĐTTT";
								colPatientType.ToolTip = "Đối tượng thanh toán";
							}
							else
							{
								colPatientType.Caption = "Mức ăn/ĐTTT";
								colPatientType.ToolTip = "Mức ăn/Đối tượng thanh toán";
							}
						}

					}
				}
				else result = null;
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				result = null;
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			return result;
		}

		private void cboBuaAn_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
				{
					cboBuaAn.EditValue = null;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboSuatAn_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
				{
					cboSuatAn.EditValue = null;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ButtonEdit__Print_Click_1(object sender, EventArgs e)
		{
			ButtonEdit__Print_Click(null, null);
		}

		private void gridControlSereSevRationDetail_Click(object sender, EventArgs e)
		{

		}
	}
}
