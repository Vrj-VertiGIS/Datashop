var DSMD = function(dialogBoxId, dialogBackgroundId, dialogStateId, offsetLeft, offsetTop) {
    var host = this;
    var visible = false;
    var dialogBox = null;
    var dialogBackground = null;
    var dialogState = null;

    this.Show = function() {
        if (!dialogBox) {
            this.Init();
        }
        dialogBox.style.display = "";
        dialogBackground.style.display = "";
        visible = true;
        Reposition();
    }

    this.Hide = function() {
        visible = false;
        dialogBox.style.display = "none";
        dialogBackground.style.display = "none";
        dialogState.value = "";
    }

    var Reposition = function(e) {
        if (!visible) return;
        var windowHeight = dialogBackground.offsetHeight;
        //windowHeight = document.body.clientHeight;  //(window.innerHeight != null) ? window.innerHeight : document.body.offsetHeight;
        var windowWidth = dialogBackground.offsetWidth;
        var dialogHeight = dialogBox.offsetHeight;
        var dialogWidth = dialogBox.offsetWidth;
        var dialogTop = 0;
        var dialogLeft = 0;
        if (dialogHeight < windowHeight)
            dialogTop = (windowHeight - dialogHeight) / 2;
        dialogTop -= offsetTop;
        if (dialogWidth < windowWidth)
            dialogLeft = (windowWidth - dialogWidth) / 2;
        dialogLeft -= offsetLeft;
        dialogBox.style.top = dialogTop + "px";
        dialogBox.style.left = dialogLeft + "px";
        dialogState.value = dialogLeft + ";" + dialogTop;
    }

    this.Init = function() {
        dialogBox = document.getElementById(dialogBoxId);
        dialogBackground = document.getElementById(dialogBackgroundId);
        dialogState = document.getElementById(dialogStateId);
        if (dialogState.value != "") {
            visible = true;
            Reposition();
        }
        window.onresize = Reposition;
    }

}