$("#btnSetNow").click(function () {
    var now = new Date();
    var hour = now.getHours();
    var minutes = now.getMinutes();
    var hourAndMinutesStr = hour.toString() + ":" + minutes.toString();
    $("#startTime").val(hourAndMinutesStr)
}

)