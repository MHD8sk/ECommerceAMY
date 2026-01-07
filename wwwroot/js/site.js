// Creative Enhancements for ECommerceAMY

// Smooth scroll animations
document.addEventListener('DOMContentLoaded', function() {
    // Animate elements on scroll
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-fade-in');
            }
        });
    }, observerOptions);
    
    // Observe all feature cards and product cards
    document.querySelectorAll('.feature-card, .product-card').forEach(el => {
        observer.observe(el);
    });
    
    // Add floating animation to cart badge
    const cartBadge = document.querySelector('.cart-badge');
    if (cartBadge) {
        cartBadge.classList.add('floating-cart');
    }
});

// Enhanced theme toggle with smooth transitions
(function () {
	const key = 'theme';
	const saved = localStorage.getItem(key) || 'light';
	document.documentElement.setAttribute('data-bs-theme', saved);

	const btn = document.getElementById('themeToggle');
	if (btn) {
		btn.addEventListener('click', function () {
			const current = document.documentElement.getAttribute('data-bs-theme') || 'light';
			const next = current === 'light' ? 'dark' : 'light';
			document.documentElement.setAttribute('data-bs-theme', next);
			localStorage.setItem(key, next);
			
			// Add ripple effect
			createRipple(event, btn);
		});
	}
})();

// Ripple effect for buttons
function createRipple(event, button) {
    const ripple = document.createElement('span');
    ripple.classList.add('ripple');
    
    const rect = button.getBoundingClientRect();
    const size = Math.max(rect.width, rect.height);
    const x = event.clientX - rect.left - size / 2;
    const y = event.clientY - rect.top - size / 2;
    
    ripple.style.width = ripple.style.height = size + 'px';
    ripple.style.left = x + 'px';
    ripple.style.top = y + 'px';
    
    button.appendChild(ripple);
    
    setTimeout(() => {
        ripple.remove();
    }, 600);
}

// Add ripple to all buttons
document.addEventListener('click', function(e) {
    if (e.target.classList.contains('btn')) {
        createRipple(e, e.target);
    }
});

// Smooth scroll for anchor links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(anchor.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// Add CSS for ripple effect
const style = document.createElement('style');
style.textContent = `
    .ripple {
        position: absolute;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.5);
        transform: scale(0);
        animation: ripple-animation 0.6s ease-out;
        pointer-events: none;
    }
    
    @keyframes ripple-animation {
        to {
            transform: scale(4);
            opacity: 0;
        }
    }
    
    .animate-fade-in {
        animation: fade-in 0.6s ease-out;
    }
    
    @keyframes fade-in {
        from {
            opacity: 0;
            transform: translateY(20px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }
`;
document.head.appendChild(style);

