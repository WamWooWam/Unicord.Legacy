using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto;

namespace WebSocket4Net
{
    class WebSocketTlsClient : DefaultTlsClient
    {
        public WebSocketTlsClient(TlsCrypto crypto)
            : base(crypto)
        {

        }

        public override TlsAuthentication GetAuthentication()
        {
            return new WebSocketTlsAuthentication();
        }
    }

    class WebSocketTlsAuthentication : ServerOnlyTlsAuthentication
    {
        public override void NotifyServerCertificate(TlsServerCertificate serverCertificate) { }
    }
}
