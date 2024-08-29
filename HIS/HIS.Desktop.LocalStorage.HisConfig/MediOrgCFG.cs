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
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Linq;
using SDA.Filter;
using System.Collections.Generic;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.LocalStorage.HisConfig
{
    public class HisMediOrgCFG
    {
        private static System.Drawing.Image organizationLogo;
        public static System.Drawing.Image ORGANIZATION_LOGO
        {
            get
            {
                if (organizationLogo == null)
                {
                    if (!String.IsNullOrWhiteSpace(ORGANIZATION_LOGO_URL))
                    {
                        System.IO.MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(ORGANIZATION_LOGO_URL);
                        organizationLogo = stream != null ? System.Drawing.Image.FromStream(stream) : null;
                    }
                }
                return organizationLogo;
            }
            set
            {
                organizationLogo = value;
            }
        }

        private static string organizationLogoUrl;
        public static string ORGANIZATION_LOGO_URL
        {
            get
            {
                if (organizationLogoUrl == null)
                {
                    organizationLogoUrl = BranchCFG.Branch.LOGO_URL;
                }
                return organizationLogoUrl;
            }
            set
            {
                organizationLogoUrl = value;
            }
        }

        private static string organizationname;
        public static string ORGANIZATION_NAME
        {
            get
            {
                if (organizationname == null)
                {
                    organizationname = BranchCFG.Branch.BRANCH_NAME;
                }
                return organizationname;
            }
            set
            {
                organizationname = value;
            }
        }

        private static string parentorganizationname;
        public static string PARENT_ORGANIZATION_NAME
        {
            get
            {
                if (parentorganizationname == null)
                {
                    parentorganizationname = BranchCFG.Branch.PARENT_ORGANIZATION_NAME;
                }
                return parentorganizationname;
            }
            set
            {
                parentorganizationname = value;
            }
        }

        private static string organizationaddress;
        public static string ORGANIZATION_ADDRESS
        {
            get
            {
                if (organizationaddress == null)
                {
                    organizationaddress = BranchCFG.Branch.ADDRESS;
                }
                return organizationaddress;
            }
            set
            {
                organizationaddress = value;
            }
        }

        private static string mediOrgValueCurrent;
        public static string MEDI_ORG_VALUE__CURRENT
        {
            get
            {
                if (mediOrgValueCurrent == null)
                {
                    mediOrgValueCurrent = BranchCFG.Branch.HEIN_MEDI_ORG_CODE;
                }
                return mediOrgValueCurrent;
            }
            set
            {
                mediOrgValueCurrent = value;
            }
        }
        
        private static string SysMediOrgCode;
        public static string SYS_MEDI_ORG_CODE
        {
            get
            {
                if (SysMediOrgCode == null)
                {
                    SysMediOrgCode = BranchCFG.Branch.SYS_MEDI_ORG_CODE;
                }
                return SysMediOrgCode;
            }
            set
            {
                SysMediOrgCode = value;
            }
        }

        private static List<string> mediOrgCodesAccept;
        public static List<string> MEDI_ORG_CODES__ACCEPT
        {
            get
            {
                if (mediOrgCodesAccept == null || mediOrgCodesAccept.Count == 0)
                {
                    mediOrgCodesAccept = GetCodesAccept(BranchCFG.Branch.ACCEPT_HEIN_MEDI_ORG_CODE);
                }
                return mediOrgCodesAccept;
            }
            set
            {
                mediOrgCodesAccept = value;
            }
        }

        private static List<string> GetCodesAccept(string codes)
        {
            List<string> result = new List<string>();
            try
            {
                //string value = HisConfigs.Get<string>(codes);
                if (String.IsNullOrEmpty(codes)) throw new ArgumentNullException(codes);

                char[] delimiterChars = { ',' };

                string[] acceptCodes = codes.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                if (acceptCodes != null && acceptCodes.Count() > 0)
                {
                    result.AddRange(acceptCodes.Where(o => (!String.IsNullOrEmpty(o))));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
