﻿/* IVT
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
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.HisPatientBankAccount.HisPatientBankAccount;
using Inventec.Core;
using Inventec.Desktop.Common;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HIS.Desktop.Plugins.HisPatientBankAccount.HisPatientBankAccount
{
    class HisPatientBankAccountBehavior : BusinessBase, IHisPatientBankAccount
    {
        object[] entity;
        HIS_TREATMENT _currentTreatment = new HIS_TREATMENT();
        internal HisPatientBankAccountBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisPatientBankAccount.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                long treatmentID = 0;
                HIS_TREATMENT obj = null;
                V_HIS_PATIENT_BANK_ACCOUNT ptba = null;
                DelegateSelectData dlg = null;
                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                            else if (entity[i] is long)
                            {
                                treatmentID = (long)entity[i];
                            }
                            else if (entity[i] is HIS_TREATMENT)
                            {
                                obj = (HIS_TREATMENT)entity[i];
                            }
                            else if (entity[i] is DelegateSelectData)
                            {
                                dlg = (DelegateSelectData)entity[i];
                            }
                            else if (entity[i] is V_HIS_PATIENT_BANK_ACCOUNT)
                            {
                                ptba = (V_HIS_PATIENT_BANK_ACCOUNT)entity[i];
                            }
                        }
                    }
                }

                return new frmHisPatientBankAccount(moduleData, obj, dlg, ptba) ;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
