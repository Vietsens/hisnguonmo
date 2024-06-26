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
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HIS.Desktop.Plugins.MedicineType.ADO;
using HIS.Desktop.Controls.Session;
using System.Collections;
using DevExpress.XtraTreeList;
using HIS.Desktop.Plugins.MedicineTypeList;
using DevExpress.XtraTreeList.Nodes;
using System.Runtime.Remoting.Contexts;
using System.Linq;
using System.Threading.Tasks;
using HIS.Desktop.Common;
using System.Resources;
using System.ComponentModel;
using HIS.UC.MedicineType.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.MedicineType.Form;
using MOS.SDO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using HIS.UC.MedicineType;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.MedicineType.MedicineTypeList.Resources;

namespace HIS.Desktop.Plugins.MedicineType.MedicineTypeList
{
    public partial class UCMedcineTypeList : UserControl
    {
        #region Declare
        List<HIS.Desktop.Plugins.MedicineType.ADO.MedicineTypeADO> medicineTypeAdos;
        MedicineTypeProcessor medicineTypeProcessor;
        UserControl ucMedicineType;
        List<V_HIS_MEDICINE_TYPE> medicineTypes;
        List<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE> medicineTypeRef;
        CheckState checkStatethieuBHYT;//trạng thái checkbox thiếu thông tin BHYT
        CheckState checkStateAll; // trạng thái checkBox tất cả
        CheckState checkStateDuThongTinBHYT;// trạng thái checkBox đủ thông tin BHYT
        CheckState checkStateLock = CheckState.Unchecked; // trạng thái check button khóa
        Inventec.Desktop.Common.Modules.Module moduleData;
        MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE resultSaveFrmLock = null;
        HIS.UC.MedicineType.ADO.MedicineTypeADO currentRightClick;
        V_HIS_MEDI_STOCK mediStock = null;

        #endregion

        #region Construct
        public UCMedcineTypeList(Inventec.Desktop.Common.Modules.Module _moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = _moduleData;
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method

        private void LoadData()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineTypeViewFilter filter = new HisMedicineTypeViewFilter();

                if (this.checkStatethieuBHYT == CheckState.Checked)
                {
                    filter.IS_MISS_BHYT_INFO = true;
                }
                else if (this.checkStateDuThongTinBHYT == CheckState.Checked)
                {
                    filter.IS_MISS_BHYT_INFO = false;
                }
                else if (this.checkStateAll == CheckState.Checked)
                {
                    filter.IS_MISS_BHYT_INFO = null;
                }

                if (this.checkStateLock == CheckState.Checked)
                {
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE;
                }
                else
                {
                    filter.IS_ACTIVE = null;
                }

                if (mediStock == null)
                    mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ROOM_ID == this.moduleData.RoomId);
                if (mediStock != null && mediStock.IS_BUSINESS == 1)
                {
                    filter.IS_BUSINESS = true;
                }
                else
                {
                    filter.IS_BUSINESS = false;
                }
                filter.ORDER_FIELD = "MEDICINE_TYPE_NAME";
                filter.ORDER_DIRECTION = "ASC";
                List<string> ColnParams = new List<string> { "ID", "MEDICINE_TYPE_CODE", "MEDICINE_TYPE_NAME", "SERVICE_UNIT_NAME", "ACTIVE_INGR_BHYT_NAME",
                    "ACTIVE_INGR_BHYT_CODE", "CONCENTRA","HEIN_SERVICE_BHYT_CODE","HEIN_SERVICE_BHYT_NAME" ,"REGISTER_NUMBER", "NATIONAL_NAME", "MANUFACTURER_NAME",
                    "LAST_IMP_PRICE", "LAST_IMP_VAT_RATIO", "LAST_EXP_PRICE", "LAST_EXP_VAT_RATIO", "PARENT_ID","MEDICINE_USE_FORM_CODE","MEDICINE_USE_FORM_NAME",
                    "MEDICINE_GROUP_NAME","BYT_NUM_ORDER", "HEIN_SERVICE_TYPE_NAME", "ATC_CODES", "HEIN_LIMIT_RATIO",
                    "IS_LEAF", "IS_ACTIVE" ,"IS_BUSINESS", "IS_DRUG_STORE","LOCKING_REASON"};
                filter.ColumnParams = ColnParams;

