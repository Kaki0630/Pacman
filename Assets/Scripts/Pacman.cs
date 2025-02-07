using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Pacman : MonoBehaviour
{
    [SerializeField]
    private AnimatedSprite deathSequence;       // 死亡時のアニメーション
    private SpriteRenderer spriteRenderer;      // スプライトの描画コンポーネント
    private CircleCollider2D circleCollider;    // 衝突判定用のコライダー
    private Movement movement;                  // 移動を管理するコンポーネント

    private void Awake()
    {
        // 必要なコンポーネントを取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        // ユーザーの入力に応じて移動方向を変更
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

        // パックマンを移動方向に応じて回転
        float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    public void ResetState()
    {
        // パックマンの状態をリセット
        enabled = true;
        spriteRenderer.enabled = true;
        circleCollider.enabled = true;
        deathSequence.enabled = false;
        movement.ResetState();
        gameObject.SetActive(true);
    }

    public void DeathSequence()
    {
        // パックマンが死亡した際の処理
        enabled = false;
        spriteRenderer.enabled = false;
        circleCollider.enabled = false;
        movement.enabled = false;
        deathSequence.enabled = true;
        deathSequence.Restart();        // 死亡アニメーションを再生
    }

}