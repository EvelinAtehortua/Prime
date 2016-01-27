function cargarCalendario(id) {
    $('#param' + id + '').datepicker({
        format: 'mm-dd-yyyy'
    });
}

function cargarCombo(id) {
    $('#param' + id + '').select2();
}

function obtenerCheck(id, dimension, dimVal) {
    dimVal.push(dimension);
    $.each($('.param' + id + ':checkbox:checked'), function () {
        dimVal.push($(this).val());
    });
    dimVal.push("|");
}

function obtenerSelect(id, dimension, dimVal) {
    dimVal.push(dimension);
    var valor = $('#param' + id + '').val();
    dimVal.push(valor);
    dimVal.push("|");
}

function obtenerRadio(id, dimension, dimVal) {
    dimVal.push(dimension);
    var valor = $('#param' + id + '').val();
    dimVal.push(valor);
    dimVal.push("|");
}

function obtenerTextarea(id, dimension, dimVal) {
    dimVal.push(dimension)
    var valor = $('#param' + id + '').val();
    if (valor == "") {
        dimVal.push("null");
    } else {
        dimVal.push(valor);
    }
    dimVal.push("|");
}

function obtenerText(id, dimension, dimVal) {
    dimVal.push(dimension);
    var valor = $('#param' + id + '').val();
    if (valor == "") {
        dimVal.push("null");
    } else {
        dimVal.push(valor);
    }
    dimVal.push("|");
}