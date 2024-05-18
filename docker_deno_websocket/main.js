
let clients = [];

function uuidv4() {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'
    .replace(/[xy]/g, function (c) {
      const r = Math.random() * 16 | 0,
        v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
}

const PORT = Deno.env.get("PORT");

Deno.serve({
  port: PORT,
  handler: async (request) => {
    // If the request is a websocket upgrade,
    // we need to use the Deno.upgradeWebSocket helper
    if (request.headers.get("upgrade") === "websocket") {
      const { socket, response } = Deno.upgradeWebSocket(request);

      socket.onopen = (client) => {
        socket.id = uuidv4();
        console.log(`CONNECTED: ${socket.id}`);
        clients.push(socket);
        console.log(`Total clients: ${clients.length}`);
      };

      socket.onmessage = (event) => {
        console.log(`RECEIVED: ${event.data}`);

        clients.filter(x => x !== socket).forEach(function (client) {
          client.send(`${event.data}`);
        });
      };

      socket.onclose = () => {
        console.log(`DISCONNECTED:${socket.id}`);

        clients = clients.filter(x => x !== socket);
      };

      socket.onerror = (error) => console.error("ERROR:", error);

      return response;

    } else {
      // If the request is a normal HTTP request,
      // we serve the client HTML file.
      return new Response("Welcome to Socket Server");
    }
  },
});
