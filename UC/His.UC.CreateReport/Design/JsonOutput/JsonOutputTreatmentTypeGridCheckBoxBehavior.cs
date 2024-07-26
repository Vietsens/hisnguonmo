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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
//using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DCV.APP.Report.JsonOutput
{
    class JsonOutputTreatmentTypeGridCheckBoxBehavior : BussinessBase, IJsonOutput
    {
        HIS.UC.FormType.TreatmentTypeGridCheckBox.UCTreatmentTypeGridCheckBox entity;
        internal JsonOutputTreatmentTypeGridCheckBoxBehavior(CommonParam param, HIS.UC.FormType.TreatmentTypeGridCheckBox.UCTreatmentTypeGridCheckBox filter)
            : base(param)
        {
            this.entity = filter;
        }

        string IJsonOutput.Run()
        {
            string result = "";
            try
            {
                result = entity.GetValue();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                result = "";
            }
            return result;
        }
    }
}
