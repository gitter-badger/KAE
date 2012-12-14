namespace KJFramework.Tracing
{
    internal class FormatTracing : NullTracing
    {
        #region Constructor

        public FormatTracing(string logger)
        {
            _logger = logger;
        }

        #endregion

        #region Members

        private string _logger;

        #endregion

        #region Methods

        protected override void Trace(TracingLevel level, System.Exception error, string format, params object[] args)
        {
            try
            {
                if (level >= TracingSettings.Level)
                {
                    string message = string.Empty;
                    try
                    {
                        message = args.Length == 0 ? format : string.Format(format ?? string.Empty, args);
                    }
                    catch (System.Exception ex)
                    {
                        if (level < TracingLevel.Warn)
                            level = TracingLevel.Warn;
                        if (error == null)
                            error = ex;
                        message = string.Concat("tracing formatting error: [", args.Length, "] ", format ?? string.Empty);
                    }

                    TracingManager.AddTraceItem(new TraceItem(_logger, level, error, message));
                }
            }
            catch
            {
                // mute everything...
            }
        }

        #endregion
    }
}