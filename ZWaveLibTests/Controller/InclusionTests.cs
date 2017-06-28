using NUnit.Framework;
using ZWaveLib;

namespace ZWaveLibTests.Controller
{
    [TestFixture]
    [Ignore("stub for future tests")]
    public class InclusionTests
    {
        [Test]
        public void NodeAddStart()
        {
            var controller = new ZWaveController();

            controller.BeginNodeAdd();
        }
    }
}
