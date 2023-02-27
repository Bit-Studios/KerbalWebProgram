const WebSocket = require('ws');
const express = require('express');
const app = express();
const ip = require('ip');
var port = 8000; //default port
const axios = require('axios');

//GET IP ADDR
var addr = ip.address();

//host public folder
app.use('/', express.static('public/'));
/*



*/
app.listen(port);

console.log('Server started at IP:'+addr+' PORT:'+ port);

app.get('/connect', async function(req, res) {
  // WS config
  const ws = new WebSocket('ws://localhost:8080');

  // Handle WS errors
  ws.on('error', async function(error) {
    console.log(`Failed to connect WS server (Falling back to REST API)`);
    
    try {
      // Fallback to REST API (ceselistialbodydata as example)
      await axios.get('http://localhost:8080/api/ceselistialbodydata');
      res.status(200).send('Connected to Kerbal Web Program');
    } catch (e) {
      res.status(500).send('Failed to connect to Kerbal Web Program. Is the mod installed correctly?');
    } 
  });

  ws.on('open', function() {
    console.log('Connected to WebSocket server');
    ws.send(Buffer.from('Frontend connected to WebSocket server', 'utf-8'));
    res.status(200).send('Connected to Kerbal Web Program');
  });

  ws.on('message', function(data) {
    if (typeof data === 'string') {
      console.log('Received message:', data);
    } else {
      // Assume binary data and convert to string
      const message = data.toString('utf-8');
      console.log('Received message:', message);
    }
  });

  ws.on('close', function() {
    console.log('Disconnected from WebSocket server');
  });
});
