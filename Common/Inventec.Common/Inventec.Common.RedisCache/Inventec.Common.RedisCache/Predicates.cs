using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.RedisCache
{
    public class Predicates
    {
        public static object GetPropertyValue<T>(T data, string propertyName)
        {
            System.Reflection.PropertyInfo propertyInfoID = data.GetType().GetProperty(propertyName);
            if (propertyInfoID != null)
            {
                try
                {
                    return (propertyInfoID.GetValue(data, null));
                }
                catch (Exception ex)
                {
                }
            }
            return null;
        }
    }

}
