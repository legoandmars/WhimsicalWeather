using WhimsicalWeather.Visuals;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WhimsicalWeather.Behaviours
{
    public class GameObjectActivityBehaviour : MonoBehaviour
    {
        public bool EventsEnabled = false;

        void OnEnable()
        {
            if (!EventsEnabled) return;
            WhimsicalVisuals.ToggleVisualsEvent(true);
        }

        void OnDisable()
        {
            if (!EventsEnabled) return;
            WhimsicalVisuals.ToggleVisualsEvent(false);
        }
    }
}
