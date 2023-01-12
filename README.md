# Pacman
## Dynamic generation
I use a string to read in the map. Later I can use files for custom maps. For now the player and ghost spawns are static programmed. But this can change later.

![world map](https://raw.githubusercontent.com/LEEVGS/Autonomous-pacman/main/Images/MapGeneration.png)

 0: Wall (not walkable)
1: Normal food
2: Power up

## Enemy AI
### Pathfinding
- By the use of A* (A-star) pathfinding the ghost can seek to a destination in the map.

- While A* creates a path. This does not mean the enemy can move. The enemy moves by using a checkpoint system.

![checkpoints](https://raw.githubusercontent.com/LEEVGS/Autonomous-pacman/main/Images/CheckpointSystem.png)

- If an enemy is near a checkpoint he removes it. Every second the pathfinding algorithm is run again to update the checkpoints.


- Each ghost has an unique target. If the ghost is already at the target it will go into seek mode.

![pathfinding](https://raw.githubusercontent.com/LEEVGS/Autonomous-pacman/main/Images/Pathfinding.gif)


- When the player picks up the powerup all the ghost go into flee mode. For the moment, they find the farthest tile and pathfind to it. I will improve this (now the ghost can run into the player)

### Hard programmed path
- In the original pacman every ghost when they spawn do a hardcoded path. So the ghost does not instantly seek you. I left this out for now because it doesn't work well for me with the dynamic map.

## Player
- Manual input
- Logs direction
