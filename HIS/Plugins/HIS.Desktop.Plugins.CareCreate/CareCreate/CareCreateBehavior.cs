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
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.CareCreate;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.HisConfig;
using MOS.Filter;

namespace HIS.Desktop.Plugins.CareCreate
{
    public sealed class CareCreateBehavior : Tool<IDesktopToolContext>, ICareCreate
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        HIS_CARE currentCare;
        long treatmentId;
        HIS_CARE_SUM currentcareSum;
        HIS_TRACKING currentTracking;
        V_HIS_TREATMENT_4 treatment4;
        DelegateSelectData delegateSelectData;
        HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter;

        public CareCreateBehavior()
            : base()
        {
        }

        public CareCreateBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ICareCreate.Run()
        {
            long UsingFormVersion = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(SdaConfigKeys.USING_FORM_VERSION));
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is HIS_CARE)
                        {
                            currentCare = (HIS_CARE)item;
                        }
                        else if (item is long)
                        {
                            treatmentId = (long)item;
                        }
                        else if (item is HIS_CARE_SUM)
                        {
                            currentcareSum = (HIS_CARE_SUM)item;
                        }
                        else if (item is HIS_TRACKING)
                        {
                            currentTracking = (HIS_TRACKING)item;
                        } 
                        else if (item is V_HIS_TREATMENT_4)
                        {
                            treatment4 = (V_HIS_TREATMENT_4)item;
                        }
                        else if (item is DelegateSelectData)
                        {
                            delegateSelectData = (DelegateSelectData)item;
                        }
                        else if (item is HisTreatmentBedRoomLViewFilter)
                        {
                            dataTransferTreatmentBedRoomFilter = (HisTreatmentBedRoomLViewFilter)item;
                        }

                        if (UsingFormVersion == 2)
                        {
                            if (currentModule != null && currentCare != null)
                            {
                                result = new frmHisCareCreate(currentModule, currentCare, delegateSelectData, dataTransferTreatmentBedRoomFilter);
                                break;
                            }
                            else if (currentModule != null && treatmentId > 0)
                            {
                                result = new frmHisCareCreate(currentModule, treatmentId, delegateSelectData, dataTransferTreatmentBedRoomFilter);
                                break;
                            }
                            else if (currentModule != null && currentTracking != null)
                            {
                                result = new frmHisCareCreate(currentModule, currentTracking, delegateSelectData, dataTransferTreatmentBedRoomFilter);
                                break;
                            }
                            else if (currentModule != null && currentcareSum != null)
                            {
                                result = new frmHisCareCreate(currentModule, currentcareSum, delegateSelectData, dataTransferTreatmentBedRoomFilter);
                                break;
                            }
                            else if (currentModule != null && treatment4 != null)
                            {
                                result = new frmHisCareCreate(currentModule, treatment4, delegateSelectData, dataTransferTreatmentBedRoomFilter);
                                break;
                            }
                        }
                        else
                        {
                            if (currentModule != null && currentCare != null)
                            {
                                result = new CareCreate(currentModule, currentCare, delegateSelectData, dataTransferTreatmentBedRoomFilter);
                                break;
                            }
                            else if (currentModule != null && treatmentId > 0)
                            {
                                result = new CareCreate(currentModule, treatmentId, delegateSelectData, dataTransferTreatmentBedRoomFilter);
                                break;
                            }
                            else if (currentModule != null && currentTracking != null)
                            {
                                result = new CareCreate(currentModule, currentTracking, delegateSelectData, dataTransferTreatmentBedRoomFilter);
                                break;
                            }
                            else if (currentModule != null && currentcareSum != null)
                            {
                                result = new CareCreate(currentModule, currentcareSum, delegateSelectData, dataTransferTreatmentBedRoomFilter);
                                break;
                            }
                            else if (currentModule != null && treatment4 != null)
                            {
                                result = new CareCreate(currentModule, treatment4, delegateSelectData, dataTransferTreatmentBedRoomFilter);
                                break;
                            }
                        }
                    }
                }
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
