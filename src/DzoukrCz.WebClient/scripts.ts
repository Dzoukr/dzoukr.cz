import './styles.css';

const toggleVisibility = (elementId: string): void => {
    const element = document.getElementById(elementId);
    if (!element) {
        console.warn(`Element with ID "${elementId}" not found.`);
        return;
    }
    element.classList.toggle('hidden');
};

document.addEventListener('DOMContentLoaded', () => {
    const button = document.getElementById('mobileMenuBtn');
    button?.addEventListener('click', () => {
        toggleVisibility('mobileMenu');
    });
});