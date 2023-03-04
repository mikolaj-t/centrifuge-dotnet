using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;

namespace Centrifuge;

public record struct ClientInfo(string User, string Client);

public record struct ClientOptions(string Token,
    string Name,
    string Version,
    byte[] Data,
    Dictionary<string, string> Headers,
    int Timeout = 100,
    int MinReconnectDelay = 500,
    int MaxReconnectDelay = 20000,
    int MaxServerPingDelay = 10000,
    WebProxy? Proxy = null,
    string ProxyLogin = "",
    string ProxyPassword = "");

public enum ClientState
{
    Disconnected,
    Connecting,
    Connected,
    Closed
}

public interface IClient
{
    public void Connect();
    public void Disconnect();
    public void Close();
}

public class Client : IClient
{
    private ClientWebSocket WebSocket { get; } = new();
    public string Endpoint { get; set; } = "";
    public ClientOptions Options { get; private set; }

    private ClientState State { get; set; } = ClientState.Disconnected;

    public void Connect()   
    {
        if (WebSocket.State is WebSocketState.Open)
        {
            WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None).Wait(Options.Timeout);
        }

        State = ClientState.Connecting;
        
        WebSocket.ConnectAsync(new Uri(Endpoint), CancellationToken.None).Wait(); 
        State = ClientState.Connected;
    }

    public void Disconnect()
    {
        State = ClientState.Disconnected;
    }

    public void Close()
    {
        State = ClientState.Closed;
    }
}