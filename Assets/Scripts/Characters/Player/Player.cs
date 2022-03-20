using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    public bool isMaxHealth => health == maxHealth;
    public bool isMaxweapon => weaponPower == 2;


    [SerializeField] bool regenerateHealth = true;
    [SerializeField] float healthRegenerateTime;
    [SerializeField, Range(0f, 1f)] float healthRegeneratePercent;
    [SerializeField] StatsBar_HUD statsBar_HUD;


    [Header("----INPUT----")]
    [SerializeField] PlayerInput input;
    [Header("----MOVE----")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float accelerationTime = 1.0f;
    [SerializeField] float decelerationTime = 1.0f;
    [SerializeField] float angle = 50;

    [Header("子弹")]
    [SerializeField] GameObject projectile1;
    [SerializeField] GameObject projectile2;
    [SerializeField] GameObject projectile3;
    [SerializeField] GameObject projectileOverdirve;

    [SerializeField] Transform muzzleTop;
    [SerializeField] Transform muzzleMiddle;
    [SerializeField] Transform muzzleBottom;
    [SerializeField] ParticleSystem muzzleVFX;

    [SerializeField, Range(0, 2)] int weaponPower = 0;
    [SerializeField] float Interval = 2f;

    [SerializeField] AudioData projectileLaunchSFX;

    [Header("----DODGE----")]
    [SerializeField, Range(0, 100)] int dodgeEnergyCost = 25;
    [SerializeField] float maxRoll = 720f;
    [SerializeField] float rollSpeed = 360f;
    [SerializeField] Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] AudioData dodgeSFX;
    float currentRoll;
    bool isDodging = false;
    bool isOverdriving = false;
    float dodgeDuration;

    [Header("----OVERDRIVE----")]
    [SerializeField] int overdriveDodgeFactor = 2;
    [SerializeField] float overdriveSpeedFactor = 1.2f;
    [SerializeField] float overdriveFireFactor = 1.2f;
    [SerializeField] float slowMotionDuration = 1f;

    [Header("----MISSILE----")]
    MissileSystem missileSystem;

    float paddingX = 0.2f;
    float paddingY = 0.2f;

    new Rigidbody2D rigidbody;
    new Collider2D collider;
    Coroutine moveCoroutine;
    Coroutine healthRegenerateCoroutine;
    WaitForSeconds waitForFireInterval;
    WaitForSeconds waitForOverdriveFireIntervel;
    WaitForSeconds waitHealthRegenerateTime;
    WaitForSeconds waitForDecelerationTime;
    Vector2 previousVelocity;
    Vector2 moveDirection;
    Quaternion previousRotation;
    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    readonly float InvincibleTime = 1f;

    WaitForSeconds waitForInvincibleTime;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        missileSystem = GetComponent<MissileSystem>();
        rigidbody.gravityScale = 0f;

        waitForFireInterval = new WaitForSeconds(Interval);
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);
        waitForOverdriveFireIntervel = new WaitForSeconds(Interval / overdriveFireFactor);
        waitForDecelerationTime = new WaitForSeconds(decelerationTime);
        waitForInvincibleTime = new WaitForSeconds(InvincibleTime);

        dodgeDuration = maxRoll / rollSpeed;
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2.0f;
        paddingY = size.y / 2.0f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire += Fire;
        input.onStopFire += StopFire;
        input.onDodge += Dodge;
        input.onOverdrive += Overdrive;
        input.onLaunchMissile += LaunchMissile;

        PlayerOverdrive.on += OverdriveOn;
        PlayerOverdrive.off += OverdriveOff;
    }

    private void OnDisable()
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.onDodge -= Dodge;
        input.onOverdrive -= Overdrive;
        input.onLaunchMissile -= LaunchMissile;

        PlayerOverdrive.on -= OverdriveOn;
        PlayerOverdrive.off -= OverdriveOff;
    }


    private void Start()
    {

        input.EnableGameplayerInput();

        statsBar_HUD.Initialized(health, maxHealth);

    }


    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        PowerDown();
        statsBar_HUD.UpdateStats(health, maxHealth);
        TimeController.Instance.BulletTime(slowMotionDuration);

        if (gameObject.activeSelf)
        {
            Move(moveDirection);
            StartCoroutine(nameof(InvincibleCoroutine));
            if (regenerateHealth)
            {
                if (healthRegenerateCoroutine != null)
                    StopCoroutine(healthRegenerateCoroutine);
                healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
            }
        }
    }

    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        statsBar_HUD.UpdateStats(health, maxHealth);
    }

    public override void Die()
    {
        GameManager.onGameOver?.Invoke();
        GameManager.GameState = GameState.GameOver;
        statsBar_HUD.UpdateStats(0f, maxHealth);
        base.Die();
    }

    #region MOVE
    void Move(Vector2 moveinput)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        Quaternion moveRotation = Quaternion.AngleAxis(angle * moveinput.normalized.y, Vector3.right);
        moveDirection = moveinput.normalized;
        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveDirection * moveSpeed, moveRotation));
        StopCoroutine(nameof(DecelerationCoroutine));
        StartCoroutine(nameof(MoveRangeLimatationCoroutine));
    }

    void StopMove()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveDirection = Vector2.zero;
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, moveDirection, Quaternion.identity));
        StartCoroutine(nameof(DecelerationCoroutine));
    }


    IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRotation)
    {
        float t = 0f;
        //循环增加直到百分百

        previousVelocity = rigidbody.velocity;
        previousRotation = transform.rotation;
        while (t < 1f)
        {
            t += Time.fixedDeltaTime / time;
            rigidbody.velocity = Vector2.Lerp(previousVelocity, moveVelocity, t);
            transform.rotation = Quaternion.Lerp(previousRotation, moveRotation, t);
            yield return waitForFixedUpdate;
        }

        //while (t < time)
        //{
        //    t += Time.fixedDeltaTime;
        //    rigidbody.velocity = Vector2.Lerp(rigidbody.velocity, moveVelocity, t/time);
        //    transform.rotation = Quaternion.Lerp(transform.rotation, moveRotation, t/time);
        //    yield return null;
        //}
    }


    IEnumerator MoveRangeLimatationCoroutine()
    {
        while (true)
        {
            transform.position = ViewPort.Instance.PlayerMoveablePositon(transform.position, paddingX, paddingY);
            yield return null;
        }
    }

    IEnumerator DecelerationCoroutine()
    {
        yield return waitForDecelerationTime;

        StopCoroutine(nameof(MoveRangeLimatationCoroutine));
    }
    #endregion


    #region FIRE
    void Fire()
    {
        muzzleVFX.Play();
        StartCoroutine(nameof(FireCoroutine));
    }

    void StopFire()
    {
        muzzleVFX.Stop();
        StopCoroutine(nameof(FireCoroutine));
    }

    IEnumerator FireCoroutine()
    {
        while (true)
        {
            //#region Old
            //switch (weaponPower)
            //{
            //    case 0: 
            //        Instantiate(projectile1, muzzleMiddle.position, Quaternion.identity);
            //        break;
            //    case 1:
            //        Instantiate(projectile1, muzzleTop.position, Quaternion.identity);
            //        Instantiate(projectile1, muzzleBottom.position, Quaternion.identity);
            //        break;
            //    case 2:
            //        Instantiate(projectile1, muzzleMiddle.position, Quaternion.identity);
            //        Instantiate(projectile2, muzzleTop.position, Quaternion.identity);
            //        Instantiate(projectile3, muzzleBottom.position, Quaternion.identity);
            //        break;
            //    default:
            //        break;
            //}
            //#endregion

            switch (weaponPower)
            {
                case 0:
                    PoolManager.Release(isOverdriving?projectileOverdirve: projectile1, muzzleMiddle.position);
                    break;
                case 1:
                    PoolManager.Release(isOverdriving ? projectileOverdirve : projectile1, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverdirve : projectile1, muzzleBottom.position);
                    break;
                case 2:
                    PoolManager.Release(isOverdriving ? projectileOverdirve : projectile1, muzzleMiddle.position);
                    PoolManager.Release(isOverdriving ? projectileOverdirve : projectile2, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverdirve : projectile3, muzzleBottom.position);
                    break;
                default:
                    break;
            }

            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);
            yield return isOverdriving ? waitForOverdriveFireIntervel : waitForFireInterval;
        }
    }
    #endregion

    #region DODGE
    void Dodge()
    {
        if (isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return;

        StartCoroutine(nameof(DodgeCoroutine));
    }

    IEnumerator DodgeCoroutine()
    {
        isDodging = true;
        AudioManager.Instance.PlayRandomSFX(dodgeSFX);
        PlayerEnergy.Instance.Use(dodgeEnergyCost);
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);
        collider.isTrigger = true;    //设为触发器表示玩家无敌


        //var scale = transform.localScale; //方法一
        //var t1 = 0f;
        //var t2 = 0f;   //方法二
        currentRoll = 0;
        while (currentRoll < maxRoll)
        {
            currentRoll += Time.deltaTime * rollSpeed;
            transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);

            //人物缩放
            #region method one
            //if (currentRoll < maxRoll / 2f)
            //{
            //    scale -= (Time.deltaTime / dodgeDuration) * Vector3.one;
            //}
            //else
            //{
            //    scale += (Time.deltaTime / dodgeDuration) * Vector3.one;
            //}
            //if (currentRoll < maxRoll / 2f)
            //{
            //    scale.x = Mathf.Clamp(scale.x - Time.deltaTime / dodgeDuration, dodgeScale.x, 1f);
            //    scale.y = Mathf.Clamp(scale.y - Time.deltaTime / dodgeDuration, dodgeScale.y, 1f);
            //    scale.z = Mathf.Clamp(scale.z - Time.deltaTime / dodgeDuration, dodgeScale.z, 1f);
            //}
            //else
            //{
            //    scale.x = Mathf.Clamp(scale.x + Time.deltaTime / dodgeDuration, dodgeScale.x, 1f);
            //    scale.y = Mathf.Clamp(scale.y + Time.deltaTime / dodgeDuration, dodgeScale.y, 1f);
            //    scale.z = Mathf.Clamp(scale.z + Time.deltaTime / dodgeDuration, dodgeScale.z, 1f);
            //}
            //transform.localScale = scale;
            #endregion


            #region method two
            //线性插值
            //if (currentRoll < maxRoll / 2f)
            //{
            //    t1 += Time.deltaTime / dodgeDuration;
            //    transform.localScale = Vector3.Lerp(transform.localScale, dodgeScale, t1);
            //}
            //else
            //{
            //    t2 += Time.deltaTime / dodgeDuration;
            //    transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, t2);
            //}
            #endregion

            #region method three
            //贝塞尔曲线
            transform.localScale = BezierCurve.QuadraticPoint(Vector3.one, Vector3.one, dodgeScale, currentRoll / maxRoll);
            #endregion

            yield return null;
        }

        collider.isTrigger = false;
        isDodging = false;
    }
    #endregion

    #region OVERDRIVE
    void Overdrive()
    {
        if (!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX)) return;

        PlayerOverdrive.on.Invoke();
    }

    void OverdriveOn()
    {
        isOverdriving = true;
        dodgeEnergyCost *= overdriveDodgeFactor;
        moveSpeed *= overdriveSpeedFactor;
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);
    }

    void OverdriveOff()
    {
        isOverdriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor;
    }

    #endregion

    #region LAUNCH MISSILE
    void LaunchMissile()
    {
        missileSystem.Launch(muzzleMiddle);
    }

    public void PickUpMissle()
    { 
        missileSystem.PickUp();
    }
    #endregion

    IEnumerator InvincibleCoroutine()
    {
        collider.isTrigger = true;

        yield return waitForInvincibleTime;

        collider.isTrigger = false;
    }

    public void WeaponPowerUp()
    {
        weaponPower = Mathf.Clamp(++weaponPower, 0, 2);
    }

    void PowerDown()
    {
        weaponPower = Mathf.Max(--weaponPower, 0);
    }
}
