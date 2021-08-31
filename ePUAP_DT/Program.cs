using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ePUAP_DT.Models;

namespace ePUAP_DT
{
    class Program
    {
        static void Main(string[] args)
        {
            AuthnRequest request = new AuthnRequest();

            XmlSerializer mySerializer = new XmlSerializer(typeof(AuthnRequest));

            StreamWriter myWriter = new StreamWriter("myFileName.xml");
            mySerializer.Serialize(myWriter, request);
            myWriter.Close();
        }
    }
}
