using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace HIS.Desktop.Plugins.PatientDocumentIssued
{
    public partial class UCPatientDocumentIssued : HIS.Desktop.Utility.UserControlBase
    {
     
        //private bool IsAllSelected(TreeList tree)
        //{
         
        //    var allLeafNodes = tree.Nodes
        //        .Cast<TreeListNode>()
        //        .SelectMany(n => GetAllLeafNodes(n))
        //        .ToList();

        //    var checkedNodes = tree.GetAllCheckedNodes();

           
        //    if (allLeafNodes.Count == 0) return false;

           
        //    return checkedNodes.Count == allLeafNodes.Count;
        //}
        private bool IsAllSelected(TreeList tree)
        {
            return tree.GetAllCheckedNodes().Count > 0 && tree.GetAllCheckedNodes().Count == tree.AllNodesCount;
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
