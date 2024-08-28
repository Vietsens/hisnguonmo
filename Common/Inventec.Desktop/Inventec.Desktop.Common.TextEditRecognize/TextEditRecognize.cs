using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Desktop.Common.TextEditRecognize
{
    public class TextEditRecognize : TextEdit
    {
        Action<string> actUpdateInputAfterRegconize;
        public void SetParamUpdateInputAfterRegconize(Action<string> __actUpdateInputAfterRegconize, string wit_Ai_Access_Token, int timereplay)
        {
            this.actUpdateInputAfterRegconize = __actUpdateInputAfterRegconize;
            Inventec.Common.WitAI.Vitals.Constan.SetConstan(wit_Ai_Access_Token, timereplay);
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            if (keyData == Keys.F8)
            {
                Inventec.Common.WitAI.Form1 f1 = new Inventec.Common.WitAI.Form1(actUpdateInputAfterRegconize);
                f1.ShowDialog();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
