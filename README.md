# GDIM33 Vertical Slice
## Milestone 1 Devlog

### Visual Scripting:

<img width="981" height="586" alt="截屏2026-04-28 20 42 41" src="https://github.com/user-attachments/assets/9875f5c9-f1e6-41f1-b8f3-b12c0f259c0e" />

explanation: I use the visual scripting graph in the door object to implement the function of the player entering the door, moving to the next scene, and progressing the current quest. I use the On Trigger Event node to detect whether any object has collided with the door object, which stores this graph. If there's any object collide with the door, triggers the event, On trigger Event listen to this event and make the graph runs the following nodes. The graph will first use ComareTag node to check if the tag of the other object is "Player". If its tag is "Player", it calls the specific method SetQuest() nodes that call from the singleton QuestManager Instance script. Get Instance use to assign correspond QuestManager instance to SetQuest()node, Quest state node use to assign specific quest to this node so that the SetQuest Node could run and change the current quest to implement the function of advancing quest progress, Then calls SceneManager Load Scene node to change the scene to the "station" scene. If its tag is not "Player" the graph calls Debug node to debug specific string in the debug node assigned by String node "Not player collide". This graph use to implement the function that only player enter the door will progress the quest and move to next scene.

### State Machine:
<img width="1260" height="1080" alt="0496eeeddb5a88c9269769d145e93abd" src="https://github.com/user-attachments/assets/21efce86-1da1-4faf-a426-9a404f895dd8" />


explanation: Blue is updated for ghost's state machine.

I updated the ghost's state machine, clarifying different movement logics depending on the ghost's current states, how the state machine with different behavior interacts with other systems and other scripts in-game. The ghost has three states: Roaming, Chasing, and Attacking. Changes of the state depend on the current distance between ghost and player. In the beginning of each turn the ghost calls DetectPlayer()to check the distance between player and change the state base on this. If the player is outside of the detect range, the ghost will remain Roaming state, which will move towards a random position set on the navmesh map within a range centered ghost each turn. If the player enter the detect range, the ghost will change its state to Chasing, which will use navmesh to generate a path between ghost and player, ghost will move along and path with specific distance each turn to implement the chase effect. If the player enter the attack range, the ghost will change its state to Attacking, which will attack and decrease player's health, after attack, transport the ghost random position away from the player enough, so that the player won't enter the ghost's dectect range in the next turn, make sure the ghost start roaming after attack. My break down shows how the state machine change the state depends on the condition and interact with other system in the game. The whole state machine implement the effect of ghost roaming chasing and attack player create a basic monster movement logic.

This state machine are highly connected with other systems in the game. It especailly highly connected with Navmesh system. Different state correspond to different particular moving behavior, and every moving behavior requires Navmesh system. In roaming states, it use navmesh agent.SetDestination(hit.position); hit a random position within a ghost's roaming range to find a random position on the generated navmesh map and use its moving logic to generate a path and move toward to that position. In Chasing states, its use navmesh agent.SetDestination(player.position); to create a path between ghost and player and use its moving logic to move along the path with specific distance. In Attacking states, it use navmesh agent.Warp(hit.position); to transport the ghost away from player to make sure the player is outside of the detection range. Every states' movement logic are highly rely on navmesh; it use navmesh system to implement different moving logic. The ghost's state machine also relate with Turn system, to strict ghost for only one action each turn and update the detect information each turn. Moreover, it relate with the health system, when the ghost attack, the health should decrase and update. All of these system help implement the whole logic of the ghost, they contribute to implement different behaviors base on state machine.







## Milestone 2 Devlog

### Break-Down

This feature will implement that when a player moves to a black area, reduce insanity each turn to achieve that the player loses sanity if they hide in the dark for too long.
Basic steps:
1. create new grid type which is insane grid
   -  add more data of grid in the node, add type "insane" by add bool isInsaneGrid and damage value. Create a new layer called InsaneLayer.
   -  Create a new method to check InsaneLayer by creating a new physicsbox and to check if this box collide with InsaneLayer. 
   -  Add this method to geneatedGrid() method to make sure it check every grid while generating it. if the check box collided with InsaneLayer then set the node of this grid, node.isInsance = true;
   -  add a new type in OnDrawGizmos() method to draw visualized grid . add node.isInsase?black into Gizmos. color to draw every grid with different color based on grid type. Run the game and test if gizmos draws black grid on the grid which is InsaneLayer.
2. add a function to check the gridtype under the players position every turn
   - Create a new script called InsaneGrid, assgin it to player, Create a new method GetCurrentNode()to get player's current grid node with a specific algorithm that divide player's position by cellsize to transfer its position data to cell coordinates. Then use method GetNode in GridManager to get corresponding grid of this coordinates to get player's grid location.
   - Create a new method called checkGrid to check the current grid type. Assign result from GetCurrentNode() to local variable node and use if(node.isInsaneGrid) to check whether current node is InsaneGrid; Make CheckGrid method subscribe OnTurnStarted to make sure it check the grid every turn.
   -  Add Debug function inside and run the game to check if it successful check the grid type.
3. create reduce sanity function
   - Create a new int turnCount to check how many turn player have stayed in the dark. if player in the insaneGrid each turn then add 1 to turncount by add turnCount++ in the if(node.isInsaneGrid).
   -  create a new if statement if(turnCount >= 2) inside the if(node.isInsaneGrid). To check player stay more than 1 turn which consider as "too long".
   -  use function TakeDamage from Playerhealth, to reduce the player's sanity based on the data of damage in node by adding PlayerHealth.Instance.TakeDamage(node.damage);
   -  Run the game and stay in black area(InsaneGrid) for more than one turn to check if everything worked.

Question 2: Yes, because this breakdown helps me break down these complicated features into some small functions that i need to implement, by thinking in that way it really help me understand the whole structure of the feature and inspired me how to achieve this feature across multiple systems. For example, stay in the dark for too long will lose sanity. By breaking down this feature instead of thinking, create a whole script that inlude clarify dark areas, lose sanity, and timer. I will use my existing system, the Grid system to create new grid type and check the grid, turn the system to record turn, health system to reduce sanity. Which makes my game more integrated, reduces my unnecessary works and keeps my system structure clear and separated. Also it helps me coding with more structured idea which increases my efficiency for exmaple in check grid i could know to getcurrentNode first then check grid type by using two different method and  what algorithm i need to use to transfer world position to grid coordinates, i will spend more time to figure out how to checkgrid each turn which is the only thing in my brain without any structured idea. If i will do this again, i will improve it by adding more debug steps in the break down so that i can make sure every specific steps are implemented their function, and if theres bug emerges it will help me quickly identify which steps create this bug or which steps didn't implement their function. Also I will clarify which system this step is using and how they connecting to other system to make steps more structured and keep system saparated.

Question 4: Please Grade my Unity system part based on my Navmesh system. Please check how ghost generated path when detect player, how i limited its moving distance based on navmesh path, how player interacting with the door to block ghost's path affecting its path generation to make ghost change the path, and also the roaming function is based on Navmesh system.

## Milestone 3 Devlog
Milestone 3 Devlog goes here.
## Milestone 4 Devlog
Milestone 4 Devlog goes here.
## Final Devlog
Final Devlog goes here.
## Open-source assets
- Cite any external assets used here!
