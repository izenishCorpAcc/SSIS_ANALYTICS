# Migration Guide: Converting to Polymorphic UI

This guide shows how to convert existing Bootstrap-based HTML/CSS to the Polymorphic UI System.

## Quick Reference: Class Mapping

### Bootstrap to Polymorphic UI

| Bootstrap Class | Polymorphic UI Equivalent |
|----------------|---------------------------|
| `btn btn-primary` | `btn btn--primary` |
| `btn btn-secondary` | `btn btn--secondary` |
| `btn btn-success` | `btn btn--success` |
| `btn btn-sm` | `btn btn--sm` |
| `badge bg-success` | `badge badge--success` |
| `alert alert-danger` | `alert alert--danger` |
| `card` | `card` (same) |
| `card-header` | `card__header` |
| `card-body` | `card__body` |
| `card-footer` | `card__footer` |
| `table table-hover` | `table table--hover` |
| `table table-striped` | `table table--striped` |
| `form-control` | `form-control` (same) |
| `form-select` | `form-select` (same) |
| `navbar` | `navbar` (same) |
| `text-muted` | `text-muted` (same) |
| `ms-2` | `ml-2` |
| `me-2` | `mr-2` |

## Step-by-Step Migration Examples

### Example 1: Converting a Button

**Before (Bootstrap):**
```html
<button class="btn btn-primary btn-lg">
  <i class="bi bi-download"></i> Download
</button>
```

**After (Polymorphic UI):**
```html
<button class="btn btn--primary btn--lg">
  <i class="bi bi-download"></i> Download
</button>
```

**Changes:**
- `btn-primary` → `btn--primary` (double dash for variants)
- `btn-lg` → `btn--lg`

### Example 2: Converting a Card

**Before (Bootstrap):**
```html
<div class="card border-primary">
  <div class="card-header bg-primary text-white">
    <h5>Card Title</h5>
  </div>
  <div class="card-body">
    <p>Card content</p>
  </div>
  <div class="card-footer">
    Footer content
  </div>
</div>
```

**After (Polymorphic UI):**
```html
<div class="card card--primary">
  <div class="card__header bg-primary text-white">
    <h5 class="card__title">Card Title</h5>
  </div>
  <div class="card__body">
    <p>Card content</p>
  </div>
  <div class="card__footer">
    Footer content
  </div>
</div>
```

**Changes:**
- `border-primary` → `card--primary` (semantic variant)
- `card-header` → `card__header` (BEM naming)
- `card-body` → `card__body`
- `card-footer` → `card__footer`
- Wrapped title in `card__title` for better semantics

### Example 3: Converting a Badge

**Before (Bootstrap):**
```html
<span class="badge bg-success">Succeeded</span>
<span class="badge bg-danger">Failed</span>
<span class="badge bg-warning text-dark">Running</span>
```

**After (Polymorphic UI):**
```html
<span class="badge badge--success">Succeeded</span>
<span class="badge badge--danger">Failed</span>
<span class="badge badge--warning">Running</span>
```

**Changes:**
- `bg-success` → `badge--success`
- `bg-danger` → `badge--danger`
- `bg-warning text-dark` → `badge--warning` (text color handled automatically)

### Example 4: Converting an Alert

**Before (Bootstrap):**
```html
<div class="alert alert-success alert-dismissible fade show" role="alert">
  <strong>Success!</strong> Your changes have been saved.
  <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
</div>
```

**After (Polymorphic UI):**
```html
<div class="alert alert--success animate-fade-in" role="alert">
  <div class="alert__title">Success!</div>
  <div>Your changes have been saved.</div>
  <button type="button" class="alert__close">
    <i class="bi bi-x-lg"></i>
  </button>
</div>
```

**Changes:**
- `alert-success` → `alert--success`
- `fade show` → `animate-fade-in` (built-in animation)
- `btn-close` → `alert__close` with icon
- Structured content with `alert__title`

### Example 5: Converting a Table

**Before (Bootstrap):**
```html
<div class="table-responsive">
  <table class="table table-hover table-striped">
    <thead>
      <tr>
        <th>Name</th>
        <th>Status</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>Item 1</td>
        <td><span class="badge bg-success">Active</span></td>
      </tr>
    </tbody>
  </table>
</div>
```

**After (Polymorphic UI):**
```html
<div class="table-container">
  <table class="table table--hover table--striped">
    <thead>
      <tr>
        <th>Name</th>
        <th>Status</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>Item 1</td>
        <td><span class="badge badge--success">Active</span></td>
      </tr>
    </tbody>
  </table>
</div>
```

**Changes:**
- `table-responsive` → `table-container`
- `table-hover` → `table--hover`
- `table-striped` → `table--striped`
- `bg-success` → `badge--success`

### Example 6: Converting Form Elements

**Before (Bootstrap):**
```html
<div class="mb-3">
  <label for="username" class="form-label">Username</label>
  <input type="text" class="form-control" id="username">
  <div class="form-text">Enter your username</div>
</div>
```

**After (Polymorphic UI):**
```html
<div class="form-group">
  <label for="username" class="form-label">Username</label>
  <input type="text" class="form-control" id="username">
  <div class="form-text">Enter your username</div>
</div>
```

