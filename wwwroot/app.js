// API Configuration
const API_BASE_URL = 'http://localhost:5009/api';

// Application State
let currentUser = null;
let currentToken = null;
let currentTransactionType = null;

// DOM Elements
const elements = {
    // Screens
    loginScreen: document.getElementById('loginScreen'),
    dashboardScreen: document.getElementById('dashboardScreen'),
    
    // Auth
    loginForm: document.getElementById('loginFormElement'),
    registerForm: document.getElementById('registerFormElement'),
    tabBtns: document.querySelectorAll('.tab-btn'),
    authForms: document.querySelectorAll('.auth-form'),
    
    // User Info
    userInfo: document.getElementById('userInfo'),
    userName: document.getElementById('userName'),
    logoutBtn: document.getElementById('logoutBtn'),
    
    // Dashboard
    accountNumber: document.getElementById('accountNumber'),
    accountName: document.getElementById('accountName'),
    accountStatus: document.getElementById('accountStatus'),
    accountStatusText: document.getElementById('accountStatusText'),
    balanceAmount: document.getElementById('balanceAmount'),
    refreshBalance: document.getElementById('refreshBalance'),
    
    // Actions
    creditBtn: document.getElementById('creditBtn'),
    debitBtn: document.getElementById('debitBtn'),
    statementBtn: document.getElementById('statementBtn'),
    activateBtn: document.getElementById('activateBtn'),
    
    // Modals
    transactionModal: document.getElementById('transactionModal'),
    statementModal: document.getElementById('statementModal'),
    closeModal: document.getElementById('closeModal'),
    closeStatementModal: document.getElementById('closeStatementModal'),
    
    // Forms
    transactionForm: document.getElementById('transactionForm'),
    transactionDate: document.getElementById('transactionDate'),
    transactionValue: document.getElementById('transactionValue'),
    transactionDescription: document.getElementById('transactionDescription'),
    cancelTransaction: document.getElementById('cancelTransaction'),
    confirmTransaction: document.getElementById('confirmTransaction'),
    
    // Statement
    statementStartDate: document.getElementById('statementStartDate'),
    statementEndDate: document.getElementById('statementEndDate'),
    loadStatement: document.getElementById('loadStatement'),
    statementContent: document.getElementById('statementContent'),
    
    // Other
    loadingOverlay: document.getElementById('loadingOverlay'),
    toastContainer: document.getElementById('toastContainer'),
    recentTransactions: document.getElementById('recentTransactions')
};

// Initialize App
document.addEventListener('DOMContentLoaded', function() {
    initializeApp();
    setupEventListeners();
    checkAuthStatus();
});

function initializeApp() {
    // Set default date to today
    const today = new Date().toISOString().split('T')[0];
    elements.transactionDate.value = today;
    
    // Set default statement dates (last 30 days)
    const thirtyDaysAgo = new Date();
    thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
    elements.statementStartDate.value = thirtyDaysAgo.toISOString().split('T')[0];
    elements.statementEndDate.value = today;
}

function setupEventListeners() {
    // Tab switching
    elements.tabBtns.forEach(btn => {
        btn.addEventListener('click', () => switchTab(btn.dataset.tab));
    });
    
    // Form submissions
    elements.loginForm.addEventListener('submit', handleLogin);
    elements.registerForm.addEventListener('submit', handleRegister);
    elements.transactionForm.addEventListener('submit', handleTransaction);
    
    // Button clicks
    elements.logoutBtn.addEventListener('click', handleLogout);
    elements.refreshBalance.addEventListener('click', loadBalance);
    elements.creditBtn.addEventListener('click', () => openTransactionModal('C'));
    elements.debitBtn.addEventListener('click', () => openTransactionModal('D'));
    elements.statementBtn.addEventListener('click', openStatementModal);
    elements.activateBtn.addEventListener('click', handleActivateAccount);
    elements.cancelTransaction.addEventListener('click', closeTransactionModal);
    elements.closeModal.addEventListener('click', closeTransactionModal);
    elements.closeStatementModal.addEventListener('click', closeStatementModal);
    elements.loadStatement.addEventListener('click', loadStatement);
    
    // Modal clicks
    elements.transactionModal.addEventListener('click', (e) => {
        if (e.target === elements.transactionModal) closeTransactionModal();
    });
    
    elements.statementModal.addEventListener('click', (e) => {
        if (e.target === elements.statementModal) closeStatementModal();
    });
}

function checkAuthStatus() {
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('user');
    
    if (token && user) {
        currentToken = token;
        currentUser = JSON.parse(user);
        showDashboard();
    } else {
        showLogin();
    }
}

function switchTab(tab) {
    // Update tab buttons
    elements.tabBtns.forEach(btn => {
        btn.classList.toggle('active', btn.dataset.tab === tab);
    });
    
    // Update forms
    elements.authForms.forEach(form => {
        form.classList.toggle('active', form.id === tab + 'Form');
    });
}

