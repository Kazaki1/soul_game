using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FastTravelPoint : MonoBehaviour
{
    public Transform destination;

    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public Text promptTextLegacy;

    public float fadeOutDuration = 1f;
    public float waitDuration = 0.5f;
    public float fadeInDuration = 1f;
    public bool disablePlayerDuringFade = true;

    public GameObject teleportEffect;
    public Color gizmoColor = Color.cyan;

    private Transform player;
    private bool playerInRange = false;
    private bool isTeleporting = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure Player has 'Player' tag.");
        }

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }

        if (destination == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Destination not set!");
        }
    }

    private void Update()
    {
        if (player == null || isTeleporting) return;

        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRange;

        if (promptUI != null)
        {
            promptUI.SetActive(playerInRange);
        }

        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            StartCoroutine(FastTravelWithFade());
        }
    }

    private IEnumerator FastTravelWithFade()
    {
        if (destination == null)
        {
            Debug.LogWarning("Destination not set!");
            yield break;
        }

        if (player == null) yield break;

        isTeleporting = true;

        PlayerController playerController = null;
        if (disablePlayerDuringFade)
        {
            playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }

        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, player.position, Quaternion.identity);
        }

        yield return SceneFadeManager.FadeOut(fadeOutDuration);

        yield return new WaitForSeconds(waitDuration);

        player.position = destination.position;
        Debug.Log($"?? Fast traveled to {destination.name}");

        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, destination.position, Quaternion.identity);
        }

        yield return SceneFadeManager.FadeIn(fadeInDuration);

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        isTeleporting = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            playerInRange = true;

            if (promptUI != null && !isTeleporting)
            {
                promptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;

            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        if (destination != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, destination.position);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(destination.position, 0.5f);
            Gizmos.DrawLine(destination.position, destination.position + Vector3.up * 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.3f);

        if (destination != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(destination.position, 0.3f);
        }
    }
}   