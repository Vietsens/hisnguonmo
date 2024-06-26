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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000475.ADO;
using MPS.Processor.Mps000475.PDO;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000475
{
    public class Mps000475Processor : AbstractProcessor
    {
        List<VaccExamADO> lstADO = new List<VaccExamADO>();
        List<VaccExamADO> lstFullADO = new List<VaccExamADO>();
        VaccExamResultADO VaccExamResul = new VaccExamResultADO();
        Mps000475PDO rdo;
        public Mps000475Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000475PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetData();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => VaccExamResul), VaccExamResul));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstFullADO.Count()), lstFullADO.Count()));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstFullADO), lstFullADO));
                if (lstFullADO != null && lstFullADO.Count > 0)
                {
                    objectTag.AddObjectData(store, "ListHisvacExamResult", lstFullADO);
                }
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetData()
        {
            try
            {

                if (rdo.HisVaccExamResult != null && rdo.HisVaccExamResult.Count > 0)
                {
                    var data = rdo.HisVaccExamResult.Where(o => o.IS_BABY == null);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    if (data != null && data.Count() > 0)
                    {
                        foreach (var item in data)
                        {
                            VaccExamADO ado = new VaccExamADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<VaccExamADO>(ado, item);
                            if (item.HIS_VAEX_VAER != null && item.HIS_VAEX_VAER.Count > 0)
                            {
                                ado.IS_NO = "";
                                ado.IS_YES = "X";
                            }
                            else
                            {
                                ado.IS_NO = "X";
                                ado.IS_YES = "";
                            };
                            ado.HIS_VAEX_VAER = new List<HIS_VAEX_VAER>();
                            ado.HIS_VAEX_VAER = item.HIS_VAEX_VAER;
                            lstFullADO.Add(ado);

                        }
                        if (lstFullADO != null && lstFullADO.Count > 0)
                        {
                            lstFullADO = lstFullADO.OrderBy(o => o.VACC_EXAM_RESULT_CODE).ToList();
                        }

                       
                        if (lstFullADO != null && lstFullADO.Count > 0)
                        {
                            VaccExamResul = new VaccExamResultADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<VaccExamResultADO>(VaccExamResul, rdo.HisVactionExam);
                            if (rdo.HisExpMest != null && rdo.HisExpMest.Count > 0)
                            {
                                VaccExamResul.MEDICINE_TYPE_NAME = String.Join("; ", rdo.HisExpMest.Select(o => o.MEDICINE_TYPE_NAME));
                            }

                            if (Check8(lstFullADO.Skip(1).Take(7).ToList()))
                            {
                                VaccExamResul.PAUSE_VACC = "X";
                                //VaccExamResul.INJECT_VACC =null;
                            }

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstFullADO.Last()), lstFullADO.Last()));
                            if (Check9(lstFullADO.Last()))
                            {
                                if (lstFullADO.Last().HIS_VAEX_VAER != null)
                                {
                                    VaccExamResul.NOTE_VACC = String.Join("; ", lstFullADO.Last().HIS_VAEX_VAER.Select(o => o.NOTE));
                                }
                                VaccExamResul.CONTRAIN_VACC = "X";
                                VaccExamResul.PAUSE_VACC = null;
                            }
                            else if (Check9(lstFullADO.FirstOrDefault()))
                            {
                                VaccExamResul.CONTRAIN_VACC = "X";
                                VaccExamResul.PAUSE_VACC = null;
                            }
                            if (rdo.HisVactionExam != null)
                            {
                                if (rdo.HisVactionExam.CONCLUDE == 1)
                                {
                                    VaccExamResul.INJECT_VACC = "X";
                                    VaccExamResul.CONTRAIN_VACC = null;
                                    VaccExamResul.PAUSE_VACC = null;
                                }
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

        private bool Check8(List<VaccExamADO> lstFullADO)
        {
            bool result = false;

            foreach (VaccExamADO item in lstFullADO)
            {
                if (!String.IsNullOrEmpty(item.IS_YES) && !String.IsNullOrWhiteSpace(item.IS_YES))
                {
                    return true;
                }
            }
            return result;
        }

        private bool Check9(VaccExamADO lstFullADO)
        {
            bool result = false;


            if (!String.IsNullOrEmpty(lstFullADO.IS_YES) && !String.IsNullOrWhiteSpace(lstFullADO.IS_YES))
            {

                return true;
            }

            return result;
        }
        private void SetSingleKey()
        {
            try
            {
                if (rdo.HisDHST != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.HisDHST, false);
                }

                if (VaccExamResul != null)
                {
                    AddObjectKeyIntoListkey<VaccExamResultADO>(VaccExamResul, false);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
