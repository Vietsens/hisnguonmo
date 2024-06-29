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

using HIS.UC.ExamFinish.Run;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using HIS.UC.ExamFinish.GetValue;
using MOS.EFMODEL.DataModels;
using HIS.UC.ExamFinish.ADO;
using HIS.UC.ExamFinish.Reload;

namespace HIS.UC.ExamFinish
{
    public class ExamFinishProcessor : BussinessBase
    {
        object uc;
        public ExamFinishProcessor()
            : base()
        {
        }

        public ExamFinishProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(ExamFinishInitADO arg)
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

        public void ReLoad(UserControl control,ExamFinishInitADO data)
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
    }
}
