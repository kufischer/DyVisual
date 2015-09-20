/**
 * Created by Oscar on 2015/4/21.
 */
var FIVES = FIVES || {};
FIVES.Testing = FIVES.Testing || {};

(function() {

    "use strict";
    var InteractTester = function () {};

    var l = InteractTester.prototype;

    l.btnStartAnimation = function() {
        //Now only one avatar
        var avatarID = "0";

        FIVES.Plugins.BVHAnimation.sendStartAnimationINTERACT(avatarID)
    }

    l.btnPauseAnimation = function() {

        //var item = document.getElementById("pauseButton");
        //console.log(item);
        //if (item.innerHTML == "Pause")
        //    item.innerHTML = "Resume";
        //else
        //    item.innerHTML = "Pause";
        //Now only one avatar
        var avatarID = "0";

        FIVES.Plugins.BVHAnimation.sendPauseAnimationINTERACT(avatarID)
    }

    l.btnToggleConstraints = function(){
        FIVES.Plugins.BVHAnimation.sendToggleConstraintsINTERACT();
    }

    l.btnStopAnimation = function() {
        //Now only one avatar
        var item = document.getElementById("pauseButton");
        //console.log(item);

        //reset the pause button
        //if (item.innerHTML == "Resume")
        //    item.innerHTML = "Pause";

        var avatarID = "0";

        FIVES.Plugins.BVHAnimation.sendStopAnimationINTERACT(avatarID)
    }

    /*
    l.btnCreateAnimationEntity = function() {
        //Now only one avatar
        var avatarID = "0";

        FIVES.Plugins.BVHAnimation.sendCreateAnimationEntity(avatarID);
    }*/

    /*
    l.btnSetAnimation = function() {
        var inputJson = $("#input-mginput").val();
        var avatarID = $("#input-avatarid").val();

        FIVES.Plugins.BVHAnimation.sendCreateAnimationEntity(inputJson, avatarID);
    }
    */

    FIVES.Testing.InteractTester = new InteractTester();
}());
