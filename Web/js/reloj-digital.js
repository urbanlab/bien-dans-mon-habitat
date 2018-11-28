$(function () {
    function actual() {
        fecha = new Date(); //Actualizar hora.
        hora = fecha.getHours();
        minuto = fecha.getMinutes();
        segundo = fecha.getSeconds();
        var Tiempo = "";
        if (hora < 10) {
            hora = "0" + hora;
        }
        if (minuto < 10) {
            minuto = "0" + minuto;
        }
        if (segundo < 10) {
            segundo = "0" + segundo;
        }

        if (hora > 11) {
            Tiempo = "PM";
        }
        else {
            Tiempo = "AM";
        }

        $("#reloj").text(hora + " : " + minuto + " : " + segundo + " " + Tiempo);
    }
    actual();
    setInterval(actual, 1000);
});