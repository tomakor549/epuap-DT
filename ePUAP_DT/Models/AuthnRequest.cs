using ePUAP_DT.Constant;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
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
            //this.RequestedAuthnContext = new RequestAuthContext();
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
                return _xmlns;

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

        //[XmlElement("RequestedAuthnContext", Namespace = Namespaces.PROTOCOL)]
        //public RequestAuthContext RequestedAuthnContext { get; set; }

        public virtual string GetSignedXML(X509Certificate2 signatureCertificate)
        {

            if (signatureCertificate == null ||
                 signatureCertificate.PrivateKey == null
                )
            {
                throw new ArgumentException("Can't compute signature without actual certificate");
            }
            // sign
            var xml = this.GetXML();
            var xmlrequest = new XmlDocument();
            xmlrequest.LoadXml(xml);

            SignedXml signedXml = new SignedXml(xmlrequest);

            try
            {
                var exportedKeyMaterial = signatureCertificate.PrivateKey.ToXmlString(true);
                var key = new RSACryptoServiceProvider(new CspParameters(24 /* PROV_RSA_AES */))
                {
                    PersistKeyInCsp = false
                };
                key.FromXmlString(exportedKeyMaterial);

                signedXml.SigningKey = key;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            //signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA256Url;
            signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmlenc#sha512";


            // transform
            var c14transform = new XmlDsigExcC14NTransform();

            // reference
            var reference = new Reference
            {
                Uri = "",
                DigestMethod = Namespaces.XmlDsigSHA256Url
            };
            reference.AddTransform(c14transform);
            //reference.DigestMethod = SignedXml.XmlDsigSHA256Url;

            // keyinfo
            var keyInfo = new KeyInfo();

            keyInfo.AddClause(new KeyInfoX509Data(signatureCertificate));
            signedXml.KeyInfo = keyInfo;

            // compose signature
            signedXml.AddReference(reference);
            signedXml.ComputeSignature();

            // import the signature node into the document
            var signatureXml = signedXml.GetXml();

            var mainNode = xmlrequest
                .DocumentElement.ChildNodes
                .Cast<XmlNode>()
                .FirstOrDefault(n => n.LocalName == "AuthnRequest");
            var signatureNode = mainNode
                        .ChildNodes
                        .Cast<XmlNode>()
                        .FirstOrDefault(n => n.LocalName == "Signature");
            var firstNode = mainNode.FirstChild;

            // insert it just after BSE
            var importedSignatureXml = xmlrequest.ImportNode(signatureXml, true);
            mainNode.InsertAfter(firstNode, importedSignatureXml);

            return xmlrequest.OuterXml;
        }

        private string GetXML()
        {
            // first, serialize to string
            var sb = new StringBuilder();
            var xs = new XmlSerializer(typeof(AuthnRequest));
            using (var sw = new StringWriter8(sb))
            {
                xs.Serialize(sw, this);
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// UTF-8 String writer
    /// </summary>
    public class StringWriter8 : StringWriter
    {
        public StringWriter8() : base() { }

        public StringWriter8(StringBuilder sb) : base(sb) { }

        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }
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
