using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;

using Newtonsoft.Json.Linq;

using FIVES;

namespace BVHAnimationPlugin
{
    class BVHActionData
    {
        public string eventName; //attach or detach
        public string jointName;
        public string targetName;
    }
    class BVHJointData
    {
        //Skeleton data
        public Vector offsetData;
        public string channelOrder;
        public int isRoot;
        public int isEndSite;
        public string parentName;

        //Motion data
        public List<Vector> translationData;
        public List<Quat> quaternionData;
        public List<Vector> rotationData;
    }

    class BVHSkeleton
    {
        private string animationName;
        private int currentFrameNumber; //for update pose
        private string wholeFrameNumber;
        private string frameTime;
        private Dictionary<string, BVHJointData> Joints;
        private Dictionary<int, BVHActionData> ActionData;

        internal BVHSkeleton(string name)
        {
            this.animationName = name;
            Joints = new Dictionary<string, BVHJointData>();
            ActionData = new Dictionary<int,BVHActionData>();
        }

        public string getAnimationName()
        {
            return this.animationName;
        }

        private void setRootJointName(string name)
        {
            BVHJointData jointData = new BVHJointData();
            jointData.offsetData = new Vector(0, 0, 0);
            jointData.translationData = new List<Vector>();
            jointData.rotationData = new List<Vector>();
            jointData.quaternionData = new List<Quat>();
            jointData.isRoot = 1;
            jointData.isEndSite = 0;
            jointData.channelOrder = "";
            jointData.parentName = "ROOT";

            Joints.Add(name, jointData);

        }

        private void setJointName(string name, int isEndSite, string parent)
        {
            BVHJointData jointData = new BVHJointData();
            jointData.offsetData = new Vector(0, 0, 0);
            jointData.translationData = new List<Vector>();
            jointData.rotationData = new List<Vector>();
            jointData.quaternionData = new List<Quat>();
            jointData.isRoot = 0;
            jointData.isEndSite = isEndSite;
            jointData.channelOrder = "";
            jointData.parentName = parent;

            Joints.Add(name, jointData);
        }

        private void setRootChannel(string name, string c1, string c2, string c3, string c4, string c5, string c6)
        {
            Joints[name].channelOrder = c1 + c2 + c3 + c4 + c5 + c6;
        }

        private void setJointChannel(string name, string c1, string c2, string c3)
        {
            Joints[name].channelOrder = c1 + c2 + c3;
        }

        private void setJointOffset(string name, string x, string y, string z)
        {
            Joints[name].offsetData = new Vector(float.Parse(x, CultureInfo.InvariantCulture), float.Parse(y, CultureInfo.InvariantCulture), float.Parse(z, CultureInfo.InvariantCulture));
        }


        private void checkSkeletonHeader()
        {
            foreach (KeyValuePair<string, BVHJointData> joint in Joints)
            {
                System.Console.WriteLine(joint.Key);
                System.Console.WriteLine(joint.Value.channelOrder);
                System.Console.WriteLine(joint.Value.offsetData.x + "," + joint.Value.offsetData.y + "," + joint.Value.offsetData.z);
                System.Console.ReadLine();
            }
            
        }

        private void checkMotionData()
        {
//             for (int i = 0; i < 10; i++)
//             {
//                 Quat q = Joints["Hips"].quaternionData[i];
//                 Vector t = Joints["Hips"].translationData[i];
//                 System.Console.WriteLine("Hips translation: " + t.x + "," + t.y + "," + t.z);
//                 System.Console.WriteLine("Hips Quaternion: " + q.x + "," + q.y + "," + q.z + "," + q.w);
//                 System.Console.ReadLine();
//             }
            for (int i = 0; i < int.Parse(this.wholeFrameNumber, CultureInfo.InvariantCulture); i++)
            {
                foreach (KeyValuePair<string, BVHJointData> joint in Joints)
                {
                    Console.WriteLine(joint.Value.quaternionData[0].x);
                }
            }

        }

