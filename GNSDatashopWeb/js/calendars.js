

function getFirstCalenderAsDijitObj() {
    var calender1 = dijit.byId(calender1ClientId);
    return calender1;
}

function getSecondCalenderAsDijitObj() {
    var calender2 = dijit.byId(calender2ClientId);
    return calender2;
}

function verifyContinuity() {
    var firstCalendar = getFirstCalenderAsDijitObj();
    var secondCalendar = getSecondCalenderAsDijitObj();

    if (firstCalendar.value != null && secondCalendar.value != null) {
        var areCalendarsContinuous = (firstCalendar.value <= secondCalendar.value);
        return areCalendarsContinuous;
    }
    else {
        return false;
    }
}

function copyFromFirstToSecondCalender() {
    var calender1 = getFirstCalenderAsDijitObj();
    var calender2 = getSecondCalenderAsDijitObj();
    calender2.set("value", calender1.value);
}

function copyFromSecondToFirstCalender() {
    var calender1 = getFirstCalenderAsDijitObj();
    var calender2 = getSecondCalenderAsDijitObj();
    calender1.set("value", calender2.value);
}

function firstCalendarOnClick() {

    var copyFromSecond = getSecondCalenderAsDijitObj().value != null && !verifyContinuity();
    if (copyFromSecond) {
        copyFromSecondToFirstCalender();
    }
}

function secondCalendarOnClick() {
    var copyFromFirst = getFirstCalenderAsDijitObj().value != null && !verifyContinuity();
    if (copyFromFirst) {
        copyFromFirstToSecondCalender();
    }
}

function firstCalendarOnChange(newDateValue) {
    console.debug(getFirstCalenderAsDijitObj().state);
    if (newDateValue == null)
        return;

    var copyToSecond = getSecondCalenderAsDijitObj().value != null && !verifyContinuity();
    if (copyToSecond) {

        copyFromFirstToSecondCalender();
    }
}

function secondCalendarOnChange(newDateValue) {
    if (newDateValue == null)
        return;

    var copyToFirst = getFirstCalenderAsDijitObj().value != null && getSecondCalenderAsDijitObj().value != null && !verifyContinuity();
    if (copyToFirst) {
        copyFromSecondToFirstCalender();
    }
}