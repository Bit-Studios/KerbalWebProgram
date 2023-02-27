
function testWS() {
  let ws = new WebSocket("ws://test");
  var status;
  var err;
  var response;

  ws.onopen = function(e) {
    console.log("[open] Connection established");
    console.log("Sending to server");
    socket.send("Establish");
    status = 200;
  };

ws.onmessage = function(event) {
  console.log(`[Websocket Worker] Data received from server: ${event.data}`);
  response = event;
};

ws.onclose = function(event) {
  if (event.wasClean) {
    console.log(`[Websocket Worker] Connection closed cleanly, code=${event.code} reason=${event.reason}`);
  } else {
    // e.g. server process killed or network down
    // event.code is usually 1006 in this case
    console.log('[Websocket Worker] Connection died');
  }
};

ws.onerror = function(error) {
  console.log(`[Websocket Worker] ERROR: ${error}`);
  status = 500;
  err = error;
};

return status, response, err;
}