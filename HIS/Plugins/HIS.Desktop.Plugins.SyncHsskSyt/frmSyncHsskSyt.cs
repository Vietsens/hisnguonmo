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
using HIS.Desktop.Plugins.SyncHsskSyt.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Data;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using Inventec.Common.Logging;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
namespace HIS.Desktop.Plugins.SyncHsskSyt
{
    public partial class frmSyncHsskSyt : DevExpress.XtraEditors.XtraForm
    {
        public frmSyncHsskSyt(Inventec.Desktop.Common.Modules.Module moduleData)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            gridView4210.OptionsSelection.MultiSelect = true;
            gridView4210.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
            gridView130.OptionsSelection.MultiSelect = true;
            gridView130.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
        }
        #region Variables and Constants
        public static string keyFilePathHistory4210 ;
        public static string keyFilePathHistory130;
        static string user = "";
        static string pass = "";
        static string urlLogin = "";
        static string urlXML4210 = "";
        static string urlXML130 = "";
        string authen = "";
        string directoryPathXML4210 = "";
        static string CredirectoryPathXML4210;
        string directoryPathXML130 = "" ;
        static string CredirectoryPathXML130;
        List<string> connectInfors = new List<string>();
        private ListBox lstFiles = new ListBox();
        //bool success = false;
        private List<FileItem> fileNamesXML130 = new List<FileItem>();
        private List<FileItem> fileNamesXML4210 = new List<FileItem>();
        private List<FileItem> fileNamesXML130Send = new List<FileItem>();
        private List<FileItem> fileNamesXML4210Send = new List<FileItem>();
        bool XML130;
        #endregion

