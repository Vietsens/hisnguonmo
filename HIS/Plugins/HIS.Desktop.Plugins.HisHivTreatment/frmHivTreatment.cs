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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using HIS.UC.WorkPlace;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.LocalStorage.Location;
using System.Configuration;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Common;
using System.IO;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.HisHivTreatment.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;

namespace HIS.Desktop.Plugins.HisHivTreatment
{
    public partial class frmHivTreatment : HIS.Desktop.Utility.FormBase
    {
        #region Declaration
        internal MOS.EFMODEL.DataModels.HIS_TREATMENT currentTreatment = null;
        DelegateSelectData refeshReference;
        internal int ActionType = 0;
        Inventec.Desktop.Common.Modules.Module currentModule;
        HIS_HIV_TREATMENT currentHivTreatment = null;
        V_HIS_HIV_TREATMENT hivVTreatment = null;
        List<HIS_REGIMEN_HIV> beginRegimenHivNameSelecteds;
        List<HIS_REGIMEN_HIV> regimenHivNameSelecteds;
        List<ComboADO> hivPatientStatusSelecteds;
        ComboADO comboAdo = new ComboADO();

        int prescriptionArcDay = 0;
        bool isInit = false;
        int positionHandle = -1;
        #endregion

        #region Load

        public frmHivTreatment(Inventec.Desktop.Common.Modules.Module _Module, HIS_TREATMENT treatment, DelegateSelectData _refeshReference)
            : base(_Module)
        {
            try
            {
                InitializeComponent();
                this.currentTreatment = treatment;
                this.refeshReference = _refeshReference;
                this.currentModule = _Module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmHivTreatment_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetIcon();
                GetHivTreatment();
                SetCaptionByLanguageKey();
                SetDefaultData();
                ValidControls();
                FillDataToControlsForm();
                FillDataToControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetHivTreatment()
        {
            try
            {
                if (currentTreatment != null)
                {
                    CommonParam param = new CommonParam();
                    HisHivTreatmentFilter filter = new HisHivTreatmentFilter();
                    filter.TREATMENT_ID = currentTreatment.ID;
                    filter.IS_DELETE = 0;
                    var data = new BackendAdapter(param)
                        .Get<List<HIS_HIV_TREATMENT>>("api/HisHivTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                    if (data != null && data.Count > 0)
                    {
                        currentHivTreatment = data.OrderByDescending(o => o.ID).First();
                    }
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisHivTreatment.Resources.Lang", typeof(frmHivTreatment).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDelete.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.btnDelete.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox2.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.groupBox2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("frmHivTreatment.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHivTreatment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboHivTreatment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtArvTreatmentEnd.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.dtArvTreatmentEnd.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtArvTreatmentBegin.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.dtArvTreatmentBegin.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtTestPcrResult.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.dtTestPcrResult.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTestPcrResult.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboTestPcrResult.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTestPcrResult.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboTestPcrResult.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTestPcrTimes.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboTestPcrTimes.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTestPcrTimes.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboTestPcrTimes.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTestPcrRnaResult.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboTestPcrRnaResult.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTestPcrRnaResult.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboTestPcrRnaResult.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTestPcrRnaReason.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboTestPcrRnaReason.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTestPcrRnaReason.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboTestPcrRnaReason.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtTestPcrRnaResult.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.dtTestPcrRnaResult.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtTestPcrRna.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.dtTestPcrRna.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTuberculosisRegimen.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboTuberculosisRegimen.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnGetInfo.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.btnGetInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTuberculosisRegimen.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboTuberculosisRegimen.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTuberculosisTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboTuberculosisTreatmentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTuberculosisTreatmentType.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboTuberculosisTreatmentType.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtTuberculosisTreatmentEnd.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.dtTuberculosisTreatmentEnd.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtTuberculosisTreatmentBegin.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.dtTuberculosisTreatmentBegin.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRegimenLevel.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboRegimenLevel.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRegimenLevel.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboRegimenLevel.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRegimenHiv.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboRegimenHiv.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRegimenHiv.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboRegimenHiv.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtCtxTreatmentBegin.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.dtCtxTreatmentBegin.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtArvPatientBegin.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.dtArvPatientBegin.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRegimenHivBegin.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboRegimenHivBegin.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRegimenHivBegin.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboRegimenHivBegin.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBeginRegimenLevel.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboBeginRegimenLevel.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBeginRegimenLevel.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboBeginRegimenLevel.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentReason.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboTreatmentReason.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientStatus.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboPatientStatus.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHivTreatment.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem16.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem17.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem18.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem20.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem21.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem22.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTuberculosisTreatmentBegin.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem23.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTuberculosisTreatmentBegin.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTuberculosisTreatmentEnd.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem24.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTuberculosisTreatmentEnd.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem25.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem26.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTestPcrRna.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem27.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTestPcrRna.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTestPcrRnaResult.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem28.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTestPcrRnaResult.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem29.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem30.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem30.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem30.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem30.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem31.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem32.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem32.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTestPcrResult.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem33.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTestPcrResult.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTestPcr.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciArvTreatmentBegin.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem35.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciArvTreatmentBegin.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciArvTreatmentEnd.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem36.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciArvTreatmentEnd.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem36.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem37.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem37.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem9.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox1.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.groupBox1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem38.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem38.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNhomDoiTuong.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.lciNhomDoiTuong.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGiaiDoanlamSang.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.lciGiaiDoanlamSang.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKetQuaDieuTriLao.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.lciKetQuaDieuTriLao.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem40.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem40.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem39.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.layoutControlItem39.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNoiLayMauXN.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.lciNoiLayMauXN.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNoiXNKD.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.lciNoiXNKD.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNoiBDDTARV.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.lciNoiBDDTARV.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNoiLayMauXN.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.lciNoiLayMauXN.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNoiXNKD.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.lciNoiXNKD.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNoiBDDTARV.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHivTreatment.lciNoiBDDTARV.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDefaultData()
        {
            try
            {

                if (currentHivTreatment != null)
                {
                    this.lciBtnDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    ActionType = GlobalVariables.ActionEdit;
                }
                else
                {
                    this.lciBtnDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHivTreatment.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    ActionType = GlobalVariables.ActionAdd;
                }
                cboPatientStatus.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        #region Init combo
        void FillDataToGridLookupEdit(DevExpress.XtraEditors.GridLookUpEdit cboEditor, object datasource)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Value", "", 30, 1));
                columnInfos.Add(new ColumnInfo("Name", "", 270, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Value", columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboEditor, datasource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void InitHivPatientStatus()
        {

            InitCheck(cboPatientStatus, SelectionGrid__cboPatientStatus);
            InitCombo(cboPatientStatus, comboAdo.listHivPatientStatus(), "Name", "Value");
        }
        private void InitRegimenHivBegin()
        {
            InitCheck(cboRegimenHivBegin, SelectionGrid__cboRegimenHivBegin);
            InitCombo(cboRegimenHivBegin, BackendDataWorker.Get<HIS_REGIMEN_HIV>(), "REGIMEN_HIV_NAME", "REGIMEN_HIV_CODE");
        }
        private void InitRegimenHiv()
        {
            InitCheck(cboRegimenHiv, SelectionGrid__cboRegimenHiv);
            InitCombo(cboRegimenHiv, BackendDataWorker.Get<HIS_REGIMEN_HIV>(), "REGIMEN_HIV_NAME", "REGIMEN_HIV_CODE");
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col1 = cbo.Properties.View.Columns.AddField(ValueMember);
                col1.VisibleIndex = 1;
                col1.Width = 50;
                col1.Caption = " ";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 2;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 250;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboRegimenHivBegin(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_REGIMEN_HIV> sgSelectedNews = new List<HIS_REGIMEN_HIV>();
                    foreach (MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append("; "); }
                            sb.Append(rv.REGIMEN_HIV_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.beginRegimenHivNameSelecteds = new List<HIS_REGIMEN_HIV>();
                    this.beginRegimenHivNameSelecteds.AddRange(sgSelectedNews);

                }
                if (beginRegimenHivNameSelecteds != null && beginRegimenHivNameSelecteds.Count > 0 && beginRegimenHivNameSelecteds.Exists(o => o.REGIMEN_HIV_CODE.Equals("KHAC")))

                    cboBeginRegimenLevel.Enabled = true;
                else
                {
                    cboBeginRegimenLevel.EditValue = null;
                    cboBeginRegimenLevel.Enabled = false;
                }

                this.cboRegimenHivBegin.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__cboRegimenHiv(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_REGIMEN_HIV> sgSelectedNews = new List<HIS_REGIMEN_HIV>();
                    foreach (MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append("; "); }
                            sb.Append(rv.REGIMEN_HIV_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.regimenHivNameSelecteds = new List<HIS_REGIMEN_HIV>();
                    this.regimenHivNameSelecteds.AddRange(sgSelectedNews);
                }
                if (regimenHivNameSelecteds != null && regimenHivNameSelecteds.Count > 0 && regimenHivNameSelecteds.Exists(o => o.REGIMEN_HIV_CODE.Equals("KHAC")))

                    cboRegimenLevel.Enabled = true;
                else
                {
                    cboRegimenLevel.EditValue = null;
                    cboRegimenLevel.Enabled = false;
                }

                this.cboRegimenHiv.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__cboPatientStatus(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<ComboADO> sgSelectedNews = new List<ComboADO>();
                    foreach (ComboADO rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append("; "); }
                            sb.Append(rv.Name.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.hivPatientStatusSelecteds = new List<ComboADO>();
                    this.hivPatientStatusSelecteds.AddRange(sgSelectedNews);
                }

                this.cboPatientStatus.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        private void SetValueRegimen(GridLookUpEdit gridLookUpEdit, List<HIS_REGIMEN_HIV> listSelect, List<HIS_REGIMEN_HIV> listAll)
        {
            try
            {
                if (listSelect != null)
                {
                    gridLookUpEdit.Properties.DataSource = listAll;
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.ID == p.ID)).ToList();
                    GridCheckMarksSelection gridCheckMark = gridLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();
                    gridCheckMark.Selection.AddRange(selectFilter);
                }
                gridLookUpEdit.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataToControl()
        {
            try
            {
                if (currentTreatment != null)
                {
                    lblTreatmentCode.Text = currentTreatment.TREATMENT_CODE;
                    lblPatientName.Text = currentTreatment.TDL_PATIENT_NAME;
                    lblPatientGenderName.Text = currentTreatment.TDL_PATIENT_GENDER_NAME;
                    if (currentTreatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        lblDob.Text = currentTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentTreatment.TDL_PATIENT_DOB);
                    }
                    lblHeinCardNumber.Text = currentTreatment.TDL_HEIN_CARD_NUMBER;
                    if (currentTreatment.TDL_PATIENT_CCCD_NUMBER != null)
                    {
                        lblPatientCCCDNumber.Text = currentTreatment.TDL_PATIENT_CCCD_NUMBER;
                    }
                    else if (currentTreatment.TDL_PATIENT_CMND_NUMBER != null)
                    {
                        lblPatientCCCDNumber.Text = currentTreatment.TDL_PATIENT_CMND_NUMBER;
                    }
                    else
                    {
                        lblPatientCCCDNumber.Text = currentTreatment.TDL_PATIENT_PASSPORT_NUMBER;
                    }
                    lblPatientAddress.Text = currentTreatment.TDL_PATIENT_ADDRESS;
                    var dob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatment.TDL_PATIENT_DOB);
                    int age = ((DateTime.Now - (dob ?? DateTime.Now)).Days) / 30;
                    if (age < 18)
                    {
                        cboTestPcrTimes.Enabled = true;
                        dtTestPcr.Enabled = true;
                        dtTestPcrResult.Enabled = true;
                        cboTestPcrResult.Enabled = true;
                    }
                    if (currentHivTreatment != null)
                    {
                        this.isInit = true;
                        cboPatientType.EditValue = currentHivTreatment.HIV_PATIENT_TYPE;
                        ProcessSelectBusinessCombo(cboPatientStatus, currentHivTreatment.HIV_PATIENT_STATUS);
                        cboTreatmentReason.EditValue = currentHivTreatment.HIV_TREATMENT_REASON;
                        dtInfection.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.HIV_INFECTION_DATE ?? 0);
                        dtArvPatientBegin.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.ARV_PATIENT_BEGIN ?? 0);
                        ProcessSelectBusinessRegimen(cboRegimenHivBegin, currentHivTreatment.BEGIN_REGIMEN_HIV_CODE);
                        cboBeginRegimenLevel.EditValue = currentHivTreatment.BEGIN_REGIMEN_LEVEL;
                        dtPregnancy.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.DATE_OF_PREGNANCY ?? 0);
                        dtCtxTreatmentBegin.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.CTX_TREATMENT_BEGIN ?? 0);
                        ProcessSelectBusinessRegimen(cboRegimenHiv, currentHivTreatment.REGIMEN_HIV_CODE);
                        cboRegimenLevel.EditValue = currentHivTreatment.REGIMEN_LEVEL;
                        dtTuberculosisTreatmentBegin.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.TUBERCULOSIS_TREATMENT_BEGIN ?? 0);
                        dtTuberculosisTreatmentEnd.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.TUBERCULOSIS_TREATMENT_END ?? 0);
                        cboTuberculosisRegimen.EditValue = currentHivTreatment.TUBERCULOSIS_REGIMEN;
                        cboTuberculosisTreatmentType.EditValue = currentHivTreatment.TUBERCULOSIS_TREATMENT_TYPE;
                        dtTestPcrRna.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.TEST_PCR_RNA_DATE ?? 0);
                        dtTestPcrRnaResult.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.TEST_PCR_RNA_RESULT_DATE ?? 0);
                        cboTestPcrRnaReason.EditValue = currentHivTreatment.TEST_PCR_RNA_REASON;
                        cboTestPcrRnaResult.EditValue = currentHivTreatment.TEST_PCR_RNA_RESULT;
                        if (age < 18)
                        {
                            cboTestPcrTimes.EditValue = currentHivTreatment.TEST_PCR_TIMES;
                            dtTestPcr.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.TEST_PCR_DATE ?? 0);
                            dtTestPcrResult.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.TEST_PCR_RESULT_DATE ?? 0);
                            cboTestPcrResult.EditValue = currentHivTreatment.TEST_PCR_RESULT;
                        }
                        cboHivTreatment.EditValue = currentHivTreatment.HIV_TREATMENT_CODE;
                        dtArvTreatmentBegin.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.ARV_TREATMEN_BEGIN ?? 0);
                        dtArvTreatmentEnd.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.ARV_TREATMEN_END ?? 0);
                        spnPrescriptionArcDay.EditValue = currentHivTreatment.PRESCRIPTION_ARV_DAY;
                        cboRegimenHivReason.EditValue = currentHivTreatment.REGIMEN_HIV_REASON;
                        cboGroupPatient.EditValue = currentHivTreatment.GROUP_PATIENT;
                        cboClinicalStage.EditValue = currentHivTreatment.CLINICAL_STAGE;
                        cboTuberculosisTreatmentResult.EditValue = currentHivTreatment.TUBERCULOSIS_TREATMENT_RESULT;
                        cboTuberculosisScreening.EditValue = currentHivTreatment.TUBERCULOSIS_SCREENING;
                        txtTestSamplePlace.Text = currentHivTreatment.TEST_SAMPLE_PLACE;
                        txtTestPlaceKD.Text = currentHivTreatment.TEST_PLACE_KD;
                        txtArvPatientBeginAddress.Text = currentHivTreatment.ARV_PATIENT_BEGIN_PLACE;
                        dtRegimenHivDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.REGIMEN_HIV_DATE ?? 0);
                        this.isInit = false;
                    }
                    else
                    {
                        this.isInit = false;
                        cboPatientType.EditValue = null;
                        cboPatientStatus.EditValue = null;
                        cboTreatmentReason.EditValue = null;
                        dtInfection.EditValue = null;
                        dtArvPatientBegin.EditValue = null;
                        cboRegimenHivBegin.EditValue = null;
                        cboBeginRegimenLevel.EditValue = null;
                        dtPregnancy.EditValue = null;
                        dtCtxTreatmentBegin.EditValue = null;
                        cboRegimenHiv.EditValue = null;
                        cboRegimenLevel.EditValue = null;
                        dtTuberculosisTreatmentBegin.EditValue = null;
                        dtTuberculosisTreatmentEnd.EditValue = null;
                        cboTuberculosisRegimen.EditValue = null;
                        cboTuberculosisTreatmentType.EditValue = null;
                        dtTestPcrRna.EditValue = null;
                        dtTestPcrRnaResult.EditValue = null;
                        cboTestPcrRnaReason.EditValue = null;
                        cboTestPcrRnaResult.EditValue = null;
                        cboTestPcrTimes.EditValue = null;
                        dtTestPcr.EditValue = null;
                        dtTestPcrResult.EditValue = null;
                        cboTestPcrResult.EditValue = null;
                        cboHivTreatment.EditValue = null;
                        dtArvTreatmentBegin.EditValue = null;
                        dtArvTreatmentEnd.EditValue = null;
                        spnPrescriptionArcDay.EditValue = null;
                        cboBeginRegimenLevel.Enabled = false;
                        cboRegimenLevel.Enabled = false;
                        cboRegimenHivReason.EditValue = null;
                        cboGroupPatient.EditValue = null;
                        cboClinicalStage.EditValue = null;
                        cboTuberculosisTreatmentResult.EditValue = null;
                        cboTuberculosisScreening.EditValue = null;
                        txtTestSamplePlace.Text = null;
                        txtTestPlaceKD.Text = null;
                        txtArvPatientBeginAddress.Text = null;
                        dtRegimenHivDate.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnGetInfo_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisHivTreatmentViewFilter filter = new HisHivTreatmentViewFilter();
                filter.PATIENT_ID = currentTreatment.PATIENT_ID;
                var data = new BackendAdapter(param)
                    .Get<List<V_HIS_HIV_TREATMENT>>("api/HisHivTreatment/GetView", ApiConsumers.MosConsumer, filter, param);

                if (data != null && data.Count > 0)
                {
                    var currentTime = currentTreatment.IN_TIME;

                    var filterData = data
                        .Where(o => o.IN_TIME < currentTime)
                        .OrderByDescending(o => o.IN_TIME)
                        .ThenByDescending(o => o.TREATMENT_ID)
                        .ToList();

                    if (filterData != null && filterData.Count > 0)
                    {
                        hivVTreatment = filterData.FirstOrDefault();
                        FillDataToButton();
                    }
                    else
                    {
                        MessageManager.Show(Resources.ResourceMessage.KhongCoThongTin);
                    }
                }
                else
                {
                    MessageManager.Show(Resources.ResourceMessage.KhongCoThongTin);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void FillDataToButton()
        {
            try
            {
                if (currentTreatment != null)
                {
                    lblTreatmentCode.Text = currentTreatment.TREATMENT_CODE;
                    lblPatientName.Text = currentTreatment.TDL_PATIENT_NAME;
                    lblPatientGenderName.Text = currentTreatment.TDL_PATIENT_GENDER_NAME;
                    if (currentTreatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        lblDob.Text = currentTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentTreatment.TDL_PATIENT_DOB);
                    }
                    lblHeinCardNumber.Text = currentTreatment.TDL_HEIN_CARD_NUMBER;
                    if (currentTreatment.TDL_PATIENT_CCCD_NUMBER != null)
                    {
                        lblPatientCCCDNumber.Text = currentTreatment.TDL_PATIENT_CCCD_NUMBER;
                    }
                    else if (currentTreatment.TDL_PATIENT_CMND_NUMBER != null)
                    {
                        lblPatientCCCDNumber.Text = currentTreatment.TDL_PATIENT_CMND_NUMBER;
                    }
                    else
                    {
                        lblPatientCCCDNumber.Text = currentTreatment.TDL_PATIENT_PASSPORT_NUMBER;
                    }
                    lblPatientAddress.Text = currentTreatment.TDL_PATIENT_ADDRESS;
                    var dob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatment.TDL_PATIENT_DOB);
                    int age = ((DateTime.Now - (dob ?? DateTime.Now)).Days) / 30;
                    if (age < 18)
                    {
                        cboTestPcrTimes.Enabled = true;
                        dtTestPcr.Enabled = true;
                        dtTestPcrResult.Enabled = true;
                        cboTestPcrResult.Enabled = true;
                    }
                    if (hivVTreatment != null)
                    {
                        this.isInit = true;
                        cboPatientType.EditValue = hivVTreatment.HIV_PATIENT_TYPE;
                        ProcessSelectBusinessCombo(cboPatientStatus, hivVTreatment.HIV_PATIENT_STATUS);
                        cboTreatmentReason.EditValue = hivVTreatment.HIV_TREATMENT_REASON;
                        dtInfection.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hivVTreatment.HIV_INFECTION_DATE ?? 0);
                        dtArvPatientBegin.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hivVTreatment.ARV_PATIENT_BEGIN ?? 0);
                        ProcessSelectBusinessRegimen(cboRegimenHivBegin, hivVTreatment.BEGIN_REGIMEN_HIV_CODE);
                        cboBeginRegimenLevel.EditValue = hivVTreatment.BEGIN_REGIMEN_LEVEL;
                        dtPregnancy.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hivVTreatment.DATE_OF_PREGNANCY ?? 0);
                        dtCtxTreatmentBegin.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hivVTreatment.CTX_TREATMENT_BEGIN ?? 0);
                        ProcessSelectBusinessRegimen(cboRegimenHiv, hivVTreatment.REGIMEN_HIV_CODE);
                        cboRegimenLevel.EditValue = hivVTreatment.REGIMEN_LEVEL;
                        dtTuberculosisTreatmentBegin.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hivVTreatment.TUBERCULOSIS_TREATMENT_BEGIN ?? 0);
                        dtTuberculosisTreatmentEnd.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hivVTreatment.TUBERCULOSIS_TREATMENT_END ?? 0);
                        cboTuberculosisRegimen.EditValue = hivVTreatment.TUBERCULOSIS_REGIMEN;
                        cboTuberculosisTreatmentType.EditValue = hivVTreatment.TUBERCULOSIS_TREATMENT_TYPE;
                        dtTestPcrRna.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hivVTreatment.TEST_PCR_RNA_DATE ?? 0);
                        dtTestPcrRnaResult.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hivVTreatment.TEST_PCR_RNA_RESULT_DATE ?? 0);
                        cboTestPcrRnaReason.EditValue = hivVTreatment.TEST_PCR_RNA_REASON;
                        cboTestPcrRnaResult.EditValue = hivVTreatment.TEST_PCR_RNA_RESULT;
                        if (age < 18)
                        {
                            cboTestPcrTimes.EditValue = hivVTreatment.TEST_PCR_TIMES;
                            dtTestPcr.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hivVTreatment.TEST_PCR_DATE ?? 0);
                            dtTestPcrResult.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hivVTreatment.TEST_PCR_RESULT_DATE ?? 0);
                            cboTestPcrResult.EditValue = hivVTreatment.TEST_PCR_RESULT;
                        }
                        cboHivTreatment.EditValue = hivVTreatment.HIV_TREATMENT_CODE;
                        dtArvTreatmentBegin.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hivVTreatment.ARV_TREATMEN_BEGIN ?? 0);
                        dtArvTreatmentEnd.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hivVTreatment.ARV_TREATMEN_END ?? 0);
                        spnPrescriptionArcDay.EditValue = hivVTreatment.PRESCRIPTION_ARV_DAY;
                        cboRegimenHivReason.EditValue = currentHivTreatment.REGIMEN_HIV_REASON;
                        cboGroupPatient.EditValue = currentHivTreatment.GROUP_PATIENT;
                        cboClinicalStage.EditValue = currentHivTreatment.CLINICAL_STAGE;
                        cboTuberculosisTreatmentResult.EditValue = currentHivTreatment.TUBERCULOSIS_TREATMENT_RESULT;
                        cboTuberculosisScreening.EditValue = currentHivTreatment.TUBERCULOSIS_SCREENING;
                        txtTestSamplePlace.Text = currentHivTreatment.TEST_SAMPLE_PLACE;
                        txtTestPlaceKD.Text = currentHivTreatment.TEST_PLACE_KD;
                        txtArvPatientBeginAddress.Text = currentHivTreatment.ARV_PATIENT_BEGIN_PLACE;
                        dtRegimenHivDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHivTreatment.REGIMEN_HIV_DATE ?? 0);
                        this.isInit = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectBusinessRegimen(GridLookUpEdit cbo, string p)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                gridCheckMark.ClearSelection(cbo.Properties.View);
                if (!String.IsNullOrWhiteSpace(p) && cbo.Properties.Tag != null)
                {
                    List<HIS_REGIMEN_HIV> ds = cbo.Properties.DataSource as List<HIS_REGIMEN_HIV>;
                    string[] arrays = p.Split(';');
                    if (arrays != null && arrays.Length > 0)
                    {
                        List<HIS_REGIMEN_HIV> selects = new List<HIS_REGIMEN_HIV>();
                        foreach (var item in arrays)
                        {
                            var row = ds != null ? ds.FirstOrDefault(o => o.REGIMEN_HIV_CODE.ToString() == item) : null;
                            if (row != null)
                            {
                                selects.Add(row);
                            }
                        }
                        gridCheckMark.SelectAll(selects);
                    }
                }
                else
                {
                    cbo.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cbo.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cbo.Properties.View);
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessSelectBusinessCombo(GridLookUpEdit cbo, string p)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                gridCheckMark.ClearSelection(cbo.Properties.View);
                if (!String.IsNullOrWhiteSpace(p) && cbo.Properties.Tag != null)
                {
                    List<ComboADO> ds = cbo.Properties.DataSource as List<ComboADO>;
                    string[] arrays = p.Split(';');
                    if (arrays != null && arrays.Length > 0)
                    {
                        List<ComboADO> selects = new List<ComboADO>();
                        foreach (var item in arrays)
                        {
                            var row = ds != null ? ds.FirstOrDefault(o => o.Value.ToString() == item) : null;
                            if (row != null)
                            {
                                selects.Add(row);
                            }
                        }
                        gridCheckMark.SelectAll(selects);
                    }
                }
                else
                {
                    cbo.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cbo.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cbo.Properties.View);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitComboGroupPatient()
        {
            List<HIS_HIV_GROUP_PATIENT> result = new List<HIS_HIV_GROUP_PATIENT>();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisHivGroupPatientFilter filter = new MOS.Filter.HisHivGroupPatientFilter()
                {
                    IS_ACTIVE = 1
                };
                var rs = new BackendAdapter(param).Get<List<HIS_HIV_GROUP_PATIENT>>("api/HisHivGroupPatient/Get", ApiConsumers.MosConsumer, filter, param);
                if(rs != null)
                {
                    result = rs;
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HIV_GROUP_PATIENT_CODE", "", 30, 1));
                columnInfos.Add(new ColumnInfo("HIV_GROUP_PATIENT_NAME", "", 270, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HIV_GROUP_PATIENT_NAME", "HIV_GROUP_PATIENT_CODE", columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboGroupPatient, result, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }
        public void FillDataToControlsForm()
        {
            try
            {
                FillDataToGridLookupEdit(this.cboPatientType, comboAdo.listHivPatientType());
                FillDataToGridLookupEdit(this.cboTreatmentReason, comboAdo.listHivTreatmentReason());
                FillDataToGridLookupEdit(this.cboBeginRegimenLevel, comboAdo.listRegimenLevel());
                FillDataToGridLookupEdit(this.cboRegimenLevel, comboAdo.listRegimenLevel());
                FillDataToGridLookupEdit(this.cboTuberculosisRegimen, comboAdo.listTuberculosisRegimen());
                FillDataToGridLookupEdit(this.cboTuberculosisTreatmentType, comboAdo.listTuberculosisTreatmentType());
                FillDataToGridLookupEdit(this.cboTestPcrRnaReason, comboAdo.listTestReason());
                FillDataToGridLookupEdit(this.cboTestPcrRnaResult, comboAdo.listTestPcrRnaResult());
                FillDataToGridLookupEdit(this.cboTestPcrTimes, comboAdo.listTestTime());
                FillDataToGridLookupEdit(this.cboTestPcrResult, comboAdo.listTestPcrResult());
                FillDataToGridLookupEdit(this.cboHivTreatment, comboAdo.listHivTreatment());
                FillDataToGridLookupEdit(this.cboRegimenHivReason, comboAdo.listRegimenHivReason());
                InitComboGroupPatient();
                //FillDataToGridLookupEdit(this.cboGroupPatient, GetDataGroup());
                FillDataToGridLookupEdit(this.cboClinicalStage, comboAdo.listClinicalStage());
                FillDataToGridLookupEdit(this.cboTuberculosisTreatmentResult, comboAdo.listTuberculosisTreatmentResult());
                FillDataToGridLookupEdit(this.cboTuberculosisScreening, comboAdo.listTuberculosisScreening());
                InitRegimenHivBegin();
                InitRegimenHiv();
                InitHivPatientStatus();
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void cboRegimenHivBegin_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append("; "); }

                    sb.Append(rv.REGIMEN_HIV_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRegimenHiv_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append("; "); }

                    sb.Append(rv.REGIMEN_HIV_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientStatus_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (ComboADO rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append("; "); }

                    sb.Append(rv.Name.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_HIV_TREATMENT updateDTO = new MOS.EFMODEL.DataModels.HIS_HIV_TREATMENT();
                if (currentTreatment != null)
                {
                    if (this.currentHivTreatment != null && this.currentHivTreatment.ID > 0)
                    {
                        LoadCurrent(this.currentHivTreatment.ID, ref updateDTO);
                    }
                    UpdateDTOFromDataForm(ref updateDTO);
                    if (ActionType == GlobalVariables.ActionAdd)
                    {
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_HIV_TREATMENT>("api/HisHivTreatment/Create", ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            this.currentHivTreatment = resultData;
                            SetDefaultData();
                            FillDataToControl();
                        }
                    }
                    else
                    {
                        updateDTO.ID = this.currentHivTreatment.ID;
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_HIV_TREATMENT>("api/HisHivTreatment/Update", ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            this.currentHivTreatment = resultData;
                            SetDefaultData();
                            FillDataToControl();
                        }
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<HIS_HIV_TREATMENT>();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_HIV_TREATMENT currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisHivTreatmentFilter filter = new HisHivTreatmentFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_HIV_TREATMENT>>("api/HisHivTreatment/GetById", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_HIV_TREATMENT hivTreatmentDTO)
        {
            try
            {
                hivTreatmentDTO.TREATMENT_ID = currentTreatment.ID;
                if (cboPatientType.EditValue != null)
                    hivTreatmentDTO.HIV_PATIENT_TYPE = Inventec.Common.TypeConvert.Parse.ToInt16(cboPatientType.EditValue.ToString());
                else
                    hivTreatmentDTO.HIV_PATIENT_TYPE = null;

                if (hivPatientStatusSelecteds != null && hivPatientStatusSelecteds.Count > 0)
                {
                    hivTreatmentDTO.HIV_PATIENT_STATUS = String.Join(";", hivPatientStatusSelecteds.Select(o => o.Value).ToList());
                }
                else
                    hivTreatmentDTO.HIV_PATIENT_STATUS = null;

                if (cboTreatmentReason.EditValue != null)
                    hivTreatmentDTO.HIV_TREATMENT_REASON = Inventec.Common.TypeConvert.Parse.ToInt16(cboTreatmentReason.EditValue.ToString());
                else
                    hivTreatmentDTO.HIV_TREATMENT_REASON = null;

                if (dtInfection.EditValue != null && dtInfection.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.HIV_INFECTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtInfection.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.HIV_INFECTION_DATE = null;

                if (dtArvPatientBegin.EditValue != null && dtArvPatientBegin.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.ARV_PATIENT_BEGIN = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtArvPatientBegin.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.ARV_PATIENT_BEGIN = null;

                if (beginRegimenHivNameSelecteds != null && beginRegimenHivNameSelecteds.Count > 0)
                {
                    hivTreatmentDTO.BEGIN_REGIMEN_HIV_CODE = String.Join(";", beginRegimenHivNameSelecteds.Select(o => o.REGIMEN_HIV_CODE).ToList());
                }
                else
                    hivTreatmentDTO.BEGIN_REGIMEN_HIV_CODE = null;

                if (cboBeginRegimenLevel.EditValue != null)
                    hivTreatmentDTO.BEGIN_REGIMEN_LEVEL = Inventec.Common.TypeConvert.Parse.ToInt16(cboBeginRegimenLevel.EditValue.ToString());
                else
                    hivTreatmentDTO.BEGIN_REGIMEN_LEVEL = null;

                if (dtPregnancy.EditValue != null && dtPregnancy.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.DATE_OF_PREGNANCY = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtPregnancy.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.DATE_OF_PREGNANCY = null;

                if (dtCtxTreatmentBegin.EditValue != null && dtCtxTreatmentBegin.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.CTX_TREATMENT_BEGIN = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtCtxTreatmentBegin.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.CTX_TREATMENT_BEGIN = null;

                if (regimenHivNameSelecteds != null && regimenHivNameSelecteds.Count > 0)
                {

                    hivTreatmentDTO.REGIMEN_HIV_CODE = String.Join(";", regimenHivNameSelecteds.Select(o => o.REGIMEN_HIV_CODE).ToList());
                }
                else
                    hivTreatmentDTO.REGIMEN_HIV_CODE = null;

                if (cboRegimenLevel.EditValue != null)
                    hivTreatmentDTO.REGIMEN_LEVEL = Inventec.Common.TypeConvert.Parse.ToInt16(cboRegimenLevel.EditValue.ToString());
                else
                    hivTreatmentDTO.REGIMEN_LEVEL = null;

                if (dtTuberculosisTreatmentBegin.EditValue != null && dtTuberculosisTreatmentBegin.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.TUBERCULOSIS_TREATMENT_BEGIN = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTuberculosisTreatmentBegin.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.TUBERCULOSIS_TREATMENT_BEGIN = null;

                if (dtTuberculosisTreatmentEnd.EditValue != null && dtTuberculosisTreatmentEnd.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.TUBERCULOSIS_TREATMENT_END = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTuberculosisTreatmentEnd.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.TUBERCULOSIS_TREATMENT_END = null;

                if (cboTuberculosisRegimen.EditValue != null)
                    hivTreatmentDTO.TUBERCULOSIS_REGIMEN = Inventec.Common.TypeConvert.Parse.ToInt16(cboTuberculosisRegimen.EditValue.ToString());
                else
                    hivTreatmentDTO.TUBERCULOSIS_REGIMEN = null;

                if (cboTuberculosisTreatmentType.EditValue != null)
                    hivTreatmentDTO.TUBERCULOSIS_TREATMENT_TYPE = Inventec.Common.TypeConvert.Parse.ToInt16(cboTuberculosisTreatmentType.EditValue.ToString());
                else
                    hivTreatmentDTO.TUBERCULOSIS_TREATMENT_TYPE = null;

                if (dtTestPcrRna.EditValue != null && dtTestPcrRna.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.TEST_PCR_RNA_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTestPcrRna.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.TEST_PCR_RNA_DATE = null;

                if (dtTestPcrRnaResult.EditValue != null && dtTestPcrRnaResult.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.TEST_PCR_RNA_RESULT_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTestPcrRnaResult.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.TEST_PCR_RNA_RESULT_DATE = null;

                if (cboTestPcrRnaReason.EditValue != null)
                    hivTreatmentDTO.TEST_PCR_RNA_REASON = Inventec.Common.TypeConvert.Parse.ToInt16(cboTestPcrRnaReason.EditValue.ToString());
                else
                    hivTreatmentDTO.TEST_PCR_RNA_REASON = null;

                if (cboTestPcrRnaResult.EditValue != null)
                    hivTreatmentDTO.TEST_PCR_RNA_RESULT = Inventec.Common.TypeConvert.Parse.ToInt16(cboTestPcrRnaResult.EditValue.ToString());
                else
                    hivTreatmentDTO.TEST_PCR_RNA_RESULT = null;

                if (cboTestPcrTimes.EditValue != null)
                    hivTreatmentDTO.TEST_PCR_TIMES = Inventec.Common.TypeConvert.Parse.ToInt16(cboTestPcrTimes.EditValue.ToString());
                else
                    hivTreatmentDTO.TEST_PCR_TIMES = null;

                if (dtTestPcr.EditValue != null && dtTestPcr.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.TEST_PCR_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTestPcr.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.TEST_PCR_DATE = null;

                if (dtTestPcrResult.EditValue != null && dtTestPcrResult.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.TEST_PCR_RESULT_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTestPcrResult.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.TEST_PCR_RESULT_DATE = null;

                if (dtTestPcrResult.EditValue != null && dtTestPcrResult.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.TEST_PCR_RESULT_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTestPcrResult.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.TEST_PCR_RESULT_DATE = null;

                if (cboTestPcrResult.EditValue != null)
                    hivTreatmentDTO.TEST_PCR_RESULT = Inventec.Common.TypeConvert.Parse.ToInt16(cboTestPcrResult.EditValue.ToString());
                else
                    hivTreatmentDTO.TEST_PCR_RESULT = null;

                if (cboHivTreatment.EditValue != null)
                    hivTreatmentDTO.HIV_TREATMENT_CODE = Inventec.Common.TypeConvert.Parse.ToInt16(cboHivTreatment.EditValue.ToString());
                else
                    hivTreatmentDTO.HIV_TREATMENT_CODE = null;

                if (dtArvTreatmentBegin.EditValue != null && dtArvTreatmentBegin.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.ARV_TREATMEN_BEGIN = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtArvTreatmentBegin.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.ARV_TREATMEN_BEGIN = null;

                if (dtArvTreatmentEnd.EditValue != null && dtArvTreatmentEnd.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.ARV_TREATMEN_END = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtArvTreatmentEnd.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.ARV_TREATMEN_END = null;

                if (spnPrescriptionArcDay.EditValue != null)
                    hivTreatmentDTO.PRESCRIPTION_ARV_DAY = Inventec.Common.TypeConvert.Parse.ToInt16(spnPrescriptionArcDay.EditValue.ToString());
                else
                    hivTreatmentDTO.PRESCRIPTION_ARV_DAY = null;

                if (dtRegimenHivDate.EditValue != null && dtRegimenHivDate.DateTime != DateTime.MinValue)
                {
                    hivTreatmentDTO.REGIMEN_HIV_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtRegimenHivDate.EditValue.ToString()).ToString("yyyyMMdd") + "000000");
                }
                else
                    hivTreatmentDTO.REGIMEN_HIV_DATE = null;

                if (cboRegimenHivReason.EditValue != null)
                    hivTreatmentDTO.REGIMEN_HIV_REASON = Inventec.Common.TypeConvert.Parse.ToInt16(cboRegimenHivReason.EditValue.ToString());
                else
                    hivTreatmentDTO.REGIMEN_HIV_REASON = null;

                if (cboGroupPatient.EditValue != null)
                    hivTreatmentDTO.GROUP_PATIENT = cboGroupPatient.EditValue.ToString();
                else
                    hivTreatmentDTO.GROUP_PATIENT = null;

                if (cboClinicalStage.EditValue != null)
                    hivTreatmentDTO.CLINICAL_STAGE = Inventec.Common.TypeConvert.Parse.ToInt16(cboClinicalStage.EditValue.ToString());
               else
                    hivTreatmentDTO.CLINICAL_STAGE = null;

                if (cboTuberculosisTreatmentResult.EditValue != null)
                    hivTreatmentDTO.TUBERCULOSIS_TREATMENT_RESULT = Inventec.Common.TypeConvert.Parse.ToInt16(cboTuberculosisTreatmentResult.EditValue.ToString());
                else
                    hivTreatmentDTO.TUBERCULOSIS_TREATMENT_RESULT = null;

                if (cboTuberculosisScreening.EditValue != null)
                    hivTreatmentDTO.TUBERCULOSIS_SCREENING = Inventec.Common.TypeConvert.Parse.ToInt16(cboTuberculosisScreening.EditValue.ToString());
                else
                    hivTreatmentDTO.TUBERCULOSIS_SCREENING = null;

                if (txtArvPatientBeginAddress.Text != null)
                    hivTreatmentDTO.ARV_PATIENT_BEGIN_PLACE = txtArvPatientBeginAddress.Text;
                else
                    hivTreatmentDTO.ARV_PATIENT_BEGIN_PLACE = null;

                if (txtTestPlaceKD.Text != null)
                    hivTreatmentDTO.TEST_PLACE_KD = txtTestPlaceKD.Text;
                else
                    hivTreatmentDTO.TEST_PLACE_KD = null;

                if (txtTestSamplePlace.Text != null)
                    hivTreatmentDTO.TEST_SAMPLE_PLACE = txtTestSamplePlace.Text;
                else
                    hivTreatmentDTO.TEST_SAMPLE_PLACE = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>("api/HisHivTreatment/Delete", ApiConsumers.MosConsumer, currentHivTreatment.ID, param);
                    if (success)
                    {
                        BackendDataWorker.Reset<HIS_HIV_TREATMENT>();
                        this.currentHivTreatment = null;
                        SetDefaultData();
                        FillDataToControl();
                        this.Close();
                    }
                    MessageManager.Show(this, param, success);
                }
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region Event combo

        private void cboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPatientType.EditValue == null)
                {
                    cboPatientType.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboPatientType.Properties.Buttons[1].Visible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentReason_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTreatmentReason.EditValue == null)
                {
                    cboTreatmentReason.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboTreatmentReason.Properties.Buttons[1].Visible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBeginRegimenLevel_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBeginRegimenLevel.EditValue == null)
                {
                    cboBeginRegimenLevel.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboBeginRegimenLevel.Properties.Buttons[1].Visible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRegimenLevel_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboRegimenLevel.EditValue == null)
                {
                    cboRegimenLevel.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboRegimenLevel.Properties.Buttons[1].Visible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTuberculosisRegimen_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTuberculosisRegimen.EditValue == null)
                {
                    cboTuberculosisRegimen.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboTuberculosisRegimen.Properties.Buttons[1].Visible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTuberculosisTreatmentType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTuberculosisTreatmentType.EditValue == null)
                {
                    cboTuberculosisTreatmentType.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboTuberculosisTreatmentType.Properties.Buttons[1].Visible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestPcrRnaReason_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTestPcrRnaReason.EditValue == null)
                {
                    cboTestPcrRnaReason.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboTestPcrRnaReason.Properties.Buttons[1].Visible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestPcrRnaResult_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTestPcrRnaResult.EditValue == null)
                {
                    cboTestPcrRnaResult.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboTestPcrRnaResult.Properties.Buttons[1].Visible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestPcrTimes_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTestPcrTimes.EditValue == null)
                {
                    cboTestPcrTimes.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboTestPcrTimes.Properties.Buttons[1].Visible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboTestPcrResult_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTestPcrResult.EditValue == null)
                {
                    cboTestPcrResult.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboTestPcrResult.Properties.Buttons[1].Visible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentReason_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTreatmentReason.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBeginRegimenLevel_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBeginRegimenLevel.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRegimenLevel_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRegimenLevel.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTuberculosisRegimen_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTuberculosisRegimen.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTuberculosisTreatmentType_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTuberculosisTreatmentType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestPcrRnaReason_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTestPcrRnaReason.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestPcrRnaResult_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTestPcrRnaResult.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestPcrTimes_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTestPcrTimes.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestPcrResult_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTestPcrResult.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHivTreatment_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHivTreatment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPatientStatus.Focus();
                    cboPatientStatus.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboTreatmentReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtInfection.Focus();
                    dtInfection.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInfection_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtArvPatientBegin.Focus();
                    dtArvPatientBegin.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtArvPatientBegin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBeginRegimenLevel.Focus();
                    cboBeginRegimenLevel.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBeginRegimenLevel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtPregnancy.Focus();
                    dtPregnancy.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtPregnancy_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtCtxTreatmentBegin.Focus();
                    dtCtxTreatmentBegin.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtCtxTreatmentBegin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRegimenLevel.Focus();
                    cboRegimenLevel.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRegimenLevel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtTuberculosisTreatmentBegin.Focus();
                    dtTuberculosisTreatmentBegin.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTuberculosisTreatmentBegin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtTuberculosisTreatmentEnd.Focus();
                    dtTuberculosisTreatmentEnd.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTuberculosisTreatmentEnd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTuberculosisTreatmentType.Focus();
                    cboTuberculosisTreatmentType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTuberculosisTreatmentType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtTestPcrRna.Focus();
                    dtTestPcrRna.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTestPcrRna_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtTestPcrRnaResult.Focus();
                    dtTestPcrRnaResult.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTestPcrRnaResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTestPcrRnaReason.Focus();
                    cboTestPcrRnaReason.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTestPcrRnaReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTestPcrRnaResult.Focus();
                    cboTestPcrRnaResult.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTestPcrRnaResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTestPcrTimes.Enabled)
                    {
                        cboTestPcrTimes.Focus();
                        cboTestPcrTimes.ShowPopup();
                    }
                    else
                    {
                        cboHivTreatment.Focus();
                        cboHivTreatment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTestPcrTimes_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtTestPcr.Focus();
                    dtTestPcr.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTestPcr_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtTestPcrResult.Focus();
                    dtTestPcrResult.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTestPcrResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTestPcrResult.Focus();
                    cboTestPcrResult.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTestPcrResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboHivTreatment.Focus();
                    cboHivTreatment.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHivTreatment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtArvTreatmentBegin.Focus();
                    dtArvTreatmentBegin.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtArvTreatmentBegin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtArvTreatmentEnd.Focus();
                    dtArvTreatmentEnd.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtArvTreatmentEnd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnPrescriptionArcDay.Focus();
                    spnPrescriptionArcDay.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnPrescriptionArcDay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
        private void spnPrescriptionArcDay_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!(e.KeyChar != '-'))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHivTreatment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboHivTreatment.EditValue != null)
                {
                    cboHivTreatment.Properties.Buttons[1].Visible = true;
                    if (cboHivTreatment.EditValue.ToString() == "1")
                    {
                        dtArvTreatmentBegin.Enabled = true;
                        dtArvTreatmentEnd.Enabled = true;
                        spnPrescriptionArcDay.Enabled = true;
                    }
                    else
                    {
                        dtArvTreatmentBegin.Enabled = false;
                        dtArvTreatmentEnd.Enabled = false;
                        spnPrescriptionArcDay.Enabled = false;
                        dtArvTreatmentBegin.EditValue = null;
                        dtArvTreatmentEnd.EditValue = null;
                        spnPrescriptionArcDay.EditValue = null;
                    }
                }
                else
                {
                    cboTuberculosisTreatmentType.Properties.Buttons[1].Visible = false;
                    dtArvTreatmentBegin.Enabled = false;
                    dtArvTreatmentEnd.Enabled = false;
                    spnPrescriptionArcDay.Enabled = false;
                    dtArvTreatmentBegin.EditValue = null;
                    dtArvTreatmentEnd.EditValue = null;
                    spnPrescriptionArcDay.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTuberculosisTreatmentBegin_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!CompareBeginDateWithEndDate(dtTuberculosisTreatmentBegin, dtTuberculosisTreatmentEnd))
                {
                    XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongDuocLonHon, lciTuberculosisTreatmentBegin.Text, lciTuberculosisTreatmentEnd.Text), Resources.ResourceMessage.ThongBao);
                    dtTuberculosisTreatmentBegin.EditValue = null;
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTestPcrRna_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!CompareBeginDateWithEndDate(dtTestPcrRna, dtTestPcrRnaResult))
                {
                    XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongDuocLonHon, lciTestPcrRna.Text, lciTestPcrRnaResult.Text), Resources.ResourceMessage.ThongBao);
                    dtTestPcrRna.EditValue = null;
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void dtTestPcrRnaResult_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!CompareBeginDateWithEndDate(dtTestPcrRna, dtTestPcrRnaResult))
                {
                    XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongDuocLonHon, lciTestPcrRna.Text, lciTestPcrRnaResult.Text), Resources.ResourceMessage.ThongBao);
                    dtTestPcrRnaResult.EditValue = null;
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void dtTestPcr_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!CompareBeginDateWithEndDate(dtTestPcr, dtTestPcrResult))
                {
                    XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongDuocLonHon, lciTestPcr.Text, lciTestPcrResult.Text), Resources.ResourceMessage.ThongBao);
                    dtTestPcr.EditValue = null;
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTestPcrResult_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!CompareBeginDateWithEndDate(dtTestPcr, dtTestPcrResult))
                {
                    XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongDuocLonHon, lciTestPcr.Text, lciTestPcrResult.Text), Resources.ResourceMessage.ThongBao);
                    dtTestPcrResult.EditValue = null;
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTuberculosisTreatmentEnd_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!CompareBeginDateWithEndDate(dtTuberculosisTreatmentBegin, dtTuberculosisTreatmentEnd))
                {
                    XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongDuocLonHon, lciTuberculosisTreatmentBegin.Text, lciTuberculosisTreatmentEnd.Text), Resources.ResourceMessage.ThongBao);
                    dtTuberculosisTreatmentEnd.EditValue = null;
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtArvTreatmentBegin_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!CompareBeginDateWithEndDate(dtArvTreatmentBegin, dtArvTreatmentEnd))
                {
                    XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongDuocLonHon, lciArvTreatmentBegin.Text, lciArvTreatmentEnd.Text), Resources.ResourceMessage.ThongBao);
                    dtArvTreatmentBegin.EditValue = null;
                    return;
                }
                this.CalculateDayNum();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtArvTreatmentEnd_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!CompareBeginDateWithEndDate(dtArvTreatmentBegin, dtArvTreatmentEnd))
                {
                    XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongDuocLonHon, lciArvTreatmentBegin.Text, lciArvTreatmentEnd.Text), Resources.ResourceMessage.ThongBao);
                    dtArvTreatmentEnd.EditValue = null;
                    return;
                }
                this.CalculateDayNum();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void spnPrescriptionArcDay_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spnPrescriptionArcDay.EditValue != null)
                {
                    if (isInit)
                    {
                        prescriptionArcDay = (int)spnPrescriptionArcDay.Value;
                        return;
                    }
                    if ((int)spnPrescriptionArcDay.Value > prescriptionArcDay)
                    {
                        XtraMessageBox.Show(Resources.ResourceMessage.SoNgayCapThuocKhongDuocLonHon, Resources.ResourceMessage.ThongBao);
                        spnPrescriptionArcDay.EditValue = null;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        private void CalculateDayNum()
        {
            try
            {
                if (isInit)
                    return;
                prescriptionArcDay = 0;
                if (dtArvTreatmentBegin.EditValue != null && dtArvTreatmentBegin.DateTime != DateTime.MinValue
                    && dtArvTreatmentEnd.EditValue != null && dtArvTreatmentEnd.DateTime != DateTime.MinValue
                    && dtArvTreatmentBegin.DateTime.Date <= dtArvTreatmentEnd.DateTime.Date)
                {
                    TimeSpan ts = (TimeSpan)(dtArvTreatmentEnd.DateTime.Date - dtArvTreatmentBegin.DateTime.Date);
                    prescriptionArcDay = ts.Days;
                    spnPrescriptionArcDay.Value = ts.Days;
                }
                else
                {
                    spnPrescriptionArcDay.EditValue = 0;
                }

                prescriptionArcDay = (int)spnPrescriptionArcDay.EditValue;
                spnPrescriptionArcDay.EditValue = 0;

                prescriptionArcDay = (int)spnPrescriptionArcDay.EditValue;
                {
                    spnPrescriptionArcDay.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool CompareBeginDateWithEndDate(DateEdit dtBegin, DateEdit dtEnd)
        {
            bool result = true;
            try
            {
                if (dtBegin.EditValue != null && dtBegin.DateTime != DateTime.MinValue
                    && dtEnd.EditValue != null && dtEnd.DateTime != DateTime.MinValue
                    && dtBegin.DateTime.Date > dtEnd.DateTime.Date)
                    return result = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        #region validate
        private void ValidControls()
        {
            try
            {
                ValidBidControlMaxlength(txtTestSamplePlace, 5);
                ValidBidControlMaxlength(txtTestPlaceKD, 5);
                ValidBidControlMaxlength(txtArvPatientBeginAddress, 5);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBidControlMaxlength(DevExpress.XtraEditors.TextEdit control, int maxlength)
        {
            try
            {
                Validation.MaxLengthValidationRule _rule = new Validation.MaxLengthValidationRule();
                _rule.txtEdit = control;
                _rule.maxlength = maxlength;
                _rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(control, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion validate

        private void dtRegimenHivDate_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    dtRegimenHivDate.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
