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
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.EmrPatientCertificateRegister
{
    class EmrPatientCertificateRegisterBehavior : Tool<IDesktopToolContext>, IEmrPatientCertificateRegister
    {
        object[] entity;

        internal EmrPatientCertificateRegisterBehavior()
            : base()
        { }

        internal EmrPatientCertificateRegisterBehavior(Inventec.Core.CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IEmrPatientCertificateRegister.Run()
        {
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            long documentId = 0;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        if (item is long)
                        {
                            documentId = (long)item;
                        }
                    }
                }

                if (moduleData != null)
                {
                    // EMR.HSM.CMC.INTEGRATE_OPTION: 0/khong khai bao => man cap chung thu 2ID; khac 0 => man cap chung thu CMC
                    if (IsCmcPlatform())
                    {
                        return new frmEmrPatientCertificateRegisterCmc(moduleData);
                    }
                    return new frmEmrPatientCertificateRegister(moduleData, documentId);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("documentId: " + documentId);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private const string CFG_HSM_CMC_INTEGRATE_OPTION = "EMR.HSM.CMC.INTEGRATE_OPTION";

        /// <summary>Doc giong BE (EmrSignCFG.IS_CMC_PLATFORM): so khac 0 => nen tang CMC (CSign + HubCA); 0, de trong hoac khong phai so => 2ID</summary>
        private static bool IsCmcPlatform()
        {
            try
            {
                // Tai lai cau hinh EMR truoc khi doc: cache may tram co the nap tu truoc khi key duoc them/doi tren server
                HIS.Desktop.LocalStorage.EmrConfig.ConfigLoader.Refresh();
                string value = HIS.Desktop.LocalStorage.EmrConfig.EmrConfigs.Get<string>(CFG_HSM_CMC_INTEGRATE_OPTION);
                int option;
                return Int32.TryParse(value, out option) && option != 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }
    }
}
