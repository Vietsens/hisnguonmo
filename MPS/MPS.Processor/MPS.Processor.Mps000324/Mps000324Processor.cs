/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using FlexCel.Report;
using Inventec.Common.Logging;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000324.PDO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System.Linq;

namespace MPS.Processor.Mps000324
{
    class Mps000324Processor : AbstractProcessor
    {
        Mps000324PDO rdo;

        public Mps000324Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000324PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                SetSingleKey();
                ProcessListSereServ();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ekipUser", rdo.ekipUsers);
                objectTag.AddObjectData(store, "SereServFollow", rdo.SereServFollows);
                objectTag.AddObjectData(store, "ServiceTypes", _ServiceTypes);

                objectTag.AddRelationship(store, "ServiceTypes", "SereServFollow", "ID", "TDL_SERVICE_TYPE_ID");


                
                SetBarcodeKey();

                singleTag.ProcessData(store, singleValueDictionary);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public List<HIS_SERVICE_TYPE> _ServiceTypes { get; set; }

        void SetSingleKey()
        {
            try
            {
                _ServiceTypes = new List<HIS_SERVICE_TYPE>();
                if (rdo.SereServFollows != null && rdo.SereServFollows.Count > 0)
                {
                    List<long> _serviceTpeIds = rdo.SereServFollows.Select(p => p.TDL_SERVICE_TYPE_ID).Distinct().ToList();

                    _ServiceTypes = rdo.ServiceTypes.Where(p => _serviceTpeIds.Contains(p.ID)).ToList();
                    _ServiceTypes = _ServiceTypes.OrderBy(p => p.ID).ToList();
                }
                if (rdo.ServiceReqPrint != null)
                {
                    //keyValues.Add(new KeyValue(Mps000324ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReqPrint.LOCK_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000324ExtendSingleKey.START_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.START_TIME ?? 0)));
                    if (rdo.ServiceReqPrint.FINISH_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000324ExtendSingleKey.FINISH_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.FINISH_TIME ?? 0)));
                }
                else
                {
                    //keyValues.Add(new KeyValue(Mps000324ExtendSingleKey.OPEN_TIME_SEPARATE_STR, ""));
                    SetSingleKey(new KeyValue(Mps000324ExtendSingleKey.START_TIME_STR, ""));
                }

                if (rdo.treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000324ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.treatment.IN_TIME)));
                }

                foreach (var ekipUser in rdo.ekipUsers)
                {
                    SetSingleKey(new KeyValue("USERNAME_EXECUTE_ROLE_" + ekipUser.EXECUTE_ROLE_CODE, ekipUser.USERNAME));
                    SetSingleKey(new KeyValue("LOGIN_NAME_EXECUTE_ROLE_" + ekipUser.EXECUTE_ROLE_CODE, ekipUser.LOGINNAME));
                }

                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.treatment, false);
                AddObjectKeyIntoListkey<V_HIS_SERE_SERV_PTTT>(rdo.sereServsPttt, false);
                AddObjectKeyIntoListkey<V_HIS_SERE_SERV_5>(rdo.sereServ, false);
                AddObjectKeyIntoListkey<HIS_SERE_SERV_EXT>(rdo.SereServExt, false);
                AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo.departmentTran, false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReqPrint, false);
                AddObjectKeyIntoListkey<PatientADO>(rdo.Patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListSereServ()
        {
            try
            {
                foreach (var item in rdo.SereServFollows)
                {
                    item.SERVICE_UNIT_NAME = (rdo.ServiceUnit.FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID) ?? new MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT()).SERVICE_UNIT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
