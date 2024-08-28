using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ConnectDevice.Message.Receive
{
    class ReceiveBehaviorDisconnect : IReceive
    {
        string[] element;

        internal ReceiveBehaviorDisconnect(string[] data)
        {
            element = data;
        }

        async Task IReceive.Run(ConnectToDevice connectDevice)
        {
            try
            {
                if (element == null || element.Length == 0)
                    return;
                if (!element[0].Equals(connectDevice.connectStore.messageIdDisconnect))
                {
                    if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, "Message id thiết bị gửi lên không trùng với phần mềm gửi xuống.", ConnectToDevice.EnumConnect.CONNECT);
                    return;
                }

                connectDevice.connectStore.messageIdDisconnect = null;
                string responseCode = element[1];
                if (string.IsNullOrEmpty(responseCode))
                {
                    if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, "Không xác định được responseCode.", ConnectToDevice.EnumConnect.DISCONNECT);
                    return;
                }
                if (responseCode.Equals(Store.ConnectConstant.RESPONSE_SUCCESS))
                {
                    connectDevice.Close();
                    if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(true, "Xử lý thành công", ConnectToDevice.EnumConnect.DISCONNECT);      
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, "Có exception xẩy ra trong quá trình xử lý bản tin trả lời DISCONNECT.", ConnectToDevice.EnumConnect.DISCONNECT);
            }
        }
    }
}
