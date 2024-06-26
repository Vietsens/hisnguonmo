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
using LIS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.LisDeliveryNoteDetail.ADO
{
    public class SampleADO : V_LIS_SAMPLE
    {
        public bool IsChecked { get; set; }
        public string DOB_ForDisplay { get; set; }
        public string PersonalIDNumber_ForDisplay { get; set; }
        public string SAMPLE_TIME_ForDisplay { get; set; }
        public string SAMPLE_LOGINNAME_USERNAME_ForDisplay { get; set; }

        public SampleADO()
        { }

        public SampleADO(V_LIS_SAMPLE data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SampleADO>(this, data);
                    this.IsChecked = false;
                    if (data.DOB != null)
                    {
                        DateTime dateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.DOB ?? 0) ?? new DateTime();
                        if (IS_HAS_NOT_DAY_DOB == 1)
                        {
                            this.DOB_ForDisplay = dateTime.Year.ToString();
                        }
                        else
                        {
                            this.DOB_ForDisplay = dateTime.ToString("dd/MM/yyyy");
                        }
                    }

                    if (data.CMND_NUMBER != null && data.CMND_NUMBER.Count()>0)
                    {
                        this.PersonalIDNumber_ForDisplay = data.CMND_NUMBER;
                    }
                    else if (data.CCCD_NUMBER != null && data.CCCD_NUMBER.Count() > 0)
                    {
                        this.PersonalIDNumber_ForDisplay = data.CCCD_NUMBER;
                    }
                    else if (data.PASSPORT_NUMBER != null && data.PASSPORT_NUMBER.Count() > 0)
                    {
                        this.PersonalIDNumber_ForDisplay = data.PASSPORT_NUMBER;
                    }

                    this.SAMPLE_LOGINNAME_USERNAME_ForDisplay = "";
                    if (data.SAMPLE_LOGINNAME != null)
                    {
                        this.SAMPLE_LOGINNAME_USERNAME_ForDisplay += data.SAMPLE_LOGINNAME;
                    }
                    if (data.SAMPLE_USERNAME != null)
                    {
                        if (this.SAMPLE_LOGINNAME_USERNAME_ForDisplay.Length > 0)
                            this.SAMPLE_LOGINNAME_USERNAME_ForDisplay += " - ";
                        this.SAMPLE_LOGINNAME_USERNAME_ForDisplay += data.SAMPLE_USERNAME;
                    }

                    if (data.SAMPLE_TIME != null)
                    {
                        DateTime dateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.SAMPLE_TIME ?? 0) ?? new DateTime();
                        this.SAMPLE_TIME_ForDisplay = dateTime.ToString("dd/MM/yyyy HH:mm");
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
