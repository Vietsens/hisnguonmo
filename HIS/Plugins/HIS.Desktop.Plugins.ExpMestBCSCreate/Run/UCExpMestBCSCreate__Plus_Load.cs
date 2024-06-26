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
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.ConfigApplication;

namespace HIS.Desktop.Plugins.ExpMestBCSCreate.Run
{
    public partial class UCExpMestBCSCreate : HIS.Desktop.Utility.UserControlBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int rowCountExpM = 0;
        int dataTotalExpM = 0;
        private void InitComboMediStock(Control cboMediStock)
        {
            try
            {
                List<V_HIS_MEDI_STOCK> _mediStocks = new List<V_HIS_MEDI_STOCK>();
                var datas = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(p => p.ROOM_ID == this._Module.RoomId && p.IS_ACTIVE == 1).ToList();
                if (datas != null)
                {
                    List<long> mediStockIds = datas.Select(p => p.MEDI_STOCK_ID).ToList();
                    _mediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p =>
                        mediStockIds.Contains(p.ID)
                        && CheckIsBusiness(p.IS_BUSINESS)//p.IS_BUSINESS == this._MediStock.IS_BUSINESS
                        && p.ID != this._MediStock.ID
                        && p.IS_ACTIVE == 1).ToList();//Sua lai 2 kho cung loai : thuong or kinh doanh
                }
                if (_mediStocks != null && _mediStocks.Count > 0)
                {
                    _mediStocks.OrderBy(p => p.MEDI_STOCK_NAME).ToList();
                    this.cboMediStock.EditValue = _mediStocks[0].ID;
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboMediStock, _mediStocks, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckIsBusiness(short? IS_BUSINESS)
        {
            bool result = false;
            try
            {
                if (this._MediStock.IS_BUSINESS == 1)
                {
                    if (IS_BUSINESS == 1)
                    {
                        result = true;
                    }
                }
                else if (IS_BUSINESS != 1)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadDataExpMestBcs()
        {
            try
            {
                int pageSize = ucPagingExpMest.pagingGrid != null ? ucPagingExpMest.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                PagingExpMestBCS(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingExpMest.Init(PagingExpMestBCS, param, pageSize, gridControlExpMestBcs);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void PagingExpMestBCS(object param)
        {
            try
            {
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                gridControlExpMestBcs.DataSource = null;
                HisExpMestFilter _expMestFilter = new HisExpMestFilter();
                _expMestFilter.ORDER_FIELD = "EXP_MEST_CODE";
                _expMestFilter.ORDER_DIRECTION = "DESC";
                _expMestFilter.IMP_MEDI_STOCK_ID = this._MediStock.ID;
                // _expMestFilter.REQ_DEPARTMENT_ID = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this._Module.RoomId).DepartmentId; // filter theo khoa yeu cau(khoa cua nguoi dung dang dang nhap)
                //==>: Mac DInh Chon Tat Ca Cac Khoa

                _expMestFilter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS;
                // _expMestFilter.IS_EXPORT_EQUAL_REQUEST = true;
                var apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, _expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    var _ExpMest_BCSs = (List<HIS_EXP_MEST>)apiResult.Data;
                    if (_ExpMest_BCSs != null)
                    {
                        gridControlExpMestBcs.DataSource = _ExpMest_BCSs;
                        rowCount = _ExpMest_BCSs.Count;
                        dataTotal = apiResult.Param.Count ?? 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static List<SereServADO> LoadDataDetailExpMest(long _expMestId)
        {
            List<SereServADO> _SereServs = new List<SereServADO>();//luu tat ca thuoc va vat tu
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<V_HIS_EXP_MEST_MEDICINE> _expMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_ID = _expMestId;
                _expMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, param);

                if (_expMedicines != null && _expMedicines.Count > 0)
                {
                    var dataGroups = _expMedicines.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        SereServADO ado = new SereServADO();
                        foreach (var itemNumber in item)
                        {
                            ado.PACKAGE_NUMBER += !String.IsNullOrEmpty(itemNumber.PACKAGE_NUMBER) ? itemNumber.PACKAGE_NUMBER : "";
                        }

                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        ado.SERVICE_UNIT_NAME = item[0].SERVICE_UNIT_NAME;
                        ado.TDL_SERVICE_CODE = item[0].MEDICINE_TYPE_CODE;
                        ado.TDL_SERVICE_NAME = item[0].MEDICINE_TYPE_NAME;
                        _SereServs.Add(ado);
                    }
                }

                List<V_HIS_EXP_MEST_MATERIAL> _expMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                HisExpMestMaterialViewFilter materialFilter = new HisExpMestMaterialViewFilter();
                materialFilter.EXP_MEST_ID = _expMestId;
                _expMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, materialFilter, param);
                if (_expMaterials != null && _expMaterials.Count > 0)
                {
                    var dataGroups = _expMaterials.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        SereServADO ado = new SereServADO();
                        foreach (var itemNumber in item)
                        {
                            ado.PACKAGE_NUMBER += !String.IsNullOrEmpty(itemNumber.PACKAGE_NUMBER) ? itemNumber.PACKAGE_NUMBER : "";
                        }
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        ado.PACKAGE_NUMBER = "";
                        ado.SERVICE_UNIT_NAME = item[0].SERVICE_UNIT_NAME;
                        ado.TDL_SERVICE_CODE = item[0].MATERIAL_TYPE_CODE;
                        ado.TDL_SERVICE_NAME = item[0].MATERIAL_TYPE_NAME;
                        _SereServs.Add(ado);
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }

            return _SereServs;
        }

        internal static List<SereServADO> LoadDataDetailExpMestBcs(HIS_EXP_MEST _expMest)
        {
            List<SereServADO> _SereServs = new List<SereServADO>();//luu tat ca thuoc va vat tu
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisExpMestMetyReqFilter metyReqFilter = new HisExpMestMetyReqFilter();
                metyReqFilter.EXP_MEST_ID = _expMest.ID;
                var dataMetyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/get", ApiConsumers.MosConsumer, metyReqFilter, param);
                if (dataMetyReqs != null && dataMetyReqs.Count > 0)
                {
                    List<V_HIS_EXP_MEST_MEDICINE> _expMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                    if (_expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || _expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                    {
                        HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                        medicineFilter.EXP_MEST_ID = _expMest.ID;
                        _expMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, param);
                    }



                    var dataGroups = dataMetyReqs.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    var _meidicneTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    foreach (var item in dataGroups)
                    {
                        var dataType = _meidicneTypes.FirstOrDefault(p => p.ID == item[0].MEDICINE_TYPE_ID);
                        if (dataType == null)
                        {
                            continue;
                        }
                        List<V_HIS_EXP_MEST_MEDICINE> medicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                        medicine = (_expMedicines != null && _expMedicines.Count > 0) ? _expMedicines.Where(p => p.MEDICINE_TYPE_ID == item[0].MEDICINE_TYPE_ID).ToList() : null;

                        SereServADO ado = new SereServADO();

                        if (medicine != null && medicine.Count > 0)
                        {
                            foreach (var itemNumber in medicine)
                            {
                                ado.PACKAGE_NUMBER += !String.IsNullOrEmpty(itemNumber.PACKAGE_NUMBER) ? itemNumber.PACKAGE_NUMBER : "";
                            }
                        }

                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        ado.DD_AMOUNT = item.Sum(p => p.DD_AMOUNT);
                        ado.SERVICE_UNIT_NAME = dataType.SERVICE_UNIT_NAME;
                        ado.TDL_SERVICE_CODE = dataType.MEDICINE_TYPE_CODE;
                        ado.TDL_SERVICE_NAME = dataType.MEDICINE_TYPE_NAME;
                        _SereServs.Add(ado);
                    }
                }

                HisExpMestMatyReqFilter matyReqFilter = new HisExpMestMatyReqFilter();
                matyReqFilter.EXP_MEST_ID = _expMest.ID;
                var dataMatyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/get", ApiConsumers.MosConsumer, matyReqFilter, param);
                if (dataMatyReqs != null && dataMatyReqs.Count > 0)
                {

                    List<V_HIS_EXP_MEST_MATERIAL> _expMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                    if (_expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || _expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                    {
                        HisExpMestMaterialViewFilter materialFilter = new HisExpMestMaterialViewFilter();
                        materialFilter.EXP_MEST_ID = _expMest.ID;
                        _expMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, materialFilter, param);
                    }

                    var dataGroups = dataMatyReqs.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                    var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                    foreach (var item in dataGroups)
                    {
                        var dataType = _materialTypes.FirstOrDefault(p => p.ID == item[0].MATERIAL_TYPE_ID);
                        if (dataType == null)
                        {
                            continue;
                        }
                        List<V_HIS_EXP_MEST_MATERIAL> material = new List<V_HIS_EXP_MEST_MATERIAL>();
                        material = (_expMaterials != null && _expMaterials.Count > 0) ? _expMaterials.Where(p => p.MATERIAL_TYPE_ID == item[0].MATERIAL_TYPE_ID).ToList() : null;

                        SereServADO ado = new SereServADO();

                        if (material != null && material.Count > 0)
                        {
                            foreach (var itemNumber in material)
                            {
                                ado.PACKAGE_NUMBER += !String.IsNullOrEmpty(itemNumber.PACKAGE_NUMBER) ? itemNumber.PACKAGE_NUMBER : "";
                            }
                        }
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        ado.DD_AMOUNT = item.Sum(p => p.DD_AMOUNT);
                        ado.PACKAGE_NUMBER = "";
                        ado.SERVICE_UNIT_NAME = dataType.SERVICE_UNIT_NAME;
                        ado.TDL_SERVICE_CODE = dataType.MATERIAL_TYPE_CODE;
                        ado.TDL_SERVICE_NAME = dataType.MATERIAL_TYPE_NAME;
                        _SereServs.Add(ado);
                    }
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }

            return _SereServs;
        }
    }
    public class SereServADO
    {
        public string SERVICE_UNIT_NAME { get; set; }
        public string TDL_SERVICE_CODE { get; set; }
        public string TDL_SERVICE_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public decimal? DD_AMOUNT { get; set; }
    }
}
