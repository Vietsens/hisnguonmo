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
using His.UC.UCHein.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.Init
{
    class InitFactory
    {
        internal static IInitAsync MakeIInit(CommonParam param, object data)
        {
            IInitAsync result = null;
            try
            {

                if (data.GetType() == typeof(DataInitHeinBhyt))
                {
                    if (((DataInitHeinBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__BHYT1)
                    {
                        result = new InitBhytBehavior(param, ((DataInitHeinBhyt)data));
                    }
                }
                //else if (data.GetType() == typeof(DataInitKskContract))
                //{
                //    if (((DataInitKskContract)data).Template == MainHisHeinBhyt.TEMPLATE__KSK_CONTRACT1)
                //    {
                //        result = new InitKskContractBehavior(param, ((DataInitKskContract)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitAiaBhyt))
                //{
                //    if (((DataInitAiaBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__AIA1)
                //    {
                //        result = new InitAiaBehavior(param, ((DataInitAiaBhyt)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitBcvBhyt))
                //{
                //    if (((DataInitBcvBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__BCV1)
                //    {
                //        result = new InitBCVBehavior(param, ((DataInitBcvBhyt)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitBicBhyt))
                //{
                //    if (((DataInitBicBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__BIC1)
                //    {
                //        result = new InitBICBehavior(param, ((DataInitBicBhyt)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitBvtmBhyt))
                //{
                //    if (((DataInitBvtmBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__BVTM1)
                //    {
                //        result = new InitBVTMBehavior(param, ((DataInitBvtmBhyt)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitDaiIChiBhyt))
                //{
                //    if (((DataInitDaiIChiBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__DAIICHI1)
                //    {
                //        result = new InitDAIICHIBehavior(param, ((DataInitDaiIChiBhyt)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitGelBhyt))
                //{
                //    if (((DataInitGelBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__GEL1)
                //    {
                //        result = new InitGELBehavior(param, ((DataInitGelBhyt)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitGeneraliBhyt))
                //{
                //    if (((DataInitGeneraliBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__GENERALI1)
                //    {
                //        result = new InitGENERALIBehavior(param, ((DataInitGeneraliBhyt)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitInsMartBhyt))
                //{
                //    if (((DataInitInsMartBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__INSMART1)
                //    {
                //        result = new InitINSMARTBehavior(param, ((DataInitInsMartBhyt)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitLibertyBhyt))
                //{
                //    if (((DataInitLibertyBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__LIBERTY1)
                //    {
                //        result = new InitLIBERTYBehavior(param, ((DataInitLibertyBhyt)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitPRUDBhyt))
                //{
                //    if (((DataInitPRUDBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__PRUD1)
                //    {
                //        result = new InitPRUDBehavior(param, ((DataInitPRUDBhyt)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitPTIBhyt))
                //{
                //    if (((DataInitPTIBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__PTI1)
                //    {
                //        result = new InitPTIBehavior(param, ((DataInitPTIBhyt)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitPVIBhyt))
                //{
                //    if (((DataInitPVIBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__PVI1)
                //    {
                //        result = new InitPVIBehavior(param, ((DataInitPVIBhyt)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitTMNBhyt))
                //{
                //    if (((DataInitTMNBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__TMN1)
                //    {
                //        result = new InitTMNBehavior(param, ((DataInitTMNBhyt)data));
                //    }
                //}
                //else if (data.GetType() == typeof(DataInitVCLIBhyt))
                //{
                //    if (((DataInitVCLIBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__VCLI1)
                //    {
                //        result = new InitVCLIBehavior(param, ((DataInitVCLIBhyt)data));
                //    }
                //}
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

        internal static IInit MakeIInitUC(CommonParam param, object data)
        {
            IInit result = null;
            try
            {

                if (data.GetType() == typeof(DataInitHeinBhyt))
                {
                    if (((DataInitHeinBhyt)data).Template == MainHisHeinBhyt.TEMPLATE__BHYT1)
                    {
                        result = new InitBhytUCBehavior(param, ((DataInitHeinBhyt)data));
                    }
                }
                //else if (data.GetType() == typeof(DataInitKskContract))
                //{
                //    if (((DataInitKskContract)data).Template == MainHisHeinBhyt.TEMPLATE__KSK_CONTRACT1)
                //    {
                //        result = new InitKskContractUCBehavior(param, ((DataInitKskContract)data));
                //    }
                //}
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
