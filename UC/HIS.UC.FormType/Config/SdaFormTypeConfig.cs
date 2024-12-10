using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Config
{
    public class SdaFormTypeConfig
    {
        private static List<SDA.EFMODEL.DataModels.SDA_DISTRICT> sdaDistrict;
        public static List<SDA.EFMODEL.DataModels.SDA_DISTRICT> SdaDistrict
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(SDA.EFMODEL.DataModels.SDA_DISTRICT));
                if (sdaDistrict == null || sdaDistrict.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    SDA.Filter.SdaDistrictFilter filter = new SDA.Filter.SdaDistrictFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    sdaDistrict = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_DISTRICT>("api/SdaDistrict/Get", ApiConsumerStore.SdaConsumer, filter);
                }
                return sdaDistrict.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                sdaDistrict = value;
            }
        }
        private static List<SDA.EFMODEL.DataModels.SDA_COMMUNE> sdaCommune;
        public static List<SDA.EFMODEL.DataModels.SDA_COMMUNE> SdaCommune
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(SDA.EFMODEL.DataModels.SDA_COMMUNE));
                if (sdaCommune == null || sdaCommune.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    SDA.Filter.SdaCommuneFilter filter = new SDA.Filter.SdaCommuneFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    sdaCommune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_COMMUNE>("api/SdaCommune/Get", ApiConsumerStore.SdaConsumer, filter);
                }
                return sdaCommune.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                sdaCommune = value;
            }
        }
        private static List<SDA.EFMODEL.DataModels.SDA_PROVINCE> sdaProvince;
        public static List<SDA.EFMODEL.DataModels.SDA_PROVINCE> SdaProvince
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(SDA.EFMODEL.DataModels.SDA_PROVINCE));
                if (sdaProvince == null || sdaProvince.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    SDA.Filter.SdaProvinceFilter filter = new SDA.Filter.SdaProvinceFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    sdaProvince = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_PROVINCE>("api/SdaProvince/Get", ApiConsumerStore.SdaConsumer, filter);
                }
                return sdaProvince.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                sdaProvince = value;
            }
        }

    }
}
