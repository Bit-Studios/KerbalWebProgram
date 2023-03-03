var initialTime;

function queryTime() {
  var data = JSON.stringify({
    "ID": "0005",
    "Action": "getUniverseTime",
  });
  
  var xhr = new XMLHttpRequest();
  
  xhr.addEventListener("readystatechange", function() {
    if(this.readyState === 4) {
        console.log(this.responseText);
      initialTime = new Date(JSON.parse(this.response));
    }
  });
  
  xhr.open("POST", "http://localhost:8080/api");
  xhr.setRequestHeader("Content-Type", "application/json");
  
  xhr.onload = function() {
  };

  xhr.onerror = function() {
  };
  
  xhr.send(data);
}

function updateClock() {
    var now = new Date();
    var elapsedTime = now.getTime() - initialTime.getTime();
    var seconds = Math.floor(elapsedTime / 1000);
    var minutes = Math.floor(seconds / 60);
    var hours = Math.floor(minutes / 60);
    seconds %= 60;
    minutes %= 60;
    hours %= 24;
    $("#time").text(hours + ":" + minutes + ":" + seconds);
  }
  
  
  $(document).ready(function() {
    setInterval(queryTime, 1* 30000); //ONLY CALL API EVERY 5 MINUTES
    setInterval(updateClock, 1000);
  });
  