using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class Head : MonoBehaviour
{
    enum MovementDirection
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NONE
    }

    private double TIC_TIME = .2;
    private const int MAX_BODY_LENGTH = 10;
    private int headPosX;
    private int headPosY;
    private int oldHeadPosX;
    private int head_pos_y;
    private int oldHeadPosY;
    private float movementStartTime;
    private MovementDirection movementDirection;
    public GameObject bodyTilePrefab;
    public GameObject foodPelletPrefab;
    private MovementDirection headDirection;
    List<GameObject> bodyPrefabs = new List<GameObject>();
    List<MovementDirection> bodyPrefabOrientation = new List<MovementDirection>();
    public Sprite snakeCorner;
    public Sprite snakeCorner2;
    public Sprite snakeBody;
    GameObject foodPrefab;
    private bool[][] snakeIsHere;
    public static Notification notification;

    // Start is called before the first frame update
    void Start()
    {
        headPosX = (int)this.transform.position.x;
        headPosY = (int)this.transform.position.y;
        head_pos_y = (int)this.transform.position.y;
        movementStartTime = 0;
        movementDirection = MovementDirection.NONE;
        headDirection = MovementDirection.LEFT;

        // Body prefabs + orientation
        bodyPrefabs.Add(new GameObject());
        bodyPrefabOrientation.Add(MovementDirection.LEFT);
        bodyPrefabs.Add(new GameObject());
        bodyPrefabOrientation.Add(MovementDirection.LEFT);
        bodyPrefabs.Add(new GameObject());
        bodyPrefabOrientation.Add(MovementDirection.LEFT);
        bodyPrefabs[0] = Instantiate(bodyTilePrefab, new Vector3(1, 0, 0), Quaternion.identity); // [1][0]
        bodyPrefabs[1] = Instantiate(bodyTilePrefab, new Vector3(2, 0, 0), Quaternion.identity); // [3][0]
        bodyPrefabs[2] = Instantiate(bodyTilePrefab, new Vector3(3, 0, 0), Quaternion.identity); // [5][0]

        //for (int i = 0; i < 18; i++)
            //for (int j = 0; j < 19; j++)
                //snakeIsHere[i][j] = false;

        int foodPelletX = Random.Range(-11, 7);
        int foodPelletY = Random.Range(-9, 10);
        foodPrefab = Instantiate(foodPelletPrefab, new Vector3(foodPelletX, foodPelletY, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
        }


        if ((Input.GetKeyDown("left") || Input.GetKeyDown("a")) && movementDirection != MovementDirection.LEFT && movementDirection != MovementDirection.RIGHT && headPosX != -12)
        //if ((Input.GetKeyDown("left") || Input.GetKeyDown("a")) && movementDirection != MovementDirection.RIGHT && headPosX != -11)
        {
            MoveLeft();
            ResetMovementTimer();
        }

        if ((Input.GetKeyDown("right") || Input.GetKeyDown("d")) && movementDirection != MovementDirection.RIGHT && movementDirection != MovementDirection.LEFT && headPosX != 6)
        //if ((Input.GetKeyDown("right") || Input.GetKeyDown("d")) && movementDirection != MovementDirection.LEFT && headPosX != 10)
        {
            MoveRight();
            ResetMovementTimer();
        }

        if ((Input.GetKeyDown("up") || Input.GetKeyDown("w")) && movementDirection != MovementDirection.UP && movementDirection != MovementDirection.DOWN && head_pos_y != 10)
        //if ((Input.GetKeyDown("up") || Input.GetKeyDown("w")) && movementDirection != MovementDirection.DOWN && head_pos_y != 10)
        {
            MoveUp();
            ResetMovementTimer();
        }

        if ((Input.GetKeyDown("down") || Input.GetKeyDown("s")) && movementDirection != MovementDirection.DOWN && movementDirection != MovementDirection.UP && head_pos_y != -11)
        //if ((Input.GetKeyDown("down") || Input.GetKeyDown("s")) && movementDirection != MovementDirection.UP && head_pos_y != -11)
        {
            MoveDown();
            ResetMovementTimer();
        }

        // If no input is given, the snake continues to slither in the same direction.
        if (Time.time - movementStartTime > TIC_TIME)
        {
            if (movementDirection == MovementDirection.RIGHT && headPosX <= 9)
            {
                MoveRight();
                movementStartTime = Time.time;
            }
            else if (movementDirection == MovementDirection.LEFT && headPosX >= -10)
            {
                MoveLeft();
                movementStartTime = Time.time;
            }
            else if (movementDirection == MovementDirection.UP && head_pos_y <= 9)
            {
                MoveUp();
                movementStartTime = Time.time;
            }
            else if (movementDirection == MovementDirection.DOWN && head_pos_y >= -10)
            {
                MoveDown();
                movementStartTime = Time.time;
            }
        }
    }

    void UpdateSnake(int x, int y, MovementDirection direction)
    {
        // Sets head direction
        this.headDirection = direction;

        float neckRotation = bodyPrefabs[0].transform.eulerAngles.z;

        bodyPrefabs[0].transform.position = new Vector3(this.headPosX, this.headPosY, 0);
        //bodyPrefabOrientation[0] = this.headDirection;
        this.headPosX = x;
        this.headPosY = y;

        // Draw body
        for (int i = 1; i < bodyPrefabs.Count; i++)
        {
            //print("asdf: " + bodyPrefabs[bodyPrefabs.Count - 1].transform.eulerAngles.z);
            bodyPrefabs[bodyPrefabs.Count - i].transform.position = new Vector3(
                bodyPrefabs[bodyPrefabs.Count - i - 1].transform.position.x,
                bodyPrefabs[bodyPrefabs.Count - i - 1].transform.position.y,
                0
            );
            //bodyPrefabOrientation[bodyPrefabs.Count - i] = MovementDirection.LEFT;
            //bodyPrefabOrientation[bodyPrefabs.Count - i] = bodyPrefabOrientation[bodyPrefabOrientation.Count - i - 1];
            //print("bodyPrefabOrientation.Count - i: " + (bodyPrefabs.Count - i));
        }

//        bodyPrefabs[0].transform.position = new Vector3(this.headPosX, this.headPosY, 0);
//        //bodyPrefabOrientation[0] = this.headDirection;
//        this.headPosX = x;
//        this.headPosY = y;

        // Rotates body tiles as needed.
        /*for (int i = 0; i < bodyPrefabOrientation.Count - 1; i++)
        {
            if (bodyPrefabOrientation[i] == MovementDirection.UP && bodyPrefabOrientation[i + 1] == MovementDirection.LEFT)
            {
                bodyPrefabs[i].GetComponent<SpriteRenderer>().sprite = snakeCorner;
                bodyPrefabs[i].transform.eulerAngles = new Vector3(0f, 0f, 180f);
            }
            else if (bodyPrefabOrientation[i] == MovementDirection.UP && bodyPrefabOrientation[i + 1] == MovementDirection.RIGHT)
            {
                bodyPrefabs[i].GetComponent<SpriteRenderer>().sprite = snakeCorner;
                bodyPrefabs[i].transform.eulerAngles = new Vector3(0f, 0f, 270f);
            }
            else if (bodyPrefabOrientation[i] == MovementDirection.DOWN && bodyPrefabOrientation[i + 1] == MovementDirection.LEFT)
            {
                bodyPrefabs[i].GetComponent<SpriteRenderer>().sprite = snakeCorner;
                bodyPrefabs[i].transform.eulerAngles = new Vector3(0f, 0f, 90f);
            }
            else if (bodyPrefabOrientation[i] == MovementDirection.DOWN && bodyPrefabOrientation[i + 1] == MovementDirection.RIGHT)
            {
                bodyPrefabs[i].GetComponent<SpriteRenderer>().sprite = snakeCorner;
                bodyPrefabs[i].transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
            else
            {
                bodyPrefabs[i].GetComponent<SpriteRenderer>().sprite = snakeBody;
                if (bodyPrefabOrientation[i] == MovementDirection.RIGHT)
                    bodyPrefabs[i].transform.eulerAngles = new Vector3(0f, 0f, 180f);
                else if (bodyPrefabOrientation[i] == MovementDirection.LEFT)
                    bodyPrefabs[i].transform.eulerAngles = new Vector3(0f, 0f, 0f);
                else if (bodyPrefabOrientation[i] == MovementDirection.UP)
                    bodyPrefabs[i].transform.eulerAngles = new Vector3(0f, 0f, 270f);
                else if (bodyPrefabOrientation[i] == MovementDirection.DOWN)
                    bodyPrefabs[i].transform.eulerAngles = new Vector3(0f, 0f, 90f);
            }
        }*/

        // Draw head
        this.transform.position = new Vector3(this.headPosX, this.headPosY, 0);
        if (direction == MovementDirection.RIGHT)
            this.transform.eulerAngles = new Vector3(0f, 0f, 180f);
        else if (direction == MovementDirection.LEFT)
            this.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        else if (direction == MovementDirection.UP)
            this.transform.eulerAngles = new Vector3(0f, 0f, 270f);
        else if (direction == MovementDirection.DOWN)
            this.transform.eulerAngles = new Vector3(0f, 0f, 90f);
        else
            print("Error!  Direction not recognized in Head.UpdateSnake()!");
    }

    void ResetMovementTimer()
    {
        movementStartTime = Time.time;
    }

    void MoveLeft()
    {
        // DEBUGGING
        if (movementDirection == MovementDirection.LEFT)
        {
            bodyPrefabs[0].GetComponent<SpriteRenderer>().sprite = snakeBody;
            bodyPrefabs[0].transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        bodyPrefabs[bodyPrefabs.Count - 1].GetComponent<SpriteRenderer>().forceRenderingOff = false;

        // Record direction.
        MovementDirection oldMovementDirection = this.movementDirection;
        if (this.movementDirection != MovementDirection.LEFT && this.movementDirection != MovementDirection.NONE)
            bodyPrefabs[0].GetComponent<SpriteRenderer>().sprite = snakeCorner;
        this.movementDirection = MovementDirection.LEFT;

        // Change head position.
        oldHeadPosX = headPosX;
        oldHeadPosY = headPosY;
        headPosX--;
        this.transform.position = new Vector3(this.headPosX, this.headPosY, 0);

        // Change head rotation.
        this.transform.eulerAngles = new Vector3(0f, 0f, 0f);

        // Change neck position.
        bodyPrefabs[0].transform.position = new Vector3(this.oldHeadPosX, this.oldHeadPosY, 0);

        // Change neck rotation.
        if (oldMovementDirection == MovementDirection.UP)
            bodyPrefabs[0].transform.eulerAngles = new Vector3(0f, 0f, 0f);
        else if (oldMovementDirection == MovementDirection.DOWN)
        {
            bodyPrefabs[0].GetComponent<SpriteRenderer>().sprite = snakeCorner2;
            bodyPrefabs[0].transform.eulerAngles = new Vector3(0f, 0f, 270f);
        }

        // Change non-neck body tiles
        for (int i = 1; i < bodyPrefabs.Count; i++)
        {
            Sprite originalSprite = bodyPrefabs[bodyPrefabs.Count - i - 1].GetComponent<SpriteRenderer>().sprite;
            float originalRotation = bodyPrefabs[bodyPrefabs.Count - i - 1].transform.eulerAngles.z;

            // Positions.
            bodyPrefabs[bodyPrefabs.Count - i].transform.position = new Vector3(
                bodyPrefabs[bodyPrefabs.Count - i - 1].transform.position.x,
                bodyPrefabs[bodyPrefabs.Count - i - 1].transform.position.y,
                0
            );

            // Sprites.
            bodyPrefabs[bodyPrefabs.Count - i].GetComponent<SpriteRenderer>().sprite = originalSprite;

            // Rotations.
            bodyPrefabs[bodyPrefabs.Count - i].transform.eulerAngles = new Vector3(0f, 0f, originalRotation);
        }
    }

    void MoveRight()
    {
        // DEBUGGING
        if (movementDirection == MovementDirection.RIGHT)
        {
            bodyPrefabs[0].GetComponent<SpriteRenderer>().sprite = snakeBody;
            bodyPrefabs[0].transform.eulerAngles = new Vector3(0f, 0f, 180f);
        }
        bodyPrefabs[bodyPrefabs.Count - 1].GetComponent<SpriteRenderer>().forceRenderingOff = false;

        // Record direction.
        MovementDirection oldMovementDirection = this.movementDirection;
        if (this.movementDirection != MovementDirection.RIGHT && this.movementDirection != MovementDirection.NONE)
            bodyPrefabs[0].GetComponent<SpriteRenderer>().sprite = snakeCorner;
        this.movementDirection = MovementDirection.RIGHT;

        // Change head position.
        oldHeadPosX = headPosX;
        oldHeadPosY = headPosY;
        headPosX++;
        this.transform.position = new Vector3(this.headPosX, this.headPosY, 0);

        // Change head rotation.
        this.transform.eulerAngles = new Vector3(0f, 0f, 180f);

        // Change neck position.
        bodyPrefabs[0].transform.position = new Vector3(this.oldHeadPosX, this.oldHeadPosY, 0);

        // Change neck rotation.
        if (oldMovementDirection == MovementDirection.UP)
        {
            bodyPrefabs[0].GetComponent<SpriteRenderer>().sprite = snakeCorner2;
            bodyPrefabs[0].transform.eulerAngles = new Vector3(0f, 0f, 90f);
        }
        else if (oldMovementDirection == MovementDirection.DOWN)
            bodyPrefabs[0].transform.eulerAngles = new Vector3(0f, 0f, 180f);

        //Sprite oldSprite1 = bodyPrefabs[1].GetComponent<SpriteRenderer>().sprite;

        // Change non-neck body tiles
        for (int i = 1; i < bodyPrefabs.Count; i++)
        {
            Sprite originalSprite = bodyPrefabs[bodyPrefabs.Count - i - 1].GetComponent<SpriteRenderer>().sprite;
            float originalRotation = bodyPrefabs[bodyPrefabs.Count - i - 1].transform.eulerAngles.z;

            // Positions.
            bodyPrefabs[bodyPrefabs.Count - i].transform.position = new Vector3(
                bodyPrefabs[bodyPrefabs.Count - i - 1].transform.position.x,
                bodyPrefabs[bodyPrefabs.Count - i - 1].transform.position.y,
                0
            );

            // Sprites.
            bodyPrefabs[bodyPrefabs.Count - i].GetComponent<SpriteRenderer>().sprite = originalSprite;

            // Rotations.
            bodyPrefabs[bodyPrefabs.Count - i].transform.eulerAngles = new Vector3(0f, 0f, originalRotation);
        }
    }

    void MoveUp()
    {
        // DEBUGGING
        if (movementDirection == MovementDirection.UP)
        {
            bodyPrefabs[0].GetComponent<SpriteRenderer>().sprite = snakeBody;
            bodyPrefabs[0].transform.eulerAngles = new Vector3(0f, 0f, 270f);
        }
        bodyPrefabs[bodyPrefabs.Count - 1].GetComponent<SpriteRenderer>().forceRenderingOff = false;

        // Record direction.
        MovementDirection oldMovementDirection = this.movementDirection;
        if (this.movementDirection != MovementDirection.UP && this.movementDirection != MovementDirection.NONE)
            bodyPrefabs[0].GetComponent<SpriteRenderer>().sprite = snakeCorner;
        this.movementDirection = MovementDirection.UP;

        // Change head position.
        oldHeadPosX = headPosX;
        oldHeadPosY = headPosY;
        headPosY++;
        this.transform.position = new Vector3(this.headPosX, this.headPosY, 0);

        // Change head rotation.
        this.transform.eulerAngles = new Vector3(0f, 0f, 270f);

        // Change neck position.
        bodyPrefabs[0].transform.position = new Vector3(this.oldHeadPosX, this.oldHeadPosY, 0);

        // Change neck rotation.
        if (oldMovementDirection == MovementDirection.LEFT)
        {
            bodyPrefabs[0].GetComponent<SpriteRenderer>().sprite = snakeCorner2;
            bodyPrefabs[0].transform.eulerAngles = new Vector3(0f, 0f, 180f);
        }
        else if (oldMovementDirection == MovementDirection.RIGHT)
        {
            bodyPrefabs[0].transform.eulerAngles = new Vector3(0f, 0f, 270f);
        }

        // Change non-neck body tiles
        for (int i = 1; i < bodyPrefabs.Count; i++)
        {
            Sprite originalSprite = bodyPrefabs[bodyPrefabs.Count - i - 1].GetComponent<SpriteRenderer>().sprite;
            float originalRotation = bodyPrefabs[bodyPrefabs.Count - i - 1].transform.eulerAngles.z;

            // Positions.
            bodyPrefabs[bodyPrefabs.Count - i].transform.position = new Vector3(
                bodyPrefabs[bodyPrefabs.Count - i - 1].transform.position.x,
                bodyPrefabs[bodyPrefabs.Count - i - 1].transform.position.y,
                0
            );

            // Sprites.
            bodyPrefabs[bodyPrefabs.Count - i].GetComponent<SpriteRenderer>().sprite = originalSprite;

            // Rotations.
            bodyPrefabs[bodyPrefabs.Count - i].transform.eulerAngles = new Vector3(0f, 0f, originalRotation);
        }
    }

    void MoveDown()
    {
        // DEBUGGING
        if (movementDirection == MovementDirection.DOWN)
        {
            bodyPrefabs[0].GetComponent<SpriteRenderer>().sprite = snakeBody;
            bodyPrefabs[0].transform.eulerAngles = new Vector3(0f, 0f, 90f);
        }
        bodyPrefabs[bodyPrefabs.Count - 1].GetComponent<SpriteRenderer>().forceRenderingOff = false;

        // Record direction.
        MovementDirection oldMovementDirection = this.movementDirection;
        if (this.movementDirection != MovementDirection.DOWN && this.movementDirection != MovementDirection.NONE)
            bodyPrefabs[0].GetComponent<SpriteRenderer>().sprite = snakeCorner;
        this.movementDirection = MovementDirection.DOWN;

        // Change head position.
        oldHeadPosX = headPosX;
        oldHeadPosY = headPosY;
        headPosY--;
        this.transform.position = new Vector3(this.headPosX, this.headPosY, 0);

        // Change head rotation.
        this.transform.eulerAngles = new Vector3(0f, 0f, 90f);

        // Change neck position.
        bodyPrefabs[0].transform.position = new Vector3(this.oldHeadPosX, this.oldHeadPosY, 0);

        // Change neck rotation.
        if (oldMovementDirection == MovementDirection.LEFT)
            bodyPrefabs[0].transform.eulerAngles = new Vector3(0f, 0f, 90f);
        else if (oldMovementDirection == MovementDirection.RIGHT)
        {
            bodyPrefabs[0].GetComponent<SpriteRenderer>().sprite = snakeCorner2;
            bodyPrefabs[0].transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }

        //Sprite oldSprite1 = bodyPrefabs[1].GetComponent<SpriteRenderer>().sprite;

        // Change non-neck body tiles
        for (int i = 1; i < bodyPrefabs.Count; i++)
        {
            Sprite originalSprite = bodyPrefabs[bodyPrefabs.Count - i - 1].GetComponent<SpriteRenderer>().sprite;
            float originalRotation = bodyPrefabs[bodyPrefabs.Count - i - 1].transform.eulerAngles.z;

            // Positions.
            bodyPrefabs[bodyPrefabs.Count - i].transform.position = new Vector3(
                bodyPrefabs[bodyPrefabs.Count - i - 1].transform.position.x,
                bodyPrefabs[bodyPrefabs.Count - i - 1].transform.position.y,
                0
            );

            // Sprites.
            bodyPrefabs[bodyPrefabs.Count - i].GetComponent<SpriteRenderer>().sprite = originalSprite;

            // Rotations.
            bodyPrefabs[bodyPrefabs.Count - i].transform.eulerAngles = new Vector3(0f, 0f, originalRotation);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "foodPellet(Clone)")
        {
            GameObject.Destroy(foodPrefab);
            int x = (int)this.transform.position.x;
            int y = (int)this.transform.position.y;
            notification.PlayEatAnim(x, y + .5f);

            TIC_TIME = TIC_TIME * 0.99;
            int foodPelletX = Random.Range(-11, 7);
            int foodPelletY = Random.Range(-9, 10);
            foodPrefab = Instantiate(foodPelletPrefab, new Vector3(foodPelletX, foodPelletY, 0), Quaternion.identity);
            bodyPrefabs.Add(new GameObject());
            int lastX = (int)bodyPrefabs[bodyPrefabs.Count - 2].GetComponent<Transform>().position.x;
            int lastY = (int)bodyPrefabs[bodyPrefabs.Count - 2].GetComponent<Transform>().position.y;
            int lastZ = (int)bodyPrefabs[bodyPrefabs.Count - 2].GetComponent<Transform>().position.z;
            bodyPrefabs[bodyPrefabs.Count - 1] = Instantiate(bodyTilePrefab, new Vector3(lastX, lastY, lastZ), Quaternion.identity);
            bodyPrefabs[bodyPrefabs.Count - 1].GetComponent<SpriteRenderer>().forceRenderingOff = true;
        }
        else if (other.name == "bodyTilePrefab(Clone)")
        {
            int x = (int)this.transform.position.x;
            int y = (int)this.transform.position.y;
            notification.PlayOuchAnim(x + 1, y + 1);
            other.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
        }
    }
}