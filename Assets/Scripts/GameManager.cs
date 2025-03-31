using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text totalTitleText;
    public TMP_Text totalScoreText;
    public TMP_Text sliderText;
    public TMP_Text clickText; 
    public Image fadeImage;
    public Slider progressSlider;

    public float hourDuration = 90f;
    [SerializeField] private int currentHour = 9;
    private bool isFlipping = false;

    private Vector3 originalTextPosition;
    private Vector3 centerPosition;
    private float fadeDuration = 1.5f;

    private int scoreToPromotion = 1500;

    private bool canClick = false;
    private Player player;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        originalTextPosition = timeText.rectTransform.anchoredPosition;
        centerPosition = Vector3.zero;

        totalTitleText.gameObject.SetActive(false);
        totalScoreText.gameObject.SetActive(false);
        clickText.gameObject.SetActive(false);
        progressSlider.value = 0;

        UpdateTimeText();
        StartCoroutine(TimerCoroutine());
    }

    private void Update()
    {
        if (canClick)
        {
            OnClick();
        }
    }

    IEnumerator TimerCoroutine()
    {
        while (currentHour < 17)
        {
            yield return new WaitForSeconds(hourDuration);
            currentHour++;
            StartCoroutine(FlipTextEffect());
        }
        player.enabled = false;
        yield return StartCoroutine(FadeToBlack());
        yield return StartCoroutine(MoveAndEnlargeText());

        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FlipToShiftOver());

        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(ShowTotalScore());

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveScoreToSlider());

        yield return StartCoroutine(FillSlider());

        yield return new WaitForSeconds(1f);
        StartCoroutine(ShowClickText()); 
    }

    IEnumerator FlipTextEffect()
    {
        if (isFlipping) yield break;
        isFlipping = true;

        float flipDuration = 0.3f;
        float elapsed = 0f;
        Vector3 originalScale = timeText.transform.localScale;

        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1, 0, elapsed / (flipDuration / 2));
            timeText.transform.localScale = new Vector3(originalScale.x, scale, originalScale.z);
            yield return null;
        }

        UpdateTimeText();

        elapsed = 0f;
        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(0, 1, elapsed / (flipDuration / 2));
            timeText.transform.localScale = new Vector3(originalScale.x, scale, originalScale.z);
            yield return null;
        }

        isFlipping = false;
    }

    void UpdateTimeText()
    {
        timeText.text = currentHour + " AM";
        if (currentHour == 12) timeText.text = "12 PM";
        else if (currentHour > 12) timeText.text = (currentHour - 12) + " PM";
    }

    IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        Color fadeColor = fadeImage.color;
        fadeColor.a = 0f;
        fadeImage.gameObject.SetActive(true);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = fadeColor;
            yield return null;
        }
    }

    IEnumerator MoveAndEnlargeText()
    {
        AudioManager.Instance.PlaySoundOnMainSource(AudioManager.Instance.timerSound);
        float moveDuration = 1f;
        float elapsed = 0f;
        Vector3 startSize = timeText.transform.localScale;
        Vector3 targetSize = startSize * 2f;
        Vector3 startPosition = timeText.rectTransform.anchoredPosition;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            timeText.rectTransform.anchoredPosition = Vector3.Lerp(startPosition, centerPosition, elapsed / moveDuration);
            timeText.transform.localScale = Vector3.Lerp(startSize, targetSize, elapsed / moveDuration);
            yield return null;
        }
    }

    IEnumerator FlipToShiftOver()
    {
        if (isFlipping) yield break;
        isFlipping = true;

        float flipDuration = 0.3f;
        float elapsed = 0f;
        Vector3 originalScale = timeText.transform.localScale;

        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1, 0, elapsed / (flipDuration / 2));
            timeText.transform.localScale = new Vector3(originalScale.x, scale, originalScale.z);
            yield return null;
        }

        timeText.text = "Shift Over!";
        
        elapsed = 0f;
        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(0, 1, elapsed / (flipDuration / 2));
            timeText.transform.localScale = new Vector3(originalScale.x, scale, originalScale.z);
            yield return null;
        }

        isFlipping = false;
    }

    IEnumerator ShowTotalScore()
    {
        timeText.gameObject.SetActive(false);
        totalTitleText.gameObject.SetActive(true);
        totalScoreText.gameObject.SetActive(true);
        progressSlider.gameObject.SetActive(true);
        sliderText.gameObject.SetActive(true);

        totalTitleText.text = "Total Points Earned:";
        totalScoreText.text = "0";

        int totalPoints = TaskManager.Instance.GetTotalPoints();
        Debug.Log("Total points: " + totalPoints);

        int currentScore = 0;
        int incrementAmount = Mathf.Max(1, totalPoints / 50); // Ensures at least 1 increment per frame
        int soundTriggerFrequency = Mathf.Max(1, totalPoints / 20); // Play sound every 1/10th of total points
        float delay = 1.5f / (totalPoints / (float)incrementAmount); // Adjusts speed based on totalPoints
        int lastSoundPlayedAt = 0;

        while (currentScore < totalPoints)
        {
            currentScore += incrementAmount;

            if (currentScore > totalPoints)
            {
                currentScore = totalPoints;
            }

            // Play sound only every `soundTriggerFrequency` points
            if (currentScore - lastSoundPlayedAt >= soundTriggerFrequency)
            {
                float pitch = Mathf.Lerp(1.0f, 1.5f, (float)currentScore / totalPoints); // Scale pitch from 1.0 to 1.5
                AudioManager.Instance.mainCameraSource.pitch = pitch;
                AudioManager.Instance.PlaySoundOnMainSource(AudioManager.Instance.counterUp);
                lastSoundPlayedAt = currentScore;
            }

            totalScoreText.text = currentScore.ToString();
            yield return new WaitForSeconds(0.02f); // Controls the speed of counting
        }

        totalScoreText.text = totalPoints.ToString();
        AudioManager.Instance.PlaySoundOnMainSource(AudioManager.Instance.smash);

        // Reset pitch to default
        AudioManager.Instance.mainCameraSource.pitch = 1.0f;

        // Change text color to green
        totalScoreText.color = Color.green;

        // Impact effect (scaling up and down)
        float impactDuration = 0.4f;
        float elapsed = 0f;
        Vector3 originalScale = totalScoreText.transform.localScale;
        Vector3 enlargedScale = originalScale * 1.3f;

        while (elapsed < impactDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.PingPong(elapsed * 4f, 1f); // Creates a quick scale in and out effect
            totalScoreText.transform.localScale = Vector3.Lerp(originalScale, enlargedScale, t);
            yield return null;
        }

        totalScoreText.transform.localScale = originalScale; // Reset scale after effect
    }



    IEnumerator MoveScoreToSlider()
    {
        float moveDuration = 0.5f;
        float shrinkDuration = 0.3f;
        float elapsed = 0f;
        Vector3 originalPos = totalScoreText.rectTransform.anchoredPosition;
        Vector3 targetPos = progressSlider.GetComponent<RectTransform>().anchoredPosition;
        Vector3 originalScale = totalScoreText.transform.localScale;
        Vector3 targetScale = originalScale * 0.5f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            totalScoreText.rectTransform.anchoredPosition = Vector3.Lerp(originalPos, targetPos, elapsed / moveDuration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            totalScoreText.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / shrinkDuration);
            yield return null;
        }

        totalScoreText.gameObject.SetActive(false);
    }

    IEnumerator ShowClickText()
    {
        clickText.gameObject.SetActive(true);
        canClick = true;

        float pulseDuration = 0.8f;
        float minScale = 0.9f;
        float maxScale = 1.1f;
        float minAlpha = 0.5f;
        float maxAlpha = 1.0f;
        Color textColor = clickText.color;

        while (canClick)
        {
            float elapsed = 0f;

            // Expand
            while (elapsed < pulseDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (pulseDuration / 2);
                clickText.transform.localScale = Vector3.Lerp(Vector3.one * minScale, Vector3.one * maxScale, t);
                textColor.a = Mathf.Lerp(minAlpha, maxAlpha, t);
                clickText.color = textColor;
                yield return null;
            }

            elapsed = 0f;

            // Shrink
            while (elapsed < pulseDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (pulseDuration / 2);
                clickText.transform.localScale = Vector3.Lerp(Vector3.one * maxScale, Vector3.one * minScale, t);
                textColor.a = Mathf.Lerp(maxAlpha, minAlpha, t);
                clickText.color = textColor;
                yield return null;
            }
        }
    }

    IEnumerator FillSlider()
    {
        float totalPoints = TaskManager.Instance.GetTotalPoints();
        float targetValue = Mathf.Clamp(totalPoints / scoreToPromotion, 0f, 1f);
        float fillDuration = 1f;
        float elapsed = 0f;
        float startValue = progressSlider.value;

        while (elapsed < fillDuration)
        {
            elapsed += Time.deltaTime;
            progressSlider.value = Mathf.Lerp(startValue, targetValue, elapsed / fillDuration);
            yield return null;
        }

        progressSlider.value = targetValue;
    }

    public void OnClick()
    {

        if (player.PressAnyKey())
        {
            canClick = false;
            clickText.gameObject.SetActive(false);
            SceneManager.LoadScene(0);
        }
        
        
    }
}
