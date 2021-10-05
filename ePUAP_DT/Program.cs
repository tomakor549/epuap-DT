using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ePUAP_DT.Models;

namespace ePUAP_DT
{
    class Program
    {
        static void Main(string[] args)
        {

            AuthnRequest request = new AuthnRequest
            {
                ID = string.Format("ID_{0}", Guid.NewGuid()),
                Version = "2.0",
                IssueInstant = DateTime.Now,
                //AssertionConsumerServiceURL = "http://client.system.pl/index",
                ProtocolBinding = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact",
                Destination = "https://pz.gov.pl/dt/SingleSignOnService",
                Issuer = "https://klient01.pl"
            };
            XmlSerializer mySerializer = new XmlSerializer(typeof(AuthnRequest));

            StreamWriter myWriter = new StreamWriter("myFileName.xml");
            mySerializer.Serialize(myWriter, request);
            myWriter.Close();

            //===============================================================================================================================
            X509Certificate2 Certificate = null;

            // Read the certificate from the store

            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                // Try to find the certificate
                // based on its common name
                X509Certificate2Collection Results =
                store.Certificates.Find(
                X509FindType.FindBySerialNumber, "74b337b17c6fd58648ec1317b068a8ad", false);

                if (Results.Count == 0)
                    throw new Exception("Unable to find certificate!");
                if (Results.Count > 1)
                    throw new Exception("More than 1 certificate meets the conditions!");
                else
                    Certificate = Results[0];
            }
            finally
            {
                store.Close();
            }

            var str = request.GetSignedXML(Certificate);
        }
    }
}
