/**
 * Created by Oscar on 2015/4/21.
 */
var FIVES = FIVES || {};
FIVES.Testing = FIVES.Testing || {};

(function() {

    "use strict";
    var BVHTester = function () {};

    var l = BVHTester.prototype;

    var er = FIVES.Models.EntityRegistry;

    var targetModel = "resources/models/male/male.xml";

    l.btnStartAnimation = function(){
        var animationName = $("#input-BVH").val();
        var sendingString = "start" + ",";

        if ($("#input-file1").val() && $("#input-joint1").val() && $("#input-axis1").val())
        {
            sendingString += $("#input-file1").val() + "," + $("#input-joint1").val() + "," + $("#input-axis1").val() + ",";
        }
        if ($("#input-file2").val() && $("#input-joint2").val() && $("#input-axis2").val())
        {
            sendingString += $("#input-file2").val() + "," + $("#input-joint2").val() + "," + $("#input-axis2").val() + ",";
        }
        if ($("#input-file3").val() && $("#input-joint3").val() && $("#input-axis3").val())
        {
            sendingString += $("#input-file3").val() + "," + $("#input-joint3").val() + "," + $("#input-axis3").val() + ",";
        }
        for (var i in er._entities) {
            var entity = er._entities[i];
            console.log(entity);
            if (entity.mesh.uri == targetModel){
                console.log(entity);
                console.log(sendingString)
                FIVES.Plugins.BVHAnimation.sendStartAnimation(entity.guid, animationName, sendingString);

                //for test setScriptValue
                //var node = FIVES.Resources.SceneManager.xml3dElement.getElementsByTagName("float")[3];
                //var key = new Float32Array(1);
                //key[0] = 0;
                //node.setScriptValue(key);
                //console.log(node);
            }

        }

    }

    l.btnUpdateAnimation = function() {
        var animationName = $("#default-BVH").val();
        var sendingString = "update" + ",";
        if ($("#input-update-frame").val() && $("#input-update-joint").val() && $("#input-update-axis").val() && $("#input-update-value").val())
        {
            sendingString += $("#input-update-frame").val() +  "," + $("#input-update-joint").val() +  "," + $("#input-update-axis").val() +  "," + $("#input-update-value").val();
        }
        for (var i in er._entities) {
            var entity = er._entities[i];
            //console.log(entity);
            if (entity.mesh.uri == targetModel){
                console.log(entity);
                console.log(sendingString)
                FIVES.Plugins.BVHAnimation.sendUpdateAnimation(entity.guid, animationName, sendingString);

            }

        }
    }

    l.animationCallback = function(){

    }

    l.btnStopAnimation = function(){
        var animationName = $("#default-BVH").val();
        var sendingString = "";
        for (var i in er._entities) {
            var entity = er._entities[i];
            //console.log(entity);
            if (entity.mesh.uri == targetModel) {
                FIVES.Plugins.BVHAnimation.sendStopAnimation(entity.guid, animationName, sendingString);
            }
        }
    }

    FIVES.Testing.BVHTester = new BVHTester();
}());
