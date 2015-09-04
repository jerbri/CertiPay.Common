namespace CertiPay.Common.Testing
{
    using ApprovalTests;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Ploeh.AutoFixture;

    public static class TestExtensions
    {
        private static readonly Fixture _fixture = new Fixture { };

        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        static TestExtensions()
        {
            _settings.Converters.Add(new StringEnumConverter { });
        }

        /// <summary>
        /// Runs ApprovalTests's VerifyJson against a JSON.net serialized representation of the provided object
        /// </summary>
        public static void VerifyMe(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj, _settings);

            Approvals.VerifyJson(json);
        }

        /// <summary>
        /// Returns an auto-initialized instance of the type T, filled via mock
        /// data via AutoFixture.
        ///
        /// This will not work for interfaces, only concrete types.
        /// </summary>
        public static T AutoGenerate<T>()
        {
            return _fixture.Create<T>();
        }
    }
}