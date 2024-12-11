// See https://aka.ms/new-console-template for more information
using Lab5.Network.Common;

public class MessageApiClient : IMessageApi
{
    private readonly NetUdpClient netUdpClient;

    public MessageApiClient(NetUdpClient netUdpClient)
    {
        this.netUdpClient = netUdpClient;
    }

    public async Task<bool> SendMessage(string message)
    {
        var command = new Command()
        {
            Code = (byte)CommandCode.SendMessage,
            Arguments = new Dictionary<string, object?>()
            {
                ["Data"] = message
            }
        };

        await netUdpClient.SendAsync(command);
        return true;
    }
}
