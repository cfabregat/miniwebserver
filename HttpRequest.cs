using System;
using System.Collections.Generic;
using System.IO;

class HttpRequest
{
    public string Method { get; set; }
    public string Path { get; set; }
    public Dictionary<string, string> Headers { get; } = new();
    public string Body { get; set; }

    public static HttpRequest Parse(StreamReader reader)
    {
        var request = new HttpRequest();
        string requestLine = reader.ReadLine();
        if (string.IsNullOrEmpty(requestLine)) return null;

        var parts = requestLine.Split(' ');
        request.Method = parts[0];
        request.Path = parts[1];

        // Leer cabeceras
        string line;
        while (!string.IsNullOrEmpty(line = reader.ReadLine()))
        {
            var header = line.Split(':', 2);
            if (header.Length == 2)
                request.Headers[header[0].Trim()] = header[1].Trim();
        }

        // Leer body si es POST
        if (request.Method == "POST" && request.Headers.TryGetValue("Content-Length", out var contentLength))
        {
            int length = int.Parse(contentLength);
            char[] buffer = new char[length];
            reader.Read(buffer, 0, length);
            request.Body = new string(buffer);
        }

        return request;
    }
}
