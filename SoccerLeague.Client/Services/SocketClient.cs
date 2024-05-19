using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SoccerLeague.Client.Models;
using SoccerLeague.Client.Services;
using SoccerLeague.Core.Entities;
using System.Data;
using System.Text.Json.Serialization;
using Websocket.Client;

namespace SoccerLeague.Client.Services
{
    public class SocketClientService
    {
        private readonly LogService log;
        private WebsocketClient? client;
        private readonly string? configSocketUrl;
        private readonly bool configIsSocketEnabled;

        public delegate void TeamNotificationHandler(Team? team);
        public event TeamNotificationHandler? TeamNotification = null!;
        public delegate void TeamsMatchNotificationHandler(TeamsMatch? teamsMatch);
        public event TeamsMatchNotificationHandler? TeamsMatchNotification;
        public event EventHandler? OpenConnection;
        public event EventHandler? CloseConnection;

        public SocketClientService(LogService log, IConfiguration configuration)
        {
            this.log = log;
            configSocketUrl = configuration.GetValue<string>("Socket");
            configIsSocketEnabled = configuration.GetValue<bool>("SocketEnabled");
        }

        public async Task<bool> Initilize()
        {
            if (!configIsSocketEnabled) return false;
            if (client?.IsRunning ?? false) return true;
            if (string.IsNullOrEmpty(configSocketUrl)) return false;

            var uri = new Uri(configSocketUrl);

            client = new WebsocketClient(uri);

            TimeSpan timeforReconnect = TimeSpan.FromSeconds(30);
            client.ReconnectTimeout = timeforReconnect;

            client.DisconnectionHappened.Subscribe(async info =>
            {

                switch (info.Type)
                {
                    case DisconnectionType.Error:
                        log.Info($"Disconnection happened, type >> ERROR");
                        await Task.Delay(timeforReconnect);
                        await client.Reconnect();
                        break;

                    case DisconnectionType.Lost:
                        log.Info($"Disconnection happened, type >> LOST");
                        await Task.Delay(timeforReconnect);
                        await Initilize();
                        break;

                    case DisconnectionType.NoMessageReceived:
                        //Nothing happened
                        break;

                    default:
                        log.Info($"Disconnection happened, type: {info.Type}");
                        CloseConnection?.Invoke(this, new());
                        break;

                }
            });

            client.ReconnectionHappened.Subscribe(info =>
            {

                switch (info.Type)
                {
                    case ReconnectionType.Error:
                        log.Info($"Reconnection happened, type >> ERROR");
                        break;

                    case ReconnectionType.Initial:
                    case ReconnectionType.ByUser:
                        log.Info($"Connection happened");
                        OpenConnection?.Invoke(this, new());
                        break;

                    case ReconnectionType.NoMessageReceived:
                        //Nothing happened
                        break;

                    default:
                        log.Info($"Reconnection happened, type: {info.Type}");
                        break;

                }
            });

            client.MessageReceived.Subscribe(msg =>
            {
                log.Info($"Message received: {msg}");
                string? data = msg?.Text?.Trim();
                if (string.IsNullOrEmpty(data)) return;

                try
                {
                    SocketClientMessage? message = JsonConvert.DeserializeObject<SocketClientMessage>(data);
                    Task.Run(() => OnSocketClientMessageReceived(message));
                }
                catch (System.Exception ex)
                {
                    log.Exception(ex);
                }
            });

            await client.Start();

            return true;
        }

        public async Task SendMessage(SocketClientMessage message)
        {
            if (await Initilize())
            {
                client!.Send(JsonConvert.SerializeObject(message));
            }
        }

        private void OnSocketClientMessageReceived(SocketClientMessage? data)
        {
            if (data == null) return;

            switch (data.Type)
            {
                case SocketClientMessage.MessageType.OnMessage:

                    if (data.Payload == null || data.PayloadType == SocketClientMessage.MainType.Undefined) return;

                    switch (data.PayloadType)
                    {
                        case SocketClientMessage.MainType.Team:
                            Team? team = JsonConvert.DeserializeObject<Team>(JsonConvert.SerializeObject(data.Payload));
                            TeamNotification?.Invoke(team);
                            log.Info("Team received");
                            break;
                        case SocketClientMessage.MainType.TeamMatch:
                            TeamsMatch? teamsMatch = JsonConvert.DeserializeObject<TeamsMatch>(JsonConvert.SerializeObject(data.Payload));
                            TeamsMatchNotification?.Invoke(teamsMatch);
                            log.Info("Teams Match received");
                            break;
                    }

                    break;
            }
            
        }

    }
}
