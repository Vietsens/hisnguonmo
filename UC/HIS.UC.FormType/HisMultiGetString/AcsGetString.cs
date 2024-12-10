using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.HisMultiGetString
{
    class AcsGetString
    {
        public static List<DataGet> Get(string value, string key)
        {
            List<DataGet> datasuft = null;
            try
            {
                if (value == null) return new List<DataGet>();

                else if (value == "ACS_USER") datasuft = Config.AcsFormTypeConfig.HisAcsUser.Select(o => new DataGet { ID = o.ID, CODE = o.LOGINNAME, NAME = o.USERNAME }).ToList();
                else if (value == "ACS_MY_LOGINNAME")
                {
                    if (HIS.UC.FormType.FormTypeConfig.MyInfo == null)
                    {
                        datasuft = new List<DataGet>();
                    }
                    else
                    {
                        datasuft = Config.AcsFormTypeConfig.HisAcsUser.Where(o => o.LOGINNAME == HIS.UC.FormType.FormTypeConfig.MyInfo.LOGINNAME || HIS.UC.FormType.FormTypeConfig.MyInfo.IS_ADMIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ID, CODE = o.LOGINNAME, NAME = o.USERNAME }).ToList();
                    }
                }
                else if (value == "ACS_USER_SALE") datasuft = Config.AcsFormTypeConfig.HisAcsUserSale.Select(o => new DataGet { ID = o.ID, CODE = o.LOGINNAME, NAME = o.USERNAME }).ToList();
                else if (value == "ACS_USER_DOCTOR")
                {
                    var employees = Config.HisFormTypeConfig.HisDEmployees;
                    datasuft = Config.AcsFormTypeConfig.HisAcsUser.Where(p => employees.Exists(q => q.LOGINNAME == p.LOGINNAME && q.IS_DOCTOR == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)).Select(o => new DataGet { ID = o.ID, CODE = o.LOGINNAME, NAME = o.USERNAME }).ToList();
                }
                else if (value == "ACS_USER_DOCTOR_DEPA")
                {
                    datasuft = Config.HisFormTypeConfig.HisDoctorDepas;
                }

                datasuft = datasuft.OrderBy(o => o.NAME).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return datasuft;
        }
    }
}
