using UnityEngine.Networking;

namespace Exploratorium
{
    public class AcceptAllCertificates : CertificateHandler
    {
        private AcceptAllCertificates(){}
        public static readonly AcceptAllCertificates Handler = new AcceptAllCertificates();
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }
}