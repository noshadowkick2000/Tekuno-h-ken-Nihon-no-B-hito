Current todo:

Player:
make list of all animations
make integer based ID system for each animation (can be based on categories, i.e. all sheathed animations start with 0)
update animator state machine (check out layers)
add StateDenoter component to all states and fill in ID
figure out player architecture
write the state machine code

Enemy:
check current architecture with mr C
code (see also state machine stuff from player)

Note on Colliders:
2d and 3d colliders do not interact with each other, however colliders which follow the edges of sprites are only in 2d
2d colliders also dont work with 3d terrain
current solution is to use 3d colliders for movement and terrain, while 2d collisions are used for combat hitboxes
as objects cannot have multiple colliders, seperate weapon from character in sprite and give them seperate hitboxes