using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    [Serializable]
    public struct Timer
    {
        [SerializeField]
        private int hours;

        [SerializeField]
        private byte minutes;

        [SerializeField]
        private byte seconds;

        private TimeSpan timeSpan;

        [HideInInspector]
        public double totalSeconds;

        public int Hours
        {
            get { return this.timeSpan.Hours; }
            set
            {
                this.hours = value;
                SetTimer();
            }
        }

        public byte Minutes
        {
            get { return (byte)this.TimeSpan.Minutes; }
            set
            {
                this.minutes = value;
                SetTimer();
            }
        }

        public byte Seconds
        {
            get { return (byte)this.TimeSpan.Seconds; }
            set
            {
                this.seconds = value;
                SetTimer();
            }
        }

        public TimeSpan TimeSpan
        {
            get
            {
                return this.timeSpan;
            }
            set
            {
                this.timeSpan = value;
                this.totalSeconds = this.timeSpan.TotalSeconds;
            }
        }

        public override string ToString()
        {
            TimeSpan time = TimeSpan.FromSeconds(this.totalSeconds);

            return
                (time.Days * 24 + time.Hours).ToString("00") + ":" +
                time.Minutes.ToString("00") + ":" +
                time.Seconds.ToString("00");
        }

        public void SetTimer()
        {
            this.TimeSpan = new TimeSpan(this.hours, this.minutes, this.seconds);
        }
    }

    public Timer timer;
    public Text placeholder;
    public string separator = ":";

    public bool serverSync;
    public string urlServer = "http://nist.time.gov/actualtime.cgi?lzbc=siqm9b";

    public UnityEvent onFinish;

    private bool isStarting;
    private float timeForUpdate;

    private string ClearTimerString
    {
        get { return "00" + this.separator + "00" + this.separator + "00"; }
    }

    public void Awake()
    {
        StopCount();
        if (this.serverSync)
        {
            StartCoroutine(GetServerTime());
        }
    }

    public void Update()
    {
        if (this.isStarting)
        {
            // Reduce calling get checks for total seconds
            if (this.timer.totalSeconds > 0)
            {
                this.timeForUpdate += Time.deltaTime;

                if (this.timeForUpdate >= 0.98f)
                {
                    this.timer.totalSeconds -= this.timeForUpdate;
                    this.timeForUpdate = 0;
                    UpdateTimerPlaceHolder();
                }
            }
            else if (this.timer.totalSeconds <= 0)
            {
                this.timer.totalSeconds = 0;
                StopCount();
                this.onFinish.Invoke();
            }
        }
    }

    public void Reset()
    {
        this.placeholder = this.GetComponent<Text>();
    }

    [ContextMenu("Start Count")]
    public void StartCount()
    {
        this.isStarting = true;
        if (this.timer.totalSeconds == 0)
        {
            this.timer.SetTimer();
        }
        
        UpdateTimerPlaceHolder();
        Update();
    }

    [ContextMenu("Pause Count")]
    public void PauseCount()
    {
        this.isStarting = false;
        Update();
    }

    [ContextMenu("Stop Count")]
    public void StopCount()
    {
        this.isStarting = false;
        this.timer.totalSeconds = 0;
        this.placeholder.text = this.ClearTimerString;
    }

    [ContextMenu("Reset Count")]
    public void ResetCount()
    {
        this.timer.SetTimer();
        UpdateTimerPlaceHolder();
    }

    [ContextMenu("Start\\Pause Count")]
    public void ToggleCount()
    {
        if (!this.isStarting)
        {
            StartCount();
        }
        else
        {
            PauseCount();
        }
    }

    private void UpdateTimerPlaceHolder()
    {
        this.placeholder.text = this.timer.ToString().Replace(":", this.separator);
    }

    public IEnumerator GetServerTime()
    {
        WWW www = new WWW(this.urlServer);

        yield return www;

        Debug.Log(www.text);
    }

    public void TestOnFinish()
    {
        Debug.Log("END");
    }
}

