﻿/**
 * Thunder Aerospace Corporation's Life Support for Kerbal Space Program.
 * Originally Written by Taranis Elsu.
 * This version written and maintained by JPLRepo (Jamie Leighton)
 * 
 * (C) Copyright 2013, Taranis Elsu
 * (C) Copyright 2016, Jamie Leighton
 * 
 * Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
 * project is in no way associated with nor endorsed by Squad.
 * 
 * This code is licensed under the Attribution-NonCommercial-ShareAlike 3.0 (CC BY-NC-SA 3.0)
 * creative commons license. See <http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode>
 * for full details.
 * 
 * Attribution — You are free to modify this code, so long as you mention that the resulting
 * work is based upon or adapted from this code.
 * 
 * Non-commercial - You may not use this work for commercial purposes.
 * 
 * Share Alike — If you alter, transform, or build upon this work, you may distribute the
 * resulting work only under the same or similar license to the CC BY-NC-SA 3.0 license.
 * 
 * Note that Thunder Aerospace Corporation is a ficticious entity created for entertainment
 * purposes. It is in no way meant to represent a real entity. Any similarity to a real entity
 * is purely coincidental.
 */

using System;
using System.Linq;

namespace Tac
{
    class AddLifeSupport
    {
        private static bool initialized = false;
        private GlobalSettings globalSettings;

        public AddLifeSupport()
        {
            this.Log("AddLifeSupport Constructor");
            this.globalSettings = TacStartOnce.globalSettings;
        }

        public void run()
        {
            if (!initialized)
            {
                this.Log("run AddLifeSupport");
                initialized = true;

                try
                {
                    var evaParts = PartLoader.LoadedPartsList.Where(p => p.name.Equals("kerbalEVA") || p.name.Equals("kerbalEVAfemale"));
                    foreach (var evaPart in evaParts)
                    {
                        EvaAddLifeSupport(evaPart);
                    }
                }
                catch (Exception ex)
                {
                    this.LogError("Failed to add Life Support to the EVA.\n" + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        private void EvaAddLifeSupport(AvailablePart part)
        {
            Part prefabPart = part.partPrefab;

            this.Log("Adding resources to " + part.name + "/" + prefabPart.partInfo.title);

            EvaAddPartModule(prefabPart);
            var settings = HighLogic.CurrentGame.Parameters.CustomParams<TAC_SettingsParms_Sec2>();
            EvaAddResource(prefabPart, settings.EvaElectricityConsumptionRate, globalSettings.Electricity, false);
            EvaAddResource(prefabPart, settings.FoodConsumptionRate, globalSettings.Food, false);
            EvaAddResource(prefabPart, settings.WaterConsumptionRate, globalSettings.Water, false);
            EvaAddResource(prefabPart, settings.OxygenConsumptionRate, globalSettings.Oxygen, false);
            EvaAddResource(prefabPart, settings.CO2ProductionRate, globalSettings.CO2, false);
            EvaAddResource(prefabPart, settings.WasteProductionRate, globalSettings.Waste, false);
            EvaAddResource(prefabPart, settings.WasteWaterProductionRate, globalSettings.WasteWater, false);
            for (int i = 0; i < prefabPart.Resources.Count; i++)
            {
                this.Log("Resource " + prefabPart.Resources[i].resourceName);
            }

        }

        private void EvaAddPartModule(Part part)
        {
            try
            {
                ConfigNode node = new ConfigNode("MODULE");
                node.AddValue("name", "LifeSupportModule");

                part.AddModule(node);

                this.LogWarning("The expected exception did not happen when adding the Life Support part module to the EVA!");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Object reference not set"))
                {
                    this.Log("Adding life support to the EVA part succeeded as expected.");
                }
                else
                {
                    this.LogError("Unexpected error while adding the Life Support part module to the EVA: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        private void EvaAddResource(Part part, double rate, string name, bool full)
        {
            try
            {
                double max = rate * HighLogic.CurrentGame.Parameters.CustomParams<TAC_SettingsParms_Sec3>().EvaDefaultResourceAmount;
                ConfigNode resourceNode = new ConfigNode("RESOURCE");
                resourceNode.AddValue("name", name);
                resourceNode.AddValue("maxAmount", max);
                if (full)
                {
                    resourceNode.AddValue("amount", max);
                }
                else
                {
                    resourceNode.AddValue("amount", 0);
                }
                resourceNode.AddValue("isTweakable", false);
                PartResource resource = part.AddResource(resourceNode);
                resource.flowState = true;
                resource.flowMode = PartResource.FlowMode.Both;
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Object reference not set"))
                {
                    this.LogError("Unexpected error while adding resource " + name + " to the EVA: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
    }
}
