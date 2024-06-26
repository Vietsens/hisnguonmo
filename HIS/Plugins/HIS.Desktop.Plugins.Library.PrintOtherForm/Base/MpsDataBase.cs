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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.Base
{
    public abstract class MpsDataBase
    {
        public Dictionary<string, object> dicParamPlus = new Dictionary<string, object>();
        public Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImageBarcodePlus = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
        public Dictionary<string, System.Drawing.Image> dicImagePlus = new Dictionary<string, Image>();

        public Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);

        public long TreatmentId { get; set; }
        public long ServiceReqId { get; set; }
        public long SereServId { get; set; }
        public long PatientId { get; set; }

        public MpsDataBase(long? _serviceReqId, long? _sereServId, long _treatmentId, long _patientId)
            : base()
        {
            this.TreatmentId = _treatmentId;
            this.ServiceReqId = _serviceReqId ?? 0;
            this.PatientId = _patientId;
            this.SereServId = _sereServId ?? 0;
        }

        public MpsDataBase(long _treatmentId, long _patientId)
            : base()
        {
            this.TreatmentId = _treatmentId;
            this.PatientId = _patientId;
        }

        public MpsDataBase()
            : base()
        {
        }
    }
}
