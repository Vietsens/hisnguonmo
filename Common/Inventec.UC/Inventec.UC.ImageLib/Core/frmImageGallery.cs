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
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;

namespace Inventec.UC.ImageLib.Core
{
    public partial class frmImageGallery : DevExpress.XtraEditors.XtraForm
    {
        #region Public variable
        public delegate void DelegateConnectCamera();
        public delegate void DelegateChooseImage(string imageUrl);
        DelegateConnectCamera ConnectCamera;
        DelegateChooseImage ChooseImage;
        #endregion

        public frmImageGallery()
        {
            InitializeComponent();
        }

        public frmImageGallery(DelegateConnectCamera connectCamera, DelegateChooseImage chooseImage)
        {
            InitializeComponent();
            this.ConnectCamera = connectCamera;
            this.ChooseImage = chooseImage;
        }

        private void btnConnect_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ConnectCamera != null)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        void InitData()
        {
            treeList1.DataSource = new object();
        }

        private void frmImageGallery_Load(object sender, EventArgs e)
        {
            try
            {
                InitData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        bool loadDrives = false;

        private void treeList1_VirtualTreeGetChildNodes(object sender, DevExpress.XtraTreeList.VirtualTreeGetChildNodesInfo e)
        {
            Cursor current = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            if (!loadDrives)
            {
                string[] roots = Directory.GetLogicalDrives();
                e.Children = roots;
                loadDrives = true;
            }
            else
            {
                try
                {
                    string path = (string)e.Node;
                    if (Directory.Exists(path))
                    {
                        string[] dirs = Directory.GetDirectories(path);
                        string[] files = Directory.GetFiles(path);
                        string[] arr = new string[dirs.Length + files.Length];
                        dirs.CopyTo(arr, 0);
                        //files.CopyTo(arr, dirs.Length);
                        e.Children = arr;
                    }
                    else e.Children = new object[] { };
                }
                catch { e.Children = new object[] { }; }
            }
            Cursor.Current = current;
        }

        private void treeList1_VirtualTreeGetCellValue(object sender, DevExpress.XtraTreeList.VirtualTreeGetCellValueInfo e)
        {
            DirectoryInfo di = new DirectoryInfo((string)e.Node);
            if (e.Column == colName) e.CellData = di.Name;
            if (e.Column == colType)
            {
                if (IsDrive((string)e.Node)) e.CellData = "Drive";
                else if (!IsFile(di))
                    e.CellData = "Folder";
                else
                    e.CellData = "File";
            }
            if (e.Column == colSize)
            {
                if (IsFile(di))
                {
                    e.CellData = new FileInfo((string)e.Node).Length;
                }
                else e.CellData = null;
            }
        }

        bool IsFile(DirectoryInfo info)
        {
            try
            {
                return (info.Attributes & FileAttributes.Directory) == 0;
            }
            catch
            {
                return false;
            }
        }
        bool IsDrive(string val)
        {
            string[] drives = Directory.GetLogicalDrives();
            foreach (string drive in drives)
            {
                if (drive.Equals(val)) return true;
            }
            return false;
        }

        private void treeList1_GetStateImage(object sender, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            if (e.Node.GetDisplayText("Type") == "Folder")
                e.NodeImageIndex = e.Node.Expanded ? 1 : 0;
            else if (e.Node.GetDisplayText("Type") == "File") e.NodeImageIndex = 2;
            else e.NodeImageIndex = 3;
        }

        private string GetSelectedNode(DevExpress.XtraTreeList.TreeList trvTreeList)
        {
            try
            {
                return trvTreeList.FocusedNode[trvTreeList.Columns["Name"]].ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
            return "";
        }

        private void treeList1_NodeChanged(object sender, DevExpress.XtraTreeList.NodeChangedEventArgs e)
        {
            try
            {
                string pathFolder = GetSelectedNode(treeList1);
                if (!string.IsNullOrEmpty(pathFolder))
                {
                    string[] files = Directory.GetFiles(pathFolder);

                    foreach (var item in files)
                    {
                        TileItem tileNew = new TileItem();
                        tileNew.Name = item + "";
                        tileNew.Text = item + "";
                        tileNew.Tag = item;
                        tileNew.AppearanceItem.Normal.ForeColor = Color.White;
                        tileNew.TextAlignment = TileItemContentAlignment.MiddleCenter;
                        tileNew.ItemSize = TileItemSize.Medium;
                        tileNew.AppearanceItem.Normal.BackColor = Color.Green;
                        tileNew.AppearanceItem.Normal.BorderColor = Color.Black;                      
                        tileNew.Visible = true;
                        tileNew.ItemDoubleClick += frmChooseImage_ItemDoubleClick;
                        tileGroup1.Items.Add(tileNew);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }
        }

        void frmChooseImage_ItemDoubleClick(object sender, TileItemEventArgs e)
        {
            try
            {
                var file = (string)(e.Item.Tag);
                if (file != null)
                {
                    this.ChooseImage(file);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        //</treeList1>
    }
}
