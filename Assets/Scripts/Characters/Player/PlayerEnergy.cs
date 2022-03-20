using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : Singleton<PlayerEnergy>
{
    [SerializeField] EnergyBar energyBar;
    [SerializeField] float overdriveInterval = 0.1f;

    public const int MAX = 100;
    public const int PERCENT = 1;
    bool available = true;
    WaitForSeconds waitForOverdriveInteval;

    int energy;

    protected override void Awake()
    {
        base.Awake();
        waitForOverdriveInteval = new WaitForSeconds(overdriveInterval);
    }

    private void OnEnable()
    {
        PlayerOverdrive.on += PlayerOverdriveOn;
        PlayerOverdrive.off += PlayerOverdriveOff;
    }

    private void OnDisable()
    {
        PlayerOverdrive.on -= PlayerOverdriveOn;
        PlayerOverdrive.off -= PlayerOverdriveOff;
    }

    private void Start()
    {
        energyBar.Initialized(energy, MAX);
        Obtain(MAX);
    }

    public void Obtain(int value)
    {
        if (energy == MAX ||!available||!gameObject.activeSelf) return;
        energy = Mathf.Clamp(energy + value, 0, MAX);
        energyBar.UpdateStats(energy, MAX);
    }

    public void Use(int value)
    {
        energy -= value;
        energyBar.UpdateStats(energy, MAX);

        if (energy == 0 && !available)
        {
            PlayerOverdrive.off.Invoke();
        }
    }

    public bool IsEnough(int value) => energy >= value;

    void PlayerOverdriveOn()
    {
        available = false;
        StartCoroutine(nameof(KeepusingCoroutine));
    }

    void PlayerOverdriveOff()
    {
        available = true;
        StopCoroutine(nameof(KeepusingCoroutine));
    }

    IEnumerator KeepusingCoroutine()
    {
        while (gameObject.activeSelf && energy > 0)
        {
            yield return waitForOverdriveInteval;


            Use(PERCENT);
        }
    }

}
