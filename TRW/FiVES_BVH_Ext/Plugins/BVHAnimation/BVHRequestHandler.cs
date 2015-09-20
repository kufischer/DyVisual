using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

using RESTServicePlugin;

using FIVES;

namespace BVHAnimationPlugin
{
    class BVHRequestHandler : RequestHandler
    {

        private string path;
        private string contentType;
        private bool debugMode;

        public BVHRequestHandler(string path, string contentType)
        {
            this.debugMode = false;
            this.path = path;
            this.contentType = contentType;
        }

        public override string Path
        {
            get
            {
                return this.path;
            }
        }

        public override string ContentType
        {
            get
            {
                return this.contentType;
            }
        }

        private JObject JsonParser(string json)
        {
            JObject o = JObject.Parse(json);
            if (this.debugMode)
            {
                foreach (JProperty property in o.Properties())
                {
                    Console.WriteLine(property.Name + " - " + property.Value);
                }
            }
            return o;
        }
        private void DeleteMarker(string content, int request)
        {
            BVHAnimationPluginInitializer.Instance.DeleteMarker(content, 0);
        }

        private void AddMarker(string content, int request)
        {
            BVHAnimationPluginInitializer.Instance.AddMarker(content, 0);
        }

        private void ApplyServiceRequest(JObject o, int request)
        {
            string avatarID = null;
            string aniName = null;
            //string targetAvatar = null;
            string configString = null;

            if (o["avatarID"] != null)
            {
                //Console.WriteLine("anName " + o["aniName"]);
                avatarID = o["avatarID"].ToString();
            }

            if (o["aniName"] != null)
            {
                aniName = o["aniName"].ToString();
            }

//             if (o["targetAvatarURI"] != null)
//             {
//                 targetAvatar = o["targetAvatarURI"].ToString();
//             }

            if (o["configString"] != null)
            {
                configString = o["configString"].ToString();
            }

            //find the Guid through avatar ID
            if (!BVHAnimationPluginInitializer.Instance.avatarCollection.ContainsKey(avatarID))
            {
                Console.WriteLine("ApplyServiceRequest: No Such avatar ID");
                return;
            }

            string avatarGUID = BVHAnimationPluginInitializer.Instance.avatarCollection[avatarID].entity;

            if (request == 1) //Start 
                BVHAnimationPluginInitializer.Instance.StartAnimation(avatarGUID, aniName, configString, 0);
            else if (request == 2) //Stop
                BVHAnimationPluginInitializer.Instance.StopAnimation(avatarGUID, aniName, configString, 0);
            else if (request == 3) //Update
                BVHAnimationPluginInitializer.Instance.UpdateAnimation(avatarGUID, aniName, configString, 0);
            else
                throw new NotSupportedException("No such request");
        }

        private void StartBVHAnimationPipeline(JObject o, int request)
        {
            string avatarID = null;
            string aniName = null;
            string aniContent = null;
            //string targetAvatar = null;
            string actionName = null;
            string actionContent = null;

            string configString = null;

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
//             if (o["targetAvatarURI"] != null)
//             {
//                 targetAvatar = o["targetAvatarURI"].ToString();
//             }
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

            //find the Guid through avatar ID
            if (!BVHAnimationPluginInitializer.Instance.avatarCollection.ContainsKey(avatarID))
            {
                Console.WriteLine("StartBVHAnimationPipeline: No Such avatar ID");
                return;
            }

            string avatarGUID = BVHAnimationPluginInitializer.Instance.avatarCollection[avatarID].entity;

            //start 
            BVHAnimationPluginInitializer.Instance.StartAnimation(avatarGUID, aniName, configString, 0);
        }

        private void CreateAnimationEntity(string content, int request)
        {
            BVHAnimationPluginInitializer.Instance.CreateAnimationEntity(content, 0);
        }

        private void SetAnimationEntity(string content, int request)
        {
            BVHAnimationPluginInitializer.Instance.SetAnimationEntity(content);
        }

        private void SetAnimationConstraints(string content, int request)
        {
            BVHAnimationPluginInitializer.Instance.SetAnimationConstraints(content);
        }

        protected override RequestResponse HandlePOST(string requestPath, string content)
        {
            //Console.WriteLine("handle post");
            RequestResponse response = new RequestResponse();

            //response.ReturnCode = 200;

            //Console.WriteLine(requestPath);

            //Console.WriteLine(content);

            JObject jObject = null;
            if (!content.Equals(""))
            {
                jObject = JsonParser(content);
            }

            if (requestPath.Equals("/StartAnimation"))
            {
                ApplyServiceRequest(jObject, 1); //Start Animation

                response.ReturnCode = 200;
            }
            else if (requestPath.Equals("/StopAnimation"))
            {
                ApplyServiceRequest(jObject, 2); //Start Animation
                response.ReturnCode = 200;
            }
            else if (requestPath.Equals("/UpdatePose"))
            {
                ApplyServiceRequest(jObject, 3); //Start Animation
                response.ReturnCode = 200;
            }
            else if (requestPath.Equals("/AddMarker"))
            {
                AddMarker(content, 4); //Add marker
                response.ReturnCode = 200;
            }
            else if (requestPath.Equals("/DeleteMarker"))
            {
                DeleteMarker(content, 5); //Add marker
                response.ReturnCode = 200;
            }
            else if (requestPath.Equals("/StartAnimationPipeline"))
            {
                StartBVHAnimationPipeline(jObject, 5); //Add marker
                response.ReturnCode = 200;
            }
            else if (requestPath.Equals("/CreateAnimationEntity"))
            {
                CreateAnimationEntity(content, 6); //Add marker
                response.ReturnCode = 200;
            }
            else if (requestPath.Equals("/SetAnimationEntity"))
            {
                SetAnimationEntity(content, 7); //Add marker
                response.ReturnCode = 200;
            }
            else if (requestPath.Equals("/SetAnimationConstraints"))
            {
                SetAnimationConstraints(content, 8); //Add marker
                response.ReturnCode = 200;
            }

            return response;
        }

        protected override RequestResponse HandleDELETE(string requestPath)
        {
            throw new NotImplementedException();
        }

        protected override RequestResponse HandleGET(string requestPath)
        {
            throw new NotImplementedException();
        }

        protected override RequestResponse HandlePUT(string requestPath, string content)
        {
            throw new NotImplementedException();
        }
    }
}
