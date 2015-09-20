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
        if ($("#input-update-joint").val() && $("#input-update-axis").val() && $("#input-update-value").val())
        {
            sendingString += $("#input-update-joint").val() +  "," + $("#input-update-axis").val() +  "," + $("#input-update-value").val();
        }
        for (var i in er._entities) {
            var entity = er._entities[i];
            //console.log(entity);
            if (entity.mesh.uri == targetModel){
                //console.log(entity);
                //console.log(sendingString)
                FIVES.Plugins.BVHAnimation.sendUpdateAnimation(entity.guid, animationName, sendingString);

            }

        }
    }

    l.animationCallback = function(){

    }

    l.btnInitPose = function(){
        var animationName = "InitPose.bvh";
        var sendingString = "update,Spine,y,0";

        for (var i in er._entities) {
            var entity = er._entities[i];
            //console.log(entity);
            if (entity.mesh.uri == targetModel){
                //console.log(entity);
                //console.log(sendingString)
                FIVES.Plugins.BVHAnimation.sendUpdateAnimation(entity.guid, animationName, sendingString);
            }
        }
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

    l.btnCreateBVHAnimationEntity = function(){
        var posX = _getValidFloatFieldValue("input-position-x");
        var posY = _getValidFloatFieldValue("input-position-y");
        var posZ = _getValidFloatFieldValue("input-position-z");

        var mesh = $("#select-mesh").val();

        var avatarID = $("#input-avatarID").val();

        var obj = new Object();
        obj.targetAvatarURI = mesh;
        obj.avatarID = avatarID;
        obj.position = [posX, posY, posZ];

        var jsonString = JSON.stringify(obj);

        //console.log(jsonString);

        FIVES.Plugins.BVHAnimation.sendCreateAnimationEntity(jsonString);

    }

    l.btnAddMarker = function(){
        var scaleX = _getValidFloatFieldValue("add-marker-input-scale-x");
        var scaleY = _getValidFloatFieldValue("add-marker-input-scale-y");
        var scaleZ = _getValidFloatFieldValue("add-marker-input-scale-z");

        var avatarID = $("#add-marker-input-avatarID").val();

        var mesh = $("#add-marker-select-mesh").val();

        var jointName = $("#add-marker-input-joint").val();

        var color = $("#add-marker-select-color").val();

        var obj = new Object();
        obj.avatarID = avatarID;
        obj.targetMarkerURI = mesh;
        obj.jointName = jointName;
        obj.diameter = scaleX + " " + scaleY + " " + scaleZ;
        obj.color = color;

        var jsonString = JSON.stringify(obj);

        //console.log(jsonString);

        FIVES.Plugins.BVHAnimation.sendAddMarker(jsonString);
    }

    l.btnDeleteMarker = function(){
        var avatarID = $("#delete-marker-input-avatarID").val();
        var obj = new Object();
        obj.avatarID = avatarID;

        var jsonString = JSON.stringify(obj);

        FIVES.Plugins.BVHAnimation.sendDeleteMarker(jsonString);
    }

    var _getValidFloatFieldValue = function (fieldName) {
        var field = $("#" + fieldName);
        if (!field) {
            console.error("[ERROR] EntityCreator._getValidFloatFieldValue: Could not access field with fieldname " + fieldName);
            return;
        }
        var value = field.val();
        if (!value) {
            console.warn("[WARNING] EntityCreator._getValidFloatFieldValue: No value specified for " + fieldName + ", will use defaultvalue instead");
            value = fieldName.indexOf("scale") < 0  ? 0 : 1;
        }
        return value;
    };

    FIVES.Testing.BVHTester = new BVHTester();
}());
