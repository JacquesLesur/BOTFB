using Microsoft.VisualStudio.TestTools.UnitTesting;
using BotFBEpsi;
using BotFBEpsi.Dialogs;
using Microsoft.Bot.Connector;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Activity act = new Activity();
            act.Text = "test";
            act.From.Id = "IdTest";
             BotDBModel botDb = new BotDBModel();
            bool resultat = botDb.enregistrementMessage(act);
            Assert.AreEqual(resultat, true);

        }
    
    }
}
