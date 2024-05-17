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

        public async Task SendMessage(JSSocketIntegrationMessage message)
        {
            if (await Initilize(js))
            {
                await _jsRef!.Value.InvokeVoidAsync("Send", message);
            }
        }

        public event EventHandler<Team?>? TeamNotification;
        public event EventHandler<TeamsMatch?>? TeamsMatchNotification;
        public event EventHandler? OpenConnection;
        public event EventHandler? CloseConnection;
        public event EventHandler<Exception> ErrorConnection;

        private async Task onSocketIntegrationFunctionFromJS(JSSocketIntegrationMessage data)
        {
            switch (data.Type)
            {
                case JSSocketIntegrationMessage.MessageType.OnOpen:
                    OpenConnection?.Invoke(this, new());
                    log.Info("Socket connected");
                    break;
                case JSSocketIntegrationMessage.MessageType.OnClose:
                    CloseConnection?.Invoke(this, new());
                    log.Info("Socket closed");
                    break;
                case JSSocketIntegrationMessage.MessageType.OnError:
                    ErrorConnection?.Invoke(this, new Exception(data.Message));
                    log.Exception(new Exception(data.Message));
                    break;
                case JSSocketIntegrationMessage.MessageType.OnMessage:

                    if (data?.Payload == null || data.PayloadType == JSSocketIntegrationMessage.MainType.Undefined) return;

                    switch (data.PayloadType)
                    {
                        case JSSocketIntegrationMessage.MainType.Team:
                            Team? team = JsonConvert.DeserializeObject<Team>(JsonConvert.SerializeObject(data.Payload));
                            TeamNotification?.Invoke(this, team);
                            log.Info("Team received");
                            break;
                        case JSSocketIntegrationMessage.MainType.TeamMatch:
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

        public static Func<JSSocketIntegrationMessage, Task>? JSSocketIintegrationFunction;

        [JSInvokable]
        public static async Task JSSocketNotification(string data)
        {
            if (JSSocketIintegrationFunction == null) return;

            if (string.IsNullOrEmpty(data.Trim())) return;

            JSSocketIntegrationMessage? message = JsonConvert.DeserializeObject<JSSocketIntegrationMessage>(data);

            if (message == null) return;

            await JSSocketIintegrationFunction.Invoke(message);

        }

        public class JSSocketIntegrationMessage
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
