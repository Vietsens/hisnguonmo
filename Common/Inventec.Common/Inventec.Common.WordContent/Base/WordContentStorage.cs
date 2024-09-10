using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WordContent.Base
{
    public class WordContentStorage
    {
        private static Inventec.Common.WebApiClient.ApiConsumer sdaConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer SdaConsumer
        {
            get
            {
                return sdaConsumer;
            }
            set
            {
                sdaConsumer = value;
            }
        }
        
        private static Inventec.Common.WebApiClient.ApiConsumer sarConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer SarConsumer
        {
            get
            {
                return sarConsumer;
            }
            set
            {
                sarConsumer = value;
            }
        }

        internal const string NguoiDungNhapDuLieuKhongHopLe = "Dữ liệu không hợp lệ";
    }
}
