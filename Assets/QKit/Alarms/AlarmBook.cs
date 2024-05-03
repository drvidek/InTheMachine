using System;
using System.Collections.Generic;
using UnityEngine;

namespace QKit
{
    [Serializable]
    public class AlarmBook<T>
    {
        public AlarmBook()
        {
            
        }

        private string name;

        private Dictionary<T, Alarm> alarmBook = new();


        public Alarm AddAlarm(T key, float time, bool looping)
        {
            Alarm alarm = new Alarm(time, looping);
            alarm.Stop();
            alarmBook.Add(key, alarm);
            return alarm;
        }

        public Alarm AddAlarmAndPlay(T key, float time, bool looping)
        {
            Alarm alarm = new Alarm(time, looping);
            alarmBook.Add(key, alarm);
            return alarm;
        }

        public Alarm GetAlarm(T key)
        {
            if (!alarmBook.TryGetValue(key, out Alarm alarm))
            {
                Debug.Log($"Alarm not found with key {key} in {name}");
                return null;
            }
            return alarm;
        }

        public void Tick(T key, float time)
        {
            if (!alarmBook.TryGetValue(key, out Alarm alarm))
            {
                Debug.LogError($"Alarm not found with key {key} in {name}");
                return;
            }
            alarm.Tick(time);
        }

        public void TickAll(float time)
        {
            foreach (Alarm alarm in alarmBook.Values)
            {
                alarm.Tick(time);
            }
        }

        public void StopAll()
        {
            foreach (Alarm alarm in alarmBook.Values)
            {
                alarm.Stop();
            }
        }

    }

}
