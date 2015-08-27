namespace CertiPay.Common.Testing
{
    using NUnit.Framework;
    using System;
    using System.Transactions;

    /// <summary>
    /// Allows a test to roll the database back to at the end to return the system to the previous state
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class AutoRollbackAttribute : Attribute, ITestAction, IDisposable
    {
        public ActionTargets Targets { get { return ActionTargets.Test; } }

        private TransactionScope _scope;

        public AutoRollbackAttribute()
        {
            IsolationLevel = IsolationLevel.Unspecified;
            ScopeOption = TransactionScopeOption.Required;
        }

        public IsolationLevel IsolationLevel { get; set; }

        public TransactionScopeOption ScopeOption { get; set; }

        public int TimeOutInSeconds { get; set; }

        public void BeforeTest(TestDetails testDetails)
        {
            var options = new TransactionOptions { IsolationLevel = this.IsolationLevel };

            if (TimeOutInSeconds > 0)
            {
                options.Timeout = TimeSpan.FromSeconds(this.TimeOutInSeconds);
            }

            this._scope = new TransactionScope(this.ScopeOption, options);
        }

        public void AfterTest(TestDetails testDetails)
        {
            this._scope.Dispose();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_scope")]
        public void Dispose()
        {
            // Nothing to do here. TransactionScope is disposed after test completion
        }
    }
}