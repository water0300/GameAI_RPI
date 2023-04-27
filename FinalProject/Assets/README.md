# Game AI Final Project - Predator Prey Ecosystem Simulation

## Installation 
1. [Clone this folder in this repository](https://github.com/water0300/GameAI_RPI/tree/master/FinalProject)
2. Open the above folder with Unity (I used Unity 2021.3.16f1)

## Running the Environment

In the hierarchy under "Handlers", contains the GameManager component. This is in charge of most of the configuration parameters for the ecosystem. In it contains the herbivore and carnivore prefabs, as well as initial conditions for population count, lifespan, mutation chances, pregancy settings, and timestep

There also is a ResourceSpawner component on this handler object, which handles the plant spawning

Once these settings are configured to your liking, just press play and the simulation will begin running.

## Collecting Data

In the hierarchy under "Handlers" exists a child object called "StatTracker". It accepts a scriptable object of type "StatData", and stores population data, as well as a "RecordIntervalSecs" variable that affects how often data is captured in seconds. Once you press stop, the data persists in the data object. Note that whenever you restart the scene (i.e. press play), this StatData object resets.

To transform the data for parsing and statistics, open the stat data object and press the button "Save to TXT". This will save the data into the folder Assets\Python. In this folder also contains a script "stat_tracker.py" which when run, parses the data and gives some nice looking graphs