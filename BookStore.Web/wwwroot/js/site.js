// Theme toggle persistence
(function(){
  const root = document.documentElement;
  const saved = localStorage.getItem('theme');
  if(saved){ root.setAttribute('data-theme', saved); }
  else if(!root.getAttribute('data-theme')){
    root.setAttribute('data-theme','light');
  }
  document.addEventListener('click', (e)=>{
    const btn = e.target.closest('[data-toggle-theme]');
    if(!btn) return;
    const cur = root.getAttribute('data-theme')==='light'?'dark':'light';
    root.setAttribute('data-theme', cur);
    localStorage.setItem('theme', cur);
  });
})();

// Init AOS (Animate on Scroll)
window.addEventListener('DOMContentLoaded', ()=>{
  if(window.AOS){ AOS.init({ duration: 700, once: true, offset: 80 }); }
});

// GSAP hero and cards
window.addEventListener('load', ()=>{
  if(window.gsap){
    const tl = gsap.timeline();
    tl.from('.navbar', {y:-30, opacity:0, duration:.6})
      .from('.hero .display-5', {y:20, opacity:0, duration:.6}, '-=.2')
      .from('.hero .lead', {y:20, opacity:0, duration:.6}, '-=.4');
    gsap.utils.toArray('.card').forEach((el)=>{
      el.addEventListener('mouseenter', ()=> gsap.to(el,{y:-4, duration:.25, overwrite:true}));
      el.addEventListener('mouseleave', ()=> gsap.to(el,{y:0, duration:.25, overwrite:true}));
    });
  }
});

// Toast helper
window.showToast = (msg, type='info') => {
  const wrap = document.getElementById('toastContainer');
  if(!wrap) return alert(msg);
  const el = document.createElement('div');
  el.className = 'toast align-items-center text-bg-' + (type==='success'?'success': type==='error'?'danger':'secondary') + ' toast-premium border-0';
  el.role = 'alert'; el.ariaLive = 'assertive'; el.ariaAtomic = 'true';
  el.innerHTML = `<div class="d-flex"><div class="toast-body">${msg}</div><button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button></div>`;
  wrap.appendChild(el);
  const t = new bootstrap.Toast(el, {delay:2500}); t.show();
  el.addEventListener('hidden.bs.toast', ()=> el.remove());
};

// Optimistic UI for cart/favorites
document.addEventListener('click', async (e)=>{
  const addCart = e.target.closest('[data-add-cart]');
  const addFav = e.target.closest('[data-add-fav]');
  if(addCart){
    e.preventDefault();
    const btn = addCart;
    btn.disabled = true;
    try{
      const {id, title, price} = btn.dataset;
      const resp = await fetch('/Cart/AddAjax', { method:'POST', headers:{'Content-Type':'application/x-www-form-urlencoded'}, body: new URLSearchParams({ id, title, price }) });
      const data = await resp.json();
      updateNavBadges({ cart: data.cartCount });
      showToast(data.message, 'success');
    }catch{ showToast('Sepete eklenemedi','error'); }
    finally{ btn.disabled=false; }
  }
  if(addFav){
    e.preventDefault();
    const btn = addFav;
    btn.disabled = true;
    try{
      const {bookId} = btn.dataset;
      const resp = await fetch('/Store/FavoriteAjax', { method:'POST', headers:{'Content-Type':'application/x-www-form-urlencoded'}, body: new URLSearchParams({ bookId }) });
      const data = await resp.json();
      updateNavBadges({ favorites: data.favoritesCount });
      showToast(data.message, 'success');
    }catch{ showToast('Favoriye eklenemedi','error'); }
    finally{ btn.disabled=false; }
  }
});

function updateNavBadges({cart, favorites}){
  if(typeof cart !== 'undefined'){
    const el = document.querySelector('[data-badge-cart]');
    if(el){ el.textContent = cart>0 ? cart : ''; el.classList.toggle('d-none', !(cart>0)); }
  }
  if(typeof favorites !== 'undefined'){
    const el = document.querySelector('[data-badge-fav]');
    if(el){ el.textContent = favorites>0 ? favorites : ''; el.classList.toggle('d-none', !(favorites>0)); }
  }
}
