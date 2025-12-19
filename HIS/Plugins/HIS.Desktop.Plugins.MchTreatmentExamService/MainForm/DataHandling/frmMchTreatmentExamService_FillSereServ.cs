using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.TreeSereServ7;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Load SereServ Data

        private async Task LoadDataSereServ7()
        {
            try
            {
                WaitingManager.Show();
                if (Treatment != null || ExamService != null)
                {
                    CommonParam param = new CommonParam();

                    DHisSereServ2Filter _sereServ2Filter = new DHisSereServ2Filter();
                    _sereServ2Filter.TREATMENT_ID = Treatment != null && Treatment.ID > 0 ? Treatment.ID : ExamService.TREATMENT_ID;
                    var dataNew = new BackendAdapter(param).Get<List<DHisSereServ2>>("api/HisSereServ/GetDHisSereServ2", ApiConsumers.MosConsumer, _sereServ2Filter, param);

                    List<V_HIS_SERE_SERV_7> _sereServ7s = new List<V_HIS_SERE_SERV_7>();
                    if (dataNew != null && dataNew.Count > 0)
                    {
                        dataNew = dataNew.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH).ToList();
                        foreach (var item in dataNew)
                        {
                            V_HIS_SERE_SERV_7 ado = new V_HIS_SERE_SERV_7();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_7>(ado, item);
                            ado.TDL_REQUEST_DEPARTMENT_ID = item.REQUEST_DEPARTMENT_ID ?? 0;
                            ado.ID = item.SERE_SERV_ID ?? 0;
                            ado.TDL_SERVICE_CODE = item.SERVICE_CODE;
                            ado.TDL_SERVICE_NAME = item.SERVICE_NAME;
                            ado.TDL_SERVICE_REQ_CODE = item.SERVICE_REQ_CODE;
                            var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == item.TDL_SERVICE_TYPE_ID);
                            ado.SERVICE_TYPE_NAME = serviceType != null ? serviceType.SERVICE_TYPE_NAME : null;
                            ado.SERVICE_TYPE_CODE = serviceType != null ? serviceType.SERVICE_TYPE_CODE : null;
                            _sereServ7s.Add(ado);
                        }
                    }
                    if (_sereServ7s != null && _sereServ7s.Count > 0)
                    {
                        if (ucSereServ != null)
                        {
                            treeSereServ7Processor.Reload(ucSereServ, _sereServ7s);
                        }
                    }
                    else
                    {
                        if (ucSereServ != null)
                        {
                            treeSereServ7Processor.Reload(ucSereServ, new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7>());
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
