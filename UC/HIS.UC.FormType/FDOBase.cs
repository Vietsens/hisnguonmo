using System.Collections.Generic;

namespace HIS.UC.FormType
{
    public abstract class FDOBase
    {
        /// <summary>
        /// Luu tru du lieu cac the single duoi dang danh sach key_value.
        /// Cac du lieu dac thu khac cua RDO se duoc luu tru o cac RDO cu the.
        /// </summary>
        //protected void SetKeyValue(string key, string value)
        //{
        //    keyValues.Add(new KeyValue(key, value));
        //}

        abstract internal void GetValue();
    }
}
