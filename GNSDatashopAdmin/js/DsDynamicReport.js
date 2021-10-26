//////function DSDRTogglePanel(evt, bodyId, collapsedClassname, expandedClassname) {
//////    if (!evt) evt = window.event;
//////    var head = evt.srcElement || evt.target;
//////    var body = document.getElementById(bodyId);
//////    if (body.style.display == "none") {
//////        body.style.display = "";
//////        head.className = expandedClassname;
//////    }
//////    else {
//////        body.style.display = "none";
//////        head.className = collapsedClassname;
//////    }
//////}
//////function DSDRTabControl() {
//////    this.Select = function(evt, bodyId, unselectedClassname, selectedClassname) {
//////        if (!evt) evt = window.event;
//////        var head = evt.srcElement || evt.target; 
//////        var parent = head.parentNode;
//////        for (var childNb = 0; childNb < parent.children.length; childNb++) {
//////            var child = parent.children[childNb];
//////            var panelId = child.getAttribute('_panelId');
//////            if (panelId != bodyId) {
//////                child.className = unselectedClassname;
//////                var body = document.getElementById(panelId);
//////                body.style.display = "none";
//////            }
//////        }
//////        var body = document.getElementById(bodyId);
//////        if (body.style.display == "none") {
//////            body.style.display = "";
//////            head.className = selectedClassname;
//////        }
//////        else {
//////            body.style.display = "none";
//////            head.className = unselectedClassname;
//////        }
//////    }
//////}
//////var DSDRTabs = new DSDRTabControl();
function DSDRTogglePanel(headId, bodyId, collapsedClassname, expandedClassname) {
    var head = document.getElementById(headId);
    var body = document.getElementById(bodyId);
    if (body.style.display == "none") {
        body.style.display = "";
        head.className = expandedClassname;
    }
    else {
        body.style.display = "none";
        head.className = collapsedClassname;
    }
}
function DSDRTabControl() {
    this.Select = function(headId, bodyId, unselectedClassname, selectedClassname) {
        // do not do anything if the body is already selected
        var body = document.getElementById(bodyId);

        if (body.style.display != "none") return;
        // or reset all other tabs
        var head = document.getElementById(headId);
        this.ActiveTab = head;
        var parent = head.parentNode;
        for (var childNb = 0; childNb < parent.children.length; childNb++) {
            var child = parent.children[childNb];
            var panelId = child.getAttribute('_panelId');
            if (panelId != bodyId) {
                child.className = unselectedClassname;
                var body = document.getElementById(panelId);
                body.style.display = "none";
            }
        }
        body = document.getElementById(bodyId);
        if (body.style.display == "none") {
            body.style.display = "";
            head.className = selectedClassname;
        } else {
            body.style.display = "none";
            head.className = unselectedClassname;
        }
    };

    this.ActiveTab = null;
}
var DSDRTabs = new DSDRTabControl();
