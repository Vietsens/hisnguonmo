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
using HIS.Desktop.LocalStorage.LocalData;
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

namespace HIS.Desktop.Plugins.CareSlipList.CareSlipList
{
    public partial class FormCareSlipList : Form
    {
        CommonParam param = new CommonParam();
        internal void LoadDataToForm(FormCareSlipList control)
        {
            try
            {
                LoadDataToRadioGroupAwareness(ref control.lstHisAwareness);
                LoadDataToGridControlCareDetail(ref control.lstHisCareType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadDataToRadioGroupAwareness(ref List<MOS.EFMODEL.DataModels.HIS_AWARENESS> lstHisAwareness)
        {
            try
            {
                MOS.Filter.HisAwarenessFilter hisAwarenessFilter = new MOS.Filter.HisAwarenessFilter();
                lstHisAwareness = new BackendAdapter(param).Get<List<HIS_AWARENESS>>(ApiConsumer.HisRequestUriStore.HIS_AWARENESS_GET, ApiConsumers.MosConsumer, hisAwarenessFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadDataToGridControlCareDetail(ref List<MOS.EFMODEL.DataModels.HIS_CARE_TYPE> lstHisCareType)
        {
            try
            {
                MOS.Filter.HisCareTypeFilter hisCareTypeFilter = new MOS.Filter.HisCareTypeFilter();

                lstHisCareType = new BackendAdapter(param).Get<List<HIS_CARE_TYPE>>(ApiConsumer.HisRequestUriStore.HIS_CARE_TYPE_GET, ApiConsumers.MosConsumer, hisCareTypeFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /*internal void LoadDataHisCareEdit(FormCareSlipList control, MOS.EFMODEL.DataModels.HIS_CARE hisCareCurrent)
        {
            try
            {
                control.dtExcuteTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisCareCurrent.EXECUTE_TIME ?? 0);
                control.dtExcuteTime.Update();
                //if (hisCareCurrent.AWARENESS_ID > 0)
                //{
                //control.cboAwareness.EditValue = Convert.ToInt64(hisCareCurrent.AWARENESS_ID);
                //}
                control.txtAwareness.Text = hisCareCurrent.AWARENESS;
                control.txtMucocutaneous.Text = hisCareCurrent.MUCOCUTANEOUS;
                control.txtUrine.Text = hisCareCurrent.URINE;
                control.txtDejecta.Text = hisCareCurrent.DEJECTA;
                control.txtSanitary.Text = hisCareCurrent.SANITARY;
                control.txtTutorial.Text = hisCareCurrent.TUTORIAL;
                control.txtEducation.Text = hisCareCurrent.EDUCATION;
                control.txtNutrition.Text = hisCareCurrent.NUTRITION;

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


                MOS.Filter.HisDhstFilter hisDHSTFilter = new HisDhstFilter();
                hisDHSTFilter.ID = hisCareCurrent.DHST_ID;
                List<MOS.EFMODEL.DataModels.HIS_DHST> hisDHST = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, hisDHSTFilter, param);
                if (hisDHST != null && hisDHST.Count > 0)
                {
                    control.currentDhst = hisDHST.FirstOrDefault();
                    control.spinPulse.EditValue = control.currentDhst.PULSE;
                    control.spinTemperature.EditValue = control.currentDhst.TEMPERATURE;
                    control.spinBloodPressureMax.EditValue = control.currentDhst.BLOOD_PRESSURE_MAX;
                    control.spinBloodPressureMin.EditValue = control.currentDhst.BLOOD_PRESSURE_MIN;
                    control.spinBreathRate.EditValue = control.currentDhst.BREATH_RATE;
                    control.spinWeight.EditValue = control.currentDhst.WEIGHT;
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
         * */        
    }
}
