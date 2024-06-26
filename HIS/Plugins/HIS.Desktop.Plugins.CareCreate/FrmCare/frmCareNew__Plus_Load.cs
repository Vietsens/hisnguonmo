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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.CareCreate.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
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

namespace HIS.Desktop.Plugins.CareCreate
{
    public partial class CareCreate : HIS.Desktop.Utility.FormBase
    {
        CommonParam param = new CommonParam();

        /// <summary>
        /// Khoi Tao Du Lieu Theo Tracking
        /// </summary>
        private void LoadDataDefaultByTracking()
        {
            try
            {
                CommonParam param = new CommonParam();
                dtExcuteTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisTracking.TRACKING_TIME);
                MOS.Filter.HisCareFilter careFilter = new MOS.Filter.HisCareFilter();
                careFilter.TRACKING_ID = this.currentHisTracking.ID;
                this.hisCareCurrent = new HIS_CARE();
                this.hisCareCurrent = new BackendAdapter(param).Get<List<HIS_CARE>>(HisRequestUriStore.HIS_CARE_GET, ApiConsumers.MosConsumer, careFilter, param).FirstOrDefault();
                if (this.hisCareCurrent != null)
                {
                    this.action = GlobalVariables.ActionEdit;
                    btnCboPrint.Enabled = true;
                    //btnDhst.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadDataToForm(CareCreate control)
        {
            try
            {
                LoadDataToRadioGroupAwareness(ref control.lstHisAwareness);
                LoadDataToGridControlCareDetail(ref control.lstHisCareType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadDataToRadioGroupAwareness(ref List<MOS.EFMODEL.DataModels.HIS_AWARENESS> lstHisAwareness)
        {
            try
            {
                MOS.Filter.HisAwarenessFilter hisAwarenessFilter = new MOS.Filter.HisAwarenessFilter();
                lstHisAwareness = new BackendAdapter(param).Get<List<HIS_AWARENESS>>(HisRequestUriStore.HIS_AWARENESS_GET, ApiConsumers.MosConsumer, hisAwarenessFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadDataToGridControlCareDetail(ref List<MOS.EFMODEL.DataModels.HIS_CARE_TYPE> lstHisCareType)
        {
            try
            {
                MOS.Filter.HisCareTypeFilter hisCareTypeFilter = new MOS.Filter.HisCareTypeFilter();
                hisCareTypeFilter.ORDER_DIRECTION = "DESC";
                hisCareTypeFilter.ORDER_FIELD = "MODIFY_TIME";
                lstHisCareType = new BackendAdapter(param).Get<List<HIS_CARE_TYPE>>(HisRequestUriStore.HIS_CARE_TYPE_GET, ApiConsumers.MosConsumer, hisCareTypeFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadDataHisCareEdit(CareCreate control, MOS.EFMODEL.DataModels.HIS_CARE hisCareCurrent)
        {
            try
            {
                control.dtExcuteTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisCareCurrent.EXECUTE_TIME ?? 0);
                control.dtExcuteTime.Update();
                control.cboAwareness.EditValue = hisCareCurrent.AWARENESS;
                control.cboSanitary.EditValue = hisCareCurrent.SANITARY;
                control.cboMucocutaneous.EditValue = hisCareCurrent.MUCOCUTANEOUS;
                control.txtUrine.Text = hisCareCurrent.URINE;
                control.txtDejecta.Text = hisCareCurrent.DEJECTA;
                control.txtTutorial.Text = hisCareCurrent.TUTORIAL;
                control.txtEducation.Text = hisCareCurrent.EDUCATION;
                control.txtNutrition.Text = hisCareCurrent.NUTRITION;
                control.txtCareDescription.Text = hisCareCurrent.CARE_DESCRIPTION;
                control.txtInstructionDescription.Text = hisCareCurrent.INSTRUCTION_DESCRIPTION;

                if (hisCareCurrent.HAS_MEDICINE == 1)
                {
                    control.chkHasMedicine.Checked = true;
                }
                if (hisCareCurrent.HAS_ADD_MEDICINE == 1)
                {
                    control.chkHasAddMedicine.Checked = true;
                }
                if (hisCareCurrent.HAS_TEST == 1)
                {
                    control.chkHasTest.Checked = true;
                }
                if (hisCareCurrent.HAS_REHABILITATION == 1)
                {
                    control.chkHasRehabiliration.Checked = true;
                }
                else
                {
                    control.chkHasRehabiliration.Checked = false;
                }

                control.lstHisCareDetailADO = new List<ADO.HisCareDetailADO>();
                MOS.Filter.HisCareDetailViewFilter hisCareDetailFilter = new MOS.Filter.HisCareDetailViewFilter();
                hisCareDetailFilter.CARE_ID = hisCareCurrent.ID;
                List<MOS.EFMODEL.DataModels.V_HIS_CARE_DETAIL> lstHisCareDetail = new BackendAdapter(param).Get<List<V_HIS_CARE_DETAIL>>(HisRequestUriStore.HIS_CARE_DETAIL_GETVIEW, ApiConsumers.MosConsumer, hisCareDetailFilter, param);
                List<ADO.HisCareDetailADO> lstHisCareDetailSDO = new List<ADO.HisCareDetailADO>();
                int i = 0;
                foreach (var item_CareDetail in lstHisCareDetail)
                {
                    ADO.HisCareDetailADO hisCareDetailSDO = new ADO.HisCareDetailADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisCareDetailADO>(hisCareDetailSDO, item_CareDetail);
                    if (i == 0)
                        hisCareDetailSDO.Action = GlobalVariables.ActionAdd;
                    else
                        hisCareDetailSDO.Action = GlobalVariables.ActionEdit;
                    lstHisCareDetailSDO.Add(hisCareDetailSDO);
                    i++;
                }
                if (lstHisCareDetailSDO != null && lstHisCareDetailSDO.Count > 0)
                {
                    control.gridControlCareDetail.DataSource = lstHisCareDetailSDO;
                    control.lstHisCareDetailADO = lstHisCareDetailSDO;
                }
                else
                {
                    ADO.HisCareDetailADO hisCareDetailSDO = new ADO.HisCareDetailADO();
                    hisCareDetailSDO.Action = GlobalVariables.ActionAdd;
                    control.lstHisCareDetailADO.Add(hisCareDetailSDO);
                    control.gridControlCareDetail.DataSource = null;
                    control.gridControlCareDetail.DataSource = control.lstHisCareDetailADO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadDataHisDhstEdit(CareCreate control, MOS.EFMODEL.DataModels.HIS_CARE hisCareCurrent)
        {
            try
            {
                MOS.Filter.HisDhstFilter hisDHSTFilter = new MOS.Filter.HisDhstFilter();
                hisDHSTFilter.ORDER_DIRECTION = "DESC";
                hisDHSTFilter.ORDER_FIELD = "MODIFY_TIME";
                hisDHSTFilter.CARE_ID = hisCareCurrent.ID;
                var hisDhst = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, hisDHSTFilter, param).FirstOrDefault();
                
                if (hisDhst != null)
                {
                    control.dtDoTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisDhst.EXECUTE_TIME ?? 0);
                    control.dtDoTime.Update();
                    control.spinPulse.EditValue = hisDhst.PULSE;
                    control.spinBloodPressureMax.EditValue = hisDhst.BLOOD_PRESSURE_MAX;
                    control.spinBloodPressureMin.EditValue = hisDhst.BLOOD_PRESSURE_MIN;
                    control.spinWeight.EditValue = hisDhst.WEIGHT;
                    control.spinHeight.EditValue = hisDhst.HEIGHT;
                    control.spinSPO2.EditValue = hisDhst.SPO2;
                    control.spinTemperature.EditValue = hisDhst.TEMPERATURE;
                    control.spinBreathRate.EditValue = hisDhst.BREATH_RATE;
                    control.spinChest.EditValue = hisDhst.CHEST;
                    control.spinBelly.EditValue = hisDhst.BELLY;
                    control.spinBloodWay.EditValue = hisDhst.CAPILLARY_BLOOD_GLUCOSE;
                    control.txtNote.Text = hisDhst.NOTE;
                }
                this.currentDhst = hisDhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadDataHisDhstCreate(CareCreate control, long hisCurrentTreatmentId)
        {
            try
            {
                MOS.Filter.HisDhstFilter hisDHSTFilter = new MOS.Filter.HisDhstFilter();
                hisDHSTFilter.ORDER_DIRECTION = "DESC";
                hisDHSTFilter.ORDER_FIELD = "MODIFY_TIME";
                hisDHSTFilter.TREATMENT_ID = hisCurrentTreatmentId;

                var hisDhst = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, hisDHSTFilter, param).OrderByDescending(o => o.EXECUTE_TIME).FirstOrDefault();

                if (hisDhst != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("log 1");
                    control.dtDoTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisDhst.EXECUTE_TIME ?? 0);
                    control.dtDoTime.Update();
                    control.spinPulse.EditValue = hisDhst.PULSE;
                    control.spinBloodPressureMax.EditValue = hisDhst.BLOOD_PRESSURE_MAX;
                    control.spinBloodPressureMin.EditValue = hisDhst.BLOOD_PRESSURE_MIN;
                    control.spinWeight.EditValue = hisDhst.WEIGHT;
                    control.spinHeight.EditValue = hisDhst.HEIGHT;
                    control.spinSPO2.EditValue = hisDhst.SPO2;
                    control.spinTemperature.EditValue = hisDhst.TEMPERATURE;
                    control.spinBreathRate.EditValue = hisDhst.BREATH_RATE;
                    control.spinChest.EditValue = hisDhst.CHEST;
                    control.spinBelly.EditValue = hisDhst.BELLY;
                    control.spinBloodWay.EditValue = hisDhst.CAPILLARY_BLOOD_GLUCOSE;
                    control.txtNote.Text = hisDhst.NOTE;
                }
                else {
                    Inventec.Common.Logging.LogSystem.Debug("log 2");
                    control.dtDoTime.EditValue = DateTime.Now;
                    control.dtDoTime.Update();
                    control.spinPulse.EditValue = null;
                    control.spinBloodPressureMax.EditValue = null;
                    control.spinBloodPressureMin.EditValue = null;
                    control.spinWeight.EditValue = null;
                    control.spinHeight.EditValue = null;
                    control.spinSPO2.EditValue = null;
                    control.spinTemperature.EditValue = null;
                    control.spinBreathRate.EditValue = null;
                    control.spinChest.EditValue = null;
                    control.spinBelly.EditValue = null;
                    control.spinBloodWay.EditValue = null;
                    control.txtNote.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadDataHisCareTemp(HisCareTempADO hisCareCurrent)
        {
            try
            {
                dtExcuteTime.DateTime = DateTime.Now;
                dtExcuteTime.Update();
                cboAwareness.EditValue  = hisCareCurrent.AWARENESS;
                cboMucocutaneous.EditValue = hisCareCurrent.MUCOCUTANEOUS;
                txtUrine.Text = hisCareCurrent.URINE;
                txtDejecta.Text = hisCareCurrent.DEJECTA;
                cboSanitary.EditValue = hisCareCurrent.SANITARY;
                txtTutorial.Text = hisCareCurrent.TUTORIAL;
                txtEducation.Text = hisCareCurrent.EDUCATION;
                txtNutrition.Text = hisCareCurrent.NUTRITION;
                txtCareDescription.Text = hisCareCurrent.CARE_DESCRIPTION;
                txtInstructionDescription.Text = hisCareCurrent.INSTRUCTION_DESCRIPTION;

                if (hisCareCurrent.HAS_MEDICINE == 1)
                {
                    chkHasMedicine.Checked = true;
                }
                if (hisCareCurrent.HAS_ADD_MEDICINE == 1)
                {
                    chkHasAddMedicine.Checked = true;
                }
                if (hisCareCurrent.HAS_TEST == 1)
                {
                    chkHasTest.Checked = true;
                }
                if (hisCareCurrent.HAS_REHABILITATION == 1)
                {
                    chkHasRehabiliration.Checked = true;
                }
                this.lstHisCareDetailSDO = new List<HisCareDetailADO>();
                this.lstHisCareDetailSDO = LoadCareTempDetail(hisCareCurrent.ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
