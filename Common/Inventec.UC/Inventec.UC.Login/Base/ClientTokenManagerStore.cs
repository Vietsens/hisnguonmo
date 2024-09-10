using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Base
{
    public class ClientTokenManagerStore
    {
        private static Inventec.Token.ClientSystem.ClientTokenManager mosClientTokenManager;
        public static Inventec.Token.ClientSystem.ClientTokenManager ClientTokenManager
        {
            get
            {
                if (mosClientTokenManager == null)
                {
                    mosClientTokenManager = new Inventec.Token.ClientSystem.ClientTokenManager(AppConfig.APPLICATION_CODE);
                }
                return mosClientTokenManager;
            }
            set
            {
                mosClientTokenManager = value;
            }
        }
    }
}