**Changes:**
- `mb-3` → `form-group` (built-in margin)
- Form classes remain mostly the same for compatibility

### Example 7: Converting Grid Layout

**Before (Bootstrap):**
```html
<div class="row g-4">
  <div class="col-md-6 col-lg-3">
    <div class="card">...</div>
  </div>
  <div class="col-md-6 col-lg-3">
    <div class="card">...</div>
  </div>
</div>
```

**After (Polymorphic UI):**
```html
<div class="row g-4">
  <div class="col-md-6 col-lg-3">
    <div class="card">...</div>
  </div>
  <div class="col-md-6 col-lg-3">
    <div class="card">...</div>
  </div>
</div>
```

**Changes:**
- Grid system remains compatible (no changes needed)

### Example 8: Converting Navbar

**Before (Bootstrap):**
```html
<nav class="navbar navbar-expand-sm navbar-dark bg-primary">
  <div class="container-fluid">
    <a class="navbar-brand" href="/">Brand</a>
    <ul class="navbar-nav">
      <li class="nav-item">
        <a class="nav-link active" href="/dashboard">Dashboard</a>
      </li>
    </ul>
  </div>
</nav>
```

**After (Polymorphic UI):**
```html
<nav class="navbar navbar-dark">
  <div class="container-fluid">
    <a class="navbar__brand" href="/">Brand</a>
    <ul class="navbar__nav">
      <li>
        <a class="navbar__link navbar__link--active" href="/dashboard">Dashboard</a>
      </li>
    </ul>
  </div>
</nav>
```

**Changes:**
- `navbar-brand` → `navbar__brand`
- `navbar-nav` → `navbar__nav`
- `nav-link active` → `navbar__link navbar__link--active`
- Removed unnecessary `nav-item` wrapper

## Common Patterns

### Loading State on Cards

**Pattern:**
```html
<div class="card card--loading position-relative">
  <div class="spinner-overlay">
    <div class="spinner"></div>
  </div>
  <div class="card__body">
    Content loading...
  </div>
</div>
```

**JavaScript:**
```javascript
// Show loading
PolymorphicUI.Loading.show(cardElement);

// Hide loading
PolymorphicUI.Loading.hide(cardElement);
```

### Button with Loading State

**HTML:**
```html
<button class="btn btn--primary" id="submitBtn">
  Submit
</button>
```

**JavaScript:**
```javascript
const btn = document.getElementById('submitBtn');

// Start loading
PolymorphicUI.Loading.button(btn, true);

// Stop loading
PolymorphicUI.Loading.button(btn, false);
```

### Dynamic Alerts

**Instead of static HTML:**
```html
<div class="alert alert--success">
  Success message
</div>
```

**Use JavaScript:**
```javascript
PolymorphicUI.Alert.show({
  type: 'success',
  title: 'Success!',
  message: 'Your changes have been saved',
  dismissible: true,
  duration: 5000
});
```

### Toast Notifications

**Replace alerts with toasts for better UX:**
```javascript
PolymorphicUI.Toast.show({
  type: 'success',
  title: 'Saved',
  message: 'Changes saved successfully',
  duration: 3000
});
```

## Migration Checklist

- [ ] Replace all `btn-*` with `btn--*`
- [ ] Replace all `badge bg-*` with `badge badge--*`
- [ ] Replace all `alert-*` with `alert--*`
- [ ] Replace `card-header/body/footer` with `card__header/body/footer`
- [ ] Replace `table-*` with `table--*`
- [ ] Replace `table-responsive` with `table-container`
- [ ] Update navbar classes to BEM naming (`navbar__brand`, `navbar__link`)
- [ ] Replace inline styles with utility classes where possible
- [ ] Use design tokens (CSS variables) instead of hardcoded values
- [ ] Implement loading states using `PolymorphicUI.Loading`
- [ ] Replace static alerts with `PolymorphicUI.Alert` or `PolymorphicUI.Toast`
- [ ] Remove Bootstrap-specific JavaScript dependencies
- [ ] Test responsive behavior on different screen sizes
- [ ] Verify dark mode support (if enabled)

## Tips for Smooth Migration

1. **Start Small**: Migrate one component at a time
2. **Test Frequently**: Test each component after migration
3. **Use Browser DevTools**: Inspect elements to verify correct classes
4. **Leverage Search & Replace**: Use VS Code's find/replace with regex
5. **Keep Documentation Handy**: Reference the Polymorphic UI documentation
6. **Validate HTML**: Ensure semantic HTML structure
7. **Check Accessibility**: Test keyboard navigation and screen readers

## Common Pitfalls

1. **Forgetting the Double Dash**: `btn-primary` vs `btn--primary`
2. **Mixing Bootstrap and Polymorphic Classes**: Choose one system
3. **Hardcoded Colors**: Use design tokens instead
4. **Inline Styles**: Use utility classes when possible
5. **Missing BEM Modifiers**: Use `--` for variants, `__` for elements

## Need Help?

Refer to:
- `POLYMORPHIC-UI-DOCUMENTATION.md` - Full component reference
- `design-tokens.css` - Available CSS variables
- `components.css` - Component source code
- `utilities.css` - Available utility classes

---

**Happy Migrating! 🚀**
