# Unity Game Simulation Match 3 Sample

Welcome to the Games Workshop at Unity AI Summit! In this workshop, you will learn how to set up simulations to playtest a sample match-3 game. 

# Prerequisites
- Have a Unity ID. If you don't have a Unity ID, please create one at https://id.unity.com/. 
- Have a Github account to download the sample game from this repository.
- Install Unity 2019.4.10 from Unity Hub
- Sign up Unity Game Simulation service [here](https://dashboard.unity3d.com/metered-billing/marketplace/products/2771b1e8-4d77-4b34-9b9d-7d6f15ca6ba1) 
using the Unity ID and organization of your choice. 
We offer you a one-time credit of 500 simulation hours. You will not be charged in this workshop.

# Documentation
- [Unity Game Simulation Package Guide](https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/index.html)
 is the best place to get started with Unity Game Simulation.
- [Implementing Game Simulation](https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/ImplementationGuide.html)
  offers a step-by-step guide on how to install the Unity Game Simulation Package and integrate it with your game. 
- [Game Simulation window](https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/GameSimulationWindow.html)
 provides instruction on setting up parameters and uploading a build to run simulations.
- [Unity Game Simulation Dashboard Guide](https://unity-technologies.github.io/gamesimulation/Docs/Dashboard.html)
 is a step-by-step uide of launching game simulation and downloading results through the web dashboard.

# Quick Start
## Download the sample game
In this repository, click "Code -> Download ZIP" to get the sample game. 
Once you have downloaded the game, unzip the game code. Next, Open "Unity Hub", and "Add" the game to Unity Hub. 
We recommend using Unity 2019.4.10 for the sample game. Now you are all set to open the sample game. 
 
## About Unity Game Simulation Match 3 Sample Project
Unity Game Simulation Match 3 Sample Project is a sample match 3 game. 
The game consists of different levels. 
In each level, you can change the following features:

- Gem type
- Board size
- Goal type
- Number of movement allowed
- Target score for winning

There is a list of levels that are defined, 
including `Level_A`, `Level_B`, `Level_C`, `Level_D`, `Level_E`. 
In Unity Editor, you can open `Assets -> Levels` and see each level's definition in the Inspector.

In the sample game, we have implemented a Bot `Match3Bot` that knows how to play the game. 
In Unity Editor, find the `Match3Bot` Game Object and make sure that it is enabled. 
Now hit the "Play" button in Unity Editor, and you should see the bot playing the game. 


## Game Balance
Imagine you are a game designer or developer of the Match 3 game, 
you would like to know if the game is balanced and fun. 
You may want to find the answer to the following questions:

- What is the average number of moves to win a level?
- What is the win rate of each level?
- Is level B too difficult compared to level A?
- How does the board size affect the game?
- How does changing the gem types affect the game play?

With Unity Game Simulation, you can instrument your game with remote configurable parameters 
and metrics. Next, you set up simulations in the cloud to playtest the game to answer game balancing questions.

## Integration Guide
Open the sample game in Unity Editor. Find the Hierarchy and make sure “Match3Bot” Component is enabled. 
Hit the "Play" button and you should see the bot playing the match 3 game. Click the "Play" button again to exit the play mode. 


In your Unity Editor, go to "Account", log in using your Unity ID. Set up a Unity project under your organization for the sample project.
Note, use the same organization that you signed up Game Simulation with. 

For this workshop, we have already set up a game object named `Match3GameSimulation` with script `Match3GameSimulation.cs` that integrates the game with Unity Game Simulation. 
You will practice setting up simulation in the `Match3GameSimulation.cs` script.

Let's integrate the sample game with Unity Game Simulation package following the [Implementing Game Simulation](https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/ImplementationGuide.html) Guide:
1. [Install Unity Game Simulation package](https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/ImplementationGuide.html#step-1-install-the-game-simulation-package)

    Add the following dependencies to the project's `Packages/manifest.json`: 
   ```
   "com.unity.simulation.games": "0.4.5-preview.2",
   ```
2. Set up [parameters for Grid Search](https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/ImplementationGuide.html#step-2-create-parameters-in-game-simulation-for-each-grid-search-parameter).

      In this sample project, we would like to find the average number of moves to win in each level. We make the level name remotely configurable. In the Game Simulation window, add a parameter `level` with the type as `string` and default value as `Level_A`. Click Save when done.

3. [Load parameters for grid search](https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/ImplementationGuide.html#step-3-load-parameters-for-grid-search)
  
      In the sample project, we want to load the parameter "level" when the game starts. The game renders the features specified in a level. 
      Open `Match3GameSimulation.cs` in your IDE. Find the `Awake()` method, and implement loading parameters with a callback `OnFetchConfig`.
      
      You should complete the "TODO: 0" through "TODO: 2" in this step. 
      
4. [Enable metric tracking](https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/ImplementationGuide.html#step-4-enable-metrics-tracking)
      
      We would like to track the number of moves used when the bot wins/loses a level. 
      Please implement `Match3_OnWin` and `Match3_OnOutOfMoves` methods.
      You should complete "TODO: 3" and "TODO: 4" in this step.
      
5. [Test your implementation](https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/ImplementationGuide.html#step-5-test-your-implementation)

      Now you are ready to test your implementation! Make sure you enable the `Match3GameSimulation` game object in the Hierarchy. 
      Click the "Play" button to run a simulation locally. Exit the play mode once the bot stops playting the sample game. 
      Verify that there is a file called `counters_0.json` in your system’s default Application.persistentDataPath. 
      The json file includes the metris that you instrumented in the sample game. 

6. [Upload your build](https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/ImplementationGuide.html#step-6-upload-your-build-to-game-simulation)
Now you are ready to run a grid search with Unity Game Simulation! Please move to the next section 

## Manage Simulations on Unity Game Simulation Dashboard
Once you have implemented the Unity Game Simulation package and have uploaded your build, you can start to run simulations through the dashboard. 
We will follow the [Unity Game Simulation Dashboard Guide](https://unity-technologies.github.io/gamesimulation/Docs/Dashboard.html) in this section.

### Create Simulation
1. Log in [gamesimulation.unity3d.com](gamesimulation.unity3d.com) using your Unity ID
2. Access your project using the navigation on the left. Select the project that you just uploaded a build to. 
3. Create Simulation

Once you select your project, click the "Create Simulation" button on the top right corner, which takes you to create a new simulation. 
You will design a grid search when creating a simulation. In the sample project, we only have parameter `level`. 
We can specify a list of levels to search, and find how the number of movements varies in each level. 

In "Parameters", set the key "level"'s values to `Level_A`, `Level_B`, `Level_C`, `Level_D`, `Level_E`, which allows the simuaulation to search through all 5 levels. 
For the number of runs for each combination, we would recommend to set it to 3 for demo purpose. For the max timeout per run, we would suggest set it to 15 minutes. 

After filling in all the details on the "Create Simulation" page, hit "Run" button to launch your simulation. 

### View Metrics
Once the simulation completes, you can download the results. You will see the metrics in both Raw Data and Aggregate Data. 
