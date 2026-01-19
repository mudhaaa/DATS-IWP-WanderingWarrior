using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingTextUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float maxLifetime;
    [SerializeField] private float currLifetime;
    private bool textDone;
    private Vector3 ogPos;

    private void Start()
    {
        ogPos = transform.localPosition;
    }

    public void SetText(string text, float lifetime)
    {
        this.text.text = text;

        currLifetime = maxLifetime = lifetime;

        textDone = false;

        gameObject.SetActive(true);
    }
    
    void EndsText()
    {
        gameObject.SetActive(false);
        textDone = false;
        transform.localPosition = ogPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (currLifetime <= 0) textDone = true;
        else if (currLifetime <= maxLifetime)
        {
            transform.DOLocalMoveY(transform.localPosition.y + 1, maxLifetime);
            currLifetime -= Time.deltaTime;
        }

        if (textDone)
        {
            EndsText();
        }
    }
}
