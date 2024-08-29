using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportType.Data
{
    public class InitData
    {
        public int PageSize { get; set; }
        public UpdateDataForPaging updateData { get; set; }
        public CultureInfo Language { get; set; }

        public InitData(int pageSize, UpdateDataForPaging updateData)
        {
            this.PageSize = pageSize;
            this.updateData = updateData;
        }
        public InitData(int pageSize, UpdateDataForPaging updateData, CultureInfo language)
        {
            this.PageSize = pageSize;
            this.updateData = updateData;
            this.Language = language;
            Inventec.UC.ListReportType.Base.LanguageManager.Init(this.Language);
        }
    }
}
