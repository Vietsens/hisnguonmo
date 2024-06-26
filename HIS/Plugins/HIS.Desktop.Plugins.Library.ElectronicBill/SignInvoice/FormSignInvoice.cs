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
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.SignInvoice
{
    public partial class FormSignInvoice : Form
    {
        SignInitData InitData;
        string TempFileName;

        public FormSignInvoice(SignInitData initData)
        {
            InitializeComponent();
            this.InitData = initData;
            SetIcon();
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormSignInvoice_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.InitData != null)
                {
                    if (this.InitData.fileToBytes != null)
                    {
                        TempFileName = Path.GetTempFileName();
                        TempFileName = TempFileName.Replace("tmp", "pdf");
                        try
                        {
                            File.WriteAllBytes(TempFileName, this.InitData.fileToBytes);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }

                        pdfView.LoadDocument(TempFileName);
                    }
                    else if (!String.IsNullOrWhiteSpace(this.InitData.FileDownload))
                    {
                        WaitingManager.Show();
                        WebClient client = new WebClient();
                        // Enable TLS 1.2
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        byte[] byteData = client.DownloadData(this.InitData.FileDownload);
                        Stream ms = new MemoryStream(byteData);
                        WaitingManager.Hide();

                        pdfView.LoadDocument(ms);
                    }

                    if (!String.IsNullOrWhiteSpace(this.InitData.Email) && !String.IsNullOrWhiteSpace(this.InitData.Name))
                    {
                        this.txtEmail.Text = this.InitData.Email;
                        this.txtName.Text = this.InitData.Name;
                    }
                }

                txtName.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSignAndRelease_Click(object sender, EventArgs e)
        {
            try
            {
                if ((!string.IsNullOrWhiteSpace(txtName.Text) && string.IsNullOrWhiteSpace(txtEmail.Text)))
                {
                    XtraMessageBox.Show("Cần bổ sung thông tin email của người nhận khi có thông tin tên");
                    txtEmail.Focus();
                    return;
                }
                else if (string.IsNullOrWhiteSpace(txtName.Text) && !string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    XtraMessageBox.Show("Cần bổ sung thông tin tên của người nhận khi có thông tin email");
                    txtName.Focus();
                    return;
                }

                SignDelegate data = new SignDelegate();
                data.Email = txtEmail.Text.Trim();
                data.Name = txtName.Text.Trim();

                string errorMessage = "";
                if (InitData != null && InitData.SignAndRelease != null && InitData.SignAndRelease(data, ref errorMessage))
                {
                    this.Close();
                }
                else if (!String.IsNullOrWhiteSpace(errorMessage))
                {
                    XtraMessageBox.Show(errorMessage);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DeleteTempFile()//gọi khi disponse
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(this.TempFileName))
                {
                    if (File.Exists(this.TempFileName))
                        File.Delete(this.TempFileName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
