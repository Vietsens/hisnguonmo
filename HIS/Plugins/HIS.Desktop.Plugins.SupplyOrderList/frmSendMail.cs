using HIS.Desktop.Plugins.SupplyOrderList.ADO;
using HIS.Desktop.Plugins.SupplyOrderList.Resources;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SupplyOrderList
{
    /// <summary>
    /// Mail of a confirmed order: recipient defaults to the supplier email, content editable before sending
    /// </summary>
    public partial class frmSendMail : DevExpress.XtraEditors.XtraForm
    {
        private const string FORMAT_AMOUNT = "#,##0.####";

        private SupplyOrderADO order;
        private List<OrderDetailADO> details;

        public HisSupplyOrderSendMailSDO Result { get; private set; }

        public frmSendMail()
        {
            InitializeComponent();
        }

        internal frmSendMail(SupplyOrderADO order, List<OrderDetailADO> details)
        {
            InitializeComponent();
            this.order = order;
            this.details = details ?? new List<OrderDetailADO>();
        }

        private void frmSendMail_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = ResourceLanguageManager.Get("frmSendMail.Text");
                this.lblTo.Text = ResourceLanguageManager.Get("frmSendMail.lblTo.Text");
                this.lblCc.Text = ResourceLanguageManager.Get("frmSendMail.lblCc.Text");
                this.lblSubject.Text = ResourceLanguageManager.Get("frmSendMail.lblSubject.Text");
                this.lblBody.Text = ResourceLanguageManager.Get("frmSendMail.lblBody.Text");
                this.lblHint.Text = ResourceLanguageManager.Get("frmSendMail.lblHint.Text");
                this.btnSend.Text = ResourceLanguageManager.Get("frmSendMail.btnSend.Text");
                this.btnCancel.Text = ResourceLanguageManager.Get("frmSendMail.btnCancel.Text");
                if (this.order == null)
                {
                    return;
                }
                this.txtTo.Text = !String.IsNullOrWhiteSpace(this.order.LAST_MAIL_TO) ? this.order.LAST_MAIL_TO : this.order.SUPPLIER_EMAIL;
                this.txtSubject.Text = string.Format(ResourceLanguageManager.Get("frmSendMail.Subject"), this.order.SUPPLY_ORDER_CODE, this.order.MEDI_STOCK_NAME);
                this.memoBody.Text = this.BuildBody();
                if (String.IsNullOrWhiteSpace(this.txtTo.Text))
                {
                    this.txtTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string BuildBody()
        {
            string lineFormat = ResourceLanguageManager.Get("frmSendMail.BodyLine");
            StringBuilder sb = new StringBuilder();
            int stt = 1;
            foreach (OrderDetailADO d in this.details)
            {
                string amount = d.IMP_UNIT_AMOUNT.HasValue && !String.IsNullOrWhiteSpace(d.IMP_UNIT_NAME)
                    ? d.IMP_UNIT_AMOUNT.Value.ToString(FORMAT_AMOUNT) + " " + d.IMP_UNIT_NAME + " (" + d.AMOUNT.ToString(FORMAT_AMOUNT) + " " + d.SERVICE_UNIT_NAME + ")"
                    : d.AMOUNT.ToString(FORMAT_AMOUNT) + " " + d.SERVICE_UNIT_NAME;
                sb.AppendLine(string.Format(lineFormat, stt++, d.TYPE_CODE, d.TYPE_NAME, amount));
            }
            string body = string.Format(ResourceLanguageManager.Get("frmSendMail.Body"),
                this.order.SUPPLIER_NAME,
                this.order.SUPPLY_ORDER_CODE,
                this.order.MEDI_STOCK_NAME,
                sb.ToString(),
                this.order.TOTAL_WITH_VAT.ToString("#,##0.##"));
            return body.Replace("\\n", Environment.NewLine);
        }

        private static List<string> SplitEmails(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }
            return text.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).Where(o => o != "").ToList();
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                System.Net.Mail.MailAddress address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string title = ResourceLanguageManager.Get("Message.Title");
                List<string> to = SplitEmails(this.txtTo.Text);
                List<string> cc = SplitEmails(this.txtCc.Text);
                if (to.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("frmSendMail.Message.ToRequired"), title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.txtTo.Focus();
                    return;
                }
                List<string> invalid = to.Concat(cc).Where(o => !IsValidEmail(o)).ToList();
                if (invalid.Count > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, string.Format(ResourceLanguageManager.Get("frmSendMail.Message.InvalidEmail"), String.Join(", ", invalid)), title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (String.IsNullOrWhiteSpace(this.txtSubject.Text))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("frmSendMail.Message.SubjectRequired"), title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.txtSubject.Focus();
                    return;
                }
                HisSupplyOrderSendMailSDO sdo = new HisSupplyOrderSendMailSDO();
                sdo.OrderId = this.order.ID;
                sdo.ToEmail = String.Join(";", to);
                sdo.CcEmail = cc.Count > 0 ? String.Join(";", cc) : null;
                sdo.Subject = this.txtSubject.Text.Trim();
                sdo.Body = this.memoBody.Text;
                this.Result = sdo;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
