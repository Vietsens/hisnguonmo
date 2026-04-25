using System.Collections.Generic;

namespace EMR.Desktop.Plugins.EmrExamCategory.ADO
{
    /// <summary>
    /// Item TDO cho category trong SaveAll request.
    /// Convention theo tài liệu spec:
    ///   - NEW      → ID <= 0 hoặc NULL (FE dùng temp ID âm để rule tham chiếu)
    ///   - UPDATED  → ID > 0
    ///   - DELETED  → ID > 0, ROW_STATE = "DELETED"
    ///   - UNCHANGED → BE bỏ qua
    /// </summary>
    public class EmrExamCategoryTDO
    {
        public long? ID { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_NAME { get; set; }
        public long? NUM_ORDER { get; set; }
        public string ROW_STATE { get; set; }
    }

    /// <summary>
    /// Item TDO cho document pair rule trong SaveAll request.
    /// EXAM_CATEGORY_ID có thể là temp ID âm nếu reference category NEW cùng request.
    /// BE sẽ map lại sang ID thực sau khi create category.
    /// </summary>
    public class EmrDocumentPairRuleTDO
    {
        public long? ID { get; set; }
        public string PATTERN { get; set; }
        public string MATCH_TYPE { get; set; }
        public string KEY_EXTRACTOR { get; set; }
        public long? EXAM_CATEGORY_ID { get; set; }
        public long? NUM_ORDER { get; set; }
        public string ROW_STATE { get; set; }
    }

    /// <summary>
    /// Request TDO for api/EmrExamCategory/SaveAll — theo tài liệu spec.
    /// Lưu toàn bộ 2 grid (Loại XN + Rule ghép cặp) trong 1 transaction.
    /// </summary>
    public class EmrExamCategorySaveAllSDO
    {
        public List<EmrExamCategoryTDO> Categories { get; set; }
        public List<EmrDocumentPairRuleTDO> Rules { get; set; }

        public EmrExamCategorySaveAllSDO()
        {
            Categories = new List<EmrExamCategoryTDO>();
            Rules = new List<EmrDocumentPairRuleTDO>();
        }
    }

    /// <summary>
    /// Response TDO — BE trả về danh sách categories + rules đã lưu kèm ID thực.
    /// </summary>
    public class EmrExamCategorySaveAllResultSDO
    {
        public List<EMR.EFMODEL.DataModels.EMR_EXAM_CATEGORY> Categories { get; set; }
        public List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_PAIR_RULE> Rules { get; set; }
    }
}
