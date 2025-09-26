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
    transferBtn: document.getElementById('transferBtn'),
    statementBtn: document.getElementById('statementBtn'),
    activateBtn: document.getElementById('activateBtn'),
    
    // Modals
    transactionModal: document.getElementById('transactionModal'),
    transferModal: document.getElementById('transferModal'),
    statementModal: document.getElementById('statementModal'),
    closeModal: document.getElementById('closeModal'),
    closeTransferModal: document.getElementById('closeTransferModal'),
    closeStatementModal: document.getElementById('closeStatementModal'),
    
    // Forms
    transactionForm: document.getElementById('transactionForm'),
    transactionDate: document.getElementById('transactionDate'),
    transactionValue: document.getElementById('transactionValue'),
    transactionDescription: document.getElementById('transactionDescription'),
    cancelTransaction: document.getElementById('cancelTransaction'),
    confirmTransaction: document.getElementById('confirmTransaction'),
    
    // Transfer Form
    transferForm: document.getElementById('transferForm'),
    transferDate: document.getElementById('transferDate'),
    transferDestinationAccount: document.getElementById('transferDestinationAccount'),
    transferValue: document.getElementById('transferValue'),
    transferDescription: document.getElementById('transferDescription'),
    cancelTransfer: document.getElementById('cancelTransfer'),
    confirmTransfer: document.getElementById('confirmTransfer'),
    
    // Statement
    statementStartDate: document.getElementById('statementStartDate'),
    statementEndDate: document.getElementById('statementEndDate'),
    loadStatement: document.getElementById('loadStatement'),
    printStatement: document.getElementById('printStatement'),
    statementContent: document.getElementById('statementContent'),
    
    // Other
    loadingOverlay: document.getElementById('loadingOverlay'),
    toastContainer: document.getElementById('toastContainer'),
    recentTransactions: document.getElementById('recentTransactions')
};

// Debug: Verificar se os elementos foram encontrados
console.log('🔍 Verificando elementos DOM:');
console.log('recentTransactions:', elements.recentTransactions);
console.log('balanceAmount:', elements.balanceAmount);
console.log('accountStatus:', elements.accountStatus);

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
    elements.transferDate.value = today;
    
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
    elements.transferForm.addEventListener('submit', handleTransfer);
    
    // Button clicks
    elements.logoutBtn.addEventListener('click', handleLogout);
    elements.refreshBalance.addEventListener('click', loadBalance);
    elements.creditBtn.addEventListener('click', () => openTransactionModal('C'));
    elements.debitBtn.addEventListener('click', () => openTransactionModal('D'));
    elements.transferBtn.addEventListener('click', openTransferModal);
    elements.statementBtn.addEventListener('click', openStatementModal);
    elements.activateBtn.addEventListener('click', handleActivateAccount);
    elements.cancelTransaction.addEventListener('click', closeTransactionModal);
    elements.cancelTransfer.addEventListener('click', closeTransferModal);
    elements.closeModal.addEventListener('click', closeTransactionModal);
    elements.closeTransferModal.addEventListener('click', closeTransferModal);
    elements.closeStatementModal.addEventListener('click', closeStatementModal);
    elements.loadStatement.addEventListener('click', loadStatement);
    elements.printStatement.addEventListener('click', printStatementToPDF);
    
    // Modal clicks
    elements.transactionModal.addEventListener('click', (e) => {
        if (e.target === elements.transactionModal) closeTransactionModal();
    });
    
    elements.transferModal.addEventListener('click', (e) => {
        if (e.target === elements.transferModal) closeTransferModal();
    });
    
    elements.statementModal.addEventListener('click', (e) => {
        if (e.target === elements.statementModal) closeStatementModal();
    });
    
    // CPF formatting
    const cpfInput = document.getElementById('registerCpf');
    if (cpfInput) {
        cpfInput.addEventListener('input', (e) => {
            e.target.value = formatCPF(e.target.value);
        });
    }
    
    // Currency formatting will be applied when modals are opened
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
            cpf: elements.registerForm.querySelector('#registerCpf').value.replace(/\D/g, ''), // Remove pontos e traços
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
    console.log('🔄 Carregando dados do dashboard...');
    await Promise.all([
        loadBalance(),
        loadRecentTransactions()
    ]);
    
    updateAccountStatus();
    console.log('✅ Dashboard carregado com sucesso');
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
        console.log('💰 Carregando saldo...');
        const response = await fetch(`${API_BASE_URL}/contas/${currentUser.id}/saldo`, {
            headers: {
                'Authorization': `Bearer ${currentToken}`
            }
        });
        
        console.log('💰 Status da resposta do saldo:', response.status);
        
        if (response.ok) {
            const result = await response.json();
            console.log('💰 Saldo recebido:', result);
            elements.balanceAmount.textContent = formatCurrency(result.saldoAtual);
            console.log('💰 Saldo atualizado no HTML');
        } else {
            console.error('❌ Erro na resposta do saldo:', response.status);
        }
    } catch (error) {
        console.error('❌ Erro ao carregar saldo:', error);
    }
}

