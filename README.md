# Baby Interaction System

This Unity project implements an interactive baby character that follows a finite state machine (FSM) to engage with a ball and the player. The baby crawls towards a ball, picks it up, delivers it to the player, and returns to its starting position—all with smooth animations and dynamic speed adjustments. Designed for a fun and responsive user experience, this project leverages Unity's animation tools, custom colliders, and the Enhanced Touch API for input handling.

## Features
- **Finite State Machine (FSM):**  
  - **Idle:** Baby stays stationary, facing the player.  
  - **Chase:** Baby crawls toward the ball with dynamic speed based on distance.  
  - **PickBall:** Baby picks up the ball when close enough.  
  - **Deliver:** Baby crawls to the player to return the ball.  
  - **ReturnToIdle:** Baby returns to its original position.  

- **Dynamic Speed Logic:**  
  - Speed adjusts using the formula: `Speed = Clamp(Acceleration × Distance, 0, MaxSpeed)`.  
  - Smooth movement with `Vector3.MoveTowards()` and natural rotation via `SmoothRotate()`.

- **Animation System:**  
  - Custom animation controller with `CrossFade()` for seamless transitions.  
  - Precomputed animation hashes for optimized performance.  
  - Facial blend shapes (happy, blink, smile) integrated into animation clips.

- **Tap Detection:**  
  - Raycasting system detects taps on the baby’s colliders.  
  - Baby rotates to face the tapped position.  
  - Custom box colliders added to the model for precise interaction.

- **Enhanced Touch Input:**  
  - Built with Unity’s Enhanced Touch API for efficient multi-touch support.

- **Additional Features:**  
  - Camera zoom button for easier interaction with the baby.  

## Project Structure
- **Scripts:** Core logic for FSM, movement, animation, and input handling.  
- **Models:** 3D baby model with custom colliders aligned to body parts.  
- **Animations:** Prebuilt clips with blend shapes and custom transitions.  
- **Scenes:** Main scene with baby, ball, and player interaction setup.

## Setup Instructions
1. **Requirements:**  
   - Unity 2021.3 LTS or later (tested with Unity 2022.x).  
   - Enhanced Touch API enabled in Unity Input System.  
   - (Optional) Mac with Xcode for iOS builds.

2. **Installation:**  
   - Clone the repository:  
     ```bash
     git clone https://github.com/ThePanchalShadow/Baby-and-Ball-Project.git## Setup Instructions (Continued)

- **Open in Unity Hub:**  
  - Open Unity Hub and click "Add" to select the cloned project folder.  
  - Load the main scene from the `Assets/Scenes` directory.

- **Running the Project:**  
  - Press "Play" in the Unity Editor to test the game.  
  - Interact by tapping the baby or using the camera zoom button.

- **Building for iOS:**  
  - Requires a Mac with Xcode installed.  
  - In Unity, go to `File > Build Settings`, switch the platform to iOS, and click "Build".  
  - Note: iOS builds were not tested due to hardware limitations (see Limitations below).

## Limitations
- **iOS Build:**  
  - The project is fully implemented and ready for iOS deployment, but a Mac is required to compile and test the build.  
  - I do not have access to a Mac, so this step remains untested.

## Challenges & Solutions
- **Touch Input Confusion:**  
  - Unity’s new Input System offers multiple options.  
  - **Solution:** The Enhanced Touch API was chosen for its efficiency and multi-touch support.  
- **Model Structure:**  
  - The baby model lacked bone-based colliders, making tap detection unreliable.  
  - **Solution:** Custom box colliders were manually added and aligned with body parts.

## Assumptions
- Tap interaction wasn’t detailed in the requirements.  
  - **Implemented:** A camera zoom button and raycast-based tap system were added for usability.

## Conclusion
This project delivers a polished baby interaction system with smooth locomotion, animations, and touch controls. While iOS deployment requires a Mac, all mechanics are fully functional in the Unity Editor and ready for further development.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact
For questions or feedback, feel free to open an issue or reach out via [jigarpanchal200203@gmail.com](mailto:jigarpanchal200203@gmail.com).
