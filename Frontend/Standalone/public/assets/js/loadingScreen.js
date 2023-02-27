$(document).ready(function() {
    let ws = new WebSocket("ws://test");
    var countdown = 10;
    var numRetries = 5;
    retryConnect();

    function retryConnect() {
      $('.error').replaceWith('<div class="spinner">');

      //test WS connection

      var socket = $.simpleWebSocket(
        {
            url: 'ws://localhost:8080/',
            timeout: 10000, // optional, default timeout between connection attempts
            attempts: 10, // optional, default attempts until closing connection
            dataType: 'json', // optional (xml, json, text), default json
            onOpen: function(event) {
              $('#loading-screen').hide(); //Hide the loading screen if the server responds with a 200 OK status code
            },
            onError: function(event) {
              console.error('Error: ' + event.data); // Display an error message if the server responds with a 500 error status code
              //Fallback to REST API (ceselistialbodydata as example)
              $.ajax({
                url: 'http://localhost:8080/api/ceselistialbodydata',
                type: 'GET',
                success: function(response) {
                  console.log(response);
                  $('#loading-screen').hide(); //Hide the loading screen if the server responds with a 200 OK status code
                },
                error: function(jqXHR, textStatus, errorThrown) {

          // Display an error message if the server responds with a 500 error status code
          $('#status').html('<span>Error: ' + jqXHR.status + ' ' + errorThrown + '<br> Is Kerbal Web Program installed correctly? </span>');
          $('.spinner').replaceWith('<img src="assets/img/x-24.svg" class="error">');
          $('#status').show(); // Show the error message again if there was an error
  
          if (countdown > 0) {
            setTimeout(retryConnect, 10000);
            $('#countdown').html('Retrying in ' + countdown + ' seconds...');
            countdown--;
          } else {
            if (numRetries !== 0){
                numRetries--;
                countdown = 10;
                $('#countdown').html('Please be patient while we connect...');
                $('#status').hide(); // Hide the error message while reconnecting
                $('.error').replaceWith('<div class="spinner">');
                setTimeout(retryConnect, 2000); // Wait 2 seconds before retrying
            } else {
                $('#countdown').html('Maximum number of retries reached.');
            }
          }

              // Wait 2 seconds before sending the request
    setTimeout(function() {
      retryConnect();
    }, 2000);
  
    // Update the countdown every second
    setInterval(function() {
      if (countdown >= 0) {
        $('#countdown').html('Retrying in ' + countdown + ' seconds...');
        countdown--;
      }
    }, 1000);
                },
            }
          )
        }
      })
      socket.connect();
    }
});
  