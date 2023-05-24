using TextDataMasking;
using System.Text.RegularExpressions;

namespace UnitTests
{
    public class TextDataMaskingTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestMaskText()
        {
            MaskDictionary maskDictionary = new MaskDictionary();

            string originalText = "\r\n\r\nLorem ipsum dolor 100 sit amet, consectetur adipiscing elit. Donec quis venenatis enim, vel 937 finibus lorem. Maecenas condimentum a magna eu varius. Suspendisse nec 123 placerat lorem. Praesent ac nulla 123mauris. Aenean vel placerat ante. Mauris eu eleifend arcu. Etiam 8735123 molestie arcu vel placerat fermentum. Nullam fringilla 12345.00 vel diam quis porta. Morbi fringilla mi non lacus laoreet eleifend. Sed vulputate sit amet nulla sit amet aliquet. Mauris a lorem diam.\r\n\r\nSed venenatis condimentum ultricies. Ut tincidunt euismod magna semper rhoncus. Vivamus vel facilisis est. Etiam convallis convallis risus, at facilisis orci porta vel. Nulla convallis odio quis leo elementum, non lobortis nibh tempor. Phasellus aliquet massa ut orci ultricies vulputate. Proin consectetur sapien eu odio imperdiet, ut viverra magna sagittis. Praesent neque nibh, porta ac pretium eu, ultrices et dui. Pellentesque euismod turpis ac risus tincidunt, sit amet eleifend mi auctor. Aliquam scelerisque malesuada dolor, eu ultricies lacus tincidunt a. In faucibus arcu ligula, non facilisis orci ullamcorper vitae.\r\n\r\nProin vestibulum nisl sed quam placerat, id fringilla odio iaculis. Suspendisse potenti. Morbi tristique nibh vitae ante lacinia lacinia. Duis dictum, turpis nec volutpat fermentum, ex lacus suscipit magna, sed porttitor diam neque at tortor. Praesent interdum posuere sollicitudin. Mauris nec https://www.google.com/search?q=test lacinia quam. Donec placerat faucibus nisl, vel vestibulum velit. Aenean malesuada varius neque vel aliquam. Nunc varius nulla non lectus sodales finibus. Donec tincidunt turpis at mi bibendum vulputate. Suspendisse potenti. Quisque consequat tincidunt augue, varius auctor neque consequat id.\r\n\r\nPraesent mi quam, fringilla a arcu sit amet, malesuada tempus arcu. Interdum et malesuada fames ac ante ipsum primis in faucibus. Morbi enim felis, luctus non nibh at, tristique viverra enim. Cras quis tincidunt lorem, vitae rutrum orci. Cras malesuada, mauris in ultrices sagittis, sapien augue molestie augue, eu vulputate velit nibh a odio. Sed sed mi leo. Nam scelerisque sodales tincidunt. Sed tortor libero, lobortis nec lacus et, semper cursus neque. Duis molestie justo quis https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference volutpat finibus. Sed commodo metus orci, in efficitur ligula congue eget. Phasellus ac erat pharetra, eleifend ipsum non, lobortis enim. Cras purus nulla, volutpat vel elementum in, vulputate id tellus. Phasellus in elementum ante, at lacinia tortor. Vivamus lacinia, elit vitae iaculis placerat, nisl nibh viverra nulla, a volutpat turpis ex ut elit. Nunc non magna in ex aliquam dictum. Curabitur commodo volutpat quam sit amet sagittis.\r\n\r\nQuisque commodo nunc auctor lectus feugiat viverra. Phasellus nec neque at sem scelerisque facilisis. Curabitur blandit massa feugiat nisi tempus, a ultrices mauris luctus. Phasellus faucibus leo at maximus interdum. Ut massa erat, varius eu finibus ac, maximus non tortor. Proin luctus maximus mi, sed dictum mauris bibendum in. Pellentesque viverra sem felis. Maecenas quis semper velit. Etiam non porta ex.";
            string replacementText = DatabaseMasker.MaskText(maskDictionary, originalText);
            
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
    }
}