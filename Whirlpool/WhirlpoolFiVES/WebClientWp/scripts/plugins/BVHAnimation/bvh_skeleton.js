var FIVES = FIVES || {};
FIVES.Plugins = FIVES.Plugins || {};

(function (){
    "use strict";

    var JointStructure = function(){
            this.BVHName = "";
            this.XML3DName = "";
            this.updateIndex = -1;
            this.parentName = "";

        };
    var j = JointStructure.prototype;

    var BVHSkeleton = function() {
        //TODO: Define a name of skeleton
        //this.name = skeletonName;

        this.AnimationName = "";

        this.joints = new Array(42);

        //Definition of joints mapping from BVH to XML3D, skinning "male"
        for (var i=0; i<42; i++)
        {
            this.joints[i] = new JointStructure();
        }

        this.joints[0].BVHName = "Hips";
        this.joints[0].XML3DName = "Root";

        this.joints[1].BVHName = "None";
        this.joints[1].XML3DName = "Hips";

        this.joints[2].BVHName = "None";
        this.joints[2].XML3DName = "Spine";

        this.joints[3].BVHName = "Spine";
        this.joints[3].XML3DName = "Spine1";

        this.joints[4].BVHName = "Spine_1";
        this.joints[4].XML3DName = "Spine2";

        this.joints[5].BVHName = "Neck";
        this.joints[5].XML3DName = "Neck";

        this.joints[6].BVHName = "Head";
        this.joints[6].XML3DName = "Head";

        this.joints[7].BVHName = "None";
        this.joints[7].XML3DName = "Jaw";

        this.joints[8].BVHName = "None";
        this.joints[8].XML3DName = "LOuterEyebrow";

        this.joints[9].BVHName = "None";
        this.joints[9].XML3DName = "ROuterEyebrow";

        this.joints[10].BVHName = "None";
        this.joints[10].XML3DName = "LEyeBlinkTop";

        this.joints[11].BVHName = "None";
        this.joints[11].XML3DName = "REyeBlinkTop";

        this.joints[12].BVHName = "None";
        this.joints[12].XML3DName = "LMouthCorner";

        this.joints[13].BVHName = "None";
        this.joints[13].XML3DName = "RMouthCorner";

        this.joints[14].BVHName = "LeftShoulder";
        this.joints[14].XML3DName = "LeftShoulder";

        this.joints[15].BVHName = "LeftArm";
        this.joints[15].XML3DName = "LeftArm";

        this.joints[16].BVHName = "LeftForeArm";
        this.joints[16].XML3DName = "LeftForeArm";

        this.joints[17].BVHName = "LeftHand";
        this.joints[17].XML3DName = "LeftHand";

        //TODO: we have 15 joints in BVH but only 6 joints in XML3D
        this.joints[18].BVHName = "Bip01_L_Finger0";
        this.joints[18].XML3DName = "LeftHandThumb1";

        this.joints[19].BVHName = "Bip01_L_Finger01";
        this.joints[19].XML3DName = "LeftHandThumb2";

        this.joints[20].BVHName = "Bip01_L_Finger1";
        this.joints[20].XML3DName = "LeftHandIndex1";

        this.joints[21].BVHName = "Bip01_L_Finger11";
        this.joints[21].XML3DName = "LeftHandIndex2";

        this.joints[22].BVHName = "Bip01_L_Finger2";
        this.joints[22].XML3DName = "LeftHandMiddle1";

        this.joints[23].BVHName = "Bip01_L_Finger21";
        this.joints[23].XML3DName = "LeftHandMiddle2";

        this.joints[24].BVHName = "RightShoulder";
        this.joints[24].XML3DName = "RightShoulder";

        this.joints[25].BVHName = "RightArm";
        this.joints[25].XML3DName = "RightArm";

        this.joints[26].BVHName = "RightForeArm";
        this.joints[26].XML3DName = "RightForeArm";

        this.joints[27].BVHName = "RightHand";
        this.joints[27].XML3DName = "RightHand";

        this.joints[28].BVHName = "Bip01_R_Finger0";
        this.joints[28].XML3DName = "RightHandThumb1";

        this.joints[29].BVHName = "Bip01_R_Finger01";
        this.joints[29].XML3DName = "RightHandThumb2";

        this.joints[30].BVHName = "Bip01_R_Finger1";
        this.joints[30].XML3DName = "RightHandIndex1";

        this.joints[31].BVHName = "Bip01_R_Finger12";
        this.joints[31].XML3DName = "RightHandIndex2";

        this.joints[32].BVHName = "Bip01_R_Finger2";
        this.joints[32].XML3DName = "RightHandMiddle1";

        this.joints[33].BVHName = "Bip01_R_Finger21";
        this.joints[33].XML3DName = "RightHandMiddle2";

        this.joints[34].BVHName = "LeftUpLeg";
        this.joints[34].XML3DName = "LeftUpLeg";

        this.joints[35].BVHName = "LeftLeg";
        this.joints[35].XML3DName = "LeftLeg";

        this.joints[36].BVHName = "LeftFoot";
        this.joints[36].XML3DName = "LeftFoot";

        this.joints[37].BVHName = "Bip01_L_Toe0";
        this.joints[37].XML3DName = "LeftToeBase";

        this.joints[38].BVHName = "RightUpLeg";
        this.joints[38].XML3DName = "RightUpLeg";

        this.joints[39].BVHName = "RightLeg";
        this.joints[39].XML3DName = "RightLeg";

        this.joints[40].BVHName = "RightFoot";
        this.joints[40].XML3DName = "RightFoot";

        this.joints[41].BVHName = "Bip01_R_Toe0";
        this.joints[41].XML3DName = "RightToeBase";

        //Initialize translation data filled with [0 0 0] and quaternion data with [0 0 0 1]
        this.translationData = new Float32Array(42*3);


        this.quaternionData = new Float32Array(42*4);

        //Initialize translation data
        for (var i=0; i<this.translationData.length; i++){
            this.translationData[i] = 0;
        }

        //Initialize quaternion data
        for (var i=0; i<this.quaternionData.length; i++){
            if ((i+1) % 4 == 0)
            {
                this.quaternionData[i] = 1;
            }
            else
            {
                this.quaternionData[i] = 0;
            }
        }

        //For fixing broken joints, copy from male-shared.json init_translation and init_rotation
        //If fix Spine (Joint 2), there would be problem with the position calculation of marker

        //this.translationData[2*3] = 15.316900253295898;
        //this.translationData[2*3+1] = 0;
        //this.translationData[2*3+2] = -0.012191999703645706;

        this.translationData[7*3] = -0.7462459802627563;
        this.translationData[7*3+1] = 0;
        this.translationData[7*3+2] = 2.5000150203704834;

        this.translationData[8*3] = 12.541560173034668;
        this.translationData[8*3+1] = -4.926715850830078;
        this.translationData[8*3+2] = 8.292945861816406;

        this.translationData[9*3] = 12.541560173034668;
        this.translationData[9*3+1] = 4.926668167114258;
        this.translationData[9*3+2] = 8.29296875;

        this.translationData[10*3] = 11.867799758911133;
        this.translationData[10*3+1] = -3.3358590602874756;
        this.translationData[10*3+2] = 8.439620971679688;

        this.translationData[11*3] = 11.867830276489258;
        this.translationData[11*3+1] = 3.3358099460601807;
        this.translationData[11*3+2] = 8.43963623046875;

        this.translationData[12*3] = 4.012382984161377;
        this.translationData[12*3+1] = -4;
        this.translationData[12*3+2] = 9.899999618530273;

        this.translationData[13*3] = 4.012382984161377;
        this.translationData[13*3+1] = 4;
        this.translationData[13*3+2] = 10.5;

        this.quaternionData[2*4] = 0.001173883443698287;
        this.quaternionData[2*4+1] = 0.026653293520212173;
        this.quaternionData[2*4+2] = 0.009978147223591805;
        this.quaternionData[2*4+3] = 0.9995942711830139;

        this.quaternionData[7*4] = -0.011826924979686737;
        this.quaternionData[7*4+1] = -0.6167266368865967;
        this.quaternionData[7*4+2] = -0.03630763664841652;
        this.quaternionData[7*4+3] = 0.786250650882721;

        this.quaternionData[12*4] = -0.3119528293609619;
        this.quaternionData[12*4+1] = 0.6539729833602905;
        this.quaternionData[12*4+2] = 0.34043678641319275;
        this.quaternionData[12*4+3] = -0.5992558002471924;

        this.quaternionData[13*4] = -0.07013167440891266;
        this.quaternionData[13*4+1] = 0.8361760973930359;
        this.quaternionData[13*4+2] = 0.11008467525243759;
        this.quaternionData[13*4+3] = -0.5327029228210449;

        //init update index
        for (var i=0; i<42; i++)
        {
            this.joints[i].updateIndex = -1;
        }

        //init update index
        for (var i=0; i<42; i++)
        {
            this.joints[i].parentName = "";
        }

    };

    var s = BVHSkeleton.prototype;

    s.initPose = function(header){
        //console.log(header);

        //update the index of joints
        var nameString = "";
        var updateIndexCount = 0;
        var headerDataString = [];

        for (var i=0; i<header.length; i++)
        {
            if (header[i] != ",")
            {
                nameString += header[i];
            }
            else{
                //console.log(nameString);

                updateIndexCount++;

                if (updateIndexCount == 1)
                {
                    //Animation Name
                    this.AnimationName = nameString;
                }
                else
                {
                    //Joint Name + Parent Name + offset data
                    headerDataString.push(nameString);
                }
                nameString = "";
            }
        }

        //console.log(headerDataString);

        updateIndexCount = 0;

        for (var i=0; i<headerDataString.length; i+=5)
        {
            var index = this.updateIndexByName(headerDataString[i], updateIndexCount);

            if(index >= 0)
            {
                this.joints[index].parentName = headerDataString[i+1];
                var transIndex = index * 3;
                this.translationData[transIndex] = parseFloat(headerDataString[i+2]);
                this.translationData[transIndex+1] = parseFloat(headerDataString[i+3]);
                this.translationData[transIndex+2] = parseFloat(headerDataString[i+4]);
            }

            updateIndexCount++;
        }
        //console.log(this.joints);
        //console.log(this.translationData);
    }

    s.getJointIndexByName = function(name)
    {
        for (var i=0; i<this.joints.length; i++)
        {
            if (this.joints[i].BVHName == name)
            {
                return i;
            }
        }
        //console.warn("WARNING: Can not find corresponding BVH name for: ", name);

        return -1;
    }

    s.updateIndexByName = function(name, index)
    {
        for (var i=0; i<this.joints.length; i++)
        {
            if (this.joints[i].BVHName == name)
            {
                this.joints[i].updateIndex = index;

                return i;
            }
        }

        //console.warn("WARNING: Can not find corresponding BVH name for: ", name);

        return -1;
    }

    s.updatePose = function(bvhframe){

        var nameString = "";
        var transCount = 0;
        var transDataString = [];
        var transFlag = 0;
        var quaternionCount = 0;
        var quaternionFlag = 0;
        var quaternionDataString = [];
        for (var i=0; i<bvhframe.length; i++)
        {
            if (bvhframe[i] != ",")
            {
                nameString += bvhframe[i];
            }
            else{

                //var value = parseFloat(nameString);
                //console.log(value);
                if (transFlag)
                {
                    transDataString[transCount] = nameString;
                    transCount ++;
                }

                if(quaternionFlag)
                {
                    quaternionDataString[quaternionCount] = nameString;
                    quaternionCount ++;
                }

                if (nameString == "translation")
                {
                    transFlag = 1;
                    quaternionFlag = 0;
                }

                if(nameString == "quaternion")
                {
                    quaternionFlag = 1;
                    transFlag = 0;
                }

                nameString = "";
            }
        }

        //update translation, now we only have root
        this.translationData[0] = parseFloat(transDataString[0]);
        this.translationData[1] = parseFloat(transDataString[1]);
        this.translationData[2] = parseFloat(transDataString[2]);

        //update quaternion
        for (var i=0; i<this.joints.length; i++)
        {
            var index = this.joints[i].updateIndex;

            if(index >= 0)
            {
                //we only consider the joint that is included in BVH file
                this.quaternionData[i*4] = parseFloat(quaternionDataString[index*4]);
                this.quaternionData[i*4+1] = parseFloat(quaternionDataString[index*4+1]);
                this.quaternionData[i*4+2] = parseFloat(quaternionDataString[index*4+2]);
                this.quaternionData[i*4+3] = parseFloat(quaternionDataString[index*4+3]);
            }
        }

        //console.log(this.translationData);
        //console.log(this.quaternionData);

    }

    FIVES.Plugins.BVHSkeleton = new BVHSkeleton();


}());/**
 * Created by Oscar on 2015/4/29.
 */
