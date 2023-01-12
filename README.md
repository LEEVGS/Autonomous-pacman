# Pacman
## Description
The following project contains A* pathfinding. For grid-based pathfinding. Every ghost has its own behavior. Initially I would made Pacman autonomous with machine learning. But the flu thought different, and I underestimated the work for just Pacman.

## Design & implementation
### Map

The design of the default map is from the original Pacman. But has been made with a twist. The map is not hard programmed. I used map loading. Currently it loads from a hard coded string. But this can be changed to a file.

![world map](https://raw.githubusercontent.com/LEEVGS/Autonomous-pacman/main/Images/MapGeneration.png)

As you can see above the map is made with numbers. A zero is a wall and everything above that is walkable and so usable for A*. Each one is normal food and each two is a powerup that makes the enemies scared.

The reasons why I did not fully implement map loading is because some things need more data then just the map. Like the player and ghosts spawns could have been added but the hardcoded paths not.

### Ghosts

As mentioned before they use A* pathfinding. Each ghost has its own target and creates a path for them. While A* generates paths this doesn’t mean the ghost can easily use that. The ghosts use “checkpoints" to follow the path.

![checkpoints](https://raw.githubusercontent.com/LEEVGS/Autonomous-pacman/main/Images/CheckpointSystem.png)

Every 1000ms or each second the ghost recalculates the path. If he reaches his destination, he will seek the player position and kill him. If the player picks up the powerup every ghost will flee. In the original Pacman every ghost when spawned follows a hardcoded path. I didn’t implement this because I didn’t know this and found out way too late.

### Player
The player input was way harder than I thought. I struggled hard to get the grid-based movement working in Unity. It just never felt right. But with the final movement it feels amazing to play in.
If you want to go up but you can’t the player will remember that. So, the next intersection you hit you will go up. You don’t need to perfectly time your input to switch from direction.

## Result
The result I have is a half working Pacman with dying and eating ghosts and basic scoring.

### Pathfinding

![pathfinding](https://raw.githubusercontent.com/LEEVGS/Autonomous-pacman/main/Images/Pathfinding.gif)

### Flee mode

![pathfinding](https://raw.githubusercontent.com/LEEVGS/Autonomous-pacman/main/Images/Flee.gif)

### Final

![pathfinding](https://raw.githubusercontent.com/LEEVGS/Autonomous-pacman/main/Images/FinalGame.gif)

## Conclusion & future work
In conclusion I underestimated the logic of Pacman. I did not know there was so much logic after every ghost. I thought they just seek the player. But they didn’t… In the future I want to add the hardcoded paths and smoother pathfinding. Also a nice visual upgrade would be nice.
I might add multiplayer if there is any audience for this. The idea is that you both start at the same time and you have to survive as long as you can.
I now have the basics to do some cool complicated decision making and can easily bring this to 3D. I would use a more polished version for this as a minigame in a bigger game. Or make it multiplayer as mentioned before.
It was a fun project to see A* pathfinding in action and not just in a framework.
