using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAttack : MonoBehaviour
{
    #region Variables
    [Header("Snowball Settings")]
    public GameObject snowballPrefab;
    public Transform firePoint;
    public int maxSnowballs = 3;
    public float shootCooldown = 0.5f;
    public float snowballSpeed = 10f;

    [Header("Recharge Settings")]
    public float rechargeRate = 2f;        // Time to make 1 snowball
    private float rechargeTimer = 0f;
    public bool isNearSnowPile = false;

    [Header("UI Elements")]
    public GameObject chargeBarParent;
    public Image chargeBarFill;
    public TextMeshProUGUI ammoCounter;

    [Header("References")]
    private PlayerMovement playerMovement;

    private int currentSnowballs;
    private float lastShootTime = -10f;
    #endregion

    void Start()
    {
        currentSnowballs = maxSnowballs;
        if (chargeBarParent != null)
            chargeBarParent.SetActive(false);
        UpdateAmmoUI();

        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        MirrorFirePoint();
        HandleShooting();
        HandleRecharging();
    }

    #region Shooting
    void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time >= lastShootTime + shootCooldown)
        {
            if (currentSnowballs > 0)
            {
                ShootSnowball();
                currentSnowballs--;
                UpdateAmmoUI();
                lastShootTime = Time.time;
            }
        }
    }

    void ShootSnowball()
    {
        if (snowballPrefab == null || firePoint == null) return;

        GameObject snowball = Instantiate(snowballPrefab, firePoint.position, Quaternion.identity);
        Snowball sb = snowball.GetComponent<Snowball>();
        if (sb != null)
        {
            Vector2 direction = playerMovement.IsFacingRight() ? Vector2.right : Vector2.left;
            sb.Launch(direction, snowballSpeed);
        }
    }
    #endregion

    #region Recharging
    void HandleRecharging()
    {
        if (isNearSnowPile && currentSnowballs < maxSnowballs)
        {
            if (chargeBarParent != null && !chargeBarParent.activeSelf)
                chargeBarParent.SetActive(true);

            rechargeTimer += Time.deltaTime;

            // Calculate fill (fraction of current ammo + partial progress)
            float targetFill = ((float)currentSnowballs + rechargeTimer / rechargeRate) / maxSnowballs;
            targetFill = Mathf.Clamp01(targetFill);

            if (chargeBarFill != null)
                chargeBarFill.fillAmount = Mathf.MoveTowards(chargeBarFill.fillAmount, targetFill, Time.deltaTime / 0.5f);

            if (rechargeTimer >= rechargeRate)
            {
                rechargeTimer = 0f;
                currentSnowballs++;
                UpdateAmmoUI();
            }
        }
        else
        {
            if (chargeBarParent != null)
                chargeBarParent.SetActive(false);
            rechargeTimer = 0f;
            if (chargeBarFill != null)
                chargeBarFill.fillAmount = 0f;
        }
    }
    #endregion

    #region UI
    void UpdateAmmoUI()
    {
        if (ammoCounter != null)
            ammoCounter.text = currentSnowballs.ToString() + "/3";
    }

    void MirrorFirePoint()
    {
        if (firePoint == null || playerMovement == null) return;

        Vector3 localPos = firePoint.localPosition;

        if (playerMovement.IsFacingRight())
            localPos.x = Mathf.Abs(localPos.x);
        else
            localPos.x = -Mathf.Abs(localPos.x);

        firePoint.localPosition = localPos;
    }
    #endregion
}
