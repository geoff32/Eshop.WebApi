# API d'Authentification EShop - Cookies

## Vue d'ensemble

L'API d'authentification utilise désormais un système d'authentification personnalisé basé sur les cookies HTTP sécurisés, remplaçant complètement les JWT et Google OAuth.

## Changements apportés

### ✅ Supprimé
- Authentification Google OAuth
- Authentification JWT Bearer
- Dépendances `Microsoft.AspNetCore.Authentication.Google`, `Microsoft.AspNetCore.Authentication.OpenIdConnect` et `Microsoft.AspNetCore.Authentication.JwtBearer`
- Endpoints `/api/auth/google`, `/api/auth/google/callback` et `/api/auth/token`
- Configuration JWT dans appsettings.json

### ✅ Ajouté
- Authentification par cookies HTTP sécurisés
- Endpoint `/api/auth/logout` pour la déconnexion
- Système d'authentification personnalisé
- Table `users` dans la base de données
- Hachage sécurisé des mots de passe (PBKDF2 avec SHA-256)
- Validation des données d'entrée
- Gestion d'erreurs améliorée

## Endpoints de l'API

### 1. Créer un compte
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "password": "motdepasse123"
}
```

**Réponse (200) + Cookie d'authentification:**
```json
{
  "message": "Compte créé et connecté avec succès",
  "user": {
    "id": 1,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "createdAt": "2023-10-06T10:00:00Z"
  }
}
```

### 2. Se connecter
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "motdepasse123"
}
```

**Réponse (200) + Cookie d'authentification:**
```json
{
  "message": "Connexion réussie",
  "user": {
    "id": 1,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "createdAt": "2023-10-06T10:00:00Z"
  }
}
```

### 3. Se déconnecter
```http
POST /api/auth/logout
```

**Réponse (200):**
```json
{
  "message": "Déconnexion réussie"
}
```

### 4. Obtenir le profil utilisateur
```http
GET /api/auth/profile
```

**Réponse (200):**
```json
{
  "id": 1,
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "createdAt": "2023-10-06T10:00:00Z"
}
```

**Note:** Tous les endpoints protégés utilisent automatiquement le cookie d'authentification.

## Validation des données

### Création de compte
- **Email:** Format email valide, unique dans la base de données
- **Prénom et nom:** Obligatoires, non vides
- **Mot de passe:** Minimum 6 caractères

### Connexion
- **Email et mot de passe:** Obligatoires, non vides

## Sécurité

### Hachage des mots de passe
- Utilise PBKDF2 avec SHA-256
- 10 000 itérations
- Sel de 16 bytes généré aléatoirement
- Hash de 32 bytes

### Cookies d'authentification
- **Expiration:** 24 heures avec renouvellement automatique (sliding)
- **HttpOnly:** Oui (protection contre XSS)
- **Secure:** Oui (HTTPS uniquement)
- **SameSite:** Strict (protection CSRF)
- **Nom du cookie:** `EShop.Auth`
- **Claims inclus:** `sub` (user ID), `email`, `firstName`, `lastName`, etc.

### Protection supplémentaire
- Pas de tokens exposés côté client
- Gestion automatique par le navigateur
- Protection contre les attaques XSS et CSRF
- Réponses API au lieu de redirections pour les erreurs d'auth

## Configuration de la base de données

### Migration existante
Si vous avez déjà une base de données, exécutez le script de migration:

```bash
psql -d eshop -f src/add_users_table.sql
```

### Nouvelle installation
Pour une nouvelle installation, utilisez le script complet:

```bash
psql -d eshop -f src/init_db.sql
```

## Configuration

Assurez-vous que votre `appsettings.json` contient:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=eshop;Username=eshop_user;Password=eshop_pass"
  }
}
```

**Note:** Aucune configuration d'authentification n'est nécessaire dans les fichiers de configuration. Les cookies sont configurés directement dans le code.

## Test avec Swagger

1. Démarrez l'application: `dotnet run`
2. Accédez à `https://localhost:5001/swagger`
3. Créez un compte avec `/api/auth/register`
4. Le cookie d'authentification est automatiquement défini
5. Testez `/api/auth/profile` pour vérifier l'authentification
6. Utilisez `/api/auth/logout` pour vous déconnecter

**Note:** Avec les cookies, il n'y a pas besoin de gérer manuellement l'autorisation dans Swagger.

## Codes d'erreur

- **400 Bad Request:** Données invalides ou manquantes
- **401 Unauthorized:** Cookie d'authentification manquant, invalide ou expiré
- **403 Forbidden:** Accès refusé
- **404 Not Found:** Utilisateur non trouvé
- **500 Internal Server Error:** Erreur serveur

## Gestion des cookies côté client

### JavaScript/Fetch
```javascript
// Les cookies sont automatiquement inclus avec credentials: 'include'
fetch('/api/auth/profile', {
  method: 'GET',
  credentials: 'include'
})
```

### cURL
```bash
# Sauvegarder les cookies lors de l'inscription/connexion
curl -c cookies.txt -X POST /api/auth/login ...

# Utiliser les cookies pour les requêtes suivantes
curl -b cookies.txt -X GET /api/auth/profile
```

## Structure des services

### Services ajoutés
- `IPasswordService` / `PasswordService`: Gestion du hachage des mots de passe
- `IUserService` / `UserService`: Gestion des opérations utilisateur (CRUD)

Ces services sont injectés automatiquement via le système DI d'ASP.NET Core.