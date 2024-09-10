using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraWaitForm;
using Inventec.Desktop.Common.LibraryMessage;

namespace Inventec.Desktop.Common.Message
{
    public partial class frmWaitForm : WaitForm
    {
        public frmWaitForm()
        {
            InitializeComponent();
            this.progressPanel1.AutoHeight = true;
        }
        public frmWaitForm(int frameCount)
            : this()
        {
            this.progressPanel1.FrameCount = frameCount;
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
            WaitFormCommand command = (WaitFormCommand)cmd;
            if (command == WaitFormCommand.Activate) // && !this.Visible)
                this.Show();
            else if (command == WaitFormCommand.Deactivate) // && this.Visible)
                this.Hide();
        }

        #endregion

        public enum WaitFormCommand
        {
            Activate,
            Deactivate
        }
    }
}