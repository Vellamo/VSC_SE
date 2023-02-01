# Space Engineer Scripting 

The following repository is a container of various (Space-Engineer specific) C# scripts. At the moment it contains a predictive script that takes the input from solar panels to calculate the length of a "Day" and "Night" based on your (assumed synchronous) orbit.

## Getting Started

These instructions will allow you to try some of my scripts for yourself! They are not available on the Steam Workshop.

### Prerequisites

The things you need:

```
* A copy of Space Engineers
```

### Installation

You will first need to spawn the Programmable Block and copy & paste the desired script (.cs) from the Space Engineers folder.

```
You may encounter an error if Scripting is not enabled in the world, amend world setting as neccesary.

Choose "Edit" on the programmable block, insert the copied code. 

The code should compile, if not choose "Recompile"
```

## Usage

A few examples of useful commands and/or tasks.


```
# First example
# Second example
# And keep this in mind
```

## Detailed Breakdown

### Light_Level.cs
A method to take solar output (ie. light) and use this data to predict when the next sunrise will occur. 

Though the "day length" may be accessed in the settings in Space Engineers (herein referred to as "SE"), the global day length doesn't neccesarily apply in space where the situation may change.

This simple program assumes a regular orbit and no deviation that would alter the prediction long-term and is ultimately a little bit of fun.

The SolarPanel.CurrentOutput has a degree "jitter", in testing, whereby it alternates between 0 and the actual value. A degree of resiliance was built in to the script to account for this. 

## Additional Documentation and Acknowledgments

Credit to https://github.com/mrdaemon for the working `.csproj` file.

My scripting could not have been completed without the exhaustive use of Malware's Documentation contained here: https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
