#region Prelude
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using VRageMath;
using VRage.Game;
using VRage.Collections;
using Sandbox.ModAPI.Ingame;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;

// Change this namespace for each script you create.
namespace SpaceEngineers.UWBlockPrograms.BatteryMonitor {
    public sealed class Program : MyGridProgram {
    // Your code goes between the next #endregion and #region
#endregion

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    //Program updates every 100 Ticks (Game runs 60 Ticks per Second)
}

// =======================================================================================
// Basic Declaration of public variables (used across the code)
// =======================================================================================

static class CraitTimer
{
    public static int daylength = 0;    
    public static int nightlength = 0;
    public static int cycles = 0;
    public static int SolarCheck = 0;
    public static int DayMax = 240400;
    public static int NightMax = 209600;
}

// =======================================================================================
// Makes a cool progress bar "image"
// =======================================================================================

string ProgressBar(int max, int progress)
{
    decimal percentage = 0;
    int i = 0;
    char[] bar = new char [10];
    
    percentage = (( (decimal) progress) / ( (decimal) max ) * 100);
    while (i < 10)
    {
        if ( (int) percentage  >= (((decimal) i * 10) + 1))
        {
            bar[i] = '+';
        }
        else
        {
            bar[i] = '-';
        }
        i++;
    }
    string barReturn = new String(bar);
    return barReturn;
 }
    

public void Main()
{

// =======================================================================================
// Initialisation of variables
// =======================================================================================

    Echo("Init began");
    var MyLCD = GridTerminalSystem.GetBlockWithName("LCDClock") as IMyTextPanel;
    string display = "Current lighting: \n";

    Echo("Init 1 : Lights");
    List<IMyLightingBlock> LightGroup = new List<IMyLightingBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(LightGroup);
    int LightCount = LightGroup.Count;

    Echo("Init 2 : Solar Panels");
    List<IMySolarPanel> SolarGroup = new List<IMySolarPanel>();
    GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(SolarGroup);
    var SolarPanel = SolarGroup[0];

    Echo("Init 3 : Pistons");
    IMyBlockGroup PistonGroup_1 = GridTerminalSystem.GetBlockGroupWithName("TowerPistons");
    List<IMyExtendedPistonBase> TowerPistons = new List<IMyExtendedPistonBase>();
    PistonGroup_1.GetBlocksOfType(TowerPistons);

   int daytime = (CraitTimer.daylength) / 60;    
   int nighttime = (CraitTimer.nightlength) / 60;

// =======================================================================================
// Validation checks.
// =======================================================================================
    
    if (LightGroup == null) 
    {
        Echo("LIGHTS not found");
        return;
    } 

    if (SolarPanel == null || MyLCD == null)
    {
        Echo("Couldn't find the Solar Panel OR the named LCD screen");
        return;
    }

    if (PistonGroup_1 == null)
    {
        Echo("Couldn't find the pistons named: " + "TowerPistons");
        return;
    }

// =======================================================================================
// Functions to output to LCDs, calculate current solar exposure duration.
// =======================================================================================

    if (SolarPanel.CurrentOutput > 0 && CraitTimer.daylength != 0)
    {
            int percentage = (int) ( (double) CraitTimer.daylength / (double) CraitTimer.DayMax * 100);
            int daymax = (CraitTimer.DayMax) / 60;

            if (CraitTimer.DayMax < CraitTimer.daylength)
                {
                    CraitTimer.DayMax = CraitTimer.daylength;
                    display += "Day length exceeds expected! Calibrating..";
                }
            else
                {
                    display += ProgressBar(CraitTimer.DayMax, CraitTimer.daylength) + "\n Day time has gone for \n"  + (daytime / 60)  + " minutes and " + (daytime % 60) + " seconds \n\n" + percentage + "% complete";
                    display += "\n\n Time Remaining (est.): \n" + ((daymax / 60) - (daytime / 60)) + " minutes and " + ((daymax / 60) - (daytime % 60)) + " seconds.";
                }

            for (int i = 0; i < TowerPistons.Count; i++) 
                {
                   TowerPistons[i].Retract();
                }

            CraitTimer.daylength += 100;
            Echo("Day time " + CraitTimer.daylength);
            CraitTimer.SolarCheck = 0;
    }    
  
     if (SolarPanel.CurrentOutput <= 0 && CraitTimer.nightlength != 0)
        {
            int percentage = (int) ( (double) CraitTimer.nightlength / (double) CraitTimer.NightMax * 100);
            int nightmax = (CraitTimer.NightMax) / 60;
            if (CraitTimer.NightMax < CraitTimer.nightlength)
                {
                    CraitTimer.NightMax = CraitTimer.nightlength;
                    display += "Night length exceeds expected! Calibrating..";
                }
            else
                {
                    display += ProgressBar((CraitTimer.NightMax), (CraitTimer.nightlength)) + "\n Night time has gone for \n" + (nighttime / 60)  + " minutes and " + (nighttime % 60) + " seconds \n\n" + percentage + "% complete";
                    display += "\n\n Time Remaining (est.): \n" + ((nightmax  / 60) - (nighttime / 60)) + " minutes and " + ((nightmax  / 60) - (nighttime % 60)) + " seconds.";
                }
            for (int i = 0; i < TowerPistons.Count; i++) 
                {
                   TowerPistons[i].Extend();
                }
            CraitTimer.nightlength += 100;
            Echo("Night time " + CraitTimer.nightlength);
            CraitTimer.SolarCheck = 0;         
         }

    if (SolarPanel.CurrentOutput > 0 && CraitTimer.daylength == 0)
    {
        Echo("Entering Daylight!");
        display += " sunrise! \n\n Rise and shine, Gamers!";
        
        Me.CustomData = "Crait Night was :" + CraitTimer.nightlength + " long... in the " + CraitTimer.cycles + " cycle.";
        CraitTimer.nightlength = 0;
        CraitTimer.daylength += 100;
        CraitTimer.cycles++;

        for (int i = 0; i < LightCount; i++)
            {
                LightGroup[i].Intensity = 3;
            }
    }

    if (SolarPanel.CurrentOutput <= 0 && CraitTimer.nightlength == 0 && CraitTimer.SolarCheck == 10)
        {
            Echo("Entering Night Time!");
            display += " sunset! \n\n Time for bed, Gamers!";
            
            if (CraitTimer.cycles != 0)
                 {
                       Me.CustomData = "Crait Day was : " + CraitTimer.daylength + " long... in the " + CraitTimer.cycles + " cycle.";
                 }
            else
                  {
                        Me.CustomData = "Initialisation Occured!";
                  }      
            CraitTimer.daylength = 0;
            CraitTimer.nightlength += 100;
            CraitTimer.cycles++;
            CraitTimer.SolarCheck = 0;
            for (int i = 0; i < LightCount; i++)
               {
                    LightGroup[i].Intensity = 8;
               }
        }      

    if (SolarPanel.CurrentOutput <= 0 && CraitTimer.nightlength == 0 && CraitTimer.SolarCheck < 10)
        {
            CraitTimer.SolarCheck += 1;
            CraitTimer.daylength += 100;
            Echo("I detected no light on the Solars! Solar Check is at "+ CraitTimer.SolarCheck);
            display += "\n\n is it dark outside? \n\n (#`Д´) Testing... " + CraitTimer.SolarCheck;          
        }

    IMyTextPanel V_LCD = MyLCD;
    if (V_LCD != null) 
         {
              MyLCD.WriteText(display, false);
         }
    else
        {
            Echo("Couldn't find the LCD to print to, piss");
            return;
        }

    Echo("I finished my work!");

    // The main entry point of the script, invoked every time
    // one of the programmable block's Run actions are invoked,
    // or the script updates itself. The updateSource argument
    // describes where the update came from.
    // 
    // The method itself is required, but the arguments above
    // can be removed if not needed.
}

#region PreludeFooter
    }
}
#endregion