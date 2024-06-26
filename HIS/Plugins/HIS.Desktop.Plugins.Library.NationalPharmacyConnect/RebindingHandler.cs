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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.NationalPharmacyConnect
{
    public class RebindingHandler : DelegatingHandler
    {
        private BindIPEndPoint bindHandler;
        string ipAdapterAddressessInternet;

        public RebindingHandler(IEnumerable<IPAddress> adapterAddresses, HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new WebRequestHandler())
        {
            this.ipAdapterAddressessInternet = adapterAddresses.FirstOrDefault(o => o.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            //  Inventec.Common.Logging.LogSystem.Error("Ip check lay dc ip" + this.ipAdapterAddressessInternet);
            if (String.IsNullOrEmpty(this.ipAdapterAddressessInternet))
            {
                throw new ArgumentException();
            }
        }

        IPEndPoint Bind(ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount)
        {
            IPAddress address = IPAddress.Parse(this.ipAdapterAddressessInternet);
            return new IPEndPoint(address, 0);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var sp = ServicePointManager.FindServicePoint(request.RequestUri);
            //sp.BindIPEndPointDelegate = bindHandler;
            sp.BindIPEndPointDelegate = new BindIPEndPoint(Bind);
            var httpResponseMessage = await base.SendAsync(request, cancellationToken);
            //Inventec.Common.Logging.LogSystem.Info(httpResponseMessage.ToString());
            return httpResponseMessage;
        }
    }
}
