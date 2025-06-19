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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignNoneMediService.Config;
using HIS.Desktop.Plugins.AssignNoneMediService.Resources;
using HIS.UC.DateEditor.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignNoneMediService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        private List<HIS_DEPARTMENT_TRAN> ListDepartmentTranCheckTime = null;
        private List<HIS_CO_TREATMENT> ListCoTreatmentCheckTime = null;

        private void UcDateInit()
        {
            try
            {
                this.ValidationSingleControl(this.dtInstructionTime, this.dxValidationProviderControl);
                //this.lcichkMultiDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void UcDateSetValue(DateInputADO input)
        {
            try
            {
                if (input != null)
                {
                    if (input.Time != null && input.Time != DateTime.MinValue)
                    {
                        this.timeIntruction.EditValue = input.Time.ToString("HH:mm");
                    }
                    if (input.Dates != null && input.Dates.Count > 0)
                    {
                        this.dtInstructionTime.EditValue = input.Dates[0];
                        this.intructionTimeSelected = new List<DateTime?>();
                        this.intructionTimeSelected.AddRange(input.Dates);
                    }
                    this.intructionTimeSelecteds = this.intructionTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList();
                    this.InstructionTime = intructionTimeSelecteds.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeIntructionTimeEditor(DateTime intructTime)
        {
            try
            {
                this.isUseTrackingInputWhileChangeTrackingTime = true;
                System.DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1);
                this.timeSelested = today.Add(this.timeIntruction.TimeSpan);

                if (this.chkMultiIntructionTime.Checked)
                {
                    if (this.intructionTimeSelected != null && this.intructionTimeSelected.Count > 0 && intructTime != DateTime.MinValue)
                    {
                        for (int i = 0, j = this.intructionTimeSelected.Count; i < j; i++)
                        {
                            this.intructionTimeSelected[i] = new DateTime(this.intructionTimeSelected[i].Value.Year, this.intructionTimeSelected[i].Value.Month, this.intructionTimeSelected[i].Value.Day, intructTime.Hour, intructTime.Minute, 0);
                        }
                        this.intructionTimeSelecteds = (this.intructionTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList());
                        this.InstructionTime = this.intructionTimeSelecteds.OrderByDescending(o => o).FirstOrDefault();
                        this.ChangeIntructionTime(intructTime);
                    }
                }
                else
                {
                    this.intructionTimeSelected = new List<DateTime?>();
                    this.InstructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(intructTime.ToString("yyyyMMdd") + this.timeSelested.ToString("HHmm") + "00");
                    this.intructionTimeSelected.Add(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.InstructionTime));
                    this.intructionTimeSelecteds = new List<long>();
                    this.intructionTimeSelecteds.Add(this.InstructionTime);
                    this.ChangeIntructionTime(intructTime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void ProcessGetDataDepartment()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessGetDataDepartment.Begin");
                CommonParam paramGet = new CommonParam();
                if (this.ListDepartmentTranCheckTime == null)
                {
                    HisDepartmentTranFilter filter = new HisDepartmentTranFilter();
                    filter.TREATMENT_ID = this.treatmentId;
                    this.ListDepartmentTranCheckTime = new BackendAdapter(paramGet).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                }

                if (this.ListCoTreatmentCheckTime == null)
                {
                    HisCoTreatmentFilter filter = new HisCoTreatmentFilter();
                    filter.TDL_TREATMENT_ID = this.treatmentId;
                    this.ListCoTreatmentCheckTime = new BackendAdapter(paramGet).Get<List<HIS_CO_TREATMENT>>("api/HisCoTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                }

                Inventec.Common.Logging.LogSystem.Debug("ProcessGetDataDepartment.End");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectMultiIntructionTime(List<DateTime?> datas, DateTime time)
        {
            try
            {
                if (datas != null && time != DateTime.MinValue)
                {
                    this.intructionTimeSelected = datas as List<DateTime?>;
                    string strTimeDisplay = "";
                    int num = 0;
                    this.intructionTimeSelected = this.intructionTimeSelected.OrderBy(o => o.Value).ToList();
                    foreach (var item in this.intructionTimeSelected)
                    {
                        if (item != null && item.Value != DateTime.MinValue)
                        {
                            strTimeDisplay += (num == 0 ? "" : "; ") + item.Value.ToString("dd/MM");
                            num++;
                        }
                    }
                    if (this.txtInstructionTime.Text != strTimeDisplay)
                    {
                        //Trường hợp chọn nhiều ngày chỉ định thì lấy đối tượng bệnh nhân tuong uong voi intructiontime dau tien duoc chon
                        //Vì các dữ liệu liên quan như chính sách giá, đối tượng chấp nhận thanh toán phải suy ra từ đối tượng BN ở trên
                        this.isInitUcDate = true;
                        this.timeSelested = time;
                        this.timeIntruction.EditValue = this.timeSelested.ToString("HH:mm");
                        this.txtInstructionTime.Text = strTimeDisplay;
                        this.isInitUcDate = false;
                    }
                }

                DelegateSelectMultiDate(datas, time);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void UcDateReload(HIS.UC.DateEditor.ADO.DateInputADO input)
        {
            try
            {
                DateTime now = DateTime.Now;
                if (ContructorIntructionTime > 0)
                {
                    now = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ContructorIntructionTime) ?? DateTime.Now;
                }
                DateTime nowTmp = DateTime.Now;
                if (input != null && input.Time != DateTime.MinValue && input.Dates != null && input.Dates.Count > 0)
                {
                    this.timeIntruction.EditValue = input.Time.ToString("HH:mm");
                    nowTmp = input.Time;
                    this.intructionTimeSelected = new List<DateTime?>();
                    this.intructionTimeSelected.AddRange(input.Dates);
                }
                else
                {
                    this.txtInstructionTime.Visible = false;
                    this.dtInstructionTime.Visible = true;
                    this.timeIntruction.EditValue = now.ToString("HH:mm");
                    nowTmp = now;
                    this.intructionTimeSelected = new List<DateTime?>();
                    this.intructionTimeSelected.Add(now);
                }

                System.DateTime today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 1);
                this.timeSelested = today.Add(timeIntruction.TimeSpan);
                this.dtInstructionTime.EditValue = nowTmp;
                this.intructionTimeSelecteds = this.intructionTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList();
                this.InstructionTime = intructionTimeSelecteds.First();

                this.chkMultiIntructionTime.Checked = false;

                if (input != null && input.IsVisibleMultiDate.HasValue)
                {
                    this.lcichkMultiDate.Visibility = (input.IsVisibleMultiDate.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                this.isMultiDateState = chkMultiIntructionTime.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusUCDate()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegateMultiDateChanged()
        {
            try
            {
                this.isMultiDateState = chkMultiIntructionTime.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
