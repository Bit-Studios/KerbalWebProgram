$(document).ready(function() {
    var apiCalls = ['getAllCelestialBodyData', 'getShiptelemetry', 'getShipThrottle', 'getStage', 'getCraftFile'];
    var output = document.getElementById('output');

      apiCalls.forEach(function(apiCall) {
          var xhr = new XMLHttpRequest();
          var url = 'http://localhost:8080/api';
          xhr.open('POST', url);
          xhr.setRequestHeader('Content-Type', 'application/json');

          var data = JSON.stringify({
            "ID": "203798423446346",
            "Action": apiCall,
            "parameters": {

            }
          });

          xhr.onreadystatechange = function() {
            if (this.readyState === 4) {
              var response = this.responseText;
              console.log(response);
              var box = document.createElement('div');
              box.classList.add('box');
              box.innerHTML = '<h3>' + apiCall + '</h3><p>' + JSON.stringify(response) + '</p>';
              output.appendChild(box);
            }
          };

          xhr.send(data);
      });
    });