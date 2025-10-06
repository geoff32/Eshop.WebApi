#!/bin/bash

# Script de test de l'API d'authentification avec cookies
# Assurez-vous que l'application fonctionne sur https://localhost:5001

echo "🍪 Test de l'API d'authentification EShop (Cookies)"
echo "================================================="

BASE_URL="https://localhost:5001/api/auth"
COOKIE_JAR="cookies.txt"

# Couleurs pour les messages
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Nettoyer les anciens cookies
rm -f $COOKIE_JAR

echo ""
echo -e "${YELLOW}1. Test de création de compte avec cookie${NC}"
REGISTER_RESPONSE=$(curl -k -s -c $COOKIE_JAR -X POST "$BASE_URL/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "firstName": "Test",
    "lastName": "User",
    "password": "password123"
  }')

echo "Réponse d'inscription:"
echo "$REGISTER_RESPONSE" | jq 2>/dev/null || echo "$REGISTER_RESPONSE"

# Vérifier si la réponse contient l'utilisateur
USER_ID=$(echo "$REGISTER_RESPONSE" | jq -r '.user.id' 2>/dev/null)

if [ "$USER_ID" != "null" ] && [ ! -z "$USER_ID" ] && [ "$USER_ID" != "" ]; then
    echo -e "${GREEN}✅ Inscription réussie${NC}"
    
    echo ""
    echo -e "${YELLOW}2. Test de récupération du profil (avec cookie)${NC}"
    PROFILE_RESPONSE=$(curl -k -s -b $COOKIE_JAR -X GET "$BASE_URL/profile")
    
    echo "Réponse profil:"
    echo "$PROFILE_RESPONSE" | jq 2>/dev/null || echo "$PROFILE_RESPONSE"
    
    echo ""
    echo -e "${YELLOW}3. Test de déconnexion${NC}"
    LOGOUT_RESPONSE=$(curl -k -s -b $COOKIE_JAR -X POST "$BASE_URL/logout")
    
    echo "Réponse déconnexion:"
    echo "$LOGOUT_RESPONSE" | jq 2>/dev/null || echo "$LOGOUT_RESPONSE"
    
    echo ""
    echo -e "${YELLOW}4. Test de connexion${NC}"
    LOGIN_RESPONSE=$(curl -k -s -c $COOKIE_JAR -X POST "$BASE_URL/login" \
      -H "Content-Type: application/json" \
      -d '{
        "email": "test@example.com",
        "password": "password123"
      }')
    
    echo "Réponse de connexion:"
    echo "$LOGIN_RESPONSE" | jq 2>/dev/null || echo "$LOGIN_RESPONSE"
    
    echo ""
    echo -e "${YELLOW}5. Test du profil après reconnexion${NC}"
    PROFILE2_RESPONSE=$(curl -k -s -b $COOKIE_JAR -X GET "$BASE_URL/profile")
    
    echo "Réponse profil après reconnexion:"
    echo "$PROFILE2_RESPONSE" | jq 2>/dev/null || echo "$PROFILE2_RESPONSE"
    
    echo ""
    echo -e "${GREEN}✅ Tests terminés avec succès${NC}"
    echo -e "${YELLOW}📋 Cookies sauvegardés dans: $COOKIE_JAR${NC}"
else
    echo -e "${RED}❌ Échec de l'inscription${NC}"
    echo "Vérifiez que l'application fonctionne et que la base de données est configurée."
fi

echo ""
echo "💡 Pour tester manuellement, accédez à: https://localhost:5001/swagger"
echo "🍪 Note: Les cookies sont automatiquement gérés par le navigateur"

# Nettoyer
rm -f $COOKIE_JAR