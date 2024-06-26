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
using HIS.Desktop.Plugins.AssignNutritionEdit.ADO;
using HIS.Desktop.Plugins.AssignNutritionEdit.Config;
using HIS.Desktop.Plugins.AssignNutritionEdit.Resources;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utilities.Extentions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
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

namespace HIS.Desktop.Plugins.AssignNutritionEdit.Run
{
	public partial class frmAssignNutritionEdit : HIS.Desktop.Utility.FormBase
	{
		
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

	
		private void LoadServicePaty()
		{
			try
			{
				var patientTypeIdAls = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID);
				var servicePatyTemps = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
					.Where(t => patientTypeIdAls.Contains(t.PATIENT_TYPE_ID))
					.ToList();
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
						var dtCheck = currentPatientTypeTemps.FirstOrDefault(o => o.ID != HisConfigCFG.PatientTypeId__BHYT && o.ID == currentHisTreatment.TDL_PATIENT_TYPE_ID);
						if (dtCheck != null)
						{
							result = dtCheck;
						}
						else
						{
							result = currentPatientTypeTemps.FirstOrDefault(o => o.ID != HisConfigCFG.PatientTypeId__BHYT);
						}
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
				valid = (this.dxValidationProvider1.Validate()) && valid;
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

			

		/// <summary>
		/// Gan gia trị mac dinh cho cac control can khoi tao san gia tri
		/// </summary>
		
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
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}


		public int positionHandleControl { get; private set; }
	

	}
}
