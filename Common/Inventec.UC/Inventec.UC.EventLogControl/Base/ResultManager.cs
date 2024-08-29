using DevExpress.Utils;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Base
{
    internal class ResultManager
    {
        internal static void ShowMessage(CommonParam param, bool? success)
        {
            try
            {
                if (success.HasValue)
                    Base.MessageUtil.SetResultParam(param, success.Value);
                string message = Base.MessageUtil.GetMessageAlert(param);
                if (!String.IsNullOrEmpty(message))
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, Base.MessageUtil.GetMessage(MessageLang.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DefaultBoolean.True);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
