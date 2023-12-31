using System;

namespace SDA.MANAGER.Core.SdaLicense
{
    partial class SdaLicenseBO : BusinessObjectBase
    {
        internal T GetLast<T>(object data)
        {
            T result = default(T);
            try
            {
                IDelegacyT delegacy = new SdaLicenseGetLast(param, data);
                result = delegacy.Execute<T>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = default(T);
            }
            return result;
        }

        internal T Decode<T>(object data)
        {
            T result = default(T);
            try
            {
                IDelegacyT delegacy = new SdaLicenseDecode(param, data);
                result = delegacy.Execute<T>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = default(T);
            }
            return result;
        }

        internal T CreateT<T>(object data)
        {
            T result = default(T);
            try
            {
                IDelegacyT delegacy = new SdaLicenseCreateT(param, data);
                result = delegacy.Execute<T>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = default(T);
            }
            return result;
        }

        internal T UpdateT<T>(object data)
        {
            T result = default(T);
            try
            {
                IDelegacyT delegacy = new SdaLicenseUpdateT(param, data);
                result = delegacy.Execute<T>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = default(T);
            }
            return result;
        }
    }
}
