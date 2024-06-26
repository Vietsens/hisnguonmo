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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.DHST;
using HIS.UC.DHST.ADO;
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

namespace HIS.Desktop.Plugins.CareCreate.DHST
{
    public partial class frmDhst : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        internal HIS_CARE hisCareCurrent { get; set; }
        UserControl ucControlDHST;
        internal DHSTProcessor dhstProcessor;
        long action = 0;
        internal HIS_DHST currentDhst { get; set; }
        internal HIS_DHST treatmentDhst { get; set; }

        public frmDhst()
        {
            InitializeComponent();
        }

        public frmDhst(HIS_CARE hisCareCurrent, HIS_DHST _dhst, Inventec.Desktop.Common.Modules.Module currentModule)
        {
            InitializeComponent();
            try
            {
                this.treatmentDhst = _dhst;
                this.currentModule = currentModule;
                this.hisCareCurrent = hisCareCurrent;
                InitUCDHST();
                this.action = GlobalVariables.ActionAdd;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //public frmDhst(HIS_DHST _dhst, Inventec.Desktop.Common.Modules.Module currentModule)
        //{
        //    InitializeComponent();
        //    try
        //    {
        //        this.currentModule = currentModule;
        //        this.treatmentDhst = _dhst;
        //        InitUCDHST();
        //        this.action = GlobalVariables.ActionAdd;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void frmDhst_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                LoadDhstByCareId();
                //LoadDhstByTreatmentId();
                if (ucControlDHST != null)
                {
                    dhstProcessor.InFocus(ucControlDHST);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCDHST()
        {
            try
            {
                dhstProcessor = new DHSTProcessor();
                DHSTInitADO ado = new DHSTInitADO();
                ado.delegateOutFocus = SAVE;
                this.ucControlDHST = (UserControl)dhstProcessor.Run(ado);
                if (this.ucControlDHST != null)
                {
                    this.layoutControlDhst.Controls.Add(this.ucControlDHST);
                    this.ucControlDHST.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SAVE()
        {
            try
            {
                btnSave.Focus();
                btnSave.Select();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDhstByTreatmentId()
        {
            try
            {
                if (ucControlDHST != null)
                {
                    dhstProcessor.SetValue(ucControlDHST, this.treatmentDhst);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDhstByCareId()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                currentDhst = new HIS_DHST();
                MOS.Filter.HisDhstFilter dhstFilter = new MOS.Filter.HisDhstFilter();
                dhstFilter.CARE_ID = this.hisCareCurrent.ID;
                currentDhst = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param).FirstOrDefault();

                if (currentDhst != null)
                {
                    if (ucControlDHST != null)
                    {
                        dhstProcessor.SetValue(ucControlDHST, currentDhst);
                        this.action = GlobalVariables.ActionEdit;
                    }
                }
                else
                {
                    if (ucControlDHST != null)
                    {
                        dhstProcessor.SetValue(ucControlDHST, this.treatmentDhst);
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                HIS_DHST dhstNew = new HIS_DHST();
                if (currentDhst != null)
                {
                    AutoMapper.Mapper.CreateMap<HIS_DHST, HIS_DHST>();
                    dhstNew = AutoMapper.Mapper.Map<HIS_DHST>(currentDhst);
                }
                dhstNew.CARE_ID = this.hisCareCurrent.ID;
                dhstNew.TREATMENT_ID = this.hisCareCurrent.TREATMENT_ID;
                dhstNew.EXECUTE_ROOM_ID = this.currentModule.RoomId;
                if (ucControlDHST != null)
                {
                    DHSTADO dhstADO = dhstProcessor.GetValue(ucControlDHST) as DHSTADO;

                    if (dhstADO == null || !dhstADO.IsVali)
                    {
                        WaitingManager.Hide();
                        return;
                    }

                    dhstNew.BELLY = dhstADO.BELLY;

                    dhstNew.BLOOD_PRESSURE_MAX = dhstADO.BLOOD_PRESSURE_MAX;
                    dhstNew.BLOOD_PRESSURE_MIN = dhstADO.BLOOD_PRESSURE_MIN;
                    dhstNew.WEIGHT = dhstADO.WEIGHT;
                    dhstNew.HEIGHT = dhstADO.HEIGHT;
                    dhstNew.PULSE = dhstADO.PULSE;
                    dhstNew.CHEST = dhstADO.CHEST;
                    dhstNew.TEMPERATURE = dhstADO.TEMPERATURE;
                    dhstNew.BREATH_RATE = dhstADO.BREATH_RATE;
                    dhstNew.BELLY = dhstADO.BELLY;
                    dhstNew.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    dhstNew.EXECUTE_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                    dhstNew.EXECUTE_TIME = dhstADO.EXECUTE_TIME;
                    dhstNew.NOTE = dhstADO.NOTE;
                }
                HIS_DHST rs = new HIS_DHST();
                if (this.action == GlobalVariables.ActionAdd)
                {
                    rs = new BackendAdapter(param).Post<HIS_DHST>(HisRequestUriStore.HIS_DHST_CREATE, ApiConsumers.MosConsumer, dhstNew, param);
                }
                else if (this.action == GlobalVariables.ActionEdit || this.action == GlobalVariables.ActionView)
                {
                    rs = new BackendAdapter(param).Post<HIS_DHST>(HisRequestUriStore.HIS_DHST_UPDATE, ApiConsumers.MosConsumer, dhstNew, param);
                }
                WaitingManager.Hide();
                if (rs != null)
                {
                    success = true;
                    currentDhst = new HIS_DHST();
                    currentDhst = rs;
                    this.action = GlobalVariables.ActionView;
                    this.Close();
                }
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
