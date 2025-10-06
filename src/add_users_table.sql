-- Migration pour ajouter la table users
-- Exécutez ce script si vous avez déjà une base de données existante

CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN DEFAULT true
);

-- Index pour améliorer les performances de recherche par email
CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);

-- Index pour améliorer les performances de recherche des utilisateurs actifs
CREATE INDEX IF NOT EXISTS idx_users_active ON users(is_active);