using TextDataMasking;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System;
using HtmlAgilityPack;
using System.Xml;
using System.Text.Json;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using System.Text;
using Microsoft.VisualBasic;

namespace UnitTests
{
    public class TextDataMaskingTests
    {
        public enum TextType
        {
            PlainText = 0,
            Html,
            Xml,
            Json
        }

        public class TextContainer
        {
            public string Value { get; set; } = string.Empty;

            public TextType TextType { get; set; } = TextType.PlainText;

            public TextContainer(string text, TextType type)
            {
                this.Value = text;
                this.TextType = type;
            }
        }

        byte[] bom = new byte[] { (byte)0xEF, (byte)0xBB, (byte)0xBF };
        List<string> vowels = new List<string>() { "a", "e", "i", "o", "u" };
        List<TextContainer> originalTexts = new List<TextContainer>();

        [SetUp]
        public void Setup()
        {
            if (originalTexts.Count == 0)
            {
                originalTexts.Add(new TextContainer(" Alpha bravo123 456charlie, 10000 deLta echo fox789trot GryphoN! ", TextType.PlainText));
                originalTexts.Add(new TextContainer("\r\n\r\nLorem ipsum dolor 100 sit amet, consectetur adipiscing elit. Donec quis venenatis enim, vel 937 finibus lorem. Maecenas condimentum a magna eu varius. Suspendisse nec 123 placerat lorem. Praesent ac nulla 123mauris. Aenean vel placerat ante. Mauris eu eleifend arcu. Etiam 8735123 molestie arcu vel placerat fermentum. Nullam fringilla 12345.00 vel diam quis porta. Morbi fringilla mi non lacus laoreet eleifend. Sed vulputate sit amet nulla sit amet aliquet. Mauris a lorem diam.\r\n\r\nSed venenatis condimentum ultricies. Ut tincidunt euismod magna semper rhoncus. Vivamus vel facilisis est. Etiam convallis convallis risus, at facilisis orci porta vel. Nulla convallis odio quis leo elementum, non lobortis nibh tempor. Phasellus aliquet massa ut orci ultricies vulputate. Proin consectetur sapien eu odio imperdiet, ut viverra magna sagittis. Praesent neque nibh, porta ac pretium eu, ultrices et dui. Pellentesque euismod turpis ac risus tincidunt, sit amet eleifend mi auctor. Aliquam scelerisque malesuada dolor, eu ultricies lacus tincidunt a. In faucibus arcu ligula, non facilisis orci ullamcorper vitae.\r\n\r\nProin vestibulum nisl sed quam placerat, id fringilla odio iaculis. Suspendisse potenti. Morbi tristique nibh vitae ante lacinia lacinia. Duis dictum, turpis nec volutpat fermentum, ex lacus suscipit magna, sed porttitor diam neque at tortor. Praesent interdum posuere sollicitudin. Mauris nec https://www.google.com/search?q=test lacinia quam. Donec placerat faucibus nisl, vel vestibulum velit. Aenean malesuada varius neque vel aliquam. Nunc varius nulla non lectus sodales finibus. Donec tincidunt turpis at mi bibendum vulputate. Suspendisse potenti. Quisque consequat tincidunt augue, varius auctor neque consequat id.\r\n\r\nPraesent mi quam, fringilla a arcu sit amet, malesuada tempus arcu. Interdum et malesuada fames ac ante ipsum primis in faucibus. Morbi enim felis, luctus non nibh at, tristique viverra enim. Cras quis tincidunt lorem, vitae rutrum orci. Cras malesuada, mauris in ultrices sagittis, sapien augue molestie augue, eu vulputate velit nibh a odio. Sed sed mi leo. Nam scelerisque sodales tincidunt. Sed tortor libero, lobortis nec lacus et, semper cursus neque. Duis molestie justo quis https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference volutpat finibus. Sed commodo metus orci, in efficitur ligula congue eget. Phasellus ac erat pharetra, eleifend ipsum non, lobortis enim. Cras purus nulla, volutpat vel elementum in, vulputate id tellus. Phasellus in elementum ante, at lacinia tortor. Vivamus lacinia, elit vitae iaculis placerat, nisl nibh viverra nulla, a volutpat turpis ex ut elit. Nunc non magna in ex aliquam dictum. Curabitur commodo volutpat quam sit amet sagittis.\r\n\r\nQuisque commodo nunc auctor lectus feugiat viverra. Phasellus nec neque at sem scelerisque facilisis. Curabitur blandit massa feugiat nisi tempus, a ultrices mauris luctus. Phasellus faucibus leo at maximus interdum. Ut massa erat, varius eu finibus ac, maximus non tortor. Proin luctus maximus mi, sed dictum mauris bibendum in. Pellentesque viverra sem felis. Maecenas quis semper velit. Etiam non porta ex.", TextType.PlainText));
                originalTexts.Add(new TextContainer("Praesent mi quam, fringilla a arcu sit amet, malesuada tempus arcu. Interdum et malesuada fames ac ante ipsum primis in faucibus. Morbi enim felis, luctus non nibh at, tristique viverra enim. Cras quis tincidunt lorem, vitae rutrum orci. Cras malesuada, mauris in ultrices sagittis, sapien augue molestie augue, eu <strong>vulputate</strong> velit nibh a odio. Sed sed mi leo. Nam <strong>scelerisque</strong> sodales tincidunt. Sed tortor libero, lobortis nec lacus et, semper cursus neque. Duis molestie justo quis volutpat finibus. Sed commodo metus orci, in efficitur ligula congue eget. Phasellus ac erat pharetra, eleifend ipsum non, lobortis enim. Cras purus nulla, volutpat vel elementum in, vulputate id tellus. Phasellus in elementum ante, at lacinia tortor. Vivamus lacinia, elit vitae iaculis placerat, nisl nibh viverra nulla, a volutpat turpis ex ut elit. Nunc non magna in ex aliquam dictum. Curabitur commodo volutpat quam sit amet sagittis.\r\n\r\nQuisque commodo nunc auctor lectus feugiat viverra. Phasellus nec neque at sem scelerisque facilisis. Curabitur blandit massa feugiat nisi tempus, a ultrices mauris luctus. Phasellus faucibus leo at maximus interdum. Ut massa erat, varius eu finibus ac, maximus non tortor. Proin luctus maximus mi, sed dictum mauris bibendum in. Pellentesque viverra sem felis. Maecenas quis semper velit. Etiam non porta ex.", TextType.PlainText));
                originalTexts.Add(new TextContainer("<!DOCTYPE html>\r\n\r\n<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title></title>\r\n</head>\r\n<body>\r\n    <p>Hello World!</p>\r\n    <p>Hello <strong>Sol</strong>!</p>\r\n    <p>Salutation <i>Milky Way</i>!</p>\r\n    <div>\r\n        <div>\r\n            <div>\r\n                Div <strong>1</strong> Content\r\n            </div>\r\n            <div>\r\n                Div < strong>2< /strong> Content\r\n            </div>\r\n            <div>\r\n                Div <strong>3</ strong> Content\r\n            </div>\r\n            <div>\r\n                Div <strong >4</strong > Content\r\n            </div>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>", TextType.Html));
                originalTexts.Add(new TextContainer("<?xml version=\"1.0\"?>\r\n<catalog>\r\n\t<book id=\"bk101\">\r\n\t\t<author>Gambardella, Matthew</author>\r\n\t\t<title>XML Developer's Guide</title>\r\n\t\t<genre>Computer</genre>\r\n\t\t<price>44.95</price>\r\n\t\t<publish_date>2000-10-01</publish_date>\r\n\t\t<description>\r\n\t\t\tAn in-depth look at creating applications\r\n\t\t\twith XML.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk102\">\r\n\t\t<author>Ralls, Kim</author>\r\n\t\t<title>Midnight Rain</title>\r\n\t\t<genre>Fantasy</genre>\r\n\t\t<price>5.95</price>\r\n\t\t<publish_date>2000-12-16</publish_date>\r\n\t\t<description>\r\n\t\t\tA former architect battles corporate zombies,\r\n\t\t\tan evil sorceress, and her own childhood to become queen\r\n\t\t\tof the world.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk103\">\r\n\t\t<author>Corets, Eva</author>\r\n\t\t<title>Maeve Ascendant</title>\r\n\t\t<genre>Fantasy</genre>\r\n\t\t<price>5.95</price>\r\n\t\t<publish_date>2000-11-17</publish_date>\r\n\t\t<description>\r\n\t\t\tAfter the collapse of a nanotechnology\r\n\t\t\tsociety in England, the young survivors lay the\r\n\t\t\tfoundation for a new society.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk104\">\r\n\t\t<author>Corets, Eva</author>\r\n\t\t<title>Oberon's Legacy</title>\r\n\t\t<genre>Fantasy</genre>\r\n\t\t<price>5.95</price>\r\n\t\t<publish_date>2001-03-10</publish_date>\r\n\t\t<description>\r\n\t\t\tIn post-apocalypse England, the mysterious\r\n\t\t\tagent known only as Oberon helps to create a new life\r\n\t\t\tfor the inhabitants of London. Sequel to Maeve\r\n\t\t\tAscendant.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk105\">\r\n\t\t<author>Corets, Eva</author>\r\n\t\t<title>The Sundered Grail</title>\r\n\t\t<genre>Fantasy</genre>\r\n\t\t<price>5.95</price>\r\n\t\t<publish_date>2001-09-10</publish_date>\r\n\t\t<description>\r\n\t\t\tThe two daughters of Maeve, half-sisters,\r\n\t\t\tbattle one another for control of England. Sequel to\r\n\t\t\tOberon's Legacy.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk106\">\r\n\t\t<author>Randall, Cynthia</author>\r\n\t\t<title>Lover Birds</title>\r\n\t\t<genre>Romance</genre>\r\n\t\t<price>4.95</price>\r\n\t\t<publish_date>2000-09-02</publish_date>\r\n\t\t<description>\r\n\t\t\tWhen Carla meets Paul at an ornithology\r\n\t\t\tconference, tempers fly as feathers get ruffled.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk107\">\r\n\t\t<author>Thurman, Paula</author>\r\n\t\t<title>Splish Splash</title>\r\n\t\t<genre>Romance</genre>\r\n\t\t<price>4.95</price>\r\n\t\t<publish_date>2000-11-02</publish_date>\r\n\t\t<description>\r\n\t\t\tA deep sea diver finds true love twenty\r\n\t\t\tthousand leagues beneath the sea.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk108\">\r\n\t\t<author>Knorr, Stefan</author>\r\n\t\t<title>Creepy Crawlies</title>\r\n\t\t<genre>Horror</genre>\r\n\t\t<price>4.95</price>\r\n\t\t<publish_date>2000-12-06</publish_date>\r\n\t\t<description>\r\n\t\t\tAn anthology of horror stories about roaches,\r\n\t\t\tcentipedes, scorpions  and other insects.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk109\">\r\n\t\t<author>Kress, Peter</author>\r\n\t\t<title>Paradox Lost</title>\r\n\t\t<genre>Science Fiction</genre>\r\n\t\t<price>6.95</price>\r\n\t\t<publish_date>2000-11-02</publish_date>\r\n\t\t<description>\r\n\t\t\tAfter an inadvertant trip through a Heisenberg\r\n\t\t\tUncertainty Device, James Salway discovers the problems\r\n\t\t\tof being quantum.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk110\">\r\n\t\t<author>O'Brien, Tim</author>\r\n\t\t<title>Microsoft .NET: The Programming Bible</title>\r\n\t\t<genre>Computer</genre>\r\n\t\t<price>36.95</price>\r\n\t\t<publish_date>2000-12-09</publish_date>\r\n\t\t<description>\r\n\t\t\tMicrosoft's .NET initiative is explored in\r\n\t\t\tdetail in this deep programmer's reference.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk111\">\r\n\t\t<author>O'Brien, Tim</author>\r\n\t\t<title>MSXML3: A Comprehensive Guide</title>\r\n\t\t<genre>Computer</genre>\r\n\t\t<price>36.95</price>\r\n\t\t<publish_date>2000-12-01</publish_date>\r\n\t\t<description>\r\n\t\t\tThe Microsoft MSXML3 parser is covered in\r\n\t\t\tdetail, with attention to XML DOM interfaces, XSLT processing,\r\n\t\t\tSAX and more.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk112\">\r\n\t\t<author>Galos, Mike</author>\r\n\t\t<title>Visual Studio 7: A Comprehensive Guide</title>\r\n\t\t<genre>Computer</genre>\r\n\t\t<price>49.95</price>\r\n\t\t<publish_date>2001-04-16</publish_date>\r\n\t\t<description>\r\n\t\t\tMicrosoft Visual Studio 7 is explored in depth,\r\n\t\t\tlooking at how Visual Basic, Visual C++, C#, and ASP+ are\r\n\t\t\tintegrated into a comprehensive development\r\n\t\t\tenvironment.\r\n\t\t</description>\r\n\t</book>\r\n</catalog>\r\n", TextType.Xml));
                originalTexts.Add(
                    new TextContainer(
@"
<orders>
    <f:table xmlns:f=""http://www.fictionalfruitswholesaler.com/xsd/"">
      <f:tr>
        <f:td>Apples</f:td>
        <f:td>Bananas</f:td>
      </f:tr>
    </f:table>

    <ad:table xmlns:ad=""http://www.fictionalantiquesdealer.com/xsd/"">
      <ad:name>Oak Coffee Table</ad:name>
      <ad:width>120</ad:width>
      <ad:length>120</ad:length>
    </ad:table>
</orders>", TextType.Xml));

                originalTexts.Add(
                    new TextContainer(
@"
<person>
  <gender>female</gender>
  <firstname>Jenny</firstname>
  <lastname>Anderson</lastname>
</person>", TextType.Xml));

                originalTexts.Add(
                    new TextContainer(
@"
<item id=""1000"" price=""100"" weight=""10""
color=""brown"" width=""120"" length=""120"" height=""100""
description=""Rustic Oak Coffee Table"">
</item>", TextType.Xml));

                originalTexts.Add(
                    new TextContainer(
@"{
    ""Position"": {
                    ""Title"": ""Editor"",
                    ""Name"": ""James Dean""
                  },
    ""   MyKey   "": ""My appsettings.json Value"",
    ""Logging"": {
                   ""LogLevel"": {
                                   ""Default"": ""Information"",
                                   ""Microsoft"": ""Warning"",
                                   ""Microsoft.Hosting.Lifetime"": ""Information""
                                 }
                 },
    ""AllowedHosts"": ""*""
}", TextType.Json));

                originalTexts.Add(
                    new TextContainer(
@"{
  ""alg"": ""HS256"",
  ""typ"": ""JWT""
}", TextType.Json));

                originalTexts.Add(
                    new TextContainer(
@"{
  ""loggedInAs"": ""admin"",
  ""iat"": 1422779638
}", TextType.Json));

                originalTexts.Add(
                    new TextContainer(
@"{
  ""access_token"": ""eyJhbABCDEFGHIJKLMNOPQRSTUVWXYZ"",
  ""token_type"": ""Bearer"",
  ""expires_in"": 3600
}", TextType.Json));

                originalTexts.Add(
                    new TextContainer(
@"{
  ""firstname"": ""John"",
  ""lastname"": ""Mason"",
  ""height"": 185,
  ""weight"": 80,
  ""membership"": ""Bronze"",
  ""member_id"": ""532120473519"",
  ""gym_access"": true,
  ""spa_access"": false,
  ""free_wifi"": true,
  ""club_suite"": null
}", TextType.Json));

                originalTexts.Add(
                    new TextContainer(
@"{
  ""place_id"": ""e361757a-d6f7-4b59-bf30-5a167f7eafd6"",
  ""type"": 0,
  ""formatted_address"": ""Topaz Shopping Center, #1424, 41 Jalan Post Oak, Punggol, Singapore, 312243"",
  ""address_parts"": [
    {
      ""long_name"": ""Topaz Shopping Center"",
      ""short_name"": ""Topaz Shopping Center"",
      ""type_name"": ""building_name""
    },
    {
      ""long_name"": ""#1424"",
      ""short_name"": ""#1424"",
      ""type_name"": ""building_name""
    },
    {
      ""long_name"": ""41"",
      ""short_name"": ""41"",
      ""type_name"": ""street_number""
    },
    {
      ""long_name"": ""Jalan Post Oak"",
      ""short_name"": ""Jalan Post Oak"",
      ""type_name"": ""street""
    },
    {
      ""long_name"": ""Punggol"",
      ""short_name"": ""Punggol"",
      ""type_name"": ""locality""
    },
    {
      ""long_name"": ""Singapore"",
      ""short_name"": ""Singapore"",
      ""type_name"": ""country""
    },
    {
      ""long_name"": ""312243"",
      ""short_name"": ""312243"",
      ""type_name"": ""postal_code""
    }
  ],
  ""geometry"": {
    ""location"": {
      ""lat"": 61.729200931498404,
      ""lng"": -94.51174330226262
    },
    ""location_type"": 0,
    ""viewport"": {
      ""southeast"": {
        ""lat"": 86.28535410844314,
        ""lng"": -72.64794947435894
      },
      ""northwest"": {
        ""lat"": 86.20062200122035,
        ""lng"": -73.06500870822384
      }
    }
  }
}", TextType.Json));
            }
        }

        [Test]
        public void TestMaskText_IgnoreAll()
        {
            DataMaskerOptions options = new DataMaskerOptions();
            options.IgnoreAngleBracketedTags = true;
            options.IgnoreJsonAttributes = true;
            options.IgnoreNumbers = true;
            options.PreserveCase = true;

            TestMaskText(options);
        }

        [Test]
        public void TestMaskText_IgnoreAngleBracketedTags()
        {
            DataMaskerOptions options = new DataMaskerOptions();
            options.IgnoreAngleBracketedTags = true;
            options.IgnoreJsonAttributes = false;
            options.IgnoreNumbers = false;
            options.PreserveCase = true;

            TestMaskText(options);
        }

        [Test]
        public void TestMaskText_IgnoreJsonAttributes()
        {
            DataMaskerOptions options = new DataMaskerOptions();
            options.IgnoreAngleBracketedTags = false;
            options.IgnoreJsonAttributes = true;
            options.IgnoreNumbers = false;
            options.PreserveCase = true;

            TestMaskText(options);
        }

        [Test]
        public void TestMaskText_IgnoreNumbers()
        {
            DataMaskerOptions options = new DataMaskerOptions();
            options.IgnoreAngleBracketedTags = false;
            options.IgnoreJsonAttributes = false;
            options.IgnoreNumbers = true;
            options.PreserveCase = true;

            TestMaskText(options);
        }

        [Test]
        public void TestMaskText_KeepTextFormatting()
        {
            DataMaskerOptions options = new DataMaskerOptions();
            options.IgnoreAngleBracketedTags = true;
            options.IgnoreJsonAttributes = true;
            options.IgnoreNumbers = false;
            options.PreserveCase = true;

            TestMaskText(options);
        }

        public void TestMaskText(DataMaskerOptions options)
        {
            MaskDictionary maskDictionary = new MaskDictionary();

            List<TextContainer> replacementTexts = new List<TextContainer>();

            for (int a = 0; a < originalTexts.Count; a++)
            {
                TextContainer originalText = originalTexts[a];
                string replacement = TextDataMasker.MaskText(originalText.Value, options, maskDictionary);
                TextContainer replacementText = new TextContainer(replacement, originalText.TextType);

                Assert.AreEqual(originalText.Value.Length, replacementText.Value.Length);

                if (originalText.TextType == TextType.Html && options.IgnoreAngleBracketedTags)
                {
                    ValidateHtml(originalText.Value, replacementText.Value, options);
                }
                else if (originalText.TextType == TextType.Xml && options.IgnoreAngleBracketedTags)
                {
                    ValidateXml(originalText.Value, replacementText.Value, options);
                }
                else if (originalText.TextType == TextType.Json && options.IgnoreJsonAttributes)
                {
                    ValidateJson(originalText.Value, replacementText.Value, options);
                }
                else
                {
                    ValidatePlainText(originalText.Value, replacementText.Value, options);
                }

                replacementTexts.Add(replacementText);
            }

            ///string pattern = @"(\s+)|(<\w+(\s+\w+(\s*=\s*[""'][^""'<>&]+[""']){0,1})*\s*>)|(</\w+>)|([^A-Za-z0-9]+)";
            Assert.AreEqual(originalTexts.Count, replacementTexts.Count);
        }

        public void ValidatePlainText(string original, string replacement, DataMaskerOptions options)
        {
            List<string> originalWords = Regex.Split(original, @"\s+").ToList();
            List<string> replacementWords = Regex.Split(replacement, @"\s+").ToList();

            Assert.AreEqual(originalWords.Count, replacementWords.Count);

            for (int i = 0; i < originalWords.Count; i++)
            {
                string originalWord = originalWords[i];
                string replacementWord = replacementWords[i];

                CompareStrings(originalWord, replacementWord, options);
            }
        }

        public void ValidateHtml(string original, string replacement, DataMaskerOptions options)
        {
            HtmlDocument oDoc = new HtmlDocument();
            oDoc.LoadHtml(original);

            HtmlDocument rDoc = new HtmlDocument();
            rDoc.LoadHtml(replacement);

            ValidateHtmlElement(oDoc.DocumentNode, rDoc.DocumentNode, options);
        }

        public void ValidateHtmlElement(HtmlNode original, HtmlNode replacement, DataMaskerOptions options)
        {
            Assert.AreEqual(original.NodeType, replacement.NodeType);
            Assert.AreEqual(original.Name, replacement.Name);
            Assert.AreEqual(original.Attributes.Count, replacement.Attributes.Count);

            for (int a = 0; a < original.Attributes.Count; a++)
            {
                var oAttr = original.Attributes[a];
                var rAttr = original.Attributes[a];

                Assert.AreEqual(oAttr.Name, rAttr.Name);
                Assert.AreEqual(oAttr.Value, rAttr.Value);
            }

            Assert.AreEqual(original.ChildNodes.Count, replacement.ChildNodes.Count);

            for (int c = 0; c < original.ChildNodes.Count; c++)
            {
                var oChild = original.ChildNodes[c];
                var rChild = replacement.ChildNodes[c];

                ValidateHtmlElement(oChild, rChild, options);
            }

            if (original.NodeType == HtmlNodeType.Comment || original.NodeType == HtmlNodeType.Text)
            {
                CompareStrings(original.InnerText, replacement.InnerText, options);
            }
        }

        public void ValidateXml(string original, string replacement, DataMaskerOptions options)
        {
            XmlDocument oDoc = new XmlDocument();
            oDoc.LoadXml(original);

            XmlDocument rDoc = new XmlDocument();
            rDoc.LoadXml(replacement);

            ValidateXmlElement(oDoc.DocumentElement, rDoc.DocumentElement, options);
        }

        public void ValidateXmlElement(XmlNode original, XmlNode replacement, DataMaskerOptions options)
        {
            Assert.AreEqual(original.NodeType, replacement.NodeType);
            Assert.AreEqual(original.Name, replacement.Name);
            Assert.AreEqual(original.Attributes == null, replacement.Attributes == null);

            if (original.Attributes != null)
            {
                Assert.AreEqual(original.Attributes.Count, replacement.Attributes.Count);

                for (int a = 0; a < original.Attributes.Count; a++)
                {
                    var oAttr = original.Attributes[a];
                    var rAttr = original.Attributes[a];

                    Assert.AreEqual(oAttr.Name, rAttr.Name);
                    Assert.AreEqual(oAttr.Value, rAttr.Value);
                }
            }

            Assert.AreEqual(original.ChildNodes == null, replacement.ChildNodes == null);

            if (original.ChildNodes != null)
            {
                Assert.AreEqual(original.ChildNodes.Count, replacement.ChildNodes.Count);

                for (int c = 0; c < original.ChildNodes.Count; c++)
                {
                    var oChild = original.ChildNodes[c];
                    var rChild = replacement.ChildNodes[c];

                    ValidateXmlElement(oChild, rChild, options);
                }
            }

            CompareStrings(original.Value, replacement.Value, options);
        }

        public void ValidateJson(string original, string replacement, DataMaskerOptions options)
        {
            var oElement = JsonSerializer.Deserialize<JsonElement>(original);
            var rElement = JsonSerializer.Deserialize<JsonElement>(replacement);

            ValidateJsonElement(oElement, rElement, options);
        }

        public void ValidateJsonElement(JsonElement original, JsonElement replacement, DataMaskerOptions options)
        {
            Assert.AreEqual(original.ValueKind, replacement.ValueKind);

            if (original.ValueKind == JsonValueKind.Array)
            {
                var oEnumerator = original.EnumerateArray();
                var rEnumerator = replacement.EnumerateArray();

                Assert.AreEqual(oEnumerator.Count(), rEnumerator.Count());

                for (int i = 0; i < oEnumerator.Count(); i++)
                {
                    var oItem = oEnumerator.ElementAt(i);
                    var rItem = rEnumerator.ElementAt(i);
                    ValidateJsonElement(oItem, rItem, options);
                }
            }
            else if (original.ValueKind == JsonValueKind.Object)
            {
                var oEnumerator = original.EnumerateObject();
                var rEnumerator = replacement.EnumerateObject();

                Assert.AreEqual(oEnumerator.Count(), rEnumerator.Count());

                for (int i = 0; i < oEnumerator.Count(); i++)
                {
                    var oProp = oEnumerator.ElementAt(i);
                    var rProp = rEnumerator.ElementAt(i);

                    Assert.AreEqual(oProp.Name, rProp.Name);

                    ValidateJsonElement(oProp.Value, rProp.Value, options);
                }
            }
            else if (original.ValueKind == JsonValueKind.False
                    || original.ValueKind == JsonValueKind.True)
            {
                Assert.AreEqual(original.GetBoolean(), replacement.GetBoolean());
            }
            else if (original.ValueKind == JsonValueKind.Number)
            {
                SByte oSByte, rSByte;
                if (original.TryGetSByte(out oSByte) && replacement.TryGetSByte(out rSByte))
                {
                    Assert.AreEqual(options.IgnoreNumbers, oSByte == rSByte);
                }

                UInt16 oUInt16, rUInt16;
                if (original.TryGetUInt16(out oUInt16) && replacement.TryGetUInt16(out rUInt16))
                {
                    Assert.AreEqual(options.IgnoreNumbers, oUInt16 == rUInt16);
                }

                UInt32 oUInt32, rUInt32;
                if (original.TryGetUInt32(out oUInt32) && replacement.TryGetUInt32(out rUInt32))
                {
                    Assert.AreEqual(options.IgnoreNumbers, oUInt32 == rUInt32);
                }

                UInt64 oUInt64, rUInt64;
                if (original.TryGetUInt64(out oUInt64) && replacement.TryGetUInt64(out rUInt64))
                {
                    Assert.AreEqual(options.IgnoreNumbers, oUInt64 == rUInt64);
                }

                byte oByte, rByte;
                if (original.TryGetByte(out oByte) && replacement.TryGetByte(out rByte))
                {
                    Assert.AreEqual(options.IgnoreNumbers, oByte == rByte);
                }

                Int16 oInt16, rInt16;
                if (original.TryGetInt16(out oInt16) && replacement.TryGetInt16(out rInt16))
                {
                    Assert.AreEqual(options.IgnoreNumbers, oInt16 == rInt16);
                }

                Int32 oInt32, rInt32;
                if (original.TryGetInt32(out oInt32) && replacement.TryGetInt32(out rInt32))
                {
                    Assert.AreEqual(options.IgnoreNumbers, oInt32 == rInt32);
                }

                Int64 oInt64, rInt64;
                if (original.TryGetInt64(out oInt64) && replacement.TryGetInt64(out rInt64))
                {
                    Assert.AreEqual(options.IgnoreNumbers, oInt64 == rInt64);
                }

                Single oSingle, rSingle;
                if (original.TryGetSingle(out oSingle) && replacement.TryGetSingle(out rSingle))
                {
                    Assert.AreEqual(options.IgnoreNumbers, oSingle == rSingle);
                }

                Double oDouble, rDouble;
                if (original.TryGetDouble(out oDouble) && replacement.TryGetDouble(out rDouble))
                {
                    Assert.AreEqual(options.IgnoreNumbers, oDouble == rDouble);
                }

                Decimal oDecimal, rDecimal;
                if (original.TryGetDecimal(out oDecimal) && replacement.TryGetDecimal(out rDecimal))
                {
                    Assert.AreEqual(options.IgnoreNumbers, oDecimal == rDecimal);
                }
            }
            else
            {
                string oString = original.GetString();
                string rString = replacement.GetString();

                CompareStrings(oString, rString, options);
            }
        }

        protected void CompareStrings(string string1, string string2, DataMaskerOptions options)
        {
            Assert.AreEqual(string.IsNullOrEmpty(string1), string.IsNullOrEmpty(string2));

            if (string.IsNullOrEmpty(string1))
                return;

            Assert.AreEqual(string1.Length, string2.Length);

            if ((BothMatch(string1, string2, @"^\W+$")))
            {
                Assert.AreEqual(string1, string2);
            }
            else if (
                (BothMatch(string1, string2, @"^\d+$"))
                || (BothMatch(string1, string2, @"\W+")
                    && BothMatch(string1, string2, @"\d+")
                    && !BothMatch(string1, string2, @"[A-Za-z]+"))
            )
            {
                Assert.AreEqual(options.IgnoreNumbers, string1 == string2);
            }
            else
            {
                Assert.AreNotEqual(string1, string2);
            }
        }

        protected bool BothMatch(string string1, string string2, string pattern)
        {
            return Regex.IsMatch(string1, pattern) && Regex.IsMatch(string2, pattern);
        }
    }
}