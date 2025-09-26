const express = require('express');
const { body, validationResult } = require('express-validator');
const db = require('../config/database');
const { authenticateToken } = require('../middleware/auth');

const router = express.Router();

// Aplicar autenticação em todas as rotas
router.use(authenticateToken);

// Validações
const transactionValidation = [
  body('description').trim().isLength({ min: 1 }).withMessage('Descrição é obrigatória'),
  body('amount').isFloat({ min: 0.01 }).withMessage('Valor deve ser maior que zero'),
  body('type').isIn(['income', 'expense']).withMessage('Tipo deve ser income ou expense'),
  body('date').isISO8601().withMessage('Data inválida')
];

// Listar transações
router.get('/', (req, res) => {
  const { page = 1, limit = 20, type, category_id, start_date, end_date } = req.query;
  const offset = (page - 1) * limit;
  const userId = req.user.id;

  let query = `
    SELECT t.*, c.name as category_name, c.color as category_color, c.icon as category_icon
    FROM transactions t
    LEFT JOIN categories c ON t.category_id = c.id
    WHERE t.user_id = ?
  `;
  let params = [userId];

  // Filtros
  if (type) {
    query += ' AND t.type = ?';
    params.push(type);
  }

  if (category_id) {
    query += ' AND t.category_id = ?';
    params.push(category_id);
  }

  if (start_date) {
    query += ' AND t.date >= ?';
    params.push(start_date);
  }

  if (end_date) {
    query += ' AND t.date <= ?';
    params.push(end_date);
  }

  query += ' ORDER BY t.date DESC, t.created_at DESC LIMIT ? OFFSET ?';
  params.push(parseInt(limit), parseInt(offset));

  db.all(query, params, (err, transactions) => {
    if (err) {
      return res.status(500).json({ message: 'Erro ao buscar transações' });
    }

    // Contar total de transações para paginação
    let countQuery = 'SELECT COUNT(*) as total FROM transactions WHERE user_id = ?';
    let countParams = [userId];

    if (type) {
      countQuery += ' AND type = ?';
      countParams.push(type);
    }

    if (category_id) {
      countQuery += ' AND category_id = ?';
      countParams.push(category_id);
    }

    if (start_date) {
      countQuery += ' AND date >= ?';
      countParams.push(start_date);
    }

    if (end_date) {
      countQuery += ' AND date <= ?';
      countParams.push(end_date);
    }

    db.get(countQuery, countParams, (err, countResult) => {
      if (err) {
        return res.status(500).json({ message: 'Erro ao contar transações' });
      }

      res.json({
        transactions,
        pagination: {
          page: parseInt(page),
          limit: parseInt(limit),
          total: countResult.total,
          pages: Math.ceil(countResult.total / limit)
        }
      });
    });
  });
});

// Buscar transação por ID
router.get('/:id', (req, res) => {
  const { id } = req.params;
  const userId = req.user.id;

  db.get(`
    SELECT t.*, c.name as category_name, c.color as category_color, c.icon as category_icon
    FROM transactions t
    LEFT JOIN categories c ON t.category_id = c.id
    WHERE t.id = ? AND t.user_id = ?
  `, [id, userId], (err, transaction) => {
    if (err) {
      return res.status(500).json({ message: 'Erro ao buscar transação' });
    }

    if (!transaction) {
      return res.status(404).json({ message: 'Transação não encontrada' });
    }

    res.json(transaction);
  });
});

// Criar transação
router.post('/', transactionValidation, (req, res) => {
  const errors = validationResult(req);
  if (!errors.isEmpty()) {
    return res.status(400).json({ 
      message: 'Dados inválidos',
      errors: errors.array()
    });
  }

  const { description, amount, type, category_id, date } = req.body;
  const userId = req.user.id;

  // Verificar se a categoria existe e pertence ao usuário (se fornecida)
  if (category_id) {
    db.get('SELECT id FROM categories WHERE id = ? AND (user_id = ? OR user_id IS NULL)', 
      [category_id, userId], (err, category) => {
        if (err) {
          return res.status(500).json({ message: 'Erro ao verificar categoria' });
        }

        if (!category) {
          return res.status(400).json({ message: 'Categoria não encontrada' });
        }

        createTransaction();
      });
  } else {
    createTransaction();
  }

  function createTransaction() {
    db.run(`
      INSERT INTO transactions (description, amount, type, category_id, user_id, date)
      VALUES (?, ?, ?, ?, ?, ?)
    `, [description, amount, type, category_id, userId, date], function(err) {
      if (err) {
        return res.status(500).json({ message: 'Erro ao criar transação' });
      }

      // Buscar a transação criada com dados da categoria
      db.get(`
        SELECT t.*, c.name as category_name, c.color as category_color, c.icon as category_icon
        FROM transactions t
        LEFT JOIN categories c ON t.category_id = c.id
        WHERE t.id = ?
      `, [this.lastID], (err, transaction) => {
        if (err) {
          return res.status(500).json({ message: 'Erro ao buscar transação criada' });
        }

        res.status(201).json({
          message: 'Transação criada com sucesso',
          transaction
        });
      });
    });
  }
});

