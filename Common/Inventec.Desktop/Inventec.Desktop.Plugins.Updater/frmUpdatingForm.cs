using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraWaitForm;
using Inventec.Desktop.Common.LibraryMessage;

namespace Inventec.Desktop.Plugins.Updater
{
    public partial class frmUpdatingForm : WaitForm
    {
        public frmUpdatingForm()
        {
            InitializeComponent();
            this.progressPanel1.AutoHeight = true;
            this.SetCaption(MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongThongBaoTieuDeChoWaitDialogFormIsPleaseWaiting));
            this.SetDescription(MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongThongBaoMoTaChoUpdatingDialogForm));
        }

        #region Overrides

        public override void SetCaption(string caption)
        {
            base.SetCaption(caption);
            this.progressPanel1.Caption = caption;
        }
        public override void SetDescription(string description)
        {
            base.SetDescription(description);
            this.progressPanel1.Description = description;
        }
        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        public enum WaitFormCommand
        {
        }
    }
}