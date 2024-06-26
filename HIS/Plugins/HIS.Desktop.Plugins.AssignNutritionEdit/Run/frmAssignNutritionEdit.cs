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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.AssignNutritionEdit.ADO;
using HIS.Desktop.Plugins.AssignNutritionEdit.Config;
using HIS.Desktop.Plugins.AssignNutritionEdit.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
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

namespace HIS.Desktop.Plugins.AssignNutritionEdit.Run
{
	public partial class frmAssignNutritionEdit : FormBase
	{
		Dictionary<long, long?> DicCapacity = new Dictionary<long, long?>();
		Dictionary<long, decimal> DicAmount = new Dictionary<long, decimal>();
		Inventec.Desktop.Common.Modules.Module currentModule;
		long serviceReqId;
		RefeshReference Refesh;
		HIS_SERVICE_REQ currentServiceReq { get; set; }
		List<HIS_SERVICE> lstService { get; set; }
		List<V_HIS_SERVICE_ROOM> lstServiceRoomView { get; set; }
		List<HIS_SERVICE_RATI> lstServiceRati { get; set; }
		List<SSServiceADO> ServiceIsleafADOs { get; set; }
		List<ServiceADO> ServiceAllADOs { get; set; }
		List<ServiceADO> ServiceParentADOs;
		List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter { get; set; }
		public V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter { get; private set; }
		internal List<long> intructionTimeSelecteds = new List<long>();
		Dictionary<long, List<V_HIS_SERVICE_PATY>> servicePatyInBranchs { get; set; }
		Dictionary<long, V_HIS_SERVICE> dicServices { get; set; }
		MOS.EFMODEL.DataModels.V_HIS_ROOM requestRoom { get; set; }
		HisTreatmentWithPatientTypeInfoSDO currentHisTreatment { get; set; }
		List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1> sereServWithTreatment = new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1>();
		private bool isSaveAndPrint;
		HisServiceReqRationUpdateResultSDO resultSDO { get; set; }
		List<long> lstServiceIds { get; set; }
		decimal min = 1;
		#region Construct

