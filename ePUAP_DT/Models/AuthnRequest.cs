using ePUAP_DT.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ePUAP_DT.Models
{
    /// <summary>
    /// SAML2 AuthnRequest model
    /// </summary>
    [XmlRoot("AuthnRequest", Namespace = Namespaces.PROTOCOL)]
    public class AuthnRequest
    {

        public AuthnRequest()
        {
            this.NameIDPolicy = new NameIDPolicy();
            this.RequestedAuthnContext = new RequestAuthContext();
        }

        private XmlSerializerNamespaces _xmlns;

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns
        {
            get
            {
                if (_xmlns == null)
                {
                    _xmlns = new XmlSerializerNamespaces();
                    _xmlns.Add("saml2p", Namespaces.PROTOCOL);
                }
                //return _xmlns;

                return Namespaces.SerializerNamespaces;
            }

            set
            {
                _xmlns = value;
            }
        }

        [XmlAttribute("ID")]
        public string ID { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlAttribute("IssueInstant")]
        public DateTime IssueInstant { get; set; }

        //optional
        //[XmlAttribute("AssertionConsumerServiceURL")]
        //public string AssertionConsumerServiceURL { get; set; }

        [XmlAttribute("Destination")]
        public string Destination { get; set; }

        [XmlAttribute("ProtocolBinding")]
        public string ProtocolBinding { get; set; }

        [XmlElement("Issuer", Namespace = Namespaces.ASSERTION)]
        public string Issuer { get; set; }

        [XmlElement("NameIDPolicy", Namespace = Namespaces.PROTOCOL)]
        public NameIDPolicy NameIDPolicy { get; set; }

        [XmlElement("RequestedAuthnContext", Namespace = Namespaces.PROTOCOL)]
        public RequestAuthContext RequestedAuthnContext { get; set; }

    }

    public class NameIDPolicy
    {
        public NameIDPolicy()
        {
            this.Format = NameID.UNSPECIFIED;
        }

        [XmlAttribute("AllowCreate")]
        public bool AllowCreate { get; set; }

        [XmlAttribute("Format")]
        public string Format { get; set; }

    }

    /// <summary>
    /// NameID values
    /// </summary>
    public class NameID
    {
        public const string UNSPECIFIED = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";
        public const string PROVIDER = "urn:oasis:names:tc:SAML:2.0:nameid-format:provider";
        public const string FEDERATED = "urn:oasis:names:tc:SAML:2.0:nameid-format:federated";
        public const string TRANSIENT = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient";
        public const string PERSISTENT = "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent";
    }

    public class RequestAuthContext
    {
        [XmlAttribute("Comparison")]
        public AuthnContextComparisonType Comparison { get; set; }

        [XmlElement("AuthnContextClassRef", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public virtual string AuthnContextClassRef { get; set; }
    }

    public enum AuthnContextComparisonType
    {
        [XmlEnum("exact")]
        Exact,
        [XmlEnum("mininum")]
        Minimum,
        [XmlEnum("maximum")]
        Maximum,
        [XmlEnum("better")]
        Better,
    }
}
