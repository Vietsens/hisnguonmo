using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Client;
using System.IO;
using Inventec.Common.SignLibrary;
using HIS.Desktop.Plugins.HisRegimenTemp.UC;
namespace HIS.Desktop.Plugins.TrackingCreate
{
    public partial class frmAttach : Form
    {
        HIS.Desktop.Plugins.HisRegimenTemp.UC.UCDocument UcDocument = new UCDocument();
        HIS.Desktop.Plugins.HisRegimenTemp.UC.UCPDF UCPdf = new UCPDF();
        long departmentId { get; set; }
        public frmAttach(long departmentId)
        {
            this.departmentId = departmentId; 
            InitializeComponent();
        }

        private void frmAttach_Load(object sender, EventArgs e)
        {
            WaitingManager.Show();
            FillDataToGridAttach();
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            WaitingManager.Hide();
        }

        private void FillDataToGridAttach()
        {
            try
            {
                HisRegimenTempFilter filter = new HisRegimenTempFilter();
                CommonParam param = new CommonParam();
                filter.IS_ACTIVE = 1;
                filter.IS_PUBLIC_IN_DEPARTMENT = true;
                filter.IS_PUBLIC = true; 
                filter.DEPARTMENT_ID = this.departmentId;
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_REGIMEN_TEMP>>("api/HisRegimenTemp/Get", ApiConsumers.MosConsumer, filter, param);
                if(result != null && result.Count > 0)
                {
                    gridControlAttach.BeginUpdate();
                    gridControlAttach.DataSource = result; 
                    gridControlAttach.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAttach_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.FieldName == "STT")
            {
                e.Value = e.ListSourceRowIndex + 1;
            }

        }

        private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (HIS_REGIMEN_TEMP)gridViewAttach.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.URL))
                {
                    WaitingManager.Show();

                    string output = Utils.GenerateTempFileWithin();
                    string path = Utils.GenerateTempFolderWithin();
                    var streamSource = Inventec.Fss.Client.FileDownload.GetFile(row.URL);
                    streamSource.Position = 0;
                    //string outputFilePath = Path.Combine(path, Guid.NewGuid().ToString() + ".doc");
                    string extension = ".doc";
                    if (!string.IsNullOrEmpty(row.FILE_NAME))
                    {
                        extension = Path.GetExtension(row.FILE_NAME).ToLower();
                        if (extension != ".doc" && extension != ".docx")
                            extension = ".doc";
                    }
                    string outputFilePath = Path.Combine(path, Guid.NewGuid().ToString() + extension);
                    WaitingManager.Hide();
                    if (row.FILE_TYPE == 2)
                    {
                        Utils.ByteToFile(Utils.StreamToByte(streamSource), output);
                        UCPdf.LoadDocument(output);
                        frmDisplay frm = new frmDisplay(UCPdf);
                        frm.ShowDialog();
                    }
                    else if (row.FILE_TYPE == 3)
                    {
                        SaveStreamToDocFile(streamSource, outputFilePath);
                        UcDocument.LoadDocument(outputFilePath);
                        frmDisplay frm = new frmDisplay(UcDocument);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        private void SaveStreamToDocFile(Stream streamSource, string outputFilePath)
        {
            try
            {
                using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {
                    streamSource.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageBox.Show("Error saving stream to .doc file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}

