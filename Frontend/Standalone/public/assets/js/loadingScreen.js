$(document).ready(function() {
  setTimeout(retryConnect, 2000); // Wait 2 seconds

  function retryConnect() {
    $('.error').replaceWith('<div class="spinner">');
    var numRetries = 5;
    //test WS connection
    var socket = new WebSocket('ws://localhost:8080/');

    socket.onopen = function(event) {
      $('#loading-screen').hide(); //Hide the loading screen if the server responds with a 200 OK status code
      $('section#nav').removeClass('hide');
    };

    socket.addEventListener('error', (event) => {
    console.log('WebSocket error: ', event);

      //Fallback to REST API (gets kerbin as example)
      
      var data = JSON.stringify({
        "ID": "203798423446346",
        "Action": "getCelestialBodyData",
        "parameters": {
          "name": "Kerbin"
        }
      });
      
      var xhr = new XMLHttpRequest();
      
      xhr.addEventListener("readystatechange", function() {
        if(this.readyState === 4) {
          console.log(this.responseText);
        }
      });
      
      xhr.open("POST", "http://localhost:8080/api");
      xhr.setRequestHeader("Content-Type", "application/json");
      
      xhr.send(data);

      xhr.onload = function() {
          $('#loading').hide(); //Hide the loading screen if the server responds with a 200 OK status code
          $('section#nav').removeClass('hide');
        };

      xhr.onerror = function() {
          // Display an error message if the server responds with a 500 error status code
          $('#status').html('<span>Error: 503 Backend Unavailable<br> Is Kerbal Web Program installed correctly? </span>');
          $('.spinner').replaceWith('<img src="assets/img/x-24.svg" class="error">');
          $('#status').show(); // Show the error message again if there was an error

          var countdown = 30;
          var retryInterval = setInterval(function() {
            if (countdown >= 0) {
              $('#countdown').html('Retrying in ' + countdown + ' seconds...');
              countdown--;
            } else {
              if (numRetries !== 0) {
                numRetries--;
                countdown = 10;
                $('#countdown').html('Please be patient while we connect...');
                $('#status').hide(); // Hide the error message while reconnecting
                $('.error').replaceWith('<div class="spinner">');
                clearInterval(retryInterval);
                setTimeout(retryConnect, 2000); // Wait 2 seconds before retrying
              } else {
                clearInterval(retryInterval);
                $('#countdown').html('Maximum number of retries reached.');
              }
            }
          }, 1000);
        }
      })
    }
  });

