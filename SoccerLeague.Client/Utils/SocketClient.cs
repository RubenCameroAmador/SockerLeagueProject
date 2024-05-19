using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SoccerLeague.Client.Services;
using SoccerLeague.Core.Entities;
using System.Data;
using System.Text.Json.Serialization;

namespace SoccerLeague.Client.Utils
{
    public class SocketClient
    {
        private Lazy<IJSObjectReference>? _jsRef;
        private IJSRuntime? js;
        private readonly LogService log;
        private readonly IConfiguration configuration;

        private bool isConencted = false;

        public SocketClient(LogService log, IConfiguration configuration)
        {
            JSSocketIintegrationFunction = onSocketIntegrationFunctionFromJS;
            this.log = log;
            this.configuration = configuration;
        }

        public async Task<bool> Initilize(IJSRuntime? js)
        {
            if (this.js == null) this.js = js;

            if (_jsRef == null) _jsRef = new();

            if (!(_jsRef?.IsValueCreated ?? true) && js != null)
            {
                try
                {
                    _jsRef = new(await js.InvokeAsync<IJSObjectReference>("import", "./js/socketclient.js"));
                    string? url = configuration.GetValue<string>("Socket");

                    if(!string.IsNullOrEmpty(url))
                        await _jsRef.Value.InvokeVoidAsync("Connect", url );
                }
                catch (Exception ex)
                {
                    log.Exception(ex);
                    _jsRef = null;
                    return false;
                }
            }

            return true;
        }

        public async Task SendMessage(SocketClientMessage message)
        {
            if (await Initilize(js) && isConencted)
            {
                await _jsRef!.Value.InvokeVoidAsync("Send", message);
            }
        }

        public event EventHandler<Team?>? TeamNotification;
        public event EventHandler<TeamsMatch?>? TeamsMatchNotification;
        public event EventHandler? OpenConnection;
        public event EventHandler? CloseConnection;
        public event EventHandler<Exception>? ErrorConnection;

        private async Task onSocketIntegrationFunctionFromJS(SocketClientMessage data)
        {
            switch (data.Type)
            {
                case SocketClientMessage.MessageType.OnOpen:
                    isConencted = true;
                    OpenConnection?.Invoke(this, new());                    
                    log.Info("Socket connected");
                    break;
                case SocketClientMessage.MessageType.OnClose:
                    isConencted = false;
                    CloseConnection?.Invoke(this, new());
                    log.Info("Socket closed");
                    break;
                case SocketClientMessage.MessageType.OnError:
                    isConencted = false;
                    ErrorConnection?.Invoke(this, new Exception(data.Message));
                    log.Exception(new Exception(data.Message));
                    break;
                case SocketClientMessage.MessageType.OnMessage:

                    if (data?.Payload == null || data.PayloadType == SocketClientMessage.MainType.Undefined) return;

                    switch (data.PayloadType)
                    {
                        case SocketClientMessage.MainType.Team:
                            Team? team = JsonConvert.DeserializeObject<Team>(JsonConvert.SerializeObject(data.Payload));
                            TeamNotification?.Invoke(this, team);
                            log.Info("Team received");
                            break;
                        case SocketClientMessage.MainType.TeamMatch:
                            TeamsMatch? teamsMatch = JsonConvert.DeserializeObject<TeamsMatch>(JsonConvert.SerializeObject(data.Payload));
                            TeamsMatchNotification?.Invoke(this, teamsMatch);
                            log.Info("Teams Match received");
                            break;
                    }

                    break;
            }

            //GenericEvent?.Invoke(JsonConvert.DeserializeObject<GenericEventResult>(json));

            await Task.FromResult(0);
        }



        #region JSRuntime integration

        public static Func<SocketClientMessage, Task>? JSSocketIintegrationFunction;

        [JSInvokable]
        public static async Task JSSocketNotification(string data)
        {
            if (JSSocketIintegrationFunction == null) return;

            if (string.IsNullOrEmpty(data.Trim())) return;

            SocketClientMessage? message = JsonConvert.DeserializeObject<SocketClientMessage>(data);

            if (message == null) return;

            await JSSocketIintegrationFunction.Invoke(message);

        }

        public class SocketClientMessage
        {
            public enum MessageType
            {
                Undefined,
                OnOpen,
                OnClose,
                OnError,
                OnMessage
            }

            public enum MainType
            {
                Undefined,
                Team,
                TeamMatch
            }

            [JsonProperty("type")]
            public MessageType Type { get; set; }

            [JsonProperty("message")]
            public string? Message { get; set; }

            [JsonProperty("payloadtype")]
            public MainType PayloadType { get; set; }

            [JsonProperty("payload")]
            public object? Payload { get; set; }

        }

        #endregion

    }
}
