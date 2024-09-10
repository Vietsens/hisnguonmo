using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;
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

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {
            try
            {
                MemoEditViewInfo viTitle = this.txtTitle.GetViewInfo() as MemoEditViewInfo;
                GraphicsCache cacheTitle = new GraphicsCache(txtTitle.CreateGraphics());
                int hTitle = (viTitle as DevExpress.XtraEditors.ViewInfo.IHeightAdaptable).CalcHeight(cacheTitle, viTitle.MaskBoxRect.Width);
                ObjectInfoArgs argsTitle = new ObjectInfoArgs();
                argsTitle.Bounds = new Rectangle(0, 0, viTitle.ClientRect.Width, hTitle);
                Rectangle rectTitle = viTitle.BorderPainter.CalcBoundsByClientRectangle(argsTitle);
                cacheTitle.Dispose();
                txtTitle.Properties.ScrollBars = rectTitle.Height > txtTitle.Height ? ScrollBars.Vertical : ScrollBars.None;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            try
            {
                MemoEditViewInfo viContent = this.txtContent.GetViewInfo() as MemoEditViewInfo;
                GraphicsCache cacheContent = new GraphicsCache(txtContent.CreateGraphics());
                int hContent = (viContent as DevExpress.XtraEditors.ViewInfo.IHeightAdaptable).CalcHeight(cacheContent, viContent.MaskBoxRect.Width);
                ObjectInfoArgs argsContent = new ObjectInfoArgs();
                argsContent.Bounds = new Rectangle(0, 0, viContent.ClientRect.Width, hContent);
                Rectangle rectContent = viContent.BorderPainter.CalcBoundsByClientRectangle(argsContent);
                cacheContent.Dispose();
                txtContent.Properties.ScrollBars = rectContent.Height > txtContent.Height ? ScrollBars.Vertical : ScrollBars.None;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAuthor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSend.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
