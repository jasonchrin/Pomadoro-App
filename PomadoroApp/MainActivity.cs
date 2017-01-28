using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Timers;

namespace PomadoroApp
{
    public enum TimerType
    {
        POMODORO,
        SHORTBREAK,
        LONGBREAK
    }
    public enum TimerState
    {
        RUNNING,
        STOPPED
    }
    [Activity(Label = "Pomadoro App", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private int _shortBreak = 300;
        private int _longBreak =  900;
        private int _pomodroTime =  1500;
        private int _countseconds =  1500;
        private int _longBreakInterval = 3;
        private int _currentPomodoro = 1;
        private int _totalBreak = 1;
        private TimerType _currentTimer = TimerType.POMODORO;
        private TimerState _timerState = TimerState.STOPPED;


        private Timer _timer = new Timer();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            _timer.Interval = 1000;
            _timer.Elapsed += TimerElapsedEvent;
            SetContentView(Resource.Layout.Main);

            Button button = FindViewById<Button>(Resource.Id.StartButton);
            TextView timerText = FindViewById<TextView>(Resource.Id.TimerTextView);
            timerText.Text = secondsToCountdown();
            button.Click += delegate { TimerButtonClicked(); };
        }
        
        private void TimerButtonClicked()
        {
            if (_timerState == TimerState.STOPPED)
            {
                _timer.Start();
                _timerState = TimerState.RUNNING;
            }
            else
            {
                _timer.Stop();
                _timerState = TimerState.STOPPED;
                GetNextTimer();
            }
            RunOnUiThread(SetTimerButtonText);
        }
        private void TimerElapsedEvent(object sender, ElapsedEventArgs e)
        {
            _countseconds--;
            RunOnUiThread(DisplaySeconds);
            if (_countseconds == 0)
            {
                _timer.Stop();
                GetNextTimer();
                _timer.Start();
            }
        }

        private void GetNextTimer()
        {
            switch (_currentTimer)
            {
                case TimerType.POMODORO:
                    _currentPomodoro++;
                    
                    if ((_totalBreak % _longBreakInterval) == 0)
                    {
                        _currentTimer = TimerType.LONGBREAK;
                        _countseconds = _longBreak;
                    }
                    else
                    {
                        _currentTimer = TimerType.SHORTBREAK;
                        _countseconds = _shortBreak;
                    }
                    break;
                case TimerType.SHORTBREAK:
                    _totalBreak++;
                    _currentTimer = TimerType.POMODORO;
                    _countseconds = _pomodroTime;
                    
                    break;
                case TimerType.LONGBREAK:
                    _totalBreak++;
                    _currentTimer = TimerType.POMODORO;
                    _countseconds = _pomodroTime;
                    break;
            }
            RunOnUiThread(SetTimeerBackground);
        }
        private string secondsToCountdown()
        {
            int mins = _countseconds / 60;
            int seconds = _countseconds - mins * 60;
            return string.Format("{0}:{1}", mins.ToString("00"), seconds.ToString("00"));
        }

        private void DisplaySeconds()
        {
            TextView timerText = FindViewById<TextView>(Resource.Id.TimerTextView);
            timerText.Text = secondsToCountdown();
        }
        private void SetTimeerBackground()
        {
            TextView timerText = FindViewById<TextView>(Resource.Id.TimerTextView);
            if(_currentTimer == TimerType.POMODORO)
            {
                timerText.SetBackgroundColor(Android.Graphics.Color.Red);
            }
            else
            {
                timerText.SetBackgroundColor(Android.Graphics.Color.Blue);
            }
           
        }

        private void SetTimerButtonText()
        {
            DisplaySeconds();
            Button timerButton = FindViewById<Button>(Resource.Id.StartButton);
            if (_timerState == TimerState.STOPPED)
            {
                timerButton.Text = "Start Timer";
            }
            else
            {
                timerButton.Text = "Stop Timer";
            }
        }
    }
}

