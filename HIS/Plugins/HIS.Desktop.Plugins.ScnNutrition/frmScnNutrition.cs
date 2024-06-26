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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using SCN.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.ScnNutrition
{
    public partial class frmScnNutrition : HIS.Desktop.Utility.FormBase
    {
        string _PersonCode = "";
        Inventec.Desktop.Common.Modules.Module currentModule;

        int positionHandleControl = -1;

        SCN_NUTRITION _NutritionClick { get; set; }
        int action = 0;

        public frmScnNutrition()
        {
            InitializeComponent();
        }


        public frmScnNutrition(Inventec.Desktop.Common.Modules.Module currentModule, string _personCode)
            : base(currentModule)
        {
            InitializeComponent();
            this.SetIcon();
            this.currentModule = currentModule;
            this._PersonCode = _personCode;
            if (this.currentModule != null)
            {
                this.Text = currentModule.text;
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

        private void frmScnNutrition_Load(object sender, EventArgs e)
        {
            try
            {
                this.ValidationSingleControl(this.dtTime, this.dxValidationProvider1);
                this.ValidationSingleControl(this.spinWeight, this.dxValidationProvider1);
                this.ValidationSingleControl(this.spinHeight, this.dxValidationProvider1);
                this.LoadDataNutrition();
                this.btnNew_Click(null, null);
                this.action = 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataNutrition()
        {
            try
            {
                SCN.Filter.ScnNutritionFilter filter = new SCN.Filter.ScnNutritionFilter();
                filter.PERSON_CODE__EXACT = this._PersonCode;
                CommonParam param = new CommonParam();
                var resultData = ApiConsumers.ScnWrapConsumer.Get<List<SCN_NUTRITION>>(true, "api/ScnNutrition/Get", param, filter);

                gridControlNutrition.DataSource = null;

                gridControlNutrition.DataSource = resultData;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewNutrition_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
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
                        else if (e.Column.FieldName == "MEASURE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MEASURE_TIME);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                    if (e.Column.FieldName == "DELETE")
                    {
                        var creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        if (creator.Trim() == data.CREATOR && data.IS_ACTIVE == IMSys.DbConfig.SCN_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.RepositoryItem = repositoryItem__Delete_E;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItem__Delete_D;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItem__Delete_E_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var dataRow = (SCN_NUTRITION)gridViewNutrition.GetFocusedRow();
                if (dataRow != null)
                {
                    CommonParam param = new CommonParam();
                    var result = ApiConsumers.ScnWrapConsumer.Post<bool>(true, "api/ScnNutrition/Delete", param, dataRow);
                    MessageManager.Show(this.ParentForm, param, result);
                    if (result)
                        this.LoadDataNutrition();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                this.action = 1;
                dtTime.DateTime = DateTime.Now;
                spinHeight.EditValue = null;
                spinWeight.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(DateEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                DateTimeEdit__ValidationRule validRule = new DateTimeEdit__ValidationRule();
                validRule.dateEdit = control;
                validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(SpinEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                SpinEdit__ValidationRule validRule = new SpinEdit__ValidationRule();
                validRule.spinEdit = control;
                validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
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
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
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
                this.positionHandleControl = -1;
                if (!this.dxValidationProvider1.Validate())
                    return;
                SCN.EFMODEL.DataModels.SCN_NUTRITION _NutritionNew = new SCN_NUTRITION();
                _NutritionNew.PERSON_CODE = this._PersonCode;
                _NutritionNew.MEASURE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime) ?? 0;
                _NutritionNew.WEIGHT = spinWeight.Value;
                _NutritionNew.HEIGHT = spinHeight.Value;

                CommonParam param = new CommonParam();
                bool succes = false;
                SCN_NUTRITION result = null;
                if (this.action == 1)
                {
                    result = ApiConsumers.ScnWrapConsumer.Post<SCN_NUTRITION>(true, "api/ScnNutrition/Create", param, _NutritionNew);
                }
                else if (this.action == 2 && this._NutritionClick != null)
                {
                    _NutritionNew.ID = this._NutritionClick.ID;
                    result = ApiConsumers.ScnWrapConsumer.Post<SCN_NUTRITION>(true, "api/ScnNutrition/Update", param, _NutritionNew);
                }
                if (result != null)
                {
                    this.action = 2;
                    succes = true;
                    this.LoadDataNutrition();
                }
                MessageManager.Show(this.ParentForm, param, succes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewNutrition_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                this._NutritionClick = new SCN_NUTRITION();
                var dataRow = (SCN_NUTRITION)gridViewNutrition.GetFocusedRow();
                if (dataRow != null)
                {
                    this._NutritionClick = dataRow;
                    this.action = 2;
                    dtTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dataRow.MEASURE_TIME) ?? DateTime.Now;
                    spinHeight.EditValue = dataRow.HEIGHT;
                    spinWeight.EditValue = dataRow.WEIGHT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (dtTime.EditValue != null)
                {
                    spinWeight.Focus();
                    spinWeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                spinWeight.Focus();
                spinWeight.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinWeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                spinHeight.Focus();
                spinHeight.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinHeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
