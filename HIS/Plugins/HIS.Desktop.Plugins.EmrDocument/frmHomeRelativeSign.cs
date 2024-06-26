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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.EmrDocument
{
    public partial class frmHomeRelativeSign : FormBase
    {
        private int positionHandleControlLeft;
        private Action<string> sName;
        private Action<EMR_RELATION> Relative;
        private Action<bool> ActionClosed;
        bool IsClosed;
        public frmHomeRelativeSign(List<EMR_RELATION> relationDatas, Action<string> aName, Action<EMR_RELATION> aRelative, Action<bool> ActionClosed)
        {
            InitializeComponent();
            this.sName = aName;
            this.Relative = aRelative;
            this.relationDatas= relationDatas;
            this.ActionClosed = ActionClosed;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlLeft == -1)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlLeft > edit.TabIndex)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        List<EMR_RELATION> relationDatas;
        private void frmHomeRelativeSign_Load(object sender, EventArgs e)
        {
            ValidationSingleControl(txtName, dxValidationProvider1);
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("RELATION_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("RELATION_NAME", "ID", columnInfos, false, 250);
            ControlEditorLoader.Load(cboRelative, this.relationDatas, controlEditorADO);
            ValidationSingleControl(cboRelative, dxValidationProvider1);
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {

            try
            {
                if (!dxValidationProvider1.Validate())
                    return;
                sName(txtName.Text.Trim());
                Relative(relationDatas.FirstOrDefault(o=>o.ID == (long)cboRelative.EditValue));
                IsClosed = true;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnChoose_Click(null, null);
        }

        private void frmHomeRelativeSign_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ActionClosed(IsClosed);
        }

        private void txtName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRelative.Focus();
                    cboRelative.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
