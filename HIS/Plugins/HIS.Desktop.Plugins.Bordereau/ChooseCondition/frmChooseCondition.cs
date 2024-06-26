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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Bordereau.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Bordereau.ChooseCondition
{
    public partial class frmChooseCondition : Form
    {
        DelegateSelectData refeshData { get; set; }
        List<SereServADO> sereServADOSelecteds { get; set; }
        long treatmentId;

        public frmChooseCondition(List<SereServADO> _sereServADOSelecteds, DelegateSelectData _refeshData, long _treatmentId)
        {
            InitializeComponent();
            try
            {
                this.treatmentId = _treatmentId;
                this.refeshData = _refeshData;
                this.sereServADOSelecteds = _sereServADOSelecteds;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChooseCondition_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                LoadCboCondition();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1View_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        var data = (HIS_SERVICE_CONDITION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (data != null)
                        {
                            if (e.Column.FieldName == "HEIN_RATIO_STR")
                            {
                                e.Value = data.HEIN_RATIO.HasValue ? (decimal?)Inventec.Common.Number.Convert.NumberToNumberRoundMax4((decimal)((data.HEIN_RATIO ?? 0) * 100)) : null;
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
        private void LoadCboCondition()
        {
            try
            {
                if (this.sereServADOSelecteds != null && this.sereServADOSelecteds.Count > 0)
                {
                    var data = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE && o.SERVICE_ID == sereServADOSelecteds.FirstOrDefault().SERVICE_ID);
                    if (sereServADOSelecteds.FirstOrDefault().SERVICE_CONDITION_ID.HasValue)
                    {
                        cboCondition.EditValue = sereServADOSelecteds.FirstOrDefault().SERVICE_CONDITION_ID;
                    }

                    List<ServiceConditionADO> serviceConditionADOs = (from r in data select new ServiceConditionADO(r)).ToList();

                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("SERVICE_CONDITION_CODE", "Mã", 80, 1));
                    columnInfos.Add(new ColumnInfo("SERVICE_CONDITION_NAME", "Tên", 300, 2));
                    columnInfos.Add(new ColumnInfo("HEIN_RATIO_STR", "Tỉ lệ thanh toán", 80, 3));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_CONDITION_NAME", "ID", columnInfos, false, 460);

                    cboCondition.Properties.DataSource = serviceConditionADOs;
                    cboCondition.Properties.DisplayMember = controlEditorADO.DisplayMember;
                    cboCondition.Properties.ValueMember = controlEditorADO.ValueMember;
                    cboCondition.Properties.TextEditStyle = TextEditStyles.Standard;
                    cboCondition.Properties.PopupFilterMode = PopupFilterMode.Contains;
                    cboCondition.Properties.ImmediatePopup = controlEditorADO.ImmediatePopup;
                    cboCondition.ForceInitialize();
                    cboCondition.Properties.View.Columns.Clear();
                    foreach (ColumnInfo columnInfo in controlEditorADO.ColumnInfos)
                    {
                        HorzAlignment hAlignment = HorzAlignment.Default;
                        switch (columnInfo.horzAlignment)
                        {
                            case ColumnInfo.HorzAlignment.Default:
                                hAlignment = HorzAlignment.Default;
                                break;
                            case ColumnInfo.HorzAlignment.Near:
                                hAlignment = HorzAlignment.Near;
                                break;
                            case ColumnInfo.HorzAlignment.Center:
                                hAlignment = HorzAlignment.Center;
                                break;
                            case ColumnInfo.HorzAlignment.Far:
                                hAlignment = HorzAlignment.Far;
                                break;
                        }

                        GridColumn gridColumn = cboCondition.Properties.View.Columns.AddField(columnInfo.fieldName);
                        gridColumn.Caption = columnInfo.caption;
                        gridColumn.Visible = columnInfo.visible;
                        gridColumn.VisibleIndex = columnInfo.VisibleIndex;
                        gridColumn.Width = ((columnInfo.width == 0) ? 100 : columnInfo.width);
                        gridColumn.AppearanceCell.TextOptions.HAlignment = hAlignment;
                        gridColumn.OptionsColumn.FixedWidth = columnInfo.FixedWidth;
                        gridColumn.ColumnEdit = repositoryItemMemoEdit;
                    }
                    cboCondition.Properties.View.OptionsView.RowAutoHeight = true;
                    cboCondition.Properties.View.OptionsView.ColumnAutoWidth = false;
                    cboCondition.Properties.View.OptionsView.ShowIndicator = false;
                    cboCondition.Properties.View.OptionsView.ShowGroupPanel = false;
                    cboCondition.Properties.PopupFormSize = new Size(controlEditorADO.PopupWidth + 20, 200);
                    cboCondition.Properties.View.OptionsView.ShowColumnHeaders = controlEditorADO.ShowHeader;
                }
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
                if (cboCondition.EditValue != null)
                {
                    this.refeshData(Convert.ToInt64(cboCondition.EditValue.ToString()));
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
