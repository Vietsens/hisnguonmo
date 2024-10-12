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
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using HIS.Desktop.Utility;
using LIS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    public partial class UC_ConnectionTest : UserControlBase
    {

        private bool IsAllSelected(TreeList tree)
        {
            return tree.GetAllCheckedNodes().Count > 0 && tree.GetAllCheckedNodes().Count == tree.AllNodesCount;
        }

        private bool IsAllSelected(GridView grid)
        {
            List<LisSampleADO> data = null;
            if (grid.DataSource != null)
            {
                data = (List<LisSampleADO>)grid.DataSource;
            }

            return data != null && data.Count == data.Count(o => o.IsCheck);
        }
        private bool IsAllSelectedReRun(TreeList tree)
        {
            List<TestLisResultADO> data = null;
            BindingList<TestLisResultADO> bindingList;
            if (tree.DataSource != null && tree.DataSource is BindingList<TestLisResultADO>)
            {
                bindingList = (BindingList<TestLisResultADO>)tree.DataSource;
                data = bindingList.ToList();
            }

            return data != null && data.Count(o => o.IS_PARENT == 1 && o.IS_RUNNING == 1) == data.Count(o => o.IS_PARENT == 1 && o.IS_RUNNING == 1 && o.RERUN == true);
        }

        protected void DrawCheckBox(GraphicsCache cache, RepositoryItemCheckEdit edit, Rectangle r, bool Checked)
        {
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info;
            DevExpress.XtraEditors.Drawing.CheckEditPainter painter;
            DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args;
            info = edit.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            painter = edit.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
            info.EditValue = Checked;
            info.Bounds = r;
            info.CalcViewInfo();
            args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, r);
            painter.Draw(args);
        }

        private void EmbeddedCheckBoxChecked(TreeList tree)
        {
            try
            {
                if (IsAllSelected(tree))
                {
                    tree.BeginUpdate();
                    tree.NodesIterator.DoOperation(new UnSelectNodeOperation());
                    tree.EndUpdate();
                }
                else
                {
                    tree.BeginUpdate();
                    tree.NodesIterator.DoOperation(new SelectNodeOperation());
                    tree.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<TestLisResultADO> EmbeddedCheckBoxCheckedReRun(TreeList tree, ref bool checkAll)
        {
            List<TestLisResultADO> data = null;
            BindingList<TestLisResultADO> bindingList;
            try
            {
                tree.BeginUpdate();
                if (IsAllSelectedReRun(tree))
                {
                    
                    checkAll = false;
                    if (tree != null && tree.DataSource != null && tree.DataSource is BindingList<TestLisResultADO>)
                    {
                        bindingList = ( BindingList<TestLisResultADO>)tree.DataSource;
                        data =  bindingList.Where(o => o.IS_PARENT == 1 && o.IS_RUNNING == 1).ToList();
                    }

                    if (data != null && data.Count > 0)
                    {
                        data.ForEach(o => o.RERUN = false);
                    }
                }
                else
                {
                    if (tree != null && tree.DataSource != null && tree.DataSource is BindingList<TestLisResultADO> )
                    {
                        bindingList = (BindingList<TestLisResultADO>)tree.DataSource;
                        data = bindingList.Where(o => o.IS_PARENT == 1 && o.IS_RUNNING == 1).ToList();
                    }

                    if (data != null && data.Count > 0)
                    {
                        data.ForEach(o => o.RERUN = true);
                        checkAll = true;
                    }
                }
                tree.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return data;
        }
        private void EmbeddedCheckBoxChecked(GridView grid)
        {
            try
            {
                grid.BeginUpdate();
                if (IsAllSelected(grid))
                {
                    List<LisSampleADO> data = null;
                    if (grid != null)
                    {
                        data = (List<LisSampleADO>)grid.DataSource;
                    }

                    if (data != null && data.Count > 0)
                    {
                        data.ForEach(o => o.IsCheck = false);
                    }
                }
                else
                {
                    List<LisSampleADO> data = null;
                    if (grid != null)
                    {
                        data = (List<LisSampleADO>)grid.DataSource;
                    }

                    if (data != null && data.Count > 0)
                    {
                        data.ForEach(o => o.IsCheck = true);
                    }
                }

                grid.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        class SelectNodeOperation : TreeListOperation
        {
            public override void Execute(TreeListNode node)
            {
                node.Checked = true;
            }
        }

        class UnSelectNodeOperation : TreeListOperation
        {
            public override void Execute(TreeListNode node)
            {
                node.Checked = false;
            }
        }
    }
}
