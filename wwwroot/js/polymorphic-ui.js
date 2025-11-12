/**
 * ==========================================
 * POLYMORPHIC UI - JAVASCRIPT UTILITIES
 * ==========================================
 * Reusable component behaviors and state management
 */

const PolymorphicUI = (function() {
  'use strict';

  // ==========================================
  // ALERT COMPONENT
  // ==========================================
  const Alert = {
    /**
     * Create and show an alert
     * @param {Object} config - Alert configuration
     * @param {string} config.type - Alert type: success, danger, warning, info
     * @param {string} config.title - Alert title
     * @param {string} config.message - Alert message
     * @param {boolean} config.dismissible - Whether alert can be dismissed
     * @param {number} config.duration - Auto-dismiss duration in ms (0 = no auto-dismiss)
     * @param {HTMLElement} config.container - Container to append alert to
     */
    show: function(config) {
      const {
        type = 'info',
        title = '',
        message = '',
        dismissible = true,
        duration = 0,
        container = document.body
      } = config;

      const alert = document.createElement('div');
      alert.className = `alert alert--${type} animate-slide-down`;
      alert.setAttribute('role', 'alert');
      
      let html = '';
      
      if (title) {
        html += `<div class="alert__title">${title}</div>`;
      }
      
      html += `<div>${message}</div>`;
      
      if (dismissible) {
        html += `
          <button type="button" class="alert__close" aria-label="Close">
            <i class="bi bi-x-lg"></i>
          </button>
        `;
      }
      
      alert.innerHTML = html;
      container.insertBefore(alert, container.firstChild);
      
      // Handle dismiss button
      if (dismissible) {
        const closeBtn = alert.querySelector('.alert__close');
        closeBtn.addEventListener('click', () => this.dismiss(alert));
      }
      
      // Auto-dismiss
      if (duration > 0) {
        setTimeout(() => this.dismiss(alert), duration);
      }
      
      return alert;
    },

    /**
     * Dismiss an alert
     * @param {HTMLElement} alert - Alert element to dismiss
     */
    dismiss: function(alert) {
      alert.style.opacity = '0';
      alert.style.transform = 'translateY(-20px)';
      setTimeout(() => alert.remove(), 300);
    }
  };

  // ==========================================
  // MODAL COMPONENT
  // ==========================================
  const Modal = {
    /**
     * Create and show a modal
     * @param {Object} config - Modal configuration
     * @param {string} config.title - Modal title
     * @param {string} config.content - Modal content (HTML)
     * @param {Array} config.buttons - Array of button configurations
     * @param {boolean} config.closeOnBackdrop - Close modal when clicking backdrop
     */
    show: function(config) {
      const {
        title = '',
        content = '',
        buttons = [],
        closeOnBackdrop = true,
        size = 'md' // sm, md, lg, xl
      } = config;

      const modal = document.createElement('div');
      modal.className = 'modal modal--active';
      
      let maxWidth = '512px';
      if (size === 'sm') maxWidth = '384px';
      if (size === 'lg') maxWidth = '672px';
      if (size === 'xl') maxWidth = '896px';
      
      modal.innerHTML = `
        <div class="modal__backdrop"></div>
        <div class="modal__content" style="max-width: ${maxWidth}; width: 90%;">
          <div class="modal__header">
            <h3 class="modal__title">${title}</h3>
          </div>
          <div class="modal__body">
            ${content}
          </div>
          ${buttons.length > 0 ? `
            <div class="modal__footer">
              ${buttons.map(btn => `
                <button type="button" 
                        class="btn ${btn.className || 'btn--secondary'}" 
                        data-action="${btn.action || 'close'}">
                  ${btn.icon ? `<i class="${btn.icon}"></i>` : ''}
                  ${btn.label}
                </button>
              `).join('')}
            </div>
          ` : ''}
        </div>
      `;
      
      document.body.appendChild(modal);
      document.body.style.overflow = 'hidden';
      
      // Handle backdrop click
      if (closeOnBackdrop) {
        const backdrop = modal.querySelector('.modal__backdrop');
        backdrop.addEventListener('click', () => this.close(modal));
      }
      
      // Handle button clicks
      buttons.forEach((btn, index) => {
        const buttonEl = modal.querySelectorAll('.modal__footer button')[index];
        if (buttonEl) {
          buttonEl.addEventListener('click', () => {
            if (btn.onClick) {
              btn.onClick();
            }
            if (btn.action === 'close') {
              this.close(modal);
            }
          });
        }
      });
      
      return modal;
    },

    /**
     * Close a modal
     * @param {HTMLElement} modal - Modal element to close
     */
    close: function(modal) {
      modal.classList.remove('modal--active');
      document.body.style.overflow = '';
      setTimeout(() => modal.remove(), 300);
    }
  };

  // ==========================================
  // LOADING STATE MANAGEMENT
  // ==========================================
  const Loading = {
    /**
     * Show loading state on an element
     * @param {HTMLElement} element - Element to show loading state
     */
    show: function(element) {
      if (!element) return;
      
      element.classList.add('card--loading');
      const spinner = element.querySelector('.spinner-overlay');
      if (spinner) {
        spinner.classList.remove('d-none');
      }
    },

    /**
     * Hide loading state on an element
     * @param {HTMLElement} element - Element to hide loading state
     */
    hide: function(element) {
      if (!element) return;
      
      element.classList.remove('card--loading');
      const spinner = element.querySelector('.spinner-overlay');
      if (spinner) {
        spinner.classList.add('d-none');
      }
      
      // Add pulse animation
      element.classList.add('pulse');
      setTimeout(() => element.classList.remove('pulse'), 1000);
    },

    /**
     * Set button loading state
     * @param {HTMLElement} button - Button element
     * @param {boolean} isLoading - Loading state
     */
    button: function(button, isLoading) {
      if (!button) return;
      
      if (isLoading) {
        button.classList.add('btn--loading');
        button.disabled = true;
      } else {
        button.classList.remove('btn--loading');
        button.disabled = false;
      }
    }
  };

  // ==========================================
  // TOAST NOTIFICATIONS
  // ==========================================
  const Toast = {
    container: null,

    /**
     * Initialize toast container
     */
    init: function() {
      if (!this.container) {
        this.container = document.createElement('div');
        this.container.className = 'toast-container';
        this.container.style.cssText = `
          position: fixed;
          top: 20px;
          right: 20px;
          z-index: 9999;
          display: flex;
          flex-direction: column;
          gap: var(--space-2);
          max-width: 400px;
        `;
        document.body.appendChild(this.container);
      }
    },

    /**
     * Show a toast notification
     * @param {Object} config - Toast configuration
     */
    show: function(config) {
      this.init();
      
      const {
        type = 'info',
        title = '',
        message = '',
        duration = 3000,
        icon = null
      } = config;

      const toast = document.createElement('div');
      toast.className = `card card--${type} animate-slide-in-right shadow-lg`;
      toast.style.cssText = `
        min-width: 300px;
        animation: slideInRight 0.3s ease-out;
      `;
      
      const iconMap = {
        success: 'bi-check-circle-fill',
        danger: 'bi-x-circle-fill',
        warning: 'bi-exclamation-triangle-fill',
        info: 'bi-info-circle-fill'
      };
      
      const toastIcon = icon || iconMap[type];
      
      toast.innerHTML = `
        <div class="card__body" style="padding: var(--space-3);">
          <div class="d-flex align-items-start gap-3">
            <i class="bi ${toastIcon} text-${type}" style="font-size: var(--font-size-xl);"></i>
            <div class="flex-1">
              ${title ? `<div class="font-semibold mb-1">${title}</div>` : ''}
              <div class="text-sm">${message}</div>
            </div>
            <button type="button" class="btn btn--ghost btn--sm" style="padding: 0; min-width: auto;">
              <i class="bi bi-x-lg"></i>
            </button>
          </div>
        </div>
      `;
      
      this.container.appendChild(toast);
      
      // Close button
      const closeBtn = toast.querySelector('button');
      closeBtn.addEventListener('click', () => this.dismiss(toast));
      
      // Auto-dismiss
      if (duration > 0) {
        setTimeout(() => this.dismiss(toast), duration);
      }
      
      return toast;
    },

    /**
     * Dismiss a toast
     * @param {HTMLElement} toast - Toast element
     */
    dismiss: function(toast) {
      toast.style.opacity = '0';
      toast.style.transform = 'translateX(100%)';
      setTimeout(() => toast.remove(), 300);
    }
  };

  // ==========================================
  // TABLE UTILITIES
  // ==========================================
  const Table = {
    /**
     * Make table sortable
     * @param {HTMLElement} table - Table element
     */
    makeSortable: function(table) {
      const headers = table.querySelectorAll('thead th');
      
      headers.forEach((header, index) => {
        header.style.cursor = 'pointer';
        header.innerHTML += ' <i class="bi bi-arrow-down-up text-muted" style="font-size: 0.75rem;"></i>';
        
        header.addEventListener('click', () => {
          const tbody = table.querySelector('tbody');
          const rows = Array.from(tbody.querySelectorAll('tr'));
          const isAscending = header.classList.contains('sorted-asc');
          
          // Remove all sort classes
          headers.forEach(h => {
            h.classList.remove('sorted-asc', 'sorted-desc');
            const icon = h.querySelector('i');
            if (icon) {
              icon.className = 'bi bi-arrow-down-up text-muted';
            }
          });
          
          // Sort rows
          rows.sort((a, b) => {
            const aVal = a.cells[index].textContent.trim();
            const bVal = b.cells[index].textContent.trim();
            
            const aNum = parseFloat(aVal);
            const bNum = parseFloat(bVal);
            
            if (!isNaN(aNum) && !isNaN(bNum)) {
              return isAscending ? bNum - aNum : aNum - bNum;
            }
            
            return isAscending ? bVal.localeCompare(aVal) : aVal.localeCompare(bVal);
          });
          
          // Update classes and icon
          header.classList.add(isAscending ? 'sorted-desc' : 'sorted-asc');
          const icon = header.querySelector('i');
          if (icon) {
            icon.className = `bi bi-arrow-${isAscending ? 'down' : 'up'} text-primary`;
          }
          
          // Reorder rows
          rows.forEach(row => tbody.appendChild(row));
        });
      });
    },

    /**
     * Filter table rows
     * @param {HTMLElement} table - Table element
     * @param {string} query - Search query
     */
    filter: function(table, query) {
      const rows = table.querySelectorAll('tbody tr');
      const searchTerm = query.toLowerCase();
      
      rows.forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(searchTerm) ? '' : 'none';
      });
    }
  };

  // ==========================================
  // FORM VALIDATION
  // ==========================================
  const Form = {
    /**
     * Validate a form
     * @param {HTMLFormElement} form - Form element
     * @returns {boolean} - Whether form is valid
     */
    validate: function(form) {
      let isValid = true;
      const inputs = form.querySelectorAll('.form-control, .form-select');
      
      inputs.forEach(input => {
        const error = input.parentElement.querySelector('.form-error');
        
        // Clear previous errors
        if (error) error.remove();
        input.classList.remove('border-danger');
        
        // Check required
        if (input.hasAttribute('required') && !input.value.trim()) {
          this.showError(input, 'This field is required');
          isValid = false;
        }
        
        // Check email
        if (input.type === 'email' && input.value) {
          const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
          if (!emailRegex.test(input.value)) {
            this.showError(input, 'Please enter a valid email address');
            isValid = false;
          }
        }
        
        // Check min length
        const minLength = input.getAttribute('minlength');
        if (minLength && input.value.length < parseInt(minLength)) {
          this.showError(input, `Minimum length is ${minLength} characters`);
          isValid = false;
        }
      });
      
      return isValid;
    },

    /**
     * Show validation error
     * @param {HTMLElement} input - Input element
     * @param {string} message - Error message
     */
    showError: function(input, message) {
      input.classList.add('border-danger');
      
      const error = document.createElement('div');
      error.className = 'form-error';
      error.textContent = message;
      
      input.parentElement.appendChild(error);
    }
  };

  // ==========================================
  // DEBOUNCE & THROTTLE UTILITIES
  // ==========================================
  const Utils = {
    /**
     * Debounce function
     * @param {Function} func - Function to debounce
     * @param {number} wait - Wait time in ms
     * @returns {Function} - Debounced function
     */
    debounce: function(func, wait) {
      let timeout;
      return function executedFunction(...args) {
        const later = () => {
          clearTimeout(timeout);
          func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
      };
    },

    /**
     * Throttle function
     * @param {Function} func - Function to throttle
     * @param {number} limit - Limit time in ms
     * @returns {Function} - Throttled function
     */
    throttle: function(func, limit) {
      let inThrottle;
      return function(...args) {
        if (!inThrottle) {
          func.apply(this, args);
          inThrottle = true;
          setTimeout(() => inThrottle = false, limit);
        }
      };
    },

    /**
     * Copy text to clipboard
     * @param {string} text - Text to copy
     */
    copyToClipboard: async function(text) {
      try {
        await navigator.clipboard.writeText(text);
        Toast.show({
          type: 'success',
          message: 'Copied to clipboard',
          duration: 2000
        });
      } catch (err) {
        console.error('Failed to copy:', err);
        Toast.show({
          type: 'danger',
          message: 'Failed to copy to clipboard',
          duration: 2000
        });
      }
    },

    /**
     * Format number with commas
     * @param {number} num - Number to format
     * @returns {string} - Formatted number
     */
    formatNumber: function(num) {
      return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    }
  };

  // ==========================================
  // PUBLIC API
  // ==========================================
  return {
    Alert,
    Modal,
    Loading,
    Toast,
    Table,
    Form,
    Utils
  };

})();

// Make available globally
window.PolymorphicUI = PolymorphicUI;

// Initialize on DOM ready
document.addEventListener('DOMContentLoaded', function() {
  console.log('🎨 Polymorphic UI System Initialized');
});
