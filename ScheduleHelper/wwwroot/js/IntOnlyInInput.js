console.log("test")
function DontPressFloat(e) {

    var invalidChars = [".", ","];
    if (invalidChars.includes(e.key)) {
        e.preventDefault();
    }
}

function IntOnlyInInput(inputId)
{

    var inputTime = $("#" + inputId)
    



    inputTime.keypress(DontPressFloat)
}





