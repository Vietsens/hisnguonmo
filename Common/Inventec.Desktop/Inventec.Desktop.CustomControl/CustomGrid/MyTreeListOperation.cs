using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.Nodes;
using System.Windows.Forms;

namespace Inventec.Desktop.CustomControl.CustomGrid
{
    public class MyTreeListOperation : TreeListOperation
    {
        TreeListNode _node;
        public delegate void CheckNodeDelegate(TreeListNode node);
        CheckNodeDelegate checkNode;

        public MyTreeListOperation(TreeListNode node, CheckNodeDelegate _checkNode)
        {
            try
            {
                this._node = node;
                this.checkNode = _checkNode;
            }
            catch (Exception ex)
            {
                
            }
        }

        public override void Execute(DevExpress.XtraTreeList.Nodes.TreeListNode node)
        {
            try
            {
                if (this.checkNode == null) return;
                if (node.Visible == false) return;
                this.checkNode(node);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
