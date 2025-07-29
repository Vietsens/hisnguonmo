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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace HIS.UC.FormType.GridCheckBox
{
    class GridCheckBoxProcessor : ProcessorBase, IProcessorGenerate
    {
        HIS.Desktop.Common.DelegateSelectDatas delegateSelectDatas;
        object generateRDO;
        internal GridCheckBoxProcessor(V_SAR_RETY_FOFI config, object generateRDO, HIS.Desktop.Common.DelegateSelectDatas delegateSelectDatas)
            : base(config)
        {
            this.generateRDO = generateRDO;
            this.delegateSelectDatas = delegateSelectDatas;
        }

        object IProcessorGenerate.Run()
        {
            object result = null;
            try
            {
                result = new UCGridCheckBox(config, generateRDO, delegateSelectDatas);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }       
    }
}
