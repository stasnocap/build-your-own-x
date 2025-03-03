using System.Net.Sockets;
using Shared;

namespace Server;

public class GuessMyNumber(TcpClient _player)
{
    private const int MinValue = 0;
    private const int MaxValue = 100;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var packet = Packet.Message($"Hello, guess my number from {MinValue} to {MaxValue}");

        await _player.SendAsync(packet, cancellationToken);

        var myNumber = Random.Shared.Next(MinValue, MaxValue);

        var running = true;

        while (running)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_player.Available > 0)
            {
                packet = await _player.ReadAsync(cancellationToken);

                if (packet is null)
                {
                    continue;
                }

                switch (packet.Command)
                {
                    case Command.Message:
                        Console.WriteLine(packet.Body);
                        break;
                    case Command.Input:
                        if (!int.TryParse(packet.Body, out var number))
                        {
                            continue;
                        }

                        if (myNumber == number)
                        {
                            packet = Packet.Message($"Congratulations. It is indeed {number}.");
                            await _player.SendAsync(packet, cancellationToken);
                            running = false;
                            break;
                        }

                        if (myNumber < number)
                        {
                            packet = Packet.Message("Too much.");
                            await _player.SendAsync(packet, cancellationToken);
                        }
                        else
                        {
                            packet = Packet.Message("Too little.");
                            await _player.SendAsync(packet, cancellationToken);
                        }

                        break;
                }
            }
        }
    }
}