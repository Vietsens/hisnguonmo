using DevExpress.Utils;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.Process
{
    class ResultManager
    {
        public static void ShowMessage(CommonParam param, bool? success)
        {
            try
            {
                if (success.HasValue)
                    MessageUtil.SetResultParam(param, success.Value);
                string message = MessageUtil.GetMessageAlert(param);
                if (!String.IsNullOrEmpty(message))
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, MessageUtil.GetMessage(Message.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DefaultBoolean.True);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
