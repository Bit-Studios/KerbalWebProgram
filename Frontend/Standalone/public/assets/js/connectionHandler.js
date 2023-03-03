let goodGlobe = `<div id="observablehq-chart-9d8c4108"></div><link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/@observablehq/inspector@5/dist/inspector.css"><script type="module">import {Runtime, Inspector} from "https://cdn.jsdelivr.net/npm/@observablehq/runtime@5/dist/runtime.js";
    import define from "https://api.observablehq.com/d/163d4a207bbfed29@106.js?v=3";
    new Runtime().module(define, name => {
      if (name === "chart") return new Inspector(document.querySelector("#observablehq-chart-9d8c4108"));
    });</script>`;

    let badGlobe = `<div id="observablehq-chart-c708747f"></div>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/@observablehq/inspector@5/dist/inspector.css">
    <script type="module">
    import {Runtime, Inspector} from "https://cdn.jsdelivr.net/npm/@observablehq/runtime@5/dist/runtime.js";
    import define from "https://api.observablehq.com/d/07f67f4f8a5eb24a@107.js?v=3";
    new Runtime().module(define, name => {
      if (name === "chart") return new Inspector(document.querySelector("#observablehq-chart-c708747f"));
    });
    </script>`;

    let isConnected = true;

function updateColor() {
    var data = JSON.stringify({
      "ID": "0001", //CONNECTION TEST ID
      "Action": "serverPing",
    });
    
    var xhr = new XMLHttpRequest();
    
    xhr.addEventListener("readystatechange", function() {
      if(this.readyState === 4) {
        console.log(this.responseText);
      }
    });
    
    xhr.open("POST", "http://localhost:8080/api");
    xhr.setRequestHeader("Content-Type", "application/json");
    
    xhr.onload = function() {
      // set background color to green if successful
      $('.connectionReactive').css('outline-color', '#BADA55');
      $('.connectionReactive').css('border-color', '#BADA55');
      $('.connectionReactive').css('color', '#BADA55');
      $('#commNetLogo').attr('src', 'assets/img/CommNetGood.svg');
      $('#serverConnectionLogo').attr('src', 'assets/img/ServerGood.svg');
      if (!isConnected) {
        $('#planetEmbed').html(goodGlobe);
        isConnected = true;
        notificationBottom.fire({
          icon: 'success',
          title: 'Reconnected!'
        })          
      }
    };
  
    xhr.onerror = function() {
      // set background color to red if error occurs
      $('.connectionReactive').css('outline-color', '#8e3b46');
      $('.connectionReactive').css('border-color', '#8e3b46');
      $('.connectionReactive').css('color', '#8e3b46');
      $('#commNetLogo').attr('src', 'assets/img/CommNetBad.svg');
      $('#serverConnectionLogo').attr('src', 'assets/img/ServerBad.svg');
      if (isConnected) {
        $('#planetEmbed').html(badGlobe);
        isConnected = false;
        notificationBottom.fire({
          icon: 'error',
          title: 'Lost connection to Backend Server!'
        })          
        }
    };
    
    xhr.send(data);
  }
  
  $(document).ready(function() {
    $('#planetEmbed').html(goodGlobe);
    // call updateColor() every 15 seconds
    setInterval(updateColor, 15000);
  });
  