using Microsoft.Extensions.Logging;

namespace Tests.Common;

public class AbstractLoggableTest
{
    protected ILoggerFactory _loggerFactory;

    public virtual void SetUp()
    {
        _loggerFactory = LoggerFactory.Create(c => c
            .AddSimpleConsole((configure) => { configure.SingleLine = false; })
            .SetMinimumLevel(LogLevel.Trace)
        );
    }
}