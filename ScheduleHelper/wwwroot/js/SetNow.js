$(".btnSetNow").click(function () {
    var now = new Date();
    var hour = now.getHours().toString().padStart(2, '0');;
    var minutes = now.getMinutes().toString().padStart(2, '0');
    var hourAndMinutesStr = hour.toString() + ":" + minutes.toString();
    $(".inputWithTimeToBeSetOnCurrent").val(hourAndMinutesStr)
}

)