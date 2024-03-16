using System;
using System.Collections.Generic;
using UnityEngine;

namespace QKit
{
    [Serializable]
    public class AlarmForPool : Alarm
    {
        /// <summary>
        /// Alarms instantiated by this constructor outside of AlarmPool.Get() will not work correctly. Use "Alarm myAlarm = AlarmPool.Get(f)" instead.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="t"></param>
        /// <param name="looping"></param>
        /// <param name="type"></param>
        public AlarmForPool(float time, bool looping, string name,  Type type = Type.scaled) : base(time, looping)
        {
            this.name = name;
            _timeMax = Mathf.Abs(time);
            _timeRemaining = time;
            _looping = looping;
            _type = type;

            _id = count;
            count++;
        }

        #region Variables
        [SerializeField] public string name;
        [SerializeField] private int _id = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the name of the alarm, "New" by default.
        /// </summary>
        public string Name { get => name; }
        /// <summary>
        /// Returns the ID and name of the alarm.
        /// </summary>
        public string Label { get => $"Alarm {_id}: {name}"; }
        
        public float TimeRemainingClipped { get => Mathf.Max(ClipToDecimalPlace(_timeRemaining, alarmPrecision), 0f); }
        /// <summary>
        /// Return the maximum time for the alarm
        /// </summary>
        
        public float TimeMaxClipped { get => ClipToDecimalPlace(_timeMax, alarmPrecision); }
     
        public Type AlarmType => _type;
       
        #endregion

        public static int alarmPrecision = 2;
        private static int count;

        #region Private Methods
        /// <summary>
        /// Runs the alarm for one frame (will not affect a fixed alarm). Calling this outside AlarmRunner will cause alarms to run faster than intended.
        /// </summary>
        public void Run()
        {
            if (_type == Type.@fixed)
                return;

            TickToZero();
        }

        /// <summary>
        /// Runs the alarm for one fixed update length (will not affect scaled and unscaled alarms).  Calling this outside AlarmRunner will cause alarms to run faster than intended.
        /// </summary>
        public void RunFixed()
        {
            if (_type != Type.@fixed)
                return;
            TickToZero();
        }

        private float ClipToDecimalPlace(float t, float decimals)
        {
            float precision = Mathf.Max(1, Mathf.Pow(10, decimals));
            return Mathf.Ceil(t * precision) / precision;
        }

        private void TickToZero()
        {
            if (_paused || _stopped)
                return;

            if (_timeRemaining == 0)
            {
                onComplete?.Invoke();
                AlarmPool.Release(this);
                return;
            }

            _timeRemaining = Mathf.MoveTowards(_timeRemaining, 0, (_type == Type.scaled ? Time.deltaTime : _type == Type.unscaled ? Time.unscaledDeltaTime : Time.fixedDeltaTime));
        }
        #endregion
    }

}