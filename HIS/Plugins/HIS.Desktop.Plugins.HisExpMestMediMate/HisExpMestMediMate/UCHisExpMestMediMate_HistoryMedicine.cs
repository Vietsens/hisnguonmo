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
using System.Windows.Markup;

namespace HIS.Desktop.Plugins.HisExpMestMediMate.HisExpMestMediMate
{
    public partial class UCHisExpMestMediMate : HIS.Desktop.Utility.UserControlBase
    {

        private void gridviewHistoryMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var data = (MedicineTypeADO)gridViewHistoryMedicine.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (data.IsExp)
                    {
                        e.Appearance.ForeColor = Color.Red;
                        if (data.MEST_ID > 0)
                        {
                            var exp = expMest.FirstOrDefault(o => o.ID == data.MEST_ID);
                            if (exp != null && exp.IS_NOT_TAKEN == 1)
                            {
                                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                            }
                        }
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridviewHistoryMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }
        private void gridviewHistoryMedicine_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MedicineTypeADO pData = (MedicineTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (pData != null)
                    {
                        if (e.Column.FieldName == "TDL_INTRUCTION_TIME_STR")
                        {
                            if (pData.TDL_INTRUCTION_TIME != null)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.TDL_INTRUCTION_TIME ?? 0);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            if (pData.CREATE_TIME != null)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                            }
                        }
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
                        else if (e.Column.FieldName == "MEDI_STOCK_NAME_STR")
                        {
                            try
                            {
                                //Check từng dòng xem là loại xuất nhập nào để hiển thị thông tin 
                                //bool IsExp = Convert.ToBoolean((view.GetRowCellValue(e.ListSourceRowIndex, "IsExp") ?? "False").ToString());
                                if (pData.IsExp)
                                {
                                    if (expMest != null && expMest.Count > 0)
                                    {
                                        long MEST_ID = pData.MEST_ID; //long.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "MEST_ID") ?? 0).ToString());
                                        long EXP_MEST_TYPE_ID = pData.EXP_MEST_TYPE_ID;// long.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "EXP_MEST_TYPE_ID") ?? 0).ToString());
                                        if (EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                                        {
                                            var mest = expMest.FirstOrDefault(o => o.ID == MEST_ID);
                                            if (mest != null)
                                            {
                                                var stock = medistocks.FirstOrDefault(p => p.ID == mest.IMP_MEDI_STOCK_ID);
                                                e.Value = stock != null ? stock.MEDI_STOCK_NAME : "";
                                            }
                                            else
                                                e.Value = "";
                                        }
                                        else if (EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK || EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT || EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT)
                                        {
                                            var expMestEdit = expMest.FirstOrDefault(o => o.ID == MEST_ID);
                                            e.Value = expMestEdit != null ? expMestEdit.TDL_TREATMENT_CODE + "-" + expMestEdit.TDL_PATIENT_NAME : "";
                                        }
                                        else if (EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                                        {
                                            var expMestEdit = expMest.FirstOrDefault(o => o.ID == MEST_ID);
                                            e.Value = expMestEdit != null ? expMestEdit.REQ_DEPARTMENT_NAME + "-" + expMestEdit.REQ_ROOM_NAME : "";
                                        }
                                        else if (EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                                        {
                                            var expMestEdit = expMest.FirstOrDefault(o => o.ID == MEST_ID);
                                            e.Value = expMestEdit != null ? expMestEdit.TDL_PATIENT_NAME : "";
                                        }
                                    }
                                }
                                //else
                                //{
                                //    if (impMest != null && impMest.Count > 0)
                                //    {
                                //        long MEST_ID = pData.MEST_ID; //long.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "MEST_ID") ?? 0).ToString());
                                //        long IMP_MEST_TYPE_ID = pData.EXP_MEST_TYPE_ID; //long.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "IMP_MEST_TYPE_ID") ?? 0).ToString());

                                //        if (IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK)
                                //        {
                                //            e.Value = dicChmsImpMest.ContainsKey(MEST_ID) ? dicChmsImpMest[MEST_ID] : "";
                                //        }
                                //        else if (IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL ||
                                //            IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL ||
                                //            IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL ||
                                //            IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL)
                                //        {
                                //            var moba = impMest.FirstOrDefault(o => o.ID == MEST_ID);
                                //            e.Value = moba != null ? moba.TDL_MOBA_EXP_MEST_CODE : "";
                                //        }
                                //    }
                                //}

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

        private void CreatThreadLoadDataMestMedcine(object param)
        {
            //System.Threading.Thread impMest = new System.Threading.Thread(ProcessGetImpMest);
            System.Threading.Thread expMest = new System.Threading.Thread(() => ProcessGetExpMestMedicine(param));
            try
            {
                //impMest.Start();
                expMest.Start();

                //impMest.Join();
                expMest.Join();
            }
            catch (Exception ex)
            {
                //impMest.Abort();
                expMest.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetExpMestMedicine(object param)
        {
            try
            {
                HisExpMestMedicineViewFilter ExpFilter = new HisExpMestMedicineViewFilter();
                ExpFilter.ORDER_FIELD = "EXP_TIME";
                ExpFilter.ORDER_DIRECTION = "DESC";
                ExpFilter.KEY_WORD = this.txtSearchMediMate.Text;
                if (dtFrom.EditValue != null && dtFrom.DateTime != DateTime.MinValue)
                {
                    ExpFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtFrom.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dtTo.EditValue != null && dtTo.DateTime != DateTime.MinValue)
                {
                    ExpFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtTo.EditValue).ToString("yyyyMMdd") + "235959");
                }
                if (!string.IsNullOrEmpty(txtPakageNumber.Text.Trim()))
                {
                    ExpFilter.PACKAGE_NUMBER__EXACT = txtPakageNumber.Text.Trim();
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
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<V_HIS_EXP_MEST_MEDICINE>> apiResult = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, ExpFilter, paramCommon);
                if (apiResult != null && apiResult.Data != null && apiResult.Data.Count > 0)
                {
                    rowCount = apiResult.Data == null ? 0 : apiResult.Data.Count;
                    dataTotal = apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0;
                    ListExpMestmedicine.AddRange(apiResult.Data);
                    List<long> expMestIds = apiResult.Data.Select(s => s.EXP_MEST_ID ?? 0).Distinct().ToList();
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
                            this.expMest.AddRange(vExpMest);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadGridDataHistoryMedicine(object param)
        {
            try
            {

                var listMedicineTypeAdos = new List<MedicineTypeADO>();

                expMest = new List<V_HIS_EXP_MEST>();

                ListExpMestmedicine = new List<V_HIS_EXP_MEST_MEDICINE>();

                CreatThreadLoadDataMestMedcine(param);

                List<HIS_MEDI_STOCK_PERIOD> _MediStockPeriods = new List<HIS_MEDI_STOCK_PERIOD>();
                List<long> MEDI_STOCK_PERIOD_IDs = new List<long>();

                if (ListImpMestMedicine != null && ListImpMestMedicine.Count > 0)
                {
                    MEDI_STOCK_PERIOD_IDs.AddRange(ListImpMestMedicine.Select(p => p.MEDI_STOCK_PERIOD_ID ?? 0).ToList());
                }

                if (ListExpMestmedicine != null && ListExpMestmedicine.Count > 0)
                {
                    MEDI_STOCK_PERIOD_IDs.AddRange(ListExpMestmedicine.Select(p => p.MEDI_STOCK_PERIOD_ID ?? 0).ToList());
                }

                if (MEDI_STOCK_PERIOD_IDs != null && MEDI_STOCK_PERIOD_IDs.Count > 0)
                {
                    MEDI_STOCK_PERIOD_IDs = MEDI_STOCK_PERIOD_IDs.Distinct().ToList();

                    var skip = 0;
                    while (MEDI_STOCK_PERIOD_IDs.Count - skip > 0)
                    {
                        var listIDs = MEDI_STOCK_PERIOD_IDs.Skip(skip).Take(MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + MAX_REQUEST_LENGTH_PARAM;
                        MOS.Filter.HisMediStockPeriodFilter filterPeriod = new HisMediStockPeriodFilter();
                        filterPeriod.IDs = listIDs;
                        var Periods = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_STOCK_PERIOD>>("api/HisMediStockPeriod/Get", ApiConsumers.MosConsumer, filterPeriod, null);
                        if (Periods != null && Periods.Count > 0)
                        {
                            _MediStockPeriods.AddRange(Periods);
                        }
                    }
                }
                if (ListExpMestmedicine != null && ListExpMestmedicine.Count > 0)
                {
                    var glstHisExpMestType = BackendDataWorker.Get<HIS_EXP_MEST_TYPE>().Where(o => ListExpMestmedicine.Select(imp => imp.EXP_MEST_TYPE_ID).Contains(o.ID)).ToList();

                    foreach (var item in ListExpMestmedicine)
                    {
                        MedicineTypeADO ado = new MedicineTypeADO();
                        ado.MEDICINE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                        ado.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                        ado.TIME = item.EXP_TIME;
                        ado.MEST_ID = item.EXP_MEST_ID ?? 0;
                        ado.MEST_CODE = item.EXP_MEST_CODE;
                        ado.CREATE_TIME = item.CREATE_TIME;
                        ado.TDL_INTRUCTION_TIME = item.TDL_INTRUCTION_TIME;
                        var _ExpType = glstHisExpMestType.FirstOrDefault(p => p.ID == item.EXP_MEST_TYPE_ID);
                        if (_ExpType != null)
                        {
                            if (item.EXP_MEST_ID != null && item.EXP_MEST_TYPE_ID == 3)//loại xuất chuyển kho
                            {
                                var _ExpMest = expMest.FirstOrDefault(p => p.ID == item.EXP_MEST_ID);
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
                        //Inventec.Common.Logging.LogSystem.Info("item.IMP_VAT_RATIO 2" + item.IMP_VAT_RATIO);
                        //if (item.IMP_VAT_RATIO > 0)
                        //{
                        //    ado.PRICE = item.PRICE + (item.IMP_VAT_RATIO * item.PRICE);
                        //    Inventec.Common.Logging.LogSystem.Info("ado.PRICE 2" + ado.PRICE);
                        //}
                        //else
                        //{
                        ado.PRICE = item.PRICE;
                        //}
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
                        listMedicineTypeAdos.Add(ado);
                    }
                }
                //Các phiếu xuất nhập được group theo mã và giá
                listMedicineTypeAdos = listMedicineTypeAdos.OrderByDescending(o => o.TIME).ToList();
                //var listGroup = listMedicineTypeAdos.GroupBy(o => new { o.MEST_ID, o.MEST_CODE, o.PRICE, o.IsExp }).ToList();
                var listGroup = listMedicineTypeAdos;
                List<MedicineTypeADO> listAdo = new List<MedicineTypeADO>();
                foreach (var item in listGroup)
                {
                    MedicineTypeADO ado = new MedicineTypeADO();
                    ado.MEST_ID = item.MEST_ID;
                    ado.CREATE_TIME = item.CREATE_TIME;
                    ado.TDL_INTRUCTION_TIME = item.TDL_INTRUCTION_TIME;
                    ado.MEDICINE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                    ado.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                    ado.AMOUNT = item.AMOUNT;
                    ado.TIME = item.TIME;
                    ado.MEST_CODE = item.MEST_CODE;
                    ado.MEST_TYPE = item.MEST_TYPE;
                    ado.IMP_MEST_TYPE_ID = item.IMP_MEST_TYPE_ID;
                    ado.EXP_MEST_TYPE_ID = item.EXP_MEST_TYPE_ID;
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
                                + convertToUnSign3(item.MEDICINE_TYPE_CODE) + item.MEDICINE_TYPE_CODE
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
                //if (listAdo != null)
                //{
                //    dataTotal = listAdo.Count();
                //    var result = listAdo.Skip(startPage).Take(limit).ToList();
                //    rowCount = (result == null ? 0 : result.Count);
                //    gridControlFormList.DataSource = result;
                //}
                //else
                //{
                //    rowCount = 0;
                //    dataTotal = 0;
                //}
                gridControlFormList.EndUpdate();
                //gridviewFormList.EndUpdate();
                //
                if (listAdo != null && listAdo.Count > 0)
                {
                    gridViewHistoryMedicine.BestFitColumns();
                }
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost((CommonParam)param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
