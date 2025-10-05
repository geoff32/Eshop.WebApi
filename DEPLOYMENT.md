# Configuration du Déploiement CI/CD

Cette pipeline GitHub Actions automatise le build, les tests, la création d'images Docker et le déploiement sur un serveur distant.

## Configuration des Secrets GitHub

Pour que la pipeline fonctionne correctement, vous devez configurer les secrets suivants dans votre repository GitHub (Settings > Secrets and variables > Actions) :

### Secrets requis pour le déploiement SSH :

1. **HOST** : L'adresse IP ou le nom de domaine de votre serveur
   ```
   Exemple: 192.168.1.100 ou myserver.example.com
   ```

2. **USERNAME** : Le nom d'utilisateur pour la connexion SSH
   ```
   Exemple: root ou ubuntu ou deploy
   ```

3. **SSH_PRIVATE_KEY** : La clé privée SSH pour l'authentification
   ```
   Générer une paire de clés SSH :
   ssh-keygen -t rsa -b 4096 -C "github-actions"
   
   Puis copiez le contenu de la clé privée (fichier sans extension .pub)
   ```

4. **PORT** (optionnel) : Le port SSH (par défaut : 22)
   ```
   Exemple: 22 ou 2222
   ```

5. **DATABASE_CONNECTION_STRING** : La chaîne de connexion à la base de données sur le serveur de production
   ```
   Exemple: Host=localhost;Database=eshop;Username=eshop_user;Password=eshop_pass
   ```

### Configuration du serveur de destination

1. **Installer Docker sur le serveur** :
   ```bash
   # Ubuntu/Debian
   curl -fsSL https://get.docker.com -o get-docker.sh
   sudo sh get-docker.sh
   sudo usermod -aG docker $USER
   ```

2. **Configurer l'accès SSH** :
   - Ajoutez la clé publique SSH correspondante à `~/.ssh/authorized_keys` sur le serveur
   - Assurez-vous que l'utilisateur a les permissions Docker

3. **Configurer les environnements GitHub** (optionnel) :
   - Allez dans Settings > Environments
   - Créez un environnement "production"
   - Ajoutez des règles de protection si nécessaire

## Fonctionnement de la Pipeline

### 1. Test Job
- Vérifie le code
- Installe .NET 8.0
- Restaure les dépendances
- Build l'application
- Execute les tests (si configurés)

### 2. Build and Push Job
- Se déclenche uniquement sur les branches `main` et `develop`
- Build l'image Docker multi-architecture (AMD64 et ARM64)
- Publie l'image sur GitHub Container Registry (ghcr.io)
- Génère une attestation de provenance

### 3. Deploy Job
- Se déclenche uniquement sur la branche `main`
- Se connecte au serveur via SSH
- Télécharge la nouvelle image Docker
- Arrête l'ancien conteneur et démarre le nouveau
- Nettoie les ressources inutilisées

## Tags d'Images Docker

La pipeline génère automatiquement plusieurs tags :
- `latest` : Pour la branche main
- `main-<sha>` : Tag avec le commit SHA
- `develop-<sha>` : Pour la branche develop

## Variables d'Environnement

Le conteneur déployé utilise les variables d'environnement suivantes :
- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection` : Configuré via le secret

## Sécurité

- L'image Docker utilise un utilisateur non-root
- Les secrets sont chiffrés dans GitHub
- L'attestation de provenance garantit l'intégrité de l'image
- Cache Docker pour accélérer les builds

## Monitoring

Après déploiement, vous pouvez vérifier l'état avec :
```bash
# Sur le serveur
docker ps
docker logs eshop-api
```