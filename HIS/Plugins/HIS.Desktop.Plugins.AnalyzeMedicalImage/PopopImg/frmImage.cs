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
        List<SereServFileADO> sereServFileADOs = new List<SereServFileADO>();
        public frmImage(List<SereServFileADO> sereServFileADO)
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
                foreach (var item in sereServFileADOs)
                {
                    if (item.IsFss)
                    {
                        string ext = Path.GetExtension(item.URL)?.ToLower();
                        if (!validExtensions.Contains(ext))
                        {
                            
                            if (item.FileContent != null )
                            {
                                this.layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                                this.layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                                this.layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                                richEditControl.RtfText = Utility.TextLibHelper.BytesToStringConverted(item.FileContent);
                            }
                            else
                            {
                                this.layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                                this.layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                                this.layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                                Stream stream = Inventec.Fss.Client.FileDownload.GetFile(item.URL);
                                stream.Position = 0;
                                pdfViewer1.LoadDocument(stream);
                            }
                        }
                        else
                        {
                            this.layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                            this.layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            this.layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                            var stream = Inventec.Fss.Client.FileDownload.GetFile(item.URL);
                            pteAnhChupFileDinhKem.Image = System.Drawing.Image.FromStream(stream);
                            pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                            stream.Close();
                        }
                        Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmImage_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (var item in sereServFileADOs)
                {
                    if (item.IsFss)
                    {
                        item.IsFss = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