async function loadRecentTransactions() {
    try {
        console.log('🔄 Carregando transações recentes...');
        const response = await fetch(`${API_BASE_URL}/contas/${currentUser.id}/extrato?page=1&pageSize=5`, {
            headers: {
                'Authorization': `Bearer ${currentToken}`
            }
        });
        
        console.log('📊 Status da resposta:', response.status);
        
        if (response.ok) {
            const result = await response.json();
            console.log('📋 Dados recebidos:', result);
            displayRecentTransactions(result.itens);
        } else {
            console.error('❌ Erro na resposta:', response.status);
        }
    } catch (error) {
        console.error('❌ Erro ao carregar transações:', error);
    }
}

function displayRecentTransactions(transactions) {
    console.log('🎯 displayRecentTransactions chamada com:', transactions);
    console.log('🎯 Elemento recentTransactions:', elements.recentTransactions);
    
    if (!transactions || transactions.length === 0) {
        console.log('⚠️ Nenhuma transação encontrada');
        elements.recentTransactions.innerHTML = '<p style="text-align: center; color: #666; padding: 2rem;">Nenhuma transação encontrada</p>';
        return;
    }
    
    console.log('✅ Atualizando HTML com', transactions.length, 'transações');
    const htmlContent = transactions.map(transaction => `
        <div class="transaction-item">
            <div class="transaction-info">
                <div class="transaction-type">${transaction.descricao || (transaction.tipo === 'C' ? 'Depósito' : 'Saque')}</div>
                <div class="transaction-date">${formatDate(transaction.data)}</div>
            </div>
            <div class="transaction-amount ${transaction.tipo === 'C' ? 'credit' : 'debit'}">
                ${transaction.tipo === 'C' ? '+' : '-'}${formatCurrency(transaction.valor)}
            </div>
        </div>
    `).join('');
    
    console.log('🔍 HTML que será inserido:', htmlContent);
    elements.recentTransactions.innerHTML = htmlContent;
    console.log('✅ HTML atualizado com sucesso');
    console.log('🔍 Elemento após atualização:', elements.recentTransactions.innerHTML);
}

function openTransactionModal(type) {
    currentTransactionType = type;
    const title = type === 'C' ? 'Lançar Depósito' : 'Lançar Saque';
    document.getElementById('modalTitle').textContent = title;
    
    // Update button text
    elements.confirmTransaction.innerHTML = `
        <i class="fas fa-check"></i> 
        ${type === 'C' ? 'Depositar' : 'Sacar'}
    `;
    
    // Add currency formatting to value input
    const transactionValueInput = document.getElementById('transactionValue');
    if (transactionValueInput) {
        transactionValueInput.addEventListener('input', (e) => {
            e.target.value = formatCurrencyInput(e.target.value);
        });
    }
    
    elements.transactionModal.classList.add('active');
}

function closeTransactionModal() {
    elements.transactionModal.classList.remove('active');
    elements.transactionForm.reset();
    elements.transactionDate.value = new Date().toISOString().split('T')[0];
    currentTransactionType = null;
}

function openTransferModal() {
    // Add currency formatting to transfer value input
    const transferValueInput = document.getElementById('transferValue');
    if (transferValueInput) {
        transferValueInput.addEventListener('input', (e) => {
            e.target.value = formatCurrencyInput(e.target.value);
        });
    }
    
    elements.transferModal.classList.add('active');
}

function closeTransferModal() {
    elements.transferModal.classList.remove('active');
    elements.transferForm.reset();
    elements.transferDate.value = new Date().toISOString().split('T')[0];
}

async function handleTransaction(e) {
    e.preventDefault();
    showLoading();
    
    try {
        console.log('💳 Iniciando transação...');
        const transactionData = {
            data: formatDateForAPI(elements.transactionDate.value),
            tipo: currentTransactionType,
            valor: parseFloat(elements.transactionValue.value.replace(/[^\d,]/g, '').replace(',', '.'))
        };
        
        console.log('💳 Dados da transação:', transactionData);
        
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
        
        console.log('💳 Status da resposta:', response.status);
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Erro ao processar transação');
        }
        
        const result = await response.json();
        console.log('💳 Resultado da transação:', result);
        
        showToast(
            `${currentTransactionType === 'C' ? 'Crédito' : 'Débito'} de ${formatCurrency(transactionData.valor)} processado com sucesso!`, 
            'success'
        );
        
        closeTransactionModal();
        
        console.log('💳 Carregando dados do dashboard...');
        loadDashboardData();
        
        // Se o modal de extrato estiver aberto, atualizar também
        if (elements.statementModal.classList.contains('active')) {
            console.log('💳 Modal de extrato aberto, atualizando...');
            loadStatement();
        }
        
    } catch (error) {
        showToast(error.message, 'error');
    } finally {
        hideLoading();
    }
}

