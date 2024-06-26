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

using HIS.UC.Hospitalize.Run;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using HIS.UC.Hospitalize.GetValue;
using MOS.EFMODEL.DataModels;
using HIS.UC.Hospitalize.ADO;
using HIS.UC.Hospitalize.Reload;
using HIS.UC.Hospitalize.GetIcd;

namespace HIS.UC.Hospitalize
{
    public class HospitalizeProcessor : BussinessBase
    {
        object uc;
        public HospitalizeProcessor()
            : base()
        {
        }

        public HospitalizeProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(HospitalizeInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIHospitalize(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public object GetValue(UserControl control)
        {
            object result = null;
            try
            {
                IGetValue behavior = GetValueFactory.MakeIGetValue(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public object GetIcd(UserControl control)
        {
            object result = null;
            try
            {
                IGetIcd behavior = GetIcdFactory.MakeIGetIcd(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public void ReLoad(UserControl control,HospitalizeInitADO data)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control),data);
                if (behavior != null)  behavior.Run() ;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public bool ValidCheckIcd(UserControl control)
        {
            bool valid = false;
            try
            {
                ValidCheckIcd.IValidCheckIcd behavior = Hospitalize.ValidCheckIcd.ValidCheckIcdFactory.MakeIValidCheckIcd(param, (control == null ? (UserControl)uc : control));
                valid = (behavior != null) ? behavior.Run() : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
