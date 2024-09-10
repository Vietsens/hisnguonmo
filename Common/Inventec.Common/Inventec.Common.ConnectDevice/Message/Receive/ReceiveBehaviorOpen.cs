using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ConnectDevice.Message.Receive
{
    class ReceiveBehaviorOpen : IReceive
    {
        string[] element;

        internal ReceiveBehaviorOpen(string[] data)
        {
            element = data;
        }

        async Task IReceive.Run(ConnectToDevice connectDevice)
        {
            try
            {
                if (element == null || element.Length == 0)
                    return;
                if (!element[0].Equals(connectDevice.connectStore.messageIdOpen))
                {
                    if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, "Message id thiết bị gửi lên không trùng với phần mềm gửi xuống.", ConnectToDevice.EnumConnect.CONNECT);
                    return;
                }
                connectDevice.connectStore.messageIdOpen = null;
                string responseCode = element[1];
                if (string.IsNullOrEmpty(responseCode))
                {
                    if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, "Không xác định được responseCode.", ConnectToDevice.EnumConnect.CONNECT);
                    return;
                }
                if (responseCode.Equals(Store.ConnectConstant.RESPONSE_SUCCESS))
                {
                    if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(true, "Xử lý thành công", ConnectToDevice.EnumConnect.CONNECT);
                }
                else
                {
                    string message = "";
                    if (responseCode.Equals("-1"))
                    {
                        message = "Mã ứng dụng không hợp lệ, Phần mềm tự động gửi lại bản tin disconect thiết bị";
                    }
                    else if (responseCode.Equals("-2"))
                    {
                        message = "Không có quyền mở ứng dụng, Phần mềm tự động gửi lại bản tin disconect thiết bị";
                    }
                    else
                    {
                        message = responseCode + ".";
                    }
                    if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, message, ConnectToDevice.EnumConnect.CONNECT);

                    Send.SendBehaviorFactory.MakeISend(Send.SendBehaviorFactory.EnumSend.DISCONNECT).Run(connectDevice);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, "Có exception xẩy ra trong quá trình xử lý bản tin trả lời OPEN.", ConnectToDevice.EnumConnect.CONNECT);
            }
        }
    }
}
