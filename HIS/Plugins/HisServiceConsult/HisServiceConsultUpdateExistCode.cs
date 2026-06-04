using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.MANAGER.HisServiceConsult
{
    partial class HisServiceConsultUpdate : BusinessBase
    {
		private List<HIS_SERVICE_CONSULT> beforeUpdateHisServiceConsults = new List<HIS_SERVICE_CONSULT>();
		
        internal HisServiceConsultUpdate()
            : base()
        {

        }

        internal HisServiceConsultUpdate(CommonParam paramUpdate)
            : base(paramUpdate)
        {

        }

        internal bool Update(HIS_SERVICE_CONSULT data)
        {
            bool result = false;
            try
            {
                bool valid = true;
                HisServiceConsultCheck checker = new HisServiceConsultCheck(param);
                valid = valid && checker.VerifyRequireField(data);
                HIS_SERVICE_CONSULT raw = null;
                valid = valid && checker.VerifyId(data.ID, ref raw);
                valid = valid && checker.IsUnLock(raw);
                valid = valid && checker.ExistsCode(data.SERVICE_CONSULT_CODE, data.ID);
                if (valid)
                {
					if (!DAOWorker.HisServiceConsultDAO.Update(data))
                    {
                        MOS.MANAGER.Base.BugUtil.SetBugCode(param, MOS.LibraryBug.Bug.Enum.HisServiceConsult_CapNhatThatBai);
                        throw new Exception("Cap nhat thong tin HisServiceConsult that bai." + LogUtil.TraceData("data", data));
                    }
					
					this.beforeUpdateHisServiceConsults.Add(raw);
                    
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
                result = false;
            }
            return result;
        }

        internal bool UpdateList(List<HIS_SERVICE_CONSULT> listData)
        {
            bool result = false;
            try
            {
                bool valid = true;
                valid = IsNotNullOrEmpty(listData);
                HisServiceConsultCheck checker = new HisServiceConsultCheck(param);
                List<HIS_SERVICE_CONSULT> listRaw = new List<HIS_SERVICE_CONSULT>();
                List<long> listId = listData.Select(o => o.ID).ToList();
                valid = valid && checker.VerifyIds(listId, listRaw);
                valid = valid && checker.IsUnLock(listRaw);
                foreach (var data in listData)
                {
                    valid = valid && checker.VerifyRequireField(data);
                    valid = valid && checker.ExistsCode(data.SERVICE_CONSULT_CODE, data.ID);
                }
                if (valid)
                {
					if (!DAOWorker.HisServiceConsultDAO.UpdateList(listData))
                    {
                        MOS.MANAGER.Base.BugUtil.SetBugCode(param, MOS.LibraryBug.Bug.Enum.HisServiceConsult_CapNhatThatBai);
                        throw new Exception("Cap nhat thong tin HisServiceConsult that bai." + LogUtil.TraceData("listData", listData));
                    }
					this.beforeUpdateHisServiceConsults.AddRange(listRaw);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
                result = false;
            }
            return result;
        }
		
		internal void RollbackData()
        {
            if (IsNotNullOrEmpty(this.beforeUpdateHisServiceConsults))
            {
                if (!DAOWorker.HisServiceConsultDAO.UpdateList(this.beforeUpdateHisServiceConsults))
                {
                    LogSystem.Warn("Rollback du lieu HisServiceConsult that bai, can kiem tra lai." + LogUtil.TraceData("HisServiceConsults", this.beforeUpdateHisServiceConsults));
                }
				this.beforeUpdateHisServiceConsults = null;
            }
        }
    }
}
