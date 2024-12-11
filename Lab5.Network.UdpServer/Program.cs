internal class Program
{
    private static void Main(string[] args)
    {
        var listenAdress = new Uri("tcp://0.0.0.0:7777");

        var udpServer = new MessageUdpServer(listenAdress);
        Console.WriteLine($"Start udp server and listen at {listenAdress}");
        udpServer.Start();//
        Console.ReadKey();
    }
}