        #region form
        private async void frmSyncHsskSyt_Load(object sender, EventArgs e)
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                checkConfig();
                WaitingManager.Show();
                authen = await Login(user, pass).ConfigureAwait(false);
                checkLogin(authen);
                DefaultFilePath();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SyncHsskSyt.Resources.Lang", typeof(frmSyncHsskSyt).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSend4210.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.btnSend4210.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset4210.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.btnReset4210.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSend130.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.btnSend130.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset130.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.btnReset130.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSyncHsskSyt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region XML130
        private void btnReset130_Click(object sender, EventArgs e)
        {
            try
            {
                lstFiles.Items.Clear();
                WaitingManager.Show();
                if (Directory.Exists(directoryPathXML130))
                {
                    // Lấy tất cả các tệp XML trong thư mục
                    var xmlFiles = Directory.GetFiles(directoryPathXML130, "*.xml");
                    fileNamesXML130.Clear();
                    // Thêm từng tệp vào ListBox
                    foreach (var file in xmlFiles)
                    {
                        FileItem newItem = new FileItem()
                        {
                            STATUS = 0, // trạng thái chưa gửi
                            FILE_NAME = file
                        };
                        fileNamesXML130.Add(newItem);
                        lstFiles.Items.Add(newItem.FILE_NAME); // Thêm tên tệp vào ListBox
                        //lstFiles.Items.Add(Path.GetFileName(file));
                    }

                    MessageBox.Show("Đã tải lại tệp thành công.");
                }
                else
                {
                    MessageBox.Show("Thư mục không tồn tại.");
                }
                //directoryPathXML130 = "";
                FillDataToControlXML130();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView130_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedRows = gridView130.GetSelectedRows();
                if (selectedRows.Length > 0)
                {
                    btnSend130.Enabled = true;
                    fileNamesXML130Send.Clear();
                    foreach (var rowHandle in selectedRows)
                    {
                        if (rowHandle >= 0)
                        {
                            var fileItem = gridView130.GetRow(rowHandle) as FileItem;
                            directoryPathXML130 = Path.GetDirectoryName(fileItem.FILE_NAME);
                            if (fileItem != null)
                            {
                                fileNamesXML130Send.Add(fileItem);
                            }
                        }
                    }
                }
                else
                {
                    btnSend130.Enabled = false;
                    fileNamesXML130Send.Clear();
                }
                txtFilePath130.Text = directoryPathXML130;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void btnSend130_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                string successFolderPath130 ;
                string failFolderPath130;
                if (CredirectoryPathXML130 != null)
                {
                    successFolderPath130 = Path.Combine(CredirectoryPathXML130, "success");
                    failFolderPath130 = Path.Combine(CredirectoryPathXML130, "fail");
                }
                else
                {
                    successFolderPath130 = Path.Combine(Environment.CurrentDirectory, "success");
                    failFolderPath130 = Path.Combine(Environment.CurrentDirectory, "fail");
                }
                foreach (var fileItem in fileNamesXML130Send)
                {
                    string fullname = fileItem.FILE_NAME;
                    CredirectoryPathXML130 = Path.GetDirectoryName(fullname);
                    if (CredirectoryPathXML130 != null)
                    {
                        successFolderPath130 = Path.Combine(CredirectoryPathXML130, "success");
                        failFolderPath130 = Path.Combine(CredirectoryPathXML130, "fail");
                    }
                    else
                    {
                        successFolderPath130 = Path.Combine(Environment.CurrentDirectory, "success");
                        failFolderPath130 = Path.Combine(Environment.CurrentDirectory, "fail");
                    }
                    if (!File.Exists(fullname))
                    {
                        MessageBox.Show("Tệp tin: " + fullname + " không tồn tại, vui lòng kiểm tra lại.");
                    }
                    else
                    {
                        string destFilePathSuc = Path.Combine(successFolderPath130, Path.GetFileName(fullname));
                        string destFilePathFal = Path.Combine(failFolderPath130, Path.GetFileName(fullname));
                        bool success = await UploadFileAsync(urlXML130, authen, fullname);
                        if (success == true)
                        {
                            if (!Directory.Exists(successFolderPath130))
                            {
                                Directory.CreateDirectory(successFolderPath130);

                                if (File.Exists(destFilePathSuc))
                                {
                                    File.Delete(destFilePathSuc);
                                }
                                File.Move(fullname, destFilePathSuc);
                            }
                            else
                            {
                                if (File.Exists(destFilePathSuc))
                                {
                                    File.Delete(destFilePathSuc);
                                }
                                File.Move(fullname, destFilePathSuc);
                            }
                            UpdateFileStatusXML130(fullname, 1);

                        }
                        else
                        {
                            if (!Directory.Exists(failFolderPath130))
                            {
                                Directory.CreateDirectory(failFolderPath130);
                                if (File.Exists(destFilePathFal))
                                {
                                    File.Delete(destFilePathFal);
                                }
                                File.Move(fullname, destFilePathFal);
                            }
                            else
                            {
                                if (File.Exists(destFilePathFal))
                                {
                                    File.Delete(destFilePathFal);
                                }
                                File.Move(fullname, destFilePathFal);
                            }
                            UpdateFileStatusXML130(fullname, 2);
                        }
                    }
                }

                FillDataToControlXML130();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView130_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    if (e.Column.FieldName == "STATUS_130")
                    {
                        var documents = fileNamesXML130;
                        if (documents != null && documents.Count > 0)
                        {
                            for (int i = 0; i < documents.Count; i++)
                            {
                                var document = documents[i];
                                if (e.ListSourceRowIndex == i)
                                {
                                    if (document.STATUS == 0)
                                    {
                                        e.Value = imageList1.Images[2];
                                    }
                                    else if (document.STATUS == 1)
                                    {
                                        e.Value = imageList1.Images[0];
                                    }
                                    else
                                    {
                                        e.Value = imageList1.Images[1];
                                    }
                                    break;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChooseFile130_Click(object sender, EventArgs e)
        {
            try
            {
                XML130 = true;
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    if (txtFilePath130.Text == null)
                    {
                        openFileDialog.InitialDirectory = "c:\\";
                    }
                    else
                    {
                        openFileDialog.InitialDirectory = txtFilePath130.Text;
                    }
                    //openFileDialog.Filter = "All files (*.*)|*.*";
                    openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.Multiselect = true;

                    DialogResult result = openFileDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        string[] filePaths = openFileDialog.FileNames;

                        // Thêm từng đường dẫn vào danh sách fileNames
                        foreach (string filePath in filePaths)
                        {
                            directoryPathXML130 = Path.GetDirectoryName(filePath);
                            FileItem newItem = new FileItem()
                            {
                                STATUS = 0, //chưa gửi
                                FILE_NAME = filePath
                            };
                            fileNamesXML130.Add(newItem);
                        }
                    }
                }
                SaveFilePathHistory();
                FillDataToControlXML130();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    
        #endregion

        #region XML 4210
        private void btnReset4210_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                lstFiles.Items.Clear();
                if (Directory.Exists(directoryPathXML4210))
                {
                    // Lấy tất cả các tệp XML trong thư mục
                    var xmlFiles = Directory.GetFiles(directoryPathXML4210, "*.xml");
                    fileNamesXML4210.Clear();
                    // Thêm từng tệp vào ListBox
                    foreach (var file in xmlFiles)
                    {
                        FileItem newItem = new FileItem()
                        {
                            STATUS = 0, // trạng thái chưa gửi
                            FILE_NAME = file
                        };
                        fileNamesXML4210.Add(newItem);
                        lstFiles.Items.Add(newItem.FILE_NAME); // Thêm tên tệp vào ListBox
                    }

                    MessageBox.Show("Đã tải lại tệp thành công.");
                }
                else
                {
                    MessageBox.Show("Thư mục không tồn tại.");
                }
                //directoryPathXML4210 = "";
                FillDataToControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView4210_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                var selectedRows = gridView4210.GetSelectedRows();
                if (selectedRows.Length > 0)
                {
                    btnSend4210.Enabled = true;
                    fileNamesXML4210Send.Clear();
                    foreach (var rowHandle in selectedRows)
                    {
                        if (rowHandle >= 0) 
                        {
                            var fileItem = gridView4210.GetRow(rowHandle) as FileItem;
                            directoryPathXML4210 = Path.GetDirectoryName(fileItem.FILE_NAME);
                            if (fileItem != null)
                            {
                                fileNamesXML4210Send.Add(fileItem);
                            }
                        }
                    }
                }
                else
                {
                    btnSend4210.Enabled = false;
                    fileNamesXML4210Send.Clear(); 
                }
                txtFilePath4210.Text = directoryPathXML4210;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void btnSend4210_Click(object sender, EventArgs e)
        {
            try
            {
                string successFolderPath4210;
                string failFolderPath4210;
                WaitingManager.Show();
                
                foreach (var fileItem in fileNamesXML4210Send)
                {
                    string fullname = fileItem.FILE_NAME;
                    CredirectoryPathXML4210 = Path.GetDirectoryName(fullname);
                    if (CredirectoryPathXML4210 != null)
                    {
                        successFolderPath4210 = Path.Combine(CredirectoryPathXML4210, "success");
                        failFolderPath4210 = Path.Combine(CredirectoryPathXML4210, "fail");
                    }
                    else
                    {
                        successFolderPath4210 = Path.Combine(Environment.CurrentDirectory, "success");
                        failFolderPath4210 = Path.Combine(Environment.CurrentDirectory, "fail");
                    }
                    if (!File.Exists(fullname))
                    {
                        MessageBox.Show("Tệp tin: "+ fullname +" không tồn tại, vui lòng kiểm tra lại." );
                    }
                    else
                    {
                        bool success = await UploadFileAsync(urlXML4210, authen, fullname);
                        string destFilePathSuc = Path.Combine(successFolderPath4210, Path.GetFileName(fullname));
                        string destFilePathFal = Path.Combine(failFolderPath4210, Path.GetFileName(fullname));
                        if (success == true)
                        {
                            if (!Directory.Exists(successFolderPath4210))
                            {
                                Directory.CreateDirectory(successFolderPath4210);
                                if (File.Exists(destFilePathSuc))
                                {
                                    File.Delete(destFilePathSuc);
                                }
                                File.Move(fullname, destFilePathSuc);
                            }
                            else
                            {
                                if (File.Exists(destFilePathSuc))
                                {
                                    File.Delete(destFilePathSuc);
                                }
                                File.Move(fullname, destFilePathSuc);
                            }
                            UpdateFileStatusXML4210(fullname, 1);

                        }
                        else
                        {
                            if (!Directory.Exists(failFolderPath4210))
                            {
                                Directory.CreateDirectory(failFolderPath4210);

                                if (File.Exists(destFilePathFal))
                                {
                                    File.Delete(destFilePathFal);
                                }
                                File.Move(fullname, destFilePathFal);
                            }
                            else
                            {
                                if (File.Exists(destFilePathFal))
                                {
                                    File.Delete(destFilePathFal);
                                }
                                File.Move(fullname, destFilePathFal);
                            }
                            UpdateFileStatusXML4210(fullname, 2);
                        }
                    }
                }
                FillDataToControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView4210_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    if (e.Column.FieldName == "STATUS_4210")
                    {
                        var documents = fileNamesXML4210;
                        if (documents != null && documents.Count > 0)
                        {
                            for (int i = 0; i < documents.Count; i++)
                            {
                                var document = documents[i];
                                if (e.ListSourceRowIndex == i)
                                {
                                    if (document.STATUS == 0)
                                    {
                                        e.Value = imageList1.Images[2];
                                    }
                                    else if (document.STATUS == 1)
                                    {
                                        e.Value = imageList1.Images[0];
                                    }
                                    else
                                    {
                                        e.Value = imageList1.Images[1];
                                    }
                                    break;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnChooseFile4210_Click(object sender, EventArgs e)
        {
            try
            {
                XML130 = false;
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    if (txtFilePath4210.Text == null)
                    {
                        openFileDialog.InitialDirectory = "c:\\";
                    }
                    else
                    {
                        openFileDialog.InitialDirectory = txtFilePath4210.Text;
                    }
                    //openFileDialog.Filter = "All files (*.*)|*.*";
                    openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.Multiselect = true;

                    DialogResult result = openFileDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        string[] filePaths = openFileDialog.FileNames;
                       
                        // Thêm từng đường dẫn vào danh sách fileNames
                        foreach (string filePath in filePaths)
                        {
                            directoryPathXML4210 = Path.GetDirectoryName(filePath);
                            FileItem newItem = new FileItem()
                            {
                                STATUS = 0, //chưa gửi
                                FILE_NAME = filePath
                            };
                            fileNamesXML4210.Add(newItem);
                        }
                    }
                }
                SaveFilePathHistory();
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion

        #region Methods

        private void UpdateFileStatusXML130(string filePath, short status)
        {
            // Cập nhật trạng thái file trong danh sách fileNames
            var fileItem = fileNamesXML130Send.FirstOrDefault(f => f.FILE_NAME == filePath);
            if (fileItem != null)
            {
                fileItem.STATUS = status;
            }
        }
        private void UpdateFileStatusXML4210(string filePath, short status)
        {
            // Cập nhật trạng thái file trong danh sách fileNames
            var fileItem = fileNamesXML4210Send.FirstOrDefault(f => f.FILE_NAME == filePath);
            if (fileItem != null)
            {
                fileItem.STATUS = status;
            }
        }

        private void FillDataToControl()
        {
            try
            {
                txtFilePath4210.Text = directoryPathXML4210;
                //CredirectoryPathXML4210 = directoryPathXML4210;
                gridView4210.GridControl.DataSource = fileNamesXML4210;
                gridView4210.GridControl.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToControlXML130()
        {
            try
            {
                txtFilePath130.Text = directoryPathXML130;
                //CredirectoryPathXML130 = directoryPathXML130;
                gridView130.GridControl.DataSource = fileNamesXML130;
                gridView130.GridControl.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkConfig()
        {
            try
            {
                HisConfigHSSKSYT.LoadConfig();
                string connect_infor = HisConfigHSSKSYT.HSSK_SYT__CONNECTION_INFO;
                if (string.IsNullOrEmpty(connect_infor))
                {
                    MessageBox.Show("Cấu hình liên thông dữ liệu không hợp lệ. Vui lòng khai báo cấu hình HIS.HSSK_SYT.CONNECTION_INFO trước khi thực hiện gửi tệp tin");
                    btnReset130.Enabled = false;
                    btnReset4210.Enabled = false;
                    btnChooseFile130.Enabled = false;
                    btnChooseFile4210.Enabled = false;
                    btnSend130.Enabled = false;
                    btnSend4210.Enabled = false;
                }
                else
                {
                    connectInfors = connect_infor.Split('|').ToList();
                    user = connectInfors[0];
                    pass = connectInfors[1];
                    urlLogin = connectInfors[2];
                    urlXML4210 = connectInfors[3];
                    urlXML130 = connectInfors[4];
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Key cấu hình hệ thống sai, vui lòng kiểm tra lại");
            }

        }

        private void checkLogin(string token)
        {
            btnSend130.Enabled = false;
            btnSend4210.Enabled = false;
            if (string.IsNullOrEmpty(token))
            {
                btnReset130.Enabled = false;
                btnReset4210.Enabled = false;
                btnChooseFile130.Enabled = false;
                btnChooseFile4210.Enabled = false;
            }
            else
            {
                btnReset130.Enabled = true;
                btnReset4210.Enabled = true;
                btnChooseFile130.Enabled = true;
                btnChooseFile4210.Enabled = true;
            }
            
        }

        private static async Task<string> Login(string user, string pass)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;

            using (var client = new HttpClient())
            {
                LoginData dfilter = new LoginData();
                dfilter.username = user;
                dfilter.password = ConvertStringToMD5(pass);
                //client.Timeout = new TimeSpan(0, 3, 0);
                client.Timeout = TimeSpan.FromMinutes(5);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(dfilter);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                LogSystem.Debug("Bat dau gọi api login");
                LogSystem.Debug("Địa chỉ API:  " + LogUtil.TraceData("API: ", urlLogin));
                LogSystem.Debug("Nội dung accoount login: " + LogUtil.TraceData("login: ", content));
                try
                {
                    // Gửi yêu cầu POST với form-data
                    var response = await client.PostAsync(urlLogin, content).ConfigureAwait(false);
                    LogSystem.Debug("_____Kết quả API trả về" + LogUtil.TraceData("__API__LOGIN", response));
                    LogSystem.Debug("ket thuc goi api login");
                    // Kiểm tra kết quả
                    if (response.IsSuccessStatusCode)
                    {
                        LogSystem.Debug("Gọi API login thành công");
                        Console.WriteLine("Login successfully.");
                        string responseData = response.Content.ReadAsStringAsync().Result;

                        Console.WriteLine("responseData: " + responseData);
                        LoginResuilt data = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResuilt>(responseData);
                        LogSystem.Debug("____data lay được: " + LogUtil.TraceData("token lấy được: ", data.data.access_token));
                        return data.data.access_token;
                    }
                    else
                    {
                        MessageBox.Show("Cấu hình liên thông dữ liệu không hợp lệ. Không thể đăng nhập hệ thống hồ sơ sức khỏe");
                        Console.WriteLine("Login failed.");
                        Console.WriteLine("Status code: " + response.StatusCode);
                        Console.WriteLine("Response: " + response.Content.ReadAsStringAsync());
                        return "";
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Debug("Gọi API login thất bại");
                    Inventec.Common.Logging.LogSystem.Error("Error login: " + ex);
                    Console.WriteLine("An error occurred: " + ex.Message);
                    return string.Empty;
                }
            }
        }


        private static async Task<bool> UploadFileAsync(string url, string authen, string filePath)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;

                using (var client = new HttpClient())
                {
                    using (var form = new MultipartFormDataContent())
                    {
                        byte[] fileBytes = File.ReadAllBytes(filePath);

                        form.Add(new StreamContent(new MemoryStream(fileBytes)), "file", Path.GetFileName(filePath));

                        client.DefaultRequestHeaders.Clear();
                        if (!string.IsNullOrWhiteSpace(authen))
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authen);
                        }

                        // Gửi yêu cầu POST với form-data
                        LogSystem.Debug("Bat dau gui yêu cầu upload file");
                        var response = await client.PostAsync(url, form);
                        LogSystem.Debug("Kết thúc upload file");

                        // Kiểm tra kết quả
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("File uploaded successfully.");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("File upload failed.");
                            Console.WriteLine("Status code: " + response.StatusCode);
                            Console.WriteLine("Response: " + await response.Content.ReadAsStringAsync());
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Error uploading file: " + ex);
                return false;
            }
        }

        private static string ConvertStringToMD5(string password)
        {
            string s_PasswordMD5 = string.Empty;
            try
            {
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                s_PasswordMD5 = BitConverter.ToString(hash).Replace("-", string.Empty);
            }
            catch (Exception)
            {
                LogSystem.Error("Lỗi khi convert chuỗi sang dạng mã hóa md5.");
            }
            return s_PasswordMD5;
        }

        private void SaveFilePathHistory()
        {
            try
            {
                keyFilePathHistory130 = "XML130";
                keyFilePathHistory4210 = "XML4210";
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter != null)
                {
                    if (XML130 == true)
                    {
                        if (!HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter.ContainsKey(keyFilePathHistory130))
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter.Add(keyFilePathHistory130, this.directoryPathXML130);
                        }
                        else
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter[keyFilePathHistory130] = this.directoryPathXML130;
                        }
                    }
                    else
                    {
                        if (!HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter.ContainsKey(keyFilePathHistory4210))
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter.Add(keyFilePathHistory4210, this.directoryPathXML4210);
                        }
                        else
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter[keyFilePathHistory4210] = this.directoryPathXML4210;
                        }
                    }
                }
                else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter == null)
                {
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter = new Dictionary<string, string>();
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter.Add(keyFilePathHistory130, this.directoryPathXML130);
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter.Add(keyFilePathHistory4210, this.directoryPathXML4210);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        
        private void DefaultFilePath()
        {
            try
            {
                if (GlobalVariables.dicPrinter != null )
                {
                    foreach (var item in GlobalVariables.dicPrinter)
                   {
                       if (item.Key == keyFilePathHistory130)
                       {
                           directoryPathXML130 = item.Value;
                           txtFilePath130.Text = item.Value;
                       }
                       else if (item.Key == keyFilePathHistory4210)
                       {
                           directoryPathXML4210 = item.Value;
                           txtFilePath4210.Text = item.Value;
                       }
                   }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}
