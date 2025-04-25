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
using ACS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DebateDiagnostic.UcDebateDetail
{
    class DetailProcessor
    {
        UcPttt Pttt;
        UcOther Thuoc;
        UcOther Khac;
        long TreatmentId;
        long RoomId;
        long RoomTypeId;
        HIS_SERVICE hisService;
        bool IsSetPttt;
        bool IsSetThuoc;
        bool IsSetKhac;
        //qtcode
        Inventec.Desktop.Common.Modules.Module module;
        //qtcode
        public List<ACS_USER> UserList;
        public List<V_HIS_EMPLOYEE> EmployeeList;
        public List<HIS_DEPARTMENT> DepartmentList;
        public List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> ExecuteRoleList;

        //public DetailProcessor(long treatmentId, long roomId, long roomTypeId)
        //qtcode
        public DetailProcessor(long treatmentId, long roomId, long roomTypeId, Inventec.Desktop.Common.Modules.Module module)
        {
            this.TreatmentId = treatmentId;
            this.RoomId = roomId;
            this.RoomTypeId = roomTypeId;
            this.module = module; 
        }
        //public DetailProcessor(long treatmentId, long roomId, long roomTypeId, HIS_SERVICE hisService)
        public DetailProcessor(long treatmentId, long roomId, long roomTypeId, HIS_SERVICE hisServicee, Inventec.Desktop.Common.Modules.Module module)
        {
            this.TreatmentId = treatmentId;
            this.RoomId = roomId;
            this.RoomTypeId = roomTypeId;
            this.hisService = hisService;


            this.module = module;
        }

        public UserControl GetControl(DetailEnum type)
        {
            UserControl result = null;
            try
            {
                switch (type)
                {
                    case DetailEnum.Thuoc:
                        if (Thuoc == null)
                        {
                            Thuoc = new UcOther(TreatmentId, RoomId, RoomTypeId, false, module);
                        }
                        result = Thuoc;
                        break;
                    case DetailEnum.Pttt:
                        if (Pttt == null)
                        {
                            Pttt = new UcPttt(TreatmentId, RoomId, RoomTypeId, UserList, EmployeeList, DepartmentList, ExecuteRoleList);
                        }
                        result = Pttt;
                        break;
                    case DetailEnum.Khac:
                        if (Khac == null)
                        {
                            if (hisService != null)
                            {
                                Khac = new UcOther(TreatmentId, RoomId, RoomTypeId, true, hisService, module);
                            }
                            else
                            {
                                Khac = new UcOther(TreatmentId, RoomId, RoomTypeId, true, module);
                            }

                        }
                        else
                        {
                            if (hisService != null)
                            {
                                Khac = new UcOther(TreatmentId, RoomId, RoomTypeId, true, hisService, module);
                            }
                        }
                        result = Khac;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void GetData(DetailEnum type, ref HIS_DEBATE saveData)
        {
            try
            {
                switch (type)
                {
                    case DetailEnum.Thuoc:
                        if (Thuoc == null) return;
                        Thuoc.GetData(ref saveData);
                        break;
                    case DetailEnum.Pttt:
                        if (Pttt == null) return;
                        Pttt.GetData(ref saveData);
                        break;
                    case DetailEnum.Khac:
                        if (Khac == null) return;
                        Khac.GetData(ref saveData);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetDataDiscussion(DetailEnum type, string content)
		{
            try
            {
                switch (type)
                {
                    case DetailEnum.Thuoc:
                        if (Thuoc == null) return;
                        Thuoc.SetContent(content);
                        break;              
                    case DetailEnum.Khac:
                        if (Khac == null) return;
                        Khac.SetContent(content);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool ValidateControl(DetailEnum type)
        {
            bool result = false;
            try
            {
                switch (type)
                {
                    case DetailEnum.Thuoc:
                        if (Thuoc == null) return false;
                        result = Thuoc.ValidControl();
                        break;
                    case DetailEnum.Pttt:
                        if (Pttt == null) return false;
                        result = Pttt.ValidControl();
                        break;
                    case DetailEnum.Khac:
                        if (Khac == null) return false;
                        result = Khac.ValidControl();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void DisableControlItem(DetailEnum type)
        {
            try
            {
                switch (type)
                {
                    case DetailEnum.Thuoc:
                        if (Thuoc == null) return;
                        Thuoc.DisableControlItem();
                        break;
                    case DetailEnum.Pttt:
                        if (Pttt == null) return;
                        Pttt.DisableControlItem();
                        break;
                    case DetailEnum.Khac:
                        if (Khac == null) return;
                        Khac.DisableControlItem();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetData(DetailEnum type, object data)
        {
            try
            {
                switch (type)
                {
                    case DetailEnum.Thuoc:
                        if (Thuoc == null) return;
                        Thuoc.SetData(data);
                        break;
                    case DetailEnum.Pttt:
                        if (Pttt == null) return;
                        Pttt.SetData(data);
                        break;
                    case DetailEnum.Khac:
                        if (Khac == null) return;
                        Khac.SetData(data);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetDataMedicine(HIS_DEBATE data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetDataMedicine.1");

                if (Thuoc == null) return;
                Thuoc.SetDataMedicine(data);
                Inventec.Common.Logging.LogSystem.Debug("SetDataMedicine.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
