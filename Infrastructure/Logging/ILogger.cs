﻿namespace Infrastructure.Logging;

public interface ILogger
{
    void Info(string message);
    void Warning(string message);
    void Error(string message, Exception ex = null);
}