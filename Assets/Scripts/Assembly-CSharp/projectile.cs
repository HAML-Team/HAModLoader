using System.Collections;
using UnityEngine;

public class projectile : MonoBehaviour
{
	public GameObject explodeParticle;

	public GameObject hide_on_collide;

	public ParticleSystem stop_on_collide;

	private string PERK_KEY;

	private int PERK_LEVEL;

	private float speed;

	private GameObject target;

	private Vector3 target_pos = Vector3.zero;

	private creatureScript shooter;

	private bool dead;

	public void SHOOT_AT(string PERK_KEY, int PERK_LEVEL, GameObject target, float speed, creatureScript shooter)
	{
		this.speed = speed;
		this.target = target;
		this.shooter = shooter;
		this.PERK_KEY = PERK_KEY;
		this.PERK_LEVEL = PERK_LEVEL;
	}

	private void FixedUpdate()
	{
		if (!dead)
		{
			if (target != null)
			{
				target_pos = target.transform.position;
			}
			base.transform.LookAt(target_pos);
			base.transform.position += base.transform.forward * speed;
			if (Vector3.Distance(base.transform.position, target_pos) < 0.5f)
			{
				EXPLODE();
			}
		}
	}

	private IEnumerator delayed_destroy()
	{
		yield return new WaitForSeconds(1.5f);
		Object.Destroy(base.gameObject);
	}

	private void EXPLODE()
	{
		dead = true;
		if (hide_on_collide != null)
		{
			hide_on_collide.SetActive(false);
		}
		if (stop_on_collide != null)
		{
			stop_on_collide.Stop();
		}
		if (explodeParticle != null)
		{
			Object.Instantiate(explodeParticle).transform.position = new Vector3(target_pos.x, 0.6f, target_pos.z);
		}
		if (target != null)
		{
			perk_controller.Instance.NATURAL_PERK(target, shooter, PERK_KEY, PERK_LEVEL, base.gameObject);
			if (target.GetComponent<NewCombatant>().mob_type == NewCombatant.TYPE_T.creature && shooter != null)
			{
				target.GetComponent<creatureScript>().targetCombatant = shooter.gameObject;
			}
		}
		StartCoroutine(delayed_destroy());
	}
}
