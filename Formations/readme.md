##	What did you use for obstacle avoidance?
I used a combination of Raycast as well as some Unity NavAgent behaviors.

##	What are the heuristics for the birds to detect the tunnels and reshape after going through them?
I decided to go with an arrowhead formation for simplicity. I cast two rays like a cone check from the leader along the two columns of the formation. If there are any collisions (i.e. tunnels, sharp walls), that means the formation is too wide, and therefore the angle of the arrow formation is modified.  

##	Did you use any additional heuristics?
N/A

##	What are the differences in the two groups’ performances?
Although both perform similarly in this example, IMO the 2 Level performs slightly better simply because the formation seems to hold better shape with an invisible leader. Curious if this would still be the case with different types of formation patterns. 