using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.ApprovaleDebate.ADO
{
    /// <summary>
    /// 1 dong cua cay tab Ket qua CLS.
    /// NODE_LEVEL: 0 = moc thoi gian chi dinh, 1 = dich vu, 2 = chi so xet nghiem.
    /// </summary>
    public class SubclinicalResultADO
    {
        public const int LEVEL__INTRUCTION_TIME = 0;
        public const int LEVEL__SERVICE = 1;
        public const int LEVEL__TEST_INDEX = 2;

        public string KEY_ID { get; set; }
        public string PARENT_KEY_ID { get; set; }
        public int NODE_LEVEL { get; set; }
        public string NAME { get; set; }
        public string RESULT { get; set; }
        public string UNIT { get; set; }
        public string NORMAL_RANGE { get; set; }
        public string RESULT_TIME { get; set; }
        public string RESULT_CODE { get; set; }
        public bool IS_NO_RESULT { get; set; }
        /// <summary>
        /// Ngay chi dinh dd/MM/yyyy, dung cho dong dem ket qua tim kiem
        /// </summary>
        public string INTRUCTION_DATE { get; set; }
        /// <summary>
        /// Noi dung duoc tim: ten dich vu, ket luan, ten chi so
        /// </summary>
        public string SEARCH_TEXT { get; set; }
    }
}
