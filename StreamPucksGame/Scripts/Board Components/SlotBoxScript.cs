using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBoxScript : MonoBehaviour
{
    private static string OCCUPIED_TRIGGER = "occupied";
    private static string EJECT_TRIGGER = "eject";

    public TextMesh TextDisplay;
    public SpriteRenderer ImageDisplay;
    public float rotationMinDuration = 3f;
    public float rotationMaxDuration = 4f;
    private float rotationDuration;
    public float startingRotationSpeed = 0.1f;
    public float endingRotationSpeed = 1f;

    public float FlushWaitSeconds = 1f;
    public float FlushForce = 100f;

    private float elapsedRotationTime = 0f;
    private float currentRotationSpeed;

    private List<GameObject> trappedPucks;

    private int bonusIdx = -1;
    public PrizeComponent currentPrize { get; private set; }

    private void Start()
    {
        trappedPucks = new List<GameObject>();
        DisplayPrize(GetNextBonus());
    }

    private PrizeComponent GetNextBonus()
    {
        PrizeComponent[] prizes = GetComponents<PrizeComponent>();
        if (prizes.Length <= 0)
        {
            Debug.LogError("Slot box is missing prize components. Please attach prize components to slot box.");
            return null;
        }

        if (prizes.Length <= ++bonusIdx || bonusIdx < 0)
        {
            bonusIdx = 0;
        }

        currentPrize = prizes[bonusIdx];
        return currentPrize;
    }

    public PrizeComponent GetPrize()
    {
        if (currentPrize == null)
        {
            return GetNextBonus();
        }

        return currentPrize;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Constants.PUCK_TAG)
        {
            if (EventList.SlotBoxCapture != null)
            {
                EventList.SlotBoxCapture(transform.parent.gameObject, other.gameObject);
            }

            trappedPucks.Add(other.gameObject);

            GetComponentInParent<Animator>().SetBool(OCCUPIED_TRIGGER, true);
            GetComponentInParent<Animator>().SetBool(EJECT_TRIGGER, false);
            StartRotation();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == Constants.PUCK_TAG)
        {
            if (EventList.SlotBoxRelease != null)
            {
                EventList.SlotBoxRelease(transform.parent.gameObject, other.gameObject);
            }
            GetComponentInParent<Animator>().SetBool(OCCUPIED_TRIGGER, false);
            GetComponentInParent<Animator>().SetBool(EJECT_TRIGGER, false);
        }
    }

    private void StartRotation()
    {
        SetRotationSpeed();
        elapsedRotationTime = 0f;
        currentRotationSpeed = startingRotationSpeed;
        StartCoroutine("Rotate");
    }

    private IEnumerator Rotate()
    {
        while(true)
        {
            if (elapsedRotationTime >= rotationDuration)
            {
                GetComponentInParent<Animator>().SetBool(EJECT_TRIGGER, true);
                yield return new WaitForSeconds(FlushWaitSeconds);
                foreach(GameObject puck in trappedPucks)
                {
                    Rigidbody rb = puck.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddForce(Vector3.down * FlushForce);
                    }
                }
                trappedPucks = new List<GameObject>();
                yield break;
            }
            
            DisplayPrize(GetNextBonus());
            currentRotationSpeed = Mathf.Lerp(startingRotationSpeed, endingRotationSpeed, elapsedRotationTime / rotationDuration);
            elapsedRotationTime += currentRotationSpeed;
            yield return new WaitForSeconds(currentRotationSpeed);
        }
    }

    private void DisplayPrize(PrizeComponent prize)
    {
        if (ImageDisplay != null && prize.optionalImage != null)
        {
            ImageDisplay.sprite = prize.optionalImage;
            TextDisplay.text = "";
        }
        else
        {
            if (ImageDisplay != null)
            {
                ImageDisplay.sprite = null;
            }

            TextDisplay.text = prize.GenerateString();
        }
    }

    private void SetRotationSpeed()
    {
        rotationDuration = Random.Range(rotationMinDuration, rotationMaxDuration);
    }
}
