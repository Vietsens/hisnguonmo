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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.ADO;
using SCN.Filter;
using SCN.EFMODEL.DataModels;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using TYT.Filter;
using TYT.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisPatient
{
    public partial class UCHisPatient : UserControl
    {
        #region declare
        Inventec.Desktop.Common.Modules.Module currentModule;
        //long PatientID;
        List<V_HIS_TREATMENT> CurrentTreatment = new List<V_HIS_TREATMENT>();
        V_HIS_PATIENT CurrentPatient = new V_HIS_PATIENT();
        V_HIS_TREATMENT rowCellClick { get; set; }
        int lastRowHandle = -1;
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;
        #endregion

        public UCHisPatient(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_PATIENT _Patient)
        {
            CurrentPatient = _Patient;
            this.currentModule = currentModule;
            InitializeComponent();
        }

        private void UCHisPatient_Load(object sender, EventArgs e)
        {
            LoadGrid();

            LoadDefault();

            LoadLanguage();

            GridControlTreatment.ToolTipController = toolTipController;

            CheckExistValue();
        }

        private void CheckExistValue()
        {
            checkbtndeath();
            checkbtnTYTTuberculosis();
            checkbtnNerves();
            checkbtnHiv();
            if (this.CurrentPatient.GENDER_ID == 1)
            {
                checkbtnTYTFetusExam();
                checkbtnTYTFetusBorn();
                checkbtnTYTKhh();
                checkbtnFetusAbortion();
            }
        }

        #region check
        void checkbtndeath()
        {
            try
            {
                CommonParam param = new CommonParam();
                TytDeathFilter filter = new TytDeathFilter();
                filter.PATIENT_CODE__EXACT = this.CurrentPatient.PATIENT_CODE;
                List<TYT_DEATH> data = new BackendAdapter(param).Get<List<TYT_DEATH>>("api/TytDeath/Get", ApiConsumers.TytConsumer, filter, null);
                if (data.Count > 0)
                {
                    btnTytDeathCreate.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
            }
        }

        void checkbtnTYTTuberculosis()
        {
            try
            {
                CommonParam param = new CommonParam();
                TytTuberculosisFilter filter = new TytTuberculosisFilter();
                filter.PATIENT_CODE__EXACT = this.CurrentPatient.PATIENT_CODE;
                List<TYT_TUBERCULOSIS> data = new BackendAdapter(param).Get<List<TYT_TUBERCULOSIS>>("api/TYTTuberculosis/Get", ApiConsumers.TytConsumer, filter, null);
                if (data.Count > 0)
                {
                    btnTYTTuberculosis.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
            }
        }

        void checkbtnNerves()
        {
            try
            {
                CommonParam param = new CommonParam();
                TytNervesFilter filter = new TytNervesFilter();
                filter.PATIENT_CODE__EXACT = this.CurrentPatient.PATIENT_CODE;
                List<TYT_NERVES> data = new BackendAdapter(param).Get<List<TYT_NERVES>>("api/TYTNerves/Get", ApiConsumers.TytConsumer, filter, null);
                if (data.Count > 0)
                {
                    btnNerves.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
            }
        }

        void checkbtnHiv()
        {
            try
            {
                CommonParam param = new CommonParam();
                TytHivFilter filter = new TytHivFilter();
                filter.PATIENT_CODE__EXACT = this.CurrentPatient.PATIENT_CODE;
                List<TYT_HIV> data = new BackendAdapter(param).Get<List<TYT_HIV>>("api/TYTHiv/Get", ApiConsumers.TytConsumer, filter, null);
                if (data.Count > 0)
                {
                    btnTytHivCreate.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
            }
        }

        void checkbtnTYTFetusExam()
        {
            try
            {
                CommonParam param = new CommonParam();
                TytFetusExamFilter filter = new TytFetusExamFilter();
                filter.PATIENT_CODE__EXACT = this.CurrentPatient.PATIENT_CODE;
                List<TYT_FETUS_EXAM> data = new BackendAdapter(param).Get<List<TYT_FETUS_EXAM>>("api/TytFetusExam/Get", ApiConsumers.TytConsumer, filter, null);
                if (data.Count > 0)
                {
                    btnTYTFetusExam.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
            }
        }

        void checkbtnTYTFetusBorn()
        {
            try
            {
                CommonParam param = new CommonParam();
                TytFetusBornFilter filter = new TytFetusBornFilter();
                filter.PATIENT_CODE__EXACT = this.CurrentPatient.PATIENT_CODE;
                List<TYT_FETUS_BORN> data = new BackendAdapter(param).Get<List<TYT_FETUS_BORN>>("api/TytFetusBorn/Get", ApiConsumers.TytConsumer, filter, null);
                if (data.Count > 0)
                {
                    btnTYTFetusBorn.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
            }
        }

        void checkbtnTYTKhh()
        {
            try
            {
                CommonParam param = new CommonParam();
                TytKhhFilter filter = new TytKhhFilter();
                filter.PATIENT_CODE__EXACT = this.CurrentPatient.PATIENT_CODE;
                List<TYT_KHH> data = new BackendAdapter(param).Get<List<TYT_KHH>>("api/TytKhh/Get", ApiConsumers.TytConsumer, filter, null);
                if (data.Count > 0)
                {
                    btnTYTKhh.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
            }
        }

        void checkbtnFetusAbortion()
        {
            try
            {
                CommonParam param = new CommonParam();
                TytFetusAbortionFilter filter = new TytFetusAbortionFilter();
                filter.PATIENT_CODE__EXACT = this.CurrentPatient.PATIENT_CODE;
                List<TYT_FETUS_ABORTION> data = new BackendAdapter(param).Get<List<TYT_FETUS_ABORTION>>("api/TytFetusAbortion/Get", ApiConsumers.TytConsumer, filter, null);
                if (data.Count > 0)
                {
                    btnFetusAbortion.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        private void LoadLanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisPatient.Resources.Lang", typeof(HIS.Desktop.Plugins.HisPatient.UCHisPatient).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn26.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclStt3.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.grclStt3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclVitaminACode.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.grclVitaminACode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.graclMedicineName.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.graclMedicineName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.REQUEST_TIME_STR.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.REQUEST_TIME_STR.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton4.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.simpleButton4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTreatmentHistory.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.btnTreatmentHistory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTytDeathCreate.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.btnTytDeathCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFetusAbortion.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.btnFetusAbortion.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTYTKhh.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.btnTYTKhh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTYTFetusBorn.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.btnTYTFetusBorn.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTYTTuberculosis.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.btnTYTTuberculosis.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTYTFetusExam.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.btnTYTFetusExam.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTytHivCreate.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.btnTytHivCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btn.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.btn.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNerves.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.btnNerves.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclStt2.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.grclStt2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclSTT.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.grclSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclVP.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.grclVP.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCHisPatient.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("UCHisPatient.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region private

        private void LoadGrid()
        {
            LoadGridVitamin();
            LoadGridTreatment();
            LoadGridVaccination();
            LoadGridNutrition();
        }

        private void LoadDefault()
        {
            txtPatientCode.EditValue = this.CurrentPatient.PATIENT_CODE;
            txtBHYT.EditValue = this.CurrentPatient.TDL_HEIN_CARD_NUMBER;
            txtPatientName.EditValue = this.CurrentPatient.VIR_PATIENT_NAME;
            txtPersonCode.EditValue = this.CurrentPatient.PERSON_CODE;
            if (this.CurrentPatient.IS_HAS_NOT_DAY_DOB != 1)
            {
                txtborn.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.CurrentPatient.DOB);
            }
            else
            {
                txtborn.EditValue = this.CurrentPatient.DOB.ToString().Substring(0,4);
            }
            cboGender.EditValue = this.CurrentPatient.GENDER_ID == 1 ? "Nữ" : "Nam";
            txtAddress.EditValue = this.CurrentPatient.VIR_ADDRESS;
            txtCMT.EditValue = this.CurrentPatient.CMND_NUMBER;
            if (this.CurrentPatient.AVATAR_URL != null && this.CurrentPatient.AVATAR_URL != "")
            {
                var stream = Inventec.Fss.Client.FileDownload.GetFile(this.CurrentPatient.AVATAR_URL);
                picAvatar.Image = Image.FromStream(stream);
                picAvatar.Image.Tag = this.CurrentPatient.AVATAR_URL;
            }
            else
            {
                string pathLocal = GetPathDefault();
                picAvatar.Image = Image.FromFile(pathLocal);
            }
            if (this.CurrentPatient.BHYT_URL != null && this.CurrentPatient.BHYT_URL != "")
            {
                var stream = Inventec.Fss.Client.FileDownload.GetFile(this.CurrentPatient.BHYT_URL);
                picBHYT.EditValue = Image.FromStream(stream);
                picBHYT.Image.Tag = this.CurrentPatient.BHYT_URL;
            }
            else
            {
                string pathLocal = GetPathDefault();
                picBHYT.Image = Image.FromFile(pathLocal);
            }
            if (this.CurrentPatient.GENDER_ID == 2)
            {
                btnTYTFetusExam.Enabled = false;
                btnTYTFetusBorn.Enabled = false;
                btnTYTKhh.Enabled = false;
                btnFetusAbortion.Enabled = false;
            }
            else
            {
                btnTYTFetusExam.Enabled = true;
                btnTYTFetusBorn.Enabled = true;
                btnTYTKhh.Enabled = true;
                btnFetusAbortion.Enabled = true;

            }
            LoadCardCode();
        }

        private void LoadCardCode()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisCardFilter filter = new HisCardFilter();
                filter.PATIENT_ID = this.CurrentPatient.ID;
                List<HIS_CARD> data = new BackendAdapter(param).Get<List<HIS_CARD>>("api/HisCard/GetView", ApiConsumers.MosConsumer, filter, null);
                if (data != null && data.Count > 0)
                {
                    textEdit5.EditValue = data.FirstOrDefault().CARD_CODE;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private string GetPathDefault()
        {

            string imageDefaultPath = string.Empty;

            try
            {

                string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

                imageDefaultPath = localPath + "\\Img\\ImageStorage\\notImage.jpg";

            }

            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);

            }

            return imageDefaultPath;

        }
        #endregion

        #region Grid

        #region GridNutrition
        private void LoadGridNutrition()
        {
            try
            {
                CommonParam param = new CommonParam();
                ScnNutritionFilter filter = new ScnNutritionFilter();
                filter.PERSON_CODE__EXACT = this.CurrentPatient.PERSON_CODE;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                var data = new BackendAdapter(param).Get<List<SCN_NUTRITION>>("api/ScnNutrition/Get", ApiConsumers.ScnConsumer, filter, null);
                gridControlNutrition.DataSource = null;
                if (this.CurrentTreatment != null)
                {
                    gridControlNutrition.DataSource = data;
                }
            }
            catch (Exception ex)
            {
            }
        }


        private void gridViewNutrition_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SCN_NUTRITION data = (SCN_NUTRITION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "MEASURE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MEASURE_TIME);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void gridViewNutrition_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    SCN.EFMODEL.DataModels.SCN_NUTRITION data = (SCN.EFMODEL.DataModels.SCN_NUTRITION)gridViewNutrition.GetRow(e.RowHandle);
                    if (data == null)
                        return;
                    if (e.Column.FieldName == "IS_HEIGHT_SDD_CHK")
                    {
                        if (data.IS_HEIGHT_SDD == 1)
                        {
                            e.RepositoryItem = rbtnHEIGHT;
                        }
                    }
                    else if (e.Column.FieldName == "IS_WEIGHT_SDD_CHK")
                    {
                        if (data.IS_WEIGHT_SDD == 1)
                        {
                            e.RepositoryItem = rbtnWEIGHT;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region GridVitaminA
        private void LoadGridVitamin()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisVitaminAViewFilter filter = new HisVitaminAViewFilter();
                filter.PATIENT_ID = this.CurrentPatient.ID;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                var data = new BackendAdapter(param).Get<List<V_HIS_VITAMIN_A>>("api/HisVitaminA/GetView", ApiConsumers.MosConsumer, filter, null);
                gridControlVitaminA.DataSource = null;
                if (this.CurrentTreatment != null)
                {
                    gridControlVitaminA.DataSource = data;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void gridViewVitaminA_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A data = (MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "REQUEST_NAME")
                        {
                            if (!string.IsNullOrEmpty(data.REQUEST_LOGINNAME))
                            {
                                e.Value = data.REQUEST_LOGINNAME + " - " + data.REQUEST_USERNAME;
                            }
                            else
                            {
                                e.Value = data.REQUEST_USERNAME;
                            }
                        }
                        else if (e.Column.FieldName == "EXECUTE_NAME")
                        {
                            if (!string.IsNullOrEmpty(data.EXECUTE_LOGINNAME))
                            {
                                e.Value = data.EXECUTE_LOGINNAME + " - " + data.EXECUTE_USERNAME;
                            }
                            else
                            {
                                e.Value = data.EXECUTE_USERNAME;
                            }
                        }
                        else if (e.Column.FieldName == "REQUEST_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.REQUEST_TIME);
                        }
                        else if (e.Column.FieldName == "EXECUTE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        #endregion

        #region GridVaccination
        private void LoadGridVaccination()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisVaccinationFilter filter = new HisVaccinationFilter();
                filter.PATIENT_ID = this.CurrentPatient.ID;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                var data = new BackendAdapter(param).Get<List<V_HIS_VACCINATION>>("api/HisVaccination/GetView", ApiConsumers.MosConsumer, filter, null);
                gridControlVaccination.DataSource = null;
                if (this.CurrentTreatment != null)
                {
                    gridControlVaccination.DataSource = data;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void gridViewVaccination_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_VACCINATION data = (MOS.EFMODEL.DataModels.V_HIS_VACCINATION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "REQUEST_NAME")
                        {
                            if (!string.IsNullOrEmpty(data.REQUEST_LOGINNAME))
                            {
                                e.Value = data.REQUEST_LOGINNAME + " - " + data.REQUEST_USERNAME;
                            }
                            else
                            {
                                e.Value = data.REQUEST_USERNAME;
                            }
                        }
                        else if (e.Column.FieldName == "EXECUTE_NAME")
                        {
                            if (!string.IsNullOrEmpty(data.EXECUTE_LOGINNAME))
                            {
                                e.Value = data.EXECUTE_LOGINNAME + " - " + data.EXECUTE_USERNAME;
                            }
                            else
                            {
                                e.Value = data.EXECUTE_USERNAME;
                            }
                        }
                        else if (e.Column.FieldName == "REQUEST_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.REQUEST_TIME);
                        }
                        else if (e.Column.FieldName == "EXECUTE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion

        #region GridTreatment
        private void LoadGridTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                param.Start = 0;
                param.Limit = 5;
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.PATIENT_ID = this.CurrentPatient.ID;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "CREATE_TIME";
                this.CurrentTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, null);
                GridControlTreatment.DataSource = null;
                if (this.CurrentTreatment != null)
                {
                    GridControlTreatment.DataSource = this.CurrentTreatment;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void btnTreatmentHistory_Click(object sender, EventArgs e)
        {

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_TREATMENT data = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "IN_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            //e.Value = HIS.Desktop.Utility.AgeHelper.CalculateAgeFromYear(data.DOB) + "  tuổi";
                        }
                        else if (e.Column.FieldName == "ICD_DISPLAY")
                        {
                            if (!string.IsNullOrEmpty(data.ICD_CODE))
                            {
                                if (!String.IsNullOrEmpty(data.ICD_NAME))
                                {
                                    e.Value = data.ICD_CODE + " - " + data.ICD_NAME;
                                }
                            }
                        }
                        else if (e.Column.FieldName == "ICD_TEXT_DISPLAY")
                        {
                            if (!String.IsNullOrEmpty(data.ICD_TEXT))
                            {
                                e.Value = data.ICD_SUB_CODE + " - " + data.ICD_TEXT;
                            }
                        }
                        else if (e.Column.FieldName == "STATUS_DISPLAY")
                        {
                            #region --- STATUS ---
                            short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_PAUSE ?? -1).ToString());
                            decimal status_islock = Inventec.Common.TypeConvert.Parse.ToDecimal((data.IS_ACTIVE ?? -1).ToString());
                            short status_islockhein = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_LOCK_HEIN ?? -1).ToString());
                            //Status
                            //1- dang dieu tri
                            //2- da ket thuc
                            //3- khóa hồ sơ
                            //4- duyệt bhyt
                            if (status_islockhein != 1)
                            {
                                if (status_islock == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                {
                                    if (status_ispause != 1)
                                    {
                                        e.Value = imageList1.Images[0];
                                    }
                                    else
                                    {
                                        e.Value = imageList1.Images[1];
                                    }
                                }
                                else
                                {
                                    e.Value = imageList1.Images[2];
                                }
                            }
                            else
                            {
                                e.Value = imageList1.Images[3];
                            }
                            #endregion
                        }
                        else if (e.Column.FieldName == "YDT_DISPLAY")
                        {
                            if (data.IS_YDT_UPLOAD == 1)
                                e.Value = imageList1.Images[4];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
        {
            try
            {
                rowCellClick = new V_HIS_TREATMENT();
                rowCellClick = (V_HIS_TREATMENT)gridView1.GetFocusedRow();
                if (rowCellClick != null)
                {
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AggrHospitalFees'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(rowCellClick.ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == GridControlTreatment)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = GridControlTreatment.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "STATUS_DISPLAY")
                            {
                                short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_PAUSE") ?? "-1").ToString());
                                decimal status_islock = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(lastRowHandle, "IS_ACTIVE") ?? "-1").ToString());
                                short status_islockhein = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_LOCK_HEIN") ?? "-1").ToString());
                                //Status
                                //1- dang dieu tri
                                //2- da ket thuc
                                //3- khóa hồ sơ
                                //4- duyệt bhyt
                                if (status_islockhein != 1)
                                {
                                    if (status_islock == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                    {
                                        if (status_ispause != 1)
                                            text = "Đang điều trị";
                                        else
                                            text = "Đã kết thúc điều trị";
                                    }
                                    else
                                        text = "Đã duyệt khóa tài chính";
                                }
                                else
                                    text = "Đã duyệt khóa bảo hiểm";
                            }
                            else if (info.Column.FieldName == "YDT_DISPLAY")
                            {
                                if (Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_YDT_UPLOAD") ?? "-1").ToString()) == 1)
                                {
                                    text = "Đã đẩy thông tin lên hệ thống y bạ điện tử";
                                }
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region EventClickButtom
        private void btnTytDeathCreate_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "TYT.Desktop.Plugins.TytDeathCreate").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TytDeathCreate'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.CurrentPatient);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTreatmentHistory_Click_1(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentHistory").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentHistory'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    TreatmentHistoryADO treatmentAdo = new TreatmentHistoryADO();
                    treatmentAdo.patient_code = this.CurrentPatient.PATIENT_CODE;
                    treatmentAdo.patientId = this.CurrentPatient.ID;
                    listArgs.Add(treatmentAdo);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void simpleButton4_Click_1(object sender, EventArgs e)
        {

        }

        private void btnNerves_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "TYT.Desktop.Plugins.Nerves").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'TYT.Desktop.Plugins.Nerves'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    //var currentRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    //TytNerverADO ado = new TytNerverADO(currentRoom.BRANCH_CODE, this.CurrentPatient.PATIENT_CODE);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.CurrentPatient);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTYTTuberculosis_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "TYT.Desktop.Plugins.TYTTuberculosis").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'TYT.Desktop.Plugins.TYTTuberculosis'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.CurrentPatient);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTYTMalaria_Click(object sender, EventArgs e)
        {

        }

        private void btnTytHivCreate_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "TYT.Desktop.Plugins.TytHivCreate").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'TYT.Desktop.Plugins.TytHivCreate'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.CurrentPatient);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTYTFetusExam_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "TYT.Desktop.Plugins.TYTFetusExam").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'TYT.Desktop.Plugins.TYTFetusExam'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.CurrentPatient);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTYTFetusBorn_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "TYT.Desktop.Plugins.TYTFetusBorn").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'TYT.Desktop.Plugins.TYTFetusBorn'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.CurrentPatient);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTYTKhh_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "TYT.Desktop.Plugins.TYTKhh").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'TYT.Desktop.Plugins.TYTKhh'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.CurrentPatient);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFetusAbortion_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "TYT.Desktop.Plugins.FetusAbortion").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'TYT.Desktop.Plugins.FetusAbortion'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.CurrentPatient);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {

        }
        #endregion


    }
}
