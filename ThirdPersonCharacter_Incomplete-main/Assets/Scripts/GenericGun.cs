using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericGun : MonoBehaviour
{
    [Header("Weapon")]
    public int clipMax = 30;
    public int clipCurrent = 30;
    public bool automatic = true;
    [Min(1f/60f)]
    public float fireRate = 0.1f;
    public float reloadTime = 0.5f;
    [Header("Firing")]
    public UnityEvent onFire;
    public Transform firePoint;
    public GameObject bullet;
    [Header("Animation")]
    public float positionRecover;
    public float rotationRecover;
    public Vector3 knockbackPosition;
    public Vector3 knockbackRotation;
    Vector3 originalPosition;
    Quaternion originalRotation;

    public RectTransform realAim_rect;

    public float delta = 0;
    public bool shooting = false;
    private void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, positionRecover * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, rotationRecover * Time.deltaTime);

        if (automatic)
        {
            if (Input.GetKey(KeyCode.Mouse0) && delta == 0 && clipCurrent > 0)
            {
                shooting = true;
                Fire();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && delta == 0 && clipCurrent > 0)
            {
                shooting = true;
                Fire();
            }
        }
            
        if (delta >= fireRate && clipCurrent > 0)
        {
            shooting = false;
            delta = 0;
        }
        else if (delta >= reloadTime && clipCurrent == 0)
        {
            shooting = false;
            clipCurrent = clipMax;
            delta = 0;

        }

        if (shooting)
            delta += Time.deltaTime;
    }
    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.transform.tag != "Player")
            {
                realAim_rect.position = Camera.main.WorldToScreenPoint(hit.point);
            }
        }
    }
    public void Fire()
    {
        if (clipCurrent > 0)
            clipCurrent--;
        else
        {
            StartCoroutine(Reload());
            return;
        }

        shooting = true;

        Destroy(Instantiate(bullet, firePoint.position, firePoint.rotation), 10);
        onFire.Invoke();
        StartCoroutine(Knockback_Corutine());
    }
    IEnumerator Knockback_Corutine()
    {
        yield return null;
        transform.localPosition -= new Vector3(Random.Range(-knockbackPosition.x, knockbackPosition.x), Random.Range(0, knockbackPosition.y), Random.Range(-knockbackPosition.z, -knockbackPosition.z * .5f));
        transform.localEulerAngles -= new Vector3(Random.Range(knockbackRotation.x * 0.5f, knockbackRotation.x), Random.Range(-knockbackRotation.y, knockbackRotation.y), Random.Range(-knockbackRotation.z, knockbackRotation.z));
    }
    IEnumerator Reload()
    {
        shooting = true;
        yield return new WaitForSeconds(reloadTime);
        clipCurrent = clipMax;
    }
}
