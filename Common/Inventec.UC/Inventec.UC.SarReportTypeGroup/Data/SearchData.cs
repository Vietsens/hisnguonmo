﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportTypeGroup.Data
{
    public class SearchData
    {
        public string KeyWord { get; set; }
        public long Time_From { get; set; }
        public long Time_To { get; set; }

        public SearchData()
        {

        }
    }
}
