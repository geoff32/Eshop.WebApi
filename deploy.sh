#!/bin/bash

# Script de déploiement manuel pour l'API EShop
# Usage: ./deploy.sh [tag] [server]

set -e

# Configuration par défaut
DEFAULT_TAG="latest"
DEFAULT_REGISTRY="ghcr.io"
IMAGE_NAME="geoff32/eshop.webapi"
CONTAINER_NAME="eshop-api"

# Paramètres
TAG=${1:-$DEFAULT_TAG}
SERVER=${2}
FULL_IMAGE_NAME="$DEFAULT_REGISTRY/$IMAGE_NAME:$TAG"

echo "🚀 Déploiement de l'API EShop"
echo "📦 Image: $FULL_IMAGE_NAME"

# Vérifier si un serveur distant est spécifié
if [ -n "$SERVER" ]; then
    echo "🌐 Serveur distant: $SERVER"
    
    # Déploiement sur serveur distant
    ssh "$SERVER" << EOF
        echo "🔄 Mise à jour de l'image Docker..."
        
        # Se connecter au registry GitHub (nécessite un token configuré)
        docker login ghcr.io
        
        # Arrêter l'ancien conteneur
        docker stop $CONTAINER_NAME || true
        docker rm $CONTAINER_NAME || true
        
        # Supprimer l'ancienne image
        docker rmi $FULL_IMAGE_NAME || true
        
        # Télécharger la nouvelle image
        docker pull $FULL_IMAGE_NAME
        
        # Démarrer le nouveau conteneur
        docker run -d \\
            --name $CONTAINER_NAME \\
            --restart unless-stopped \\
            -p 8090:8080 \\
            -e ASPNETCORE_ENVIRONMENT=Production \\
            --env-file .env \\
            $FULL_IMAGE_NAME
        
        echo "✅ Déploiement terminé avec succès!"
        echo "📊 Statut du conteneur:"
        docker ps | grep $CONTAINER_NAME || echo "❌ Conteneur non trouvé"
EOF
else
    echo "🏠 Déploiement local"
    
    # Vérifier si l'image existe localement
    if ! docker image inspect "$FULL_IMAGE_NAME" > /dev/null 2>&1; then
        echo "📥 Téléchargement de l'image..."
        docker pull "$FULL_IMAGE_NAME"
    fi
    
    # Arrêter l'ancien conteneur
    echo "⏹️ Arrêt de l'ancien conteneur..."
    docker stop $CONTAINER_NAME 2>/dev/null || true
    docker rm $CONTAINER_NAME 2>/dev/null || true
    
    # Démarrer le nouveau conteneur
    echo "▶️ Démarrage du nouveau conteneur..."
    docker run -d \
        --name $CONTAINER_NAME \
        --restart unless-stopped \
        -p 8090:8080 \
        -e ASPNETCORE_ENVIRONMENT=Production \
        --env-file .env \
        "$FULL_IMAGE_NAME"
    
    echo "✅ Déploiement local terminé avec succès!"
    echo "📊 Statut du conteneur:"
    docker ps | grep $CONTAINER_NAME || echo "❌ Conteneur non trouvé"
    echo "🌐 L'API devrait être accessible sur http://localhost"
fi

echo "🎉 Déploiement terminé!"