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
        private List<PatyAlterBhytADO> patyAlterBHYTADOs { get; set; }
        private List<SereServADO> sereServADOs { get; set; }
        private List<SereServADO> sereServADOsLoaiDV { get; set; }
        private List<SereServADO> sereServADOsByDepa { get; set; }
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

                objectTag.AddObjectData(store, "OtherPaySource", this.ListOtherSource);
                objectTag.AddObjectData(store, "PatyAlterBHYT", this.patyAlterBHYTADOs);
                objectTag.AddObjectData(store, "PatyAlterBHYTDepaRoom", this.patyAlterBHYTADOs);

                if (heinServiceTypeADOs_LoaiDV == null) heinServiceTypeADOs_LoaiDV = new List<HeinServiceTypeADO>();
                if (medicineLineADOs_LoaiDV == null) medicineLineADOs_LoaiDV = new List<MedicineLineADO>();
                if (HeinServiceTypeBeds_LoaiDV == null) HeinServiceTypeBeds_LoaiDV = new List<HeinServiceTypeADO>();
                if (sereServADOsLoaiDV == null) sereServADOsLoaiDV = new List<SereServADO>();

                // Mẫu KHÔNG có band khoa/phòng (ServiceGroupByDepa / ServiceGroupByRoom) = mẫu cũ nhiều viện đang dùng:
                // bind HeinServiceType/MedicineLine/HeinServiceTypeBed/Service, hoặc *LoaiDV lồng Service.
                // Với mẫu này phải cấp bộ gộp THUẦN theo loại DV (tương đương GroupType=None trước commit f6b733d58):
                // nếu cấp bộ gộp theo khoa+phòng thì dịch vụ cùng tên ở nhiều phòng tách dòng, loại DV lặp theo phòng.
                bool flatTemplate = IsFlatTemplate();

                #region bộ 1 theo khoa phòng
                objectTag.AddObjectData(store, "ServiceGroupByDepa", this.ServiceGroupByDepa);
                objectTag.AddObjectData(store, "ServiceGroupByRoom", this.ServiceGroupByRoom);
                objectTag.AddObjectData(store, "HeinServiceType", (flatTemplate ? heinServiceTypeADOs_LoaiDV : heinServiceTypeADOs).OrderBy(o => o.NUM_ORDER ?? 99999999).ToList());
                objectTag.AddObjectData(store, "MedicineLine", flatTemplate ? medicineLineADOs_LoaiDV : medicineLineADOs);
                objectTag.AddObjectData(store, "HeinServiceTypeBed", flatTemplate ? HeinServiceTypeBeds_LoaiDV : HeinServiceTypeBeds);
                objectTag.AddObjectData(store, "Service", flatTemplate ? sereServADOsLoaiDV : sereServADOs);

                objectTag.AddRelationship(store, "ServiceGroupByDepa", "ServiceGroupByRoom", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "HeinServiceType", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "MedicineLine", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "HeinServiceTypeBed", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "Service", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");

                objectTag.AddRelationship(store, "ServiceGroupByRoom", "Service", "GROUP_ROOM_ID", "GROUP_ROOM_ID");
                objectTag.AddRelationship(store, "ServiceGroupByRoom", "MedicineLine", "GROUP_ROOM_ID", "GROUP_ROOM_ID");
                objectTag.AddRelationship(store, "ServiceGroupByRoom", "HeinServiceType", "GROUP_ROOM_ID", "GROUP_ROOM_ID");
                objectTag.AddRelationship(store, "ServiceGroupByRoom", "HeinServiceTypeBed", "GROUP_ROOM_ID", "GROUP_ROOM_ID");

                objectTag.AddRelationship(store, "HeinServiceType", "Service", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceType", "MedicineLine", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceType", "HeinServiceTypeBed", "ID", "PARENT_ID");

                objectTag.AddRelationship(store, "MedicineLine", "Service", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "MedicineLine", "HeinServiceTypeBed", "ID", "MEDICINE_LINE_ID");

                objectTag.AddRelationship(store, "HeinServiceTypeBed", "Service", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID"); 
                #endregion

                #region bộ 2 theo khoa
                if (heinServiceTypeADOs_ByDepa == null) heinServiceTypeADOs_ByDepa = new List<HeinServiceTypeADO>();
                objectTag.AddObjectData(store, "HeinServiceTypeByDepa", heinServiceTypeADOs_ByDepa.OrderBy(o => o.NUM_ORDER ?? 99999999).ToList());
                objectTag.AddObjectData(store, "MedicineLineDepa", medicineLineADOs_Depa);
                objectTag.AddObjectData(store, "HeinServiceTypeBedDepa", HeinServiceTypeBeds_Depa);
                objectTag.AddObjectData(store, "ServiceByDepa", sereServADOsByDepa);

                objectTag.AddRelationship(store, "ServiceGroupByDepa", "HeinServiceTypeByDepa", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "MedicineLineDepa", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "HeinServiceTypeBedDepa", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "ServiceByDepa", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");

                objectTag.AddRelationship(store, "HeinServiceTypeByDepa", "ServiceByDepa", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeByDepa", "MedicineLineDepa", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeByDepa", "HeinServiceTypeBedDepa", "ID", "PARENT_ID");


                objectTag.AddRelationship(store, "MedicineLineDepa", "ServiceByDepa", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "MedicineLineDepa", "HeinServiceTypeBedDepa", "ID", "MEDICINE_LINE_ID");

                objectTag.AddRelationship(store, "HeinServiceTypeBedDepa", "ServiceByDepa", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID");
                #endregion

                #region bộ 3 theo loại hình dịch vụ
                objectTag.AddObjectData(store, "HeinServiceTypeLoaiDV", heinServiceTypeADOs_LoaiDV.OrderBy(o => o.NUM_ORDER ?? 99999999).ToList());
                objectTag.AddObjectData(store, "MedicineLineLoaiDV", medicineLineADOs_LoaiDV);
                objectTag.AddObjectData(store, "HeinServiceTypeBedLoaiDV", HeinServiceTypeBeds_LoaiDV);
                objectTag.AddObjectData(store, "ServiceLoaiDV", sereServADOsLoaiDV);

                objectTag.AddRelationship(store, "HeinServiceTypeLoaiDV", "ServiceLoaiDV", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeLoaiDV", "HeinServiceTypeBedLoaiDV", "ID", "PARENT_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeLoaiDV", "MedicineLineLoaiDV", "ID", "HEIN_SERVICE_TYPE_ID");

                objectTag.AddRelationship(store, "MedicineLineLoaiDV", "ServiceLoaiDV", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "MedicineLineLoaiDV", "HeinServiceTypeBedLoaiDV", "ID", "MEDICINE_LINE_ID");

                objectTag.AddRelationship(store, "HeinServiceTypeBedLoaiDV", "ServiceLoaiDV", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID");

                // Mẫu cũ lồng band Service (không phải ServiceLoaiDV) trong *LoaiDV: không có relationship thì FlexCel
                // in toàn bộ Service dưới mỗi loại DV / dòng thuốc / giường -> lặp dịch vụ. Nối lại như trước f6b733d58.
                objectTag.AddRelationship(store, "HeinServiceTypeLoaiDV", "Service", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "MedicineLineLoaiDV", "Service", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeBedLoaiDV", "Service", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID");
                #endregion

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

        /// <summary>
        /// Mẫu "phẳng" = template không có band ServiceGroupByDepa / ServiceGroupByRoom (named range __X__ trong file Excel).
        /// Lỗi đọc template -> coi như không phẳng (giữ nguyên hành vi hiện tại).
        /// </summary>
        private bool IsFlatTemplate()
        {
            try
            {
                FlexCel.XlsAdapter.XlsFile xls = new FlexCel.XlsAdapter.XlsFile(System.IO.Path.GetFullPath(fileName), true);
                for (int i = 1; i <= xls.NamedRangeCount; i++)
                {
                    string name = xls.GetNamedRange(i).Name ?? "";
                    if (name.Equals("__ServiceGroupByDepa__", StringComparison.OrdinalIgnoreCase)
                        || name.Equals("__ServiceGroupByRoom__", StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        private List<SurchargeADO> SurchargeProcess()

        {

            List<SurchargeADO> r = new List<SurchargeADO>();

            try
            {

                if (rdo.SurchargePayforms == null || rdo.SurchargePayforms.Count == 0) return r;

                int stt = 1;

                foreach (var item in rdo.SurchargePayforms.Where(o => (o.SURCHARGE_AMOUNT ?? 0) > 0).OrderBy(o => o.SORT_ORDER ?? 0))

                    r.Add(new SurchargeADO { STT = stt++, SURCHARGE_NAME = item.SURCHARGE_NAME, AMOUNT = 1, SURCHARGE_AMOUNT = item.SURCHARGE_AMOUNT ?? 0, SORT_ORDER = item.SORT_ORDER });

            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }

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
