const sqlite3 = require('sqlite3').verbose();
const path = require('path');

const dbPath = process.env.DB_PATH || path.join(__dirname, '../database.sqlite');

const db = new sqlite3.Database(dbPath, (err) => {
  if (err) {
    console.error('Erro ao conectar com o banco de dados:', err.message);
  } else {
    console.log('✅ Conectado ao banco de dados SQLite');
    initializeDatabase();
  }
});

function initializeDatabase() {
  // Tabela de usuários
  db.run(`
    CREATE TABLE IF NOT EXISTS users (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      name TEXT NOT NULL,
      email TEXT UNIQUE NOT NULL,
      password TEXT NOT NULL,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
    )
  `);

  // Tabela de categorias
  db.run(`
    CREATE TABLE IF NOT EXISTS categories (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      name TEXT NOT NULL,
      type TEXT NOT NULL CHECK (type IN ('income', 'expense')),
      color TEXT DEFAULT '#3B82F6',
      icon TEXT DEFAULT '💰',
      user_id INTEGER,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
    )
  `);

  // Tabela de transações
  db.run(`
    CREATE TABLE IF NOT EXISTS transactions (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      description TEXT NOT NULL,
      amount DECIMAL(10,2) NOT NULL,
      type TEXT NOT NULL CHECK (type IN ('income', 'expense')),
      category_id INTEGER,
      user_id INTEGER NOT NULL,
      date DATE NOT NULL,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (category_id) REFERENCES categories (id) ON DELETE SET NULL,
      FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
    )
  `);

  // Tabela de metas financeiras
  db.run(`
    CREATE TABLE IF NOT EXISTS goals (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      title TEXT NOT NULL,
      description TEXT,
      target_amount DECIMAL(10,2) NOT NULL,
      current_amount DECIMAL(10,2) DEFAULT 0,
      target_date DATE,
      user_id INTEGER NOT NULL,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
    )
  `);

  // Inserir categorias padrão
  const defaultCategories = [
    { name: 'Salário', type: 'income', color: '#10B981', icon: '💼' },
    { name: 'Freelance', type: 'income', color: '#8B5CF6', icon: '💻' },
    { name: 'Investimentos', type: 'income', color: '#F59E0B', icon: '📈' },
    { name: 'Alimentação', type: 'expense', color: '#EF4444', icon: '🍽️' },
    { name: 'Transporte', type: 'expense', color: '#3B82F6', icon: '🚗' },
    { name: 'Lazer', type: 'expense', color: '#EC4899', icon: '🎬' },
    { name: 'Saúde', type: 'expense', color: '#06B6D4', icon: '🏥' },
    { name: 'Educação', type: 'expense', color: '#84CC16', icon: '📚' }
  ];

  defaultCategories.forEach(category => {
    db.run(`
      INSERT OR IGNORE INTO categories (name, type, color, icon, user_id)
      VALUES (?, ?, ?, ?, NULL)
    `, [category.name, category.type, category.color, category.icon]);
  });

  console.log('📊 Banco de dados inicializado com sucesso');
}

module.exports = db;

