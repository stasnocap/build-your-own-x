using System.Net.Sockets;
using System.Text;

namespace Shared;

public static class TcpClientExtensions
{
    private const int LengthOffset = 2;

    public static async Task SendAsync(this TcpClient client, Packet packet, CancellationToken cancellationToken = default)
    {
        var stream = client.GetStream();

        var json = packet.ToJson();
        var jsonBytes = Encoding.UTF8.GetBytes(json);

        var jsonLength = json.Length;
        var jsonLengthBytes = BitConverter.GetBytes(Convert.ToInt16(jsonLength));

        var bytes = new byte[jsonLengthBytes.Length + jsonBytes.Length];
        jsonLengthBytes.CopyTo(bytes, 0);
        jsonBytes.CopyTo(bytes, LengthOffset);

        await stream.WriteAsync(bytes, cancellationToken);
    }

    public static async Task<Packet?> ReadAsync(this TcpClient client, CancellationToken cancellationToken = default)
    {
        var stream = client.GetStream();

        byte[] jsonLengthBytes = new byte[LengthOffset];
        await stream.ReadExactlyAsync(jsonLengthBytes, cancellationToken);
        var jsonLength = BitConverter.ToInt16(jsonLengthBytes);

        byte[] bytes = new byte[jsonLength];
        await stream.ReadExactlyAsync(bytes, cancellationToken);
        var json = Encoding.UTF8.GetString(bytes);

        return Packet.FromJson(json);
    }
}