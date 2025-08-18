using i4p;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("helloworld", "abcdefgijkl", "Rejtjelezett üzenet: hfnosauzun")]
        [TestCase("alma", "korte", "Rejtjelezett üzenet: kzct")]
        public void CodeTest(string message, string key, string expected)
        {
            Rejtjelezes rejtjelezes = new Rejtjelezes(message, key);
            Assert.That(rejtjelezes.Code(), Is.EqualTo(expected));

            

        }



        [TestCase("hfnosauzun", "abcdefgijkl", "helloworld")]
        [TestCase("kzct", "korte", "alma")]
        public void MessageTest(string code, string key, string expected)
        {
            Rejtjelezes rejtjelezes = new Rejtjelezes(code, key);
            Assert.That(rejtjelezes.Message(code, key), Is.EqualTo(expected));

            

        }

        [Test]
        public void KeyBlockTest()
        {
            Rejtjelezes rejtjelezes = new Rejtjelezes("early ", "ebtobehpzmjnmfqwuirlsoleakk", "cvtlsxo fiutxysspjzxkmmb");
            Assert.That(rejtjelezes.KeyBlock("curios", "Code2"), Is.EqualTo("abcdefghij"));
        }

        [Test]
        public void CommonKeyTest()
        {
            Rejtjelezes rejtjelezes = new Rejtjelezes("early ", "ebtobehpzmjnmfqwuirlsoleakk", "cvtlsxo fiutxysspjzxkmmb");
            Assert.That(rejtjelezes.CommonKey(), Is.EqualTo("abcdefghijklmnopqrstlkmjnuzh"));
        }
    }
}