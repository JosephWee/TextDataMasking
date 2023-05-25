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
                originalTexts.Add("<!DOCTYPE html>\r\n\r\n<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title></title>\r\n</head>\r\n<body>\r\n    <p>Hello World!</p>\r\n    <p>Hello <strong>Sol</strong>!</p>\r\n    <p>Salutation <i>Milky Way</i>!</p>\r\n    <div>\r\n        <div>\r\n            <div>\r\n                Div <strong>1</strong> Content\r\n            </div>\r\n            <div>\r\n                Div < strong>2< /strong> Content\r\n            </div>\r\n            <div>\r\n                Div <strong>3</ strong> Content\r\n            </div>\r\n            <div>\r\n                Div <strong >4</strong > Content\r\n            </div>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>");
                originalTexts.Add("Praesent mi quam, fringilla a arcu sit amet, malesuada tempus arcu. Interdum et malesuada fames ac ante ipsum primis in faucibus. Morbi enim felis, luctus non nibh at, tristique viverra enim. Cras quis tincidunt lorem, vitae rutrum orci. Cras malesuada, mauris in ultrices sagittis, sapien augue molestie augue, eu <strong>vulputate</strong> velit nibh a odio. Sed sed mi leo. Nam <strong>scelerisque</strong> sodales tincidunt. Sed tortor libero, lobortis nec lacus et, semper cursus neque. Duis molestie justo quis volutpat finibus. Sed commodo metus orci, in efficitur ligula congue eget. Phasellus ac erat pharetra, eleifend ipsum non, lobortis enim. Cras purus nulla, volutpat vel elementum in, vulputate id tellus. Phasellus in elementum ante, at lacinia tortor. Vivamus lacinia, elit vitae iaculis placerat, nisl nibh viverra nulla, a volutpat turpis ex ut elit. Nunc non magna in ex aliquam dictum. Curabitur commodo volutpat quam sit amet sagittis.\r\n\r\nQuisque commodo nunc auctor lectus feugiat viverra. Phasellus nec neque at sem scelerisque facilisis. Curabitur blandit massa feugiat nisi tempus, a ultrices mauris luctus. Phasellus faucibus leo at maximus interdum. Ut massa erat, varius eu finibus ac, maximus non tortor. Proin luctus maximus mi, sed dictum mauris bibendum in. Pellentesque viverra sem felis. Maecenas quis semper velit. Etiam non porta ex.");

                List<Uri> uris = new List<Uri>();
                uris.Add(new Uri("https://en.wikipedia.org/wiki/Lorem_ipsum"));
                uris.Add(new Uri("https://en.wikipedia.org/wiki/HTTP"));

                //GetHtml(uris).Wait();
            }
        }

        public async Task GetHtml(List<Uri> uris)
        {
            using (HttpClient client = new HttpClient())
            {
                var tasksHttpGet = new List<Task<HttpResponseMessage>>();
                for (int i = 0; i < uris.Count; i++)
                {
                    var uri = uris[i];
                    var task = client.GetAsync(uri.AbsoluteUri);
                    tasksHttpGet.Add(task);
                }

                var httpResponses = await Task.WhenAll(tasksHttpGet);

                var tasksReadContent = new List<Task<string>>();
                for (int i = 0; i < httpResponses.Length; i++)
                {
                    var task = httpResponses[i].Content.ReadAsStringAsync();
                    tasksReadContent.Add(task);
                }

                var htmlContents = await Task.WhenAll(tasksReadContent);
                originalTexts.AddRange(htmlContents);
            }
        }

        [Test]
        public void TestMaskText()
        {
            DatabaseMaskerOptions options = new DatabaseMaskerOptions();
            options.IgnoreAngleBracketedTags = true;
            options.IgnoreJson = true;

            MaskDictionary maskDictionary = new MaskDictionary();

            List<string> replacementTexts = new List<string>();

            for (int a = 0; a < originalTexts.Count; a++)
            {
                string originalText = originalTexts[a];
                string replacementText = DatabaseMasker.MaskText(originalText, options, maskDictionary);
                
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