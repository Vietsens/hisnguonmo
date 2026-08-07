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
using DevExpress.XtraExport;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicineMediStockSummary
{
    public partial class ucMediaStockSummaryByMedicine : UserControl
    {
        //Field dựng cây: cấp 1 nhóm thuốc/vật tư -> cấp 2 thuốc/vật tư -> cấp 3 lô
        private const string FIELD_NAME__NODE_ID = "NODE_ID";
        private const string FIELD_NAME__PARENT_NODE_ID = "PARENT_NODE_ID";
        private const string FIELD_NAME__NODE_DISPLAY = "NODE_DISPLAY";

        private const string FIELD_NAME__PARENT_TYPE_NAME = "PARENT_TYPE_NAME";
        private const string FIELD_NAME__PACKAGE_NUMBER = "PACKAGE_NUMBER";
        private const string FIELD_NAME__EXPIRED_DATE_DISPLAY = "EXPIRED_DATE_DISPLAY";
        private const string FIELD_NAME__TOTAL_AMOUNT = "TOTAL_AMOUNT";
        private const string FIELD_NAME__MEDICINE_TYPE_CODE = "MEDICINE_TYPE_CODE";
        private const string FIELD_NAME__MEDICINE_TYPE_NAME = "MEDICINE_TYPE_NAME";
        private const string FIELD_NAME__MATERIAL_TYPE_CODE = "MATERIAL_TYPE_CODE";
        private const string FIELD_NAME__MATERIAL_TYPE_NAME = "MATERIAL_TYPE_NAME";

        long RoomId;
        long RoomTypeId;
        string tenkhoHientai;
        List<long> roomIds = new List<long>();
        bool loadFirst = true;

        string fileNameMedicine = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", "HIS.Desktop.Plugins.MedicineMediStockSummary.trvMediMateStockSum1.xml"));
        string fileNameMaterial = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", "HIS.Desktop.Plugins.MedicineMediStockSummary.trvMediMateStockSum2.xml"));

        List<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM> lstV_UserRoom;
        List<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM> lstV_UserRoomForBranch;
        HIS_MEDI_STOCK mediStock;
        MedicineTypeInHospitalSDO lstMediInStocks = new MedicineTypeInHospitalSDO();
        MaterialTypeInHospitalSDO lstMateInStocks = new MaterialTypeInHospitalSDO();

        public ucMediaStockSummaryByMedicine(long roomId, long roomTypeId)
        {
            InitializeComponent();
            this.RoomId = roomId;
            this.RoomTypeId = roomTypeId;
        }

        private void ucMediaStockSummaryByMedicine_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                InitCheck(cboBrach, SelectionGrid__Status);
                lstV_UserRoom = new List<V_HIS_USER_ROOM>();

                var rooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var branchId = HIS.Desktop.LocalStorage.LocalData.BranchWorker.GetCurrentBranchId();

                lstV_UserRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == loginName && (o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)).ToList();
                if (lstV_UserRoom != null)
                    roomIds = lstV_UserRoom.Select(o => o.ROOM_ID).Distinct().ToList();

                lstV_UserRoomForBranch  = lstV_UserRoom.GroupBy(o => o.BRANCH_ID).Select(g => g.FirstOrDefault()).ToList();
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomIds), roomIds));
                var _WorkPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.RoomId);
                if (_WorkPlace != null)
                {
                    mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == RoomId).FirstOrDefault();
                    string tenkho = mediStock.MEDI_STOCK_NAME;
                    tenkhoHientai = Inventec.Common.String.Convert.UnSignVNese(tenkho.ToLower().Trim());
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tenkhoHientai), tenkhoHientai));

                    ShowGridControl(loadFirst);
                }

                InitCombo(cboBrach, lstV_UserRoomForBranch, "BRANCH_NAME", "BRANCH_ID");     
                //InitComboBranh();
                //InitComboBranhCheck();  
                  
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowGridControl(bool _loadFirst)
        {
            try
            {
                trvMediMateStockSum.DataSource = null;
                trvMediMateStockSum.Columns.Clear();
                string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtKeyword.Text.ToLower().Trim());

                List<HIS_MEDI_STOCK> MediStock = new List<HIS_MEDI_STOCK>();
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                if (chkMedicine.Checked)
                {
                    if (lstV_UserRoom != null)
                    {
                        var selectedBranchIds = cboBrach.EditValue as IEnumerable<object>;
                        var branchIdList = lstV_UserRoom.Select(o => o.BRANCH_ID).ToList();
                        roomIds = lstV_UserRoom.Where(o => branchIdList.Contains(o.BRANCH_ID)).Select(o => o.ROOM_ID).Distinct().ToList();
                    }

                    txtKeyword.Focus();
                    MOS.Filter.HisMedicineTypeHospitalViewFilter mediFilter = new MOS.Filter.HisMedicineTypeHospitalViewFilter();
                    if (mediStock.IS_BUSINESS == 1)
                    {
                        mediFilter.IS_BUSINESS = true;
                        foreach (long roomID in roomIds)
                        {
                            HIS_MEDI_STOCK medi = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == roomID).FirstOrDefault();
                            if (medi != null && medi.IS_BUSINESS == 1)
                            {
                                MediStock.Add(medi);
                            }
                        }

                    }
                    else
                    {
                        mediFilter.IS_BUSINESS = false;
                        foreach (long roomID in roomIds)
                        {
                            HIS_MEDI_STOCK medi = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == roomID).FirstOrDefault();
                            if (medi != null && medi.IS_BUSINESS != 1)
                                MediStock.Add(medi);
                        }
                    }

                    lstMediInStocks = new MedicineTypeInHospitalSDO();
                    List<MedicineTypeInHospitalSDO> MediHosList = new List<MedicineTypeInHospitalSDO>();
                    int count = 0;
                    
                    while (MediStock.Count - count > 0)
                    {
                        mediFilter.MEDI_STOCK_IDs = MediStock.Select(o => o.ID).Skip(count).Take(100).ToList();
                        var lstMediInStocksTmp = new BackendAdapter(param).Get<MedicineTypeInHospitalSDO>("api/HisMedicineType/GetInHospitalMedicineType", ApiConsumers.MosConsumer, mediFilter, param);
                        if (lstMediInStocksTmp != null)
                            MediHosList.Add(lstMediInStocksTmp);
                        count += 100;
                    }

                    if (MediHosList != null && MediHosList.Count > 0)
                    {
                        List<string> MediStockNames = new List<string>();

                        List<string> MediStockCodes = new List<string>();

                        List<ExpandoObject> MedicineTypeDatas = new List<ExpandoObject>();
                        foreach (var item in MediHosList)
                        {
                            if (item.MediStockNames == null || item.MediStockNames.Count == 0)
                                continue;
                            MediStockNames.AddRange(item.MediStockNames);
                            MediStockCodes.AddRange(item.MediStockCodes);
                            MedicineTypeDatas.AddRange(item.MedicineTypeDatas);
                        }

                        var ListMedicineTypeTemp = new List<ExpandoObject>();
                        foreach (var item in MedicineTypeDatas)
                        {
                            foreach (var codeStock in MediStockCodes)
                            {
                                if (!((IDictionary<string, object>)item).ContainsKey(codeStock))
                                {
                                    ((IDictionary<string, object>)item).Add(codeStock, 0m);
                                }
                            }
                            ListMedicineTypeTemp.Add(item);
                        }
                        MedicineTypeDatas = ListMedicineTypeTemp;

                        //Bỏ kho trùng nhưng giữ đúng cặp mã kho - tên kho (2 kho khác nhau vẫn có thể trùng tên)
                        List<string> distinctMediStockCodes = null;
                        List<string> distinctMediStockNames = null;
                        DistinctMediStocks(MediStockCodes, MediStockNames, out distinctMediStockCodes, out distinctMediStockNames);

                        lstMediInStocks = new MedicineTypeInHospitalSDO()
                        {
                            MediStockCodes = distinctMediStockCodes,
                            MediStockNames = distinctMediStockNames,
                            MedicineTypeDatas = MedicineTypeDatas.OrderBy(x => ((IDictionary<string, object>)x)[FIELD_NAME__MEDICINE_TYPE_CODE]).ToList()
                        };
                    }
                    initTreeMedicine(lstMediInStocks);

                    if (lstMediInStocks.MedicineTypeDatas != null && lstMediInStocks.MedicineTypeDatas.Count > 0)
                    {
                        //Cấp 1: nhóm thuốc -> Cấp 2: thuốc -> Cấp 3: từng lô     
                        DataTable treeData = BuildTreeTable(
                            lstMediInStocks.MedicineTypeDatas,
                            FIELD_NAME__MEDICINE_TYPE_CODE,
                            FIELD_NAME__MEDICINE_TYPE_NAME,
                            GetMedicineTextFieldNames(),
                            lstMediInStocks.MediStockCodes,
                            lstMediInStocks.MediStockNames,
                            GetMedicinePackageStocks(MediStock.Select(o => o.ID).Distinct().ToList()));
                        BindTree(treeData);
                    }
                }
                else if (chkMaterial.Checked)
                {
                    txtKeyword.Focus();
                    MOS.Filter.HisMaterialTypeHospitalViewFilter mateFilter = new MOS.Filter.HisMaterialTypeHospitalViewFilter();
                    if (mediStock.IS_BUSINESS == 1)
                    {
                        mateFilter.IS_BUSINESS = true;
                        foreach (long roomID in roomIds)
                        {
                            HIS_MEDI_STOCK medi = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == roomID).FirstOrDefault();
                            if (medi != null && medi.IS_BUSINESS == 1)
                                MediStock.Add(medi);
                        }

                    }
                    else
                    {
                        mateFilter.IS_BUSINESS = false;
                        foreach (long roomID in roomIds)
                        {
                            HIS_MEDI_STOCK medi = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == roomID).FirstOrDefault();
                            if (medi != null && medi.IS_BUSINESS != 1)
                                MediStock.Add(medi);
                        }

                    }

                    lstMateInStocks = new MaterialTypeInHospitalSDO();
                    List<MaterialTypeInHospitalSDO> MateHosList = new List<MaterialTypeInHospitalSDO>();
                    int count = 0;
                    while (MediStock.Count - count > 0)
                    {
                        mateFilter.MEDI_STOCK_IDs = MediStock.Select(o => o.ID).Skip(count).Take(100).ToList();

                        var lstMateInStocksTmp = new BackendAdapter(param).Get<MaterialTypeInHospitalSDO>("api/HisMaterialType/GetInHospitalMaterialType", ApiConsumers.MosConsumer, mateFilter, param);
                        if (lstMateInStocksTmp != null)
                            MateHosList.Add(lstMateInStocksTmp);
                        count += 100;
                    }
                    if (MateHosList != null && MateHosList.Count > 0)
                    {
                        List<string> MediStockNames = new List<string>();

                        List<string> MediStockCodes = new List<string>();

                        List<ExpandoObject> MaterialTypeDatas = new List<ExpandoObject>();
                        foreach (var item in MateHosList)
                        {
                            MediStockNames.AddRange(item.MediStockNames);
                            MediStockCodes.AddRange(item.MediStockCodes);
                            MaterialTypeDatas.AddRange(item.MaterialTypeDatas);
                        }

                        var ListMaterialTypeTemp = new List<ExpandoObject>();
                        foreach (var item in MaterialTypeDatas)
                        {
                            foreach (var codeStock in MediStockCodes)
                            {
                                if (!((IDictionary<string, object>)item).ContainsKey(codeStock))
                                {
                                    ((IDictionary<string, object>)item).Add(codeStock, 0m);
                                }
                            }
                            ListMaterialTypeTemp.Add(item);
                        }
                        MaterialTypeDatas = ListMaterialTypeTemp;

                        //Bỏ kho trùng nhưng giữ đúng cặp mã kho - tên kho (2 kho khác nhau vẫn có thể trùng tên)
                        List<string> distinctMateStockCodes = null;
                        List<string> distinctMateStockNames = null;
                        DistinctMediStocks(MediStockCodes, MediStockNames, out distinctMateStockCodes, out distinctMateStockNames);

                        lstMateInStocks = new MaterialTypeInHospitalSDO()
                        {
                            MediStockCodes = distinctMateStockCodes,
                            MediStockNames = distinctMateStockNames,
                            MaterialTypeDatas = MaterialTypeDatas.OrderBy(x => ((IDictionary<string, object>)x)[FIELD_NAME__MATERIAL_TYPE_CODE]).ToList()
                        };
                    }

                    initTreeMaterial(lstMateInStocks);
                    if (lstMateInStocks.MaterialTypeDatas != null && lstMateInStocks.MaterialTypeDatas.Count > 0)
                    {
                        //Cấp 1: nhóm vật tư -> Cấp 2: vật tư -> Cấp 3: từng lô
                        DataTable treeData = BuildTreeTable(
                            lstMateInStocks.MaterialTypeDatas,
                            FIELD_NAME__MATERIAL_TYPE_CODE,
                            FIELD_NAME__MATERIAL_TYPE_NAME,
                            GetMaterialTextFieldNames(),
                            lstMateInStocks.MediStockCodes,
                            lstMateInStocks.MediStockNames,
                            GetMaterialPackageStocks(MediStock.Select(o => o.ID).Distinct().ToList()));
                        BindTree(treeData);
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

        /// <summary>
        /// Các field text của thuốc lấy từ dòng tổng theo loại xuống node thuốc/lô
        /// </summary>
        private List<string> GetMedicineTextFieldNames()
        {
            return new List<string>()
            {
                FIELD_NAME__MEDICINE_TYPE_CODE,
                FIELD_NAME__MEDICINE_TYPE_NAME,
                "ACTIVE_INGR_BHYT_CODE",
                "ACTIVE_INGR_BHYT_NAME",
                "CONCENTRA",
                "REGISTER_NUMBER",
                "SERVICE_UNIT_NAME"
            };
        }

        /// <summary>
        /// Các field text của vật tư lấy từ dòng tổng theo loại xuống node vật tư/lô
        /// </summary>
        private List<string> GetMaterialTextFieldNames()
        {
            return new List<string>()
            {
                FIELD_NAME__MATERIAL_TYPE_CODE,
                FIELD_NAME__MATERIAL_TYPE_NAME,
                "CONCENTRA",
                "SERVICE_UNIT_NAME"
            };
        }

        private TreeListColumn AddTreeColumn(string caption, string fieldName, int width, int visibleIndex, bool isAmount)
        {
            TreeListColumn column = new TreeListColumn();
            try
            {
                column.Caption = caption;
                column.FieldName = fieldName;
                column.Width = width;
                column.VisibleIndex = visibleIndex;
                column.OptionsColumn.AllowEdit = false;
                column.OptionsColumn.AllowSort = true;
                if (isAmount)
                {
                    column.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
                    column.Format.FormatString = "#,##0";
                    column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                }
                trvMediMateStockSum.Columns.Add(column);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return column;
        }

        private void initTreeMedicine(MedicineTypeInHospitalSDO medicineTypeInHospitalSDO)
        {
            try
            {
                //Cột cây: cấp 1 nhóm thuốc -> cấp 2 mã - tên thuốc -> cấp 3 số lô
                AddTreeColumn("Nhóm thuốc / Thuốc / Số lô", FIELD_NAME__NODE_DISPLAY, 320, 0, false);
                AddTreeColumn("Mã thuốc", FIELD_NAME__MEDICINE_TYPE_CODE, 100, 1, false);
                AddTreeColumn("Tên thuốc", FIELD_NAME__MEDICINE_TYPE_NAME, 300, 2, false);
                AddTreeColumn("Mã hoạt chất", "ACTIVE_INGR_BHYT_CODE", 100, 3, false);
                AddTreeColumn("Tên hoạt chất", "ACTIVE_INGR_BHYT_NAME", 150, 4, false);
                AddTreeColumn("Hàm lượng", "CONCENTRA", 100, 5, false);
                AddTreeColumn("Số đăng ký", "REGISTER_NUMBER", 120, 6, false);
                AddTreeColumn("Tổng tồn", FIELD_NAME__TOTAL_AMOUNT, 100, 7, true);
                AddTreeColumn("Đơn vị tính", "SERVICE_UNIT_NAME", 90, 8, false);
                AddTreeColumn("Số lô", FIELD_NAME__PACKAGE_NUMBER, 100, 9, false);
                AddTreeColumn("Hạn dùng", FIELD_NAME__EXPIRED_DATE_DISPLAY, 100, 10, false);

                int index = 11;
                if (medicineTypeInHospitalSDO != null &&
                    medicineTypeInHospitalSDO.MediStockCodes != null &&
                    medicineTypeInHospitalSDO.MediStockNames != null)
                {
                    for (int i = 0; i < medicineTypeInHospitalSDO.MediStockCodes.Count && i < medicineTypeInHospitalSDO.MediStockNames.Count; i++)
                    {
                        AddTreeColumn(medicineTypeInHospitalSDO.MediStockNames[i], medicineTypeInHospitalSDO.MediStockCodes[i], 130, index, true);
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void initTreeMaterial(MaterialTypeInHospitalSDO materialTypeInHospitalSDO)
        {
            try
            {
                //Cột cây: cấp 1 nhóm vật tư -> cấp 2 mã - tên vật tư -> cấp 3 số lô
                AddTreeColumn("Nhóm vật tư / Vật tư / Số lô", FIELD_NAME__NODE_DISPLAY, 320, 0, false);
                AddTreeColumn("Mã vật tư", FIELD_NAME__MATERIAL_TYPE_CODE, 100, 1, false);
                AddTreeColumn("Tên vật tư", FIELD_NAME__MATERIAL_TYPE_NAME, 300, 2, false);
                AddTreeColumn("Hàm lượng", "CONCENTRA", 100, 3, false);
                AddTreeColumn("Tổng tồn", FIELD_NAME__TOTAL_AMOUNT, 100, 4, true);
                AddTreeColumn("Đơn vị tính", "SERVICE_UNIT_NAME", 90, 5, false);
                AddTreeColumn("Số lô", FIELD_NAME__PACKAGE_NUMBER, 100, 6, false);
                AddTreeColumn("Hạn dùng", FIELD_NAME__EXPIRED_DATE_DISPLAY, 100, 7, false);

                int index = 8;
                if (materialTypeInHospitalSDO != null &&
                    materialTypeInHospitalSDO.MediStockCodes != null &&
                    materialTypeInHospitalSDO.MediStockNames != null)
                {
                    for (int i = 0; i < materialTypeInHospitalSDO.MediStockCodes.Count && i < materialTypeInHospitalSDO.MediStockNames.Count; i++)
                    {
                        AddTreeColumn(materialTypeInHospitalSDO.MediStockNames[i], materialTypeInHospitalSDO.MediStockCodes[i], 130, index, true);
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Đổ dữ liệu vào cây rồi bung/thu theo checkbox Chi tiết - Thu gọn
        /// </summary>
        private void BindTree(DataTable treeData)
        {
            try
            {
                trvMediMateStockSum.BeginUpdate();
                trvMediMateStockSum.KeyFieldName = FIELD_NAME__NODE_ID;
                trvMediMateStockSum.ParentFieldName = FIELD_NAME__PARENT_NODE_ID;
                trvMediMateStockSum.DataSource = treeData;
                trvMediMateStockSum.EndUpdate();

                ApplyExpandState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dựng dữ liệu 3 cấp cho cây: nhóm -> loại thuốc/vật tư -> lô.
        /// Số lượng của node cha = tổng các node con (TreeList không tự cộng con lên cha).
        /// Loại không lấy được dữ liệu lô thì chỉ có node cấp 2, số lượng lấy theo dòng tổng cũ.
        /// </summary>
        private DataTable BuildTreeTable(List<ExpandoObject> typeDatas, string typeCodeFieldName, string typeNameFieldName,
            List<string> textFieldNames, List<string> mediStockCodes, List<string> mediStockNames, List<PackageStockADO> packageStocks)
        {
            DataTable treeData = new DataTable("MediMateStockSummary");
            try
            {
                List<string> stockKeys = new List<string>();
                foreach (string stockCode in (mediStockCodes ?? new List<string>()))
                {
                    if (!String.IsNullOrEmpty(stockCode) && !stockKeys.Contains(stockCode))
                        stockKeys.Add(stockCode);
                }

                treeData.Columns.Add(FIELD_NAME__NODE_ID, typeof(long));
                treeData.Columns.Add(FIELD_NAME__PARENT_NODE_ID, typeof(long)).AllowDBNull = true;
                treeData.Columns.Add(FIELD_NAME__NODE_DISPLAY, typeof(string));
                treeData.Columns.Add(FIELD_NAME__PACKAGE_NUMBER, typeof(string));
                treeData.Columns.Add(FIELD_NAME__EXPIRED_DATE_DISPLAY, typeof(string));
                foreach (string textFieldName in (textFieldNames ?? new List<string>()))
                {
                    if (!treeData.Columns.Contains(textFieldName))
                        treeData.Columns.Add(textFieldName, typeof(string));
                }
                DataColumn totalColumn = treeData.Columns.Add(FIELD_NAME__TOTAL_AMOUNT, typeof(decimal));
                totalColumn.DefaultValue = 0m;
                foreach (string stockKey in stockKeys)
                {
                    if (!treeData.Columns.Contains(stockKey))
                        treeData.Columns.Add(stockKey, typeof(decimal)).DefaultValue = 0m;
                }

                if (typeDatas == null || typeDatas.Count == 0)
                    return treeData;

                List<long> lotStockIds = new List<long>();
                foreach (PackageStockADO lot in (packageStocks ?? new List<PackageStockADO>()))
                {
                    if (lot != null && lot.MediStockId.HasValue && !lotStockIds.Contains(lot.MediStockId.Value))
                        lotStockIds.Add(lot.MediStockId.Value);
                }
                Dictionary<long, string> stockKeyByStockId = BuildStockKeyByStockId(stockKeys, mediStockNames, lotStockIds);

                //Gom lô theo mã loại (cùng comparer để không đụng key khi mã chỉ khác hoa/thường)
                Dictionary<string, List<PackageStockADO>> lotsByTypeCode = new Dictionary<string, List<PackageStockADO>>(StringComparer.OrdinalIgnoreCase);
                foreach (PackageStockADO lot in (packageStocks ?? new List<PackageStockADO>()))
                {
                    if (lot == null || String.IsNullOrEmpty(lot.TypeCode))
                        continue;
                    if (!lotsByTypeCode.ContainsKey(lot.TypeCode))
                        lotsByTypeCode.Add(lot.TypeCode, new List<PackageStockADO>());
                    lotsByTypeCode[lot.TypeCode].Add(lot);
                }

                //Log 1 dòng tổng đầu tiên để biết chắc key và kiểu dữ liệu server trả về
                if (typeDatas.Count > 0)
                {
                    System.Text.StringBuilder firstRowDetail = new System.Text.StringBuilder();
                    foreach (var field in (IDictionary<string, object>)typeDatas[0])
                    {
                        firstRowDetail.Append(field.Key + "=" + Convert.ToString(field.Value)
                            + "(" + (field.Value == null ? "null" : field.Value.GetType().Name) + ") | ");
                    }
                    Inventec.Common.Logging.LogSystem.Debug("BuildTreeTable: dong tong dau tien: " + firstRowDetail.ToString());
                }

                long nodeId = 0;
                int unmappedLotCount = 0;
                int typeWithoutLotCount = 0;
                int typeLotDiffCount = 0;
                int packageNodeCount = 0;
                Dictionary<string, DataRow> groupRowByName = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);

                foreach (var typeData in typeDatas)
                {
                    IDictionary<string, object> typeDataFields = (IDictionary<string, object>)typeData;
                    string typeCode = GetFieldString(typeDataFields, typeCodeFieldName);
                    string typeName = GetFieldString(typeDataFields, typeNameFieldName);
                    string groupName = GetFieldString(typeDataFields, FIELD_NAME__PARENT_TYPE_NAME);
                    if (String.IsNullOrEmpty(groupName))
                        groupName = "(Không có nhóm)";

                    //Cấp 1: nhóm
                    DataRow groupRow = null;
                    if (groupRowByName.ContainsKey(groupName))
                    {
                        groupRow = groupRowByName[groupName];
                    }
                    else
                    {
                        nodeId++;
                        groupRow = treeData.NewRow();
                        groupRow[FIELD_NAME__NODE_ID] = nodeId;
                        groupRow[FIELD_NAME__PARENT_NODE_ID] = DBNull.Value;
                        groupRow[FIELD_NAME__NODE_DISPLAY] = groupName;
                        treeData.Rows.Add(groupRow);
                        groupRowByName.Add(groupName, groupRow);
                    }

                    //Cấp 2: loại thuốc/vật tư
                    nodeId++;
                    DataRow typeRow = treeData.NewRow();
                    typeRow[FIELD_NAME__NODE_ID] = nodeId;
                    typeRow[FIELD_NAME__PARENT_NODE_ID] = Convert.ToInt64(groupRow[FIELD_NAME__NODE_ID]);
                    typeRow[FIELD_NAME__NODE_DISPLAY] = (typeCode + " - " + typeName).Trim(' ', '-');
                    foreach (string textFieldName in (textFieldNames ?? new List<string>()))
                        typeRow[textFieldName] = GetFieldString(typeDataFields, textFieldName);
                    treeData.Rows.Add(typeRow);
                    long typeNodeId = nodeId;

                    //Cấp 3: từng lô
                    List<PackageGroupADO> packages = null;
                    if (!String.IsNullOrEmpty(typeCode) && lotsByTypeCode.ContainsKey(typeCode))
                        packages = BuildPackageGroups(lotsByTypeCode[typeCode], stockKeys, stockKeyByStockId, ref unmappedLotCount);

                    //Số của cấp 2 luôn lấy từ dòng tổng của server để khớp đúng bản cũ,
                    //cấp 3 (lô) chỉ là chi tiết bên dưới
                    decimal typeTotalAmount = 0;
                    foreach (string stockKey in stockKeys)
                    {
                        decimal serverAmount = ToDecimal(typeDataFields.ContainsKey(stockKey) ? typeDataFields[stockKey] : null);
                        typeRow[stockKey] = serverAmount;
                        AddAmount(groupRow, stockKey, serverAmount);
                        typeTotalAmount += serverAmount;
                    }
                    typeRow[FIELD_NAME__TOTAL_AMOUNT] = typeTotalAmount;
                    AddAmount(groupRow, FIELD_NAME__TOTAL_AMOUNT, typeTotalAmount);

                    if (packages == null || packages.Count == 0)
                    {
                        typeWithoutLotCount++;
                        continue;
                    }

                    decimal typePackageAmount = 0;
                    foreach (PackageGroupADO package in packages)
                    {
                        nodeId++;
                        DataRow packageRow = treeData.NewRow();
                        packageRow[FIELD_NAME__NODE_ID] = nodeId;
                        packageRow[FIELD_NAME__PARENT_NODE_ID] = typeNodeId;
                        packageRow[FIELD_NAME__NODE_DISPLAY] = String.IsNullOrEmpty(package.PackageNumber) ? "(Không có số lô)" : package.PackageNumber;
                        packageRow[FIELD_NAME__PACKAGE_NUMBER] = package.PackageNumber ?? "";
                        packageRow[FIELD_NAME__EXPIRED_DATE_DISPLAY] = package.ExpiredDate > 0
                            ? Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)package.ExpiredDate)
                            : "";
                        foreach (string textFieldName in (textFieldNames ?? new List<string>()))
                            packageRow[textFieldName] = GetFieldString(typeDataFields, textFieldName);

                        decimal packageTotalAmount = 0;
                        foreach (string stockKey in stockKeys)
                        {
                            decimal amount = package.AmountByStockKey.ContainsKey(stockKey) ? package.AmountByStockKey[stockKey] : 0;
                            packageRow[stockKey] = amount;
                            packageTotalAmount += amount;
                        }
                        packageRow[FIELD_NAME__TOTAL_AMOUNT] = packageTotalAmount;
                        typePackageAmount += packageTotalAmount;
                        treeData.Rows.Add(packageRow);
                        packageNodeCount++;
                    }

                    //Đếm trường hợp tổng các lô lệch so với số của server để soi lại nếu cần
                    if (typePackageAmount != typeTotalAmount)
                    {
                        typeLotDiffCount++;
                        if (typeLotDiffCount <= 5)
                        {
                            decimal typeAvailableAmount = 0;
                            foreach (PackageGroupADO package in packages)
                                typeAvailableAmount += package.AvailableAmount;

                            System.Text.StringBuilder stockDetail = new System.Text.StringBuilder();
                            foreach (string stockKey in stockKeys)
                            {
                                decimal serverAmount = ToDecimal(typeDataFields.ContainsKey(stockKey) ? typeDataFields[stockKey] : null);
                                decimal lotAmount = 0;
                                foreach (PackageGroupADO package in packages)
                                    lotAmount += package.AmountByStockKey.ContainsKey(stockKey) ? package.AmountByStockKey[stockKey] : 0;
                                stockDetail.Append(" | " + stockKey + ": server=" + serverAmount + " lo=" + lotAmount);
                            }

                            Inventec.Common.Logging.LogSystem.Debug("BuildTreeTable: lech tong lo - server. TypeCode=" + typeCode
                                + ", so lo=" + packages.Count
                                + ", tong lo(ton)=" + typePackageAmount + ", tong lo(kha dung)=" + typeAvailableAmount
                                + ", tong server=" + typeTotalAmount + stockDetail.ToString());
                        }
                    }
                }

                if (unmappedLotCount > 0)
                    Inventec.Common.Logging.LogSystem.Warn("BuildTreeTable: co " + unmappedLotCount + " dong lo khong map duoc vao cot kho nao");
                Inventec.Common.Logging.LogSystem.Debug("BuildTreeTable: " + groupRowByName.Count + " nhom, " + typeDatas.Count
                    + " loai (trong do " + typeWithoutLotCount + " loai khong co lo, " + typeLotDiffCount
                    + " loai co tong lo lech so server), " + packageNodeCount + " lo");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return treeData;
        }

        private void AddAmount(DataRow row, string columnName, decimal amount)
        {
            try
            {
                if (row == null || String.IsNullOrEmpty(columnName) || !row.Table.Columns.Contains(columnName))
                    return;

                row[columnName] = ToDecimal(row[columnName]) + amount;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        /// <summary>
        /// Bỏ kho trùng nhưng giữ đúng cặp mã kho - tên kho (2 kho khác nhau vẫn có thể trùng tên)
        /// </summary>
        private void DistinctMediStocks(List<string> mediStockCodes, List<string> mediStockNames, out List<string> distinctCodes, out List<string> distinctNames)
        {
            distinctCodes = new List<string>();
            distinctNames = new List<string>();
            try
            {
                if (mediStockCodes == null || mediStockNames == null)
                    return;

                for (int i = 0; i < mediStockCodes.Count && i < mediStockNames.Count; i++)
                {
                    if (distinctCodes.Contains(mediStockCodes[i]))
                        continue;

                    distinctCodes.Add(mediStockCodes[i]);
                    distinctNames.Add(mediStockNames[i]);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tồn của 1 lô thuốc/vật tư trong 1 kho
        /// </summary>
        private class PackageStockADO
        {
            internal string TypeCode { get; set; }
            internal string PackageNumber { get; set; }
            internal decimal? ExpiredDate { get; set; }
            internal long? MediStockId { get; set; }
            internal string MediStockCode { get; set; }
            internal string MediStockName { get; set; }
            //Amount = số dùng để lên lưới (tổng tồn, khớp caption cột "Tổng tồn"),
            //AvailableAmount giữ lại để đối chiếu khi lệch
            internal decimal Amount { get; set; }
            internal decimal AvailableAmount { get; set; }
        }

        /// <summary>
        /// 1 lô (số lô + hạn dùng) và tồn của lô đó ở từng cột kho
        /// </summary>
        private class PackageGroupADO
        {
            internal string PackageNumber { get; set; }
            internal decimal ExpiredDate { get; set; }
            //Tổng tồn của lô ở tất cả kho đã lấy, kể cả kho chưa map được vào cột
            internal decimal TotalAmount { get; set; }
            internal decimal AvailableAmount { get; set; }
            internal Dictionary<string, decimal> AmountByStockKey { get; set; }
        }

        /// <summary>
        /// Lấy tồn theo từng lô thuốc trong các kho (nguồn HIS_MEDICINE, dùng chung API với cây tồn kho)
        /// </summary>
        private List<PackageStockADO> GetMedicinePackageStocks(List<long> mediStockIds)
        {
            List<PackageStockADO> result = new List<PackageStockADO>();
            try
            {
                if (mediStockIds == null || mediStockIds.Count == 0)
                    return result;

                CommonParam param = new CommonParam();
                //Gọi theo từng kho: SDO trả về không điền đủ MEDI_STOCK_ID nên phải tự biết lô thuộc kho nào
                foreach (long mediStockId in mediStockIds)
                {
                    MOS.Filter.HisMedicineStockViewFilter filter = new MOS.Filter.HisMedicineStockViewFilter();
                    filter.MEDI_STOCK_ID = mediStockId;
                    filter.MEDI_STOCK_IDs = new List<long>() { mediStockId };
                    var lots = new BackendAdapter(param).Get<List<HisMedicineInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_GETVIEW_IN_STOCK_MEDICINE_TYPE_TREE, ApiConsumers.MosConsumer, filter, param);
                    if (lots != null)
                    {
                        //API trả kèm node loại thuốc để dựng cây -> chỉ lấy dòng chi tiết theo lô
                        result.AddRange(lots.Where(o => !o.isTypeNode && o.ID > 0).Select(o => new PackageStockADO()
                        {
                            TypeCode = o.MEDICINE_TYPE_CODE,
                            PackageNumber = o.PACKAGE_NUMBER,
                            ExpiredDate = o.EXPIRED_DATE,
                            MediStockId = o.MEDI_STOCK_ID ?? mediStockId,
                            MediStockCode = o.MEDI_STOCK_CODE,
                            MediStockName = o.MEDI_STOCK_NAME,
                            Amount = o.TotalAmount ?? o.AvailableAmount ?? 0,
                            AvailableAmount = o.AvailableAmount ?? 0
                        }));
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("GetMedicinePackageStocks: " + mediStockIds.Count + " kho, so dong lo = " + result.Count
                    + ", tong ton = " + result.Sum(o => o.Amount) + ", tong kha dung = " + result.Sum(o => o.AvailableAmount));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Lấy tồn theo từng lô vật tư trong các kho (nguồn HIS_MATERIAL, dùng chung API với cây tồn kho)
        /// </summary>
        private List<PackageStockADO> GetMaterialPackageStocks(List<long> mediStockIds)
        {
            List<PackageStockADO> result = new List<PackageStockADO>();
            try
            {
                if (mediStockIds == null || mediStockIds.Count == 0)
                    return result;

                CommonParam param = new CommonParam();
                //Gọi theo từng kho: SDO trả về không điền đủ MEDI_STOCK_ID nên phải tự biết lô thuộc kho nào
                foreach (long mediStockId in mediStockIds)
                {
                    MOS.Filter.HisMaterialStockViewFilter filter = new MOS.Filter.HisMaterialStockViewFilter();
                    filter.MEDI_STOCK_ID = mediStockId;
                    filter.MEDI_STOCK_IDs = new List<long>() { mediStockId };
                    var lots = new BackendAdapter(param).Get<List<HisMaterialInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_GETVIEW_IN_STOCK_MATERIAL_TYPE_TREE, ApiConsumers.MosConsumer, filter, param);
                    if (lots != null)
                    {
                        //API trả kèm node loại vật tư để dựng cây -> chỉ lấy dòng chi tiết theo lô
                        result.AddRange(lots.Where(o => !o.isTypeNode && o.ID > 0).Select(o => new PackageStockADO()
                        {
                            TypeCode = o.MATERIAL_TYPE_CODE,
                            PackageNumber = o.PACKAGE_NUMBER,
                            ExpiredDate = o.EXPIRED_DATE,
                            MediStockId = o.MEDI_STOCK_ID ?? mediStockId,
                            MediStockCode = o.MEDI_STOCK_CODE,
                            MediStockName = o.MEDI_STOCK_NAME,
                            Amount = o.TotalAmount ?? o.AvailableAmount ?? 0,
                            AvailableAmount = o.AvailableAmount ?? 0
                        }));
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("GetMaterialPackageStocks: " + mediStockIds.Count + " kho, so dong lo = " + result.Count
                    + ", tong ton = " + result.Sum(o => o.Amount) + ", tong kha dung = " + result.Sum(o => o.AvailableAmount));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Map ID kho -> tên cột kho trên lưới, bắt buộc 1 kho ứng 1 cột và 1 cột ứng 1 kho
        /// (nhiều kho khác chi nhánh có thể trùng tên nên không được map nhiều kho vào 1 cột).
        /// Chỉ xét đúng các kho đang lấy dữ liệu, thử lần lượt: số cuối tên cột = ID kho, rồi tên kho, rồi mã kho.
        /// </summary>
        private Dictionary<long, string> BuildStockKeyByStockId(List<string> stockKeys, List<string> stockNames, List<long> mediStockIds)
        {
            Dictionary<long, string> stockKeyByStockId = new Dictionary<long, string>();
            try
            {
                if (stockKeys == null || stockKeys.Count == 0 || mediStockIds == null || mediStockIds.Count == 0)
                    return stockKeyByStockId;

                List<HIS_MEDI_STOCK> mediStocks = (BackendDataWorker.Get<HIS_MEDI_STOCK>() ?? new List<HIS_MEDI_STOCK>())
                    .Where(o => mediStockIds.Contains(o.ID)).ToList();
                List<string> usedStockKeys = new List<string>();

                //Vòng 1: số cuối tên cột chính là ID kho
                for (int i = 0; i < stockKeys.Count; i++)
                {
                    long stockIdInKey = 0;
                    int separatorIndex = stockKeys[i].LastIndexOf('_');
                    string idText = separatorIndex >= 0 ? stockKeys[i].Substring(separatorIndex + 1) : stockKeys[i];
                    if (!long.TryParse(idText, out stockIdInKey))
                        continue;

                    if (mediStockIds.Contains(stockIdInKey) && !stockKeyByStockId.ContainsKey(stockIdInKey))
                    {
                        stockKeyByStockId.Add(stockIdInKey, stockKeys[i]);
                        usedStockKeys.Add(stockKeys[i]);
                    }
                }

                //Vòng 2 + 3: cột chưa gán thì khớp theo tên kho, sau đó theo mã kho
                for (int i = 0; i < stockKeys.Count; i++)
                {
                    if (usedStockKeys.Contains(stockKeys[i]))
                        continue;

                    string stockName = (stockNames != null && i < stockNames.Count) ? (stockNames[i] ?? "").Trim() : "";
                    HIS_MEDI_STOCK matchedStock = mediStocks.FirstOrDefault(o => !stockKeyByStockId.ContainsKey(o.ID)
                        && !String.IsNullOrEmpty(stockName) && !String.IsNullOrEmpty(o.MEDI_STOCK_NAME)
                        && String.Equals(o.MEDI_STOCK_NAME.Trim(), stockName, StringComparison.OrdinalIgnoreCase));
                    if (matchedStock == null)
                    {
                        matchedStock = mediStocks.FirstOrDefault(o => !stockKeyByStockId.ContainsKey(o.ID)
                            && !String.IsNullOrEmpty(o.MEDI_STOCK_CODE)
                            && String.Equals(o.MEDI_STOCK_CODE.Trim(), stockKeys[i].Trim(), StringComparison.OrdinalIgnoreCase));
                    }
                    if (matchedStock != null)
                    {
                        stockKeyByStockId.Add(matchedStock.ID, stockKeys[i]);
                        usedStockKeys.Add(stockKeys[i]);
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("BuildStockKeyByStockId: " + stockKeys.Count + " cot kho, "
                    + mediStockIds.Count + " kho lay du lieu -> map duoc " + stockKeyByStockId.Count + " cap kho-cot");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return stockKeyByStockId;
        }

        /// <summary>
        /// Gom các dòng lô của 1 loại theo (số lô + hạn dùng), cộng tồn vào đúng cột kho
        /// </summary>
        private List<PackageGroupADO> BuildPackageGroups(List<PackageStockADO> lots, List<string> stockKeys, Dictionary<long, string> stockKeyByStockId, ref int unmappedLotCount)
        {
            List<PackageGroupADO> result = new List<PackageGroupADO>();
            try
            {
                if (lots == null || lots.Count == 0)
                    return result;

                Dictionary<string, PackageGroupADO> packageByKey = new Dictionary<string, PackageGroupADO>(StringComparer.OrdinalIgnoreCase);
                foreach (PackageStockADO lot in lots)
                {
                    string packageNumber = lot.PackageNumber ?? "";
                    decimal expiredDate = lot.ExpiredDate ?? 0;
                    string packageKey = packageNumber + "|" + expiredDate.ToString();

                    PackageGroupADO package = null;
                    if (packageByKey.ContainsKey(packageKey))
                    {
                        package = packageByKey[packageKey];
                    }
                    else
                    {
                        package = new PackageGroupADO()
                        {
                            PackageNumber = packageNumber,
                            ExpiredDate = expiredDate,
                            AmountByStockKey = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
                        };
                        packageByKey.Add(packageKey, package);
                        result.Add(package);
                    }

                    package.TotalAmount = package.TotalAmount + lot.Amount;
                    package.AvailableAmount = package.AvailableAmount + lot.AvailableAmount;

                    string stockKey = ResolveStockKey(lot, stockKeys, stockKeyByStockId);
                    if (String.IsNullOrEmpty(stockKey))
                    {
                        unmappedLotCount++;
                        if (unmappedLotCount <= 3)
                            Inventec.Common.Logging.LogSystem.Warn("Lo khong map duoc cot kho: TypeCode=" + lot.TypeCode
                                + ", MediStockId=" + lot.MediStockId + ", MediStockCode=" + lot.MediStockCode
                                + ", MediStockName=" + lot.MediStockName + ", cac cot kho hien co=" + String.Join(",", (stockKeys ?? new List<string>()).ToArray()));
                        continue;
                    }

                    if (!package.AmountByStockKey.ContainsKey(stockKey))
                        package.AmountByStockKey.Add(stockKey, 0);
                    package.AmountByStockKey[stockKey] = package.AmountByStockKey[stockKey] + lot.Amount;
                }

                result = result.OrderBy(o => o.ExpiredDate).ThenBy(o => o.PackageNumber).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tìm tên cột kho ứng với 1 dòng lô: ưu tiên theo ID kho (AMOUNT_&lt;ID&gt;), không được thì thử theo mã kho
        /// </summary>
        private string ResolveStockKey(PackageStockADO lot, List<string> stockKeys, Dictionary<long, string> stockKeyByStockId)
        {
            if (lot == null)
                return "";

            if (lot.MediStockId.HasValue && stockKeyByStockId != null && stockKeyByStockId.ContainsKey(lot.MediStockId.Value))
                return stockKeyByStockId[lot.MediStockId.Value];

            if (!String.IsNullOrEmpty(lot.MediStockCode) && stockKeys != null)
            {
                foreach (string stockKey in stockKeys)
                {
                    if (String.Equals(stockKey, lot.MediStockCode, StringComparison.OrdinalIgnoreCase))
                        return stockKey;
                }
            }
            return "";
        }

        private string GetFieldString(IDictionary<string, object> dataFields, string fieldName)
        {
            if (dataFields == null || String.IsNullOrEmpty(fieldName))
                return "";
            if (!dataFields.ContainsKey(fieldName) || dataFields[fieldName] == null)
                return "";
            return dataFields[fieldName].ToString();
        }

        private decimal ToDecimal(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                    return 0;
                return Convert.ToDecimal(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void chkChiTiet_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkChiTiet.Checked)
                {
                    chkThuGon.Checked = false;
                    ApplyExpandState();
                }
                else
                {
                    //Luôn phải tích 1 trong 2
                    if (chkThuGon.Checked == false)
                        chkChiTiet.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkThuGon_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkThuGon.Checked)
                {
                    chkChiTiet.Checked = false;
                    ApplyExpandState();
                }
                else
                {
                    //Luôn phải tích 1 trong 2
                    if (chkChiTiet.Checked == false)
                        chkThuGon.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Chi tiết: bung hết 3 cấp. Thu gọn: chỉ hiện cấp 1 + cấp 2, thu cấp lô lại
        /// (chỉ thu chứ không bỏ dòng, bấm mũi tên là xem được).
        /// </summary>
        private void ApplyExpandState()
        {
            try
            {
                if (trvMediMateStockSum.Nodes.Count == 0)
                    return;

                trvMediMateStockSum.BeginUpdate();
                if (chkThuGon.Checked)
                {
                    trvMediMateStockSum.CollapseAll();
                    //Mở cấp nhóm để vẫn thấy cấp thuốc/vật tư, cấp lô thu lại
                    foreach (TreeListNode node in trvMediMateStockSum.Nodes)
                        node.Expanded = true;
                }
                else
                {
                    trvMediMateStockSum.ExpandAll();
                }
                trvMediMateStockSum.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkMedicine_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkMedicine.Checked)
                {
                    chkMaterial.Checked = false;
                    txtKeyword.Text = "";
                    ShowGridControl(loadFirst);
                }
                else
                {
                    if (chkMaterial.Checked == false)
                        chkMedicine.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkMaterial_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkMaterial.Checked)
                {
                    chkMedicine.Checked = false;
                    txtKeyword.Text = "";
                    ShowGridControl(loadFirst);
                }
                else
                {
                    if (chkMedicine.Checked == false)
                        chkMaterial.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvMediMateStockSum_ColumnWidthChanged(object sender, DevExpress.XtraTreeList.ColumnChangedEventArgs e)
        {
            SaveTreeLayout();
        }

        private void trvMediMateStockSum_ColumnPositionChanged(object sender, EventArgs e)
        {
            SaveTreeLayout();
        }

        private void SaveTreeLayout()
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign")))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"));
                }
                if (chkMedicine.Checked)
                    trvMediMateStockSum.SaveLayoutToXml(this.fileNameMedicine);
                else if (chkMaterial.Checked)
                    trvMediMateStockSum.SaveLayoutToXml(this.fileNameMaterial);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    this.trvMediMateStockSum.Focus();
                    if (this.trvMediMateStockSum.Nodes.Count > 0)
                        this.trvMediMateStockSum.FocusedNode = this.trvMediMateStockSum.Nodes[0];
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void trvMediMateStockSum_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                if (e.Node == null || e.Column == null)
                    return;

                //Node cha (nhóm, thuốc/vật tư) in đậm để thấy rõ 3 cấp
                if (e.Node.Level < 2)
                    e.Appearance.FontStyleDelta = FontStyle.Bold;

                if (e.Column.FieldName == FIELD_NAME__TOTAL_AMOUNT)
                {
                    e.Appearance.ForeColor = Color.Red;
                }

                List<string> stockCodes = chkMedicine.Checked ? lstMediInStocks.MediStockCodes : lstMateInStocks.MediStockCodes;
                List<string> stockNames = chkMedicine.Checked ? lstMediInStocks.MediStockNames : lstMateInStocks.MediStockNames;
                if (stockCodes == null || stockNames == null)
                    return;

                for (int i = 0; i < stockCodes.Count && i < stockNames.Count; i++)
                {
                    if (String.IsNullOrEmpty(stockNames[i]))
                        continue;

                    if (Inventec.Common.String.Convert.UnSignVNese(stockNames[i].Trim().ToLower()).Equals(tenkhoHientai)
                        && e.Column.FieldName == stockCodes[i])
                    {
                        e.Appearance.ForeColor = Color.RoyalBlue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtKeyword_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtKeyword.Text))
                {
                    //FilterMode.Smart giữ lại node cha của dòng khớp -> vẫn thấy nhóm/thuốc của lô tìm được
                    if (chkMedicine.Checked)
                    {
                        trvMediMateStockSum.ActiveFilterString = String.Format("[MEDICINE_TYPE_CODE] Like '%{0}%' OR [MEDICINE_TYPE_NAME] Like '%{0}%' OR [ACTIVE_INGR_BHYT_CODE] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME] Like '%{0}%' OR [CONCENTRA] Like '%{0}%' OR [PACKAGE_NUMBER] Like '%{0}%' OR [NODE_DISPLAY] Like '%{0}%'", txtKeyword.Text);
                    }
                    else
                        trvMediMateStockSum.ActiveFilterString = String.Format("[MATERIAL_TYPE_CODE] Like '%{0}%' OR [MATERIAL_TYPE_NAME] Like '%{0}%' OR [CONCENTRA] Like '%{0}%' OR [PACKAGE_NUMBER] Like '%{0}%' OR [NODE_DISPLAY] Like '%{0}%'", txtKeyword.Text);

                    trvMediMateStockSum.OptionsView.ShowFilterPanelMode = DevExpress.XtraTreeList.ShowFilterPanelMode.Never;
                    trvMediMateStockSum.ExpandAll();

                    txtKeyword.Focus();
                }
                else
                {
                    trvMediMateStockSum.ActiveFilterString = "";
                    ApplyExpandState();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.DefaultExt = "xlsx";
                saveFile.Filter = "Excel file|*.xlsx|All file|*.*";

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    bool success = false;
                    trvMediMateStockSum.ExportToXlsx(saveFile.FileName);

                    ShowGridControl(loadFirst);
                    WaitingManager.Hide();

                    if (saveFile.FileName != "")
                        success = true;
                    MessageManager.Show(success == true ? "Xử lý thành công" : "Xử lý thất bại");
                    if (!success)
                        return;
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFile.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboBranh()
        {
            try
            {
                var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var userRoomByUsers = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == loginName && (o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)).ToList();

                cboBrach.Properties.DataSource = userRoomByUsers;
                cboBrach.Properties.DisplayMember = "BRANCH_NAME";
                cboBrach.Properties.ValueMember = "BRANCH_ID";

                cboBrach.Properties.View.Columns.Clear();

                DevExpress.XtraGrid.Columns.GridColumn branchIdColumn = cboBrach.Properties.View.Columns.AddField("BRANCH_ID");
                branchIdColumn.VisibleIndex = 0;
                branchIdColumn.Width = 50;
                branchIdColumn.Visible = false;
                branchIdColumn.OptionsColumn.ShowInCustomizationForm = false;

                DevExpress.XtraGrid.Columns.GridColumn column = cboBrach.Properties.View.Columns.AddField("BRANCH_NAME");
                column.VisibleIndex = 1;
                column.Width = 200;
                column.Caption = "Tất cả";
                cboBrach.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboBrach.Properties.View.OptionsSelection.MultiSelect = true;

                cboBrach.Properties.View.RefreshData();
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
                cbo.Properties.View.SelectionChanged += (s, e) =>
                {
                    // Sử dụng BeginInvoke để đảm bảo chạy sau khi selection đã được cập nhật
                    cbo.BeginInvoke(new MethodInvoker(() =>
                    {
                        eventSelect(gridCheck, EventArgs.Empty);
                        cbo.RefreshEditValue();
                    }));
                };

                // Event khi đóng popup
                cbo.CloseUp += (s, e) =>
                {
                    eventSelect(gridCheck, EventArgs.Empty);
                    cbo.RefreshEditValue();
                };
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

        private void SelectionGrid__Status(object sender, EventArgs e)
        {
            try
            {
                cboBrach.RefreshEditValue();
                lstV_UserRoomForBranch = new List<V_HIS_USER_ROOM>();
                foreach (V_HIS_USER_ROOM rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        lstV_UserRoomForBranch.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
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
        private void InitComboBranhCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboBrach.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check);
                cboBrach.Properties.Tag = gridCheck;
                cboBrach.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboBrach.Properties.Tag as GridCheckMarksSelection;

                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboBrach.Properties.View);
                    cboBrach.EditValue = null;
                    cboBrach.Focus();

                    var currentBranchId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId();
                    if (currentBranchId != null)
                    {
                        var dataSource = cboBrach.Properties.DataSource as List<V_HIS_USER_ROOM>;
                        if (dataSource != null)
                        {
                            var matchingBranch = dataSource.FirstOrDefault(x => x.BRANCH_ID == currentBranchId);
                            if (matchingBranch != null)
                            {
                                lstV_UserRoom = new List<V_HIS_USER_ROOM>();
                                lstV_UserRoom.Add(matchingBranch);
                                gridCheckMark.SelectAll(lstV_UserRoom);
                                cboBrach.EditValue = matchingBranch.BRANCH_NAME;
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
        private void Event_Check(object sender, EventArgs e)
        {

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                lstV_UserRoom = new List<V_HIS_USER_ROOM>();
                if (gridCheckMark != null)
                {
                    List<V_HIS_USER_ROOM> erSelectedNews = new List<V_HIS_USER_ROOM>();
                    foreach (V_HIS_USER_ROOM er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.BRANCH_NAME);
                            erSelectedNews.Add(er);
                        }
                    }
                    this.lstV_UserRoom = new List<V_HIS_USER_ROOM>();
                    this.lstV_UserRoom.AddRange(erSelectedNews);
                }
                this.cboBrach.EditValue = sb.ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnSerach_Click(object sender, EventArgs e)
        {
            try
            {
                ShowGridControl(loadFirst);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboBrach_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = cboBrach.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboBrach.Properties.View);
                    }
                    this.cboBrach.EditValue = null;
                    this.cboBrach.Text = "";
                    lstV_UserRoom = new List<V_HIS_USER_ROOM>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboBrach_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo == null) return;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;

                var selectedItems = gridCheckMark.Selection.OfType<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>().ToList();

                if (selectedItems.Count > 0)
                {
                    string statusName = string.Join(", ", selectedItems.Select(x => x.BRANCH_NAME));
                    e.DisplayText = statusName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

