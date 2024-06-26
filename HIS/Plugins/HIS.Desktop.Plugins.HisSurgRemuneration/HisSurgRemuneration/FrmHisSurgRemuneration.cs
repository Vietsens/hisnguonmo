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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using HIS.Desktop.Utility;
using Inventec.UC.Paging;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Controls.Session;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.HisSurgRemuneration.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.HisSurgRemuneration.Validate;

namespace HIS.Desktop.Plugins.HisSurgRemuneration.HisSurgRemuneration
{
    public partial class FrmHisSurgRemuneration : FormBase
    {
        #region Reclare variable
        Module ModuleData;
        PagingGrid pagingrid;
        int ActionType = -1;
        HIS_SURG_REMUNERATION CurrentData;
        #endregion
        public FrmHisSurgRemuneration(Module module)
            : base(module)
        {
            try
            {
                InitializeComponent();
                pagingrid = new PagingGrid();
                this.ModuleData = module;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {

                    LogSystem.Warn(ex);
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FrmHisSurgRemuneration_Load(object sender, EventArgs e)
        {
            try
            {
                Meshow();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void Meshow()
        {
            try
            {
                SetDataDefault();
                EnableControlChanged(this.ActionType);
                FillDataToGrdControlRemuEration();
                FillDataToCombobox();
                SetCaptionByLanguageKey();
                Validateform();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        #region ---Set Data
        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.ModuleData != null && !String.IsNullOrEmpty(this.ModuleData.text))
                {
                    this.Text = this.ModuleData.text;
                }
                ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisSurgRemuneration.Resources.Lang", typeof(HIS.Desktop.Plugins.HisSurgRemuneration.HisSurgRemuneration.FrmHisSurgRemuneration).Assembly);
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.btnAdd.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.btnEdit.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRest.Text = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.btnRest.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAddDetalt.Text = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.btnAddDetalt.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.btnSearch.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcServiceType.Text = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.lcServiceType.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcPtttGroup.Text = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.lcPtttGroup.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEmotionlessMethod.Text = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.lcEmotionlessMethod.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcNumOfPeople.Text = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.lcNumOfPeople.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcExcuteRole.Text = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.lcExcuteRole.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcPrice.Text = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.lcPrice.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.STT.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.STT.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSTT.Caption = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColSTT.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSTT.ToolTip = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColSTT.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceType.Caption = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColServiceType.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceType.ToolTip = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColServiceType.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttGroup.Caption = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColPtttGroup.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttGroup.ToolTip = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColPtttGroup.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColEmotionless.Caption = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColEmotionless.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColEmotionless.Caption = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColEmotionless.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNumOfPeople.Caption = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColNumOfPeople.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNumOfPeople.ToolTip = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColNumOfPeople.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExcuteRole.Caption = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColExcuteRole.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExcuteRole.ToolTip = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColExcuteRole.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPrice.Caption = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColPrice.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPrice.ToolTip = Inventec.Common.Resource.Get.Value("FrmHisSurgRemuneration.grdColPrice.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetDataDefault()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                cboServiceType.EditValue = null;
                cboPtttGroup.EditValue = null;
                cboExcuteRole.EditValue = null;
                spNumberOfPeople.EditValue = null;
                txtExcuteRole.Text = "";
                spPrice.EditValue = null;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int actionType)
        {
            try
            {
                btnAdd.Enabled = (actionType == GlobalVariables.ActionAdd);
                btnEdit.Enabled = (actionType == GlobalVariables.ActionEdit);
                btnAddDetalt.Enabled = (actionType == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void RestFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized)
                    return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            cboServiceType.Focus();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcessor()
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                    return;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                HIS_SURG_REMUNERATION UpdateDTO = new HIS_SURG_REMUNERATION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SURG_REMUNERATION>(UpdateDTO, CurrentData);
                UpdateDTOFormToDataRemuneration(UpdateDTO);
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    var data = new BackendAdapter(param).Post<HIS_SURG_REMUNERATION>(HisRequestUriStore.HisSurgRemuneration_Create, ApiConsumers.MosConsumer, UpdateDTO, param);
                    if (data != null)
                    {
                        BackendDataWorker.Reset<HIS_SURG_REMUNERATION>();
                        FillDataToGrdControlRemuEration();
                        success = true;
                        RestFormData();

                    }
                }
                else
                {
                    UpdateDTO.ID = CurrentData.ID;
                    var data = new BackendAdapter(param).Post<HIS_SURG_REMUNERATION>(HisRequestUriStore.HisSurgRemuneration_Update, ApiConsumers.MosConsumer, UpdateDTO, param);
                    if (data != null)
                    {
                        BackendDataWorker.Reset<HIS_SURG_REMUNERATION>();
                        FillDataToGrdControlRemuEration();
                        success = true;
                    }
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

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void UpdateDTOFormToDataRemuneration(HIS_SURG_REMUNERATION data)
        {
            try
            {
                if (cboServiceType.EditValue != null)
                {
                    data.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "").ToString());
                }
                if (cboPtttGroup.EditValue != null)
                {
                    data.PTTT_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPtttGroup.EditValue ?? "").ToString());
                }
                if (cboEmotionlessMethod.EditValue != null)
                {
                    data.EMOTIONLESS_METHOD_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboEmotionlessMethod.EditValue ?? "").ToString());
                }
                else
                    data.EMOTIONLESS_METHOD_ID = null;

                if (spNumberOfPeople.EditValue != null)
                {
                    data.NUMBER_OF_PEOPLE = Inventec.Common.TypeConvert.Parse.ToInt64((spNumberOfPeople.EditValue ?? "").ToString());
                }
                if (dtFromTime.DateTime != null && dtFromTime.DateTime != DateTime.MinValue)
                {
                    data.SURG_FROM_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtFromTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                else
                {
                    data.SURG_FROM_TIME = null;
                }
                if (dtToTime.DateTime != null && dtToTime.DateTime != DateTime.MinValue)
                {
                    data.SURG_TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtToTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                else
                {
                    data.SURG_TO_TIME = null;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFormToDataDetail(HIS_SURG_REMU_DETAIL data)
        {
            try
            {
                data.SURG_REMUNERATION_ID = CurrentData.ID;
                if (cboExcuteRole.EditValue != null)
                {
                    data.EXECUTE_ROLE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboExcuteRole.EditValue ?? "").ToString());
                }
                if (spPrice.EditValue != null)
                {
                    data.PRICE = Inventec.Common.TypeConvert.Parse.ToInt64((spPrice.EditValue ?? "").ToString());
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region ---Fill Data
        private void FillDataToGrdControlRemuEration()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                ApiResultObject<List<V_HIS_SURG_REMUNERATION>> apiResult = null;
                HisSurgRemunerationViewFilter filter = new HisSurgRemunerationViewFilter();
                SetFilter(ref filter);
                gridControlRemuneration.DataSource = null;
                gridViewRemuneration.BeginUpdate();
                apiResult = new BackendAdapter(param).GetRO<List<V_HIS_SURG_REMUNERATION>>(HisRequestUriStore.HisSurgRemuneration_GetView, ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null)
                {
                    var data = (List<V_HIS_SURG_REMUNERATION>)apiResult.Data;
                    if (data != null)
                    {
                        gridControlRemuneration.DataSource = data;
                        WaitingManager.Hide();
                    }
                }
                gridViewRemuneration.EndUpdate();
                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();

            }
        }

        private void SetFilter(ref HisSurgRemunerationViewFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControlRemuDetail()
        {
            try
            {
                if (CurrentData != null)
                {
                    WaitingManager.Show();
                    List<V_HIS_SURG_REMU_DETAIL> Data = new List<V_HIS_SURG_REMU_DETAIL>();
                    gridControlRemuDetail.DataSource = null;
                    gridViewRemuDetail.BeginUpdate();
                    Data = BackendDataWorker.Get<V_HIS_SURG_REMU_DETAIL>().Where(o => o.SURG_REMUNERATION_ID == CurrentData.ID).ToList();
                    if (Data != null)
                    {
                        gridControlRemuDetail.DataSource = Data;
                        gridViewRemuDetail.EndUpdate();
                        BackendDataWorker.Reset<V_HIS_SURG_REMU_DETAIL>();
                        WaitingManager.Hide();
                    }

                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ChangeDataRow(HIS_SURG_REMUNERATION dataRow)
        {
            try
            {
                if (dataRow != null)
                {
                    FillDataEditorControl(dataRow);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    btnEdit.Enabled = (CurrentData.IS_ACTIVE == 1);
                    btnAddDetalt.Enabled = (CurrentData.IS_ACTIVE == 1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider2, dxErrorProvider1);
                }

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void FillDataEditorControl(HIS_SURG_REMUNERATION dataRow)
        {
            try
            {
                if (dataRow != null)
                {
                    cboServiceType.EditValue = dataRow.SERVICE_TYPE_ID;
                    cboPtttGroup.EditValue = dataRow.PTTT_GROUP_ID;
                    cboEmotionlessMethod.EditValue = dataRow.EMOTIONLESS_METHOD_ID;
                    spNumberOfPeople.EditValue = dataRow.NUMBER_OF_PEOPLE;
                    if (dataRow.SURG_FROM_TIME != null)
                    {
                        dtFromTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dataRow.SURG_FROM_TIME ?? 0);
                    }
                    else
                    {
                        dtFromTime.EditValue = null;
                    }
                    if (dataRow.SURG_TO_TIME != null)
                    {
                        dtToTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dataRow.SURG_TO_TIME ?? 0);
                    }
                    else
                    {
                        dtToTime.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        #region ---Load data to combobox
        private void FillDataToCombobox()
        {
            try
            {
                FillDataToCboServiceType();
                FillDataToCboPtttGroup();
                LoadDataTocboEmotionlessMethod();
                LoadDataTocboExcuteRole();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FillDataToCboServiceType()
        {
            try
            {
                List<HIS_SERVICE_TYPE> data = new List<HIS_SERVICE_TYPE>();
                data = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columninfo = new List<ColumnInfo>();
                columninfo.Add(new ColumnInfo("SERVICE_TYPE_CODE", "Mã dịch vụ", 100, 1));
                columninfo.Add(new ColumnInfo("SERVICE_TYPE_NAME", "Tên dịch vụ", 150, 2));
                ControlEditorADO controleditro = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columninfo, true, 250);
                ControlEditorLoader.Load(cboServiceType, data, controleditro);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FillDataToCboPtttGroup()
        {
            try
            {
                List<HIS_PTTT_GROUP> data = new List<HIS_PTTT_GROUP>();
                data = BackendDataWorker.Get<HIS_PTTT_GROUP>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columninfo = new List<ColumnInfo>();
                columninfo.Add(new ColumnInfo("PTTT_GROUP_CODE", "Mã nhóm PTTT", 100, 1));
                columninfo.Add(new ColumnInfo("PTTT_GROUP_NAME", "Tên nhóm PTTT", 150, 2));
                ControlEditorADO controlEditor = new ControlEditorADO("PTTT_GROUP_NAME", "ID", columninfo, true, 250);
                ControlEditorLoader.Load(cboPtttGroup, data, controlEditor);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void LoadDataTocboEmotionlessMethod()
        {
            try
            {
                List<HIS_EMOTIONLESS_METHOD> data = new List<HIS_EMOTIONLESS_METHOD>();
                data = BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("EMOTIONLESS_METHOD_CODE", "", 100, 1));
                columnInfo.Add(new ColumnInfo("EMOTIONLESS_METHOD_NAME", "", 150, 2));
                ControlEditorADO controlADO = new ControlEditorADO("EMOTIONLESS_METHOD_NAME", "ID", columnInfo, false, 250);
                ControlEditorLoader.Load(cboEmotionlessMethod, data, controlADO);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void LoadDataTocboExcuteRole()
        {
            try
            {
                List<HIS_EXECUTE_ROLE> data = new List<HIS_EXECUTE_ROLE>();
                data = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columninfo = new List<ColumnInfo>();
                columninfo.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "Mã vai trò", 100, 1));
                columninfo.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "Tên vai trò", 150, 2));
                ControlEditorADO controlADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columninfo, true, 250);
                ControlEditorLoader.Load(cboExcuteRole, data, controlADO);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void LoadDataTocboExcuteRoleGridControl()
        {
            try
            {
                List<HIS_EXECUTE_ROLE> data = new List<HIS_EXECUTE_ROLE>();
                data = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columninfo = new List<ColumnInfo>();
                columninfo.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "Mã vai trò", 100, 1));
                columninfo.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "Tên vai trò", 150, 2));
                ControlEditorADO controlADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columninfo, true, 250);
                ControlEditorLoader.Load(CboExecuteRole, data, controlADO);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region---even gridView

        private void gridViewRemuneration_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (V_HIS_SURG_REMUNERATION)gridViewRemuneration.GetFocusedRow();
                var ado = BackendDataWorker.Get<HIS_SURG_REMUNERATION>().SingleOrDefault(o => o.ID == row.ID);
                if (ado != null)
                {
                    CurrentData = ado;
                    ChangeDataRow(ado);
                    LoadDataTocboExcuteRoleGridControl();
                    FillDataToGridControlRemuDetail();

                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewRemuneration_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    V_HIS_SURG_REMUNERATION data = (V_HIS_SURG_REMUNERATION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewRemuneration_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_SURG_REMUNERATION data = (V_HIS_SURG_REMUNERATION)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (data.IS_ACTIVE == 1)
                        {
                            e.RepositoryItem = btnDelete;
                        }
                        else
                        {
                            e.RepositoryItem = btnenableDelete;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewRemuDetail_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_SURG_REMU_DETAIL)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "DELETE")
                        {
                            e.RepositoryItem = (data.IS_ACTIVE == 1 ? btnDeleteDetail : btnenableDelete);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewRemuDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_SURG_REMU_DETAIL)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT_STR")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }


                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion

        #region ---Key up
        private void cboServiceType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPtttGroup.Focus();
                    if (cboServiceType.EditValue == null)
                    {
                        cboServiceType.ShowPopup();
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboPtttGroup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboEmotionlessMethod.Focus();
                    if (cboPtttGroup.EditValue == null)
                    {
                        cboPtttGroup.ShowPopup();
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboEmotionlessMethod_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spNumberOfPeople.Focus();
                    spNumberOfPeople.SelectAll();
                    if (cboEmotionlessMethod.EditValue == null)
                    {
                        cboEmotionlessMethod.ShowPopup();
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void spNumberOfPeople_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    {
                        btnEdit.Focus();
                    }
                    else if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else
                        btnRest.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrdControlRemuEration();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region _Properties_ButtonClick

        private void cboServiceType_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboServiceType.Properties.Buttons[1].Visible = true;
                    cboServiceType.EditValue = null;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboPtttGroup_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPtttGroup.Properties.Buttons[1].Visible = true;
                    cboPtttGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboEmotionlessMethod_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboEmotionlessMethod.Properties.Buttons[1].Visible = true;
                    cboEmotionlessMethod.EditValue = null;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboExcuteRole_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboExcuteRole.EditValue != null && cboExcuteRole.EditValue != cboExcuteRole.OldEditValue)
                    {
                        var data = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExcuteRole.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtExcuteRole.Text = data.EXECUTE_ROLE_CODE;
                            spPrice.Focus();
                            spPrice.SelectAll();
                        }
                        else
                        {
                            spPrice.Focus();
                            spPrice.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtExcuteRole_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var text = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadDataFromTextToCboExcuteRole(text);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void LoadDataFromTextToCboExcuteRole(string text)
        {
            try
            {
                List<HIS_EXECUTE_ROLE> data = new List<HIS_EXECUTE_ROLE>();
                data = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => (o.EXECUTE_ROLE_CODE != null && o.EXECUTE_ROLE_CODE.StartsWith(text))).ToList();
                if (data.Count == 1)
                {
                    cboExcuteRole.EditValue = data[0].ID;
                    txtExcuteRole.Text = data[0].EXECUTE_ROLE_CODE;
                    spPrice.Focus();
                }
                else
                {
                    cboExcuteRole.EditValue = null;
                    cboExcuteRole.Focus();
                    cboExcuteRole.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void spPrice_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.Focus();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }


        private void cboExcuteRole_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

        }
        #endregion

        #region ---Button click
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrdControlRemuEration();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                {
                    btnAdd_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnRest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRest_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnRestFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                cboServiceType.Focus();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessor();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessor();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnAddDetalt_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dxValidationProvider2.Validate())
                    return;
                CommonParam param = new CommonParam();
                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                    return;
                if (!dxValidationProvider1.Validate())
                    return;
                if (!CheckExecuterRoleID())
                {
                    MessageBox.Show("Xử lý thất bại. Vai trò đã tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                WaitingManager.Show();
                HIS_SURG_REMU_DETAIL UpdateDTo = new HIS_SURG_REMU_DETAIL();
                UpdateDTOFormToDataDetail(UpdateDTo);
                var data = new BackendAdapter(param).Post<HIS_SURG_REMU_DETAIL>(HisRequestUriStore.HisSurgDetail_Create, ApiConsumers.MosConsumer, UpdateDTo, param);
                if (data != null)
                {
                    BackendDataWorker.Reset<HIS_SURG_REMU_DETAIL>();
                    BackendDataWorker.Reset<V_HIS_SURG_REMU_DETAIL>();
                    success = true;
                    FillDataToGridControlRemuDetail();
                    cboExcuteRole.EditValue = null;
                    txtExcuteRole.Text = "";
                    spPrice.EditValue = null;
                    txtExcuteRole.Focus();
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

                LogSystem.Warn(ex);
            }
        }

        private bool CheckExecuterRoleID()
        {
            bool result = true;
            try
            {
                if (cboExcuteRole.EditValue != null && CurrentData != null)
                {
                    List<HIS_SURG_REMU_DETAIL> RemuDetail = new List<HIS_SURG_REMU_DETAIL>();
                    Int64 ExecuteRoleID = Inventec.Common.TypeConvert.Parse.ToInt64(cboExcuteRole.EditValue.ToString());
                    RemuDetail = BackendDataWorker.Get<HIS_SURG_REMU_DETAIL>().Where(o => o.SURG_REMUNERATION_ID == CurrentData.ID).ToList();
                    for (int i = 0; i < RemuDetail.Count(); i++)
                    {
                        if (RemuDetail[i].EXECUTE_ROLE_ID == ExecuteRoleID)
                        {
                            result = false;
                            break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                var data = (V_HIS_SURG_REMUNERATION)gridViewRemuneration.GetFocusedRow();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HisSurgRemuneration_Delete, ApiConsumers.MosConsumer, data.ID, null);
                    if (success)
                    {
                        BackendDataWorker.Reset<HIS_SURG_REMUNERATION>();
                        BackendDataWorker.Reset<V_HIS_SURG_REMUNERATION>();
                        FillDataToGrdControlRemuEration();
                        btnRest_Click(null, null);
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnDeleteDetail_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool succes = false;
                var data = (V_HIS_SURG_REMU_DETAIL)gridViewRemuDetail.GetFocusedRow();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    succes = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HisSurgDetail_Delete, ApiConsumers.MosConsumer, data.ID, param);
                    if (succes)
                    {
                        BackendDataWorker.Reset<V_HIS_SURG_REMU_DETAIL>();
                        FillDataToGridControlRemuDetail();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, succes);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnRest_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider2, dxErrorProvider1);
                RestFormData();
                CurrentData = new HIS_SURG_REMUNERATION();
                gridControlRemuDetail.DataSource = null;
                cboServiceType.Focus();

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion

        #region --Validateform
        private void Validateform()
        {
            try
            {
                ValidateNullControlcboServiceType();
                ValidateNullControlcboPtttGroup();
                ValidateNullControlcboExcuteRole();
                ValidateNullControlSpinEditPrice();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ValidateNullControlcboServiceType()
        {
            try
            {
                Validate_Combobox validate = new Validate_Combobox();
                validate.txtControl = cboServiceType;
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboServiceType, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ValidateNullControlcboPtttGroup()
        {
            try
            {
                Validate_Combobox validate = new Validate_Combobox();
                validate.txtControl = cboPtttGroup;
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboPtttGroup, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ValidateNullControlcboExcuteRole()
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = txtExcuteRole;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider2.SetValidationRule(txtExcuteRole, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateNullControlSpinEditPrice()
        {
            try
            {
                Validate_SpinEdit validate = new Validate_SpinEdit();
                validate.txtControl = spPrice;
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider2.SetValidationRule(spPrice, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        private void bbtnAdddetail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnAddDetalt.Enabled)
                {
                    btnAddDetalt_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSession.Warn(ex);
            }
        }

        private void BTNEditGrid_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {

                CommonParam param = new CommonParam();
                bool success = false;
                var dataRow = (V_HIS_SURG_REMU_DETAIL)gridViewRemuDetail.GetFocusedRow();
                WaitingManager.Show();
                HIS_SURG_REMU_DETAIL UpdateDTo = new HIS_SURG_REMU_DETAIL();
                //UpdateDTOFormToDataDetailControl(UpdateDTo);
                UpdateDTo.ID = dataRow.ID;
                UpdateDTo.EXECUTE_ROLE_ID = dataRow.EXECUTE_ROLE_ID;
                UpdateDTo.PRICE = dataRow.PRICE;
                UpdateDTo.SURG_REMUNERATION_ID = CurrentData.ID;
                var data = new BackendAdapter(param).Post<HIS_SURG_REMU_DETAIL>(HisRequestUriStore.HisSurgDetail_Update, ApiConsumers.MosConsumer, UpdateDTo, param);
                if (data != null)
                {
                    BackendDataWorker.Reset<HIS_SURG_REMU_DETAIL>();
                    BackendDataWorker.Reset<V_HIS_SURG_REMU_DETAIL>();
                    success = true;
                    FillDataToGridControlRemuDetail();
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

                LogSystem.Error(ex);
            }
        }
        private void UpdateDTOFormToDataDetailControl(HIS_SURG_REMU_DETAIL updateDTo)
        {
            try
            {
                updateDTo.SURG_REMUNERATION_ID = CurrentData.ID;
                if (CboExecuteRole.Editable != null)
                {
                    updateDTo.EXECUTE_ROLE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((CboExecuteRole.Editable.ToString() ?? "").ToString());
                }
                if (SpinPrice.Editable != null)
                {
                    updateDTo.PRICE = Inventec.Common.TypeConvert.Parse.ToInt64((SpinPrice.Editable.ToString() ?? "").ToString());
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void dtFromTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtFromTime.EditValue != null)
                    {
                        dtToTime.Focus();
                    }
                    else
                    {
                        dtFromTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spNumberOfPeople_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtFromTime.Focus();
                    dtFromTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtToTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtToTime.EditValue != null)
                    {
                        if (this.ActionType == GlobalVariables.ActionAdd)
                        {
                            btnAdd.Focus();
                        }
                        else
                        {
                            btnEdit.Focus();
                        }
                    }
                    else
                    {
                        dtToTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtToTime_CloseUp(object sender, CloseUpEventArgs e)
        {

        }

        private void dtToTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (dtToTime.EditValue != null)
                        dtToTime.DateTime = dtToTime.DateTime.Date.AddHours(23).AddMinutes(59);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
