namespace CertiPay.Common.Testing
{
    using ApprovalTests;
    using Newtonsoft.Json;
    using Ploeh.AutoFixture;

    public static class TestExtensions
    {
        private static readonly Fixture _fixture = new Fixture { };

        /// <summary>
        /// Runs ApprovalTests's VerifyJson against a JSON.net serialized representation of the provided object
        /// </summary>
        public static void VerifyMe(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            Approvals.VerifyJson(json);
        }

        public static T AutoGenerate<T>()
        {
            return _fixture.Create<T>();
        }
    }
}