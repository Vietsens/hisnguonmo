using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.TimeFromTo
{
    /// <summary>
    /// Doi tuong dung de tra du lieu ve
    /// </summary>
    public class TimeFromToFDO
    {
        public TimeFromToFDO()
        {
        }

        public long? FromTimeDefault { get; set; }
        public long? ToTimeDefault { get; set; }
    }
}
