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
using HIS.Desktop.Utility;
using HIS.UC.Death;
using HIS.UC.Death.ADO;
using HIS.UC.Sick;
using HIS.UC.Sick.ADO;
using HIS.UC.SurgeryAppointment;
using HIS.UC.SurgeryAppointment.ADO;
using HIS.UC.UCCauseOfDeath;
using HIS.UC.UCCauseOfDeath.ADO;
using Inventec.Desktop.Common.LanguageManager;
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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.UC.ExamTreatmentFinish.Run
{
    public partial class frmPopUpSick : FormBase
    {
        private SickProcessor sickProcessor;
        UserControl ucSick;
        SickInitADO sickInitADO { get; set; }
        Action<HisTreatmentFinishSDO> sickResult { get; set; }

        Action<HisTreatmentFinishSDO> deathResult { get; set; }
        private SurgeryAppointmentProcessor surProcessor;
        UserControl ucSur;
        private DeathProcessor deathProcessor;
        private UserControl ucDeath;
        DeathInitADO deathInitADO;

        UCCauseOfDeathProcessor causeOfDeathProcessor { get; set; }
        UserControl ucCauseOfDeath { get; set; }
        CauseOfDeathADO causeOfDeathAdo { get; set; }
        SurgeryAppointmentInitADO surInitADO { get; set; }
        Action<SurgAppointmentADO> surResult { get; set; }
        Action<CauseOfDeathADO> causeReult { get; set; }

        bool IsFocusSickLeaveDay = false;


        public frmPopUpSick(SickInitADO sickInitADO, Action<HisTreatmentFinishSDO> sickResult, bool IsFocusSickLeaveDay = false)
        {
            InitializeComponent();
            try
            {
                this.sickResult = sickResult;
                this.sickInitADO = sickInitADO;
                this.IsFocusSickLeaveDay = IsFocusSickLeaveDay;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmPopUpSick(SurgeryAppointmentInitADO surInitADO, Action<SurgAppointmentADO> surResult)
        {
            InitializeComponent();
            try
            {
                this.surInitADO = surInitADO;
                this.surResult = surResult;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmPopUpSick(DeathInitADO deathInitADO, CauseOfDeathADO causeOfDeathADO,  Action<HisTreatmentFinishSDO> deathResult,Action<CauseOfDeathADO> causeReult)
        {
            InitializeComponent();
            try
            {
                this.deathInitADO = deathInitADO;
                this.deathResult = deathResult;
                this.causeReult = causeReult;
                this.causeOfDeathAdo = causeOfDeathADO;
                this.Size = new Size(1150,600);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void frmPopUpSick_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
                lciDeath.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciOther.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                if (sickInitADO != null)
                {
                    sickProcessor = new SickProcessor();
                    this.ucSick = (UserControl)sickProcessor.Run(sickInitADO);
                    this.ucSick.Dock = DockStyle.Fill;
                    xtraScrollableControl1.Controls.Clear();
                    xtraScrollableControl1.Controls.Add(this.ucSick);
                    RegisterTimer(ModuleLink, this.Name + ".timer1", timer1.Interval, timer1_Tick);
                    if (IsFocusSickLeaveDay)
                        StartTimer(ModuleLink, this.Name + ".timer1");
                    if (!sickInitADO.IsDuongThai)
                        ReSizeForm();
                }
                else if (surInitADO != null)
                {
                    surProcessor = new SurgeryAppointmentProcessor();
                    this.ucSur = (UserControl)surProcessor.Run(surInitADO);
                    this.ucSur.Dock = DockStyle.Fill;
                    xtraScrollableControl1.Controls.Clear();
                    xtraScrollableControl1.Controls.Add(this.ucSur);
                }
                else if (deathInitADO != null)
                {                  
                    lciDeath.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciOther.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    deathProcessor = new DeathProcessor();
                    this.ucDeath = (UserControl)deathProcessor.Run(deathInitADO);
                    this.ucDeath.Dock = DockStyle.Fill;
                    xtraScrollableControl3.Controls.Clear();
                    xtraScrollableControl3.Controls.Add(this.ucDeath);

                    causeOfDeathProcessor = new UCCauseOfDeathProcessor();
                    ucCauseOfDeath = (UserControl)causeOfDeathProcessor.Run(causeOfDeathAdo);
                    causeOfDeathProcessor.SetValue(ucCauseOfDeath, causeOfDeathAdo);
                    ucCauseOfDeath.Dock = DockStyle.Fill;
                    xtraScrollableControl2.Controls.Clear();
                    xtraScrollableControl2.Controls.Add(this.ucCauseOfDeath);
                }
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmPopUpSick
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.ExamTreatmentFinish.Resources.Lang", typeof(frmPopUpSick).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPopUpSick.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnOke.Text = Inventec.Common.Resource.Get.Value("frmPopUpSick.btnOke.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPopUpSick.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void ReSizeForm()
        {
            try
            {
                this.Size = new Size(600, 460);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnOke_Click(object sender, EventArgs e)
        {
            try
            {
                if (sickInitADO != null)
                {
                    if (!sickProcessor.ValidControl(ucSick))
                        return;
                    HisTreatmentFinishSDO sick = sickProcessor.GetValue(this.ucSick) as HisTreatmentFinishSDO;
                    sickResult(sick);
                }
                else if (surInitADO != null)
                {
                    surResult(this.surProcessor.GetValuePlus(this.ucSur) as SurgAppointmentADO);
                }
                else if (deathInitADO != null)
                {
                    if (!deathProcessor.ValidControl(ucDeath)) return;

                    HisTreatmentFinishSDO death = deathProcessor.GetValue(this.ucDeath) as HisTreatmentFinishSDO;
                    deathResult(death);

                    CauseOfDeathADO cause = causeOfDeathProcessor.GetValue(this.ucCauseOfDeath) as CauseOfDeathADO;
                    causeReult(cause);                  
                }
                
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timer1_Tick()
        {
            try
            {
                sickProcessor.FocusControl(ucSick);
                StopTimer(ModuleLink, this.Name + "timer1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