function showLogin() {
    elements.loginScreen.style.display = 'block';
    elements.dashboardScreen.style.display = 'none';
    elements.userInfo.style.display = 'none';
}

function showDashboard() {
    elements.loginScreen.style.display = 'none';
    elements.dashboardScreen.style.display = 'block';
    elements.userInfo.style.display = 'flex';
    
    // Update user info
    elements.userName.textContent = currentUser.nome;
    elements.accountNumber.textContent = currentUser.numero;
    elements.accountName.textContent = currentUser.nome;
    
    // Load dashboard data
    loadDashboardData();
}

async function handleLogin(e) {
    e.preventDefault();
    showLoading();
    
    try {
        const formData = new FormData(e.target);
        const loginData = {
            numero: parseInt(elements.loginForm.querySelector('#loginNumero').value),
            senha: elements.loginForm.querySelector('#loginSenha').value
        };
        
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(loginData)
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Erro no login');
        }
        
        const result = await response.json();
        currentToken = result.token;
        
        // Get account info
        const accountResponse = await fetch(`${API_BASE_URL}/contas/${result.id}`, {
            headers: {
                'Authorization': `Bearer ${currentToken}`
            }
        });
        
        if (accountResponse.ok) {
            currentUser = await accountResponse.json();
            
            // Save to localStorage
            localStorage.setItem('token', currentToken);
            localStorage.setItem('user', JSON.stringify(currentUser));
            
            showToast('Login realizado com sucesso!', 'success');
            showDashboard();
        } else {
            throw new Error('Erro ao carregar dados da conta');
        }
        
    } catch (error) {
        showToast(error.message, 'error');
    } finally {
        hideLoading();
    }
}

async function handleRegister(e) {
    e.preventDefault();
    showLoading();
    
    try {
        const registerData = {
            numero: parseInt(elements.registerForm.querySelector('#registerNumero').value),
            nome: elements.registerForm.querySelector('#registerNome').value,
            senha: elements.registerForm.querySelector('#registerSenha').value
        };
        
        const response = await fetch(`${API_BASE_URL}/contas`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(registerData)
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Erro ao criar conta');
        }
        
        const result = await response.json();
        showToast('Conta criada com sucesso! Faça login para continuar.', 'success');
        
        // Switch to login tab
        switchTab('login');
        elements.loginForm.querySelector('#loginNumero').value = registerData.numero;
        
    } catch (error) {
        showToast(error.message, 'error');
    } finally {
        hideLoading();
    }
}

async function handleActivateAccount() {
    showLoading();
    
    try {
        const response = await fetch(`${API_BASE_URL}/contas/${currentUser.id}/ativar`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${currentToken}`
            },
            body: JSON.stringify({ ativo: true })
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Erro ao ativar conta');
        }
        
        currentUser.ativo = true;
        localStorage.setItem('user', JSON.stringify(currentUser));
        
        showToast('Conta ativada com sucesso!', 'success');
        loadDashboardData();
        
    } catch (error) {
        showToast(error.message, 'error');
    } finally {
        hideLoading();
    }
}

async function loadDashboardData() {
    await Promise.all([
        loadBalance(),
        loadRecentTransactions()
    ]);
    
    updateAccountStatus();
}

function updateAccountStatus() {
    const isActive = currentUser.ativo;
    elements.accountStatus.className = `status-badge ${isActive ? 'active' : 'inactive'}`;
    elements.accountStatus.textContent = isActive ? 'Ativa' : 'Inativa';
    elements.accountStatusText.textContent = isActive ? 'Ativa' : 'Inativa';
    
    // Show/hide activate button
    elements.activateBtn.style.display = isActive ? 'none' : 'flex';
    
    // Enable/disable transaction buttons
    const transactionButtons = [elements.creditBtn, elements.debitBtn];
    transactionButtons.forEach(btn => {
        btn.disabled = !isActive;
        btn.style.opacity = isActive ? '1' : '0.5';
    });
}

async function loadBalance() {
    try {
        const response = await fetch(`${API_BASE_URL}/contas/${currentUser.id}/saldo`, {
            headers: {
                'Authorization': `Bearer ${currentToken}`
            }
        });
        
        if (response.ok) {
            const result = await response.json();
            elements.balanceAmount.textContent = formatCurrency(result.saldoAtual);
        }
    } catch (error) {
        console.error('Erro ao carregar saldo:', error);
    }
}

async function loadRecentTransactions() {
    try {
        const response = await fetch(`${API_BASE_URL}/contas/${currentUser.id}/extrato?page=1&pageSize=5`, {
            headers: {
                'Authorization': `Bearer ${currentToken}`
            }
        });
        
        if (response.ok) {
            const result = await response.json();
            displayRecentTransactions(result.itens);
        }
    } catch (error) {
        console.error('Erro ao carregar transações:', error);
    }
}

function displayRecentTransactions(transactions) {
    if (!transactions || transactions.length === 0) {
        elements.recentTransactions.innerHTML = '<p style="text-align: center; color: #666; padding: 2rem;">Nenhuma transação encontrada</p>';
        return;
    }
    
    elements.recentTransactions.innerHTML = transactions.map(transaction => `
        <div class="transaction-item">
            <div class="transaction-info">
                <div class="transaction-type">${transaction.tipoMovimento === 'C' ? 'Crédito' : 'Débito'}</div>
                <div class="transaction-date">${formatDate(transaction.dataMovimento)}</div>
            </div>
            <div class="transaction-amount ${transaction.tipoMovimento === 'C' ? 'credit' : 'debit'}">
                ${transaction.tipoMovimento === 'C' ? '+' : '-'}${formatCurrency(transaction.valor)}
            </div>
        </div>
    `).join('');
}

function openTransactionModal(type) {
    currentTransactionType = type;
    const title = type === 'C' ? 'Lançar Crédito' : 'Lançar Débito';
    document.getElementById('modalTitle').textContent = title;
    
    // Update button text
    elements.confirmTransaction.innerHTML = `
        <i class="fas fa-check"></i> 
        ${type === 'C' ? 'Creditar' : 'Debitar'}
    `;
    
    elements.transactionModal.classList.add('active');
}

function closeTransactionModal() {
    elements.transactionModal.classList.remove('active');
    elements.transactionForm.reset();
    elements.transactionDate.value = new Date().toISOString().split('T')[0];
    currentTransactionType = null;
}

async function handleTransaction(e) {
    e.preventDefault();
    showLoading();
    
    try {
        const transactionData = {
            data: formatDateForAPI(elements.transactionDate.value),
            tipo: currentTransactionType,
            valor: parseFloat(elements.transactionValue.value)
        };
        
        const idempotencyKey = generateIdempotencyKey();
        
        const response = await fetch(`${API_BASE_URL}/contas/${currentUser.id}/movimentos`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${currentToken}`,
                'Idempotency-Key': idempotencyKey
            },
            body: JSON.stringify(transactionData)
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Erro ao processar transação');
        }
        
        const result = await response.json();
        showToast(
            `${currentTransactionType === 'C' ? 'Crédito' : 'Débito'} de ${formatCurrency(transactionData.valor)} processado com sucesso!`, 
            'success'
        );
        
        closeTransactionModal();
        loadDashboardData();
        
    } catch (error) {
        showToast(error.message, 'error');
    } finally {
        hideLoading();
    }
}

