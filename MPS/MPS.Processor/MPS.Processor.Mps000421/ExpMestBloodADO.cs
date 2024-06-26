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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000421
{
    public class ExpMestBloodADO : V_HIS_EXP_MEST_BLOOD
    {
        public string AC_SELF_ENVIDENCE_STR { get; set; }
        public string AC_SELF_ENVIDENCE_SECOND_STR { get; set; }
        public string ANTI_GLOBULIN_ENVI_STR { get; set; }
        public string ANTI_GLOBULIN_ENVI_TWO_STR { get; set; }
        public string SALT_ENVI_STR { get; set; }
        public string SALT_ENVI_TWO_STR { get; set; }
        public ExpMestBloodADO(V_HIS_EXP_MEST_BLOOD data)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }
                if (data.AC_SELF_ENVIDENCE != null)
                {
                    if (data.AC_SELF_ENVIDENCE == (decimal)0.0)
                    {
                        this.AC_SELF_ENVIDENCE_STR = "Âm tính";
                    }
                    else if(data.AC_SELF_ENVIDENCE == (decimal)0.5)
                    {
                        this.AC_SELF_ENVIDENCE_STR = "0.5";
                    }
                    else if (data.AC_SELF_ENVIDENCE == (decimal)1)
                    {
                        this.AC_SELF_ENVIDENCE_STR = "1+";
                    }
                    else if (data.AC_SELF_ENVIDENCE == (decimal)2)
                    {
                        this.AC_SELF_ENVIDENCE_STR = "2+";
                    }
                    else if (data.AC_SELF_ENVIDENCE == (decimal)3)
                    {
                        this.AC_SELF_ENVIDENCE_STR = "3+";
                    }
                    else if (data.AC_SELF_ENVIDENCE == (decimal)4)
                    {
                        this.AC_SELF_ENVIDENCE_STR = "4+";
                    }
                    else if (data.AC_SELF_ENVIDENCE == (decimal)5)
                    {
                        this.AC_SELF_ENVIDENCE_STR = "5+";
                    }                  
                }
                if (data.AC_SELF_ENVIDENCE_SECOND != null)
                {
                    if (data.AC_SELF_ENVIDENCE_SECOND == (decimal)0.0)
                    {
                        this.AC_SELF_ENVIDENCE_SECOND_STR = "Âm tính";
                    }
                    else if (data.AC_SELF_ENVIDENCE_SECOND == (decimal)0.5)
                    {
                        this.AC_SELF_ENVIDENCE_SECOND_STR = "0.5";
                    }
                    else if (data.AC_SELF_ENVIDENCE_SECOND == (decimal)1)
                    {
                        this.AC_SELF_ENVIDENCE_SECOND_STR = "1+";
                    }
                    else if (data.AC_SELF_ENVIDENCE_SECOND == (decimal)2)
                    {
                        this.AC_SELF_ENVIDENCE_SECOND_STR = "2+";
                    }
                    else if (data.AC_SELF_ENVIDENCE_SECOND == (decimal)3)
                    {
                        this.AC_SELF_ENVIDENCE_SECOND_STR = "3+";
                    }
                    else if (data.AC_SELF_ENVIDENCE_SECOND == (decimal)4)
                    {
                        this.AC_SELF_ENVIDENCE_SECOND_STR = "4+";
                    }
                    else if (data.AC_SELF_ENVIDENCE_SECOND == (decimal)5)
                    {
                        this.AC_SELF_ENVIDENCE_SECOND_STR = "5+";
                    }
                }
                if (data.ANTI_GLOBULIN_ENVI != null)
                {
                    switch ((data.ANTI_GLOBULIN_ENVI ?? 0).ToString())
                    {
                        case "1":
                            this.ANTI_GLOBULIN_ENVI_STR = "1+";
                            break;
                        case "2":
                            this.ANTI_GLOBULIN_ENVI_STR = "2+";
                            break;
                        case "3":
                            this.ANTI_GLOBULIN_ENVI_STR = "3+";
                            break;
                        case "4":
                            this.ANTI_GLOBULIN_ENVI_STR = "4+";
                            break;
                        case "5":
                            this.ANTI_GLOBULIN_ENVI_STR = "Âm tính";
                            break;
                        default:
                            break;
                    }
                }
                if (data.ANTI_GLOBULIN_ENVI_TWO != null)
                {
                    switch ((data.ANTI_GLOBULIN_ENVI_TWO ?? 0).ToString())
                    {
                        case "1":
                            this.ANTI_GLOBULIN_ENVI_TWO_STR = "1+";
                            break;
                        case "2":
                            this.ANTI_GLOBULIN_ENVI_TWO_STR = "2+";
                            break;
                        case "3":
                            this.ANTI_GLOBULIN_ENVI_TWO_STR = "3+";
                            break;
                        case "4":
                            this.ANTI_GLOBULIN_ENVI_TWO_STR = "4+";
                            break;
                        case "5":
                            this.ANTI_GLOBULIN_ENVI_TWO_STR = "Âm tính";
                            break;
                        default:
                            break;
                    }
                }
                if (data.SALT_ENVI != null)
                {
                    switch ((data.SALT_ENVI ?? 0).ToString())
                    {
                        case "1":
                            this.SALT_ENVI_STR = "1+";
                            break;
                        case "2":
                            this.SALT_ENVI_STR = "2+";
                            break;
                        case "3":
                            this.SALT_ENVI_STR = "3+";
                            break;
                        case "4":
                            this.SALT_ENVI_STR = "4+";
                            break;
                        case "5":
                            this.SALT_ENVI_STR = "Âm tính";
                            break;
                        default:
                            break;
                    }
                }
                if (data.SALT_ENVI_TWO != null)
                {
                    switch ((data.SALT_ENVI_TWO ?? 0).ToString())
                    {
                        case "1":
                            this.SALT_ENVI_TWO_STR = "1+";
                            break;
                        case "2":
                            this.SALT_ENVI_TWO_STR = "2+";
                            break;
                        case "3":
                            this.SALT_ENVI_TWO_STR = "3+";
                            break;
                        case "4":
                            this.SALT_ENVI_TWO_STR = "4+";
                            break;
                        case "5":
                            this.SALT_ENVI_TWO_STR = "Âm tính";
                            break;
                        default:
                            break;
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
