using TextDataMasking;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace UnitTests
{
    public class TextDataMaskingTests
    {
        List<string> originalTexts = new List<string>();

        [SetUp]
        public void Setup()
        {
            if (originalTexts.Count == 0)
            {
                originalTexts.Add("\r\n\r\nLorem ipsum dolor 100 sit amet, consectetur adipiscing elit. Donec quis venenatis enim, vel 937 finibus lorem. Maecenas condimentum a magna eu varius. Suspendisse nec 123 placerat lorem. Praesent ac nulla 123mauris. Aenean vel placerat ante. Mauris eu eleifend arcu. Etiam 8735123 molestie arcu vel placerat fermentum. Nullam fringilla 12345.00 vel diam quis porta. Morbi fringilla mi non lacus laoreet eleifend. Sed vulputate sit amet nulla sit amet aliquet. Mauris a lorem diam.\r\n\r\nSed venenatis condimentum ultricies. Ut tincidunt euismod magna semper rhoncus. Vivamus vel facilisis est. Etiam convallis convallis risus, at facilisis orci porta vel. Nulla convallis odio quis leo elementum, non lobortis nibh tempor. Phasellus aliquet massa ut orci ultricies vulputate. Proin consectetur sapien eu odio imperdiet, ut viverra magna sagittis. Praesent neque nibh, porta ac pretium eu, ultrices et dui. Pellentesque euismod turpis ac risus tincidunt, sit amet eleifend mi auctor. Aliquam scelerisque malesuada dolor, eu ultricies lacus tincidunt a. In faucibus arcu ligula, non facilisis orci ullamcorper vitae.\r\n\r\nProin vestibulum nisl sed quam placerat, id fringilla odio iaculis. Suspendisse potenti. Morbi tristique nibh vitae ante lacinia lacinia. Duis dictum, turpis nec volutpat fermentum, ex lacus suscipit magna, sed porttitor diam neque at tortor. Praesent interdum posuere sollicitudin. Mauris nec https://www.google.com/search?q=test lacinia quam. Donec placerat faucibus nisl, vel vestibulum velit. Aenean malesuada varius neque vel aliquam. Nunc varius nulla non lectus sodales finibus. Donec tincidunt turpis at mi bibendum vulputate. Suspendisse potenti. Quisque consequat tincidunt augue, varius auctor neque consequat id.\r\n\r\nPraesent mi quam, fringilla a arcu sit amet, malesuada tempus arcu. Interdum et malesuada fames ac ante ipsum primis in faucibus. Morbi enim felis, luctus non nibh at, tristique viverra enim. Cras quis tincidunt lorem, vitae rutrum orci. Cras malesuada, mauris in ultrices sagittis, sapien augue molestie augue, eu vulputate velit nibh a odio. Sed sed mi leo. Nam scelerisque sodales tincidunt. Sed tortor libero, lobortis nec lacus et, semper cursus neque. Duis molestie justo quis https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference volutpat finibus. Sed commodo metus orci, in efficitur ligula congue eget. Phasellus ac erat pharetra, eleifend ipsum non, lobortis enim. Cras purus nulla, volutpat vel elementum in, vulputate id tellus. Phasellus in elementum ante, at lacinia tortor. Vivamus lacinia, elit vitae iaculis placerat, nisl nibh viverra nulla, a volutpat turpis ex ut elit. Nunc non magna in ex aliquam dictum. Curabitur commodo volutpat quam sit amet sagittis.\r\n\r\nQuisque commodo nunc auctor lectus feugiat viverra. Phasellus nec neque at sem scelerisque facilisis. Curabitur blandit massa feugiat nisi tempus, a ultrices mauris luctus. Phasellus faucibus leo at maximus interdum. Ut massa erat, varius eu finibus ac, maximus non tortor. Proin luctus maximus mi, sed dictum mauris bibendum in. Pellentesque viverra sem felis. Maecenas quis semper velit. Etiam non porta ex.");
                originalTexts.Add("Praesent mi quam, fringilla a arcu sit amet, malesuada tempus arcu. Interdum et malesuada fames ac ante ipsum primis in faucibus. Morbi enim felis, luctus non nibh at, tristique viverra enim. Cras quis tincidunt lorem, vitae rutrum orci. Cras malesuada, mauris in ultrices sagittis, sapien augue molestie augue, eu <strong>vulputate</strong> velit nibh a odio. Sed sed mi leo. Nam <strong>scelerisque</strong> sodales tincidunt. Sed tortor libero, lobortis nec lacus et, semper cursus neque. Duis molestie justo quis volutpat finibus. Sed commodo metus orci, in efficitur ligula congue eget. Phasellus ac erat pharetra, eleifend ipsum non, lobortis enim. Cras purus nulla, volutpat vel elementum in, vulputate id tellus. Phasellus in elementum ante, at lacinia tortor. Vivamus lacinia, elit vitae iaculis placerat, nisl nibh viverra nulla, a volutpat turpis ex ut elit. Nunc non magna in ex aliquam dictum. Curabitur commodo volutpat quam sit amet sagittis.\r\n\r\nQuisque commodo nunc auctor lectus feugiat viverra. Phasellus nec neque at sem scelerisque facilisis. Curabitur blandit massa feugiat nisi tempus, a ultrices mauris luctus. Phasellus faucibus leo at maximus interdum. Ut massa erat, varius eu finibus ac, maximus non tortor. Proin luctus maximus mi, sed dictum mauris bibendum in. Pellentesque viverra sem felis. Maecenas quis semper velit. Etiam non porta ex.");
                originalTexts.Add("<!DOCTYPE html>\r\n\r\n<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title></title>\r\n</head>\r\n<body>\r\n    <p>Hello World!</p>\r\n    <p>Hello <strong>Sol</strong>!</p>\r\n    <p>Salutation <i>Milky Way</i>!</p>\r\n    <div>\r\n        <div>\r\n            <div>\r\n                Div <strong>1</strong> Content\r\n            </div>\r\n            <div>\r\n                Div < strong>2< /strong> Content\r\n            </div>\r\n            <div>\r\n                Div <strong>3</ strong> Content\r\n            </div>\r\n            <div>\r\n                Div <strong >4</strong > Content\r\n            </div>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>");
                originalTexts.Add("<?xml version=\"1.0\"?>\r\n<catalog>\r\n\t<book id=\"bk101\">\r\n\t\t<author>Gambardella, Matthew</author>\r\n\t\t<title>XML Developer's Guide</title>\r\n\t\t<genre>Computer</genre>\r\n\t\t<price>44.95</price>\r\n\t\t<publish_date>2000-10-01</publish_date>\r\n\t\t<description>\r\n\t\t\tAn in-depth look at creating applications\r\n\t\t\twith XML.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk102\">\r\n\t\t<author>Ralls, Kim</author>\r\n\t\t<title>Midnight Rain</title>\r\n\t\t<genre>Fantasy</genre>\r\n\t\t<price>5.95</price>\r\n\t\t<publish_date>2000-12-16</publish_date>\r\n\t\t<description>\r\n\t\t\tA former architect battles corporate zombies,\r\n\t\t\tan evil sorceress, and her own childhood to become queen\r\n\t\t\tof the world.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk103\">\r\n\t\t<author>Corets, Eva</author>\r\n\t\t<title>Maeve Ascendant</title>\r\n\t\t<genre>Fantasy</genre>\r\n\t\t<price>5.95</price>\r\n\t\t<publish_date>2000-11-17</publish_date>\r\n\t\t<description>\r\n\t\t\tAfter the collapse of a nanotechnology\r\n\t\t\tsociety in England, the young survivors lay the\r\n\t\t\tfoundation for a new society.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk104\">\r\n\t\t<author>Corets, Eva</author>\r\n\t\t<title>Oberon's Legacy</title>\r\n\t\t<genre>Fantasy</genre>\r\n\t\t<price>5.95</price>\r\n\t\t<publish_date>2001-03-10</publish_date>\r\n\t\t<description>\r\n\t\t\tIn post-apocalypse England, the mysterious\r\n\t\t\tagent known only as Oberon helps to create a new life\r\n\t\t\tfor the inhabitants of London. Sequel to Maeve\r\n\t\t\tAscendant.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk105\">\r\n\t\t<author>Corets, Eva</author>\r\n\t\t<title>The Sundered Grail</title>\r\n\t\t<genre>Fantasy</genre>\r\n\t\t<price>5.95</price>\r\n\t\t<publish_date>2001-09-10</publish_date>\r\n\t\t<description>\r\n\t\t\tThe two daughters of Maeve, half-sisters,\r\n\t\t\tbattle one another for control of England. Sequel to\r\n\t\t\tOberon's Legacy.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk106\">\r\n\t\t<author>Randall, Cynthia</author>\r\n\t\t<title>Lover Birds</title>\r\n\t\t<genre>Romance</genre>\r\n\t\t<price>4.95</price>\r\n\t\t<publish_date>2000-09-02</publish_date>\r\n\t\t<description>\r\n\t\t\tWhen Carla meets Paul at an ornithology\r\n\t\t\tconference, tempers fly as feathers get ruffled.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk107\">\r\n\t\t<author>Thurman, Paula</author>\r\n\t\t<title>Splish Splash</title>\r\n\t\t<genre>Romance</genre>\r\n\t\t<price>4.95</price>\r\n\t\t<publish_date>2000-11-02</publish_date>\r\n\t\t<description>\r\n\t\t\tA deep sea diver finds true love twenty\r\n\t\t\tthousand leagues beneath the sea.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk108\">\r\n\t\t<author>Knorr, Stefan</author>\r\n\t\t<title>Creepy Crawlies</title>\r\n\t\t<genre>Horror</genre>\r\n\t\t<price>4.95</price>\r\n\t\t<publish_date>2000-12-06</publish_date>\r\n\t\t<description>\r\n\t\t\tAn anthology of horror stories about roaches,\r\n\t\t\tcentipedes, scorpions  and other insects.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk109\">\r\n\t\t<author>Kress, Peter</author>\r\n\t\t<title>Paradox Lost</title>\r\n\t\t<genre>Science Fiction</genre>\r\n\t\t<price>6.95</price>\r\n\t\t<publish_date>2000-11-02</publish_date>\r\n\t\t<description>\r\n\t\t\tAfter an inadvertant trip through a Heisenberg\r\n\t\t\tUncertainty Device, James Salway discovers the problems\r\n\t\t\tof being quantum.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk110\">\r\n\t\t<author>O'Brien, Tim</author>\r\n\t\t<title>Microsoft .NET: The Programming Bible</title>\r\n\t\t<genre>Computer</genre>\r\n\t\t<price>36.95</price>\r\n\t\t<publish_date>2000-12-09</publish_date>\r\n\t\t<description>\r\n\t\t\tMicrosoft's .NET initiative is explored in\r\n\t\t\tdetail in this deep programmer's reference.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk111\">\r\n\t\t<author>O'Brien, Tim</author>\r\n\t\t<title>MSXML3: A Comprehensive Guide</title>\r\n\t\t<genre>Computer</genre>\r\n\t\t<price>36.95</price>\r\n\t\t<publish_date>2000-12-01</publish_date>\r\n\t\t<description>\r\n\t\t\tThe Microsoft MSXML3 parser is covered in\r\n\t\t\tdetail, with attention to XML DOM interfaces, XSLT processing,\r\n\t\t\tSAX and more.\r\n\t\t</description>\r\n\t</book>\r\n\t<book id=\"bk112\">\r\n\t\t<author>Galos, Mike</author>\r\n\t\t<title>Visual Studio 7: A Comprehensive Guide</title>\r\n\t\t<genre>Computer</genre>\r\n\t\t<price>49.95</price>\r\n\t\t<publish_date>2001-04-16</publish_date>\r\n\t\t<description>\r\n\t\t\tMicrosoft Visual Studio 7 is explored in depth,\r\n\t\t\tlooking at how Visual Basic, Visual C++, C#, and ASP+ are\r\n\t\t\tintegrated into a comprehensive development\r\n\t\t\tenvironment.\r\n\t\t</description>\r\n\t</book>\r\n</catalog>\r\n");
                originalTexts.Add(
@"
<h:table>
  <h:tr>
    <h:td>Apples</h:td>
    <h:td>Bananas</h:td>
  </h:tr>
</h:table>

<f:table>
  <f:name>Oak Coffee Table</f:name>
  <f:width>120</f:width>
  <f:length>120</f:length>
</f:table>");

                originalTexts.Add(
@"
<person>
  <gender>female</gender>
  <firstname>Jenny</firstname>
  <lastname>Anderson</lastname>
</person>");

                originalTexts.Add(
@"
<item id=""1000"" price=""100"" weight=""10""
color=""brown"" width=""120"" length=""120"" height=""100""
description=""Rustic Oak Coffee Table"">
</item>");

                originalTexts.Add(
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
}");

                originalTexts.Add(
@"{
  ""alg"": ""HS256"",
  ""typ"": ""JWT""
}");

                originalTexts.Add(
@"{
  ""loggedInAs"": ""admin"",
  ""iat"": 1422779638
}");

                originalTexts.Add(
@"{
  ""access_token"": ""eyJhbABCDEFGHIJKLMNOPQRSTUVWXYZ"",
  ""token_type"": ""Bearer"",
  ""expires_in"": 3600
}");
            }
        }

        [Test]
        public void TestMaskText()
        {
            DataMaskerOptions options = new DataMaskerOptions();
            options.IgnoreAngleBracketedTags = true;
            options.IgnoreJsonAttributes = true;
            options.IgnoreNumbers = true;

            MaskDictionary maskDictionary = new MaskDictionary();

            List<string> replacementTexts = new List<string>();

            for (int a = 0; a < originalTexts.Count; a++)
            {
                string originalText = originalTexts[a];
                string replacementText = TextDataMasker.MaskText(originalText, options, maskDictionary);
                
                replacementTexts.Add(replacementText);

                string[] originalWords = Regex.Split(originalText, @"\s+");
                string[] replacementWords = Regex.Split(replacementText, @"\s+");

                var wordsWithDiffLength = new List<Tuple<string, string>>();
                for (int i = 0; i < originalWords.Length; i++)
                {
                    if (originalWords[i].Length != replacementWords[i].Length)
                    {
                        wordsWithDiffLength.Add(new Tuple<string, string>(originalWords[i], replacementWords[i]));
                    }
                }

                Assert.AreEqual(originalWords.Length, replacementWords.Length);
                Assert.AreEqual(0, wordsWithDiffLength.Count);
                Assert.AreEqual(originalText.Length, replacementText.Length);
            }

            Assert.AreEqual(originalTexts.Count, replacementTexts.Count);
        }
    }
}