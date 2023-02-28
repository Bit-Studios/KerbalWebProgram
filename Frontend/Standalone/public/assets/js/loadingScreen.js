$(document).ready(function() {
  retryConnect();

  function retryConnect() {
    $('.error').replaceWith('<div class="spinner">');
    var numRetries = 5;
    //test WS connection
    var socket = new WebSocket('ws://localhost:8080/');

    socket.onopen = function(event) {
      $('#loading-screen').hide(); //Hide the loading screen if the server responds with a 200 OK status code
    };

    socket.addEventListener('error', (event) => {
    console.log('WebSocket error: ', event);

      //Fallback to REST API (gets kerbin as example)
      
      $.ajax({
        location: 'localhost:8080/api',
        type: 'GET',
        contentType: 'application/json',
        data: JSON.stringify({
          "ID": "0001",
          "Action" : "getCelestialBodyData",
          "parameters": {
            "name": "Kerbin"
          } 
        }),
        success: function(response) {
          console.log(`Data about kerbin (CON. TEST): ${response}`);
          $('#loading-screen').hide(); //Hide the loading screen if the server responds with a 200 OK status code
        },
        error: function(jqXHR, textStatus, errorThrown) {
          console.log(jqXHR, textStatus, errorThrown);
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
    })
  }
});
