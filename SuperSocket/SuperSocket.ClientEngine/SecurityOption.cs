using System;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace SuperSocket.ClientEngine
{
	// Token: 0x02000015 RID: 21
	public class SecurityOption
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x000039B2 File Offset: 0x00001BB2
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x000039BA File Offset: 0x00001BBA
		public SslProtocols EnabledSslProtocols { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000BA RID: 186 RVA: 0x000039C3 File Offset: 0x00001BC3
		// (set) Token: 0x060000BB RID: 187 RVA: 0x000039CB File Offset: 0x00001BCB
		public X509CertificateCollection Certificates { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000BC RID: 188 RVA: 0x000039D4 File Offset: 0x00001BD4
		// (set) Token: 0x060000BD RID: 189 RVA: 0x000039DC File Offset: 0x00001BDC
		public bool AllowUnstrustedCertificate { get; set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000BE RID: 190 RVA: 0x000039E5 File Offset: 0x00001BE5
		// (set) Token: 0x060000BF RID: 191 RVA: 0x000039ED File Offset: 0x00001BED
		public bool AllowNameMismatchCertificate { get; set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x000039F6 File Offset: 0x00001BF6
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x000039FE File Offset: 0x00001BFE
		public bool AllowCertificateChainErrors { get; set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x00003A07 File Offset: 0x00001C07
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x00003A0F File Offset: 0x00001C0F
		public NetworkCredential Credential { get; set; }

		// Token: 0x060000C4 RID: 196 RVA: 0x00003A18 File Offset: 0x00001C18
		public SecurityOption() : this(SecurityOption.GetDefaultProtocol(), new X509CertificateCollection())
		{
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00003A2A File Offset: 0x00001C2A
		public SecurityOption(SslProtocols enabledSslProtocols) : this(enabledSslProtocols, new X509CertificateCollection())
		{
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00003A38 File Offset: 0x00001C38
		public SecurityOption(SslProtocols enabledSslProtocols, X509Certificate certificate) : this(enabledSslProtocols, new X509CertificateCollection(new X509Certificate[]
		{
			certificate
		}))
		{
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00003A50 File Offset: 0x00001C50
		public SecurityOption(SslProtocols enabledSslProtocols, X509CertificateCollection certificates)
		{
			this.EnabledSslProtocols = enabledSslProtocols;
			this.Certificates = certificates;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00003A66 File Offset: 0x00001C66
		public SecurityOption(NetworkCredential credential)
		{
			this.Credential = credential;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00003A75 File Offset: 0x00001C75
		private static SslProtocols GetDefaultProtocol()
		{
			return SslProtocols.Tls11 | SslProtocols.Tls12;
		}
	}
}
