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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Inventec.Common.Logging
{
    public static class LogUtil
    {
        //private static List<Type> listAllowType = new List<Type>() { typeof(string), typeof(long), typeof(Nullable<long>), typeof(decimal), typeof(Nullable<decimal>), typeof(short), typeof(Nullable<short>), typeof(bool), typeof(Nullable<bool>), typeof(int), typeof(Nullable<int>), typeof(double), typeof(Nullable<double>), typeof(float), typeof(Nullable<float>) };

        /// <summary>
        /// GetMemberName(() => abc)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            try
            {
                MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
                return expressionBody.Member.Name;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Su dung CommonUtil.GetMemberName(() => variable) de lay ra ten bien variable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string TraceData(string name, object data)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("___");
                sb.Append(name + ":");
                sb.Append(JsonConvert.SerializeObject(data));
                sb.Append("___");

                return sb.ToString();
            }
            catch (Exception)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("___Inventec.Common.Logging.LogUtil.TraceData has exception when trace data [" + name + "]___");
                    return sb.ToString();
                }
                catch (Exception)
                {
                    return "___Inventec.Common.Logging.LogUtil.TraceData has exception___";
                }
            }
        }

        public static string TraceDbException(System.Data.Entity.Validation.DbEntityValidationException e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("___Loi tuong tac CDSL (DbEntityValidationException)").Append("{");
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.Append(string.Format("{0}:{1}; ", ve.PropertyName, ve.ErrorMessage));
                    }
                }
                sb.Append("}___");
                return sb.ToString();
            }
            catch (Exception)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("___Loi tuong tac CSDL").Append("{...LogUtil.TraceDbException. Co exception khi logging...}___");
                    return sb.ToString();
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public static void LogActionSuccess(string className, string methodName, string userName)
        {
            try
            {
                LogSystem.Info(new StringBuilder(className).Append(".").Append(methodName).Append(".Username=").Append(userName).Append(".Xu ly thanh cong.").ToString());
            }
            catch (Exception ex)
            {
                try
                {
                    LogSystem.Error("___Co exception trong qua trinh ghi log success action___");
                    LogSystem.Error(ex);
                }
                catch (Exception)
                {

                }
            }
        }

        public static void LogActionFail(string className, string methodName, string userName)
        {
            try
            {
                LogSystem.Info(new StringBuilder(className).Append(".").Append(methodName).Append(".Username=").Append(userName).Append(".Xu ly that bai.").ToString());
            }
            catch (Exception ex)
            {
                try
                {
                    LogSystem.Error("___Co exception trong qua trinh ghi log fail action___");
                    LogSystem.Error(ex);
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
