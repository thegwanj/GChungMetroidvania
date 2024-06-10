# GChungMetroidvania
 Game dev project for CSI255 Spring Quarter 2024

## Documentation and Updates

### June 9, 2024
#### Features Added/Updated Since Last Update
- Added Forest_2 scene
    - Added Dash ability unlock
- Added Cave_1 scene
- Reverted player collider to box since capsule was making the character physics behave differently than intended
- Added Forest_2 and Cave_1 to build

#### Issues Encountered
- Pause/resume was only working on certain scenes. Fixed by making sure each scene had an EventSystem in place
- Player could occasionally fall through platforms and walls even if tilemap colliders were touching. Fixed by placing an additional invisible wall collider in the gaps

#### Lessons Learned
- Level design should NOT be done blind. It is very handy to have a plan in place before starting to build or else there will be a lot of back tracking and rebuilding of levels
- Levels take a long time to make. I greatly underestimated how long it would take to get just one level done, and now I have 2-3 more to go. Time management will be key as we start wrapping up the project

### May 26, 2024
#### Features Added/Updated Since Last Update
- Added in Main Menu
- Added in Options Menu
- Added in Credits
- Added in Quit Game option on the main menu
- Added pause/resume function
- Added option to return to main menu while paused
- Added Forest_1 scene
    - Added in parallax background
    - Added in forest environment via tilemap
- Added in ability unlocks
- Added camera bounds so that the camera could only move so far in a level
- Modified player collider from box to capsule
- Replaced default material for all assets to include a new default which can handle lighting
- Changed first playable scene to load from SampleScene to Forest_1

#### Issues Encountered
- Player script would not recognize tiles as being on the ground and would constantly play the jumping animation. Fixed by changing Composite Collider 2d geometry shape from Outline to Polygons
- Player would sometimes get caught on an edge where two tiles meet. Fixed by changing collider on player from box to capsule
- Player character will continue moving in the direction they died in. Mostly fixed by manually overriding movement/velocity when hitting 0hp. Dashing and dying will still cause the character to continue moving in the direction they died in
- Player could jump infinitely if they were pressed against a wall. Fixed by shrinking area script uses for finding the ground

#### Lessons Learned
- Small changes to just the camera can make a very large difference in how things are viewed by the player
- Ability unlocks were more or less just a series of booleans and I was very much overthinking it
- It's important to double-check variables and components after moving onto the next "phase" of the project because it's easy to forget that some settings were meant to be temporary and not changing/removing them will cause more issues down the line

### May 12, 2024
#### Features Added/Updated Since Last Update
- Added mana regeneration via attacking enemies
- Added in player sprite
- Added in player animations for idling, running, jumping, dashing, attacking, and dying
- Adjusted camera offset to make the world appear a little more zoomed in
- Added in second scene to test scene transitions
- Added in scene transitions from the first test scene to the second and back
- Adjusted movement settings to make movement less drastic
- Added in death/respawn UI
- Added in "checkpoint" object for the player to interact with in order to set a new respawn point

#### Issues Encountered
- Dashing through an enemy can cause the game to freeze. Fixed by changing the order in which functions are called in Update
- Attacking while in the air would cause the attack animation to play only after the player has landed. Found a missing animation transition and added it in
- Screen used for scene fade in/out was showing all black on game start. Fixed by disabling it in the editor and let the script handle the on/off toggle for it
- Interact function fires, but checkpoint object does not recognize the action. Working on a fix
- Player character will continue moving in the direction they died in. Working on a fix

#### Lessons Learned
- Sometimes, things just break for no reason. Had my UI not show up while running the game, but running it again right after had it back. This happened with a few other components during testing and I'm still not sure why or how it happened
- Making sure the order of operations is correct is very important as having something just one line off of where it needs to be can cause a game breaking bug
- It takes time and effort to design good levels. Just slapping something things together and hoping it works is not good level design (usually)

### April 28, 2024
#### Features Added/Updated Since Last Update
- Added in basic movement to the player
    - Horizontal movement, jumping
- Added in a double jump and dash movement to the player
- Added attack zones to the player
- Added basic enemy
- Added Zombie enemy
- Added player health
- Added player healing
- Added player mana
- Added in player and enemy recoil when player is attacking
- Added effects for when player gets hit
- Added iframes that activate when player gets hit

#### Issues Encountered
- Recoil movement in editor is not consistent with recoil movement in builds. Resolved by moving recoil function into FixedUpdate instead of Update
- Enemy would get "stuck" when coming in to contact with player. Resolved by fixing collision code and adding some mass onto enemy rigidBody
- Enemy would deal infinite damage when coming into contact with player. Resolved by fixing health code that was not firing off correctly
- Dashing through an enemy can cause the game to freeze. Working on a fix

#### Lessons Learned
- Update and FixedUpdate while very similar, are still different enough that you need to make sure you are putting the proper functions into each one
- Code that updates the HUD can also cause issues with other code that need to run at the same time
- It's important to save code frequently since only after you hit save does Unity compile everything
