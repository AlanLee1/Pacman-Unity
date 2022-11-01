using System;
using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    public float MoveSpeed;

    public event Action<Direction> OnDirectionChanged;
    public event Action OnAlignedWithGrid;
    public event Action OnResetPosition;
    public event Action OnDisabled;

    private Vector2 _currentMovementDirection;
    private Vector2 _desiredMovementDirection;
    private Vector3 _initialPosition;

    //Rigidbody do object
    private Rigidbody2D _rigidbody;
    //box do object
    private Vector2 _boxSize;
    //layer
    private LayerMask _collisionLayerMask;

    public LayerMask CollisionLayerMask
    {
        get => _collisionLayerMask;
    }

    private void Start()
    {
        _desiredMovementDirection = Vector2.zero;
        _currentMovementDirection = Vector2.zero;
        _rigidbody = GetComponent<Rigidbody2D>();
        _boxSize = GetComponent<BoxCollider2D>().size;
        CollideWithGates(true);
        _initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        //O quanto o gameobject vai se mover
        float moveDistance = MoveSpeed * Time.fixedDeltaTime;

        //proxima posição final
        var nextMovePosition = _rigidbody.position + _currentMovementDirection * moveDistance;

        //=================================================================================
        //alinhar o gameobject(pacman/ghost) com a grid do position


        //se a movimentação esta sendo para cima
        if (_currentMovementDirection.y > 0)
        {
            //pega o valor e arredonda para cima. 1.1 -> 2 
            //ceil arredonda para cima
            var maxY = Mathf.CeilToInt(_rigidbody.position.y);

            //se ele passou ou bateu
            if (nextMovePosition.y >= maxY)
            {
                //forcar ele ficar na posicao maxY
                transform.position = new Vector2(_rigidbody.position.x, maxY);
                moveDistance = nextMovePosition.y - maxY;
            }
        }

        //se a movimentação esta sendo para esquerda
        if (_currentMovementDirection.x < 0)
        {
            //pega o valor e arredonda para baixo. -1.1 -> -2 
            //floor arredonda para baixo
            var minX = Mathf.FloorToInt(_rigidbody.position.x);

            if (nextMovePosition.x <= minX)
            {
                transform.position = new Vector2(minX, _rigidbody.position.y);
                moveDistance = minX - nextMovePosition.x;
            }
        }

        //se a movimentação esta sendo para baixo
        if (_currentMovementDirection.y < 0)
        {
            var minY = Mathf.FloorToInt(_rigidbody.position.y);

            if (nextMovePosition.y <= minY)
            {
                transform.position = new Vector2(_rigidbody.position.x, minY);
                moveDistance = minY - nextMovePosition.y;
            }
        }

        //se a movimentação esta sendo para direita
        if (_currentMovementDirection.x > 0)
        {
            var maxX = Mathf.CeilToInt(_rigidbody.position.x);

            if (nextMovePosition.x >= maxX)
            {
                transform.position = new Vector2(maxX, _rigidbody.position.y);
                moveDistance = nextMovePosition.x - maxX;
            }
        }

        //=================================================================================

        //metodo da fisica que sincroniza o componente do transform
        Physics2D.SyncTransforms();

        //verifica se o objeto esta alinhado na grid2D
        if ((_rigidbody.position.x == Mathf.CeilToInt(_rigidbody.position.x) &&
            _rigidbody.position.y == Mathf.CeilToInt(_rigidbody.position.y)) ||
            _currentMovementDirection == Vector2.zero)
        {
            OnAlignedWithGrid?.Invoke();
            if (_currentMovementDirection != _desiredMovementDirection)
            {
                //verificar se não esta batendo em nada
                // 1 << LayerMask.NameToLayer("Nome da Layer")
                if (!Physics2D.BoxCast(_rigidbody.position, _boxSize, 0, _desiredMovementDirection, 1f, _collisionLayerMask))
                {
                    _currentMovementDirection = _desiredMovementDirection;
                    OnDirectionChanged?.Invoke(CurrentMoveDirection);
                }
            }

            //verificar se ele colide com a layer andando para frente
            if (Physics2D.BoxCast(_rigidbody.position, _boxSize, 0, _currentMovementDirection, 1f, _collisionLayerMask))
            {
                _currentMovementDirection = Vector2.zero;
                OnDirectionChanged?.Invoke(CurrentMoveDirection);
            }
        }

        //movimentacao
        _rigidbody.MovePosition(_rigidbody.position + _currentMovementDirection * moveDistance);
    }

    public void CollideWithGates(bool shouldCollide)
    {
        if (shouldCollide)
        {
            _collisionLayerMask = LayerMask.GetMask(new string[] { "Level", "Gates" });
        } else
        {
            _collisionLayerMask = LayerMask.GetMask(new string[] { "Level" });
        }
    }

    public void ResetPosition()
    {
        _desiredMovementDirection = Vector2.zero;
        _currentMovementDirection = Vector2.zero;
        transform.position = _initialPosition;
        OnResetPosition?.Invoke();
    }

    public void SetMoveDirection(Direction newMoveDirection)
    {
        switch (newMoveDirection)
        {
            default:
            case Direction.None:
                break;

            case Direction.Up:
                _desiredMovementDirection = Vector2.up;
                break;

            case Direction.Left:
                _desiredMovementDirection = Vector2.left;
                break;

            case Direction.Down:
                _desiredMovementDirection = Vector2.down;
                break;

            case Direction.Right:
                _desiredMovementDirection = Vector2.right;
                break;
        }
    }

    public Direction CurrentMoveDirection
    {
        get
        {
            //up
            if (_currentMovementDirection.y > 0)
            {
                return Direction.Up;
            }

            //left
            if (_currentMovementDirection.x < 0)
            {
                return Direction.Left;
            }

            //down
            if (_currentMovementDirection.y < 0)
            {
                return Direction.Down;
            }

            //right
            if (_currentMovementDirection.x > 0)
            {
                return Direction.Right;
            }

            return Direction.None;
        }
    }

    private void OnDisable()
    {
        OnDisabled?.Invoke();
    }
}

public enum Direction
{
    None,
    Up,
    Left,
    Down,
    Right
}
