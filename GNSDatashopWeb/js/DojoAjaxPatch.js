/********************************************************************************************
*Include this file in any page that uses ASP.NET update panels and dijit layout form elements.
***********************************************************************************************/
Sys.Application.add_load(ApplicationLoadHandler);
function ApplicationLoadHandler(sender, args) {
    /* Update 6-05-2009 - Added partial load check to prevent page load handlers from being added multiple times */
    if (!args.get_isPartialLoad()) {
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(pageLoaded);
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoading(pageLoading);
    }
}

function pageLoaded(sender, args) {
    var updatedPanels = args.get_panelsUpdated();
    if (typeof (updatedPanels) === "undefined") {
        return;
    }
    //call the dojo parser on the newly loaded html in each panel so the new elements are instantiated
    for (i = 0; i < updatedPanels.length; i++) {
        dojo.parser.parse(updatedPanels[i]);
    }
}

function pageLoading(sender, args) {
    var updatedPanels = args.get_panelsUpdating();
    if (typeof (updatedPanels) === "undefined") {
        return;
    }
    //remove all the widgets in the outgoing panel so the dojo parser doesn't throw 
    //an error when it reloads them.
    for (i = 0; i < updatedPanels.length; i++) {
        var unloadPanel = dojo.byId(updatedPanels[i].id);
        if (!unloadPanel) {
            continue;
        }

        var nodeList = dojo.query('[widgetId]', unloadPanel);
        dojo.forEach(nodeList, function(widget) { destroyWidget(widget) });
    }
}

function destroyWidget(widget) {

    var widgetId = dojo.attr(widget, 'widgetId');
    if (dijit.byId(widgetId)) {
        dijit.byId(widgetId).destroy(true);
    }
}