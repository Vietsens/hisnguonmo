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

namespace HIS.Desktop.LocalStorage.ConfigButton.ADO
{
    public class ConfigButtonADO : SDA.EFMODEL.DataModels.SDA_MODULE_BUTTON_USER
    {
        public string BUTTON_CODE { get; set; }
        public string BUTTON_NAME { get; set; }
        public short? IS_GROUP { get; set; }
        public long? MAX_SIZE_HEIGHT { get; set; }
        public long? MAX_SIZE_WIDTH { get; set; }
        public long? MIN_SIZE_HEIGHT { get; set; }
        public long? MIN_SIZE_WIDTH { get; set; }
        public string MODULE_LINK { get; set; }
        public string BUTTON_GROUP_NAME { get; set; }
        public string BUTTON_TOOLTIP { get; set; }
        public string PARENT_CODE { get; set; }


        public ConfigButtonADO()
        {

        }

        public ConfigButtonADO(SDA.EFMODEL.DataModels.SDA_MODULE_BUTTON modButton)
        {
            try
            {
                this.BUTTON_CODE = modButton.BUTTON_CODE;
                this.BUTTON_NAME = modButton.BUTTON_NAME;
                this.IS_GROUP = modButton.IS_GROUP;
                this.MAX_SIZE_HEIGHT = modButton.MAX_SIZE_HEIGHT;
                this.MAX_SIZE_WIDTH = modButton.MAX_SIZE_WIDTH;
                this.MIN_SIZE_HEIGHT = modButton.MIN_SIZE_HEIGHT;
                this.MIN_SIZE_WIDTH = modButton.MIN_SIZE_WIDTH;
                this.MODULE_LINK = modButton.MODULE_LINK;
                this.PARENT_CODE = modButton.PARENT_CODE;
                this.IS_VISIBLE = modButton.IS_VISIBLE;
                this.NUM_ORDER = modButton.NUM_ORDER;
                this.BUTTON_TOOLTIP = modButton.BUTTON_TOOLTIP;
                this.BUTTON_GROUP_NAME = modButton.BUTTON_GROUP_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public ConfigButtonADO(SDA.EFMODEL.DataModels.SDA_MODULE_BUTTON_USER data, List<SDA.EFMODEL.DataModels.SDA_MODULE_BUTTON> configApps)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ConfigButtonADO>(this, data);
                var confApp = (configApps != null ? configApps.FirstOrDefault(o => o.ID == data.MODULE_BUTTON_ID) : new SDA.EFMODEL.DataModels.SDA_MODULE_BUTTON());
                if (confApp == null) throw new ArgumentNullException("Get confApp by id is null. id = " + data.MODULE_BUTTON_ID);

                this.BUTTON_CODE = confApp.BUTTON_CODE;
                this.BUTTON_NAME = confApp.BUTTON_NAME;
                this.IS_GROUP = confApp.IS_GROUP;
                this.MAX_SIZE_HEIGHT = confApp.MAX_SIZE_HEIGHT;
                this.MAX_SIZE_WIDTH = confApp.MAX_SIZE_WIDTH;
                this.MIN_SIZE_HEIGHT = confApp.MIN_SIZE_HEIGHT;
                this.MIN_SIZE_WIDTH = confApp.MIN_SIZE_WIDTH;
                this.MODULE_LINK = confApp.MODULE_LINK;
                this.PARENT_CODE = confApp.PARENT_CODE;
                this.BUTTON_TOOLTIP = confApp.BUTTON_TOOLTIP;
                this.BUTTON_GROUP_NAME = confApp.BUTTON_GROUP_NAME;
                this.IS_VISIBLE = (data != null ? (data.IS_VISIBLE.HasValue ? data.IS_VISIBLE : confApp.IS_VISIBLE) : confApp.IS_VISIBLE);
                this.NUM_ORDER = (data != null ? (data.NUM_ORDER.HasValue ? data.NUM_ORDER : confApp.NUM_ORDER) : confApp.NUM_ORDER);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
