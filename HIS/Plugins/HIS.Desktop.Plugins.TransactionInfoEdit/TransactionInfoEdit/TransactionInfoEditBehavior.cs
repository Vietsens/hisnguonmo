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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.TransactionInfoEdit;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.TransactionInfoEdit.TransactionInfoEdit
{
    public sealed class TransactionInfoEditBehavior : Tool<IDesktopToolContext>, ITransactionInfoEdit
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        public TransactionInfoEditBehavior()
            : base()
        {
        }

        public TransactionInfoEditBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ITransactionInfoEdit.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    V_HIS_TRANSACTION _transaction = null;
                    HIS.Desktop.Common.DelegateRefreshData _dlg = null;
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is V_HIS_TRANSACTION)
                        {
                            _transaction = (V_HIS_TRANSACTION)item;
                        }
                        else if (item is HIS.Desktop.Common.DelegateRefreshData)
                        {
                            _dlg = (HIS.Desktop.Common.DelegateRefreshData)item;
                        }
                    }
                    if (currentModule != null && _transaction != null)
                    {
                        if (_dlg != null)
                            result = new frmTransactionInfoEdit(currentModule, _transaction, _dlg);
                        else
                            result = new frmTransactionInfoEdit(currentModule, _transaction);
                    }
                }
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
