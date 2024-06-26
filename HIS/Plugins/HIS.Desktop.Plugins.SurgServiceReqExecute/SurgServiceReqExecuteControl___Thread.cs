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
using HIS.Desktop.Utility;
using Inventec.Common.ThreadCustom;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    public partial class SurgServiceReqExecuteControl : UserControlBase
    {
        internal void LoadDataUseThread()
        {
            try
            {
                //LoadTreatment();
                //List<Action> methods = new List<Action>();            
                //methods.Add(LoadPTTTMethod);
                //methods.Add(LoadPTTTGroup);
                //methods.Add(LoadEmotionlessMethod);
                //methods.Add(LoadBloodABO);
                //methods.Add(LoadBloodRH);
                //methods.Add(LoadPTTTCondition);
                //methods.Add(LoadPTTTCatastrophe);
                //methods.Add(LoadDeathWithin);
                //methods.Add(LoadPricePolicy);
                //methods.Add(LoadUser);
                //methods.Add(LoadExecuteRole);
                //methods.Add(LoadExecuteRoleUser);
                //methods.Add(LoadTreatment);
                //ThreadCustomManager.MultipleThreadWithJoin(methods);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPTTTMethod()
        {
            try
            {
                List<HIS_PTTT_METHOD> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPTTTGroup()
        {
            try
            {
                List<HIS_PTTT_GROUP> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadEmotionlessMethod()
        {
            try
            {
                List<HIS_EMOTIONLESS_METHOD> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBloodABO()
        {
            try
            {
                List<HIS_BLOOD_ABO> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_ABO>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBloodRH()
        {
            try
            {
                List<HIS_BLOOD_RH> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_RH>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPTTTCondition()
        {
            try
            {
                List<HIS_PTTT_CONDITION> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CONDITION>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPTTTCatastrophe()
        {
            try
            {
                List<HIS_PTTT_CATASTROPHE> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CATASTROPHE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDeathWithin()
        {
            try
            {
                List<HIS_DEATH_WITHIN> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPricePolicy()
        {
            try
            {
                List<HIS_PACKAGE> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PACKAGE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadExecuteRole()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void LoadSurgServiceType()
        //{
        //    try
        //    {
        //        List<MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE>();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void LoadMisuServiceType()
        //{
        //    try
        //    {
        //        List<MOS.EFMODEL.DataModels.V_HIS_MISU_SERVICE_TYPE> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MISU_SERVICE_TYPE>();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}
