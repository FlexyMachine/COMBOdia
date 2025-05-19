using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This class is created for the example scene. There is no support for this script.
public class SimplePlayerHealth : HealthManager
{
	public bool godMode;
	public float health = 100f;
	public float maxHealth = 100f;

	public Transform canvas;
	public GameObject hurtPrefab;
	public Slider healthBar;
	public float decayFactor = 0.8f;
	private bool dead = false;

	private HurtHUD hurtUI;

	private void Awake()
	{
		AudioListener.pause = false;
		hurtUI = this.gameObject.AddComponent<HurtHUD>();
		hurtUI.Setup(canvas, hurtPrefab, decayFactor, this.transform);
	}

	public override void TakeDamage(Vector3 location, Vector3 direction, float damage, Collider bodyPart, GameObject origin)
	{
		if (!godMode)
		{
			health -= damage;
				healthBar.value = health / maxHealth;
		}

		if (hurtPrefab && canvas)
			hurtUI.DrawHurtUI(origin.transform, origin.GetHashCode());
	}

	public void OnGUI()
	{
		if (health <= 0 && !dead)
		{
			dead = true;
			StartCoroutine(nameof(ReloadScene));
		}
	}

	private IEnumerator ReloadScene()
	{
		SendMessage("PlayerDead", SendMessageOptions.DontRequireReceiver);
		yield return new WaitForSeconds(0.5f);
		canvas.gameObject.SetActive(false);
		AudioListener.pause = true;
		Camera.main.clearFlags = CameraClearFlags.SolidColor;
		Camera.main.backgroundColor = Color.black;
		Camera.main.cullingMask = LayerMask.GetMask();

		yield return new WaitForSeconds(1);

		SceneManager.LoadScene(0);
	}
}
