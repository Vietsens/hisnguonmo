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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using Inventec.Aup.Utility;
using HIS.Desktop.ADO;
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;

namespace HIS.Desktop.Plugins.UploadFileAUP
{
    public partial class UpdateAUP : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Module currentModule;
        List<NodeADO> NodeADOs = new List<NodeADO>();

        #endregion

        public UpdateAUP(Module _Module)
            : base(_Module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _Module;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLoadDirectory_Click(object sender, EventArgs e)
        {
            try
            {
                //if (string.IsNullOrEmpty(txtDirectoryPath.Text))
                //{
                 FolderBrowserDialog fbd = new FolderBrowserDialog();
                //XtraFolderBrowserDialog fbd = new XtraFolderBrowserDialog();
                fbd.ShowNewFolderButton = false;
               
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtDirectoryPath.Text = fbd.SelectedPath;

                    TreeList.BeginUpdate();
                    TreeList.DataSource = null;
                    TreeList.EndUpdate();
                    PathStoreWorker.ChangeWorkingPath(fbd.SelectedPath);
                    ProcessLoadDirectory();
                }
                //}
                //else
                //{
                //    ProcessLoadDirectory();
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessLoadDirectory()
        {
            try
            {
                btnSave.Enabled = false;
                progressBar1.Value = 0;
                toolTip1.ShowAlways = true;
                if (!String.IsNullOrEmpty(txtDirectoryPath.Text) && Directory.Exists(txtDirectoryPath.Text))
                    LoadDirectory(txtDirectoryPath.Text);
                else
                    MessageBox.Show("Select Directory!!");
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                btnSave.Enabled = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadDirectory(string Dir)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(Dir);

                progressBar1.Maximum = Directory.GetFiles(Dir, "*.*", SearchOption.AllDirectories).Length + Directory.GetDirectories(Dir, "**", SearchOption.AllDirectories).Length;
                NodeADOs.Clear();
                //NodeADOs.Add(new NodeADO()
                //{
                //    IsFolder = true,
                //    Name = di.Name,
                //    Path = di.FullName,
                //    ParentPath = ""
                //});

                LoadFiles(Dir, "");
                LoadSubDirectories(Dir, "");

                TreeList.KeyFieldName = "Path";
                TreeList.ParentFieldName = "ParentPath";
                TreeList.DataSource = new BindingList<NodeADO>(NodeADOs);
                TreeList.ExpandAll();

              //  progressBar1.Value = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSubDirectories(string dir, string parentPath)
        {
            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(dir);

                foreach (string subdirectory in subdirectoryEntries)
                {
                    DirectoryInfo di = new DirectoryInfo(subdirectory);
                    NodeADOs.Add(new NodeADO()
                    {
                        IsFolder = true,
                        Name = di.Name,
                        Path = di.FullName,
                        ParentPath = parentPath
                    });

                    LoadFiles(subdirectory, di.FullName);
                    LoadSubDirectories(subdirectory, di.FullName);
                    UpdateProgress();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void LoadFiles(string dir, string parentPath)
        {
            try
            {
                string[] Files = Directory.GetFiles(dir, "*.*");
                for (int i = 0; i < Files.Count(); i++)
                {
                    if (File.Exists(Files[i]))
                    {
                        FileInfo fi = new FileInfo(Files[i]);

                        NodeADOs.Add(new NodeADO()
                        {
                            IsFolder = false,
                            Name = fi.Name,
                            Path = fi.FullName,
                            ImageIndex = 1,
                            ParentPath = parentPath
                        });
                        UpdateProgress();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateProgress()
        {
            if (progressBar1.Value < progressBar1.Maximum)
            {
                progressBar1.Value++;
                // int percent = (int)(((double)progressBar1.Value / (double)progressBar1.Maximum) * 100);
                // progressBar1.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)10, FontStyle.Regular), Brushes.Black, new PointF(progressBar1.Width / 2 - 10, progressBar1.Height / 2 - 7));
                Application.DoEvents();
            }
        }

        private void treeList1_GetStateImage(object sender, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            try
            {
                var data = (NodeADO)TreeList.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    e.NodeImageIndex = data.ImageIndex;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            List<FileUploadInfo> files = new List<FileUploadInfo>();
           
            try
            {
                //if (NodeADOs.Select(o => o.Name) == null || NodeADOs.Select(o => o.Name).Count() == 0)
                //{
                //    if (MessageBox.Show("Chưa chọn file/folder cần upload lên AUP!", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                //    {
                //        // return;
                //    }
                //   // MessageBox.Show("Chưa chọn file/folder cần upload lên AUP!", "", MessageBoxButtons.OK, MessageBoxIcon.Error) ;
                //}
                //else
                //{
                WaitingManager.Show();
                List<NodeADO> NodeADOss = new List<NodeADO>();
                NodeADOss = GetParentSelectedIntree();
                foreach (var item in NodeADOss)
                {
                    FileUploadInfo file = new FileUploadInfo();
                    if (!item.IsFolder)
                    {
                        byte[] fileContents1 = File.ReadAllBytes(item.Path);
                        
                        FileAttributes attr = File.GetAttributes(@item.Path);
                        if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                        {
                            file.Base64FileData = Convert.ToBase64String(fileContents1);
                        }
                    }

                    file.OriginalName = item.Name;
                    file.Url = item.Path.Replace(txtDirectoryPath.Text, "");
                    files.Add(file);
                    //files.Clear()
                }
                try
                {
                    if (files == null || files.Count() == 0)
                    {
                        MessageBox.Show("Chưa chọn file/folder cần upload lên AUP!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        btnSave.Enabled = false;
                        var rs = Inventec.Aup.Client.FileUpload.UploadFile(txtAppCode.Text, files);  //EMR_MAIN HIS_TEST //.Where(x => x != null).ToList()
                        if (rs != null && rs.Count > 0)
                        {
                            WaitingManager.Hide();
                            btnSave.Enabled = true;
                            Inventec.Desktop.Common.Message.MessageManager.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBKQXLYCCuaFrontendThanhCong));
                        }
                        else
                        {
                            WaitingManager.Hide();
                            btnSave.Enabled = true;
                            Inventec.Desktop.Common.Message.MessageManager.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBKQXLYCCuaFrontendThatBai));
                        }
                           
                    }
                }

                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                
                // }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }


        }

        private void treeList1_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateAUP_Load(object sender, EventArgs e)
        {
            try
            {
                TreeList.StateImageList = imageCollection1;
                var path = PathStoreWorker.GetWorkingPath();
                if (!String.IsNullOrEmpty(path))
                {
                    txtDirectoryPath.Text = path;
                    // ProcessLoadDirectory();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDirectoryPath_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var path = txtDirectoryPath.Text;
                if (!String.IsNullOrEmpty(path) && Directory.Exists(path))
                {
                    PathStoreWorker.ChangeWorkingPath(path);
                    ProcessLoadDirectory();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDirectoryPath_EditValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    var path = txtDirectoryPath.Text;
            //    if (!String.IsNullOrEmpty(path) && Directory.Exists(path))
            //    {
            //        PathStoreWorker.ChangeWorkingPath(path);
            //        ProcessLoadDirectory();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDirectoryPath.Text) && Directory.Exists(txtDirectoryPath.Text))
            {
                WaitingManager.Show();
                TreeList.BeginUpdate();
                TreeList.DataSource = null;
                TreeList.EndUpdate();
                PathStoreWorker.ChangeWorkingPath(txtDirectoryPath.Text);
                //treeList1_GetStateImage(null, null);

                ProcessLoadDirectory();
                this.Refresh();
                WaitingManager.Hide();

            }

        }

        private void UpdateAUP_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if ()
            //{

            //}
        }

        private void UpdateAUP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.Enter)
            {
                btnLoad.Focus();
                btnLoad.Select();
                // btnLoad
                btnLoad_Click(null, null);
            }
        }

        bool lockAfterCheck = false;
        bool lockSelectionChanged = false;
        private void TreeList_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            //if (lockAfterCheck) return;
            //lockSelectionChanged = true;
            //TreeList tl = sender as TreeList;
            //if (e.Node.Checked)
            //    tl.Selection.Add(e.Node);
            //else
            //    tl.Selection.Remove(e.Node);
            //lockSelectionChanged = false;
        }

        private void TreeList_SelectionChanged(object sender, EventArgs e)
        {
            //if (lockSelectionChanged)
            //    return;
            //TreeList tl = sender as TreeList;
            //lockAfterCheck = true;
            //tl.UncheckAll();
            //foreach (DevExpress.XtraTreeList.Nodes.TreeListNode treeListNode in tl.Selection)
            //    treeListNode.Checked = true;
            //lockAfterCheck = false;
        }
        private List<NodeADO> GetParentSelectedIntree()
        {
            List<NodeADO> parentNodes = new List<NodeADO>();
            var nodeCheckeds = this.TreeList.GetAllCheckedNodes();
            if (nodeCheckeds != null && nodeCheckeds.Count > 0)
            {
                //lay data cua cac dong tuong ung voi cac node duoc check
                foreach (var node in nodeCheckeds)
                {
                    var data = this.TreeList.GetDataRecordByNode(node) as NodeADO;
                    if (data != null)
                    {
                        parentNodes.Add(data);
                    }
                }
            }
            return parentNodes;
        }
    }
}
