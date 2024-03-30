using System.Collections.Generic;
using UnityEngine;

namespace QKit
{
    public class AlarmPool : MonoBehaviour
    {

        #region Pooling
        private static List<AlarmForPool> _alarmPool = new List<AlarmForPool>();
        private static List<AlarmForPool> _alarmsInUse = new List<AlarmForPool>();
        /// <summary>
        /// A list of all alarms currently in use.
        /// </summary>
        public static List<AlarmForPool> AlarmsInUse
        {
            get => _alarmsInUse;
        }

        public static int maxAlarmsAllowedInPool = 10;
        public static int currentAlarmsInPool => _alarmPool.Count;
        public static int currentAlarmsInUse => _alarmsInUse.Count;
        #endregion

        #region Singleton + Awake
        private static AlarmPool _singleton = null;
        public static AlarmPool Singleton
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

        private void OnDestroy()
        {
            _singleton = null;
        }

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
            for (int i = 0; i < AlarmsInUse.Count; i++)
            {
                AlarmForPool alarm = AlarmsInUse[i];
                alarm.Run();
                if (!AlarmsInUse.Contains(alarm))
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
            for (int i = 0; i < AlarmsInUse.Count; i++)
            {
                AlarmForPool alarm = AlarmsInUse[i];
                alarm.RunFixed();
                if (!AlarmsInUse.Contains(alarm))
                {
                    i--;
                }
            }
        }

        private static void CheckForAlarmRunner()
        {
            if (_singleton == null)
            {
                CreateAlarmRunner();
                Debug.LogWarning("No AlarmPool found in scene. Spawned an AlarmPool object.");
            }
        }

        private static void CreateAlarmRunner()
        {
            if (!Application.isPlaying)
                return;
            if (_singleton == null)
            {
                GameObject obj = new GameObject();
                obj.AddComponent<AlarmPool>();
                obj.name = "AlarmRunner";
            }
        }

        public static AlarmForPool Get(float time, AlarmForPool.Type type = AlarmForPool.Type.scaled)
        {
            CheckForAlarmRunner();

            AlarmForPool alarm = CheckPoolForAlarms() as AlarmForPool;

            if (alarm == null)
            {
                alarm = new(time, false, "New", type);
                _alarmPool.Add(alarm);
                _alarmsInUse.Add(alarm);
                alarm.Pause();
                return alarm;
            }

            alarm.name = "New";
            alarm.onComplete = null;
            alarm.Reset(time);
            alarm.Pause();

            if (!_alarmsInUse.Contains(alarm))
                _alarmsInUse.Add(alarm);

            return alarm;
        }

        public static AlarmForPool Get(float time, string name, AlarmForPool.Type type = AlarmForPool.Type.scaled)
        {
            CheckForAlarmRunner();

            AlarmForPool alarm = CheckPoolForAlarms() as AlarmForPool;

            if (alarm == null)
            {
                alarm = new(time, false, name, type);
                _alarmPool.Add(alarm);
                _alarmsInUse.Add(alarm);
                alarm.Pause();
                return alarm;
            }

            alarm.name = name;
            alarm.onComplete = null;
            alarm.Reset(time);
            alarm.Pause();

            if (!_alarmsInUse.Contains(alarm))
                _alarmsInUse.Add(alarm);

            return alarm;
        }

        public static AlarmForPool GetAndPlay(float time, AlarmForPool.Type type = AlarmForPool.Type.scaled)
        {
            AlarmForPool alarm = Get(time, type);
            alarm.Play();
            return alarm;
        }

        public static AlarmForPool GetAndPlay(float time, string name, AlarmForPool.Type type = AlarmForPool.Type.scaled)
        {
            AlarmForPool alarm = Get(time, name, type);
            alarm.Play();
            return alarm;
        }

        private static Alarm CheckPoolForAlarms()
        {
            if (_alarmsInUse.Count < _alarmPool.Count)  //if we have more alarms available than in use, we can grab from the pool
            {
                foreach (var alarm in _alarmPool)    //check the pool for the first alarm not in use
                {
                    if (!_alarmsInUse.Contains(alarm))
                    {
                        return alarm;
                    }
                }
            }
            return null;
        }

        public static void Release(AlarmForPool alarm)
        {
            alarm.onComplete = null;
            _alarmsInUse.Remove(alarm);
            if (currentAlarmsInPool > maxAlarmsAllowedInPool)
            {
                _alarmPool.Remove(alarm);
            }
        }

        #region Act on All
        /// <summary>
        /// Stops all alarms and sets their time to 0 (this will not trigger release).
        /// </summary>
        public static void StopAll()
        {
            foreach (var alarm in _alarmsInUse)
            {
                alarm.Stop();
            }
        }
        /// <summary>
        /// Pause all alarms at their current time.
        /// </summary>
        public static void PauseAll()
        {
            foreach (var alarm in _alarmsInUse)
            {
                alarm.Pause();
            }
        }
        /// <summary>
        /// Play all alarms from their current time.
        /// </summary>
        public static void PlayAll()
        {
            foreach (var alarm in _alarmsInUse)
            {
                alarm.Play();
            }
        }

        /// <summary>
        /// Reset all alarms to their max time.
        /// </summary>
        public static void ResetAll()
        {
            foreach (var alarm in _alarmsInUse)
            {
                alarm.Reset();
            }
        }

        /// <summary>
        /// Release all alarms.
        /// </summary>
        public static void ReleaseAll()
        {
            for (int i = 0; i < _alarmsInUse.Count; i++)
            {
                Release(_alarmsInUse[i]);
                i--;
            }
        }

        #endregion
    }
}
