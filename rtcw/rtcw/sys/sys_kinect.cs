/*
===========================================================================

Return to Castle Wolfenstein XNA Managed C# Port
Copyright (c) 2010 JV Software
Copyright (C) 1999-2010 id Software LLC, a ZeniMax Media company. 

This file is part of the Return to Castle Wolfenstein XNA Managed C# Port GPL Source Code.  

RTCW C# Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

RTCW C# Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with RTCW C# Source Code.  If not, see <www.gnu.org/licenses/>.

In addition, the RTCW SP Source Code is also subject to certain additional terms. 
You should have received a copy of these additional terms immediately following the terms 
and conditions of the GNU General Public License which accompanied the RTCW C# Source Code.  
If not, please request a copy in writing from id Software at the address below.

If you have questions concerning this license or the applicable additional terms, you may contact in writing 
id Software LLC, c/o ZeniMax Media Inc., Suite 120, Rockville, Maryland 20850 USA.

===========================================================================
*/

// sys_kinect.cs (c) 2010 JV Software
//

using xn;
using System;
using Microsoft.Xna.Framework;

using idLib.Engine.Public;
using System.Collections.Generic;

namespace rtcw.sys
{
#if WINDOWS
    //
    // idKinectDevice
    //
    struct idKinectDevice
    {
        public DepthMetaData depthMD;
        public string calibPose;
        public int[] histogram;
        public Context context;
        public DepthGenerator depth;
        public UserGenerator userGenerator;
        public SkeletonCapability skeletonCapbility;
        public PoseDetectionCapability poseDetectionCapability;
        public Dictionary<uint, Dictionary<SkeletonJoint, SkeletonJointPosition>> joints;
    };

    //
    // idSysKinect
    //
    class idSysKinect
    {
        idKinectDevice device;
        idImage debugImage;
        Color[] debugImageData;

        void Kinect_NewUser(ProductionNode node, uint id)
        {
            device.poseDetectionCapability.StartPoseDetection(device.calibPose, id);
        }

        void Kinect_LostUser(ProductionNode node, uint id)
        {
            device.joints.Remove(id);
        }

        void Kinect_CalibrationEnd(ProductionNode node, uint id, bool success)
        {
            if (success)
            {
                device.skeletonCapbility.StartTracking(id);
                device.joints.Add(id, new Dictionary<SkeletonJoint, SkeletonJointPosition>());
            }
            else
            {
                device.poseDetectionCapability.StartPoseDetection(device.calibPose, id);
            }
        }

        void Kinect_PoseDetected(ProductionNode node, string pose, uint id)
        {
            device.poseDetectionCapability.StopPoseDetection(id);
            device.skeletonCapbility.RequestCalibration(id, true);
        }

        //
        // UpdateDebugKinectImage
        //
        private unsafe void UpdateDebugKinectImage()
        {
            ushort *pDepth = (ushort *)device.depthMD.DepthMapPtr.ToPointer();
            ushort* userDepthData = (ushort*)device.userGenerator.GetUserPixels(0).SceneMapPtr.ToPointer();
            int pDest = 0;

            for (int y = 0; y < device.depthMD.YRes; ++y)
            {
                for (int x = 0; x < device.depthMD.XRes; x++, pDepth++, pDest++)
                {
                    byte pixel = (byte)device.histogram[*pDepth];
                    Color labelColor = Color.White;

                    if (userDepthData[pDest] != 0)
                    {
                        labelColor = Color.Blue;
                    }

                    debugImageData[pDest].R = (byte)(pixel * (labelColor.B / 256.0f));
                    debugImageData[pDest].G = (byte)(pixel * (labelColor.G / 256.0f));
                    debugImageData[pDest].B = (byte)(pixel * (labelColor.R / 256.0f));
                    debugImageData[pDest].A = 255;
                }
            }
        }

