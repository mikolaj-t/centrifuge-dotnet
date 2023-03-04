using Centrifuge;

var client = new Client
{
    Endpoint = "ws://localhost:8000/connection/websocket"
};
client.Connect();