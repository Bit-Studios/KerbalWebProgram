$(document).ready(function() {
    var spans = $('h1 span');
    var spanCount = spans.length;
    var i = 0;
    
    setInterval(function() {
      if (i >= spanCount) {
        i = 0;
      }
      spans.eq(i).addClass('wave');
      i++;
      if (i >= spanCount) {
        setTimeout(function() {
          spans.removeClass('wave');
          i = 0;
        }, 1000);
      }
    }, 100);
  });
  