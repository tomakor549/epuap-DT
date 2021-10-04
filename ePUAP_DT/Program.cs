using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        }
    }
}
