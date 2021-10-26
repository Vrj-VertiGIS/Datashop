// this requires dojo.require("dijit.form.AutoTextBox");

// ============================================================================
// wia
// These are some goodies to get dogo widgits surviving a partial page update
// (and also improve soem rendering, e.g. required field, calendar icons, etc.)
// ============================================================================

// DateTextBox (calendar)
// ======================
//function DojoCreateDateTextBox(digitId) {
//    var dateTextBox = new dijit.form.DateTextBox({ id: digitId }, digitId);
//}

//function DojoRecreateDateTextBox(digitId) {
//    var dateTextBox = dijit.byId(digitId);
//    if (dateTextBox) {
//        dateTextBox.destroyRecursive();
//    }
//    DojoCreateDateTextBox(digitId);
//}
