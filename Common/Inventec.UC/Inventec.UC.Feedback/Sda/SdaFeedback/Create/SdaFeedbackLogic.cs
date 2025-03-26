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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Feedback.Sda.SdaFeedback.Create
{
    internal class SdaFeedbackLogic : Inventec.UC.Feedback.Process.BusinessBase
    {
        public SdaFeedbackLogic()
            : base()
        {

        }

        public SdaFeedbackLogic(CommonParam paramGet)
            : base(paramGet)
        {

        }
        public SDA.EFMODEL.DataModels.SDA_FEEDBACK Create(SDA.EFMODEL.DataModels.SDA_FEEDBACK data)
        {
            bool valid = true;
            valid = valid && IsNotNull(param);
            valid = valid && IsNotNull(data);
            SDA.EFMODEL.DataModels.SDA_FEEDBACK result = null;
            #region Logging Input Data
            try
            {
                Input = Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param);
            }
            catch { }
            #endregion

            try
            {
                TokenCheck();
                result = new SdaFeedbackCreateFactory(param).CreateFactory(SdaFeedbackCreateFactory.FeedbacktType.DEFAULT, data).Create();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            TroubleCheck(result); return result;
        }
    }
}
