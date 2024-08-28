using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.UC.Feedback.Process;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Feedback.Design.Template1
{
    internal partial class Template1
    {
        private void Template1_Load(object sender, EventArgs e)
        {
            try
            {
                txtTitle.Focus();
                txtAuthor.Text = (TokenClient.clientTokenManager.GetUserName() == null ? "" : TokenClient.clientTokenManager.GetUserName()) + " - " + (TokenClient.clientTokenManager.GetLoginName() == null ? "" : TokenClient.clientTokenManager.GetLoginName());
                // tự động ẩn thanh cuộn trái
                MemoEditViewInfo viTitle = this.txtTitle.GetViewInfo() as MemoEditViewInfo;
                GraphicsCache cacheTitle = new GraphicsCache(txtTitle.CreateGraphics());
                int hTitle = (viTitle as DevExpress.XtraEditors.ViewInfo.IHeightAdaptable).CalcHeight(cacheTitle, viTitle.MaskBoxRect.Width);
                ObjectInfoArgs argsTitle = new ObjectInfoArgs();
                argsTitle.Bounds = new Rectangle(0, 0, viTitle.ClientRect.Width, hTitle);
                Rectangle rectTitle = viTitle.BorderPainter.CalcBoundsByClientRectangle(argsTitle);
                cacheTitle.Dispose();
                txtTitle.Properties.ScrollBars = rectTitle.Height > txtTitle.Height ? ScrollBars.Vertical : ScrollBars.None;

                MemoEditViewInfo viContent = this.txtContent.GetViewInfo() as MemoEditViewInfo;
                GraphicsCache cacheContent = new GraphicsCache(txtContent.CreateGraphics());
                int hContent = (viContent as DevExpress.XtraEditors.ViewInfo.IHeightAdaptable).CalcHeight(cacheContent, viContent.MaskBoxRect.Width);
                ObjectInfoArgs argsContent = new ObjectInfoArgs();
                argsContent.Bounds = new Rectangle(0, 0, viContent.ClientRect.Width, hContent);
                Rectangle rectContent = viContent.BorderPainter.CalcBoundsByClientRectangle(argsContent);
                cacheContent.Dispose();
                txtContent.Properties.ScrollBars = rectContent.Height > txtContent.Height ? ScrollBars.Vertical : ScrollBars.None;

                ValidationFeedbackTitle();
                ValidationFeedbackContent();
                ValidationFeedbackAuthor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
