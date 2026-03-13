using System.Collections;
using UnityEngine;
using TMPro;

public class NPC : Interactable
{
    [Header("Dialogue")]
    [TextArea(2, 6)]
    public string[] dialogueLines;
    public float typeSpeed = 0.04f;

    [Header("Dialogue UI")]
    public GameObject dialogueBubble;
    public TMP_Text dialogueText;

    [Header("Ability Drop")]
    public GameObject abilityItemPrefab;
    public Vector2 dropOffset = new Vector2(0.8f, 0.5f);

    private bool talking;
    private bool itemDropped;
    private bool lineFinished;
    private int currentLine;
    private Coroutine typeRoutine;

    protected override void OnInteract()
    {
        if (!talking)
        {
            StartDialogue();
            return;
        }

        if (!lineFinished) // skip typewriter
        {
            StopCoroutine(typeRoutine);
            dialogueText.text = dialogueLines[currentLine];
            lineFinished = true;
            return;
        }

        currentLine++;
        if (currentLine < dialogueLines.Length)
            typeRoutine = StartCoroutine(TypeLine(dialogueLines[currentLine]));
        else
            EndDialogue();
    }

    void StartDialogue()
    {
        talking = true;
        currentLine = 0;
        if (dialogueBubble) dialogueBubble.SetActive(true);
        typeRoutine = StartCoroutine(TypeLine(dialogueLines[0]));
    }
    void EndDialogue()
    {
        talking = false;
        if (dialogueBubble) dialogueBubble.SetActive(false);
        if (promptObject) promptObject.SetActive(false); // add this

        if (!itemDropped && abilityItemPrefab != null)
        {
            itemDropped = true;
            Instantiate(abilityItemPrefab, (Vector2)transform.position + dropOffset, Quaternion.identity);
        }
    }

    IEnumerator TypeLine(string line)
    {
        lineFinished = false;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        lineFinished = true;
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (!other.CompareTag("Player") || !talking) return;

        talking = false;
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        if (dialogueBubble) dialogueBubble.SetActive(false);
    }
}