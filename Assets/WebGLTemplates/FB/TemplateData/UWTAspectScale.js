/**
 * 
 * @param {string} elementId 
 * @param {number} width 
 * @param {number} height 
 * @param {number} maxAspectRatio 
 */
function UWTAspectScaler(elementId, width, height, maxAspectRatio) {
    
    this.elementId = elementId;
    
    this.desiredWidth = width;
    this.desiredHeight = height;

    this.minAspectRatio = width / height;
    this.maxAspectRatio = !!maxAspectRatio ? Math.max(maxAspectRatio, this.minAspectRatio) : this.minAspectRatio;

    this.element = null;
    this.elementVisible = false;
}

/**
 * 
 * @param {string} elementId 
 * @param {number} width 
 * @param {number} height 
 * @param {number|undefined} maxAspectRatio 
 * @returns {UWTAspectScaler}
 */
function UWTAspectScaleCreate(width, height, maxAspectRatio, elementId) {
    var scaler = new UWTAspectScaler(elementId, width, height, maxAspectRatio);

    scaler.element = document.getElementById(scaler.elementId);
    UWTAspectScaleHide(scaler);

    window.addEventListener("resize", function() {
        if (scaler.elementVisible) {
            setTimeout(function() {
                UWTAspectScaleUpdate(scaler);
            }, 0);
        }
    });

    return scaler;
}

/**
 * Hides the aspect ratio scaler element
 * @param {UWTAspectScaler} scaler
 */
function UWTAspectScaleHide(scaler) {
    scaler.elementVisible = false;

    if (scaler.element) {
        scaler.element.style.visibility = "hidden";
        return true;
    }

    return false;
}

/**
 * Shows the aspect ratio scale element
 * @param {UWTAspectScaler} scaler 
 */
function UWTAspectScaleShow(scaler) {
    scaler.elementVisible = true;

    if (scaler.element) {
        scaler.element.style.visibility = "visible";
        UWTAspectScaleUpdate(scaler);
        return true;
    }

    return false;
}

/**
 * Updates the aspect ratio scaler element
 * @param {UWTAspectScaler} scaler 
 */
function UWTAspectScaleUpdate(scaler) {
    if (!scaler.element) {
        return false;
    }

    var parent = scaler.element.parentElement;

    var parentWidth = parent.clientWidth;
    var parentHeight = parent.clientHeight;

    var aspectRatio = Math.min(Math.max(parentWidth / parentHeight, scaler.minAspectRatio), scaler.maxAspectRatio);
    var finalWidth = parentHeight * aspectRatio;
    var finalHeight = parentHeight;

    if (finalWidth > parentWidth) {
        finalHeight = parentWidth / aspectRatio;
        finalWidth = finalHeight * aspectRatio;
    }

    var elementStyle = "width: " + finalWidth + "px; height: " + finalHeight + "px; margin: auto";
    scaler.element.setAttribute("style", elementStyle);

    var childCanvases = scaler.element.getElementsByClassName("canvas");
    if (childCanvases && childCanvases.length > 0) {
        
        /** @type {HTMLCanvasElement} */
        var childCanvas = childCanvases[0];
        childCanvas.setAttribute("style", elementStyle);
        childCanvas.width = finalWidth;
        childCanvas.height = finalHeight;
    }

    return true;
}