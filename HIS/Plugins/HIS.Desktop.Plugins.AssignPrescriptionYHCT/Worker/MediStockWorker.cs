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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT
{
    class MediStockWorker
    {
        /// <summary>
        /// Kiểm tra cấu hình có hiển thị thuốc hay không trong bảng MEDI_STOCK_METY, nếu IS_PREVENT_EXP = true thì không hiển thị loại thuốc ứng với kho xuất trong danh sách thuốc.
        /// </summary>
        /// <param name="mediStockIds"></param>
        internal static void FilterByMediStockMeti(List<long> mediStockIds, ref List<DMediStock1ADO> mediStockD1SDOs)
        {
            try
            {
                if (mediStockIds == null || mediStockIds.Count == 0) throw new ArgumentNullException("mediStockIds");

                var mediStockMetis = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_METY>();
                if (mediStockMetis != null && mediStockMetis.Count > 0)
                {
                    var mediStockMetis__IdNotAllows = mediStockMetis.Where(o => mediStockIds.Contains(o.MEDI_STOCK_ID) && o.IS_PREVENT_EXP == 1).Select(o => o.MEDICINE_TYPE_ID).Distinct().ToList();
                    if (mediStockMetis__IdNotAllows != null && mediStockMetis__IdNotAllows.Count > 0)
                    {
                        mediStockD1SDOs = mediStockD1SDOs
                            .Where(o =>
                                o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && !mediStockMetis__IdNotAllows.Contains(o.ID ?? 0))
                                ).ToList();

                        LogSystem.Debug("Load du lieu kho theo du lieu theo dieu kien MEDI_STOCK_METY.IS_PREVENT_EXP nếu = true thì không hiển thị loại thuốc ứng với kho xuất trong danh sách thuốc.____ " + "____ket qua tim thay " + (mediStockD1SDOs != null ? mediStockD1SDOs.Count : 0));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Thuốc trong kho cấu hình chỉ có khoa nào được phép sử dụng
        /// Sử dụng bảng HIS_MEST_METY_DEPA.
        /// Căn cứ theo khoa yêu cầu, kho được chọn --> không hiển thị các loại thuốc bị khóa.
        /// Nếu không tồn tại dữ liệu medi_stock_id & department_id nào trong bảng này tức là khoa yêu cầu không bị kho đó khóa thuốc nào cả.
        /// </summary>
        /// <param name="mediStockIds"></param>
        internal static void FilterByMestMetyDepa(List<long> mediStockIds, MOS.SDO.WorkPlaceSDO currentWorkPlace, ref List<DMediStock1ADO> mediStockD1SDOs)
        {
            try
            {
                if (mediStockIds == null || mediStockIds.Count == 0) throw new ArgumentNullException("mediStockIds");

                var mestMetyDepas = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEST_METY_DEPA>();
                if (mestMetyDepas != null && mestMetyDepas.Count > 0)
                {
                    var mestMetyDepas__IdNotAllows = mestMetyDepas
                        .Where(o =>
                            o.DEPARTMENT_ID == currentWorkPlace.DepartmentId
                            && mediStockIds.Contains(o.MEDI_STOCK_ID ?? 0))
                        .Select(o => o.MEDICINE_TYPE_ID).Distinct().ToList();
                    if (mestMetyDepas__IdNotAllows != null && mestMetyDepas__IdNotAllows.Count > 0)
                    {
                        mediStockD1SDOs = mediStockD1SDOs
                            .Where(o =>
                                o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && !mestMetyDepas__IdNotAllows.Contains(o.ID ?? 0))
                                ).ToList();
                    }

                    LogSystem.Debug("Load du lieu kho theo du lieu theo dieu kien Thuốc trong kho cấu hình chỉ có khoa nao được phep su dung.____ " + "____ket qua tim thay " + (mediStockD1SDOs != null ? mediStockD1SDOs.Count : 0));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