async function handleTransfer(e) {
    e.preventDefault();
    showLoading();
    
    try {
        const transferData = {
            numeroContaDestino: parseInt(elements.transferDestinationAccount.value),
            valor: parseFloat(elements.transferValue.value.replace(/[^\d,]/g, '').replace(',', '.')),
            data: formatDateForAPI(elements.transferDate.value),
            descricao: elements.transferDescription.value || null
        };
        
        const response = await fetch(`${API_BASE_URL}/transferencias/${currentUser.id}/transferir`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${currentToken}`
            },
            body: JSON.stringify(transferData)
        });
        
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.error || 'Erro ao realizar transferência');
        }
        
        const result = await response.json();
        
        showToast(`Transferência realizada com sucesso! Novo saldo: ${formatCurrency(result.saldoAtual)}`, 'success');
        
        closeTransferModal();
        
        loadDashboardData();
        
        // Se o modal de extrato estiver aberto, atualizar também
        if (elements.statementModal.classList.contains('active')) {
            loadStatement();
        }
        
    } catch (error) {
        showToast(error.message, 'error');
    } finally {
        hideLoading();
    }
}

function openStatementModal() {
    console.log('📄 Abrindo modal de extrato...');
    elements.statementModal.classList.add('active');
    loadStatement();
}

function closeStatementModal() {
    elements.statementModal.classList.remove('active');
}

async function loadStatement() {
    console.log('📄 Carregando extrato...');
    showLoading();
    
    try {
        const startDate = elements.statementStartDate.value;
        const endDate = elements.statementEndDate.value;
        
        console.log('📄 Elementos encontrados:', {
            startDateElement: elements.statementStartDate,
            endDateElement: elements.statementEndDate
        });
        console.log('📄 Datas selecionadas:', { startDate, endDate });
        
        let url = `${API_BASE_URL}/contas/${currentUser.id}/extrato?page=1&pageSize=50`;
        
        if (startDate) {
            url += `&data_inicio=${formatDateForAPI(startDate)}`;
        }
        
        if (endDate) {
            url += `&data_fim=${formatDateForAPI(endDate)}`;
        }
        
        console.log('📄 URL do extrato:', url);
        
        const response = await fetch(url, {
            headers: {
                'Authorization': `Bearer ${currentToken}`
            }
        });
        
        console.log('📄 Status da resposta do extrato:', response.status);
        
        if (response.ok) {
            const result = await response.json();
            console.log('📄 Dados do extrato recebidos:', result);
            displayStatement(result.itens);
        } else {
            console.error('❌ Erro na resposta do extrato:', response.status);
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
    console.log('📄 displayStatement chamada com:', transactions);
    console.log('📄 Elemento statementContent:', elements.statementContent);
    
    if (!transactions || transactions.length === 0) {
        console.log('⚠️ Nenhuma transação encontrada no período');
        elements.statementContent.innerHTML = '<p style="text-align: center; color: #666; padding: 2rem;">Nenhuma transação encontrada no período</p>';
        elements.printStatement.style.display = 'none';
        return;
    }
    
    console.log('✅ Exibindo', transactions.length, 'transações no extrato');
    const htmlContent = transactions.map(transaction => `
        <div class="statement-item">
            <div class="transaction-info">
                <div class="transaction-type">${transaction.descricao || (transaction.tipo === 'C' ? 'Depósito' : 'Saque')}</div>
                <div class="transaction-date">${formatDate(transaction.data)}</div>
            </div>
            <div class="transaction-amount ${transaction.tipo === 'C' ? 'credit' : 'debit'}">
                ${transaction.tipo === 'C' ? '+' : '-'}${formatCurrency(transaction.valor)}
            </div>
        </div>
    `).join('');
    
    console.log('📄 HTML do extrato:', htmlContent);
    elements.statementContent.innerHTML = htmlContent;
    console.log('✅ Extrato atualizado com sucesso');
    
    // Mostrar botão de impressão
    elements.printStatement.style.display = 'inline-block';
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
    // Verificar se dateString existe e não é undefined
    if (!dateString) {
        return 'Data não disponível';
    }
    
    // Converter de dd/MM/yyyy para yyyy-MM-dd para o JavaScript
    const parts = dateString.split('/');
    if (parts.length === 3) {
        const day = parts[0].padStart(2, '0');
        const month = parts[1].padStart(2, '0');
        const year = parts[2];
        const isoDate = `${year}-${month}-${day}`;
        const date = new Date(isoDate);
        return date.toLocaleDateString('pt-BR');
    }
    return dateString; // Fallback se não conseguir converter
}

function formatDateForAPI(dateString) {
    console.log('📅 formatDateForAPI chamada com:', dateString);
    console.log('📅 Tipo da data:', typeof dateString);
    
    if (!dateString) {
        console.log('📅 Data vazia, retornando vazio');
        return '';
    }
    
    // Usar a data exata selecionada, sem conversão de timezone
    // dateString já vem no formato YYYY-MM-DD do input[type="date"]
    const [year, month, day] = dateString.split('-');
    const formatted = `${day}/${month}/${year}`;
    
    console.log('📅 Partes da data:', { year, month, day });
    console.log('📅 Data formatada para API:', formatted);
    return formatted;
}

function formatCPF(value) {
    // Remove tudo que não é dígito
    const numbers = value.replace(/\D/g, '');
    
    // Aplica a máscara do CPF
    if (numbers.length <= 3) {
        return numbers;
    } else if (numbers.length <= 6) {
        return numbers.replace(/(\d{3})(\d+)/, '$1.$2');
    } else if (numbers.length <= 9) {
        return numbers.replace(/(\d{3})(\d{3})(\d+)/, '$1.$2.$3');
    } else {
        return numbers.replace(/(\d{3})(\d{3})(\d{3})(\d+)/, '$1.$2.$3-$4');
    }
}

function formatCurrencyInput(value) {
    // Remove tudo que não é dígito
    const numbers = value.replace(/\D/g, '');
    
    if (numbers === '') {
        return '';
    }
    
    // Converte para número e formata como moeda brasileira
    const amount = parseInt(numbers) / 100;
    
    // Formata como moeda brasileira sem símbolo R$
    return new Intl.NumberFormat('pt-BR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    }).format(amount);
}

function printStatementToPDF() {
    try {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();
        
        // Configurações do PDF
        const pageWidth = doc.internal.pageSize.getWidth();
        const margin = 20;
        let yPosition = 20;
        
        // Cabeçalho
        doc.setFontSize(18);
        doc.setFont(undefined, 'bold');
        doc.text('EXTRATO BANCÁRIO', pageWidth / 2, yPosition, { align: 'center' });
        yPosition += 15;
        
        // Informações da conta
        doc.setFontSize(12);
        doc.setFont(undefined, 'normal');
        doc.text(`Conta: ${currentUser.numero}`, margin, yPosition);
        yPosition += 8;
        doc.text(`Cliente: ${currentUser.nome}`, margin, yPosition);
        yPosition += 8;
        doc.text(`Período: ${elements.statementStartDate.value} a ${elements.statementEndDate.value}`, margin, yPosition);
        yPosition += 8;
        doc.text(`Data de emissão: ${new Date().toLocaleDateString('pt-BR')}`, margin, yPosition);
        yPosition += 15;
        
        // Linha separadora
        doc.line(margin, yPosition, pageWidth - margin, yPosition);
        yPosition += 10;
        
        // Cabeçalho da tabela
        doc.setFont(undefined, 'bold');
        doc.text('Data', margin, yPosition);
        doc.text('Descrição', margin + 40, yPosition);
        doc.text('Valor', pageWidth - margin - 30, yPosition, { align: 'right' });
        yPosition += 8;
        
        // Linha separadora
        doc.line(margin, yPosition, pageWidth - margin, yPosition);
        yPosition += 8;
        
        // Transações
        doc.setFont(undefined, 'normal');
        const statementItems = elements.statementContent.querySelectorAll('.statement-item');
        
        statementItems.forEach(item => {
            // Verificar se precisa de nova página
            if (yPosition > 250) {
                doc.addPage();
                yPosition = 20;
            }
            
            const transactionType = item.querySelector('.transaction-type').textContent;
            const transactionDate = item.querySelector('.transaction-date').textContent;
            const transactionAmount = item.querySelector('.transaction-amount').textContent;
            
            doc.text(transactionDate, margin, yPosition);
            doc.text(transactionType, margin + 40, yPosition);
            doc.text(transactionAmount, pageWidth - margin - 30, yPosition, { align: 'right' });
            yPosition += 8;
        });
        
        // Salvar o PDF
        const fileName = `extrato_${currentUser.numero}_${new Date().toISOString().split('T')[0]}.pdf`;
        doc.save(fileName);
        
        showToast('Extrato salvo em PDF com sucesso!', 'success');
        
    } catch (error) {
        console.error('Erro ao gerar PDF:', error);
        showToast('Erro ao gerar PDF. Tente novamente.', 'error');
    }
}

function generateIdempotencyKey() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
