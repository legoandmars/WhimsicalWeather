using LethalVision.Visuals;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Behaviours
{
    public class GameObjectActivityBehaviour : MonoBehaviour
    {
        public bool EventsEnabled = false;

        void OnEnable()
        {
            if (!EventsEnabled) return;
            LethalVisuals.ToggleVisualsEvent(true);
        }

        void OnDisable()
        {
            if (!EventsEnabled) return;
            LethalVisuals.ToggleVisualsEvent(false);
        }
    }
}
