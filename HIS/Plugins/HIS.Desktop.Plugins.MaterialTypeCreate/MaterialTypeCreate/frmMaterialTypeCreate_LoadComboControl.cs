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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDA.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Utilities.Extensions;
using System.Runtime.InteropServices;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.MaterialTypeCreate.MaterialTypeCreate
{
    public partial class frmMaterialTypeCreate : HIS.Desktop.Utility.FormBase
    {
        private void LoadProvinceCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMaterialTypeParent.Properties.DataSource = null;
                    cboMaterialTypeParent.EditValue = null;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServiceUnit(string _serviceUnitCode)
        {
            try
            {
                List<HIS_SERVICE_UNIT> listResult = new List<HIS_SERVICE_UNIT>();
                listResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().Where(o => (o.SERVICE_UNIT_CODE != null && o.SERVICE_UNIT_CODE.StartsWith(_serviceUnitCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 250, 2));
                if (listResult.Count == 1)
                {
                    cboServiceUnit.EditValue = listResult[0].ID;
                    txtServiceUnitCode.Text = listResult[0].SERVICE_UNIT_CODE;
                    cboHeinServiceType.Focus();
                    cboHeinServiceType.SelectAll();
                }
                else if (listResult.Count > 1)
                {
                    cboServiceUnit.EditValue = null;
                    cboServiceUnit.Focus();
                    cboServiceUnit.ShowPopup();
                }
                else
                {
                    cboServiceUnit.EditValue = null;
                    cboServiceUnit.Focus();
                    cboServiceUnit.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadManufacturer(string _manufacturerCode)
        {
            try
            {
                List<HIS_MANUFACTURER> listResult = new List<HIS_MANUFACTURER>();
                listResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MANUFACTURER>().Where(o => (o.MANUFACTURER_CODE != null && o.MANUFACTURER_CODE.StartsWith(_manufacturerCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MANUFACTURER_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MANUFACTURER_NAME", "", 250, 2));
                if (listResult.Count == 1)
                {
                    cboManufacture.EditValue = listResult[0].ID;
                    txtManufactureCode.Text = listResult[0].MANUFACTURER_CODE;
                    txtPackingTypeCode.Focus();
                    txtPackingTypeCode.SelectAll();

                }
                else if (listResult.Count > 1)
                {
                    cboManufacture.EditValue = null;
                    cboManufacture.Focus();
                    cboManufacture.ShowPopup();
                }
                else
                {
                    cboManufacture.EditValue = null;
                    cboManufacture.Focus();
                    cboManufacture.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitHeinServiceType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_NAME", "", 100, 2));
                var a = HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer;
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_TYPE_NAME", "ID", columnInfos, false, 150);
                List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE> source = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>();
                if (source != null)
                {
                    ControlEditorLoader.Load(cboHeinServiceType, source.Where(o => new List<long>() { IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL }.Contains(o.ID)).ToList(), controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitNguonCTK()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("OTHER_PAY_SOURCE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("OTHER_PAY_SOURCE_NAME", "", 100, 2));
                var a = HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer;
                ControlEditorADO controlEditorADO = new ControlEditorADO("OTHER_PAY_SOURCE_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboCTK, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE>().Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitComboGender()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboGender, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboMaterialTypeMapId()
        {
            try
            {
                if (chkVatTu.Checked == false)
                {
                    this.lciMaterialTypeMap.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.LayCboAnhXa.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                }
                if (chkVatTu.Checked == true)
                {
                    this.LayCboAnhXa.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciMaterialTypeMap.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                this.lciMaterialTypeMap.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    InitCheck(cboMaterialTypeMapId, SelectionGrid__MaterialTypeMap);
                    List<HIS_MATERIAL_TYPE> datasource = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(o => o.ID != this.materialTypeId).ToList();
                    InitCombo(cboMaterialTypeMapId, datasource, "MATERIAL_TYPE_NAME", "ID", "MATERIAL_TYPE_CODE");
               
                //List<HIS_MATERIAL_TYPE_MAP> dataMaterialTypeMapId = new List<HIS_MATERIAL_TYPE_MAP>();

                //dataMaterialTypeMapId = BackendDataWorker.Get<HIS_MATERIAL_TYPE_MAP>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                //List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                //columnInfo.Add(new ColumnInfo("MATERIAL_TYPE_MAP_CODE", "Mã vật tư ánh xạ", 100, 1));
                //columnInfo.Add(new ColumnInfo("MATERIAL_TYPE_MAP_NAME", "Tên vật tư ánh xạ", 200, 1));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MATERIAL_TYPE_MAP_NAME", "ID", columnInfo, true, 300);
                //ControlEditorLoader.Load(cboMaterialTypeMapId, dataMaterialTypeMapId, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboMaterialTypeMapId_()
        {
            if (chkVatTu.Checked == false)
            {
                this.lciMaterialTypeMap.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.LayCboAnhXa.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            }
            if (chkVatTu.Checked == true)
            {
                this.LayCboAnhXa.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lciMaterialTypeMap.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            this.LayCboAnhXa.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            this.lciMaterialTypeMap.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            List<HIS_MATERIAL_TYPE_MAP> datasource_ = BackendDataWorker.Get<HIS_MATERIAL_TYPE_MAP>().Where(o => o.IS_ACTIVE == 1).ToList();
            List<ColumnInfo> columnInfo = new List<ColumnInfo>();
            columnInfo.Add(new ColumnInfo("MATERIAL_TYPE_MAP_CODE", "Mã vật tư ánh xạ", 50, 1));
            columnInfo.Add(new ColumnInfo("MATERIAL_TYPE_MAP_NAME", "Tên vật tư ánh xạ", 200, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("MATERIAL_TYPE_MAP_NAME", "ID", columnInfo, false, 250);
            ControlEditorLoader.Load(cboAnhXa, datasource_, controlEditorADO);
        }
        private async Task InitManufacture()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MANUFACTURER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MANUFACTURER_NAME", "", 100, 2));
                var a = HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer;
                ControlEditorADO controlEditorADO = new ControlEditorADO("MANUFACTURER_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboManufacture, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MANUFACTURER>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        private async Task InitServiceUnit()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceUnitFilter filter = new HisServiceUnitFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = await new BackendAdapter(param).GetAsync<List<HIS_SERVICE_UNIT>>("api/HisServiceUnit/Get", ApiConsumers.MosConsumer, filter, null);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 100, 2));
                var controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboServiceUnit, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>(), controlEditorADO);
                ControlEditorLoader.Load(this.cboImpUnit, data, controlEditorADO);
                cboImpUnit.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitMaterialType()
        {
            try
            {
                List<ColumnInfo> columinfo = new List<ColumnInfo>();
                columinfo.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 100, 1));
                columinfo.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 200, 2));
                ControlEditorADO controlEditorAdo = new ControlEditorADO("MATERIAL_TYPE_NAME", "ID", columinfo, false, 300);
                ControlEditorLoader.Load(cboMaterialType, BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorAdo);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private async Task InitHisFilmSeze()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("FILM_SIZE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("FILM_SIZE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("FILM_SIZE_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboFileSize, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_FILM_SIZE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitMedicineTypeParent()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MATERIAL_TYPE_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboMaterialTypeParent, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>().Where(o => o.IS_LEAF == null).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember, [Optional] string ma)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 350;
                col2.Caption = "Tất cả";
                col2.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                cbo.Properties.PopupFormWidth = 450;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                cbo.Properties.View.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DefaultBoolean.True;

                if (!String.IsNullOrWhiteSpace(ma))
                {
                    DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField(ma);
                    aColumnCode.Caption = "Mã";
                    aColumnCode.Visible = true;
                    aColumnCode.VisibleIndex = 1;
                    aColumnCode.Width = 150;
                }

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboMaterialTypeMapId.Properties.View);
                   
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

        private void SelectionGrid__MaterialTypeMap(object sender, EventArgs e)
        {
            try
            {
                MaterialTypeMap__Seleced = new List<HIS_MATERIAL_TYPE>();
                foreach (HIS_MATERIAL_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        MaterialTypeMap__Seleced.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
