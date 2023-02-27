$(document).ready(function() {
    var countdown = 10;
    var numRetries = 5;
  
    function retryConnect() {
      $('.error').replaceWith('<div class="spinner">');
      $.ajax({
        url: '/connect',
        type: 'GET',
        success: function(response) {
          console.log(response);
  
          // Hide the loading screen if the server responds with a 200 OK status code
          $('#loading-screen').hide();
        },
        error: function(jqXHR, textStatus, errorThrown) {
          console.error(textStatus, errorThrown);
  
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
        }
      });
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
  });
  