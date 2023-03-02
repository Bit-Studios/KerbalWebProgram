$(document).ready(function() {
    // Hide all exoplanets
    $('.exoHolder').hide();
    
    // Add a click event listener to all arrow icons
    $('.arrow').click(function() {
      // Find the parent div of the arrow icon
      const parentDiv = $(this).closest('.planetSection');
  
      // Find the arrow icon in the current div and toggle its class
      const arrow = parentDiv.find('.arrow');
      arrow.toggleClass('ms-Icon--ChevronUp ms-Icon--ChevronDown');
  
      // Find the exoplanet holder in the current div and toggle its visibility
      const exoHolder = parentDiv.find('.exoHolder');
      exoHolder.toggleClass('ms-motion-slideUpIn ms-motion-slideDownOut $ms-motion-duration-4').toggle();
  
    });
  });
