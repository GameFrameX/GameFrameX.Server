using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace SuperSocket.ClientEngine
{
	// Token: 0x02000016 RID: 22
	public class SslStreamTcpSession : AuthenticatedStreamTcpSession
	{
		// Token: 0x060000CA RID: 202 RVA: 0x00003A7C File Offset: 0x00001C7C
		protected override void StartAuthenticatedStream(Socket client)
		{
			if (base.Security == null)
			{
				throw new Exception("securityOption was not configured");
			}
			this.AuthenticateAsClientAsync(new SslStream(new NetworkStream(client), false, new RemoteCertificateValidationCallback(this.ValidateRemoteCertificate)), base.Security);
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00003AB8 File Offset: 0x00001CB8
		private async void AuthenticateAsClientAsync(SslStream sslStream, SecurityOption securityOption)
		{
			try
			{
				await sslStream.AuthenticateAsClientAsync(base.HostName, securityOption.Certificates, securityOption.EnabledSslProtocols, false);
			}
			catch (Exception e)
			{
				base.EnsureSocketClosed();
				this.OnError(e);
				return;
			}
			base.OnAuthenticatedStreamConnected(sslStream);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00003B04 File Offset: 0x00001D04
		private bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			if (base.Security.AllowNameMismatchCertificate)
			{
				sslPolicyErrors &= ~SslPolicyErrors.RemoteCertificateNameMismatch;
			}
			if (base.Security.AllowCertificateChainErrors)
			{
				sslPolicyErrors &= ~SslPolicyErrors.RemoteCertificateChainErrors;
			}
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			if (!base.Security.AllowUnstrustedCertificate)
			{
				this.OnError(new Exception(sslPolicyErrors.ToString()));
				return false;
			}
			if (sslPolicyErrors != SslPolicyErrors.None && sslPolicyErrors != SslPolicyErrors.RemoteCertificateChainErrors)
			{
				this.OnError(new Exception(sslPolicyErrors.ToString()));
				return false;
			}
			if (chain != null && chain.ChainStatus != null)
			{
				foreach (X509ChainStatus x509ChainStatus in chain.ChainStatus)
				{
					if ((!(certificate.Subject == certificate.Issuer) || x509ChainStatus.Status != X509ChainStatusFlags.UntrustedRoot) && x509ChainStatus.Status != X509ChainStatusFlags.NoError)
					{
						this.OnError(new Exception(sslPolicyErrors.ToString()));
						return false;
					}
				}
			}
			return true;
		}
	}
}
