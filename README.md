Rapport de Projet : Run & Gun 2D


Ce document synthétise les fonctionnalités développées dans le cadre du projet Unity 2D "Run & Gun", un mini-jeu d'action et de plateforme inspiré de Metal Slug. Le projet respecte l'organisation exigée avec des dossiers dédiés pour les Scripts, Scènes, Animations, Prefabs et Sprites. 
1. Contrôles du Joueur
•	Déplacement : Touches Q (Gauche) et D (Droite).
•	Saut : Touche Espace.
•	Action spéciale : Glissade (Left Shift).
•	Tir : Touche J.
•	Direction du tir : Touches Z, Q, S, D pour orienter le projectile.

2. Mécaniques et Fonctionnalités du Joueur (Player)
•	Mouvements fluides : Le joueur peut se déplacer, sauter, effectuer des glissades et traverser certaines plateformes par en dessous (One-Way Platforms) pour dynamiser l'exploration.
•	Système d'Animation : Un Animator Controller gère les différents états visuels du personnage en temps réel : Idle, Running, Jumping, Sliding  et Dead. 
•	Tir Multidirectionnel : Conformément aux attentes, le joueur peut tirer dans 8 directions différentes. Chaque balle est instanciée à partir d'un Prefab dédié via le script BulletBehavior, gérant sa propre physique et ses collisions. 
•	Interface (UI) : Le joueur et les ennemis dispose d'une barre de vie visuelle (Canvas Slider) qui l'accompagne et se met à jour lorsqu'il subit des dégâts. 
•	Caméra : Un script CameraController est paramétré pour suivre le joueur dans ses déplacements horizontaux et verticaux. 

3. Intelligence Artificielle et Ennemis Le jeu intègre plusieurs types d'ennemis capables de détecter le joueur via des zones de détection (Triggers) et de subir des dégâts physiques. Tous les ennemis sont équipés d'animations et d'une barre de vie visuelle flottante (World Space Canvas). 
•	Ennemi de mêlée (EnemyMeleeBehavior) : Détecte la présence du joueur et s'approche pour déclencher une attaque au corps-à-corps. 
•	Ennemi à distance (EnemyOneBehavior) : Ennemi statique qui détecte la cible et génère ses propres projectiles pour attaquer le joueur dans toutes les directions. 

4. Level Design et Progression
•	Structure des niveaux : Le projet contient deux scènes jouables distinctes (Level1 et Level2), chacune proposant sa propre disposition de plateformes et d'ennemis. 
•	Événements de Gameplay : Une porte de sortie (équipée d'un Trigger physique) est placée à la fin du premier niveau (LevelExit) pour permettre de charger automatiquement la scène du niveau suivant. 

5. Bonus et Ajouts Spécifiques
•	Intégration d'un système de santé avancé avec des barres de vie dynamiques (changement de couleur) au-dessus de chaque entité ennemie.
•	Ajout de la mécanique de "Slide" pour esquiver ou passer sous des obstacles.
•	Gestion des plateformes traversables depuis le bas.
https://github.com/raphael12345678/RunAndGun 

