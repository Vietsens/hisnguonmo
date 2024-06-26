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
using Inventec.Core;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.FeeHospitalWarning.Config;

namespace HIS.Desktop.Plugins.FeeHospitalWarning
{
    public partial class UCFeeWarning : HIS.Desktop.Utility.UserControlBase
    {
        #region declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int limit = 0;
        long roomId;
        long roomtypeId;

        List<WarningColor> ListWarningColor = new List<WarningColor>();
        #endregion

        #region contructor
        public UCFeeWarning(Inventec.Desktop.Common.Modules.Module moduleDT)
            : base(moduleDT)
        {
            InitializeComponent();
            this.roomId = moduleDT.RoomId;
            this.roomtypeId = moduleDT.RoomTypeId;
            HisConfigCFG.LoadConfig();
        }
        #endregion

        #region load
        private void UCFeeWarning_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                InitDataComboDepartment();
                InitDataComboTreatmentType();
                GetConfigWarning();
                SetDefaultValue();
                SetFormatColumn();
                //InitComboFill(SetDataToListFeeHospitalFill());
                //cboFill.EditValue = 0;
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetConfigWarning()
        {
            try
            {
                ListWarningColor = HisConfigWarning.GetConfigWarning();
                if (ListWarningColor != null && ListWarningColor.Count > 0)
                {
                    ListWarningColor = ListWarningColor.OrderBy(o => o.Price).ToList();
                }
                else
                {
                    LciChkCanThu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    LciChkThuBhyt.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFormatColumn()
        {
            try
            {
                string formatStr = "#,##0";
                if (ConfigApplications.NumberSeperator > 0)
                {
                    formatStr += ".";
                    for (int i = 0; i < ConfigApplications.NumberSeperator; i++)
                    {
                        formatStr += "0";
                    }
                }

                Gc_CanThuThem.DisplayFormat.FormatString = formatStr;
                Gc_CanThuThem.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                Gc_DaThu.DisplayFormat.FormatString = formatStr;
                Gc_DaThu.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                Gc_TotalHeinPrice.DisplayFormat.FormatString = formatStr;
                Gc_TotalHeinPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                Gc_TotalPatientPrice.DisplayFormat.FormatString = formatStr;
                Gc_TotalPatientPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                Gc_TotalPrice.DisplayFormat.FormatString = formatStr;
                Gc_TotalPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.txtKeyword.Text = null;
                this.cboFill.EditValue = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == roomId).DepartmentId;
                SpTotalHeinPriceFrom.EditValue = null;
                SpTotalHeinPriceTo.EditValue = null;
                ChkCanthu.Checked = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataComboDepartment()
        {
            try
            {
                var ListDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
                if (ListDepartment != null && ListDepartment.Count > 0)
                {
                    ListDepartment = ListDepartment.Where(o => o.IS_ACTIVE == 1).OrderBy(o => o.NUM_ORDER).ToList();

                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 100, 1));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 100);
                    ControlEditorLoader.Load(this.cboFill, ListDepartment, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataComboTreatmentType()
        {
            try
            {
                var listTreatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>();
                if (listTreatmentType != null && listTreatmentType.Count > 0)
                {
                    listTreatmentType = listTreatmentType.Where(o => o.IS_ACTIVE == 1).OrderBy(o => o.TREATMENT_TYPE_CODE).ToList();

                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_NAME", "", 100, 1));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_TYPE_NAME", "ID", columnInfos, false, 100);
                    ControlEditorLoader.Load(this.cboTreatmentType, listTreatmentType, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<FeeHostitalComboAdo> SetDataToListFeeHospitalFill()
        {
            List<FeeHostitalComboAdo> FeeHostitalComboAdos = new List<FeeHostitalComboAdo>();
            try
            {
                FeeHostitalComboAdo taiKhoa = new FeeHostitalComboAdo();
                taiKhoa.IdFee = 0;
                taiKhoa.NameFee = "Tại khoa";
                FeeHostitalComboAdos.Add(taiKhoa);
                FeeHostitalComboAdo tatCa = new FeeHostitalComboAdo();
                tatCa.IdFee = 1;
                tatCa.NameFee = "Tất cả";
                FeeHostitalComboAdos.Add(tatCa);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return FeeHostitalComboAdos;
        }

        private void InitComboFill(List<FeeHostitalComboAdo> FeeHostitalComboAdos)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NameFee", "", 100, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("NameFee", "IdFee", columnInfos, false, 100);
                ControlEditorLoader.Load(this.cboFill, FeeHostitalComboAdos, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void FillDataToGrid()
        {
            try
            {
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                GridPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(GridPaging, param, numPageSize, this.gridControlFeeWarning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                HisTreatmentLView1Filter filter = new HisTreatmentLView1Filter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "REMAIN_AMOUNT";

                SetFilter(ref filter);
                var apiResult = new Inventec.Common.Adapter.BackendAdapter
                       (paramCommon).GetRO<List<L_HIS_TREATMENT_1>>
                       ("api/HisTreatment/GetLView1", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;

                    List<Treatment6ADO> treatment6Ado = new List<Treatment6ADO>();
                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            var ado = new Treatment6ADO(item);
                            treatment6Ado.Add(ado);
                        }
                    }

                    treatment6Ado = treatment6Ado.OrderByDescending(o => o.SoNo).ToList();

                    gridViewFeeWarning.BeginUpdate();
                    gridControlFeeWarning.DataSource = treatment6Ado;
                    rowCount = (param == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    gridViewFeeWarning.EndUpdate();
                }
                WaitingManager.Hide();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisTreatmentLView1Filter filter)
        {
            try
            {
                if (txtKeyword.Text != null)
                {
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                }
                //if (cboFill.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64(cboFill.EditValue.ToString()) == 0)
                //{
                //    filter.DEPARTMENT_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == roomId).DepartmentId;
                //}
                //else if (cboFill.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64(cboFill.EditValue.ToString()) == 1)
                //{
                //    filter.DEPARTMENT_ID = null;
                //}

                if (cboFill.EditValue != null)
                {
                    filter.LAST_DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboFill.EditValue.ToString());
                }
                if (cboTreatmentType.EditValue != null)
                {
                    filter.TDL_TREATMENT_TYPE_ID = Convert.ToInt64(cboTreatmentType.EditValue);
                }

                if (dtInTimeFrom.EditValue != null && dtInTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.IN_DATE_FROM = Convert.ToInt64(dtInTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtInTimeTo.EditValue != null && dtInTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.IN_DATE_TO = Convert.ToInt64(dtInTimeTo.DateTime.ToString("yyyyMMdd") + "000000");
                }

                filter.IS_PAUSE = false;

                if (SpTotalHeinPriceFrom.EditValue != null && SpTotalHeinPriceFrom.Value > 0)
                {
                    filter.TOTAL_HEIN_PRICE_FROM = SpTotalHeinPriceFrom.Value;
                }

                if (SpTotalHeinPriceTo.EditValue != null && SpTotalHeinPriceTo.Value > 0)
                {
                    filter.TOTAL_HEIN_PRICE_TO = SpTotalHeinPriceTo.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboFill_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFeeWarning_CustomUnboundColumnData(Object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (Treatment6ADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewFeeWarning_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (Treatment6ADO)gridViewFeeWarning.GetRow(e.RowHandle);
                    if (ChkThuBhyt.Checked && ListWarningColor != null && ListWarningColor.Count > 0)
                    {
                        e.Appearance.ForeColor = GetRowColor(data);
                        if (data.IS_BOLD)
                            e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
                    }
                    else
                    {
                        decimal canThuThem = data.CanThuThem;
                        //string canThuThemStr = gridViewFeeWarning.GetRowCellValue(e.RowHandle, "CanThuThem") as string;
                        //decimal canThuThem = Convert.ToDecimal(canThuThemStr);
                        if (canThuThem < -1000000)
                            e.Appearance.ForeColor = Color.Black;
                        else if (canThuThem < 0)
                            e.Appearance.ForeColor = Color.Green;
                        else if (canThuThem < 1000000)
                            e.Appearance.ForeColor = Color.Orange;
                        else if (canThuThem > 1000000)
                            e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private Color GetRowColor(Treatment6ADO data)
        {
            Color result = Color.Empty;
            try
            {
                //if (data != null && data.TOTAL_HEIN_PRICE.HasValue)
                //{
                //    var lstcolor = ListWarningColor.Where(o => o.TreatmentTypeId == data.TDL_TREATMENT_TYPE_ID).ToList();
                //    if (lstcolor != null && lstcolor.Count > 0)
                //    {
                //        lstcolor = lstcolor.OrderBy(o => o.Price).ToList();
                //        foreach (var item in lstcolor)
                //        {
                //            if (data.TOTAL_HEIN_PRICE.Value < item.Price)
                //            {
                //                result = Color.FromName(item.Color);
                //                break;
                //            }
                //        }
                //    }
                //}
                if (!String.IsNullOrWhiteSpace(data.WARRING_COLOR))
                {
                    result = System.Drawing.ColorTranslator.FromHtml(data.WARRING_COLOR);
                }
            }
            catch (Exception ex)
            {
                result = Color.Empty;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Treatment6ADO currentTreatment = gridViewFeeWarning.GetFocusedRow() as Treatment6ADO;
                if (currentTreatment != null)
                {
                    CommonParam param = new CommonParam();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RequestDeposit").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.RequestDeposit");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        listArgs.Add(currentTreatment.ID);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomtypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)(extenceInstance)).ShowDialog();
                    }
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.FeeHospitalWarning.Resources.Lang", typeof(HIS.Desktop.Plugins.FeeHospitalWarning.UCFeeWarning).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCFeeWarning.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCFeeWarning.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCFeeWarning.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCFeeWarning.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCFeeWarning.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCFeeWarning.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Stt.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_BtnAdd.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_PatientName.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Dob.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Gender.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_TotalPrice.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_TotalHeinPrice.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_TotalPatientPrice.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_CanThuThem.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_DaThu.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCFeeWarning.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Search()
        {
            btnSearch_Click(null, null);
        }

        private void cboFill_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboFill.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkCanthu_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridControlFeeWarning.RefreshDataSource();
                if (ChkCanthu.Checked)
                {
                    ChkThuBhyt.Checked = false;
                }
                else
                {
                    if (!ChkThuBhyt.Checked)
                    {
                        ChkCanthu.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkThuBhyt_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridControlFeeWarning.RefreshDataSource();
                if (ChkThuBhyt.Checked)
                {
                    ChkCanthu.Checked = false;
                }
                else
                {
                    if (!ChkCanthu.Checked)
                    {
                        ChkThuBhyt.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboTreatmentType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