                this.medicineTypes = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE>>(HisRequestUri.HIS_MEDICINE_TYPE_GetViewDynamic, ApiConsumers.MosConsumer, filter, param);
                if (mediStock != null && mediStock.IS_BUSINESS == 1 && mediStock.IS_SHOW_DRUG_STORE == 1)
                {
                    this.medicineTypes = this.medicineTypes != null ? this.medicineTypes.Where(o => o.IS_DRUG_STORE == 1).ToList() : null;
                }
                else if (mediStock != null && mediStock.IS_BUSINESS == 1 && mediStock.IS_SHOW_DRUG_STORE == null)
                {
                    this.medicineTypes = this.medicineTypes != null ? this.medicineTypes.Where(o => o.IS_DRUG_STORE == null).ToList() : null;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        private void FillDataToTreeControl()
        {
            try
            {
                medicineTypeAdos = new List<ADO.MedicineTypeADO>();
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineTypeViewFilter filter = new HisMedicineTypeViewFilter();
                if (this.checkStatethieuBHYT == CheckState.Checked)
                {
                    filter.IS_MISS_BHYT_INFO = true;
                }
                else if (this.checkStateDuThongTinBHYT == CheckState.Checked)
                {
                    filter.IS_MISS_BHYT_INFO = false;
                }
                else if (this.checkStateAll == CheckState.Checked)
                {
                    filter.IS_MISS_BHYT_INFO = null;
                }

                List<string> colunmParam = new List<string> { "ID", "MEDICINE_TYPE_CODE", "MEDICINE_TYPE_NAME", "SERVICE_UNIT_NAME", "ACTIVE_INGR_BHYT_NAME",
                    "ACTIVE_INGR_BHYT_CODE", "CONCENTRA","HEIN_SERVICE_BHYT_CODE","HEIN_SERVICE_BHYT_NAME" ,"REGISTER_NUMBER", "NATIONAL_NAME", "MANUFACTURER_NAME",
                    "LAST_IMP_PRICE", "LAST_IMP_VAT_RATIO", "LAST_EXP_PRICE", "LAST_EXP_VAT_RATIO", "PARENT_ID","MEDICINE_USE_FORM_CODE","MEDICINE_USE_FORM_NAME",
                    "MEDICINE_GROUP_NAME","BYT_NUM_ORDER", "HEIN_SERVICE_TYPE_NAME", "ATC_CODES", "HEIN_LIMIT_RATIO","IS_BUSINESS", "IS_DRUG_STORE",
                    "IS_LEAF", "IS_ACTIVE" };
                filter.ColumnParams = colunmParam;
                var medicineTypes = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE>>(HisRequestUri.HIS_MEDICINE_TYPE_GetViewDynamic, ApiConsumers.MosConsumer, filter, param);
                if (mediStock != null && mediStock.IS_BUSINESS == 1 && mediStock.IS_SHOW_DRUG_STORE == 1)
                {
                    this.medicineTypes = this.medicineTypes != null ? this.medicineTypes.Where(o => o.IS_DRUG_STORE == 1).ToList() : null;
                }
                else if (mediStock != null && mediStock.IS_BUSINESS == 1 && mediStock.IS_SHOW_DRUG_STORE == null)
                {
                    this.medicineTypes = this.medicineTypes != null ? this.medicineTypes.Where(o => o.IS_DRUG_STORE == null).ToList() : null;
                }
                foreach (var medicineType in medicineTypes)
                {
                    ADO.MedicineTypeADO medicineTypeAdo = new ADO.MedicineTypeADO(medicineType);


                    if (medicineType.IS_FUNCTIONAL_FOOD == 1)
                    {
                        medicineTypeAdo.IsFood = true;
                    }
                    else
                    {
                        medicineTypeAdo.IsFood = false;
                    }

                    if (medicineType.IS_STOP_IMP == 1)
                    {
                        medicineTypeAdo.IsStopImp = true;
                    }
                    else
                    {
                        medicineTypeAdo.IsStopImp = false;
                    }
                    if (medicineType.IS_AUTO_EXPEND == 1)
                    {
                        medicineTypeAdo.IsAutoExpend = true;
                    }
                    else
                    {
                        medicineTypeAdo.IsAutoExpend = false;
                    }
                    if (medicineType.IS_OUT_PARENT_FEE == 1)
                    {
                        medicineTypeAdo.IsCPNG = true;
                    }
                    else
                    {
                        medicineTypeAdo.IsCPNG = false;
                    }

                    medicineTypeAdos.Add(medicineTypeAdo);
                }
                MedicineTypeListProcess.FillDataToControl(medicineTypeAdos, this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void RefreshData()
        {
            this.medicineTypeProcessor.Reload(this.ucMedicineType, this.medicineTypes);
        }

        private List<DevExpress.Utils.Menu.DXMenuItem> MenuItems(HIS.UC.MedicineType.ADO.MedicineTypeADO data)
        {
            List<DevExpress.Utils.Menu.DXMenuItem> rs = null;
            try
            {
                currentRightClick = data;
                if (data.IS_ACTIVE == 1)
                {
                    rs = new List<DevExpress.Utils.Menu.DXMenuItem>();
                    DevExpress.Utils.Menu.DXMenuItem item = new DevExpress.Utils.Menu.DXMenuItem();
                    item.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__TITLE", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()); ;
                    item.Click += OnRightClick;
                    rs.Add(item);
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
                    var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == this.currentRightClick.ID);

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
                    this.MedicineType_PrintPriceList(currentRightClick);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MedicineType_PrintPriceList(HIS.UC.MedicineType.ADO.MedicineTypeADO parent)
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(mediStock);
                listArgs.Add((short)1);

                CallModule callModule = new CallModule(CallModule.PriceListExport, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InItMedicineTypeTree()
        {
            try
            {
                this.medicineTypeProcessor = new MedicineTypeProcessor();
                MedicineTypeInitADO ado = new MedicineTypeInitADO();
                ado.IsShowSearchPanel = true;
                ado.IsExportExcel = true;
                ado.IsShowRadioThieuThongTinBHYT = true;
                ado.IsShowImport = true;
                ado.IsShowChkLock = true;
                ado.MenuItems = MenuItems;
                ado.MedicineTypeColumns = new List<MedicineTypeColumn>();
                ado.SelectImageCollection = this.imageCollection1;
                ado.StateImageCollection = this.imageCollection1;
                //state image
                ado.MedicineType_GetStateImage = MedicineTypeGetStateImage;
                ado.MedicineType_StateImageClick = MedicineTypeStateImageClick;
                //select image
                ado.MedicineType_GetSelectImage = MedicineTypeGetSelectImage;
                ado.MedicineType_SelectImageClick = MedicineTypeSelectImageClick;
                //double click
                ado.MedicineTypeDoubleClick = MedicineTypeDoubleClick;
                ado.UpdateSingleRow = SetDataToMedicineTypeCreateForm;
                ado.MedicineTypeNodeCellStyle = MedicineTypeNodeCellStyle;
                // check thieu thong tin bhyt
                ado.checkThieuThongTinBHYT_CheckChange = checkThieuThongTinBHYT_CheckChange;
                ado.MedicineType_ExportExcel = MedicineType_ExportExcel;
                ado.MedicineType_Import = medicineType_Import;
                ado.MedicineType_PrintPriceList = MedicineType_PrintPriceList;
                ado.MedicineType_CustomUnboundColumnData = medicineType_CustomUnboundColumnData;
                // check khoa
                ado.chkLock_CheckChange = chkLock_CheckChange;
                ado.MedicineTypes = this.medicineTypes;
                ResourceLangManager.InitResourceLanguageManager();
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MedicineType.Resources.Lang", typeof(HIS.Desktop.Plugins.MedicineType.MedicineTypeList.UCMedcineTypeList).Assembly);
                //Column mã loại thuốc
                MedicineTypeColumn codeCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_MEDICINE_TYPE_CODE", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDICINE_TYPE_CODE", 200, false);
                codeCol.VisibleIndex = 0;
                codeCol.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
                ado.MedicineTypeColumns.Add(codeCol);
                //Column tên loại thuốc
                MedicineTypeColumn nameCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_MEDICINE_TYPE_NAME", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDICINE_TYPE_NAME", 250, false);
                nameCol.VisibleIndex = 1;
                nameCol.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
                ado.MedicineTypeColumns.Add(nameCol);
                //Column đơn vị tính
                MedicineTypeColumn serviceUnitNameCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_SERVICE_UNIT_NAME", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SERVICE_UNIT_NAME_STR", 100, false);
                serviceUnitNameCol.UnboundColumnType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                serviceUnitNameCol.VisibleIndex = 2;
                ado.MedicineTypeColumns.Add(serviceUnitNameCol);
                //Column hoạt chất
                MedicineTypeColumn activeIngrBhytNameCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_ACTIVE_INGR_BHYT_NAME", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "ACTIVE_INGR_BHYT_NAME", 120, false);
                activeIngrBhytNameCol.VisibleIndex = 3;
                ado.MedicineTypeColumns.Add(activeIngrBhytNameCol);
                //Column mã hoạt chất
                MedicineTypeColumn activeIngrBhytCodeCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_ACTIVE_INGR_BHYT_CODE", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "ACTIVE_INGR_BHYT_CODE", 100, false);
                activeIngrBhytCodeCol.VisibleIndex = 4;
                ado.MedicineTypeColumns.Add(activeIngrBhytCodeCol);
                //Column nồng độ hàm lượng
                MedicineTypeColumn concentraCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_CONCENTRA", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "CONCENTRA", 130, false);
                concentraCol.VisibleIndex = 5;
                ado.MedicineTypeColumns.Add(concentraCol);

                //Column Mã BHYT
                MedicineTypeColumn maBHYT = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_CODE_BHYT", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "HEIN_SERVICE_BHYT_CODE", 120, false);
                maBHYT.VisibleIndex = 6;
                ado.MedicineTypeColumns.Add(maBHYT);
                //Column Tên BHYT
                MedicineTypeColumn tenBHYT = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_NAME_BHYT", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "HEIN_SERVICE_BHYT_NAME", 120, false);
                tenBHYT.VisibleIndex = 7;
                ado.MedicineTypeColumns.Add(tenBHYT);

                //Column mã đường dùng
                MedicineTypeColumn medicineUseCodeCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_MEDICINE_USE_FORM_CODE", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDICINE_USE_FORM_CODE", 100, false);
                medicineUseCodeCol.VisibleIndex = 8;
                ado.MedicineTypeColumns.Add(medicineUseCodeCol);

                //Column tên đường dùng
                MedicineTypeColumn medicineUseNameCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_MEDICINE_USE_FORM_NAME2", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDICINE_USE_FORM_NAME", 120, false);
                medicineUseNameCol.VisibleIndex = 9;
                ado.MedicineTypeColumns.Add(medicineUseNameCol);

                //Column nhóm cha
                MedicineTypeColumn parentIDCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_PARENT_ID", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "PARENT_NAME", 140, false);
                parentIDCol.VisibleIndex = 10;
                ado.MedicineTypeColumns.Add(parentIDCol);

                //Column nhóm thuốc
                MedicineTypeColumn medicineGroupNameCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_MEDICINE_GROUP_NAME2", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDICINE_GROUP_NAME", 100, false);
                medicineGroupNameCol.VisibleIndex = 11;
                ado.MedicineTypeColumns.Add(medicineGroupNameCol);

                //Column STT(TT40)
                MedicineTypeColumn bytNumOrderCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_BYT_NUM_ORDER2", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BYT_NUM_ORDER", 100, false);
                bytNumOrderCol.VisibleIndex = 12;
                ado.MedicineTypeColumns.Add(bytNumOrderCol);

                //Column nhóm BHYT
                MedicineTypeColumn heinServiceBHYTNameCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_HEIN_SERVICE_TYPE_NAME", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "HEIN_SERVICE_TYPE_NAME", 130, false);
                heinServiceBHYTNameCol.VisibleIndex = 13;
                ado.MedicineTypeColumns.Add(heinServiceBHYTNameCol);

                //Column mã ATC
                MedicineTypeColumn atcCodesCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_ATC_CODES", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "ATC_CODES", 70, false);
                atcCodesCol.VisibleIndex = 14;
                ado.MedicineTypeColumns.Add(atcCodesCol);

                //Column tỷ lệ BHYT
                MedicineTypeColumn heinLimitRatioCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_HEIN_LIMIT_RATIO", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "HEIN_LIMIT_RATIO_STR", 120, false);
                heinLimitRatioCol.VisibleIndex = 15;
                ado.MedicineTypeColumns.Add(heinLimitRatioCol);


                //Column số đăng ký
                MedicineTypeColumn registerNumberNameCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_REGISTER_NUMBER", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "REGISTER_NUMBER", 120, false);
                registerNumberNameCol.VisibleIndex = 16;
                ado.MedicineTypeColumns.Add(registerNumberNameCol);

                //Column quốc gia
                MedicineTypeColumn nationalNameCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_NATIONAL_NAME", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "NATIONAL_NAME", 100, false);
                nationalNameCol.VisibleIndex = 17;
                ado.MedicineTypeColumns.Add(nationalNameCol);

                //Column hãng sản xuất
                MedicineTypeColumn manufacturerNameCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_MANUFACTURER_NAME", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MANUFACTURER_NAME", 120, false);
                manufacturerNameCol.VisibleIndex = 18;
                ado.MedicineTypeColumns.Add(manufacturerNameCol);

                //Column Lý do khóa
                MedicineTypeColumn lockingReasonCol = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_LOOKINGREASON", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "LOCKING_REASON", 120, false);
                lockingReasonCol.VisibleIndex = 19;
                ado.MedicineTypeColumns.Add(lockingReasonCol);
                //Column giá nhập

                MedicineTypeColumn importPrice = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_IMPORT_PRICE", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IMPORT_PRICE", 100, false);
                importPrice.VisibleIndex = 20;
                importPrice.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_TOOLTIP1", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.MedicineTypeColumns.Add(importPrice);
                //Column giá bán
                MedicineTypeColumn exportPrice = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_EXPORT_PRICE", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "EXPORT_PRICE", 100, false);
                exportPrice.VisibleIndex = 21;
                exportPrice.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDICINE_TYPE__TREE_MEDICINE_TYPE__COLUMN_TOOLTIP2", ResourceLangManager.LanguageUCMedicineType, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.MedicineTypeColumns.Add(exportPrice);


