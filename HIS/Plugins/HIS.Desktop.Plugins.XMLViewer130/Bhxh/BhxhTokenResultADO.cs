/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.XMLViewer130.Bhxh
{
    public class BhxhTokenResultADO
    {
        public string maKetQua { get; set; }
        public BhxhApiKey APIKey { get; set; }
    }

    public class BhxhApiKey
    {
        public string access_token { get; set; }
        public string id_token { get; set; }
        public string token_type { get; set; }
        public string username { get; set; }
        public string expires_in { get; set; }
    }
}
