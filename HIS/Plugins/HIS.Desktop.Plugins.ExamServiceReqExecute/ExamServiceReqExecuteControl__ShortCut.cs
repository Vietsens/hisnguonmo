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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Utility;
using System;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
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

        public void SaveFinishShortCut()
        {
            try
            {
                if (btnSaveFinish.Enabled == true)
                    btnSaveFinish_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusIcd()
        {
            try
            {
                if (txtIcdCode.Enabled)
				{
                    txtIcdCode.Focus();
                    txtIcdCode.SelectAll();
				}                    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SaveShortCut()
        {
            try
            {
                GetUcIcdYHCT();
                Inventec.Common.Logging.LogSystem.Debug("Valid ICD");
                if (!ValidIcd(false)) return;
                bool check = CheckIcd();
                if (check == false)
                {
                    return;
                }
                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void AssignService()
        {
            try
            {
                btnAssignService_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void AssignPre()
        {
            try
            {
                btnAssignPre_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void TreatmentFinish()
        {
            try
            {
                //btnTreatmentFinish_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void CloseTab()
        {
            try
            {
                HIS.Desktop.ModuleExt.TabControlBaseProcess.CloseSelectedTabPage(SessionManager.GetTabControlMain());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }
    }
}
