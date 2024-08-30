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
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.Plugins.AssignService.Resources;
using HIS.UC.DateEditor.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignService.AssignService
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

        public void UcDateSetValueHasCheck(DateInputHasCheckADO input)
        {
            try
            {
                if (input != null)
                {
                    DateInputADO dateInputADO = new DateInputADO();
                    dateInputADO.Dates = input.Dates;
                    dateInputADO.IsVisibleMultiDate = input.IsVisibleMultiDate;
                    dateInputADO.Time = input.Time;
                    if (input.Time != null && input.Time != DateTime.MinValue)
                    {
                        this.timeIntruction.EditValue = input.Time.ToString("HH:mm");
                    }
                    this.timeSelested = input.Time;

                    this.isStopEventChangeMultiDate = true;
                    this.chkMultiIntructionTime.Checked = input.IsMultiDayChecked;
                    this.txtInstructionTime.Visible = input.IsMultiDayChecked;
                    this.dtInstructionTime.Visible = !input.IsMultiDayChecked;

                    if (input.IsMultiDayChecked)
                    {
                        this.SelectMultiIntructionTime(input.Dates, input.Time);
                    }
                    else
                    {
                        if (input.Dates != null && input.Dates.Count > 0)
                        {
                            this.dtInstructionTime.EditValue = input.Dates[0];
                            this.intructionTimeSelected = new List<DateTime?>();
                            this.intructionTimeSelected.AddRange(input.Dates);
                        }
                        this.intructionTimeSelecteds = (this.intructionTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList());
                        this.InstructionTime = this.intructionTimeSelecteds.First();
                    }
                    this.isStopEventChangeMultiDate = false;
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

                if (HisConfigCFG.IsCheckDepartmentInTimeWhenPresOrAssign && this.currentWorkingRoom != null && currentWorkingRoom.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                {
                    ProcessGetDataDepartment();
                    CheckTimeInDepartment(this.intructionTimeSelecteds);
                }

                CheckAssignServiceSimultaneityOption();
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.intructionTimeSelecteds), this.intructionTimeSelecteds));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckTimeInDepartment(List<long> listTime)
        {
            bool result = true;
            try
            {
                V_HIS_ROOM currentWorkingRoom = null;
                if (cboAssignRoom.EditValue != null)
                {
                    currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().First(o => o.ID == Convert.ToInt64(cboAssignRoom.EditValue));
                }
                else
                {
                    currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().First(o => o.ID == this.currentModule.RoomId);
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listTime), listTime));

                List<HIS_DEPARTMENT_TRAN> curremtTrans = null;
                if (this.ListDepartmentTranCheckTime != null && this.ListDepartmentTranCheckTime.Count > 0)
                {
                    curremtTrans = this.ListDepartmentTranCheckTime.Where(o => o.DEPARTMENT_ID == currentWorkingRoom.DEPARTMENT_ID && o.DEPARTMENT_IN_TIME.HasValue).ToList();
                }

                List<HIS_CO_TREATMENT> currentCo = null;
                if (this.ListCoTreatmentCheckTime != null && this.ListCoTreatmentCheckTime.Count > 0)
                {
                    currentCo = this.ListCoTreatmentCheckTime.Where(o => o.DEPARTMENT_ID == currentWorkingRoom.DEPARTMENT_ID && o.START_TIME.HasValue).ToList();
                }

                foreach (var intructionTime in listTime)
                {
                    bool hasTran = false;

                    List<string> times = new List<string>();
                    if (curremtTrans != null && curremtTrans.Count > 0)
                    {
                        curremtTrans = curremtTrans.OrderBy(o => o.DEPARTMENT_IN_TIME ?? 0).ToList();

                        long fromTime = 0;
                        long toTime = 0;

                        foreach (var item in curremtTrans)
                        {
                            fromTime = item.DEPARTMENT_IN_TIME ?? 0;
                            toTime = long.MaxValue;
                            HIS_DEPARTMENT_TRAN nextTran = this.ListDepartmentTranCheckTime.FirstOrDefault(o => o.PREVIOUS_ID == item.ID);
                            if (nextTran != null)
                            {
                                toTime = nextTran.DEPARTMENT_IN_TIME ?? long.MaxValue;
                            }

                            hasTran = hasTran || (fromTime <= intructionTime && intructionTime <= toTime);

                            times.Add(string.Format("từ {0}{1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(fromTime),
                            (toTime > 0 && toTime != long.MaxValue) ? " đến " + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(toTime) : ""));
                        }
                    }

                    if (!hasTran && times.Count > 0 && currentCo != null && currentCo.Count > 0)
                    {
                        times.Clear();
                    }

                    if (!hasTran && currentCo != null && currentCo.Count > 0)
                    {
                        currentCo = currentCo.OrderBy(o => o.START_TIME ?? 0).ToList();

                        long fromTime = 0;
                        long toTime = 0;

                        foreach (var item in currentCo)
                        {
                            fromTime = item.START_TIME ?? 0;
                            toTime = item.FINISH_TIME ?? long.MaxValue;

                            hasTran = hasTran || (fromTime <= intructionTime && intructionTime <= toTime);

                            times.Add(string.Format("từ {0}{1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(fromTime),
                            (toTime > 0 && toTime != long.MaxValue) ? " đến " + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(toTime) : ""));
                        }
                    }

                    if (!hasTran)
                    {
                        XtraMessageBox.Show(string.Format(ResourceMessage.ThoiGianYLenhKhongThuocKhoangThoiGianTrongKhoa,
                           string.Join(",", times)),
                            MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        this.isNotLoadWhileChangeInstructionTimeInFirst = true;
                        this.dtInstructionTime.Focus();
                        this.isNotLoadWhileChangeInstructionTimeInFirst = false;
                        return false;
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

        internal List<long> UcDateGetValue()
        {
            List<long> result = new List<long>();
            try
            {
                System.DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 2);
                this.timeSelested = today.Add(this.timeIntruction.TimeSpan);
                if (this.chkMultiIntructionTime.Checked)
                {
                    result = (this.intructionTimeSelected.Where(o => o.Value != DateTime.MinValue).Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + this.timeSelested.ToString("HHmm") + "00")).ToList());
                }
                else
                {
                    result = new List<long>();
                    if (this.dtInstructionTime.DateTime != DateTime.MinValue)
                    {
                        result.Add(Inventec.Common.TypeConvert.Parse.ToInt64(this.dtInstructionTime.DateTime.ToString("yyyyMMdd") + this.timeSelested.ToString("HHmm") + "00"));
                    }
                }
                result = result != null ? result.OrderByDescending(o => o).ToList() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal bool UcDateGetChkMultiDateState()
        {
            return chkMultiIntructionTime.Checked;
        }

        public void UcDateFocusControl()
        {
            try
            {
                dtInstructionTime.Focus();
                dtInstructionTime.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusUCDate()
        {
            try
            {
                //SendKeys.Send("{TAB}");
                ProcessShowpopupControlContainerRoom();
                //this.FocusShowpopup(this.cboServiceGroup, false);
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
                this.LoadDataToTrackingCombo();
                this.isMultiDateState = chkMultiIntructionTime.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
