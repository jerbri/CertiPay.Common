//namespace CertiPay.Common.Logging
//{
//    using Microsoft.AspNet.Identity;
//    using System;

//    /// <summary>
//    /// Provides extensions for logging audit level events that need to be stored for later review
//    /// </summary>
//    public static class AuditLoggingExtensions
//    {
//        public const String Audit_Log_Format = "User {@User} performed {Action} : {Reason}";

//        public const String Audit_Log_With_Context_Format = "User {@User} performed {Action} : {Reason} {@Context}";

//        // TODO They'll pass in a logger in the extension method, but we'll use a more refined logging mechanism?

//        private static readonly ILog audit_log = new SerilogLogger(LogManager.logger.ForContext("IsAudit", true));

//        // TODO Need to include the following information:

//        // WHO Userid/username
//        // WHAT Action
//        // WHEN Timestamp (already included)
//        // WHERE Application (already included, but might be narrowed down?)
//        // WHY Might come from the extension method?

//        public static void Audit<TUserKey>(this ILog logger, IUser<TUserKey> current_user, String action, String reason = "")
//        {
//            audit_log.Info(Audit_Log_Format, new { UserId = current_user.Id, Username = current_user.UserName }, action, reason);
//        }

//        public static void Audit<TContext, TUserKey>(this ILog logger, IUser<TUserKey> current_user, TContext context, String action, String reason = "")
//        {
//            audit_log.Info(Audit_Log_With_Context_Format, new { UserId = current_user.Id, Username = current_user.UserName }, action, reason, context);
//        }
//    }
//}