using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    [SerializeField]
    private EDo _state = EDo.stay;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _moveSpeed = 9f;
    [SerializeField]
    private bool _isPlayerFound = false;

    public const float PLANET_DISTANCE = 2.2f;
    public const float METEOR_DISTANCE = 1.6f;
    public const float PLAYER_DISTANCE = 1.5f;
    public const float MAX_PLAYER_DISTANCE = 5f;
    public const float DIFERENCE_DISTANCE = 0.333f;

    private float _nextChangeTime = -999f;
    [SerializeField]
    private EDo _priority = EDo.follow_player;
    private float _dontWorryTime = 1f;


    public event Action OnCoinCollect;
    public event Action OnExtraCoinCollect;
    public event Action OnKill;



    public bool IsPlayerFound => _isPlayerFound;



    private void Update()
    {
        _dontWorryTime -= Time.deltaTime;

        if (_dontWorryTime <= 0f)
        {
            FindTarget();
            _dontWorryTime = UnityEngine.Random.Range(0.01f, 0.03f);
        }

        if (_target == null)
            _state = EDo.stay;

        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.tag == "Coin")
        {
            Destroy(obj);
            OnCoinCollect.Invoke();
        }
        else if (obj.tag == "ExtraCoin")
        {
            Destroy(obj);
            OnExtraCoinCollect.Invoke();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.tag == "Meteor")
        {
            Destroy(obj);
            OnKill.Invoke();
            Destroy(this.gameObject);
        }
        else if (obj.tag == "Planet" || obj.tag == "Player")
        {
            OnKill.Invoke();
            Destroy(this.gameObject);
        }
    }

    private void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        GameObject[] extraCoins = GameObject.FindGameObjectsWithTag("ExtraCoin");
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        GameObject[] meterorits = GameObject.FindGameObjectsWithTag("Meteor");

        GameObject dangerousMeteor = null;
        GameObject dangerousPlanet = null;

        float meteorDist = Mathf.Infinity;
        for (int i = 0; i < meterorits.Length; i++)
        {
            float d = Vector3.Distance(meterorits[i].transform.position, transform.position);

            if (d < meteorDist)
            {
                dangerousMeteor = meterorits[i];
                meteorDist = d;
            }
        }

        float planetDist = Mathf.Infinity;
        for (int i = 0; i < planets.Length; i++)
        {
            float d = Vector3.Distance(planets[i].transform.position, transform.position);

            if (d < planetDist)
            {
                dangerousPlanet = planets[i];
                planetDist = d;
            }
        }



        if(dangerousMeteor != null && meteorDist < METEOR_DISTANCE - DIFERENCE_DISTANCE)
        {
            _target = dangerousMeteor.transform;
            _state = EDo.save_from_meteor;

            return;
        }

        if (dangerousPlanet != null && planetDist < PLANET_DISTANCE - DIFERENCE_DISTANCE)
        {
            _target = dangerousPlanet.transform;
            _state = EDo.save_form_planet;

            return;
        }



        if(_nextChangeTime <= Time.time)
        {
            _nextChangeTime = Time.time + UnityEngine.Random.Range(1f, 3f);

            if (_isPlayerFound)
            {
                int r = UnityEngine.Random.Range(0, 2);

                switch (r)
                {
                    case 1:
                        _priority = EDo.collect_coins;
                        break;

                    default:
                        _priority = EDo.follow_player;
                        break;
                }
            }
            else
            {
                _priority = EDo.follow_player;
            }
        }


        if(player != null)
        {
            float dist = Vector3.Distance(player.transform.position, transform.position);

            if((_priority == EDo.follow_player && (dist < MAX_PLAYER_DISTANCE || _isPlayerFound)) || (_isPlayerFound && dist > MAX_PLAYER_DISTANCE))
            {
                _isPlayerFound = true;

                _target = player.transform;
                _state = EDo.follow_player;

                return;
            }
        }


        // this began after player because if player is too far, i use it by priority!!!
        //start
        if (_priority == EDo.collect_coins)
        {
            Transform coin = null;

            float dist = Mathf.Infinity;

            for (int i = 0; i < extraCoins.Length; i++)
            {
                float d = Vector3.Distance(transform.position, extraCoins[i].transform.position);
                if (d < dist)
                {
                    dist = d;
                    coin = extraCoins[i].transform;
                }
            }

            for (int i = 0; i < coins.Length; i++)
            {
                float d = Vector3.Distance(transform.position, coins[i].transform.position);
                if (d < dist)
                {
                    dist = d;
                    coin = coins[i].transform;
                }
            }

            if (coin != null)
            {
                _target = coin;
                _state = EDo.collect_coins;
                return;
            }
        }
        //end

        _target = null;
        _state = EDo.stay;
    }

    private void Move()
    {
        if (_state == EDo.follow_player)
        {
            Vector3 dir = _target.position - transform.position;
            transform.position = Vector3.MoveTowards(transform.position,
                _target.position - dir.normalized * PLAYER_DISTANCE, _moveSpeed * Time.deltaTime);
        }
        else if (_state == EDo.save_from_meteor)
        {
            Vector3 dir = _target.position - transform.position;
            transform.position = Vector3.MoveTowards(transform.position,
                _target.position - dir.normalized * METEOR_DISTANCE, _moveSpeed * Time.deltaTime);
        }
        else if (_state == EDo.save_form_planet)
        {
            Vector3 dir = _target.position - transform.position;
            transform.position = Vector3.MoveTowards(transform.position,
                _target.position - dir.normalized * PLANET_DISTANCE, _moveSpeed * Time.deltaTime);
        }
        else if (_state == EDo.collect_coins)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                _target.position, _moveSpeed * Time.deltaTime);
        }
        else if(_state == EDo.stay)
        {
            Vector3 pos = new Vector3(Mathf.Sin(Time.time) * 2.5f, transform.position.y, 0f);

            transform.position = Vector3.MoveTowards(transform.position, pos, _moveSpeed * Time.deltaTime);
        }
    }
}


public enum EDo
{
    follow_player, collect_coins, save_from_meteor, stay, save_form_planet, 
}

