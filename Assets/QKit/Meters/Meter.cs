using System;
using UnityEngine;

namespace QKit
{
    [Serializable]
    public class Meter
    {
        public enum ColorType
        {
            single,
            gradient
        }

        [SerializeField] protected float _min = 0;
        [SerializeField] protected float _max;
        [SerializeField] protected float _value;
        [SerializeField] protected float _rateUp = 1f;
        [SerializeField] protected float _rateDown = 1f;
        [SerializeField] private Color _colorMain, _colorBackground;
        [SerializeField] private Gradient _meterGradient;
        [SerializeField] private ColorType _colorType;

        public static Color defaultMeterColor = Color.cyan, defaultMeterBgColor = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        public static Gradient defaultGradient = new Gradient();

        private string _warningMeterRateZero => $"QKit > Meters\nThe {(_rateDown == 0 ? "downward" : "upward")} rate on the meter adjusted is set to 0, so no value change occured.";


        public Meter(float min, float max, float value, float rateUp = 1f, float rateDown = 1f)
        {
            _min = min;
            _max = max;
            _value = value;
            _rateUp = rateUp;
            _rateDown = rateDown;
            _colorType = ColorType.single;
            _colorMain = defaultMeterColor;
            _colorBackground = defaultMeterBgColor;
        }
        public Meter(float min, float max, float value, float rateUp, float rateDown, Color color, Color backgroundColor)
        {
            _min = min;
            _max = max;
            _value = value;
            _rateUp = rateUp;
            _rateDown = rateDown;
            _colorType = ColorType.single;
            _colorMain = color;
            _colorBackground = backgroundColor;
        }
        public Meter(float min, float max, float value, float rateUp, float rateDown, Gradient gradient, Color backgroundColor)
        {
            _min = min;
            _max = max;
            _value = value;
            _rateUp = rateUp;
            _rateDown = rateDown;
            _colorType = ColorType.gradient;
            _meterGradient = gradient;
            _colorBackground = backgroundColor;
        }

        /// <summary>
        /// Returns the minimum value of the meter
        /// </summary>
        public float Min { get => _min; }
        /// <summary>
        /// Returns the maximum value of the meter
        /// </summary>
        public float Max { get => _max; }
        /// <summary>
        /// Returns the current value of the meter
        /// </summary>
        public float Value { get => _value; }
        /// <summary>
        /// Returns how full the meter is, 0 being at minimum value and 1 being at maximum value
        /// </summary>
        public float Percent { get => (_value - _min) / (_max - _min); }
        /// <summary>
        /// Returns the rate the meter will adjust by when moving up
        /// </summary>
        public float RateUp { get => _rateUp; }
        /// <summary>
        /// Returns the rate the meter will adjust by when moving up
        /// </summary>
        public float RateDown { get => _rateDown; }
        /// <summary>
        /// Returns true if the meter is at or below minimum value
        /// </summary>
        public bool IsEmpty { get => _value <= _min; }
        /// <summary>
        /// Returns true if the meter is at or above maximum value
        /// </summary>
        public bool IsFull { get => _value >= _max; }
        /// <summary>
        /// Returns the numerical range of the meter
        /// </summary>
        public float Range { get => Mathf.Abs(_max - _min); }
        /// <summary>
        /// Returns the current color of the meter, either the single color or evaluating the gradient based on current value
        /// </summary>
        public Color CurrentColor => _colorType == ColorType.single ? _colorMain : _meterGradient.Evaluate(Percent);
        /// <summary>
        /// Returns the background color of the meter.
        /// </summary>
        public Color BackgroundColor => _colorBackground;

        /// <summary>
        /// Triggered when the meter reaches minimum value or lower
        /// </summary>
        public Action onMin;

        /// <summary>
        /// Triggered when the meter reaches maximum value or higher
        /// </summary>
        public Action onMax;

        /// <summary>
        /// Adjust the value of the meter by f, optionally disabling clamping to the min/max values
        /// </summary>
        /// <param name="f"></param>
        /// <param name="clamp"></param>
        public void Adjust(float f, bool clamp = true)
        {
            float rate = f > 0 ? _rateUp : _rateDown;
            f *= rate;

            if (f == 0 && rate == 0)
                Debug.LogWarning(_warningMeterRateZero);

            _value = clamp ? Mathf.Clamp(_value + f, _min, _max) : _value + f;

            if (f != 0)
                CheckForAction();
        }



        /// <summary>
        /// Adjust the value of the meter by f and out the overflow value, optionally disable clamping
        /// </summary>
        /// <param name="f"></param>
        /// <param name="overflow"></param>
        public void Adjust(float f, out float overflow, bool clamp = true)
        {
            float rate = f > 0 ? _rateUp : _rateDown;
            float n = f * rate;

            if (f == 0 && rate == 0)

                Debug.LogWarning(_warningMeterRateZero);

            _value += n;

            overflow = 0;

            if (_value < _min || _value > _max)
            {
                float checkPoint = _value < _min ? _min : _max;
                overflow = (checkPoint - _value) / rate;

                if (clamp)
                    _value = Mathf.Clamp(_value, _min, _max);
            }
            if (f != 0)

                CheckForAction();
        }

