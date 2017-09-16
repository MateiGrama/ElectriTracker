// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=397704
// To debug code on page load in Ripple or on Android devices/emulators: launch your app, set breakpoints, 
// and then run "window.location.reload()" in the JavaScript Console.

    "use strict";

(function () {
    document.addEventListener( 'deviceready', onDeviceReady.bind( this ), false );

    function onDeviceReady() {
        

        document.addEventListener( 'pause', onPause.bind( this ), false );
        document.addEventListener( 'resume', onResume.bind( this ), false );

        var ocupat = $("#ocupat");
        var liber = $("#liber");
        var urgenta = $("#urgenta");
        var gata = $("#gata");

        // Status: 0=liber; 1=ocupat
        // Urgenta: 0=nu; 1=da
        var status = 0;
        localStorage.setItem("status", status);
        var stareUrgenta = 0;
        localStorage.setItem("stareUrgenta", stareUrgenta);

        //conditia ca sa se apeleze SendAllInfo: sa se fi apelat SendFirstInfo
        localStorage.setItem("readyToSendAllInformation", false);

        ocupat.click(function () {

            if (ocupat.css("background-color") == "rgb(84, 84, 84)") {

                ocupat.css("background-color", "rgb(255, 0, 0)");
                liber.css("background-color", "rgb(84, 84, 84)");
                status = 1;
                localStorage.setItem("status", status);
                $("body").css("background-image", "url('../images/rosu.jpg')");
                $("html").css("background-image", "url('../images/rosu.jpg')");
            }
        });

        liber.click(function () {

            if (liber.css("background-color") == "rgb(84, 84, 84)") {

                liber.css("background-color", "rgb(5, 114, 4)");
                ocupat.css("background-color", "rgb(84, 84, 84)");
                urgenta.css("background-color", "rgb(236, 236, 86)");
                urgenta.text("Semnaleaza situatie de urgenta");

                $("body").css("background-image", "url('../images/verde.jpg')");
                $("html").css("background-image", "url('../images/verde.jpg')");

                status = 0;
                localStorage.setItem("status", status);
                stareUrgenta = 0;
                localStorage.setItem("stareUrgenta", stareUrgenta);
            }

             if (navigator.notification) { // Override default HTML alert with native dialog
                    window.alert = function () {
                        navigator.notification.alert(
                            "Numarul introdus contine caractere nevalabile.",    // message
                            null,       // callback
                            "Atentie!", // title
                            "oK"        // buttonName
                        );
                       };
            }

        });


        if (localStorage.getItem("nume") != "Nume complet" && localStorage.getItem("nume")!=null) {

            $("#form").toggle(0);
            document.getElementById("numeElectrician").innerHTML = localStorage.getItem("nume");
        }


        urgenta.click(function () {

            if (ocupat.css("background-color") == "rgb(255, 0, 0)" && urgenta.css("background-color") == "rgb(236, 236, 86)") {
                urgenta.css("background-color", "rgb(194, 178, 4)");
                urgenta.text("Situatie de urgenta incheiata");
                stareUrgenta = 1;
                localStorage.setItem("stareUrgenta", stareUrgenta);
                $("body").css("background-image", "url('../images/2.jpg')");
                $("html").css("background-image", "url('../images/2.jpg')");
            }
            else if (ocupat.css("background-color") == "rgb(255, 0, 0)" && urgenta.css("background-color") == "rgb(194, 178, 4)") {
                urgenta.css("background-color", "rgb(236, 236, 86)");
                urgenta.text("Semnaleaza situatie de urgenta");
                stareUrgenta = 0;
                localStorage.setItem("stareUrgenta", stareUrgenta);
                $("body").css("background-image", "url('../images/rosu.jpg')");
                $("html").css("background-image", "url('../images/rosu.jpg')");
            }
        });
        var nume, numar;

        $("#nume").click(function () {
            //document.getElementById('nume').value = "\0";
        });

        $("#numar").click(function () {
            //document.getElementById('numar').value = "\0";
        });


        gata.click(function () {

            nume = $("#nume").val();
            numar = $("#numar").val();
            console.log(typeof (numar));

            var isnum = /^\d+$/.test(numar);

            if(isnum==true){
                localStorage.setItem("nume", nume);
                localStorage.setItem("numar", numar);
                SendFirstInformation(nume, numar);

                $("#form").toggle(500);
                document.getElementById("numeElectrician").innerHTML = nume;
            }
            else{
              //  if (navigator.notification) { // Override default HTML alert with native dialog
                window.alert = function () {
                    navigator.notification.alert(
                        "Numarul introdus contine caractere nevalabile.",    // message
                        null,       // callback
                        "Atentie!", // title
                        'OK'        // buttonName
                    );
                    //       };
                };
                numarGresit();
               }
        });


        window.addEventListener("batterystatus", onBatteryStatus, false);

        setInterval(function () { GetGPSCoordinates(); }, 5000);

    };



    function eroare() {
        $("#eroare").show();
        setTimeout(function () { $("#eroare").hide(); }, 5000);

    }
    function numarGresit() {
        document.getElementById("eroare").innerHTML = "Numarul contine caractere nevalide";
        $("#eroare").show();
        setTimeout(function () { $("#eroare").hide(); document.getElementById("eroare").innerHTML = "EROARE CONEXIUNE"; }, 5000);
    }
   
    
    function onPause() {
        // TODO: This application has been suspended. Save application state here.
    };

    function onResume() {
        // TODO: This application has been reactivated. Restore application state here.
    };

    function GetGPSCoordinates() {
        var ready2use = localStorage.getItem("readyToSendAllInformation");
        if (ready2use == "true") {
            navigator.geolocation.getCurrentPosition(geolocationSuccess, geolocationError);
        }
    }

    var geolocationSuccess = function (position) {
        document.getElementById('mesaj').innerText = 'Latitudine: ' + position.coords.latitude + '\n' + 'Longitudine: ' + position.coords.longitude + '\n';
        var nume = localStorage.getItem("nume");
        if (nume == "" || nume == null)
            return;

        var baterie = parseInt(localStorage.getItem("baterie"));
        var status = parseInt(localStorage.getItem("status"));
        var stareUrgenta = parseInt(localStorage.getItem("stareUrgenta"));
        var latitudine = position.coords.latitude;
        var longitudine = position.coords.longitude;
        SendAllInformation(nume, latitudine, longitudine , baterie , status , stareUrgenta);
    };

    function geolocationError(error) {
        eroare();
    }


    function onBatteryStatus(info) {
        var baterie = info.level;
        localStorage.setItem("baterie", baterie);
    }

    function SendFirstInformation(nume, numar) {
        var informationstring = nume + "," + numar;
        var jqxhr = $.ajax({
            method: 'POST',
            url: "http://oppdev01.cloudapp.net:49003/api/tracker/RegisterUser",
            data: { userdata: informationstring },
            //dataType: 'json',
            crossDomain: true,
            jsonp: false
        })
        .done(function () {
            document.getElementById('mesaj').innerText = "Success";
            localStorage.setItem("readyToSendAllInformation", true);

        })
        .fail(function (xhr, textStatus, errorThrown) {
            document.getElementById('mesaj').innerText = xhr.status;
            SendFirstInformation(nume, numar);
            eroare();

        });
    };
 


    //Nume,Latitudine,Longitudine,Baterie,Status,Urgenta
    function SendAllInformation(nume, latitudine, longitudine, baterie, status, stareUrgenta) {
        var informationstring = nume + "," + latitudine + "," + longitudine + "," + baterie + "," + status + "," + stareUrgenta;
        console.log(informationstring);
        var jqxhr = $.ajax({
            method: 'POST',
            url: "http://oppdev01.cloudapp.net:49003/api/tracker/SendAllInformation",
            data: { information: informationstring },
            //dataType: 'json',
            crossDomain: true,
            jsonp: false
        })
        .done(function () {
            document.getElementById('mesaj').innerText = "Success";
        })

        .fail( function(xhr, textStatus, errorThrown) {
            document.getElementById('mesaj').innerText = xhr.status;
            eroare();

        });
    };
    
})();