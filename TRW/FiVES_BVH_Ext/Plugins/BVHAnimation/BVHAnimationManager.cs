using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventLoopPlugin;
using FIVES;

namespace BVHAnimationPlugin
{
    class AnimationInfo
    {
        public int frameTime;
        public List<string> bvhAnimationString;
        public string bvhActionString;
    }

    class BVHAnimationManager
    {
        public static BVHAnimationManager Instance;

        private string isStoppedEntity;

        private string isPausedEntity;
        private bool isPaused;

        internal BVHAnimationManager() { }

        internal void Initialize()
        {
            EventLoop.Instance.TickFired += new EventHandler<TickEventArgs>(HandleEventTick);
        }

        internal void Initialize(BVHAnimationPluginInitializer plugin)
        {
            this.animationPlugin = plugin;

            this.isStoppedEntity = "";

            this.isPausedEntity = "";
            this.isPaused = false;

            this.loopCounter = 0;
            Initialize();
        }

        private void HandleEventTick(Object sender, TickEventArgs e)
        {
            //Triggered within fixed time
            lock (RunningAnimationsForEntities)
            {
                foreach (KeyValuePair<string, AnimationInfo>
                    animatedEntity in RunningAnimationsForEntities)
                {
                    loopCounter++;
                    Entity entity = World.Instance.FindEntity(animatedEntity.Key);

                    //check if this is paused entity
                    int pausedFrame = 0;
                    if(this.isPausedEntity.Equals(entity.Guid.ToString("D")))
                    {
                        //we should not send the action string again
                        //we need to take the paused frame
                        pausedFrame = this.PausedFrameForEntities[this.isPausedEntity];
                    }
                    else
                    {
                        //otherwise we send it at the very start
                        string actionString = loopCounter.ToString() + ":" + animatedEntity.Value.bvhActionString;
                        entity["BVHAnimation"]["bvhaction"].Suggest(actionString);

                        //FIXME: we need to wait for answer, but how?
                        System.Threading.Thread.Sleep(500);
                    }

                    int frameNum = 0;
                    foreach (string frame in animatedEntity.Value.bvhAnimationString)
                    {
                        frameNum ++;

                        //firstly check if this is paused entity
                        if (pausedFrame > 0)
                        {
                            if(frameNum < pausedFrame)
                                continue;
                            else
                            {
                                //clear the paused information
                                PausedAnimationsForEntities.Remove(this.isPausedEntity);
                                this.isPaused = false;
                                PausedFrameForEntities.Remove(this.isPausedEntity);
                                this.isPausedEntity = "";
                            }
                        }

                        //Not paused
                        string sendString = frameNum.ToString() + frame;
                        //Console.WriteLine(sendString);
                        entity["BVHAnimation"]["bvhframe"].Suggest(sendString);

                        //FIXME: replace this sleep of pose-to-pose interpolation
                        System.Threading.Thread.Sleep(animatedEntity.Value.frameTime);

                        //if receive the signal of stop, just break
                        if (this.isStoppedEntity.Equals(entity.Guid.ToString("D")))
                        {
                            break;
                        }

                        //if receive the signal of pause, remember the frame number and break
                        if(this.isPausedEntity.Equals(entity.Guid.ToString("D")))
                        {
                            this.PausedFrameForEntities[this.isPausedEntity] = frameNum;

                            break;
                        }

                    }
                }
            }
        }

