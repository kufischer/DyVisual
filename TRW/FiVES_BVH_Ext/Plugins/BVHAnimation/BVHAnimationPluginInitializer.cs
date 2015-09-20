using System;
using FIVES;
using System.Collections.Generic;
using ClientManagerPlugin;

using Newtonsoft.Json.Linq;

using System.IO;

namespace BVHAnimationPlugin
{
    class AvatarInfo
    {
        public string entity;
        public string animationName;
        public string actionName;
        public string meshURI;
        public string configString;
    }

    public class BVHAnimationPluginInitializer : IPluginInitializer
    {
        #region IPluginInitializer implementation

        public string Name
        {
            get
            {
                return "BVHAnimation";
            }
        }

        public List<string> PluginDependencies
        {
            get { return new List<string> { "EventLoop", "ClientManager" }; }
        }

        public List<string> ComponentDependencies
        {
            get
            {
                return new List<string>();
            }
        }

        public void Initialize()
        {
            DefineComponents();
            RegisterClientServices();

            RESTServicePlugin.RequestDispatcher.Instance.RegisterHandler(new BVHRequestHandler("/BVH", "application/json"));

            BVHAnimationPluginInitializer.Instance = new BVHAnimationPluginInitializer();

            BVHAnimationManager.Instance = new BVHAnimationManager();
            BVHAnimationManager.Instance.Initialize(this);
        }

        public void Shutdown()
        {
        }

        #endregion

        public static BVHAnimationPluginInitializer Instance;

        internal Dictionary<string, BVHSkeleton> animationskeletons = new Dictionary<string, BVHSkeleton>();
        internal Dictionary<string, BVHSkeleton> updateskeletons = new Dictionary<string, BVHSkeleton>();

        internal Dictionary<string, List<Entity>> markerCollection = new Dictionary<string, List<Entity>>();

        //Avatar ID -> Animation name
        internal Dictionary<string, AvatarInfo> avatarCollection = new Dictionary<string, AvatarInfo>();

        internal List<Entity> constraintsPointCollection = new List<Entity>();

        void DefineComponents()
        {
            ComponentDefinition BVHAnimation = new ComponentDefinition("BVHAnimation");
            BVHAnimation.AddAttribute<string>("bvhheader");
            BVHAnimation.AddAttribute<string>("bvhframe");
            BVHAnimation.AddAttribute<string>("bvhmarker");
            BVHAnimation.AddAttribute<string>("bvhaction");
            ComponentRegistry.Instance.Register(BVHAnimation);
        }

        void RegisterClientServices()
        {
            string BVHAnimationIdl = File.ReadAllText("BVHAnimation.kiara");
            SINFONIPlugin.SINFONIServerManager.Instance.SinfoniServer.AmendIDL(BVHAnimationIdl);

            ClientManager.Instance.RegisterClientService("BVHAnimation", true, new Dictionary<string, Delegate> {
                {"startAnimation", (Action<string, string, string, int>) StartAnimation},
                {"stopAnimation", (Action<string, string, string, int>) StopAnimation},
                {"updateAnimation", (Action<string, string, string, int>) UpdateAnimation},
                {"createAnimationEntity", (Action<string, int>) CreateAnimationEntity},
                {"addMarker", (Action<string, int>) AddMarker},
                {"deleteMarker", (Action<string, int>) DeleteMarker},
                {"startAnimationINTERACT", (Action<string, int>) StartAnimationINTERACT},
                {"stopAnimationINTERACT", (Action<string, int>) StopAnimationINTERACT},
                {"pauseAnimationINTERACT", (Action<string, int>) PauseAnimationINTERACT},
                {"toggleConstraintsINTERACT", (Action<int>) ToggleConstraintsINTERACT},
            });
        }

        public void ToggleConstraintsINTERACT(int timestamp)
        {
            if (BVHAnimationPluginInitializer.Instance.constraintsPointCollection.Count > 0)
            {
                foreach (Entity e in BVHAnimationPluginInitializer.Instance.constraintsPointCollection)
                {
                    if (e["mesh"]["visible"].Value.Equals(false))
                        e["mesh"]["visible"].Suggest(true);
                    else
                        e["mesh"]["visible"].Suggest(false);
                }
            }
        }

