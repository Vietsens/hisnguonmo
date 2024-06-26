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
using HIS.Desktop.ADO;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.TransDepartment.TransDepartment;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HIS.Desktop.Plugins.TransDepartment.TransDepartment
{
    class TransDepartmentFactory
    {
        internal static ITransDepartment MakeITransDepartment(CommonParam param, object[] data)
        {

            RefeshReference RefeshReference = null;
            DelegateReturnSuccess DelegateReturnSuccess = null;
            ITransDepartment result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            TransDepartmentADO TransDepartmentADO = null;
            V_HIS_DEPARTMENT_TRAN departmentTran = null;
            bool isView = false;
            try
            {
                if (data.GetType() == typeof(object[]))
                {
                    if (data != null && data.Count() > 0)
                    {
                        for (int i = 0; i < data.Count(); i++)
                        {
                            if (data[i] is TransDepartmentADO)
                            {
                                TransDepartmentADO = (TransDepartmentADO)data[i];
                            }
                            else if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                            }
                            else if (data[i] is RefeshReference)
                            {
                                RefeshReference = (RefeshReference)data[i];
                            }
                            else if (data[i] is DelegateReturnSuccess)
                            {
                                DelegateReturnSuccess = (DelegateReturnSuccess)data[i];
                            }
                            else if (data[i] is V_HIS_DEPARTMENT_TRAN)
                            {
                                departmentTran = (V_HIS_DEPARTMENT_TRAN)data[i];
                            }
                            else if (data[i] is bool)
                            {
                                isView = (bool)data[i];
                            }

                        }

                        if (moduleData != null && TransDepartmentADO != null && departmentTran != null)
                        {
                            result = new TransDepartmentBehavior(param, moduleData, (TransDepartmentADO)TransDepartmentADO, RefeshReference, DelegateReturnSuccess, departmentTran, isView);
                        }
                        else if (moduleData != null && TransDepartmentADO != null && departmentTran == null)
                        {
                            result = new TransDepartmentBehavior(param, moduleData, (TransDepartmentADO)TransDepartmentADO, RefeshReference, DelegateReturnSuccess);
                        }
                        else
                        {
                            result = null;
                        }
                    }
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
