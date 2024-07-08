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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
//using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using His.UC.LibraryMessage;

namespace HIS.UC.FormType.FilterExtra
{
    public partial class UCFilterExtra : DevExpress.XtraEditors.XtraUserControl
    {
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCFilterExtra(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                this.config = config;
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                if (this.report != null)
                {
                    SetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }               

        public string GetValue()
        {
            string result = "";
            try
            {
                result = this.config.JSON_OUTPUT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        public void SetValue()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool Valid()
        {
            bool result = true;
            try
            {
               
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

       
       
    }
}
