# Polymorphic UI - Quick Reference Cheat Sheet

## 🎨 Design Tokens (Most Used)

### Colors
```css
var(--color-primary)        /* #2563eb */
var(--color-success)        /* #16a34a */
var(--color-danger)         /* #dc2626 */
var(--color-warning)        /* #f59e0b */
var(--color-info)           /* #0ea5e9 */
var(--color-text)           /* Main text */
var(--color-text-secondary) /* Muted text */
var(--color-border)         /* Borders */
var(--color-background)     /* Page bg */
```

### Spacing
```css
var(--space-1)  /* 4px */
var(--space-2)  /* 8px */
var(--space-3)  /* 12px */
var(--space-4)  /* 16px */
var(--space-6)  /* 24px */
var(--space-8)  /* 32px */
```

---

## 🔘 Buttons

```html
<!-- Solid Buttons -->
<button class="btn btn--primary">Primary</button>
<button class="btn btn--success">Success</button>
<button class="btn btn--danger">Danger</button>

<!-- Outline Buttons -->
<button class="btn btn--outline-primary">Outline</button>

<!-- Sizes -->
<button class="btn btn--primary btn--sm">Small</button>
<button class="btn btn--primary btn--lg">Large</button>

<!-- With Icon -->
<button class="btn btn--primary">
  <i class="bi bi-download"></i> Download
</button>

<!-- States -->
<button class="btn btn--primary" disabled>Disabled</button>
<button class="btn btn--primary btn--loading">Loading</button>
```

---

## 🎴 Cards

```html
<!-- Basic Card -->
<div class="card">
  <div class="card__header">
    <h5 class="card__title">Title</h5>
  </div>
  <div class="card__body">Content</div>
</div>

<!-- Variants -->
<div class="card card--elevated">Elevated shadow</div>
<div class="card card--primary">Primary border</div>
<div class="card card--interactive">Hover effect</div>

<!-- With Loading -->
<div class="card card--loading">
  <div class="spinner-overlay">
    <div class="spinner"></div>
  </div>
  <div class="card__body">Loading...</div>
</div>
```

---

## 🏷️ Badges

```html
<span class="badge badge--primary">Primary</span>
<span class="badge badge--success">Success</span>
<span class="badge badge--danger">Danger</span>
<span class="badge badge--warning">Warning</span>

<!-- With Icon -->
<span class="badge badge--success">
  <i class="bi bi-check"></i> Done
</span>

<!-- Pill -->
<span class="badge badge--primary badge--pill">Pill</span>
```

---

## ⚠️ Alerts

```html
<div class="alert alert--success">
  <div class="alert__title">Success!</div>
  <div>Message here</div>
  <button class="alert__close">
    <i class="bi bi-x-lg"></i>
  </button>
</div>
```

**Types**: `alert--success`, `alert--danger`, `alert--warning`, `alert--info`

---

## 📊 Tables

```html
<div class="table-container">
  <table class="table table--hover table--striped">
    <thead>
      <tr>
        <th>Column 1</th>
        <th>Column 2</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>Data</td>
        <td>Data</td>
      </tr>
    </tbody>
  </table>
</div>
```

**Variants**: `table--hover`, `table--striped`, `table--bordered`, `table--compact`

---

## 📝 Forms

```html
<div class="form-group">
  <label class="form-label">Label</label>
  <input type="text" class="form-control" placeholder="Enter text">
  <div class="form-text">Help text</div>
</div>

<div class="form-group">
  <select class="form-select">
    <option>Option 1</option>
  </select>
</div>

<!-- Error State -->
<input class="form-control border-danger">
<div class="form-error">Error message</div>
```

---

## 🔄 Progress

```html
<div class="progress">
  <div class="progress__bar progress__bar--success" style="width: 75%">
    75%
  </div>
</div>
```

**Types**: `progress__bar--success`, `progress__bar--danger`, `progress__bar--warning`

---

## 🎯 Common Utilities

### Layout
```html
<div class="d-flex justify-content-between align-items-center gap-4">
<div class="d-grid grid-cols-3 gap-4">
```

### Spacing
```html
<div class="m-4">         <!-- margin all sides -->
<div class="mt-4 mb-4">   <!-- margin top/bottom -->
<div class="p-4">         <!-- padding all sides -->
<div class="px-4 py-2">   <!-- padding x/y -->
```

### Typography
```html
<p class="text-lg font-semibold text-primary">
<p class="text-sm text-muted">
<p class="text-center uppercase">
<p class="truncate">Long text...</p>
```

### Colors
```html
<div class="text-primary">Text color</div>
<div class="bg-success">Background</div>
<div class="border border-danger">Border</div>
```

### Width/Height
```html
<div class="w-full h-full">
<div class="max-w-lg">
<div class="min-h-screen">
```

### Position
```html
<div class="position-relative">
<div class="position-absolute top-0 right-0">
```

