using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootItem : MonoBehaviour
{
    [SerializeField] float minSpeed = 5f;
    [SerializeField] float maxSpeed = 15f;
    [SerializeField] protected AudioData defaultPickUpSFX;
    

    protected AudioData pickUpSFX;

    protected Player player;

    Animator animator;

    int pickUpStateID = Animator.StringToHash("PickUp");

    protected Text lootMessage;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();

        lootMessage = GetComponentInChildren<Text>(true);
        pickUpSFX = defaultPickUpSFX;
    }

    private void OnEnable()
    {
        StartCoroutine(nameof(MoveCoroutine));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PickUp();
        animator.Play(pickUpStateID);
        AudioManager.Instance.PlayRandomSFX(pickUpSFX);
    }

    protected virtual void PickUp()
    {
        StopAllCoroutines();
    }

    IEnumerator MoveCoroutine()
    {
        float speed = Random.Range(minSpeed, maxSpeed);
        Vector3 direction = Vector3.left;

        while (true)
        {
            if (player.isActiveAndEnabled)
            {
                direction = (player.transform.position - transform.position).normalized;
            }
            transform.Translate(direction * speed * Time.deltaTime);
            yield return null;
        }
    
    }
}
