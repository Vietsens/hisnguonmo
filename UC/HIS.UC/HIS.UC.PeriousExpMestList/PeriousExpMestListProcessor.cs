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
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.UC.PeriousExpMestList.ADO;
using HIS.UC.PeriousExpMestList.Run;
using System.Windows.Forms;
using HIS.UC.PeriousExpMestList.Reload;
using HIS.UC.PeriousExpMestList.GetFocusRow;
using MOS.EFMODEL.DataModels;
using HIS.UC.PeriousExpMestList.FLoad;
using HIS.UC.PeriousExpMestList.GetServiceReqData;
using HIS.UC.PeriousExpMestList.GetPreServiceReqADOData;
using HIS.UC.PeriousExpMestList.Dispose;
using HIS.UC.PeriousExpMestList.GetIntructionTime;

namespace HIS.UC.PeriousExpMestList
{
    public class PeriousExpMestListProcessor : BussinessBase
    {
        object uc;
        public PeriousExpMestListProcessor()
            : base()
        {
        }

        public PeriousExpMestListProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(PeriousExpMestInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIDepositRequestList(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public void Load(UserControl control)
        {
            try
            {
                IFLoad behavior = FLoadFactory.MakeIFLoad(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(UserControl control, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7> expMest)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), expMest);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void DisposeControl(UserControl control)
        {
            try
            {
                IDispose behavior = DisposeFactory.MakeIDispose(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public PreServiceReqADO GetFocusRow(UserControl control)
        {
            PreServiceReqADO result = null;
            try
            {
                IGetFocusRow behavior = GetFocusRowFactory.MakeIGetFocusRow(control);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        public IntructionTimeADO GetIntructionTime(UserControl control)
        {
            IntructionTimeADO result = null;
            try
            {
                IGetIntructionTime behavior = GetIntructionTimeFactory.MakeIGetIntructionTime(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7> GetServiceReqData(UserControl control)
        {
            List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7> result = null;
            try
            {
                IGetServiceReqData behavior = GetServiceReqDataFactory.MakeIGetServiceReqData(control);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        public List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7> GetServiceReqDataAll(UserControl control)
        {
            List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7> result = null;
            try
            {
                IGetServiceReqDataAll behavior = GetServiceReqDataFactory.MakeIGetServiceReqDataAll(control);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        public List<PreServiceReqADO> GetPreServiceReqADOData(UserControl control)
        {
            List<PreServiceReqADO> result = null;
            try
            {
                IGetPreServiceReqADOData behavior = GetPreServiceReqADODataFactory.MakeIGetPreServiceReqADOData(control);
                result = (behavior != null) ? behavior.Run() : null;
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
