using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class HttpServer
{
    private int port;
    private string root;

    public HttpServer()
    {
        LoadConfig();
    }

    public void Start()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Servidor iniciado en el puerto {port}");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread thread = new Thread(() => HandleClient(client));
            thread.Start();
        }
    }

    private void LoadConfig()
    {
        var lines = File.ReadAllLines("config.txt");
        foreach (var line in lines)
        {
            var parts = line.Split('=');
            if (parts.Length != 2) continue;

            if (parts[0] == "PORT") port = int.Parse(parts[1]);
            if (parts[0] == "ROOT") root = parts[1];
        }
    }

    private void HandleClient(TcpClient client)
    {
        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new StreamReader(stream);
        using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

        var request = HttpRequest.Parse(reader);
        Logger.LogRequest(request, ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

        string path = request.Path == "/" ? "/index.html" : request.Path;
        string filePath = Path.Combine(root, path.TrimStart('/'));

        if (!File.Exists(filePath))
        {
            filePath = Path.Combine(root, "404.html");
            writer.Write(HttpResponse.NotFound(File.ReadAllText(filePath)));
            return;
        }

        writer.Write(HttpResponse.Ok(File.ReadAllText(filePath)));
    }
}
