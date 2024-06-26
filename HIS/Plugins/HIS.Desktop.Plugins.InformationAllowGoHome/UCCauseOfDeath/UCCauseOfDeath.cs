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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.InformationAllowGoHome.UCCauseOfDeath.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InformationAllowGoHome.UCCauseOfDeath
{
    public partial class UCCauseOfDeath : UserControl
    {
        private CauseOfDeathADO currentCauseOfDeathADO { get; set; }
        private HIS_TREATMENT Treatment { get; set; }
        private HIS_SEVERE_ILLNESS_INFO SevereIllNessInfo { get; set; }
        public List<HIS_EVENTS_CAUSES_DEATH> ListEventsCausesDeath { get; set; }
        private List<EventsCausesDeathADO> ListEventsCausesDeathTable1 { get; set; }
        private List<EventsCausesDeathADO> ListEventsCausesDeathTable2 { get; set; }
        private List<EventsCausesDeathADO> ListEventsCausesDeathTable3 { get; set; }
        private CheckEdit[] ArrDeathType { get; set; }
        private CheckEdit[] ArrWithin4WeekSurgery { get; set; }
        //private CheckEdit[] ArrForensicExamination { get; set; }
        private CheckEdit[] ArrDeathCausesType { get; set; }
        private CheckEdit[] ArrExtenalCauses { get; set; }
        private CheckEdit[] ArrFetalOrInfantDeath { get; set; }
        private CheckEdit[] ArrMultiplePregnancy { get; set; }
        private CheckEdit[] ArrPrematureBirth { get; set; }
        private CheckEdit[] ArrDiedWhenPregnancy { get; set; }
        private CheckEdit[] ArrTimePregnancy { get; set; }
        private CheckEdit[] ArrPregnancyCausingDeath { get; set; }
        public List<HIS_ICD> icd { get; set; }
        private bool IsEdit { get; set; }

        private String[] ArrNameRowDefault = new String[] { "Tình trạng bệnh nặng trước khi xin về", "Nguyên nhân gây ra 1 (a)", "Nguyên nhân gây ra 1 (b)", "Nguyên nhân gây ra 1 (c)" };
        public UCCauseOfDeath(CauseOfDeathADO ado)
        {
            InitializeComponent();
            this.currentCauseOfDeathADO = ado;
            InitData();
        }
        private void UCCauseOfDeath_Load(object sender, EventArgs e)
        {

        }
        private void InitData()
        {
            try
            {
                IsEdit = false;
                ArrDeathType = new CheckEdit[] { chkDeathType1 };
                ArrWithin4WeekSurgery = new CheckEdit[] { chk4WeekY, chk4WeekN, chk4WeekYN };
                //ArrForensicExamination = new CheckEdit[] { chkForensicExaminationY, chkForensicExaminationN, chkForensicExaminationYN };
                ArrDeathCausesType = new CheckEdit[] { chkCausesType1, chkCausesType2, chkCausesType3, chkCausesType4, chkCausesType5, chkCausesType6, chkCausesType7, chkCausesType8, chkCausesType9 };
                ArrExtenalCauses = new CheckEdit[] { chkExternalCausesType1, chkExternalCausesType2, chkExternalCausesType3, chkExternalCausesType4, chkExternalCausesType5, chkExternalCausesType6, chkExternalCausesType7, chkExternalCausesType8, chkExternalCausesType9, chkExternalCausesType10 };
                ArrFetalOrInfantDeath = new CheckEdit[] { chkInfantDeathY, chkInfantDeathN };
                ArrMultiplePregnancy = new CheckEdit[] { chkMultiplePregnancyY, chkMultiplePregnancyN, chkMultiplePregnancyYN };
                ArrPrematureBirth = new CheckEdit[] { chkPrematureBirthY, chkPrematureBirthN, chkPrematureBirthYN };
                ArrDiedWhenPregnancy = new CheckEdit[] { chkDieWhenPregnancyY, chkDieWhenPregnancyN, chkDieWhenPregnancyYN };
                ArrTimePregnancy = new CheckEdit[] { chkTimeOfPregnancyType1, chkTimeOfPregnancyType2, chkTimeOfPregnancyType3, chkTimeOfPregnancyType4 };
                ArrPregnancyCausingDeath = new CheckEdit[] { chkIsPregnancyCausingDeathY, chkIsPregnancyCausingDeathN, chkIsPregnancyCausingDeathYN };

                ListEventsCausesDeathTable1 = new List<EventsCausesDeathADO>();
                ListEventsCausesDeathTable2 = new List<EventsCausesDeathADO>();
                ListEventsCausesDeathTable3 = new List<EventsCausesDeathADO>();
                InitIcd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillTable()
        {
            try
            {
                gridControl1.DataSource = null;
                gridControl1.DataSource = ListEventsCausesDeathTable1;
                gridControl2.DataSource = null;
                gridControl2.DataSource = ListEventsCausesDeathTable2;
                gridControl3.DataSource = null;
                gridControl3.DataSource = ListEventsCausesDeathTable3;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetValue(object entity)
        {
            try
            {
                if (entity is CauseOfDeathADO)
                {
                    DefaultListTable();
                    this.currentCauseOfDeathADO = (CauseOfDeathADO)entity;
                    if (currentCauseOfDeathADO != null)
                    {
                        Treatment = currentCauseOfDeathADO.Treatment;
                        SevereIllNessInfo = currentCauseOfDeathADO.SevereIllNessInfo;
                        ListEventsCausesDeath = currentCauseOfDeathADO.ListEventsCausesDeath;
                        if (ListEventsCausesDeath != null && ListEventsCausesDeath.Count > 0)
                        {
                            var dtTable1 = ListEventsCausesDeath.Where(o => o.IS_OTHER_CAUSE == 1).ToList();
                            if (dtTable1 == null || dtTable1.Count == 0)
                            {
                                AddLastRowTable1();
                            }
                            else
                            {
                                ListEventsCausesDeathTable1 = new List<EventsCausesDeathADO>();
                                foreach (var item in dtTable1)
                                {
                                    EventsCausesDeathADO ado = new EventsCausesDeathADO(item);
                                    ListEventsCausesDeathTable1.Add(ado);
                                }
                                IsEdit = true;
                                ListEventsCausesDeathTable1.ForEach(o => o.actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                                ListEventsCausesDeathTable1.LastOrDefault().actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            }

                            var dtTable2 = ListEventsCausesDeath.Where(o => o.IS_OTHER_CAUSE != 1 && string.IsNullOrEmpty(o.EXTERNAL_CAUSE)).ToList();
                            if (dtTable2 != null && dtTable2.Count > 0)
                            {
                                ListEventsCausesDeathTable2 = new List<EventsCausesDeathADO>();
                                foreach (var item in dtTable2)
                                {
                                    EventsCausesDeathADO ado = new EventsCausesDeathADO(item);
                                    ListEventsCausesDeathTable2.Add(ado);
                                }
                                ListEventsCausesDeathTable2.ForEach(o => o.actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                                ListEventsCausesDeathTable2.LastOrDefault().actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;

                            }


                            var dtTable3 = ListEventsCausesDeath.Where(o => o.IS_OTHER_CAUSE != 1 && !string.IsNullOrEmpty(o.EXTERNAL_CAUSE)).ToList();
                            if (dtTable3 != null && dtTable3.Count > 0)
                            {
                                ListEventsCausesDeathTable3 = new List<EventsCausesDeathADO>();
                                foreach (var item in dtTable3)
                                {
                                    EventsCausesDeathADO ado = new EventsCausesDeathADO(item);
                                    ListEventsCausesDeathTable3.Add(ado);
                                }
                                ListEventsCausesDeathTable3.ForEach(o => o.actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                                ListEventsCausesDeathTable3.LastOrDefault().actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            }
                        }
                        else
                        {
                            AddLastRowTable1();
                        }
                    }
                    FillTable();
                    FillControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddLastRowTable1()
        {
            try
            {
                EventsCausesDeathADO adoTable1Plus = new EventsCausesDeathADO();
                adoTable1Plus.actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                ListEventsCausesDeathTable1.Add(adoTable1Plus);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void Reload()
        {
            try
            {
                DefaultControl();
                DefaultListTable();
                AddLastRowTable1();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal object GetValue()
        {
            object result = null;
            try
            {
                CauseOfDeathADO ado = new CauseOfDeathADO();
                ado.Treatment = Treatment;
                HIS_SEVERE_ILLNESS_INFO severe = new HIS_SEVERE_ILLNESS_INFO();
                if (SevereIllNessInfo != null)
                    severe.ID = SevereIllNessInfo.ID;
                severe.IS_DEATH = 0;
                if (spnAbsentDay.EditValue != null)
                    severe.ABSENT_DAY = (long)spnAbsentDay.Value;
                if (spnIcuBed.EditValue != null)
                    severe.ICU_BED_DAYS_COUNT = (long)spnIcuBed.Value;
                if (ArrDeathType.FirstOrDefault(o => o.Checked) != null)
                    severe.DEATH_TYPE = ArrDeathType.ToList().FirstOrDefault(o => o.Checked) != null ? (short?)2 : null;
                if (ArrWithin4WeekSurgery.FirstOrDefault(o => o.Checked) != null)
                {
                    severe.IS_WITHIN_4_WEEK_SURGERY = (short)(ArrWithin4WeekSurgery.ToList().IndexOf(ArrWithin4WeekSurgery.FirstOrDefault(o => o.Checked)) + 1);
                    if (ArrWithin4WeekSurgery.First().Checked && dtechk4Week.DateTime != DateTime.MinValue && dtechk4Week.EditValue != null)
                        severe.SURGERY_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtechk4Week.DateTime);
                }
                severe.SURGERY_REASON = txtSurgetReason.Text;
                //if (ArrForensicExamination.FirstOrDefault(o => o.Checked) != null)
                //    severe.IS_USING_FORENSIC_EXAMINATION = (short)(ArrForensicExamination.ToList().IndexOf(ArrForensicExamination.FirstOrDefault(o => o.Checked)) + 1);
                if (ArrDeathCausesType.FirstOrDefault(o => o.Checked) != null)
                    severe.DEATH_CAUSES_TYPE = (short)(ArrDeathCausesType.ToList().IndexOf(ArrDeathCausesType.FirstOrDefault(o => o.Checked)) + 1);
                if (ArrExtenalCauses.FirstOrDefault(o => o.Checked) != null)
                {
                    severe.EXTERNAL_CAUSES = (short)(ArrExtenalCauses.ToList().IndexOf(ArrExtenalCauses.FirstOrDefault(o => o.Checked)) + 1);
                    if (ArrExtenalCauses.Last().Checked)
                        severe.OTHER_EXTERNAL_CAUSES = txtOtherExten.Text;
                }
                if (ArrFetalOrInfantDeath.FirstOrDefault(o => o.Checked) != null)
                    severe.IS_FETAL_OR_INFANT_DEATH = (short)(ArrFetalOrInfantDeath.ToList().IndexOf(ArrFetalOrInfantDeath.FirstOrDefault(o => o.Checked)) + 1);
                if (ArrMultiplePregnancy.FirstOrDefault(o => o.Checked) != null)
                    severe.MULTIPLE_PREGNANCY = (short)(ArrMultiplePregnancy.ToList().IndexOf(ArrMultiplePregnancy.FirstOrDefault(o => o.Checked)) + 1);
                if (ArrPrematureBirth.FirstOrDefault(o => o.Checked) != null)
                    severe.PREMATURE_BIRTH = (short)(ArrPrematureBirth.ToList().IndexOf(ArrPrematureBirth.FirstOrDefault(o => o.Checked)) + 1);
                if (spnInfantAge.EditValue != null)
                    severe.INFANT_AGE = (long)spnInfantAge.Value;
                if (spnInfantWeight.EditValue != null)
                    severe.INFANT_WEIGHT = (long)spnInfantWeight.Value;
                if (spnFetalAge.EditValue != null)
                    severe.FETAL_AGE = (long)spnFetalAge.Value;
                if (spnMotherAge.EditValue != null)
                    severe.MOTHER_AGE = (long)spnMotherAge.Value;
                if (cboFetalInfant.EditValue != null)
                    severe.FETAL_INFANT_AFFECTED_ICD = cboFetalInfant.EditValue.ToString();
                if (ArrDiedWhenPregnancy.FirstOrDefault(o => o.Checked) != null)
                    severe.IS_DIED_WHEN_PREGNANCY = (short)(ArrDiedWhenPregnancy.ToList().IndexOf(ArrDiedWhenPregnancy.FirstOrDefault(o => o.Checked)) + 1);
                if (ArrPregnancyCausingDeath.FirstOrDefault(o => o.Checked) != null)
                    severe.IS_PREGNANCY_CAUSING_DEATH = (short)(ArrPregnancyCausingDeath.ToList().IndexOf(ArrPregnancyCausingDeath.FirstOrDefault(o => o.Checked)) + 1);
                if (ArrTimePregnancy.FirstOrDefault(o => o.Checked) != null)
                    severe.TIME_OF_PREGNANCY = (short)(ArrTimePregnancy.ToList().IndexOf(ArrTimePregnancy.FirstOrDefault(o => o.Checked)) + 1);
                if (cboFetalInfant.EditValue != null)
                    severe.FETAL_INFANT_AFFECTED_ICD = cboFetalInfant.EditValue.ToString();
                if (cboDeathMainCauseCode.EditValue != null)
                    severe.DEATH_MAIN_CAUSE = cboDeathMainCauseCode.Text.ToString();
                severe.TREATMENT_ID = Treatment.ID;
                ado.SevereIllNessInfo = severe;
                ado.ListEventsCausesDeath = new List<HIS_EVENTS_CAUSES_DEATH>();
                ado.ListEventsCausesDeath.AddRange(GetList(gridControl1, true, false));
                ado.ListEventsCausesDeath.AddRange(GetList(gridControl2, false, true));
                ado.ListEventsCausesDeath.AddRange(GetList(gridControl3));
                result = ado;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<HIS_EVENTS_CAUSES_DEATH> GetList(GridControl grid, bool IsGrid1 = false, bool IsGrid2 = false)
        {
            List<HIS_EVENTS_CAUSES_DEATH> lst = new List<HIS_EVENTS_CAUSES_DEATH>();
            try
            {
                var lstAdo = grid.DataSource as List<EventsCausesDeathADO>;
                foreach (var item in lstAdo)
                {
                    if (IsGrid1)
                    {
                        if (!string.IsNullOrEmpty(item.ICD_CODE) || item.HAPPEN_TIME > 0)
                        {
                            HIS_EVENTS_CAUSES_DEATH obj = new HIS_EVENTS_CAUSES_DEATH();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EVENTS_CAUSES_DEATH>(obj, item);
                            obj.IS_OTHER_CAUSE = 1;
                            lst.Add(obj);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.CAUSE_NAME) || !string.IsNullOrEmpty(item.DESCRIPTION) || !string.IsNullOrEmpty(item.ICD_CODE) || item.HAPPEN_TIME != null)
                        {
                            HIS_EVENTS_CAUSES_DEATH obj = new HIS_EVENTS_CAUSES_DEATH();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EVENTS_CAUSES_DEATH>(obj, item);
                            if (IsGrid2)
                                obj.IS_OTHER_CAUSE = null;
                            else
                                obj.EXTERNAL_CAUSE = "Nguyên nhân ngoài";
                            lst.Add(obj);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return lst;
        }

        private void FillControl()
        {
            try
            {
                if (Treatment != null)
                {
                    this.lblPatientName.Text = Treatment.TDL_PATIENT_NAME;
                    this.lblPatientGender.Text = Treatment.TDL_PATIENT_GENDER_NAME;
                    this.lblPatientDob.Text = Treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1 ? Int64.Parse(Treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4)).ToString() : Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Treatment.TDL_PATIENT_DOB);
                    this.lblPatientHein.Text = Treatment.TDL_HEIN_CARD_NUMBER;
                }
                if (SevereIllNessInfo != null)
                {
                    spnAbsentDay.EditValue = SevereIllNessInfo.ABSENT_DAY;
                    spnIcuBed.EditValue = SevereIllNessInfo.ICU_BED_DAYS_COUNT;
                    if (SevereIllNessInfo.DEATH_TYPE != null)
                        ArrDeathType[0].Checked = true;
                    if (SevereIllNessInfo.IS_WITHIN_4_WEEK_SURGERY != null)
                    {
                        ArrWithin4WeekSurgery[(short)(SevereIllNessInfo.IS_WITHIN_4_WEEK_SURGERY ?? 0) - 1].Checked = true;
                        if (ArrWithin4WeekSurgery.First().Checked)
                            dtechk4Week.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(SevereIllNessInfo.SURGERY_DATE ?? 0) ?? DateTime.Now;
                    }
                    txtSurgetReason.Text = SevereIllNessInfo.SURGERY_REASON;
                    //if (SevereIllNessInfo.IS_USING_FORENSIC_EXAMINATION != null)
                    //    ArrForensicExamination[(SevereIllNessInfo.IS_USING_FORENSIC_EXAMINATION ?? 0) - 1].Checked = true;
                    if (SevereIllNessInfo.DEATH_CAUSES_TYPE != null)
                        ArrDeathCausesType[(SevereIllNessInfo.DEATH_CAUSES_TYPE ?? 0) - 1].Checked = true;
                    if (SevereIllNessInfo.EXTERNAL_CAUSES != null)
                    {
                        ArrExtenalCauses[(short)(SevereIllNessInfo.EXTERNAL_CAUSES ?? 0) - 1].Checked = true;
                        if (ArrExtenalCauses.Last().Checked)
                            txtOtherExten.Text = SevereIllNessInfo.OTHER_EXTERNAL_CAUSES;
                    }
                    if (SevereIllNessInfo.IS_FETAL_OR_INFANT_DEATH != null)
                    {
                        ArrFetalOrInfantDeath[(short)(SevereIllNessInfo.IS_FETAL_OR_INFANT_DEATH ?? 0) - 1].Checked = true;
                    }
                    if (SevereIllNessInfo.MULTIPLE_PREGNANCY != null)
                    {
                        ArrMultiplePregnancy[(short)(SevereIllNessInfo.MULTIPLE_PREGNANCY ?? 0) - 1].Checked = true;
                    }
                    if (SevereIllNessInfo.PREMATURE_BIRTH != null)
                    {
                        ArrPrematureBirth[(short)(SevereIllNessInfo.PREMATURE_BIRTH ?? 0) - 1].Checked = true;
                    }
                    if (SevereIllNessInfo.IS_DIED_WHEN_PREGNANCY != null)
                    {
                        ArrDiedWhenPregnancy[(short)(SevereIllNessInfo.IS_DIED_WHEN_PREGNANCY ?? 0) - 1].Checked = true;
                    }
                    if (SevereIllNessInfo.TIME_OF_PREGNANCY != null)
                    {
                        ArrTimePregnancy[(short)(SevereIllNessInfo.TIME_OF_PREGNANCY ?? 0) - 1].Checked = true;
                    }
                    if (SevereIllNessInfo.IS_PREGNANCY_CAUSING_DEATH != null)
                    {
                        ArrPregnancyCausingDeath[(short)(SevereIllNessInfo.IS_PREGNANCY_CAUSING_DEATH ?? 0) - 1].Checked = true;
                    }
                    spnInfantAge.EditValue = SevereIllNessInfo.INFANT_AGE;
                    spnInfantWeight.EditValue = SevereIllNessInfo.INFANT_WEIGHT;
                    spnFetalAge.EditValue = SevereIllNessInfo.FETAL_AGE;
                    spnMotherAge.EditValue = SevereIllNessInfo.MOTHER_AGE;
                    cboFetalInfant.EditValue = SevereIllNessInfo.FETAL_INFANT_AFFECTED_ICD;
                    //
                    if (!String.IsNullOrEmpty(SevereIllNessInfo.DEATH_MAIN_CAUSE))
                    {
                        var icdMain = icd.FirstOrDefault(o => o.ICD_NAME == SevereIllNessInfo.DEATH_MAIN_CAUSE);
                        if (icdMain != null)
                            cboDeathMainCauseCode.EditValue = icdMain.ICD_CODE;
                    }
                    //
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DefaultControl()
        {
            try
            {
                this.lblPatientName.Text = this.lblPatientHein.Text = this.lblPatientGender.Text = this.lblPatientDob.Text = String.Empty;
                //spnAbsentDay.EditValue = spnFetalAge.EditValue = spnIcuBed.EditValue = spnInfantAge.EditValue = spnInfantWeight.EditValue = spnMotherAge.EditValue = null;
                if (!layoutControl1.IsInitialized) return;
                layoutControl1.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl1.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            if (lci.Control is TextEdit)
                            {
                                var fomatFrm = lci.Control as DevExpress.XtraEditors.TextEdit;
                                fomatFrm.Text = null;
                            }
                            else if (lci.Control is SpinEdit)
                            {
                                var fomatFrm = lci.Control as DevExpress.XtraEditors.SpinEdit;
                                fomatFrm.EditValue = null;
                            }
                            else if (lci.Control is DevExpress.XtraGrid.GridControl)
                            {
                                var fomatFrm = lci.Control as DevExpress.XtraGrid.GridControl;
                                fomatFrm.DataSource = null;
                            }
                            else if (lci.Control is DevExpress.XtraEditors.CheckEdit)
                            {
                                var fomatFrm = lci.Control as DevExpress.XtraEditors.CheckEdit;
                                fomatFrm.Checked = false;
                            }
                            else if (lci.Control is GridLookUpEdit)
                            {
                                var fomatFrm = lci.Control as DevExpress.XtraEditors.GridLookUpEdit;
                                fomatFrm.EditValue = null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl1.EndUpdate();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DefaultListTable()
        {
            try
            {
                ListEventsCausesDeathTable1 = new List<EventsCausesDeathADO>();
                ListEventsCausesDeathTable2 = new List<EventsCausesDeathADO>();
                ListEventsCausesDeathTable3 = new List<EventsCausesDeathADO>();
                for (int i = 0; i < ArrNameRowDefault.Length; i++)
                {
                    EventsCausesDeathADO adoTable1 = new EventsCausesDeathADO();
                    adoTable1.CAUSE_NAME = ArrNameRowDefault[i];
                    adoTable1.actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionView;
                    ListEventsCausesDeathTable1.Add(adoTable1);
                }
                EventsCausesDeathADO adoTable2 = new EventsCausesDeathADO();
                adoTable2.actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                ListEventsCausesDeathTable2.Add(adoTable2);
                EventsCausesDeathADO adoTable3 = new EventsCausesDeathADO();
                adoTable3.actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                ListEventsCausesDeathTable3.Add(adoTable3);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnT1")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Int32.Parse((gridView1.GetRowCellValue(e.RowHandle, "actionType") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repAddT1;
                    }
                    else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = repEditT1;
                    }
                    else
                    {
                        e.RepositoryItem = repView;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitIcd()
        {
            try
            {
                icd = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>().Where(o => o.IS_ACTIVE == 1).OrderBy(o => o.ICD_CODE).ToList();
                InitComboIcdRep(icd.ToList(), repIcdMutilT1);
                InitComboIcdRep(icd.ToList(), repIcdMutilT2);
                InitComboIcdRep(icd.ToList(), repIcdNameMtT3, true);
                InitComboIcdRep(icd.ToList(), repIcdCodeMutilT3);
                InitComboIcd(icd.ToList(), cboDeathMainCauseCode);
                InitComboIcd(icd.ToList(), cboFetalInfant);

                var dt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().Where(o => o.IS_ACTIVE == 1).ToList();
                InitComboUnitRep(dt, repUnitNameT1);
                InitComboUnitRep(dt, repUnitNameT2);
                InitComboUnitRep(dt, repUnitNameT3);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboIcd(List<HIS_ICD> lst, DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ICD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("ICD_NAME", "", 250, 2));

                ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_NAME", "ICD_CODE", columnInfos, false, 400);
                ControlEditorLoader.Load(cbo, lst, controlEditorADO);
                cbo.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboIcdRep(List<HIS_ICD> lst, RepositoryItemCustomGridLookUpEdit cbo, bool IsNameTable3 = false)
        {
            try
            {
                cbo.DataSource = lst;
                cbo.DisplayMember = IsNameTable3 ? "ICD_NAME" : "ICD_CODE";
                cbo.ValueMember = "ICD_CODE";
                cbo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.ImmediatePopup = true;
                cbo.View.Columns.Clear();
                cbo.PopupFormSize = new System.Drawing.Size(900, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.View.Columns.AddField("ICD_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.View.Columns.AddField("ICD_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.View.Columns.AddField("ICD_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.View.Columns["ICD_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboUnitRep(List<HIS_SERVICE_UNIT> lst, RepositoryItemGridLookUpEdit cbo)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 150, 1));

                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "SERVICE_UNIT_NAME", columnInfos, false, 150);
                ControlEditorLoader.Load(cbo, lst, controlEditorADO);
                cbo.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnT2")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Int32.Parse((gridView2.GetRowCellValue(e.RowHandle, "actionType") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repAddT2;
                    }
                    else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = repEditT2;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView3_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnT3")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Int32.Parse((gridView3.GetRowCellValue(e.RowHandle, "actionType") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repAddT3;
                    }
                    else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = repEditT3;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chk4WeekY_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ArrWithin4WeekSurgery.First().Checked)
                {
                    dtechk4Week.Enabled = true;
                    txtSurgetReason.Enabled = true;
                }
                else
                {
                    dtechk4Week.Enabled = false;
                    txtSurgetReason.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExternalCausesType1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ArrExtenalCauses.Last().Checked)
                    txtOtherExten.Enabled = true;
                else
                    txtOtherExten.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboFetalInfant_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboFetalInfant.EditValue != null)
                {
                    txtFetalInfantCode.Text = cboFetalInfant.EditValue.ToString();
                }
                else
                {
                    txtFetalInfantCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDeathMainCauseCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDeathMainCauseCode.EditValue != null)
                {
                    txtDeathMainCauseCode.Text = cboDeathMainCauseCode.EditValue.ToString();
                }
                else
                {
                    txtDeathMainCauseCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboFetalInfant_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboFetalInfant.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDeathMainCauseCode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboDeathMainCauseCode.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFetalInfantCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboFetalInfant.Focus();
                    if (!string.IsNullOrEmpty(txtFetalInfantCode.Text))
                    {
                        var dt = icd.FirstOrDefault(o => o.ICD_CODE == txtFetalInfantCode.Text.Trim());
                        if (dt != null)
                        {
                            cboFetalInfant.EditValue = dt.ICD_CODE;
                        }
                        else
                        {
                            cboFetalInfant.ShowPopup();
                        }
                    }
                    else
                    {
                        cboFetalInfant.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDeathMainCauseCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDeathMainCauseCode.Focus();
                    if (!string.IsNullOrEmpty(txtDeathMainCauseCode.Text))
                    {
                        var dt = icd.FirstOrDefault(o => o.ICD_CODE == txtDeathMainCauseCode.Text.Trim());
                        if (dt != null)
                        {
                            cboDeathMainCauseCode.EditValue = dt.ICD_CODE;
                        }
                        else
                        {
                            cboDeathMainCauseCode.ShowPopup();
                        }
                    }
                    else
                    {
                        cboDeathMainCauseCode.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var ado = (EventsCausesDeathADO)this.gridView3.GetFocusedRow();
                if (ado != null)
                {
                    if (e.Column.FieldName == "ICD_CODE" || e.Column.FieldName == "ICD_NAME")
                    {
                        ado.ICD_CODE = ado.ICD_CODE;
                        ado.ICD_NAME = ado.ICD_CODE;
                    }
                    if (e.Column.FieldName == "Date")
                    {
                        ado.HAPPEN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ado.Date);
                    }
                    if (e.Column.FieldName == "SERVICE_UNIT_NAME")
                    {
                        ado.UNIT_NAME = ado.SERVICE_UNIT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {

                var ado = (EventsCausesDeathADO)this.gridView1.GetFocusedRow();
                if (ado != null)
                {
                    if (e.Column.FieldName == "Date")
                    {
                        ado.HAPPEN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ado.Date);
                    }
                    if (e.Column.FieldName == "SERVICE_UNIT_NAME")
                    {
                        ado.UNIT_NAME = ado.SERVICE_UNIT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {

                var ado = (EventsCausesDeathADO)this.gridView2.GetFocusedRow();
                if (ado != null)
                {
                    if (e.Column.FieldName == "Date")
                    {
                        ado.HAPPEN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ado.Date);
                    }
                    if (e.Column.FieldName == "SERVICE_UNIT_NAME")
                    {
                        ado.UNIT_NAME = ado.SERVICE_UNIT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    var dataRow = (EventsCausesDeathADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "Date" && dataRow.Date == DateTime.MinValue && dataRow.Date == null)
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    var dataRow = (EventsCausesDeathADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "Date" && dataRow.Date == DateTime.MinValue && dataRow.Date == null)
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView3_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    var dataRow = (EventsCausesDeathADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "Date" && dataRow.Date == DateTime.MinValue && dataRow.Date == null)
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repEditT1_Click(object sender, EventArgs e)
        {
            try
            {
                var ado = (EventsCausesDeathADO)this.gridView1.GetFocusedRow();

                List<EventsCausesDeathADO> lst = gridControl1.DataSource as List<EventsCausesDeathADO>;
                lst.Remove(ado);
                lst.LastOrDefault().actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                gridControl1.DataSource = null;
                gridControl1.DataSource = lst;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repAddT1_Click(object sender, EventArgs e)
        {
            try
            {
                List<EventsCausesDeathADO> lst = gridControl1.DataSource as List<EventsCausesDeathADO>;
                EventsCausesDeathADO participant = new EventsCausesDeathADO();
                lst.Add(participant);
                for (int i = 0; i < lst.Count; i++)
                {
                    if (IsEdit)
                    {
                        lst[i].actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    }
                    else
                    {
                        if (i < 4)
                            lst[i].actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionView;
                        else
                            lst[i].actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    }
                }
                lst.LastOrDefault().actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;

                gridControl1.DataSource = null;
                gridControl1.DataSource = lst;
                gridView1.FocusedRowHandle = lst.Count - 1;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repEditT2_Click(object sender, EventArgs e)
        {
            try
            {
                var ado = (EventsCausesDeathADO)this.gridView2.GetFocusedRow();

                ListEventsCausesDeathTable2.Remove(ado);
                ListEventsCausesDeathTable2.LastOrDefault().actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;

                gridControl2.DataSource = null;
                gridControl2.DataSource = ListEventsCausesDeathTable2;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repAddT2_Click(object sender, EventArgs e)
        {
            try
            {

                EventsCausesDeathADO participant = new EventsCausesDeathADO();
                ListEventsCausesDeathTable2.Add(participant);
                ListEventsCausesDeathTable2.ForEach(o => o.actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                ListEventsCausesDeathTable2.LastOrDefault().actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;

                gridControl2.DataSource = null;
                gridControl2.DataSource = ListEventsCausesDeathTable2;
                gridView2.FocusedRowHandle = ListEventsCausesDeathTable2.Count - 1;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repEditT3_Click(object sender, EventArgs e)
        {
            try
            {
                var ado = (EventsCausesDeathADO)this.gridView3.GetFocusedRow();

                ListEventsCausesDeathTable3.Remove(ado);
                ListEventsCausesDeathTable3.LastOrDefault().actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;

                gridControl3.DataSource = null;
                gridControl3.DataSource = ListEventsCausesDeathTable3;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repAddT3_Click(object sender, EventArgs e)
        {
            try
            {
                EventsCausesDeathADO participant = new EventsCausesDeathADO();
                ListEventsCausesDeathTable3.Add(participant);
                ListEventsCausesDeathTable3.ForEach(o => o.actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                ListEventsCausesDeathTable3.LastOrDefault().actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                gridControl3.DataSource = null;
                gridControl3.DataSource = ListEventsCausesDeathTable3;
                gridView3.FocusedRowHandle = ListEventsCausesDeathTable3.Count - 1;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
