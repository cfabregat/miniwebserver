using System;
using System.IO;

static class Logger
{
    public static void LogRequest(HttpRequest request, string ip)
    {
        string fileName = $"logs/{DateTime.Now:yyyy-MM-dd}.log";
        Directory.CreateDirectory("logs");

        string log = $"{DateTime.Now:HH:mm:ss} | IP: {ip} | {request.Method} {request.Path}";
        if (request.Path.Contains('?'))
        {
            log += $" | Params: {request.Path.Split('?')[1]}";
        }
        if (!string.IsNullOrEmpty(request.Body))
        {
            log += $" | Body: {request.Body}";
        }

        File.AppendAllText(fileName, log + Environment.NewLine);
    }
}
