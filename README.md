# CyberSwing!

## Game Overview
- **Title:** CyberSwing
- **Genre:** Parkour/Runner
- **Theme and Inspiration:** Integration of Spiderman-like swinging with the parkour of *Ghostrunner*.
- **Target Audience:** A universal game designed for stress relief and leisure, appealing to players of all ages without the pressure of competitive gameplay.
- **Platform:** Optimized for laptop/desktop.

---

## Technologies Used

The development of **CyberSwing** involves the use of the following technologies and programming languages:

- **C#: The primary programming language used for implementing game mechanics, player controls, and overall logic in Unity.
- **HLSL: Utilized for creating shaders and custom graphical effects to enhance the visual experience.
- **ShaderLab: Used for defining and managing Unity's shader assets, including materials and textures.
  
### Frameworks and Tools:
- **Unity 3D**: The core game engine used for creating the game environment, physics-based mechanics, and level design.
- **Visual Studio**: The primary code editor and integrated development environment (IDE) for writing and debugging scripts.
- **Git & GitHub**: For version control and collaboration.
- **Unity Asset Store**: For acquiring 3D assets, shaders, and plugins to expedite the development process.

---

## Game Concept
### Mechanics
The mechanics of *CyberSwing* revolve around the experience of swinging and grappling, combined with wall running. As players progress through levels, they will encounter a variety of obstacles that challenge their agility and reaction time while a wall of lasers chase them from behind.

### Controls
- **Movement:** W, A, S, D keys
- **Grappling Hook Deployment:** Mouse clicks
- **Swinging:** Mouse hold after click
- **Wall Running:** Automatic

### Game Logic Engine
Unity 3D will be the backbone of our game, offering the necessary tools for implementing the physics-driven movement system we envision.

---

## Major Classes and Scripts
- **`GrapplingGun.cs`:** Manages the grappling hook's launch, the connection to points within the environment, and the swing dynamics, ensuring a realistic pendulum motion.
- **`MoveCamera.cs`:** Adapts dynamically to the player’s movements, enhancing immersion and providing the best view of the action.
- **`PlayerCam.cs`:** Ensures the first-person perspective accurately follows the player’s orientation and position.
- **`PlayerMovement.cs`:** Interprets keyboard inputs for running, jumping, and initiating wall runs.
- **`PlayerRespawn.cs`:** Manages game checkpoints and the respawn system, crucial for a game that challenges the player's precision and timing.

---

## Controls and Physics
### Grappling Mechanics
- Activated by a mouse click, the grappling hook casts toward targeted points.
- The duration of the mouse click determines the rope length, adding skill and precision to the swinging motion.

### Swinging Dynamics
- Players swing in a pendulum motion after hooking, with the ability to release and launch themselves forward.
- Mastery of momentum and timing is essential for effective traversal.

### Rigidbody Physics
Unity's physics engine brings the swinging to life, simulating gravity and inertia. The **Rigidbody** component ensures realistic arcs and impacts for every movement.

---

## Development Team Responsibilities
- **Sambhav:** Development of core gameplay mechanics, especially grappling and swinging systems, refining **Rigidbody** settings for desired swing behavior and player feedback.
- **Zeeshan:** Creation of challenging, visually engaging levels that naturally escalate in difficulty, testing the player's reflexes and rewarding strategic use of swinging mechanics.

---

## Game Assets and Environment
- **Characters:** The player character, equipped with a grappling hook.
- **Objects:** Obstacles to dodge and platforms to land on.
- **Backgrounds:** Calm and immersive "Lofi Infinite" backdrop to contrast with high-energy gameplay.

---

## User Interface
- **Main Menu:** Simple and user-friendly, offering level selection and game settings.
- **In-Game UI:** Displays:
  - Current level
  - Checkpoints reached
  - Basic controls
  - All within an unobtrusive layout

---

## Goal and Win Condition
### Objective
Navigate through levels filled with obstacles and challenges using grappling and parkour skills. Checkpoints serve as progress markers and respawn points.

### Win Condition
- Reach the end of each level, passing through all checkpoints.
- Skillful navigation, timing, and effective use of swinging mechanics are key to overcoming obstacles.
- Levels become progressively more complex, requiring mastery of physics and controls.

---

