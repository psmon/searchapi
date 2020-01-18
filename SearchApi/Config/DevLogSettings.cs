using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;

namespace SearchApi.Config
{
    // Dev콘솔용으로만 사용, 공식적인 로깅은 Nlog사용권장
    public class DevLogSettings
    {
        public static readonly LoggerFactory ConsoleLogger = new LoggerFactory(new[] {
            new ConsoleLoggerProvider((category, level) => category == DbLoggerCategory.Database.Command.Name
               && level == LogLevel.Debug, true)
        });

        public static readonly LoggerFactory DebugLogger = new LoggerFactory(new[] {
            new DebugLoggerProvider((_, logLevel) => logLevel >= LogLevel.Debug)
        });
    }
}