        public void PauseAnimationINTERACT(string avatarID, int timestamp)
        {
            avatarCollection = BVHAnimationPluginInitializer.Instance.avatarCollection;
            if (!avatarCollection.ContainsKey(avatarID))
            {
                Console.WriteLine("PauseAnimationINTERACT: Error, no such avatar ID.");
                return;
            }

            if (avatarCollection[avatarID].animationName == null)
            {
                Console.WriteLine("PauseAnimationINTERACT: Error, no such animation name");
                return;
            }

            PauseAnimation(avatarCollection[avatarID].entity, avatarCollection[avatarID].animationName, avatarCollection[avatarID].configString, timestamp);
        }

        public void StopAnimationINTERACT(string avatarID, int timestamp)
        {
            avatarCollection = BVHAnimationPluginInitializer.Instance.avatarCollection;
            if (!avatarCollection.ContainsKey(avatarID))
            {
                Console.WriteLine("StopAnimationINTERACT: Error, no such avatar ID.");
                return;
            }

            if (avatarCollection[avatarID].animationName == null)
            {
                Console.WriteLine("StopAnimationINTERACT: Error, no such animation name");
                return;
            }

            StopAnimation(avatarCollection[avatarID].entity, avatarCollection[avatarID].animationName, avatarCollection[avatarID].configString, timestamp);
        }

        public void StartAnimationINTERACT(string avatarID, int timestamp)
        {
            avatarCollection = BVHAnimationPluginInitializer.Instance.avatarCollection;

            if (!avatarCollection.ContainsKey(avatarID))
            {
                Console.WriteLine("StartAnimationINTERACT: Error, no such avatar ID.");
                return;
            }

            if (avatarCollection[avatarID].animationName == null)
            {
                Console.WriteLine("StartAnimationINTERACT: Error, no such animation name");
                return;
            }

            StartAnimation(avatarCollection[avatarID].entity, avatarCollection[avatarID].animationName, avatarCollection[avatarID].configString, timestamp);
        }

        private void HandleInteractInputJson(string content)
        {
            JObject o = JObject.Parse(content);

            string aniName = null;
            string aniContent = null;
            string targetAvatar = null;
            string actionName = null;
            string actionContent = null;

            string configString = null;

            string avatarID = null;

            if (o["avatarID"] != null)
            {
                //Console.WriteLine("anName " + o["aniName"]);
                avatarID = o["avatarID"].ToString();
            }

            if (o["aniName"] != null)
            {
                //Console.WriteLine("anName " + o["aniName"]);
                aniName = o["aniName"].ToString();
            }
            if (o["aniContent"] != null)
            {
                aniContent = o["aniContent"].ToString();
            }
            if (o["targetAvatarURI"] != null)
            {
                targetAvatar = o["targetAvatarURI"].ToString();
            }
            if (o["actionName"] != null)
            {
                actionName = o["actionName"].ToString();
            }
            if (o["actionContent"] != null)
            {
                actionContent = o["actionContent"].ToString();
            }
            if (o["configString"] != null)
            {
                configString = o["configString"].ToString();
            }

            //Write animation to file
            string aniPath = "../../Plugins/BVHAnimation/additionalfile/" + aniName;
            System.IO.File.WriteAllText(aniPath, aniContent);

            //Write action to file
            string actionPath = "../../Plugins/BVHAnimation/additionalfile/" + actionName;
            System.IO.File.WriteAllText(actionPath, actionContent);


            if (avatarCollection.ContainsKey(avatarID))
            {
                //update the new info
                AvatarInfo a = new AvatarInfo();
                a.actionName = actionName;
                a.animationName = aniName;
                a.meshURI = targetAvatar;
                a.configString = configString;
                a.entity = avatarCollection[avatarID].entity;
                BVHAnimationPluginInitializer.Instance.avatarCollection[avatarID] = a;
            }
            else
            {
                Console.WriteLine("SetAnimationEntity: No such avatar");
            }
        }

        public void SetAnimationEntity(string content)
        {
            //Console.WriteLine("Get in here");
            //string inputpath = "../../Plugins/BVHAnimation/additionalfile/" + inputName;
            //JObject o = JObject.Parse(System.IO.File.ReadAllText(inputpath));

            HandleInteractInputJson(content);
        }

