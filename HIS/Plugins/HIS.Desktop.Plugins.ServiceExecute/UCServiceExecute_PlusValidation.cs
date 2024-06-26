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
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class UCServiceExecute : UserControlBase
    {
        private void ValidNumberOfFilm()
        {
            try
            {
                    Validation.FilmValidationRule FilmRule = new Validation.FilmValidationRule();
                    FilmRule.txtNumberOfFilm = txtNumberOfFilm;
                    FilmRule.serviceReqTypeId = ServiceReqConstruct.SERVICE_REQ_TYPE_ID;
                    FilmRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    this.dxValidationProvider1.SetValidationRule(txtNumberOfFilm, FilmRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBeginTime()
        {
            try
            {
                Validation.BeginTimeValidationRule valid = new Validation.BeginTimeValidationRule();
                valid.dtBeginTime = dtBeginTime;
                valid.dtEndTime = dtEndTime;
                valid.IntructionTime = ServiceReqConstruct.INTRUCTION_TIME;
                valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(dtBeginTime, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidEndTime()
        {
            try
            {
                Validation.EndTimeValidationRule valid = new Validation.EndTimeValidationRule();
                valid.dtEndTime = dtEndTime;
                valid.IntructionTime = ServiceReqConstruct.INTRUCTION_TIME;
                valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(dtEndTime, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
