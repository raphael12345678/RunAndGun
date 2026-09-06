using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Limites de la caméra")]
    public float minX = 0f;
    public float maxX = 1000f;

    // Ajout des limites verticales pour ne pas voir sous la carte ou trop haut
    public float minY = 0f;
    public float maxY = 20f;

    [Header("Confort visuel")]
    // Permet de viser un peu au-dessus de la tête du joueur pour mieux anticiper les sauts
    public float yOffset = 0.5f;

    private void Update()
    {
        // Sécurité pour éviter une erreur si le joueur n'est pas encore chargé
        if (PlayerController.instant == null) return;

        var playerPosition = PlayerController.instant.transform.position;
        var cameraPosition = this.transform.position;

        // Suit le joueur sur l'axe X
        cameraPosition.x = Mathf.Clamp(playerPosition.x, minX, maxX);

        // Suit le joueur sur l'axe Y avec un décalage
        cameraPosition.y = Mathf.Clamp(playerPosition.y + yOffset, minY, maxY);

        transform.position = cameraPosition;
    }
}