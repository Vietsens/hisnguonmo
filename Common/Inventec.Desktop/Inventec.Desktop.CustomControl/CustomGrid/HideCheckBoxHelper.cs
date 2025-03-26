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
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.CustomControl.CustomGrid
{
    public class HideCheckBoxHelper
    {
        public delegate bool CanCheckNodeDelegate(TreeListNode node);
        CanCheckNodeDelegate canCheckNode;
        public HideCheckBoxHelper(TreeList treeList)
        {
            _TreeList = treeList;
            treeList.CustomDrawNodeCheckBox += treeList_CustomDrawNodeCheckBox;
            treeList.BeforeCheckNode += treeList_BeforeCheckNode;
        }
        public HideCheckBoxHelper(TreeList treeList, CanCheckNodeDelegate _canCheckNode)
        {
            _TreeList = treeList;
            treeList.CustomDrawNodeCheckBox += treeList_CustomDrawNodeCheckBox;
            treeList.BeforeCheckNode += treeList_BeforeCheckNode;
            canCheckNode = _canCheckNode;
        }

        private int _Level = 2;
        private TreeList _TreeList;
        public int Level
        {
            get { return _Level; }
            set { _Level = value; _TreeList.Refresh(); }
        }

        private bool _Hide = true;
        public bool NeedHide
        {
            get { return _Hide; }
            set { _Hide = value; _TreeList.Refresh(); }
        }

        private bool CanCheckNode(TreeListNode node)
        {
            if (canCheckNode!=null)
            {
                return canCheckNode(node);
            }
            return true;
        }

        void treeList_BeforeCheckNode(object sender, CheckNodeEventArgs e)
        {
            e.CanCheck = CanCheckNode(e.Node);
        }

        void treeList_CustomDrawNodeCheckBox(object sender, CustomDrawNodeCheckBoxEventArgs e)
        {
            bool canCheckNode = CanCheckNode(e.Node);
            if (canCheckNode)
                return;
            e.ObjectArgs.State = DevExpress.Utils.Drawing.ObjectState.Disabled;
            e.Handled = NeedHide;
        }
    }
}
