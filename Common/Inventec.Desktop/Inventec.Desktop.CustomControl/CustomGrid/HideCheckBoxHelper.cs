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
