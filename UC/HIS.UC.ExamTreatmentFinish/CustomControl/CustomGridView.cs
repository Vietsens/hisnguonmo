using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ExamTreatmentFinish.CustomControl
{
    public class CustomGridView : Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn
    {
        public override void EndInit()
        {
            base.EndInit();
            if (!this.DesignMode)
            {
                if (base.GridControl == null || base.GridControl.LevelTree.Nodes.Count == 0)
                {
                    base.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
                }
            }
        }
        protected override bool IsAllowPixelScrollingAutoRowHeight
        {
            get
            {
                return base.OptionsBehavior.AllowPixelScrolling == DevExpress.Utils.DefaultBoolean.True;
            }
        }
    }

}