        private void setFrameQuaternion(string jointName, string x, string y, string z)
        {

            Quat q = new Quat(0, 0, 0, 1);
            Vector xAxis = new Vector(1, 0, 0);
            Vector yAxis = new Vector(0, 1, 0);
            Vector zAxis = new Vector(0, 0, 1);

            Vector rotData = new Vector(0, 0, 0);

            double angle1 = System.Math.PI * double.Parse(x, CultureInfo.InvariantCulture) / 180.0;
            double angle2 = System.Math.PI * double.Parse(y, CultureInfo.InvariantCulture) / 180.0;
            double angle3 = System.Math.PI * double.Parse(z, CultureInfo.InvariantCulture) / 180.0;

            if (Joints[jointName].channelOrder.Contains("XrotationYrotationZrotation")) //XYZ
            {
                q = FIVES.Math.QuaternionFromAxisAngle(xAxis, (float)angle1);
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(yAxis, (float)angle2));
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(zAxis, (float)angle3));
                rotData = new Vector((float)angle1, (float)angle2, (float)angle3);
            }
            else if (Joints[jointName].channelOrder.Contains("XrotationZrotationYrotation")) //XZY
            {
                q = FIVES.Math.QuaternionFromAxisAngle(xAxis, (float)angle1);
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(zAxis, (float)angle2));
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(yAxis, (float)angle3));
                rotData = new Vector((float)angle1, (float)angle3, (float)angle2);
            }
            else if (Joints[jointName].channelOrder.Contains("YrotationXrotationZrotation")) //YXZ
            {
                q = FIVES.Math.QuaternionFromAxisAngle(yAxis, (float)angle1);
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(xAxis, (float)angle2));
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(zAxis, (float)angle3));
                rotData = new Vector((float)angle2, (float)angle1, (float)angle3);
            }
            else if (Joints[jointName].channelOrder.Contains("YrotationZrotationXrotation")) //YZX
            {
                q = FIVES.Math.QuaternionFromAxisAngle(yAxis, (float)angle1);
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(zAxis, (float)angle2));
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(xAxis, (float)angle3));
                rotData = new Vector((float)angle2, (float)angle3, (float)angle1);
            }
            else if (Joints[jointName].channelOrder.Contains("ZrotationYrotationXrotation")) //ZYX
            {
                q = FIVES.Math.QuaternionFromAxisAngle(zAxis, (float)angle1);
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(yAxis, (float)angle2));
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(xAxis, (float)angle3));
                rotData = new Vector((float)angle3, (float)angle2, (float)angle1);
            }
            else if (Joints[jointName].channelOrder.Contains("ZrotationXrotationYrotation")) //ZXY
            {
                q = FIVES.Math.QuaternionFromAxisAngle(zAxis, (float)angle1);
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(xAxis, (float)angle2));
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(yAxis, (float)angle3));
                rotData = new Vector((float)angle3, (float)angle1, (float)angle2);
            }

            //Console.WriteLine("quaternion: " + q.x + "," + q.y + "," + q.z + "," + q.w);
            //Console.ReadLine();

            Joints[jointName].quaternionData.Add(q);
            Joints[jointName].rotationData.Add(rotData);

        }

        private string getParentName(Dictionary<string, int> jointOrderCollection, int index)
        {
            foreach (var item in jointOrderCollection.Reverse())
            {
                if (item.Value.Equals(index))
                {
                    return item.Key;
                }
            }
            return String.Empty;
        }

        private void readAndProcessMotionData(string[] motionData)
        {
            //It is special for human body skeleton： normally only root joint has 6 channels, other joints only have 3 channels
            int count = 0;

            //Console.WriteLine(motionData.Length);
            
            foreach (KeyValuePair<string, BVHJointData> joint in Joints)
            {
                if (joint.Value.isRoot == 1)
                {
                    Vector trans = new Vector(float.Parse(motionData[count], CultureInfo.InvariantCulture), float.Parse(motionData[count+1], CultureInfo.InvariantCulture), float.Parse(motionData[count+2], CultureInfo.InvariantCulture));
                    Vector offset = joint.Value.offsetData;

                    //TODO: I use default translation channel order(XYZ), but it is in fact wrong 
                    Joints[joint.Key].translationData.Add(FIVES.Math.AddVectors(trans, offset));
                    count += 3;

                    this.setFrameQuaternion(joint.Key, motionData[count], motionData[count + 1], motionData[count + 2]);
                    count += 3;
                                       
                }
                else if (joint.Value.isEndSite == 1)
                {
                    //Ignore it..
                }
                else // isRoot ==0, isEndSite == 0
                {
                    Vector trans = new Vector(0, 0, 0);
                    Vector offset = joint.Value.offsetData;

                    Joints[joint.Key].translationData.Add(FIVES.Math.AddVectors(trans, offset));

                    this.setFrameQuaternion(joint.Key, motionData[count], motionData[count + 1], motionData[count + 2]);
                    count += 3;
                }
            }

            //Console.WriteLine("Count: " + count);
        }

        public string GenerateBVHAnimationAction()
        {
            string actionString = "";

            foreach (KeyValuePair<int, BVHActionData> a in this.ActionData)
            {
                actionString += a.Key.ToString();
                actionString += ",";
                actionString += a.Value.eventName;
                actionString += ",";
                actionString += a.Value.jointName;
                actionString += ",";
                actionString += a.Value.targetName;
                actionString += ",";
                actionString += ":";
            }

            return actionString;
        }

        public List<string> GenerateBVHAnimationFrames()
        {
            //String Format: "translation","root trans.x","root trans.y","root trans.z","quaternion","q1.x","q1.y","q1.z","q1.w","q2.x",...
            List<string> frames = new List<string>();

            string bvhHeaderString = ":" + GenerateBVHHeaderString() + ":";

            //Console.WriteLine(this.wholeFrameNumber);
            int numberOfFrames = (int)float.Parse(this.wholeFrameNumber, CultureInfo.InvariantCulture);
            for (int i = 0; i < numberOfFrames; i++)
            {
                string singleFrame = "";

                foreach (KeyValuePair<string, BVHJointData> joint in Joints)
                {
                    if (joint.Value.isRoot == 1)
                    {
                        singleFrame += "translation,";
                        singleFrame += joint.Value.translationData[i].x.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                        singleFrame += joint.Value.translationData[i].y.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                        singleFrame += joint.Value.translationData[i].z.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                        singleFrame += "quaternion,";
                    }

                    if (joint.Value.isEndSite == 0)
                    {
                        singleFrame += joint.Value.quaternionData[i].x.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                        singleFrame += joint.Value.quaternionData[i].y.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                        singleFrame += joint.Value.quaternionData[i].z.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                        singleFrame += joint.Value.quaternionData[i].w.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                    }

                    //else: end site, ignore it.

                    //Console.WriteLine(singleFrame);
                    //Console.ReadLine();
                }                
                frames.Add(bvhHeaderString + singleFrame);
            }

            return frames;
        }

        public string GenerateCurrentBVHAnimationFrames()
        {

                int i = this.currentFrameNumber;
                string singleFrame = "";

                foreach (KeyValuePair<string, BVHJointData> joint in Joints)
                {
                    if (joint.Value.isRoot == 1)
                    {
                        singleFrame += "translation,";
                        singleFrame += joint.Value.translationData[i].x.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                        singleFrame += joint.Value.translationData[i].y.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                        singleFrame += joint.Value.translationData[i].z.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                        singleFrame += "quaternion,";
                    }

                    if (joint.Value.isEndSite == 0)
                    {
                        singleFrame += joint.Value.quaternionData[i].x.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                        singleFrame += joint.Value.quaternionData[i].y.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                        singleFrame += joint.Value.quaternionData[i].z.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                        singleFrame += joint.Value.quaternionData[i].w.ToString("R").Replace(",", ".");
                        singleFrame += ",";
                    }
                }
                return singleFrame;
        }

        public string GenerateBVHHeaderString()
        {
            //String Format: "Animation Name","Joint1 Name","Joint1 Parent Name", "Joint1 Offset.x","Joint1 Offset.y","Joint1 Offset.z","Joint2 Name"...
            string headerString = animationName + ",";

            foreach (KeyValuePair<string, BVHJointData> joint in Joints)
            {
                if (joint.Value.isEndSite == 0) //Only send non-endsite joint
                {
                    headerString += joint.Key;
                    headerString += ",";
                    headerString += joint.Value.parentName;
                    headerString += ",";
                    headerString += joint.Value.offsetData.x.ToString("R").Replace(",", ".");
                    headerString += ",";
                    headerString += joint.Value.offsetData.y.ToString("R").Replace(",", ".");
                    headerString += ",";
                    headerString += joint.Value.offsetData.z.ToString("R").Replace(",", ".");
                    headerString += ",";
                }

            }

            return headerString;
        }

        public int getFrameTime()
        {
            Console.WriteLine("try to read the frame time");
            return (int)(float.Parse(frameTime, CultureInfo.InvariantCulture) * 1000);
        }

        public void LoadBVHFile()
        {
            //It stores the joint name and its order, we can get each joints' parent by sorting it.
            Dictionary<string, int> jointNameOrder = new Dictionary<string,int>();

            string line;
            System.IO.StreamReader file = null;
            string filename = "../../Plugins/BVHAnimation/additionalfile/" + this.animationName;
            // Read the file and display it line by line.
            try
            {
                file = new System.IO.StreamReader(filename);
            }

            catch(System.IO.IOException e)
            {
                Console.WriteLine("No such BVH file: " + filename);
            }

            string LastJointName = "";

            //read skeleton data
            while ((line = file.ReadLine()) != null)
            {
                char[] splitSeparators = new char[] { ' ', '\t' };
                string[] LineStrings = line.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries);

                //foreach (string character in LineStrings)
                //{
                    //System.Console.WriteLine(character);

                    //System.Console.ReadLine();
                //}
                if (LineStrings.Length > 0)
                {

                    string LineLabel = LineStrings[0];
                    if (LineLabel.Contains("HIERARCHY"))
                    {
                        //Enter the skeleton definition, simply skip it.
                    }
                    else if (LineLabel.Contains("ROOT"))
                    {
                        this.setRootJointName(LineStrings[1]);
                        jointNameOrder.Add(LineStrings[1], 0); //Root is parent of all joints
                        LastJointName = LineStrings[1];
                    }
                    else if (LineLabel.Contains("JOINT"))
                    {
                        int count = line.TakeWhile(Char.IsWhiteSpace).Count();
                        string parentName = this.getParentName(jointNameOrder, count - 1);
                        jointNameOrder.Add(LineStrings[1], count);
                    
                        //Console.WriteLine("White Space Count:" + LineStrings[1] + " has " + count);
                        this.setJointName(LineStrings[1], 0, parentName); //para1: joint name, para2: is end site?
                        LastJointName = LineStrings[1];

                    }
                    else if (LineLabel.Contains("End"))
                    {
                        //We ignore it, because it does not exist in XML3D skeleton definition
                        //But still, it has offset data
                        int count = line.TakeWhile(Char.IsWhiteSpace).Count();
                        string parentName = this.getParentName(jointNameOrder, count - 1);
                        string endsiteName = LastJointName + "EndSite";
                        jointNameOrder.Add(endsiteName, count);
                        this.setJointName(endsiteName, 1, parentName); //para1: joint name, para2: is end site?
                        //Console.WriteLine("White Space Count:" + endsiteName + " has " + count);
                        LastJointName = endsiteName;
                    }
                    else if (LineLabel.Contains("CHANNELS"))
                    {
                        if (LineStrings[1] == "6")
                        {
                            this.setRootChannel(LastJointName, LineStrings[2], LineStrings[3], LineStrings[4], LineStrings[5], LineStrings[6], LineStrings[7]);
                        }
                        else if (LineStrings[1] == "3")
                        {
                             this.setJointChannel(LastJointName, LineStrings[2], LineStrings[3], LineStrings[4]);
                        }
                        else
                        {
                            System.Console.WriteLine("Error: BVHReader doesn't support this channel number, terminate..");
                            System.Console.ReadLine();
                            System.Environment.Exit(1);
                        }
                    }
                    else if (LineLabel.Contains("OFFSET"))
                    {
                        this.setJointOffset(LastJointName, LineStrings[1], LineStrings[2], LineStrings[3]);
                    }
                    else if (LineLabel.Contains("MOTION"))
                    {
                        //break this loop and read the motion part
                        break;
                    }
                }
            }

            //read motion data
            int frameCount = 0;
            while ((line = file.ReadLine()) != null)
            {
                
                char[] splitSeparators = new char[] { ' ', '\t' };
                string[] LineStrings = line.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries);
                string LineLabel = LineStrings[0];
                //0Console.WriteLine(frameCount + "Label: " + LineLabel);

                if (LineLabel == null)
                {
                    break;
                }


                if (LineLabel.Contains("Frames"))
                {
                    this.wholeFrameNumber = LineStrings[1];
                }
                else if (LineLabel.Contains("Frame")) //May be other way??
                {
                    this.frameTime = LineStrings[2];
                }
                else
                {
                    //for motion part
                    //Console.WriteLine("Label: " + LineLabel);
                    this.readAndProcessMotionData(LineStrings);
                    frameCount++; //TODO: check this number with this.wholeFrameNumber
                }
            }
            //Console.WriteLine("Frame Count: " + frameCount);
            
            if (frameCount != (float.Parse(this.wholeFrameNumber, CultureInfo.InvariantCulture)))
            {
                Console.WriteLine("Motion part is not read completely, exit!");
                Console.ReadLine();
                System.Environment.Exit(1);
            }
            else
            {
                //Console.WriteLine("Verified that the motion was read correctly");
                //Console.WriteLine( float.Parse(this.wholeFrameNumber, CultureInfo.InvariantCulture));
            }
            //this.checkSkeletonHeader();
            //this.checkMotionData();

            file.Close();
            //End of BVH file load

            //System.Console.WriteLine("There were {0} lines.", counter);
            // Suspend the screen.
            //System.Console.ReadLine();
        }

        public void configAnimationParameter(string revString)
        {
            char[] splitSeparators = new char[] { ',', '\t' };
            string[] LineStrings = revString.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries);
            int count = 0;
            List<string> animationParameters = new List<string>();
            foreach (string s in LineStrings)
            {
                if (count == 0)
                {
                    if (s != "start")
                        throw new Exception("BVHSkeleton: Start Animation:Wrong config string");
                }
                else
                {
                    animationParameters.Add(s);
                }
                count++;
                //Console.WriteLine(s);
            }

            if (animationParameters.Count == 0)//No additioal control parameters
            {
                return;
            }

            if (animationParameters[0] == "action") //Read action json script
            {
                readAndProcessActionScript(animationParameters[1]);
            }
            else
            {
                for (int i = 0; i < animationParameters.Count; i += 3)
                {
                    readAndProcessAdditionalJointControl(animationParameters[i], animationParameters[i + 1], animationParameters[i + 2]);
                }
            }

        }

        public void configUpdateParameter(string revString)
        {
            char[] splitSeparators = new char[] { ',', '\t' };
            string[] LineStrings = revString.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries);
            int count = 0;
            List<string> updateParameters = new List<string>();
            foreach (string s in LineStrings)
            {
                if (count == 0)
                {
                    if (s != "update")
                        throw new Exception("BVHSkeleton: Update:Wrong config string");
                }
                else
                {
                    updateParameters.Add(s);
                }
                count++;
                //Console.WriteLine(s);
            }

            if (updateParameters.Count % 3 != 0 )
                throw new Exception("BVHSkeleton: wrong number of update parameters");

            for (int i = 0; i < updateParameters.Count; i = i + 3)
            {
                updateJointValue(updateParameters[i], updateParameters[i+1], updateParameters[i+2]);
            }
        }

        private void updateJointValue(string jointName, string axis, string value)
        {
            //Fix current frame number to 0, because every time we will reload the update BVH
            this.currentFrameNumber = 0;

            int index = this.currentFrameNumber;
            Vector rotation = FIVES.Math.AxisFromQuaternion(this.Joints[jointName].quaternionData[index]);
            float angle = (float)(System.Math.PI * float.Parse(value, CultureInfo.InvariantCulture) / 180);
            if (axis == "x")
                applyAdditionalJointControl(jointName, index, angle, rotation.y, rotation.z);
            if (axis == "y")
                applyAdditionalJointControl(jointName, index, rotation.x, angle, rotation.z);
            if (axis == "z")
                applyAdditionalJointControl(jointName, index, rotation.x, rotation.y, angle);

        }

        private void applyAdditionalJointControl(string jointName, int index, float x, float y, float z)
        {
            Quat q = new Quat(0, 0, 0, 1);
            Vector xAxis = new Vector(1, 0, 0);
            Vector yAxis = new Vector(0, 1, 0);
            Vector zAxis = new Vector(0, 0, 1);

            if (Joints[jointName].channelOrder.Contains("XrotationYrotationZrotation")) //XYZ
            {
                q = FIVES.Math.QuaternionFromAxisAngle(xAxis, x);
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(yAxis, y));
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(zAxis, z));
            }
            else if (Joints[jointName].channelOrder.Contains("XrotationZrotationYrotation")) //XZY
            {
                q = FIVES.Math.QuaternionFromAxisAngle(xAxis, x);
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(zAxis, z));
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(yAxis, y));
            }
            else if (Joints[jointName].channelOrder.Contains("YrotationXrotationZrotation")) //YXZ
            {
                q = FIVES.Math.QuaternionFromAxisAngle(yAxis, y);
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(xAxis, x));
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(zAxis, z));
            }
            else if (Joints[jointName].channelOrder.Contains("YrotationZrotationXrotation")) //YZX
            {
                q = FIVES.Math.QuaternionFromAxisAngle(yAxis, y);
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(zAxis, z));
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(xAxis, x));
            }
            else if (Joints[jointName].channelOrder.Contains("ZrotationYrotationXrotation")) //ZYX
            {
                q = FIVES.Math.QuaternionFromAxisAngle(zAxis, z);
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(yAxis, y));
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(xAxis, x));
            }
            else if (Joints[jointName].channelOrder.Contains("ZrotationXrotationYrotation")) //ZXY
            {
                q = FIVES.Math.QuaternionFromAxisAngle(zAxis, z);
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(xAxis, x));
                q = FIVES.Math.MultiplyQuaternions(q, FIVES.Math.QuaternionFromAxisAngle(yAxis, y));
            }

            Joints[jointName].quaternionData[index] = q;
        }

        private void readAndProcessActionScript(string actionName)
        {
            actionName = "../../Plugins/BVHAnimation/additionalfile/" + actionName;

            JObject o = JObject.Parse(System.IO.File.ReadAllText(actionName));

            foreach (KeyValuePair<string, JToken> property in o)
            {
                BVHActionData data = new BVHActionData();
                //Console.WriteLine(property.Key);
                string eventValue = (string)o[property.Key][0]["event"];
                data.eventName = eventValue;
                //Console.WriteLine(eventValue);
                string jointValue = (string)o[property.Key][0]["parameters"]["joint"];
                data.jointName = jointValue;
                //Console.WriteLine(jointValue);
                string targetValue = (string)o[property.Key][0]["parameters"]["target"];
                data.targetName = targetValue;
                //Console.WriteLine(targetValue);

                if (!this.ActionData.ContainsKey(int.Parse((string)property.Key)))
                    this.ActionData.Add(int.Parse((string)property.Key), data);
            }
            
        }

        private void readAndProcessAdditionalJointControl(string fileName, string jointName, string axisName)
        {
            //Console.WriteLine(fileName + jointName + axisName);
            System.IO.StreamReader file = null;
            fileName = "../../Plugins/BVHAnimation/additionalfile/" + fileName;
            string line = null;
            bool readDataflag = false;
            List<float> data = new List<float>();
            try
            {
                file = new System.IO.StreamReader(fileName);
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("No such Additional Joint Control file: " + fileName);
            }

            do
            {
                line = file.ReadLine();

                if (line == null)
                {
                    break;
                }

                if (line == String.Empty)
                {
                    continue;
                }

                //char[] splitSeparators = new char[] { ' ', '\t' };
                //string[] LineStrings = line.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries);

                if (readDataflag)
                {
                    data.Add(float.Parse(line.Remove(0, 1), CultureInfo.InvariantCulture));
                    //Console.WriteLine(float.Parse(line.Remove(0, 1)));
                }

                if (line.Contains("Dumping"))
                    readDataflag = true;

                //Console.WriteLine(line);

                // Here you process the non-empty line

            } while (true);

            file.Close();

            if (this.Joints.ContainsKey(jointName))
            {
                for (int index = 0; index <this.Joints[jointName].quaternionData.Count; index ++)
                {
                    Vector rotation = FIVES.Math.AxisFromQuaternion(this.Joints[jointName].quaternionData[index]);
                    float angle = (float)(System.Math.PI * data[index] / 180);
                    if (axisName == "x")
                        applyAdditionalJointControl(jointName, index, angle, rotation.y, rotation.z);
                    if (axisName == "y")
                        applyAdditionalJointControl(jointName, index, rotation.x, angle, rotation.z);
                    if (axisName == "z")
                        applyAdditionalJointControl(jointName, index, rotation.x, rotation.y, angle);
                }
            }
            else
            {
                throw new Exception("Additional Control: No such joint name");
            }


            //Console.ReadLine();
                    
        }
    }
}
