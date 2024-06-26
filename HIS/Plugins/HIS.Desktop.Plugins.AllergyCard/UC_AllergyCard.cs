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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.Message;
using MPS.Processor.Mps000253.PDO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;

namespace HIS.Desktop.Plugins.AllergyCard
{
    public partial class UC_AllergyCard : UserControl
    {
        long treatmentID;
        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_TREATMENT currentTreatment;
        V_HIS_ALLERGY_CARD allergyCard;
        V_HIS_PATIENT currentPatient;

        string loginName;

        public UC_AllergyCard()
        {
            InitializeComponent();
        }

        public UC_AllergyCard(Inventec.Desktop.Common.Modules.Module module, long treatmentId)
        {
            InitializeComponent();
            try
            {
                this.treatmentID = treatmentId;
                this.currentModule = module;
                this.currentTreatment = GetTreatment(treatmentId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public UC_AllergyCard(Inventec.Desktop.Common.Modules.Module module, V_HIS_PATIENT patient)
        {
            InitializeComponent();
            try
            {
                this.currentPatient = patient;
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UC_AllergyCard_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SetCaptionByLanguageKey();
                FillDataTreatmentToForm(this.currentTreatment);
                FillDataTreatmentToForm(this.currentPatient);
                LoadDataGridAllergyCard();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #region Event

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                var data = GetTreatmentByCode(txtTreatmentCode.Text);
                if (data != null)
                {
                    List<object> listObj = new List<object>();
                    listObj.Add(data.ID);

                    CallModule callModule = new CallModule(CallModule.AllergyCardCreate, this.currentModule.RoomId, this.currentModule.RoomTypeId, listObj);
                    LoadDataGridAllergyCard();
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy thông tin hồ sơ điều trị tương ứng với mã điều trị: " + txtTreatmentCode.Text, "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtPatientCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    LoadDataGridAllergyCard();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtTreatmentCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    LoadDataGridAllergyCard();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtKeyWord_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    LoadDataGridAllergyCard();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void toggleSwitchGomNhom_Toggled(object sender, EventArgs e)
        {
            try
            {
                gridViewTheDiUng_SelectionChanged(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewTheDiUng_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.FieldName == "Edit")
                        {
                            var row = (V_HIS_ALLERGY_CARD)gridViewTheDiUng.GetRow(hi.RowHandle);
                            if (row != null)
                            {
                                Btn_Edit_Enable_ButtonClick(row);
                            }
                        }
                        else if (hi.Column.FieldName == "Print")
                        {
                            var row = (V_HIS_ALLERGY_CARD)gridViewTheDiUng.GetRow(hi.RowHandle);
                            if (row != null)
                            {
                                Btn_Print_Enable_ButtonClick(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event AllergyCard

        private void gridViewTheDiUng_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_ALLERGY_CARD data = (MOS.EFMODEL.DataModels.V_HIS_ALLERGY_CARD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "ISSUE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.ISSUE_TIME);
                        }
                        if (e.Column.FieldName == "DOCTOR_STR")
                        {
                            e.Value = data.DIAGNOSE_LOGINNAME + " - " + data.DIAGNOSE_USERNAME;
                        }
                        if (e.Column.FieldName == "DOB_STR")
                        {
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            {
                                if (data.TDL_PATIENT_DOB > 0)
                                {
                                    e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                                }
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewTheDiUng_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_ALLERGY_CARD)gridViewTheDiUng.GetRow(e.RowHandle);

                    if (e.Column.FieldName == "Edit")//duyệt
                    {
                        if (data.DIAGNOSE_LOGINNAME == loginName || data.CREATOR == loginName)
                        {
                            e.RepositoryItem = Btn_Edit_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = Btn_Edit_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewTheDiUng_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //e.ControllerRow
                List<V_HIS_ALLERGY_CARD> listAllergyCard = new List<V_HIS_ALLERGY_CARD>();
                int[] selectRow = gridViewTheDiUng.GetSelectedRows();
                if (selectRow != null && selectRow.Count() > 0)
                {
                    foreach (var item in selectRow)
                    {
                        V_HIS_ALLERGY_CARD dataRow = (V_HIS_ALLERGY_CARD)gridViewTheDiUng.GetRow(item);
                        if (dataRow != null)
                        {
                            listAllergyCard.Add(dataRow);
                        }
                    }
                }
                LoadDataGridAllergenic(listAllergyCard);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_Edit_Enable_ButtonClick(V_HIS_ALLERGY_CARD row)
        {
            try
            {
                if (row != null && (row.DIAGNOSE_LOGINNAME == loginName || row.CREATOR == loginName))
                {
                    List<object> listObj = new List<object>();
                    listObj.Add(row);

                    CallModule callModule = new CallModule(CallModule.AllergyCardCreate, this.currentModule.RoomId, this.currentModule.RoomTypeId, listObj);
                    LoadDataGridAllergyCard();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_Print_Enable_ButtonClick(V_HIS_ALLERGY_CARD row)
        {
            try
            {
                if (row != null)
                {
                    this.allergyCard = row;
                    PrintTypeCare type = new PrintTypeCare();
                    type = PrintTypeCare.IN_THE_DI_UNG;
                    PrintProcess(type);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event Allergenic

        private void gridViewAllergenic_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_ALLERGENIC data = (MOS.EFMODEL.DataModels.V_HIS_ALLERGENIC)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "DOUBT")
                        {
                            e.Value = data.IS_DOUBT == 1 ? true : false;
                        }
                        if (e.Column.FieldName == "SURE")
                        {
                            e.Value = data.IS_SURE == 1 ? true : false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion

        #region Method

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AllergyCard.Resources.Lang", typeof(HIS.Desktop.Plugins.AllergyCard.UC_AllergyCard).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCard.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCreate.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCard.btnCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.ToolTip = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.ToolTip = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.ToolTip = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn6.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("UC_AllergyCard.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCard.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCard.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCard.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private V_HIS_TREATMENT GetTreatment(long _treatmentID)
        {
            V_HIS_TREATMENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = _treatmentID;
                var treatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (treatment != null && treatment.Count > 0)
                {
                    result = treatment.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }

        private void FillDataTreatmentToForm(V_HIS_PATIENT data)
        {
            try
            {
                if (data != null)
                {
                    btnCreate.Enabled = false;
                    lblPatientCode.Text = data.PATIENT_CODE;
                    lblTenBenhNhan.Text = data.VIR_PATIENT_NAME;
                    lblGioiTinh.Text = data.GENDER_NAME;
                    lblNgaySinh.Text = data.DOB > 0 ? data.DOB.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                    lblDiaChi.Text = data.VIR_ADDRESS;
                    txtTreatmentCode.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FillDataTreatmentToForm(V_HIS_TREATMENT data)
        {
            try
            {
                if (data != null)
                {
                    txtTreatmentCode.Text = data.TREATMENT_CODE;
                    txtTreatmentCode.Enabled = false;
                    lblPatientCode.Text = data.TDL_PATIENT_CODE;
                    lblTenBenhNhan.Text = data.TDL_PATIENT_NAME;
                    lblGioiTinh.Text = data.TDL_PATIENT_GENDER_NAME;
                    lblNgaySinh.Text = data.TDL_PATIENT_DOB > 0 ? data.TDL_PATIENT_DOB.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    lblDiaChi.Text = data.TDL_PATIENT_ADDRESS;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataGridAllergyCard()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisAllergyCardViewFilter filter = new HisAllergyCardViewFilter();

                SetFilterAllergyCard(ref filter);

                var allergyCard = new BackendAdapter(param).Get<List<V_HIS_ALLERGY_CARD>>(HisRequestUriStore.HIS_ALLERGY_CARD_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                gridControlTheDiUng.DataSource = null;
                gridControlTheDiUng.BeginUpdate();
                gridControlTheDiUng.DataSource = allergyCard;
                gridControlTheDiUng.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetFilterAllergyCard(ref HisAllergyCardViewFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "ISSUE_TIME";
                if (this.currentPatient != null)
                {
                    filter.PATIENT_ID = this.currentPatient.ID;
                }

                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }
                else
                {
                    filter.KEY_WORD = txtKeyWord.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void LoadDataGridAllergenic(List<V_HIS_ALLERGY_CARD> data)
        {
            try
            {
                if (data != null && data.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisAllergenicViewFilter filter = new HisAllergenicViewFilter();
                    filter.ALLERGY_CARD_IDs = data.Select(o => o.ID).ToList();
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";

                    var allergenic = new BackendAdapter(param).Get<List<V_HIS_ALLERGENIC>>(HisRequestUriStore.HIS_ALLERGEIC_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                    gridControlAllergenic.DataSource = null;
                    gridControlAllergenic.BeginUpdate();

                    if (toggleSwitchGomNhom.IsOn)
                    {
                        List<V_HIS_ALLERGENIC> listDataRs = new List<V_HIS_ALLERGENIC>();
                        if (allergenic != null && allergenic.Count > 0)
                        {
                            var groupAllergenic = allergenic.GroupBy(o => new { o.ALLERGENIC_NAME, o.IS_DOUBT }).ToList();
                            foreach (var item in groupAllergenic)
                            {
                                V_HIS_ALLERGENIC aller = new V_HIS_ALLERGENIC();
                                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_ALLERGENIC>(aller, item.First());
                                aller.CLINICAL_EXPRESSION = "";
                                foreach (var subItem in item)
                                {
                                    if (!string.IsNullOrEmpty(subItem.CLINICAL_EXPRESSION))
                                        aller.CLINICAL_EXPRESSION += subItem.CLINICAL_EXPRESSION + "; ";
                                }
                                listDataRs.Add(aller);
                            }
                        }
                        gridControlAllergenic.DataSource = listDataRs;
                    }
                    else
                    {
                        gridControlAllergenic.DataSource = allergenic;
                    }
                    gridControlAllergenic.EndUpdate();
                }
                else
                {
                    gridControlAllergenic.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private HIS_TREATMENT GetTreatmentByCode(string treatmentCode)
        {
            HIS_TREATMENT rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.TREATMENT_CODE__EXACT = treatmentCode;

                var treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                if (treatment != null && treatment.Count > 0)
                {
                    rs = treatment.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        #endregion

        #region Print

        internal enum PrintTypeCare
        {
            IN_THE_DI_UNG,
        }

        void PrintProcess(PrintTypeCare printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeCare.IN_THE_DI_UNG:
                        richEditorMain.RunPrintTemplate("Mps000253", DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000253":
                        LoadInTheDiUng(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadInTheDiUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (allergyCard != null)
                {
                    V_HIS_PATIENT patientPrint = new V_HIS_PATIENT();
                    V_HIS_TREATMENT treatmentPrint = new V_HIS_TREATMENT();
                    Mps000253ADO adoPrint = new Mps000253ADO();
                    V_HIS_ALLERGY_CARD allergenyCardPrint = new V_HIS_ALLERGY_CARD();
                    List<V_HIS_ALLERGENIC> listAllergenicPrint = new List<V_HIS_ALLERGENIC>();

                    allergenyCardPrint = allergyCard;
                    listAllergenicPrint = GetAllergenicPrint(allergyCard);

                    if (allergenyCardPrint != null)
                    {
                        treatmentPrint = GetTreatment(allergenyCardPrint.TREATMENT_ID);
                    }
                    adoPrint.DEPARTMENT_NAME = WorkPlace.GetDepartmentName();

                    if (treatmentPrint != null)
                    {
                        patientPrint = GetPatientPrint(treatmentPrint.PATIENT_ID);
                    }

                    MPS.Processor.Mps000253.PDO.Mps000253PDO mps000253PDO = new MPS.Processor.Mps000253.PDO.Mps000253PDO(treatmentPrint, patientPrint, allergenyCardPrint, adoPrint, listAllergenicPrint);

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatmentPrint != null ? treatmentPrint.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                    MPS.ProcessorBase.Core.PrintData PrintData = null;

                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                    {
                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000253PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter[printTypeCode]){ EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000253PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, GlobalVariables.dicPrinter[printTypeCode]){ EmrInputADO = inputADO };
                        }
                    }
                    else
                    {
                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000253PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""){ EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000253PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, ""){ EmrInputADO = inputADO };
                        }
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_ALLERGENIC> GetAllergenicPrint(V_HIS_ALLERGY_CARD data)
        {
            List<V_HIS_ALLERGENIC> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisAllergenicViewFilter filter = new HisAllergenicViewFilter();
                filter.ALLERGY_CARD_ID = data.ID;

                result = new BackendAdapter(param).Get<List<V_HIS_ALLERGENIC>>(HisRequestUriStore.HIS_ALLERGEIC_GETVIEW, ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return result;
        }

        private V_HIS_PATIENT GetPatientPrint(long patientID)
        {
            V_HIS_PATIENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisPatientViewFilter filter = new HisPatientViewFilter();
                filter.ID = patientID;

                var getResult = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                if (getResult != null && getResult.Count > 0)
                {
                    result = getResult.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return result;

        }

        #endregion

    }
}
