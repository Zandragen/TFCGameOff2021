using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
  [SerializeField] float speed;
  float direction;
  bool hit;
  float age;

  BoxCollider2D collider;
  Animator anim;

  private void Awake()
  {
    collider = GetComponent<BoxCollider2D>();
    anim = GetComponent<Animator>();
  }

  private void Update()
  {
    if(hit) return;
    float movementSpeed = speed*Time.deltaTime * direction;
    transform.Translate(movementSpeed, 0, 0);

    age += Time.deltaTime;
    if (age > 5) gameObject.SetActive(false);
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    hit = true;
    collider.enabled = false;
    anim.SetTrigger("explode");
  }

  public void SetDirection(float _direction)
  {
    age = 0.0f;
    direction = _direction;
    gameObject.SetActive(true);
    hit = false;
    collider.enabled = true;

    float localScaleX = transform.localScale.x;
    if(Mathf.Sign(localScaleX) != _direction)
      localScaleX = -localScaleX;

    transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
  }

  private void Deactivate()
  {
    gameObject.SetActive(false);
  }
}
