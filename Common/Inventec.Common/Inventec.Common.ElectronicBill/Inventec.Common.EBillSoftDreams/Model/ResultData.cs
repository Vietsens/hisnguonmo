using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.EBillSoftDreams.Model
{
    internal class ResultData<T>
    {
        /// <summary>
        /// -	Status 2: thành công.
        /// -	Status 4: Lỗi từ phía client.
        /// -	Status 5: Lỗi từ phía server.
        /// </summary>
        public long Status { get; set; }

        /// <summary>
        /// Nội dung lỗi
        /// </summary>
        public string Message { get; set; }

        public T Data { get; set; }
    }
}
