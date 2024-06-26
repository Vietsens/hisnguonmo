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
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using System;
using System.Configuration;

namespace HIS.Desktop.ApiConsumer
{
    public class ApiConsumers
    {
        public static void SetConsunmer(string tokenCode)
        {
            try
            {
                MosConsumer.SetTokenCode(tokenCode);
                SdaConsumer.SetTokenCode(tokenCode);
                SarConsumer.SetTokenCode(tokenCode);
                MrsConsumer.SetTokenCode(tokenCode);
                AcsConsumer.SetTokenCode(tokenCode);
                HtcConsumer.SetTokenCode(tokenCode);
                LisConsumer.SetTokenCode(tokenCode);
                ScnConsumer.SetTokenCode(tokenCode);
                HidConsumer.SetTokenCode(tokenCode);
                TytConsumer.SetTokenCode(tokenCode);
                EmrConsumer.SetTokenCode(tokenCode);
                QcsConsumer.SetTokenCode(tokenCode);
                VvaConsumer.SetTokenCode(tokenCode);
                CrmConsumer.SetTokenCode(tokenCode);

                Inventec.Common.LocalStorage.SdaConfig.ApiConsumerConfig.SdaConsumer.SetTokenCode(tokenCode);

                His.EventLog.Logger.InitConsumer(SdaConsumer);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer emrConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer EmrConsumer
        {
            get
            {
                if (emrConsumer == null)
                {
                    emrConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_EMR, GlobalVariables.APPLICATION_CODE);
                }
                return emrConsumer;
            }
            set
            {
                emrConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer mosConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer MosConsumer
        {
            get
            {
                if (mosConsumer == null)
                {
                    mosConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_MOS, GlobalVariables.APPLICATION_CODE);
                }
                return mosConsumer;
            }
            set
            {
                mosConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer sdaConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer SdaConsumer
        {
            get
            {
                if (sdaConsumer == null)
                {
                    sdaConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_SDA, GlobalVariables.APPLICATION_CODE);
                }
                return sdaConsumer;
            }
            set
            {
                sdaConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer acsConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer AcsConsumer
        {
            get
            {
                if (acsConsumer == null)
                {
                    acsConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_ACS, GlobalVariables.APPLICATION_CODE);
                }
                return acsConsumer;
            }
            set
            {
                acsConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer sarConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer SarConsumer
        {
            get
            {
                if (sarConsumer == null)
                {
                    sarConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_SAR, GlobalVariables.APPLICATION_CODE);
                }
                return sarConsumer;
            }
            set
            {
                sarConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer mrsConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer MrsConsumer
        {
            get
            {
                if (mrsConsumer == null)
                {
                    mrsConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_MRS, GlobalVariables.APPLICATION_CODE);
                }
                return mrsConsumer;
            }
            set
            {
                mrsConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer htcConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer HtcConsumer
        {
            get
            {
                if (htcConsumer == null)
                {
                    htcConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_HTC, GlobalVariables.APPLICATION_CODE);
                }
                return htcConsumer;
            }
            set
            {
                htcConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer lisConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer LisConsumer
        {
            get
            {
                if (lisConsumer == null)
                {
                    lisConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_LIS, GlobalVariables.APPLICATION_CODE);
                }
                return lisConsumer;
            }
            set
            {
                lisConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer scnConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer ScnConsumer
        {
            get
            {
                if (scnConsumer == null)
                {
                    scnConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_SCN, GlobalVariables.APPLICATION_CODE);
                }
                return scnConsumer;
            }
            set
            {
                scnConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer hidConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer HidConsumer
        {
            get
            {
                if (hidConsumer == null)
                {
                    hidConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_HID, GlobalVariables.APPLICATION_CODE);
                }
                return hidConsumer;
            }
            set
            {
                hidConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer tytConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer TytConsumer
        {
            get
            {
                if (tytConsumer == null)
                {
                    tytConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_TYT, GlobalVariables.APPLICATION_CODE);
                }
                return tytConsumer;
            }
            set
            {
                tytConsumer = value;
            }
        }

        public static Inventec.Common.WebApiClient.ApiConsumer MosConsumerNoStore
        {
            get
            {
                var tc = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData();
                return new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_MOS, (tc != null ? tc.TokenCode : ""), GlobalVariables.APPLICATION_CODE);
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumerWrapper scnWrapConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumerWrapper ScnWrapConsumer
        {
            get
            {
                if (scnWrapConsumer == null)
                {
                    scnWrapConsumer = new Inventec.Common.WebApiClient.ApiConsumerWrapper(
                        true,
                        "SCN",
                        ConfigurationSettings.AppSettings["Inventec.ScnConsumer.Base.Uri"],
                        ConfigurationSettings.AppSettings["Inventec.ScnConsumer.Acs.Uri"],
                        ConfigurationSettings.AppSettings["Inventec.ScnConsumer.LoginName"],
                        ConfigurationSettings.AppSettings["Inventec.ScnConsumer.Password"]);
                    scnWrapConsumer.UseRegistry(false);
                }
                return scnWrapConsumer;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumerWrapper hidWrapConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumerWrapper HidWrapConsumer
        {
            get
            {
                if (hidWrapConsumer == null)
                {
                    hidWrapConsumer = new Inventec.Common.WebApiClient.ApiConsumerWrapper(
                        true,
                        "HIS",
                        ConfigurationSettings.AppSettings["Inventec.HidConsumer.Base.Uri"],
                        ConfigurationSettings.AppSettings["Inventec.HidConsumer.Acs.Uri"],
                        ConfigurationSettings.AppSettings["Inventec.HidConsumer.LoginName"],
                        ConfigurationSettings.AppSettings["Inventec.HidConsumer.Password"]);
                    hidWrapConsumer.UseRegistry(false);
                }
                return hidWrapConsumer;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer rdCacheConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer RdCacheConsumer
        {
            get
            {
                if (rdCacheConsumer == null)
                {
                    rdCacheConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_RSCACHE, GlobalVariables.APPLICATION_CODE);
                }
                return rdCacheConsumer;
            }
            set
            {
                rdCacheConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer qcsConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer QcsConsumer
        {
            get
            {
                if (qcsConsumer == null)
                {
                    qcsConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_QCS, GlobalVariables.APPLICATION_CODE);
                }
                return qcsConsumer;
            }
            set
            {
                qcsConsumer = value;
            }
        }
        private static Inventec.Common.WebApiClient.ApiConsumer vvaConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer VvaConsumer
        {
            get
            {
                if (vvaConsumer == null)
                {
                    vvaConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_VVA, GlobalVariables.APPLICATION_CODE);
                }
                return vvaConsumer;
            }
            set
            {
                vvaConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer mpsConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer MpsConsumer
        {
            get
            {
                if (mpsConsumer == null)
                {
                    mpsConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_MPS, GlobalVariables.APPLICATION_CODE);
                }
                return mpsConsumer;
            }
            set
            {
                mpsConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer crmConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer CrmConsumer
        {
            get
            {
                if (crmConsumer == null)
                {
                    crmConsumer = new Inventec.Common.WebApiClient.ApiConsumer(ConfigSystems.URI_API_CRM, GlobalVariables.APPLICATION_CODE);
                }
                return crmConsumer;
            }
            set
            {
                crmConsumer = value;
            }
        }
    }
}
