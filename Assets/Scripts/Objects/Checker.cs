using BaseStructures;
using UnityEngine;
using UnityEngine.UI;

public class Checker : MonoBehaviour
{
    /* coiuntId - cчетчик id для шайб
     * upCount - счетчик количества шайб у верхнего игрока
     * downCoun - счетчик количества шайб у нижнего игрока
     */
    public static byte countId = 0;
    public byte id;

    //Нитка
    public BezierLine DownString, UpString;

    // Физическое тело
    public Rigidbody2D body;
    public Transform objTransform; // компоненты Transfrom объекта

    bool mouseDown = false; // Проверка нажатия на предмет
    float V = 0.0f, radius; // начальная скорость объекта и радиус объекта

    // Границы поля
    public GameObject leftBorderHolder;
    public GameObject rightBorderHolder;
    public Border playerDownBorder;
    public Border playerUpBorder;

    public struct Border
    {
        public float Up, Down, Left, Right;

        public Border(float Up, float Down, float Left, float Right)
        {
            this.Up = Up;
            this.Down = Down;
            this.Left = Left;
            this.Right = Right;
        }
    }


    public enum Field
    {
        Up, 
        Down
    }
    public Field field; 

    private void Awake()
    {
        id = ++countId;
        // Оптимизация под разные экраны
        ScreenOptimization.setSize(gameObject, this.GetComponent<CircleCollider2D>());
        ScreenOptimization.setColider(gameObject, this.GetComponent<CircleCollider2D>());

        Debug.Log(gameObject.GetComponent<RectTransform>().sizeDelta);
        radius = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x * (gameObject.GetComponent<CircleCollider2D>().radius / Screen.width) * 2; // радиус в мировых координатах

        objTransform = GetComponent<Transform>(); // Оптимизация чтобы не вызывать постоянно GetComponent для Transform
        body = GetComponent<Rigidbody2D>(); // Оптимизация чтобы не вызывать постоянно GetComponent для Rigidbody2D

        gameObject.transform.GetChild(0).GetComponent<TrailRenderer>().emitting = true; // След шайбы

        field = objTransform.position.y > 0 ? Field.Up : Field.Down; 
    }

    void Start()
    {
        // Границы поля
        Pair<Vector2, Vector2> points;
        points = ScreenOptimization.GetWorldCoord2D(leftBorderHolder);
        playerDownBorder = new Border(
            points.first.y - radius,
            points.second.y + radius,
            points.first.x + radius,
            points.second.x - radius);

        points = ScreenOptimization.GetWorldCoord2D(rightBorderHolder);
        playerUpBorder = new Border(
            points.first.y - radius,
            points.second.y + radius,
            points.first.x + radius,
            points.second.x - radius);
    }

    private void OnMouseDrag()
    {
        Vector2 Cursor = Input.mousePosition;
        Cursor = Camera.main.ScreenToWorldPoint(Cursor);

        if (objTransform.position.y < 0)
        {
            Vector2 clampedMousePos = new Vector2(Mathf.Clamp(Cursor.x, playerDownBorder.Left, playerDownBorder.Right),
            Mathf.Clamp(Cursor.y, playerDownBorder.Down, playerDownBorder.Up));
            transform.position = Vector2.MoveTowards(transform.position, clampedMousePos, Time.deltaTime * 100f);
        }
        else
        {
            Vector2 clampedMousePos = new Vector2(Mathf.Clamp(Cursor.x, playerUpBorder.Left, playerUpBorder.Right),
            Mathf.Clamp(Cursor.y, playerUpBorder.Down, playerUpBorder.Up));
            transform.position = Vector2.MoveTowards(transform.position, clampedMousePos, Time.deltaTime * 100f);
        }

    }


    public void OnMouseDown()
    {
        body.velocity *= 0;
        V = 0.0f;
        mouseDown = true;
    }

    public void OnMouseUp()
    {
        mouseDown = false;
        if (objTransform.position.y < 0)
        {
            float checkY = DownString.coordY + radius + DownString.correction;
            if (objTransform.position.y < checkY)
                V = ((checkY - objTransform.position.y) * 124 + 4) / 20.0f; // Формула рассчета начальной скорости объекта
            objTransform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            float checkY = UpString.coordY - radius - UpString.correction;
            if (objTransform.position.y > checkY)
                V = ((objTransform.position.y - checkY) * 124 + 4) / 20.0f; // Формула рассчета начальной скорости объекта
            objTransform.rotation = Quaternion.Euler(0, 0, -90);
        }

        body.AddForce(transform.right * V * 300);
    }

    public float getRadius()
    {
        return radius;
    }

    public bool getMouseDown()
    {
        return mouseDown;
    }
}