        public void StartAnimation(string guid, string aniName, BVHSkeleton skeleton)
        {

            //Clear the stop information
            if (this.isStoppedEntity.Equals(guid))
            {
                this.isStoppedEntity = "";
            }

            //recover the pause information, and resume
            if (this.isPausedEntity.Equals(guid))
            {
                
                //recover

                if (PausedAnimationsForEntities.ContainsKey(guid))
                {
                    //add the animation to running animation
                    lock (RunningAnimationsForEntities)
                    {
                        if (RunningAnimationsForEntities.ContainsKey(guid))
                        {
                            //Means user start a new animation, clear the paused frame
                            //PausedFrameForEntities.Remove(guid);
                        }
                        else
                        {
                            //else resume the old animation
                            RunningAnimationsForEntities.Add(guid, PausedAnimationsForEntities[guid]);

                            //and remove the old animation
                            //PausedAnimationsForEntities.Remove(guid);
                        }

                    }
                }
                else
                {
                    Console.WriteLine("No such entity has been paused!");
                }            

                return;
            }

            AnimationInfo info = new AnimationInfo();

            //get frame time
            info.frameTime = skeleton.getFrameTime();

            //get frame motion data
            info.bvhAnimationString = skeleton.GenerateBVHAnimationFrames();

            //get frame action
            info.bvhActionString = skeleton.GenerateBVHAnimationAction();

            lock (RunningAnimationsForEntities)
            {
                if (!RunningAnimationsForEntities.ContainsKey(guid))
                    RunningAnimationsForEntities[guid] = new AnimationInfo();

                //FIXME: maybe we need to check the animation name to prevent potenial double "start" click
                RunningAnimationsForEntities[guid] = info;
            }
        }

        public void PauseAnimation(string guid, string aniName)
        {
                this.isPaused = true;
                this.isPausedEntity = guid;

                lock (RunningAnimationsForEntities)
                {
                    if (RunningAnimationsForEntities.ContainsKey(guid))
                    {
                        //remember the paused animation
                        if (!PausedAnimationsForEntities.ContainsKey(guid))
                            PausedAnimationsForEntities.Add(guid, RunningAnimationsForEntities[guid]);
                        else
                        {
                            Console.WriteLine("Already has the same entity that has been paused!");
                        }
                        
                        //then remove the animation from running animation
                        RunningAnimationsForEntities.Remove(guid);
                    }
                    else
                    {
                        Console.WriteLine("No animation regarding this avatar playing yet!");
                        this.isPaused = false;
                        this.isPausedEntity = "";
                    }
                }          
        }

        public void StopAnimation(string guid, string aniName)
        {
            this.isStoppedEntity = guid;

            //if it is stopped, send the first frame and break

            Entity entity = World.Instance.FindEntity(guid);

            string firstFrame = null;

            
            if (this.isPausedEntity.Equals(guid))
            {
                //if it is paused, then we need to take the first frame from paused entity
                firstFrame = this.PausedAnimationsForEntities[guid].bvhAnimationString[0];
            }
            else
            {
                //if it is not paused
                lock (RunningAnimationsForEntities)
                {
                    if (RunningAnimationsForEntities.ContainsKey(guid))
                    {
                        firstFrame = RunningAnimationsForEntities[guid].bvhAnimationString[0];
                        RunningAnimationsForEntities.Remove(guid);
                    }
                    else
                        return;  //click more than once stop
                    
                }
            }


            //send the first frame
            int firstFrameNum = 1;
            string sendFirstString = firstFrameNum.ToString() + firstFrame;

            entity["BVHAnimation"]["bvhframe"].Suggest(sendFirstString);

            //clear the action info (already in client side)
            

            //clear the pause information
            if (this.isPausedEntity.Equals(guid))
            {
                PausedAnimationsForEntities.Remove(guid);
                this.isPaused = false;
                this.isPausedEntity = "";
                PausedFrameForEntities.Remove(guid);
            }

        }

        //entity name->List of frames, because we can only playback one animation once a time
        internal Dictionary<string, AnimationInfo> RunningAnimationsForEntities =
        new Dictionary<string, AnimationInfo>();

        internal Dictionary<string, AnimationInfo> PausedAnimationsForEntities =
        new Dictionary<string, AnimationInfo>();

        internal Dictionary<string, int> PausedFrameForEntities = new Dictionary<string, int>();

        private int loopCounter;

        BVHAnimationPluginInitializer animationPlugin;
    }
}
