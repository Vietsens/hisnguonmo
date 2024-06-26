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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.UpdatePatientExt.Ado;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.UpdatePatientExt
{
    public partial class frmUpdatePatientExt : Form
    {
        public void LoadDataToControl()
        {
            try
            {
                if (patientADO != null)
                {
                    spinBornWeight.EditValue = patientADO.BORN_WEIGHT.HasValue ? patientADO.BORN_WEIGHT : null;
                    spinBornHeight.EditValue = patientADO.BORN_HEIGHT.HasValue ? patientADO.BORN_HEIGHT : null;
                    chkIS_BORN_INADEQUACY.CheckState = patientADO.IS_BORN_INADEQUACY.HasValue && patientADO.IS_BORN_INADEQUACY.Value == 1
                        ? CheckState.Checked : CheckState.Unchecked;
                    chkIS_BORN_SUFFOCATE.CheckState = patientADO.IS_BORN_SUFFOCATE.HasValue && patientADO.IS_BORN_SUFFOCATE.Value == 1
                        ? CheckState.Checked : CheckState.Unchecked;
                    dtPregnancyLastTime.EditValue = patientADO.PREGNANCY_LAST_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientADO.PREGNANCY_LAST_TIME.Value) : null;
                    spinPREGNANCY_COUNT.EditValue = patientADO.PREGNANCY_COUNT.HasValue ? patientADO.PREGNANCY_COUNT : null;
                    spinPREGNANCY_MISCARRIAGE_COUNT.EditValue = patientADO.PREGNANCY_MISCARRIAGE_COUNT.HasValue ? patientADO.PREGNANCY_MISCARRIAGE_COUNT : null;
                    spinPREGNANCY_ABORTION_COUNT.EditValue = patientADO.PREGNANCY_ABORTION_COUNT.HasValue ? patientADO.PREGNANCY_ABORTION_COUNT : null;
                    spinBORN_COUNT.EditValue = patientADO.BORN_COUNT.HasValue ? patientADO.BORN_COUNT : null;
                    spinBORN_NORMAL_COUNT.EditValue = patientADO.BORN_NORMAL_COUNT.HasValue ? patientADO.BORN_NORMAL_COUNT : null;
                    spinBORN_CAESAREAN_SECTION_COUNT.EditValue = patientADO.BORN_CAESAREAN_SECTION_COUNT.HasValue ? patientADO.BORN_CAESAREAN_SECTION_COUNT : null;
                    spinBORN_HARD_COUNT.EditValue = patientADO.BORN_HARD_COUNT.HasValue ? patientADO.BORN_HARD_COUNT : null;
                    spinBORN_INADEQUACY_COUNT.EditValue = patientADO.BORN_INADEQUACY_COUNT.HasValue
                        ? patientADO.BORN_INADEQUACY_COUNT : null;
                    spinBORN_LIVING_COUNT.EditValue = patientADO.BORN_LIVING_COUNT.HasValue ? patientADO.BORN_LIVING_COUNT : null;
                    spinUV_VACCINATION_MOTHER_COUNT.EditValue = patientADO.UV_VACCINATION_MOTHER_COUNT.HasValue
                        ? patientADO.UV_VACCINATION_MOTHER_COUNT : null;
                    mmBornMalformation.Text = patientADO.BORN_MALFORMATION;
                    mmBORN_OTHER.Text = patientADO.BORN_OTHER;
                    mmGYNECOLOGY_DISEASE.Text = patientADO.GYNECOLOGY_DISEASE;
                    mmCONTRACEPTION.Text = patientADO.CONTRACEPTION;

                    //Thong tin cha/me
                    txtFatherCode.Text = patientADO.FATHER_CODE;
                    lblFatherName.Text = patientADO.FATHER_NAME;
                    lblFatherDob.Text = patientADO.FATHER_DOB.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientADO.FATHER_DOB.Value) : null;
                    txtMotherCode.Text = patientADO.MOTHER_CODE;
                    lblMotherName.Text = patientADO.MOTHER_NAME;
                    lblMotherDob.Text = patientADO.MOTHER_DOB.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientADO.MOTHER_DOB.Value) : null;

                    if (patientADO.FATHER_ID.HasValue)
                        fatherId = patientADO.FATHER_ID.Value;
                    if (patientADO.MOTHER_ID.HasValue)
                        motherId = patientADO.MOTHER_ID.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private bool CheckPregnancyVali()
        {
            bool result = true;
            try
            {
                int soLanMangThai = 0;
                int soLanXay = 0;
                int soLanPha = 0;
                int soLanSinh = 0;
                if (spinPREGNANCY_COUNT.EditValue != null)
                    soLanMangThai = Inventec.Common.TypeConvert.Parse.ToInt32(spinPREGNANCY_COUNT.Value.ToString());
                if (spinPREGNANCY_MISCARRIAGE_COUNT.EditValue != null)
                    soLanXay = Inventec.Common.TypeConvert.Parse.ToInt32(spinPREGNANCY_MISCARRIAGE_COUNT.Value.ToString());
                if (spinPREGNANCY_ABORTION_COUNT.EditValue != null)
                    soLanPha = Inventec.Common.TypeConvert.Parse.ToInt32(spinPREGNANCY_ABORTION_COUNT.Value.ToString());
                if (spinBORN_COUNT.EditValue != null)
                    soLanSinh = Inventec.Common.TypeConvert.Parse.ToInt32(spinBORN_COUNT.Value.ToString());


                if (soLanMangThai < soLanSinh)
                {
                    MessageBox.Show("Số lần sinh phải nhỏ hơn số lần mang thai");
                    return false;
                }

                if (soLanMangThai < (soLanXay + soLanPha + soLanSinh))
                {
                    MessageBox.Show("Tổng số lần xảy, phá, sinh phải nhỏ hơn lần mang thai");
                    return false;
                }

                if (soLanMangThai < soLanXay)
                {
                    MessageBox.Show("Số lần xảy thai phải nhỏ hơn số lần mang thai");
                    return false;
                }
                if (soLanMangThai < soLanPha)
                {
                    MessageBox.Show("Số lần phá thai phải nhỏ hơn số lần mang thai");
                    return false;
                }
                if (soLanMangThai < soLanXay)
                {
                    MessageBox.Show("Số lần xảy thai phải nhỏ hơn số lần mang thai");
                    return false;
                }

            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckBornVali()
        {
            bool result = true;
            try
            {
                int soLanSinh = 0;
                int slSinhBT = 0;
                int slSinhMo = 0;
                int slSinhKho = 0;
                int slSinhNon = 0;
                int slConSong = 0;

                if (spinBORN_COUNT.EditValue != null)
                    soLanSinh = Inventec.Common.TypeConvert.Parse.ToInt32(spinBORN_COUNT.Value.ToString());
                if (spinBORN_NORMAL_COUNT.EditValue != null)
                    slSinhBT = Inventec.Common.TypeConvert.Parse.ToInt32(spinBORN_NORMAL_COUNT.Value.ToString());
                if (spinBORN_CAESAREAN_SECTION_COUNT.EditValue != null)
                    slSinhMo = Inventec.Common.TypeConvert.Parse.ToInt32(spinBORN_CAESAREAN_SECTION_COUNT.Value.ToString());
                if (spinBORN_HARD_COUNT.EditValue != null)
                    slSinhKho = Inventec.Common.TypeConvert.Parse.ToInt32(spinBORN_HARD_COUNT.Value.ToString());
                if (spinBORN_INADEQUACY_COUNT.EditValue != null)
                    slSinhNon = Inventec.Common.TypeConvert.Parse.ToInt32(spinBORN_INADEQUACY_COUNT.Value.ToString());
                if (spinBORN_LIVING_COUNT.EditValue != null)
                    slConSong = Inventec.Common.TypeConvert.Parse.ToInt32(spinBORN_LIVING_COUNT.Value.ToString());

                if (soLanSinh < slSinhMo)
                {
                    MessageBox.Show("Số lần sinh phải lớn hơn hoặc bằng số lần sinh mổ");
                    return false;
                }

                if (soLanSinh < slSinhBT)
                {
                    MessageBox.Show("Số lần sinh phải lớn hơn hoặc bằng số lần sinh thường");
                    return false;
                }

                if (soLanSinh < slSinhKho)
                {
                    MessageBox.Show("Số lần sinh phải lớn hơn hoặc bằng số lần sinh khó");
                    return false;
                }

                if (soLanSinh < (slSinhBT + slSinhMo))
                {
                    MessageBox.Show("Số lần sinh phải lớn hơn hoặc bằng tổng số lần sinh thường và sinh mổ");
                    return false;
                }

                if (soLanSinh < slSinhNon)
                {
                    MessageBox.Show("Số lần sinh phải lớn hơn hoặc bằng số lần sinh non");
                    return false;
                }
                if (soLanSinh < slConSong)
                {
                    MessageBox.Show("Số lần sinh phải lớn hơn hoặc bằng bằng số con còn sống");
                    return false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        private void LoadPatient()
        {
            try
            {
                WaitingManager.Show();
                patientADO = new PatientADO();
                CommonParam param = new CommonParam();
                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = patientId;
                currentPatient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();
                if (currentPatient == null)
                    throw new Exception("Khong tim thay tong tin benh nhan . PatientId : " + patientId);

                Inventec.Common.Mapper.DataObjectMapper.Map<PatientADO>(patientADO, currentPatient);

                if (currentPatient.FATHER_ID.HasValue && currentPatient.FATHER_ID.Value > 0)
                {
                    HisPatientFilter patiFilter = new HisPatientFilter();
                    patiFilter.ID = currentPatient.FATHER_ID.Value;
                    HIS_PATIENT patientFather = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patiFilter, param).FirstOrDefault();
                    if (patientFather != null)
                    {
                        patientADO.FATHER_CODE = patientFather.PATIENT_CODE;
                        patientADO.FATHER_DOB = patientFather.DOB;
                    }

                }

                if (currentPatient.MOTHER_ID.HasValue && currentPatient.MOTHER_ID.Value > 0)
                {
                    HisPatientFilter patiFilter = new HisPatientFilter();
                    patiFilter.ID = currentPatient.MOTHER_ID.Value;
                    HIS_PATIENT patientMother = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patiFilter, param).FirstOrDefault();
                    if (patientMother != null)
                    {
                        patientADO.MOTHER_CODE = patientMother.PATIENT_CODE;
                        patientADO.MOTHER_DOB = patientMother.DOB;
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public HIS_PATIENT SetPatientFromControl()
        {
            HIS_PATIENT patient = null;
            try
            {
                if (this.currentPatient != null)
                {
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_PATIENT, HIS_PATIENT>();
                    patient = AutoMapper.Mapper.Map<HIS_PATIENT, HIS_PATIENT>(this.currentPatient);

                    if (fatherId > 0)
                        patient.FATHER_ID = fatherId;
                    if (motherId > 0)
                        patient.MOTHER_ID = motherId;
                    patient.FATHER_NAME = lblFatherName.Text;
                    patient.MOTHER_NAME = lblMotherName.Text;
                    if (spinBornWeight.EditValue != null)
                        patient.BORN_WEIGHT = spinBornWeight.Value;
                    if (spinBornHeight.EditValue != null)
                        patient.BORN_HEIGHT = spinBornHeight.Value;
                    patient.IS_BORN_INADEQUACY = (short)(chkIS_BORN_INADEQUACY.Checked ? 1 : 0);
                    patient.IS_BORN_SUFFOCATE = (short)(chkIS_BORN_SUFFOCATE.Checked ? 1 : 0);
                    if (dtPregnancyLastTime.EditValue != null)
                    {
                        patient.PREGNANCY_LAST_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtPregnancyLastTime.DateTime);
                    }
                    if (spinPREGNANCY_COUNT.EditValue != null)
                        patient.PREGNANCY_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(spinPREGNANCY_COUNT.Value.ToString());
                    if (spinPREGNANCY_MISCARRIAGE_COUNT.EditValue != null)
                        patient.PREGNANCY_MISCARRIAGE_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(spinPREGNANCY_MISCARRIAGE_COUNT.Value.ToString());
                    if (spinPREGNANCY_ABORTION_COUNT.EditValue != null)
                        patient.PREGNANCY_ABORTION_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(spinPREGNANCY_ABORTION_COUNT.Value.ToString());
                    if (spinBORN_COUNT.EditValue != null)
                        patient.BORN_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(spinBORN_COUNT.Value.ToString());
                    if (spinBORN_NORMAL_COUNT.EditValue != null)
                        patient.BORN_NORMAL_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(spinBORN_NORMAL_COUNT.Value.ToString());
                    if (spinBORN_CAESAREAN_SECTION_COUNT.EditValue != null)
                        patient.BORN_CAESAREAN_SECTION_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(spinBORN_CAESAREAN_SECTION_COUNT.Value.ToString());
                    if (spinBORN_HARD_COUNT.EditValue != null)
                        patient.BORN_HARD_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(spinBORN_HARD_COUNT.Value.ToString());
                    if (spinBORN_INADEQUACY_COUNT.EditValue != null)
                        patient.BORN_INADEQUACY_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(spinBORN_INADEQUACY_COUNT.Value.ToString());
                    if (spinBORN_LIVING_COUNT.EditValue != null)
                        patient.BORN_LIVING_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(spinBORN_LIVING_COUNT.Value.ToString());
                    if (spinUV_VACCINATION_MOTHER_COUNT.EditValue != null)
                        patient.UV_VACCINATION_MOTHER_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(spinUV_VACCINATION_MOTHER_COUNT.Value.ToString());
                    patient.BORN_MALFORMATION = mmBornMalformation.Text;
                    patient.BORN_OTHER = mmBORN_OTHER.Text;
                    patient.GYNECOLOGY_DISEASE = mmGYNECOLOGY_DISEASE.Text;
                    patient.CONTRACEPTION = mmCONTRACEPTION.Text;
                }
            }
            catch (Exception ex)
            {
                patient = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return patient;
        }
    }
}
