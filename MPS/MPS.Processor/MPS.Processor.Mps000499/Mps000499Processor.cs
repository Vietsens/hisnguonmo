using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000499.PDO;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000499
{
    class Mps000499Processor :AbstractProcessor
    {
        Mps000499PDO rdo;

        public Mps000499Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000499PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                SetSingleKey();
                SetSignatureKeyImageByCFG();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                // Điền dữ liệu vào mẫu excel
                objectTag.AddObjectData(store, "KskOccupational", new List<HIS_KSK_OCCUPATIONAL> { rdo.HisKskOccupational });
                objectTag.AddObjectData(store, "Treatment", new List<V_HIS_TREATMENT_4> { rdo.HisTreatment });
                objectTag.AddObjectData(store, "ServiceReq", new List<V_HIS_SERVICE_REQ> { rdo.HisServiceReq });
                objectTag.AddObjectData(store, "Dhst", new List<V_HIS_DHST> { rdo.HisDhst });
                objectTag.AddObjectData(store, "KskRank", rdo.ExamRank);

                // Thiết lập mối quan hệ
                objectTag.AddRelationship(store, "KskOccupational", "Dhst", "DHST_ID", "ID");

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                
                if (rdo.HisKskOccupational != null)
                {
                    AddObjectKeyIntoListkey<HIS_KSK_OCCUPATIONAL>(rdo.HisKskOccupational, false);
                }
                if (rdo.HisTreatment != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT_4>(rdo.HisTreatment, false);
                }
                if (rdo.HisServiceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);
                }
                if (rdo.HisDhst != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_DHST>(rdo.HisDhst, false);
                    SetSingleKey(new KeyValue(Mps000499ExtendSingleKey.DHST_LOGINNAME, rdo.HisDhst.EXECUTE_LOGINNAME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }    }
}
