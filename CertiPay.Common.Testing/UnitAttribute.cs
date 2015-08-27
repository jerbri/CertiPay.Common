namespace CertiPay.Common.Testing
{
    using NUnit.Framework;

    /// <summary>
    /// An attribute for a test that is a unit test.  Should only be applied if the test only tests a singular class.
    /// </summary>
    public class UnitAttribute : CategoryAttribute
    {
        public UnitAttribute()
            : base("Unit")
        {
        }
    }
}