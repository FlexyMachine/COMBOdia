using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This class is created for the example scene. There is no support for this script.
public class PlayerShooting : MonoBehaviour
{
	public Transform drawShotOrigin;
	public LayerMask shotMask;
	public WeaponMode weaponMode;
	public ComboMode comboMode;
	public float comboMultiplier = 0.1f;
	public float baseRange = 100f;
	public float baseDamage = 10f;
	public int baseFireRate = 100;
	public TMP_Text comboText;
	public TMP_Text fireRateText;
	public TMP_Text damageText;
	public TMP_Text rangeText;
	public enum ComboMode
	{
		FIRE_RATE,
		DAMAGE,
		RANGE
	}
	public enum WeaponMode
	{
		SEMI,
		AUTO,
		BURST
	}

	private int comboCount = 0;
	private Transform shotOrigin;
	private LineRenderer laserLine;
	private bool canShot;
	private int realFireRate;
	private float realDamage;
	private float realRange;

	private AudioSource gunAudio;

	private WaitForSeconds halfShotDuration;// = new WaitForSeconds(0.06f);

	void CalculateHalfShotDuration()
	{
		float waitTime = 60f / realFireRate;
		halfShotDuration = new WaitForSeconds(waitTime / 2);
	}

	void UpdateUI()
	{
		if (comboText != null)
			comboText.text = "Combo:	" + comboCount;

		if (fireRateText != null)
			fireRateText.text = "Fire Rate:	" + realFireRate;

		if (damageText != null)
			damageText.text = "Damage:	" + realDamage;

		if (rangeText != null)
			rangeText.text = "Range:	" + realRange;
	}

	void CalculateRealValues()
	{

		float multiplier = 1 + (comboMultiplier * comboCount);

		switch (comboMode)
		{
			case ComboMode.FIRE_RATE:
				realFireRate = (int)(multiplier * baseFireRate);
				CalculateHalfShotDuration();
				break;

			case ComboMode.DAMAGE:
				realDamage = multiplier * baseDamage;
				break;

			case ComboMode.RANGE:
				realRange = multiplier * baseRange;
				break;
		}
		UpdateUI();
	}

	// Start is called before the first frame update
	void Start()
	{
		realFireRate = baseFireRate;
		realDamage = baseDamage;
		realRange = baseRange;
		laserLine = GetComponent<LineRenderer>();
		gunAudio = GetComponent<AudioSource>();
		canShot = true;
		CalculateHalfShotDuration();


		// Fetch TMP_Text fields from the grandparent (parent of parent)
		Transform grandParent = transform.parent != null ? transform.parent.parent : null;
		if (grandParent != null)
		{
			TMP_Text[] texts = grandParent.GetComponentsInChildren<TMP_Text>(true);
			foreach (var text in texts)
			{
				switch (text.name)
				{
					case "Combo":
						comboText = text;
						break;
					case "FireRate":
						fireRateText = text;
						break;
					case "Damage":
						damageText = text;
						break;
					case "Range":
						rangeText = text;
						break;
				}
			}
		}

		UpdateUI();

		if (transform.parent != null)
			shotOrigin = transform.parent;
	}

	// Update is called once per frame
	void Update()
	{
		if (weaponMode == WeaponMode.SEMI && Input.GetButtonDown("Fire1") && canShot)
		{
			Shoot();
		}
		else if (weaponMode == WeaponMode.BURST && Input.GetButtonDown("Fire1") && canShot)
		{
			for (int i = 0; i < 3; i++)
				Shoot();

		}
		else if (weaponMode == WeaponMode.AUTO && Input.GetButton("Fire1") && canShot)
		{
			Shoot();
		}
	}

	void Shoot()
	{
		StartCoroutine(ShotEffect());
		laserLine.SetPosition(0, drawShotOrigin.position);
		Physics.SyncTransforms();

		if (Physics.Raycast(shotOrigin.position, shotOrigin.forward, out RaycastHit hit, realRange, shotMask))
		{
			laserLine.SetPosition(1, hit.point);

			// Call the damage behaviour of target if exists.
			if (hit.collider)
			{
				hit.collider.SendMessageUpwards(
					"HitCallback",
					new HealthManager.DamageInfo(
						hit.point,
						shotOrigin.forward,
						realDamage,
						hit.collider
						),
					SendMessageOptions.DontRequireReceiver
				);

				if (hit.collider.CompareTag("Enemy"))
					comboCount++;
				else
					comboCount = 0;
			}
			else
				comboCount = 0;
		}
		else
		{
			laserLine.SetPosition(1, drawShotOrigin.position + (shotOrigin.forward * realRange));
			comboCount = 0;
		}

		CalculateRealValues();
		// Call the alert manager to notify the shot noise.
		GameObject.FindGameObjectWithTag("GameController").SendMessage("RootAlertNearby", shotOrigin.position, SendMessageOptions.DontRequireReceiver);
	}

	private IEnumerator ShotEffect()
	{
		gunAudio.Play();
		// Turn on our line renderer
		laserLine.enabled = true;
		canShot = false;

		yield return halfShotDuration;

		// Deactivate our line renderer after waiting
		laserLine.enabled = false;

		yield return halfShotDuration;

		if (weaponMode == WeaponMode.SEMI)
		{
			yield return halfShotDuration;
			yield return halfShotDuration;
		}

		canShot = true;
	}

	// Player dead callback.
	public void PlayerDead()
	{
		canShot = false;
	}
}
