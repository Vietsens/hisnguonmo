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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.EventLogControl.Base;

namespace Inventec.UC.EventLogControl.Design.Template1
{
    internal partial class Template1 : UserControl, Inventec.UC.EventLogControl.Base.IFormCallBack
    {

        PagingGrid pagingGrid;
        DevExpress.Utils.WaitDialogForm waitLoad;

        private List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG> ListEventLog = new List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG>();
        private ProcessHasException _HasException;
        Data.DataInit currentData;

        public Template1(Data.DataInit data)
        {
            InitializeComponent();
            try
            {
                ApiConsumerStore.SdaConsumer = data.sdaComsumer;
                GlobalStore.NumPageSize = data.pageNum <= 0 ? 100 : data.pageNum;
                currentData = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
