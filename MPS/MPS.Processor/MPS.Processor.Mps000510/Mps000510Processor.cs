/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using Inventec.Core;
using MPS.Processor.Mps000510.ADO;
using MPS.Processor.Mps000510.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000510
{
    public partial class Mps000510Processor : AbstractProcessor
    {
        private List<SereServADO> sereServADOs { get; set; }
        private List<OtherSourceADO> ListOtherSource = new List<OtherSourceADO>();

        private Mps000510PDO rdo;
        private PrintData printData;

        public Mps000510Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000510PDO)rdoBase;
            this.printData = printData;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                DataInputProcess();
                ProcessSingleKey();
                SetQrCode();
                SetBarcodeKey();
                SetImageKey();

                // ghi đè PrintLogData và UniqueCodeData + lấy số lần in
                ProcessPrintLogData();
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                if (sereServADOs == null || sereServADOs.Count == 0)
                    return false;

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                // Bộ chi tiết phẳng + nguồn khác
                objectTag.AddObjectData(store, "Service", sereServADOs);
                objectTag.AddObjectData(store, "OtherPaySource", this.ListOtherSource);

                // Master gom theo loại hình DV / dòng thuốc / giường
                objectTag.AddObjectData(store, "HeinServiceType", heinServiceTypeADOs);
                objectTag.AddObjectData(store, "MedicineLine", medicineLineADOs);
                objectTag.AddObjectData(store, "HeinServiceTypeBed", HeinServiceTypeBeds);

                objectTag.AddRelationship(store, "HeinServiceType", "Service", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceType", "HeinServiceTypeBed", "ID", "PARENT_ID");
                objectTag.AddRelationship(store, "HeinServiceType", "MedicineLine", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "MedicineLine", "Service", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "MedicineLine", "HeinServiceTypeBed", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeBed", "Service", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID");

                // Master gom theo khoa / phòng, nối với Service qua cột khoá có sẵn
                objectTag.AddObjectData(store, "ServiceGroupByDepa", this.ServiceGroupByDepa);
                objectTag.AddObjectData(store, "ServiceGroupByRoom", this.ServiceGroupByRoom);

                objectTag.AddRelationship(store, "ServiceGroupByDepa", "Service", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "ServiceGroupByRoom", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroupByRoom", "Service", "GROUP_ROOM_ID", "GROUP_ROOM_ID");

                objectTag.AddObjectData(store, "Surcharge", SurchargeProcess()); // PTTK 2656


                objectTag.SetUserFunction(store, "ReplaceValue", new ReplaceValueFunction());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<SurchargeADO> SurchargeProcess()

        {

            List<SurchargeADO> r = new List<SurchargeADO>();

            try {

                if (rdo.SurchargePayforms == null || rdo.SurchargePayforms.Count == 0) return r;

                int stt = 1;

                foreach (var item in rdo.SurchargePayforms.Where(o => (o.SURCHARGE_AMOUNT ?? 0) > 0).OrderBy(o => o.SORT_ORDER ?? 0))

                    r.Add(new SurchargeADO { STT = stt++, SURCHARGE_NAME = item.SURCHARGE_NAME, AMOUNT = 1, SURCHARGE_AMOUNT = item.SURCHARGE_AMOUNT ?? 0, SORT_ORDER = item.SORT_ORDER });

            } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }

            return r;

        }


        class ReplaceValueFunction : FlexCel.Report.TFlexCelUserFunction
        {
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                try
                {
                    string value = parameters[0] + "";
                    if (!String.IsNullOrEmpty(value))
                        value = value.Replace(';', '/');
                    return value;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    return parameters[0];
                }
            }
        }
    }
}
