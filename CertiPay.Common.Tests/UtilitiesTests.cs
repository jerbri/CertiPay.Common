using NUnit.Framework;

namespace CertiPay.Common.Tests
{
    public class UtilitiesTests
    {
        [Test]
        public void ShouldMatchVersionOfCaller()
        {
            Assert.AreEqual("0.9.9.local", Utilities.Version<UtilitiesTests>());
        }
    }
}