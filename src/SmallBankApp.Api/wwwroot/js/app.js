const state = {
  token: localStorage.getItem('smallbank.token') || '',
  accounts: [
    { id:'demo-checking', accountType:'Everyday Checking', accountNumberMasked:'****1234', availableBalance:2450.52, currentBalance:2480.52, currency:'USD', isFrozen:false },
    { id:'demo-savings', accountType:'Goal Savings', accountNumberMasked:'****7788', availableBalance:12600.01, currentBalance:12600.01, currency:'USD', isFrozen:false }
  ],
  transactions: [
    { postedAt:'2026-05-20', description:'Payroll deposit', type:'Deposit', amount:3250.00, status:'Posted' },
    { postedAt:'2026-05-19', description:'Utility bill pay', type:'BillPay', amount:-142.67, status:'Posted' },
    { postedAt:'2026-05-18', description:'Coffee shop', type:'Debit', amount:-6.88, status:'Posted' },
    { postedAt:'2026-05-17', description:'External transfer', type:'TransferOut', amount:-250.00, status:'Pending' }
  ],
  amlCases: []
};
const money = value => Number(value).toLocaleString(undefined,{style:'currency',currency:'USD'});
function renderAccounts(){
  const list = document.getElementById('accountList');
  const fromAccount = document.getElementById('fromAccount');
  list.innerHTML = ''; fromAccount.innerHTML = '';
  state.accounts.forEach(account => {
    const tile = document.createElement('article');
    tile.className = 'account-tile';
    tile.innerHTML = `<div class="d-flex justify-content-between gap-3"><div><h3 class="h6 mb-1">${account.accountType}</h3><div class="account-mask text-muted small">${account.accountNumberMasked}</div></div><span class="badge ${account.isFrozen ? 'text-bg-danger':'text-bg-success'} align-self-start">${account.isFrozen ? 'Frozen':'Active'}</span></div><div class="mt-3 d-flex justify-content-between"><span>Available</span><strong>${money(account.availableBalance)}</strong></div><div class="small text-muted d-flex justify-content-between"><span>Current</span><span>${money(account.currentBalance)}</span></div>`;
    list.appendChild(tile);
    fromAccount.insertAdjacentHTML('beforeend', `<option value="${account.id}">${account.accountType} ${account.accountNumberMasked}</option>`);
  });
  const total = state.accounts.reduce((sum,a)=>sum + Number(a.availableBalance),0);
  document.getElementById('totalAvailable').textContent = money(total);
}
function renderTransactions(){
  const rows = document.getElementById('transactionRows');
  rows.innerHTML = state.transactions.map(tx => `<tr><td>${tx.postedAt}</td><td>${tx.description}</td><td>${tx.type}</td><td class="text-end ${tx.amount < 0 ? 'text-danger':'text-success'}">${money(tx.amount)}</td><td><span class="badge ${tx.status === 'Posted' ? 'text-bg-success':'text-bg-secondary'}">${tx.status}</span></td></tr>`).join('');
}
function renderAml(){
  const queue = document.getElementById('amlQueue');
  document.getElementById('amlCount').textContent = state.amlCases.length;
  if(!state.amlCases.length){ queue.innerHTML = '<p class="text-muted mb-0">No transfers currently held for review.</p>'; return; }
  queue.innerHTML = state.amlCases.map(c => `<div class="alert alert-warning mb-0"><strong>${c.riskLevel} risk:</strong> ${c.reason}<br><small>Case ${c.id} · ${money(c.amount)} · ${c.memo}</small></div>`).join('');
}
function showToast(title, body, tone='primary'){
  const region = document.getElementById('toastRegion');
  const id = `toast-${Date.now()}`;
  region.insertAdjacentHTML('beforeend', `<div id="${id}" class="toast" role="status" aria-live="polite" aria-atomic="true"><div class="toast-header text-bg-${tone}"><strong class="me-auto">${title}</strong><button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button></div><div class="toast-body">${body}</div></div>`);
  const toast = new bootstrap.Toast(document.getElementById(id), { delay: 5000 });
  toast.show();
}
async function api(path, options = {}){
  const headers = { 'Content-Type':'application/json', ...(options.headers || {}) };
  if(state.token) headers.Authorization = `Bearer ${state.token}`;
  const response = await fetch(path, { ...options, headers });
  if(!response.ok) throw new Error(await response.text());
  return response.json();
}
async function loadAccountsFromApi(){
  if(!state.token) return;
  try { state.accounts = await api('/api/accounts'); renderAccounts(); }
  catch(error){ console.warn('Using demo account data because API is not authenticated or database is unavailable.', error); }
}
document.getElementById('transferForm').addEventListener('submit', async event => {
  event.preventDefault();
  const amount = Number(document.getElementById('amount').value);
  const memo = document.getElementById('memo').value.trim();
  const review = amount >= 10000 || /crypto mixer|sanction|structuring|cash mule/i.test(memo);
  if(review){
    state.amlCases.unshift({ id:crypto.randomUUID().slice(0,8), riskLevel: amount >= 10000 ? 'High':'Medium', reason:'Transfer matched AML review rules.', amount, memo });
    renderAml(); showToast('AML review', 'Transfer held for compliance review.', 'warning'); return;
  }
  state.transactions.unshift({ postedAt:new Date().toISOString().slice(0,10), description:memo || 'Transfer', type:'TransferOut', amount:-amount, status:'Posted' });
  const account = state.accounts.find(a => a.id === document.getElementById('fromAccount').value);
  if(account){ account.availableBalance -= amount; account.currentBalance -= amount; }
  renderAccounts(); renderTransactions(); showToast('Transfer posted', 'Real-time transaction alert sent.', 'success');
});
document.getElementById('demoLoginBtn').addEventListener('click', async () => {
  showToast('Biometric/passkey demo', 'Production uses platform biometrics through WebAuthn/passkeys or native mobile APIs before JWT issuance.', 'primary');
});
document.getElementById('contrastBtn').addEventListener('click', () => document.body.classList.toggle('high-contrast'));
document.getElementById('lowBalance').addEventListener('input', event => document.getElementById('lowBalanceValue').textContent = money(event.target.value));
renderAccounts(); renderTransactions(); renderAml(); loadAccountsFromApi();
