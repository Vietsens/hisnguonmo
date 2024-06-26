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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MobaBloodCreate.ADO;
using HIS.Desktop.Plugins.MobaBloodCreate.Base;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HIS.Desktop.Plugins.MobaBloodCreate
{
    public partial class frmMobaBloodCreate : HIS.Desktop.Utility.FormBase
    {
        long expMestId;
        V_HIS_EXP_MEST hisExpMest = null;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        bool isCheckAll;
        int positionHandle = -1;
        List<ExpMestBloodADO> listExpMestBloodADO = new List<ExpMestBloodADO>();

        HisImpMestResultSDO resultMobaSdo = null;

        public frmMobaBloodCreate(Inventec.Desktop.Common.Modules.Module module, long data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.expMestId = data;
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void frmMobaBloodCreate_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfigCFG.LoadConfig();
                WaitingManager.Show();
                LoadKeyFrmLanguage();
                gridColumn_IsCheck.Image = imageCollectionService.Images[0];
                LoadExpMest();
                LoadExpMestBlood();
                if (HisConfigCFG.IsTrackingRequired)
                {
                    this.lciTracking.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    ValidationSingleControl(cboTracking);
                }
                SetComboTracking();
                btnSave.Enabled = true;
                btnPrint.Enabled = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMest()
        {
            try
            {
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this.expMestId;
                var hisExpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, expMestFilter, null);
                if (hisExpMests != null && hisExpMests.Count == 1)
                {
                    hisExpMest = hisExpMests.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestBlood()
        {
            try
            {
                if (this.hisExpMest != null)
                {
                    CommonParam param = new CommonParam();
                    HisExpMestBloodViewFilter medicineFilter = new HisExpMestBloodViewFilter()
                    {
                        EXP_MEST_ID = this.hisExpMest.ID,
                    };
                    var listExpMestBlood = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>(RequestUri.HIS_EXP_MEST_BLOOD_GET_VIEW, ApiConsumers.MosConsumer, medicineFilter, param);
                    listExpMestBloodADO = new List<ExpMestBloodADO>();
                    foreach (var item in listExpMestBlood)
                    {
                        ExpMestBloodADO ado = new ExpMestBloodADO(item);
                        ado.IsCheck = false;
                        listExpMestBloodADO.Add(ado);
                    }
                    gridControlExpMestBlood.BeginUpdate();
                    gridControlExpMestBlood.DataSource = listExpMestBloodADO;
                    gridControlExpMestBlood.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetComboTracking()
        {
            try
            {

                CommonParam param = new CommonParam();
                long departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                HisTrackingFilter filter = new HisTrackingFilter();
                filter.TREATMENT_ID = hisExpMest.TDL_TREATMENT_ID;
                filter.DEPARTMENT_ID = departmentId;
                List<HIS_TRACKING> trackings = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, filter, param);

                trackings = trackings.OrderByDescending(o => o.TRACKING_TIME).ToList();

                List<TrackingADO> trackingADOs = new List<TrackingADO>();
                foreach (var item in trackings)
                {
                    TrackingADO tracking = new TrackingADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TrackingADO>(tracking, item);
                    tracking.TRACKING_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(tracking.TRACKING_TIME);
                    trackingADOs.Add(tracking);
                }
                trackingADOs = trackingADOs.OrderByDescending(o => o.TRACKING_TIME).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TRACKING_TIME_STR", "Thời gian", 250, 1));
                columnInfos.Add(new ColumnInfo("CREATOR", "Người tạo", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TRACKING_TIME_STR", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboTracking, trackingADOs, controlEditorADO);
                cboTracking.Properties.ImmediatePopup = true;
                cboTracking.EditValue = null;
                if (HisConfigCFG.IsTrackingRequired)
                {
                    var startDay = Inventec.Common.DateTime.Get.StartDay() ?? 0;
                    var endDay = Inventec.Common.DateTime.Get.EndDay() ?? 0;
                    if (trackingADOs != null && trackingADOs.Count > 0)
                    {
                        foreach (var item in trackingADOs)
                        {
                            if (item.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                                && item.TRACKING_TIME <= endDay && item.TRACKING_TIME >= startDay)
                            {
                                cboTracking.EditValue = item.ID;
                                break;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void CalculTotalPrice(List<ExpMestBloodADO> listExpMestBloodADOSelect)
        {
            try
            {
                decimal totalPrice = 0;
                decimal totalFeePrice = 0;
                decimal totalVatPrice = 0;
                if (listExpMestBloodADOSelect != null && listExpMestBloodADOSelect.Count > 0)
                {
                    if (listExpMestBloodADOSelect != null && listExpMestBloodADOSelect.Count > 0)
                    {
                        totalFeePrice += listExpMestBloodADOSelect.Sum(s => (s.IMP_PRICE));
                        totalVatPrice += listExpMestBloodADOSelect.Sum(s => (s.IMP_PRICE * s.IMP_VAT_RATIO));
                    }
                    totalVatPrice = Math.Round(totalVatPrice, 0);
                    totalPrice = totalFeePrice + totalVatPrice;
                    lblTotalFeePrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalFeePrice, 0);
                    lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 0);
                    lblTotalVatPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalVatPrice, 0);
                }
                else
                {
                    lblTotalFeePrice.Text = "";
                    lblTotalPrice.Text = "";
                    lblTotalVatPrice.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBlood_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ExpMestBloodADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "VAT_RATIO_PLUS")
                        {
                            try
                            {
                                e.Value = data.IMP_VAT_RATIO * 100;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "VIR_TOTAL_IMP_PRICE")
                        {
                            try
                            {
                                e.Value = data.IMP_PRICE * (1 + data.IMP_VAT_RATIO);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBlood_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "IsCheck")
                    {
                        short isAllowChecked = Convert.ToInt16((gridViewExpMestBlood.GetRowCellValue(e.RowHandle, "IS_TH") ?? "").ToString());
                        if (isAllowChecked == 1)
                        {
                            e.RepositoryItem = CheckEdit_IsCheckDisable;
                        }
                        else
                        {
                            e.RepositoryItem = this.CheckEdit_IsCheckEnable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBlood_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBlood_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            try
            {
                //GridView view = sender as GridView;
                //if (e.RowHandle < 0)
                //    return;
                //var data = (ExpMestBloodADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                //if (data == null)
                //    return;
                //if (data.IS_TH == 1)
                //{
                //    e.Valid = false;
                //    view.SetColumnError(view.Columns["MOBA_AMOUNT"], Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_CONTROL__INVALID_ROW__MOBA_AMOUNT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                //}
                //if (!e.Valid)
                //{
                //    gridViewExpMestBlood.FocusedColumn = view.Columns["MOBA_AMOUNT"];
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBlood_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                //for (int i = 0; i < gridViewExpMestBlood.DataRowCount; i++)
                //{
                //    var data = (ExpMestBloodADO)gridViewExpMestBlood.GetRow(i);
                //    if (data.IS_TH == 1)
                //    {
                //        gridViewExpMestBlood.UnselectRow(i);
                //    }
                //}

                //List<ExpMestBloodADO> expMestCheckeds = new List<ExpMestBloodADO>();
                //int[] selectRows = gridViewExpMestBlood.GetSelectedRows();
                //if (selectRows != null && selectRows.Count() > 0)
                //{
                //    for (int i = 0; i < selectRows.Count(); i++)
                //    {
                //        expMestCheckeds.Add((ExpMestBloodADO)gridViewExpMestBlood.GetRow(selectRows[i]));
                //    }
                //}

                //CalculTotalPrice(expMestCheckeds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBlood_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "MOBA_AMOUNT")
                {
                }
                var data = (List<ExpMestBloodADO>)gridControlExpMestBlood.DataSource;
                List<ExpMestBloodADO> expMestCheckeds = data != null && data.Count() > 0 ? data.Where(o => o.IsCheck && o.IS_TH != 1).ToList() : null;
                CalculTotalPrice(expMestCheckeds);
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
                if (!btnSave.Enabled || this.expMestId <= 0 || this.hisExpMest == null)
                    return;
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool sucsess = false;
                var createMoba = CreateMobaImpMest(param, ref sucsess);
                if (!createMoba)
                    return;
                if (sucsess)
                {
                    this.LoadExpMestBlood();
                    btnSave.Enabled = false;
                    btnPrint.Enabled = true;
                }
                WaitingManager.Hide();
                if (sucsess)
                {
                    MessageManager.Show(this, param, sucsess);
                }
                else
                {
                    MessageManager.Show(param, sucsess);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CreateMobaImpMest(CommonParam param, ref bool sucsess)
        {
            bool verify = false;
            try
            {
                HisImpMestMobaBloodSDO data = new HisImpMestMobaBloodSDO();
                data.BloodIds = new List<long>();
                data.ExpMestId = this.expMestId;
                data.RequestRoomId = this.currentModule.RoomId;
                if (cboTracking.EditValue != null)
                {
                    data.TrackingId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTracking.EditValue.ToString());
                }
                gridViewExpMestBlood.PostEditor();
                var dataSource = (List<ExpMestBloodADO>)gridControlExpMestBlood.DataSource;
                if (dataSource == null || dataSource.Count() == 0)
                {
                    return verify;
                }
                var dataSourceAllow = dataSource.Where(o => o.IsCheck && o.IS_TH != 1).ToList();

                foreach (var item in dataSourceAllow)
                {
                    data.BloodIds.Add(item.BLOOD_ID);
                }
                if (data.BloodIds.Count <= 0 || data.ExpMestId <= 0 || data.RequestRoomId <= 0)
                {
                    MessageManager.Show(ResourceMessageLang.NguoiDungChuaChonMau);
                    verify = false;
                }
                else
                {
                    verify = true;
                    data.Description = this.txtDescription.Text;
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestResultSDO>(RequestUri.HIS_MOBA_BLOOD_CREATE, ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        this.resultMobaSdo = new HisImpMestResultSDO();
                        this.resultMobaSdo.ImpBloods = new List<V_HIS_IMP_MEST_BLOOD>();
                        this.resultMobaSdo.ImpMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                        this.resultMobaSdo.ImpMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                        this.resultMobaSdo.ImpMest = new V_HIS_IMP_MEST();
                        this.resultMobaSdo = rs;
                        sucsess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                sucsess = false;
            }
            return verify;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || this.resultMobaSdo == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__PhieuYeuCauNhapThuHoi_MPS000213, delegateRunPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridViewExpMestBlood.PostEditor();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateRunPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultMobaSdo == null)
                    return result;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                MPS.Processor.Mps000213.PDO.Mps000213PDO rdo = new MPS.Processor.Mps000213.PDO.Mps000213PDO(this.resultMobaSdo.ImpMest, this.resultMobaSdo.ImpBloods);
                rdo.ImpMest = this.resultMobaSdo.ImpMest;
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                //this.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__CAPTION", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Button
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__BTN_SAVE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__TXT_DESCRIPTION", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__BTN_PRINT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Layout
                this.layoutTotalFeePrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__LAYOUT_TOTAL_FEE_PRICE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTotalPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__LAYOUT_TOTAL_PRICE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTotalVatPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__LAYOUT_TOTAL_VAT_PRICE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_EXPIRED_DATE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ImpPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_IMP_PRICE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_MEDICINE_TYPE_CODE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MedicineTypName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_MEDICINE_TYPE_NAME", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_PACKAGE_NUMBER", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_CONTROL__COLUMN_STT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_VatRatio.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_VAT_RATIO", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_VirTotalImpPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_VIR_TOTAL_IMP_PRICE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBlood_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                if (IsCancelCheck(gridViewExpMestBlood, gridViewExpMestBlood.FocusedRowHandle))
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsCancelCheck(GridView view, int row)
        {
            bool result = false;
            try
            {
                string val = Convert.ToString(view.GetRowCellValue(row, "IS_TH") ?? "");
                if (!String.IsNullOrEmpty(val) && Convert.ToInt32(val) == 1)
                    result = true;
                else
                    result = false;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private void gridViewExpMestBlood_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (ExpMestBloodADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IS_TH == 1)
                        {
                            e.Appearance.ForeColor = Color.Red; // máu đã được thu hồi = > màu đỏ
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBlood_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            var lstCheckAll = listExpMestBloodADO;
                            List<ExpMestBloodADO> lstChecks = new List<ExpMestBloodADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = listExpMestBloodADO.Where(o => o.IsCheck == true).Count();
                                //var ServiceNum = listExpMestBloodADO.Count();
                                //if ((ServiceCheckedNum > 0 && ServiceCheckedNum < ServiceNum) || ServiceCheckedNum == 0)
                                //{
                                //    isCheckAll = true;
                                //    hi.Column.Image = imageCollectionService.Images[1];
                                //}

                                if (ServiceCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionService.Images[1];
                                }
                                else
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionService.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.IS_TH != 1)
                                        {
                                            item.IsCheck = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            item.IsCheck = false;
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.IS_TH != 1)
                                        {
                                            item.IsCheck = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                gridControlExpMestBlood.BeginUpdate();
                                gridControlExpMestBlood.DataSource = null;
                                gridControlExpMestBlood.DataSource = lstChecks;
                                gridControlExpMestBlood.EndUpdate();

                                List<ExpMestBloodADO> expMestCheckeds = lstChecks != null && lstChecks.Count() > 0 ? lstChecks.Where(o => o.IsCheck && o.IS_TH != 1).ToList() : null;
                                CalculTotalPrice(expMestCheckeds);
                            }
                        }
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

        private void cboTracking_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTracking.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TrackingCreate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TrackingCreate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(this.hisExpMest.TDL_TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();

                        this.SetComboTracking();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