        public void SetAnimationConstraints(string content)
        {
            //if there is existed constraint points, first delete them
            if (constraintsPointCollection.Count > 0)
            {
                foreach (Entity e in constraintsPointCollection)
                {
                    if (!World.Instance.Remove(e))
                        Console.WriteLine("Delete existing constraint points failed!");
                }

                constraintsPointCollection.Clear();
            }

            //Console.WriteLine(content);

            JObject o = JObject.Parse(content);

            string aniConstranitsName = null;
            string aniConstranitsContent = null;
            string trajectoryMarkerURL = null;
            string keyframeMarkerURL = null;

            if (o["aniConstranitsName"] != null)
            {
                //Console.WriteLine("anName " + o["aniName"]);
                aniConstranitsName = o["aniConstranitsName"].ToString();
            }

            if (o["aniConstranitsContent"] != null)
            {
                //Console.WriteLine("anName " + o["aniName"]);
                aniConstranitsContent = o["aniConstranitsContent"].ToString();
            }

            if (o["trajectoryMarkerURL"] != null)
            {
                //Console.WriteLine("anName " + o["aniName"]);
                trajectoryMarkerURL = o["trajectoryMarkerURL"].ToString();
            }

            if (o["keyframeMarkerURL"] != null)
            {
                //Console.WriteLine("anName " + o["aniName"]);
                keyframeMarkerURL = o["keyframeMarkerURL"].ToString();
            }

            //handle constraint json
            JObject c = JObject.Parse(aniConstranitsContent);

            if (c["startPose"] != null)
            {
                float x = 0.0f;
                float y = 0.0f;
                float z = 0.0f;

                if ((string)c["startPose"]["position"][0] != null)
                    x = (float)c["startPose"]["position"][0];
                if ((string)c["startPose"]["position"][1] != null)
                    y = (float)c["startPose"]["position"][1];
                if ((string)c["startPose"]["position"][2] != null)
                    z = (float)c["startPose"]["position"][2];

                Entity newAvatar = new Entity();
                newAvatar["mesh"]["uri"].Suggest(trajectoryMarkerURL);
                newAvatar["mesh"]["visible"].Suggest(true);
                newAvatar["location"]["position"].Suggest(new Vector(x, y, z));
                World.Instance.Add(newAvatar);

                constraintsPointCollection.Add(newAvatar);
            }

            //for constraint point
            if (c["tasks"] != null)
            {
                foreach (var task in c["tasks"])
                {
                    foreach (var elementaryActions in task["elementaryActions"])
                    {
                        foreach (var constraints in elementaryActions["constraints"])
                        {
                            if (constraints["trajectoryConstraints"] != null)
                            {
                                foreach (var trajectoryConstraints in constraints["trajectoryConstraints"])
                                {

                                    float x = 0.0f;
                                    float y = 0.0f;
                                    float z = 0.0f;

                                    if ((string)trajectoryConstraints["position"][0] != null)
                                        x = (float)trajectoryConstraints["position"][0];
                                    if ((string)trajectoryConstraints["position"][1] != null)
                                        y = (float)trajectoryConstraints["position"][1];
                                    if ((string)trajectoryConstraints["position"][2] != null)
                                        z = (float)trajectoryConstraints["position"][2];

                                    //Console.WriteLine(x);
                                    //Console.WriteLine(y);
                                    //Console.WriteLine(z);

                                    Entity newAvatar = new Entity();
                                    newAvatar["mesh"]["uri"].Suggest(trajectoryMarkerURL);
                                    newAvatar["mesh"]["visible"].Suggest(true);
                                    newAvatar["location"]["position"].Suggest(new Vector(x, y, z));
                                    World.Instance.Add(newAvatar);

                                    constraintsPointCollection.Add(newAvatar);
                                }
                            }
                            if (constraints["keyframeConstraints"] != null)
                            {
                                foreach (var keyframeConstraints in constraints["keyframeConstraints"])
                                {
                                    float x = 0.0f;
                                    float y = 0.0f;
                                    float z = 0.0f;

                                    if ((string)keyframeConstraints["position"][0] != null)
                                        x = (float)keyframeConstraints["position"][0];
                                    if ((string)keyframeConstraints["position"][1] != null)
                                        y = (float)keyframeConstraints["position"][1];
                                    if ((string)keyframeConstraints["position"][2] != null)
                                        z = (float)keyframeConstraints["position"][2];

                                    //Console.WriteLine(x);
                                    //Console.WriteLine(y);
                                    //Console.WriteLine(z);

                                    Entity newAvatar = new Entity();
                                    newAvatar["mesh"]["uri"].Suggest(keyframeMarkerURL);
                                    newAvatar["mesh"]["visible"].Suggest(true);
                                    newAvatar["location"]["position"].Suggest(new Vector(x, y, z));
                                    World.Instance.Add(newAvatar);

                                    constraintsPointCollection.Add(newAvatar);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("ConstraintModel.Json reader: Wrong format!");
                return;
            }

            

        }

        public void CreateAnimationEntity(string content, int timestamp)
        {
            //Console.WriteLine("Get in here");
            //string inputpath = "../../Plugins/BVHAnimation/additionalfile/" + inputName;
            //JObject o = JObject.Parse(System.IO.File.ReadAllText(inputpath));

            //HandleInteractInputJson(inputpath, avatarID);

            JObject o = JObject.Parse(content);

            string avatarID = null;
            if (o["avatarID"] != null)
            {
                //Console.WriteLine("anName " + o["aniName"]);
                avatarID = o["avatarID"].ToString();
            }
            string targetAvatar = null;
            if (o["targetAvatarURI"] != null)
            {
                targetAvatar = o["targetAvatarURI"].ToString();
            }
            float x = 0.0f;
            float y = 0.0f;
            float z = 0.0f;

            if (o["position"] != null)
            {
                if ((string)o["position"][0] != null)
                    x = (float)o["position"][0];
                if ((string)o["position"][1] != null)
                    y = (float)o["position"][1];
                if ((string)o["position"][2] != null)
                    z = (float)o["position"][2];
            }

            if (!avatarCollection.ContainsKey(avatarID))
            {
                //Create avatar and add it to world
                Entity newAvatar = new Entity();
                newAvatar["mesh"]["uri"].Suggest(targetAvatar);
                newAvatar["mesh"]["visible"].Suggest(true);
                newAvatar["location"]["position"].Suggest(new Vector(x, y, z));
                World.Instance.Add(newAvatar);

                //Create the new info
                AvatarInfo a = new AvatarInfo();
                a.actionName = null;
                a.animationName = null;
                a.meshURI = targetAvatar;
                a.configString = null;
                a.entity = newAvatar.Guid.ToString("D");
                //Console.WriteLine(a.entity);
                BVHAnimationPluginInitializer.Instance.avatarCollection.Add(avatarID, a);

                //Console.WriteLine("avatarID:" + avatarID);
            }

        }

        public void DeleteMarker(string content, int timestamp)
        {
            JObject o = JObject.Parse(content);
            string avatarID = null;

            if (o["avatarID"] != null)
            {
                //Console.WriteLine("anName " + o["aniName"]);
                avatarID = o["avatarID"].ToString();
            }

            //find the Guid through avatar ID
            if (!BVHAnimationPluginInitializer.Instance.avatarCollection.ContainsKey(avatarID))
            {
                Console.WriteLine("DeleteMarker: No Such avatar ID");
                return;
            }

            string avatarGUID = BVHAnimationPluginInitializer.Instance.avatarCollection[avatarID].entity;

            var entity = World.Instance.FindEntity(avatarGUID);
            entity["BVHAnimation"]["bvhmarker"].Suggest("Delete");

            foreach (Entity e in BVHAnimationPluginInitializer.Instance.markerCollection[avatarGUID])
            {
                if (!World.Instance.Remove(e))
                    Console.WriteLine("Delete marker:" + e.Guid.ToString("D") + " failed!");
            }

            BVHAnimationPluginInitializer.Instance.markerCollection.Remove(avatarGUID);
        }

        public void AddMarker(string content, int timestamp)
        {
            JObject o = JObject.Parse(content);

            string avatarID = null;
            string jointName = null;
            string diameter = null;
            string targetMarker = null;
            //string targetAvatar = null;
            string color = null;

            if (o["avatarID"] != null)
            {
                //Console.WriteLine("anName " + o["aniName"]);
                avatarID = o["avatarID"].ToString();
            }

            if (o["jointName"] != null)
            {
                jointName = o["jointName"].ToString();
            }
            if (o["diameter"] != null)
            {
                diameter = o["diameter"].ToString();
            }
            if (o["targetMarkerURI"] != null)
            {
                targetMarker = o["targetMarkerURI"].ToString();
            }
            //             if (o["targetAvatarURI"] != null)
            //             {
            //                 targetAvatar = o["targetAvatarURI"].ToString();
            //             }
            if (o["color"] != null)
            {
                color = o["color"].ToString();
            }

            //find the Guid through avatar ID
            if (!BVHAnimationPluginInitializer.Instance.avatarCollection.ContainsKey(avatarID))
            {
                Console.WriteLine("AddMarker: No Such avatar ID");
                return;
            }

            string avatarGUID = BVHAnimationPluginInitializer.Instance.avatarCollection[avatarID].entity;

            var entity = World.Instance.FindEntity(avatarGUID);

            Entity newAvatar = new Entity();
            newAvatar["mesh"]["uri"].Suggest(targetMarker);
            newAvatar["mesh"]["visible"].Suggest(true);
            newAvatar["location"]["position"].Suggest(new Vector(-150, 100, 0));
            World.Instance.Add(newAvatar);
            if (!BVHAnimationPluginInitializer.Instance.markerCollection.ContainsKey(avatarGUID))
            {
                List<Entity> markers = new List<Entity>();
                BVHAnimationPluginInitializer.Instance.markerCollection.Add(avatarGUID, markers);
            }
            BVHAnimationPluginInitializer.Instance.markerCollection[avatarGUID].Add(newAvatar);

            entity["BVHAnimation"]["bvhmarker"].Suggest(newAvatar.Guid.ToString("D") + "," + jointName + "," + diameter + "," + color + ",");
        }

        public void StartAnimation(string guid, string animationName, string configString, int timestamp)
        {
            var entity = World.Instance.FindEntity(guid);

            if (!animationskeletons.ContainsKey(animationName))
            {
                Console.WriteLine("Try to load bvh file " + animationName);
                BVHSkeleton skeleton = new BVHSkeleton(animationName);
                skeleton.LoadBVHFile();
                Console.WriteLine("Finished reading the file");
                animationskeletons.Add(animationName, skeleton);
            }

            //Console.WriteLine("added animation to ist");
            animationskeletons[animationName].configAnimationParameter(configString);
            //Console.WriteLine("Finished reading the config string");
            //Console.WriteLine("Get in here!");
            BVHAnimationManager.Instance.StartAnimation(guid, animationName, animationskeletons[animationName]);
        }

        public void UpdateAnimation(string guid, string animationName, string configString, int timestamp)
        {
            var entity = World.Instance.FindEntity(guid);

            //if (!updateskeletons.ContainsKey(animationName))
            //{
            //    BVHSkeleton skeleton = new BVHSkeleton(animationName);
            //    skeleton.LoadBVHFile();
            //    updateskeletons.Add(animationName, skeleton);
            //}
            //updateskeletons[animationName].configUpdateParameter(configString);

            //string bvhHeaderString = ":" + updateskeletons[animationName].GenerateBVHHeaderString() + ":";
            //string frame = updateskeletons[animationName].GenerateCurrentBVHAnimationFrames();
            //string sendString = bvhHeaderString + frame;

            BVHSkeleton skeleton = new BVHSkeleton(animationName);
            skeleton.LoadBVHFile();
            skeleton.configUpdateParameter(configString);

            string bvhHeaderString = ":" + skeleton.GenerateBVHHeaderString() + ":";
            string frame = skeleton.GenerateCurrentBVHAnimationFrames();
            string sendString = bvhHeaderString + frame;

            entity["BVHAnimation"]["bvhframe"].Suggest(sendString);

            //BVHAnimationManager.Instance.StartAnimation(guid, skeleton.getAnimationName(), skeleton);
        }

        public void StopAnimation(string guid, string animationName, string revString, int timestamp)
        {
            BVHAnimationManager.Instance.StopAnimation(guid, animationName);              
        }

        public void PauseAnimation(string guid, string animationName, string revString, int timestamp)
        {
            BVHAnimationManager.Instance.PauseAnimation(guid, animationName);
        }
    }
}
