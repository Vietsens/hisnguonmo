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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisExpMestMediMate.HisExpMestMediMate
{
    public partial class UCHisExpMestMediMate : HIS.Desktop.Utility.UserControlBase
    {
        List<V_HIS_EXP_MEST_MATERIAL> ListExpMestMaterial;
        private void gridviewHistoryMaterial_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var data = (MaterialTypeADO)gridViewHistoryMaterial.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (data.IsExp)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }

                    if (data.MEST_ID > 0 && expMest != null && expMest.Count > 0)
                    {
                        var exp = expMest.FirstOrDefault(o => o.ID == data.MEST_ID);
                        if (exp != null && exp.IS_NOT_TAKEN == 1)
                        {
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridviewHistoryMaterial_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MaterialTypeADO pData = (MaterialTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (pData != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + ((ucPaging.pagingGrid?.CurrentPage - 1) * ucPaging.pagingGrid?.PageSize);
                        }
                        else if (e.Column.FieldName == "TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "TDL_INTRUCTION_TIME_STR")
                        {
                            try
                            {
                                long intructionTime = long.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "TDL_INTRUCTION_TIME") ?? 0).ToString());
                                if (intructionTime != null && intructionTime > 0)
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(intructionTime);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "MEDI_STOCK_NAME_STR")
                        {
                            try
                            {
                                //Check từng dòng xem là loại xuất nhập nào để hiển thị thông tin 
                                bool IsExp = pData.IsExp;// bool.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "IsExp") ?? "").ToString());
                                if (IsExp)
                                {
                                    if (expMest != null && expMest.Count > 0)
                                    {
                                        long MEST_ID = pData.MEST_ID;// long.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "MEST_ID") ?? 0).ToString());
                                        long EXP_MEST_TYPE_ID = pData.EXP_MEST_TYPE_ID;//long.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "EXP_MEST_TYPE_ID") ?? 0).ToString());

                                        if (EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                                        {
                                            var mest = expMest.FirstOrDefault(o => o.ID == MEST_ID);
                                            if (mest != null)
                                            {
                                                var stock = medistocks != null && medistocks.Count > 0 ? medistocks.FirstOrDefault(p => p.ID == mest.IMP_MEDI_STOCK_ID) : new V_HIS_MEDI_STOCK();
                                                e.Value = stock != null ? stock.MEDI_STOCK_NAME : "";
                                            }
                                            else
                                                e.Value = "";
                                        }
                                        if (EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK || EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT || EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT)
                                        {
                                            var expMestEdit = expMest.FirstOrDefault(o => o.ID == MEST_ID);
                                            e.Value = expMestEdit != null ? expMestEdit.TDL_TREATMENT_CODE + "-" + expMestEdit.TDL_PATIENT_NAME : "";
                                        }
                                        if (EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                                        {
                                            var expMestEdit = expMest.FirstOrDefault(o => o.ID == MEST_ID);
                                            e.Value = expMestEdit != null ? expMestEdit.REQ_DEPARTMENT_NAME + "-" + expMestEdit.REQ_ROOM_NAME : "";
                                        }
                                        if (EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                                        {
                                            var expMestEdit = expMest.FirstOrDefault(o => o.ID == MEST_ID);
                                            e.Value = expMestEdit != null ? expMestEdit.TDL_PATIENT_NAME : "";
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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
        private void CreatThreadLoadDataMestMaterial(object param)
        {
            System.Threading.Thread expMest = new System.Threading.Thread(() => ProcessGetExpMestMaterial(param));
            try
            {
                expMest.Start();

                expMest.Join();
            }
            catch (Exception ex)
            {
                expMest.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessGetExpMestMaterial(object param)
        {
            try
            {
                HisExpMestMaterialViewFilter ExpFilter = new HisExpMestMaterialViewFilter();
                ExpFilter.ORDER_FIELD = "EXP_TIME";
                ExpFilter.ORDER_DIRECTION = "DESC";
                ExpFilter.KEY_WORD = this.txtSearchMediMate.Text;
                if (dtFrom.EditValue != null && dtFrom.DateTime != DateTime.MinValue)
                {
                    ExpFilter.EXP_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtFrom.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dtTo.EditValue != null && dtTo.DateTime != DateTime.MinValue)
                {
                    ExpFilter.EXP_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtTo.EditValue).ToString("yyyyMMdd") + "235959");
                }
                if (this._StatusSelecteds != null && this._StatusSelecteds.Count > 0)
                {
                    ExpFilter.EXP_MEST_STT_IDs = new List<long>();
                    ExpFilter.EXP_MEST_STT_IDs = this._StatusSelecteds.Select(p => p.ID).ToList();
                }
                if (this._DepartmentSelecteds != null && this._DepartmentSelecteds.Count > 0)
                {
                    ExpFilter.REQ_DEPARTMENT_IDs = new List<long>();
                    ExpFilter.REQ_DEPARTMENT_IDs = this._DepartmentSelecteds.Select(p => p.ID).ToList();
                }
                if (this._ExpMediStockSelecteds != null && this._ExpMediStockSelecteds.Count > 0)
                {
                    ExpFilter.MEDI_STOCK_IDs = new List<long>();
                    ExpFilter.MEDI_STOCK_IDs = this._ExpMediStockSelecteds.Select(p => p.ID).ToList();
                }
                if (this._ExpMestTypeSelecteds != null && this._ExpMestTypeSelecteds.Count > 0)
                {
                    ExpFilter.EXP_MEST_TYPE_IDs = new List<long>();
                    ExpFilter.EXP_MEST_TYPE_IDs = this._ExpMestTypeSelecteds.Select(p => p.ID).ToList();
                }
                else
                {
                    return;
                }
                if (!string.IsNullOrEmpty(txtPakageNumber.Text.Trim()))
                {
                    ExpFilter.PACKAGE_NUMBER__EXACT = txtPakageNumber.Text.Trim();
                }
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<V_HIS_EXP_MEST_MATERIAL>> apiResult = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, ExpFilter, paramCommon);
                if (apiResult != null && apiResult.Data != null)
                {
                    rowCount = apiResult.Data == null ? 0 : apiResult.Data.Count;
                    dataTotal = apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0;
                    ListExpMestMaterial.AddRange(apiResult.Data);
                    List<long> expMestIds = apiResult.Data.Select(s => s.EXP_MEST_ID ?? 0).Distinct().ToList();

                    this.expMest = new List<V_HIS_EXP_MEST>();
                    var skip = 0;
                    while (expMestIds.Count - skip > 0)
                    {
                        var listIDs = expMestIds.Skip(skip).Take(MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + MAX_REQUEST_LENGTH_PARAM;
                        HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                        expMestFilter.IDs = listIDs;
                        var vExpMest = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, null);
                        if (vExpMest != null && vExpMest.Count > 0)
                        {
                            expMest.AddRange(vExpMest);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadGridDataHistoryMaterial(object param)
        {
            try
            {
                LoadDefaultLoadForm();

                var listMatyAdos = new List<MaterialTypeADO>();
                var listMaterialTypeAdos = new List<MaterialTypeADO>();

                expMestByImp = new List<HIS_EXP_MEST>();
                ListExpMestMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();

                CreatThreadLoadDataMestMaterial(param);

                List<HIS_MEDI_STOCK_PERIOD> _MediStockPeriods = new List<HIS_MEDI_STOCK_PERIOD>();
                List<long> MEDI_STOCK_PERIOD_IDs = new List<long>();

                if (ListExpMestMaterial != null && ListExpMestMaterial.Count > 0)
                {
                    MEDI_STOCK_PERIOD_IDs.AddRange(ListExpMestMaterial.Select(p => p.MEDI_STOCK_PERIOD_ID ?? 0).ToList());
                }
                if (MEDI_STOCK_PERIOD_IDs != null && MEDI_STOCK_PERIOD_IDs.Count > 0)
                {
                    MEDI_STOCK_PERIOD_IDs = MEDI_STOCK_PERIOD_IDs.Where(o => o != 0).Distinct().ToList();
                }

                int skip = 0;
                while (MEDI_STOCK_PERIOD_IDs.Count - skip > 0)
                {
                    var lstMety = MEDI_STOCK_PERIOD_IDs.Skip(skip).Take(MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip += MAX_REQUEST_LENGTH_PARAM;
                    MOS.Filter.HisMediStockPeriodFilter filterPeriod = new HisMediStockPeriodFilter();
                    filterPeriod.IDs = lstMety;
                    var _MediStockPeriodsTmp = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_STOCK_PERIOD>>("api/HisMediStockPeriod/Get", ApiConsumers.MosConsumer, filterPeriod, new CommonParam());
                    if (_MediStockPeriodsTmp != null && _MediStockPeriodsTmp.Count > 0)
                        _MediStockPeriods.AddRange(_MediStockPeriodsTmp);
                }


                if (ListExpMestMaterial != null && ListExpMestMaterial.Count > 0)
                {
                    foreach (var item in ListExpMestMaterial)
                    {
                        MaterialTypeADO ado = new MaterialTypeADO();
                        ado.MATERIAL_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                        ado.TIME = item.EXP_TIME;
                        ado.CREATE_TIME = item.CREATE_TIME;
                        ado.TDL_INTRUCTION_TIME = item.TDL_INTRUCTION_TIME;
                        ado.MEST_ID = item.EXP_MEST_ID ?? 0;
                        ado.MEST_CODE = item.EXP_MEST_CODE;
                        var _ExpType = BackendDataWorker.Get<HIS_EXP_MEST_TYPE>().FirstOrDefault(p => p.ID == item.EXP_MEST_TYPE_ID);
                        if (_ExpType != null)
                        {
                            if (item.EXP_MEST_ID != null && item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)//loại xuất chuyển kho
                            {
                                var _ExpMest = this.expMest.FirstOrDefault(p => p.ID == item.EXP_MEST_ID);
                                if (_ExpMest != null)
                                {
                                    if (_ExpMest.CHMS_TYPE_ID == null)
                                        ado.MEST_TYPE = "Xuất chuyển kho";
                                    if (_ExpMest.CHMS_TYPE_ID == 1)
                                        ado.MEST_TYPE = "Bổ sung cơ số";
                                    if (_ExpMest.CHMS_TYPE_ID == 2)
                                        ado.MEST_TYPE = "Thu hồi cơ số";
                                }
                            }
                            else
                                ado.MEST_TYPE = _ExpType != null ? _ExpType.EXP_MEST_TYPE_NAME : "";
                        }

                        ado.EXP_MEST_TYPE_ID = item.EXP_MEST_TYPE_ID;
                        ado.AMOUNT = item.AMOUNT;
                        //Inventec.Common.Logging.LogSystem.Info("item.IMP_VAT_RATIO matre2" + item.IMP_VAT_RATIO);
                        //if (item.IMP_VAT_RATIO > 0)
                        //{
                        //    ado.PRICE = item.PRICE + (item.IMP_VAT_RATIO * item.PRICE);
                        //    Inventec.Common.Logging.LogSystem.Info("ado.PRICE mater2" + ado.PRICE);
                        //}
                        //else
                        //{
                        ado.PRICE = item.PRICE;
                        //}                        //}
                        ado.IsExp = true;
                        var _Period = _MediStockPeriods.FirstOrDefault(p => p.ID == item.MEDI_STOCK_PERIOD_ID);
                        ado.MEDI_STOCK_PERIOD_NAME = _Period != null ? _Period.MEDI_STOCK_PERIOD_NAME : "";
                        var _MediStock = glstMediStock.FirstOrDefault(p => p.ID == item.MEDI_STOCK_ID);
                        ado.MEDI_STOCK_NAME = _MediStock != null ? _MediStock.MEDI_STOCK_NAME : "";
                        ado.EXP_MEDI_STOCK_NAME = ado.MEDI_STOCK_NAME;
                        var _STT = glstExpMestStt.FirstOrDefault(p => p.ID == item.EXP_MEST_STT_ID);
                        ado.STT_NAME = _STT != null ? _STT.EXP_MEST_STT_NAME : "";
                        ado.STT_ID = _STT != null ? _STT.ID : 0;
                        var _Department = glstDepartment.FirstOrDefault(p => p.ID == item.REQ_DEPARTMENT_ID);
                        ado.REQ_DEPARTMENT_NAME = _Department != null ? _Department.DEPARTMENT_NAME : "";
                        ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;

                        listMaterialTypeAdos.Add(ado);
                    }
                }

                //Các phiếu xuất nhập được group theo mã và giá
                listMaterialTypeAdos = listMaterialTypeAdos.OrderByDescending(o => o.TIME).ToList();
                //var listGroup = listMaterialTypeAdos.GroupBy(o => new { o.MEST_ID, o.MEST_CODE, o.PRICE, o.IsExp }).ToList();
                var listGroup = listMaterialTypeAdos;
                List<MaterialTypeADO> listAdo = new List<MaterialTypeADO>();
                foreach (var item in listGroup)
                {
                    MaterialTypeADO ado = new MaterialTypeADO();
                    ado.MEST_ID = item.MEST_ID;
                    ado.MATERIAL_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                    ado.AMOUNT = item.AMOUNT;
                    ado.TIME = item.TIME;
                    ado.CREATE_TIME = item.CREATE_TIME;
                    ado.TDL_INTRUCTION_TIME = item.TDL_INTRUCTION_TIME;
                    ado.MEST_CODE = item.MEST_CODE;
                    ado.MEST_TYPE = item.MEST_TYPE;
                    ado.EXP_MEST_TYPE_ID = item.EXP_MEST_TYPE_ID;
                    ado.IMP_MEST_TYPE_ID = item.IMP_MEST_TYPE_ID;
                    ado.PRICE = item.PRICE;
                    ado.MEDI_STOCK_PERIOD_NAME = item.MEDI_STOCK_PERIOD_NAME;
                    ado.MEDI_STOCK_NAME = item.MEDI_STOCK_NAME;
                    ado.IsExp = item.IsExp;
                    ado.MEDI_STOCK_NAME = item.MEDI_STOCK_NAME;
                    ado.IMP_MEDI_STOCK_NAME = item.IMP_MEDI_STOCK_NAME;
                    ado.EXP_MEDI_STOCK_NAME = item.EXP_MEDI_STOCK_NAME;
                    ado.STT_NAME = item.STT_NAME;
                    ado.REQ_DEPARTMENT_NAME = item.REQ_DEPARTMENT_NAME;
                    ado.DOCUMENT_NUMBER = item.DOCUMENT_NUMBER;
                    ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;

                    ado.KEY_WORD = convertToUnSign3(item.EXP_MEDI_STOCK_NAME) + item.EXP_MEDI_STOCK_NAME
                                + convertToUnSign3(item.IMP_MEDI_STOCK_NAME) + item.IMP_MEDI_STOCK_NAME
                                + convertToUnSign3(item.MEDI_STOCK_NAME) + item.MEDI_STOCK_NAME
                                + convertToUnSign3(item.MEDI_STOCK_PERIOD_NAME) + item.MEDI_STOCK_PERIOD_NAME
                                + convertToUnSign3(item.REQ_DEPARTMENT_NAME) + item.REQ_DEPARTMENT_NAME
                                + convertToUnSign3(item.STT_NAME) + item.STT_NAME
                                + convertToUnSign3(item.MATERIAL_TYPE_CODE) + item.MATERIAL_TYPE_CODE
                                + convertToUnSign3(item.MEST_CODE) + item.MEST_CODE
                                + convertToUnSign3(item.MEST_TYPE) + item.MEST_TYPE;

                    listAdo.Add(ado);
                }

                if (listAdo != null && listAdo.Count > 0)
                {
                    listAdo = listAdo.OrderByDescending(p => p.TIME).ThenByDescending(p => p.MEST_CODE).ToList();
                }

                gridControlFormList.BeginUpdate();
                gridControlFormList.DataSource = listAdo;
                gridControlFormList.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
