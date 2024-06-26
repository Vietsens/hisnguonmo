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
using SAR.Desktop.Plugins.SarPrintType;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EmrDocumentType.ADO
{
  public class AcsUserADO : ACS.EFMODEL.DataModels.ACS_USER
  {
    public bool CHECKEDIT { get; set; }
    public string userNames { get; set; }
    public AcsUserADO()
    {
    }
    public AcsUserADO(SDA_GROUP data)
    {
      if (data != null)
      {
        //Inventec.Common.Mapper.
        Inventec.Common.Mapper.DataObjectMapper.Map<AcsUserADO>(this, data);
      }
    }
    public DelegateRefeshAcsUsers DelegateRefeshAcsUsers { get; set; }
    public AcsUserADO(ACS.EFMODEL.DataModels.ACS_USER user, string[] users)
        {
            try
            {
                if (user != null)
                {
                    this.LOGINNAME = user.LOGINNAME;
                    this.USERNAME = user.USERNAME;

                    if (users != null && users.Count() > 0 && users.Contains(this.LOGINNAME))
                    {
                        this.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    public AcsUserADO(DelegateRefeshAcsUsers refeshAcsUsers, string userNames)            
        {
            this.DelegateRefeshAcsUsers = refeshAcsUsers;
            this.userNames = userNames;
            
        }
        public bool IsChecked { get; set; }
  }
}
