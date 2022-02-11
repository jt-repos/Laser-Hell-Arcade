using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    //Color colorStart = Color.red;
    //Color colorEnd = Color.green;
    //float duration = 1.0f;
    [SerializeField] float slowScrollSpeed = 0.02f;
    [SerializeField] float fastScrollSpeed = 0.08f;
    [SerializeField] float transitionTime = 1f;
    [SerializeField] int numberOfTransitions = 60;
    float currentScrollSpeed;
    Material myMaterial;
    Vector2 offset;

	// Use this for initialization
	void Start ()
    {
        currentScrollSpeed = slowScrollSpeed;
        myMaterial = GetComponent<Renderer>().material;
    }

    private void Awake()
    {
        SetUpSingleton();
    }

    private void SetUpSingleton()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        myMaterial.mainTextureOffset += offset * Time.deltaTime;
        offset = new Vector2(0f, currentScrollSpeed);
        //float lerp = Mathf.PingPong(Time.time, duration) / duration;
        //myMaterial.color = Color.Lerp(colorStart, colorEnd, lerp);
    }

    public void SlowDownScorlling()
    {
        StartCoroutine(ChangeSpeed(slowScrollSpeed));
    }

    public void SpeedUpScorlling()
    {
        //myMaterial.SetColor("_Color", Random.ColorHSV()); //tu musi udemy na rejon wjechac
        StartCoroutine(ChangeSpeed(fastScrollSpeed));
    }

    private IEnumerator ChangeSpeed(float newScrollSpeed)
    {
        float interval = transitionTime / numberOfTransitions - Time.deltaTime;
        float speedIncrease = (newScrollSpeed - currentScrollSpeed) / numberOfTransitions;
        for (int i = 0; i <= numberOfTransitions; i++)
        {
            currentScrollSpeed = currentScrollSpeed + speedIncrease;
            yield return new WaitForSeconds(interval);
        }
    }
}
