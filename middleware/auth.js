const jwt = require('jsonwebtoken');
const db = require('../config/database');

const authenticateToken = (req, res, next) => {
  const authHeader = req.headers['authorization'];
  const token = authHeader && authHeader.split(' ')[1]; // Bearer TOKEN

  if (!token) {
    return res.status(401).json({ message: 'Token de acesso necessário' });
  }

  jwt.verify(token, process.env.JWT_SECRET || 'fallback_secret', (err, user) => {
    if (err) {
      return res.status(403).json({ message: 'Token inválido ou expirado' });
    }

    // Verificar se o usuário ainda existe no banco
    db.get('SELECT id, name, email FROM users WHERE id = ?', [user.id], (err, row) => {
      if (err) {
        return res.status(500).json({ message: 'Erro interno do servidor' });
      }

      if (!row) {
        return res.status(403).json({ message: 'Usuário não encontrado' });
      }

      req.user = row;
      next();
    });
  });
};

const generateToken = (user) => {
  return jwt.sign(
    { id: user.id, email: user.email },
    process.env.JWT_SECRET || 'fallback_secret',
    { expiresIn: process.env.JWT_EXPIRES_IN || '7d' }
  );
};

module.exports = {
  authenticateToken,
  generateToken
};

