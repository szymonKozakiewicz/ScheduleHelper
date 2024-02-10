function ToggleAvaiableStatusWhenCheckBoxChenge() {
    var checkbox = document.getElementById("HasStartTime");
    
    if (checkbox.checked) {
        $("#StartTime").prop('disabled', false);
    } else {
        $("#StartTime").prop('disabled', true);
    }
    console.log("jestem")
}


$("#HasStartTime").change(ToggleAvaiableStatusWhenCheckBoxChenge)
