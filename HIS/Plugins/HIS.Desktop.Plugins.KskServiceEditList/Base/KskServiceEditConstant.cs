namespace HIS.Desktop.Plugins.KskServiceEditList.Base
{
    internal class KskServiceEditConstant
    {
        /// <summary>Số hồ sơ mỗi lần gọi API ServiceEdit (backend giới hạn tối đa 200)</summary>
        internal const int BATCH_SIZE = 200;

        /// <summary>Số id tối đa mỗi lần lấy sere_serv theo TREATMENT_IDs</summary>
        internal const int QUERY_CHUNK_SIZE = 100;
    }
}
