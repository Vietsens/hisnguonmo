using ACS.DAO.Base;
using ACS.DAO.StagingObject;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACS.DAO.AcsCredentialData
{
    partial class AcsCredentialDataGet : EntityBase
    {
        public V_ACS_CREDENTIAL_DATA GetViewByCode(string code, AcsCredentialDataSO search)
        {
            V_ACS_CREDENTIAL_DATA result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNullOrEmpty(code);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.V_ACS_CREDENTIAL_DATA.AsQueryable().Where(p => p.CREDENTIAL_DATA_CODE == code);
                        if (search.listVAcsCredentialDataExpression != null && search.listVAcsCredentialDataExpression.Count > 0)
                        {
                            foreach (var item in search.listVAcsCredentialDataExpression)
                            {
                                query = query.Where(item);
                            }
                        }
                        result = query.SingleOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => search), search), LogType.Error);
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
