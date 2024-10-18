const modal = document.getElementById('modal');
const modalBtn = document.getElementById('modalBtn');
const closeBtn = document.querySelector('.closeBtn');

function openModal() {
  modal.style.display = 'block';
}

function closeModal() {
  modal.style.display = 'none';
}


eventListener();
