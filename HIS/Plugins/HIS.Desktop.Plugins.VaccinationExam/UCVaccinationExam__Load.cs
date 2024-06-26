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
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.VaccinationExam.ADO;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.VaccinationExam.Base;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.VaccinationExam
{
    public partial class UCVaccinationExam : UserControlBase
    {
        internal void FillDataToGridVaccinationExam()
        {
            try
            {
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>(AppConfigKey.CONFIG_KEY__NUM_PAGESIZE);
                }

                FillDataToGridVaccinationExam(new CommonParam(0, (int)numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridVaccinationExam, param, numPageSize, gridControlVaccinationExam);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void FillDataToGridVaccinationExam(object param)
        {
            try
            {
                WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                numPageSize = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<V_HIS_VACCINATION_EXAM>> apiResult = new ApiResultObject<List<V_HIS_VACCINATION_EXAM>>();
                HisVaccinationExamViewFilter hisVaccinationExamFilter = new HisVaccinationExamViewFilter();
                hisVaccinationExamFilter.EXECUTE_ROOM_ID = roomId;
                hisVaccinationExamFilter.KEY_WORD = txtKeyword.Text.Trim();

                if (dtRequestTimeFrom != null && dtRequestTimeFrom.DateTime != DateTime.MinValue)
                    hisVaccinationExamFilter.REQUEST_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtRequestTimeFrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (dtRequestTimeTo != null && dtRequestTimeTo.DateTime != DateTime.MinValue)
                    hisVaccinationExamFilter.REQUEST_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtRequestTimeTo.EditValue).ToString("yyyyMMddHHmm") + "59");

                //Tat ca : 0
                //Chua xu ly : 1
                //Dang xu ly : 2
                //Ket thuc : 3
                switch (cboStatus.SelectedIndex)
                {
                    case 0:
                        hisVaccinationExamFilter.VACCINATION_EXAM_STT_ID = null;
                        break;
                    case 1:
                        hisVaccinationExamFilter.VACCINATION_EXAM_STT_ID = 1;
                        break;
                    case 2:
                        hisVaccinationExamFilter.VACCINATION_EXAM_STT_ID = 2;
                        break;
                    case 3:
                        hisVaccinationExamFilter.VACCINATION_EXAM_STT_ID = 3;
                        break;
                }

                hisVaccinationExamFilter.ORDER_FIELD = "REQUEST_TIME";
                hisVaccinationExamFilter.ORDER_DIRECTION = "DESC";
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("hisVaccinationExamFilter", hisVaccinationExamFilter));
                apiResult = new BackendAdapter(paramCommon)
                    .GetRO<List<V_HIS_VACCINATION_EXAM>>("api/HisVaccinationExam/GetView", ApiConsumers.MosConsumer, hisVaccinationExamFilter, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("apiResult", apiResult));

                gridControlVaccinationExam.DataSource = null;
                if (apiResult != null)
                {
                    rowCount = (apiResult.Data == null ? 0 : apiResult.Data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    gridControlVaccinationExam.DataSource = apiResult.Data;
                }

                gridViewVaccinationExam.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewVaccinationExam.OptionsSelection.EnableAppearanceFocusedRow = true;

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void LoadVaccExamResullt(V_HIS_VACCINATION_EXAM vaccinationExam)
        {
            try
            {
                if (this.vaccExamResults == null || this.vaccExamResults.Count == 0)
                {
                    LoadDataVaccExamResults();
                }
                List<HIS_VACC_EXAM_RESULT> listData = this.vaccExamResults != null ? this.vaccExamResults.Where(o => o.IS_BABY == null).ToList() : null;
                if (vaccinationExam != null && this.vaccExamResults != null)
                {
                    var requestDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(vaccinationExam.REQUEST_DATE);
                    var dob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(vaccinationExam.TDL_PATIENT_DOB);
                    if (requestDate != null && dob != null)
                    {
                        var resultDate = requestDate - dob.Value.Date;
                        if (resultDate.Value.Days + 1 < 30)
                        {
                            listData = this.vaccExamResults.Where(o => o.IS_BABY == 1).ToList();
                        }
                    }
                }
                List<VaccExamResultADO> listADO = new List<VaccExamResultADO>();
                if (listData != null)
                {
                    foreach (var item in listData)
                    {
                        var ado = new VaccExamResultADO(item);
                        listADO.Add(ado);
                    }
                }
                listADO = listADO.OrderBy(o => o.VACC_EXAM_RESULT_CODE).ToList();

                gridControlVaccExamResult.DataSource = listADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataVaccExamResults()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisVaccExamResultFilter filter = new HisVaccExamResultFilter();
                this.vaccExamResults = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_VACC_EXAM_RESULT>>("api/HisVaccExamResult/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDefault()
        {
            try
            {
                dtRequestTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtRequestTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                dtExecuteTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                cboStatus.SelectedIndex = 0;

                string currentLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                txtLoginname.Text = currentLoginname;
                cboAcsUser.EditValue = currentLoginname;
                cboAcsUser.Properties.Buttons[1].Visible = true;
                mmNote.Text = "";
                mmPtAllergicHistory.Text = "";
                mmPtPathologicalHistory.Text = "";
                mmAppointmentAdvise.Text = "";
                dtAppointmentTime.EditValue = null;

                //Tab1
                radioGroupTestHbsAg.SelectedIndex = 0;
                radioGroupPositiveResult.SelectedIndex = -1;
                radioGroupPositiveResult.Enabled = false;
                //Tab2
                radioGroupSpecialistExam.SelectedIndex = 0;
                txtSpecialistDepartment.Enabled = false;
                cboSpecialistDepartment.Enabled = false;
                txtSpecialistReason.Enabled = false;
                txtSpecialistResult.Enabled = false;
                txtSpecialistConclude.Enabled = false;
                //Tab3
                spRabiesNumberOfDays.EditValue = null;
                radioGroupRabiesWoundRank.SelectedIndex = -1;
                radioGroupRabiesAnimalStatus.SelectedIndex = -1;

                InitExpMestMedicineGrid();
                InitAppointmentVaccGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataFromVaccinationExam(V_HIS_VACCINATION_EXAM vaccinationExam)
        {
            try
            {
                if (vaccinationExam != null)
                {
                    WaitingManager.Show();
                    LoadVaccExamResullt(vaccinationExam);
                    btnSave.Enabled = true;
                    if (vaccinationExam.VACCINATION_EXAM_STT_ID == 3)
                    {
                        btnSave.Enabled = false;
                        btnKetThuc.Enabled = false;
                        btnAssignee.Enabled = false;
                        btnNew.Enabled = false;
                    }
                    else
                    {
                        btnSave.Enabled = true;
                        btnKetThuc.Enabled = true;
                        btnAssignee.Enabled = true;
                        btnNew.Enabled = true;
                    }

                    //Khám sàng lọc
                    if (vaccinationExam.IS_TEST_HBSAG != null && vaccinationExam.IS_TEST_HBSAG == 1)
                    {
                        radioGroupTestHbsAg.SelectedIndex = 1;
                    }else
	                {
                        radioGroupTestHbsAg.SelectedIndex = 0;
	                }
                    if (vaccinationExam.IS_POSITIVE_RESULT == null)
                        radioGroupPositiveResult.SelectedIndex = -1;
                    else if (vaccinationExam.IS_POSITIVE_RESULT == 0)
                        radioGroupPositiveResult.SelectedIndex = 1;//Âm tính
                    else if (vaccinationExam.IS_POSITIVE_RESULT == 1)
                        radioGroupPositiveResult.SelectedIndex = 0;//Dương tính
                    mmNote.Text = vaccinationExam.NOTE;
                    mmPtPathologicalHistory.Text = vaccinationExam.PT_PATHOLOGICAL_HISTORY;
                    mmPtAllergicHistory.Text = vaccinationExam.PT_ALLERGIC_HISTORY;
                    radioGroupConclude.SelectedIndex = (int)(vaccinationExam.CONCLUDE ?? 0);
                    if (!String.IsNullOrEmpty(vaccinationExam.EXECUTE_LOGINNAME))
                    {
                        txtLoginname.Text = vaccinationExam.EXECUTE_LOGINNAME;
                        cboAcsUser.EditValue = vaccinationExam.EXECUTE_LOGINNAME;
                        cboAcsUser.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        string currentLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        txtLoginname.Text = currentLoginname;
                        cboAcsUser.EditValue = currentLoginname;
                    }
                    //Khám sàng lọc chuyên khoa
                    if (vaccinationExam.IS_SPECIALIST_EXAM != null && vaccinationExam.IS_SPECIALIST_EXAM == 1)
                    {
                        radioGroupSpecialistExam.SelectedIndex = 1;
                    }
                    else
                    {
                        radioGroupSpecialistExam.SelectedIndex = 0;
                    }
                    cboSpecialistDepartment.EditValue = vaccinationExam.SPECIALIST_DEPARTMENT_ID;
                    txtSpecialistReason.Text = vaccinationExam.SPECIALIST_REASON;
                    txtSpecialistResult.Text = vaccinationExam.SPECIALIST_RESULT;
                    txtSpecialistConclude.Text = vaccinationExam.SPECIALIST_CONCLUDE;
                    //Khai báo theo dõi dại
                    spRabiesNumberOfDays.EditValue = vaccinationExam.RABIES_NUMBER_OF_DAYS;
                    chkRabiesAnimalDog.Checked = vaccinationExam.RABIES_ANIMAL_DOG == 1;
                    chkRabiesAnimalCat.Checked = vaccinationExam.RABIES_ANIMAL_CAT == 1;
                    chkRabiesAnimalBat.Checked = vaccinationExam.RABIES_ANIMAL_BAT == 1;
                    chkRabiesAnimalOther.Checked = vaccinationExam.RABIES_ANIMAL_OTHER == 1;
                    chkRabiesWoundLocationFace.Checked = vaccinationExam.RABIES_WOUND_LOCATION_FACE == 1;
                    chkRabiesWoundLocationFoot.Checked = vaccinationExam.RABIES_WOUND_LOCATION_FOOT == 1;
                    chkRabiesWoundLocationHand.Checked = vaccinationExam.RABIES_WOUND_LOCATION_HAND == 1;
                    chkRabiesWoundLocationHead.Checked = vaccinationExam.RABIES_WOUND_LOCATION_HEAD == 1;
                    chkRabiesWoundLocationNeck.Checked = vaccinationExam.RABIES_WOUND_LOCATION_NECK == 1;

                    if (vaccinationExam.RABIES_WOUND_RANK == 1)
                        radioGroupRabiesWoundRank.SelectedIndex = 0;
                    else if (vaccinationExam.RABIES_WOUND_RANK == 2)
                        radioGroupRabiesWoundRank.SelectedIndex = 1;
                    else if (vaccinationExam.RABIES_WOUND_RANK == 3)
                        radioGroupRabiesWoundRank.SelectedIndex = 2;
                    else
                        radioGroupRabiesWoundRank.SelectedIndex = -1;

                    if (vaccinationExam.RABIES_ANIMAL_STATUS == 1)
                        radioGroupRabiesAnimalStatus.SelectedIndex = 0;
                    else if (vaccinationExam.RABIES_ANIMAL_STATUS == 2)
                        radioGroupRabiesAnimalStatus.SelectedIndex = 1;
                    else if (vaccinationExam.RABIES_ANIMAL_STATUS == 3)
                        radioGroupRabiesAnimalStatus.SelectedIndex = 2;
                    else
                        radioGroupRabiesAnimalStatus.SelectedIndex = -1;

                    LoadVaexVaer(vaccinationExam);
                    LoadVaccinationMety(vaccinationExam);
                    LoadDataAppointment(vaccinationExam);

                    //không có thông tin sẽ lấy từ patient
                    if (String.IsNullOrWhiteSpace(vaccinationExam.PT_ALLERGIC_HISTORY))
                    {
                        CommonParam paparam = new CommonParam();
                        HisPatientFilter pafilter = new HisPatientFilter();
                        pafilter.ID = vaccinationExam.PATIENT_ID;
                        List<HIS_PATIENT> paResult = new BackendAdapter(paparam).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, pafilter, paparam);
                        if (paResult != null && paResult.Count > 0)
                        {
                            mmPtAllergicHistory.Text = paResult.FirstOrDefault().PT_ALLERGIC_HISTORY;
                        }
                    }

                    // fill data to DHST
                    CommonParam param = new CommonParam();
                    HisDhstFilter filter = new HisDhstFilter();
                    filter.VACCINATION_EXAM_ID = vaccinationExam.ID;
                    List<HIS_DHST> dhst = new BackendAdapter(param).Get<List<HIS_DHST>>("api/HisDHST/Get", ApiConsumers.MosConsumer, filter, param);
                    HIS_DHST currentDhst = dhst != null ? dhst.FirstOrDefault() : null;
                    DHSTSetValue(currentDhst);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DHSTSetValue(HIS_DHST dhst)
        {
            try
            {
                if (dhst != null)
                {
                    idDhst = dhst.ID;

                    if (dhst.EXECUTE_TIME != null)
                        dtExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dhst.EXECUTE_TIME ?? 0) ?? DateTime.Now;
                    else
                        dtExecuteTime.EditValue = DateTime.Now;
                    spinBloodPressureMax.EditValue = dhst.BLOOD_PRESSURE_MAX;
                    spinBloodPressureMin.EditValue = dhst.BLOOD_PRESSURE_MIN;
                    spinBreathRate.EditValue = dhst.BREATH_RATE;
                    spinHeight.EditValue = dhst.HEIGHT;
                    spinChest.EditValue = dhst.CHEST;
                    spinBelly.EditValue = dhst.BELLY;
                    spinPulse.EditValue = dhst.PULSE;
                    spinTemperature.EditValue = dhst.TEMPERATURE;
                    spinWeight.EditValue = dhst.WEIGHT;
                    if (dhst.SPO2.HasValue)
                        spinSPO2.Value = (dhst.SPO2.Value * 100);
                    else
                        spinSPO2.EditValue = null;
                    txtNote.Text = dhst.NOTE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadVaccinationMety(V_HIS_VACCINATION_EXAM vaccinationExam)
        {
            try
            {
                if (vaccinationExam != null && vaccinationExam.CONCLUDE == 1 && vaccinationExam.VACCINATION_EXAM_STT_ID != 3)
                {
                    btnAssignee.Enabled = true;
                    btnAppointment.Enabled = true;
                }
                else
                {
                    btnAssignee.Enabled = false;
                    btnAppointment.Enabled = false;
                }

                //Load lich su tiem de load thong tin tiem hien tai
                LoadVaccinationMetyByPatient(vaccinationExam.PATIENT_ID);
                LoadOldVaction(vaccinationExam);
                //LoadVaccinatitonMety(vaccinationExam);
                EnabledComboMediStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitExpMestMedicineGrid()
        {
            try
            {
                //Khoi tao mui tiem
                gridControlVaccinationMety.DataSource = null;
                List<ExpMestMedicineADO> vaccinantionMetys = new List<ExpMestMedicineADO>();
                ExpMestMedicineADO vaccinantionMety = new ExpMestMedicineADO();
                vaccinantionMety.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                vaccinantionMety.VACCINE_TURN = 1;
                vaccinantionMety.AMOUNT = 1;
                if (vaccinationExam != null)
                {
                    vaccinantionMety.PATIENT_TYPE_ID = vaccinationExam.PATIENT_TYPE_ID;
                }
                vaccinantionMetys.Add(vaccinantionMety);
                gridControlVaccinationMety.DataSource = vaccinantionMetys;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitAppointmentVaccGrid()
        {
            try
            {
                //Khoi tao mui tiem
                gridControlAppointmentVacc.DataSource = null;
                List<AppointmentVaccineADO> appointmentVaccines = new List<AppointmentVaccineADO>();
                appointmentVaccines.Add(new AppointmentVaccineADO { Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd });
                gridControlAppointmentVacc.DataSource = appointmentVaccines;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadVaexVaer(V_HIS_VACCINATION_EXAM vaccinationExam)
        {
            try
            {
                var listData = (gridControlVaccExamResult.DataSource as IEnumerable<VaccExamResultADO>).ToList();
                if (listData == null || listData .Count == 0)
	                return;

                gridViewVaccExamResult.BeginUpdate();
                gridViewVaccExamResult.ClearSelection();

                CommonParam param = new CommonParam();
                HisVaexVaerFilter filter = new HisVaexVaerFilter();
                filter.VACCINATION_EXAM_ID = vaccinationExam.ID;
                List<HIS_VAEX_VAER> vaexVaers = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_VAEX_VAER>>("api/HisVaexVaer/Get", ApiConsumers.MosConsumer, filter, param);
                
                if (vaexVaers != null && vaexVaers.Count > 0)
                {
                    //Fill NOTE data
                    foreach (var item in vaexVaers)
                    {
                        var index = listData.FindIndex(o => o.ID == item.VACC_EXAM_RESULT_ID);
                        var data = listData.Find(o => o.ID == item.VACC_EXAM_RESULT_ID);
                        data.Note = item.NOTE;
                        if (index != -1 && index > 0)
                            listData[index] = data;
                    }
                    gridControlVaccExamResult.DataSource = listData;
                    gridControlVaccExamResult.RefreshDataSource();

                    //Select to grid
                    foreach (var item in vaexVaers)
                    {
                        int rowHandle = gridViewVaccExamResult.LocateByValue("ID", item.VACC_EXAM_RESULT_ID);
                        if (rowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                        {
                            gridViewVaccExamResult.SelectRow(rowHandle);
                        }
                    }
                }
                gridViewVaccExamResult.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadVaccinationMetyByPatient(long? patientId)
        {
            try
            {
                gridControlExpMestMedicineByPatient.DataSource = null;
                if (patientId.HasValue)
                {
                    HisExpMestMedicineView5Filter filter = new HisExpMestMedicineView5Filter();
                    filter.PATIENT_ID = patientId;
                    expMestMedicine5s = new BackendAdapter(new CommonParam())
                        .Get<List<V_HIS_EXP_MEST_MEDICINE_5>>("api/HisExpMestMedicine/GetView5",
                        ApiConsumers.MosConsumer, filter, new CommonParam());

                    if (expMestMedicine5s != null && expMestMedicine5s.Count > 0)
                    {
                        List<ExpMestMedicineADO> expMestMedicineADOs = new List<ExpMestMedicineADO>();
                        var expMestMedicineGroup = expMestMedicine5s.Where(o => o.EXECUTE_TIME.HasValue)
                            .GroupBy(o => o.TDL_MEDICINE_TYPE_ID);
                        foreach (var g in expMestMedicineGroup)
                        {
                            int index = 1;
                            foreach (var item in g)
                            {
                                ExpMestMedicineADO ado = new ExpMestMedicineADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineADO>(ado, item);
                                ado.AMOUNT = item.AMOUNT;
                                ado.VACCINE_TURN = item.VACCINE_TURN;
                                ado.TDL_MEDICINE_TYPE_ID = item.TDL_MEDICINE_TYPE_ID;
                                ado.PATIENT_TYPE_ID = item.PATIENT_TYPE_ID;
                                ado.VaccinationIndex = index;
                                expMestMedicineADOs.Add(ado);
                                index++;
                            }
                        }
                        gridControlExpMestMedicineByPatient.DataSource = expMestMedicineADOs;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadVaccinatitonMety(V_HIS_VACCINATION_EXAM vaccinationExam)
        {
            try
            {
                if (vaccinationExam == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong timm thay vaccinationExam");
                    return;
                }

                dtRequestTime.DateTime = DateTime.Now;
                actionAssign = EnumUtil.ACTION.CREATE;

                var expMestMedicineTempGroups = expMestMedicine5s != null
                    ? expMestMedicine5s.Where(o => o.VACCINATION_EXAM_ID == vaccinationExam.ID)
                    .GroupBy(o => new { o.TDL_VACCINATION_ID, o.TDL_MEDICINE_TYPE_ID }).ToList() : null;

                if (expMestMedicineTempGroups != null && expMestMedicineTempGroups.Count > 0)
                {
                    V_HIS_EXP_MEST_MEDICINE_5 expMestMedicine5 = expMestMedicineTempGroups.First().First();
                    actionAssign = EnumUtil.ACTION.UPDATE;
                    V_HIS_MEDI_STOCK mediStock = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDI_STOCK>()
                        .FirstOrDefault(o => o.ID == expMestMedicine5.TDL_MEDI_STOCK_ID);
                    if (mediStock != null)
                    {
                        cboMediStockName.EditValue = mediStock.ID;
                        txtMediStockCode.Text = mediStock.MEDI_STOCK_CODE;
                    }

                    if (expMestMedicine5.PATIENT_TYPE_ID.HasValue)
                    {
                        cboPatientType.EditValue = expMestMedicine5.PATIENT_TYPE_ID;
                    }

                    List<ExpMestMedicineADO> expMestMedicineADOs = new List<ExpMestMedicineADO>();
                    this.VaccinationResult.Medicines = new List<V_HIS_EXP_MEST_MEDICINE>();

                    List<long> vaccinationIds = expMestMedicineTempGroups.Select(s => s.First().TDL_VACCINATION_ID ?? 0).ToList();
                    if (vaccinationIds != null && vaccinationIds.Count > 0)
                    {
                        CommonParam paparam = new CommonParam();
                        HisVaccinationViewFilter vaccFilter = new HisVaccinationViewFilter();
                        vaccFilter.IDs = vaccinationIds.Distinct().ToList();
                        this.VaccinationResult.Vaccinations = new BackendAdapter(paparam).Get<List<V_HIS_VACCINATION>>("api/HisVaccination/GetView", ApiConsumers.MosConsumer, vaccFilter, paparam);
                    }

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("LoadVaccinatitonMety: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => expMestMedicineTempGroups), expMestMedicineTempGroups));
                    foreach (var g in expMestMedicineTempGroups)
                    {
                        ExpMestMedicineADO ado = new ExpMestMedicineADO();
                        ado.TDL_MEDICINE_TYPE_ID = g.First().TDL_MEDICINE_TYPE_ID;
                        ado.AMOUNT = g.Sum(o => o.AMOUNT);
                        ado.TDL_VACCINATION_ID = g.First().TDL_VACCINATION_ID;
                        if (expMestMedicineTempGroups.IndexOf(g) == 0)
                            ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        else
                            ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;

                        V_HIS_EXP_MEST_MEDICINE mety = new V_HIS_EXP_MEST_MEDICINE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MEDICINE>(mety, g.First());
                        ado.AMOUNT = g.Sum(o => o.AMOUNT);
                        this.VaccinationResult.Medicines.Add(mety);

                        expMestMedicineADOs.Add(ado);
                    }

                    gridControlVaccinationMety.DataSource = expMestMedicineADOs;
                    dtRequestTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(expMestMedicine5.REQUEST_TIME) ?? DateTime.Now;
                }
                else
                {
                    cboMediStockName.EditValue = null;
                    txtMediStockCode.Text = "";
                    //Lay doi tuong hien tai cua benh nhan
                    cboPatientType.EditValue = vaccinationExam.PATIENT_TYPE_ID;
                    InitExpMestMedicineGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadOldVaction(V_HIS_VACCINATION_EXAM vaccinationExam)
		{
			try
			{
                if (vaccinationExam == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong timm thay vaccinationExam");
                    return;
                }
                ResetAssign();

                var expMestMedicineTempGroups = expMestMedicine5s != null
                  ? expMestMedicine5s.Where(o => o.VACCINATION_EXAM_ID == vaccinationExam.ID)
                  .GroupBy(o => new { o.TDL_VACCINATION_ID, o.TDL_MEDICINE_TYPE_ID }).ToList() : null;

                if (expMestMedicineTempGroups != null && expMestMedicineTempGroups.Count > 0)
                {
                    List<long> vaccinationIds = expMestMedicineTempGroups.Select(s => s.First().TDL_VACCINATION_ID ?? 0).ToList();
                    if (vaccinationIds != null && vaccinationIds.Count > 0)
                    {
                        CommonParam paparam = new CommonParam();
                        HisVaccinationViewFilter vaccFilter = new HisVaccinationViewFilter();
                        vaccFilter.IDs = vaccinationIds.Distinct().ToList();
                        lstVaccination = new BackendAdapter(paparam).Get<List<V_HIS_VACCINATION>>("api/HisVaccination/GetView", ApiConsumers.MosConsumer, vaccFilter, paparam);
                    }
                    if(lstVaccination!=null && lstVaccination.Count>0)
					{
                        gridControl1.DataSource = lstVaccination;
					}
				}
				else
				{
                    gridControl1.DataSource = null;
                }

            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}

        private V_HIS_VACCINATION_EXAM GetVaccinationExamById(long id)
        {
            V_HIS_VACCINATION_EXAM result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisVaccinationExamViewFilter filter = new HisVaccinationExamViewFilter();
                filter.ID = id;
                List<V_HIS_VACCINATION_EXAM> vaccinationExams = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_VACCINATION_EXAM>>("api/HisVaccinationExam/GetView", ApiConsumers.MosConsumer, filter, param);
                if (vaccinationExams != null && vaccinationExams.Count == 1)
                {
                    result = vaccinationExams[0];
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void EnabledComboMediStock()
        {
            try
            {
                if (actionAssign == EnumUtil.ACTION.CREATE)
                {
                    txtMediStockCode.Enabled = true;
                    cboMediStockName.Enabled = true;
                }
                else if (actionAssign == EnumUtil.ACTION.UPDATE)
                {
                    txtMediStockCode.Enabled = false;
                    cboMediStockName.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.CurrentModule.ModuleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPrintAssign.Name)
                        {
                            chkPrintAssign.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkPrintAppointment.Name)
                        {
                            chkPrintAppointment.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void LoadDataAppointment(V_HIS_VACCINATION_EXAM vaccinationExam)
        {
            try
            {
                mmAppointmentAdvise.Text = "";
                dtAppointmentTime.EditValue = null;
                InitAppointmentVaccGrid();
                btnPrintAppointment.Enabled = false;

                if (vaccinationExam != null)
                {
                    mmAppointmentAdvise.Text = vaccinationExam.ADVISE;
                    dtAppointmentTime.EditValue = vaccinationExam.APPOINTMENT_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(vaccinationExam.APPOINTMENT_TIME ?? 0) : null;

                    CommonParam param = new CommonParam();
                    HisVaccAppointmentViewFilter filter = new HisVaccAppointmentViewFilter();
                    filter.VACCINATION_EXAM_ID = vaccinationExam.ID;
                    this.VaccAppointmentResult = new BackendAdapter(param).Get<List<V_HIS_VACC_APPOINTMENT>>("api/HisVaccAppointment/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (this.VaccAppointmentResult != null && this.VaccAppointmentResult.Count > 0)
                    {
                        List<AppointmentVaccineADO> appointmentVaccineADOs = new List<AppointmentVaccineADO>();

                        foreach (var g in this.VaccAppointmentResult)
                        {
                            AppointmentVaccineADO ado = new AppointmentVaccineADO();
                            ado.VACCINE_TYPE_ID = g.VACCINE_TYPE_ID;
                            ado.VACCINE_TURN = g.VACCINE_TURN ?? 0;
                            if (this.VaccAppointmentResult.IndexOf(g) == 0)
                                ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            else
                                ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                            appointmentVaccineADOs.Add(ado);
                        }

                        btnPrintAppointment.Enabled = true;
                        gridControlAppointmentVacc.DataSource = appointmentVaccineADOs;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
