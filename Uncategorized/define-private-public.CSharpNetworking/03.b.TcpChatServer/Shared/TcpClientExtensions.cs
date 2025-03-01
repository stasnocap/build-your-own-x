using System.Net.Sockets;
using System.Text;

namespace Shared;

public static class TcpClientExtensions
{
    public static async Task SendAsync(this TcpClient client, string message, CancellationToken cancellationToken = default)
    {
        var stream = client.GetStream();

        var bytes = Encoding.UTF8.GetBytes(message);

        await stream.WriteAsync(bytes, cancellationToken);
    }

    public static async Task<string> ReadAsync(this TcpClient client, CancellationToken cancellationToken = default)
    {
        var stream = client.GetStream();

        byte[] bytes = new byte[client.Available];

        await stream.ReadAsync(bytes, cancellationToken);

        return Encoding.UTF8.GetString(bytes);
    }
}