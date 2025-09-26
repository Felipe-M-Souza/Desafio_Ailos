const express = require('express');
const { body, validationResult } = require('express-validator');
const db = require('../config/database');
const { authenticateToken } = require('../middleware/auth');

const router = express.Router();

// Aplicar autenticação em todas as rotas
router.use(authenticateToken);

// Validações
const categoryValidation = [
  body('name').trim().isLength({ min: 1 }).withMessage('Nome é obrigatório'),
  body('type').isIn(['income', 'expense']).withMessage('Tipo deve ser income ou expense'),
  body('color').optional().isHexColor().withMessage('Cor deve ser um código hexadecimal válido'),
  body('icon').optional().isLength({ min: 1 }).withMessage('Ícone é obrigatório')
];

// Listar categorias
router.get('/', (req, res) => {
  const userId = req.user.id;

  db.all(`
    SELECT * FROM categories 
    WHERE user_id = ? OR user_id IS NULL
    ORDER BY user_id DESC, name ASC
  `, [userId], (err, categories) => {
    if (err) {
      return res.status(500).json({ message: 'Erro ao buscar categorias' });
    }

    res.json(categories);
  });
});

// Buscar categoria por ID
router.get('/:id', (req, res) => {
  const { id } = req.params;
  const userId = req.user.id;

  db.get(`
    SELECT * FROM categories 
    WHERE id = ? AND (user_id = ? OR user_id IS NULL)
  `, [id, userId], (err, category) => {
    if (err) {
      return res.status(500).json({ message: 'Erro ao buscar categoria' });
    }

    if (!category) {
      return res.status(404).json({ message: 'Categoria não encontrada' });
    }

    res.json(category);
  });
});

// Criar categoria
router.post('/', categoryValidation, (req, res) => {
  const errors = validationResult(req);
  if (!errors.isEmpty()) {
    return res.status(400).json({ 
      message: 'Dados inválidos',
      errors: errors.array()
    });
  }

  const { name, type, color = '#3B82F6', icon = '💰' } = req.body;
  const userId = req.user.id;

  // Verificar se já existe uma categoria com o mesmo nome para o usuário
  db.get(`
    SELECT id FROM categories 
    WHERE name = ? AND type = ? AND user_id = ?
  `, [name, type, userId], (err, existingCategory) => {
    if (err) {
      return res.status(500).json({ message: 'Erro ao verificar categoria' });
    }

    if (existingCategory) {
      return res.status(400).json({ message: 'Já existe uma categoria com este nome' });
    }

    db.run(`
      INSERT INTO categories (name, type, color, icon, user_id)
      VALUES (?, ?, ?, ?, ?)
    `, [name, type, color, icon, userId], function(err) {
      if (err) {
        return res.status(500).json({ message: 'Erro ao criar categoria' });
      }

      db.get('SELECT * FROM categories WHERE id = ?', [this.lastID], (err, category) => {
        if (err) {
          return res.status(500).json({ message: 'Erro ao buscar categoria criada' });
        }

        res.status(201).json({
          message: 'Categoria criada com sucesso',
          category
        });
      });
    });
  });
});

// Atualizar categoria
router.put('/:id', categoryValidation, (req, res) => {
  const errors = validationResult(req);
  if (!errors.isEmpty()) {
    return res.status(400).json({ 
      message: 'Dados inválidos',
      errors: errors.array()
    });
  }

  const { id } = req.params;
  const { name, type, color, icon } = req.body;
  const userId = req.user.id;

  // Verificar se a categoria existe e pertence ao usuário
  db.get(`
    SELECT id FROM categories 
    WHERE id = ? AND user_id = ?
  `, [id, userId], (err, category) => {
    if (err) {
      return res.status(500).json({ message: 'Erro ao verificar categoria' });
    }

    if (!category) {
      return res.status(404).json({ message: 'Categoria não encontrada ou não pertence ao usuário' });
    }

    // Verificar se já existe outra categoria com o mesmo nome
    db.get(`
      SELECT id FROM categories 
      WHERE name = ? AND type = ? AND user_id = ? AND id != ?
    `, [name, type, userId, id], (err, existingCategory) => {
      if (err) {
        return res.status(500).json({ message: 'Erro ao verificar categoria' });
      }

      if (existingCategory) {
        return res.status(400).json({ message: 'Já existe uma categoria com este nome' });
      }

      db.run(`
        UPDATE categories 
        SET name = ?, type = ?, color = ?, icon = ?
        WHERE id = ? AND user_id = ?
      `, [name, type, color, icon, id, userId], function(err) {
        if (err) {
          return res.status(500).json({ message: 'Erro ao atualizar categoria' });
        }

        if (this.changes === 0) {
          return res.status(404).json({ message: 'Categoria não encontrada' });
        }

        db.get('SELECT * FROM categories WHERE id = ?', [id], (err, updatedCategory) => {
          if (err) {
            return res.status(500).json({ message: 'Erro ao buscar categoria atualizada' });
          }

          res.json({
            message: 'Categoria atualizada com sucesso',
            category: updatedCategory
          });
        });
      });
    });
  });
});

// Deletar categoria
router.delete('/:id', (req, res) => {
  const { id } = req.params;
  const userId = req.user.id;

  // Verificar se a categoria está sendo usada em transações
  db.get(`
    SELECT COUNT(*) as count FROM transactions 
    WHERE category_id = ? AND user_id = ?
  `, [id, userId], (err, result) => {
    if (err) {
      return res.status(500).json({ message: 'Erro ao verificar uso da categoria' });
    }

    if (result.count > 0) {
      return res.status(400).json({ 
        message: 'Não é possível deletar categoria que está sendo usada em transações' 
      });
    }

    db.run(`
      DELETE FROM categories 
      WHERE id = ? AND user_id = ?
    `, [id, userId], function(err) {
      if (err) {
        return res.status(500).json({ message: 'Erro ao deletar categoria' });
      }

      if (this.changes === 0) {
        return res.status(404).json({ message: 'Categoria não encontrada' });
      }

      res.json({ message: 'Categoria deletada com sucesso' });
    });
  });
});

// Estatísticas de categorias
router.get('/stats/usage', (req, res) => {
  const { start_date, end_date } = req.query;
  const userId = req.user.id;

  let dateFilter = '';
  let params = [userId];

  if (start_date && end_date) {
    dateFilter = ' AND t.date BETWEEN ? AND ?';
    params.push(start_date, end_date);
  }

  const query = `
    SELECT 
      c.id,
      c.name,
      c.type,
      c.color,
      c.icon,
      COUNT(t.id) as transaction_count,
      COALESCE(SUM(t.amount), 0) as total_amount
    FROM categories c
    LEFT JOIN transactions t ON c.id = t.category_id AND t.user_id = ? ${dateFilter}
    WHERE c.user_id = ? OR c.user_id IS NULL
    GROUP BY c.id, c.name, c.type, c.color, c.icon
    ORDER BY total_amount DESC
  `;

  db.all(query, params, (err, results) => {
    if (err) {
      return res.status(500).json({ message: 'Erro ao buscar estatísticas das categorias' });
    }

    res.json(results);
  });
});

module.exports = router;

