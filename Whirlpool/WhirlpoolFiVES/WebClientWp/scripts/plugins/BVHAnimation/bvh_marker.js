var FIVES = FIVES || {};
FIVES.Plugins = FIVES.Plugins || {};

(function (){
    "use strict";

    var quat = XML3D.math.quat;
    var vec3 = XML3D.math.vec3;
    var vec4 = XML3D.math.vec4;
    var mat4 = XML3D.math.mat4;

    var BVHMarker = function() {
        this.markerCollection = {};
    }

    var m = BVHMarker.prototype;

    m.initMarker = function(entity, configString)
    {

        var nameString = "";
        var markerData = [];
        for (var i=0; i<configString.length; i++)
        {
            if (configString[i] != ",")
            {
                nameString += configString[i];
            }
            else{
                markerData.push(nameString); //1: marker's entity, 2: JointName, 3:diameter, 4:color
                nameString = "";
            }

        }
        //console.log(markerData);
        if (this.markerCollection[entity] == null)
            this.markerCollection[entity] = [];
        this.markerCollection[entity].push(markerData);

        var element = FIVES.Resources.SceneManager.xml3dElement;
        //console.log(element.getElementsByTagName("transform"));

        var transformName = "transform-" + markerData[0];
        var assetmeshName = "marker-assetmesh-" + markerData[0];

        //Set diameter
        //console.log(element.getElementsByTagName("transform"));
        element.getElementsByTagName("transform").namedItem(transformName).setAttribute("scale",markerData[2]);

        //set color
        var colorShaderTarget = "resources/models/marker/marker_defs.xml#Material_" + markerData[3];
        element.getElementsByTagName("assetmesh").namedItem(assetmeshName).setAttribute("shader", colorShaderTarget);

        //Set Position
        //element.getElementsByTagName("transform").namedItem(transformName).setAttribute("translation"," 15.31760025024414 85 -0.012191999703645706");
        //element.getElementsByTagName("transform").namedItem(transformName).setAttribute("rotation","0.001173883443698287 0.026653293520212173 0.009978147223591805 0.9995942711830139");
    }

    m.removeMarker = function(entity)
    {
        for (var i=0; i<FIVES.Plugins.BVHMarker.markerCollection[entity].length; i++)
        {
            var target = FIVES.Plugins.BVHMarker.markerCollection[entity][i];
            var er = FIVES.Models.EntityRegistry;
            var markerEntity = er.getEntity(target[0]);
            //console.log(target[0]);
            FIVES.Resources.SceneManager.removeEntity(markerEntity);
        }

        FIVES.Plugins.BVHMarker.markerCollection[entity] = null;
    }

    m.getParentTransformation = function(index)
    {
        var parentTransformation = mat4.create();
        parentTransformation =mat4.identity(parentTransformation);

        var joint = FIVES.Plugins.BVHSkeleton.joints[index];
        //console.log(joint.parentName);

        var jointOrderArray = [];
        var count = 0;
        while(joint.parentName != "ROOT")
        {
            //console.log(joint);
            jointOrderArray[count] = FIVES.Plugins.BVHSkeleton.getJointIndexByName(joint.parentName);

            joint = FIVES.Plugins.BVHSkeleton.joints[jointOrderArray[count]];
            count ++;
        }



        for (var i=jointOrderArray.length-1; i>=0; i--)
        {
            var index = jointOrderArray[i];
            var rotation = this.getQuaternionByIndex(index);
            var translation = this.getTranslationByIndex(index);
            var relativeJointTransformation = mat4.create();
            relativeJointTransformation	= mat4.fromRotationTranslation(relativeJointTransformation, rotation, translation);
            //console.log(index);
            //console.log(relativeJointTransformation);

            parentTransformation = mat4.multiply(parentTransformation, parentTransformation, relativeJointTransformation);
        }


        return parentTransformation;
    }

    m.getTranslationByIndex = function(index)
    {
        return vec3.fromValues(FIVES.Plugins.BVHSkeleton.translationData[index*3],FIVES.Plugins.BVHSkeleton.translationData[index*3+1],FIVES.Plugins.BVHSkeleton.translationData[index*3+2]);
    }

    m.getQuaternionByIndex = function(index)
    {
        return quat.fromValues(FIVES.Plugins.BVHSkeleton.quaternionData[index*4], FIVES.Plugins.BVHSkeleton.quaternionData[index*4+1], FIVES.Plugins.BVHSkeleton.quaternionData[index*4+2], FIVES.Plugins.BVHSkeleton.quaternionData[index*4+3]);
    }

    m.updateMarkerPos = function(entity)
    {
        var p = entity.location.position;
        var vec3Pos = vec3.fromValues(p.x, p.y, p.z);

        //console.log(p);

        for (var i=0; i<FIVES.Plugins.BVHMarker.markerCollection[entity].length; i++)
        {
            var target = FIVES.Plugins.BVHMarker.markerCollection[entity][i];
            var jointName = target[1];

            var element = FIVES.Resources.SceneManager.xml3dElement;
            var transformName = "transform-" + target[0];

            var index = FIVES.Plugins.BVHSkeleton.getJointIndexByName(jointName);

            //console.log(index);
            var absoluteParentTransformation = this.getParentTransformation(index);
            var relativePosition = vec3.create();
            relativePosition = this.getTranslationByIndex(index);
            var absolutePosition = vec3.create();
            absolutePosition = vec3.transformMat4(absolutePosition,relativePosition,absoluteParentTransformation);

            //absolutePosition = vec3.add(absolutePosition, absolutePosition, vec3Pos);

            //console.log(absolutePosition);
            //console.log(element.getElementsByTagName("transform"));
            element.getElementsByTagName("transform").namedItem(transformName).setAttribute("translation",absolutePosition[0]+" "+absolutePosition[1]+" "+absolutePosition[2]);
            //FIVES.Plugins.Location.updatePosition(target[0], {x: parseFloat(absolutePosition[0]), y: parseFloat(absolutePosition[1]), z: parseFloat(absolutePosition[2])});
        }

    }

    FIVES.Plugins.BVHMarker = new BVHMarker();
}());/**
 * Created by Oscar on 2015/4/29.
 */