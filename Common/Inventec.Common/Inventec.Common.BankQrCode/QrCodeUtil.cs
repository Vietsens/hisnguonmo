using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.BankQrCode
{
    class QrCodeUtil
    {
        internal static string ProcessConvertAmount(decimal amount)
        {
            long longAmount = long.Parse(Math.Round(amount, 0, MidpointRounding.AwayFromZero).ToString());
            //làm tròn lên
            //lơn hơn 5 đã làm tròn lên rồi nên không cộng.
            //nhỏ hơm 5 là làm tròn xuống nên cộng thêm 1 đơn vị
            if (amount - longAmount > 0)
            {
                longAmount++;
            }

            return longAmount.ToString();
        }
    }
}
