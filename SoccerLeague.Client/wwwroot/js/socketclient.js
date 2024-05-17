
let socket;

export function Disconnect() {
    if (!socket || socket.readyState !== WebSocket.OPEN) {
        throw new Error("socket not connected");
    }
    socket.close(1000, "Closing from client");
}

export function Send(data) {
    if (!socket || socket.readyState !== WebSocket.OPEN) {
        throw new Error("socket not connected");
    }
    socket.send(JSON.stringify(data));
}

const MessageType = {
    Undefined : 0,
    OnOpen : 1,
    OnClose : 2,
    OnError : 3,
    OnMessage : 4
}

export function Connect(url) {
    socket = new WebSocket(url);
    socket.onopen = function (event) {
        DotNet.invokeMethodAsync('SoccerLeague.Client', 'JSSocketNotification', JSON.stringify({ type: MessageType.OnOpen, message: "Connected" }));
    };
    socket.onclose = function (event) {
        DotNet.invokeMethodAsync('SoccerLeague.Client', 'JSSocketNotification', JSON.stringify({ type: MessageType.OnClose, message: "Disconnected" }));
    };
    socket.onerror = function (event) {
        DotNet.invokeMethodAsync('SoccerLeague.Client', 'JSSocketNotification', JSON.stringify({ type: MessageType.OnError, message: event }));
    };
    socket.onmessage = function (event) {
        DotNet.invokeMethodAsync('SoccerLeague.Client', 'JSSocketNotification', event.data);
    };
}
