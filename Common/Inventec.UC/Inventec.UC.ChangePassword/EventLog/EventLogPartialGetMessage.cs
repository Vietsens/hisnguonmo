using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.EventLog
{
    internal partial class EventLog
    {
        private string GetMessage(Enum enumBC)
        {
            string message = "";

            if (Language == LanguageEnum.Vietnamese)
            {
                switch (enumBC)
                {
                    case Enum.Action_DoiMatKhauThanhCong: message = MessageViResource.Action_DoiMatKhauThanhCong;
                        break;
                    default:
                        break;
                }
            }
            else if (Language == LanguageEnum.English)
            {
                switch (enumBC)
                {
                    case Enum.Action_DoiMatKhauThanhCong: message = MessageEnResource.Action_DoiMatKhauThanhCong;
                        break;
                    default:
                        break;
                }
            }

            return message;
        }
    }
}
