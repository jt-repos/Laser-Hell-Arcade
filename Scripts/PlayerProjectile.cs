using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    Player player;
    Color color;

    [SerializeField] bool isRocket = false;
    [SerializeField] bool isBarrageShot = false;
    bool isAcid = false;

    [Header("Colors")]
    [SerializeField] ColorConfig acidColor;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        isAcid = player.GetIsAcid();
        if (isRocket)
        {
            myRigidbody.velocity = new Vector2(0, player.GetProjectileSpeed() / 2);
        }
        else if (isBarrageShot)
        {
            myRigidbody.velocity = new Vector2(0, -player.GetProjectileSpeed());
        }
        else
        {
            Vector2 velocityUp = myRigidbody.transform.up * player.GetProjectileSpeed(); //leci w strone w która patrzy
            myRigidbody.velocity = velocityUp;
        }
        if (isAcid)
        {
            ChangeProjectileColor();
        }
    }

    private void ChangeProjectileColor()
    {
        foreach (Transform child in transform)
        {
            color = child.GetComponent<SpriteRenderer>().color;
            color.r = acidColor.GetRed();
            color.g = acidColor.GetGreen();
            color.b = acidColor.GetBlue();
            child.GetComponent<SpriteRenderer>().color = color;
        }
    }

    public bool GetIsAcid()
    {
        return isAcid;
    }

    public Player GetPlayer()
    {
        return player;
    }
}
