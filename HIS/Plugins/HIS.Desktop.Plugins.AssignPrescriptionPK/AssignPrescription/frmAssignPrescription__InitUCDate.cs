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
using ACS.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.UC.DateEditor;
using HIS.UC.DateEditor.ADO;
using HIS.UC.PatientSelect;
using HIS.UC.PeriousExpMestList;
using HIS.UC.SecondaryIcd;
using HIS.UC.TreatmentFinish;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void UcDateInit()
        {
            try
            {
                //GlobalStore.IsTreatmentIn: Buồng
                //GlobalStore.IsCabinet: Tủ trực
                this.ValidationSingleControl(this.dtInstructionTime, this.dxValidationProviderControl, "", IsValidInstructionTime);
                this.lcichkMultiDate.Visibility = ((this.actionType != GlobalVariables.ActionEdit) && !GlobalStore.IsCabinet) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (!GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                {
                    chkMultiIntructionTime.ToolTip = "Với đơn phòng khám, chỉ cho phép kê đơn nhiều ngày với thuốc/vật tư mua ngoài";
                }
                this.licUseTime.Visibility = ((GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) || VHistreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.layoutControlItem41.Visibility = (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (HisConfigCFG.ManyDayPrescriptionOption == 2 && (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet))
                {
                    this.chkMultiIntructionTimeForMedi.Checked = true;
                    this.ValidationSingleControl(this.dtInstructionTimeForMedi, this.dxValidProviderBoXung, "", IsValidInstructionTimeForMedi);
                    this.lcichkMultiDateForMedi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lcitimeIntructionForMedi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                if (this.licUseTime.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    UcDateReloadUseTime();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool IsValidInstructionTime()
        {
            bool valid = true;
            try
            {
                if (!chkMultiIntructionTime.Checked)
                {
                    valid = ((this.dtInstructionTime.EditValue != null && !String.IsNullOrEmpty(this.dtInstructionTime.Text) && this.dtInstructionTime.DateTime != DateTime.MinValue)) && this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0;
                }
                else
                {
                    valid = (!String.IsNullOrEmpty(txtInstructionTime.Text)) && this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0;
                }
                if (!valid)
                {
                    Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.dtInstructionTime.EditValue), this.dtInstructionTime.EditValue) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.dtInstructionTime.DateTime), this.dtInstructionTime.DateTime) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.dtInstructionTime.Text), dtInstructionTime.Text) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.intructionTimeSelecteds), intructionTimeSelecteds) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => chkMultiIntructionTime.Checked), chkMultiIntructionTime.Checked));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        bool IsValidInstructionTimeForMedi()
        {
            bool valid = true;
            try
            {
                if (chkMultiIntructionTime.Checked)
                {
                    if (!chkMultiIntructionTimeForMedi.Checked)
                    {
                        valid = ((this.dtInstructionTimeForMedi.EditValue != null && !String.IsNullOrEmpty(this.dtInstructionTimeForMedi.Text) && this.dtInstructionTimeForMedi.DateTime != DateTime.MinValue)) && this.intructionTimeSelectedsForMedi != null && this.intructionTimeSelectedsForMedi.Count > 0;
                    }
                    else
                    {
                        valid = (!String.IsNullOrEmpty(txtInstructionTimeForMedi.Text)) && this.intructionTimeSelectedsForMedi != null && this.intructionTimeSelectedsForMedi.Count > 0;
                    }
                    if (!valid)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.dtInstructionTimeForMedi.EditValue), this.dtInstructionTimeForMedi.EditValue) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.dtInstructionTime.DateTime), this.dtInstructionTimeForMedi.DateTime) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.dtInstructionTimeForMedi.Text), dtInstructionTimeForMedi.Text) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.intructionTimeSelectedsForMedi), intructionTimeSelectedsForMedi) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => chkMultiIntructionTimeForMedi.Checked), chkMultiIntructionTimeForMedi.Checked));
                    }
                }                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
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
                        this.timeSelested = input.Time;
                    }
                    if (input.Dates != null && input.Dates.Count > 0)
                    {
                        this.dtInstructionTime.EditValue = input.Dates[0];
                        this.intructionTimeSelected = new List<DateTime?>();
                        this.intructionTimeSelected.AddRange(input.Dates);
                    }
                    this.intructionTimeSelecteds = this.intructionTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList();
                    this.InstructionTime = intructionTimeSelecteds.First();
                    this.LoadMLCT();
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
                        this.timeSelested = input.Time;
                    }

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
                        this.timeSelested = time;
                        this.timeIntruction.EditValue = this.timeSelested.ToString("HH:mm");
                        this.txtInstructionTime.Text = strTimeDisplay;
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
                LogSystem.Debug(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.ContructorIntructionTime), this.ContructorIntructionTime)
                   + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.treatmentId), this.treatmentId)
                   + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.intructionTimeSelecteds), this.intructionTimeSelecteds));
                DateTime now = DateTime.Now;
                if (ContructorIntructionTime > 0)
                {
                    now = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ContructorIntructionTime) ?? DateTime.Now;
                }

                if (input != null && input.Time != DateTime.MinValue && input.Dates != null && input.Dates.Count > 0)
                {
                    this.timeIntruction.EditValue = input.Time.ToString("HH:mm");
                    this.dtInstructionTime.EditValue = input.Time;
                    this.intructionTimeSelected = new List<DateTime?>();
                    this.intructionTimeSelected.AddRange(input.Dates);
                }
                else
                {
                    this.txtInstructionTime.Visible = false;
                    this.dtInstructionTime.Visible = true;
                    this.timeIntruction.EditValue = now.ToString("HH:mm");
                    this.dtInstructionTime.EditValue = now;
                    this.intructionTimeSelected = new List<DateTime?>();
                    this.intructionTimeSelected.Add(this.dtInstructionTime.DateTime);
                }
                System.DateTime today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 1);
                this.timeSelested = today.Add(timeIntruction.TimeSpan);
                this.intructionTimeSelecteds = this.intructionTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList();
                this.InstructionTime = intructionTimeSelecteds.First();
                this.LoadMLCT();
                GetListEMMedicineAcinInteractive();
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
                System.DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1);
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
                cboMediStockExport.Focus();
                cboMediStockExport.ShowPopup();
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
                this.InitComboTracking(cboPhieuDieuTri);
                this.isMultiDateState = chkMultiIntructionTime.Checked;
                if (HisConfigCFG.ManyDayPrescriptionOption == 2 && GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                {
                    if (this.isMultiDateState)
                    {
                        //if (this.ucDateForMediProcessor == null || this.ucDateForMedi == null)
                        //    InitUcDateForMedi();
                        //this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);

                        UC.DateEditor.ADO.DateInputHasCheckADO dateInputForMediADO = new UC.DateEditor.ADO.DateInputHasCheckADO();
                        dateInputForMediADO.Dates = new List<DateTime?>();
                        dateInputForMediADO.IsMultiDayChecked = true;
                        dateInputForMediADO.IsVisibleMultiDate = true;
                        foreach (var itemDate in this.intructionTimeSelecteds)
                        {
                            DateTime? itemDateCV = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(itemDate);
                            if (itemDateCV != null && itemDateCV.Value != DateTime.MinValue)
                            {
                                dateInputForMediADO.Dates.Add(itemDateCV);
                                dateInputForMediADO.Time = itemDateCV.Value;
                            }
                        }

                        UcDateSetValueHasCheckForMedi(dateInputForMediADO);
                        //this.ucDateForMediProcessor.SetValue(this.ucDateForMedi, dateInputForMediADO);

                        lciForpnlUCDateForMedi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        emptySpaceItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        emptySpaceItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => intructionTimeSelectedsForMedi), intructionTimeSelectedsForMedi));
                    }
                    else
                    {
                        lciForpnlUCDateForMedi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        emptySpaceItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        emptySpaceItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangeIntructionTimeEditor(DateTime intructTime)
        {
            try
            {
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
                        EnableCheckTemporaryPres();
                        this.ChangeIntructionTime(intructTime);
                        this.GetListEMMedicineAcinInteractive();
                    }
                }
                else
                {
                    this.intructionTimeSelected = new List<DateTime?>();
                    this.InstructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(intructTime.ToString("yyyyMMdd") + this.timeSelested.ToString("HHmm") + "00");
                    EnableCheckTemporaryPres();
                    this.intructionTimeSelected.Add(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.InstructionTime));
                    this.intructionTimeSelecteds = new List<long>();
                    this.intructionTimeSelecteds.Add(this.InstructionTime);
                    this.ChangeIntructionTime(intructTime);
                    this.GetListEMMedicineAcinInteractive();
                }
                this.CheckAssignServiceSimultaneityOption();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void UcDateSetValueForMedi(DateInputADO input)
        {
            try
            {
                if (input != null)
                {
                    if (input.Time != null && input.Time != DateTime.MinValue)
                    {
                        this.timeIntructionForMedi.EditValue = input.Time.ToString("HH:mm");
                        this.timeSelestedForMedi = input.Time;
                    }
                    if (input.Dates != null && input.Dates.Count > 0)
                    {
                        this.dtInstructionTimeForMedi.EditValue = input.Dates[0];
                        this.intructionTimeSelectedForMedi = new List<DateTime?>();
                        this.intructionTimeSelectedForMedi.AddRange(input.Dates);
                    }
                    this.intructionTimeSelectedsForMedi = this.intructionTimeSelectedForMedi.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelestedForMedi.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void UcDateSetValueHasCheckForMedi(DateInputHasCheckADO input)
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
                        this.timeIntructionForMedi.EditValue = input.Time.ToString("HH:mm");
                        this.timeSelestedForMedi = input.Time;
                    }

                    this.isStopEventChangeMultiDate = true;
                    this.chkMultiIntructionTimeForMedi.Checked = input.IsMultiDayChecked;
                    this.txtInstructionTimeForMedi.Visible = input.IsMultiDayChecked;
                    this.dtInstructionTimeForMedi.Visible = !input.IsMultiDayChecked;

                    if (input.IsMultiDayChecked)
                    {
                        this.SelectMultiIntructionTimeForMedi(input.Dates, input.Time);
                    }
                    else
                    {
                        if (input.Dates != null && input.Dates.Count > 0)
                        {
                            this.dtInstructionTimeForMedi.EditValue = input.Dates[0];
                            this.intructionTimeSelectedForMedi = new List<DateTime?>();
                            this.intructionTimeSelectedForMedi.AddRange(input.Dates);
                        }

                        //this.InstructionTime = this.intructionTimeSelecteds.First();
                    }
                    this.intructionTimeSelectedsForMedi = this.intructionTimeSelectedForMedi != null ? (this.intructionTimeSelectedForMedi.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelestedForMedi.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList()) : null;
                    this.isStopEventChangeMultiDate = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectMultiIntructionTimeForMedi(List<DateTime?> datas, DateTime time)
        {
            try
            {
                if (datas != null && time != DateTime.MinValue)
                {
                    this.intructionTimeSelectedForMedi = datas as List<DateTime?>;
                    string strTimeDisplay = "";
                    int num = 0;
                    this.intructionTimeSelectedForMedi = this.intructionTimeSelectedForMedi.OrderBy(o => o.Value).ToList();
                    foreach (var item in this.intructionTimeSelectedForMedi)
                    {
                        if (item != null && item.Value != DateTime.MinValue)
                        {
                            strTimeDisplay += (num == 0 ? "" : "; ") + item.Value.ToString("dd/MM");
                            num++;
                        }
                    }
                    if (this.txtInstructionTimeForMedi.Text != strTimeDisplay)
                    {
                        //Trường hợp chọn nhiều ngày chỉ định thì lấy đối tượng bệnh nhân tuong uong voi intructiontime dau tien duoc chon
                        //Vì các dữ liệu liên quan như chính sách giá, đối tượng chấp nhận thanh toán phải suy ra từ đối tượng BN ở trên
                        this.timeSelestedForMedi = time;
                        this.timeIntructionForMedi.EditValue = this.timeSelestedForMedi.ToString("HH:mm");
                        this.txtInstructionTimeForMedi.Text = strTimeDisplay;
                    }

                    this.intructionTimeSelectedsForMedi = this.intructionTimeSelectedForMedi != null ? (this.intructionTimeSelectedForMedi.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + time.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList()) : null;
                }
                //DelegateSelectMultiDate(datas, time);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void UcDateForMediFocusControl()
        {
            try
            {
                dtInstructionTimeForMedi.Focus();
                dtInstructionTimeForMedi.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusUCDateForMedi()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("NextForcusUCDateForMedi");
                btnAdd.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegateMultiDateForMediChanged()
        {
            //this.InitComboTracking(cboPhieuDieuTri);
        }

        private void ChangeIntructionTimeForMedi(DateTime intructTime)
        {
            try
            {
                System.DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1);
                this.timeSelestedForMedi = today.Add(this.timeIntructionForMedi.TimeSpan);
                if (chkMultiIntructionTimeForMedi.Checked)
                {
                    if (this.intructionTimeSelectedForMedi != null && this.intructionTimeSelectedForMedi.Count > 0 && intructTime != DateTime.MinValue)
                    {
                        for (int i = 0, j = this.intructionTimeSelectedForMedi.Count; i < j; i++)
                        {
                            this.intructionTimeSelectedForMedi[i] = new DateTime(this.intructionTimeSelectedForMedi[i].Value.Year, this.intructionTimeSelectedForMedi[i].Value.Month, this.intructionTimeSelectedForMedi[i].Value.Day, intructTime.Hour, intructTime.Minute, 0);
                        }
                        this.intructionTimeSelectedsForMedi = (this.intructionTimeSelectedForMedi.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelestedForMedi.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList());
                        //this.InstructionTimeForMedi = this.intructionTimeSelectedsForMedi.OrderByDescending(o => o).FirstOrDefault();
                        //ChangeIntructionTimeForMedi(intructTime);
                    }
                }
                else
                {
                    this.intructionTimeSelectedForMedi = new List<DateTime?>();
                    var InstructionTimeForMedi = Inventec.Common.TypeConvert.Parse.ToInt64(intructTime.ToString("yyyyMMdd") + this.timeSelestedForMedi.ToString("HHmm") + "00");
                    this.intructionTimeSelectedForMedi.Add(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(InstructionTimeForMedi));
                    this.intructionTimeSelectedsForMedi = new List<long>();
                    this.intructionTimeSelectedsForMedi.Add(InstructionTimeForMedi);
                    //ChangeIntructionTimeForMedi(this.dtInstructionTimeForMedi.DateTime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public List<long> UcDateGetValueForMedi()
        {
            return intructionTimeSelectedsForMedi;
        }

        private void SelectMultiUseTime(List<DateTime?> datas, DateTime time)
        {
            try
            {

                this.UseTimeSelected = datas as List<DateTime?>;
                string strTimeDisplay = "";
                int num = 0;
                this.UseTimeSelected = this.UseTimeSelected.OrderBy(o => o.Value).ToList();
                foreach (var item in this.UseTimeSelected)
                {
                    if (item != null && item.Value != DateTime.MinValue)
                    {
                        strTimeDisplay += (num == 0 ? "" : "; ") + item.Value.ToString("dd/MM");
                        num++;
                    }
                }
                if (this.txtUseTime.Text != strTimeDisplay)
                {
                    this.txtUseTime.Text = strTimeDisplay;
                }

                DelegateSelectMultiDateUseTime(datas, time);

                if (this.UseTimeSelected != null && this.UseTimeSelected.Count > 0)
                {
                    this.chkMultiIntructionTime.Enabled = false;
                }
                else
                {
                    this.chkMultiIntructionTime.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeUseTimeEditor(DateTime useTime)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    if (this.UseTimeSelected != null && this.UseTimeSelected.Count > 0 && useTime != DateTime.MinValue)
                    {
                        for (int i = 0, j = this.UseTimeSelected.Count; i < j; i++)
                        {
                            this.UseTimeSelected[i] = new DateTime(this.UseTimeSelected[i].Value.Year, this.UseTimeSelected[i].Value.Month, this.UseTimeSelected[i].Value.Day, 0, 0, 0);
                        }
                        this.UseTimeSelecteds = (this.UseTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + "000000")).OrderByDescending(o => o).ToList());
                        this.UseTime = this.UseTimeSelecteds.OrderByDescending(o => o).FirstOrDefault();
                        
                    }
                }
                else
                {
                    this.UseTimeSelected = new List<DateTime?>();
                    this.UseTime = Inventec.Common.TypeConvert.Parse.ToInt64(useTime.ToString("yyyyMMdd") + "000000");
                    this.UseTimeSelected.Add(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.UseTime));
                    this.UseTimeSelecteds = new List<long>();
                    this.UseTimeSelecteds.Add(this.UseTime);
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void UcDateReloadUseTime()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    this.txtUseTime.Visible = true;
                    this.dtUseTime.Visible = false;
                }
                else
                {
                    this.txtUseTime.Visible = false;
                    this.dtUseTime.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void UcDateSetValueUseTime(DateInputADO input)
        {
            try
            {
                if (input != null)
                {
                    if (input.Dates != null && input.Dates.Count > 0)
                    {
                        this.dtUseTime.EditValue = input.Dates[0];
                        this.UseTimeSelected = new List<DateTime?>();
                        this.UseTimeSelected.AddRange(input.Dates);
                    }
                    this.UseTimeSelecteds = this.UseTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + "000000")).OrderByDescending(o => o).ToList();
                    this.UseTime = UseTimeSelecteds.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //public void UcDateSetValueHasCheckUseTime(DateInputHasCheckADO input)
        //{
        //    try
        //    {
        //        if (input != null)
        //        {
        //            DateInputADO dateInputADO = new DateInputADO();
        //            dateInputADO.Dates = input.Dates;

        //            if (this.actionType == GlobalVariables.ActionAdd)
        //            {
        //                this.txtUseTime.Visible = true;
        //                this.dtUseTime.Visible = false;

        //                this.SelectMultiUseTime(input.Dates, input.Time);
        //            }
        //            else
        //            {
        //                this.txtUseTime.Visible = false;
        //                this.dtUseTime.Visible = true;

        //                if (input.Dates != null && input.Dates.Count > 0)
        //                {
        //                    this.dtUseTime.EditValue = input.Dates[0];
        //                    this.UseTimeSelected = new List<DateTime?>();
        //                    this.UseTimeSelected.AddRange(input.Dates);
        //                }
        //                this.UseTimeSelecteds = (this.UseTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + "000000")).OrderByDescending(o => o).ToList());
        //                this.UseTime = this.UseTimeSelecteds.First();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        internal List<long> UcDateGetValueUseTime()
        {
            List<long> result = new List<long>();
            try
            {
                if (this.txtUseTime.Visible)
                {
                    result = (this.UseTimeSelected.Where(o => o.Value != DateTime.MinValue).Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + "000000")).ToList());
                }
                else
                {
                    result = new List<long>();
                    if (this.dtUseTime.DateTime != DateTime.MinValue)
                    {
                        result.Add(Inventec.Common.TypeConvert.Parse.ToInt64(this.dtUseTime.DateTime.ToString("yyyyMMdd") + "000000"));
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
    }
}
