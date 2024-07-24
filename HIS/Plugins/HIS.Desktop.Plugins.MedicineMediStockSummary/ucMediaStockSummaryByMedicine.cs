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
using AutoMapper;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.HisMaterialInStock.ADO;
using HIS.UC.HisMedicineInStock.ADO;
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
        long RoomId;
        long RoomTypeId;
        string tenkhoHientai;
        List<long> roomIds = new List<long>();
        bool loadFirst = true;

        string fileNameMedicine = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", "HIS.Desktop.Plugins.MedicineMediStockSummary.gridViewMediMateStockSum1.xml"));
        string fileNameMaterial = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", "HIS.Desktop.Plugins.MedicineMediStockSummary.gridViewMediMateStockSum2.xml"));

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
                var rooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                var userRoomByUsers = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == loginName && (o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)).ToList();
                if (userRoomByUsers != null)
                    roomIds = userRoomByUsers.Select(o => o.ROOM_ID).Distinct().ToList();

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
                gridControlMediMateStockSum.DataSource = null;
                gridViewMediMateStockSum.Columns.Clear();
                string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtKeyword.Text.ToLower().Trim());

                List<HIS_MEDI_STOCK> MediStock = new List<HIS_MEDI_STOCK>();
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                if (chkMedicine.Checked)
                {
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
                                    ((IDictionary<string, object>)item).Add(codeStock, 0);
                                }
                            }
                            ListMedicineTypeTemp.Add(item);
                        }
                        MedicineTypeDatas = ListMedicineTypeTemp;

                        lstMediInStocks = new MedicineTypeInHospitalSDO()
                        {
                            MediStockCodes = MediStockCodes.Distinct().ToList(),
                            MediStockNames = MediStockNames.Distinct().ToList(),
                            MedicineTypeDatas = MedicineTypeDatas
                        };
                    }
                    initGridMedicine(lstMediInStocks);

                    if (lstMediInStocks.MedicineTypeDatas != null && lstMediInStocks.MedicineTypeDatas.Count > 0)
                    {
                        lstMediInStocks.MedicineTypeDatas = lstMediInStocks.MedicineTypeDatas.OrderBy(x => ((IDictionary<string, object>)x)["MEDICINE_TYPE_CODE"]).ToList();
                        gridControlMediMateStockSum.DataSource = lstMediInStocks.MedicineTypeDatas;
                        gridControlMediMateStockSum.RefreshDataSource();
                        gridViewMediMateStockSum.Columns["PARENT_TYPE_NAME"].Group();
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
                                    ((IDictionary<string, object>)item).Add(codeStock, 0);
                                }
                            }
                            ListMaterialTypeTemp.Add(item);
                        }
                        MaterialTypeDatas = ListMaterialTypeTemp;

                        lstMateInStocks = new MaterialTypeInHospitalSDO()
                        {
                            MediStockCodes = MediStockCodes,
                            MediStockNames = MediStockNames,
                            MaterialTypeDatas = MaterialTypeDatas
                        };
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstMateInStocks), lstMateInStocks));
                    initGridMaterial(lstMateInStocks);
                    if (lstMateInStocks.MaterialTypeDatas != null && lstMateInStocks.MaterialTypeDatas.Count > 0)
                    {
                        lstMateInStocks.MaterialTypeDatas = lstMateInStocks.MaterialTypeDatas.OrderBy(x => ((IDictionary<string, object>)x)["MATERIAL_TYPE_CODE"]).ToList();
                        gridControlMediMateStockSum.DataSource = lstMateInStocks.MaterialTypeDatas;
                        gridControlMediMateStockSum.RefreshDataSource();
                        gridViewMediMateStockSum.Columns["PARENT_TYPE_NAME"].Group();
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

        private void initGridMedicine(MedicineTypeInHospitalSDO medicineTypeInHospitalSDO)
        {
            try
            {
                // thêm cột group theo tên cha
                GridColumn colParentName = new GridColumn();
                colParentName.Caption = "Nhóm thuốc";
                colParentName.FieldName = "PARENT_TYPE_NAME";
                colParentName.Width = 150;
                colParentName.VisibleIndex = 0;
                //colParentName.GroupIndex = 0;
                //colParentName.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
                colParentName.OptionsColumn.AllowEdit = false;
                gridViewMediMateStockSum.Columns.Add(colParentName);

                GridColumn colMathuoc = new GridColumn();
                colMathuoc.Caption = "Mã thuốc";
                colMathuoc.FieldName = "MEDICINE_TYPE_CODE";
                colMathuoc.Width = 150;
                colMathuoc.VisibleIndex = 1;
                colMathuoc.OptionsColumn.AllowEdit = false;
                gridViewMediMateStockSum.Columns.Add(colMathuoc);

                GridColumn colTenthuoc = new GridColumn();
                colTenthuoc.Caption = "Tên thuốc";
                colTenthuoc.FieldName = "MEDICINE_TYPE_NAME";
                colTenthuoc.Width = 350;
                colTenthuoc.VisibleIndex = 2;
                colTenthuoc.OptionsColumn.AllowEdit = false;
                gridViewMediMateStockSum.Columns.Add(colTenthuoc);

                GridColumn colMaHoatchat = new GridColumn();
                colMaHoatchat.Caption = "Mã hoạt chất";
                colMaHoatchat.FieldName = "ACTIVE_INGR_BHYT_CODE";
                colMaHoatchat.Width = 100;
                colMaHoatchat.VisibleIndex = 3;
                colMaHoatchat.OptionsColumn.AllowEdit = false;
                gridViewMediMateStockSum.Columns.Add(colMaHoatchat);

                GridColumn colTenHoatchat = new GridColumn();
                colTenHoatchat.Caption = "Tên hoạt chất";
                colTenHoatchat.FieldName = "ACTIVE_INGR_BHYT_NAME";
                colTenHoatchat.Width = 100;
                colTenHoatchat.VisibleIndex = 4;
                colTenHoatchat.OptionsColumn.AllowEdit = false;
                gridViewMediMateStockSum.Columns.Add(colTenHoatchat);

                GridColumn colHamluong = new GridColumn();
                colHamluong.Caption = "Hàm lượng";
                colHamluong.FieldName = "CONCENTRA";
                colHamluong.Width = 100;
                colHamluong.VisibleIndex = 5;
                colHamluong.OptionsColumn.AllowEdit = false;
                gridViewMediMateStockSum.Columns.Add(colHamluong);

                GridColumn colSoDK = new GridColumn();
                colSoDK.Caption = "Số đăng ký";
                colSoDK.FieldName = "REGISTER_NUMBER";
                colSoDK.Width = 120;
                colSoDK.VisibleIndex = 6;
                colSoDK.OptionsColumn.AllowEdit = false;
                gridViewMediMateStockSum.Columns.Add(colSoDK);

                GridColumn colTotal = new GridColumn();
                colTotal.Caption = "Tổng tồn";
                colTotal.FieldName = "TOTAL_AMOUNT";
                colTotal.Width = 150;
                colTotal.VisibleIndex = 7;
                //colTotal.AppearanceCell.ForeColor = System.Drawing.Color.Red;
                //colTotal.AppearanceCell.Options.UseForeColor = true;
                colTotal.OptionsColumn.AllowEdit = false;
                colTotal.DisplayFormat.FormatString = "#,##0";
                colTotal.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                colTotal.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMateStockSum.Columns.Add(colTotal);

                //Column đơn vị tính
                GridColumn colUnit = new GridColumn();
                colUnit.Caption = "Đơn vị tính";
                colUnit.FieldName = "SERVICE_UNIT_NAME";
                colUnit.Width = 100;
                colUnit.VisibleIndex = 8;
                colUnit.OptionsColumn.AllowEdit = false;
                //colUnit.DisplayFormat.FormatString = "#,##0";
                colUnit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                colUnit.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMateStockSum.Columns.Add(colUnit);

                int index = 9;
                for (int i = 0; i < medicineTypeInHospitalSDO.MediStockCodes.Count; i++)
                {
                    Inventec.Common.Logging.LogSystem.Info(medicineTypeInHospitalSDO.MediStockNames[i] + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeInHospitalSDO.MediStockNames[i]), medicineTypeInHospitalSDO.MediStockNames[i]));
                    Inventec.Common.Logging.LogSystem.Info(medicineTypeInHospitalSDO.MediStockCodes[i] + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeInHospitalSDO.MediStockCodes[i]), medicineTypeInHospitalSDO.MediStockCodes[i]));
                    GridColumn colKho = new GridColumn();
                    colKho.Caption = medicineTypeInHospitalSDO.MediStockNames[i];
                    colKho.FieldName = medicineTypeInHospitalSDO.MediStockCodes[i];
                    colKho.VisibleIndex = index;
                    colKho.Width = 150;
                    if (Inventec.Common.String.Convert.UnSignVNese(medicineTypeInHospitalSDO.MediStockNames[i].Trim().ToLower()).Equals(tenkhoHientai))
                    {
                        colTotal.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                    }
                    colKho.OptionsColumn.AllowEdit = false;
                    colKho.DisplayFormat.FormatString = "#,##0";
                    colKho.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    gridViewMediMateStockSum.Columns.Add(colKho);
                    index++;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void initGridMaterial(MaterialTypeInHospitalSDO materialTypeInHospitalSDO)
        {
            try
            {
                // thêm cột group theo tên cha
                GridColumn colParentName = new GridColumn();
                colParentName.Caption = "Nhóm vật tư";
                colParentName.FieldName = "PARENT_TYPE_NAME";
                colParentName.Width = 150;
                colParentName.VisibleIndex = 0;
                //colParentName.GroupIndex = 0;
                //colParentName.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
                colParentName.OptionsColumn.AllowEdit = false;
                gridViewMediMateStockSum.Columns.Add(colParentName);

                GridColumn colMavattu = new GridColumn();
                colMavattu.Caption = "Mã vật tư";
                colMavattu.FieldName = "MATERIAL_TYPE_CODE";
                colMavattu.Width = 150;
                colMavattu.VisibleIndex = 1;
                colMavattu.OptionsColumn.AllowEdit = false;
                gridViewMediMateStockSum.Columns.Add(colMavattu);

                GridColumn colTenvattu = new GridColumn();
                colTenvattu.Caption = "Tên vật tư";
                colTenvattu.FieldName = "MATERIAL_TYPE_NAME";
                colTenvattu.Width = 350;
                colTenvattu.VisibleIndex = 2;
                colTenvattu.OptionsColumn.AllowEdit = false;
                gridViewMediMateStockSum.Columns.Add(colTenvattu);

                GridColumn colHamluong = new GridColumn();
                colHamluong.Caption = "Hàm lượng";
                colHamluong.FieldName = "CONCENTRA";
                colHamluong.Width = 100;
                colHamluong.VisibleIndex = 3;
                colHamluong.OptionsColumn.AllowEdit = false;
                gridViewMediMateStockSum.Columns.Add(colHamluong);

                GridColumn colTotal = new GridColumn();
                colTotal.Caption = "Tổng tồn";
                colTotal.FieldName = "TOTAL_AMOUNT";
                colTotal.Width = 150;
                colTotal.VisibleIndex = 4;
                colTotal.DisplayFormat.FormatString = "#,##0";
                colTotal.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                colTotal.OptionsColumn.AllowEdit = false;
                //colTotal.AppearanceCell.ForeColor = System.Drawing.Color.Red;
                //colTotal.AppearanceCell.Options.UseForeColor = true;
                colTotal.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMateStockSum.Columns.Add(colTotal);

                //Column đơn vị tính
                GridColumn colUnit = new GridColumn();
                colUnit.Caption = "Đơn vị tính";
                colUnit.FieldName = "SERVICE_UNIT_NAME";
                colUnit.Width = 100;
                colUnit.VisibleIndex = 5;
                colUnit.OptionsColumn.AllowEdit = false;
                //colUnit.DisplayFormat.FormatString = "#,##0";
                colUnit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                colUnit.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMateStockSum.Columns.Add(colUnit);

                int index = 6;
                for (int i = 0; i < materialTypeInHospitalSDO.MediStockCodes.Count; i++)
                {
                    GridColumn colKho = new GridColumn();
                    colKho.Caption = materialTypeInHospitalSDO.MediStockNames[i];
                    colKho.FieldName = materialTypeInHospitalSDO.MediStockCodes[i];
                    colKho.VisibleIndex = index;
                    colKho.Width = 150;
                    if (Inventec.Common.String.Convert.UnSignVNese(materialTypeInHospitalSDO.MediStockNames[i].Trim().ToLower()).Equals(tenkhoHientai))
                    {
                        colTotal.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                    }
                    colKho.OptionsColumn.AllowEdit = false;
                    colKho.DisplayFormat.FormatString = "#,##0";
                    colKho.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    gridViewMediMateStockSum.Columns.Add(colKho);
                    index++;
                }
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

        private void gridViewMediMateStockSum_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign")))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"));
                }
                if (chkMedicine.Checked)
                    gridViewMediMateStockSum.SaveLayoutToXml(this.fileNameMedicine);
                else if (chkMaterial.Checked)
                    gridViewMediMateStockSum.SaveLayoutToXml(this.fileNameMaterial);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMateStockSum_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign")))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"));
                }
                if (chkMedicine.Checked)
                    gridViewMediMateStockSum.SaveLayoutToXml(this.fileNameMedicine);
                else if (chkMaterial.Checked)
                    gridViewMediMateStockSum.SaveLayoutToXml(this.fileNameMaterial);
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
                if (e.KeyCode == Keys.Enter)
                {
                    this.gridViewMediMateStockSum.FocusedRowHandle = 0;
                    this.gridViewMediMateStockSum.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewMediMateStockSum.FocusedRowHandle = 0;
                    this.gridViewMediMateStockSum.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewMediMateStockSum_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "TOTAL_AMOUNT")
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                    if (chkMedicine.Checked)
                    {
                        for (int i = 0; i < lstMediInStocks.MediStockCodes.Count; i++)
                        {
                            if (Inventec.Common.String.Convert.UnSignVNese(lstMediInStocks.MediStockNames[i].Trim().ToLower()).Equals(tenkhoHientai))
                            {
                                if (e.Column.FieldName == lstMediInStocks.MediStockCodes[i])
                                    e.Appearance.ForeColor = Color.RoyalBlue;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < lstMateInStocks.MediStockCodes.Count; i++)
                        {
                            if (Inventec.Common.String.Convert.UnSignVNese(lstMateInStocks.MediStockNames[i].Trim().ToLower()).Equals(tenkhoHientai))
                            {
                                if (e.Column.FieldName == lstMateInStocks.MediStockCodes[i])
                                    e.Appearance.ForeColor = Color.RoyalBlue;
                            }
                        }
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
                    if (chkMedicine.Checked)
                    {
                        gridViewMediMateStockSum.ActiveFilterString = String.Format("[MEDICINE_TYPE_CODE] Like '%{0}%' OR [MEDICINE_TYPE_NAME] Like '%{0}%' OR [ACTIVE_INGR_BHYT_CODE] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME] Like '%{0}%' OR [CONCENTRA] Like '%{0}%'", txtKeyword.Text);
                    }
                    else
                        gridViewMediMateStockSum.ActiveFilterString = String.Format("[MATERIAL_TYPE_CODE] Like '%{0}%' OR [MATERIAL_TYPE_NAME] Like '%{0}%' OR [CONCENTRA] Like '%{0}%'", txtKeyword.Text);

                    gridViewMediMateStockSum.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                    gridViewMediMateStockSum.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                    gridViewMediMateStockSum.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                    gridViewMediMateStockSum.FocusedRowHandle = 0;
                    gridViewMediMateStockSum.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                    gridViewMediMateStockSum.OptionsFind.HighlightFindResults = false;

                    txtKeyword.Focus();
                }
                else
                {
                    gridViewMediMateStockSum.ActiveFilter.Clear();
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
                    gridControlMediMateStockSum.ExportToXlsx(saveFile.FileName);

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

        private void gridViewMediMateStockSum_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                string rowValue = Convert.ToString(this.gridViewMediMateStockSum.GetGroupRowValue(e.RowHandle, info.Column));
                info.GroupText = rowValue;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}

