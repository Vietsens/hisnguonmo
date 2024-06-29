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
using DevExpress.XtraTreeList;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.UC.MaterialType;
using HIS.UC.MaterialType.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Resources;
using HIS.Desktop.Plugins.MaterialType.Properties;
using System.ComponentModel;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.SDO;
using DevExpress.Utils;

namespace HIS.Desktop.Plugins.MaterialType.MaterialTypeList
{
    public partial class UCMaterialTypeList : UserControl
    {
        #region Declare
        MaterialTypeTreeProcessor materialTypeTreeProcessor;
        UserControl ucMaterialType;
        List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE> materialTypes;
        List<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE> materialTypeRef;
        HIS_MATERIAL_TYPE resultSaveFrmLock;
        Inventec.Desktop.Common.Modules.Module moduleData;
        CheckState checkStateLock = CheckState.Unchecked; // trạng thái check button khóa
        MaterialTypeADO currentRightClick;
        V_HIS_MEDI_STOCK currentMediStock = null;
        #endregion
        #region Construct

        public UCMaterialTypeList(Inventec.Desktop.Common.Modules.Module _moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = _moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method

        private void UCMaterialTypeList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadData();
                InitMaterialTypeTree();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private List<DevExpress.Utils.Menu.DXMenuItem> MenuItems(MaterialTypeADO data)
        {
            List<DevExpress.Utils.Menu.DXMenuItem> rs = null;
            try
            {
                currentRightClick = data;
                if (data != null)
                {
                    if (data.IS_ACTIVE == 1)
                    {
                        rs = new List<DevExpress.Utils.Menu.DXMenuItem>();
                        DevExpress.Utils.Menu.DXMenuItem item = new DevExpress.Utils.Menu.DXMenuItem();
                        item.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MATERIAL_TYPE__TITLE1", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        item.Click += OnRightClick;
                        rs.Add(item);
                    }

                    if (!data.IS_LEAF.HasValue || data.IS_LEAF.Value != 1)
                    {
                        rs = new List<DevExpress.Utils.Menu.DXMenuItem>();
                        DevExpress.Utils.Menu.DXMenuItem item = new DevExpress.Utils.Menu.DXMenuItem();
                        item.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MATERIAL_TYPE__TITLE2", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        item.Click += PrintPriceList;
                        rs.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }
        private void OnRightClick(object sender, EventArgs e)
        {
            try
            {
                if (this.currentRightClick != null)
                {
                    var data = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == this.currentRightClick.ID);

                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(data);
                    CallModule callModule = new CallModule(CallModule.HisServiceHein, 0, 0, listArgs);

                    WaitingManager.Hide();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintPriceList(object sender, EventArgs e)
        {
            try
            {
                if (this.currentRightClick != null)
                {
                    this.MaterialType_PrintPriceList(currentRightClick);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MaterialType_PrintPriceList(MaterialTypeADO parent)
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(currentMediStock);
                listArgs.Add((short)2);

                CallModule callModule = new CallModule(CallModule.PriceListExport, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        private void InitMaterialTypeTree()
        {
            try
            {
                materialTypeTreeProcessor = new MaterialTypeTreeProcessor();
                MaterialTypeInitADO ado = new MaterialTypeInitADO();
                ado.IsShowSearchPanel = true;
                ado.IsShowExportExcel = true;
                ado.IsShowImport = true;
                ado.IsShowChkLock = true;
                ado.MaterialTypeColumns = new List<MaterialTypeColumn>();
                ado.SelectImageCollection = this.imageCollection1;
                ado.StateImageCollection = this.imageCollection1;
                ado.MenuItems = MenuItems;
                //state image
                ado.MaterialType_GetStateImage = MaterialTypeGetStateImage;
                ado.MaterialType_StateImageClick = MaterialTypeStateImageClick;

                //select image
                ado.MaterialType_GetSelectImage = TreeMaterialTypeGetSelectImage;
                ado.MaterialType_SelectImageClick = TreeMaterialTypeSelectImageClick;
                //double click
                ado.MaterialTypeDoubleClick = MaterialTypeDoubleClick;
                ado.MaterialTypeNodeCellStyle = MaterialTypeNodeCellStyle;
                ado.UpdateSingleRow = SetDataToMedicineTypeCreateForm;
                ado.chkLock_CheckChange = chkLock_CheckChange;
                ado.MaterialTypes = this.materialTypes;
                ado.MaterialType_ExportExcel = MaterialType_ExportExcel;
                ado.MaterialType_Import = materialType_Import;
                ado.MaterialType_PrintPriceList = MaterialType_PrintPriceList;
                ado.MaterialType_CustomUnboundColumnData = materialType_CustomUnboundColumnData;
                ado.cboIsReusable_EditValueChanged = cboReusable_EditValueChanged;
                ResourceLangManager.InitResourceLanguageManager();

                ////Khoi tao doi tuong resource
                Resources.ResourceLangManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MaterialType.Resources.Lang", typeof(HIS.Desktop.Plugins.MaterialType.MaterialTypeList.UCMaterialTypeList).Assembly);

                //Column mã vật tư
                MaterialTypeColumn amountCol = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MATERIAL_TYPE__TREE_MATERIAL_TYPE__COLUMN_MATERIAL_TYPE_CODE", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MATERIAL_TYPE_CODE", 200, false);
                amountCol.VisibleIndex = 0;
                amountCol.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(amountCol);

                //Column tên vật tư
                MaterialTypeColumn serviceNameCol = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MATERIAL_TYPE__TREE_MATERIAL_TYPE__COLUMN_MATERIAL_TYPE_NAME", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MATERIAL_TYPE_NAME", 300, false);
                serviceNameCol.VisibleIndex = 1;
                serviceNameCol.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
                ado.MaterialTypeColumns.Add(serviceNameCol);

                //Column đơn vị tính
                MaterialTypeColumn virPriceCol = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MATERIAL_TYPE__TREE_MATERIAL_TYPE__COLUMN_SERVICE_UNIT_NAME", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SERVICE_UNIT_NAME_STR", 110, false);
                virPriceCol.VisibleIndex = 2;
                virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                virPriceCol.UnboundColumnType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.MaterialTypeColumns.Add(virPriceCol);

                //Column hàm lượng nồng độ
                MaterialTypeColumn virTotalPriceCol = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MATERIAL_TYPE__TREE_MATERIAL_TYPE__COLUMN_CONCENTRA", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "CONCENTRA", 150, false);
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(virTotalPriceCol);

                //Column quốc gia
                MaterialTypeColumn virTotalPatientPriceCol = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MATERIAL_TYPE__TREE_MATERIAL_TYPE__COLUMN_NATIONAL_NAME", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "NATIONAL_NAME", 110, false);
                virTotalPatientPriceCol.VisibleIndex = 4;
                virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPatientPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(virTotalPatientPriceCol);

                //Column hãng sản xuất
                MaterialTypeColumn virTotalHeinPriceCol = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MATERIAL_TYPE__TREE_MATERIAL_TYPE__COLUMN_MANUFACTURER_NAME", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MANUFACTURER_NAME", 250, false);
                virTotalHeinPriceCol.VisibleIndex = 5;
                virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalHeinPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(virTotalHeinPriceCol);

                //Column Lý do khóa
                MaterialTypeColumn lockingReasonCol = new MaterialTypeColumn("Lý do khóa", "LOCKING_REASON", 120, false);
                lockingReasonCol.VisibleIndex = 6;
                ado.MaterialTypeColumns.Add(lockingReasonCol);

                //Column hãng giá nhập
                MaterialTypeColumn virTotalImpPriceCol = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MATERIAL_TYPE__TREE_MATERIAL_TYPE__COLUMN_IMPORT_PRICE", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IMPORT_PRICE", 120, false);
                virTotalImpPriceCol.VisibleIndex = 7;
                virTotalImpPriceCol.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MATERIAL_TYPE__COLUMN_TOOLTIP1", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                virTotalImpPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalImpPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(virTotalImpPriceCol);

                //Column hãng giá xuất 
                MaterialTypeColumn virTotalExpPriceCol = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MATERIAL_TYPE__TREE_MATERIAL_TYPE__COLUMN_EXPORT_PRICE", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "EXPORT_PRICE", 120, false);
                virTotalExpPriceCol.VisibleIndex = 8;
                virTotalExpPriceCol.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MATERIAL_TYPE__COLUMN_TOOLTIP2", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                virTotalExpPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalExpPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(virTotalExpPriceCol);

                //Column Mã BHYT
                MaterialTypeColumn maBHYT = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MATERIAL_TYPE__COLUMN_MA_BHYT", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "HEIN_SERVICE_BHYT_CODE", 120, false);
                maBHYT.VisibleIndex = 9;
                maBHYT.Format = new DevExpress.Utils.FormatInfo();
                maBHYT.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(maBHYT);

                //Column Tên BHYT
                MaterialTypeColumn tenBHYT = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MATERIAL_TYPE__COLUMN_TEN_BHYT", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "HEIN_SERVICE_BHYT_NAME", 120, false);
                tenBHYT.VisibleIndex = 10;
                tenBHYT.Format = new DevExpress.Utils.FormatInfo();
                tenBHYT.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(tenBHYT);

                //Column số đăng ký
                MaterialTypeColumn registerNumberCol = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MATERIAL_TYPE__TREE_MATERIAL_TYPE__COLUMN_REGISTER_NUMBER", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "REGISTER_NUMBER", 120, false);
                registerNumberCol.VisibleIndex = 11;
                registerNumberCol.Format = new DevExpress.Utils.FormatInfo();
                registerNumberCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(registerNumberCol);

                //Column Giá tiền
                MaterialTypeColumn impPriceCol = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MATERIAL_TYPE__TREE_MATERIAL_TYPE__COLUMN_IMP_PRICE", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IMP_PRICE", 120, false);
                impPriceCol.VisibleIndex = -1;
                impPriceCol.Format = new DevExpress.Utils.FormatInfo();
                impPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                impPriceCol.UnboundColumnType = DevExpress.XtraTreeList.Data.UnboundColumnType.Bound;
                ado.MaterialTypeColumns.Add(impPriceCol);

                //Column Vat Giá tiền
                MaterialTypeColumn impPriceVatCol = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MATERIAL_TYPE__TREE_MATERIAL_TYPE__COLUMN_VAT_PRICE", ResourceLangManager.LanguageUCMaterialType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IMP_VAT_RATIO", 120, false);
                impPriceVatCol.VisibleIndex = -1;
                impPriceVatCol.Format = new DevExpress.Utils.FormatInfo();
                impPriceVatCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                impPriceCol.UnboundColumnType = DevExpress.XtraTreeList.Data.UnboundColumnType.Bound;
                ado.MaterialTypeColumns.Add(impPriceVatCol);

                //Column Vat Giá tiền
                MaterialTypeColumn packingTypeCol = new MaterialTypeColumn(" ", "PACKING_TYPE_NAME", 120, false);
                packingTypeCol.VisibleIndex = -1;
                packingTypeCol.Format = new DevExpress.Utils.FormatInfo();
                packingTypeCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                impPriceCol.UnboundColumnType = DevExpress.XtraTreeList.Data.UnboundColumnType.Bound;
                ado.MaterialTypeColumns.Add(packingTypeCol);

                this.ucMaterialType = (UserControl)materialTypeTreeProcessor.Run(ado);
                if (this.ucMaterialType != null)
                {
                    this.Controls.Add(this.ucMaterialType);
                    this.ucMaterialType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        int? reusable = null;
        private void cboReusable_EditValueChanged(int? data)
        {
            try
            {
                reusable = data;
                LoadData();
                if (materialTypeTreeProcessor != null)
                {
                    materialTypeTreeProcessor.Reload(this.ucMaterialType, this.materialTypes);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }
        private void materialType_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    HIS.UC.MaterialType.ADO.MaterialTypeADO data = e.Row as HIS.UC.MaterialType.ADO.MaterialTypeADO;

                    if (data != null)
                    {
                        if (e.Column.FieldName == "IMPORT_PRICE")
                        {
                            if (data.LAST_IMP_VAT_RATIO != null)
                            {
                                if (data.LAST_IMP_PRICE != null)
                                {
                                    e.Value = data.LAST_IMP_PRICE * (1 + data.LAST_IMP_VAT_RATIO);
                                }
                                else
                                {
                                    e.Value = null;
                                }
                            }
                            else
                            {
                                e.Value = 0;
                            }
                        }
                        else if (e.Column.FieldName == "EXPORT_PRICE")
                        {
                            if (data.LAST_EXP_VAT_RATIO != null)
                            {
                                if (data.LAST_EXP_PRICE != null)
                                {
                                    e.Value = data.LAST_EXP_PRICE * (1 + data.LAST_EXP_VAT_RATIO);
                                }
                                else
                                {
                                    e.Value = null;
                                }
                            }
                            else
                            {
                                e.Value = 0;
                            }
                        }
                        else if (e.Column.FieldName == "SERVICE_UNIT_NAME_STR")
                        {
                            var rs = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.ID == data.ID).FirstOrDefault();
                            if (rs != null)
                                if (rs.IMP_UNIT_ID.HasValue)
                                    e.Value = rs.IMP_UNIT_NAME;
                                else
                                    e.Value = data.SERVICE_UNIT_NAME;
                        }
                        else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(data.IMP_PRICE ?? 0);
                        }
                        else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            e.Value = (data.IMP_VAT_RATIO * 100);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void MaterialTypeNodeCellStyle(object data, DevExpress.Utils.AppearanceObject appearanceObject)
        {
            try
            {
                if (data is HIS.UC.MaterialType.ADO.MaterialTypeADO)
                {
                    if (((HIS.UC.MaterialType.ADO.MaterialTypeADO)data).IS_LEAF != 1)
                    {
                        appearanceObject.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void MaterialTypeStateImageClick(MaterialTypeADO data)
        {
            try
            {
                try
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    Inventec.Common.Adapter.BackendAdapter adapter = new Inventec.Common.Adapter.BackendAdapter(param);
                    success = adapter.Post<bool>(HisRequestUri.HIS_MATERIAL_TYPE_DELETE, ApiConsumers.MosConsumer, data, param);
                    WaitingManager.Hide();
                    if (success)
                    {
                        this.materialTypes.RemoveAll(o => o.ID == data.ID);
                        this.materialTypeTreeProcessor.Reload(this.ucMaterialType, this.materialTypes);
                    }

                    MessageManager.Show(param, success);
                    SessionManager.ProcessTokenLost(param);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MaterialTypeDoubleClick(MaterialTypeADO _data)
        {
            try
            {
                try
                {
                    if (_data != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(_data.ID);
                        listArgs.Add(HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                        listArgs.Add((DelegateSelectData)SetDataToMedicineTypeCreateForm);
                        CallModule callModule = new CallModule(CallModule.MaterialTypeCreate, 0, 0, listArgs);
                        SetFocus();
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn("medicineTypeFocus is null");
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToMedicineTypeCreateForm(object data)
        {
            try
            {
                if (data != null && data is MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE)
                {
                    MOS.Filter.HisMaterialTypeViewFilter materialTypeViewFilter = new HisMaterialTypeViewFilter();
                    materialTypeViewFilter.ID = ((MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE)data).ID;
                    var materialTypeViews = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MATERIAL_TYPE>>(HisRequestUri.HIS_MATERIAL_TYPE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, materialTypeViewFilter, new CommonParam());
                    if (materialTypeViews != null && materialTypeViews.Count > 0)
                    {
                        var materialTypeAdo = new UC.MaterialType.ADO.MaterialTypeADO(materialTypeViews.FirstOrDefault());
                        foreach (var item in this.materialTypes)
                        {
                            if (item.ID == materialTypeAdo.ID)
                            {
                                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MATERIAL_TYPE>(item, materialTypeAdo);
                            }
                        }
                        this.materialTypeTreeProcessor.Reload(this.ucMaterialType, this.materialTypes);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void MaterialTypeGetStateImage(MaterialTypeADO data, GetStateImageEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    e.NodeImageIndex = 1;
                }
                else
                    e.NodeImageIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TreeMaterialTypeGetSelectImage(MaterialTypeADO noteData, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                if (noteData != null)
                {
                    if (noteData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        e.NodeImageIndex = 2;
                    }
                    else
                    {
                        e.NodeImageIndex = 3;
                    }
                }
                else
                {
                    e.NodeImageIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SavefrmLockSucces(object data)
        {
            try
            {
                if (data != null && data is HIS_MATERIAL_TYPE)
                {
                    resultSaveFrmLock = (HIS_MATERIAL_TYPE)data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TreeMaterialTypeSelectImageClick(MaterialTypeADO data)
        {
            try
            {
                bool success = false;
                MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE materialType = new HIS_MATERIAL_TYPE();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>(materialType, data);
                CommonParam param = new CommonParam();

                if (materialType.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    HIS.Desktop.Plugins.MaterialType.Form.frmLock frm = new HIS.Desktop.Plugins.MaterialType.Form.frmLock(materialType, SavefrmLockSucces);
                    frm.ShowDialog();
                }
                else
                {                
                    WaitingManager.Show();
                    Inventec.Common.Adapter.BackendAdapter adapter = new Inventec.Common.Adapter.BackendAdapter(param);
                    resultSaveFrmLock = adapter.Post<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>("api/HisMaterialType/Unlock", ApiConsumers.MosConsumer, materialType, param);
                    WaitingManager.Hide();
                    if (resultSaveFrmLock != null)
                    {
                        success = true;
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                }
                if (resultSaveFrmLock != null)
                {
                    if (resultSaveFrmLock.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        Parallel.ForEach(this.materialTypes.Where(f => f.ID == resultSaveFrmLock.ID), l => l.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    }
                    else
                    {
                        Parallel.ForEach(this.materialTypes.Where(f => f.ID == resultSaveFrmLock.ID), l => { l.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE; l.LOCKING_REASON = resultSaveFrmLock.LOCKING_REASON; });
                    }
                    Parallel.ForEach(this.materialTypes.Where(f => f.ID == resultSaveFrmLock.ID), l => l.DESCRIPTION = resultSaveFrmLock.DESCRIPTION);

                    this.materialTypeTreeProcessor.Reload(this.ucMaterialType, this.materialTypes);             
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void RefreshData()
        {
            this.materialTypeTreeProcessor.Reload(this.ucMaterialType, this.materialTypes);
        }

        private void LoadData()
        {
            try
            {
                if (currentMediStock == null)
                {
                    currentMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ROOM_ID == this.moduleData.RoomId);
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisMaterialTypeViewFilter filter = new HisMaterialTypeViewFilter();
                filter.ORDER_FIELD = "MATERIAL_TYPE_NAME";
                filter.ORDER_DIRECTION = "ASC";
                if (currentMediStock != null && currentMediStock.IS_BUSINESS == 1)
                {
                    filter.IS_BUSINESS = true;
                }
                else
                {
                    filter.IS_BUSINESS = false;
                }

                if (reusable != null)
                {
                    if (reusable == 1)
                    {
                        filter.IS_REUSABLE = true;
                    }
                    else if (reusable == 2)
                    {
                        filter.IS_REUSABLE = false;
                    }
                }
                if (checkStateLock != null && checkStateLock == CheckState.Checked)
                {
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE;
                }
                else
                {
                    filter.IS_ACTIVE = null;
                }
                List<String> ColnParams = new List<string> { "ID", "MATERIAL_TYPE_CODE", "MATERIAL_TYPE_NAME", "SERVICE_UNIT_NAME", "CONCENTRA",
                    "NATIONAL_NAME", "MANUFACTURER_NAME", "LAST_IMP_PRICE", "LAST_IMP_VAT_RATIO",
                    "LAST_EXP_PRICE","HEIN_SERVICE_BHYT_CODE","HEIN_SERVICE_BHYT_NAME", "LAST_EXP_VAT_RATIO", "IS_LEAF", "PARENT_ID", "IS_ACTIVE", "REGISTER_NUMBER",
                    "IMP_VAT_RATIO","IMP_PRICE","PACKING_TYPE_NAME","IS_BUSINESS", "IS_DRUG_STORE", "LOCKING_REASON", "IS_REUSABLE"};
                filter.ColumnParams = ColnParams;
                this.materialTypes = new BackendAdapter(param).Get<List<V_HIS_MATERIAL_TYPE>>(HisRequestUri.HIS_MATERIAL_TYPE_GETVIEWDynamic, ApiConsumers.MosConsumer, filter, param);
                if (currentMediStock != null && currentMediStock.IS_BUSINESS == 1 && currentMediStock.IS_SHOW_DRUG_STORE == 1)
                {
                    this.materialTypes = this.materialTypes != null ? this.materialTypes.Where(o => o.IS_DRUG_STORE == 1).ToList() : null;
                }
                else if (currentMediStock != null && currentMediStock.IS_BUSINESS == 1 && currentMediStock.IS_SHOW_DRUG_STORE == null)
                {
                    this.materialTypes = this.materialTypes != null ? this.materialTypes.Where(o => o.IS_DRUG_STORE == null).ToList() : null;
                }
                WaitingManager.Hide();
                //var lst = this.materialTypes.Where(o => o.REGISTER_NUMBER != null).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        #region Public method
        public void Reload()
        {
            try
            {
                LoadData();
                if (materialTypeTreeProcessor != null)
                {
                    materialTypeTreeProcessor.Reload(this.ucMaterialType, this.materialTypes);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ImportReload()
        {
            try
            {
                Reload();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void Search()
        {
            try
            {
                if (materialTypeTreeProcessor != null)
                {
                    materialTypeTreeProcessor.Search(this.ucMaterialType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void New()
        {
            try
            {
                if (materialTypeTreeProcessor != null)
                {
                    materialTypeTreeProcessor.New(this.ucMaterialType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetFocus()
        {
            try
            {
                materialTypeTreeProcessor.FocusKeyword(ucMaterialType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion

        #region ExportExcell
        private void MaterialType_ExportExcel()
        {
            PrintProcess(ExportDataExcel.EXPORT_DATA_EXCEL);
        }

        internal enum ExportDataExcel
        {
            EXPORT_DATA_EXCEL
        }

        void PrintProcess(ExportDataExcel printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore
                  (HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                  Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                  (HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath));

                switch (printType)
                {
                    case ExportDataExcel.EXPORT_DATA_EXCEL:
                        richEditorMain.RunPrintTemplate("Mps000201", DelegateRunPrinterTest);
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

        bool DelegateRunPrinterTest(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000201":
                        LoadBieuMauPhieuYCInKetQuaXetNghiem(printTypeCode, fileName, ref result);
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

        private void LoadBieuMauPhieuYCInKetQuaXetNghiem(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();

                var getdata = (BindingList<MaterialTypeADO>)materialTypeTreeProcessor.GetData(ucMaterialType);

                string savePath = null;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    savePath = saveFileDialog1.FileName;
                }
                else
                {
                    WaitingManager.Hide();
                    return;
                }

                List<MaterialTypeADO> listDataSources = getdata.ToList();
                List<V_HIS_MATERIAL_TYPE> listMedicines = new List<V_HIS_MATERIAL_TYPE>();
                var listLeaf = listDataSources.Where(o => o.IS_LEAF == 1).ToList();
                AutoMapper.Mapper.CreateMap<MaterialTypeADO, V_HIS_MATERIAL_TYPE>();

                listMedicines = AutoMapper.Mapper.Map<List<V_HIS_MATERIAL_TYPE>>(listDataSources);

                if (listMedicines != null && listMedicines.Count > 0)
                {
                    MPS.Processor.Mps000201.PDO.Mps000201PDO mps000014RDO = new MPS.Processor.Mps000201.PDO.Mps000201PDO(listMedicines);
                    WaitingManager.Show();
                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "", 1, savePath + ".xlsx");
                    result = MPS.MpsPrinter.Run(PrintData);
                    WaitingManager.Hide();
                    if (result)
                    {
                        MessageManager.Show(Resources.ResourceMessage.Plugin_XuatFileThanhCong);
                    }
                    else
                    {
                        MessageManager.Show(Resources.ResourceMessage.Plugin_XuatFileThatBai);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region Import

        private void materialType_Import()
        {
            try
            {
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add((RefeshReference)ImportReload);
                CallModule callModule = new CallModule(CallModule.HisImportMaterialType, 0, 0, listArgs);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void chkLock_CheckChange(CheckState checkState)
        {
            try
            {
                this.checkStateLock = checkState;
                LoadData();
                RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}

