using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials.ADO
{
    class KSKContractADO : HIS_KSK_CONTRACT
    {
        public string WORK_PLACE_NAME { get; set; }

        public KSKContractADO()
        {
        }

        public KSKContractADO(HIS_KSK_CONTRACT data)
        {
            try
            {
                if (data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, (item.GetValue(data)));
                    }

                    var workPlace = BackendDataWorker.Get<HIS_WORK_PLACE>().FirstOrDefault(o => o.ID == data.WORK_PLACE_ID);
                    if (workPlace != null)
                    {
                        this.WORK_PLACE_NAME = workPlace.WORK_PLACE_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