        /// <summary>
        /// Try to adjust the value of the meter by f, returns true if the adjusted value falls inside the meter bounds, else returns false and makes no adjustment. 
        /// </summary>
        /// <param name="f"></param>
        /// <param name="clamp"></param>
        public bool TryToAdjust(float f)
        {
            float rate = f > 0 ? _rateUp : _rateDown;
            f *= rate;

            if (f == 0 && rate == 0)
                Debug.LogWarning(_warningMeterRateZero);

            float newValue = _value + f;
            if (newValue > _max || newValue < _min)
            {
                return false;
            }
            _value = newValue;
            if (f != 0)

                CheckForAction();
            return true;
        }

        /// <summary>
        /// Try to adjust the value of the meter by f, returns true if the adjusted value falls inside the meter bounds, else returns false and makes no adjustment. Outputs the overflow amount.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="overflow"></param>
        public bool TryToAdjust(float f, out float overflow)
        {
            float rate = f > 0 ? _rateUp : _rateDown;
            float n = f * rate;

            if (f == 0 && rate == 0)
                Debug.LogWarning(_warningMeterRateZero);

            float newValue = _value + n;

            overflow = 0;

            if (newValue < _min || newValue > _max)
            {
                float checkPoint = newValue < _min ? _min : _max;
                overflow = (checkPoint - newValue) / rate;
                return false;
            }

            _value = newValue;
            if (f != 0)

                CheckForAction();
            return true;
        }

        /// <summary>
        /// Sets the meter to a value. Optionally disable clamping, or enable events.
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="clamp"></param>
        public void Set(float newValue, bool clamp = true, bool events = false)
        {
            _value = newValue;
            if (clamp) Clamp();
            if (events) CheckForAction();
        }

        /// <summary>
        /// Set the current value to the maximum value, and optionally disable triggering onMax event
        /// </summary>
        public void Fill(bool trigger = true)
        {
            _value = _max;
            if (trigger)
                CheckOnMax();
        }

        /// <summary>
        /// Set the current value to the minimum value, and optionally disable triggering onMin event
        /// </summary>
        /// <param name="trigger"></param>
        public void Empty(bool trigger = true)
        {
            _value = _min;
            if (trigger)
                CheckOnMin();
        }

        /// <summary>
        /// Increase the current value so it would take t seconds to reach full from empty. Optionally pass true to use rates, false disable clamping, and true to use fixed delta time. Only triggers onMax once.
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="fixedDelta"></param>
        public void FillOver(float t, bool useRates = false, bool clamp = true, bool fixedDelta = false)
        {
            float oldValue = _value;
            _value += (Range / t) * (useRates ? _rateUp : 1) * (fixedDelta ? Time.fixedDeltaTime : Time.deltaTime);
            if (clamp)
                _value = Mathf.Clamp(_value, _min, _max);
            if (oldValue < _max)
                CheckForAction();
        }

        /// <summary>
        /// Decrease the current value so it would take t seconds to reach empty from full. Optionally pass true to use rates, false disable clamping, and true to use fixed delta time. Only triggers onMin once.
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="fixedDelta"></param>
        public void EmptyOver(float t, bool useRates = false, bool clamp = true, bool fixedDelta = false)
        {
            float oldValue = _value;
            _value -= (Range / t) * (useRates ? _rateDown : 1) * (fixedDelta ? Time.fixedDeltaTime : Time.deltaTime);
            if (clamp)
                _value = Mathf.Clamp(_value, _min, _max);
            if (oldValue > _min)
                CheckForAction();
        }

        /// <summary>
        /// Clamp the current value between the minimum and maximum values
        /// </summary>
        public void Clamp()
        {
            _value = Mathf.Clamp(_value, _min, _max);
        }

        /// <summary>
        /// Set new minimum and maximum values for the meter, optionally disable clamping the current value between them
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="clamp"></param>
        public void SetNewBounds(float min, float max, bool clamp = true)
        {
            _min = min;
            _max = max;
            if (clamp)
                _value = Mathf.Clamp(_value, _min, _max);
        }

        /// <summary>
        /// Set new rates for increasing and decreasing the value
        /// </summary>
        /// <param name="down"></param>
        /// <param name="up"></param>
        public void SetNewRates(float down, float up)
        {
            _rateDown = down;
            _rateUp = up;
        }

        /// <summary>
        /// Checks if either min or max event should trigger
        /// </summary>
        private void CheckForAction()
        {
            CheckOnMin();
            CheckOnMax();
        }

        /// <summary>
        /// Checks if current is at or lower than minimum, and invokes onMin if true
        /// </summary>
        private void CheckOnMin()
        {
            if (_value > _min)
                return;
            onMin?.Invoke();
        }

        /// <summary>
        /// Checks if current is at or larger than maximum, and invokes onMax if true
        /// </summary>
        private void CheckOnMax()
        {
            if (_value < _max)
                return;
            onMax?.Invoke();
        }

        /// <summary>
        /// Set whether the meter uses a single color or a gradient
        /// </summary>
        /// <param name="type"></param>
        public void ChangeColorType(ColorType type)
        {
            _colorType = type;
        }

        /// <summary>
        /// Set the single color the meter will use when type is ColorType.Single
        /// </summary>
        /// <param name="color"></param>
        public void SetSingleColor(Color color)
        {
            _colorMain = color;
        }

        /// <summary>
        /// Set the background color of the meter
        /// </summary>
        /// <param name="color"></param>
        public void SetBackgroundColor(Color color)
        {
            _colorBackground = color;
        }

        /// <summary>
        /// Set the gradient the meter will use when type is ColorType.Gradient
        /// </summary>
        /// <param name="gradient"></param>
        public void SetGradient(Gradient gradient)
        {
            _meterGradient = gradient;
        }

    }
}