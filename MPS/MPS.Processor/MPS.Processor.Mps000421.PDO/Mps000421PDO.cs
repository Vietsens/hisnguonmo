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
using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using MPS.ProcessorBase;
using System.Runtime.InteropServices;

namespace MPS.Processor.Mps000421.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000421PDO : RDOBase
    {
        public Mps000421PDO(V_HIS_TREATMENT treatment, V_HIS_PATIENT patient, V_HIS_EXP_MEST expmest, List<V_HIS_EXP_MEST_BLOOD> expmestblood, List<V_HIS_EXP_BLTY_SERVICE> expbltyservice, List<V_HIS_TRANSFUSION_SUM> transFusionSum, List<HIS_EXP_MEST> ListExpMest, List<HIS_TRANSFUSION> TransFusions) : this(treatment, patient, expmest, expmestblood, expbltyservice, transFusionSum)
        {
            this.ListExpMest = ListExpMest;
            this.TransFusions = TransFusions;
        }
        public Mps000421PDO(V_HIS_TREATMENT treatment, V_HIS_PATIENT patient, V_HIS_EXP_MEST expmest, List<V_HIS_EXP_MEST_BLOOD> expmestblood, List<V_HIS_EXP_BLTY_SERVICE> expbltyservice, List<V_HIS_TRANSFUSION_SUM> transFusionSum)
        {
            try
            {
                this.Treatment = treatment;
                this.Patient = patient;
                this.ExpMest = expmest;
                this.ExpMestBlood = expmestblood;
                this.ExpBltyService = expbltyservice;
                this.TransFusionSum = TransFusionSum;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
