$(document).ready(function () {
    var xml3d = document.querySelector("xml3d");
    var meshCount = 20;

    xml3d.addEventListener("load", function () {
        $("span.fa-spin").removeClass("fa-spin fa-circle-o-notch").addClass("fa-check");
    });

    var currentStep = 1;
    var playing = false;
    var discrete = true;

    $(".step-backward").click(function () {
        var oldStep = currentStep;
        currentStep = Math.floor(currentStep);
        if (currentStep == oldStep) {
            currentStep = Math.max(1, currentStep - 1);
        }
        playing = false;
        updateStep();
    });

    $(".step-forward").click(function () {
        var oldStep = currentStep;
        currentStep = Math.ceil(currentStep);
        if (currentStep == oldStep) {
            currentStep = Math.min(meshCount, currentStep + 1);
        }
        playing = false;
        updateStep();
    });

    $(".step-play").click(function () {
        playing = !playing;
    });


    function updateStep() {
        $(".currentStep").text(currentStep.toFixed(2));
    }

    function updateColorMap() {
        var src = "assets/textures/colors";
        if (discrete) {
            src += "-discrete";
        }
        $(".colormap").attr("src", src +".png");
        $("#slider-range,#legend-colors").toggleClass("discrete", discrete);
    }

    updateStep();

    if (window.Stats) {
        var stats = new Stats();
        stats.setMode(0); // 0: fps, 1: ms
        document.querySelector(".stats").appendChild(stats.domElement);
        var loop = function () {
            stats.update();
            if (playing) {
                currentStep += 0.01;
                if (currentStep > meshCount) {
                    currentStep -= meshCount;
                }
                updateStep();
            }
            requestAnimationFrame(loop);
        };
        loop();
    }

    var slider = $("#slider-range");

    var domMin = $(".viz-minimum");
    var domMax = $(".viz-maximum");
    var threshold = $(".viz-threshold");

    slider.slider({
        range: true,
        min: -1,
        max: 1,
        step: 0.05,
        values: [-1, 1],
        slide: function (event, ui) {
            domMin.text(ui.values[0]);
            domMax.text(ui.values[1]);
        }
    });
    domMin.text(slider.slider("values", 0));
    domMax.text(slider.slider("values", 1));

    var gslider = $("#slider-grey");
    gslider.slider({
        min: 0,
        max: 1.5,
        step: 0.01,
        values: [1.5],
        slide: function (event, ui) {
            threshold.text(ui.values[0]);
        }
    });

    $("#discrete").click(function (e) {
        discrete = this.checked;
        updateColorMap();
    });

});
