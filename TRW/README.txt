1. Start the FiVES server

(1.1) There is pre-compiled application(VS2012 + 64bit Windows 8) located in "TO_PATH\\magnifi\FiVES_BVHPlayer\\FiVES\Binaries\Release\FIVES.exe", or you can follow the intruction of FiVES's website (https://github.com/fives-team/FiVES) to rebuild the project. 

(1.2) run FIVES.exe to start the FiVES server

2. Start the FiVES web client

(2.1) Locate the webclient directory into web service directory(e.g. WAMP or JetBrain IDE)

(2.2) Open client.xhtml in browser

3. Usage of web client

(3.1) You can directly sign in without Login name and Password, after that you will see the Avatar standing back to you.

4. Usage of REST service

(4.1) Install DHC 0.8.1(or any other REST client) as the extension of Chrome, and if you choose DHC 0.8.1, you can import the script of "DHC_Script/DHC_BVHAnimation.json" in DHC, this json contains some example script.

(4.2) SEND those scripts to excute different commands. e.g. you can start the "carrying" animation by excuting "Start", and send "Stop" to pause the animation.

(4.3) Parameter settings of scripts in DHC are as follows:

	 (1) localhost:8081/BVH/UpdateBVHPose: Modify the specific joint angle.

		"avatarID": the ID of avatar created by the interface "CreateAnimationEntity"

		"aniName": the bvh file name locating in "TO_PATH\magnifi\FiVES_BVHPlayer\FiVES\Binaries\Release\additionalfile"

		"configString": A config string specify the modification of euler angle of each joint. For example, input "update,Spine,y,-30" means you want to modify the value of Joint Spine's y axis in dummy.bvh to -30 degree. All possible joint name you can find in your .bvh file. The string can be extended to any number of joint, e.g. "update,Spine,y,-30,RightForeArm,y,50" will make the joint "Spine" and joint "RightForeArm" move to the demanded angle at the same time.
	
	(2)  localhost:8081/BVH/StartBVHAnimation: Play a .bvh animation

		"avatarID": the ID of avatar created by the interface "CreateAnimationEntity"

		"aniName": the bvh file name locating in "TO_PATH\magnifi\FiVES_BVHPlayer\FiVES\Binaries\Release\additionalfile"

		"configString": A config string to signal the animation player. For example, "start," means to play the current animation.

	(3) localhost:8081/BVH/StopBVHAnimation: Stop the current animation

		"avatarID": the ID of avatar created by the interface "CreateAnimationEntity"

		"aniName": the bvh file name locating in "TO_PATH\magnifi\FiVES_BVHPlayer\FiVES\Binaries\Release\additionalfile"

		"configString": IGNORED

	(4) localhost:8081/BVH/AddMarker: Add a customized ball-like marker attached to avatar

		"avatarID": the ID of avatar created by the interface "CreateAnimationEntity"

		"targetMarkerURI": target marker locating in "TO_PATH\magnifi\FiVES_BVHPlayer\FiVES\WebClient", the default marker is a ball

		"jointName": the joint name of avatar. All possible joint name you can find in your .bvh file.

		"diameter": the scale of your marker, you can scale it up or down in three different dimensions.

		"color": the color of marker, supports "red", "blue", "green", "yellow", "orange".

	(5) localhost:8081/BVH/DeleteMarker: Delete all the markers of target avatar.

		"avatarID": the ID of avatar created by the interface "CreateAnimationEntity"

	(6) localhost:8081/BVH/CreateAnimationEntity: Create an initial avatar of corresponding resource located in "targetAvatarURI".

		"avatarID": the ID of avatar to be created
	
		"targetAvatarURI": the resources of avatar located in "TO_PATH\magnifi\FiVES_BVHPlayer\FiVES\WebClient"

5. An example to add and delete marker 

(5.1) After importing the DHC script, you can some pre-written sample json script in the left under the label of "BVHAnimation", an exmaple of adding and deleting marker could be:

	(1) Start FiVES server side, start FiVES client side, and open DHC

	(2) Excute "CreateAnimationEntity" script in DHC, click send (green button in the right side), you will see a avatar created in client browser

	(2) Excute "InitPose" script in DHC, you will see the default avatar standing up with initial pose.

	(3) Click "AddMarker" script in DHC, change the "jointName", "diameter" and "color" you want, you will see a customized marker adding to avatar after you send your request.

	(4) Excute "Start" script in DHC, you will see avatar moving along with marker.

	(5) Excute "Stop" script to stop the avatar.

	(6) Excute "DeleteMarker" script to delete all the markers of current avatar. You can all the markers attached to this avatar will be removed.

