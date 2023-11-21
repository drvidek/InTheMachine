using System;
using System.Collections.Generic;
using UnityEngine;

namespace QKit
{
    [Serializable]
    public class ScoreKeeper : MonoBehaviour
    {
        #region Variables
        private static List<ScoreKeeper> _scoreKeepers = new();
        [SerializeField] private float _score;
        [SerializeField] private bool _useMultiplier;
        [SerializeField] float _multiplier = 1;
        [SerializeField] private float _multiplierMin = 1f;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the first ScoreKeeper created, usable like a singleton if only one ScoreKeeper exists.
        /// </summary>
        public static ScoreKeeper main => _scoreKeepers[0];

        /// <summary>
        /// Returns the current score of this ScoreKeeper.
        /// </summary>
        public float Score
        {
            get => _score; private set => _score = Mathf.Max(value, 0);
        }
        /// <summary>
        /// Returns the current multiplier value of this ScoreKeeper.
        /// </summary>
        public float Multiplier
        {
            get => _multiplier; private set => _multiplier = Mathf.Max(value, _multiplierMin);
        }
        #endregion

        #region Events
        /// <summary>
        /// Triggers when the score changes, with the value of the score change
        /// </summary>
        public Action<float> onScoreChange;
        /// <summary>
        /// Triggers when the multiplier changes, with the value of the change
        /// </summary>
        public Action<float> onMultiplierChange;
        #endregion

        private void OnValidate()
        {
            if (_multiplier < _multiplierMin)
                _multiplier = _multiplierMin;
        }

        private void Awake()
        {
            if (!_scoreKeepers.Contains(this))
                _scoreKeepers.Add(this);
        }

        #region Public Methods

        /// <summary>
        /// Increase score by f times multiplier, or decrease by f, never lower than zero. Optionally pass false to disable the multiplier.
        /// </summary>
        /// <param name="f"></param>
        public void AdjustScore(float f, bool multiplier = true)
        {
            float final = f * (multiplier && _useMultiplier && f > 0 ? Multiplier : 1);
            Score += final;
            onScoreChange?.Invoke(final);
        }

        /// <summary>
        /// Increases or decreases the multiplier by f, never lower than the multiplier minimum
        /// </summary>
        /// <param name="f"></param>
        public void AdjustMultiplier(float f)
        {
            Multiplier += f;
            onMultiplierChange?.Invoke(f);
        }

        /// <summary>
        /// Sets the score to f, never lower than zero
        /// </summary>
        /// <param name="f"></param>
        public void SetScore(float f)
        {
            float oldScore = _score;
            Score = f;
            onScoreChange?.Invoke(_score - oldScore);
        }

        /// <summary>
        /// Sets the multiplier to f, never lower than the multiplier minimum
        /// </summary>
        /// <param name="f"></param>
        public void SetMultiplier(float f)
        {
            float oldMulti = _multiplier;
            Multiplier = f;
            onMultiplierChange?.Invoke(_multiplier - oldMulti);
        }

        /// <summary>
        /// Resets score and multiplier to 0, with no event triggered
        /// </summary>
        public void Reset()
        {
            Score = 0;
            _multiplier = _multiplierMin;
        }
        #endregion

    }
}