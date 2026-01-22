namespace HIS.Desktop.Plugins.Register.ADO
{
    public class CustomerSourceDetailADO
    {
        public string LOGINNAME { get; set; }
        public string USERNAME { get; set; }
        public long CUSTOMER_SOURCE_ID { get; set; }
        public long IS_ACTIVE { get; set; }

        public string DISPLAY_NAME
        {
            get
            {
                return string.Format("{0} - {1}", LOGINNAME, USERNAME);
            }
        }
    }
}