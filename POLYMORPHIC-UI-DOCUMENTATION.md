# Polymorphic UI System Documentation

## Table of Contents
1. [Introduction](#introduction)
2. [Design Tokens](#design-tokens)
3. [Components](#components)
4. [Utilities](#utilities)
5. [JavaScript API](#javascript-api)
6. [Usage Examples](#usage-examples)

---

## Introduction

The Polymorphic UI System is a modern, modular CSS framework built specifically for the SSIS Analytics Dashboard. It provides a comprehensive set of design tokens, components, and utilities that enable consistent, maintainable, and scalable UI development.

### Key Features
- **Design Tokens**: CSS custom properties for colors, spacing, typography, and more
- **Polymorphic Components**: Flexible components with multiple variants and states
- **Utility Classes**: Comprehensive utility system for rapid development
- **JavaScript Utilities**: Reusable functions for common UI patterns
- **Dark Mode Support**: Built-in dark mode with prefers-color-scheme
- **Accessibility**: ARIA-compliant and keyboard navigable
- **Performance**: Optimized CSS with minimal specificity

### File Structure
```
wwwroot/css/
├── design-tokens.css    # CSS custom properties (colors, spacing, etc.)
├── components.css       # Component styles (cards, buttons, etc.)
├── utilities.css        # Utility classes (spacing, typography, etc.)
└── site.css            # Main stylesheet (imports all above)

wwwroot/js/
├── polymorphic-ui.js   # JavaScript utilities and component behaviors
└── site.js             # Application-specific JavaScript
```

---

## Design Tokens

Design tokens are defined as CSS custom properties in `:root` and can be used throughout your stylesheets.

### Color Palette

#### Primary Colors
```css
--color-primary-50 to --color-primary-900  /* Blue shades */
--color-primary                             /* Default: #2563eb */
```

#### Semantic Colors
```css
--color-success     /* #16a34a - Green */
--color-danger      /* #dc2626 - Red */
--color-warning     /* #f59e0b - Amber */
--color-info        /* #0ea5e9 - Sky blue */
```

#### Neutral Colors
```css
--color-gray-50 to --color-gray-900
--color-background              /* Page background */
--color-text                    /* Primary text color */
--color-text-secondary          /* Secondary text */
--color-border                  /* Default border color */
```

### Spacing Scale
```css
--space-0: 0
--space-1: 0.25rem   /* 4px */
--space-2: 0.5rem    /* 8px */
--space-3: 0.75rem   /* 12px */
--space-4: 1rem      /* 16px */
--space-5: 1.25rem   /* 20px */
--space-6: 1.5rem    /* 24px */
--space-8: 2rem      /* 32px */
--space-10: 2.5rem   /* 40px */
--space-12: 3rem     /* 48px */
```

### Typography
```css
/* Font Sizes */
--font-size-xs: 0.75rem      /* 12px */
--font-size-sm: 0.875rem     /* 14px */
--font-size-base: 1rem       /* 16px */
--font-size-lg: 1.125rem     /* 18px */
--font-size-xl: 1.25rem      /* 20px */
--font-size-2xl: 1.5rem      /* 24px */

/* Font Weights */
--font-weight-light: 300
--font-weight-normal: 400
--font-weight-medium: 500
--font-weight-semibold: 600
--font-weight-bold: 700
```

### Shadows
```css
--shadow-xs    /* Subtle shadow */
--shadow-sm    /* Small shadow */
--shadow-base  /* Default shadow */
--shadow-md    /* Medium shadow */
--shadow-lg    /* Large shadow */
--shadow-xl    /* Extra large shadow */
```

### Border Radius
```css
--radius-sm: 0.25rem     /* 4px */
--radius-base: 0.375rem  /* 6px */
--radius-md: 0.5rem      /* 8px */
--radius-lg: 0.75rem     /* 12px */
--radius-xl: 1rem        /* 16px */
--radius-full: 9999px    /* Pill shape */
```

---

## Components

### Card

Cards are flexible content containers with multiple variants.

#### Basic Card
```html
<div class="card">
  <div class="card__header">
    <h5 class="card__title">Card Title</h5>
    <p class="card__subtitle">Optional subtitle</p>
  </div>
  <div class="card__body">
    Card content goes here
  </div>
  <div class="card__footer">
    Footer content
  </div>
</div>
```

#### Card Variants
```html
<!-- Elevated card with shadow -->
<div class="card card--elevated">...</div>

<!-- Flat card (no shadow) -->
<div class="card card--flat">...</div>

<!-- Bordered card -->
<div class="card card--bordered">...</div>

<!-- Color variants -->
<div class="card card--primary">...</div>
<div class="card card--success">...</div>
<div class="card card--danger">...</div>
<div class="card card--warning">...</div>
<div class="card card--info">...</div>

<!-- Interactive card (hover effect) -->
<div class="card card--interactive">...</div>

<!-- Loading state -->
<div class="card card--loading">
  <div class="spinner-overlay">
    <div class="spinner"></div>
  </div>
  ...
</div>
```

### Button

Buttons come in multiple variants, sizes, and states.

#### Basic Button
```html
<button class="btn btn--primary">Primary Button</button>
```

#### Button Variants
```html
<!-- Solid buttons -->
<button class="btn btn--primary">Primary</button>
<button class="btn btn--success">Success</button>
<button class="btn btn--danger">Danger</button>
<button class="btn btn--warning">Warning</button>
<button class="btn btn--info">Info</button>
<button class="btn btn--secondary">Secondary</button>

<!-- Outline buttons -->
<button class="btn btn--outline-primary">Outline Primary</button>
<button class="btn btn--outline-success">Outline Success</button>
<button class="btn btn--outline-danger">Outline Danger</button>

<!-- Ghost button -->
<button class="btn btn--ghost">Ghost Button</button>
```

#### Button Sizes
```html
<button class="btn btn--primary btn--sm">Small</button>
<button class="btn btn--primary btn--md">Medium (default)</button>
<button class="btn btn--primary btn--lg">Large</button>
<button class="btn btn--primary btn--xl">Extra Large</button>
```

#### Button with Icon
```html
<button class="btn btn--primary">
  <i class="bi bi-download"></i>
  Download
</button>
```

#### Button States
```html
<!-- Disabled -->
<button class="btn btn--primary" disabled>Disabled</button>

<!-- Loading -->
<button class="btn btn--primary btn--loading">Loading</button>
```

#### Button Group
```html
<div class="btn-group">
  <button class="btn btn--primary">Left</button>
  <button class="btn btn--primary">Middle</button>
  <button class="btn btn--primary">Right</button>
</div>

<!-- Attached buttons -->
<div class="btn-group btn-group--attached">
  <button class="btn btn--primary">Left</button>
  <button class="btn btn--primary">Middle</button>
  <button class="btn btn--primary">Right</button>
</div>
```

### Badge

Small status indicators and labels.

```html
<!-- Basic badges -->
<span class="badge badge--primary">Primary</span>
<span class="badge badge--success">Success</span>
<span class="badge badge--danger">Danger</span>
<span class="badge badge--warning">Warning</span>
<span class="badge badge--info">Info</span>
<span class="badge badge--secondary">Secondary</span>

<!-- Badge sizes -->
<span class="badge badge--success badge--sm">Small</span>
<span class="badge badge--success">Default</span>
<span class="badge badge--success badge--lg">Large</span>

<!-- Pill badge -->
<span class="badge badge--primary badge--pill">Pill Badge</span>

<!-- Badge with icon -->
<span class="badge badge--success">
  <i class="bi bi-check-circle"></i> Completed
</span>
```

### Alert

Contextual feedback messages.

```html
<!-- Success alert -->
<div class="alert alert--success">
  <div class="alert__title">Success!</div>
  <div>Your operation completed successfully.</div>
  <button type="button" class="alert__close">
    <i class="bi bi-x-lg"></i>
  </button>
</div>

<!-- Danger alert -->
<div class="alert alert--danger">
  <div class="alert__title">Error!</div>
  <div>Something went wrong. Please try again.</div>
</div>

<!-- Warning alert -->
<div class="alert alert--warning">
  <div class="alert__title">Warning!</div>
  <div>This action cannot be undone.</div>
</div>

<!-- Info alert -->
<div class="alert alert--info">
  <div class="alert__title">Info</div>
  <div>Here's some helpful information.</div>
</div>
```

### Table

Responsive data tables with variants.

```html
<div class="table-container">
  <table class="table">
    <thead>
      <tr>
        <th>Name</th>
        <th>Status</th>
        <th>Date</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>John Doe</td>
        <td><span class="badge badge--success">Active</span></td>
        <td>2025-01-15</td>
      </tr>
    </tbody>
  </table>
</div>
```

#### Table Variants
```html
<!-- Striped table -->
<table class="table table--striped">...</table>

<!-- Hoverable rows -->
<table class="table table--hover">...</table>

<!-- Bordered table -->
<table class="table table--bordered">...</table>

<!-- Compact table -->
<table class="table table--compact">...</table>

<!-- Sticky header -->
<table class="table table--sticky">...</table>
```

### Form Components

#### Input
```html
<div class="form-group">
  <label class="form-label" for="username">Username</label>
  <input type="text" id="username" class="form-control" placeholder="Enter username">
  <div class="form-text">Choose a unique username</div>
</div>

<!-- Input sizes -->
<input type="text" class="form-control form-control--sm">
<input type="text" class="form-control">
<input type="text" class="form-control form-control--lg">
```

#### Select
```html
<div class="form-group">
  <label class="form-label" for="status">Status</label>
  <select id="status" class="form-select">
    <option value="">Choose...</option>
    <option value="active">Active</option>
    <option value="inactive">Inactive</option>
  </select>
</div>
```

#### Error State
```html
<div class="form-group">
  <label class="form-label" for="email">Email</label>
  <input type="email" id="email" class="form-control border-danger">
  <div class="form-error">Please enter a valid email address</div>
</div>
```

### Progress Bar

```html
<div class="progress">
  <div class="progress__bar" style="width: 75%">75%</div>
</div>

<!-- Color variants -->
<div class="progress">
  <div class="progress__bar progress__bar--success" style="width: 90%">90%</div>
</div>
<div class="progress">
  <div class="progress__bar progress__bar--danger" style="width: 25%">25%</div>
</div>
```

### Spinner/Loader

```html
<!-- Default spinner -->
<div class="spinner"></div>

<!-- Spinner sizes -->
<div class="spinner spinner--sm"></div>
<div class="spinner spinner--lg"></div>
<div class="spinner spinner--xl"></div>

<!-- Spinner overlay (for cards) -->
<div class="card">
  <div class="spinner-overlay">
    <div class="spinner"></div>
  </div>
  <div class="card__body">Content</div>
</div>
```

### Timeline

```html
<div class="timeline">
  <div class="timeline__item timeline__item--success">
    <h6>Package Completed</h6>
    <p class="text-muted">Execution finished successfully</p>
    <small>2025-01-15 10:30:00</small>
  </div>
  <div class="timeline__item timeline__item--danger">
    <h6>Package Failed</h6>
    <p class="text-muted">Error occurred during execution</p>
    <small>2025-01-15 09:15:00</small>
  </div>
</div>
```

### Navbar

```html
<nav class="navbar">
  <a href="/" class="navbar__brand">
    <i class="bi bi-graph-up"></i> SSIS Analytics
  </a>
  <ul class="navbar__nav">
    <li>
      <a href="/dashboard" class="navbar__link navbar__link--active">Dashboard</a>
    </li>
    <li>
      <a href="/reports" class="navbar__link">Reports</a>
    </li>
  </ul>
</nav>
```

---

## Utilities

### Display
```html
<div class="d-none">Hidden</div>
<div class="d-block">Block</div>
<div class="d-inline">Inline</div>
<div class="d-inline-block">Inline Block</div>
<div class="d-flex">Flex</div>
<div class="d-grid">Grid</div>
```

### Flexbox
```html
<div class="d-flex justify-content-center align-items-center gap-4">
  <div>Item 1</div>
  <div>Item 2</div>
</div>

<!-- Flex direction -->
<div class="d-flex flex-column">...</div>
<div class="d-flex flex-row">...</div>

<!-- Justify content -->
<div class="d-flex justify-content-start">...</div>
<div class="d-flex justify-content-end">...</div>
<div class="d-flex justify-content-center">...</div>
<div class="d-flex justify-content-between">...</div>

<!-- Align items -->
<div class="d-flex align-items-start">...</div>
<div class="d-flex align-items-center">...</div>
<div class="d-flex align-items-end">...</div>

<!-- Gap -->
<div class="d-flex gap-2">...</div>
<div class="d-flex gap-4">...</div>
```

### Spacing
```html
<!-- Margin -->
<div class="m-4">Margin all sides</div>
<div class="mt-4">Margin top</div>
<div class="mb-4">Margin bottom</div>
<div class="mx-4">Margin horizontal</div>
<div class="my-4">Margin vertical</div>

<!-- Padding -->
<div class="p-4">Padding all sides</div>
<div class="pt-4">Padding top</div>
<div class="pb-4">Padding bottom</div>
<div class="px-4">Padding horizontal</div>
<div class="py-4">Padding vertical</div>
```

### Typography
```html
<!-- Font sizes -->
<p class="text-xs">Extra small text</p>
<p class="text-sm">Small text</p>
<p class="text-base">Base text</p>
<p class="text-lg">Large text</p>
<p class="text-xl">Extra large text</p>

<!-- Font weights -->
<p class="font-light">Light</p>
<p class="font-normal">Normal</p>
<p class="font-medium">Medium</p>
<p class="font-semibold">Semibold</p>
<p class="font-bold">Bold</p>

<!-- Text alignment -->
<p class="text-left">Left aligned</p>
<p class="text-center">Center aligned</p>
<p class="text-right">Right aligned</p>

<!-- Text transform -->
<p class="uppercase">UPPERCASE</p>
<p class="lowercase">lowercase</p>
<p class="capitalize">Capitalized</p>

<!-- Text truncation -->
<p class="truncate">This text will be truncated with ellipsis...</p>
```

### Colors
```html
<!-- Text colors -->
<p class="text-primary">Primary text</p>
<p class="text-success">Success text</p>
<p class="text-danger">Danger text</p>
<p class="text-warning">Warning text</p>
<p class="text-muted">Muted text</p>

<!-- Background colors -->
<div class="bg-primary">Primary background</div>
<div class="bg-success">Success background</div>
<div class="bg-danger">Danger background</div>
<div class="bg-light">Light background</div>
```

### Borders
```html
<!-- Border -->
<div class="border">All borders</div>
<div class="border-t">Top border</div>
<div class="border-b">Bottom border</div>

<!-- Border colors -->
<div class="border border-primary">Primary border</div>
<div class="border border-success">Success border</div>

<!-- Border radius -->
<div class="rounded">Rounded</div>
<div class="rounded-lg">Large rounded</div>
<div class="rounded-full">Pill shape</div>
```

### Shadows
```html
<div class="shadow-sm">Small shadow</div>
<div class="shadow">Default shadow</div>
<div class="shadow-lg">Large shadow</div>
<div class="shadow-xl">Extra large shadow</div>
```

### Width & Height
```html
<div class="w-full">Full width</div>
<div class="h-full">Full height</div>
<div class="max-w-lg">Max width large</div>
<div class="min-h-screen">Minimum height 100vh</div>
```

---

## JavaScript API

### Alert

```javascript
// Show an alert
PolymorphicUI.Alert.show({
  type: 'success',           // success, danger, warning, info
  title: 'Success!',
  message: 'Operation completed successfully',
  dismissible: true,
  duration: 5000,            // Auto-dismiss after 5 seconds (0 = no auto-dismiss)
  container: document.body
});

// Dismiss an alert
PolymorphicUI.Alert.dismiss(alertElement);
```

### Modal

```javascript
// Show a modal
const modal = PolymorphicUI.Modal.show({
  title: 'Confirm Action',
  content: '<p>Are you sure you want to delete this item?</p>',
  size: 'md',                // sm, md, lg, xl
  closeOnBackdrop: true,
  buttons: [
    {
      label: 'Cancel',
      className: 'btn--secondary',
      action: 'close'
    },
    {
      label: 'Delete',
      className: 'btn--danger',
      icon: 'bi bi-trash',
      onClick: () => {
        console.log('Item deleted');
      },
      action: 'close'
    }
  ]
});

// Close a modal
PolymorphicUI.Modal.close(modal);
```

### Loading States

```javascript
// Show loading state on a card
const card = document.querySelector('.card');
PolymorphicUI.Loading.show(card);

// Hide loading state
PolymorphicUI.Loading.hide(card);

// Set button loading state
const button = document.querySelector('.btn');
PolymorphicUI.Loading.button(button, true);  // Start loading
PolymorphicUI.Loading.button(button, false); // Stop loading
```

### Toast Notifications

```javascript
// Show a toast notification
PolymorphicUI.Toast.show({
  type: 'success',
  title: 'Success',
  message: 'Data saved successfully',
  duration: 3000
});

// Show different types
PolymorphicUI.Toast.show({
  type: 'danger',
  message: 'Error occurred',
  duration: 5000
});
```

### Table Utilities

```javascript
// Make table sortable
const table = document.querySelector('.table');
PolymorphicUI.Table.makeSortable(table);

// Filter table rows
PolymorphicUI.Table.filter(table, 'search query');
```

### Form Validation

```javascript
// Validate a form
const form = document.querySelector('form');
const isValid = PolymorphicUI.Form.validate(form);

if (isValid) {
  // Submit form
}
```

### Utilities

```javascript
// Debounce function
const debouncedSearch = PolymorphicUI.Utils.debounce((query) => {
  console.log('Searching for:', query);
}, 300);

// Throttle function
const throttledScroll = PolymorphicUI.Utils.throttle(() => {
  console.log('Scrolling...');
}, 100);

// Copy to clipboard
PolymorphicUI.Utils.copyToClipboard('Text to copy');

// Format number
PolymorphicUI.Utils.formatNumber(1234567); // "1,234,567"
```

---

## Usage Examples

### Dashboard Metrics Card

```html
<div class="card card--elevated position-relative">
  <div class="spinner-overlay d-none">
    <div class="spinner"></div>
  </div>
  <div class="card__body">
    <h6 class="card__subtitle text-muted mb-2">Total Executions</h6>
    <h2 class="display-4 text-primary mb-2">1,234</h2>
    <p class="text-muted mb-0">Last 30 days</p>
  </div>
</div>
```

### Filter Section

```html
<div class="card mb-4">
  <div class="card__header">
    <h5 class="card__title">
      <i class="bi bi-funnel"></i> Filters
    </h5>
  </div>
  <div class="card__body">
    <div class="row g-3">
      <div class="col-md-3">
        <label class="form-label">Status</label>
        <select class="form-select">
          <option value="">All Statuses</option>
          <option value="success">Success</option>
          <option value="failed">Failed</option>
        </select>
      </div>
      <div class="col-md-3">
        <label class="form-label">Date Range</label>
        <select class="form-select">
          <option value="7">Last 7 Days</option>
          <option value="30">Last 30 Days</option>
        </select>
      </div>
      <div class="col-md-4">
        <label class="form-label">Search</label>
        <input type="text" class="form-control" placeholder="Search packages...">
      </div>
      <div class="col-md-2 d-flex align-items-end">
        <button class="btn btn--primary w-full">
          <i class="bi bi-search"></i> Search
        </button>
      </div>
    </div>
  </div>
</div>
```

### Data Table with Actions

```html
<div class="card">
  <div class="card__header">
    <h5 class="card__title">Recent Executions</h5>
  </div>
  <div class="card__body p-0">
    <div class="table-container">
      <table class="table table--hover table--striped">
        <thead>
          <tr>
            <th>ID</th>
            <th>Package</th>
            <th>Status</th>
            <th>Duration</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td><strong>12345</strong></td>
            <td>ETL_Process.dtsx</td>
            <td><span class="badge badge--success">Succeeded</span></td>
            <td>2m 35s</td>
            <td>
              <div class="btn-group btn-group--attached">
                <button class="btn btn--sm btn--outline-primary">
                  <i class="bi bi-eye"></i> View
                </button>
                <button class="btn btn--sm btn--outline-secondary">
                  <i class="bi bi-download"></i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</div>
```

### Responsive Grid Layout

```html
<div class="row g-4 mb-4">
  <div class="col-12 col-md-6 col-lg-3">
    <div class="card">...</div>
  </div>
  <div class="col-12 col-md-6 col-lg-3">
    <div class="card">...</div>
  </div>
  <div class="col-12 col-md-6 col-lg-3">
    <div class="card">...</div>
  </div>
  <div class="col-12 col-md-6 col-lg-3">
    <div class="card">...</div>
  </div>
</div>
```

---

## Best Practices

1. **Use Design Tokens**: Always use CSS custom properties instead of hardcoded values
2. **Component Composition**: Build complex UIs by composing simple components
3. **Utility-First**: Use utility classes for one-off styling needs
4. **Semantic HTML**: Use appropriate HTML elements for accessibility
5. **Responsive Design**: Use responsive utilities and test on multiple devices
6. **Performance**: Minimize custom CSS and leverage the utility system
7. **Consistency**: Follow the established patterns and naming conventions

---

## Browser Support

- Chrome (last 2 versions)
- Firefox (last 2 versions)
- Safari (last 2 versions)
- Edge (last 2 versions)

---

## License

This Polymorphic UI System is proprietary to the SSIS Analytics Dashboard project.

---

**Version**: 1.0.0  
**Last Updated**: November 12, 2025
