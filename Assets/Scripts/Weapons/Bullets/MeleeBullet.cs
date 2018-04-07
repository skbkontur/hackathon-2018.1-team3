﻿using UnityEngine;

public abstract class MeleeBullet : MovingObject
{
	private const int TtlLimit = 2;
	private int ttl;

	private float _objectSpeed = 0.01f;

	protected abstract Rigidbody2D rb2d { get; }

	public abstract int Damage { get; }

	// Update is called once per frame
	void Update()
	{
		ttl++;
		var movementMultiplier = GetMovementMultiplier();
		MoveObject(rb2d, new Vector2(movementMultiplier, 0)); // TODO: сделать направление
		Debug.Log(ttl);
		if (ttl >= TtlLimit)
		{
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("Hey");
		if (!other.gameObject.CompareTag("Player"))
			Destroy(gameObject);
	}

	protected override float ObjectSpeed
	{
		get { return _objectSpeed; }
	}
}
