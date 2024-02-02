using System.Collections.Generic;
using System;
using UnityEngine;
//using Unity.Android.Gradle;
//using UnityEngine.UIElements;

namespace QKit
{
    [Serializable]
    public class Alarm
    {
        /// <summary>
        /// Alarms instantiated by this constructor outside of Alarm.Get() will not work correctly. Use "Alarm myAlarm = Alarm.Get(f)" instead.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="t"></param>
        /// <param name="looping"></param>
        /// <param name="autoRelease"></param>
        /// <param name="scale"></param>
        /// <param name="type"></param>
        private Alarm(string name, float t, bool looping, bool autoRelease, float scale = 1f, Type type = Type.scaled)
        {
            _name = name;
            _timeMax = Mathf.Abs(t);
            _timeRemaining = t;
            _timeScale = scale;
            _autoRelease = autoRelease;
            _looping = looping;
            _type = type;

            _id = count;
            count++;

            if (!AlarmRunner.AlarmsInUse.Contains(this))
                AlarmRunner.AlarmsInUse.Add(this);

        }


        #region Variables
        [SerializeField] private string _name;
        [SerializeField] private int _id = -1;
        [SerializeField] private float _timeRemaining;
        [SerializeField] private float _timeMax;
        [SerializeField] private float _timeScale = 1f;
        [SerializeField] private bool _looping = false;
        [SerializeField] private bool _paused;
        [SerializeField] private bool _stopped;
        [SerializeField] private bool _autoRelease;

        public enum Type { scaled, unscaled, @fixed }
        [SerializeField] private Type _type;

        /// <summary>
        /// Triggered when the alarm reaches 0.
        /// </summary>
        public Action onComplete;
        /// <summary>
        /// Triggered when the alarm reaches 0.  Listening methods must accept an Alarm parameter, which will be this alarm.
        /// </summary>
        public Action<Alarm> onCompleteDestroy;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the name of the alarm, "New" by default.
        /// </summary>
        public string Name { get => _name; }
        /// <summary>
        /// Returns the ID and name of the alarm.
        /// </summary>
        public string Label { get => AlarmRunner.AlarmsFromInspector.Contains(this) ? _name : $"Alarm {_id}: {_name}"; }
        /// <summary>
        /// Return the time remaining, between 0 and maximum
        /// </summary>
        public float TimeRemaining { get => Mathf.Max(_timeRemaining, 0f); }
        /// <summary>
        /// Return the time remaining, between 0 and maximum, clipped to decimal places set in Alarm Options
        /// </summary>
        public float TimeRemainingClipped { get => Mathf.Max(ClipToDecimalPlace(_timeRemaining, alarmPrecision), 0f); }
        /// <summary>
        /// Return the maximum time for the alarm
        /// </summary>
        public float TimeMax { get => _timeMax; }
        /// <summary>
        /// Return the maximum time for the alarm, clipped to decimal places set in Alarm Options
        /// </summary>
        public float TimeMaxClipped { get => ClipToDecimalPlace(_timeMax, alarmPrecision); }
        /// <summary>
        /// Return the time scale of the alarm
        /// </summary>
        public float TimeScale { get => _timeScale; }
        /// <summary>
        /// Returns true if the timer is neither paused nor stopped and there is time remaining
        /// </summary>
        public bool IsPlaying { get => !_paused && !_stopped && (_timeRemaining > 0 || _looping); }

        public Type AlarmType => _type;
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
        /// Returns true if the alarm will auto-release
        /// </summary>
        public bool AutoRelease { get => _autoRelease; }
        /// <summary>
        /// Returns the percent of time left in the alarm, starting at 1 and approaching 0 as the alarm runs
        /// </summary>
        public float PercentRemaining { get => _timeRemaining / _timeMax; }
        /// <summary>
        /// Returns the percent of time completed in the alarm, starting at 0 and approaching 1 as the alarm runs
        /// </summary>
        public float PercentComplete { get => 1 - _timeRemaining / _timeMax; }
        #endregion

        public static int alarmPrecision = 2;
        public static bool disableAllAutoRelease = false;
        public static bool disableAllComplete = false;
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
                AttemptComplete(disableAllComplete);
            }

            if (_timeRemaining <= 0)
            {
                ManageAlarmAtZero();
                return;
            }

            _timeRemaining = Mathf.MoveTowards(_timeRemaining, 0, _timeScale * (_type == Type.scaled ? Time.deltaTime : _type == Type.unscaled ? Time.unscaledDeltaTime : Time.fixedDeltaTime));

