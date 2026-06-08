using Inventec.Common.Logging;
using Inventec.Core;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using MOS.MANAGER.Base;
using MOS.MANAGER.HisConsultPackage;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.MANAGER.HisServiceConsult
{
    partial class HisServiceConsultGetByTreatment : BusinessBase
    {
        internal HisServiceConsultGetByTreatment()
            : base()
        {

        }

        internal HisServiceConsultGetByTreatment(CommonParam paramGet)
            : base(paramGet)
        {

        }

        internal HisServiceConsultSDO GetByTreatment(long treatmentId)
        {
            HisServiceConsultSDO result = null;
            try
            {
                if (treatmentId <= 0)
                {
                    param.Messages.Add("Mã điều trị không hợp lệ");
                    param.HasException = true;
                    return null;
                }

                HisServiceConsultFilterQuery filter = new HisServiceConsultFilterQuery();
                filter.TREATMENT_ID = treatmentId;
                List<HIS_SERVICE_CONSULT> consults = new HisServiceConsultGet().Get(filter);
                if (IsNotNullOrEmpty(consults))
                {
                    HIS_SERVICE_CONSULT consult = consults.FirstOrDefault();

                    if (IsNotNull(consult))
                    {
                        HisConsultPackageFilterQuery packageFilter = new HisConsultPackageFilterQuery();
                        packageFilter.SERVICE_CONSULT_ID = consult.ID;
                        List<HIS_CONSULT_PACKAGE> packages = new HisConsultPackageGet().Get(packageFilter);

                        result = new HisServiceConsultSDO();
                        result.Consult = consult;
                        result.Packages = packages ?? new List<HIS_CONSULT_PACKAGE>();
                    }
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
                result = null;
            }
            return result;
        }
    }
}
