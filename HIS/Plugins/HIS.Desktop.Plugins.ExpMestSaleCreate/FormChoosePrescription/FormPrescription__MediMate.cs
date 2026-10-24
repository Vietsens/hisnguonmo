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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExpMestSaleCreate.ADO;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Base;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate.FormChoosePrescription
{
    public partial class FormPrescription : FormBase
    {
        const string MEDI_MATE_RELATION_NAME = "MediMates";
        const int MAX_SERVICE_REQ_ID_PER_REQUEST = 100;

        // Prescribed medicines/materials by SERVICE_REQ_ID. Read only: no bean is taken here, prices are known only after "Đồng ý"
        Dictionary<long, List<MediMateTypeADO>> dicMediMateByServiceReq = new Dictionary<long, List<MediMateTypeADO>>();
        HashSet<long> collapsedServiceReqIds = new HashSet<long>();
        bool isUpdatingGridSelect = false;

        private void LoadMediMateByServiceReqs(List<V_HIS_SERVICE_REQ_11> serviceReqs)
        {
            try
            {
                List<long> serviceReqIds = serviceReqs.Select(o => o.ID).Distinct().Where(o => !dicMediMateByServiceReq.ContainsKey(o)).ToList();
                if (!serviceReqIds.Any())
                    return;

                WaitingManager.Show();
                Dictionary<long, V_HIS_MEDICINE_TYPE> dicMedicineType = (BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>() ?? new List<V_HIS_MEDICINE_TYPE>())
                    .GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First());
                Dictionary<long, V_HIS_MATERIAL_TYPE> dicMaterialType = (BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>() ?? new List<V_HIS_MATERIAL_TYPE>())
                    .GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First());

                for (int skip = 0; skip < serviceReqIds.Count; skip += MAX_SERVICE_REQ_ID_PER_REQUEST)
                {
                    List<long> batchIds = serviceReqIds.Skip(skip).Take(MAX_SERVICE_REQ_ID_PER_REQUEST).ToList();
                    CommonParam param = new CommonParam();

                    HisServiceReqMetyFilter metyFilter = new HisServiceReqMetyFilter();
                    metyFilter.SERVICE_REQ_IDs = batchIds;
                    List<HIS_SERVICE_REQ_METY> metys = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>(RequestUriStore.HIS_SERVICE_REQ_METY__GET, ApiConsumers.MosConsumer, metyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    HisServiceReqMatyFilter matyFilter = new HisServiceReqMatyFilter();
                    matyFilter.SERVICE_REQ_IDs = batchIds;
                    List<HIS_SERVICE_REQ_MATY> matys = metys == null ? null : new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>(RequestUriStore.HIS_SERVICE_REQ_MATY__GET, ApiConsumers.MosConsumer, matyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (metys == null || matys == null)
                    {
                        // Not cached so the next selection retries
                        Inventec.Common.Logging.LogSystem.Warn("Khong lay duoc thuoc/vat tu da ke cua y lenh"
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => batchIds), batchIds)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        continue;
                    }

                    // Also items prescribed outside the catalog (no medicine/material type, name only): BA VIETNT 24/10 wants them shown
                    ILookup<long, HIS_SERVICE_REQ_METY> metyLookup = metys.ToLookup(o => o.SERVICE_REQ_ID);
                    ILookup<long, HIS_SERVICE_REQ_MATY> matyLookup = matys.ToLookup(o => o.SERVICE_REQ_ID);
                    foreach (long serviceReqId in batchIds)
                    {
                        List<MediMateTypeADO> mediMates = new List<MediMateTypeADO>();
                        mediMates.AddRange(metyLookup[serviceReqId].GroupBy(o => GetMediMateGroupKey(o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_NAME)).Select(g => CreateMedicineAdo(g.ToList(), dicMedicineType)));
                        mediMates.AddRange(matyLookup[serviceReqId].GroupBy(o => GetMediMateGroupKey(o.MATERIAL_TYPE_ID, o.MATERIAL_TYPE_NAME)).Select(g => CreateMaterialAdo(g.ToList(), dicMaterialType)));
                        dicMediMateByServiceReq[serviceReqId] = mediMates.OrderBy(o => o.NUM_ORDER ?? 9999).ThenBy(o => o.MEDI_MATE_TYPE_NAME).ToList();
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

        // Catalog items group by type; items outside the catalog group by prescribed name
        private static string GetMediMateGroupKey(long? typeId, string typeName)
        {
            return typeId.HasValue ? "T" + typeId.Value : "N" + (typeName ?? "").Trim().ToLower();
        }

        private static string AppendOutOfCatalogNote(string name)
        {
            return String.Format("{0} ({1})", name, ResourceMessageLang.NgoaiDanhMuc);
        }

        private MediMateTypeADO CreateMedicineAdo(List<HIS_SERVICE_REQ_METY> metys, Dictionary<long, V_HIS_MEDICINE_TYPE> dicMedicineType)
        {
            HIS_SERVICE_REQ_METY first = metys[0];
            MediMateTypeADO ado = new MediMateTypeADO();
            ado.IsMedicine = true;
            ado.MEDI_MATE_TYPE_ID = first.MEDICINE_TYPE_ID ?? 0;
            ado.MEDI_MATE_TYPE_NAME = first.MEDICINE_TYPE_NAME;
            ado.SERVICE_UNIT_NAME = first.UNIT_NAME;
            ado.EXP_AMOUNT = metys.Sum(o => o.AMOUNT);
            ado.NUM_ORDER = first.NUM_ORDER;
            ado.SERVICE_REQ_ID = first.SERVICE_REQ_ID;
            if (!first.MEDICINE_TYPE_ID.HasValue)
            {
                ado.MEDI_MATE_TYPE_NAME = AppendOutOfCatalogNote(ado.MEDI_MATE_TYPE_NAME);
                return ado;
            }
            V_HIS_MEDICINE_TYPE medicineType;
            if (dicMedicineType.TryGetValue(ado.MEDI_MATE_TYPE_ID, out medicineType))
            {
                ado.MEDI_MATE_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
                ado.CONCENTRA = medicineType.CONCENTRA;
                ado.ACTIVE_INGR_BHYT_NAME = medicineType.ACTIVE_INGR_BHYT_NAME;
                if (String.IsNullOrEmpty(ado.MEDI_MATE_TYPE_NAME))
                    ado.MEDI_MATE_TYPE_NAME = medicineType.MEDICINE_TYPE_NAME;
                if (!String.IsNullOrEmpty(medicineType.SERVICE_UNIT_NAME))
                    ado.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                if (HisConfigCFG.IS_JOIN_NAME_WITH_CONCENTRA)
                    ado.MEDI_MATE_TYPE_NAME = String.Format("{0} {1}", ado.MEDI_MATE_TYPE_NAME, ado.CONCENTRA);
            }
            return ado;
        }

        private MediMateTypeADO CreateMaterialAdo(List<HIS_SERVICE_REQ_MATY> matys, Dictionary<long, V_HIS_MATERIAL_TYPE> dicMaterialType)
        {
            HIS_SERVICE_REQ_MATY first = matys[0];
            MediMateTypeADO ado = new MediMateTypeADO();
            ado.IsMaterial = true;
            ado.MEDI_MATE_TYPE_ID = first.MATERIAL_TYPE_ID ?? 0;
            ado.MEDI_MATE_TYPE_NAME = first.MATERIAL_TYPE_NAME;
            ado.SERVICE_UNIT_NAME = first.UNIT_NAME;
            ado.EXP_AMOUNT = matys.Sum(o => o.AMOUNT);
            ado.NUM_ORDER = first.NUM_ORDER;
            ado.SERVICE_REQ_ID = first.SERVICE_REQ_ID;
            if (!first.MATERIAL_TYPE_ID.HasValue)
            {
                ado.MEDI_MATE_TYPE_NAME = AppendOutOfCatalogNote(ado.MEDI_MATE_TYPE_NAME);
                return ado;
            }
            V_HIS_MATERIAL_TYPE materialType;
            if (dicMaterialType.TryGetValue(ado.MEDI_MATE_TYPE_ID, out materialType))
            {
                ado.MEDI_MATE_TYPE_CODE = materialType.MATERIAL_TYPE_CODE;
                ado.CONCENTRA = materialType.CONCENTRA;
                if (String.IsNullOrEmpty(ado.MEDI_MATE_TYPE_NAME))
                    ado.MEDI_MATE_TYPE_NAME = materialType.MATERIAL_TYPE_NAME;
                if (!String.IsNullOrEmpty(materialType.SERVICE_UNIT_NAME))
                    ado.SERVICE_UNIT_NAME = materialType.SERVICE_UNIT_NAME;
            }
            return ado;
        }

        private List<MediMateTypeADO> GetMediMatesByRow(GridView view, int rowHandle)
        {
            List<MediMateTypeADO> result = null;
            V_HIS_SERVICE_REQ_11 row = view.GetRow(rowHandle) as V_HIS_SERVICE_REQ_11;
            if (row != null)
                dicMediMateByServiceReq.TryGetValue(row.ID, out result);
            return result ?? new List<MediMateTypeADO>();
        }

        // Re-assigning the same DataSource is a no-op, so expanded details stay on row handles and shift after a delete:
        // drop them all, then expand again by service req (unless the user collapsed it)
        private void ExpandMediMateRows()
        {
            try
            {
                gridViewSelecteServiceReq.CollapseAllDetails();
                for (int i = 0; i < gridViewSelecteServiceReq.DataRowCount; i++)
                {
                    V_HIS_SERVICE_REQ_11 row = gridViewSelecteServiceReq.GetRow(i) as V_HIS_SERVICE_REQ_11;
                    if (row != null && !collapsedServiceReqIds.Contains(row.ID))
                        gridViewSelecteServiceReq.SetMasterRowExpanded(i, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSelecteServiceReq_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            try
            {
                e.IsEmpty = !GetMediMatesByRow((GridView)sender, e.RowHandle).Any();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSelecteServiceReq_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            try
            {
                e.ChildList = GetMediMatesByRow((GridView)sender, e.RowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSelecteServiceReq_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = MEDI_MATE_RELATION_NAME;
        }

        private void gridViewSelecteServiceReq_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gridViewSelecteServiceReq_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            try
            {
                if (isUpdatingGridSelect)
                    return;
                V_HIS_SERVICE_REQ_11 row = ((GridView)sender).GetRow(e.RowHandle) as V_HIS_SERVICE_REQ_11;
                if (row != null)
                    collapsedServiceReqIds.Remove(row.ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSelecteServiceReq_MasterRowCollapsed(object sender, CustomMasterRowEventArgs e)
        {
            try
            {
                if (isUpdatingGridSelect)
                    return;
                V_HIS_SERVICE_REQ_11 row = ((GridView)sender).GetRow(e.RowHandle) as V_HIS_SERVICE_REQ_11;
                if (row != null)
                    collapsedServiceReqIds.Add(row.ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCollapseAll_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < gridViewSelecteServiceReq.DataRowCount; i++)
                {
                    V_HIS_SERVICE_REQ_11 row = gridViewSelecteServiceReq.GetRow(i) as V_HIS_SERVICE_REQ_11;
                    if (row != null)
                        collapsedServiceReqIds.Add(row.ID);
                }
                gridViewSelecteServiceReq.CollapseAllDetails();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExpandAll_Click(object sender, EventArgs e)
        {
            try
            {
                collapsedServiceReqIds.Clear();
                ExpandMediMateRows();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSelectMediMate_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.FieldName == "MEDI_MATE_TYPE_NAME_STR")
                {
                    var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                        e.Value = (e.ListSourceRowIndex + 1) + " - " + data.MEDI_MATE_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
