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
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Utils.Menu;
using HIS.Desktop.Plugins.RehaServiceReqExecute.Base;
using Inventec.Desktop.Common.LanguageManager;
using AutoMapper;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.RehaServiceReqExecute.ADO;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace HIS.Desktop.Plugins.RehaServiceReqExecute
{
    public partial class RehaServiceReqExecuteControl : HIS.Desktop.Utility.UserControlBase
    {
        public void SaveShortCut()
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FinishShortCut()
        {
            try
            {
                btnFinish_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ThemTapShortCut()
        {
            try
            {
                btnThemTTTap.Focus();
                btnThemTTTap_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void AssignServiceShortCut()
        {
            try
            {
                btnAssignService_Click_1(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void AssignPreShortCut()
        {
            try
            {
                if (btnAssignPre.Enabled)
                    btnAssignPre_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
