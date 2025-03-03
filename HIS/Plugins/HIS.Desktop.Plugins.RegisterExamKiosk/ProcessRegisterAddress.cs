using DevExpress.Utils.Navigation;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using Microsoft.VisualBasic.Logging;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RegisterExamKiosk
{
    internal class ProcessRegisterAddress
    {
        internal static bool CheckAddress(HIS_PATIENT hisPatient)
        {
            bool result = true;
            try
            {
                if (hisPatient != null)
                {
                    bool address = !String.IsNullOrWhiteSpace(hisPatient.DISTRICT_CODE) && !String.IsNullOrWhiteSpace(hisPatient.PROVINCE_CODE) && !String.IsNullOrWhiteSpace(hisPatient.COMMUNE_NAME);
                    bool htAddress = !String.IsNullOrWhiteSpace(hisPatient.HT_DISTRICT_CODE) && !String.IsNullOrWhiteSpace(hisPatient.HT_PROVINCE_CODE) && !String.IsNullOrWhiteSpace(hisPatient.HT_COMMUNE_NAME);
                    result = (htAddress || address);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static void SplitAddress(HisCardSDO cardsdo)
        {
            try
            {
                if (cardsdo != null)
                {
                    Inventec.Common.Address.AddressProcessor adProc = new Inventec.Common.Address.AddressProcessor(BackendDataWorker.Get<V_SDA_PROVINCE>(), BackendDataWorker.Get<V_SDA_DISTRICT>(), BackendDataWorker.Get<V_SDA_COMMUNE>());

                    if (!String.IsNullOrWhiteSpace(cardsdo.Address) && (String.IsNullOrWhiteSpace(cardsdo.DistrictCode) || String.IsNullOrWhiteSpace(cardsdo.ProvinceCode) || String.IsNullOrWhiteSpace(cardsdo.CommuneName)))
                    {
                        Inventec.Common.Address.AddressADO splitAdress = adProc.SplitFromFullAddress(cardsdo.Address);
                        if (splitAdress != null && !string.IsNullOrEmpty(splitAdress.ProvinceName) && !string.IsNullOrEmpty(splitAdress.DistrictName) && !string.IsNullOrEmpty(splitAdress.CommuneName))
                        {
                            cardsdo.DistrictCode = splitAdress.DistrictCode;
                            cardsdo.DistrictName = splitAdress.DistrictName;
                            cardsdo.CommuneCode = splitAdress.CommuneCode;
                            cardsdo.CommuneName = splitAdress.CommuneName;
                            cardsdo.ProvinceCode = splitAdress.ProvinceCode;
                            cardsdo.ProvinceName = splitAdress.ProvinceName;
                            cardsdo.Address = splitAdress.Address;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(cardsdo.HtAddress) && (String.IsNullOrWhiteSpace(cardsdo.HtDistrictCode) || String.IsNullOrWhiteSpace(cardsdo.HtProvinceCode) || String.IsNullOrWhiteSpace(cardsdo.HtCommuneName)))
                    {
                        Inventec.Common.Address.AddressADO splitAdress = adProc.SplitFromFullAddress(cardsdo.HtAddress);
                        if (splitAdress != null && !string.IsNullOrEmpty(splitAdress.ProvinceName) && !string.IsNullOrEmpty(splitAdress.DistrictName) && !string.IsNullOrEmpty(splitAdress.CommuneName))
                        {
                            cardsdo.HtDistrictCode = splitAdress.DistrictCode;
                            cardsdo.HtDistrictName = splitAdress.DistrictName;
                            cardsdo.HtCommuneCode = splitAdress.CommuneCode;
                            cardsdo.HtCommuneName = splitAdress.CommuneName;
                            cardsdo.HtProvinceCode = splitAdress.ProvinceCode;
                            cardsdo.HtProvinceName = splitAdress.ProvinceName;
                            cardsdo.HtAddress = splitAdress.Address;
                        }
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
