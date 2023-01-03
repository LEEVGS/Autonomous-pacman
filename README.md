# Pacman
## Map generation
I use a string to read in the map. Later I can use files for custom maps.
![world map](https://raw.githubusercontent.com/LEEVGS/Autonomous-pacman/main/Images/MapGeneration.png)
- 0: Wall (not walkable)
- 1: Normal food
- 2: Power up

## Enemy AI
### Pathfinding
- By the use of A* (A-star) pathfinding the ghost can seek to a destination in the map.

- Each ghost has an unique target. If the ghost is already at the target it will go into seek mode.

- When the player picks up the powerup all the ghost go into flee mode. For the moment, they find the farthest tile and pathfind to it. I will improve this (now the ghost can run into the player)

### Hard programmed path
- In the original pacman every ghost when they spawn do a hardcoded path. So the ghost does not instantly seek you. (I will add this later)

## Player
- Manual input (for now)