                this.ucMedicineType = (UserControl)medicineTypeProcessor.Run(ado);
                if (this.ucMedicineType != null)
                {
                    this.Controls.Add(this.ucMedicineType);
                    this.ucMedicineType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void medicineType_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    HIS.UC.MedicineType.ADO.MedicineTypeADO data = e.Row as HIS.UC.MedicineType.ADO.MedicineTypeADO;
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

                        else if (e.Column.FieldName == "SERVICE_UNIT_NAME_STR")
                        {
                            var rs = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.ID == data.ID).FirstOrDefault();
                            if (rs != null)
                                if (rs.IMP_UNIT_ID.HasValue)
                                    e.Value = rs.IMP_UNIT_NAME;
                                else
                                    e.Value = data.SERVICE_UNIT_NAME;
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
                        if (e.Column.FieldName == "PARENT_NAME")
                        {
                            if (data == null) return;
                            if (data.PARENT_ID.HasValue)
                            {
                                var rs = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == data.PARENT_ID);
                                if (rs != null)
                                {
                                    e.Value = rs.MEDICINE_TYPE_NAME;
                                }
                            }
                        }
                        if (e.Column.FieldName == "HEIN_LIMIT_RATIO_STR")
                        {
                            if (data == null) return;
                            if (data.HEIN_LIMIT_RATIO.HasValue)
                            {

                                e.Value = data.HEIN_LIMIT_RATIO * 100;

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
        public void checkThieuThongTinBHYT_CheckChange(CheckState _checkStateAll, CheckState _checkStateDuThongTinBHYT, CheckState _checkStateThieuThongTinBHYT)
        {
            try
            {
                this.checkStateAll = _checkStateAll;
                this.checkStateDuThongTinBHYT = _checkStateDuThongTinBHYT;
                this.checkStatethieuBHYT = _checkStateThieuThongTinBHYT;
                LoadData();
                RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MedicineTypeNodeCellStyle(object data, DevExpress.Utils.AppearanceObject appearanceObject)
        {
            try
            {
                if (data is HIS.UC.MedicineType.ADO.MedicineTypeADO)
                {
                    if (((HIS.UC.MedicineType.ADO.MedicineTypeADO)data).IS_LEAF != 1)
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

        private void MedicineTypeDoubleClick(UC.MedicineType.ADO.MedicineTypeADO _data)
        {
            try
            {
                if (_data != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_data.ID);
                    listArgs.Add(HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    listArgs.Add((DelegateSelectData)SetDataToMedicineTypeCreateForm);

                    CallModule callModule = new CallModule(CallModule.MedicineTypeCreate, 0, 0, listArgs);

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

        private void SetDataToMedicineTypeCreateForm(object data)
        {
            try
            {
                if (data != null && data is MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE)
                {
                    // get view medicineType
                    MOS.Filter.HisMedicineTypeViewFilter medicineTypeViewFilter = new HisMedicineTypeViewFilter();
                    medicineTypeViewFilter.ID = ((MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE)data).ID;
                    var medicineTypeViews = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICINE_TYPE>>(HisRequestUri.HIS_MEDICINE_TYPE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, medicineTypeViewFilter, new CommonParam());
                    if (medicineTypeViews != null && medicineTypeViews.Count > 0)
                    {
                        var medicineTypeAdo = new UC.MedicineType.ADO.MedicineTypeADO(medicineTypeViews.FirstOrDefault());
                        foreach (var item in this.medicineTypes)
                        {
                            if (item.ID == medicineTypeAdo.ID)
                            {
                                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MEDICINE_TYPE>(item, medicineTypeAdo);
                            }
                        }
                        this.medicineTypeProcessor.Reload(this.ucMedicineType, this.medicineTypes);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MedicineTypeGetStateImage(UC.MedicineType.ADO.MedicineTypeADO data, GetStateImageEventArgs e)
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

        private void MedicineTypeStateImageClick(UC.MedicineType.ADO.MedicineTypeADO data)
        {
            try
            {
                try
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    Inventec.Common.Adapter.BackendAdapter adapter = new Inventec.Common.Adapter.BackendAdapter(param);
                    success = adapter.Post<bool>(HisRequestUri.HIS_MEDICINE_TYPE_DELETE, ApiConsumers.MosConsumer, data, param);
                    WaitingManager.Hide();
                    if (success)
                    {
                        this.medicineTypes.RemoveAll(o => o.ID == data.ID);
                        this.medicineTypeProcessor.Reload(this.ucMedicineType, this.medicineTypes);
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

        private void MedicineTypeGetSelectImage(UC.MedicineType.ADO.MedicineTypeADO data, GetSelectImageEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        e.NodeImageIndex = 2;
                    else
                        e.NodeImageIndex = 3;
                }
                else
                    e.NodeImageIndex = -1;
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
                if (data != null && data is HIS_MEDICINE_TYPE)
                {
                    resultSaveFrmLock = (HIS_MEDICINE_TYPE)data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MedicineTypeSelectImageClick(UC.MedicineType.ADO.MedicineTypeADO data)
        {
            try
            {
                bool success = false;
                MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE medicineType = new HIS_MEDICINE_TYPE();
                AutoMapper.Mapper.CreateMap<UC.MedicineType.ADO.MedicineTypeADO, HIS_MEDICINE_TYPE>();
                medicineType = AutoMapper.Mapper.Map<HIS_MEDICINE_TYPE>(data);

                MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE vmedicineType = new V_HIS_MEDICINE_TYPE();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MEDICINE_TYPE>(vmedicineType, data);
                CommonParam param = new CommonParam();

                if (medicineType.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    frmLock frm = new frmLock(medicineType, SavefrmLockSucces);
                    frm.ShowDialog();
                }
                else
                {
                    WaitingManager.Show();
                    Inventec.Common.Adapter.BackendAdapter adapter = new Inventec.Common.Adapter.BackendAdapter(param);
                    resultSaveFrmLock = adapter.Post<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>("api/HisMedicineType/Unlock", ApiConsumers.MosConsumer, medicineType, param);
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
                        Parallel.ForEach(this.medicineTypes.Where(f => f.ID == resultSaveFrmLock.ID), l => l.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    }
                    else
                    {
                        Parallel.ForEach(this.medicineTypes.Where(f => f.ID == resultSaveFrmLock.ID), l => { l.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE; l.LOCKING_REASON = resultSaveFrmLock.LOCKING_REASON; });
                    }
                    Parallel.ForEach(this.medicineTypes.Where(f => f.ID == resultSaveFrmLock.ID), l => l.DESCRIPTION = resultSaveFrmLock.DESCRIPTION);
                    this.medicineTypeProcessor.Refresh(this.ucMedicineType, this.medicineTypes);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        #region exportexcel

        private void MedicineType_ExportExcel()
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
                  HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case ExportDataExcel.EXPORT_DATA_EXCEL:
                        richEditorMain.RunPrintTemplate("Mps000200", DelegateRunPrinterTest);
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
                    case "Mps000200":
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

                var list = (BindingList<HIS.UC.MedicineType.ADO.MedicineTypeADO>)medicineTypeProcessor.GetData(ucMedicineType);

                List<HIS.UC.MedicineType.ADO.MedicineTypeADO> listDataSources = list.ToList();
                List<V_HIS_MEDICINE_TYPE> listMedicines = new List<V_HIS_MEDICINE_TYPE>();
                var listLeaf = listDataSources.Where(o => o.IS_LEAF == 1).ToList();
                AutoMapper.Mapper.CreateMap<HIS.UC.MedicineType.ADO.MedicineTypeADO, V_HIS_MEDICINE_TYPE>();
                listMedicines = AutoMapper.Mapper.Map<List<V_HIS_MEDICINE_TYPE>>(listDataSources);

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

                if (listMedicines != null && listMedicines.Count > 0)
                {
                    MPS.Processor.Mps000200.PDO.Mps000200PDO mps000014RDO = new MPS.Processor.Mps000200.PDO.Mps000200PDO(listMedicines);
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
        #region Public method

        public void MeShow()
        {
            try
            {
                WaitingManager.Show();
                LoadData();
                InItMedicineTypeTree();
                SetFocus();
                SetEnableSave(false);
                WaitingManager.Hide();

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
                medicineTypeProcessor.FocusKeyword(ucMedicineType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void SetEnableSave(bool enable)
        {
            try
            {
                medicineTypeProcessor.EnableSaveButton(ucMedicineType, enable);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void Reload()
        {
            try
            {
                LoadData();
                if (medicineTypeProcessor != null)
                {
                    medicineTypeProcessor.Reload(this.ucMedicineType, this.medicineTypes);
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
        #endregion

        #region event control

        #endregion

        #region Import

        private void medicineType_Import()
        {
            try
            {
                WaitingManager.Show();


                List<object> listArgs = new List<object>();
                listArgs.Add((RefeshReference)ImportReload);
                CallModule callModule = new CallModule(CallModule.HisImportMedicineType, 0, 0, listArgs);

                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void convertDateStringToTimeNumber(string date, ref long? dateTime)
        {
            try
            {
                string[] substring = date.Split('/');
                string dateString = substring[2] + substring[1] + substring[0] + "000000";
                dateTime = Inventec.Common.TypeConvert.Parse.ToInt64(dateString);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addServiceToProcessList(List<MedicineTypeImportADO> _importAdo, ref List<HIS_MEDICINE_TYPE> _materialRef, ref List<V_HIS_MEDICINE_TYPE> _vmaterialRef, ref List<string> error)
        {
            try
            {
                _materialRef = new List<HIS_MEDICINE_TYPE>();
                _vmaterialRef = new List<V_HIS_MEDICINE_TYPE>();
                error = new List<string>();
                foreach (var item in _importAdo)
                {
                    var data = new HIS_MEDICINE_TYPE();
                    var vdata = new V_HIS_MEDICINE_TYPE();
                    data.HIS_SERVICE = new HIS_SERVICE();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEDICINE_TYPE>(data, item);
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MEDICINE_TYPE>(vdata, item);


                    if (!string.IsNullOrEmpty(item.ALLOW_EXPORT_ODD))
                    {
                        if (item.ALLOW_EXPORT_ODD == "x")
                        {
                            data.IS_ALLOW_EXPORT_ODD = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALLOW_ODD))
                    {
                        if (item.ALLOW_ODD == "x")
                        {
                            data.IS_ALLOW_ODD = 1;
                        }
                    }


                    if (!string.IsNullOrEmpty(item.BUSINESS))
                    {
                        if (item.BUSINESS == "x")
                        {
                            data.IS_BUSINESS = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.FUNCTIONAL_FOOD))
                    {
                        if (item.FUNCTIONAL_FOOD == "x")
                        {
                            data.IS_FUNCTIONAL_FOOD = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.REQUIRE_HSD))
                    {
                        if (item.REQUIRE_HSD == "x")
                        {
                            data.IS_REQUIRE_HSD = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SALE_EQUAL_IMP_PRICE))
                    {
                        if (item.SALE_EQUAL_IMP_PRICE == "x")
                        {
                            data.IS_SALE_EQUAL_IMP_PRICE = 1;
                        }
                    }


                    if (!string.IsNullOrEmpty(item.STOP_IMP))
                    {
                        if (item.STOP_IMP == "x")
                        {
                            data.IS_STOP_IMP = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.STAR_MARK))
                    {
                        if (item.STAR_MARK == "x")
                        {
                            data.IS_STAR_MARK = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_IN_TIME_STR))
                    {
                        long? dateTime = null;
                        convertDateStringToTimeNumber(item.HEIN_LIMIT_PRICE_IN_TIME_STR, ref dateTime);
                        if (dateTime != null)
                        {
                            data.HIS_SERVICE.HEIN_LIMIT_PRICE_IN_TIME = dateTime;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_INTR_TIME_STR))
                    {
                        long? dateTime = null;
                        convertDateStringToTimeNumber(item.HEIN_LIMIT_PRICE_INTR_TIME_STR, ref dateTime);
                        if (dateTime != null)
                        {
                            data.HIS_SERVICE.HEIN_LIMIT_PRICE_INTR_TIME = dateTime;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.OUT_PARENT_FEE))
                    {
                        if (item.OUT_PARENT_FEE == "x")
                        {
                            data.HIS_SERVICE.IS_OUT_PARENT_FEE = 1;
                        }
                    }

                    data.HIS_SERVICE.HEIN_LIMIT_PRICE = item.HEIN_LIMIT_PRICE;
                    data.HIS_SERVICE.HEIN_LIMIT_PRICE_OLD = item.HEIN_LIMIT_PRICE_OLD;
                    data.HIS_SERVICE.HEIN_LIMIT_RATIO = item.HEIN_LIMIT_RATIO;
                    data.HIS_SERVICE.HEIN_LIMIT_RATIO_OLD = item.HEIN_LIMIT_RATIO_OLD;
                    data.HIS_SERVICE.HEIN_SERVICE_BHYT_CODE = item.HEIN_SERVICE_BHYT_CODE;
                    data.HIS_SERVICE.HEIN_SERVICE_BHYT_CODE = item.HEIN_SERVICE_BHYT_CODE;
                    data.HIS_SERVICE.HEIN_SERVICE_BHYT_NAME = item.HEIN_SERVICE_BHYT_NAME;
                    data.HIS_SERVICE.HEIN_ORDER = item.HEIN_ORDER;
                    data.HIS_SERVICE.NUM_ORDER = item.NUM_ORDER;

                    _materialRef.Add(data);
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MEDICINE_TYPE>(vdata, data);
                    _vmaterialRef.Add(vdata);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion

        private void chkLock_CheckChange(CheckState checkState)
        {
            try
            {
                this.checkStateLock = checkState;
                if (this.checkStateLock == CheckState.Checked)
                {
                    LoadData();
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

