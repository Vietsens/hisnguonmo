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
using HIS.UC.FormType.DepartmentCombo;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace HIS.UC.FormType
{
    abstract class ProcessorBase
    {
        protected V_SAR_RETY_FOFI config;
        protected Dictionary<string, object> singleValueDictionary = new Dictionary<string, object>();

        internal ProcessorBase(V_SAR_RETY_FOFI configData)
        {
            this.config = configData;
        }

        //abstract internal object GetValue();

    }
}
