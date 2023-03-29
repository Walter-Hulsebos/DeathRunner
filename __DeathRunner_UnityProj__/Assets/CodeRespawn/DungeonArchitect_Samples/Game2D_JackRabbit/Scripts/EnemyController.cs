//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;
using DungeonArchitect.Navigation;

namespace JackRabbit {
	public class EnemyController : MonoBehaviour {

		public Animator animator;
		public float maxHealth;

		private bool facingRight = true;
		private float currentHealth;


		private Rigidbody2D rigidBody2D;

		private void Awake() {
			rigidBody2D = GetComponent<Rigidbody2D>();
			currentHealth = maxHealth;
		}

		// Update is called once per frame
		private void FixedUpdate () {
			animator.SetFloat("Speed", rigidBody2D.velocity.magnitude);

			var moveX = rigidBody2D.velocity.x;
			if (moveX > 0 && facingRight) {
				Flip();
			} else if (moveX < 0 && !facingRight) {
				Flip ();
			}

		}

		private void Flip() {
			facingRight = !facingRight;
			var scale = transform.localScale;
			scale.x *= -1;
			transform.localScale = scale;
		}

		public bool Alive {
			get { return currentHealth > 0; }
		}

		public void ApplyDamage(float amount) {
			if (Alive) {
				currentHealth -= amount;
				if (!Alive) {
					OnDead();
				}
			}
		}

		private void OnDead() {
			animator.SetTrigger("Dead");
			rigidBody2D.velocity = Vector2.zero;
			rigidBody2D.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
			var colliders = GetComponents<Collider2D>();
			foreach (var collider in colliders) {
				collider.enabled = false;
			}

			
			GetComponent<DungeonNavAgent>().enabled = false;
			GetComponent<DungeonArchitect.Samples.ShooterGame.TwoD.AIController2D>().enabled = false;
		}

	}
}
