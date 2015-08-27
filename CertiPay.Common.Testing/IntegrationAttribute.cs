namespace CertiPay.Common.Testing
{
    using NUnit.Framework;

    /// <summary>
    /// An attribute for a test that is an integration test.  Should be applied if the test covers more than one class
    /// or has a dependency on an external resource (DB, File System, API)
    /// </summary>
    public class IntegrationAttribute : CategoryAttribute
    {
        public IntegrationAttribute()
            : base("Integration")
        {
        }

        /// <summary>
        /// A string to indicate the dependencies that make the test an integration test.
        /// </summary>
        public string DependsOn { get; set; }
    }
}