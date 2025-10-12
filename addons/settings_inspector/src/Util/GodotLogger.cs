using System;
using Godot;
using Microsoft.Extensions.Logging;

namespace SettingInspector.Util;

public class GodotLogger : ILogger
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        switch (logLevel)
        {
            case LogLevel.Critical:
                GD.PrintErr(formatter(state, exception));
                break;
            case LogLevel.Error:
                GD.PrintErr(formatter(state, exception));
                break;
            case LogLevel.Warning:
                GD.Print(formatter(state, exception));
                break;
            case LogLevel.Information:
                GD.Print(formatter(state, exception));
                break;
            case LogLevel.Debug:
                GD.Print(formatter(state, exception));
                break;
            case LogLevel.Trace:
                GD.Print(formatter(state, exception));
                break;
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
#if GODOT_DEBUG
        return true;
#endif
        return logLevel >= LogLevel.Warning;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}