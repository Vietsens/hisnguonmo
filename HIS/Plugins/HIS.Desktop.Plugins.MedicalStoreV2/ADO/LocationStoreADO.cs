using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicalStoreV2.ADO
{

    public class LocationStore
    {
        public long ID { get; set; }                     
        public string StoreCode { get; set; }             
        public string StoreName { get; set; }       
        public long? ParentID { get; set; }          
        public List<LocationStore> Children { get; set; } 

        // Constructor mặc định
        public LocationStore()
        {
            Children = new List<LocationStore>();
        }

        // Hàm thêm node con
        public void AddChild(LocationStore child)
        {
            // Kiểm tra nếu node con không phải null thì thêm vào danh sách con
            if (child != null)
            {
                Children.Add(child);
            }
        }

        // Hàm kiểm tra nếu node là lá (không có con)
        public bool IsLeaf()
        {
            return Children.Count == 0;
        }
    }
}
