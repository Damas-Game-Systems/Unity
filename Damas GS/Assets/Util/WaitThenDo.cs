using System;
using System.Collections;
using UnityEngine;

public class WaitThenDo
{
    private readonly MonoBehaviour runner;

    private readonly Func<bool> conditionsBeenMet;
    private readonly Action onConditionsMet;

    private readonly float timeout;
    private readonly Action onTimeout;

    private Coroutine process;
    private float timeRemaining;

    public bool IsStarted { get; private set; }
    public bool IsFinished { get; private set; }
    public bool BeenCanceled { get; private set; }

    public WaitThenDo(MonoBehaviour runner, float timeout, Action onTimeout)
        : this (
              runner,
              () => false,
              () => { },
              timeout,
              onTimeout
        )
    { }

    public WaitThenDo(
        MonoBehaviour runner,
        Func<bool> conditionsBeenMet,
        Action onConditionsMet,
        float timeout,
        Action onTimeout)
    {
        this.runner = runner;
        this.conditionsBeenMet = conditionsBeenMet;
        this.onConditionsMet = onConditionsMet;
        this.timeout = timeout;
        this.onTimeout = onTimeout;

        IsStarted = false;
        IsFinished = false;
        BeenCanceled = false;
    }

    public void Start()
    {
        IsStarted = true;
        IsFinished = false;
        BeenCanceled = false;
        process = runner.StartCoroutine(Process());
    }

    public void Cancel()
    {
        BeenCanceled = true;
        if (process != null)
        {
            runner.StopCoroutine(process);
            process = null;
        }
    }

    private IEnumerator Process()
    {
        timeRemaining = timeout;
        while (timeRemaining > 0)
        {
            if (BeenCanceled)
            {
                timeRemaining = 0;
                IsFinished = true;
                yield break;
            }
            else if (conditionsBeenMet())
            {
                onConditionsMet();
                timeRemaining = 0;
                IsFinished = true;
                yield break;
            }

            float frame = Time.deltaTime;
            timeRemaining -= frame;
            yield return null;
        }

        onTimeout();
        IsFinished = true;
    }
}
