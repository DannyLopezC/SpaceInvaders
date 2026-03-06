# 👾 Space Invaders -- Unity Implementation

This project is a recreation of the classic Space Invaders arcade game
developed in Unity.\
The goal of the project is to demonstrate gameplay programming, clean
architecture, and object-oriented design principles applied to a simple
but complete game system.

------------------------------------------------------------------------

## 🎯 Project Objective

Develop a playable version of Space Invaders while focusing on:

-   Clean and maintainable code architecture.
-   Gameplay system implementation.
-   Separation of responsibilities using good software design practices.

------------------------------------------------------------------------

## 📌 Features

-   Player movement and shooting mechanics.
-   Enemy wave movement system.
-   Enemy shooting behavior.
-   Collision detection and damage system.
-   Score tracking.
-   Basic UI elements.

------------------------------------------------------------------------

## 🖼️ Preview

![Gameplay
Demo](https://github.com/DannyLopezC/SpaceInvaders/blob/main/demo_video.gif)

------------------------------------------------------------------------

## 🔧 Requirements

This project requires:

Unity 2021 or newer

------------------------------------------------------------------------

## ▶️ How to run the project

1.  Clone this repository:

git clone https://github.com/DannyLopezC/SpaceInvaders.git cd
SpaceInvaders

2.  Open the project in Unity.

3.  Open the main scene.

4.  Press Play in the Unity Editor.

------------------------------------------------------------------------

## 🎮 Controls

Move Left → A / Left Arrow\
Move Right → D / Right Arrow\
Shoot → Spacebar

------------------------------------------------------------------------

## 🧠 Concepts Used

-   Object-Oriented Programming
-   Gameplay architecture
-   Unity component system
-   Collision detection
-   Event-driven gameplay systems

------------------------------------------------------------------------

## 🏗 Architecture

The project follows a modular gameplay architecture that separates
player logic, enemy behavior, and game management systems. This approach
helps maintain clean code and allows individual gameplay systems to
evolve independently.

### Player System

The **player system** is responsible for handling the player's behavior
and input.

Responsibilities include:

-   Processing movement input
-   Managing shooting mechanics
-   Interacting with projectiles
-   Handling collisions and damage

The player script focuses exclusively on player behavior and does not
control global game logic.

------------------------------------------------------------------------

### Enemy System

Enemy behavior is organized as a separate system that manages the
movement and actions of enemy units.

Responsibilities include:

-   Coordinating enemy wave movement
-   Managing enemy shooting behavior
-   Handling enemy destruction
-   Updating enemy state

Separating enemy logic allows different enemy types or behaviors to be
introduced easily.

------------------------------------------------------------------------

### Projectile System

Projectiles fired by both the player and enemies are handled through a
dedicated system.

Responsibilities include:

-   Projectile movement
-   Collision detection with other entities
-   Damage application
-   Destroying projectiles when they leave the play area

This modular design avoids duplicating shooting logic across different
entities.

------------------------------------------------------------------------

### Game Management

The **game manager** coordinates global game state and progression.

Responsibilities include:

-   Managing score updates
-   Handling game over conditions
-   Coordinating enemy wave progression
-   Managing global gameplay flow

Centralizing these responsibilities ensures that gameplay systems remain
loosely coupled.

------------------------------------------------------------------------

### Unity Component Architecture

The project leverages Unity's **component-based architecture**, where
behavior is divided into reusable scripts attached to game objects.

This allows systems such as:

-   Player control
-   Enemy behavior
-   Collision detection
-   UI updates

to remain modular and easy to maintain.

------------------------------------------------------------------------

## 📚 Credits

Project developed as a personal programming exercise focused on gameplay
systems and clean architecture in Unity.

Author: [DannyLopezC](https://github.com/DannyLopezC)
