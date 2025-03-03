using System.Text.Json;

namespace Shared;

public record Packet(Command Command, string Body)
{
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static Packet Message(string body)
    {
        return new(Command.Message, body);
    }

    public static Packet Input(string body)
    {
        return new(Command.Input, body);
    }

    public static Packet? FromJson(string json)
    {
        var packet = JsonSerializer.Deserialize<Packet>(json);

        if (packet is not null && !Enum.IsDefined(packet.Command))
        {
            return null;
        }

        return packet;
    }
}