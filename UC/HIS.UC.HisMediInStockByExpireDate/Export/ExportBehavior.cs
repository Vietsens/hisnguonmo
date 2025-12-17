using HIS.UC.HisMediInStockByExpireDate.Run;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.HisMediInStockByExpireDate.Export
{
    internal class ExportBehavior : IExport
    {
        public ExportBehavior()
        {
                
        }

        public ExportBehavior(CommonParam param, UserControl data)
        {
            this.control = data;
        }

        void IExport.Run()
        {
            try
            {
                ((UCHisMediInStockByExpireDate)this.control).ExportExcel();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private UserControl control;
    }
}
