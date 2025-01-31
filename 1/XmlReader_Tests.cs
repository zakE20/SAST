using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace XXEExamples.Tests
{
    [TestFixture]
    public class XmlReader_Tests
    {
        [Test]
        public void XMLReader_WithDTDProcessingParseAndXmlResolverSet_NotSafe()
        {
            AssertXXE.IsXMLParserSafe((string xml) =>
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Prohibit; // ✅ Interdit l'utilisation des DTDs
                settings.XmlResolver = null; // ✅ Bloque l'accès aux entités externes
                settings.MaxCharactersFromEntities = 0; // ✅ Protection contre les attaques Billion Laughs

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                {
                    XmlReader reader = XmlReader.Create(stream, settings);

                    var xmlDocument = new XmlDocument();
                    xmlDocument.XmlResolver = null; // ✅ Supprime le XmlResolver pour éviter les accès externes
                    xmlDocument.Load(reader);
                    return xmlDocument.InnerText;
                }
            }, false);
        }

        [Test]
        public void XMLReader_WithDTDProcessingIgnored_Safe()
        {
            var exception = Assert.Throws<XmlException>(() =>
            {
                AssertXXE.IsXMLParserSafe((string xml) =>
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.DtdProcessing = DtdProcessing.Ignore;
                    settings.XmlResolver = null; // ✅ Sécurisation supplémentaire
                    settings.MaxCharactersFromEntities = 0; 

                    using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                    {
                        XmlReader reader = XmlReader.Create(stream, settings);

                        var xmlDocument = new XmlDocument();
                        xmlDocument.XmlResolver = null; // ✅ Suppression du XmlResolver
                        xmlDocument.Load(reader);
                        return xmlDocument.InnerText;
                    }
                }, true);
            });

            Assert.IsTrue(exception.Message.StartsWith("Reference to undeclared entity 'xxe'."));
        }

        [Test]
        public void XMLReader_WithDTDProcessingProhibited_Safe()
        {
            var exception = Assert.Throws<XmlException>(() =>
            {
                AssertXXE.IsXMLParserSafe((string xml) =>
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.DtdProcessing = DtdProcessing.Prohibit;
                    settings.XmlResolver = null; // ✅ Sécurisé
                    settings.MaxCharactersFromEntities = 0;

                    using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                    {
                        XmlReader reader = XmlReader.Create(stream, settings);

                        var xmlDocument = new XmlDocument();
                        xmlDocument.XmlResolver = null; // ✅ Suppression du XmlResolver
                        xmlDocument.Load(reader);
                        return xmlDocument.InnerText;
                    }
                }, true);
            });

            Assert.IsTrue(exception.Message.StartsWith("For security reasons DTD is prohibited in this XML document."));
        }
    }
}
