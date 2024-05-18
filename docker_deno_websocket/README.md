# Deno Websocket using Docker
A simple project where we will create a websocket server and we can validate it using the index.html file.

The service we will provide with Deno sends messages to all clients without including the same client who sent the message (a simple broadcast).

## Serve it
Just run in terminal

> docker run -d --init -p <HOST_PORT>:<PORT> --mount type=bind,source=".\main.js",target=/app/main.js --name denosockerapp -e PORT=<PORT> denoland/deno deno run --allow-net --allow-env /app/main.js

## Enjoy!!

Based on:
[MDN Web Docs](https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API/Writing_a_WebSocket_server_in_JavaScript_Deno) 

