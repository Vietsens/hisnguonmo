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
using HIS.Desktop.Plugins.InvoiceBook;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventec.Desktop.Plugins.InvoiceBook.InvoiceBook
{
    public sealed class InvoiceBookBehavior : Tool<IDesktopToolContext>, IInvoiceBook
    {
        object[] entity;
        public InvoiceBookBehavior()
            : base()
        {
        }

        public InvoiceBookBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IInvoiceBook.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module Module = null;

                for (int i = 0; i < entity.Count(); i++)
                {
                    if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                    {
                        Module = (Inventec.Desktop.Common.Modules.Module)entity[i];
                    }
                }

                return new UCInvoiceBook(Module);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true;
                return null;
            }
        }
    }
}