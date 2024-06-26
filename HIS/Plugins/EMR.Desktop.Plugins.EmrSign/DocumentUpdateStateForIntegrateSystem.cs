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
using EMR.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.EmrSign
{
    internal class DocumentUpdateStateForIntegrateSystem
    {
        internal DocumentUpdateStateForIntegrateSystem() { }

        internal async Task UpdateStateIGSys(V_EMR_DOCUMENT document, string code)
        {
            try
            {
                DocumentSignedUpdateIGSysResultDTO dataSigned = new DocumentSignedUpdateIGSysResultDTO();
                dataSigned.DocumentName = document.DOCUMENT_NAME;
                dataSigned.DocumentTypeCode = document.DOCUMENT_TYPE_CODE;
                dataSigned.HisCode = document.HIS_CODE;
                dataSigned.TREATMENT_CODE = document.TREATMENT_CODE;
                dataSigned.MaLoi = code;
                dataSigned.token = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().TokenCode;

                string INTERGRATE_SYS_BASE_URI = "";
                string INTERGRATE_SYS_API = "";

                var emrConfigs = EmrConfigCFG.EmrConfigs;
                if (emrConfigs != null && emrConfigs.Count > 0)
                {
                    var config1 = emrConfigs.FirstOrDefault(o => o.KEY == "EMR.INTERGRATE_SYS_BASE_URI");
                    string vlIntegrateSysBaseUri = (config1 != null ? (!String.IsNullOrEmpty(config1.VALUE) ? config1.VALUE : config1.DEFAULT_VALUE) : "");
                    if (!String.IsNullOrEmpty(vlIntegrateSysBaseUri))
                    {
                        INTERGRATE_SYS_BASE_URI = vlIntegrateSysBaseUri;
                    }

                    var config2 = emrConfigs.FirstOrDefault(o => o.KEY == "EMR.INTERGRATE_SYS_API");
                    string cfgIntegrateSysApi = (config2 != null ? (!String.IsNullOrEmpty(config2.VALUE) ? config2.VALUE : config2.DEFAULT_VALUE) : "");
                    if (!String.IsNullOrEmpty(cfgIntegrateSysApi))
                    {
                        INTERGRATE_SYS_API = cfgIntegrateSysApi;
                    }
                }

                if (!String.IsNullOrEmpty(INTERGRATE_SYS_BASE_URI) && !String.IsNullOrEmpty(INTERGRATE_SYS_API))
                {
                    CommonParam paramIG = new CommonParam();
                    try
                    {
                        Inventec.Common.Logging.LogSystem.Info("SendSignedInfoToIGSys:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataSigned), dataSigned)
                          );
                        Inventec.Common.WebApiClient.ApiConsumer IntegrateConsumer = new Inventec.Common.WebApiClient.ApiConsumer(INTERGRATE_SYS_BASE_URI, "");
                        var rsApi = await IntegrateConsumer.PostWithouApiParamAsync<bool>(INTERGRATE_SYS_API, dataSigned, 0);

                    }
                    catch (Exception exx1)
                    {
                        LogSystem.Warn(exx1);
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => INTERGRATE_SYS_BASE_URI), INTERGRATE_SYS_BASE_URI)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => INTERGRATE_SYS_API), INTERGRATE_SYS_API));
            }
            catch (Exception exx)
            {
                LogSystem.Warn(exx);
            }
        }
    }
}
