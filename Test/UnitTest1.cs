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

        [TestCase("helloworld", "abcdefgijkl", "Rejtjelezett �zenet: hfnosauzun")]
        [TestCase("alma", "korte", "Rejtjelezett �zenet: kzct")]
        public void CodeTest(string message, string key, string expected)
        {
            Rejtjelezes rejtjelezes = new Rejtjelezes(message, key);
            Assert.That(rejtjelezes.Code(), Is.EqualTo(expected));

            //Assert.Throws<InvalidOperationException>(() => new Rejtjelezes(message, key)

        }

        //[TestCase("Alm�s", "abcdefghij")]
        //public void ExceptionTest(string InvalidMessage, string Key)
        //{
        //    var r = new Rejtjelezes(InvalidMessage, Key);
        //    Assert.Throws<InvalidOperationException>(() => new Rejtjelezes(InvalidMessage, Key));
        //    //Assert.That(throws.Message, Is.EqualTo("Az �zenet csak az angol abc kisbet�it �s sz�k�zt tartalmazhat!"));

        //}

        [TestCase("hfnosauzun", "abcdefgijkl", "helloworld")]
        [TestCase("kzct", "korte", "alma")]
        public void MessageTest(string code, string key, string expected)
        {
            Rejtjelezes rejtjelezes = new Rejtjelezes(code, key);
            Assert.That(rejtjelezes.Message(code, key), Is.EqualTo(expected));

            //Assert.Throws<InvalidOperationException>(() => new Rejtjelezes(message, key)

        }
    }
}