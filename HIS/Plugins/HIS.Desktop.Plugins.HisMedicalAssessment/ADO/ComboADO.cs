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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisMedicalAssessment.ADO
{
    public class ComboADO
    {
        public long Value { get; set; }

        public string Name { get; set; }
        public ComboADO()
        {

        }
        public ComboADO(long value, string name)
        {
            this.Value = value;
            this.Name = name;
        }
        
        public static List<ComboADO> listAssType()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.GiamDinhLanDau));
                list.Add(new ComboADO(2, Resources.ResourceMessage.GiamDinhLai));
                list.Add(new ComboADO(3, Resources.ResourceMessage.GiamDinhTaiPhat));
                list.Add(new ComboADO(4, Resources.ResourceMessage.PhucQuyet));
                list.Add(new ComboADO(5, Resources.ResourceMessage.GiamDinhTaiPhat));
                list.Add(new ComboADO(6, Resources.ResourceMessage.BoSung));
                list.Add(new ComboADO(7, Resources.ResourceMessage.VetThuongConSot));
                list.Add(new ComboADO(8, Resources.ResourceMessage.TongHop));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
        public static List<ComboADO> listWelfareType()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.ThuongBinh));
                list.Add(new ComboADO(2, Resources.ResourceMessage.BenhTat));
                list.Add(new ComboADO(3, Resources.ResourceMessage.BenhNN));
                list.Add(new ComboADO(4, Resources.ResourceMessage.TaiNanLaoDong));
                list.Add(new ComboADO(5, Resources.ResourceMessage.ChatDocHoaHoc));
                list.Add(new ComboADO(6, Resources.ResourceMessage.BenhBinh));
                list.Add(new ComboADO(7, Resources.ResourceMessage.Khac));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
        public static List<ComboADO> listDisabilityType()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.VanDong));
                list.Add(new ComboADO(2, Resources.ResourceMessage.NgheNoi));
                list.Add(new ComboADO(3, Resources.ResourceMessage.Nhin));
                list.Add(new ComboADO(4, Resources.ResourceMessage.ThanKinhTamThan));
                list.Add(new ComboADO(5, Resources.ResourceMessage.TriTue));
                list.Add(new ComboADO(6, Resources.ResourceMessage.KtKhac));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
        public static List<ComboADO> listDisabilityStatus()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.ThucHienDuoc));
                list.Add(new ComboADO(2, Resources.ResourceMessage.CanTroGiup));
                list.Add(new ComboADO(3, Resources.ResourceMessage.KhongThucHienDuoc));
                list.Add(new ComboADO(4, Resources.ResourceMessage.KhongXacDinhDuoc));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
    }
}
