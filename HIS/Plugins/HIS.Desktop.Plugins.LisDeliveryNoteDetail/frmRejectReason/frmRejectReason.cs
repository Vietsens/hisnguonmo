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
using HIS.Desktop.Plugins.LisDeliveryNoteDetail.ADO;
using HIS.Desktop.Plugins.LisDeliveryNoteDetail.Validtion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.LisDeliveryNoteDetail
{
    public delegate void DelegateProcessReject(SampleADO _sample, string _rejectReason);
    public delegate void DelegateProcessRejectList(List<long> _listSampleID, string _rejectReason);
    public partial class frmRejectReason : HIS.Desktop.Utility.FormBase
    {
        SampleADO sample;
        List<long> listSampleID;
        DelegateProcessReject actProcessReject;
        DelegateProcessRejectList actProcessRejectList;
        public frmRejectReason(Inventec.Desktop.Common.Modules.Module _moduleData, DelegateProcessReject _actProcessReject, SampleADO _sample)
            : base(_moduleData)
        {
            InitializeComponent();
            this.actProcessReject = _actProcessReject;
            this.sample = _sample;
        }

        public frmRejectReason(Inventec.Desktop.Common.Modules.Module _moduleData, DelegateProcessRejectList _actProcessRejectList, List<long> _listSampleID)
            : base(_moduleData)
        {
            InitializeComponent();
            this.actProcessRejectList = _actProcessRejectList;
            this.listSampleID = _listSampleID;
        }

        private void frmRejectReason_Load(object sender, EventArgs e)
        {
            try
            {
                txtRejectReason.Focus();
                txtRejectReason.SelectAll();

                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidMaxlengthTextBox(txtRejectReason, 4000, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTextBox(BaseEdit txtEdit, int? maxLength, bool isRequired)
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtEdit;
                validateMaxLength.isRequired = isRequired;
                validateMaxLength.maxLength = maxLength;
                dxValidationProvider1.SetValidationRule(txtEdit, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRejectReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    ProcessSave();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSave()
        {
            try
            {
                if (!dxValidationProvider1.Validate())
                    return;

                if (this.actProcessReject != null && this.sample != null)
                {
                    this.actProcessReject(sample,txtRejectReason.Text.Trim());
                }
                if (this.actProcessRejectList != null && this.listSampleID != null)
                {
                    this.actProcessRejectList(listSampleID, txtRejectReason.Text.Trim());
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
