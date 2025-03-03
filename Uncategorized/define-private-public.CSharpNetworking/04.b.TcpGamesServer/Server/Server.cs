using System.Net;
using System.Net.Sockets;
using Shared;

namespace Server;

public class Server(IPAddress iPAddress, int port) : IDisposable
{
    private readonly TcpListener _listener = new(iPAddress, port);
    private readonly List<TcpClient> _clients = [];

    private bool _disposed = false;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _listener.Start();

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_listener.Pending())
            {
                var client = await _listener.AcceptTcpClientAsync(cancellationToken);

                _clients.Add(client);

                _ = Task.Run(async () =>
                {
                    var game = new GuessMyNumber(client);

                    await game.StartAsync(cancellationToken);

                    _clients.Remove(client);
    
                    client.Dispose();
                }, cancellationToken);
            }

            Thread.Sleep(10);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            foreach (var client in _clients)
            {
                client.Dispose();
            }

            _listener.Dispose();

            _disposed = true;
        }
    }

    ~Server()
    {
        Dispose(false);
    }
}