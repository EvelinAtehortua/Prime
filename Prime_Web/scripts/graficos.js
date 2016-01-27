function cargarGauges() {
    var caras = ["cara1", "cara2", "cara3", "cara4", "cara5", "cara6", "cara7"];
    //var cara1, cara2, cara3, cara4, cara5, cara6, cara7;
    window.onload = function () {
        for (i = 0; i < 7; i++) {
            //var cara = caras[i];
            var cara = new JustGage({
                id: "cara" + (i + 1),
                value: getRandomInt(0, 100),
                min: 0,
                max: 100,
                symbol: "%",
                relativeGaugeSize: true,
            });
        }
    };
}