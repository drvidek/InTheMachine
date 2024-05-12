using System;
using System.Collections.Generic;
using UnityEngine;

namespace QKit
{
    [Serializable]
    public class Alarm
    {
        public Alarm(float time, bool looping)
        {
            _timeMax = time;
            _timeRemaining = time;
            _looping = looping;
        }

        [SerializeField] protected float _timeRemaining;
        [SerializeField] protected float _timeMax;
        [SerializeField] protected bool _looping = false;
        [SerializeField] protected bool _paused;
        [SerializeField] protected bool _stopped;

        public enum Type { scaled, unscaled, @fixed }
        [SerializeField] protected Type _type;

        public Action onComplete;

        #region Properties
        /// <summary>
        /// Return the time remaining, between 0 and maximum
        /// </summary>
        public float TimeRemaining { get => Mathf.Max(_timeRemaining, 0f); }
        /// <summary>
        /// Return the maximum time for the alarm
        /// </summary>
        public float TimeMax { get => _timeMax; }
        /// <summary>
        /// Returns true if the timer is neither paused nor stopped and there is time remaining
        /// </summary>
        public bool IsPlaying { get => !_paused && !_stopped && (_timeRemaining > 0 || _looping); }
        /// <summary>
        /// Returns true if the alarm is paused
        /// </summary>
        public bool IsPaused { get => _paused; }
        /// <summary>
        /// Returns true if the alarm is stopped
        /// </summary>
        public bool IsStopped { get => _stopped; }
        /// <summary>
        /// Returns true if the alarm is looping
        /// </summary>
        public bool Looping { get => _looping; }
        /// <summary>
        /// Returns the percent of time left in the alarm, starting at 1 and approaching 0 as the alarm runs
        /// </summary>
        public float PercentRemaining { get => _timeRemaining / _timeMax; }
        /// <summary>
        /// Returns the percent of time completed in the alarm, starting at 0 and approaching 1 as the alarm runs
        /// </summary>
        public float PercentComplete { get => 1 - _timeRemaining / _timeMax; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Move the alarm towards 0 by time, and trigger its complete event. Does not affect paused or stopped alarms.
        /// </summary>
        /// <param name="time"></param>
        public void Tick(float time)
        {
            if (_paused || _stopped)
                return;

            if (_timeRemaining <= 0)
            {
                if (Looping)
                {
                    _timeRemaining = _timeMax + _timeRemaining;
                }
                else
                {
                    _timeRemaining = 0;
                    _stopped = true;
                }
                onComplete?.Invoke();
            }

            _timeRemaining -= time;
        }

        /// <summary>
        /// Start an alarm. Will not start an alarm with 0 time remaining.
        /// </summary>
        public Alarm Play()
        {
            _paused = false;
            _stopped = false;

            if (_timeRemaining <= 0)
            {
                _stopped = true;
            }
            return this;
        }

        /// <summary>
        /// Pause an alarm
        /// </summary>
        public Alarm Pause()
        {
            _paused = true;
            return this;
        }

        /// <summary>
        /// Reset the remaining time of the alarm.
        /// </summary>
        public Alarm Reset()
        {
            _timeRemaining = _timeMax;
            return this;
        }

        /// <summary>
        /// Reset the remaining time of the alarm using the new maximum value.
        /// </summary>
        /// <param name="newMax"></param>
        public Alarm Reset(float newMax)
        {
            _timeMax = newMax;
            _timeRemaining = _timeMax;
            return this;
        }

        /// <summary>
        /// Reset the alarm and play it.
        /// </summary>
        public Alarm ResetAndPlay()
        {
            Reset();
            return Play();
        }

        /// <summary>
        /// Reset the alarm to the new maximum amount and start it.
        /// </summary>
        /// <param name="newMax"></param>
        public Alarm ResetAndPlay(float newMax)
        {
            Reset(newMax);
            return Play();
        }

        /// <summary>
        /// Stop an alarm and reduce its remaining time to 0
        /// </summary>
        public Alarm Stop()
        {
            _stopped = true;
            _timeRemaining = 0;
            return this;
        }


        #region Alarm settings
        /// <summary>
        /// Adds or subtracts t seconds, clamping between 0 and maximum. Hitting 0 will trigger Complete.
        /// </summary>
        /// <param name="t"></param>
        public Alarm AdjustTimeRemaining(float t)
        {
            _timeRemaining = Mathf.Clamp(_timeRemaining + t, 0, _timeMax);
            return this;
        }

        /// <summary>
        /// Sets the time remaining to t seconds, clamping between 0 and maximum.
        /// </summary>
        /// <param name="t"></param>
        public Alarm SetTimeRemaining(float t)
        {
            _timeRemaining = Mathf.Clamp(t, 0, _timeMax);
            return this;
        }

        /// <summary>
        /// Sets the maximum time for the alarm to t seconds. If time remaning is larger than t, it will be set to t.
        /// </summary>
        /// <param name="t"></param>
        public Alarm SetTimeMaximum(float t)
        {
            _timeMax = t;
            _timeRemaining = Mathf.Clamp(_timeRemaining, 0, _timeMax);
            return this;
        }

        /// <summary>
        /// Set whether the alarm loops automatically on completion.
        /// </summary>
        /// <param name="loop"></param>
        public Alarm SetLooping(bool loop)
        {
            _looping = loop;
            return this;
        }

        #endregion
        #endregion
    }
}