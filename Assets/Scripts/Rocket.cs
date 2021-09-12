using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 0f;

    [SerializeField]
    private float _maxMoveSpeed = 3f;

    [SerializeField]
    private Rigidbody2D _rb;

    [SerializeField]
    private Animator _animator;



    public event Action OnGameOver;

    private bool _gameOver = false;

    private bool _fly = false;
    private bool _left = false;
    private bool _right = false;

    public event Action OnCoinCollect;
    public event Action OnExtraCoinCollect;
    public event Func<Vector2> OnCalculateForce;



    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _moveSpeed = 0f;
        _gameOver = false;
    }



    private void Update()
    {
        if (_gameOver)
            return;


        if (_fly)
        {
            _moveSpeed = Mathf.Clamp(_moveSpeed + _maxMoveSpeed * Time.deltaTime, 0f, _maxMoveSpeed);
            _animator.SetBool("Fly", true);
        }
        else
        {
            _animator.SetBool("Fly", false);
            _moveSpeed = Mathf.Clamp(_moveSpeed - _maxMoveSpeed * Time.deltaTime, 0f, _maxMoveSpeed);
        }

        if (_left)
        {
            transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles,
                new Vector3(0f, 0f, transform.eulerAngles.z + 90f), _maxMoveSpeed * Time.deltaTime * 30f);
        }
        else if (_right)
        {
            transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles,
                new Vector3(0f, 0f, transform.eulerAngles.z - 90f), _maxMoveSpeed * Time.deltaTime * 30f);
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.W))
        {
            _fly = true;
        }
        else if(Input.GetKeyUp(KeyCode.W))
        {
            _fly = false;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _left = true;
            _right = false;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _right = true;
            _left = false;
        }
        else if(Input.GetKeyUp(KeyCode.A))
        {
            _left = false;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            _right = false;
        }
#endif


        if (transform.position.x < -3.9f)
        {
            transform.position = new Vector3(3.9f, transform.position.y, 0f);
        }
        else if (transform.position.x > 3.9f)
        {
            transform.position = new Vector3(-3.9f, transform.position.y, 0f);
        }
    }

    private void FixedUpdate()
    {
        if (_gameOver)
        {
            _rb.velocity = Vector2.zero;
        }
        else
        {
            _rb.velocity = (Vector2)transform.up * _moveSpeed + OnCalculateForce();
        }
    }



    public void Drive(EDriveParameter parameter, bool state)
    {
        if (parameter == EDriveParameter.fly)
        {
            _fly = state;
        }
        else if (parameter == EDriveParameter.left)
        {
            _right = false;
            _left = state;
        }
        else if (parameter == EDriveParameter.right)
        {
            _left = false;
            _right = state;
        }
    }

    public enum EDriveParameter
    {
        fly, right, left
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_gameOver)
        {
            if (collision.gameObject.tag == "Planet" || collision.gameObject.tag == "Meteor")
            {
                    _gameOver = true;
                    _animator.SetBool("Lose", true);
                    Destroy(this.gameObject, 1f);
                    OnGameOver.Invoke();
            }

        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_gameOver)
            return;

        GameObject obj = collision.gameObject;

        if(obj.tag == "Coin")
        {
            Destroy(obj);
            OnCoinCollect.Invoke();
        }
        else if(obj.tag == "ExtraCoin")
        {
            Destroy(obj);
            OnExtraCoinCollect.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
    }
}