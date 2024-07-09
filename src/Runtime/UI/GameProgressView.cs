using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Markins.Runtime.Game.GUI.Views
{
    public class GameProgressView : MonoBehaviour
    {
        public TextMeshProUGUI Level;
        public TextMeshProUGUI NextLevel;
        public TextMeshProUGUI Score;
        public TextMeshProUGUI BestScore;

        [SerializeField] private Slider _progressSlider;

        private float _stepProgressBar;



        //oldScore,
        //newScore,
        //addScore,
        //TargetScore,
        //Сколько очков было, сколько очков стало, и сколько очков Добавили. И какая цель у очков. Так считает прогресс бар.


        public void SetBestScore(int bestScore)
        {
            BestScore.text = "Best: " + bestScore.ToString();
        }

        public void SetScoreForNextLevel(int newValue)
        {
            CalculateStepSlider(newValue);
        }

        public void UpdateScore(int score, int addScore)
        {
            Score.text = score.ToString();
            _progressSlider.value += addScore * _stepProgressBar;
        }

        public void LevelChanged(int level)
        {
            SetLevel(level);
            SetNextLevel(level);
            ResetSlider();
        }

        private void SetNextLevel(int level)
        {
            level++;
            NextLevel.text = level.ToString();
        }

        private void SetLevel(int level)
        {
            Level.text = level.ToString();
        }

        private void ResetSlider()
        {
            _progressSlider.value = 0;
        }

        private void CalculateStepSlider(int newValue)
        {
            _stepProgressBar = (float)1 / newValue;
        }

    }
}