using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Pacman : MonoBehaviour
{
    [SerializeField]
    private AnimatedSprite deathSequence;       // ���S���̃A�j���[�V����
    private SpriteRenderer spriteRenderer;      // �X�v���C�g�̕`��R���|�[�l���g
    private CircleCollider2D circleCollider;    // �Փ˔���p�̃R���C�_�[
    private Movement movement;                  // �ړ����Ǘ�����R���|�[�l���g

    private void Awake()
    {
        // �K�v�ȃR���|�[�l���g���擾
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        // ���[�U�[�̓��͂ɉ����Ĉړ�������ύX
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            movement.SetDirection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            movement.SetDirection(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movement.SetDirection(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            movement.SetDirection(Vector2.right);
        }

        // �p�b�N�}�����ړ������ɉ����ĉ�]
        float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    public void ResetState()
    {
        // �p�b�N�}���̏�Ԃ����Z�b�g
        enabled = true;
        spriteRenderer.enabled = true;
        circleCollider.enabled = true;
        deathSequence.enabled = false;
        movement.ResetState();
        gameObject.SetActive(true);
    }

    public void DeathSequence()
    {
        // �p�b�N�}�������S�����ۂ̏���
        enabled = false;
        spriteRenderer.enabled = false;
        circleCollider.enabled = false;
        movement.enabled = false;
        deathSequence.enabled = true;
        deathSequence.Restart();        // ���S�A�j���[�V�������Đ�
    }

}