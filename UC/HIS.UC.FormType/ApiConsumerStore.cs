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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType
{
    public class ApiConsumerStore
    {
        private static Inventec.Common.WebApiClient.ApiConsumer mosConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer MosConsumer
        {
            get
            {
                if (mosConsumer == null)
                {
                    mosConsumer = new Inventec.Common.WebApiClient.ApiConsumer(UriBaseStore.URI_API_MOS, ApplicationStore.APPLICATION_CODE__EXE);
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
                    sdaConsumer = new Inventec.Common.WebApiClient.ApiConsumer(UriBaseStore.URI_API_SDA, ApplicationStore.APPLICATION_CODE__EXE);
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
                    acsConsumer = new Inventec.Common.WebApiClient.ApiConsumer(UriBaseStore.URI_API_ACS, ApplicationStore.APPLICATION_CODE__EXE);
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
                    sarConsumer = new Inventec.Common.WebApiClient.ApiConsumer(UriBaseStore.URI_API_SAR, ApplicationStore.APPLICATION_CODE__EXE);
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
                    mrsConsumer = new Inventec.Common.WebApiClient.ApiConsumer(UriBaseStore.URI_API_MRS, ApplicationStore.APPLICATION_CODE__EXE);
                }
                return mrsConsumer;
            }
            set
            {
                mrsConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer tosConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer TosConsumer
        {
            get
            {
                if (tosConsumer == null)
                {
                    tosConsumer = new Inventec.Common.WebApiClient.ApiConsumer(UriBaseStore.URI_API_TOS, ApplicationStore.APPLICATION_CODE__EXE);
                }
                return tosConsumer;
            }
            set
            {
                tosConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer pacConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer PacConsumer
        {
            get
            {
                if (pacConsumer == null)
                {
                    pacConsumer = new Inventec.Common.WebApiClient.ApiConsumer(UriBaseStore.URI_API_PAC, ApplicationStore.APPLICATION_CODE__EXE);
                }
                return pacConsumer;
            }
            set
            {
                pacConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer theConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer TheConsumer
        {
            get
            {
                if (theConsumer == null)
                {
                    theConsumer = new Inventec.Common.WebApiClient.ApiConsumer(UriBaseStore.URI_API_THE, ApplicationStore.APPLICATION_CODE__EXE);
                }
                return theConsumer;
            }
            set
            {
                theConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer aosConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer AosConsumer
        {
            get
            {
                if (aosConsumer == null)
                {
                    aosConsumer = new Inventec.Common.WebApiClient.ApiConsumer(UriBaseStore.URI_API_AOS, ApplicationStore.APPLICATION_CODE__EXE);
                }
                return aosConsumer;
            }
            set
            {
                aosConsumer = value;
            }
        }
    }
}
