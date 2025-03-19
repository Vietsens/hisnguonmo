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
using DevExpress.Utils;

namespace Inventec.UC.ChangePassword.Design.Template2
{
    internal partial class Template2 : UserControl
    {

        private ChangePasswordSuccess _ChangeSuccess;
        private HasExceptionApi _HasException;

        private WaitDialogForm waitLoad;

        public Template2(Data.DataInitChangePass Data)
        {
            InitializeComponent();
            Inventec.UC.ChangePassword.Process.ApiConsumerStore.SdaConsumer = Data.sdaConsumer;
            Inventec.UC.ChangePassword.Process.TokenClient.clientTokenManager = Data.clientTokenManager;
        }

        private void Template2_Load(object sender, EventArgs e)
        {
            try
            {
                ValidControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        
    }
}
