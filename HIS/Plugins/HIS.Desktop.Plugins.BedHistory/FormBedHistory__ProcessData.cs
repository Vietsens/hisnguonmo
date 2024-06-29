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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.BedHistory.ADO;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BedHistory
{
    public partial class FormBedHistory : HIS.Desktop.Utility.FormBase
    {
        private void FillDataIntoPrimaryPatientTypeCombo(HisBedServiceTypeADO data, GridLookUpEdit patientTypeCombo)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = new List<HIS_PATIENT_TYPE>();
                long instructionTime = data.START_TIME;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("FillDataIntoPrimaryPatientTypeCombo HisBedServiceTypeADO", instructionTime));
                long? intructionNumByType = 1;
                var patientType = currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ACTIVE == 1 && o.IS_ADDITION == 1).ToList();
                foreach (var item in patientType)
                {
                    if (item.ID == data.PATIENT_TYPE_ID)
                        continue;
                    var itemPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(Base.GlobalStore.HisVServicePatys, WorkPlaceSDO.BranchId, null, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, instructionTime, this.CurrentTreatment.IN_TIME, data.BED_SERVICE_TYPE_ID ?? 0, item.ID, null, intructionNumByType);
                    if (itemPaty != null)
                    {
                        dataCombo.Add(item);
                    }
                }

                this.InitComboPatientType(patientTypeCombo, dataCombo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoPrimaryPatientTypeCombo(HisBedHistoryADO data, GridLookUpEdit patientTypeCombo)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = new List<HIS_PATIENT_TYPE>();
                long instructionTime = data.START_TIME > 0 ? data.START_TIME : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(data.startTime) ?? 0);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("FillDataIntoPrimaryPatientTypeCombo HisBedHistoryADO", instructionTime));
                long? intructionNumByType = 1;
                var patientType = currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ACTIVE == 1 && o.IS_ADDITION == 1).ToList();
                foreach (var item in patientType)
                {
                    if (item.ID == data.PATIENT_TYPE_ID)
                        continue;
                    var itemPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(Base.GlobalStore.HisVServicePatys, WorkPlaceSDO.BranchId, null, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, instructionTime, this.CurrentTreatment.IN_TIME, data.BED_SERVICE_TYPE_ID ?? 0, item.ID, null, intructionNumByType);
                    if (itemPaty != null)
                    {
                        dataCombo.Add(item);
                    }
                }

                this.InitComboPatientType(patientTypeCombo, dataCombo);
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
                ControlEditorLoader.Load(cboPatientType, (data != null ? data.OrderBy(o => o.PRIORITY).ToList() : null), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //chia theo ngày
        //Vào 18h ngày 01/05/2019 ra 15h ngày 05/05/2019 Số lượng giường =5
        //ngày 1: thời gian chỉ định 18h số lượng 1
        //ngày 2: thời gian chỉ định 18h số lượng 1 (có thể set 00h00 ngày 02/05 - tùy code)
        //ngày 3: thời gian chỉ định 18h số lượng 1
        //ngày 4: thời gian chỉ định 18h số lượng 1
        //ngày 5: thời gian chỉ định 15h số lượng 1
        private List<HisBedServiceTypeADO> ProcessSplitDay()
        {
            List<HisBedServiceTypeADO> result = null;
            try
            {
                if (bedLogCheckProcessing != null && bedLogCheckProcessing.Count > 0)
                {
                    result = new List<HisBedServiceTypeADO>();

                    var lstSplitBedLogByDay = ProcessSplitBedLogByDay(bedLogCheckProcessing);
                    if (lstSplitBedLogByDay != null && lstSplitBedLogByDay.Count > 0)
                    {
                        var groupByDate = lstSplitBedLogByDay.GroupBy(g => new { g.startTime.Date, g.BED_SERVICE_TYPE_ID, g.SHARE_COUNT }).Select(grc => grc.ToList()).ToList();

                        foreach (var bedServiceTypes in groupByDate)
                        {
                            this.ExecuteTotalDateTimeBed(bedServiceTypes, ref result);
                        }
                        if (IsShareBed == "1")
                        {
                            foreach (var item in result)
                            {
                                if (dicBedLog.ContainsKey(item.BED_ID) && dicBedLog[item.BED_ID] != null && dicBedLog[item.BED_ID].Count > 0)
                                {
                                    var bedLogs = dicBedLog[item.BED_ID].Where(o => o.TREATMENT_ID != _TreatmentBedRoom.TREATMENT_ID).ToList();
                                    if (bedLogs.FirstOrDefault(o => o.START_TIME <= item.START_TIME && item.START_TIME <= o.FINISH_TIME) != null)
                                    {
                                        item.AmmoutNamGhep = bedLogs.Where(o => o.START_TIME <= item.START_TIME && item.START_TIME <= o.FINISH_TIME).Count() + 1;
                                    }
                                    else
                                    {
                                        item.AmmoutNamGhep = null;
                                    }
                                }
                                else
                                    item.AmmoutNamGhep = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = ListBedServiceTypes;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<HisBedHistoryADO> ProcessSplitBedLogByDay(List<HisBedHistoryADO> bedLogCheckProcessing)
        {
            List<HisBedHistoryADO> result = null;
            try
            {
                if (bedLogCheckProcessing != null && bedLogCheckProcessing.Count > 0)
                {
                    result = new List<HisBedHistoryADO>();
                    foreach (var item in bedLogCheckProcessing)
                    {
                        long? namghep = null;

                        var days = chkSplitByResult.Checked ? SplitBedServiceByCircular(item.startTime, item.finishTime ?? DateTime.Now) : ProcessTotalBedDay(new List<HisBedHistoryADO>() { item }, ref namghep);

                        if (days > 1)
                        {
                            for (int i = 0; i < days; i++)
                            {
                                ADO.HisBedHistoryADO ado = new ADO.HisBedHistoryADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisBedHistoryADO>(ado, item);

                                DateTime startTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.START_TIME) ?? DateTime.Now;
                                if (startTime != DateTime.Now)
                                {
                                    startTime = startTime.AddDays(i);
                                    ado.START_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(startTime) ?? 0;
                                    ado.FINISH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(startTime.AddDays(1)) ?? 0;
                                }

                                if (i == days - 1)
                                {
                                    if (ado.START_TIME > item.FINISH_TIME)
                                    {
                                        if (chkSplitByResult.Checked)
                                            ado.START_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(startTime.ToString("yyyyMMdd") + "000000");
                                        else
                                            ado.START_TIME = item.FINISH_TIME ?? 0;

                                    }
                                    ado.FINISH_TIME = item.FINISH_TIME;
                                }

                                ado.startTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ado.START_TIME) ?? DateTime.Now;
                                ado.finishTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ado.FINISH_TIME ?? 0) ?? DateTime.Now;

                                result.Add(ado);
                            }
                        }
                        else
                        {
                            result.Add(item);
                        }

                        var last = result.Last();
                        if (last.startTime.Day != last.finishTime.Value.Day && !chkSplitByResult.Checked)
                        {
                            ADO.HisBedHistoryADO ado = new ADO.HisBedHistoryADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisBedHistoryADO>(ado, item);

                            //DateTime date = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.FINISH_TIME ?? 0);

                            ado.START_TIME = item.FINISH_TIME ?? 0;

                            ado.FINISH_TIME = item.FINISH_TIME ?? 0;

                            ado.startTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ado.START_TIME) ?? DateTime.Now;
                            ado.finishTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ado.FINISH_TIME ?? 0) ?? DateTime.Now;

                            result.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private decimal ProcessTotalBedDay(List<HisBedHistoryADO> bebHistoryAdos, ref long? namghep)
        {
            decimal result = 0;
            try
            {
                if (bebHistoryAdos != null && bebHistoryAdos.Count > 0)
                {
                    long tongSoGio = 0;
                    foreach (var item in bebHistoryAdos)
                    {
                        DateTime timeFinish = new DateTime();
                        DateTime timeStart = new DateTime();
                        if (ChkNotCountHours.Checked)
                        {
                            DateTime time = item.finishTime ?? DateTime.Now;
                            timeFinish = new DateTime(time.Year, time.Month, time.Day);
                            timeStart = new DateTime(item.startTime.Year, item.startTime.Month, item.startTime.Day);
                        }
                        else
                        {
                            timeFinish = item.finishTime ?? DateTime.Now;
                            timeStart = item.startTime;
                        }

                        TimeSpan diff = timeFinish - timeStart;
                        if (diff.Days > 0)
                        {
                            result += diff.Days;
                        }
                        else
                        {
                            if (ChkNotCountHours.Checked) result += 1;
                        }

                        if (diff.Hours > 0)
                        {
                            tongSoGio += diff.Hours;
                        }

                        if (item.SHARE_COUNT.HasValue)
                        {
                            if (!namghep.HasValue || namghep < item.SHARE_COUNT)
                            {
                                namghep = item.SHARE_COUNT;
                            }
                        }
                    }

                    if (tongSoGio > 0)
                    {
                        result += tongSoGio / 24;
                        if ((tongSoGio % 24) != 0)
                        {
                            result += 1;
                        }
                        else
                        {
                            result += 1;
                            //tongSoNgayGiuong += System.Convert.ToDecimal(0.5);
                        }
                    }
                    //else
                    //{
                    //    tongSoNgayGiuong += 1;
                    //    //tongSoNgayGiuong += System.Convert.ToDecimal(0.5);
                    //}

                    if (result == 0) result = 1;
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void FillDataIntoServiceConditionCombo(HisBedHistoryADO data, GridLookUpEdit grd)
        {
            try
            {
                if (grd != null && data.PATIENT_TYPE_ID != null)
                {
                    var dataCondition = GetListServicePatyCondition(data);
                    InitComboServiceCondition(grd, dataCondition);
                    if (dataCondition != null && dataCondition.Count == 1)
                    {
                        data.SERVICE_CONDITION_ID = dataCondition[0].ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<ServicePatyConditionADO> GetListServicePatyCondition(HisBedHistoryADO data)
        {
            List<ServicePatyConditionADO> result = new List<ServicePatyConditionADO>();
            try
            {
                var dataCondition = BranchDataWorker.ServicePatyWithListPatientType(data.BED_SERVICE_TYPE_ID ?? 0, new List<long>() { data.PATIENT_TYPE_ID ?? 0 });
                if (dataCondition != null && dataCondition.Count > 0)
                {
                    dataCondition = dataCondition.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.SERVICE_CONDITION_ID.HasValue && o.SERVICE_CONDITION_ID > 0).ToList();
                    if (dataCondition != null && dataCondition.Count > 0)
                    {
                        List<V_HIS_SERVICE_PATY> dataConditionTmps = new List<V_HIS_SERVICE_PATY>();
                        foreach (var item in dataCondition)
                        {
                            if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.SERVICE_CONDITION_ID == item.SERVICE_CONDITION_ID))
                            {
                                dataConditionTmps.Add(item);
                            }
                        }
                        dataCondition.Clear();
                        dataCondition.AddRange(dataConditionTmps);
                    }
                }
                if (dataCondition != null && dataCondition.Count > 0 && lstConditionService != null && lstConditionService.Count > 0)
                {
                    dataCondition = dataCondition.Where(o => lstConditionService.Exists(p => p.SERVICE_ID == (data.BED_SERVICE_TYPE_ID ?? 0) && p.ID == o.SERVICE_CONDITION_ID)).ToList();
                }
                AutoMapper.Mapper.CreateMap<V_HIS_SERVICE_PATY, ServicePatyConditionADO>();
                return AutoMapper.Mapper.Map<List<ServicePatyConditionADO>>(dataCondition);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;

        }

        private void InitComboServiceCondition(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cboServiceCondition, List<HIS_SERVICE_CONDITION> servicePatyConditionADOs)
        {
            try
            {
                cboServiceCondition.DataSource = servicePatyConditionADOs;
                cboServiceCondition.DisplayMember = "SERVICE_CONDITION_NAME";
                cboServiceCondition.ValueMember = "ID";

                cboServiceCondition.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceCondition.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceCondition.ImmediatePopup = true;
                cboServiceCondition.View.Columns.Clear();

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboServiceCondition.View.Columns.AddField("SERVICE_CONDITION_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 80;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cboServiceCondition.View.Columns.AddField("SERVICE_CONDITION_NAME");
                aColumnName.Caption = "Tên";
                DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                repositoryItemMemoEdit1.Appearance.Options.UseTextOptions = true;
                repositoryItemMemoEdit1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
                aColumnName.ColumnEdit = repositoryItemMemoEdit1;
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 250;

                DevExpress.XtraGrid.Columns.GridColumn aColumnHein = cboServiceCondition.View.Columns.AddField("HEIN_RATIO_DISPLAY");
                aColumnHein.Caption = "Tỉ lệ thanh toán";
                aColumnHein.Visible = true;
                aColumnHein.VisibleIndex = 2;
                aColumnHein.Width = 100;

                cboServiceCondition.PopupFormSize = new Size(430, 320);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboServiceCondition(GridLookUpEdit cboServiceCondition, List<ServicePatyConditionADO> servicePatyConditionADOs)
        {
            try
            {
                cboServiceCondition.Properties.DataSource = servicePatyConditionADOs;
                cboServiceCondition.Properties.DisplayMember = "SERVICE_CONDITION_NAME";
                cboServiceCondition.Properties.ValueMember = "SERVICE_CONDITION_ID";

                cboServiceCondition.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceCondition.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceCondition.Properties.ImmediatePopup = true;
                cboServiceCondition.ForceInitialize();
                cboServiceCondition.Properties.View.Columns.Clear();

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboServiceCondition.Properties.View.Columns.AddField("SERVICE_CONDITION_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 80;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cboServiceCondition.Properties.View.Columns.AddField("SERVICE_CONDITION_NAME");
                aColumnName.Caption = "Tên";
                DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                repositoryItemMemoEdit1.Appearance.Options.UseTextOptions = true;
                repositoryItemMemoEdit1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
                aColumnName.ColumnEdit = repositoryItemMemoEdit1;
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 250;

                DevExpress.XtraGrid.Columns.GridColumn aColumnHein = cboServiceCondition.Properties.View.Columns.AddField("HEIN_RATIO_DISPLAY");
                aColumnHein.Caption = "Tỉ lệ thanh toán";
                aColumnHein.Visible = true;
                aColumnHein.VisibleIndex = 2;
                aColumnHein.Width = 100;

                cboServiceCondition.Properties.PopupFormSize = new Size(430, 320);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataIntoPatientTypeCombo(HisBedHistoryADO data, GridLookUpEdit patientTypeCombo)
        {
            try
            {
                if (patientTypeCombo != null)
                {
                    long intructionTime = data.START_TIME > 0 ? data.START_TIME : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(data.startTime) ?? 0);
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("FillDataIntoPatientTypeCombo HisBedHistoryADO", intructionTime));
                    long treatmentTime = this.CurrentTreatment.IN_TIME;

                    List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = null;

                    var patientType = currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ACTIVE == 1).ToList();
                    if (BranchDataWorker.HasServicePatyWithListPatientType(data.BED_SERVICE_TYPE_ID ?? 0, patientType.Select(o=>o.ID).Distinct().ToList()))
                    {
                        var arrPatientTypeCode = BranchDataWorker.DicServicePatyInBranch[data.BED_SERVICE_TYPE_ID ?? 0].Where(o => ((!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime) || (!o.TO_TIME.HasValue || o.TO_TIME.Value >= intructionTime))).Select(s => s.PATIENT_TYPE_CODE).Distinct().ToList();

                        if (currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                        {
                            foreach (var item in currentPatientTypeWithPatientTypeAlter)
                            {
                                var arrInheritPatientTypeCode = BranchDataWorker.DicServicePatyInBranch[data.BED_SERVICE_TYPE_ID ?? 0].Where(o => (o.INHERIT_PATIENT_TYPE_IDS != null && ("," + o.INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + item.ID + ","))).ToList();

                                if (arrInheritPatientTypeCode != null && arrInheritPatientTypeCode.Count > 0)
                                {
                                    var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.ID == item.ID).Select(s => s.PATIENT_TYPE_CODE).ToList();
                                    arrPatientTypeCode.AddRange(patientTypes);
                                }
                            }
                        }

                        arrPatientTypeCode = arrPatientTypeCode.Distinct().ToList();

                        dataCombo = (arrPatientTypeCode != null && arrPatientTypeCode.Count > 0 ? currentPatientTypeWithPatientTypeAlter.Where(o => arrPatientTypeCode.Contains(o.PATIENT_TYPE_CODE)).ToList() : null);

                    }
                    this.InitComboPatientType(patientTypeCombo, dataCombo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoPatientTypeCombo(HisBedServiceTypeADO data, GridLookUpEdit patientTypeCombo)
        {
            try
            {
                if (patientTypeCombo != null)
                {
                    long intructionTime = data.START_TIME;
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("FillDataIntoPatientTypeCombo HisBedServiceTypeADO: ", intructionTime));

                    long treatmentTime = this.CurrentTreatment.IN_TIME;
                    List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = new List<HIS_PATIENT_TYPE>();
                    var patientType = this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ACTIVE == 1).ToList();
                    foreach (var item in patientType)
                    {
                        var itemPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(Base.GlobalStore.HisVServicePatys, WorkPlaceSDO.BranchId, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, intructionTime, this.CurrentTreatment.IN_TIME, data.BED_SERVICE_TYPE_ID ?? 0, item.ID, null, 1);
                        if (itemPaty != null)
                        {
                            dataCombo.Add(item);
                        }
                    }

                    this.InitComboPatientType(patientTypeCombo, dataCombo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadAppliedPatientType(long patientTypeId, long serviceId, ref HisBedHistoryADO sereServADO)
        {
            try
            {
                if (serviceId > 0)
                {
                    var checkService = _services.Find(o => o.ID == serviceId);
                    if (checkService != null && (string.IsNullOrEmpty(checkService.APPLIED_PATIENT_TYPE_IDS) || IsContainString(checkService.APPLIED_PATIENT_TYPE_IDS, patientTypeId.ToString())))
                    {
                        sereServADO.IsContainAppliedPatientType = true;
                    }
                    else
                    {
                        sereServADO.IsContainAppliedPatientType = false;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadAppliedPatientType(long patientTypeId, long serviceId, ref HisBedServiceTypeADO sereServADO)
        {
            try
            {
                if (serviceId > 0)
                {
                    var checkService = _services.Find(o => o.ID == serviceId);
                    if (checkService != null && (string.IsNullOrEmpty(checkService.APPLIED_PATIENT_TYPE_IDS) || IsContainString(checkService.APPLIED_PATIENT_TYPE_IDS, patientTypeId.ToString())))
                    {
                        sereServADO.IsContainAppliedPatientType = true;
                    }
                    else
                    {
                        sereServADO.IsContainAppliedPatientType = false;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool IsContainString(string arrStr, string str)
        {
            bool result = false;
            try
            {
                if (arrStr.Contains(","))
                {
                    var arr = arrStr.Split(',');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        result = arr[i] == str;
                        if (result) break;
                    }
                }
                else
                {
                    result = arrStr == str;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
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
        private void ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, HisBedServiceTypeADO sereServADO)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                var patientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                if (patientTypes != null && patientTypes.Count > 0 && currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                {
                    LoadAppliedPatientType(patientTypeId, serviceId, ref sereServADO);
                    long intructionTime = sereServADO.START_TIME;
                    long treatmentTime = this.CurrentTreatment.IN_TIME;
                    List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeTemps = new List<HIS_PATIENT_TYPE>();
                    var patientType = currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ACTIVE == 1).ToList();
                    foreach (var item in patientType)
                    {
                        var itemPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(Base.GlobalStore.HisVServicePatys, WorkPlaceSDO.BranchId, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, intructionTime, this.CurrentTreatment.IN_TIME, serviceId, item.ID, null, 1);
                        if (itemPaty != null)
                        {
                            currentPatientTypeTemps.Add(item);
                        }
                    }

                    if (currentPatientTypeTemps != null && currentPatientTypeTemps.Count > 0)
                    {
                        if (Base.GlobalStore.IsPrimaryPatientType != commonString__true
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != Base.GlobalStore.PatientTypeId__BHYT
                            && currentPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                        }
                        else if (Base.GlobalStore.IsPrimaryPatientType == "0"
                            && sereServADO.BILL_PATIENT_TYPE_ID.HasValue
                            && patientTypeId != sereServADO.BILL_PATIENT_TYPE_ID.Value
                            && currentPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                        }
                        //else if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == Base.GlobalStore.PatientTypeId__BHYT
                        //&& ((this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) > (intructionTime - (intructionTime % 1000000))
                        //|| (this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0) < (intructionTime - (intructionTime % 1000000))
                        //))
                        else if (this.CurrentTreatment.TDL_PATIENT_TYPE_ID == Base.GlobalStore.PatientTypeId__BHYT
                            && (this.CurrentTreatment.HEIN_CARD_FROM_TIME > (intructionTime - (intructionTime % 1000000))
                            || this.CurrentTreatment.HEIN_CARD_TO_TIME < (intructionTime - (intructionTime % 1000000)))
                            )
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == Base.GlobalStore.PatientTypeId__VP);
                        }
                        else
                        {
                            result = (currentPatientTypeTemps != null ? (currentPatientTypeTemps.FirstOrDefault(o => o.ID == patientTypeId) ?? currentPatientTypeTemps[0]) : null);
                        }

                        if (result != null && sereServADO != null)
                        {
                            sereServADO.PATIENT_TYPE_ID = result.ID;
                            sereServADO.PATIENT_TYPE_CODE = result.PATIENT_TYPE_CODE;
                            sereServADO.PATIENT_TYPE_NAME = result.PATIENT_TYPE_NAME;
                        }

                        if (Base.GlobalStore.IsPrimaryPatientType == "2")
                        {
                            if (this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID <= 0)
                            {
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = null;
                            }
                            else
                            {
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID;
                                if (currentPatientTypeTemps.Exists(e => e.ID == this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID))
                                {
                                    var priPaty = currentPatientTypeTemps.FirstOrDefault(o => o.ID == this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID);
                                    sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                                }
                                else
                                {
                                    //try
                                    //{
                                    //    var billPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID);
                                    //    string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                                    //    sereServADO.ErrorMessagePatientTypeId = String.Format(ResourceMessage.DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia, patyName);
                                    //    sereServADO.ErrorTypePatientTypeId = ErrorType.Warning;
                                    //}
                                    //catch (Exception ex)
                                    //{
                                    //    Inventec.Common.Logging.LogSystem.Error(ex);
                                    //}
                                }
                            }
                        }
                        else if (Base.GlobalStore.IsPrimaryPatientType == commonString__true
                            && sereServADO.BILL_PATIENT_TYPE_ID.HasValue
                            && sereServADO.PATIENT_TYPE_ID != sereServADO.BILL_PATIENT_TYPE_ID.Value
                            && sereServADO.IsContainAppliedPatientType
                            )
                        {
                            if (currentPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value))
                            {
                                var priPaty = currentPatientTypeTemps.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                            }
                            else
                            {
                                //try
                                //{
                                //    var billPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                                //    string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                                //    sereServADO.ErrorMessagePatientTypeId = String.Format(ResourceMessage.DichVuCoDTTTBatBuocNhungKhongCoChinhSachGia, patyName);
                                //    sereServADO.ErrorTypePatientTypeId = ErrorType.Warning;
                                //}
                                //catch (Exception ex)
                                //{
                                //    Inventec.Common.Logging.LogSystem.Error(ex);
                                //}
                            }
                        }
                        else if (Base.GlobalStore.IsPrimaryPatientType == commonString__true
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != Base.GlobalStore.PatientTypeId__BHYT
                            && currentPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                            && result.ID != this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                        {
                            var priPaty = currentPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                        }
                        else
                        {
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = null;
                        }
                    }
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADO), sereServADO));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, HisBedHistoryADO sereServADO)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                sereServADO.ErrorMessagePrimaryPatientTypeId = null;
                sereServADO.ErrorTypePrimaryPatientTypeId = ErrorType.None;
                sereServADO.ErrorMessagePatientTypeId = null;
                sereServADO.ErrorTypePatientTypeId = ErrorType.None;
                var patientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                if (patientTypes != null && patientTypes.Count > 0 && currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                {
                    LoadAppliedPatientType(patientTypeId, serviceId, ref sereServADO);
                    long intructionTime = sereServADO.START_TIME > 0 ? sereServADO.START_TIME : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(sereServADO.startTime) ?? 0);
                    long treatmentTime = this.CurrentTreatment.IN_TIME;
                    List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeTemps = new List<HIS_PATIENT_TYPE>();
                    var patientType = currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ACTIVE == 1).ToList();
                    foreach (var item in patientType)
                    {
                        var itemPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(Base.GlobalStore.HisVServicePatys, WorkPlaceSDO.BranchId, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, intructionTime, this.CurrentTreatment.IN_TIME, serviceId, item.ID, null, 1);
                        if (itemPaty != null)
                        {
                            currentPatientTypeTemps.Add(item);
                        }
                    }
                    //Inventec.Common.Logging.LogSystem.Debug("currentPatientTypeTemps: " + string.Join(",", currentPatientTypeTemps.Select(s => s.PATIENT_TYPE_NAME)));
                    if (currentPatientTypeTemps != null && currentPatientTypeTemps.Count > 0)
                    {
                        if (Base.GlobalStore.IsPrimaryPatientType != commonString__true
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != Base.GlobalStore.PatientTypeId__BHYT
                            && currentPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                        }
                        else if (Base.GlobalStore.IsPrimaryPatientType == "0"
                            && sereServADO.BILL_PATIENT_TYPE_ID.HasValue
                            && patientTypeId != sereServADO.BILL_PATIENT_TYPE_ID.Value
                            && currentPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                        }
                        //else if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == Base.GlobalStore.PatientTypeId__BHYT
                        //&& ((this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) > (intructionTime - (intructionTime % 1000000))
                        //|| (this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0) < (intructionTime - (intructionTime % 1000000))
                        //))
                        else if (this.CurrentTreatment.TDL_PATIENT_TYPE_ID == Base.GlobalStore.PatientTypeId__BHYT
                            && (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.CurrentTreatment.HEIN_CARD_FROM_TIME).Value.Date > Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTime).Value.Date
                            || (BhytExceedDayAllowForInPatient > 0 ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.CurrentTreatment.HEIN_CARD_TO_TIME).Value.AddDays(BhytExceedDayAllowForInPatient).Date : Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.CurrentTreatment.HEIN_CARD_TO_TIME).Value.Date) < Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTime).Value.Date))
                        {
                            result = currentPatientTypeTemps.FirstOrDefault(o => o.ID == Base.GlobalStore.PatientTypeId__VP);
                        }
                        else
                        {
                            result = (currentPatientTypeTemps != null ? (currentPatientTypeTemps.FirstOrDefault(o => o.ID == patientTypeId) ?? currentPatientTypeTemps[0]) : null);
                        }

                        if (result != null && sereServADO != null)
                        {
                            sereServADO.PATIENT_TYPE_ID = result.ID;
                            sereServADO.PATIENT_TYPE_CODE = result.PATIENT_TYPE_CODE;
                            sereServADO.PATIENT_TYPE_NAME = result.PATIENT_TYPE_NAME;
                        }
                        if (Base.GlobalStore.IsPrimaryPatientType == "2")
                        {
                            if (this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID <= 0 || result.ID == this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID)
                            {
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = null;
                            }
                            else
                            {
                                if (currentPatientTypeTemps.Exists(e => e.ID == this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID))
                                {
                                    var priPaty = currentPatientTypeTemps.FirstOrDefault(o => o.ID == this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID);
                                    sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                                }
                                else
                                {
                                    try
                                    {
                                        var billPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID);
                                        string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                                        sereServADO.ErrorMessagePrimaryPatientTypeId = String.Format(ResourceMessage.DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia, patyName);
                                        sereServADO.ErrorTypePrimaryPatientTypeId = ErrorType.Warning;
                                    }
                                    catch (Exception ex)
                                    {
                                        Inventec.Common.Logging.LogSystem.Error(ex);
                                    }
                                }
                            }
                        }
                        else if (Base.GlobalStore.IsPrimaryPatientType == commonString__true
                            && sereServADO.BILL_PATIENT_TYPE_ID.HasValue
                            && sereServADO.PATIENT_TYPE_ID != sereServADO.BILL_PATIENT_TYPE_ID.Value
                            && sereServADO.IsContainAppliedPatientType)
                        {
                            if (currentPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value))
                            {
                                var priPaty = currentPatientTypeTemps.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                            }
                            else
                            {
                                try
                                {
                                    var billPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                                    string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                                    sereServADO.ErrorMessagePatientTypeId = String.Format(ResourceMessage.DichVuCoDTTTBatBuocNhungKhongCoChinhSachGia, patyName);
                                    sereServADO.ErrorTypePatientTypeId = ErrorType.Warning;
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            }
                        }
                        else if (Base.GlobalStore.IsPrimaryPatientType == commonString__true
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != Base.GlobalStore.PatientTypeId__BHYT
                            && currentPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                            && result.ID != this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                        {
                            var priPaty = currentPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                        }
                        else
                        {
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCboRequestUser(Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboRequestUser)
        {
            try
            {
                var hisEmployees = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if(Base.GlobalStore.RequserMustHaveDiploma == "1")
                {
                    hisEmployees = hisEmployees.Where(o => o.DIPLOMA != null).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboRequestUser, hisEmployees, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCboRequestUser()
        {
            try
            {
                var hisEmployees = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (Base.GlobalStore.RequserMustHaveDiploma == "1")
                {
                    hisEmployees = hisEmployees.Where(o => o.DIPLOMA != null).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(repositoryItemCboRequestUser, hisEmployees, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboPatientType(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit patientTypeCombo, List<HIS_PATIENT_TYPE> data)
        {
            try
            {
                patientTypeCombo.DataSource = data;
                patientTypeCombo.DisplayMember = "PATIENT_TYPE_NAME";
                patientTypeCombo.ValueMember = "ID";

                patientTypeCombo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                patientTypeCombo.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                patientTypeCombo.ImmediatePopup = true;
                patientTypeCombo.View.Columns.Clear();

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = patientTypeCombo.View.Columns.AddField("PATIENT_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = patientTypeCombo.View.Columns.AddField("PATIENT_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataCboOtherPaySource()
        {
            try
            {
                var otherPaySource = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>().Where(o => o.IS_ACTIVE == 1).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("OTHER_PAY_SOURCE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("OTHER_PAY_SOURCE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("OTHER_PAY_SOURCE_NAME", "ID", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(repositoryItemCboOtherPaySource, otherPaySource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoOtherPaySourceCombo(HisBedServiceTypeADO data, GridLookUpEdit otherPaySourceCombo)
        {
            try
            {
                bool hasOtherSource = false;
                List<HIS_OTHER_PAY_SOURCE> dataCombo = GetOtherPaySourceByPatientTypeId(data.PATIENT_TYPE_ID ?? 0, ref hasOtherSource);

                this.InitComboOtherPaySource(otherPaySourceCombo, dataCombo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboOtherPaySource(GridLookUpEdit cboPatientType, List<HIS_OTHER_PAY_SOURCE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("OTHER_PAY_SOURCE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("OTHER_PAY_SOURCE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("OTHER_PAY_SOURCE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, data, controlEditorADO);
                controlEditorADO.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_OTHER_PAY_SOURCE> GetOtherPaySourceByPatientTypeId(long patientTypeid, ref bool typeHasOtherSource)
        {
            List<HIS_OTHER_PAY_SOURCE> result = null;
            try
            {
                var otherPaySource = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>().Where(o => o.IS_ACTIVE == 1).ToList();
                if (this.currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0 && otherPaySource != null && otherPaySource.Count > 0)
                {
                    result = new List<HIS_OTHER_PAY_SOURCE>();
                    var patientType = this.currentPatientTypeWithPatientTypeAlter.FirstOrDefault(o => o.ID == patientTypeid);
                    if (patientType != null)
                    {
                        if (!String.IsNullOrWhiteSpace(patientType.OTHER_PAY_SOURCE_IDS))
                        {
                            typeHasOtherSource = true;
                            string[] ids = patientType.OTHER_PAY_SOURCE_IDS.Split(',');
                            List<HIS_OTHER_PAY_SOURCE> otherSource = otherPaySource.Where(o => ids.Contains(o.ID.ToString())).ToList();
                            if (otherSource != null && otherSource.Count > 0)
                            {
                                result.AddRange(otherSource);
                            }
                        }
                        else
                        {
                            result.AddRange(otherPaySource);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private long? ProcessAutoSetOtherPaySource(HisBedServiceTypeADO bedServiceType)
        {
            long? result = null;
            try
            {
                bool hasOtherSource = false;
                List<HIS_OTHER_PAY_SOURCE> dataCombo = GetOtherPaySourceByPatientTypeId(bedServiceType.PATIENT_TYPE_ID ?? 0, ref hasOtherSource);
                if (dataCombo != null && dataCombo.Count > 0)
                {
                    if (dataCombo.Count == 1 && hasOtherSource)
                    {
                        result = dataCombo.First().ID;
                    }
                    else if (this.CurrentTreatment.OTHER_PAY_SOURCE_ID.HasValue && dataCombo.Exists(o => o.ID == this.CurrentTreatment.OTHER_PAY_SOURCE_ID))
                    {
                        result = this.CurrentTreatment.OTHER_PAY_SOURCE_ID;
                    }
                    else
                    {
                        var service = _services.FirstOrDefault(o => o.ID == bedServiceType.BED_SERVICE_TYPE_ID);
                        if (service != null && service.OTHER_PAY_SOURCE_ID.HasValue)
                        {
                            if (String.IsNullOrWhiteSpace(service.OTHER_PAY_SOURCE_ICDS))
                            {
                                result = service.OTHER_PAY_SOURCE_ID;
                            }
                            else
                            {
                                List<string> icds = service.OTHER_PAY_SOURCE_ICDS.Split(',').ToList();
                                if (icds.Exists(o => o.ToLower() == (this.CurrentTreatment.ICD_CODE ?? "").ToLower()))
                                {
                                    result = service.OTHER_PAY_SOURCE_ID;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
    }
}
