using System;
using UnityEngine;

[RequireComponent(typeof(GhostMove))]
public class GhostAI : MonoBehaviour
{
    public float VulnerabilityEndingTime;
    //evento para atualizar na view (sprite)
    public event Action<GhostState> OnGhostStateChanged;
    public event Action<GhostState> OnResetCount;

    public GhostState _ghostState;
    private GhostMove _ghostMove;
    private CharacterMotor _ghostMotor;
    private Transform _pacman;
    public float _vulnerabilityTimer;
    private bool _leaveHouse;

    private void Start()
    {
        _ghostMove = GetComponent<GhostMove>();
        _ghostMotor = GetComponent<CharacterMotor>();
        _ghostMove.OnUpdateMoveTarget += _ghostMove_OnUpdateMoveTarget;
        _pacman = GameObject.FindWithTag("Player").transform;
        _ghostState = GhostState.Active;
        _leaveHouse = false;
    }
    private void Update()
    {
        switch (_ghostState)
        {
            case GhostState.Vulnerable:
                _vulnerabilityTimer -= Time.deltaTime;
                //ultimos 3 seg o ghost começa a piscar
                if (_vulnerabilityTimer <= VulnerabilityEndingTime)
                {
                    _ghostState = GhostState.VulnerabilityEnding;
                    OnGhostStateChanged?.Invoke(_ghostState);
                }
                break;
            case GhostState.VulnerabilityEnding:
                _vulnerabilityTimer -= Time.deltaTime;

                if (_vulnerabilityTimer <= 0)
                {
                    _ghostState = GhostState.Active;
                    //AKIIII
                    OnResetCount?.Invoke(_ghostState);
                    OnGhostStateChanged?.Invoke(_ghostState);
                }
                break;
            case GhostState.Defeated:
                _ghostMotor.MoveSpeed = 14;
                break;
        }
    }

    public void LeaveHouse()
    {
        _ghostMove.CharacterMotor.CollideWithGates(false);
        _leaveHouse = true;
    }

    public void SetVulnerable(float duration)
    {
        _vulnerabilityTimer = duration;
        _ghostState = GhostState.Vulnerable;
        OnGhostStateChanged?.Invoke(_ghostState);
        _ghostMove.AllowReverseDirection();
        _ghostMotor.MoveSpeed = 6;
    }

    public void Recover()
    {
        _ghostMove.CharacterMotor.CollideWithGates(true);
        _ghostState = GhostState.Active;
        OnGhostStateChanged?.Invoke(_ghostState);
        _ghostMotor.MoveSpeed = 8;
        _leaveHouse = false;
    }

    public void StartMoving()
    {
        _ghostMove.CharacterMotor.enabled = true;
    }

    public void StopMoving()
    {
        _ghostMove.CharacterMotor.enabled = false;
    }

    public void Reset()
    {
        _ghostMove.CharacterMotor.ResetPosition();
        _ghostState = GhostState.Active;
        OnGhostStateChanged?.Invoke(_ghostState);
        _leaveHouse = false;
    }

    private void _ghostMove_OnUpdateMoveTarget()
    {
        switch (_ghostState)
        {
            case GhostState.Active:
                if (_leaveHouse)
                {
                    if (transform.position == new Vector3(0, 3, 0))
                    {
                        _leaveHouse = false;
                        _ghostMove.CharacterMotor.CollideWithGates(true);
                        _ghostMove.SetTargetMoveLocation(_pacman.position);
                    } else
                    {
                        _ghostMove.SetTargetMoveLocation(new Vector3(0, 3, 0));
                    }
                } else
                {
                    _ghostMove.SetTargetMoveLocation(_pacman.position);
                }
                break;
            case GhostState.Vulnerable:
            case GhostState.VulnerabilityEnding:
                _ghostMove.SetTargetMoveLocation((transform.position - _pacman.position) * 2);
                break;
            case GhostState.Defeated:
                _ghostMove.SetTargetMoveLocation(Vector3.zero);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (_ghostState)
        {
            case GhostState.Active:
                if (collision.CompareTag("Player"))
                {
                    collision.GetComponent<Life>().RemoveLife();
                }
                break;
            case GhostState.Vulnerable:
            case GhostState.VulnerabilityEnding:
                if (collision.CompareTag("Player"))
                {
                    _ghostMove.CharacterMotor.CollideWithGates(false);
                    _ghostState = GhostState.Defeated;
                    OnGhostStateChanged?.Invoke(_ghostState);

                }
                break;
            case GhostState.Defeated:

                break;
        }
    }
}

public enum GhostState
{
    Active,
    Vulnerable,
    VulnerabilityEnding,
    Defeated
}
