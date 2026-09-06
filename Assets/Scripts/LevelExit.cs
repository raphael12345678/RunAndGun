using UnityEngine;
using UnityEngine.SceneManagement; // Indispensable pour charger des scènes

public class LevelExit : MonoBehaviour
{
    [Header("Configuration")]
    public string nextSceneName = "Level2"; // Le nom exact de ta prochaine scène

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Vérifie si c'est bien le joueur qui touche la porte
        if (collision.CompareTag("Player"))
        {
            // 2. Cherche tous les objets avec le Tag "Enemy" dans la scène
            GameObject[] remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            // 3. Vérifie s'il n'y en a plus aucun (tableau vide)
            if (remainingEnemies.Length == 0)
            {
                Debug.Log("Tous les ennemis sont vaincus ! Chargement du niveau suivant...");
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                // Optionnel : Affiche un message dans la console pour le joueur
                Debug.Log("Accès refusé. Il reste " + remainingEnemies.Length + " ennemi(s) à éliminer !");
            }
        }
    }
}