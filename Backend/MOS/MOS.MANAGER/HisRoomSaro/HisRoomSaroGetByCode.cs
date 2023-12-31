using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisRoomSaro
{
    partial class HisRoomSaroGet : BusinessBase
    {
        internal HIS_ROOM_SARO GetByCode(string code)
        {
            try
            {
                return GetByCode(code, new HisRoomSaroFilterQuery());
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }

        internal HIS_ROOM_SARO GetByCode(string code, HisRoomSaroFilterQuery filter)
        {
            try
            {
                return DAOWorker.HisRoomSaroDAO.GetByCode(code, filter.Query());
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
