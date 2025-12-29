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
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;

namespace HIS.Desktop.Plugins.Optometrist.UC
{
    public partial class UCOptometrist : UserControlBase
    {
        private const string moduleLink = "HIS.Desktop.Plugins.Optometrist";
        private readonly Inventec.Desktop.Common.Modules.Module currentModule;
        private readonly HIS_SERE_SERV currentsereServ;
        public UCOptometrist()
        {
            InitializeComponent();
        }
        public UCOptometrist(Inventec.Desktop.Common.Modules.Module currentModule, HIS_SERE_SERV sereServ)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            this.currentsereServ = sereServ;
        }
        private void UCOptometrist_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                LoadControlStateWorker();
                gridViewSereServ.FocusedRowChanged += OnSereServFocusedRowChanged;
                InitExecuteName();
                InitExcuteRoom();
                InitEnterTabNavigation();
                LoadSereServGrid();
                ValidateControl();
                isNotLoadWhileChangeControlStateInFirst = false;
                UpdateEditModeBySelectedSereServ(GetSelectedSereServ());
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