            if (_timeScale == 0)
            {
                Debug.LogWarning($"QKit Alarm->The alarm {Name} has a timescale of 0, so it is not running.");
            }
        }

        private void AttemptComplete(bool disable)
        {
            if (disable)
                return;

            onComplete?.Invoke();
            onCompleteDestroy?.Invoke(this);
        }

        private void ManageAlarmAtZero()
        {
            if (_looping && !_stopped)
            {
                Reset();
                return;
            }

            if (_autoRelease && !disableAllAutoRelease)
            {
                Release();
                return;
            }

            Stop();
        }
        private void ApplyAlarmProperties(string name, float t, bool looping, bool autoRelease, float scale, Type type, bool playing)
        {
            _name = name;
            _timeMax = Mathf.Abs(t);
            _timeScale = scale;
            _autoRelease = autoRelease;
            _looping = looping;
            _type = type;
            _timeRemaining = t;
            _stopped = false;
            _paused = !playing;

        }
        private static void CheckForAlarmRunner()
        {
            if (AlarmRunner.Singleton == null && Application.isPlaying)
            {
                AlarmRunner.CreateAlarmRunner();
                Debug.LogWarning("No AlarmRunner found in scene. Spawned an AlarmRunner object.\n\nIf you don't have an AlarmRunner in your scene, Alarms will not work.");
            }
        }

        private static Alarm CheckPoolForAlarm()
        {
            if (AlarmRunner.AlarmsInUse.Count < AlarmRunner.AlarmPool.Count)  //if we have more alarms available than in use, we can grab from the pool
            {
                foreach (var curAlarm in AlarmRunner.AlarmPool)    //check the pool for the first alarm not in use
                {
                    if (!AlarmRunner.AlarmsInUse.Contains(curAlarm) && !AlarmRunner.AlarmsFromInspector.Contains(curAlarm))
                    {
                        return curAlarm;
                    }
                }
            }
            return null;
        }

        private static Alarm GetNewAlarm(float t, bool looping, bool autoRelease, float scale, Type type)
        {
            Alarm alarm = new("New", t, looping, autoRelease, scale, type);
            AlarmRunner.AlarmPool.Add(alarm);
            alarm._stopped = false;
            alarm._paused = true;
            return alarm;
        }

        private static Alarm GetNewAlarm(float t, string name, bool looping, bool autoRelease, float scale, Type type)
        {
            Alarm alarm = new(name, t, looping, autoRelease, scale, type);
            AlarmRunner.AlarmPool.Add(alarm);
            alarm._stopped = false;
            alarm._paused = true;
            return alarm;
        }

        private static Alarm GetAndPlayNewAlarm(float t, bool looping, bool autoRelease, float scale, Type type)
        {
            Alarm alarm = new("New", t, looping, autoRelease, scale, type);
            AlarmRunner.AlarmPool.Add(alarm);
            alarm._stopped = false;
            alarm._paused = false;
            return alarm;
        }

        private static Alarm GetAndPlayNewAlarm(float t, string name, bool looping, bool autoRelease, float scale, Type type)
        {
            Alarm alarm = new(name, t, looping, autoRelease, scale, type);
            AlarmRunner.AlarmPool.Add(alarm);
            alarm._stopped = false;
            alarm._paused = false;
            return alarm;
        }
        #endregion

        #region Public Methods

        #region Start and stop
        /// <summary>
        /// Play the alarm from its current time (an alarm at 0sec will loop/release but will not trigger the Complete event).
        /// </summary>
        public void Play()
        {
            _paused = false;
            _stopped = false;
        }

        /// <summary>
        /// Pause the alarm at its current time.
        /// </summary>
        public void Pause()
        {
            _paused = true;
        }

        /// <summary>
        /// Stop the alarm and set it to 0 (this will not release the alarm).
        /// </summary>
        public void Stop()
        {
            _stopped = true;
            _timeRemaining = -1;
        }

        /// <summary>
        /// Reset the alarm to maximum value.
        /// </summary>
        public void Reset()
        {
            _timeRemaining = _timeMax;
        }

        /// <summary>
        /// Reset the alarm and its maximum to t.
        /// </summary>
        public void Reset(float t)
        {
            _timeMax = t;
            _timeRemaining = _timeMax;
        }

        /// <summary>
        /// Reset the alarm to maximum value and start it.
        /// </summary>
        public void ResetAndPlay()
        {
            Reset();
            Play();
        }

        /// <summary>
        /// Reset the alarm and its maximum to t, and start it.
        /// </summary>
        public void ResetAndPlay(float t)
        {
            Reset(t);
            Play();
        }

        #endregion

        #region Alarm settings
        /// <summary>
        /// Adds or subtracts t seconds, clamping between 0 and maximum. Hitting 0 will trigger Complete.
        /// </summary>
        /// <param name="t"></param>
        public void AdjustTimeRemaining(float t)
        {
            _timeRemaining = Mathf.Clamp(_timeRemaining + t, 0, _timeMax);
        }

        /// <summary>
        /// Sets the time remaining to t seconds, clamping between 0 and maximum.
        /// </summary>
        /// <param name="t"></param>
        public void SetTimeRemaining(float t)
        {
            _timeRemaining = Mathf.Clamp(t, 0, _timeMax);
        }

        /// <summary>
        /// Sets the maximum time for the alarm to t seconds. If time remaning is larger than t, it will be set to t.
        /// </summary>
        /// <param name="t"></param>
        public void SetTimeMaximum(float t)
        {
            _timeMax = t;
            _timeRemaining = Mathf.Clamp(_timeRemaining, 0, _timeMax);
        }

        /// <summary>
        /// Set whether the alarm loops automatically on completion.
        /// </summary>
        /// <param name="loop"></param>
        public void SetLooping(bool loop)
        {
            _looping = loop;
        }

        /// <summary>
        /// Set whether the alarm automatically releases on completion.
        /// </summary>
        /// <param name="release"></param>
        public void SetAutoRelease(bool release)
        {
            _autoRelease = release;
        }
        #endregion

        /// <summary>
        /// Release a poolable alarm back to the pool.  
        /// </summary>
        public void Release()
        {
            if (AlarmRunner.AlarmsFromInspector.Contains(this))
                return;

            onComplete = null;
            onCompleteDestroy = null;
            AlarmRunner.AlarmsInUse.Remove(this);
            if (AlarmRunner.AlarmPool.Count > AlarmRunner.maxAlarmsAllowed)
            {
                AlarmRunner.AlarmPool.Remove(this);
            }
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Set an alarm for t seconds, optionally set if it should loop or auto-release on complete, and what time scale to use.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="looping"></param>
        /// <param name="scale"></param>
        /// <param name="autoRelease"></param>
        /// <returns></returns>
        public static Alarm Get(float t, bool looping = false, bool autoRelease = true, float scale = 1f, Type type = Type.scaled)
        {
            CheckForAlarmRunner();

            Alarm alarm = CheckPoolForAlarm();

            if (alarm == null)
            {
                return GetNewAlarm(t, looping, autoRelease, scale, type);
            }

            alarm.ApplyAlarmProperties("New", t, looping, autoRelease, scale, type, false);


            if (!AlarmRunner.AlarmsInUse.Contains(alarm))
                AlarmRunner.AlarmsInUse.Add(alarm);

            return alarm;
        }

        /// <summary>
        /// Set an alarm for t seconds and name it, optionally set if it should loop or auto-release on complete, and what time scale to use.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="name"></param>
        /// <param name="looping"></param>
        /// <param name="autoRelease"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Alarm Get(float t, string name, bool looping = false, bool autoRelease = true, float scale = 1f, Type type = Type.scaled)
        {
            CheckForAlarmRunner();

            Alarm alarm = CheckPoolForAlarm();

            if (alarm == null)
            {
                return GetNewAlarm(t, name, looping, autoRelease, scale, type);
            }

            alarm.ApplyAlarmProperties(name, t, looping, autoRelease, scale, type, false);

            if (!AlarmRunner.AlarmsInUse.Contains(alarm))
                AlarmRunner.AlarmsInUse.Add(alarm);

            return alarm;
        }
        /// <summary>
        /// Set and start an alarm for t seconds, optionally set if it should loop or auto-release on complete, and what time scale to use.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="looping"></param>
        /// <param name="autoRelease"></param>
        /// <param name="scale"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Alarm GetAndPlay(float t, bool looping = false, bool autoRelease = true, float scale = 1f, Type type = Type.scaled)
        {
            CheckForAlarmRunner();

            Alarm alarm = CheckPoolForAlarm();

            if (alarm == null)
            {
                return GetAndPlayNewAlarm(t, "New", looping, autoRelease, scale, type);
            }

            alarm.ApplyAlarmProperties("New", t, looping, autoRelease, scale, type, true);

            if (!AlarmRunner.AlarmsInUse.Contains(alarm))
                AlarmRunner.AlarmsInUse.Add(alarm);

            return alarm;
        }
        /// <summary>
        /// Set and start an alarm for t seconds and name it, optionally set if it should loop or auto-release on complete, and what time scale to use.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="name"></param>
        /// <param name="looping"></param>
        /// <param name="autoRelease"></param>
        /// <param name="scale"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Alarm GetAndPlay(float t, string name, bool looping = false, bool autoRelease = true, float scale = 1f, Type type = Type.scaled)
        {
            CheckForAlarmRunner();

            Alarm alarm = CheckPoolForAlarm();

            if (alarm == null)
            {
                return GetAndPlayNewAlarm(t, name, looping, autoRelease, scale, type);
            }

            alarm.ApplyAlarmProperties("New", t, looping, autoRelease, scale, type, true);

            if (!AlarmRunner.AlarmsInUse.Contains(alarm))
                AlarmRunner.AlarmsInUse.Add(alarm);

            return alarm;
        }

        /// <summary>
        /// Initialise an alarm using values set in the inspector. Optionally set a name for it.
        /// </summary>
        /// <param name="alarm"></param>
        /// <param name="name"></param>
        public static void Add(Alarm alarm, string name = "Alarm from inspector")
        {
            CheckForAlarmRunner();
            alarm._name = name;
            AlarmRunner.AlarmsInUse.Add(alarm);
            AlarmRunner.AlarmsFromInspector.Add(alarm);
        }

        /// <summary>
        /// Initialise multiple alarms at once using values set in the inspector.
        /// </summary>
        /// <param name="alarm"></param>
        /// <param name="name"></param>
        public static void Add(params Alarm[] alarms)
        {
            CheckForAlarmRunner();
            foreach (var alarm in alarms)
            {
                alarm._name = "Alarm from inspector";
                AlarmRunner.AlarmsInUse.Add(alarm);
                AlarmRunner.AlarmsFromInspector.Add(alarm);
            }
        }

        #region Act on All
        /// <summary>
        /// Stops all alarms and sets their time to 0 (this will not trigger release).
        /// </summary>
        public static void StopAll()
        {
            foreach (var alarm in AlarmRunner.AlarmsInUse)
            {
                alarm.Stop();
            }
        }
        /// <summary>
        /// Pause all alarms at their current time.
        /// </summary>
        public static void PauseAll()
        {
            foreach (var alarm in AlarmRunner.AlarmsInUse)
            {
                alarm.Pause();
            }
        }
        /// <summary>
        /// Play all alarms from their current time.
        /// </summary>
        public static void PlayAll()
        {
            foreach (var alarm in AlarmRunner.AlarmsInUse)
            {
                alarm.Play();
            }
        }

        /// <summary>
        /// Reset all alarms to their max time.
        /// </summary>
        public static void ResetAll()
        {
            foreach (var alarm in AlarmRunner.AlarmsInUse)
            {
                alarm.Reset();
            }
        }

        /// <summary>
        /// Release all alarms.
        /// </summary>
        public static void ReleaseAll()
        {
            for (int i = 0; i < AlarmRunner.AlarmsInUse.Count; i++)
            {
                if (AlarmRunner.AlarmsFromInspector.Contains(AlarmRunner.AlarmsInUse[i]))
                    continue;

                AlarmRunner.AlarmsInUse[i].Release();
                i--;
            }
        }

        #endregion

        #endregion

    }

    public class AlarmRunner : MonoBehaviour
    {

        #region Pooling
        private static List<Alarm> _alarmPool = new List<Alarm>();
        private static List<Alarm> _alarmsInUse = new List<Alarm>();
        private static List<Alarm> _alarmsFromInspector = new();
        /// <summary>
        /// A list of all alarms currently in use.
        /// </summary>
        public static List<Alarm> AlarmsInUse
        {
            get => _alarmsInUse;
        }
        public static List<Alarm> AlarmPool => _alarmPool;
        public static List<Alarm> AlarmsFromInspector => _alarmsFromInspector;
        public static int maxAlarmsAllowed = 10;
        #endregion


        #region Singleton + Awake
        private static AlarmRunner _singleton = null;
        public static AlarmRunner Singleton
        {
            get => _singleton;
            private set
            {
                if (_singleton == null)
                {
                    _singleton = value;
                }
                else if (_singleton != value)
                {
                    Debug.LogWarning("AlarmManager instance already exists, destroy duplicate!");
                    Destroy(value.gameObject);
                }
            }
        }

        private void Awake()
        {
            Singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion
        void Update()
        {
            Tick();
        }

        private void FixedUpdate()
        {
            TickFixed();
        }

        /// <summary>
        /// Mandatory to run all non-fixed alarms
        /// </summary>
        private void Tick()
        {
            for (int i = 0; i < _alarmsInUse.Count; i++)
            {
                Alarm alarm = _alarmsInUse[i];
                alarm.Run();
                if (!_alarmsInUse.Contains(alarm))
                {
                    i--;
                }
            }
        }

        /// <summary>
        /// Mandatory to run all fixed alarms
        /// </summary>
        private void TickFixed()
        {
            for (int i = 0; i < _alarmsInUse.Count; i++)
            {
                Alarm alarm = _alarmsInUse[i];
                alarm.RunFixed();
                if (!_alarmsInUse.Contains(alarm))
                {
                    i--;
                }
            }
        }

        /// <summary>
        /// Creates a game object with a AlarmRunner in the scene. One AlarmRunner in a scene is mandatory to make alarms work.
        /// </summary>
        public static void CreateAlarmRunner()
        {
            if (_singleton == null)
            {
                GameObject obj = new GameObject();
                obj.AddComponent<AlarmRunner>();
                obj.name = "AlarmRunner";
            }
        }

    }
}
