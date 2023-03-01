$(document).ready(function() {
    $('#settings_BTN').click(function() {
        settings.fire({
            title: 'Settings',
            icon: 'none',
            html: `
            <div class="settingsContainer">
            <div class="settingsModuleContainer">
                        <span class="alignLeft">Test</span>
            <label class="switch">
            <input type="checkbox">
            <span class="slider round"></span>
          </label>
                                  <span class="alignLeft">Test</span>
            <label class="switch">
            <input type="checkbox">
            <span class="slider round"></span>
          </label>
                                  <span class="alignLeft">Test</span>
            <label class="switch">
            <input type="checkbox">
            <span class="slider round"></span>
          </label>
          </div>
          </div>
            `
        })
    })
  });
  