function openStatementModal() {
    elements.statementModal.classList.add('active');
    loadStatement();
}

function closeStatementModal() {
    elements.statementModal.classList.remove('active');
}

async function loadStatement() {
    showLoading();
    
    try {
        const startDate = elements.statementStartDate.value;
        const endDate = elements.statementEndDate.value;
        
        let url = `${API_BASE_URL}/contas/${currentUser.id}/extrato?page=1&pageSize=50`;
        
        if (startDate) {
            url += `&data_inicio=${formatDateForAPI(startDate)}`;
        }
        
        if (endDate) {
            url += `&data_fim=${formatDateForAPI(endDate)}`;
        }
        
        const response = await fetch(url, {
            headers: {
                'Authorization': `Bearer ${currentToken}`
            }
        });
        
        if (response.ok) {
            const result = await response.json();
            displayStatement(result.itens);
        } else {
            throw new Error('Erro ao carregar extrato');
        }
        
    } catch (error) {
        showToast(error.message, 'error');
        elements.statementContent.innerHTML = '<p style="text-align: center; color: #dc3545; padding: 2rem;">Erro ao carregar extrato</p>';
    } finally {
        hideLoading();
    }
}

function displayStatement(transactions) {
    if (!transactions || transactions.length === 0) {
        elements.statementContent.innerHTML = '<p style="text-align: center; color: #666; padding: 2rem;">Nenhuma transação encontrada no período</p>';
        return;
    }
    
    elements.statementContent.innerHTML = transactions.map(transaction => `
        <div class="statement-item">
            <div class="transaction-info">
                <div class="transaction-type">${transaction.tipoMovimento === 'C' ? 'Crédito' : 'Débito'}</div>
                <div class="transaction-date">${formatDate(transaction.dataMovimento)}</div>
            </div>
            <div class="transaction-amount ${transaction.tipoMovimento === 'C' ? 'credit' : 'debit'}">
                ${transaction.tipoMovimento === 'C' ? '+' : '-'}${formatCurrency(transaction.valor)}
            </div>
        </div>
    `).join('');
}

function handleLogout() {
    currentUser = null;
    currentToken = null;
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    showLogin();
    showToast('Logout realizado com sucesso!', 'success');
}

// Utility Functions
function showLoading() {
    elements.loadingOverlay.style.display = 'flex';
}

function hideLoading() {
    elements.loadingOverlay.style.display = 'none';
}

function showToast(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    toast.textContent = message;
    
    elements.toastContainer.appendChild(toast);
    
    setTimeout(() => {
        toast.remove();
    }, 5000);
}

function formatCurrency(value) {
    return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    }).format(value);
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
}

function formatDateForAPI(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
}

function generateIdempotencyKey() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}


