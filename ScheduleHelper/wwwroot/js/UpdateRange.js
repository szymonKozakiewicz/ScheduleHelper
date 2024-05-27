function UpdateRange() {
    colorClasses = "sucessColor text-danger colorWarning"
    var min = $('#ComplitedShareOfTask').attr('min');
    var max = $('#ComplitedShareOfTask').attr('max');
    var val = $('#ComplitedShareOfTask').val();
    var percentage = ((val - min) / (max - min)) * 100;
    $('#labShareOfComplited').text(percentage.toFixed(0) + '%');
    $('#labShareOfComplited').removeClass(colorClasses);
    if (val < 30) {
        $('#labShareOfComplited').addClass('text-danger')
    } else if (val < 80) {
        $('#labShareOfComplited').addClass('colorWarning')
    } else {
        $('#labShareOfComplited').addClass('sucessColor ')
    }
}

$('#ComplitedShareOfTask').on('input', UpdateRange);