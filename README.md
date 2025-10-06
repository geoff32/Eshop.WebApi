# E-Shop API

[![CI/CD Pipeline](https://github.com/geoff32/eshop.webapi/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/geoff32/eshop.webapi/actions/workflows/ci-cd.yml)

API REST pour une boutique en ligne développée avec .NET 8, utilisant PostgreSQL comme base de données et authentification par cookies HTTP sécurisés.

## 🚀 Fonctionnalités

- **API RESTful** complète pour e-commerce
- **Authentification** personnalisée par cookies HTTP sécurisés
- **Base de données** PostgreSQL avec Dapper ORM
- **Sécurité** Hachage PBKDF2 des mots de passe
- **Documentation** Swagger/OpenAPI
- **Containerisation** Docker
- **CI/CD** GitHub Actions
- **Déploiement** automatique sur serveur distant

## 📋 Prérequis

- .NET 8.0 SDK
- PostgreSQL 15+
- Docker (pour le déploiement)

## 🛠️ Installation et Développement

### 1. Cloner le repository
```bash
git clone https://github.com/geoff32/eshop.webapi.git
cd eshop.webapi
```

### 2. Configuration de la base de données
```bash
# Démarrer PostgreSQL avec Docker Compose
docker-compose up -d postgres
```

### 3. Initialiser la base de données
```bash
# Exécuter le script d'initialisation
psql -d eshop -f src/init_db.sql
```

### 4. Configuration de l'application
```bash
# Copier et modifier la configuration si nécessaire
cp src/appsettings.json src/appsettings.Development.json
# Modifier les paramètres de connexion DB et JWT si besoin
```

### 5. Lancer l'application
```bash
cd src
dotnet restore
dotnet run
```

L'API sera accessible sur `https://localhost:5001` et la documentation Swagger sur `https://localhost:5001/swagger`.

## 🐳 Docker

### Build local
```bash
docker build -t eshop-api .
docker run -p 8080:8080 eshop-api
```

### Utiliser docker-compose
```bash
# Développement
docker-compose up

# Production
docker-compose -f docker-compose.prod.yml up -d
```

## 🔄 CI/CD Pipeline

La pipeline GitHub Actions automatise :

### ✅ Tests et Build
- Vérification du code
- Build de l'application .NET
- Tests unitaires (si configurés)

### 📦 Docker Registry
- Build d'images multi-architecture (AMD64/ARM64)
- Publication sur GitHub Container Registry
- Tagging automatique (latest, branch, SHA)
- Attestation de provenance pour la sécurité

### 🚀 Déploiement Automatique
- Déploiement automatique sur branche `main`
- Connexion SSH sécurisée au serveur
- Mise à jour zero-downtime
- Nettoyage des ressources

## ⚙️ Configuration du Déploiement

### Secrets GitHub requis
Configurez ces secrets dans GitHub Settings > Secrets :

| Secret | Description | Exemple |
|--------|-------------|---------|
| `HOST` | Adresse du serveur | `192.168.1.100` |
| `USERNAME` | Utilisateur SSH | `ubuntu` |
| `SSH_PRIVATE_KEY` | Clé privée SSH | `-----BEGIN RSA PRIVATE KEY-----...` |
| `PORT` | Port SSH (optionnel) | `22` |
| `DATABASE_CONNECTION_STRING` | Connexion DB production | `Host=localhost;Database=eshop;...` |

### Configuration du serveur
```bash
# Installer Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Configurer l'utilisateur
sudo usermod -aG docker $USER

# Ajouter la clé publique SSH
echo "ssh-rsa YOUR_PUBLIC_KEY" >> ~/.ssh/authorized_keys
```

Voir [DEPLOYMENT.md](DEPLOYMENT.md) pour la documentation complète.

## 📚 Documentation API

### Endpoints principaux

#### Authentification
- `POST /api/auth/register` - Création de compte (définit le cookie)
- `POST /api/auth/login` - Connexion utilisateur (définit le cookie)
- `POST /api/auth/logout` - Déconnexion (supprime le cookie)
- `GET /api/auth/profile` - Profil utilisateur (protégé par cookie)

#### Produits & Commandes
- `GET /api/products` - Liste des produits
- `GET /api/classes` - Liste des classes
- `POST /api/orders` - Créer une commande

Tous les endpoints nécessitent une authentification JWT (sauf register/login).

### Système d'Authentification
L'API utilise un système d'authentification par cookies sécurisés :
- **Inscription** : Email, prénom, nom, mot de passe
- **Connexion** : Email et mot de passe
- **Sécurité** : Mots de passe hachés avec PBKDF2-SHA256
- **Cookies** : HttpOnly, Secure, SameSite=Strict, expiration 24h
- **Validation** : Format email, mot de passe minimum 6 caractères
- **Protection** : Anti-XSS, Anti-CSRF, gestion automatique

Voir [AUTH_README.md](AUTH_README.md) pour la documentation complète de l'authentification.

### Authentification
```bash
# 1. Créer un compte (sauvegarde le cookie)
curl -c cookies.txt -X POST https://localhost:5001/api/auth/register \\
  -H "Content-Type: application/json" \\
  -d '{
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "password123"
  }'

# 2. Se connecter (sauvegarde le cookie)
curl -c cookies.txt -X POST https://localhost:5001/api/auth/login \\
  -H "Content-Type: application/json" \\
  -d '{
    "email": "user@example.com",
    "password": "password123"
  }'

# 3. Utiliser le cookie pour accéder aux ressources protégées
curl -b cookies.txt -X GET https://localhost:5001/api/products

# 4. Se déconnecter
curl -b cookies.txt -X POST https://localhost:5001/api/auth/logout
```

## 📁 Structure du Projet

```
├── .github/workflows/     # Pipeline CI/CD
├── src/                   # Code source .NET
│   ├── Controllers/       # API Controllers
│   ├── Models/           # Modèles de données
│   ├── Services/         # Services (Auth, Password, User)
│   ├── init_db.sql       # Script initialisation DB
│   └── ...
├── Dockerfile            # Image Docker
├── docker-compose.yml    # Développement
├── docker-compose.prod.yml # Production
├── deploy.sh            # Script déploiement manuel
├── test_auth.sh         # Script de test authentification
├── AUTH_README.md       # Documentation authentification
└── README.md           # Cette documentation
```

## 🔧 Scripts Utiles

### Base de données
```bash
# Initialisation complète (nouvelle installation)
psql -d eshop -f src/init_db.sql

# Migration (base existante)
psql -d eshop -f src/add_users_table.sql
```

### Test de l'authentification
```bash
# Rendre le script exécutable
chmod +x test_auth.sh

# Lancer les tests d'API
./test_auth.sh
```

### Déploiement manuel
```bash
# Déploiement local
./deploy.sh latest

# Déploiement sur serveur distant  
./deploy.sh latest user@server.com
```

### Monitoring
```bash
# Logs du conteneur
docker logs eshop-api -f

# Statut
docker ps | grep eshop-api
```

## 🤝 Contribution

1. Fork le projet
2. Créer une branche feature (`git checkout -b feature/AmazingFeature`)
3. Commit les changes (`git commit -m 'Add some AmazingFeature'`)
4. Push vers la branche (`git push origin feature/AmazingFeature`)
5. Créer une Pull Request

## 📄 License

Ce projet est sous licence MIT. Voir le fichier `LICENSE` pour plus de détails.

## 📞 Support

Pour toute question ou problème :
- 🐛 [Issues GitHub](https://github.com/geoff32/eshop.webapi/issues)
- 📚 [Documentation complète](./DEPLOYMENT.md)
- 🚀 [Actions GitHub](https://github.com/geoff32/eshop.webapi/actions)