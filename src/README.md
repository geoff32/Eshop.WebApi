# E-Shop API (.NET 9)

Cette API expose le catalogue de produits, les classes, et permet de persister les commandes avec authentification OpenID Connect (Google) et JWT.

- Base de données : PostgreSQL
- ORM : Dapper
- Authentification : Google OpenID Connect + JWT

## Configuration

### 1. Configuration Google OAuth

1. Allez sur [Google Cloud Console](https://console.cloud.google.com/)
2. Créez un nouveau projet ou sélectionnez un projet existant
3. Activez l'API "Google+ API" et "People API"
4. Créez des credentials OAuth 2.0 :
   - Type : Web Application
   - Authorized redirect URIs : `https://localhost:5001/api/auth/google/callback`
5. Copiez le Client ID et Client Secret dans `appsettings.Development.json`

### 2. Configuration JWT

Modifiez la clé secrète JWT dans `appsettings.Development.json` avec une valeur sécurisée (minimum 32 caractères).

## Endpoints

### Authentification
- `GET /api/auth/google` : Connexion via Google
- `GET /api/auth/google/callback` : Callback Google (retourne un JWT)
- `POST /api/auth/token` : Génération d'un token JWT (pour test)
- `GET /api/auth/profile` : Profil utilisateur (nécessite authentification)

### API Sécurisées (nécessitent un token JWT)
- `GET /api/products` : Liste des produits
- `GET /api/classes` : Liste des classes  
- `POST /api/orders` : Création d'une commande

## Utilisation

### 1. Obtenir un token JWT

**Option A : Via Google OAuth**
1. Naviguez vers `https://localhost:5001/api/auth/google`
2. Connectez-vous avec votre compte Google
3. Récupérez le token JWT retourné

**Option B : Via l'endpoint de test**
```bash
curl -X POST https://localhost:5001/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "test"}'
```

### 2. Utiliser le token pour accéder aux API

```bash
curl -X GET https://localhost:5001/api/products \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 3. Tester avec Swagger

1. Lancez l'API
2. Allez sur `https://localhost:5001/swagger`
3. Cliquez sur "Authorize"
4. Entrez `Bearer YOUR_JWT_TOKEN`
5. Testez les endpoints

## Initialisation de la base de données

Voir le script `init_db.sql` pour créer les tables nécessaires dans PostgreSQL.

## Structure de sécurité

- **Authentification** : OpenID Connect avec Google
- **Autorisation** : JWT Bearer tokens
- **Protection** : Tous les endpoints API nécessitent une authentification
- **Swagger** : Interface de test avec support JWT intégré
