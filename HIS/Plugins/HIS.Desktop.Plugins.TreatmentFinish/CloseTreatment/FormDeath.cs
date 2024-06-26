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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.Death;
using HIS.UC.Death.ADO;
using HIS.UC.UCCauseOfDeath;
using HIS.UC.UCCauseOfDeath.ADO;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
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
    public partial class FormDeath : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        private HIS_TREATMENT hisTreatment;
        internal FormTreatmentFinish Form;
        private DeathProcessor deathProcessor;
        private UserControl ucDeath;
        private UCCauseOfDeathProcessor causeOfDeathProcessor;
        private UserControl ucCauseOfDeath;
        DeathInitADO deathInitADO;
        private MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO { get; set; }
        CauseOfDeathADO causeOfDeathAdo { get; set; }
        Action<CauseOfDeathADO> causeResult { get; set; }
        Action<MOS.SDO.HisTreatmentFinishSDO> deathResult { get; set; }
        public FormDeath()
        {
            InitializeComponent();
        }
        public FormDeath(MOS.EFMODEL.DataModels.HIS_TREATMENT treatment, Inventec.Desktop.Common.Modules.Module module, Action<MOS.SDO.HisTreatmentFinishSDO> deathResult)
            : base(module)
        {
            InitializeComponent();
            try
            {
                if (treatment != null)
                {
                    this.hisTreatment = treatment;
                }
                this.deathResult = deathResult;
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public FormDeath(MOS.EFMODEL.DataModels.HIS_TREATMENT treatment, CauseOfDeathADO causeOfDeathADO, Inventec.Desktop.Common.Modules.Module module, Action<CauseOfDeathADO> causeResult, Action<MOS.SDO.HisTreatmentFinishSDO> deathResult)
            : base(module)
        {
            InitializeComponent();
            try
            {
                if (treatment != null)
                {
                    this.hisTreatment = treatment;
                }
                this.deathResult = deathResult;
                this.causeResult = causeResult;
                this.causeOfDeathAdo = causeOfDeathADO;
                this.Size = new Size(1100, 600);

                this.currentModule = module;
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

        private void frmHisDeathInfo_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                if (hisTreatment != null)
                {
                    InitUCDeath();
                    InitUCCauseOfDeath();
                }
                else
                {
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentFinish.Resources.Lang", typeof(FormDeath).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormDeath.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("FormDeath.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("FormDeath.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value("FormDeath.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("FormDeath.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormDeath.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormDeath.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Edit.Caption = Inventec.Common.Resource.Get.Value("FormDeath.barButtonItem__Edit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Save.Caption = Inventec.Common.Resource.Get.Value("FormDeath.barButtonItem__Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormDeath.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUCCauseOfDeath()
        {
            try
            {
                causeOfDeathProcessor = new UCCauseOfDeathProcessor();
                ucCauseOfDeath = (UserControl)causeOfDeathProcessor.Run(causeOfDeathAdo);
                causeOfDeathProcessor.SetValue(ucCauseOfDeath, causeOfDeathAdo);
                ucCauseOfDeath.Dock = DockStyle.Fill;
                xtraTabPage2.Controls.Clear();
                xtraTabPage2.Controls.Add(this.ucCauseOfDeath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCDeath()
        {
            try
            {
                deathInitADO = new DeathInitADO();
                deathInitADO.DeathDataSource = new DeathDataSourcesADO();
                deathInitADO.DeathDataSource.CurrentHisTreatment = hisTreatment;
                deathInitADO.DeathDataSource.HisDeathCauses = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>();
                deathInitADO.DeathDataSource.HisDeathWithins = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>();

                BackendDataWorker.Reset<V_HIS_DEATH_CERT_BOOK>();
                var deathCertBooks = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_DEATH_CERT_BOOK>().Where(o => o.IS_ACTIVE == 1 && o.TOTAL > 0 && (o.FROM_NUM_ORDER + o.TOTAL - 1 > (o.CURRENT_DEATH_CERT_NUM ?? 0)) && (o.BRANCH_ID == null || o.BRANCH_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId()));

                if (deathCertBooks != null && deathCertBooks.Count() > 0)
                    deathInitADO.DeathDataSource.HisDeathCertBooks = deathCertBooks.ToList();
                deathInitADO.BranchName = BranchDataWorker.Branch.BRANCH_NAME;
                deathProcessor = new DeathProcessor();
                this.ucDeath = (UserControl)deathProcessor.Run(deathInitADO);
                this.ucDeath.Dock = DockStyle.Fill;
                xtraScrollableControl1.Controls.Clear();
                xtraScrollableControl1.Controls.Add(this.ucDeath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!deathProcessor.ValidControl(ucDeath)) return;
                WaitingManager.Show();
                MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO = deathProcessor.GetValue(ucDeath) as MOS.SDO.HisTreatmentFinishSDO;
                if (currentTreatmentFinishSDO != null)
                {
                    deathResult(currentTreatmentFinishSDO);
                    CauseOfDeathADO cause = causeOfDeathProcessor.GetValue(this.ucCauseOfDeath) as CauseOfDeathADO;
                    causeResult(cause);
                }
                else
                {
                    WaitingManager.Hide();
                    return;
                }
                WaitingManager.Hide();
                this.Close();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