		public frmAssignNutritionEdit(Inventec.Desktop.Common.Modules.Module module, AssignServiceEditADO ado)
			: base(module)
		{
			try
			{
				InitializeComponent();

				#region ---- Icon ----
				try
				{
					string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
					this.Icon = Icon.ExtractAssociatedIcon(iconPath);

					if (module != null)
					{
						this.Text = module.text;
					}
				}
				catch (Exception ex)
				{
					LogSystem.Warn(ex);
				}
				#endregion
				this.currentModule = module;
				this.serviceReqId = ado.serviceReqId;
				this.Refesh = ado.DelegateRefeshReference;

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		#endregion

		private void gvAssignNutrition_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
		{
			try
			{
				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
					{
						SSServiceADO oneServiceSDO = (SSServiceADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
						long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
						if (oneServiceSDO != null)
						{
							if (e.Column.FieldName == "PRICE_DISPLAY" && oneServiceSDO.IsChecked)
							{
								if (oneServiceSDO.PATIENT_TYPE_ID != 0 && this.servicePatyInBranchs.ContainsKey(oneServiceSDO.ID) && instructionTime > 0)
								{
									List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = new List<V_HIS_EXECUTE_ROOM>();
									var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
									var executeRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
									List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> arrExcuteRoomCode = new List<V_HIS_SERVICE_ROOM>();
									if (executeRoomViews != null && executeRoomViews.Count > 0 && serviceRoomViews != null && serviceRoomViews.Count > 0)
									{
										arrExcuteRoomCode = serviceRoomViews.Where(o => oneServiceSDO != null && o.SERVICE_ID == oneServiceSDO.ID).ToList();
										dataCombo = ((arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0 && executeRoomViews != null) ? executeRoomViews.Where(o => arrExcuteRoomCode.Select(p => p.ROOM_ID).Contains(o.ROOM_ID)).ToList() : null);
									}
									var checkExecuteRoom = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault(o => o.BRANCH_ID == this.requestRoom.BRANCH_ID) : null;
									if (checkExecuteRoom != null)
									{
										oneServiceSDO.TDL_EXECUTE_BRANCH_ID = checkExecuteRoom.BRANCH_ID;
									}
									else
									{
										oneServiceSDO.TDL_EXECUTE_BRANCH_ID = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault().BRANCH_ID : HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
									}
									List<V_HIS_SERVICE_PATY> servicePaties = this.servicePatyInBranchs[oneServiceSDO.ID];
									var oneServicePatyPrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(
										servicePaties,
										oneServiceSDO.TDL_EXECUTE_BRANCH_ID,
										null,
										this.requestRoom.ID,
										this.requestRoom.DEPARTMENT_ID,
										instructionTime,
										this.currentHisTreatment.IN_TIME,
										oneServiceSDO.ID,
										oneServiceSDO.PATIENT_TYPE_ID,
										 null, null, null, null, null, this.currentServiceReq.RATION_TIME_ID
										);
									if (oneServicePatyPrice != null)
									{
										e.Value = (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO));
									}
								}
								else
								{
									Inventec.Common.Logging.LogSystem.Debug("oneServiceSDO.PATIENT_TYPE_ID else continued");
								}
							}
							else if (e.Column.FieldName == "ACTUAL_PRICE_DISPLAY" && oneServiceSDO.IsChecked)
							{
								if (oneServiceSDO.PATIENT_TYPE_ID != 0 && this.servicePatyInBranchs.ContainsKey(oneServiceSDO.ID) && instructionTime > 0)
								{
									List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = new List<V_HIS_EXECUTE_ROOM>();
									var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
									var executeRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
									List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> arrExcuteRoomCode = new List<V_HIS_SERVICE_ROOM>();
									if (executeRoomViews != null && executeRoomViews.Count > 0 && serviceRoomViews != null && serviceRoomViews.Count > 0)
									{
										arrExcuteRoomCode = serviceRoomViews.Where(o => oneServiceSDO != null && o.SERVICE_ID == oneServiceSDO.ID).ToList();
										dataCombo = ((arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0 && executeRoomViews != null) ? executeRoomViews.Where(o => arrExcuteRoomCode.Select(p => p.ROOM_ID).Contains(o.ROOM_ID)).ToList() : null);
									}
									var checkExecuteRoom = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault(o => o.BRANCH_ID == this.requestRoom.BRANCH_ID) : null;
									if (checkExecuteRoom != null)
									{
										oneServiceSDO.TDL_EXECUTE_BRANCH_ID = checkExecuteRoom.BRANCH_ID;
									}
									else
									{
										oneServiceSDO.TDL_EXECUTE_BRANCH_ID = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault().BRANCH_ID : HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
									}
									List<V_HIS_SERVICE_PATY> servicePaties = this.servicePatyInBranchs[oneServiceSDO.ID];
									var oneServicePatyPrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(
										servicePaties,
										oneServiceSDO.TDL_EXECUTE_BRANCH_ID,
										null,
										this.requestRoom.ID,
										this.requestRoom.DEPARTMENT_ID,
										instructionTime,
										this.currentHisTreatment.IN_TIME,
										oneServiceSDO.ID,
										oneServiceSDO.PATIENT_TYPE_ID,
										null
										);
									if (oneServicePatyPrice != null)
									{
										e.Value = oneServicePatyPrice.ACTUAL_PRICE;
									}
								}
							}
						}
						else
						{
							e.Value = null;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gvAssignNutrition_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
		{
			try
			{
				if (e.RowHandle >= 0)
				{
					long _serviceId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewService.GetRowCellValue(e.RowHandle, "ID") ?? "").ToString().Trim());
					if (e.Column.FieldName == "AMOUNT")
					{
						short isMultiRequest = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewService.GetRowCellValue(e.RowHandle, "IS_MULTI_REQUEST") ?? "").ToString().Trim());
						if (isMultiRequest == 1)
						{
							e.RepositoryItem = this.repositoryItemSpinEdit__E;
						}
						else
						{
							e.RepositoryItem = this.repositoryItemSpinEdit__D;
						}
					}
					if (e.Column.FieldName == "NOTE")
					{
						if ((bool)(gridViewService.GetRowCellValue(e.RowHandle, "IsChecked") ?? ""))
						{
							e.RepositoryItem = this.repNote;
						}
						else
						{
							e.RepositoryItem = null;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void frmAssignNutritionEdit_Load(object sender, EventArgs e)
		{
			try
			{
				CommonParam param = new CommonParam();
				MOS.Filter.HisServiceReqFilter filter = new HisServiceReqFilter();
				filter.IS_ACTIVE = 1;
				filter.ID = serviceReqId;
				currentServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
				this.requestRoom = GetRequestRoom(this.currentModule.RoomId);
				if (currentServiceReq != null)
				{
					lblTreatmentCode.Text = currentServiceReq.TDL_TREATMENT_CODE;
					lblPatientName.Text = currentServiceReq.TDL_PATIENT_NAME;
					lblPatientCode.Text = currentServiceReq.TDL_PATIENT_CODE;
					lblServiceReqCode.Text = currentServiceReq.SERVICE_REQ_CODE;
					lblIntructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentServiceReq.INTRUCTION_TIME);
					lblIcd.Text = currentServiceReq.ICD_CODE + " - " + currentServiceReq.ICD_NAME;
					lblSubIcd.Text = currentServiceReq.ICD_SUB_CODE + " - " + currentServiceReq.ICD_TEXT;
					intructionTimeSelecteds.Add(currentServiceReq.INTRUCTION_TIME);
				}
				this.LoadDataToCurrentTreatmentData(currentServiceReq.TREATMENT_ID, this.intructionTimeSelecteds.FirstOrDefault());
				this.ProcessDataWithTreatmentWithPatientTypeInfo();
				this.LoadServicePaty();
				this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
				this.BindTree();
				LoadDataToGrid(false, true);
				EnableSave();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitComboRepositoryPatientType(List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
		{
			try
			{
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
				columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
				if (data != null)
				{
					ControlEditorLoader.Load(this.repositoryItemGridLookUpEdit_PatientType, data, controlEditorADO);
				}
				else
					ControlEditorLoader.Load(this.repositoryItemGridLookUpEdit_PatientType, this.currentPatientTypeWithPatientTypeAlter, controlEditorADO);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		V_HIS_ROOM GetRequestRoom(long requestRoomId)
		{
			V_HIS_ROOM result = new V_HIS_ROOM();
			try
			{
				if (requestRoomId > 0)
				{
					result = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == requestRoomId);
				}
			}
			catch (Exception ex)
			{
				result = new V_HIS_ROOM();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return result;
		}

		private void gvAssignNutrition_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
		{
			try
			{
				var sereServADO = (SSServiceADO)this.gridViewService.GetFocusedRow();
				if (sereServADO != null)
				{
					if (e.Column.FieldName == this.grcChecked_TabService.FieldName)
					{
						if (sereServADO.IsChecked)
						{
							this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.ID, sereServADO);
							this.ValidServiceDetailProcessing(sereServADO);
							if(DicAmount.ContainsKey(sereServADO.ID))
								sereServADO.AMOUNT = DicAmount[sereServADO.ID];
							else
								sereServADO.AMOUNT = min;
							if (DicCapacity.ContainsKey(sereServADO.ID))
								sereServADO.CAPACITY = DicCapacity[sereServADO.ID];
						}
						else
						{
							this.ResetOneService(sereServADO);
						}
						this.gridControlService.RefreshDataSource();
					}
					EnableSave();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void EnableSave()
		{
			try
			{
				if (this.ServiceIsleafADOs.FirstOrDefault(o => o.IsChecked) != null)
				{
					btnSave.Enabled = true;
					btnSaveAndPrint.Enabled = true;
				}
				else
				{
					btnSave.Enabled = false;
					btnSaveAndPrint.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void ResetOneService(SSServiceADO item)
		{
			try
			{
				item.IsChecked = false;
				//item.NOTE = null;
				item.AMOUNT = 0;
				item.PATIENT_TYPE_ID = 0;
				item.PATIENT_TYPE_CODE = null;
				item.PATIENT_TYPE_NAME = null;
				item.ROOM_ID = 0;
				item.PRICE = 0;
				item.RationTimeIds = null;
				item.NOTE = null;
				item.CAPACITY = null;

			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
		}

		private void gridViewService_DoubleClick(object sender, EventArgs e)
		{
			try
			{
				if (!this.ValidPatientTypeForAdd())
					return;

				GridView view = (GridView)sender;
				Point pt = view.GridControl.PointToClient(Control.MousePosition);
				GridHitInfo info = view.CalcHitInfo(pt);
				if ((info.InRow || info.InRowCell)
					&& info.Column.FieldName != this.grcChecked_TabService.FieldName
					&& info.Column.FieldName != this.gridColumnPatientTypeName__TabService.FieldName
					&& info.Column.FieldName != this.grcAmount_TabService.FieldName
				   )
				{
					var sereServADO = (SSServiceADO)this.gridViewService.GetFocusedRow();
					if (sereServADO != null)
					{
						UpdateCurrentFocusRow(sereServADO);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void UpdateCurrentFocusRow(SSServiceADO sereServADO)
		{
			try
			{
				if (sereServADO == null)
					return;

				sereServADO.IsChecked = !sereServADO.IsChecked;
				if (sereServADO.IsChecked)
				{
					this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.ID, sereServADO);
					this.ChooseExecuteRoomDefaultlService(sereServADO.ID, sereServADO);

					this.ValidServiceDetailProcessing(sereServADO);
				}
				else
				{
					this.ResetOneService(sereServADO);
				}

				this.gridControlService.RefreshDataSource();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void LoadDataToGrid(bool isAutoSetPaty, bool IsFirstLoadForm)
		{
			try
			{
				List<SSServiceADO> listSSServiceADO = null;
				this.gridControlService.DataSource = null;
				var allDatas = this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count > 0 ? this.ServiceIsleafADOs.AsQueryable() : null;
				if (this.toggleSwitchDataChecked.EditValue != null && (this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() == "true" && allDatas != null && allDatas.Count() > 0 && isAutoSetPaty)
				{
					Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 1");
					listSSServiceADO = allDatas.Where(o => o.IsChecked).ToList();
				}
				else
				{
					listSSServiceADO = allDatas != null && allDatas.Count() > 0 ? allDatas.ToList() : null;
				}

				if (IsFirstLoadForm)
				{
					MOS.Filter.HisSereServRationFilter sereServViewFilter = new MOS.Filter.HisSereServRationFilter();
					sereServViewFilter.SERVICE_REQ_ID = this.serviceReqId;
					var sereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_RATION>>("api/HisSereServRation/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
					lstServiceIds = new List<long>();
					min = sereServs.Min(o => o.AMOUNT);
					
					if (sereServs != null && sereServs.Count > 0)
					{
						foreach (var item in listSSServiceADO)
						{
							var ss = sereServs.FirstOrDefault(o => o.SERVICE_ID == item.ID);
							if (ss != null)
							{
								lstServiceIds.Add(ss.ID);
								item.IsChecked = true;
								item.AMOUNT = ss.AMOUNT;
								item.PATIENT_TYPE_ID = ss.PATIENT_TYPE_ID;
								item.NOTE = ss.INSTRUCTION_NOTE;
								item.SERE_SERV_RATIO_ID = ss.ID;
								if (DicCapacity.ContainsKey(item.ID))
									item.CAPACITY = DicCapacity[item.ID];
								if (!DicAmount.ContainsKey(item.ID))
									DicAmount[item.ID] = item.AMOUNT;
							}													
						}
					}
				}
				Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 5");
				this.gridViewService.GridControl.DataSource = listSSServiceADO != null && listSSServiceADO.Count > 0 ? listSSServiceADO.Distinct().OrderByDescending(o => o.NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList() : null;

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private bool ValidPatientTypeForAdd()
		{
			bool valid = true;
			try
			{
				if (this.currentHisPatientTypeAlter == null || this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == 0)
				{
					MessageManager.Show(String.Format(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh, Inventec.Common.DateTime.Convert.TimeNumberToDateString(intructionTimeSelecteds.First())));
					valid = false;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return valid;
		}

		private void gridViewService_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					var sereServADO = (SSServiceADO)this.gridViewService.GetFocusedRow();
					if (sereServADO != null)
					{
						//sereServADO.IsChecked = true;
						if (sereServADO.IsChecked)
						{
							this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.ID, sereServADO);
							this.ChooseExecuteRoomDefaultlService(sereServADO.ID, sereServADO);
							this.ValidServiceDetailProcessing(sereServADO);
						}
						else
						{
							this.ResetOneService(sereServADO);
						}

						this.gridControlService.RefreshDataSource();
					}
				}
				else if (e.KeyCode == Keys.Space)
				{
					var sereServADO = (SSServiceADO)this.gridViewService.GetFocusedRow();
					if (sereServADO != null)
					{
						UpdateCurrentFocusRow(sereServADO);
					}
				}
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
		}

		private void gridViewService_MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
				{
					GridView view = sender as GridView;
					GridHitInfo hi = view.CalcHitInfo(e.Location);
					if (hi.InRowCell)
					{
						if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
						{
							view.FocusedRowHandle = hi.RowHandle;
							view.FocusedColumn = hi.Column;
							view.ShowEditor();
							CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
							DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
							Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
							GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
							Rectangle gridGlyphRect =
								new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
								 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
								 glyphRect.Width,
								 glyphRect.Height);
							if (!gridGlyphRect.Contains(e.Location))
							{
								view.CloseEditor();
								if (!view.IsCellSelected(hi.RowHandle, hi.Column))
								{
									view.SelectCell(hi.RowHandle, hi.Column);
								}
								else
								{
									view.UnselectCell(hi.RowHandle, hi.Column);
								}
							}
							else
							{
								checkEdit.Checked = !checkEdit.Checked;
								view.CloseEditor();
							}
							(e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewService_ShownEditor(object sender, EventArgs e)
		{
			try
			{
				DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
				SSServiceADO data = view.GetFocusedRow() as SSServiceADO;
				if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
				{
					GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
					if (data != null)
					{
						this.FillDataIntoPatientTypeCombo(data, editor);
						editor.EditValue = data.PATIENT_TYPE_ID;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void FillDataIntoPatientTypeCombo(SSServiceADO data, GridLookUpEdit patientTypeCombo)
		{
			try
			{
				if (patientTypeCombo != null && this.servicePatyInBranchs != null && this.servicePatyInBranchs.Count > 0)
				{
					List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = null;
					if (this.servicePatyInBranchs.ContainsKey(data.ID))
					{
						var servicePaties = this.servicePatyInBranchs[data.ID];
						//Chỉ hiển thị ra các chính sách giá không khai báo “Đối tượng chi tiết" hoặc có thông tin “Đối tượng chi tiết” trùng với thông tin đối tượng chi tiết của bệnh nhân
						servicePaties = servicePaties.Where(o => !o.PATIENT_CLASSIFY_ID.HasValue || o.PATIENT_CLASSIFY_ID.Value == currentHisTreatment.TDL_PATIENT_CLASSIFY_ID).ToList();

						var arrPatientTypeCode = servicePaties != null && servicePaties.Count > 0 ? servicePaties.Select(o => o.PATIENT_TYPE_CODE).Distinct().ToList() : null;

						dataCombo = (arrPatientTypeCode != null && arrPatientTypeCode.Count > 0 ? currentPatientTypeWithPatientTypeAlter.Where(o => arrPatientTypeCode.Contains(o.PATIENT_TYPE_CODE) && o.ID != HisConfigCFG.PatientTypeId__BHYT).ToList() : null);
					}

					this.InitComboPatientType(patientTypeCombo, dataCombo);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void InitComboPatientType(GridLookUpEdit cboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
		{
			try
			{
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
				columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
				ControlEditorLoader.Load(cboPatientType, data, controlEditorADO);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void toggleSwitchDataChecked_Toggled(object sender, EventArgs e)
		{
			try
			{
				ToggleSwitch toggleSwitch = sender as ToggleSwitch;
				if (toggleSwitch != null)
				{
					this.LoadDataToGrid(true, false);
					EnableSave();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewService_RowCellStyle(object sender, RowCellStyleEventArgs e)
		{
			try
			{
				if (this.currentHisPatientTypeAlter != null
						&& this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
				{
					//var index = this.gridViewService.GetDataSourceRowIndex(e.RowHandle);
					//if (index < 0) return;

					//var listDatas = this.gridControlService.DataSource as List<SereServADO>;
					//var dataRow = listDatas[index];
					//if (dataRow != null && dataRow.PATIENT_TYPE_ID > 0
					//    && dataRow.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT)
					//{
					//    //Đối tượng điều trị là BHYT, nhưng do ko có chính sách giá theo BHYT nên khi tích chọn dịch vụ, sẽ hiển thị màu tím.
					//    //Có chính sách giá nhưng là đối tượng khác, không phải BHYT ==> màu tím

					//    var bFindservice = (BranchDataWorker.DicServicePatyInBranch != null
					//        && BranchDataWorker.DicServicePatyInBranch.ContainsKey(dataRow.SERVICE_ID)) ? BranchDataWorker.HasServicePatyWithListPatientType(dataRow.SERVICE_ID, new List<long>() { this.currentHisPatientTypeAlter.PATIENT_TYPE_ID }) : false;
					//    if (!bFindservice)
					//        e.Appearance.ForeColor = System.Drawing.Color.Violet;
					//}

					//if (dataRow != null)
					//{
					//    var bFindservice = !String.IsNullOrWhiteSpace(dataRow.TDL_HEIN_SERVICE_BHYT_CODE) && (BranchDataWorker.DicServicePatyInBranch != null
					//       && BranchDataWorker.DicServicePatyInBranch.ContainsKey(dataRow.SERVICE_ID)) ? BranchDataWorker.HasServicePatyWithListPatientType(dataRow.SERVICE_ID, new List<long>() { HisConfigCFG.PatientTypeId__BHYT }) : false;
					//    if (bFindservice && !String.IsNullOrEmpty(HisConfigCFG.BhytColorCode))
					//    {
					//        e.Appearance.ForeColor = GetColor(HisConfigCFG.BhytColorCode);
					//    }
					//}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		Color GetColor(string colorCode)
		{
			try
			{
				if (!String.IsNullOrEmpty(colorCode))
				{
					return System.Drawing.ColorTranslator.FromHtml(colorCode);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			return Color.Red;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			try
			{
				this.ProcessSaveData(false);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ProcessSaveData(bool isSaveAndPrint)
		{
			try
			{
				if (this.gridViewService.IsEditing)
					this.gridViewService.CloseEditor();

				if (this.gridViewService.FocusedRowModified)
					this.gridViewService.UpdateCurrentRow();
				CommonParam param = new CommonParam();
				bool success = false;
				WaitingManager.Show();

				var dtSend = ServiceIsleafADOs.Where(o => o.IsChecked).ToList();
				if (dtSend != null && dtSend.Count > 0)
				{
					var dataGridMaxlengthDes = dtSend.Where(o => !String.IsNullOrEmpty(o.NOTE) && Encoding.UTF8.GetByteCount(o.NOTE) > 500);
					if (dataGridMaxlengthDes != null && dataGridMaxlengthDes.Count() > 0)
					{
						WaitingManager.Hide();
						MessageManager.Show(String.Join(", ", dataGridMaxlengthDes.Select(o => o
							.SERVICE_NAME).ToList()) + " ghi chú vượt quá độ dài cho phép [500 kí tự]");
						return;
					}
				}
				HisServiceReqRationUpdateSDO sdo = new HisServiceReqRationUpdateSDO();
				sdo.ExecuteRoomId = currentModule.RoomId;
				sdo.ServiceReqId = serviceReqId;
				sdo.InsertServices = new List<RationServiceSDO>();
				sdo.UpdateServices = new List<RationServiceSDO>();
				sdo.DeleteSereServRationIds = new List<long>();


				var serviceNew = dtSend.Where(o => !lstServiceIds.Exists(p => p == o.SERE_SERV_RATIO_ID)).ToList();
				var serviceOld = dtSend.Where(o => lstServiceIds.Exists(p => p == o.SERE_SERV_RATIO_ID)).ToList();
				var serviceDelete = lstServiceIds.Where(o => !dtSend.Exists(p => p.SERE_SERV_RATIO_ID == o)).ToList();
				sdo.DeleteSereServRationIds = serviceDelete;
				foreach (var item in serviceNew)
				{
					RationServiceSDO s = new RationServiceSDO();
					s.RoomId = currentModule.RoomId;
					s.PatientTypeId = item.PATIENT_TYPE_ID;
					s.RationTimeIds = new List<long> { this.currentServiceReq.RATION_TIME_ID ?? 0 };
					s.ServiceId = item.ID;
					s.InstructionNote = item.NOTE;
					s.Amount = item.AMOUNT;
					sdo.InsertServices.Add(s);
				}
				foreach (var item in serviceOld)
				{
					RationServiceSDO s = new RationServiceSDO();
					s.RoomId = currentModule.RoomId;
					s.PatientTypeId = item.PATIENT_TYPE_ID;
					s.RationTimeIds = new List<long> { this.currentServiceReq.RATION_TIME_ID ?? 0 };
					s.ServiceId = item.ID;
					s.InstructionNote = item.NOTE;
					s.Amount = item.AMOUNT;
					s.SereServRationId = item.SERE_SERV_RATIO_ID;
					sdo.UpdateServices.Add(s);
				}
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
				//Gọi api chỉ định dv
				this.resultSDO = new BackendAdapter(param).Post<HisServiceReqRationUpdateResultSDO>("api/HisServiceReq/UpdateSereServRation", ApiConsumers.MosConsumer, sdo, ProcessLostToken, param);

				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultSDO), resultSDO));
				if (this.resultSDO != null)
				{
					if(resultSDO.SereServRations!=null && resultSDO.SereServRations.Count>0)
					{
						lstServiceIds = resultSDO.SereServRations.Select(o=>o.ID).ToList();
					}	
					success = true;
					if (isSaveAndPrint)
					{
						ProcessingPrintV2("Mps000275");
					}
					if (Refesh != null)
						Refesh();
				}

				WaitingManager.Hide();

				#region Show message
				MessageManager.Show(this, param, success);
				#endregion

				#region Process has exception
				SessionManager.ProcessTokenLost(param);
				#endregion
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repNote_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				var data = (SSServiceADO)gridViewService.GetFocusedRow();
				ButtonEdit editor = sender as ButtonEdit;
				Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
				popupControlContainer1.ShowPopup(new Point(buttonPosition.X + 400, buttonPosition.Bottom + 340));
				memNote.Text = data.NOTE;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			try
			{
				SSServiceADO testLisResultADO = new SSServiceADO();
				var data = (SSServiceADO)gridViewService.GetFocusedRow();
				if (data != null && data is SSServiceADO)
				{
					testLisResultADO = (SSServiceADO)data;
				}
				if (memNote.Text != null && memNote.Text != "")
				{
					testLisResultADO.NOTE = memNote.Text;
				}
				else
				{
					testLisResultADO.NOTE = null;
				}
				gridControlService.RefreshDataSource();
				gridViewService.FocusedColumn = gridColumn1;
				popupControlContainer1.HidePopup();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			try
			{
				memNote.Text = null;
				popupControlContainer1.HidePopup();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnSaveAndPrint_Click(object sender, EventArgs e)
		{
			try
			{
				this.ProcessSaveData(true);
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
				if (!btnSaveAndPrint.Enabled)
					return;
				btnSaveAndPrint_Click(null, null);
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				if (!btnSave.Enabled)
					return;
				btnSave_Click(null, null);
			}
			catch (Exception ex)
			{

				throw;
			}
		}
	}
}
