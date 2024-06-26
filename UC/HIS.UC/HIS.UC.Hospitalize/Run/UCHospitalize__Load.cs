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

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CAREER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("CAREER_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CAREER_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboCareer, careers, controlEditorADO);
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
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("BED_ROOM_CODE", "", 50, 1));
				columnInfos.Add(new ColumnInfo("BED_ROOM_NAME", "", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("BED_ROOM_NAME", "ID", columnInfos, false, 250);
				ControlEditorLoader.Load(cboBedRoom, this.listBedRoom, controlEditorADO);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
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
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}

		}
	}
}
