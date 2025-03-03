using System.Net.Sockets;
using Shared;

namespace Client;

public class Client(string host, int port)
{
    private readonly TcpClient _client = new();
    private readonly string _host = host;
    private readonly int _port = port;

    public async Task SayHelloAsync(CancellationToken cancellationToken = default)
    {
        await ConnectAsync(cancellationToken);

        _ = Task.Run(async () =>
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var sendMessage = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(sendMessage))
                {
                    var packet = Packet.Input(sendMessage);

                    await _client.SendAsync(packet, cancellationToken);
                }
            }
        }, cancellationToken);

        while (true)
        {
            if (_client.Available > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var receivedMessage = await _client.ReadAsync(cancellationToken);

                Console.WriteLine(receivedMessage);
            }

            Thread.Sleep(10);
        }
    }

    private async Task ConnectAsync(CancellationToken cancellationToken)
    {
        await _client.ConnectAsync(_host, _port, cancellationToken);

        Console.WriteLine("Connected.");

        var packet = Packet.Message("Hello from client");

        await _client.SendAsync(packet, cancellationToken);
    }
}