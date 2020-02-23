using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // multiplayer

public class PlayerController : MonoBehaviourPun // multiplayer
{
    GameObject camera;
    float distanceToCamera = 5;

    public float facingAngle = 0;
    Vector2 unitV2;

    public float volume = 1;

    void Start()
    {
        if (photonView.IsMine)
        {
            camera = Instantiate(
                Resources.Load<GameObject>("Camera"),
                gameObject.transform.position,
                gameObject.transform.rotation
            );
        }
        volume = getVolumeOfGameObject(gameObject);
    }

    private float getVolumeOfGameObject(GameObject obj)
    {
        Vector3 size = obj.GetComponent<Renderer>().bounds.size;
        return size.x * size.y * size.z;
    }

    void Update()
    {
        float z = 0;
        float x = 0;
        // controls
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);
            if (touch.position.x < Screen.width / 4) x = -1;
            else if (touch.position.x > Screen.width * 3 / 4) x = 1;
            if (touch.position.y < Screen.height / 2) z = -1;
            else if (touch.position.y > Screen.height / 2) z = 1;
        }
        else
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }
        x *= Time.deltaTime * -100;
        z *= Time.deltaTime * 1000;
        facingAngle += x;
        unitV2 = new Vector2(Mathf.Cos(facingAngle * Mathf.Deg2Rad), Mathf.Sin(facingAngle * Mathf.Deg2Rad));
        gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(unitV2.x, 0, unitV2.y) * z);
    }

    private void LateUpdate()
    {
        camera.transform.position = new Vector3(-unitV2.x * distanceToCamera, distanceToCamera, -unitV2.y * distanceToCamera) + gameObject.transform.position;
        camera.transform.LookAt(gameObject.transform.position + new Vector3(0, 1, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.CompareTag("PickUppable"))
        {
            float pickUppableVolume = getVolumeOfGameObject(collision.gameObject);
            if (pickUppableVolume > volume)
            {

            }
            else
            {
                Debug.Log("Pickup!");
                gameObject.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                volume += pickUppableVolume;
                distanceToCamera = 5 + (0.005f * volume);
                Destroy(collision.gameObject.GetComponent<BoxCollider>());
                collision.gameObject.transform.SetParent(gameObject.transform);
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            PopChild();
            Vector3 dir = collision.contacts[0].point - transform.position;
            GetComponent<Rigidbody>().AddForce(-dir.normalized * collision.gameObject.GetComponent<PlayerController>().volume);
        }
    }

    private void PopChild()
    {
        if (gameObject.transform.childCount == 0) return;
        Transform lastChild = gameObject.transform.GetChild(gameObject.transform.childCount - 1);

        if (lastChild.CompareTag("PickUppable"))
        {
            Debug.Log("pop!");
            gameObject.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            volume -= getVolumeOfGameObject(lastChild.gameObject);
            distanceToCamera = 5 + (0.005f * volume);

            lastChild.SetParent(null);
            lastChild.gameObject.AddComponent<Rigidbody>();
            lastChild.gameObject.AddComponent<BoxCollider>();
            lastChild.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 100, 0) * volume);
        }
    }
}
