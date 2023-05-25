using UnityEngine;

public class Player : MonoBehaviour
{
    float MoveSpeed;
    float BaseSpeed = 5f;
    float timerPowerUp;
    float timeToPowerUps = 5f;

    bool hasKey = false;
    bool hasPowerUp = false;
    bool captainAmerica = false;

    Rigidbody rb;

    public Material shield_material;
    public Material speed_material;

    Material base_material;
    MeshRenderer player_mesh;

    enum PowerUp { Speed, Shield }

    private void Start()
    {
        MoveSpeed = BaseSpeed;
        timerPowerUp = timeToPowerUps;
        rb = GetComponent<Rigidbody>();
        player_mesh = gameObject.GetComponent<MeshRenderer>();
        base_material = player_mesh.material;
    }

    public void Restart()
    {
        hasKey = false;
        hasPowerUp = false;
        timerPowerUp = timeToPowerUps;
        MoveSpeed = BaseSpeed;
        player_mesh.material = base_material;
    }

    private void Update()
    {
        if (hasPowerUp)
        {
            timerPowerUp -= Time.deltaTime;

            if (timerPowerUp <= 0)
            {
                hasPowerUp = false;
                captainAmerica = false;

                MoveSpeed = BaseSpeed;
                timerPowerUp = timeToPowerUps;

                player_mesh.material = base_material;

                UIMngr.Instance.HideInInventory(Item.PowerUpSpeed);
            }
        }

        if (GameManager.Instance.GamePaused)
        {
            MoveSpeed = 0f;
        }
    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical") * -1;

        Vector3 movement = new Vector3(moveVertical, 0f, moveHorizontal);

        rb.velocity = movement * MoveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.collider.tag;

        switch (tag)
        {
            case "Key":

                hasKey = true;

                UIMngr.Instance.ShowInInventory(Item.Key);

                Destroy(collision.collider.gameObject);

                break;

            case "Item":

                int point = collision.collider.gameObject.GetComponent<ItemController>().Point;

                GameManager.Instance.UpdateScore(point);

                Destroy(collision.collider.gameObject);

                break;

            case "Coin":

                GameManager.Instance.UpdateScore(5);

                Destroy(collision.collider.gameObject);

                break;

            case "Door":

                if (hasKey)
                {
                    UIMngr.Instance.HideInInventory(Item.Key);

                    Destroy(collision.collider.gameObject);
                }

                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        switch (tag)
        {
            case "PowerUp":

                if (hasPowerUp)
                {
                    timerPowerUp = timeToPowerUps;
                }
                else
                {
                    hasPowerUp = true;
                }

                if (other.gameObject.name == "PowerUp_Speed(Clone)")
                {
                    if (MoveSpeed == BaseSpeed)
                    {
                        MoveSpeed *= 2f;
                    }
                    player_mesh.material = speed_material;
                    UIMngr.Instance.ShowInInventory(Item.PowerUpSpeed);
                }
                else
                {
                    captainAmerica = true;
                    player_mesh.material = shield_material;
                    UIMngr.Instance.ShowInInventory(Item.PowerUpShield);
                }

                Destroy(other.gameObject);

                break;

            case "Trap":

                if (!captainAmerica)
                {
                    GameManager.Instance.GameOver();
                }

                break;

            case "Exit":

                GameManager.Instance.SetStageComplete();
                Destroy(other.gameObject);

                break;
        }
    }
}
