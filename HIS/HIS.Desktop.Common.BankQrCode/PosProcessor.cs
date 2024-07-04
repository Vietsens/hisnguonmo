using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Common.BankQrCode
{
    public enum SendType
    {
        QR,
        Text
    }
    public class PosProcessor : SerialPort
    {
        private const int QR_LENGTH = 28;
        private const int A_LENGTH = 17;
        private const string QR_FORMAT = "\"C\":\"03\",\"A\":\"0.01\",\"D\":\"{0}\"";
        private const string A_FORMAT = "\"C\":\"04\",\"T\":\"{0}\"";
        private const string RESULT_FORMAT = "MF0019{\"C\":\"03\",\"R\":\"00\"}ED";
        public SendType typeSend = SendType.QR;
        private string MessageError = null;
        public PosProcessor(string portName)
        {
            this.PortName = portName;
            this.DataReceived += PosProcessor_DataReceived;
        }

        private void PosProcessor_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort com = sender as SerialPort;
                int a = com.BytesToRead;
                byte[] buffer = new byte[a];
                com.Read(buffer, 0, a);
                MessageError = System.Text.Encoding.ASCII.GetString(buffer) == RESULT_FORMAT ? "Kết nối tới thiết bị IPOS thành công" : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public bool ConnectPort(ref string messageError)
        {
            try
            {
                this.Open();
                if (this.IsOpen)
                {
                    this.Send(null);
                    Thread.Sleep(2000);
                    if (!string.IsNullOrEmpty(MessageError))
                    {
                        messageError = MessageError;
                        return true;
                    }
                }
                messageError = string.Format("Kết nối tới thiết bị IPOS thất bại. Vui lòng kiểm tra lại kết nối cổng {0}", this.PortName);
            }
            catch (Exception ex)
            {
                messageError = ex.ToString();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        public void Send(string dataSend)
        {
            try
            {
                MessageError = null;
                if (this.IsOpen)
                {
                    dataSend = dataSend ?? "";
                    switch (typeSend)
                    {
                        case SendType.QR:
                            dataSend = string.Format("MF{0}", string.Format("{0:0000}", QR_LENGTH + dataSend.Length)) + "{" + string.Format(QR_FORMAT, dataSend) + "}" + "ED";
                            break;
                        case SendType.Text:
                            dataSend = string.Format("MF{0}", string.Format("{0:0000}", A_LENGTH + dataSend.Length)) + "{" + string.Format(A_FORMAT, dataSend) + "}" + "ED";
                            break;
                        default:
                            break;
                    }
                    this.Write(dataSend);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void DisposePort()
        {

            try
            {
                if (this.IsOpen)
                {
                    this.Send(null);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
