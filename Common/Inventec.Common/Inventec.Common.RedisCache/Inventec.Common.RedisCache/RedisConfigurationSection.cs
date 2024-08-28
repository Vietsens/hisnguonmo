using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.RedisCache
{
    public class RedisConfigurationManager
    {
        #region Constants

        private const string SectionName = "RedisConfiguration";

        public static RedisConfigurationSection Config
        {
            get
            {
                return (RedisConfigurationSection)ConfigurationManager.GetSection(SectionName);
            }
        }

        #endregion
    }
}
