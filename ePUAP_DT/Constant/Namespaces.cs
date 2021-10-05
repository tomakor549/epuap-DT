using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ePUAP_DT.Constant
{
    /// <summary>
    /// Saml2 namespaces
    /// </summary>
    class Namespaces
    {
        /// <summary>
        /// Assertion namespace
        /// </summary>
        public const string ASSERTION = "urn:oasis:names:tc:SAML:2.0:assertion";

        /// <summary>
        /// Protocol namespace
        /// </summary>
        public const string PROTOCOL = "urn:oasis:names:tc:SAML:2.0:protocol";

        /// <summary>
        /// XmlDsig namespace
        /// </summary>
        public const string XMLDSIG = "http://www.w3.org/2000/09/xmldsig#";

        // <summary>
        /// XmlDsigSHA512Url namespace
        /// </summary>
        public const string XmlDsigSHA256Url = "http://www.w3.org/2001/04/xmlenc#sha512";

        /// <summary>
        /// In order to conform to common SAML namespacing, the serializer
        /// needs to be told how do to it
        /// </summary>
        public static XmlSerializerNamespaces SerializerNamespaces
        {
            get
            {
                var serializerNamespaces = new XmlSerializerNamespaces();

                serializerNamespaces.Add("ds", Namespaces.XMLDSIG);
                serializerNamespaces.Add("saml", Namespaces.ASSERTION);
                serializerNamespaces.Add("samlp", Namespaces.PROTOCOL);

                return serializerNamespaces;
            }
        }
    }
}
