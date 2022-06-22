namespace Practise.Extension
{
    public static class NLogExtension
    {
        public static void LogExt(this NLog.Logger logger, NLog.LogLevel level, string message, string userName)
        {
            NLog.LogEventInfo theEvent = new NLog.LogEventInfo(level, logger.Name, message);
            theEvent.Properties["UserName"] = userName;
            logger.Log(theEvent);
        }
    }
}
