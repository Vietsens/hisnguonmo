using Inventec.UC.Login.Design.Template1;
using Inventec.UC.Login.Design.Template2;
using Inventec.UC.Login.Design.Template3;
using Inventec.UC.Login.UCD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Init
{
    class InitUC : IInitUC
    {
        public System.Windows.Forms.UserControl Init(MainLogin.EnumTemplate Template, InitUCD Data)
        {
            System.Windows.Forms.UserControl result = null;
            try
            {
                if (Template == MainLogin.EnumTemplate.TEMPLATE1)
                {
                    result = new Template1(Data);
                }
                else if (Template == MainLogin.EnumTemplate.TEMPLATE2)
                {
                    result = new Template2(Data);
                }
                else if (Template == MainLogin.EnumTemplate.TEMPLATE3)
                {
                    result = new Template3(Data);
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }
    }
}
