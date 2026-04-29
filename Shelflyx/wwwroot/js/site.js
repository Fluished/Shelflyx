// Auto-dismiss alerts
document.addEventListener('DOMContentLoaded', () => {
    const alerts = document.querySelectorAll('.alert:not(.fixed-alert)');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.transition = 'opacity .5s';
            alert.style.opacity = '0';
            setTimeout(() => alert.remove(), 500);
        }, 4000);
    });

    // Star rating hover effect
    const starLabels = document.querySelectorAll('.rate-form .star-label');
    starLabels.forEach((label, idx) => {
        label.addEventListener('mouseenter', () => {
            starLabels.forEach((l, i) => {
                l.querySelector('.star-input').style.color = i <= idx ? 'var(--gold)' : 'var(--border)';
            });
        });
        label.addEventListener('mouseleave', () => {
            starLabels.forEach(l => l.querySelector('.star-input').style.color = '');
        });
    });

    // Image preview for avatar
    const picUpload = document.getElementById('picUpload');
    const avatarPreview = document.getElementById('avatarPreview');
    if (picUpload && avatarPreview) {
        picUpload.addEventListener('change', e => {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = ev => avatarPreview.src = ev.target.result;
                reader.readAsDataURL(file);
            }
        });
    }
});
