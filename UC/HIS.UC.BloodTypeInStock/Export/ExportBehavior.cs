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
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.HisBloodTypeInStock.Export
{
    public sealed class ExportBehavior : IExport
    {
        UserControl control;
        string path;
        public ExportBehavior()
            : base()
        {
        }

        public ExportBehavior(CommonParam param, UserControl data, string _path)
            : base()
        {
            this.control = data;
            this.path = _path;
        }

        void IExport.Run()
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                    ((HIS.UC.HisBloodTypeInStock.Run.UCHisBloodTypeInStock)this.control).Export(path);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