// Atualizar transação
router.put('/:id', transactionValidation, (req, res) => {
  const errors = validationResult(req);
  if (!errors.isEmpty()) {
    return res.status(400).json({ 
      message: 'Dados inválidos',
      errors: errors.array()
    });
  }

  const { id } = req.params;
  const { description, amount, type, category_id, date } = req.body;
  const userId = req.user.id;

  // Verificar se a transação existe e pertence ao usuário
  db.get('SELECT id FROM transactions WHERE id = ? AND user_id = ?', 
    [id, userId], (err, transaction) => {
      if (err) {
        return res.status(500).json({ message: 'Erro ao verificar transação' });
      }

      if (!transaction) {
        return res.status(404).json({ message: 'Transação não encontrada' });
      }

      // Verificar categoria se fornecida
      if (category_id) {
        db.get('SELECT id FROM categories WHERE id = ? AND (user_id = ? OR user_id IS NULL)', 
          [category_id, userId], (err, category) => {
            if (err) {
              return res.status(500).json({ message: 'Erro ao verificar categoria' });
            }

            if (!category) {
              return res.status(400).json({ message: 'Categoria não encontrada' });
            }

            updateTransaction();
          });
      } else {
        updateTransaction();
      }

      function updateTransaction() {
        db.run(`
          UPDATE transactions 
          SET description = ?, amount = ?, type = ?, category_id = ?, date = ?, updated_at = CURRENT_TIMESTAMP
          WHERE id = ? AND user_id = ?
        `, [description, amount, type, category_id, date, id, userId], function(err) {
          if (err) {
            return res.status(500).json({ message: 'Erro ao atualizar transação' });
          }

          if (this.changes === 0) {
            return res.status(404).json({ message: 'Transação não encontrada' });
          }

          // Buscar a transação atualizada
          db.get(`
            SELECT t.*, c.name as category_name, c.color as category_color, c.icon as category_icon
            FROM transactions t
            LEFT JOIN categories c ON t.category_id = c.id
            WHERE t.id = ?
          `, [id], (err, updatedTransaction) => {
            if (err) {
              return res.status(500).json({ message: 'Erro ao buscar transação atualizada' });
            }

            res.json({
              message: 'Transação atualizada com sucesso',
              transaction: updatedTransaction
            });
          });
        });
      }
    });
});

// Deletar transação
router.delete('/:id', (req, res) => {
  const { id } = req.params;
  const userId = req.user.id;

  db.run('DELETE FROM transactions WHERE id = ? AND user_id = ?', 
    [id, userId], function(err) {
      if (err) {
        return res.status(500).json({ message: 'Erro ao deletar transação' });
      }

      if (this.changes === 0) {
        return res.status(404).json({ message: 'Transação não encontrada' });
      }

      res.json({ message: 'Transação deletada com sucesso' });
    });
});

// Estatísticas de transações
router.get('/stats/summary', (req, res) => {
  const { start_date, end_date } = req.query;
  const userId = req.user.id;

  let dateFilter = '';
  let params = [userId];

  if (start_date && end_date) {
    dateFilter = ' AND date BETWEEN ? AND ?';
    params.push(start_date, end_date);
  }

  const query = `
    SELECT 
      type,
      COUNT(*) as count,
      SUM(amount) as total
    FROM transactions 
    WHERE user_id = ? ${dateFilter}
    GROUP BY type
  `;

  db.all(query, params, (err, results) => {
    if (err) {
      return res.status(500).json({ message: 'Erro ao buscar estatísticas' });
    }

    const stats = {
      income: { count: 0, total: 0 },
      expense: { count: 0, total: 0 }
    };

    results.forEach(row => {
      stats[row.type] = {
        count: row.count,
        total: parseFloat(row.total)
      };
    });

    stats.balance = stats.income.total - stats.expense.total;

    res.json(stats);
  });
});

module.exports = router;

