using Newtonsoft.Json;

namespace EMR.Desktop.Plugins.EmrExamCategory.ADO
{
    public class EMR_DOCUMENT_PAIR_RULE
    {
        public long? ID { get; set; }
        public string PATTERN { get; set; }
        public string MATCH_TYPE { get; set; }
        public string KEY_EXTRACTOR { get; set; }
        public long? EXAM_CATEGORY_ID { get; set; }
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

        public EMR_DOCUMENT_PAIR_RULE()
        {
            ROW_STATE = RowState.UNCHANGED;
        }
    }

    public class EmrDocumentPairRuleFilter
    {
        public long? ID { get; set; }
        public long? EXAM_CATEGORY_ID { get; set; }
        public string KEY_WORD { get; set; }
        public string ORDER_FIELD { get; set; }
        public string ORDER_DIRECTION { get; set; }
    }
}
