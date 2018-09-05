Graphics and Interaction Project 1 - Fractal Heightmap 
Description of our implementation

Modelling of Fractal Lanscape 
For the implementation of the of the Fractal Lanscape, we used the Diamond-Square Algorithm to generate the terrain. We started off with an empty object and placed an algorithmic script on it. We took inspiration from the Lectures and YouTube tutorials e.g. (https://www.youtube.com/watch?v=1HV8GbFnCik&frags=pl%2Cwn)
We first generate the an array of Vertices (figure 1) for laying out the sqaure (2 triangles in clockwise motion). Then, we lay out the terrain of specified size and then perform the Diamond-Square Algorithm in order to give the terrain details. The Diamond-Square Algorithm (Diamond Step and Square Step) (https://en.wikipedia.org/wiki/Diamond-square_algorithm) is performed over the number of said details.

Figure 1. Vertices Example 
v1 ######## v2
# #          #
#   #        #
#      #     #
#         #  #
v4 ######## v3

Figure 2. Diamond-Square 
c1 #### v1 #### c2
#                #
#                #
v4      m       v2
#                #
#                #
c4 #### v3 #### c3

Camera Motion
For the implementation of the Camera Motion, we first create our Main Camera object in Unity and added a CameraController script to it. We also added Sphere Collider and Rigid Body Component to the Camera Object, in order to maintain the "flight stimulator" style view. Also, in our CameraController, we used Unity's framework to obtain User Inputs for the Camera Movements and determine the moving speed and camera rotating speed in aiding the smooth transition of the camera view. Lastly, we set the Terrain a Box Collider so that the camera isn't allowed to go through the terrain and we scripted the terrain's out of bound coordinates so that the Camera Object is limited to the area within the parameters of the terrain. 

Surface Properties 
For the water shader, we added transparency to Render Queue to make the water semi-transparent. For the rest, we followed the tutorials. As for the actual Waves, we took an empty plane and attached a water script to it that displaces vertices depending on the specified number of details. 
For the terrain shader, the majority of the work was inspired from our tutorials, but in order for our terrain to look realistic (changing colours during night and day) we tweaked (decreased) both Ka and Ks so that the Ambient light reflects the night/day cycle and the terrain isn't as shiny as it would be from the Specular reflection. As for colouring the terrain, we implemented 4 different high levels (Snow, Mountain Higher, Mountain Lower, and Sand) that has both colour and height threshold, and used lerp to render a linear gradient effect for the terrain. 
Lastly, we implemented the Sun by creating a Direction Light object and attached it to an Sphere Object (Sun) as a child. We also created a Sun Material and connect it to the Sun Object as part of it's Component. Later on, We attached the SunController script to the Sun Object to rotate the Sun at the edge of the terrain and maintain it's focus on to the center of the terrain. As for the Direction Light, we attached a Point Light script so that it dynamically changes the point light angle at every frame. 
