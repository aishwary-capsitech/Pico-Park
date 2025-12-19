using UnityEngine;

using System.Collections;

public class TeamJumpRamp : MonoBehaviour
{
    public static TeamJumpRamp Instance;

    public int totalPlayers = 2;          // how many players in level

    public float jumpTimeWindow = 0.3f;   // max allowed delay

    public float moveDistance = 6f;

    public float moveSpeed = 2f;

    private int jumpedPlayers = 0;

    private bool resolved = false;

    private Vector3 startPos;

    void Awake()
    {
        Instance = this;
    }

    void Start()

    {

        startPos = transform.position;

    }

    // called by players when they jump

    public void PlayerJumped()

    {

        if (resolved) return;

        jumpedPlayers++;

        if (jumpedPlayers == 1)

        {

            StartCoroutine(CheckTeamJump());

        }

    }

    IEnumerator CheckTeamJump()

    {

        yield return new WaitForSeconds(jumpTimeWindow);

        if (jumpedPlayers == totalPlayers)

        {

            resolved = true;

            StartCoroutine(MoveRamp());

        }

        else

        {

            resolved = true;

            BreakRamp();

        }

    }

    IEnumerator MoveRamp()

    {

        Vector3 targetPos = startPos + Vector3.right * moveDistance;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)

        {

            transform.position = Vector3.MoveTowards(

                transform.position,

                targetPos,

                moveSpeed * Time.deltaTime

            );

            yield return null;

        }

    }

    void BreakRamp()

    {

        GetComponent<BoxCollider2D>().enabled = false;

        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 3f;

    }

}

