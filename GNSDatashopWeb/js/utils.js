


function AddToElementCssClass(elementId, cssClass) {
    var element = dojo.byId(elementId);
    dojo.addClass(element, cssClass);
}

function RemoveFromElementCssClass(elementId, cssClass) {
    var element = dojo.byId(elementId);
    if (element == null)
        return;
    dojo.removeClass(element, cssClass);
}

