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
using AutoMapper;
using DevExpress.Utils;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.CareCreate.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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
        void InitGroupAwareness()
        {
            try
            {
                if (lstHisAwareness != null && lstHisAwareness.Count > 0)
                {
                    //for (int i = 0; i < lstHisAwareness.Count; i++)
                    //{
                    //    cboAwareness.Properties.Items.Add(new ComboBoxItem(lstHisAwareness[i].ID, lstHisAwareness[i].AWARENESS_NAME));
                    //}
                    // LoadDataToComboYThuc(cboAwareness, lstHisAwareness);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khoi tao Du Lieu Grid CARE_DETAIL
        /// </summary>
        void InitCareDetail()
        {
            try
            {
                this.lstHisCareDetailADO = new List<ADO.HisCareDetailADO>();
                ADO.HisCareDetailADO hisCareDetail = new ADO.HisCareDetailADO();
                hisCareDetail.Action = GlobalVariables.ActionAdd;
                this.lstHisCareDetailADO.Add(hisCareDetail);
                gridControlCareDetail.DataSource = null;
                gridControlCareDetail.DataSource = this.lstHisCareDetailADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void ProcessDataDHST(ref MOS.EFMODEL.DataModels.HIS_DHST hisDHST)
        {
            try
            {
                if (hisDHST == null)
                {
                    hisDHST = new HIS_DHST();
                }
                if (this.hisCareCurrent != null && this.hisCareCurrent.ID > 0)
                {
                    hisDHST.CARE_ID = this.hisCareCurrent.ID;
                }
                hisDHST.EXECUTE_ROOM_ID = this.currentModule.RoomId;
                hisDHST.TREATMENT_ID = this.currentTreatmentId;
                if (spinBelly.EditValue != null)
                {
                    hisDHST.BELLY = (decimal)spinBelly.EditValue;
                }
                else { hisDHST.BELLY = null; }
                if (spinBloodPressureMax.EditValue != null && spinBloodPressureMax.EditValue.ToString() != "")
                {
                    hisDHST.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMax.EditValue.ToString());
                }
                else
                {
                    hisDHST.BLOOD_PRESSURE_MAX = null;
                }
                if (spinBloodPressureMin.EditValue != null && spinBloodPressureMin.EditValue.ToString() != "")
                {
                    hisDHST.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMin.EditValue.ToString());
                }
                else
                {
                    hisDHST.BLOOD_PRESSURE_MIN = null;
                }
                if (spinWeight.EditValue != null)
                {
                    hisDHST.WEIGHT = (decimal)spinWeight.EditValue;
                }
                else { hisDHST.WEIGHT = null; }
                if (spinHeight.EditValue != null)
                {
                    hisDHST.HEIGHT = (decimal)spinHeight.EditValue;
                }
                else { hisDHST.HEIGHT = null; }
                if (spinPulse.EditValue != null && spinPulse.EditValue.ToString() != "")
                {
                    hisDHST.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(spinPulse.EditValue.ToString());
                }
                else
                {
                    hisDHST.PULSE = null;
                }
                if (spinChest.EditValue != null && spinChest.EditValue.ToString() != "")
                {
                    hisDHST.CHEST = (decimal)spinChest.EditValue;
                }
                else
                {
                    hisDHST.CHEST = null;
                }
                if (spinSPO2.EditValue != null && spinSPO2.EditValue.ToString() != "")
                {
                    hisDHST.SPO2 = (decimal)spinSPO2.EditValue;
                }
                else
                {
                    hisDHST.SPO2 = null;
                }
                if (spinTemperature.EditValue != null && spinTemperature.EditValue.ToString() != "")
                {
                    hisDHST.TEMPERATURE = (decimal)spinTemperature.EditValue;
                }
                else
                {
                    hisDHST.TEMPERATURE = null;
                }
                if (spinBreathRate.EditValue != null && spinBreathRate.EditValue.ToString() != "")
                {
                    hisDHST.BREATH_RATE = (decimal)spinBreathRate.EditValue;
                }
                else
                {
                    hisDHST.BREATH_RATE = null;
                }
                hisDHST.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                hisDHST.EXECUTE_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                if (dtDoTime.EditValue != null && dtDoTime.EditValue.ToString() != "")
                {
                    hisDHST.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDoTime.DateTime);
                }
                else
                {
                    hisDHST.EXECUTE_TIME = null;
                }
                if (spinBloodWay.EditValue != null)
                {
                    hisDHST.CAPILLARY_BLOOD_GLUCOSE = (decimal)spinBloodWay.EditValue;
                }
                else { hisDHST.CAPILLARY_BLOOD_GLUCOSE = null; }

                hisDHST.NOTE = txtNote.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ProcessDataCare(ref MOS.EFMODEL.DataModels.HIS_CARE hisCare)
        {
            if (hisCare == null)
            {
                hisCare = new HIS_CARE();
            }
            hisCare.HIS_CARE_DETAIL = new List<HIS_CARE_DETAIL>();
            hisCare.DEJECTA = txtDejecta.Text.Trim();
            //if (this.currentDhst != null && this.currentDhst.ID > 0)
            //    hisCare.DHST_ID = this.currentDhst.ID;
            hisCare.EDUCATION = txtEducation.Text.Trim();
            hisCare.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            hisCare.EXECUTE_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
            hisCare.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExcuteTime.DateTime);
            hisCare.EXECUTE_ROOM_ID = this.currentModule.RoomId;
            if (chkHasAddMedicine.Checked)
            {
                hisCare.HAS_ADD_MEDICINE = (short)1;
            }
            else
            {
                hisCare.HAS_ADD_MEDICINE = null;
            }
            if (chkHasMedicine.Checked)
            {
                hisCare.HAS_MEDICINE = (short)1;
            }
            else
            {
                hisCare.HAS_MEDICINE = null;
            }
            if (chkHasTest.Checked)
            {
                hisCare.HAS_TEST = (short)1;
            }
            else
            {
                hisCare.HAS_TEST = null;
            }
            if (chkHasRehabiliration.Checked)
            {
                hisCare.HAS_REHABILITATION = (short)1;
            }
            else
            {
                hisCare.HAS_REHABILITATION = null;
            }
            if (cboMucocutaneous.EditValue != null && cboMucocutaneous.EditValue.ToString() != "")
            {
                hisCare.MUCOCUTANEOUS = cboMucocutaneous.EditValue.ToString();
            }
            else
            {
                hisCare.MUCOCUTANEOUS = null;
            }

            hisCare.NUTRITION = txtNutrition.Text.Trim();
            if (cboSanitary.EditValue != null && cboSanitary.EditValue.ToString() != "")
            {
                hisCare.SANITARY = cboSanitary.EditValue.ToString();
            }
            else
            {
                hisCare.SANITARY = null;
            }
            if (cboAwareness.EditValue != null && cboAwareness.EditValue.ToString() != "")
            {
                hisCare.AWARENESS = cboAwareness.EditValue.ToString();
            }
            else
            {
                hisCare.AWARENESS = null;
            }

            hisCare.TREATMENT_ID = this.currentTreatmentId;
            hisCare.TUTORIAL = txtTutorial.Text.Trim();
            hisCare.URINE = txtUrine.Text.Trim();

            if (this.currentHisTracking != null && this.currentHisTracking.ID > 0 )
            {
                hisCare.TRACKING_ID = this.currentHisTracking.ID;
            }
            if (this.currentCareSum != null)
            {
                hisCare.CARE_SUM_ID = this.currentCareSum.ID;
            }
            if (this.lstHisCareDetailADO != null)
            {
                foreach (var item in this.lstHisCareDetailADO)
                {
                    if (item.CARE_TYPE_ID > 0 && !String.IsNullOrEmpty(item.CONTENT))
                    {
                        HIS_CARE_DETAIL detail = new HIS_CARE_DETAIL();
                        Mapper.CreateMap<HisCareDetailADO, MOS.EFMODEL.DataModels.HIS_CARE_DETAIL>();
                        detail = Mapper.Map<HisCareDetailADO, MOS.EFMODEL.DataModels.HIS_CARE_DETAIL>(item);
                        hisCare.HIS_CARE_DETAIL.Add(detail);
                    }
                }
            }
            hisCare.INSTRUCTION_DESCRIPTION = txtInstructionDescription.Text;
            hisCare.CARE_DESCRIPTION = txtCareDescription.Text;
        }

        private void ProcessDataCare(ref MOS.EFMODEL.DataModels.HIS_CARE_TEMP hisCare)
        {
            if (hisCare == null)
            {
                hisCare = new HIS_CARE_TEMP();
            }
            hisCare.HIS_CARE_TEMP_DETAIL = new List<HIS_CARE_TEMP_DETAIL>();

            //hisCare.AWARENESS = cboAwareness.EditValue.ToString();
            //hisCare.SANITARY = cboSanitary.EditValue.ToString();

            if (cboSanitary.EditValue != null && cboSanitary.EditValue.ToString() != "")
            {
                hisCare.SANITARY = cboSanitary.EditValue.ToString();
            }
            else
            {
                hisCare.SANITARY = null;
            }
            if (cboAwareness.EditValue != null && cboAwareness.EditValue.ToString() != "")
            {
                hisCare.AWARENESS = cboAwareness.EditValue.ToString();
            }
            else
            {
                hisCare.AWARENESS = null;
            }

            if (cboMucocutaneous.EditValue != null && cboMucocutaneous.EditValue.ToString() != "")
            {
                hisCare.MUCOCUTANEOUS = cboMucocutaneous.EditValue.ToString();
            }
            else 
            {
                hisCare.MUCOCUTANEOUS = null;
            }

            hisCare.DEJECTA = txtDejecta.Text.Trim();

            hisCare.EDUCATION = txtEducation.Text.Trim();
            if (chkHasAddMedicine.Checked)
            {
                hisCare.HAS_ADD_MEDICINE = (short)1;
            }
            else
            {
                hisCare.HAS_ADD_MEDICINE = null;
            }
            if (chkHasMedicine.Checked)
            {
                hisCare.HAS_MEDICINE = (short)1;
            }
            else
            {
                hisCare.HAS_MEDICINE = null;
            }
            if (chkHasTest.Checked)
            {
                hisCare.HAS_TEST = (short)1;
            }
            else
            {
                hisCare.HAS_TEST = null;
            }
            if (chkHasRehabiliration.Checked)
            {
                hisCare.HAS_REHABILITATION = (short)1;
            }
            else
            {
                hisCare.HAS_REHABILITATION = null;
            }
            hisCare.NUTRITION = txtNutrition.Text.Trim();
            hisCare.TUTORIAL = txtTutorial.Text.Trim();
            hisCare.URINE = txtUrine.Text.Trim();

            if (this.lstHisCareDetailADO != null)
            {
                foreach (var item in this.lstHisCareDetailADO)
                {
                    if (item.CARE_TYPE_ID > 0 && !String.IsNullOrEmpty(item.CONTENT))
                    {
                        HIS_CARE_TEMP_DETAIL detail = new HIS_CARE_TEMP_DETAIL();
                        Mapper.CreateMap<HisCareDetailADO, MOS.EFMODEL.DataModels.HIS_CARE_TEMP_DETAIL>();
                        detail = Mapper.Map<HisCareDetailADO, MOS.EFMODEL.DataModels.HIS_CARE_TEMP_DETAIL>(item);
                        hisCare.HIS_CARE_TEMP_DETAIL.Add(detail);
                    }
                }
            }
            hisCare.INSTRUCTION_DESCRIPTION = txtInstructionDescription.Text;
            hisCare.CARE_DESCRIPTION = txtCareDescription.Text;
        }

        private void SaveDataCare(MOS.EFMODEL.DataModels.HIS_CARE hisCare)
        {
            if (hisCare == null) throw new ArgumentNullException("hisCare is null");
            MOS.EFMODEL.DataModels.HIS_CARE rsHisCare = null;
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();

                switch (this.action)
                {
                    case GlobalVariables.ActionEdit:
                        rsHisCare = new BackendAdapter(param).Post<HIS_CARE>(HisRequestUriStore.HIS_CARE_UPDATE, ApiConsumers.MosConsumer, hisCare, param);
                        break;
                    case GlobalVariables.ActionAdd:
                        rsHisCare = new BackendAdapter(param).Post<HIS_CARE>(HisRequestUriStore.HIS_CARE_CREATE, ApiConsumers.MosConsumer, hisCare, param);
                        break;
                    default:
                        break;
                }

                if (rsHisCare != null && rsHisCare.ID > 0)
                {
                    success = true;
                    Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_CARE, MOS.EFMODEL.DataModels.HIS_CARE>();
                    this.hisCareCurrent = Mapper.Map<MOS.EFMODEL.DataModels.HIS_CARE, MOS.EFMODEL.DataModels.HIS_CARE>(rsHisCare);
                    btnCboPrint.Enabled = true;
                    //btnDhst.Enabled = true;
                    //this.action = GlobalVariables.ActionEdit;
                    if (this.delegateSelectData != null)
                        this.delegateSelectData(this.hisCareCurrent);
                }

                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void SaveDataDHST(MOS.EFMODEL.DataModels.HIS_DHST hisDHST)
        {
            if (hisDHST == null) throw new ArgumentNullException("hisDHST is null");
            MOS.EFMODEL.DataModels.HIS_DHST rsHisDHST = null;

            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();

                switch (this.action)
                {
                    case GlobalVariables.ActionEdit:

                        hisDHST.ID = this.currentDhst.ID;
                        rsHisDHST = new BackendAdapter(param).Post<HIS_DHST>(HisRequestUriStore.HIS_DHST_UPDATE, ApiConsumers.MosConsumer, hisDHST, param);

                        break;
                    case GlobalVariables.ActionAdd:
                        rsHisDHST = new BackendAdapter(param).Post<HIS_DHST>(HisRequestUriStore.HIS_DHST_CREATE, ApiConsumers.MosConsumer, hisDHST, param);
                        break;
                    default:
                        break;
                }
                Inventec.Common.Logging.LogSystem.Debug("hisDHST.ID" + hisDHST.ID);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsHisDHST), rsHisDHST));
                if (rsHisDHST != null && rsHisDHST.ID > 0)
                {
                    success = true;
                    Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_CARE, MOS.EFMODEL.DataModels.HIS_DHST>();
                    currentDhst = new HIS_DHST();
                    currentDhst = rsHisDHST;
                    this.hisCareCurrent.DHST_ID = rsHisDHST.ID;
                    //this.action = GlobalVariables.ActionView;
                    //this.hisCareCurrent = Mapper.Map<MOS.EFMODEL.DataModels.HIS_DHST, MOS.EFMODEL.DataModels.HIS_DHST>(rsHisDHST);
                    btnCboPrint.Enabled = true;
                    this.action = GlobalVariables.ActionEdit;
                }

                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }
    }
}
