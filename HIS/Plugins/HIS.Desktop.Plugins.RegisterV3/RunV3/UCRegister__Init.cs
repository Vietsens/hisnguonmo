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
using DevExpress.XtraEditors;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.RegisterV3.Run3
{
    public partial class UCRegister : UserControlBase
    {

        private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo1Column(Control cboEditor, object data, string valueMember, string displayMember)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(displayMember, "", cboEditor.Width, 0, true));

                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetDataBeforeClickbtnPatientNew()
        {
            try
            {
               this.dataPatientRaw  = this.ucPatientRaw1.GetValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
