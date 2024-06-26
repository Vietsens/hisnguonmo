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
using HIS.Desktop.Plugins.VaccinationExam.ADO;
using Inventec.Core;
using DevExpress.XtraEditors;
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.VaccinationExam
{
    public partial class UCVaccinationExam : UserControlBase
    {
        private List<HisVaexVaerSDO> GetVaccExamResult()
        {
            List<HisVaexVaerSDO> result = new List<HisVaexVaerSDO>();
            try
            {
                int[] selectRows = gridViewVaccExamResult.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        VaccExamResultADO vaccExam = (VaccExamResultADO)gridViewVaccExamResult.GetRow(selectRows[i]);
                        if (vaccExam != null)
                        {
                            HisVaexVaerSDO addingSDO = new HisVaexVaerSDO();
                            addingSDO.VaccExamResultId = vaccExam.ID;
                            addingSDO.Note = vaccExam.Note;
                            result.Add(addingSDO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void CreateExecuteSDO(ref HisVaccinationExamTreatSDO hisVaccinationExamTreatSDO)
        {
            try
            {
                if (this.vaccinationExam.VACCINATION_EXAM_STT_ID == 1)
                {
                    hisVaccinationExamTreatSDO.VaccinationExamSttId = 2;//Đang xử lý
                }
                hisVaccinationExamTreatSDO.VaccinationExamId = vaccinationExam.ID;
                hisVaccinationExamTreatSDO.VaexVaerInfos = GetVaccExamResult();
                hisVaccinationExamTreatSDO.Note = mmNote.Text;
                hisVaccinationExamTreatSDO.PtAllergicHistory = mmPtAllergicHistory.Text;
                hisVaccinationExamTreatSDO.PtPathologicalHistory = mmPtPathologicalHistory.Text;
                if (radioGroupConclude.SelectedIndex == 1)
                {
                    hisVaccinationExamTreatSDO.Conclude = VaccinationExamConcludeEnum.OK;
                }
                else if (radioGroupConclude.SelectedIndex == 2)
                {
                    hisVaccinationExamTreatSDO.Conclude = VaccinationExamConcludeEnum.NOK;
                }
                hisVaccinationExamTreatSDO.WorkingRoomId = roomId;
                if (cboAcsUser.EditValue != null)
                {
                    hisVaccinationExamTreatSDO.ExecuteLoginname = cboAcsUser.EditValue.ToString();
                    hisVaccinationExamTreatSDO.ExecuteUsername = cboAcsUser.Text;
                }
                //
                if (radioGroupTestHbsAg.SelectedIndex == 1)//Có
	            {
                    hisVaccinationExamTreatSDO.IsTestHBSAG = 1;
                    if (radioGroupPositiveResult.SelectedIndex == 1)//Âm tính
                    {
                        hisVaccinationExamTreatSDO.IsPositiveResult = 0;
                    }
                    else if (radioGroupPositiveResult.SelectedIndex == 0)//Dương tính
                    {
                        hisVaccinationExamTreatSDO.IsPositiveResult = 1;
                    }
                }
                else if (radioGroupTestHbsAg.SelectedIndex == 0)//Không
                {
                    hisVaccinationExamTreatSDO.IsTestHBSAG = 0;
                }
                //Khám sàng lọc chuyên khoa
                if (radioGroupSpecialistExam.SelectedIndex == 1)
                {
                    hisVaccinationExamTreatSDO.IsSpecialistExam = 1;
                    hisVaccinationExamTreatSDO.SpecialistDepartmentId = cboSpecialistDepartment.EditValue != null ? (long?)(Convert.ToInt64(cboSpecialistDepartment.EditValue)) : null;
                    hisVaccinationExamTreatSDO.SpecialistReason = txtSpecialistReason.Text;
                    hisVaccinationExamTreatSDO.SpecialistResult = txtSpecialistResult.Text;
                    hisVaccinationExamTreatSDO.SpecialistConclude = txtSpecialistConclude.Text;
                }
                else if (radioGroupSpecialistExam.SelectedIndex == 0)
                {
                    hisVaccinationExamTreatSDO.IsSpecialistExam = 0;
                }
                //Khai báo theo dõi dại
                hisVaccinationExamTreatSDO.RabiesNumberOfDays = spRabiesNumberOfDays.EditValue != null ? (long?)spRabiesNumberOfDays.Value : null;
                hisVaccinationExamTreatSDO.IsRabiesAnimalDog = chkRabiesAnimalDog.Checked;
                hisVaccinationExamTreatSDO.IsRabiesAnimalCat = chkRabiesAnimalCat.Checked;
                hisVaccinationExamTreatSDO.IsRabiesAnimalBat = chkRabiesAnimalBat.Checked;
                hisVaccinationExamTreatSDO.IsRabiesAnimalOther = chkRabiesAnimalOther.Checked;
                hisVaccinationExamTreatSDO.IsRabiesWoundLocationFace = chkRabiesWoundLocationFace.Checked;
                hisVaccinationExamTreatSDO.IsRabiesWoundLocationFoot = chkRabiesWoundLocationFoot.Checked;
                hisVaccinationExamTreatSDO.IsRabiesWoundLocationHand = chkRabiesWoundLocationHand.Checked;
                hisVaccinationExamTreatSDO.IsRabiesWoundLocationHead = chkRabiesWoundLocationHead.Checked;
                hisVaccinationExamTreatSDO.IsRabiesWoundLocationNeck = chkRabiesWoundLocationNeck.Checked;
                if (radioGroupRabiesWoundRank.SelectedIndex == 0)
                    hisVaccinationExamTreatSDO.RabiesWoundRank = 1;
                else if (radioGroupRabiesWoundRank.SelectedIndex == 1)
                    hisVaccinationExamTreatSDO.RabiesWoundRank = 2;
                else if (radioGroupRabiesWoundRank.SelectedIndex == 2)
                    hisVaccinationExamTreatSDO.RabiesWoundRank = 3;

                if (radioGroupRabiesAnimalStatus.SelectedIndex == 0)
                    hisVaccinationExamTreatSDO.RabiesWoundStatus = 1;
                else if (radioGroupRabiesAnimalStatus.SelectedIndex == 1)
                    hisVaccinationExamTreatSDO.RabiesWoundStatus = 2;
                else if (radioGroupRabiesAnimalStatus.SelectedIndex == 2)
                    hisVaccinationExamTreatSDO.RabiesWoundStatus = 3;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateAssigneeSDO(ref HisVaccinationAssignSDO hisVaccinationAssignSDO)
        {
            try
            {
                hisVaccinationAssignSDO.RequestLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                hisVaccinationAssignSDO.RequestUsername = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                hisVaccinationAssignSDO.VaccinationExamId = vaccinationExam.ID;
                long? vaccinationId = null;
                hisVaccinationAssignSDO.VaccinationMeties = GetVaccinationMety(ref vaccinationId);
                hisVaccinationAssignSDO.WorkingRoomId = roomId;
                hisVaccinationAssignSDO.VaccinationId = vaccinationId;
                hisVaccinationAssignSDO.RequestTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtRequestTime.DateTime) ?? 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<VaccinationMetySDO> GetVaccinationMety(ref long? vaccinationId)
        {
            List<VaccinationMetySDO> result = new List<VaccinationMetySDO>();
            try
            {
                List<ExpMestMedicineADO> vaccinationMetyADOs = gridControlVaccinationMety.DataSource as List<ExpMestMedicineADO>;
                if (vaccinationMetyADOs != null && vaccinationMetyADOs.Count > 0)
                {
                    foreach (var item in vaccinationMetyADOs)
                    {
                        VaccinationMetySDO sdo = new VaccinationMetySDO();
                        sdo.Amount = item.AMOUNT;
                        sdo.VaccineTurn = item.VACCINE_TURN;
                        sdo.MediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStockName.EditValue.ToString());
                        sdo.MedicineTypeId = item.TDL_MEDICINE_TYPE_ID ?? 0;
                        sdo.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                        result.Add(sdo);
                    }
                    Inventec.Common.Logging.LogSystem.Info("GetVaccinationMety1: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => vaccinationMetyADOs), vaccinationMetyADOs));
                    ExpMestMedicineADO expMestMedicine = vaccinationMetyADOs.FirstOrDefault(o => o.TDL_VACCINATION_ID.HasValue);
                    Inventec.Common.Logging.LogSystem.Info("GetVaccinationMety2: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => expMestMedicine), expMestMedicine));
                    if (expMestMedicine != null)
                    {
                        vaccinationId = expMestMedicine.TDL_VACCINATION_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void CreateAppointmentSDO(ref HisVaccinationAppointmentSDO hisVaccinationAppointmentSDO)
        {
            try
            {
                hisVaccinationAppointmentSDO.Advise = mmAppointmentAdvise.Text;
                hisVaccinationAppointmentSDO.AppointmentTime = Inventec.Common.TypeConvert.Parse.ToInt64(dtAppointmentTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                hisVaccinationAppointmentSDO.Details = GetVaccAppointment();
                hisVaccinationAppointmentSDO.RequestRoomId = this.roomId;
                hisVaccinationAppointmentSDO.VaccinationExamId = vaccinationExam.ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<VaccAppointmentDetail> GetVaccAppointment()
        {
            List<VaccAppointmentDetail> result = null;
            try
            {
                List<AppointmentVaccineADO> vaccinationMetyADOs = gridControlAppointmentVacc.DataSource as List<AppointmentVaccineADO>;
                if (vaccinationMetyADOs != null && vaccinationMetyADOs.Count > 0)
                {
                    result = new List<VaccAppointmentDetail>();
                    foreach (var item in vaccinationMetyADOs)
                    {
                        VaccAppointmentDetail sdo = new VaccAppointmentDetail();
                        sdo.VaccineTurn = item.VACCINE_TURN ?? 0;
                        sdo.VaccineTypeId = item.VACCINE_TYPE_ID ?? 0;
                        result.Add(sdo);
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
