# E-Shop API (.NET 9)

Cette API expose le catalogue de produits, les classes, et permet de persister les commandes.

- Base de données : PostgreSQL
- ORM : Dapper

## Endpoints
- `GET /api/products` : Liste des produits
- `GET /api/classes` : Liste des classes
- `POST /api/orders` : Création d'une commande

## Initialisation
Voir le script `init_db.sql` pour créer les tables nécessaires dans PostgreSQL.
