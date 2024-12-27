using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace HIS.UC.FormType.RoomCombo
{
    class RoomComboProcessor : ProcessorBase, IProcessorGenerate
    {
        object generateRDO;
        internal RoomComboProcessor(V_SAR_RETY_FOFI config, object generateRDO)
            : base(config)
        {
            this.generateRDO = generateRDO;
        }

        object IProcessorGenerate.Run()
        {
            object result = null;
            try
            {
                result = new UCRoomCombo(config, generateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
