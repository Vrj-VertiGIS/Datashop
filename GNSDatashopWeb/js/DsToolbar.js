// This class handles all button events, responds to some client commands and renders the buttons according to their state.
// Rendering is done using css,.
// We assume that the outter buttons are not button-specific (frame only)
// We assume that the inner buttons are button specific (frame + icon). This is done by combining one or several classes (button state) 
// with one class for the icon. The combination can be done using a space or an underscore. Which provides us with full flexibility.

function dsToolbar(tbDefaultClass, tbDisabledClass, btnOutDefaultClass, btnInDefaultClass,
				btnOutHoverClass, btnInHoverClass, btnOutPressedClass, btnInPressedClass,
				btnOutSelectedClass, btnInSelectedClass, btnOutDisabledClass, btnInDisabledClass) {

    //*****************
    // toolbar methods
    //*****************

    // not used
    // the intention was to enable/disable the whole toolbar in cases such as map reload
    this.Enable = function(div) {
        // use tbDefaultClass;
    }
    this.Disable = function(div) {
        // use tbDisabledClass
    }

    //******************************
    // button methods or properties
    //******************************

    // this is the button renderer (the only place where the buttons are layouted)
    this.RenderButton = function(btn) {
        var iconClass = btn.getAttribute("_iconClass");
        var btnState = btn.getAttribute("_state");
        switch (btnState) {
            case "disabled":
                btn.parentNode.className = btnOutDisabledClass;
                btn.className = btnInDisabledClass + iconClass;
                break;
            case "hover":
                btn.parentNode.className = btnOutHoverClass;
                btn.className = btnInHoverClass + iconClass;
                break;
            case "pressed":
                btn.parentNode.className = btnOutPressedClass;
                btn.className = btnInPressedClass + iconClass;
                break;
            case "selected":
                btn.parentNode.className = btnOutSelectedClass;
                btn.className = btnInSelectedClass + iconClass;
                break;
            default:
                btn.parentNode.className = btnOutDefaultClass;
                btn.className = btnInDefaultClass + iconClass;
                break;
        }
    }

    this.IsDisabled = function(btn) {
        return (btn.getAttribute("_disabled")) ? true : false;
    }
    this.IsSelected = function(btn) {
        return (btn.getAttribute("_selected")) ? true : false;
    }
    // the button state can be 
    this.GetButtonState = function(btn) {
        return btn.getAttribute("_state");
    }

    // these are the button adapters, that invoke the button renderer
    this.DefaultButton = function(btn) {
        btn.setAttribute("_state", "default");
        this.RenderButton(btn);
    }
    this.EnterButton = function(btn) {
        if (this.IsDisabled(btn)) return;
        if (this.IsSelected(btn)) return;
        btn.setAttribute("_state", "hover");
        this.RenderButton(btn);
    }
    this.PressButton = function(btn) {
        if (this.IsDisabled(btn)) return;
        btn.setAttribute("_state", "pressed");
        this.RenderButton(btn);
    }
    this.ReleaseButton = function(btn) {
        if (this.IsDisabled(btn)) return;
        btn.setAttribute("_state", "hover");
        this.RenderButton(btn);
    }
    this.ExitButton = function(btn) {
        if (this.IsDisabled(btn)) return;
        if (this.IsSelected(btn)) return;
        this.DefaultButton(btn);
    }
    this.SelectButton = function(btn) {
        if (this.IsDisabled(btn)) return;
        btn.setAttribute("_selected", "true");
        btn.setAttribute("_state", "selected");
        this.RenderButton(btn);
    }
    this.UnselectButton = function(btn) {
        if (this.IsDisabled(btn)) return;
        btn.removeAttribute("_selected");
        this.DefaultButton(btn);
    }
    this.EnableButton = function(btn) {
        btn.removeAttribute("_disabled");
        this.DefaultButton(btn);
    }
    this.DisableButton = function(btn) {
        btn.setAttribute("_disabled", "true");
        btn.setAttribute("_state", "disabled");
        this.RenderButton(btn);
    }

    // these are the four event handlers for the buttons (mouseover, mouseout, mousedown, mouseup)
    this.BtnMouseOver = function(evt) {
        if (!evt) evt = window.event;
        var btn = (evt.srcElement) ? evt.srcElement : evt.target;
        this.EnterButton(btn);
    }
    this.BtnMouseOut = function(evt) {
        if (!evt) evt = window.event;
        var btn = (evt.srcElement) ? evt.srcElement : evt.target;
        this.ExitButton(btn);
    }
    this.BtnMouseDown = function(evt) {
        if (!evt) evt = window.event;
        var btn = (evt.srcElement) ? evt.srcElement : evt.target;
        this.PressButton(btn);
    }
    this.BtnMouseUp = function(evt) {
        if (!evt) evt = window.event;
        var btn = (evt.srcElement) ? evt.srcElement : evt.target;
        this.ReleaseButton(btn);
    }

}
