using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EMR.Desktop.Plugins.EmrExamCategory.ADO
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RowState
    {
        UNCHANGED = 0,
        NEW = 1,
        UPDATED = 2,
        DELETED = 3
    }

    public class EMR_EXAM_CATEGORY
    {
        public long? ID { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_NAME { get; set; }
        public long NUM_ORDER { get; set; }
        public string GROUP_CODE { get; set; }
        public short? IS_ACTIVE { get; set; }
        public short? IS_DELETE { get; set; }
        public string CREATOR { get; set; }
        public string MODIFIER { get; set; }
        public string APP_CREATOR { get; set; }
        public string APP_MODIFIER { get; set; }
        public long? CREATE_TIME { get; set; }
        public long? MODIFY_TIME { get; set; }

        // Dirty tracking — sent to SaveAll API for BE to determine operation
        public RowState ROW_STATE { get; set; }

        public EMR_EXAM_CATEGORY()
        {
            ROW_STATE = RowState.UNCHANGED;
        }
    }

    public class EmrExamCategoryFilter
    {
        public long? ID { get; set; }
        public string KEY_WORD { get; set; }
        public string ORDER_FIELD { get; set; }
        public string ORDER_DIRECTION { get; set; }
    }
}
