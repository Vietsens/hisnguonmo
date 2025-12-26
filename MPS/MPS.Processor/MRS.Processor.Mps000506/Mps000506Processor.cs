using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using MRS.Processor.Mps000506.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRS.Processor.Mps000506
{
    public class Mps000506Processor : AbstractProcessor
    {
        Mps000506PDO rdo;
        public Mps000506Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000506PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                ProcessSingleKey();

                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessSingleKey()
        {
            try
            {
                if (rdo != null)
                {
                    if(rdo.ServiceReq != null)
                        AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.ServiceReq, false);
                    if(rdo.sereServViex != null)
                        AddObjectKeyIntoListkey<HIS_SERE_SERV_VIEX>(rdo.sereServViex, false);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
