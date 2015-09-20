var FIVES = FIVES || {};
FIVES.Plugins = FIVES.Plugins || {};

(function (){
    "use strict";

    var _fivesCommunicator = FIVES.Communication.FivesCommunicator;

    var BVHAnimation = function() {
        FIVES.Events.AddConnectionEstablishedHandler(this._createFunctionWrappers.bind(this));
        FIVES.Events.AddOnComponentUpdatedHandler(this._componentUpdatedHandler.bind(this));

        this.counter = 0;

        this.lastBVHFrameEnitity = false;
        //this.dataObeserver.observe(this.dataNodes[0]);
    };

    var l = BVHAnimation.prototype;

    l._createFunctionWrappers = function (){
        this.startBVHAnimation = _fivesCommunicator.connection.generateFuncWrapper("BVHAnimation.startAnimation");
        this.stopBVHAnimation = _fivesCommunicator.connection.generateFuncWrapper("BVHAnimation.stopAnimation");
        this.updateBVHAnimation = _fivesCommunicator.connection.generateFuncWrapper("BVHAnimation.updateAnimation");
        this.addMarker = _fivesCommunicator.connection.generateFuncWrapper("BVHAnimation.addMarker");
        this.deleteMarker = _fivesCommunicator.connection.generateFuncWrapper("BVHAnimation.deleteMarker");

        //For INTERACT
        this.createAnimationEntity = _fivesCommunicator.connection.generateFuncWrapper("BVHAnimation.createAnimationEntity");
        this.startAnimationINTERACT = _fivesCommunicator.connection.generateFuncWrapper("BVHAnimation.startAnimationINTERACT");
        this.stopAnimationINTERACT = _fivesCommunicator.connection.generateFuncWrapper("BVHAnimation.stopAnimationINTERACT");
        this.pauseAnimationINTERACT = _fivesCommunicator.connection.generateFuncWrapper("BVHAnimation.pauseAnimationINTERACT");

        this.ToggleConstraintsINTERACT = _fivesCommunicator.connection.generateFuncWrapper("BVHAnimation.toggleConstraintsINTERACT");
    };

    l._componentUpdatedHandler = function(entity, componentName, attributeName) {
        if(componentName == "BVHAnimation")
        {

            if (attributeName == "bvhheader")
            {
                //TODO: Only trigger once if separating header and frame
                FIVES.Plugins.BVHSkeleton.initPose(entity.BVHAnimation.bvhheader);

                //console.log("header!");

            }

            if (attributeName == "bvhaction")
            {
                //console.log(entity.BVHAnimation.bvhaction);

                FIVES.Plugins.BVHAction.initActionData(entity.BVHAnimation.bvhaction);

            }

            if (attributeName == "bvhframe")
            {

                //TODO: playback the animation frame by frame
                //console.log(entity.BVHAnimation.bvhframe);
                var result = this.separateHeadAndMotionString(entity.BVHAnimation.bvhframe);
                FIVES.Plugins.BVHSkeleton.initPose(result["header"]);
                FIVES.Plugins.BVHSkeleton.updatePose(result["motion"]);

                if (FIVES.Plugins.BVHAction.actionData != null)
                {
                    for(var i = 0; i<FIVES.Plugins.BVHAction.actionData.length; i++)
                    {
                        if (parseInt(result["framenum"]) > parseInt(FIVES.Plugins.BVHAction.actionData[i][0]))
                        {
                            console.log(FIVES.Plugins.BVHAction.actionData[i][1]);
                            FIVES.Plugins.BVHAction.actionData.splice(i, 1);
                            break;
                        }
                    }
                }

                //if (result["action"] != "None,")
                //console.log(result["framenum"]);

                //console.log(entity.BVHAnimation.bvhframe);

                //var node = FIVES.Resources.SceneManager.xml3dElement.getElementsByTagName("float")[3];
                //node.setScriptValue(key);
                //console.log(node);
                //console.log("frame!");

                this.setAnimationData(entity);

                //A flag to tell marker if it is available to update the marker pose
                this.lastBVHFrameEnitity = true;
                if(FIVES.Plugins.BVHMarker.markerCollection[entity] != null)
                    FIVES.Plugins.BVHMarker.updateMarkerPos(entity);
            }

            if(attributeName == "bvhmarker")
            {
                if (entity.BVHAnimation.bvhmarker == "Delete") {
                    FIVES.Plugins.BVHMarker.removeMarker(entity);
                }

                else
                {
                    FIVES.Plugins.BVHMarker.initMarker(entity, entity.BVHAnimation.bvhmarker);

                    //If we have a avatar, we attach the marker directly into it.
                    if (this.lastBVHFrameEnitity == true)
                        FIVES.Plugins.BVHMarker.updateMarkerPos(entity);
                }

                console.log(entity.BVHAnimation.bvhmarker);

            }




            //var frameData = entity.BVHAnimation.bvhframe;
            //var xml3dPosition = new XML3DVec3(position.x, position.y, position.z);
            //transfer string frameData into translation and quaternion

            //var xmlDoc=loadXMLDoc("resources/models/male/male_defs.xml");
            //var keyNode = xmlDoc.getElementsByTagName("float");
            //var transNode = xmlDoc.getElementsByTagName("float3").namedItem("male-trans");
            //var rotNode = xmlDoc.getElementsByTagName("float4").namedItem("male-rot");

            //console.log(keyNode[6]);
            //set script value here
            //var key = new Float32Array(1);
            //key[0] = 1;

            //console.log(frameData);
        }
    };

    l.separateHeadAndMotionString = function(bvhframe)
    {
        var result = new Array();
        var headerString = "";
        var motionString = "";
        var framenumString = "";

        var counter = 0;
        for (var i=0; i<bvhframe.length; i++)
        {
            if (bvhframe[i] == ":")
            {
                counter = counter + 1;
                continue;
            }

            if (counter == 0)
            {
                framenumString += bvhframe[i];
            }

            if (counter == 1)
            {
                headerString += bvhframe[i];
            }

            if (counter == 2)
            {
                motionString += bvhframe[i];
            }
        }


        result["header"] = headerString;
        result["motion"] = motionString;
        result["framenum"] = framenumString;

        return result;
    }

    l.sendAddMarker = function(jsonString)
    {
        this.addMarker(jsonString, _fivesCommunicator.generateTimestamp());
    }

    l.sendDeleteMarker = function(jsonString)
    {
        this.deleteMarker(jsonString, _fivesCommunicator.generateTimestamp());
    }

    l.sendToggleConstraintsINTERACT = function()
    {
        this.ToggleConstraintsINTERACT(_fivesCommunicator.generateTimestamp());
    }

    l.sendPauseAnimationINTERACT = function(avatarID)
    {
        this.pauseAnimationINTERACT(avatarID, _fivesCommunicator.generateTimestamp());
    }

    l.sendStopAnimationINTERACT = function(avatarID)
    {
        this.stopAnimationINTERACT(avatarID, _fivesCommunicator.generateTimestamp());
    }


    l.sendStartAnimationINTERACT = function(avatarID)
    {
        this.startAnimationINTERACT(avatarID, _fivesCommunicator.generateTimestamp());
    }

    l.sendCreateAnimationEntity = function(jsonString){
        this.createAnimationEntity(jsonString, _fivesCommunicator.generateTimestamp());
    }

    l.sendStartAnimation = function(guid, animationName, sendString){
        this.startBVHAnimation(guid, animationName, sendString, _fivesCommunicator.generateTimestamp() );
    }

    l.sendUpdateAnimation = function(guid, animationName, sendString){
        this.updateBVHAnimation(guid, animationName, sendString, _fivesCommunicator.generateTimestamp() );
    }

    l.sendStopAnimation = function(guid, animationName, sendString){
        this.stopBVHAnimation(guid, animationName, sendString, _fivesCommunicator.generateTimestamp() );
    }

    l.setAnimationData = function(entity)
    {
        var transName = "male-trans-" + entity.guid;
        var rotName = "male-rot-" + entity.guid;
        var keyName = "male-key-" + entity.guid;

        var element = FIVES.Resources.SceneManager.xml3dElement;

        var keynode = element.getElementsByTagName("float").namedItem(keyName);
        var transnode = element.getElementsByTagName("float3").namedItem(transName);
        var rotnode = element.getElementsByTagName("float4").namedItem(rotName);

        var key = new Float32Array(1);
        key[0] = 1;
        keynode.setScriptValue(key);

        //console.log(element.getElementsByTagName("transform"));

        //element.getElementsByTagName("assetmesh")[2].setAttribute("shader", "marker_defs.xml#Material_red");
        //element.getElementsByTagName("transform")[5].setAttribute("scale", "8 8 8")
        transnode.setScriptValue(FIVES.Plugins.BVHSkeleton.translationData);
        rotnode.setScriptValue(FIVES.Plugins.BVHSkeleton.quaternionData);

    }

    FIVES.Plugins.BVHAnimation = new BVHAnimation();

}());