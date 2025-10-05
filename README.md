# E-Shop API

[![CI/CD Pipeline](https://github.com/geoff32/eshop.webapi/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/geoff32/eshop.webapi/actions/workflows/ci-cd.yml)

API REST pour une boutique en ligne développée avec .NET 8, utilisant PostgreSQL comme base de données et authentification Google OAuth + JWT.

## 🚀 Fonctionnalités

- **API RESTful** complète pour e-commerce
- **Authentification** Google OAuth + JWT
- **Base de données** PostgreSQL avec Dapper ORM
- **Documentation** Swagger/OpenAPI
- **Containerisation** Docker
- **CI/CD** GitHub Actions
- **Déploiement** automatique sur serveur distant

## 📋 Prérequis

- .NET 8.0 SDK
- PostgreSQL 15+
- Docker (pour le déploiement)
- Compte Google Cloud (pour OAuth)

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

### 3. Configuration de l'application
```bash
# Copier et modifier la configuration
cp src/appsettings.json src/appsettings.Development.json
# Modifier les paramètres de connexion DB et Google OAuth
```

### 4. Lancer l'application
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
- `GET /api/auth/google` - Connexion Google OAuth
- `GET /api/auth/profile` - Profil utilisateur
- `POST /api/auth/token` - Token JWT (test)

#### Produits & Commandes
- `GET /api/products` - Liste des produits
- `GET /api/classes` - Liste des classes
- `POST /api/orders` - Créer une commande

Tous les endpoints nécessitent une authentification JWT (sauf auth).

### Authentification
```bash
# 1. Obtenir un token
curl -X POST https://localhost:5001/api/auth/token \\
  -H "Content-Type: application/json" \\
  -d '{"email": "test@example.com"}'

# 2. Utiliser le token
curl -X GET https://localhost:5001/api/products \\
  -H "Authorization: Bearer YOUR_TOKEN"
```

## 📁 Structure du Projet

```
├── .github/workflows/     # Pipeline CI/CD
├── src/                   # Code source .NET
│   ├── Controllers/       # API Controllers
│   ├── Models/           # Modèles de données
│   └── ...
├── Dockerfile            # Image Docker
├── docker-compose.yml    # Développement
├── docker-compose.prod.yml # Production  
├── deploy.sh            # Script déploiement manuel
└── README.md           # Cette documentation
```

## 🔧 Scripts Utiles

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