using HIS.UC.HisMateInStockByExpireDate.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.HisMateInStockByExpireDate.GetListTreeView
{
    internal interface IGetListTreeView
    {
        List<HisMateInStockByExpireDateADO> Run();
    }
}
