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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignNutrition.ADO;
using HIS.Desktop.Plugins.AssignNutrition.Config;
using HIS.Desktop.Plugins.AssignNutrition.Resources;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utilities.Extentions;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignNutrition.AssignNutrition
{
	public partial class frmAssignNutrition : HIS.Desktop.Utility.FormBase
	{
		int rowIndexSelected = -1;
		int rowCount;
		List<long> treatmentIds = new List<long>();
		public List<V_HIS_TREATMENT_BED_ROOM> GetSelectedRows()
		{
			List<V_HIS_TREATMENT_BED_ROOM> result = new List<V_HIS_TREATMENT_BED_ROOM>();
			try
			{
				int[] selectRows = gridViewTreatmentBedRoom.GetSelectedRows();
				if (selectRows != null && selectRows.Count() > 0)
				{
					for (int i = 0; i < selectRows.Count(); i++)
					{
						var mediMatyTypeADO = (V_HIS_TREATMENT_BED_ROOM)gridViewTreatmentBedRoom.GetRow(selectRows[i]);
						result.Add(mediMatyTypeADO);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			return result;
		}
		private async Task FillDataToGridTreatment()
		{
			try
			{
				WaitingManager.Show();
				this.ListTreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();
				this.gridControlTreatmentBedRoom.DataSource = null;
				CommonParam paramCommon = new CommonParam();
				MOS.Filter.HisTreatmentBedRoomViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();

				if (this.treatmentBedRoomLViewFilterInput != null)
				{
					treatFilter.IS_IN_ROOM = this.treatmentBedRoomLViewFilterInput.IS_IN_ROOM;
				}
				else
				{
					treatFilter.IS_IN_ROOM = true;
				}
				if (!String.IsNullOrEmpty(txtKeyWord.Text))
					treatFilter.KEYWORD__PATIENT_NAME__TREATMENT_CODE__BED_NAME__PATIENT_CODE = txtKeyWord.Text;
				long bedRoomId = 0;
				MOS.EFMODEL.DataModels.V_HIS_BED_ROOM data = BackendDataWorker.Get<V_HIS_BED_ROOM>().SingleOrDefault(o => o.ROOM_ID == this.GetRoomId());
				if (data != null)
					bedRoomId = data.ID;
				if (!HisConfigCFG.IsChooseRoomGroupRoomOption)
				{
					treatFilter.BED_ROOM_ID = bedRoomId;
					treatFilter.ORDER_DIRECTION = "ASC";
					treatFilter.ORDER_FIELD = "TDL_PATIENT_FIRST_NAME";
				}
				var rs = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, treatFilter, paramCommon);
				List<int> indexRow = new List<int>();
				if (rs != null && rs.Count > 0)
				{
					if (!HisConfigCFG.IsChooseRoomGroupRoomOption)
					{
						this.ListTreatmentBedRooms = (List<V_HIS_TREATMENT_BED_ROOM>)rs.OrderBy(p => p.TDL_PATIENT_FIRST_NAME).ToList();
					}
					else
					{
						this.ListTreatmentBedRooms = (List<V_HIS_TREATMENT_BED_ROOM>)rs.Where(o => o.DEPARTMENT_ID == BackendDataWorker.Get<HIS_ROOM>().First(p => p.ID == currentModule.RoomId).DEPARTMENT_ID).OrderBy(o => o.BED_ROOM_NAME).ThenBy(o => o.TDL_PATIENT_FIRST_NAME).ToList();

					}
				}

				rowCount = (ListTreatmentBedRooms == null ? 0 : ListTreatmentBedRooms.Count);
				if (this.isInitForm)
				{
					for (int i = 0; i < rowCount; i++)
					{
						if (ListTreatmentBedRooms[i].TREATMENT_ID == this.treatmentId)
						{
							rowIndexSelected = i;
							break;
						}
					}
				}
				else
				{
					for (int i = 0; i < rowCount; i++)
					{
						if (treatmentIds.Exists(o => o == ListTreatmentBedRooms[i].TREATMENT_ID))
						{
							indexRow.Add(i);
						}
					}
					if (indexRow == null || indexRow.Count == 0)
					{
						for (int i = 0; i < rowCount; i++)
						{
							if (ListTreatmentBedRooms[i].TREATMENT_ID == treatmentId)
							{
								indexRow.Add(i);
							}
						}
					}
				}



				gridControlTreatmentBedRoom.BeginUpdate();
				gridControlTreatmentBedRoom.DataSource = this.ListTreatmentBedRooms;
				gridControlTreatmentBedRoom.EndUpdate();
				if (indexRow != null && indexRow.Count > 0)
				{
					for (int i = 0; i < indexRow.Count; i++)
					{
						gridViewTreatmentBedRoom.SelectRow(indexRow[i]);
					}
					gridViewTreatmentBedRoom.FocusedRowHandle = indexRow.LastOrDefault();
				}
				else if (this.rowIndexSelected >= 0)
				{
					gridViewTreatmentBedRoom.FocusedRowHandle = rowIndexSelected;
					gridViewTreatmentBedRoom.SelectRow(rowIndexSelected);
				}

				this.isInitForm = false;
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ValidServiceDetailProcessing(SSServiceADO sereServADO, bool isValidExecuteRoom)
		{
			try
			{
				if (sereServADO != null && 1 > 2)
				{
					bool vlPatientTypeId = (sereServADO.IsChecked && sereServADO.PATIENT_TYPE_ID <= 0);
					sereServADO.ErrorMessagePatientTypeId = (vlPatientTypeId ? Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc) : "");
					sereServADO.ErrorTypePatientTypeId = (vlPatientTypeId ? ErrorType.Warning : ErrorType.None);

					bool vlAmount = (sereServADO.IsChecked && sereServADO.AMOUNT <= 0);
					sereServADO.ErrorMessageAmount = (vlAmount ? Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc) : "");
					sereServADO.ErrorTypeAmount = (vlAmount ? ErrorType.Warning : ErrorType.None);

					List<V_HIS_SERE_SERV_1> serviceSames = null;
					List<SSServiceADO> serviceSameResult = null;
					CheckServiceSameByServiceId(sereServADO, ServiceSameADO.ServiceSameAllADOs, ref serviceSames, ref serviceSameResult);
					var existsSereServInDate = this.sereServWithTreatment.Any(o => o.SERVICE_ID == sereServADO.ID && o.INTRUCTION_TIME.ToString().Substring(0, 8) == intructionTimeSelecteds.First().ToString().Substring(0, 8));

					if (existsSereServInDate && (serviceSames != null && serviceSames.Count > 0))
					{
						sereServADO.ErrorMessageIsAssignDay = String.Format("", string.Join("; ", serviceSames.Select(o => o.TDL_SERVICE_NAME).ToArray()));
						sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
					}
					else if (existsSereServInDate)
					{
						sereServADO.ErrorMessageIsAssignDay = (existsSereServInDate ? ResourceMessage.CanhBaoDichVuDaChiDinhTrongNgay : "");
						sereServADO.ErrorTypeIsAssignDay = (existsSereServInDate ? ErrorType.Warning : ErrorType.None);
					}
					else if (serviceSames != null && serviceSames.Count > 0)
					{
						sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuCungCoCheDaChiDinhTrongNgay, string.Join("; ", serviceSames.Select(o => o.TDL_SERVICE_NAME).ToArray()));
						sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
					}
					else if (serviceSameResult != null && serviceSameResult.Count > 0)
					{
						sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuCungCoChe, string.Join("; ", serviceSameResult.Select(o => o.SERVICE_NAME).ToArray()));
						sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
					}
					else
					{
						sereServADO.ErrorMessageIsAssignDay = "";
						sereServADO.ErrorTypeIsAssignDay = ErrorType.None;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void CheckServiceSameByServiceId(SSServiceADO sereServADO, List<V_HIS_SERVICE_SAME> serviceSameAll, ref List<V_HIS_SERE_SERV_1> result, ref List<SSServiceADO> resultSelect)
		{
			try
			{
				result = null;
				resultSelect = null;
				if (sereServADO != null && serviceSameAll != null && serviceSameAll.Count > 0)
				{
					//Lay ra cac dich vu cung co che voi dich vu dang duoc chon

					//Lay cac dich vu cung co che voi no
					List<long> serviceSameId1s = serviceSameAll
						.Where(o => o.SERVICE_ID == sereServADO.ID && o.SAME_ID != sereServADO.ID)
						.Select(o => o.SAME_ID).ToList();
					//Hoac cac dich vu ma no cung co che
					List<long> serviceSameId2s = serviceSameAll
						.Where(o => o.SAME_ID == sereServADO.ID && o.SERVICE_ID != sereServADO.ID)
						.Select(o => o.SERVICE_ID).ToList();

					List<long> serviceSameIds = new List<long>();

					if (serviceSameId1s != null)
					{
						serviceSameIds.AddRange(serviceSameId1s);
					}
					if (serviceSameId2s != null)
					{
						serviceSameIds.AddRange(serviceSameId2s);
					}
					result = new List<V_HIS_SERE_SERV_1>();

					if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count() > 0)
					{
						var checkServiceSame = this.sereServWithTreatment.Where(o => serviceSameIds.Contains(o.SERVICE_ID));

						if (checkServiceSame != null && checkServiceSame.Count() > 0)
						{

							var groupServiceSame = checkServiceSame.GroupBy(o => o.SERVICE_ID).ToList();
							foreach (var serviceSameItems in groupServiceSame)
							{
								result.Add(serviceSameItems.FirstOrDefault());
							}
						}
						else
						{
							result = null;
						}
					}

					List<SSServiceADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
					if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
					{
						var checkServiceSame = serviceCheckeds__Send.Where(o => serviceSameIds.Contains(o.ID));
						resultSelect = new List<SSServiceADO>();
						if (checkServiceSame != null && checkServiceSame.Count() > 0)
						{
							var groupServiceSame = checkServiceSame.GroupBy(o => o.ID).ToList();
							foreach (var serviceSameItems in groupServiceSame)
							{
								resultSelect.Add(serviceSameItems.FirstOrDefault());
							}
						}
						else
						{
							resultSelect = null;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ValidServiceDetailProcessing(SSServiceADO sereServADO)
		{
			try
			{
				this.ValidServiceDetailProcessing(sereServADO, false);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void SetDefaultSerServTotalPrice()
		{
			try
			{
				decimal totalPrice = 0;
				long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
				if (this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count > 0)
				{
					var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
					var executeRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
					foreach (var item in this.ServiceIsleafADOs)
					{
						item.CaptionRationTime = null;
						if (item.IsChecked && item.PATIENT_TYPE_ID != 0)
						{
							var servicePaties = this.servicePatyInBranchs[item.ID].ToList();
							V_HIS_SERVICE_PATY data_ServicePrice = null;
							if (servicePaties != null && servicePaties.Count > 0 && this.requestRoom != null)
							{
								//Chỉ hiển thị ra các chính sách giá không khai báo “Đối tượng chi tiết" hoặc có thông tin “Đối tượng chi tiết” trùng với thông tin đối tượng chi tiết của bệnh nhân
								servicePaties = servicePaties.Where(o => !o.PATIENT_CLASSIFY_ID.HasValue || o.PATIENT_CLASSIFY_ID.Value == currentHisTreatment.TDL_PATIENT_CLASSIFY_ID).ToList();

								List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = new List<V_HIS_EXECUTE_ROOM>();

								if (executeRoomViews != null && executeRoomViews.Count > 0 && serviceRoomViews != null && serviceRoomViews.Count > 0)
								{
									var arrExcuteRoomCode = serviceRoomViews.Where(o => item != null && o.SERVICE_ID == item.ID).Select(o => o.ROOM_ID).ToArray();
									dataCombo = ((arrExcuteRoomCode != null && arrExcuteRoomCode.Count() > 0 && executeRoomViews != null) ? executeRoomViews.Where(o => arrExcuteRoomCode.Contains(o.ROOM_ID)).ToList() : null);
								}
								var checkExecuteRoom = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault(o => o.BRANCH_ID == this.requestRoom.BRANCH_ID) : null;
								if (checkExecuteRoom != null)
								{
									item.TDL_EXECUTE_BRANCH_ID = checkExecuteRoom.BRANCH_ID;
								}
								else
								{
									item.TDL_EXECUTE_BRANCH_ID = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault().BRANCH_ID : 0;
									item.TDL_EXECUTE_BRANCH_ID = item.TDL_EXECUTE_BRANCH_ID == 0 ? HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId() : item.TDL_EXECUTE_BRANCH_ID;
								}
								if (IsVisibleColumn)
								{
									data_ServicePrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, item.TDL_EXECUTE_BRANCH_ID, null, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, item.ID, item.PATIENT_TYPE_ID, null, null, null, null, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);
								}
								else
								{
									item.RationTimeIds = new List<long>();
									List<V_HIS_SERVICE_PATY> servicePatiesNew = new List<V_HIS_SERVICE_PATY>();
									var dicServicePaty = servicePaties.GroupBy(o=>o.PATIENT_TYPE_ID);
                                    foreach (var ser in dicServicePaty)
                                    {
										if (ser.Count() == 1 || ser.Where(o=>o.RATION_TIME_ID == null).Count() == ser.Count())
											servicePatiesNew.AddRange(ser);
										else
											servicePatiesNew.AddRange(ser.Where(o=>o.RATION_TIME_ID != null));
                                    }
									System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<SSServiceADO>();
									foreach (var s in pi)
									{
										if (s.Name.Contains("cot") && (bool)s.GetValue(item) && ProcessRationTime(item.ID, s.Name.Substring(3).ToString()))
										{											
											data_ServicePrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePatiesNew, item.TDL_EXECUTE_BRANCH_ID, null, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, item.ID, item.PATIENT_TYPE_ID, null, null, null, null, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, this.__curentRationTimes[Inventec.Common.TypeConvert.Parse.ToInt32(s.Name.Substring(3).ToString())].ID);
											if (data_ServicePrice != null)
											{
												item.CaptionRationTime += item.ID + "____" + Inventec.Common.TypeConvert.Parse.ToInt32(s.Name.Substring(3).ToString()) + "____" + string.Format("{0:#,##0.}", data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO)) + ";";
												totalPrice += item.AMOUNT * (data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO));
												item.RationTimeIds.Add(this.__curentRationTimes[Inventec.Common.TypeConvert.Parse.ToInt32(s.Name.Substring(3).ToString())].ID);
											}
											else
											{
												DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.KhongCoChinhSachGiaBuaAn, item.SERVICE_NAME, this.__curentRationTimes[Inventec.Common.TypeConvert.Parse.ToInt32(s.Name.Substring(3).ToString())].RATION_TIME_NAME));
												s.SetValue(item, false);
											}
										}
									}
								}								
							}

							if (data_ServicePrice != null && IsVisibleColumn)
							{
								totalPrice += item.AMOUNT * (data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO));
							}
                        }
					}
				}
				this.lblTotalServicePrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice, ConfigApplications.NumberSeperator);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadDefaultUser()
		{
			try
			{
				string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
				var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).ToList();
				if (data != null)
				{
					this.cboUser.EditValue = data[0].LOGINNAME;
					this.txtLoginName.Text = data[0].LOGINNAME;
				}

				//Cấu hình để ẩn/hiện trường người chỉ định tai form chỉ định, kê đơn
				//- Giá trị mặc định (hoặc ko có cấu hình này) sẽ ẩn
				//- Nếu có cấu hình, đặt là 1 thì sẽ hiển thị
				this.cboUser.Enabled = (HisConfigCFG.ShowRequestUser == commonString__true);
				this.txtLoginName.Enabled = (HisConfigCFG.ShowRequestUser == commonString__true);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadServicePaty()
		{
			try
			{
				var patientTypeIdAls = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID);
				var servicePatyTemps = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
					.Where(t => patientTypeIdAls.Contains(t.PATIENT_TYPE_ID))
					.ToList();

				//emptySpaceItem3.Size = new System.Drawing.Size((int)((lciCashierRoom.Size.Width + layoutControlItem3.Size.Width) / 2), emptySpaceItem3.Size.Height);
				if (servicePatyTemps != null && servicePatyTemps.Count > 0 && servicePatyTemps.FirstOrDefault(o=>o.RATION_TIME_ID != null) != null)
                {
					gridColumn5.VisibleIndex = -1;
					grcActualPrice.VisibleIndex = -1;
					IsVisibleColumn = false;
				}					
				this.servicePatyInBranchs = servicePatyTemps
					.GroupBy(o => o.SERVICE_ID)
					.ToDictionary(o => o.Key, o => o.ToList());


				//Lọc các đối tượng thanh toán không có chính sách giá
				var patientHasSetyIds = servicePatyTemps.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();
				this.currentPatientTypeWithPatientTypeAlter = this.currentPatientTypeWithPatientTypeAlter.Where(o => patientHasSetyIds.Contains(o.ID)).ToList();

				this.dicServices = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>()
					.Where(t => t.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
					.ToDictionary(o => o.ID);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadDataToGrid(bool isAutoSetPaty)
		{
			try
			{
				List<SSServiceADO> listSSServiceADO = new List<SSServiceADO>();
				this.gridControlService.DataSource = null;
				var allDatas = this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count > 0 ? this.ServiceIsleafADOs.AsQueryable() : null;
				if (this.toggleSwitchDataChecked.EditValue != null && (this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() == "true" && allDatas != null && allDatas.Count() > 0 && isAutoSetPaty)
				{
					this.notSearch = 2;
					txtServiceCode_Search.Text = "";
					txtServiceName_Search.Text = "";
					Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 1");
					listSSServiceADO = allDatas.Where(o => o.IsChecked).ToList();
				}
				else
				{
					List<ServiceADO> parentNodes = new List<ServiceADO>();

					//Lay tat ca cac node duoc check tren tree
					this.treeService.CloseEditor();
					this.treeService.EndCurrentEdit();

					var nodeCheckeds = this.treeService.GetAllCheckedNodes();

					Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 2  Count node check " + nodeCheckeds.Count());

					if (nodeCheckeds != null && nodeCheckeds.Count > 0 && !HisConfigCFG.IsSearchAll)
					{
						listSSServiceADO = new List<SSServiceADO>();
						//lay data cua cac dong tuong ung voi cac node duoc check
						foreach (var node in nodeCheckeds)
						{
							var data = this.treeService.GetDataRecordByNode(node) as ServiceADO;
							if (data != null)
							{
								parentNodes.Add(data);
							}
						}
						Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 3");
						if (parentNodes.Count > 0)
						{
							//var checkPtttSelected = parentNodes.Any(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);
							//if (checkPtttSelected)
							//{
							//    this.ChangeStateGroupInGrid(groupType__PtttGroupName);
							//}
							//else
							//{
							//    this.ChangeStateGroupInGrid(groupType__ServiceTypeName);
							//}
							var parentIdAllows = parentNodes.Select(o => o.ID).ToArray();

							Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 3.1");
							//Lay tat ca cac dich vụ khong co cha cua tat ca cac loai dich vụ duoc check tren tree
							var parentRootSetys = parentNodes.Where(o => String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
							if (parentRootSetys != null && parentRootSetys.Count > 0)
							{
								foreach (var item in parentRootSetys)
								{
									if (item != null)
									{
										var childOfParentNodeNoParents = allDatas.Where(o =>
										(o.PARENT_ID == null || item.ID == o.PARENT_ID)
										&& o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID
										&& parentIdAllows.Contains(o.PARENT_ID ?? 0)
										).ToList();
										if (childOfParentNodeNoParents != null && childOfParentNodeNoParents.Count > 0)
										{
											listSSServiceADO.AddRange(childOfParentNodeNoParents);
										}
									}
								}
								Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 3.2");
							}

							//Lay ra tat ca cac dich vụ con cua dich vu cha va cac dich vu con cua con cua no neu co -> duyet de quy cho den het cay dich vu,..
							var parentRoots = parentNodes.Where(o => !String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
							if (parentRoots != null && parentRoots.Count > 0)
							{
								Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 3.3");
								foreach (var item in parentRoots)
								{
									var childs = GetChilds(item);
									if (childs != null && childs.Count > 0)
									{
										listSSServiceADO.AddRange(childs);
									}
								}
							}
							listSSServiceADO = listSSServiceADO.Distinct().ToList();
						}
					}
					else
					{
						Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 4");
						//this.ChangeStateGroupInGrid(groupType__ServiceTypeName);
						listSSServiceADO = allDatas != null && allDatas.Count() > 0 ? allDatas.ToList() : new List<SSServiceADO>();
					}

					//listSSServiceADO = new List<SSServiceADO>();
					foreach (var item in listSSServiceADO)
					{
						if (!isAutoSetPaty)
						{
							ResetOneService(item);
						}

						//listSSServiceADO.Add(item);
					}
				}
				Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid. 5");
				if (!String.IsNullOrWhiteSpace(txtServiceName_Search.Text) && listSSServiceADO != null && listSSServiceADO.Count() > 0)
				{
					listSSServiceADO = listSSServiceADO.Where(o => o.SERVICE_NAME_HIDDEN.ToLower().Contains(txtServiceName_Search.Text.ToLower().Trim())).ToList();
				}
				if (!String.IsNullOrWhiteSpace(txtServiceCode_Search.Text) && listSSServiceADO != null && listSSServiceADO.Count() > 0)
				{
					listSSServiceADO = listSSServiceADO.Where(o => o.SERVICE_CODE_HIDDEN.ToLower().Contains(txtServiceCode_Search.Text.ToLower().Trim())).ToList();
				}
				this.gridViewService.GridControl.DataSource = listSSServiceADO != null && listSSServiceADO.Count > 0 ? listSSServiceADO.Distinct().OrderByDescending(o => o.NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList() : null;
				var index = 0;
				if (this.__curentRationTimes != null && this.__curentRationTimes.Count > 0)
				{
					this.__curentRationTimes = this.__curentRationTimes.OrderBy(p => p.RATION_TIME_CODE).ToList();
					index = this.__curentRationTimes.Count;
					if (index > 0)
					{
						CreateUnboundColumn(index);
					}
				}

				this.SetEnableButtonControl(this.actionType);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private List<SSServiceADO> GetChilds(ServiceADO parentNode)
		{
			List<SSServiceADO> result = new List<SSServiceADO>();
			try
			{
				if (parentNode != null)
				{
					var childs = ServiceIsleafADOs.Where(o => o.PARENT_ID == parentNode.ID && o.SERVICE_TYPE_ID == parentNode.SERVICE_TYPE_ID).ToList();
					if (childs != null && childs.Count > 0)
					{
						result.AddRange(childs);
					}

					var childOfParents = ServiceParentADOs.Where(o => o.PARENT_ID == parentNode.ID && o.SERVICE_TYPE_ID == parentNode.SERVICE_TYPE_ID).ToList();
					if (childOfParents != null && childOfParents.Count > 0)
					{
						foreach (var item in childOfParents)
						{
							var childOfChilds = GetChilds(item);
							if (childOfChilds != null && childOfChilds.Count > 0)
							{
								result.AddRange(childOfChilds);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return result;
		}

		/// <summary>
		/// Bổ sung: trong trường hợp đối tượng BN là BHYT và chưa đến ngày hiệu lực 
		/// hoặc đã hết hạn sử dụng (thời gian y lệnh ko nằm trong khoảng [từ ngày - đến ngày] của thẻ BHYT), 
		/// thì hiển thị đối tượng thanh toán mặc định là đối tượng viện phí
		/// Ngược lại xử lý như hiện tại: ưu tiên lấy theo đối tượng Bn trước, không có sẽ lấy mặc định theo đối tượng chấp nhận TT đầu tiên tìm thấy
		/// </summary>
		/// <param name="patientTypeId"></param>
		/// <param name="serviceId"></param>
		/// <returns></returns>
		private MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, SSServiceADO sereServADO)
		{
			MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
			try
			{
				var patientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
				if (patientTypes != null && patientTypes.Count > 0 && currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
				{
					var servicePaties = this.servicePatyInBranchs[serviceId];
					//Chỉ hiển thị ra các chính sách giá không khai báo “Đối tượng chi tiết" hoặc có thông tin “Đối tượng chi tiết” trùng với thông tin đối tượng chi tiết của bệnh nhân
					servicePaties = servicePaties.Where(o => !o.PATIENT_CLASSIFY_ID.HasValue || o.PATIENT_CLASSIFY_ID.Value == currentHisTreatment.TDL_PATIENT_CLASSIFY_ID).ToList();

					var patientTypeIdInSePas = servicePaties.Select(o => o.PATIENT_TYPE_ID).ToList();

					var currentPatientTypeTemps = this.currentPatientTypeWithPatientTypeAlter.Where(o => patientTypeIdInSePas != null && patientTypeIdInSePas.Contains(o.ID)).ToList();
					if (currentPatientTypeTemps != null && currentPatientTypeTemps.Count > 0)
					{
						//if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
						//&& (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0).Value.Date > Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTimeSelecteds.OrderByDescending(o => o).First()).Value.Date
						//|| Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0).Value.Date < Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTimeSelecteds.OrderByDescending(o => o).First()).Value.Date
						//))
						//{
						//    result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == HisConfigCFG.PatientTypeId__VP);
						//}
						//else
						//{
						//    result = (currentPatientTypeTemps != null ? (currentPatientTypeTemps.FirstOrDefault(o => o.ID == patientTypeId) ?? currentPatientTypeTemps[0]) : null);
						//}

						result = currentPatientTypeTemps.FirstOrDefault(o => o.ID != HisConfigCFG.PatientTypeId__BHYT);
						if (result != null && sereServADO != null)
						{
							sereServADO.PATIENT_TYPE_ID = result.ID;
							sereServADO.PATIENT_TYPE_CODE = result.PATIENT_TYPE_CODE;
							sereServADO.PATIENT_TYPE_NAME = result.PATIENT_TYPE_NAME;
						}
					}
				}
				return (result ?? new HIS_PATIENT_TYPE());
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return result;
		}

		private void ChooseExecuteRoomDefaultlService(long serviceId, SSServiceADO sereServADO)
		{
			try
			{
				if (sereServADO.ROOM_ID > 0)
					return;
				var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
				//MOS.Filter.HisRefectoryFilter filter = new HisRefectoryFilter();
				//var datas = new BackendAdapter(new CommonParam()).Get<List<HIS_REFECTORY>>("/api/HisRefectory/Get", ApiConsumers.MosConsumer, filter, null);

				if (serviceRoomViews != null && serviceRoomViews.Count > 0)
				{
					var arrExcuteRooms = serviceRoomViews.Where(o => o.SERVICE_ID == serviceId && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__NA).ToList();
					// List<MOS.EFMODEL.DataModels.HIS_REFECTORY> dataCombo = ((arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0 && datas != null) ? datas.Where(o => arrExcuteRoomCode.Contains(o.ROOM_ID)).ToList() : null);
					if (arrExcuteRooms != null && arrExcuteRooms.Count > 0)
					{
						var room = arrExcuteRooms.FirstOrDefault(p => p.ROOM_ID == this.requestRoom.ID);
						if (room != null)
						{
							sereServADO.ROOM_ID = room.ROOM_ID;
						}
						else
						{
							var room1 = arrExcuteRooms.Where(p => p.DEPARTMENT_ID == this.requestRoom.DEPARTMENT_ID).ToList();
							if (room1 != null && room1.Count > 0)
							{
								room1 = room1.OrderByDescending(p => p.ROOM_ID).ToList();
								sereServADO.ROOM_ID = room1.FirstOrDefault().ROOM_ID;
							}
							else
							{
								var room2 = arrExcuteRooms.Where(p => p.BRANCH_ID == this.requestRoom.BRANCH_ID).ToList();
								if (room2 != null && room2.Count > 0)
								{
									room2 = room2.OrderByDescending(p => p.ROOM_ID).ToList();
									sereServADO.ROOM_ID = room2.FirstOrDefault().ROOM_ID;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private long GetRoomId()
		{
			long roomId = 0;
			try
			{
				roomId = (this.currentModule != null ? this.currentModule.RoomId : 0);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return roomId;
		}

		List<HIS_ICD> getIcdCodeListFromUcIcd()
		{
			List<HIS_ICD> icdList = new List<HIS_ICD>();
			try
			{
				if (ucIcd != null)
				{
					var icdValue = icdProcessor.GetValue(ucIcd);
					if (icdValue != null && icdValue is HIS.UC.Icd.ADO.IcdInputADO)
					{
						if (icdValue != null && !string.IsNullOrEmpty(((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE))
						{
							HIS_ICD icdMain = new HIS_ICD();
							//icdMain.ID = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_ID ?? 0;
							icdMain.ICD_NAME = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
							icdMain.ICD_CODE = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
							var icd = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ICD_CODE == icdMain.ICD_CODE);
							icdMain.ID = icd != null ? icd.ID : 0;
							icdList.Add(icdMain);
						}
					}
				}

				if (ucSecondaryIcd != null)
				{
					var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
					if (subIcd != null && subIcd is HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)
					{
						string icd_sub_code = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
						if (!string.IsNullOrEmpty(icd_sub_code))
						{
							String[] icdCodes = icd_sub_code.Split(';');
							foreach (var item in icdCodes)
							{
								var icd = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ICD_CODE == item);
								if (icd != null)
								{
									icdList.Add(icd);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				icdList = new List<HIS_ICD>();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return icdList;
		}
		private bool CheckIcdService(ref string messageErr, List<SereServADO> ServiceCheckeds_Send, ref List<SereServADO> ServiceNotConfigResult, List<HIS_ICD> icdCodeList)
		{
			bool valid = true;
			try
			{
				ServiceNotConfigResult = new List<SereServADO>();
				// kiểm tra dịch vụ theo cấu hình ICD - Dịch vụ


				if (icdCodeList == null || icdCodeList.Count == 0)
				{
					valid = true;
					return valid;
				}

				List<long> serviceIdChecks = ServiceCheckeds_Send.Select(o => o.SERVICE_ID).Distinct().ToList();

				if (serviceIdChecks == null || serviceIdChecks.Count == 0)
				{
					valid = false;
					return valid;
				}

				MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
				icdServiceFilter.SERVICE_IDs = serviceIdChecks;
				icdServiceFilter.ICD_CODE__EXACTs = icdCodeList.Select(o => o.ICD_CODE).Distinct().ToList();
				Inventec.Common.Logging.LogSystem.Debug("begin call HisIcdService/Get");
				var IcdServices = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
				Inventec.Common.Logging.LogSystem.Debug("end call HisIcdService/Get");
				if (ServiceCheckeds_Send != null && ServiceCheckeds_Send.Count > 0)
				{
					foreach (var item in ServiceCheckeds_Send)
					{
						if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G || item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC)
						{
							continue;
						}

						var checkIcdService = IcdServices.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID);
						if (checkIcdService == null)
						{
							valid = false;
							ServiceNotConfigResult.Add(item);
							messageErr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
							Inventec.Common.Logging.LogSystem.Debug("Dich vu (" + item.TDL_SERVICE_CODE + "-" + item.TDL_SERVICE_NAME + " chua duoc cau hinh ICD - Dich vu.");
						}
					}
				}

			}
			catch (Exception ex)
			{
				valid = true;
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return valid;
		}

		private bool CheckValidDataInGridService(CommonParam param, List<SereServADO> serviceCheckeds__Send)
		{
			bool valid = true;
			try
			{

				if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
				{
					foreach (var item in serviceCheckeds__Send)
					{
						string messageErr = "";
						messageErr = String.Format(ResourceMessage.CanhBaoDichVu, item.TDL_SERVICE_NAME);

						if (item.PATIENT_TYPE_ID <= 0)
						{
							valid = false;
							messageErr += ResourceMessage.KhongCoDoiTuongThanhToan;
							Inventec.Common.Logging.LogSystem.Debug("Dich vu (" + item.TDL_SERVICE_CODE + "-" + item.TDL_SERVICE_NAME + " khong co doi tuong thanh toan.");
						}
						if (item.AMOUNT <= 0)
						{
							valid = false;
							messageErr += ResourceMessage.KhongNhapSoLuong;
							Inventec.Common.Logging.LogSystem.Debug("Dich vu (" + item.TDL_SERVICE_CODE + "-" + item.TDL_SERVICE_NAME + " khong co so luong.");
						}

						if (!valid)
						{
							param.Messages.Add(messageErr + ";");
						}
					}
				}
				else
				{
					HIS.Desktop.LibraryMessage.MessageUtil.SetParam(param, HIS.Desktop.LibraryMessage.Message.Enum.ThongBaoDuLieuTrong);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return valid;
		}

		private bool Valid(List<SereServADO> serviceCheckeds__Send)
		{
			CommonParam param = new CommonParam();
			bool valid = true;
			try
			{
				this.positionHandleControl = -1;
				valid = valid && (bool)icdProcessor.ValidationIcd(ucIcd);
				valid = valid && (bool)subIcdProcessor.GetValidate(ucSecondaryIcd);
				valid = (this.dxValidationProviderControl.Validate()) && valid;
				valid = valid && this.CheckValidDataInGridService(param, serviceCheckeds__Send);

				if (!valid)
				{
					MessageManager.Show(param, null);
				}
			}
			catch (Exception ex)
			{
				valid = false;
				Inventec.Common.Logging.LogSystem.Error(ex);
			}

			return valid;
		}

		private async void FillDataToComboPriviousServiceReq(HisTreatmentWithPatientTypeInfoSDO currentHisTreatment)
		{
			try
			{
				WaitingManager.Show();
				CommonParam param = new CommonParam(0, 10);
				MOS.Filter.HisServiceReqView6Filter serviceReqFilter = new MOS.Filter.HisServiceReqView6Filter();
				serviceReqFilter.TDL_PATIENT_ID = this.currentHisTreatment.PATIENT_ID;
				serviceReqFilter.ORDER_DIRECTION = "DESC";
				serviceReqFilter.ORDER_FIELD = "CREATE_TIME";
				serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long>();
				//Nếu thêm một loại yêu cầu dv khác thì phải vào đây bổ sung
				serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN);
				Inventec.Common.Logging.LogSystem.Debug("begin call HisServiceReq/GetView6");
				this.currentPreServiceReqs = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6>>(RequestUriStore.HIS_SERVICE_REQ_GETVIEW_6, ApiConsumers.MosConsumer, serviceReqFilter, ProcessLostToken, param);
				Inventec.Common.Logging.LogSystem.Debug("end call HisServiceReq/GetView6");
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("SERVICE_REQ_TYPE_NAME", "", 150, 1));
				columnInfos.Add(new ColumnInfo("RENDERER_INTRUCTION_TIME", "", 150, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("RENDERER_INTRUCTION_TIME", "ID", columnInfos, false, 300);
				ControlEditorLoader.Load(this.cboPriviousServiceReq, this.currentPreServiceReqs, controlEditorADO);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ProcessChoiceServiceReqPrevious(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6 serviceReq)
		{
			try
			{
				if (serviceReq != null)
				{
					var allDatas = this.ServiceIsleafADOs.AsQueryable();
					List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServs = SereServGet.GetByServiceReqId(serviceReq.ID);
					if (sereServs != null && sereServs.Count > 0)
					{
						this.gridViewService.BeginUpdate();
						if (sereServs != null && sereServs.Count > 0)
						{
							var serviceIds = sereServs.Select(o => o.SERVICE_ID).Distinct().ToArray();
							allDatas = allDatas.Where(o => serviceIds.Contains(o.ID));
						}
						var resultData = allDatas.ToList();

						if (resultData != null && resultData.Count > 0)
						{
							foreach (var sereServADO in resultData)
							{
								sereServADO.IsChecked = true;
								this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.ID, sereServADO);
								this.ChooseExecuteRoomDefaultlService(sereServADO.ID, sereServADO);
								this.ChooseCotDefaultlService(sereServADO.ID, sereServADO);
								this.ValidServiceDetailProcessing(sereServADO);
							}
							this.toggleSwitchDataChecked.EditValue = true;
						}
						this.gridViewService.GridControl.DataSource = resultData.OrderByDescending(o => o.NUM_ORDER).ToList();
						this.gridViewService.EndUpdate();

						this.SetEnableButtonControl(this.actionType);
						this.SetDefaultSerServTotalPrice();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void FillDataOtherPaySourceDataRow(SSServiceADO currentRowSereServADO)
		{
			try
			{
				if (currentRowSereServADO.IsChecked && currentRowSereServADO.PATIENT_TYPE_ID > 0)
				{
					var dataOtherPaySources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
					List<HIS_OTHER_PAY_SOURCE> dataOtherPaySourceTmps = new List<HIS_OTHER_PAY_SOURCE>();
					dataOtherPaySources = (dataOtherPaySources != null && dataOtherPaySources.Count > 0) ? dataOtherPaySources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
					if (dataOtherPaySources != null && dataOtherPaySources.Count > 0)
					{
						var workingPatientType = currentPatientTypes.Where(t => t.ID == currentRowSereServADO.PATIENT_TYPE_ID).FirstOrDefault();

						if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS))
						{
							dataOtherPaySourceTmps = dataOtherPaySources.Where(o => ("," + workingPatientType.OTHER_PAY_SOURCE_IDS + ",").Contains("," + o.ID + ",")).ToList();

							if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null && dataOtherPaySourceTmps != null && dataOtherPaySourceTmps.Count == 1)
							{
								currentRowSereServADO.OTHER_PAY_SOURCE_ID = dataOtherPaySourceTmps[0].ID;
								currentRowSereServADO.OTHER_PAY_SOURCE_CODE = dataOtherPaySourceTmps[0].OTHER_PAY_SOURCE_CODE;
								currentRowSereServADO.OTHER_PAY_SOURCE_NAME = dataOtherPaySourceTmps[0].OTHER_PAY_SOURCE_NAME;
							}
						}
						else
						{
							dataOtherPaySourceTmps.AddRange(dataOtherPaySources);
						}

						if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null
							&& currentHisTreatment != null && currentHisTreatment.OTHER_PAY_SOURCE_ID.HasValue && currentHisTreatment.OTHER_PAY_SOURCE_ID.Value > 0
							&& dataOtherPaySourceTmps != null && dataOtherPaySourceTmps.Exists(k => k.ID == currentHisTreatment.OTHER_PAY_SOURCE_ID.Value))
						{
							var otherPaysourceByTreatment = dataOtherPaySourceTmps.Where(k => k.ID == currentHisTreatment.OTHER_PAY_SOURCE_ID.Value).FirstOrDefault();
							if (otherPaysourceByTreatment != null)
							{
								currentRowSereServADO.OTHER_PAY_SOURCE_ID = otherPaysourceByTreatment.ID;
								currentRowSereServADO.OTHER_PAY_SOURCE_CODE = otherPaysourceByTreatment.OTHER_PAY_SOURCE_CODE;
								currentRowSereServADO.OTHER_PAY_SOURCE_NAME = otherPaysourceByTreatment.OTHER_PAY_SOURCE_NAME;
							}
						}
						else if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null)
						{

							HIS.UC.Icd.ADO.IcdInputADO icdData = this.icdProcessor.GetValue(this.ucIcd) as HIS.UC.Icd.ADO.IcdInputADO;
							var serviceTemp = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == currentRowSereServADO.ID).FirstOrDefault();
							if (serviceTemp != null && serviceTemp.OTHER_PAY_SOURCE_ID.HasValue && dataOtherPaySourceTmps.Exists(k =>
								k.ID == serviceTemp.OTHER_PAY_SOURCE_ID.Value)
								&& (String.IsNullOrEmpty(serviceTemp.OTHER_PAY_SOURCE_ICDS) || (icdData != null && !String.IsNullOrEmpty(serviceTemp.OTHER_PAY_SOURCE_ICDS) && !String.IsNullOrEmpty(icdData.ICD_CODE) && ("," + serviceTemp.OTHER_PAY_SOURCE_ICDS.ToLower() + ",").Contains("," + icdData.ICD_CODE.ToLower() + ","))))
							{
								var otherPaysourceByService = dataOtherPaySourceTmps.Where(k => k.ID == serviceTemp.OTHER_PAY_SOURCE_ID.Value).FirstOrDefault();
								if (otherPaysourceByService != null)
								{
									currentRowSereServADO.OTHER_PAY_SOURCE_ID = otherPaysourceByService.ID;
									currentRowSereServADO.OTHER_PAY_SOURCE_CODE = otherPaysourceByService.OTHER_PAY_SOURCE_CODE;
									currentRowSereServADO.OTHER_PAY_SOURCE_NAME = otherPaysourceByService.OTHER_PAY_SOURCE_NAME;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		/// <summary>
		/// Gan gia trị mac dinh cho cac control can khoi tao san gia tri
		/// </summary>
		private void SetDefaultData()
		{
			try
			{
				gridColumnPRIMARY_PATIENT_TYPE_ID.VisibleIndex = -1;
				gridColumnPRIMARY_PATIENT_TYPE_ID.Visible = false;
				gridColumnOTHER_PAY_SOURCE_NAME.VisibleIndex = -1;
				gridColumnOTHER_PAY_SOURCE_NAME.Visible = false;

				this.btnShowDetail.Enabled = false;
				this.btnPrint.Enabled = false;
				this.btnSave.Enabled = false;
				this.btnSaveAndPrint.Enabled = false;
				this.btnShowDetail.Enabled = false;
				this.btnCreateBill.Enabled = false;
				this.selectedSeviceGroups = null;
				this.cboServiceGroup.EditValue = null;
				this.cboServiceGroup.Properties.Buttons[1].Visible = false;
				this.txtDescription.Text = "";
				this.lblTotalServicePrice.Text = "0";
				this.actionType = GlobalVariables.ActionAdd;

				this.currentPatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

				UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
				ucDateProcessor.Reload(ucDate, dateInputADO);
				this.intructionTimeSelecteds = ucDateProcessor.GetValue(ucDate);
				this.isMultiDateState = false;

				GridCheckMarksSelection gridCheckMark = cboServiceGroup.Properties.Tag as GridCheckMarksSelection;
				if (gridCheckMark != null)
					gridCheckMark.ClearSelection(cboServiceGroup.Properties.View);
				//GridCheckMarksSelection gridCheckMark2 = repositoryItemGridLookUp__BuaAn.Tag as GridCheckMarksSelection;
				//if (gridCheckMark2 != null)
				//    gridCheckMark2.ClearSelection(this.repositoryItemGridLookUp__BuaAn.View);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void CreateThreadLoadDataByPackageService(object param)
		{
			Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataByPackageServiceNewThread));
			try
			{
				thread.Start(param);
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
				thread.Abort();
			}
		}

		private void LoadDataByPackageServiceNewThread(object data)
		{
			try
			{
				if (this.InvokeRequired)
				{
					this.Invoke(new MethodInvoker(delegate { this.LoadDataByPackageService((V_HIS_SERE_SERV)data); }));
				}
				else
				{
					this.LoadDataByPackageService((V_HIS_SERE_SERV)data);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private async Task LoadDataByPackageService(V_HIS_SERE_SERV sereServ)
		{
			try
			{
				if (sereServ != null)
				{
					CommonParam param = new CommonParam();
					//Lấy list service package
					HisServicePackageViewFilter filter = new HisServicePackageViewFilter();
					filter.SERVICE_ID = sereServ.SERVICE_ID;
					var servicePackageByServices = await new BackendAdapter(param).GetAsync<List<V_HIS_SERVICE_PACKAGE>>(HisRequestUriStore.HIS_SERVICE_PACKAGE_GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
					if (servicePackageByServices != null && servicePackageByServices.Count > 0)
					{
						List<long> serviceIds = servicePackageByServices.Select(o => o.SERVICE_ATTACH_ID).Distinct().ToList();

						MOS.Filter.HisServiceViewFilter filterMedicine = new HisServiceViewFilter();
						filterMedicine.IDs = serviceIds;
						Inventec.Common.Logging.LogSystem.Debug("begin call HisService/GetView");
						List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> serviceInPackages = await new BackendAdapter(param).GetAsync<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumer.ApiConsumers.MosConsumer, filterMedicine, param);
						Inventec.Common.Logging.LogSystem.Debug("end call HisService/GetView");
						//Load data for service package
						this.LoadPageServiceInServicePackage(serviceInPackages);

						//Tính lại tổng số tiền đã thanh toán là hao phí trong gói
						await this.SetTotalPriceInPackage(sereServ);
					}
					else
					{
						this.toggleSwitchDataChecked.EditValue = false;
						// this.LoadDataToGrid(true);
						this.LoadDataToGrid(false);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private async Task SetTotalPriceInPackage(V_HIS_SERE_SERV sereServ)
		{
			try
			{
				CommonParam param = new CommonParam();
				//Lấy list service package
				HisSereServFilter sereServFilter = new HisSereServFilter();
				sereServFilter.IS_EXPEND = true;
				sereServFilter.PARENT_ID = sereServ.ID;
				Inventec.Common.Logging.LogSystem.Debug("begin call HisSereServ/Get");
				var serviceInPackage__Expends = await new BackendAdapter(param).GetAsync<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, sereServFilter, ProcessLostToken, param);
				Inventec.Common.Logging.LogSystem.Debug("end call HisSereServ/Get");
				if (serviceInPackage__Expends != null && serviceInPackage__Expends.Count > 0)
				{
					this.currentExpendInServicePackage = serviceInPackage__Expends.Sum(o => (o.AMOUNT * o.PRICE));
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private async void LoadPageServiceInServicePackage(List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> serviceInPackages)
		{
			try
			{
				//this.gridViewServiceProcess.BeginUpdate();
				//var allDatas = this.ServiceIsleafADOs.AsQueryable();
				////treeService.UncheckAll();
				//if (serviceInPackages != null && serviceInPackages.Count > 0)
				//{
				//    var serviceIds = serviceInPackages.Select(o => o.ID).Distinct().ToArray();
				//    allDatas = allDatas.Where(o => serviceIds.Contains(o.ID));
				//}
				//var resultData = allDatas.ToList();
				//if (resultData != null && resultData.Count > 0)
				//{
				//    foreach (var sereServADO in resultData)
				//    {
				//        sereServADO.IsChecked = true;
				//        this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);
				//        this.ValidServiceDetailProcessing(sereServADO);
				//    }
				//    this.toggleSwitchDataChecked.EditValue = true;

				//}
				//this.gridViewServiceProcess.GridControl.DataSource = resultData.OrderByDescending(o => o.SERVICE_NUM_ORDER).ToList();
				//this.gridViewServiceProcess.EndUpdate();

				//this.SetEnableButtonControl(this.actionType);
				//this.SetDefaultSerServTotalPrice();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadDataToCurrentTreatmentData(long treatmentId, long intructionTime)
		{
			try
			{
				CommonParam param = new CommonParam();
				MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
				filter.TREATMENT_ID = treatmentId;
				filter.INTRUCTION_TIME = intructionTime;
				Inventec.Common.Logging.LogSystem.Debug("begin call HisTreatment/GetTreatmentWithPatientTypeInfoSdo");
				var hisTreatments = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>(RequestUriStore.HIS_TREATMENT_GET_TREATMENT_WITH_PATIENT_TYPE_INFO_SDO, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
				Inventec.Common.Logging.LogSystem.Debug("end call HisTreatment/GetTreatmentWithPatientTypeInfoSdo");
				this.currentHisTreatment = hisTreatments != null && hisTreatments.Count > 0 ? hisTreatments.FirstOrDefault() : null;

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ProcessDataWithTreatmentWithPatientTypeInfo()
		{
			try
			{
				var patientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>();
				var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
				if (patientTypeAllows != null && patientTypes != null)
				{
					if (this.currentHisTreatment != null && !String.IsNullOrEmpty(this.currentHisTreatment.PATIENT_TYPE_CODE))
					{
						var patientType = patientTypes.FirstOrDefault(o => o.PATIENT_TYPE_CODE == this.currentHisTreatment.PATIENT_TYPE_CODE);
						if (patientType == null) throw new AggregateException("Khong lay duoc thong tin PatientType theo ma doi tuong (PATIENT_TYPE trong HisTreatmentWithPatientTypeInfoSDO).");

						this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
						this.currentHisPatientTypeAlter.PATIENT_TYPE_ID = patientType.ID;
						this.currentHisPatientTypeAlter.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
						this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
						this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE = this.currentHisTreatment.TREATMENT_TYPE_CODE;
						this.currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE = this.currentHisTreatment.HEIN_MEDI_ORG_CODE;
						this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME = this.currentHisTreatment.HEIN_CARD_FROM_TIME;
						this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME = this.currentHisTreatment.HEIN_CARD_TO_TIME;
						this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER = this.currentHisTreatment.HEIN_CARD_NUMBER;
						this.currentHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = this.currentHisTreatment.RIGHT_ROUTE_TYPE_CODE;
						this.currentHisPatientTypeAlter.LEVEL_CODE = this.currentHisTreatment.LEVEL_CODE;
						this.currentHisPatientTypeAlter.RIGHT_ROUTE_CODE = this.currentHisTreatment.RIGHT_ROUTE_CODE;
						var tt = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == this.currentHisTreatment.TREATMENT_TYPE_CODE);
						this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID = (tt != null ? tt.ID : 0);
						this.currentHisPatientTypeAlter.TREATMENT_TYPE_NAME = (tt != null ? tt.TREATMENT_TYPE_NAME : "");

						var patientTypeAllow = patientTypeAllows.Where(o => o.PATIENT_TYPE_ID == patientType.ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
						this.currentPatientTypeWithPatientTypeAlter = ((patientTypeAllow != null && patientTypeAllow.Count > 0) ? patientTypes.Where(o => patientTypeAllow.Contains(o.ID)).ToList() : new List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>());
					}
					else
						throw new AggregateException("currentHisTreatment.PATIENT_TYPE_CODE is null");
				}
				else
					throw new AggregateException("patientTypeAllows is null");
			}
			catch (AggregateException ex)
			{
				this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
				this.currentPatientTypeWithPatientTypeAlter = new List<HIS_PATIENT_TYPE>();
				WaitingManager.Hide();
				MessageManager.Show(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh);
				Inventec.Common.Logging.LogSystem.Info("LoadDataToCurrentTreatmentData => khong lay duoc doi tuong benh nhan. Dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentId), treatmentId) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => intructionTimeSelecteds), intructionTimeSelecteds) + "____Dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisTreatment), currentHisTreatment));
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void PatientTypeWithPatientTypeAlter()
		{
			try
			{
				var patientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>();
				var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
				if (patientTypeAllows != null && patientTypeAllows.Count > 0 && patientTypes != null)
				{
					if (this.currentHisPatientTypeAlter != null)
					{
						var patientTypeAllow = patientTypeAllows.Where(o => o.PATIENT_TYPE_ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
						if (patientTypeAllow != null && patientTypeAllow.Count > 0)
						{
							this.currentPatientTypeWithPatientTypeAlter = patientTypes.Where(o => patientTypeAllow.Contains(o.ID)).ToList();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void FillDataToControlsForm()
		{
			try
			{
				this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
				this.InitComboRepositoryServiceRoom(BackendDataWorker.Get<V_HIS_SERVICE_ROOM>());
				this.InitComboServiceGroup();
				this.InitComboUser();
				this.InitDefaultDataByPatientType();
				this.InitComboTracking(this.treatmentId);
				this.ValidateForm();
				// this.InitComboRationTime();
				// this.InitComboRationTime(this.repositoryItemGridLookUp__BuaAn, null);
				//this.grcRation_TabService.ColumnEdit = this.repositoryItemGridLookUp__BuaAn;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		/// <summary>
		/// Lay Chan doan mac dinh: Lay chan doan cuoi cung trong cac xu ly dich vu Kham benh
		/// </summary>
		private void LoadIcdDefault()
		{
			try
			{
				if (this.currentHisTreatment != null)
				{
					//Nếu hồ sơ chưa có thông tin ICD, và là hồ sơ đến khám theo loại là hẹn khám thì khi chỉ định dịch vụ, tự động hiển thị ICD của đợt điều trị trước, tương ứng với mã hẹn khám
					if (string.IsNullOrEmpty(this.currentHisTreatment.ICD_CODE)
						&& !String.IsNullOrEmpty(this.currentHisTreatment.PREVIOUS_ICD_CODE))
					{
						HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
						icd.ICD_CODE = currentHisTreatment.PREVIOUS_ICD_CODE;
						icd.ICD_NAME = this.currentHisTreatment.PREVIOUS_ICD_NAME;
						if (ucIcd != null)
						{
							icdProcessor.Reload(ucIcd, icd);
						}

						HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcd = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
						subIcd.ICD_SUB_CODE = this.currentHisTreatment.PREVIOUS_ICD_SUB_CODE;
						subIcd.ICD_TEXT = this.currentHisTreatment.PREVIOUS_ICD_TEXT;
						if (ucSecondaryIcd != null)
						{
							subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
						}
					}
					else
					{
						HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
						icd.ICD_CODE = currentHisTreatment.ICD_CODE;
						icd.ICD_NAME = this.currentHisTreatment.ICD_NAME;
						if (ucIcd != null)
						{
							icdProcessor.Reload(ucIcd, icd);
						}

						HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcd = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
						subIcd.ICD_SUB_CODE = this.currentHisTreatment.ICD_SUB_CODE;
						subIcd.ICD_TEXT = this.currentHisTreatment.ICD_TEXT;
						if (ucSecondaryIcd != null)
						{
							subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadTreatmentInfo__PatientType()
		{
			try
			{
				//decimal totalPrice = 0;
				//if (this.dSereServ1WithTreatment != null && this.dSereServ1WithTreatment.Count > 0)
				//{
				//    totalPrice = this.dSereServ1WithTreatment.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);              
				//}
				this.lblPatientName.Text = this.currentHisTreatment.TDL_PATIENT_NAME;
				if (this.currentHisTreatment.TDL_PATIENT_DOB > 0)
					lblDob.Text = currentHisTreatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1 ? this.currentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(0,4) :  Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentHisTreatment.TDL_PATIENT_DOB);
				this.lblGenderName.Text = this.currentHisTreatment.TDL_PATIENT_GENDER_NAME;

				if (this.currentHisPatientTypeAlter != null)
				{
					this.lblPatientTypeName.Text = this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME;
					this.lblTreatmentTypeName.Text = this.currentHisPatientTypeAlter.TREATMENT_TYPE_NAME;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void CreateThreadLoadDataSereServWithTreatment(object param)
		{
			Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataSereServWithTreatmentNewThread));
			  
			try
			{
				thread.Start(param);
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
				thread.Abort();
			}
		}

		private void LoadDataSereServWithTreatmentNewThread(object param)
		{
			try
			{
				if (this.InvokeRequired)
				{
					this.Invoke(new MethodInvoker(delegate { this.LoadDataSereServWithTreatment((HisTreatmentWithPatientTypeInfoSDO)param, null); }));
				}
				else
				{
					this.LoadDataSereServWithTreatment((HisTreatmentWithPatientTypeInfoSDO)param, null);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private async Task LoadDataSereServWithTreatment(HisTreatmentWithPatientTypeInfoSDO treatment, DateTime? intructionTime)
		{
			try
			{
				if (treatment != null)
				{
					LogSystem.Debug("LoadDataSereServWithTreatment => start");
					CommonParam param = new CommonParam();
					HisSereServView1Filter hisSereServFilter = new HisSereServView1Filter();
					hisSereServFilter.TREATMENT_ID = treatment.ID;
					if (intructionTime != null && intructionTime.Value != DateTime.MinValue)
					{
						hisSereServFilter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(intructionTime.Value.ToString("yyyyMMdd") + "000000");
						hisSereServFilter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(intructionTime.Value.ToString("yyyyMMdd") + "235959");
					}
					else
					{
						hisSereServFilter.INTRUCTION_TIME_FROM = Inventec.Common.DateTime.Get.StartDay();
						hisSereServFilter.INTRUCTION_TIME_TO = Inventec.Common.DateTime.Get.EndDay();
					}
					List<long> setyAllowsIds = new List<long>();
					setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU);
					setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC);
					setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
					hisSereServFilter.NOT_IN_SERVICE_TYPE_IDs = setyAllowsIds;
					Inventec.Common.Logging.LogSystem.Debug("begin call HisSereServ/GetView1");
					this.sereServWithTreatment = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_1, ApiConsumers.MosConsumer, hisSereServFilter, ProcessLostToken, param);
					Inventec.Common.Logging.LogSystem.Debug("end call HisSereServ/GetView1");
					LogSystem.Debug("LoadDataSereServWithTreatment => end");
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitDefaultDataByPatientType()
		{
			try
			{
				if (this.currentSereServ == null)
				{
					//this.gridViewServiceProcess.Columns["IsOutKtcFee"].Visible = false;
				}

				this.SetPatientInfoToControl(); //thong tin ve BN                
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetPatientInfoToControl()
		{
			try
			{
				if (this.currentHisTreatment != null)
				{
					//this.lblTreatmentCode_TabBlood.Text = currentHisTreatment.TREATMENT_CODE;
					//this.lblPatientName_TabBlood.Text = currentHisTreatment.VIR_PATIENT_NAME;
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

		private void FillDataIntoPrimaryPatientTypeCombo(SSServiceADO data, GridLookUpEdit patientTypeCombo)
		{
			try
			{
				Inventec.Common.Logging.LogSystem.Debug("FillDataIntoPrimaryPatientTypeCombo.1");
				var patientTypeIdAls = currentPatientTypeWithPatientTypeAlter.Where(o => o.ID != HisConfigCFG.PatientTypeId__BHYT).Select(o => o.ID).ToList();
				List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = new List<HIS_PATIENT_TYPE>();
				if (BranchDataWorker.HasServicePatyWithListPatientType(data.ID, patientTypeIdAls))
				{
					Inventec.Common.Logging.LogSystem.Debug("FillDataIntoPrimaryPatientTypeCombo.2");
					long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
					//List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == data.SERVICE_TYPE_ID).ToList() : null;
					//long? intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;
					List<V_HIS_SERVICE_PATY> servicePaties = BranchDataWorker.ServicePatyWithListPatientType(data.ID, patientTypeIdAls);

					//Chỉ hiển thị ra các chính sách giá không khai báo “Đối tượng chi tiết" hoặc có thông tin “Đối tượng chi tiết” trùng với thông tin đối tượng chi tiết của bệnh nhân
					servicePaties = servicePaties.Where(o => !o.PATIENT_CLASSIFY_ID.HasValue || o.PATIENT_CLASSIFY_ID.Value == currentHisTreatment.TDL_PATIENT_CLASSIFY_ID).ToList();

					var currentPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, data.TDL_EXECUTE_BRANCH_ID, null, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, data.ID, data.PATIENT_TYPE_ID, null, null, null, null, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);

					var patientTypePrimatyList = this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ADDITION == (short)1).ToList();

					var patyIds = servicePaties.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();
					patyIds = patientTypePrimatyList != null && patientTypePrimatyList.Count > 0 ? patyIds.Where(o => patientTypePrimatyList.Select(p => p.ID).Contains(o)).ToList() : null;
					
					if (patyIds != null)
					{
						foreach (var item in patyIds)
						{
							if (item == data.PATIENT_TYPE_ID)
								continue;
							//var itemPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, data.TDL_EXECUTE_BRANCH_ID, null, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, data.ID, item, null, null);
							//if (itemPaty == null || currentPaty == null || (currentPaty.PRICE * (1 + currentPaty.VAT_RATIO)) >= (itemPaty.PRICE * (1 + itemPaty.VAT_RATIO)))
							//    continue;
							dataCombo.Add(this.currentPatientTypeWithPatientTypeAlter.FirstOrDefault(o => o.ID == item));
						}
					}
				}
				this.InitComboPatientType(patientTypeCombo, dataCombo);
				Inventec.Common.Logging.LogSystem.Debug("FillDataIntoPrimaryPatientTypeCombo.3");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void FillDataIntoExcuteRoomCombo(SSServiceADO data, DevExpress.XtraEditors.GridLookUpEdit excuteRoomCombo)
		{
			try
			{
				var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
				//MOS.Filter.HisRefectoryFilter filter = new HisRefectoryFilter();
				//var datas = new BackendAdapter(new CommonParam()).Get<List<HIS_REFECTORY>>("/api/HisRefectory/Get", ApiConsumers.MosConsumer, filter, null);

				if (excuteRoomCombo != null && serviceRoomViews != null && serviceRoomViews.Count > 0)
				{
					var arrExcuteRoomCode = serviceRoomViews.Where(o => data != null && o.SERVICE_ID == data.ID && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__NA).ToList();

					this.InitComboExecuteRoom(excuteRoomCombo, arrExcuteRoomCode);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void CreateUnboundColumn(int indexCreate)
		{
			try
			{
				var columnCot = gridViewService.Columns.FirstOrDefault(o => o.FieldName.Contains("cot"));
				var indexCot = gridViewService.Columns.IndexOf(columnCot);
				if (indexCot != -1)
				{
					for (int i = indexCot; i < gridViewService.Columns.Count; i++)
					{
						gridViewService.Columns.RemoveAt(i);
						i--;
					}
				}

				GridColumn col = new GridColumn();
				for (int i = 0; i < indexCreate; i++)
				{
					col = gridViewService.Columns.AddVisible("cot" + i.ToString(), "Bữa " + this.__curentRationTimes[i].RATION_TIME_NAME);
					col.OptionsColumn.AllowEdit = true;
					col.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
					col.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
					col.OptionsFilter.AllowAutoFilter = false;
					col.OptionsFilter.AllowFilter = false;
					//col.OptionsColumn.
					DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
					// repositoryItemCheckEdit.CheckedChanged += new System.EventHandler(this.Check_CheckedChanged);
					//col.ColumnEdit = repositoryItemCheckEdit;
					col.UnboundType = DevExpress.Data.UnboundColumnType.Object;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void Check_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				var data = (SSServiceADO)gridViewService.GetFocusedRow();
				if (data != null)
				{
					CheckEdit chk = sender as CheckEdit;
					if (chk.Checked)
					{

					}

					this.gridControlService.RefreshDataSource();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void CheckProcess(SSServiceADO data)
		{
			try
			{
				data.RationTimeIds = new List<long>();
				if (data.cot0)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[0].ID);
				}
				if (data.cot1)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[1].ID);
				}
				if (data.cot2)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[2].ID);
				}
				if (data.cot3)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[3].ID);
				}
				if (data.cot4)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[4].ID);
				}
				if (data.cot5)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[5].ID);
				}
				if (data.cot6)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[6].ID);
				}
				if (data.cot7)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[7].ID);
				}
				if (data.cot8)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[8].ID);
				}
				if (data.cot9)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[9].ID);
				}
				if (data.cot10)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[10].ID);
				}
				if (data.cot11)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[11].ID);
				}
				if (data.cot12)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[12].ID);
				}
				if (data.cot13)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[13].ID);
				}
				if (data.cot14)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[14].ID);
				}
				if (data.cot15)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[15].ID);
				}
				if (data.cot16)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[16].ID);
				}
				if (data.cot17)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[17].ID);
				}
				if (data.cot18)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[18].ID);
				}
				if (data.cot19)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[19].ID);
				}
				if (data.cot20)
				{
					data.RationTimeIds.Add(this.__curentRationTimes[20].ID);
				}

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void LoadTrackingDefault()
		{
			try
			{
				List<V_HIS_TRACKING> lstTracking = new List<V_HIS_TRACKING>();
				if (this.trackings != null && this.trackings.Count > 0)
				{
					lstTracking = this.trackings.Where(o => o.TRACKING_TIME.ToString().StartsWith(DateTime.Now.ToString("yyyyMMdd"))).ToList();
				}

				if (lstTracking != null && lstTracking.Count > 0)
				{

					lstTracking = lstTracking.OrderByDescending(o => o.TRACKING_TIME).ToList();

					if (HisConfigCFG.IsDefaultTracking == 1)
					{
						if (HisConfigCFG.IsMineCheckedByDefault != 1)
						{
							cboTracking.EditValue = lstTracking.FirstOrDefault().ID;
						}
						else
						{
							string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
							cboTracking.EditValue = lstTracking.Where(o => o.CREATOR == loginName).OrderByDescending(p => p.TRACKING_TIME).FirstOrDefault().ID;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ChooseCotDefaultlService(long serviceId, SSServiceADO sereServADO)
		{
			try
			{
				//Trong trường hợp chỉ có 1 bữa thì khi check chọn "dịch vụ ăn" thì tự động check chọn "bữa ăn"
				var data = this.__curentServiceRatis.Where(p => p.SERVICE_ID == serviceId).ToList();
				//lọc lại theo dữ liệu bữa ăn không khóa
				if (data != null && data.Count > 1)
				{
					data = data.Where(o => this.__curentRationTimes.Exists(e => e.ID == o.RATION_TIME_ID)).ToList();
				}

				if (data != null && data.Count == 1)
				{
					var oneRationTime = this.__curentRationTimes.FirstOrDefault(o => o.ID == data.First().RATION_TIME_ID);
					if (oneRationTime != null)
					{
						int index = this.__curentRationTimes.IndexOf(oneRationTime);

						System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<SSServiceADO>();
						foreach (var item in pi)
						{
							if (item.Name == "cot" + index)
							{
								item.SetValue(sereServADO, true);
								break;
							}
						}
					}

					StringBuilder sb = new StringBuilder();
					sb.Append(oneRationTime.RATION_TIME_NAME);
					SelectOneRationGroupProcess(new List<HIS_RATION_TIME> { oneRationTime }, sereServADO, sb);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
