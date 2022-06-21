using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeToPlanet.Patches
{
    [HarmonyPatch(typeof(SGLocationWidget), nameof(SGLocationWidget.RefreshLocation))]
    public static class TimeToPlanetPatch
    {

        /// <summary>
        /// Displays the travel time to a planet from the jump ship.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="___simState"></param>
        /// <param name="___locationNameField"></param>
        public static void Postfix(SGLocationWidget __instance, SimGameState ___simState, LocalizableText ___locationNameField)
        {
            try
            {
                StarSystem curSystem = ___simState?.CurSystem;

                if (___simState == null || curSystem == null) return;

                //When traveling will be the remaining days.  Relative to direction.
                int travelTime = ___simState.TravelTime;
                int jumpDistance = curSystem.JumpDistance;

                string travelText;

                switch (___simState.TravelState)
                {
                    //At the jump point no well travel.
                    case SimGameTravelStatus.WARMING_ENGINES:
                    travelText = $"{jumpDistance}/{jumpDistance * 2}";
                        break;
                    case SimGameTravelStatus.IN_SYSTEM:
                        //Show up-well cost.
                        travelText = jumpDistance.ToString();
                        break;
                    case SimGameTravelStatus.TRANSIT_FROM_JUMP:
                        travelText = $"{travelTime}/{jumpDistance}";
                        break;
                    case SimGameTravelStatus.TRANSIT_TO_JUMP:
                        travelText = travelTime.ToString();
                        break;
                    default:
                        //All others, use default text.
                        return;
                }

                string newText = $" [{travelText} days]";

                ___locationNameField.text += newText;

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

    }
}
