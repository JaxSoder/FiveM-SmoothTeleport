using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using SharedScripts;

namespace SmoothTeleport
{
    public class Main : BaseScript
    {

        private List<Teleporter> teleporter_list = new List<Teleporter>()
        {
            new Teleporter("Roof1",     new Vector3(6.62472f, -934.666f,  28.905f),   new Vector3(1.06041f, -930.285f, 113.593f)),
            new Teleporter("Build1",    new Vector3(-529.016f, -878.815f, 25.2694f), new Vector3(-539.904f, -880.342f, 31.7332f)),
            new Teleporter("NewsHeli",  new Vector3(-555.01f, -911.983f, 22.867f),   new Vector3(-569.289f, -927.904f, 35.8335f)),

        };

        public Main()
        {
            foreach (Teleporter item in teleporter_list)
            {
                item.ActivateTeleporter();
            }
        }
    }

    public class Teleporter : BaseScript
    {
        private readonly string teleporter_name;

        private readonly Vector3 first_point;
        private readonly Vector3 second_point;

        private float distance_to_first_point;
        private float distance_to_second_point;

        private static Vector3 marker_dir   = new Vector3(0, 0, 0);
        private static Vector3 marker_rot   = new Vector3(0, 0, 0);
        private static Vector3 marker_scale = new Vector3(1.5f, 1.5f, 1.5f);
        private static Color   marker_color = Color.FromArgb(150, 255, 255, 0);

        public Teleporter(string teleporter_name, Vector3 first_point, Vector3 second_point)
        {
            this.teleporter_name = teleporter_name;
            this.first_point     = first_point;
            this.second_point    = second_point;
        }

        public async void ActivateTeleporter()
        {
            while (true)
            {
                distance_to_first_point  = Vector3.Distance(Game.PlayerPed.Position, first_point);
                distance_to_second_point = Vector3.Distance(Game.PlayerPed.Position, second_point);

                if (distance_to_first_point <= 30 || distance_to_second_point <= 30)
                {
                    CheckTeleporterInputIfNecessary();
                    DrawTeleporterMarkerIfNecessary();
                    DrawTeleporterTextIfNecessary();
                    await Delay(5);

                }
                else
                {
                    await Delay(3000);
                }
            }
        }

        private async void CheckTeleporterInputIfNecessary()
        {
            if (distance_to_first_point <= 2 && API.IsControlJustPressed(1, 18))
            {
                await FadeAndTeleport(second_point);
            }
            else if (distance_to_second_point <= 2 && API.IsControlJustPressed(1, 18))
            {
                await FadeAndTeleport(first_point);
            }
        }

        private void DrawTeleporterMarkerIfNecessary()
        {
            if (distance_to_first_point <= 30 || distance_to_second_point <= 30)
            {
                
                World.DrawMarker(MarkerType.VerticalCylinder,  first_point, marker_dir, marker_rot, marker_scale, marker_color);
                World.DrawMarker(MarkerType.VerticalCylinder, second_point, marker_dir, marker_rot, marker_scale, marker_color);
            }
        }

        private void DrawTeleporterTextIfNecessary()
        {
            if (distance_to_first_point <= 2 || distance_to_second_point <= 2)
            {
                Shared.DrawTextSimple("Press ~g~Enter~w~");
            }
        }

        private async Task FadeAndTeleport(Vector3 gotoPos)
        {
            API.DoScreenFadeOut(1000);
            await Delay(1500);
            Game.PlayerPed.Position = gotoPos;
            API.DoScreenFadeIn(2500);
        }
    }
}