### Borders & Shadows
```html
<div class="rounded-lg shadow-md border border-primary">
```

---

## 🎬 JavaScript API

### Alerts
```javascript
PolymorphicUI.Alert.show({
  type: 'success',
  title: 'Success!',
  message: 'Done',
  dismissible: true,
  duration: 5000
});
```

### Toast
```javascript
PolymorphicUI.Toast.show({
  type: 'info',
  message: 'Notification',
  duration: 3000
});
```

### Modal
```javascript
PolymorphicUI.Modal.show({
  title: 'Confirm',
  content: '<p>Are you sure?</p>',
  buttons: [
    { label: 'Cancel', className: 'btn--secondary', action: 'close' },
    { label: 'OK', className: 'btn--primary', onClick: () => {...}, action: 'close' }
  ]
});
```

### Loading
```javascript
// Card loading
PolymorphicUI.Loading.show(element);
PolymorphicUI.Loading.hide(element);

// Button loading
PolymorphicUI.Loading.button(btn, true);  // start
PolymorphicUI.Loading.button(btn, false); // stop
```

### Table
```javascript
PolymorphicUI.Table.makeSortable(table);
PolymorphicUI.Table.filter(table, 'query');
```

### Form
```javascript
const isValid = PolymorphicUI.Form.validate(form);
```

### Utils
```javascript
const debounced = PolymorphicUI.Utils.debounce(fn, 300);
const throttled = PolymorphicUI.Utils.throttle(fn, 100);
PolymorphicUI.Utils.copyToClipboard('text');
PolymorphicUI.Utils.formatNumber(1234567); // "1,234,567"
```

---

## 📐 Grid System

```html
<div class="row g-4">
  <div class="col-12 col-md-6 col-lg-3">Column</div>
  <div class="col-12 col-md-6 col-lg-3">Column</div>
  <div class="col-12 col-md-6 col-lg-3">Column</div>
  <div class="col-12 col-md-6 col-lg-3">Column</div>
</div>
```

---

## 🎨 Common Patterns

### Metric Card
```html
<div class="card card--elevated">
  <div class="card__body">
    <h6 class="text-muted mb-2">Total Users</h6>
    <h2 class="text-4xl font-bold text-primary mb-1">1,234</h2>
    <p class="text-sm text-success">+12% from last month</p>
  </div>
</div>
```

### Action Card
```html
<div class="card card--interactive">
  <div class="card__header">
    <h5 class="card__title">Package Status</h5>
  </div>
  <div class="card__body">
    <div class="d-flex justify-content-between mb-3">
      <span>Progress</span>
      <span class="badge badge--success">Running</span>
    </div>
    <div class="progress">
      <div class="progress__bar" style="width: 65%">65%</div>
    </div>
  </div>
  <div class="card__footer">
    <button class="btn btn--primary btn--sm w-full">View Details</button>
  </div>
</div>
```

### Filter Bar
```html
<div class="card mb-4">
  <div class="card__body">
    <div class="row g-3">
      <div class="col-md-4">
        <input type="text" class="form-control" placeholder="Search...">
      </div>
      <div class="col-md-3">
        <select class="form-select">
          <option>Filter by...</option>
        </select>
      </div>
      <div class="col-md-2">
        <button class="btn btn--primary w-full">
          <i class="bi bi-search"></i> Search
        </button>
      </div>
    </div>
  </div>
</div>
```

### Data Row
```html
<div class="d-flex justify-content-between align-items-center p-3 border-b">
  <div>
    <h6 class="mb-1">Package Name</h6>
    <p class="text-sm text-muted mb-0">Description</p>
  </div>
  <div class="d-flex align-items-center gap-2">
    <span class="badge badge--success">Active</span>
    <button class="btn btn--sm btn--outline-primary">Edit</button>
  </div>
</div>
```

---

## 🎯 Quick Tips

1. **Use BEM**: `block`, `block__element`, `block--modifier`
2. **Design Tokens First**: `var(--color-primary)` not `#2563eb`
3. **Utility Classes**: For one-off styling
4. **Component Classes**: For reusable patterns
5. **Responsive**: Test on mobile, tablet, desktop
6. **Accessibility**: Use semantic HTML and ARIA

---

## 📱 Responsive Breakpoints

- **sm**: 640px
- **md**: 768px
- **lg**: 1024px
- **xl**: 1280px

```html
<div class="d-block md:d-flex lg:d-grid">
  Responsive!
</div>
```

---

## 🔗 Resources

- **Full Docs**: `POLYMORPHIC-UI-DOCUMENTATION.md`
- **Migration**: `MIGRATION-GUIDE.md`
- **Overview**: `POLYMORPHIC-UI-README.md`
- **Tokens**: `wwwroot/css/design-tokens.css`
- **Components**: `wwwroot/css/components.css`
- **Utilities**: `wwwroot/css/utilities.css`

---

**Print this and keep it handy! 📌**
