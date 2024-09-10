using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.EventLog
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
                    case Enum.Action_DangNhapThanhCong: message = MessageViResource.Action_DangNhapThanhCong;
                        break;
                    default:
                        break;
                }
            }
            else if (Language == LanguageEnum.English)
            {
                switch (enumBC)
                {
                    case Enum.Action_DangNhapThanhCong: message = MessageEnResource.Action_DangNhapThanhCong;
                        break;
                    default:
                        break;
                }
            }

            return message;
        }
    }
}
