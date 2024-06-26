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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.DHST;
using HIS.UC.DHST.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish.CloseTreatment
{
    public partial class frmDHST : Form
    {
        UserControl ucDHST;
        DHSTProcessor dhstProcessor;
        HIS.Desktop.Common.DelegateSelectData delegateData;
        HIS_TREATMENT currentTreatment;
        HIS_DHST currentDhst;

        public frmDHST(HIS.Desktop.Common.DelegateSelectData delegateData, HIS_TREATMENT treatment)
        {
            InitializeComponent();
            try
            {
                this.delegateData = delegateData;
                this.currentTreatment = treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmDHST(HIS.Desktop.Common.DelegateSelectData delegateData, HIS_TREATMENT treatment, HIS_DHST dhst)
        {
            InitializeComponent();
            try
            {
                this.delegateData = delegateData;
                this.currentTreatment = treatment;
                this.currentDhst = dhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmDHST_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetIcon();
                InitDHST();
                if (dhstProcessor != null && ucDHST != null)
                {
                    if (this.currentDhst != null)
                        dhstProcessor.SetValue(ucDHST, this.currentDhst);
                    dhstProcessor.SetValidate(ucDHST, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentFinish.Resources.Lang", typeof(frmDHST).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmDHST.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmDHST.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmDHST.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmDHST.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmDHST.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDHST()
        {
            try
            {
                dhstProcessor = new DHSTProcessor();
                DHSTInitADO initAdo = new DHSTInitADO();
                initAdo.IsRequiredWeight = true;
                initAdo.delegateOutFocus = FocusSave;

                ucDHST = (UserControl)dhstProcessor.Run(initAdo);
                ucDHST.Dock = DockStyle.Fill;
                panelControl1.Dock = DockStyle.Fill;
                panelControl1.Controls.Add(ucDHST);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ucDHST != null && this.dhstProcessor != null)
                {
                    DHSTADO dhstAdo = (DHSTADO)this.dhstProcessor.GetValue(this.ucDHST);
                    if (!dhstAdo.IsVali)
                    {
                        return;
                    }

                    bool success = false;
                    HIS_DHST dhst = new HIS_DHST();

                    CommonParam param = new CommonParam();
                    HIS_DHST rs = null;

                    if (this.currentDhst != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_DHST>(dhst, this.currentDhst);
                        dhst.EXECUTE_TIME = dhstAdo.EXECUTE_TIME;
                        dhst.BLOOD_PRESSURE_MAX = dhstAdo.BLOOD_PRESSURE_MAX;
                        dhst.BLOOD_PRESSURE_MIN = dhstAdo.BLOOD_PRESSURE_MIN;
                        dhst.BREATH_RATE = dhstAdo.BREATH_RATE;
                        dhst.HEIGHT = dhstAdo.HEIGHT;
                        dhst.CHEST = dhstAdo.CHEST;
                        dhst.BELLY = dhstAdo.BELLY;
                        dhst.PULSE = dhstAdo.PULSE;
                        dhst.TEMPERATURE = dhstAdo.TEMPERATURE;
                        dhst.WEIGHT = dhstAdo.WEIGHT;

                        rs = new BackendAdapter(param).Post<HIS_DHST>("api/HisDhst/Update", ApiConsumer.ApiConsumers.MosConsumer, dhst, param);
                    }
                    else
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_DHST>(dhst, dhstAdo);
                        dhst.TREATMENT_ID = this.currentTreatment.ID;
                        dhst.EXECUTE_DEPARTMENT_ID = WorkPlace.GetDepartmentId();
                        dhst.EXECUTE_ROOM_ID = WorkPlace.GetRoomId();
                        dhst.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        dhst.EXECUTE_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                        dhst.EXECUTE_TIME = Inventec.Common.DateTime.Get.Now();

                        rs = new BackendAdapter(param).Post<HIS_DHST>("api/HisDhst/Create", ApiConsumer.ApiConsumers.MosConsumer, dhst, param);
                    }

                    if (rs != null)
                    {
                        success = true;
                        this.Close();
                        if (this.delegateData != null)
                            this.delegateData(dhst);
                    }

                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusSave()
        {
            try
            {
                if (btnSave.Enabled)
                    btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
