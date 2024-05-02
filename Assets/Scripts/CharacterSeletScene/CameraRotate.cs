using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;




public class CameraRotate : MonoBehaviour
{
    float side;
    float ver;
    const float speedRaw = 2f;
    float speed;
    const float speedStep = 0.2f;
    [SerializeField] TextMeshProUGUI speedText;
    float speedTextShowTime = 0f; // Œ¸‚Á‚Ä‚¢‚­
    [SerializeField] GameObject mainCamera;
    [SerializeField] Vector3 cameraStartPos = new Vector3 (19f,180f,0);

    [SerializeField] Tutorial tutorial;

    void Start()
    {
        speed = speedRaw * BaseSystem.GameData.GameData.DirectionMoveSpeedCoef;

        mainCamera.transform.localEulerAngles = cameraStartPos;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //StartCoroutine("ForStartCamera");
    }
    float additionalPower;
    [SerializeField] GameObject angleDetector;
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha4))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                BaseSystem.GameData.GameData.DirectionMoveSpeedCoef += speedStep;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                BaseSystem.GameData.GameData.DirectionMoveSpeedCoef -= speedStep;
            }
            else return;

            speed = speedRaw * BaseSystem.GameData.GameData.DirectionMoveSpeedCoef;
            speedTextShowTime = 1;
            speedText.text = "~ " + BaseSystem.GameData.GameData.DirectionMoveSpeedCoef.ToString("F1"); // ¬”‘æˆêˆÊ‚Ü‚Å
            speedText.enabled = true;
        }

        //if (tutorial.tutorailFin)
        //{
        Physics.Raycast(angleDetector.transform.position, Vector3.up, out RaycastHit hit, 6.0f);
            Vector2 val = IA.InputGetter.Instance.ValueDirection;
            float h = -val.x;
            float v = -val.y;
            float div = 5;
            if ((angleDetector.transform.position - hit.transform.gameObject.transform.position).sqrMagnitude > Mathf.Pow(2, 2))
            {
                additionalPower -= (Vector3.Distance(angleDetector.transform.position, hit.transform.gameObject.transform.position) - 2) / div;
            }
            else if ((angleDetector.transform.position - hit.transform.gameObject.transform.position).sqrMagnitude < Mathf.Pow(1.5f, 2))
            {
                additionalPower += (1.5f - Vector3.Distance(angleDetector.transform.position, hit.transform.gameObject.transform.position)) / div;
            }
            else
            {
                additionalPower = 0;
            }
            side += h * speed;
            ver += (v + additionalPower) * speed;
            // Debug.Log(Input.GetAxis(gMouse Xh));


            transform.rotation = Quaternion.Euler(ver, -side, 0f);
        // }

        if (speedTextShowTime > 0f)
        {
            speedTextShowTime -= Time.deltaTime;

            if (speedTextShowTime <= 0f)
            {
                speedTextShowTime = 0f;
                speedText.enabled = false;
            }
        }
    }

    IEnumerator ForStartCamera()
    {

        this.gameObject.GetComponent<CameraRotate>().enabled = false;

        yield return new WaitForSeconds(1f);

        this.gameObject.GetComponent<CameraRotate>().enabled = true;
    }

}
