using System;
using System.Collections.Generic;
using UnityEngine;

namespace QKit
{
    public class FixedInput
    {
        private static List<FixedInput> inputs = new();
        public FixedInput(string buttonName)
        {
            button = buttonName;
            inputs.Add(this);
        }

        public Action onPress, onHold, onRelease;

        private string button;

        private bool currentState;
        private bool lastState;

        public bool Press
        {
            get
            {
                return currentState == true && lastState == false;
            }
        }
        public bool Hold
        {
            get
            {
                return currentState == true && lastState == true;
            }
        }
        public bool Release
        {
            get
            {
                return currentState == false && lastState == true;
            }
        }

        public void ReadInput()
        {
            currentState = Input.GetButton(button) || currentState;
        }

        public void ResetInput()
        {
            currentState = false;
        }

        public void EatInput()
        {
            if (Press)
                onPress?.Invoke();
            if (Hold)
                onHold?.Invoke();
            if (Release)
                onRelease?.Invoke();

            lastState = currentState;

            ResetInput();
            ReadInput();
        }

        public static void ReadAll()
        {
            foreach (var input in inputs)
            {
                input.ReadInput();
            }
        }

        public static void EatAll()
        {
            foreach (var input in inputs)
            {
                input.EatInput();
            }
        }
    }
}
