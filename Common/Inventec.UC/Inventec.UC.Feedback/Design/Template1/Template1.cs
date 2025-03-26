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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Drawing;

namespace Inventec.UC.Feedback.Design.Template1
{
    internal partial class Template1 : UserControl
    {
        private HasExceptionApi _HasException;
        private CloseForm _Close;

        int positionHandleControl = -1;

        public Template1(Data.DataInitFeedback Data)
        {
            InitializeComponent();
            Inventec.UC.Feedback.Process.ApiConsumerStore.SdaConsumer = Data.sdaConsumer;
            Inventec.UC.Feedback.Process.TokenClient.clientTokenManager = Data.clientTokenManager;
        }
    }
}
