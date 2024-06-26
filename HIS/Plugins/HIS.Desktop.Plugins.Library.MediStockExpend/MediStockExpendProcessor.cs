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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.MediStockExpend
{
    public class MediStockExpendProcessor
    {
        private static long? MediStockId { get; set; }
        private const string moduleLink = "HIS.Desktop.Plugins.Library.MediStockExpend";
        private const string controlName = "cboMediStock";

        public static long? GetMediStock(long roomId, bool isShow)
        {
            long? result = null;
            try
            {
                var ListMestRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == roomId).ToList();

                if (!MediStockId.HasValue)
                {
                    var controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                    var currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                    if (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                    {
                        foreach (var item in currentControlStateRDO)
                        {
                            if (item.KEY == controlName)
                            {
                                long mediStockId = 0;
                                if (long.TryParse(item.VALUE, out mediStockId) && mediStockId > 0)
                                    MediStockId = mediStockId;
                            }
                        }
                    }
                }

                if (isShow || !MediStockId.HasValue || !ListMestRoom.Exists(o => o.MEDI_STOCK_ID == MediStockId))
                {
                    //phòng mới không có thiết lập thì xóa thông tin đang lưu
                    if (!ListMestRoom.Exists(o => o.MEDI_STOCK_ID == MediStockId))
                    {
                        MediStockId = null;
                    }

                    FormMediStockExpend form = new FormMediStockExpend(ListMestRoom, MediStockId, SaveMediStock);
                    form.ShowDialog();
                }

                result = MediStockId;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private static void SaveMediStock(long mediStockId)
        {
            try
            {
                MediStockId = mediStockId;
                var controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                var currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == controlName && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = mediStockId.ToString();
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = controlName;
                    csAddOrUpdate.VALUE = mediStockId.ToString();
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (currentControlStateRDO == null)
                        currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDO.Add(csAddOrUpdate);
                }

                controlStateWorker.SetData(currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
