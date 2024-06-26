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
using MPS.Processor.Mps000225.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000225
{
    class Mps000225Processor : AbstractProcessor
    {
        Mps000225PDO rdo;
        List<Mps000225BySereServ> PatientType = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> ServiceType = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> ServiceTotal = new List<Mps000225BySereServ>();

        public Mps000225Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000225PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetBarcodeKey();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "ServiceCDHA", _ADO_CDHAs);
                objectTag.AddObjectData(store, "ServiceG", _ADO_Gs);
                objectTag.AddObjectData(store, "ServiceGPBL", _ADO_GPBLs);
                objectTag.AddObjectData(store, "ServiceKH", _ADO_KHs);
                objectTag.AddObjectData(store, "ServiceKHAC", _ADO_KHACs);
                objectTag.AddObjectData(store, "ServiceMAU", _ADO_MAUs);
                objectTag.AddObjectData(store, "ServiceNS", _ADO_NSs);
                objectTag.AddObjectData(store, "ServicePHCN", _ADO_PHCNs);
                objectTag.AddObjectData(store, "ServicePT", _ADO_PTs);
                objectTag.AddObjectData(store, "ServiceSA", _ADO_SAs);
                objectTag.AddObjectData(store, "ServiceTDCN", _ADO_TDCNs);
                objectTag.AddObjectData(store, "ServiceTHUOC", _ADO_THUOCs);
                objectTag.AddObjectData(store, "ServiceTT", _ADO_TTs);
                objectTag.AddObjectData(store, "ServiceVT", _ADO_VTs);
                objectTag.AddObjectData(store, "ServiceXN", _ADO_XNs);
                objectTag.AddObjectData(store, "ServiceAN", _ADO_ANs);
                //them co hao phi
                objectTag.AddObjectData(store, "ServiceHPCDHA", _ADO_HP_CDHAs);
                objectTag.AddObjectData(store, "ServiceHPG", _ADO_HP_Gs);
                objectTag.AddObjectData(store, "ServiceHPGPBL", _ADO_HP_GPBLs);
                objectTag.AddObjectData(store, "ServiceHPKH", _ADO_HP_KHs);
                objectTag.AddObjectData(store, "ServiceHPKHAC", _ADO_HP_KHACs);
                objectTag.AddObjectData(store, "ServiceHPMAU", _ADO_HP_MAUs);
                objectTag.AddObjectData(store, "ServiceHPNS", _ADO_HP_NSs);
                objectTag.AddObjectData(store, "ServiceHPPHCN", _ADO_HP_PHCNs);
                objectTag.AddObjectData(store, "ServiceHPPT", _ADO_HP_PTs);
                objectTag.AddObjectData(store, "ServiceHPSA", _ADO_HP_SAs);
                objectTag.AddObjectData(store, "ServiceHPTDCN", _ADO_HP_TDCNs);
                objectTag.AddObjectData(store, "ServiceHPTHUOC", _ADO_HP_THUOCs);
                objectTag.AddObjectData(store, "ServiceHPTT", _ADO_HP_TTs);
                objectTag.AddObjectData(store, "ServiceHPVT", _ADO_HP_VTs);
                objectTag.AddObjectData(store, "ServiceHPXN", _ADO_HP_XNs);
                objectTag.AddObjectData(store, "ServiceHPAN", _ADO_HP_ANs);
                //them khong hao phi
                objectTag.AddObjectData(store, "ServiceNHPCDHA", _ADO_NHP_CDHAs);
                objectTag.AddObjectData(store, "ServiceNHPG", _ADO_NHP_Gs);
                objectTag.AddObjectData(store, "ServiceNHPGPBL", _ADO_NHP_GPBLs);
                objectTag.AddObjectData(store, "ServiceNHPKH", _ADO_NHP_KHs);
                objectTag.AddObjectData(store, "ServiceNHPKHAC", _ADO_NHP_KHACs);
                objectTag.AddObjectData(store, "ServiceNHPMAU", _ADO_NHP_MAUs);
                objectTag.AddObjectData(store, "ServiceNHPNS", _ADO_NHP_NSs);
                objectTag.AddObjectData(store, "ServiceNHPPHCN", _ADO_NHP_PHCNs);
                objectTag.AddObjectData(store, "ServiceNHPPT", _ADO_NHP_PTs);
                objectTag.AddObjectData(store, "ServiceNHPSA", _ADO_NHP_SAs);
                objectTag.AddObjectData(store, "ServiceNHPTDCN", _ADO_NHP_TDCNs);
                objectTag.AddObjectData(store, "ServiceNHPTHUOC", _ADO_NHP_THUOCs);
                objectTag.AddObjectData(store, "ServiceNHPTT", _ADO_NHP_TTs);
                objectTag.AddObjectData(store, "ServiceNHPVT", _ADO_NHP_VTs);
                objectTag.AddObjectData(store, "ServiceNHPXN", _ADO_NHP_XNs);
                objectTag.AddObjectData(store, "ServiceNHPAN", _ADO_NHP_ANs);


                objectTag.AddObjectData(store, "PatientType", PatientType);
                objectTag.AddObjectData(store, "ServiceType", ServiceType);
                objectTag.AddObjectData(store, "ServiceTotal", ServiceTotal);
                objectTag.AddRelationship(store, "PatientType", "ServiceType", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceTotal", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceType", "ServiceTotal", "Service_Type_Id", "Service_Type_Id");

                //HP
                objectTag.AddRelationship(store, "PatientType", "ServiceHPCDHA", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPG", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPGPBL", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPKH", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPKHAC", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPMAU", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPNS", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPPHCN", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPPT", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPSA", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPTDCN", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPTHUOC", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPTT", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPVT", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPXN", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceHPAN", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");

                //Khp
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPCDHA", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPG", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPGPBL", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPKH", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPKHAC", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPMAU", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPNS", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPPHCN", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPPT", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPSA", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPTDCN", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPTHUOC", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPTT", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPVT", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPXN", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                objectTag.AddRelationship(store, "PatientType", "ServiceNHPAN", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");

                //HP
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPCDHA", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPG", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPGPBL", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPKH", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPKHAC", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPMAU", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPNS", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPPHCN", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPPT", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPSA", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPTDCN", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPTHUOC", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPTT", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPVT", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPXN", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceHPAN", "Service_Type_Id", "Service_Type_Id");

                //Khp
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPCDHA", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPG", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPGPBL", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPKH", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPKHAC", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPMAU", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPNS", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPPHCN", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPPT", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPSA", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPTDCN", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPTHUOC", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPTT", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPVT", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPXN", "Service_Type_Id", "Service_Type_Id");
                objectTag.AddRelationship(store, "ServiceType", "ServiceNHPAN", "Service_Type_Id", "Service_Type_Id");


                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        List<Mps000225BySereServ> _ADO_CDHAs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_Gs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_GPBLs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_KHs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_KHACs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_MAUs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NSs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_PHCNs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_PTs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_SAs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_TDCNs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_THUOCs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_TTs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_VTs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_XNs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_ANs = new List<Mps000225BySereServ>();
        //them co hao phi
        List<Mps000225BySereServ> _ADO_HP_CDHAs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_Gs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_GPBLs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_KHs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_KHACs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_MAUs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_NSs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_PHCNs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_PTs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_SAs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_TDCNs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_THUOCs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_TTs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_VTs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_XNs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_HP_ANs = new List<Mps000225BySereServ>();
        //them khong hao phi
        List<Mps000225BySereServ> _ADO_NHP_CDHAs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_Gs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_GPBLs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_KHs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_KHACs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_MAUs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_NSs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_PHCNs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_PTs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_SAs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_TDCNs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_THUOCs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_TTs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_VTs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_XNs = new List<Mps000225BySereServ>();
        List<Mps000225BySereServ> _ADO_NHP_ANs = new List<Mps000225BySereServ>();
        private void SetSingleKey()
        {
            try
            {
                #region ---- SetSingleKeyDateTime
                if (rdo._Mps000225ADOs != null && rdo._Mps000225ADOs.Count > 0)
                {
                    foreach (var item in rdo._Mps000225ADOs)
                    {
                        #region ---Day---
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day1, item.Day1));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day2, item.Day2));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day3, item.Day3));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day4, item.Day4));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day5, item.Day5));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day6, item.Day6));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day7, item.Day7));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day8, item.Day8));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day9, item.Day9));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day10, item.Day10));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day11, item.Day11));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day12, item.Day12));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day13, item.Day13));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day14, item.Day14));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day15, item.Day15));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day16, item.Day16));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day17, item.Day17));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day18, item.Day18));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day19, item.Day19));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day20, item.Day20));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day21, item.Day21));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day22, item.Day22));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day23, item.Day23));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.Day24, item.Day24));
                        #endregion

                        #region ---Day and year---
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear1, item.DayAndYear1));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear2, item.DayAndYear2));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear3, item.DayAndYear3));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear4, item.DayAndYear4));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear5, item.DayAndYear5));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear6, item.DayAndYear6));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear7, item.DayAndYear7));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear8, item.DayAndYear8));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear9, item.DayAndYear9));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear10, item.DayAndYear10));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear11, item.DayAndYear11));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear12, item.DayAndYear12));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear13, item.DayAndYear13));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear14, item.DayAndYear14));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear15, item.DayAndYear15));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear16, item.DayAndYear16));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear17, item.DayAndYear17));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear18, item.DayAndYear18));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear19, item.DayAndYear19));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear20, item.DayAndYear20));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear21, item.DayAndYear21));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear22, item.DayAndYear22));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear23, item.DayAndYear23));
                        SetSingleKey(new KeyValue(Mps000225ExtendSingleKey.DayAndYear24, item.DayAndYear24));
                        #endregion
                    }
                }
                #endregion

                _ADO_CDHAs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA).ToList());
                _ADO_Gs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList());
                _ADO_GPBLs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL).ToList());
                _ADO_KHs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH).ToList());
                _ADO_KHACs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC).ToList());
                _ADO_MAUs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).ToList());
                _ADO_NSs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS).ToList());
                _ADO_PHCNs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN).ToList());
                _ADO_PTs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT).ToList());
                _ADO_SAs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA).ToList());
                _ADO_TDCNs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN).ToList());
                _ADO_THUOCs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC).ToList());
                _ADO_TTs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT).ToList());
                _ADO_VTs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList());
                _ADO_XNs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList());
                _ADO_ANs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN).ToList());
                //them co hao phi
                _ADO_HP_CDHAs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_Gs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_GPBLs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_KHs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_KHACs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_MAUs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_NSs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_PHCNs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_PTs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_SAs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_TDCNs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_THUOCs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_TTs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_VTs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_XNs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_HP_ANs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN && o.TypeExpend == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                //them khong hao phi
                _ADO_NHP_CDHAs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_Gs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_GPBLs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_KHs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_KHACs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_MAUs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_NSs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_PHCNs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_PTs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_SAs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_TDCNs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_THUOCs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_TTs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_VTs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_XNs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                _ADO_NHP_ANs = GroupByService(rdo._Mps000225BySereServs.Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN && o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());

                ProcessDataGroupPatyType();

                if (rdo.currentTreatment != null)
                {
                    SetSingleKey((new KeyValue(Mps000225ExtendSingleKey.IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.IN_TIME))));
                    if (rdo.currentTreatment.OUT_TIME != null)
                        SetSingleKey((new KeyValue(Mps000225ExtendSingleKey.OUT_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.OUT_TIME ?? 0))));
                    if (rdo.currentTreatment.CLINICAL_IN_TIME != null)
                        SetSingleKey((new KeyValue(Mps000225ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.CLINICAL_IN_TIME ?? 0))));
                    SetSingleKey((new KeyValue(Mps000225ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.currentTreatment.TDL_PATIENT_DOB))));
                }
                AddObjectKeyIntoListkey<SingleKeys>(rdo._SingleKeys);

                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.currentTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataGroupPatyType()
        {
            try
            {
                if (rdo._Mps000225BySereServs != null && rdo._Mps000225BySereServs.Count > 0)
                {
                    //patient type
                    var groupPatientType = rdo._Mps000225BySereServs.GroupBy(o => o.PATIENT_TYPE_ID).ToList();
                    foreach (var item in groupPatientType)
                    {
                        if (rdo.ListPatientType != null && rdo.ListPatientType.Count > 0)
                        {
                            var patientType = rdo.ListPatientType.FirstOrDefault(o => o.ID == item.First().PATIENT_TYPE_ID);
                            if (patientType != null)
                            {
                                Mps000225BySereServ paty = new Mps000225BySereServ();
                                paty.PATIENT_TYPE_ID = patientType.ID;
                                paty.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                                paty.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                                this.PatientType.Add(paty);
                            }
                        }
                    }

                    this.PatientType = this.PatientType.OrderBy(o => o.PATIENT_TYPE_ID).ToList();

                    //service type
                    var groupByServiceType = rdo._Mps000225BySereServs.GroupBy(o => new { o.PATIENT_TYPE_ID, o.Service_Type_Id }).ToList();
                    foreach (var item in groupByServiceType)
                    {
                        if (rdo.ListServiceType != null && groupByServiceType.Count > 0)
                        {
                            var serviceType = rdo.ListServiceType.FirstOrDefault(o => o.ID == item.First().Service_Type_Id);
                            if (serviceType != null)
                            {
                                Mps000225BySereServ paty = new Mps000225BySereServ();
                                paty.PATIENT_TYPE_ID = item.First().PATIENT_TYPE_ID;
                                paty.Service_Type_Id = serviceType.ID;
                                paty.SERVICE_TYPE_CODE = serviceType.SERVICE_TYPE_CODE;
                                paty.SERVICE_TYPE_NAME = serviceType.SERVICE_TYPE_NAME;
                                paty.HEIN_SERVICE_TYPE_NUM_ORDER = serviceType.NUM_ORDER;//gán vào HEIN_SERVICE_TYPE_NUM_ORDER để sắp xếp
                                this.ServiceType.Add(paty);
                            }
                        }
                    }

                    this.ServiceType = this.ServiceType.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 9999).ToList();

                    //service 
                    var rsGroup = rdo._Mps000225BySereServs.GroupBy(p => new { p.SERVICE_ID, p.PRICE, p.Service_Type_Id, p.CONCENTRA, p.PATIENT_TYPE_ID }).ToList();
                    foreach (var itemGroup in rsGroup)
                    {
                        MPS.Processor.Mps000225.PDO.Mps000225BySereServ ado = new MPS.Processor.Mps000225.PDO.Mps000225BySereServ();
                        ado.Service_Type_Id = itemGroup.First().Service_Type_Id;
                        ado.TDL_SERVICE_NAME = itemGroup.First().TDL_SERVICE_NAME;
                        ado.SERVICE_UNIT_NAME = itemGroup.First().SERVICE_UNIT_NAME;
                        ado.PATIENT_TYPE_ID = itemGroup.First().PATIENT_TYPE_ID;
                        ado.CONCENTRA = itemGroup.First().CONCENTRA;
                        ado.PRICE = itemGroup.First().PRICE;
                        ado.AMOUNT = itemGroup.Sum(p => p.AMOUNT);
                        ado.AMOUNT_STRING = itemGroup.Where(o => o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Sum(p => p.AMOUNT).ToString();//so luong khong hao phi
                        ado.INSTRUCTION_NOTE = string.Join(", ", itemGroup.Select(s => s.INSTRUCTION_NOTE).Distinct().ToList());

                        PropertyInfo[] ps = Inventec.Common.Repository.Properties.Get<MPS.Processor.Mps000225.PDO.Mps000225BySereServ>();
                        foreach (var item in itemGroup)
                        {
                            for (int j = 0; j < 60; j++)
                            {
                                PropertyInfo info = ps.FirstOrDefault(o => o.Name == string.Format("Day{0}", j + 1));
                                if (info != null)
                                {
                                    decimal itemValue = Inventec.Common.TypeConvert.Parse.ToDecimal((info.GetValue(item) ?? "0").ToString());
                                    decimal adoValue = Inventec.Common.TypeConvert.Parse.ToDecimal((info.GetValue(ado) ?? "0").ToString());
                                    if (itemValue > 0)
                                    {
                                        info.SetValue(ado, (adoValue + itemValue).ToString());
                                    }
                                }
                            }
                        }

                        this.ServiceTotal.Add(ado);
                    }

                    this.ServiceTotal = this.ServiceTotal.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<Mps000225BySereServ> GroupByService(List<Mps000225BySereServ> _Mps000225BySereServs)
        {
            List<MPS.Processor.Mps000225.PDO.Mps000225BySereServ> result = new List<MPS.Processor.Mps000225.PDO.Mps000225BySereServ>();
            if (_Mps000225BySereServs != null && _Mps000225BySereServs.Count > 0)
            {
                var rsGroup = _Mps000225BySereServs.GroupBy(p => new { p.SERVICE_ID, p.PRICE, p.Service_Type_Id, p.CONCENTRA }).ToList();
                foreach (var itemGroup in rsGroup)
                {
                    MPS.Processor.Mps000225.PDO.Mps000225BySereServ ado = new MPS.Processor.Mps000225.PDO.Mps000225BySereServ();
                    ado.Service_Type_Id = itemGroup.FirstOrDefault().Service_Type_Id;
                    ado.AMOUNT = itemGroup.Sum(p => p.AMOUNT);
                    ado.AMOUNT_STRING = itemGroup.Where(o => o.TypeExpend != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Sum(p => p.AMOUNT).ToString();//so luong khong hao phi
                    ado.TDL_SERVICE_NAME = itemGroup.FirstOrDefault().TDL_SERVICE_NAME;
                    ado.PRICE = itemGroup.FirstOrDefault().PRICE;
                    ado.SERVICE_UNIT_NAME = itemGroup.FirstOrDefault().SERVICE_UNIT_NAME;
                    ado.CONCENTRA = itemGroup.FirstOrDefault().CONCENTRA;
                    ado.INSTRUCTION_NOTE = string.Join(", ", itemGroup.Select(s => s.INSTRUCTION_NOTE).Distinct().ToList());
                    ado.PATIENT_TYPE_ID = itemGroup.FirstOrDefault().PATIENT_TYPE_ID;
                    ado.PATIENT_TYPE_CODE = itemGroup.FirstOrDefault().PATIENT_TYPE_CODE;
                    ado.PATIENT_TYPE_NAME = itemGroup.FirstOrDefault().PATIENT_TYPE_NAME;

                    PropertyInfo[] ps = Inventec.Common.Repository.Properties.Get<MPS.Processor.Mps000225.PDO.Mps000225BySereServ>();
                    foreach (var item in itemGroup)
                    {
                        for (int j = 0; j < 60; j++)
                        {
                            decimal value2 = 0;
                            decimal value3 = 0;
                            decimal value4 = 0;
                            decimal value5 = 0;
                            PropertyInfo propertyInfo = ps.FirstOrDefault(o => o.Name == string.Format("Day{0}", j + 1));
                            if (propertyInfo != null)
                            {
                                decimal itemValue = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo.GetValue(item) ?? "0").ToString());
                                decimal adoValue = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo.GetValue(ado) ?? "0").ToString());
                                if (itemValue > 0)
                                {
                                    propertyInfo.SetValue(ado, (adoValue + itemValue).ToString());
                                }
                            }
                            PropertyInfo propertyInfo2 = ps.FirstOrDefault(o => o.Name == string.Format("MORNING_Day{0}", j + 1));
                            if (propertyInfo2 != null)
                            {
                                decimal itemValue = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo2.GetValue(item) ?? "0").ToString());
                                decimal adoValue = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo2.GetValue(ado) ?? "0").ToString());
                                value2 = itemValue + adoValue;
                                if (itemValue > 0)
                                {
                                    propertyInfo2.SetValue(ado, (adoValue + itemValue).ToString());
                                }
                            }
                            PropertyInfo propertyInfo3 = ps.FirstOrDefault(o => o.Name == string.Format("NOON_Day{0}", j + 1));
                            if (propertyInfo3 != null)
                            {
                                decimal itemValue = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo3.GetValue(item) ?? "0").ToString());
                                decimal adoValue = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo3.GetValue(ado) ?? "0").ToString());
                                value3 = itemValue + adoValue;
                                if (itemValue > 0)
                                {
                                    propertyInfo3.SetValue(ado, (adoValue + itemValue).ToString());
                                }
                            }
                            PropertyInfo propertyInfo4 = ps.FirstOrDefault(o => o.Name == string.Format("AFTERNOON_Day{0}", j + 1));
                            if (propertyInfo4 != null)
                            {
                                decimal itemValue = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo4.GetValue(item) ?? "0").ToString());
                                decimal adoValue = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo4.GetValue(ado) ?? "0").ToString());
                                value4 = itemValue + adoValue;
                                if (itemValue > 0)
                                {
                                    propertyInfo4.SetValue(ado, (adoValue + itemValue).ToString());
                                }
                            }
                            PropertyInfo propertyInfo5 = ps.FirstOrDefault(o => o.Name == string.Format("EVENING_Day{0}", j + 1));
                            if (propertyInfo5 != null)
                            {
                                decimal itemValue = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo5.GetValue(item) ?? "0").ToString());
                                decimal adoValue = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo5.GetValue(ado) ?? "0").ToString());
                                value5 = itemValue + adoValue;
                                if (itemValue > 0)
                                {
                                    propertyInfo5.SetValue(ado, (adoValue + itemValue).ToString());
                                }
                            }
                            PropertyInfo propertyInfo6 = ps.FirstOrDefault(o => o.Name == string.Format("SUM_Day{0}", j + 1));
                            if (propertyInfo6 != null)
                            {
                                decimal sum_Day = value2 + value3 + value4 + value5;
                                if (sum_Day > 0)
                                {
                                    propertyInfo6.SetValue(ado, sum_Day.ToString());
                                }
                            }
                        }
                    }

                    result.Add(ado);
                }
            }

            return result;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000225ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
