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
using HIS.UC.CreateReport.Base;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCV.LOGIC.HisSereServ.Update
{
    class CreateReportGenerateBehavior : BussinessBase, ICreateReportGenerate
    {
        HIS.UC.FormType.GenerateRDO entity;
        internal CreateReportGenerateBehavior(CommonParam param, HIS.UC.FormType.GenerateRDO paramData)
            : base(param)
        {
            this.entity = paramData;
        }

        System.Windows.Forms.UserControl ICreateReportGenerate.Run()
        {
            System.Windows.Forms.UserControl result = null;
            try
            {
                //string vlCustomFormType = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ReportCreate.CustomFormType");
                //if (vlCustomFormType == "1")
                //{
                //    result = new His.UC.CreateReport.Design.CreateReport1.CreateReport1(this.entity);
                //}
                //else
                //{
                result = new His.UC.CreateReport.Design.CreateReport.CreateReport(this.entity);
                //}
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
