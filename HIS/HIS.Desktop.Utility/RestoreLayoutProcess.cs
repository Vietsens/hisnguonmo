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
using DevExpress.XtraGrid.Views.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Utility
{
    internal class RestoreLayoutProcess
    {
        string ModuleLink;
        string treeListName;
        string gridViewName;
        bool isChange = false;

        internal RestoreLayoutProcess(string moduleLink)
        {
            this.ModuleLink = moduleLink;
        }

        internal void InitRestoreLayoutGridViewFromXml(DevExpress.XtraGrid.Views.Grid.GridView gridViewList, string name = "")
        {
            try
            {
                this.gridViewName = name;
                this.gridViewName = StringUtil.GetHashString(this.gridViewName);

                gridViewList.ColumnWidthChanged += new DevExpress.XtraGrid.Views.Base.ColumnEventHandler(this.gridViewDataList_ColumnWidthChanged);
                gridViewList.ColumnPositionChanged += new System.EventHandler(this.gridViewDataList_ColumnPositionChanged);
                gridViewList.ColumnFilterChanged += new System.EventHandler(this.gridViewList_ColumnFilterChanged);
                gridViewList.GridMenuItemClick += gridViewList_GridMenuItemClick;
                gridViewList.StartSorting += gridViewList_StartSorting;
                gridViewList.EndSorting += gridViewList_EndSorting;
                gridViewList.MouseUp += gridViewList_MouseUp;

                string fileName = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", System.IO.Path.Combine(this.ModuleLink, String.IsNullOrEmpty(this.gridViewName) ? gridViewList.Name : gridViewName)));

                if (System.IO.File.Exists(String.Format("{0}.xml", fileName)))
                {
                    gridViewList.RestoreLayoutFromXml(String.Format("{0}.xml", fileName));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void gridViewList_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var gridViewList = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = gridViewList.CalcHitInfo(e.Location);
            if (hi.InColumnPanel)
            {              
                this.isChange = true;
            }
            else
                this.isChange = false;
        }

        void gridViewList_EndSorting(object sender, EventArgs e)
        {
            if (this.isChange)
            {
                gridViewDataList_ColumnWidthChanged(sender, null);
                this.isChange = false;
            }
        }

        void gridViewList_StartSorting(object sender, EventArgs e)
        {
            //Inventec.Common.Logging.LogSystem.Debug("gridViewList_StartSorting.");
        }

        void gridViewList_GridMenuItemClick(object sender, DevExpress.XtraGrid.Views.Grid.GridMenuItemClickEventArgs e)
        {
            try
            {
                if (e.DXMenuItem != null && e.DXMenuItem.Tag != null && (e.DXMenuItem.Tag.Equals(DevExpress.XtraGrid.Localization.GridStringId.MenuColumnClearSorting)
                    || e.DXMenuItem.Tag.Equals(DevExpress.XtraGrid.Localization.GridStringId.MenuColumnClearAllSorting)))
                {
                    DevExpress.XtraGrid.Columns.GridColumn column = null;
                    var gridViewList = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (gridViewList == null)
                    {
                        column = sender as DevExpress.XtraGrid.Columns.GridColumn;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("column.name", column != null ? column.Name : ""));
                        gridViewList = column != null ? (column.View as DevExpress.XtraGrid.Views.Grid.GridView) : null;
                    }

                    string fileName = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", System.IO.Path.Combine(this.ModuleLink, String.Format("{0}.xml", String.IsNullOrEmpty(this.gridViewName) ? gridViewList != null ? gridViewList.Name : "" : gridViewName))));
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void gridViewList_ColumnFilterChanged(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {                
                if (!Directory.Exists(System.IO.Path.Combine(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"), this.ModuleLink)))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"), this.ModuleLink));
                }
                DevExpress.XtraGrid.Columns.GridColumn column = null;
                var gridViewList = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (gridViewList == null)
                {
                    column = sender as DevExpress.XtraGrid.Columns.GridColumn;
                    gridViewList = column != null ? (column.View as DevExpress.XtraGrid.Views.Grid.GridView) : null;
                }
                fileName = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", System.IO.Path.Combine(this.ModuleLink, String.Format("{0}.xml", String.IsNullOrEmpty(this.gridViewName) ? gridViewList != null ? gridViewList.Name : "" : gridViewName))));

                gridViewList.SaveLayoutToXml(fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileName), fileName));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDataList_ColumnWidthChanged(object sender, ColumnEventArgs e)
        {
            string fileName = "";
            try
            {
                if (!Directory.Exists(System.IO.Path.Combine(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"), this.ModuleLink)))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"), this.ModuleLink));
                }
                DevExpress.XtraGrid.Columns.GridColumn column = null;
                var gridViewList = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (gridViewList == null)
                {
                    column = sender as DevExpress.XtraGrid.Columns.GridColumn;
                    gridViewList = column != null ? (column.View as DevExpress.XtraGrid.Views.Grid.GridView) : null;
                }
                fileName = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", System.IO.Path.Combine(this.ModuleLink, String.Format("{0}.xml", String.IsNullOrEmpty(this.gridViewName) ? gridViewList != null ? gridViewList.Name : "" : gridViewName))));

                gridViewList.SaveLayoutToXml(fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileName), fileName));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDataList_ColumnPositionChanged(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {
                if (!Directory.Exists(System.IO.Path.Combine(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"), this.ModuleLink)))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"), this.ModuleLink));
                }
                DevExpress.XtraGrid.Columns.GridColumn column = null;
                var gridViewList = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (gridViewList == null)
                {
                    column = sender as DevExpress.XtraGrid.Columns.GridColumn;
                    gridViewList = column != null ? (column.View as DevExpress.XtraGrid.Views.Grid.GridView) : null;
                }
                fileName = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", System.IO.Path.Combine(this.ModuleLink, String.Format("{0}.xml", String.IsNullOrEmpty(this.gridViewName) ? gridViewList.Name : gridViewName))));
                gridViewList.SaveLayoutToXml(fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileName), fileName));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void InitRestoreLayoutTreeListFromXml(DevExpress.XtraTreeList.TreeList treeList, string name = "")
        {
            try
            {
                this.treeListName = name;
                this.treeListName = StringUtil.GetHashString(this.treeListName);
                treeList.ColumnWidthChanged += new DevExpress.XtraTreeList.ColumnWidthChangedEventHandler(this.TreeListForRestoreDataList_ColumnWidthChanged);
                treeList.ColumnPositionChanged += new System.EventHandler(this.TreeListDataList_ColumnPositionChanged);
                treeList.ColumnFilterChanged += new System.EventHandler(this.TreeListDataList_ColumnFilterChanged);
                string fileName = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", System.IO.Path.Combine(this.ModuleLink, String.Format("{0}.xml", String.IsNullOrEmpty(this.treeListName) ? treeList.Name : this.treeListName))));
                if (System.IO.File.Exists(fileName))
                {
                    treeList.RestoreLayoutFromXml(fileName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void TreeListDataList_ColumnFilterChanged(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {
                if (!Directory.Exists(System.IO.Path.Combine(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"), this.ModuleLink)))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"), this.ModuleLink));
                }
                var treeList = sender as DevExpress.XtraTreeList.TreeList;
                fileName = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", System.IO.Path.Combine(this.ModuleLink, String.Format("{0}.xml", String.IsNullOrEmpty(this.treeListName) ? treeList.Name : this.treeListName))));
                treeList.SaveLayoutToXml(fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileName), fileName));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TreeListForRestoreDataList_ColumnWidthChanged(object sender, DevExpress.XtraTreeList.ColumnChangedEventArgs e)
        {
            string fileName = "";
            try
            {
                if (!Directory.Exists(System.IO.Path.Combine(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"), this.ModuleLink)))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"), this.ModuleLink));
                }
                var treeList = sender as DevExpress.XtraTreeList.TreeList;
                fileName = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", System.IO.Path.Combine(this.ModuleLink, String.Format("{0}.xml", String.IsNullOrEmpty(this.treeListName) ? treeList.Name : this.treeListName))));
                treeList.SaveLayoutToXml(fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileName), fileName));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TreeListDataList_ColumnPositionChanged(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {
                if (!Directory.Exists(System.IO.Path.Combine(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"), this.ModuleLink)))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"), this.ModuleLink));
                }
                var column = sender as DevExpress.XtraTreeList.Columns.TreeListColumn;
                var treeList = column.TreeList;
                fileName = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", System.IO.Path.Combine(this.ModuleLink, String.Format("{0}.xml", String.IsNullOrEmpty(this.treeListName) ? treeList.Name : treeListName))));
                treeList.SaveLayoutToXml(fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileName), fileName));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
