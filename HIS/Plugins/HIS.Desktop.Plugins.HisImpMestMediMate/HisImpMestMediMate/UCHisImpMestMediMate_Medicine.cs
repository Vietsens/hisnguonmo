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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.HisImpMestMediMate.ADO;
using HIS.Desktop.Plugins.HisImpMestMediMate.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.HisImpMestMediMate.HisImpMestMediMate
{
    public partial class UCHisImpMestMediMate : HIS.Desktop.Utility.UserControlBase
    {
        /// <summary>
        /// Lay cac lan nhap cua 1 loai thuoc da chon.
        /// Chi lay phieu nhap tu Nha cung cap va da thuc nhap.
        /// </summary>
        private void LoadDataMedicine()
        {
            try
            {
                this.listMedicineResult = new List<ImpMestMedicineADO>();
                this.gridControlData.DataSource = null;

                long medicineTypeId = GetSelectedTypeId(this.cboMedicineType);
                if (medicineTypeId <= 0)
                {
                    UpdateResultState();
                    return;
                }

                var detail = GetImpMestMedicineView(medicineTypeId);
                if (detail == null || detail.Count == 0)
                {
                    UpdateResultState();
                    return;
                }

                var ados = detail.Select(o => new ImpMestMedicineADO(o)).ToList();

                // Ngay hoa don khong nam cung du lieu chi tiet nhap -> lay tu thong tin phieu nhap
                var documentDates = GetDocumentDateByImpMest(ados.Select(o => o.IMP_MEST_ID).ToList());

                // Nguon nhap nam tren lo thuoc -> lay theo danh sach lo
                var impSourceByMedicine = GetImpSourceIdByMedicine(ados.Select(o => o.MEDICINE_ID).ToList());

                foreach (var ado in ados)
                {
                    if (documentDates.ContainsKey(ado.IMP_MEST_ID))
                    {
                        ado.DOCUMENT_DATE = documentDates[ado.IMP_MEST_ID];
                    }
                    ado.MEDI_STOCK_NAME = GetMediStockName(ado.MEDI_STOCK_ID);
                    if (impSourceByMedicine.ContainsKey(ado.MEDICINE_ID))
                    {
                        ado.IMP_SOURCE_NAME = GetImpSourceName(impSourceByMedicine[ado.MEDICINE_ID]);
                    }
                    ado.RebuildKeyWord();
                }

                // Moi nhat len tren
                ados = ados
                    .OrderByDescending(o => o.IMP_TIME ?? 0)
                    .ThenByDescending(o => o.IMP_MEST_CODE)
                    .ToList();

                for (int i = 0; i < ados.Count; i++)
                {
                    ados[i].STT = i + 1;
                }

                this.listMedicineResult = ados;

                this.gridControlData.BeginUpdate();
                this.gridControlData.DataSource = this.listMedicineResult;
                this.gridControlData.EndUpdate();

                UpdateResultState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_IMP_MEST_MEDICINE> GetImpMestMedicineView(long medicineTypeId)
        {
            var result = new List<V_HIS_IMP_MEST_MEDICINE>();
            try
            {
                CommonParam param = new CommonParam();
                var filter = new HisImpMestMedicineViewFilter();
                filter.ORDER_FIELD = "IMP_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.MEDICINE_TYPE_ID = medicineTypeId;
                filter.IMP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC;
                filter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;

                long timeFrom = GetTimeFrom();
                if (timeFrom > 0) filter.IMP_TIME_FROM = timeFrom;
                long timeTo = GetTimeTo();
                if (timeTo > 0) filter.IMP_TIME_TO = timeTo;

                var data = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(
                    MediMateRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW,
                    ApiConsumers.MosConsumer, filter, param);

                if (data != null && data.Count > 0)
                {
                    result = data;
                }

                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Nguon nhap duoc khai tren lo thuoc. Chia lo danh sach Id de khong vuot
        /// gioi han do dai tham so request.
        /// </summary>
        private Dictionary<long, long?> GetImpSourceIdByMedicine(List<long> medicineIds)
        {
            var result = new Dictionary<long, long?>();
            try
            {
                if (medicineIds == null || medicineIds.Count == 0) return result;

                var ids = medicineIds.Where(o => o > 0).Distinct().ToList();
                int skip = 0;
                while (ids.Count - skip > 0)
                {
                    var pageIds = ids.Skip(skip).Take(MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip += MAX_REQUEST_LENGTH_PARAM;

                    var filter = new HisMedicineViewFilter();
                    filter.IDs = pageIds;
                    var data = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICINE>>(
                        MediMateRequestUriStore.HIS_MEDICINE_GETVIEW,
                        ApiConsumers.MosConsumer, filter, null);

                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            if (!result.ContainsKey(item.ID))
                            {
                                result.Add(item.ID, item.IMP_SOURCE_ID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
