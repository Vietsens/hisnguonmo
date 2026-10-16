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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.Hospitalize.ADO;

namespace HIS.UC.Hospitalize.Run
{
	public partial class UCHospitalize : UserControl
	{
		List<V_HIS_DEPARTMENT_1> _HisDepartments { get; set; }
		private void LoadDataToDepartmentComboExecute()
		{
			try
			{
				_HisDepartments = new List<V_HIS_DEPARTMENT_1>();
				//Lấy các khoa là khoa cận lâm sàng IS_CLINICAL
				CommonParam param = new CommonParam();
				HisDepartmentView1Filter filter = new HisDepartmentView1Filter();
				filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
				filter.IS_CLINICAL = true;

				_HisDepartments = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_1>>("api/HisDepartment/GetView1", ApiConsumers.MosConsumer, filter, param);


			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetDataComboDepartment(long _treatmentTypeId)
		{
			try
			{
				this.listDepartment = new List<V_HIS_DEPARTMENT_1>();
				List<V_HIS_DEPARTMENT_1> listDepartmentAll = new List<V_HIS_DEPARTMENT_1>();
                string icdCode = (txtIcdCode.Text ?? "").Trim();
				if (_HisDepartments != null && _HisDepartments.Count > 0)
				{
                    listDepartmentAll = _HisDepartments.Where(p => CheckIds(p, _treatmentTypeId)).ToList() ?? new List<V_HIS_DEPARTMENT_1>();
                    //listDepartmentAll = listDepartmentAll.Where(p => CheckAcceptedIcdCodes(p, icdCode)).ToList();

                    if (!string.IsNullOrEmpty(icdCode))
                    {
                        var listDepartmentICD = listDepartmentAll.Where(o => (","+o.ACCEPTED_ICD_CODES+",").Contains(","+icdCode+",")).ToList();

                        if (listDepartmentICD != null && listDepartmentICD.Count > 0)
                        {
                            listDepartmentAll = listDepartmentICD;
                        }
                    }
				}

				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listDepartmentAll), listDepartmentAll));
				if (chkCCS.Checked)
				{
					if (listDepartmentAll != null && listDepartmentAll.Count > 0)
					{
						listDepartment = listDepartmentAll.Where(o => o.BRANCH_ID == WorkPlace.GetBranchId()).OrderBy(p => p.DEPARTMENT_NAME).ToList();
					}
				}
				else
				{
					if (listDepartmentAll != null && listDepartmentAll.Count > 0)
					{
						var listDepartment1 = listDepartmentAll.Where(o => o.BRANCH_ID == WorkPlace.GetBranchId()).OrderBy(p => p.DEPARTMENT_NAME).ToList();
						var listDepartment2 = listDepartmentAll.Where(o => o.BRANCH_ID != WorkPlace.GetBranchId()).OrderBy(p => p.DEPARTMENT_NAME).ToList();

						this.listDepartment.AddRange(listDepartment1);
						this.listDepartment.AddRange(listDepartment2);
					}
				}
				List<DepartmentADO> lstAdo = new List<DepartmentADO>();
				foreach (var department in listDepartment)
				{
					DepartmentADO ado = new DepartmentADO(department);
					lstAdo.Add(ado);

				}

				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 50, 1));
				columnInfos.Add(new ColumnInfo("NAME_STR", "", 300, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("NAME_STR", "ID", columnInfos, false, 350);
				ControlEditorLoader.Load(cboDepartment, lstAdo, controlEditorADO);

				cboDepartment.EditValue = null;
				txtDepartmentCode.EditValue = null;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private bool CheckIds(V_HIS_DEPARTMENT_1 ado, long id)
		{
			bool result = false;
			try
			{
				if (!string.IsNullOrEmpty(ado.ALLOW_TREATMENT_TYPE_IDS))
				{
					string[] _str = ado.ALLOW_TREATMENT_TYPE_IDS.Split(',');
					foreach (var item in _str)
					{
						if (id == Inventec.Common.TypeConvert.Parse.ToInt64(item))
						{
							result = true;
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				result = false;
				Inventec.Common.Logging.LogSystem.Error(ex);
			}

			return result;
		}

        private bool CheckAcceptedIcdCodes(V_HIS_DEPARTMENT_1 department, string icdCode)
        {
            bool result = false;
            try
            {
                if (string.IsNullOrEmpty(department.ACCEPTED_ICD_CODES))
                {
                    result = true;
                }
                else if (!string.IsNullOrEmpty(icdCode))
                {
                    string[] _str = department.ACCEPTED_ICD_CODES.Split(',');
                    foreach (var item in _str)
                    {
                        if (icdCode.Trim() == item.Trim())
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadDataToComboCareer()
        {
            try
            {
                var careers = BackendDataWorker.Get<HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                int popupWidth = 350;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CAREER_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("CAREER_NAME", "Tên nghề nghiệp", 250, 2));
                AddCareerLevelColumns(columnInfos, ref popupWidth);
                ControlEditorADO controlEditorADO = new ControlEditorADO("CAREER_NAME", "ID", columnInfos, true, popupWidth);
                ControlEditorLoader.Load(cboCareer, careers, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Bo sung cot nhom cap 2/3/4 cho combo nghe nghiep theo key cau hinh
        /// MOS.HIS_CAREER.IS_SHOW_LEVEL_2/3/4 (rong/khac 1 = an, mac dinh an nhu cu)
        /// </summary>
        private static void AddCareerLevelColumns(List<ColumnInfo> columnInfos, ref int popupWidth)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_CAREER.IS_SHOW_LEVEL_2") == "1")
                {
                    columnInfos.Add(new ColumnInfo("LEVEL2_NAME", "Nhóm cấp 2", 180, 3));
                    popupWidth += 180;
                }
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_CAREER.IS_SHOW_LEVEL_3") == "1")
                {
                    columnInfos.Add(new ColumnInfo("LEVEL3_NAME", "Nhóm cấp 3", 180, 4));
                    popupWidth += 180;
                }
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_CAREER.IS_SHOW_LEVEL_4") == "1")
                {
                    columnInfos.Add(new ColumnInfo("LEVEL4_NAME", "Nhóm cấp 4", 180, 5));
                    popupWidth += 180;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // PTTK_19083: Bind danh mục phân loại cấp cứu (IS_EMERGENCY=1) + visibility theo phòng cấp cứu
        private List<HIS_PATIENT_CLASSIFY> emergencyClassifyData;
        private void LoadDataToComboEmergencyClassify()
        {
            try
            {
                emergencyClassifyData = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                && o.IS_DELETE != 1
                                && o.IS_EMERGENCY == 1)
                    .OrderBy(o => o.PATIENT_CLASSIFY_NAME)
                    .ToList();

                if (emergencyClassifyData == null || emergencyClassifyData.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    HisPatientClassifyFilter filter = new HisPatientClassifyFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.IS_EMERGENCY = 1;
                    emergencyClassifyData = new BackendAdapter(param)
                        .Get<List<HIS_PATIENT_CLASSIFY>>("api/HisPatientClassify/Get",
                            ApiConsumers.MosConsumer, filter, param);
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_CLASSIFY_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_CLASSIFY_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_CLASSIFY_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboEmergencyClassifyId2, emergencyClassifyData, controlEditorADO);

                // Load value khi edit hồ sơ cũ
                cboEmergencyClassifyId2.EditValue = hospitalizeInitADO != null ? hospitalizeInitADO.EmergencyClassifyId2 : null;
                Inventec.Common.Logging.LogSystem.Debug("LoadComboEmergencyClassifyId2 - IsEmergencyRoom: " + hospitalizeInitADO.IsEmergencyRoom);
                // Ẩn combo khi phòng không phải cấp cứu
                bool isEmergencyRoom = hospitalizeInitADO != null && hospitalizeInitADO.IsEmergencyRoom;
                layoutControlItem34.Visibility = isEmergencyRoom
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

		private void LoadDataToComboTreatmentType()
		{
			try
			{
				this.listTreatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM).ToList();

				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_CODE", "", 150, 1));
				columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_NAME", "", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_TYPE_NAME", "ID", columnInfos, false, 250);
				ControlEditorLoader.Load(cboTreatmentType, this.listTreatmentType, controlEditorADO);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetShowControl(bool set)
		{
			try
			{
				//Set ẩn hiện combo buồng
				if (set)
				{
					layoutControlItemBedRoom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
					layoutControlItemBedRoomCbo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
				}
				else
				{
					layoutControlItemBedRoom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
					layoutControlItemBedRoomCbo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}

		}

		private void LoadDataToBedRoomCombo(DevExpress.XtraEditors.GridLookUpEdit cboBedRoom)
		{
			try
			{
				if (!hospitalizeInitADO.DepartmentId.HasValue)
					throw new Exception("DepartmentId is null");
				// Lấy giường dựa trên khoa đã được chọn
				txtBedRoomCode.Text = null;
				CommonParam param = new CommonParam();
				HisBedRoomFilter filter = new HisBedRoomFilter();
				filter.IS_ACTIVE = 1;
				this.listBedRoom = new BackendAdapter(param).Get<List<V_HIS_BED_ROOM>>(HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, filter, param);
				if (cboTreatmentType.EditValue != null)
				{
					List<V_HIS_BED_ROOM> lstBedRoomTemp = new List<V_HIS_BED_ROOM>();
					this.listBedRoom = this.listBedRoom.Where(o => o.DEPARTMENT_ID == hospitalizeInitADO.DepartmentId).ToList();
					foreach (var item in listBedRoom)
					{
						if (string.IsNullOrEmpty(item.TREATMENT_TYPE_IDS))
						{
							lstBedRoomTemp.Add(item);
						}
						else
						{
							var lstIds = item.TREATMENT_TYPE_IDS.Split(',').ToList();
							if(lstIds.FirstOrDefault(o=>o == cboTreatmentType.EditValue.ToString()) != null)
							{
								lstBedRoomTemp.Add(item);
							}	
						}
					}
					listBedRoom = lstBedRoomTemp;
				}
				else
				{

					this.listBedRoom = this.listBedRoom.Where(o => o.DEPARTMENT_ID == hospitalizeInitADO.DepartmentId && (string.IsNullOrEmpty(o.TREATMENT_TYPE_IDS))).ToList();
				}
				BindBedRoomCombo(cboBedRoom, this.listBedRoom);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		/// <summary>
		/// Đổ danh sách buồng vào ô chọn Buồng bệnh: bật dòng tiêu đề cột (Mã buồng, Tên buồng)
		/// và thêm cột Tổng BN/Tổng giường ghép từ PATIENT_COUNT/BED_COUNT của V_HIS_BED_ROOM,
		/// cùng dữ liệu api/HisBedRoom/GetView đang gọi, không gọi thêm đường gọi nào.
		/// Chữ cả dòng: đỏ khi PATIENT_COUNT > BED_COUNT, xanh dương khi bằng, xanh lá khi ít hơn
		/// (giống ô Buồng bệnh màn Tiếp nhận chuyển khoa).
		/// Nguồn của ô là BedRoomADO (kế thừa V_HIS_BED_ROOM) nên ValueMember ID không đổi,
		/// các xử lý tra theo listBedRoom (gõ mã buồng, Closed, KeyUp) giữ nguyên.
		/// </summary>
		internal static void BindBedRoomCombo(DevExpress.XtraEditors.GridLookUpEdit cboBedRoom, List<V_HIS_BED_ROOM> bedRooms)
		{
			List<BedRoomADO> dataSource = new List<BedRoomADO>();
			if (bedRooms != null)
			{
				foreach (V_HIS_BED_ROOM bedRoom in bedRooms)
				{
					dataSource.Add(new BedRoomADO(bedRoom));
				}
			}

			List<ColumnInfo> columnInfos = new List<ColumnInfo>();
			columnInfos.Add(new ColumnInfo("BED_ROOM_CODE", GetBedRoomColumnCaption("UCHospitalize.cboBedRoom.ColBedRoomCode.Caption", "Mã buồng"), 70, 1));
			columnInfos.Add(new ColumnInfo("BED_ROOM_NAME", GetBedRoomColumnCaption("UCHospitalize.cboBedRoom.ColBedRoomName.Caption", "Tên buồng"), 180, 2));
			ColumnInfo patientBedColumn = new ColumnInfo("TT_PATIENT_BED_STR", GetBedRoomColumnCaption("UCHospitalize.cboBedRoom.ColPatientBed.Caption", "Tổng BN/Tổng giường"), 130, 3);
			patientBedColumn.horzAlignment = ColumnInfo.HorzAlignment.Center;
			columnInfos.Add(patientBedColumn);

			ControlEditorADO controlEditorADO = new ControlEditorADO("BED_ROOM_NAME", "ID", columnInfos, true, 400);
			ControlEditorLoader.Load(cboBedRoom, dataSource, controlEditorADO);

			DevExpress.XtraGrid.Views.Grid.GridView bedRoomView = cboBedRoom.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
			if (bedRoomView != null)
			{
				// Hàm nạp chạy lại mỗi lần chọn khoa: gỡ trước khi gắn để không đăng ký trùng.
				bedRoomView.RowStyle -= BedRoomView_RowStyle;
				bedRoomView.RowStyle += BedRoomView_RowStyle;
				bedRoomView.FocusedRowChanged -= BedRoomView_FocusedRowChanged;
				bedRoomView.FocusedRowChanged += BedRoomView_FocusedRowChanged;
				ApplyFocusedBedRoomForeColor(bedRoomView);
			}
		}

		/// <summary>
		/// Tô màu chữ cả dòng buồng: đỏ khi số BN đang nằm vượt số giường, xanh dương khi bằng, xanh lá khi còn giường.
		/// </summary>
		private static void BedRoomView_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
		{
			try
			{
				DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
				if (view == null || e.RowHandle < 0)
					return;

				BedRoomADO bedRoom = view.GetRow(e.RowHandle) as BedRoomADO;
				if (bedRoom == null)
					return;

				e.Appearance.ForeColor = GetBedRoomForeColor(bedRoom);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private static Color GetBedRoomForeColor(BedRoomADO bedRoom)
		{
			if (bedRoom.PatientBedCompare > 0)
				return Color.Red;
			if (bedRoom.PatientBedCompare == 0)
				return Color.Blue;
			return Color.Green;
		}

		/// <summary>
		/// Dòng đang chọn dùng kiểu hiển thị riêng của DevExpress nên màu RowStyle không áp vào.
		/// Đặt màu chữ của kiểu dòng đang chọn theo đúng dòng đó, nền chọn của skin giữ nguyên.
		/// </summary>
		private static void BedRoomView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
		{
			try
			{
				ApplyFocusedBedRoomForeColor(sender as DevExpress.XtraGrid.Views.Grid.GridView);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private static void ApplyFocusedBedRoomForeColor(DevExpress.XtraGrid.Views.Grid.GridView view)
		{
			if (view == null)
				return;

			BedRoomADO bedRoom = view.GetFocusedRow() as BedRoomADO;
			Color foreColor = bedRoom != null ? GetBedRoomForeColor(bedRoom) : Color.Empty;
			view.Appearance.FocusedRow.ForeColor = foreColor;
			view.Appearance.SelectedRow.ForeColor = foreColor;
			view.Appearance.HideSelectionRow.ForeColor = foreColor;
		}

		private static string GetBedRoomColumnCaption(string languageKey, string defaultCaption)
		{
			try
			{
				if (HIS.UC.Hospitalize.Base.ResourceLangManager.LanguageUCHospitalize == null)
				{
					HIS.UC.Hospitalize.Base.ResourceLangManager.InitResourceLanguageManager();
				}
				string caption = Inventec.Common.Resource.Get.Value(languageKey, HIS.UC.Hospitalize.Base.ResourceLangManager.LanguageUCHospitalize, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				return string.IsNullOrEmpty(caption) ? defaultCaption : caption;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
				return defaultCaption;
			}
		}

		private void LoadBedCount(V_HIS_DEPARTMENT_1 data)
		{
			try
			{
				if (data != null)
				{
					lblGiuongKeHoach.Text = ((long)(data.THEORY_PATIENT_COUNT == null ? 0 : data.THEORY_PATIENT_COUNT)).ToString();
					lblGiuongThucKe.Text = ((long)(data.PATIENT_COUNT == null ? 0 : data.PATIENT_COUNT)).ToString();
					lblGiuongThucKe.ForeColor = System.Drawing.Color.Black;
					UpdateBedStatistics(data.ID);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}

		}

		private void UpdateBedStatistics(long selectedDepartmentId)
		{
			try
			{
				CommonParam param = new CommonParam();
				HisBedRoomViewFilter filter = new HisBedRoomViewFilter();
				filter.DEPARTMENT_ID = selectedDepartmentId;
				filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

				var bedRooms = new BackendAdapter(param).Get<List<V_HIS_BED_ROOM>>(
					HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_BED_ROOM_GETVIEW,
					ApiConsumers.MosConsumer, filter, param);

				if (bedRooms != null && bedRooms.Count > 0)
				{
					long totalBeds = 0;
					long totalPatients = 0;
					long occupiedBeds = 0;
					long sharedBeds = 0;

					foreach (var room in bedRooms)
					{
						long beds = (long)(room.BED_COUNT ?? 0);
						long patients = (long)(room.PATIENT_COUNT ?? 0);
						totalBeds += beds;
						totalPatients += patients;
						occupiedBeds += Math.Min(patients, beds);
						sharedBeds += Math.Max(0, patients - beds);
					}

					long availableBeds = totalBeds - occupiedBeds;

					string bedCountText = lblGiuongThucKe.Text;
					string stats = string.Format("{0} \u2014 Đã có BN: {1}, Còn: {2} giường",
						bedCountText, totalPatients, availableBeds);

					if (sharedBeds > 0)
					{
						stats += string.Format(". Giường nằm ghép: {0}", sharedBeds);
					}

					lblGiuongThucKe.Text = stats;

					lblGiuongThucKe.ForeColor = (availableBeds <= 0)
						? System.Drawing.Color.Red
						: System.Drawing.Color.Black;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
