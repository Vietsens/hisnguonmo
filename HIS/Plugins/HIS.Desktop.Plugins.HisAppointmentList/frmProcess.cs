using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Runtime.InteropServices;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraLayout;
namespace HIS.Desktop.Plugins.HisAppointmentList
{
    public partial class frmProcess : Form
    {
        private int positionHandle;
        Action<bool> IsReload;
        long IdAppointment { get; set; }
        public frmProcess(long id, Action<bool> isReload)
        {
            InitializeComponent();

            try
            {
                this.IdAppointment = id;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            IsReload = isReload;
        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmProcess_Load(object sender, EventArgs e)
        {

            try
            {
                chkUpdate.Checked = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                CommonParam param = new CommonParam();
                if (chkCancel.Checked && !dxValidationProvider1.Validate())
                    return;
                HIS_APPOINTMENT data = null;
                if (chkCancel.Checked)
                {
                    MOS.SDO.AppointmentCancelSDO sdo = new MOS.SDO.AppointmentCancelSDO();
                    sdo.Id = IdAppointment;
                    sdo.CancelReason = memReason.Text.Trim();
                    data = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_APPOINTMENT>("api/HisAppointment/Cancel", ApiConsumers.MosConsumer, sdo, param);
                }
                else if (chkUpdate.Checked)
                {
                    data = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_APPOINTMENT>("api/HisAppointment/Process", ApiConsumers.MosConsumer, IdAppointment, param);
                }
                MessageManager.Show(this.ParentForm, param, data != null);
                if (data != null)
                {
                    IsReload(true);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkUpdate_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (chkUpdate.Checked)
                {
                    memReason.Text = null;
                    layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.Size = new System.Drawing.Size(this.Size.Width, this.Size.Height - layoutControlItem3.Size.Height + 10);
                    dxValidationProvider1.SetValidationRule(memReason, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkCancel_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCancel.Checked)
                {
                    layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    //layoutControlItem3.Size = new Size(layoutControlItem3.Size.Width, 300);
                    this.Size = new System.Drawing.Size(this.Size.Width, this.Size.Height + layoutControlItem3.Size.Height + 100);
                    ValidationSingleControl(memReason, 200, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void ValidationSingleControl(BaseEdit control, int? maxLength, [Optional] bool IsRequest)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequest;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }
    }
}
