// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function hasValue(variable) {
    if (variable === undefined || variable === null) return false;
    if (typeof variable === 'string' && variable.trim() === '') return false;
    if (Array.isArray(variable) && variable.length === 0) return false;
    if (variable.constructor === Object && Object.keys(variable).length === 0) return false;
    return true;
}

// Disable Enable all child Controls(input, select, button, select2) of Container Class (Paramater=ClassName)
function DisableAllChildControlsByContainerClass(className) {
    const parent = document.querySelector('.' + className); // Select the parent element with the class "content"

    const controls = parent.querySelectorAll('input, button, select'); // Find all input elements within this parent
    controls.forEach(input => { // Disable each control
        input.disabled = true;
    });

    // For Select2 elements specifically, you may need to trigger a change because Select2 uses a different approach by masking the real select element
    $('.content select').select2().prop('disabled', true).select2();
}
function EnableAllChildControlsByContainerClass(className) {
    const parent = document.querySelector('.' + className); // Select the parent element with the class "content"

    const controls = parent.querySelectorAll('input, button, select'); // Find all input elements within this parent
    controls.forEach(input => { // Disable each control
        input.disabled = false;
    });

    // For Select2 elements specifically, you may need to trigger a change because Select2 uses a different approach by masking the real select element
    $('.content select').select2().prop('disabled', false).select2();
}

// Add Remove Loading Overlay 
function addOverlayToDivByQuerySelector(querySelectionStr) {
    var containerElem = document.querySelector(querySelectionStr);
    if (containerElem) {
        // Create overlay div
        var overlayDiv = document.createElement('div');
        overlayDiv.className = 'overlay';

        // Create icon
        var icon = document.createElement('i');
        icon.className = 'fas fa-3x fa-sync-alt fa-spin text-succes';

        // Create text div
        var textDiv = document.createElement('div');
        textDiv.className = 'text-bold pt-2';
        textDiv.textContent = 'Loading...';

        // Append icon and text to overlay div
        overlayDiv.appendChild(icon);
        overlayDiv.appendChild(textDiv);

        // Prepend overlay div to card body
        containerElem.prepend(overlayDiv);
    }
}
function removeOverlayByQuerySelector(querySelectionStr) {
    var overlayDiv = document.querySelector(querySelectionStr + ' .overlay');
    if (overlayDiv) {
        overlayDiv.remove();
    }
}