        //
        // CreateDebugKinectImage
        //
        private void CreateDebugKinectImage()
        {
            device.depth.GetMetaData(device.depthMD);
            debugImageData = new Color[device.depth.GetMapOutputMode().nXRes * device.depth.GetMapOutputMode().nYRes];
            UpdateDebugKinectImage();
        }

        //
        // InitKinect
        //
        public bool InitKinect()
        {
            idThread kinectThread;
            Engine.common.Printf("Init Kinect Device...\n");

            device.context = new Context("kinect/SamplesConfig.xml");

            if (device.context == null)
            {
                Engine.common.Warning("Kinect device was not detected, kinect input disabled. \n");
                return false;
            }

            device.depth = device.context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            if (device.depth == null)
            {
                Engine.common.Warning("Kinect depth device not present, kinect input disabled. \n");
                return false;
            }

            device.histogram = new int[device.depth.GetDeviceMaxDepth()];

            device.userGenerator = new UserGenerator(device.context);
            device.skeletonCapbility = new SkeletonCapability(device.userGenerator);
            device.poseDetectionCapability = new PoseDetectionCapability(device.userGenerator);
            device.calibPose = device.skeletonCapbility.GetCalibrationPose();

            device.userGenerator.NewUser += new UserGenerator.NewUserHandler(Kinect_NewUser);
            device.userGenerator.LostUser += new UserGenerator.LostUserHandler(Kinect_LostUser);
            device.poseDetectionCapability.PoseDetected += new PoseDetectionCapability.PoseDetectedHandler(Kinect_PoseDetected);
            device.skeletonCapbility.CalibrationEnd += new SkeletonCapability.CalibrationEndHandler(Kinect_CalibrationEnd);

            device.skeletonCapbility.SetSkeletonProfile(SkeletonProfile.All);
            device.joints = new Dictionary<uint, Dictionary<SkeletonJoint, SkeletonJointPosition>>();
            device.userGenerator.StartGenerating();

            device.depthMD = new DepthMetaData();

            // Create the kinect debug image
            CreateDebugKinectImage();
            

            kinectThread = Engine.Sys.CreateThread("kinectthread", () => KinectThreadWorker());
            kinectThread.Start(null);

            return true;
        }

        //
        // KinectThreadWorker
        //
        private void KinectThreadWorker()
        {
            while (true)
            {
                device.context.WaitOneUpdateAll(device.depth);
                device.depth.GetMetaData(device.depthMD);

                GetHistogram(device.depthMD);
                UpdateDebugKinectImage();
            }
        }

        //
        // Frame
        //
        public void Frame()
        {

        }

        private unsafe void GetHistogram(DepthMetaData depthMD)
        {
            // reset
            for (int i = 0; i < device.histogram.Length; ++i)
                device.histogram[i] = 0;

            ushort* pDepth = (ushort*)depthMD.DepthMapPtr.ToPointer();

            int points = 0;
            for (int y = 0; y < depthMD.YRes; ++y)
            {
                for (int x = 0; x < depthMD.XRes; ++x, ++pDepth)
                {
                    ushort depthVal = *pDepth;
                    if (depthVal != 0)
                    {
                        device.histogram[depthVal]++;
                        points++;
                    }
                }
            }

            for (int i = 1; i < device.histogram.Length; i++)
            {
                device.histogram[i] += device.histogram[i - 1];
            }

            if (points > 0)
            {
                for (int i = 1; i < device.histogram.Length; i++)
                {
                    device.histogram[i] = (int)(256 * (1.0f - (device.histogram[i] / (float)points)));
                }
            }
        }

        //
        // DrawDebug
        //
        public void DrawDebug()
        {
            if (debugImage == null)
            {
                debugImage = Engine.imageManager.CreateImage("*kinectdebug", debugImageData, device.depthMD.XRes, device.depthMD.YRes, false, false, Microsoft.Xna.Framework.Graphics.SamplerState.LinearClamp);
            }

            debugImage.BlitImageData(ref debugImageData);
            Engine.RenderSystem.DrawStrechPic(30, 30, 220, 220, debugImage);
        }
    }
#endif
}
