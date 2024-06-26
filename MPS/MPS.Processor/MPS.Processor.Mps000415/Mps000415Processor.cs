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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000415.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000415
{
    public class Mps000415Processor : AbstractProcessor
    {
        Mps000415PDO rdo;
        List<ADO.ExpendADO> listMediMateExpend = new List<ADO.ExpendADO>();

        public Mps000415Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000415PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListExpend", listMediMateExpend);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo != null)
                {
                    if (rdo.SereServExt != null && rdo.SereServExt.BEGIN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue("BEGIN_TIME", rdo.SereServExt.BEGIN_TIME));
                    }
                    else if (rdo.Mps000415ADO != null && rdo.Mps000415ADO.BEGIN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue("BEGIN_TIME", rdo.Mps000415ADO.BEGIN_TIME));
                    }

                    if (rdo.ServiceReq != null)
                    {
                        AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReq, false);
                    }

                    if (rdo.BedRoom != null)
                    {
                        AddObjectKeyIntoListkey<V_HIS_TREATMENT_BED_ROOM>(rdo.BedRoom, false);
                    }

                    if (rdo.SereServPttt != null)
                    {
                        AddObjectKeyIntoListkey<V_HIS_SERE_SERV_PTTT>(rdo.SereServPttt, false);
                    }

                    if (rdo.ListExpend != null && rdo.ListExpend.Count > 0)
                    {
                        long begin_time = 0;
                        long end_time = 0;

                        if (rdo.SereServExt != null)
                        {
                            begin_time = rdo.SereServExt.BEGIN_TIME ?? 0;
                            end_time = rdo.SereServExt.END_TIME ?? 0;
                        }

                        if (begin_time == 0 && rdo.Mps000415ADO != null)
                        {
                            begin_time = rdo.Mps000415ADO.BEGIN_TIME ?? 0;
                        }

                        if (end_time == 0 && rdo.Mps000415ADO != null)
                        {
                            end_time = rdo.Mps000415ADO.END_TIME ?? 0;
                        }

                        foreach (var item in rdo.ListExpend)
                        {
                            if (item.IS_EXPEND == 1)
                            {
                                ADO.ExpendADO ado = new ADO.ExpendADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.ExpendADO>(ado, item);
                                if (begin_time > 0 && item.TDL_INTRUCTION_TIME < begin_time)
                                {
                                    ado.TRUOC_MO = "X";
                                }
                                else if (begin_time > 0 && end_time > 0 && begin_time <= item.TDL_INTRUCTION_TIME && item.TDL_INTRUCTION_TIME <= end_time)
                                {
                                    ado.TRONG_MO = "X";
                                }
                                else if (end_time > 0 && item.TDL_INTRUCTION_TIME > end_time)
                                {
                                    ado.SAU_MO = "X";
                                }

                                if (rdo.ListServiceUnit != null && rdo.ListServiceUnit.Count > 0)
                                {
                                    var unit = rdo.ListServiceUnit.FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                                    if (unit != null)
                                    {
                                        ado.SERVICE_UNIT_NAME = unit.SERVICE_UNIT_NAME;
                                    }
                                }

                                listMediMateExpend.Add(ado);
                            }
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
