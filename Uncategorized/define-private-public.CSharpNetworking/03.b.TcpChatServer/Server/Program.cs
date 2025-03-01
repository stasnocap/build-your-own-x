using System.Net;

var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (obj, args) =>
{
    cancellationTokenSource.Cancel();
};

using var server = new Server.Server(IPAddress.Any, 8080);

await server.RunAsync(cancellationTokenSource.Token);