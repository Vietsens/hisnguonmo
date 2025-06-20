using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.AnalyzeMedicalImage.ADO;
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AnalyzeMedicalImage.PopopImg
{
    public partial class frmImage : FormBase
    {
        SereServFileADO sereServFileADOs = new SereServFileADO();
        public frmImage(SereServFileADO sereServFileADO)
        {
            InitializeComponent();
            try
            {
                this.sereServFileADOs = sereServFileADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmImage_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataImg();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataImg()
        {
            try
            {
                string[] validExtensions = new[] { ".png", ".jpg", ".jpeg" };
                string ext = Path.GetExtension(this.sereServFileADOs.URL)?.ToLower();
                if (!validExtensions.Contains(ext))
                {

                    if (this.sereServFileADOs.Content != null )
                    {
                        this.layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.wordview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                        if (this.sereServFileADOs.FileContent != null)
                        {
                            richEditControl.RtfText = Utility.TextLibHelper.BytesToStringConverted(this.sereServFileADOs.FileContent);
                        }
                        else
                        {
                            string tempDescription = "";
                            if (this.sereServFileADOs.Content.Trim().Contains("<br/>"))
                            {
                                tempDescription = this.sereServFileADOs.Content.Replace("<br/>", "\r\n");
                                richEditControl.Text = tempDescription;
                            }
                            else
                            {
                                richEditControl.Text = this.sereServFileADOs.Content;
                            }
                        }
                    }
                    else
                    {
                        this.layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        this.layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.wordview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                        Stream stream = Inventec.Fss.Client.FileDownload.GetFile(this.sereServFileADOs.URL);
                        stream.Position = 0;
                        pdfViewer1.LoadDocument(stream);
                    }
                }
                else
                {
                    this.layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.wordview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    var stream = Inventec.Fss.Client.FileDownload.GetFile(this.sereServFileADOs.URL);
                    pteAnhChupFileDinhKem.Image = System.Drawing.Image.FromStream(stream);
                    pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    stream.Close();
                }
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
