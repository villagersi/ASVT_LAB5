// See https://aka.ms/new-console-template for more information
using Lab5.Network.Common;

internal class MessageUdpServer : UdpServerBase, IMessageApi
{
    public MessageUdpServer(Uri listenAddress) : base(listenAddress)
    {
    }

    public Task<bool> SendMessage(string message)
    {
        Console.WriteLine($"MESSAGE: {message}");

        return Task.FromResult(true);
    }

    protected override async Task ProcessCommandAsync(Command command)
    {
        var commandCode = (CommandCode)command!.Code;
        Console.WriteLine($"+ command: {commandCode}");

        await SendMessage(command.Arguments["Data"]?.ToString() ?? string.Empty);
    }
}