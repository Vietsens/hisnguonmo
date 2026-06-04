using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisServiceConsult
{
    partial class HisServiceConsultCreate : BusinessBase
    {
		private List<HIS_SERVICE_CONSULT> recentHisServiceConsults = new List<HIS_SERVICE_CONSULT>();
		
        internal HisServiceConsultCreate()
            : base()
        {

        }

        internal HisServiceConsultCreate(CommonParam paramCreate)
            : base(paramCreate)
        {

        }

        internal bool Create(HIS_SERVICE_CONSULT data)
        {
            bool result = false;
            try
            {
                bool valid = true;
                HisServiceConsultCheck checker = new HisServiceConsultCheck(param);
                valid = valid && checker.VerifyRequireField(data);
                if (valid)
                {
					if (!DAOWorker.HisServiceConsultDAO.Create(data))
                    {
                        MOS.MANAGER.Base.BugUtil.SetBugCode(param, MOS.LibraryBug.Bug.Enum.HisServiceConsult_ThemMoiThatBai);
                        throw new Exception("Them moi thong tin HisServiceConsult that bai." + LogUtil.TraceData("data", data));
                    }
                    this.recentHisServiceConsults.Add(data);
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
		
		internal bool CreateList(List<HIS_SERVICE_CONSULT> listData)
        {
            bool result = false;
            try
            {
                bool valid = true;
                valid = IsNotNullOrEmpty(listData);
                HisServiceConsultCheck checker = new HisServiceConsultCheck(param);
                foreach (var data in listData)
                {
                    valid = valid && checker.VerifyRequireField(data);
                }
                if (valid)
                {
                    if (!DAOWorker.HisServiceConsultDAO.CreateList(listData))
                    {
                        MOS.MANAGER.Base.BugUtil.SetBugCode(param, MOS.LibraryBug.Bug.Enum.HisServiceConsult_ThemMoiThatBai);
                        throw new Exception("Them moi thong tin HisServiceConsult that bai." + LogUtil.TraceData("listData", listData));
                    }
                    this.recentHisServiceConsults.AddRange(listData);
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
            if (IsNotNullOrEmpty(this.recentHisServiceConsults))
            {
                if (!DAOWorker.HisServiceConsultDAO.TruncateList(this.recentHisServiceConsults))
                {
                    LogSystem.Warn("Rollback du lieu HisServiceConsult that bai, can kiem tra lai." + LogUtil.TraceData("recentHisServiceConsults", this.recentHisServiceConsults));
                }
				this.recentHisServiceConsults = null;
            }
        }
    }
}
