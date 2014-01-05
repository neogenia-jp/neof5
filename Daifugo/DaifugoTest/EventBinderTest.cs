using Microsoft.VisualStudio.TestTools.UnitTesting;
using Daifugo.Bases;
using Moq;

namespace DaifugoTest
{
    internal class TestGameEvents : GameEvents
    {
        internal void callStart() { base.Start(p=>null); }
    }


	[TestClass]
    public class EventBinderTest
    {
        [TestMethod]
        public void TestUnbind()
        {
			int result = 0;
            var e = new TestGameEvents();

            var player = new Mock<IGamePlayer>();
            player.Setup(m => m.Start(null)).Callback(() => { result++; });
            var p = player.Object;

            p.BindEvents(e);

			e.callStart();

            for (int i=0;i<10 && result==0;i++) { System.Threading.Thread.Sleep(500); } 
            Assert.AreEqual(1, result);

			e.callStart();

            for (int i=0;i<10 && result==1;i++) { System.Threading.Thread.Sleep(500); } 
            Assert.AreEqual(2, result);

            p.UnbindEvents(e);

            e.callStart();
            
            for (int i=0;i<10 && result==2;i++) { System.Threading.Thread.Sleep(500); } 
            Assert.AreEqual(2, result);
        }
    }
}
