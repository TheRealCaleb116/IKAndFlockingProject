---
layout: default
---
<div id="HeaderPics">

 <img src="./assets/img/IKsimulation.PNG" alt=""> 
 <img src="./assets/img/BOIDS.PNG" alt=""> 
 
</div>

# 2D CCDIK Simulation and Flocking and Pathing Simulation - Caleb Wiebolt

Below is the write-up for my CCD IK Project which also includes a separate Flocking simulation that used BOIDS and extends the algorithm to incorporate collision avoidance in order to simulate a basic goal-based pathing system as well. Both of these simulations were created in Unity for my Animation and Planning in Games class. All the graded features that I attempted can be found below. To look at the source code or a pre-built executable click the button below. 

<a href="{{ site.github.repository_url }}" class="btn btn-dark">Go to the Code</a>



## Features Attempted
### Showcase Video


{% include video.html %}


### Feature List

| Feature                           | Description       | TimeCode |
|:-------------                     |:------------------|:------|
| Multi Arm IK          | My IK has two arms connected together part way through the armature and rooted at a singular point. | 0:06-0:45  |
| Joint Limits       | Joints on the armature can be limited to certain angles of movement. | 0:45-1:55   |
| User Interaction |  In the IK simulation the end effector and the obstacles can both be moved. In the BOIDS sim, new BOIDS can be created on demands and a goal can be set. | Throughout the Video |
| IK Obstacles |  In the IK simulation there is an obstacle that can prevent the arms from reaching their targets. | 1:55-3:52 |
| Flocking   | I implemented the BOIDS algorithm to create natural flocking and group movement. | 4:00-6:38   |
| 3D Rendering & Camera | The BOIDS simulation is rendered in 3D with a moving camera, textures for the ground and obstacles, and dynamic lighting as the BOIDS move around. | 4:00-The End |


In addition to the features listed above, I extended the BOIDS simulation to include obstacle avoidance and goal-based pathing. Since this wasn't necessarily a requested feature in the write-up I was unsure of how to categorize it. It is showcased in the video after 6:40 and at the very least is an interesting application of the algorithm.



## Tools and Libraries Used
*   Unity 2022.3.9f1 and Visual Studio
*   I referenced this paper when creating my own obstacle avoidance addon for the BOIDS algorithm, while not a direct implementation many of my ideas came from <a href="https://www.hindawi.com/journals/jam/2014/659805/">Here.</a>

## Assets Used
* 2D targets Sprite by <a href="https://assetstore.unity.com/packages/3d/animations/2d-targets-sprites-142142">Elizabeth Studio</a> on the unity asset store.
* <a href="https://skfb.ly/6RusL">Oil barrel Textured Game assets</a> by Dinesh Naidu is licensed under Creative Commons Attribution.
* Coast Land Rocks 1 Textures and Material by <a href="https://polyhaven.com/a/coast_land_rocks_01">Rico Cilliers and Rob Tuytel</a>


## Difficulties Encountered
This project was fun and difficult. I started my IK simulation in 3D using quaternions to represent rotation. While I got almost every part of that working, I spent hours trying to figure out how to implement angular constraints with that representation. I then tried to change the way I was representing the rotation at each joint in order to figure out how to constrain the range of motion. After many hours spent on something that felt so close and with some prompting from the sunk cost fallacy I pressed on. Eventually, the deadline was looming and I realized that figuring everything out in time wouldn't happen. I then refactored everything into the 2D simulation you see now and added collision to make up for the points.

Despite the large amount of frustration this caused me, I do feel that refactoring and redesigning this system multiple times has given me a much deeper understanding of how the CCDIK model works. In addition to that I created the BOIDs sim. It was a blast to implement and I was intrigued by what the professor had mentioned in class about BOIDS and collision avoidance. I was curious if I could get decent single and multi-agent navigation style behavior without a more complicated nav system. I think the results, while not perfect, are really good for the amount of work I put into it. 
