using UnityEngine;

public class PacmanView : MonoBehaviour
{
    public CharacterMotor CharacterMotor;

    public Animator Animator;

    public Life CharacterLife;

    public AudioSource AudioSource;
    public AudioClip LifeLostSound;

    private void Start()
    {
        CharacterMotor.OnDirectionChanged += CharacterMotor_OnDirectionChanged;
        CharacterMotor.OnResetPosition += CharacterMotor_OnResetPosition;
        CharacterMotor.OnDisabled += CharacterMotor_OnDisabled;
        CharacterLife.OnLifeRemoved += CharacterLife_OnLifeRemoved;
        Animator.SetBool("Moving", false);
        Animator.SetBool("Dead", false);

    }

    //A velocidade anima��o do pacman fica desativo
    private void CharacterMotor_OnDisabled()
    {
        Animator.speed = 0;
    }

    //Quando reseta a cena a anima��o de mover e morto n�o s�o ativadas
    private void CharacterMotor_OnResetPosition()
    {
        Animator.SetBool("Moving", false);
        Animator.SetBool("Dead", false);
    }

    //Ao perder uma vida
    private void CharacterLife_OnLifeRemoved(int _)
    {
        Animator.speed = 1;
        //dire��o que ele esta
        transform.Rotate(0, 0, -90);
        //audio de morte
        AudioSource.PlayOneShot(LifeLostSound);
        //anima��o de mover fica desativado
        Animator.SetBool("Moving", false);
        //anima��o de morte ativado
        Animator.SetBool("Dead", true);
    }

    private void CharacterMotor_OnDirectionChanged(Direction direction)
    {
        switch (direction)
        {
            case Direction.None:
                //ativar animacao
                Animator.SetBool("Moving", false);
                break;

            case Direction.Up:
                Animator.SetBool("Moving", true);
                //fazer rotacao da view
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;

            case Direction.Left:
                Animator.SetBool("Moving", true);
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;

            case Direction.Down:
                Animator.SetBool("Moving", true);
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;

            case Direction.Right:
                Animator.SetBool("Moving", true);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
        }
    }
}
