
dojo.require("dijit.Dialog");

function ShowHelp(url) {
    window.open(url, "GNSD_HELP", "status=1,toolbar=1,location=1,resizable=1,scrollbars=1,height=400, width=600").focus();
}

var dialogHelp = null;
function ShowHelpModal(url) {
    var done = false;
    if (dialogHelp == null) {
        var dialogContent = '<div id="dlgHelp"><iframe id="iframeHelp" src=""></iframe></div>';
        dialogHelp = new dijit.Dialog({
            title: "GNSD - Online Help",
            content: dialogContent,
            autofocus: !dojo.isIE, // NOTE: turning focus ON in IE causes errors when reopening the dialog 
            refocus: !dojo.isIE
        });
        done = true;
    }
    dialogHelp.show();
    var iFrame = document.getElementById("iframeHelp");
    if (iFrame == null) return;
    //iFrame.contentWindow.navigate(url);
    iFrame.contentWindow.location.href = url;
}
